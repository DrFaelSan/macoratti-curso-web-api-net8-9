namespace APICatalogo.DTOs;

public sealed record TokenModel
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}
