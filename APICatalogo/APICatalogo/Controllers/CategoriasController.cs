using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaRepository _repository;

    public CategoriasController(ICategoriaRepository repository)
        =>  _repository = repository;

    [HttpGet]
    [ServiceFilter(typeof(ApiLoggingFilter))]
    public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
    {
        var categorias = _repository.GetCategorias();
        return Ok(categorias);
    }

    
    [HttpGet("{id:int:min(1)}", Name = "ObterCategoria")]
    public ActionResult<Categoria> Get(int id)
    {
        var categoria = _repository.GetCategoria(id);
        return categoria is null ? NotFound($"Categoria com id={id} não encontrada") : Ok(categoria);
    }

    [HttpPost]
    public ActionResult Post(Categoria categoria)
    {
        if (categoria is null) return BadRequest("Categoria inválida.");

        var categoriaCriada = _repository.CreateCategoria(categoria);

        return new CreatedAtRouteResult("ObterCategoria",
                                         new { id = categoriaCriada.Id },
                                         categoriaCriada);
    }

    [HttpPut("{id:int:min(1)}")]
    public ActionResult Put(int id, Categoria categoria)
    {

        if (id != categoria.Id) return BadRequest("Categoria inválida.");

        _repository.UpdateCategoria(categoria);
        return Ok(categoria);
    }

    [HttpDelete("{id:int:min(1)}")]
    public ActionResult Delete(int id)
    {

        var categoria = _repository.GetCategoria(id);
        if (categoria is null) return NotFound($"Categoria com id={id} não localizada");

        _repository.DeleteCategoria(id);

        return Ok(categoria);
    }
}
