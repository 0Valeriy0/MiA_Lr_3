using FluentValidation;
using MiA_Lr_3.Models;
using MiA_Lr_3.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Add validation services
builder.Services.AddScoped<IValidator<Car>, CarValidator>();
builder.Services.AddScoped<IValidator<CarContract>, ContractValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
