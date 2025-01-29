using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace APICatalogo.Models;

[Table("Categorias")]
public class Categoria
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string Nome { get; set; } = default!;

    [Required]
    [StringLength(300)]
    public string? ImagemUrl { get; set; } = default!;

    [JsonIgnore]
    public ICollection<Produto>? Produtos { get; set; }

    public Categoria()
        => Produtos = [];
}
