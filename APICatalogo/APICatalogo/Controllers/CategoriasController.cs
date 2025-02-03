using APICatalogo.DTOs;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
public class CategoriasController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    private readonly IMapper _mapper;

    public CategoriasController(IUnitOfWork uof, IMapper mapper)
    {
        _uof = uof;
        _mapper = mapper;
    }

    [HttpGet]
    [ServiceFilter(typeof(ApiLoggingFilter))]
    public ActionResult<IEnumerable<CategoriaDTO>> GetCategoriasProdutos()
    {
        var categorias = _uof.CategoriaRepository.GetAll();
        var categoriasDto = _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);
        return Ok(categoriasDto);
    }

    
    [HttpGet("{id:int:min(1)}", Name = "ObterCategoria")]
    public ActionResult<CategoriaDTO> Get(int id)
    {
        var categoria = _uof.CategoriaRepository.Get(c => c.Id == id);
        var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);
        return categoriaDTO is null ? NotFound($"Categoria com id={id} não encontrada") : Ok(categoriaDTO);
    }

    [HttpPost]
    public ActionResult<CategoriaDTO> Post(CategoriaDTO categoriaDto)
    {
        if (categoriaDto is null) return BadRequest("Categoria inválida.");

        var categoria = _mapper.Map<Categoria>(categoriaDto);

        var categoriaCriada = _uof.CategoriaRepository.Create(categoria);
        _uof.Commit();

        var categoriaCriadaDTO = _mapper.Map<CategoriaDTO>(categoriaCriada);

        return new CreatedAtRouteResult("ObterCategoria",
                                         new { id = categoriaCriadaDTO.Id },
                                         categoriaCriadaDTO);
    }

    [HttpPut("{id:int:min(1)}")]
    public ActionResult<CategoriaDTO> Put(int id, CategoriaDTO categoriaDto)
    {

        if (id != categoriaDto.Id) return BadRequest("Categoria inválida.");

        var categoria = _mapper.Map<Categoria>(categoriaDto);

        var categoriaAtualizada = _uof.CategoriaRepository.Update(categoria);
        _uof.Commit();

        var categoriaAtualizadaDto = _mapper.Map<CategoriaDTO>(categoriaAtualizada);

        return Ok(categoriaAtualizadaDto);
    }

    [HttpDelete("{id:int:min(1)}")]
    public ActionResult<CategoriaDTO> Delete(int id)
    {

        var categoria = _uof.CategoriaRepository.Get(c => c.Id == id);
        if (categoria is null) return NotFound($"Categoria com id={id} não localizada");

        _uof.CategoriaRepository.Delete(categoria);
        _uof.Commit();

        var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

        return Ok(categoriaDTO);
    }
}
