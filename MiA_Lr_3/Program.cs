using FluentValidation;
using FluentValidation.AspNetCore;
using MiA_Lr_3.Mappings;
using MiA_Lr_3.Repositories;
using MiA_Lr_3.Repositories.Interface;
using MiA_Lr_3.Services;
using MiA_Lr_3.Services.Interface;
using MiA_Lr_3.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllers()
    .AddFluentValidation();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Реєструємо всі валідатори автоматично
builder.Services.AddValidatorsFromAssemblyContaining<CreateCarDtoValidator>();

// 1. Реєструємо репозиторії MongoDB
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<ICarContractRepository, CarContractRepository>();
// Реєстрація AutoMapper
builder.Services.AddAutoMapper(cfg =>{cfg.AddProfile<MappingProfile>();});
/* builder.Services.AddAutoMapper(typeof(UserProfile));  
 * Не працює?*/

// Реєстрація сервісів
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<ICarContractService, CarContractService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
