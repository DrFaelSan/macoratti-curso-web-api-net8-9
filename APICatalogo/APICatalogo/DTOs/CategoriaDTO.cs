using System.ComponentModel.DataAnnotations;

namespace APICatalogo.DTOs;

public sealed record CategoriaDTO
{
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string Nome { get; set; } = default!;

    [Required]
    [StringLength(300)]
    public string? ImagemUrl { get; set; } = default!;
}
