namespace GiaPha_Application.Service;

public interface ICloudinaryService
{
    /// <summary>
    /// Upload ảnh lên Cloudinary và trả về URL public
    /// </summary>
    /// <param name="file">Stream của file ảnh</param>
    /// <param name="fileName">Tên file (không bao gồm extension)</param>
    /// <param name="folder">Thư mục trên Cloudinary (VD: "avatars")</param>
    /// <returns>URL public của ảnh trên Cloudinary</returns>
    Task<string> UploadImageAsync(Stream file, string fileName, string folder);

    /// <summary>
    /// Xóa ảnh trên Cloudinary
    /// </summary>
    /// <param name="publicId">Public ID của ảnh (VD: "avatars/user123")</param>
    Task<bool> DeleteImageAsync(string publicId);
}
