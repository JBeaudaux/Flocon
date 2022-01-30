using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Flocon.Services.FileManager
{
    public interface IFileManager
    {
        FileStream ImageStream(string image);
        FileStream DocumentStream(string doc);
        Task<string> SaveImage(IFormFile image);
        Task<string> SaveDocument(IFormFile doc);
    }
}
