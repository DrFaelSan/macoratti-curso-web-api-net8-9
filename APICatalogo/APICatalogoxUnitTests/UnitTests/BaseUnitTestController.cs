using APICatalogo.Context;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Models;
using APICatalogo.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace APICatalogoxUnitTests.UnitTests;

public class BaseUnitTestController
{
    public IUnitOfWork repository;
    public IMapper mapper;

    public static DbContextOptions<AppDbContext> DbContextOptions { get; protected set; }

    private readonly string imagemUrl = "https://static.wikia.nocookie.net/zelda/images/9/9d/Link_Artwork_1_%28Twilight_Princess%29.png/revision/latest/scale-to-width-down/213?cb=20101014225139&path-prefix=pt";

    public BaseUnitTestController()
    {
        DbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(DbContextOptions);

        SeedCategorias(context);
        SeedProdutos(context);
        context.ChangeTracker.Clear(); //desanexar entidades inseridas no contexto

        repository = new UnitOfWork(context);

        mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProdutoDTOMappingProfile>();
        }));
    }

    private List<Categoria> SeedCategorias(AppDbContext context)
    {
        if (context.Categorias.Any())
            return [.. context.Categorias.AsNoTracking()];

        var categorias = new List<Categoria>
        {
            new() { Nome = "Eletrônicos", ImagemUrl = imagemUrl },
            new() { Nome = "Saúde", ImagemUrl = imagemUrl }
        };

        context.Categorias.AddRange(categorias);
        context.SaveChanges();

        return categorias;
    }

    private void SeedProdutos(AppDbContext context)
    {
        if (context.Produtos.Any())
            return;

        var produtos = new List<Produto>
        {
            new() {
                Nome = "iPhone",
                Preco = 10.0M,
                Descricao = "Apple iPhone 15",
                Estoque = 1,
                CategoriaId = 1,
                ImagemUrl = imagemUrl,
            },
            new() {
                Nome = "Samsung Galaxy S25 Ultra",
                Preco = 20.0M,
                Descricao = "Top de linha Samsung S25 Ultra",
                Estoque = 1,
                CategoriaId = 1,
                ImagemUrl = imagemUrl,
            }
        };

        context.Produtos.AddRange(produtos);
        context.SaveChanges();
    }
}
