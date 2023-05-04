using ApiCatalogoMinimal.Context;
using ApiCatalogoMinimal.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogoMinimal.ApiEndpoints;

public static class ProdutosEndpoints
{
    public static void MapProdutosEndpoints(this WebApplication app)
    {
        app.MapGet("/produtos", async (AppDbContext db) =>
            await db.Produtos.ToListAsync()).WithTags("Produtos").RequireAuthorization();

        app.MapGet("/produtos/{id:int}", async (int id, AppDbContext db) =>
        {
            return await db.Produtos.FindAsync(id)
                is Produto produto
                ? Results.Ok(produto)
                : Results.NotFound($"Produto com Id {id} não foi encontrado!");
        }).WithTags("Produtos").RequireAuthorization();

        app.MapPost("/produtos", async (Produto produto, AppDbContext db) =>
        {
            db.Produtos.Add(produto);

            await db.SaveChangesAsync();

            return Results.Created($"/produtos/{produto.ProdutoId}", produto);
        }).WithTags("Produtos").RequireAuthorization();

        app.MapPut("/produtos/{id:int}", async (int id, Produto produto, AppDbContext db) =>
        {
            if (produto.ProdutoId != id)
            {
                return Results.BadRequest();
            }

            var produtoDb = await db.Produtos.FindAsync(id);

            if (produtoDb is null)
            {
                return Results.NotFound($"Produto com Id {id} não foi encontrado!");
            }

            produtoDb.Nome = produto.Nome;
            produtoDb.Descricao = produto.Descricao;
            produtoDb.Preco = produto.Preco;
            produtoDb.Imagem = produto.Imagem;
            produtoDb.DataCompra = produto.DataCompra;
            produtoDb.Estoque = produto.Estoque;
            produtoDb.CategoriaId = produto.CategoriaId;

            await db.SaveChangesAsync();

            return Results.Ok(produtoDb);
        }).WithTags("Produtos").RequireAuthorization();

        app.MapDelete("/produtos/{id:int}", async (int id, AppDbContext db) =>
        {
            var produto = await db.Produtos.FindAsync(id);

            if (produto is null)
            {
                return Results.NotFound($"Produto com Id {id} não foi encontrado!");
            }

            db.Produtos.Remove(produto);

            await db.SaveChangesAsync();

            return Results.NoContent();
        }).WithTags("Produtos").RequireAuthorization();
    }
}