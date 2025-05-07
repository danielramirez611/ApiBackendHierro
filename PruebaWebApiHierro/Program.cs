using PruebaWebApiHierro.Data;
using Microsoft.EntityFrameworkCore;
using PruebaWebApiHierro.Services;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization; // 👈 agrega este using arriba


var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddScoped<ReniecService>();
builder.Services.AddHttpClient(); // Asegúrate de tener esto
builder.Services.AddSingleton<TwilioVerifyService>();
builder.Services.AddSingleton<ReniecService>();
builder.Services.AddSingleton<SmsService>(); // <-- AGREGA ESTA LÍNEA

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// 👉 Agregar la conexión a SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; // ✅ evita ciclos

}); builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
