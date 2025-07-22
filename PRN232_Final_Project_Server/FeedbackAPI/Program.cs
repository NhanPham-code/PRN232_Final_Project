using FeedbackAPI.Data;
using FeedbackAPI.Services;
using FeedbackAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using FeedbackAPI.Models;
using Microsoft.AspNetCore.OData;
using FeedbackAPI.Services.Interface;
using FeedbackAPI.Repositories.Interface;
using FeedbackAPI.DTOs;

var builder = WebApplication.CreateBuilder(args);
// 1. Build EDM model
IEdmModel GetEdmModel()
{
    var odataBuilder = new ODataConventionModelBuilder();
    odataBuilder.EntitySet<Feedback>("OdataFeedbacks");
    odataBuilder.EntityType<ReadFeedbackDTO>(); // <--- Thêm dòng này

    return odataBuilder.GetEdmModel();
}

// 2. Add OData services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers().AddOData(options =>
{
    options
        .AddRouteComponents("odata", GetEdmModel()) // route: /odata/Feedbacks
        .Select()
        .Filter()
        .OrderBy()
        .Expand()
        .Count()
        .SetMaxTop(100);
});

// Add services to the container.builder.Services.AddDbContext<….DbContext…..>(options =>
builder.Services.AddDbContext<DBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI xxxxxxat https://aka.ms/aspnetcore/swashbuckle
// Build app
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseODataRouteDebug(); // Thêm dòng này vào đây

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseRouting();
// Map controllers
app.MapControllers();


app.Run();
