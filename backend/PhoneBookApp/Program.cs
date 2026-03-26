using Microsoft.EntityFrameworkCore;
using Npgsql;
using PhoneBookApp.Data;
using PhoneBookApp.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<INaturalLanguageInterpreter, GeminiLanguageInterpreter>();
builder.Services.AddScoped<IActionExecutorService, ActionExecutorService>();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.InitializeDatabase();

app.Run();
