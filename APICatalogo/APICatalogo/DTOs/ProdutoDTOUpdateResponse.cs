namespace APICatalogo.DTOs;

public sealed record ProdutoDTOUpdateResponse
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public string Descricao { get; set; } = default!;
    public string ImagemUrl { get; set; } = default!;
    public decimal Preco { get; set; }
    public float Estoque { get; set; }
    public DateTime DataCadastro { get; set; }
    public int CategoriaId { get; set; }
}
