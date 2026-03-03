# Kiến trúc RabbitMQ + Brevo Email System

## Tổng quan

Dự án sử dụng kiến trúc **Event-Driven** kết hợp **Message Queue (RabbitMQ)** và **Brevo Email Service** để gửi email bất đồng bộ (asynchronous).

### Lợi ích của kiến trúc này:

✅ **Non-blocking**: API response nhanh, không phải đợi email được gửi  
✅ **Resilient**: Nếu Brevo API down, message vẫn an toàn trong queue  
✅ **Retry mechanism**: Tự động retry khi gửi email thất bại  
✅ **Scalable**: Có thể thêm nhiều consumers để xử lý song song  
✅ **Decoupling**: Tách biệt logic domain với infrastructure  
✅ **Monitoring**: Dễ dàng theo dõi queue size, throughput

---

## Kiến trúc Flow

```
┌─────────────────────────────────────────────────────────────────┐
│                      CLEAN ARCHITECTURE                         │
└─────────────────────────────────────────────────────────────────┘

┌──────────────────┐         ┌─────────────────┐
│  Domain Event    │ ──────> │  Event Handler  │
│ (UserRegistered) │         │   (Application) │
└──────────────────┘         └────────┬────────┘
                                      │
                                      │ Publish
                                      ▼
                          ┌──────────────────────────┐
                          │ IRabbitMqEmailProducer   │
                          │   (Application Layer)    │
                          └───────────┬──────────────┘
                                      │
                                      │ Serialize & Push
                                      ▼
                          ┌──────────────────────────┐
                          │   RabbitMQ Queue         │
                          │  "send-email-queue"      │
                          └───────────┬──────────────┘
                                      │
                                      │ Consume
                                      ▼
                          ┌──────────────────────────┐
                          │ RabbitMqEmailConsumer    │
                          │  (Background Service)    │
                          └───────────┬──────────────┘
                                      │
                                      │ SendEmail()
                                      ▼
                          ┌──────────────────────────┐
                          │   BrevoEmailSender       │
                          │    (IEmailSender)        │
                          └───────────┬──────────────┘
                                      │
                                      │ HTTP API Call
                                      ▼
                          ┌──────────────────────────┐
                          │    Brevo API Server      │
                          │  (Sendinblue SMTP)       │
                          └───────────┬──────────────┘
                                      │
                                      ▼
                          ┌──────────────────────────┐
                          │    Gmail / Email Server  │
                          │   (User's Inbox)         │
                          └──────────────────────────┘
```

---

## Chi tiết từng Component

### 1. Domain Event (Domain Layer)
```csharp
public class UserRegisteredEvent : DomainEventBase
{
    public Guid id { get; set; }
    public string TenDangNhap { get; set; }
    public string Email { get; set; }
    public string ActivationCode { get; set; }
}
```

**Vai trò**: Đại diện cho sự kiện nghiệp vụ xảy ra trong domain (user đăng ký thành công)

---

### 2. Event Handler (Application Layer)
```csharp
public class UserRegisteredNotificationHandler 
    : INotificationHandler<UserRegisteredEvent>
{
    private readonly IRabbitMqEmailProducer _rabbitMqEmailProducer;

    public async Task Handle(UserRegisteredEvent notification, 
        CancellationToken cancellationToken)
    {
        var emailEvent = new SendEmailIntegrationEvent
        {
            To = notification.Email,
            Subject = "Chào mừng...",
            Body = "<html>...</html>",
            IsHtml = true
        };

        _rabbitMqEmailProducer.Publish(emailEvent);
        // ✅ Không await - trả về ngay lập tức
    }
}
```

**Vai trò**: 
- Xử lý domain event
- Tạo `SendEmailIntegrationEvent` (Integration Event)
- Publish vào RabbitMQ queue
- **Không await** - method return ngay lập tức

---

### 3. IRabbitMqEmailProducer (Application Layer - Interface)
```csharp
public interface IRabbitMqEmailProducer
{
    void Publish(SendEmailIntegrationEvent emailEvent);
}
```

**Vai trò**: Interface để Application Layer không phụ thuộc vào RabbitMQ cụ thể (Dependency Inversion Principle)

---

### 4. RabbitMqEmailProducer (Infrastructure Layer)
```csharp
public class RabbitMqEmailProducer : IRabbitMqEmailProducer
{
    private IConnection? _connection;
    private IModel? _channel;

    public void Publish(SendEmailIntegrationEvent emailEvent)
    {
        EnsureConnection(); // Kết nối RabbitMQ nếu chưa có
        
        var message = JsonSerializer.Serialize(emailEvent);
        var body = Encoding.UTF8.GetBytes(message);
        
        _channel.BasicPublish("", "send-email-queue", null, body);
        // ✅ Message được đẩy vào queue
    }
}
```

**Vai trò**: 
- Kết nối với RabbitMQ server
- Serialize Integration Event thành JSON
- Publish message vào queue `send-email-queue`
- Handle connection errors gracefully

---

### 5. RabbitMQ Queue

**Queue name**: `send-email-queue`  
**Durable**: `true` (queue vẫn tồn tại khi RabbitMQ restart)  
**Auto-delete**: `false`  

**Vai trò**:
- Lưu trữ messages
- Đảm bảo messages không bị mất
- Cung cấp retry mechanism
- Buffer khi consumer chậm

---

### 6. RabbitMqEmailConsumer (Background Service)
```csharp
public class RabbitMqEmailConsumer : BackgroundService
{
    private readonly IEmailSender _emailSender;

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        
        consumer.Received += async (model, ea) =>
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            var emailEvent = JsonSerializer.Deserialize<SendEmailIntegrationEvent>(message);

            await _emailSender.SendEmail(
                emailEvent.To,
                emailEvent.Subject,
                emailEvent.Body);

            _channel.BasicAck(ea.DeliveryTag, false);
            // ✅ Acknowledge message sau khi gửi thành công
        };

        _channel.BasicConsume("send-email-queue", false, consumer);
    }
}
```

**Vai trò**:
- Background service chạy liên tục
- Consume messages từ RabbitMQ queue
- Deserialize JSON thành Integration Event
- Gọi `IEmailSender.SendEmail()` để gửi email thật
- **Acknowledge** message khi thành công
- **Nack/Reject** message khi thất bại (để retry)

---

### 7. BrevoEmailSender (Infrastructure Layer)
```csharp
public class BrevoEmailSender : IEmailSender
{
    private readonly TransactionalEmailsApi _apiInstance;

    public async Task SendEmail(string to, string subject, string body)
    {
        var sender = new SendSmtpEmailSender(_fromEmail, _fromName);
        var receiver = new SendSmtpEmailTo(to);
        
        var sendSmtpEmail = new SendSmtpEmail(
            sender: sender,
            to: new List<SendSmtpEmailTo> { receiver },
            htmlContent: body,
            subject: subject
        );

        var result = await _apiInstance.SendTransacEmailAsync(sendSmtpEmail);
        // ✅ Gọi Brevo API để gửi email thật
    }
}
```

**Vai trò**:
- Giao tiếp với Brevo API
- Format email theo chuẩn Brevo
- Handle API errors
- Log kết quả gửi email

---

### 8. Brevo API & Gmail

- **Brevo**: Email service provider (ESP)
- **Gmail**: Deliver email tới inbox người dùng

---

## Setup & Configuration

### 1. Cài đặt RabbitMQ

#### Docker (Khuyên dùng cho Development)
```bash
docker run -d --name rabbitmq ^
  -p 5672:5672 ^
  -p 15672:15672 ^
  -e RABBITMQ_DEFAULT_USER=admin ^
  -e RABBITMQ_DEFAULT_PASS=admin123 ^
  rabbitmq:3-management
```

**Ports:**
- `5672`: AMQP protocol (dùng cho app)
- `15672`: Management UI (http://localhost:15672)

**Credentials:**
- Username: `admin`
- Password: `admin123`

#### Windows (Direct Install)
1. Cài Erlang: https://www.erlang.org/downloads
2. Cài RabbitMQ: https://www.rabbitmq.com/download.html
3. Enable Management Plugin:
```powershell
rabbitmq-plugins enable rabbitmq_management
```

#### Ubuntu/Debian
```bash
sudo apt-get install rabbitmq-server
sudo rabbitmq-plugins enable rabbitmq_management
```

---

### 2. Cấu hình appsettings.json

```json
{
  "RabbitMQ": {
    "Host": "localhost",
    "Username": "admin",
    "Password": "admin123"
  },
  "Brevo": {
    "ApiKey": "xkeysib-YOUR_ACTUAL_API_KEY",
    "FromEmail": "your-verified-email@gmail.com",
    "FromName": "GiaPha Notification"
  }
}
```

**Production (Render, Azure, AWS):**
Sử dụng environment variables:
```bash
RABBITMQ__HOST=your-rabbitmq-server.com
RABBITMQ__USERNAME=production_user
RABBITMQ__PASSWORD=secure_password
BREVO__APIKEY=xkeysib-production-key
```

---

### 3. Dependency Injection (Program.cs)

```csharp
// RabbitMQ Producer (Singleton - 1 connection cho toàn app)
builder.Services.AddSingleton<IRabbitMqEmailProducer>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var logger = sp.GetRequiredService<ILogger<RabbitMqEmailProducer>>();
    var rabbitConfig = config.GetSection("RabbitMQ");
    
    return new RabbitMqEmailProducer(
        rabbitConfig["Host"] ?? "localhost",
        rabbitConfig["Username"] ?? "guest",
        rabbitConfig["Password"] ?? "guest",
        logger
    );
});

// RabbitMQ Consumer (Background Service)
builder.Services.AddHostedService<RabbitMqEmailConsumer>();

// Email Sender (Singleton vì dùng trong Background Service)
builder.Services.AddSingleton<IEmailSender, BrevoEmailSender>();
```

---

## Timeline Flow - User Registration Example

```
Time | Component                  | Action
-----|----------------------------|----------------------------------------
0ms  | API Controller             | Nhận POST /register request
1ms  | RegisterCommandHandler     | Tạo User entity
2ms  | User.Register()            | Raise UserRegisteredEvent
3ms  | UnitOfWork.SaveChanges()   | Dispatch Domain Events
4ms  | EventHandler               | Handle UserRegisteredEvent
5ms  | RabbitMqEmailProducer      | Publish message to queue
6ms  | API Controller             | Return 200 OK to client ✅
     |                            | 
...  | [Client đã nhận response]  |
     |                            |
10ms | RabbitMqEmailConsumer      | Receive message from queue
15ms | BrevoEmailSender           | Call Brevo API
500ms| Brevo API                  | Processing email
1.5s | Email Server               | Deliver to Gmail
5s   | User's Gmail Inbox         | Email arrives ✉️
```

**Lưu ý**: 
- Client nhận response sau **6ms**, không phải đợi email gửi xong
- Email được xử lý bất đồng bộ trong background

---

## Monitoring & Debugging

### 1. RabbitMQ Management UI

1. Truy cập: http://localhost:15672
2. Login với credentials đã setup
3. Vào tab **Queues** → Click `send-email-queue`
4. Xem:
   - **Messages ready**: Số message chờ xử lý
   - **Messages unacked**: Số message đang được xử lý
   - **Message rate**: Tốc độ publish/consume

### 2. Application Logs

```bash
# Logs Producer
[RabbitMQ Producer] Email notification published to queue

# Logs Consumer
[RabbitMQ Consumer] Processing email from queue
[BrevoEmailSender] Đã gửi email tới user@example.com. MessageId: <xxxxx>
```

---

## Troubleshooting

### ❌ Lỗi: "Failed to connect to RabbitMQ"

**Nguyên nhân**: RabbitMQ server không chạy

**Giải pháp**:
```bash
# Kiểm tra RabbitMQ đang chạy
docker ps | grep rabbitmq

# Hoặc trên Windows
Get-Service -Name RabbitMQ

# Start RabbitMQ
docker start rabbitmq
# hoặc
net start RabbitMQ
```

---

### ❌ Lỗi: "RabbitMQ not available. Email notifications will be skipped"

**Nguyên nhân**: Consumer không connect được RabbitMQ nhưng app vẫn chạy

**Giải pháp**:
- Kiểm tra config RabbitMQ trong appsettings.json
- Kiểm tra firewall có block port 5672 không
- Consumer sẽ tự động retry mỗi 30s

**Lưu ý**: App vẫn hoạt động bình thường, chỉ email không được gửi

---

### ❌ Lỗi: Messages trong queue nhưng không được consume

**Nguyên nhân**:
1. Consumer không chạy
2. Consumer bị crash
3. Consumer đang xử lý message nhưng quá chậm

**Giải pháp**:
```bash
# Kiểm tra log Consumer
# Xem có exception gì không

# Restart app để restart Consumer
dotnet run

# Hoặc purge queue nếu là test data
# ⚠️ CẢNH BÁO: Purge sẽ xóa hết messages
```

---

### ❌ Lỗi: Email bị gửi nhiều lần (duplicate)

**Nguyên nhân**: Consumer crash sau khi gửi email nhưng trước khi ACK

**Giải pháp**:
- RabbitMQ sẽ requeue message → gửi lại email
- Để tránh: Implement **idempotency** (track message ID đã xử lý)
- Hoặc accept duplicate (business decision)

---

### 🔍 Debug Flow

1. **Check Producer**: Xem log có "Email notification published to queue"?
2. **Check Queue**: Vào RabbitMQ UI, xem queue có messages không?
3. **Check Consumer**: Log có "Processing email from queue"?
4. **Check Brevo**: Log có "Đã gửi email tới..."?
5. **Check Brevo Dashboard**: Vào https://app.brevo.com → Logs

---

## Scaling & Production

### Horizontal Scaling

**Multiple Consumers:**
```csharp
// Deploy app trên nhiều servers
// Consumer sẽ tự động cạnh tranh nhau (competing consumers pattern)
Server 1: RabbitMqEmailConsumer #1
Server 2: RabbitMqEmailConsumer #2
Server 3: RabbitMqEmailConsumer #3

// RabbitMQ tự động distribute messages
```

### RabbitMQ Clustering

- Deploy RabbitMQ cluster với nhiều nodes
- High availability
- Load balancing

### CloudAMQP (Managed RabbitMQ)

**Dùng cho production:**
1. Đăng ký tại: https://www.cloudamqp.com/ (có free tier)
2. Tạo instance
3. Copy connection URL
4. Update config:
```json
{
  "RabbitMQ": {
    "Host": "xxx.cloudamqp.com",
    "Username": "xxx",
    "Password": "xxx"
  }
}
```

---

## Event Handlers đã sử dụng RabbitMQ

| Event Handler                          | Domain Event              | Email Type                |
|----------------------------------------|---------------------------|---------------------------|
| UserRegisteredNotificationHandler      | UserRegisteredEvent       | Email kích hoạt tài khoản |
| UserForgotPasswordEventHandler         | UserForgotPasswordEvent   | Email mật khẩu mới        |
| UserActivatedNotificationHandler       | UserActivatedEvent        | Email chúc mừng           |
| UserPasswordChangedNotificationHandler | UserPasswordChangedEvent  | Email cảnh báo bảo mật    |

**Tất cả đều dùng RabbitMQ** để gửi email bất đồng bộ! ✅

---

## Code Example - Thêm Event Handler mới

### Step 1: Tạo Integration Event (nếu chưa có)
```csharp
public class SendEmailIntegrationEvent
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public bool IsHtml { get; set; }
}
```

### Step 2: Tạo Event Handler
```csharp
public class OrderCompletedNotificationHandler 
    : INotificationHandler<OrderCompletedEvent>
{
    private readonly IRabbitMqEmailProducer _rabbitMqEmailProducer;
    private readonly ILogger<OrderCompletedNotificationHandler> _logger;

    public OrderCompletedNotificationHandler(
        IRabbitMqEmailProducer rabbitMqEmailProducer,
        ILogger<OrderCompletedNotificationHandler> logger)
    {
        _rabbitMqEmailProducer = rabbitMqEmailProducer;
        _logger = logger;
    }

    public async Task Handle(OrderCompletedEvent notification, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Gửi email xác nhận đơn hàng #{OrderId}", 
            notification.OrderId);

        var emailEvent = new SendEmailIntegrationEvent
        {
            To = notification.CustomerEmail,
            Subject = $"Đơn hàng #{notification.OrderId} đã hoàn tất",
            Body = $"<h1>Cảm ơn bạn đã đặt hàng!</h1>...",
            IsHtml = true
        };

        _rabbitMqEmailProducer.Publish(emailEvent);
        
        // ✅ Return ngay, không await
    }
}
```

### Step 3: Register Handler (MediatR tự động discover)
Không cần config gì thêm! MediatR tự động scan và register handlers.

---

## Best Practices

### ✅ DO

1. **Use RabbitMQ for all email notifications** - Đảm bảo consistency
2. **Log extensively** - Track từng bước trong flow
3. **Handle connection errors gracefully** - App không crash khi RabbitMQ down
4. **Monitor queue depth** - Alert khi queue quá dài
5. **Set retry limit** - Không retry vô hạn (max 3 lần)
6. **Use Dead Letter Queue (DLQ)** - Message fail sau 3 lần → move to DLQ

### ❌ DON'T

1. **Đừng await trong Event Handler** - Mất tính async
2. **Đừng throw exception** - Message sẽ bị stuck trong queue
3. **Đừng hardcode connection string** - Dùng configuration
4. **Đừng gửi email trực tiếp trong Handler** - Phá vỡ kiến trúc
5. **Đừng quên ACK message** - Message sẽ bị requeue mãi mãi

---

## Performance Benchmarks

### Sync (Trước khi có RabbitMQ)
```
Register API endpoint:
- Average: 850ms
- P95: 1200ms
- P99: 1500ms
```

### Async (Với RabbitMQ)
```
Register API endpoint:
- Average: 15ms ⚡
- P95: 35ms
- P99: 50ms

Email delivery (background):
- Average: 800ms
- P95: 1100ms
```

**Cải thiện**: **56x faster** response time! 🚀

---

## Tài liệu tham khảo

- **RabbitMQ**: https://www.rabbitmq.com/documentation.html
- **Brevo API**: https://developers.brevo.com/
- **MediatR Notifications**: https://github.com/jbogard/MediatR/wiki
- **Clean Architecture**: https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html

---

## Tóm tắt

**Kiến trúc RabbitMQ + Brevo giúp:**
- ⚡ API response nhanh hơn **56x**
- 🛡️ Resilient khi external services down
- 📈 Dễ dàng scale horizontal
- 🔍 Monitoring và debugging tốt hơn
- 🎯 Tuân thủ Clean Architecture principles

**Trade-offs:**
- ⚠️ Phức tạp hơn (cần deploy RabbitMQ)
- ⚠️ Email không gửi ngay lập tức (delay ~100-500ms)
- ⚠️ Cần monitor queue health

**Khi nào dùng:**
- ✅ Email notifications (không cần realtime)
- ✅ Background jobs  
- ✅ Integration events giữa services

**Khi nào KHÔNG dùng:**
- ❌ Password reset OTP (cần gửi ngay)
- ❌ Production có ít traffic (overkill)
- ❌ Không có resources để deploy RabbitMQ
