using APICatalogo.Models;
using APICatalogo.Pagination;
using X.PagedList;

namespace APICatalogo.Repositories;

public interface IProdutoRepository : IRepository<Produto>
{
    Task<IPagedList<Produto>> GetProdutosAsync(ProdutosParameters produtosParameters);
    Task<IEnumerable<Produto>> GetProdutosPorCategoriaAsync(int id);
}