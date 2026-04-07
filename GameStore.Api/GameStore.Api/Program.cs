using GameStore.Api.Data;
using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidation();

var connectionString = "DataSource=GameStore.db";
builder.Services.AddSqlite<GameStoreContext>(connectionString);

// var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var app = builder.Build();

app.MapGamesEndpoints();


app.Run();
