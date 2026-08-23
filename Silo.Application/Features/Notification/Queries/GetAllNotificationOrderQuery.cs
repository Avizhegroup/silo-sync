namespace Silo.Application.Features;

public class GetAllNotificationOrderQuery
{
    public int Id { get; set; } = 0;
    public int Status { get; set; }
    public DateTime DateTime { get; set; }
    public string DateTimeShamsi { get; set; }
    public string UserId { get; set; }
    public string Username { get; set; }
    public string Type { get; set; }
    public string TimePeriod { get; set; }
    public string SendDay { get; set; }
    public string SendClock { get; set; }
    public string SendType { get; set; }
    public string SendContacts { get; set; }
    public string Content { get; set; }
}