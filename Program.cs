using System.Globalization;
using System.Reflection;
using System.Text;
using HospitaAppointmentSystem.Data;
using HospitaAppointmentSystem.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using static HospitaAppointmentSystem.Services.LanguageService;

var builder = WebApplication.CreateBuilder(args);



#region Localizer
builder.Services.AddSingleton<LanguageService>();
builder.Services.AddLocalization(options=>options.ResourcesPath="Resources");
builder.Services.AddMvc().AddViewLocalization().AddDataAnnotationsLocalization(options=>
options.DataAnnotationLocalizerProvider =(type,factory)=>
{
    var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName);
    return factory.Create(nameof(SharedResource),assemblyName.Name);
});
builder.Services.Configure<RequestLocalizationOptions>(options=>
{
    var supportCultures = new List<CultureInfo>
    {
        new CultureInfo("en-US"),
        new CultureInfo("fr-FR"),
        new CultureInfo("tr-TR")
    };
    options.DefaultRequestCulture=new RequestCulture(culture:"tr-TR");
    options.SupportedCultures= supportCultures;
    options.SupportedUICultures=supportCultures;
    options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
});
#endregion

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Users/SignIn";
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IDoctorRepository,DoctorRepository>();


builder.Services.AddDbContext<DataContext>(options=>{
    var config = builder.Configuration;
    var connectionString = config.GetConnectionString("HospitalDb");
    options.UseSqlServer(connectionString);
});
builder.Services.AddIdentity<AppUser,AppRole>().AddEntityFrameworkStores<DataContext>();

builder.Services.Configure<IdentityOptions>(options=>{
    options.Password.RequireDigit=false;
    options.Password.RequireLowercase=false;
    options.Password.RequireNonAlphanumeric=false;
    options.Password.RequireUppercase=false;
    options.Password.RequiredLength=1;
    options.User.AllowedUserNameCharacters="abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/ ";
});

builder.Services.ConfigureApplicationCookie(options=>{
    options.LoginPath="/Account/Login";
    options.AccessDeniedPath="/Account/AccessDenied";
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

IdentitySeedData.IdentityTestUser(app);
app.Run();
