using APICatalogo.Context;
using APICatalogo.Models;

namespace APICatalogo.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context)
    {
        _context = context;
    }

    public Produto Create(Produto produto)
    {
        if(produto is null) ArgumentException.ThrowIfNullOrEmpty(nameof(produto));

        _context.Produtos.Add(produto);
        _context.SaveChanges();
        return produto;

    }

    public bool Delete(int id)
    {
        var produto = _context.Produtos.Find(id);

        if(produto is null) return false;

        _context.Produtos.Remove(produto);
        _context.SaveChanges();
        return true;
    }

    public Produto GetProduto(int id)
    {
        return _context.Produtos.FirstOrDefault(p => p.Id == id);
    }

    public IQueryable<Produto> GetProdutos()
    {
        return _context.Produtos;
    }

    public bool Update(Produto produto)
    {
        if (produto is null) ArgumentException.ThrowIfNullOrEmpty(nameof(produto));

        if(_context.Produtos.Any(p => p.Id == produto.Id))
        {
            _context.Produtos.Update(produto);
            _context.SaveChanges();
            return true;
        }
        return false;

    }
}