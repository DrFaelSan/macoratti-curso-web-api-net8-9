using APICatalogo.Controllers;
using APICatalogo.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogoxUnitTests.UnitTests;

public class PutProdutosUnitTests(ProdutosUnitTestController controller) : IClassFixture<ProdutosUnitTestController>
{
    private readonly ProdutosController _controller = new(controller.repository, controller.mapper);

    [Fact]
    public async Task PutProduto_Return_OkResult()
    {
        //Arrange
        var produtoId = 1;

        string imagemUrl = "https://static.wikia.nocookie.net/zelda/images/9/9d/Link_Artwork_1_%28Twilight_Princess%29.png/revision/latest/scale-to-width-down/213?cb=20101014225139&path-prefix=pt";

        var produtoDto = new ProdutoDTO
        {
            Id = produtoId,
            Nome = "IPhone 14",
            Descricao = "Smartphone apple iphone 14",
            ImagemUrl =  imagemUrl,
            CategoriaId = 1,
        };

        //Action
        var data = await _controller.PutAsync(produtoId, produtoDto);


        //Assert (xUnit)
        //Assert.NotNull(data);
        //Assert.IsType<OkObjectResult>(data.Result);

        ////Assert (FluentAssertions)
        data.Should()
            .NotBeNull();
        data.Result.Should()
            .BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PutProduto_Return_BadRequest()
    {

        //Arrange
        var produtoId = 1;

        string imagemUrl = "https://static.wikia.nocookie.net/zelda/images/9/9d/Link_Artwork_1_%28Twilight_Princess%29.png/revision/latest/scale-to-width-down/213?cb=20101014225139&path-prefix=pt";

        var produtoDto = new ProdutoDTO
        {
            Id = 2,
            Nome = "IPhone 14",
            Descricao = "Smartphone apple iphone 14",
            ImagemUrl = imagemUrl,
            CategoriaId = 1,
        };

        //Action
        var data = await _controller.PutAsync(produtoId, produtoDto);

        //Assert (xUnit)
        //var badRequestResult = Assert.IsType<BadRequestObjectResult>(data.Result);
        //Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

        ////Assert (FluentAssertions)
        data.Result.Should()
                   .BeOfType<BadRequestObjectResult>()
                   .Which.StatusCode
                   .Should()
                   .Be(StatusCodes.Status400BadRequest);
    }
}