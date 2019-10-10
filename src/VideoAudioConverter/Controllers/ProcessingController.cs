using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using VideoAudioConverter.Models;

namespace VideoAudioConverter.Controllers
{

    [Route("api/v1")]
    public class ProcessingController : Controller
    {
        private IHostEnvironment _hostingEnvironment;

        public ProcessingController(IHostEnvironment hostingEnvironment)
        {
            this._hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        [Route("fileupload")]
        public async Task<IActionResult> Post([FromForm]List<IFormFile> files)
        {
            try
            {
                var today = DateTime.Today;
                var uploadPath = @"C:\web\files\audio"; // Path.Combine(_hostingEnvironment.WebRootPath, "Content", "Files");
                uploadPath += $"\\{today.Year}\\{today.Month}".PadLeft(2, '0');
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                const int fileMaxLength = 1024 * 1024* 10;
                var fileUrls = new List<string>();
                foreach (var file in files)
                {
                    var fileExtension = Path.GetExtension(file.FileName);

                    if (file.Length > 0 && file.Length <= fileMaxLength && ( fileExtension.EndsWith("wma") || fileExtension.EndsWith("wav")))
                    {                        
                        
                        using (var stream =  new FileStream(Path.Combine(uploadPath, file.FileName), FileMode.Create, FileAccess.Write))
                        {
                            await file.CopyToAsync(stream);                             
                            fileUrls.Add(file.FileName);
                        }
                    }
                }

                var fileItemResponse = new
                {
                    Data = fileUrls,
                    Code = 200,
                    Message = "Ok "  
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

        [HttpPost]
        [Route("convert_wma_mp3")]
        public async Task<IActionResult> ConvertAudio(FileModel model)
        {
            try
            {
                var today = DateTime.Today;
                var uploadPath = @"C:\web\files\audio"; // Path.Combine(_hostingEnvironment.WebRootPath, "Content", "Files");
                uploadPath += $"\\{today.Year}\\{today.Month}".PadLeft(2, '0');
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                var fileToConvert = Path.Combine(uploadPath, model.FileName);
                
                var fileExtension = Path.GetExtension(model.FileName);
                if (!(fileExtension.EndsWith("wma") || fileExtension.EndsWith("wav")))
                {
                    var fileItemResponse = new
                    {
                        Message = "File không đúng định dạng",
                        Code = -1
                    };
                    return Ok(fileItemResponse);
                }
                if (System.IO.File.Exists(fileToConvert))
                    {


                    var fileName = Path.GetFileNameWithoutExtension(model.FileName)+".mp3";
                    var fileToConvertOutput = Path.Combine(uploadPath, fileName);
                    var command = $"-y -i {fileToConvert} {fileToConvertOutput}";
                    Process proc = new Process();
                    proc.StartInfo.FileName = @"C:\ffmpeg\bin\ffmpeg.exe";
                    proc.StartInfo.Arguments = command;
                    proc.Start();

                    var data = new FileModel
                    {
                        FileName = fileName,
                        FileUrl = "https://files.tradingproedu.com/audio" + $"/{today.Year}/{today.Month}".PadLeft(2, '0') + "/"+fileName,
                    };

                    var fileItemResponse = new
                    {
                        Data = data,
                        Code = 200,
                        Message = "Ok "
                    };
                    return Ok(fileItemResponse);
                }
                else {
                    var fileItemResponse = new
                    {                      
                        Code = 404,
                        Message = "File not found"
                    };
                    return Ok(fileItemResponse);
                }
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