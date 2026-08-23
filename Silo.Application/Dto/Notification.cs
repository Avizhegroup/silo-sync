namespace Silo.Application.Dto;

public class Notification
{
    public int Id { get; set; }
    public bool Status { get; set; }
    public string Title { get; set; }
    public DateTime DateTime { get; set; }
    public string DateTimeShamsi { get; set; }
    public string UserId { get; set; }
    public string Username { get; set; }
    public string Type { get; set; }
    public int EventType { get; set; }
    public string TimePeriod { get; set; }
    public string SendDay { get; set; }
    public string SendClock { get; set; }
    public string SendType { get; set; }
    public string SendContacts { get; set; }
    public string Content { get; set; }
}

public class Queue
{
    public int Id { get; set; }
    public int SendStatus { get; set; }
    public DateTime SendDateTime { get; set; }
    public string Text { get; set; }
    public int SendType { get; set; }
    public string SendDate { get; set; }
    public string SendTime { get; set; }
    public string Contact { get; set; }
    public int OrderId { get; set; }
    public string ActionCode { get; set; }
    public DateTime SaveDateTime { get; set; }
}
