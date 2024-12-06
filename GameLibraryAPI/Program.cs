using GameLibraryAPI.Persistence;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configura o DbContext para usar um banco de dados em memória
builder.Services.AddDbContext<GamesLibraryDbContext>(options =>
    options.UseInMemoryDatabase("GameLibraryDb"));

// Configuração da chave secreta para JWT
var secretKey = "ChaveSecretaParaDidatica12345"; // Chave simples para fins didáticos
var key = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Use true em produção
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false, // Opcional, defina se necessário
        ValidateAudience = false, // Opcional, defina se necessário
        ValidateLifetime = true
    };
});

builder.Services.AddControllers();

// Configuração do Swagger com autenticação JWT
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

    //pega o nome do arquivo Assembly e armazena na varíavel
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    //pega o diretório base da aplicação e concatena com o nome do arquivo xml
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

app.UseAuthentication(); // Adiciona o middleware de autenticação
app.UseAuthorization();

app.MapControllers();

app.Run();
