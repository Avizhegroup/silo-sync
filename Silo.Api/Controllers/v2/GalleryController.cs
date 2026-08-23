using MediatR;
using Microsoft.AspNetCore.Mvc;
using Silo.Application.Features;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v2;
public class GalleryController(ILogger<GalleryController> logger, IMediator mediator) : SiloBaseControllerVersion2(logger)
{
    [HttpPost("[action]")]
    public async Task<IActionResult> GetAllGalleryImages()
    {
        var response = await mediator.Send(new GetAllGalleryImagesQuery());

        return File(response.ZipFile, "application/zip", "GalleryImages.zip");
    }

    [HttpPost("[action]")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> SaveGalleryMediaWithFile([FromForm] SaveGalleryMediaWithFileCommand command)
    => Ok(new ApiResponse<SaveGalleryMediaWithFileVm>()
    {
        Successful = true,
        Value = await mediator.Send<SaveGalleryMediaWithFileVm>(command)
    });


    [HttpPost("[action]")]
    public async Task<IActionResult> GetGalleryImageFile(GetGalleryImageFileQuery request)
    {
        var response = await mediator.Send<GetGalleryImageFileVm>(request);

        if(response.ImageFile == null)
        {
            return NotFound();
        }

        return File(response.ImageFile, "image/jpeg", "GalleryImage.jpg");
    }


}
