using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Silo.Application.Contracts;

namespace Silo.Application.Api.Features;
public class SaveGalleryMediaWithFileHandler(WmsApiContext context
    , IConfiguration configuration
    , IHttpContextAccessor httpContextAccessor
    , IMapper mapper
    , IDataAccess dataAccess) : IRequestHandler<SaveGalleryMediaWithFileCommand, SaveGalleryMediaWithFileVm>
{
    public async Task<SaveGalleryMediaWithFileVm> Handle(SaveGalleryMediaWithFileCommand request, CancellationToken cancellationToken)
    {
        if (request.File == null || request.File.Length == 0)
            return new SaveGalleryMediaWithFileVm();

        using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var filePath = await SaveFileAsync(request.File);

            var galleryMedia = await CreateAndSaveGalleryMediaAsync(filePath, cancellationToken);

            if (await HandleGalleryProductImageAsync(galleryMedia, cancellationToken))
            {
                await transaction.CommitAsync(cancellationToken);

                return mapper.Map<SaveGalleryMediaWithFileVm>(galleryMedia);
            }

            await transaction.RollbackAsync(cancellationToken);

            return new SaveGalleryMediaWithFileVm();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);

            return new SaveGalleryMediaWithFileVm();
        }

        async Task<string> SaveFileAsync(IFormFile file)
        {
            var saveDirectoryPath = GetFileSavePath();

            if (!Directory.Exists(saveDirectoryPath))
                Directory.CreateDirectory(saveDirectoryPath);

            var filePath = Path.Combine(saveDirectoryPath, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }

        async Task<Domains.Entities.Gallery> CreateAndSaveGalleryMediaAsync(
            string filePath,
            CancellationToken cancellationToken)
        {
            var galleryMedia = mapper.Map<Domains.Entities.Gallery>(request);

            galleryMedia.UpldoadDateTime = DateTime.Now;

            galleryMedia.MediaName = request.File.FileName;

            galleryMedia.MediaPath = filePath;

            GetAdditionalData(galleryMedia);

            if (galleryMedia.UserId.HasValue())
            {
                galleryMedia.UserId = httpContextAccessor.HttpContext.User.GetUserId();
            }

            context.GalleryMedias.Add(galleryMedia);

            await context.SaveChangesAsync(cancellationToken);

            return galleryMedia;
        }

        async Task<bool> HandleGalleryProductImageAsync(
            Domains.Entities.Gallery galleryMedia,
            CancellationToken cancellationToken)
        {
            if (request.UsageType.NotEquals(GalleryUsageType.Product))
                return true;

            var product = await context.Products
                .FirstOrDefaultAsync(p => p.Code == request.UsageId, cancellationToken);

            if (product == null)
                return false;

            if (product.ProductGalleryId.NotEquals(0))
            {
                await RemoveExistingGalleryMediaAsync(product.ProductGalleryId, cancellationToken);
            }

            product.ProductGalleryId = galleryMedia.Id;
            context.Products.Update(product);

            return await context.SaveChangesAsync(cancellationToken) > 0;
        }

        async Task RemoveExistingGalleryMediaAsync(int galleryId, CancellationToken cancellationToken)
        {
            var existingMedia = await context.GalleryMedias
                .FirstAsync(p => p.Id == galleryId, cancellationToken);

            if (File.Exists(existingMedia.MediaPath))
                File.Delete(existingMedia.MediaPath);

            context.GalleryMedias.Remove(existingMedia);
            await context.SaveChangesAsync(cancellationToken);
        }

        string GetFileSavePath()
        {
            var path = configuration["ProjectConfigs:WmsConfigs:GallerySavePath"];

            return path.HasNoValue()
                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Gallery")
                : path;
        }

        void GetAdditionalData(Domains.Entities.Gallery data)
        {
            if (data.UsageType == (int)GalleryUsageType.Tag)
            {
                data.Data = JsonConvert.SerializeObject(new
                {
                    ActionTypeTitle = "رجیستر",
                });
            }

            if (data.UsageType == (int)GalleryUsageType.Action)
            {
                string uhfLogCommand = $"""
                    SELECT TOP(1) fld_Reader_GateType
                    FROM tbl_UHF_ReaderLog
                    WHERE fld_InventoryId = {data.UsageId}
                    """;

                var resultUhfLog = dataAccess.SqlDataAdapter(uhfLogCommand).Select();

                if (resultUhfLog.Any())
                {
                    var actionTypeCode = (int)resultUhfLog[0].ItemArray[0];

                    var actionTypesTitle = context.ActionTypes.FirstOrDefault(p => p.Code == actionTypeCode)?.Title;

                    data.Data = JsonConvert.SerializeObject(new
                    {
                        ActionTypeTitle = actionTypesTitle,
                    });
                }
            }
        }
    }
}
