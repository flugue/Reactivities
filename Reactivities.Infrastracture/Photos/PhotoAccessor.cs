using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Reactivities.Application.Interfaces;
using Reactivities.Application.Photos;
using System;
using System.Threading.Tasks;

namespace Reactivities.Infrastracture.Photos
{
    public class PhotoAccessor : IPhotoAccessor
    {
        public PhotoAccessor(IOptions<CloudinarySettings> config)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
                );
            Cloudinary = new Cloudinary(account);
        }

        private Cloudinary Cloudinary { get; set; }


        public async Task<PhotoUploadResult> AddPhoto(IFormFile file)
        {
            if(file.Length > 0)
            {
                await using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill")
                };
                var uploadResult = await Cloudinary.UploadAsync(uploadParams);

                if(uploadResult.Error != null)
                {
                    throw new Exception(uploadResult.Error.Message);
                }

                return new PhotoUploadResult
                {
                    PublidId = uploadResult.PublicId,
                    Url = uploadResult.SecureUrl.ToString(),
                };
            }
            return null;
        }

        public async Task<string> DeletePhoto(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);    
            var result = await Cloudinary.DestroyAsync(deleteParams);
            return result.Result == "ok" ? result.Result : null;
        }
    }
}
