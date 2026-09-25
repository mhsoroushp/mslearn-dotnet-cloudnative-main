using Diagnostics;
using Microsoft.EntityFrameworkCore;
using Products.Data;
using Products.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ProductDataContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ProductsContext") ?? throw new InvalidOperationException("Connection string 'ProductsContext' not found.")));

// builder.Services.AddObservability("Products", builder.Configuration);

builder.Services.AddObservability("Products", builder.Configuration, ["eShopLite.Products"]);

// Register the metrics service.
builder.Services.AddSingleton<ProductsMetrics>();

// Add observability code here

// Add services to the container.
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapProductEndpoints();

app.UseStaticFiles();

app.CreateDbIfNotExists();

//This method adds the Prometheus scraping endpoint to the Products service.
app.MapObservability(); 

app.Run();
