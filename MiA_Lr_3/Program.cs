using FluentValidation;
using FluentValidation.AspNetCore;
using MiA_Lr_3.Mappings;
using MiA_Lr_3.Repositories;
using MiA_Lr_3.Repositories.Interface;
using MiA_Lr_3.Services;
using MiA_Lr_3.Services.Interface;
using MiA_Lr_3.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using MongoDB.Driver;
using MiA_Lr_3.Data;
var builder = WebApplication.CreateBuilder(args);

// 1. Контролери та валідація
builder.Services
    .AddControllers()
    .AddFluentValidation();

builder.Services.AddValidatorsFromAssemblyContaining<CreateCarDtoValidator>();

// 2. Swagger з підтримкою JWT (кнопка Authorize)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Car Rental API",
        Version = "v1",
        Description = "Трирівнева архітектура RESTful API з JWT-авторизацією"
    });
    // Додаємо опис Bearer-схеми
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Вставте ваш JWT токен сюди (слово 'Bearer' додавати не потрібно):"
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document),
            new List<string>()
        }
    });

});

// 3. Реєстрація репозиторіїв MongoDB
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<ICarContractRepository, CarContractRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient(builder.Configuration["MongoDbSettings:ConnectionString"]));

// 4. Реєстрація AutoMapper

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

// 5. Реєстрація сервісів бізнес-логіки та токенів

builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<ICarContractService, CarContractService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// 6. Налаштування JWT Автентифікації

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]!;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero // Без додаткової затримки терміну дії
    };
});
builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Порядок важливий: спершу автентифікація, потім авторизація!

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();