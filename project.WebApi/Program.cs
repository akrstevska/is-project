using Microsoft.EntityFrameworkCore;
using project.Data;
using project.Data.Interfaces;
using project.Data.Repositories;
using project.Service.Interfaces;
using project.Service.Profiles;
using project.Service.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<ProjectContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ProjectContext")));

builder.Services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<CategoryProfile>();
            cfg.AddProfile<ProductProfile>();
        });


builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();


builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
