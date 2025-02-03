using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    private readonly IMapper _mapper;

    public ProdutosController(IUnitOfWork uof, IMapper mapper)
    {
        _uof = uof;
        _mapper = mapper;
    }

    [HttpGet("produtos-categoria/{id:int}")]
    public ActionResult<IEnumerable<ProdutoDTO>> GetProdutosCategoria(int id)
    {
        var produtos = _uof.ProdutoRepository.GetProdutosPorCategoria(id);

        if (produtos is null)
            return NotFound();

        var produtosDTO = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

        return Ok(produtosDTO);
    }


    [HttpGet("/primeiro")] //cancela o atributo route do controller ex.: localhost:port/primeiro
    public ActionResult<ProdutoDTO> GetPrimeiro()
    {
        var produto = _uof.ProdutoRepository.GetAll().FirstOrDefault();

        var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

        return produtoDTO is null ? NotFound("Produto não encontrado") : Ok(produtoDTO);
    }

    [HttpGet]
    public ActionResult<IEnumerable<ProdutoDTO>> GetAsync()
    {
        var produtos = _uof.ProdutoRepository.GetAll().ToList();
        var produtosDTO = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
        return produtosDTO is null ? NotFound("Produtos não encontrados") : Ok(produtosDTO);
    }

    [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
    public ActionResult<ProdutoDTO> Get(int id)
    {
        var produto = _uof.ProdutoRepository.Get(p => p.Id == id);
        var produtoDTO = _mapper.Map<ProdutoDTO>(produto);
        return produtoDTO is null ? NotFound($"Produto com id={id} não encontrado") : Ok(produtoDTO);
    }

    [HttpPost]
    public ActionResult<ProdutoDTO> Post(ProdutoDTO produtoDto)
    {
        if (produtoDto is null) return BadRequest("Produto inválido.");
        var produto = _mapper.Map<Produto>(produtoDto);

        var novoProduto = _uof.ProdutoRepository.Create(produto);
        _uof.Commit();

        var novoProdutoDto = _mapper.Map<ProdutoDTO>(novoProduto);

        return new CreatedAtRouteResult("ObterProduto",
                                         new { id = novoProdutoDto.Id },
                                         novoProdutoDto);
    }

    [HttpPatch("{id}/UpdatePartial")]
    public ActionResult<ProdutoDTOUpdateResponse> Patch(int id, JsonPatchDocument<ProdutoDTOUpdateRequest> patchProdutoDTO)
    {
        if (patchProdutoDTO is null || id <= 0) return BadRequest();

        var produto = _uof.ProdutoRepository.Get(p => p.Id == id);

        if (produto is null)
            return NotFound();

        var produtoUpdateRequest = _mapper.Map<ProdutoDTOUpdateRequest>(produto);

        patchProdutoDTO.ApplyTo(produtoUpdateRequest,ModelState);

        if (!ModelState.IsValid || TryValidateModel(produtoUpdateRequest))
            return BadRequest(ModelState);

        _mapper.Map(produtoUpdateRequest, produto);

        _uof.ProdutoRepository.Update(produto);
        _uof.Commit();

        return Ok(_mapper.Map<ProdutoDTOUpdateResponse>(produto));
    }

    [HttpPut("{id:int:min(1)}")]
    public ActionResult<ProdutoDTO> Put(int id, ProdutoDTO produtoDto)
    {
        if (id != produtoDto.Id) return BadRequest("Produto inválido.");
        var produto = _mapper.Map<Produto>(produtoDto);
        var produtoAtualizado = _uof.ProdutoRepository.Update(produto);

        var produtoAtualizadoDto = _mapper.Map<ProdutoDTO>(produtoAtualizado);
        _uof.Commit();

        return Ok(produtoAtualizadoDto);
    }

    [HttpDelete("{id:int:min(1)}")]
    public ActionResult<ProdutoDTO> Delete(int id)
    {
        var produto = _uof.ProdutoRepository.Get(p => p.Id == id);
        if (produto is null)
            NotFound($"Produto com id={id} não encontrado");

        var produtoDeletado = _uof.ProdutoRepository.Delete(produto);
        var produtoDeletadoDto = _mapper.Map<ProdutoDTO>(produto);
        _uof.Commit();

        return Ok(produtoDeletadoDto);
    }

}
