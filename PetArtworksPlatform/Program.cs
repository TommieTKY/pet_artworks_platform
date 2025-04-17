using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetArtworksPlatform.Controllers;
using PetArtworksPlatform.Data;
using Ganss.Xss;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

builder.Services.AddEndpointsApiExplorer();

IHtmlSanitizer sanitizer = new HtmlSanitizer();
builder.Services.AddSingleton<IHtmlSanitizer>(sanitizer);

builder.Services.AddSwaggerGen();

// Register Controller
builder.Services.AddScoped<ArtistsController>();
builder.Services.AddScoped<ArtworksController>();
builder.Services.AddScoped<ExhibitionsController>();
builder.Services.AddScoped<ConnectionController>();
builder.Services.AddScoped<MemberController>();
builder.Services.AddScoped<PetController>();
builder.Services.AddScoped<ArtistProfileController>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PetArtworksPlatform API v1");
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
