using APICatalogo.Controllers;
using APICatalogo.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogoxUnitTests.UnitTests;
public class PostProdutosUnitTests(ProdutosUnitTestController controller) : IClassFixture<ProdutosUnitTestController>
{
    private readonly ProdutosController _controller = new(controller.repository, controller.mapper);

    [Fact]
    public async Task PostProduto_Return_CreatedStatusCode()
    {
        //Arrange
        string imagemUrl = "https://static.wikia.nocookie.net/zelda/images/9/9d/Link_Artwork_1_%28Twilight_Princess%29.png/revision/latest/scale-to-width-down/213?cb=20101014225139&path-prefix=pt";

        ProdutoDTO produto = new()
        {
            Nome = "Kindle",
            Descricao = "Device para leitura fácil e portatil",
            Preco = 400,
            ImagemUrl = imagemUrl,
            CategoriaId = 1
        };

        //Action
        var data = await _controller.PostAsync(produto);

        //Assert (xUnit)
        //var createdResult = Assert.IsType<CreatedAtRouteResult>(data.Result);
        //Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);

        //Assert (FluentAssertions)
        var createdResult = data.Result.Should().BeOfType<CreatedAtRouteResult>();
        createdResult.Subject.StatusCode.Should().Be(StatusCodes.Status201Created);
    }

    [Fact]
    public async Task PostProduto_Return_BadRequest()
    {
        //Arrange
        ProdutoDTO? produto = null;

        //Action
        var data = await _controller.PostAsync(produto);

        //Assert (xUnit)
        //var badRequest = Assert.IsType<BadRequestObjectResult>(data.Result);
        //Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);

        //Assert (FluentAssertions)
        var badRequest = data.Result.Should().BeOfType<BadRequestObjectResult>();
        badRequest.Subject.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }
}
