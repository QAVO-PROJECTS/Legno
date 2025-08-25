using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Persistence.Concreters.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Fayl mövcud deyil və ya boşdur.");

            await using var stream = file.OpenReadStream();

            UploadResult uploadResult;

            try
            {
                if (file.ContentType.StartsWith("image"))
                {
                    var imageParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        PublicId = Guid.NewGuid().ToString(),
                        Folder = "legno_uploads"
                    };

                    uploadResult = await _cloudinary.UploadAsync(imageParams);
                }
                else
                {
                    var rawParams = new RawUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        PublicId = Guid.NewGuid().ToString(),
                        Folder = "legno_uploads"
                    };

                    uploadResult = await _cloudinary.UploadAsync(rawParams);
                }

                if (uploadResult.StatusCode == HttpStatusCode.OK)
                    return uploadResult.SecureUrl.ToString();

                // Burada detallı error ver:
                var error = uploadResult.Error?.Message ?? "naməlum xəta";
                throw new Exception($"Cloudinary-ə yükləmə alınmadı: {error}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Cloudinary Upload Exception: {ex.Message}", ex);
            }
        }
        public async Task DeleteFileAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Fayl adı təyin edilməyib.");

            try
            {
                var publicId = System.IO.Path.GetFileNameWithoutExtension(fileName); // Fayl adından Public ID-ni əldə edirik
                var deleteParams = new DeletionParams($"legno_uploads/{publicId}");
                var deleteResult = await _cloudinary.DestroyAsync(deleteParams);

                if (deleteResult.StatusCode != System.Net.HttpStatusCode.OK)
                    throw new Exception($"Cloudinary-dən fayl silinə bilmədi: {deleteResult.Error?.Message ?? "naməlum xəta"}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Cloudinary Delete Exception: {ex.Message}", ex);
            }
        }


    }
}
