using ApiCatalogoMinimal.Context;
using ApiCatalogoMinimal.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogoMinimal.ApiEndpoints;

public static class CategoriasEndpoints
{
    public static void MapCategoriasEndpoints(this WebApplication app)
    {
        app.MapGet("/categorias", async (AppDbContext db) =>
            await db.Categorias.ToListAsync()).WithTags("Categorias").RequireAuthorization();

        app.MapGet("/categorias/{id:int}", async (int id, AppDbContext db) =>
        {
            return await db.Categorias.FindAsync(id)
                is Categoria categoria
                ? Results.Ok(categoria)
                : Results.NotFound($"Categoria com Id {id} não foi encontrado!");
        }).WithTags("Categorias").RequireAuthorization();

        app.MapPost("/categorias", async (Categoria categoria, AppDbContext db) =>
        {
            db.Categorias.Add(categoria);

            await db.SaveChangesAsync();

            return Results.Created($"/categorias/{categoria.CategoriaId}", categoria);
        }).WithTags("Categorias").RequireAuthorization();

        app.MapPut("/categorias/{id:int}", async (int id, Categoria categoria, AppDbContext db) =>
        {
            if (categoria.CategoriaId != id)
            {
                return Results.BadRequest();
            }

            var categoriaDb = await db.Categorias.FindAsync(id);

            if (categoriaDb is null)
            {
                return Results.NotFound($"Categoria com Id {id} não foi encontrado!");
            }

            categoriaDb.Nome = categoria.Nome;
            categoriaDb.Descricao = categoria.Descricao;

            await db.SaveChangesAsync();

            return Results.Ok(categoriaDb);
        }).WithTags("Categorias").RequireAuthorization();

        app.MapDelete("/categorias/{id:int}", async (int id, AppDbContext db) =>
        {
            var categoria = await db.Categorias.FindAsync(id);

            if (categoria is null)
            {
                return Results.NotFound($"Categoria com Id {id} não foi encontrado!");
            }

            db.Categorias.Remove(categoria);

            await db.SaveChangesAsync();

            return Results.NoContent();
        }).WithTags("Categorias").RequireAuthorization();
    }
}