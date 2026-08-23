namespace Silo.Application.Dto;

public class TreeviewNode
{
    public string thisnodeid { get; set; }
    public string text { get; set; }
    public bool selectable { get; set; }
    public TreeviewNode[] nodes { get; set; }
    public string href { get; set; }
    public string value { get; set; }
    public int index { get; set; }
    public bool isSelected { get; set; } = false;
    public bool IsExpanded { get; set; } = false;
}
