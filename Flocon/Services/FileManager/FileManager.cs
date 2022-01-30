using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Flocon.Services.FileManager
{
    // WARNING : As of now, file upload does not work with Brave!!!
    public class FileManager : IFileManager
    {
        // ToDo : Manage different folders
        private string _imageAvatarsPath;
        private string _docsSignsPath;

        public FileManager(IConfiguration config)
        {
            _imageAvatarsPath = config["StaticPaths:AvatarImgs"]; // The path to the photos of the profile pictures and company logos
            _docsSignsPath = config["StaticPaths:DocumentsSigns"]; // The path to the documents (original and signed) of the sign trails
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

        public FileStream DocumentStream(string doc)
        {
            try
            {
                var fs = new FileStream(Path.Combine(_docsSignsPath, doc), FileMode.Open, FileAccess.Read);
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
                var save_path = Path.Combine(_docsSignsPath);
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

        public async Task<string> SaveDocument(IFormFile doc)
        {
            try
            {
                var save_path = Path.Combine(_docsSignsPath);
                if (!Directory.Exists(save_path))
                {
                    Directory.CreateDirectory(save_path);
                }

                var mime = doc.FileName.Substring(doc.FileName.LastIndexOf('.'));
                var fileName = $"doc_{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss-FF")}{mime}";

                using (var fileStream = new FileStream(Path.Combine(save_path, fileName), FileMode.Create))
                {
                    await doc.CopyToAsync(fileStream);
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
