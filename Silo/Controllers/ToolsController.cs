using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Silo.Controllers;

public partial class ToolsController(IWebHostEnvironment Environment) : Controller
{
    public async Task<IActionResult> UploadProductImage(IFormFile file)
    {
        string directory = $"{Environment.WebRootPath}\\images\\Products";

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string fileName = string.Concat(Guid.NewGuid().ToString().AsSpan(0, 6), ".png");

        string path = $"{directory}\\{fileName}";

        using FileStream stream = System.IO.File.Create(path);

        await file.CopyToAsync(stream);

        return Json(fileName);
    }

    public async Task<IActionResult> UploadUserImage(IFormFile file)
    {
        string directory = $"{Environment.WebRootPath}\\images\\Users";

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string fileName = string.Concat(Guid.NewGuid().ToString().AsSpan(0, 6), ".png");

        string path = $"{directory}\\{fileName}";

        using FileStream stream = System.IO.File.Create(path);

        await file.CopyToAsync(stream);

        return Json(fileName);
    }

    [HttpPost]
    public async Task<IActionResult> UploadAccountingFile(IFormFile file)
    {
        string directory = Environment.WebRootPath + "\\temp";

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string extension = file.FileName.Split('.')[1];

        string path = $"{directory}\\{Guid.NewGuid().ToString().Substring(0, 5)}.{extension}";

        using FileStream stream = System.IO.File.Create(path);

        await file.CopyToAsync(stream);

        return Json(path);
    }

    [HttpPost]
    public async Task<IActionResult> UploadDynamicExcel(IFormFile file)
    {
        string directory = Environment.WebRootPath + "\\temp";

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

}
