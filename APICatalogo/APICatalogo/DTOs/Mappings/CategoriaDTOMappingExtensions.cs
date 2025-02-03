using APICatalogo.DTOs;
using APICatalogo.Models;

namespace APICatalogo.DTOs.Mappings;

public static class CategoriaDTOMappingExtensions
{
    public static CategoriaDTO? ToCategoriaDTO(this Categoria categoria)
    {
        if (categoria is null) return null;
        return new CategoriaDTO
        {
            Id = categoria.Id,
            ImagemUrl = categoria?.ImagemUrl,
            Nome = categoria.Nome
        };
    }

    public static Categoria? ToCategoria(this CategoriaDTO categoriaDTO)
    {
        if (categoriaDTO is null) return null;
        return new Categoria
        {
            Nome = categoriaDTO.Nome,
            Id = categoriaDTO.Id,
            ImagemUrl = categoriaDTO?.ImagemUrl
        };
    }

    public static IEnumerable<CategoriaDTO> ToCategoriaDTOList(this IEnumerable<Categoria> categorias)
    {
        if (categorias is null || !categorias.Any()) return Enumerable.Empty<CategoriaDTO>();
        return categorias.Select(categoria => new CategoriaDTO
        {
            Id = categoria.Id,
            ImagemUrl = categoria.ImagemUrl,
            Nome = categoria.Nome
        }).ToList();
    }
}
