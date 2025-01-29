using APICatalogo.Models;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoRepository _repository;

    public ProdutosController(IProdutoRepository repository)
        => _repository = repository;


    [HttpGet("/primeiro")] //cancela o atributo route do controller ex.: localhost:port/primeiro
    public ActionResult<Produto> GetPrimeiro()
    {
        var produto = _repository.GetProdutos().FirstOrDefault();

        return produto is null ? NotFound("Produto não encontrado") : Ok(produto);
    }

    [HttpGet]
    public ActionResult<IEnumerable<Produto>> GetAsync()
    {
        var produtos = _repository.GetProdutos().ToList();

        return produtos is null ? NotFound("Produtos não encontrados") : Ok(produtos);
    }

    [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
    public ActionResult<Produto> Get(int id)
    {
        var produto = _repository.GetProduto(id);
        return produto is null ? NotFound($"Produto com id={id} não encontrado") : Ok(produto);
    }

    [HttpPost]
    public ActionResult Post(Produto produto)
    {
        if (produto is null) return BadRequest("Produto inválido.");

        var novoProduto = _repository.Create(produto);

        return new CreatedAtRouteResult("ObterProduto",
                                         new { id = novoProduto.Id },
                                         novoProduto);
    }

    [HttpPut("{id:int:min(1)}")]
    public ActionResult Put(int id, Produto produto)
    {
        if (id != produto.Id) return BadRequest("Produto inválido.");

        var atualizado = _repository.Update(produto);

        return atualizado ? Ok(produto) : StatusCode(500, $"Falha em atualizar o produto de id={id}");
    }

    [HttpDelete("{id:int:min(1)}")]
    public ActionResult Delete(int id)
    {
        var deletado = _repository.Delete(id);
        return deletado ? Ok($"Produto de id={id} foi excluído com sucesso!") : StatusCode(500, $"Falha em excluir o produto de id={id}");
    }

}
