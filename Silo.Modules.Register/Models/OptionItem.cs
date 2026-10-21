namespace Silo.Modules.Register.Models;

/// <summary>A value and the text shown for it in a drop-down (modes, outputs, lines, shifts...).</summary>
public sealed record OptionItem(string Value, string Text);
