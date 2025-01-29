using System.ComponentModel.DataAnnotations;

namespace APICatalogo.Validation;

public class PrimeiraLetraMaiusculaAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    { 
        if(value == null || string.IsNullOrEmpty(value?.ToString()))
            return ValidationResult.Success;
        

        char primeiraLetra = value!.ToString()?[0] ?? default;

        if (!char.IsUpper(primeiraLetra)) return new ValidationResult("A primeira letra deve ser maiúscula");

        return ValidationResult.Success;
    }
}
