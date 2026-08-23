namespace Silo.Application.Dto;

public class GallerySaveDto
{
    public string UserId { get; set; }
    public string MediaName { get; set; }
    public string MediaPath { get; set; }
    public int UsageType { get; set; }
    public DateTime UpldoadDateTime { get; set; }
    public string UsageId { get; set; }
    public int Extension { get; set; }
}

public class GetGalleryDto
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Username { get; set; }
    public string MediaName { get; set; }
    public string MediaPath { get; set; }
    public int UsageType { get; set; }
    public DateTime UpldoadDateTime { get; set; }
    public string UsageId { get; set; }
    public int Extension { get; set; }
    public string Data { get; set; }
}
