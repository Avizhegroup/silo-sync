namespace Silo.Application.Features;

public class GetUserTokensVm
{
    public List<GetUserTokensDto> Result { get; set; }
}

public class GetUserTokensDto
{
    public int Id { get; set; }
    public string Value { get; set; }
    public string MaskedValue => MaskToken(Value);
    public string UserId { get; set; }
    public bool HasExpired { get; set; }

    private string MaskToken(string token)
    {
        if (string.IsNullOrEmpty(token) || token.Length < 20)
        {
            return "***";
        }

        return $"{token.Substring(0, 10)}...{token.Substring(token.Length - 10)}";
    }
}
