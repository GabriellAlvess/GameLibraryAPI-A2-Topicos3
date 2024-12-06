using GameLibraryAPI.Persistence;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configura o DbContext para usar um banco de dados em mem�ria
builder.Services.AddDbContext<GamesLibraryDbContext>(options =>
    options.UseInMemoryDatabase("GameLibraryDb"));

// Configura��o da chave secreta para JWT
var secretKey = "ChaveSecretaParaDidatica12345"; // Chave simples para fins did�ticos
var key = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Use true em produ��o
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false, // Opcional, defina se necess�rio
        ValidateAudience = false, // Opcional, defina se necess�rio
        ValidateLifetime = true
    };
});

builder.Services.AddControllers();

// Configura��o do Swagger com autentica��o JWT
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
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o Token JWT no campo abaixo",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
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

app.UseAuthentication(); // Adiciona o middleware de autentica��o
app.UseAuthorization();

app.MapControllers();

app.Run();
