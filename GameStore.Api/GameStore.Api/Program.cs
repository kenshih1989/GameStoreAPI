using GameStore.Api.Data;
using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidation();

builder.AddGameStoreDbContext();

var app = builder.Build();

app.MapGamesEndpoints();

app.MigrateDatabase();

app.Run();
