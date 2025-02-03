using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APICatalogo.DTOs;

public class ProdutoDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(80)]
    public string Nome { get; set; } = default!;

    [Required]
    [StringLength(300, ErrorMessage = "A descrição deve ter no máximo {1} caracteres")]
    public string Descricao { get; set; } = default!;

    [Required]
    [StringLength(300)]
    public string ImagemUrl { get; set; } = default!;

    [Required]
    public decimal Preco { get; set; }
    public int CategoriaId { get; set; }
}
