using APICatalogo.Models;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
    private readonly IUnitOfWork _uof;

    public ProdutosController(IUnitOfWork uof)
    {
        _uof = uof;
    }

    [HttpGet("produtos-categoria/{id:int}")]
    public ActionResult<IEnumerable<Produto>> GetProdutosCategoria(int id)
    {
        var produtos = _uof.ProdutoRepository.GetProdutosPorCategoria(id);

        if (produtos is null)
            return NotFound();

        return Ok(produtos);
    }


    [HttpGet("/primeiro")] //cancela o atributo route do controller ex.: localhost:port/primeiro
    public ActionResult<Produto> GetPrimeiro()
    {
        var produto = _uof.ProdutoRepository.GetAll().FirstOrDefault();

        return produto is null ? NotFound("Produto não encontrado") : Ok(produto);
    }

    [HttpGet]
    public ActionResult<IEnumerable<Produto>> GetAsync()
    {
        var produtos = _uof.ProdutoRepository.GetAll().ToList();

        return produtos is null ? NotFound("Produtos não encontrados") : Ok(produtos);
    }

    [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
    public ActionResult<Produto> Get(int id)
    {
        var produto = _uof.ProdutoRepository.Get(p => p.Id == id);
        return produto is null ? NotFound($"Produto com id={id} não encontrado") : Ok(produto);
    }

    [HttpPost]
    public ActionResult Post(Produto produto)
    {
        if (produto is null) return BadRequest("Produto inválido.");

        var novoProduto = _uof.ProdutoRepository.Create(produto);
        _uof.Commit();

        return new CreatedAtRouteResult("ObterProduto",
                                         new { id = novoProduto.Id },
                                         novoProduto);
    }

    [HttpPut("{id:int:min(1)}")]
    public ActionResult Put(int id, Produto produto)
    {
        if (id != produto.Id) return BadRequest("Produto inválido.");

        var produtoAtualizado = _uof.ProdutoRepository.Update(produto);
        _uof.Commit();

        return Ok(produtoAtualizado); //: StatusCode(500, $"Falha em atualizar o produto de id={id}");
    }

    [HttpDelete("{id:int:min(1)}")]
    public ActionResult Delete(int id)
    {
        var produto = _uof.ProdutoRepository.Get(p => p.Id == id);
        if (produto is null)
            NotFound($"Produto com id={id} não encontrado");

        var produtoDeletado = _uof.ProdutoRepository.Delete(produto);
        _uof.Commit();

        return Ok(produtoDeletado);
    }

}
