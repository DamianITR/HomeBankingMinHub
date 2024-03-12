using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Services;
using HomeBankingMindHub.Services.Impl;
using HomeBankingMinHub.Models;
using HomeBankingMinHub.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<HomeBankingContext>(options => options
    .UseSqlServer(builder.Configuration.GetConnectionString("HomeBankingConexion")));

//repositorios
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ICardRepository, CardRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IClientLoanRepository, ClientLoanRepository>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();

//servicios
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IAccountService, AccountService>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddControllers().AddJsonOptions(x =>
x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Autenticacion
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
      .AddCookie(options =>
      {
          options.Cookie.Name = ".HomeBanking.Cookie";
          options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
          options.LoginPath = new PathString("/index.html");
      });

//Autorizacion
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ClientOnly", policy => policy.RequireClaim("Client"));
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin"));
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{

    //Aqui obtenemos todos los services registrados en la App
    var services = scope.ServiceProvider;
    try
    {

        // En este paso buscamos un service que este con la clase HomeBankingContext
        var context = services.GetRequiredService<HomeBankingContext>();
        DBInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ha ocurrido un error al enviar la información a la base de datos!");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

//app.MapRazorPages();

app.MapControllers();

app.Run();
