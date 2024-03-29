using Microsoft.EntityFrameworkCore;
using ProEventos.API.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

#region String de Conexão
builder.Services
.AddDbContext<AppDbContext>(
    context => context.UseSqlite(
        builder.Configuration
        .GetConnectionString("DefaultConnection")
        )
);
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.UseCors(cors => cors.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin()
);

app.MapControllers();

app.Run();
