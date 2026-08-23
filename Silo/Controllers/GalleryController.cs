using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Silo.Controllers;
public partial class GalleryController(IWebHostEnvironment Environment) : Controller
{

    [HttpPost]
    public async Task<IActionResult> UploadGallery(IFormFile file)
    {
        string directory = Environment.WebRootPath + "\\gallery";

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string extension = file.FileName.Split('.')[1];

        string path = $"{directory}\\{Guid.NewGuid().ToString().Substring(0, 6)}.{extension}";

        using FileStream stream = System.IO.File.Create(path);

        await file.CopyToAsync(stream);

        return Json(JsonConvert.SerializeObject(new
        {
            Path = path,
            FileName = file.FileName
        }));
    }

    [HttpGet]
    public async Task<IActionResult> ShowMedia(string media)
    {
        string directory = $"{Environment.WebRootPath}\\gallery\\{media}";

        string extension = media.Split(".")[1];

        string contentType = extension switch
        {
            "png" => "image/png",
            "jpg" or "jpeg" => "image/jpeg",
            "pdf" => "application/pdf",
            _ => ""
        };

        return File(System.IO.File.ReadAllBytes(directory), contentType);
    }
}
