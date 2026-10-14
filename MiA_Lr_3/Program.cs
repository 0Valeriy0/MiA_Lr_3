using FluentValidation;
using MiA_Lr_3.Models;
using MiA_Lr_3.Services;
using MiA_Lr_3.Services.Interface;
using MiA_Lr_3.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Реєстрація валідаторів
builder.Services.AddScoped<IValidator<Car>, CarValidator>();
builder.Services.AddScoped<IValidator<CarContract>, ContractValidator>();


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
