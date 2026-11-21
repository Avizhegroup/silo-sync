namespace Silo.Application.Features;

public enum ActionTypeKindEnum
{
    Movement,
    Register
}

public class ActionTypeKindItem
{
    public ActionTypeKindEnum Value { get; set; }
    public string Title { get; set; }
}
