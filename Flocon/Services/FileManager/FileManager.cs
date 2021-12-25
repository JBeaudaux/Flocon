using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Flocon.Services.FileManager
{
    public class FileManager : IFileManager
    {
        // ToDo : Manage different folders
        private string _imageAvatarsPath;

        public FileManager(IConfiguration config)
        {
            _imageAvatarsPath = config["Path:AvatarImgs"]; // The path to the photos of the profile pictures (for User model) 
        }

        public FileStream ImageStream(string image)
        {
            try
            {
                var fs = new FileStream(Path.Combine(_imageAvatarsPath, image), FileMode.Open, FileAccess.Read);
                return fs;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<string> SaveImage(IFormFile image)
        {
            try 
            {
                var save_path = Path.Combine(_imageAvatarsPath);
                if (!Directory.Exists(save_path))
                {
                    Directory.CreateDirectory(save_path);
                }

                var mime = image.FileName.Substring(image.FileName.LastIndexOf('.'));
                var fileName = $"img_{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss-FF")}{mime}";

                using (var fileStream = new FileStream(Path.Combine(save_path, fileName), FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                return fileName;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return $"FileManager Error : {e.Message}";
            }
            
        }
    }
}
