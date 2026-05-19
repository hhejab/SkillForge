var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("SkillForgeAPI", client =>
{
    client.BaseAddress = new Uri(
    builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7249/"
);
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Reports}/{action=Courses}/{id?}")
    .WithStaticAssets();

app.Run();