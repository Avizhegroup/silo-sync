namespace Silo.Application.Features;

public class SaveNotificationOrderCommand
{
    public int Id { get; set; } = 0;
    public bool Status { get; set; } = false;
    public string Title { get; set; }
    public DateTime DateTime { get; set; }
    public string DateTimeShamsi { get; set; }
    public string UserId { get; set; }
    public string Username { get; set; }
    public string Type { get; set; } = "0";

    public int EventType { get; set; }

    public string TimePeriod { get; set; }

    public string SendDay { get; set; }

    public string SendClock { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Notif_SendType))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string SendType { get; set; }

    public string SendContacts { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Notif_Content))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string Content { get; set; }
}
