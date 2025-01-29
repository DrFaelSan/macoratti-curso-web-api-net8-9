using APICatalogo.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace APICatalogo.Models;

[Table("Produtos")]
public class Produto : IValidatableObject
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage ="O nome é obrigatório")]
    [StringLength(80)]
    //[PrimeiraLetraMaiuscula]
    public string Nome { get; set; } = default!;

    [Required]
    [StringLength(300, ErrorMessage ="A descrição deve ter no máximo {1} caracteres")]
    public string Descricao { get; set; } = default!;

    [Required]
    [StringLength(300)]
    public string ImagemUrl { get; set; } = default!;

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    [Range(1,100000, ErrorMessage ="O preço deve estar entre {1} e {2}")]
    public decimal Preco { get; set; }
    public float Estoque { get; set; }
    public DateTime DataCadastro { get; set; }
    public int CategoriaId { get; set; }

    [JsonIgnore]
    public Categoria? Categoria { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DataCadastro < DateTime.Today) yield return new ValidationResult("A data do cadastro não pode ser inferior a data de hoje.",
            [
                nameof(DataCadastro),
            ]);
       
        if(Estoque <= 0)
            yield return new ValidationResult("O estoque não pode ser inferior a 0.",
             [
                 nameof(Estoque),
            ]);
    }
}
