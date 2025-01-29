using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly AppDbContext _context;

    public CategoriaRepository(AppDbContext context)
        =>  _context = context;

    public Categoria CreateCategoria(Categoria categoria)
    {
        if(categoria is null) ArgumentException.ThrowIfNullOrEmpty(nameof(categoria));

        _context.Categorias.Add(categoria);
        _context.SaveChanges();
        return categoria;
    }

    public Categoria DeleteCategoria(int id)
    {
        var categoria = _context.Categorias.Find(id);
        if (categoria is null) ArgumentException.ThrowIfNullOrEmpty(nameof(categoria));

        _context.Categorias.Remove(categoria);
        _context.SaveChanges();
        return categoria;

    }

    public Categoria GetCategoria(int id)
    {
        return _context.Categorias.FirstOrDefault(c => c.Id == id);
    }

    public IEnumerable<Categoria> GetCategorias()
    {
        return _context.Categorias.ToList();
    }

    public Categoria UpdateCategoria(Categoria categoria)
    {
        if (categoria is null) ArgumentException.ThrowIfNullOrEmpty(nameof(categoria));

        _context.Entry(categoria).State = EntityState.Modified;
        _context.SaveChanges();
        return categoria;
    }
}
