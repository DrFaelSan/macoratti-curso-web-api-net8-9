using APICatalogo.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogoxUnitTests.UnitTests;

public class DeleteProdutosUnitTests(ProdutosUnitTestController controller) : IClassFixture<ProdutosUnitTestController>
{
    private readonly ProdutosController _controller = new(controller.repository, controller.mapper);

    [Fact]
    public async Task DeleteProdutoById_Return_OkResult()
    {
        //Arrange
        var produtoId = 1;
        
        //Action
        var data = await _controller.DeleteAsync(produtoId);
        
        //Assert (xUnit)
        //Assert.NotNull(data);
        //Assert.IsType<OkResult>(data.Result);
        
        
        //Assert FluentAssertions
        data.Should().NotBeNull();
        data.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task DeleteProdutoById_Return_BadRequest()
    {
        //Arrange
        var produtoId = 321321;
        
        //Action
        var data = await _controller.DeleteAsync(produtoId);

        //Assert (xUnit)
        //Assert.NotNull(data);
        //Assert.IsType<NotFoundObjectResult>(data.Result);


        //Assert FluentAssertions
        data.Should().NotBeNull();
        data.Result.Should().BeOfType<NotFoundObjectResult>();
    }
}