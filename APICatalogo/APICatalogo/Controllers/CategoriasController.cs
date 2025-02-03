using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
public class CategoriasController : ControllerBase
{
    private readonly IUnitOfWork _uof;

    public CategoriasController( IUnitOfWork uof)
    {
        _uof = uof;
    }

    [HttpGet]
    [ServiceFilter(typeof(ApiLoggingFilter))]
    public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
    {
        var categorias = _uof.CategoriaRepository.GetAll();
        return Ok(categorias);
    }

    
    [HttpGet("{id:int:min(1)}", Name = "ObterCategoria")]
    public ActionResult<Categoria> Get(int id)
    {
        var categoria = _uof.CategoriaRepository.Get(c => c.Id == id);
        return categoria is null ? NotFound($"Categoria com id={id} não encontrada") : Ok(categoria);
    }

    [HttpPost]
    public ActionResult Post(Categoria categoria)
    {
        if (categoria is null) return BadRequest("Categoria inválida.");

        var categoriaCriada = _uof.CategoriaRepository.Create(categoria);
        _uof.Commit();

        return new CreatedAtRouteResult("ObterCategoria",
                                         new { id = categoriaCriada.Id },
                                         categoriaCriada);
    }

    [HttpPut("{id:int:min(1)}")]
    public ActionResult Put(int id, Categoria categoria)
    {

        if (id != categoria.Id) return BadRequest("Categoria inválida.");

        _uof.CategoriaRepository.Update(categoria);
        _uof.Commit();

        return Ok(categoria);
    }

    [HttpDelete("{id:int:min(1)}")]
    public ActionResult Delete(int id)
    {

        var categoria = _uof.CategoriaRepository.Get(c => c.Id == id);
        if (categoria is null) return NotFound($"Categoria com id={id} não localizada");

        _uof.CategoriaRepository.Delete(categoria);
        _uof.Commit();

        return Ok(categoria);
    }
}
