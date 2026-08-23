using Silo.Domains.Entities;

namespace Silo.Application.Dto;

public class GetAllAggDocDto
{
    public string DocumentKey { get; set; }
    public string DocumentType { get; set; }
    public DateTime? ImportDateTime { get; set; }
    public int ItemCount { get; set; }
    public decimal ItemSum { get; set; }
    public string DocumentData { get; set; }
    public ICollection<DocumentItem> DocumentItems { get; set; }
}

public class GetAllDocAggSuggestDetailDto
{
    public string DocumentKey { get; set; }
    public string DocumentType { get; set; }
    public DateTime? ImportDateTime { get; set; }
    public int ItemCount { get; set; }
    public decimal ItemSum { get; set; }
    public string DocumentData { get; set; }
}
