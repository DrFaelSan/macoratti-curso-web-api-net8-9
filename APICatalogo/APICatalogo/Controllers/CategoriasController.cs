using APICatalogo.DTOs;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using X.PagedList;
using Microsoft.AspNetCore.Http;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
[Produces("application/json")]
[ApiExplorerSettings(IgnoreApi = true)]
public class CategoriasController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    private readonly IMapper _mapper;

    public CategoriasController(IUnitOfWork uof, IMapper mapper)
    {
        _uof = uof;
        _mapper = mapper;
    }

    ///<summary>
    ///Obtem um lista de objetos Categoria
    ///</summary>
    ///<returns>Uma lista de objetos Categoria</returns>
    [HttpGet]
    [ServiceFilter(typeof(ApiLoggingFilter))]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasProdutosAsync()
    {
        var categorias = await _uof.CategoriaRepository.GetAllAsync();
        var categoriasDto = _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);
        return Ok(categoriasDto);
    }

    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasPaginadasAsync([FromQuery] CategoriasParameters categoriasParameters)
    {
        var categorias = await _uof.CategoriaRepository.GetCategoriasAsync(categoriasParameters);
        return ObterCategorias(categorias);
    }

    [HttpGet("filter/nome/pagination")]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasFiltradas([FromQuery] CategoriasFiltroNome categoriasParameters)
    {
        var categorias = await _uof.CategoriaRepository.GetCategoriasFiltroNomeAsync(categoriasParameters);

        return ObterCategorias(categorias);
    }

    private ActionResult<IEnumerable<CategoriaDTO>> ObterCategorias(IPagedList<Categoria> categorias)
    {
        var metadata = new
        {
            categorias.Count,
            categorias.PageSize,
            categorias.TotalItemCount,
            categorias.PageCount,
            categorias.HasNextPage,
            categorias.HasPreviousPage
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        var categoriasDto = categorias.ToCategoriaDTOList();

        return Ok(categoriasDto);
    }

    ///<summary>
    ///Obtem uma Categoria pelo seu Id
    ///</summary>
    ///<returns>objeto Categoria</returns>
    [HttpGet("{id:int:min(1)}", Name = "ObterCategoria")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaDTO>> GetAsync(int id)
    {
        var categoria = await _uof.CategoriaRepository.GetAsync(c => c.Id == id);
        var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);
        return categoriaDTO is null ? NotFound($"Categoria com id={id} não encontrada") : Ok(categoriaDTO);
    }

    ///<summary>
    ///Inclui uma nova categoria
    ///</summary>
    ///<remarks>
    ///Exemplo de request:
    ///
    ///     POST api/Categorias
    ///     {
    ///         "id":1,
    ///         "nome": "categoria1",
    ///         "imagemUrl": "http://teste.net/1.jpg"
    ///     }
    ///</remarks>
    ///<returns>O objeto da categoria incluida</returns>
    ///<remarks>Retorna um objeto Categoria Inlcuido</remarks>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoriaDTO>> PostAsync(CategoriaDTO categoriaDto)
    {
        if (categoriaDto is null) return BadRequest("Categoria inválida.");

        var categoria = _mapper.Map<Categoria>(categoriaDto);

        var categoriaCriada = _uof.CategoriaRepository.Create(categoria);
        await _uof.CommitAsync();

        var categoriaCriadaDTO = _mapper.Map<CategoriaDTO>(categoriaCriada);

        return new CreatedAtRouteResult("ObterCategoria",
                                         new { id = categoriaCriadaDTO.Id },
                                         categoriaCriadaDTO);
    }

    [HttpPut("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<CategoriaDTO>> PutAsync(int id, CategoriaDTO categoriaDto)
    {

        if (id != categoriaDto.Id) return BadRequest("Categoria inválida.");

        var categoria = _mapper.Map<Categoria>(categoriaDto);

        var categoriaAtualizada = _uof.CategoriaRepository.Update(categoria);
        await _uof.CommitAsync();

        var categoriaAtualizadaDto = _mapper.Map<CategoriaDTO>(categoriaAtualizada);

        return Ok(categoriaAtualizadaDto);
    }

    [HttpDelete("{id:int:min(1)}")]
    [Authorize(Policy ="AdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<CategoriaDTO>> DeleteAsync(int id)
    {

        var categoria = await _uof.CategoriaRepository.GetAsync(c => c.Id == id);
        if (categoria is null) return NotFound($"Categoria com id={id} não localizada");

        _uof.CategoriaRepository.Delete(categoria);
        await _uof.CommitAsync();

        var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

        return Ok(categoriaDTO);
    }
}
