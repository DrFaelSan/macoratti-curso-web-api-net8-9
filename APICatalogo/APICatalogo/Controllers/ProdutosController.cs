using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sieve.Services;
using X.PagedList;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    private readonly IMapper _mapper;
    private readonly SieveProcessor _sieveProcessor;

    public ProdutosController(IUnitOfWork uof, 
                              IMapper mapper, 
                              SieveProcessor sieveProcessor)
    {
        _uof = uof;
        _mapper = mapper;
        _sieveProcessor = sieveProcessor;
    }

    [HttpGet("produtos-categoria/{id:int}")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosCategoriaAsync(int id)
    {
        var produtos = await _uof.ProdutoRepository.GetProdutosPorCategoriaAsync(id);

        if (produtos is null)
            return NotFound();

        var produtosDTO = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

        return Ok(produtosDTO);
    }


    [HttpGet("/primeiro")] //cancela o atributo route do controller ex.: localhost:port/primeiro
    public async Task<ActionResult<ProdutoDTO>> GetPrimeiroAsync()
    {
        var produto = await _uof.ProdutoRepository.GetAllAsync();

        var produtoDTO = _mapper.Map<ProdutoDTO>(produto.FirstOrDefault());

        return produtoDTO is null ? NotFound("Produto não encontrado") : Ok(produtoDTO);
    }

    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetAsync([FromQuery] ProdutosParameters produtosParameters)
    {
        var produtos = await _uof.ProdutoRepository.GetProdutosAsync(produtosParameters);

        return ObterProdutos(produtos);
    }

    private ActionResult<IEnumerable<ProdutoDTO>> ObterProdutos(IPagedList<Produto> produtos)
    {
        var metadata = new
        {
            produtos.Count,
            produtos.PageSize,
            produtos.TotalItemCount,
            produtos.PageCount,
            produtos.HasNextPage,
            produtos.HasPreviousPage
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        var produtosDTO = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
        return Ok(produtosDTO);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetAsync()
    {
        var produtos = await _uof.ProdutoRepository.GetAllAsync();
        var produtosDTO = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
        return produtosDTO is null ? NotFound("Produtos não encontrados") : Ok(produtosDTO);
    }

    [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
    public async Task<ActionResult<ProdutoDTO>> GetAsync(int id)
    {
        var produto = await _uof.ProdutoRepository.GetAsync(p => p.Id == id);
        var produtoDTO = _mapper.Map<ProdutoDTO>(produto);
        return produtoDTO is null ? NotFound($"Produto com id={id} não encontrado") : Ok(produtoDTO);
    }

    [HttpPost]
    public async Task<ActionResult<ProdutoDTO>> PostAsync(ProdutoDTO produtoDto)
    {
        if (produtoDto is null) return BadRequest("Produto inválido.");
        var produto = _mapper.Map<Produto>(produtoDto);

        var novoProduto = _uof.ProdutoRepository.Create(produto);
        await _uof.CommitAsync();

        var novoProdutoDto = _mapper.Map<ProdutoDTO>(novoProduto);

        return new CreatedAtRouteResult("ObterProduto",
                                         new { id = novoProdutoDto.Id },
                                         novoProdutoDto);
    }

    [HttpPatch("{id}/UpdatePartial")]
    public async Task<ActionResult<ProdutoDTO>> PatchAsync(int id, JsonPatchDocument<ProdutoDTOUpdateRequest> patchProdutoDTO)
    {
        if (patchProdutoDTO is null || id <= 0) return BadRequest();

        var produto = await _uof.ProdutoRepository.GetAsync(p => p.Id == id);

        if (produto is null)
            return NotFound();

        var produtoUpdateRequest = _mapper.Map<ProdutoDTOUpdateRequest>(produto);

        patchProdutoDTO.ApplyTo(produtoUpdateRequest,ModelState);

        if (!ModelState.IsValid || TryValidateModel(produtoUpdateRequest))
            return BadRequest(ModelState);

        _mapper.Map(produtoUpdateRequest, produto);

        _uof.ProdutoRepository.Update(produto);
        await _uof.CommitAsync();

        return Ok(_mapper.Map<ProdutoDTOUpdateResponse>(produto));
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult<ProdutoDTO>> PutAsync(int id, ProdutoDTO produtoDto)
    {
        if (id != produtoDto.Id) return BadRequest("Produto inválido.");
        var produto = _mapper.Map<Produto>(produtoDto);
        var produtoAtualizado = _uof.ProdutoRepository.Update(produto);

        var produtoAtualizadoDto = _mapper.Map<ProdutoDTO>(produtoAtualizado);
        await _uof.CommitAsync();

        return Ok(produtoAtualizadoDto);
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult<ProdutoDTO>> DeleteAsync(int id)
    {
        var produto = await _uof.ProdutoRepository.GetAsync(p => p.Id == id);
        if (produto is null)
            NotFound($"Produto com id={id} não encontrado");

        var produtoDeletado = _uof.ProdutoRepository.Delete(produto);
        var produtoDeletadoDto = _mapper.Map<ProdutoDTO>(produto);
        await _uof.CommitAsync();

        return Ok(produtoDeletadoDto);
    }

}
