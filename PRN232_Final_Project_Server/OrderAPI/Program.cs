using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using OrderAPI.Data;
using OrderAPI.DTOs;
using OrderAPI.Models;
using OrderAPI.Profiles;
using OrderAPI.Repositories;
using OrderAPI.Repositories.Interfaces;
using OrderAPI.Services;
using OrderAPI.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(OrderProfile));

// Add services to the container.

builder.Services.AddControllers()
    .AddOData(opt => opt
        .Select()
        .Filter()
        .Expand()
        .OrderBy()
        .SetMaxTop(100)
        .Count()
        .AddRouteComponents("odata", GetEdmModel())
    );

/*builder.Services.AddControllers()
    .AddOData(opt =>
    {
        opt.EnableQueryFeatures();
    });*/

/*builder.Services.AddControllers()
    .AddOData(opt =>
    {
        opt.EnableQueryFeatures(null); // Enable tất cả features
        opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(100);
    });*/


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IOrderRepo, OrderRepo>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderDetailRepo, OrderDetailRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();


    // EntitySet for Order
    var orderEntitySet = builder.EntitySet<Order>("ODataOrders");
    orderEntitySet.EntityType.HasKey(o => o.OrderID);
    orderEntitySet.EntityType.HasMany(o => o.OrderDetails); // Quan hệ 1-nhiều

    // EntitySet for OrderDetail
    var detailEntitySet = builder.EntitySet<OrderDetail>("ODataOrderDetails");
    detailEntitySet.EntityType.HasKey(d => d.OrderDetailID);
    detailEntitySet.EntityType.HasRequired(d => d.Order); // navigation ngược


    return builder.GetEdmModel();
}
