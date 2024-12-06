using GameLibraryAPI.Persistence;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Configura o DbContext para usar um banco de dados em mem�ria
builder.Services.AddDbContext<GamesLibraryDbContext>(options =>
    options.UseInMemoryDatabase("GameLibraryDb")); ;

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GameLibraryAPI",
        Description = "API para gerenciamento de biblioteca de jogos",
        Contact = new OpenApiContact
        {
            Name = "Gabriel Alves",
            Email = "gabrielalves@unitins.br",
            Url = new Uri("https://unitins.br"),
        }
    });

    //pega o nome do arquivo Assembly e armazena na var�avel
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    //pega o diret�rio base da aplica��o e concatena com o nome do arquivo xml
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
