using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace VideoAudioConverter.Controllers
{

    [Route("api/v1/fileupload")]
    public class FileUploadController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;

        public FileUploadController(IHostingEnvironment hostingEnvironment)
        {
            this._hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        [Route("")]
        // [MimeMultipart]
        public async Task<IActionResult> Post([FromForm]List<IFormFile> files)
        {
            try
            {
                var today = DateTime.Today;
                var uploadPath = @"C:\web\files\files"; // Path.Combine(_hostingEnvironment.WebRootPath, "Content", "Files");
                uploadPath += $"\\{today.Year}\\{today.Month}".PadLeft(2, '0');
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                // long size = files.Sum(f => f.Length);

                // full path to file in temp location

                var fileUrls = new List<string>();
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var fileExtension = Path.GetExtension(file.FileName);
                        var filePath = Path.GetTempFileName();
                        using (var stream = new FileStream(Path.Combine(uploadPath, filePath), FileMode.Create, FileAccess.Write))
                        {
                            await file.CopyToAsync(stream);
                            var fileUrl = $"files/{today.Year}/{today.Month}".PadLeft(2, '0') + "/" + file.FileName;
                            fileUrls.Add(fileUrl);
                        }

                    }
                }

                var fileItemResponse = new
                {
                    Data = fileUrls,
                    Code = 200,
                    Message = "Ok " + files.Count
                };
                return Ok(fileItemResponse);
            }
            catch (Exception ex)
            {
                var fileItemResponse = new
                {
                    Message = ex.Message,
                    Code = -1
                };
                return Ok(fileItemResponse);
            }
        }


    }
}