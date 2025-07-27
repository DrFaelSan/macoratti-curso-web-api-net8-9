using APICatalogo.Controllers;
using APICatalogo.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogoxUnitTests.UnitTests;

public class GetProdutosUnitTests(ProdutosUnitTestController controller) : IClassFixture<ProdutosUnitTestController>
{
    private readonly ProdutosController _controller = new(controller.repository, controller.mapper);

    [Fact]
    public async Task GetProdutoById_Return_OkResult()
    {
        //Arrange
        var prodId = 1;
        
        //Action
        var data = await _controller.GetAsync(prodId);

        //Assert (xUnit)
        //var OkResult = Assert.IsType<OkObjectResult>(data.Result);
        //Assert.Equal(StatusCodes.Status200OK, OkResult.StatusCode);

        //Assert (FluentAssertions)
        data.Result.Should()
                   .BeOfType<OkObjectResult>()
                   .Which.StatusCode
                   .Should()
                   .Be(StatusCodes.Status200OK);

    } 

    [Fact]
    public async Task GetProdutoById_Return_NotFound()
    {

        //Arrange
        var prodId = 32131;

        //Action
        var data = await _controller.GetAsync(prodId);

        //Assert (xUnit)
        //var notFoundResult = Assert.IsType<NotFoundObjectResult>(data.Result);
        //Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);

        //Assert (FluentAssertions)
        data.Result.Should()
                   .BeOfType<NotFoundObjectResult>()
                   .Which.StatusCode
                   .Should()
                   .Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task GetProdutoById_Return_BadRequest()
    {
        //Arrange
        var prodId = -1;

        //Action
        var data = await _controller.GetAsync(prodId);

        //Assert (xUnit)
        //var badRequestResult = Assert.IsType<BadRequestObjectResult>(data.Result);
        //Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

        //Assert (FluentAssertions)
        data.Result.Should()
                   .BeOfType<BadRequestObjectResult>()
                   .Which.StatusCode
                   .Should()
                   .Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetProdutos_Return_ListOfProdutoDto()
    {
        //Arrange
        //Action
        var data = await _controller.GetAsync();

        //Assert (xUnit)
        //var OkResult = Assert.IsType<OkObjectResult>(data.Result);
        //Assert.Equal(StatusCodes.Status200OK, OkResult.StatusCode);
        //Assert.IsAssignableFrom<IEnumerable<ProdutoDTO>>(data.Value);
        //Assert.NotNull(data.Value);

        //Assert (FluentAssertions)
        data.Result.Should()
                   .BeOfType<OkObjectResult>()
                   .Which.Value
                   .Should()
                   .BeAssignableTo<IEnumerable<ProdutoDTO>>()
                   .And
                   .NotBeNull();
    }
}