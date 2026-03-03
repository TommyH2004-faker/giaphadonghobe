# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution + project files first for better layer caching
COPY GiaPhaDongHo.sln ./
COPY GiaPha_WebAPI/GiaPha_WebAPI.csproj GiaPha_WebAPI/
COPY GiaPha_Application/GiaPha_Application.csproj GiaPha_Application/
COPY GiaPha_Infrastructure/GiaPha_Infrastructure.csproj GiaPha_Infrastructure/
COPY GiaPha_Domain/GiaPha_Domain.csproj GiaPha_Domain/

RUN dotnet restore GiaPha_WebAPI/GiaPha_WebAPI.csproj

# Copy remaining source and publish
COPY . .
RUN dotnet publish GiaPha_WebAPI/GiaPha_WebAPI.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Create non-root user for security
RUN addgroup --system --gid 1001 appgroup && \
    adduser --system --uid 1001 --ingroup appgroup appuser

# Copy published app
COPY --from=build /app/publish .

# Change ownership to non-root user
RUN chown -R appuser:appgroup /app

# Switch to non-root user
USER appuser

# Environment variables (Render injects PORT at runtime)
ENV ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Expose port - Render typically uses 10000
EXPOSE 10000

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=15s --retries=3 \
    CMD curl -f http://localhost:${PORT:-10000}/health || exit 1

# Use shell form to evaluate PORT variable at runtime
CMD ASPNETCORE_URLS=http://+:${PORT:-10000} dotnet GiaPha_WebAPI.dll
