﻿using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using X.PagedList;

namespace APICatalogo.Repositories;

public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
   public CategoriaRepository(AppDbContext context) : base(context){}

    public async Task<IPagedList<Categoria>> GetCategoriasAsync(CategoriasParameters categoriasParameters)
    {

        var categorias = await GetAllAsync();

        var categoriasOrdenadas = categorias.OrderBy(c => c.Id).AsQueryable();

        var resultado = await categoriasOrdenadas.ToPagedListAsync(categoriasParameters.PageNumber, categoriasParameters.PageSize);
        return resultado;
    }

    public async Task<IPagedList<Categoria>> GetCategoriasFiltroNomeAsync(CategoriasFiltroNome categoriasFiltroNome)
    {
        var categorias = await GetAllAsync();

        if (!string.IsNullOrEmpty(categoriasFiltroNome.Nome))
            categorias = categorias.Where(c => c.Nome.Contains(categoriasFiltroNome.Nome));

        var categoriasFiltradas = await categorias.ToPagedListAsync(categoriasFiltroNome.PageNumber, categoriasFiltroNome.PageSize);
        return categoriasFiltradas;
    }
}
