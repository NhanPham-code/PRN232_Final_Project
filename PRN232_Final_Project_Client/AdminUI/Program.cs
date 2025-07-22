using Service.Interfaces;
using Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session tồn tại trong 30 phút
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// HttpClient
builder.Services.AddHttpClient<Service.BaseService.GatewayHttpClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7112/");
});

// Dependency Injection
builder.Services.AddScoped<Service.Interfaces.IUserService, Service.Services.UserService>();
builder.Services.AddScoped<Service.Interfaces.IProductService, Service.Services.ProuctService>();
builder.Services.AddScoped<Service.Interfaces.ICategoryService, Service.Services.CategoryService>();
builder.Services.AddScoped<Service.Interfaces.IFeedbackService, Service.Services.FeedbackService>();
builder.Services.AddScoped<Service.Interfaces.INotificationService, Service.Services.NotificationService>();
builder.Services.AddScoped<Service.Services.EmailService>();
builder.Services.AddScoped<Service.Interfaces.IOrderService, Service.Services.OrderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); 

app.UseAuthorization();

app.MapRazorPages();

app.Run();
