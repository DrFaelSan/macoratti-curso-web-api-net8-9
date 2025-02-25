namespace APICatalogo.Configuracoes;
public sealed record JwtSection
{
    public string? ValidAudience { get; set; }
    public string? ValidIssuer { get; set; }
    public string? SecretKey { get; set; }
    public double TokenValidityInMinutes { get; set; }
    public double RefreshTokenValidityInMinutes { get; set; }
}