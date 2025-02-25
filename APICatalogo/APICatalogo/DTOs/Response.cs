namespace APICatalogo.DTOs;

public sealed record Response
{
    public string? Status { get; set; }
    public string? Message { get; set; }
}
