using System.Globalization;
using System.Text;
using Firebase.Auth;
using FirebaseAdmin;
using Data.Dependency;
using Newtonsoft.Json;
using RJOS.Middlewares;
using Google.Apis.Auth.OAuth2;
using Firebase.Auth.Providers;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Application.Interfaces.Services;
using DNTCaptcha.Core;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Net.Http.Headers;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

var configuration = builder.Configuration;

services.AddControllersWithViews(options =>
{
    // options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    
    var jsonInputFormatter = options.InputFormatters
        .OfType<Microsoft.AspNetCore.Mvc.Formatters.SystemTextJsonInputFormatter>()
        .Single();
    
    jsonInputFormatter.SupportedMediaTypes.Add("application/csp-report");
});

services.AddMvc(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

// services.Configure<FormOptions>(options =>
// {
//     options.MultipartBodyLengthLimit = 31457280; // Set the limit to 30 MB => 31457280 Bytes (in binary)
// });

services.AddHsts(options =>
{
    options.Preload = true;
    options.MaxAge = TimeSpan.FromDays(90);
    options.IncludeSubDomains = true;
});

services.AddSession(option => { option.IdleTimeout = TimeSpan.FromMinutes(15); });

services.AddCors();

services.AddRazorPages();

services.AddSwaggerGen();

services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RSOS",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and then your valid JWT token."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
         {
             new OpenApiSecurityScheme
             {
                 Reference = new OpenApiReference
                 {
                     Type = ReferenceType.SecurityScheme,
                     Id = "Bearer"
                 }
             },
             Array.Empty<string>()
         }
    });
});

services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

services.AddResponseCompression(options =>
{
    options.Providers.Add<GzipCompressionProvider>();
    options.EnableForHttps = true;
});

services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en-IN") 
    };
    
    options.DefaultRequestCulture = new RequestCulture("en-IN"); 
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

services.Configure<KestrelServerOptions>(options =>
{
    options.AddServerHeader = false;
});

var credentials = builder.Configuration.GetValue<string>("FIREBASE_CONFIG");

services.AddSingleton(FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromJson(credentials)
}));

var firebaseProjectName = JsonConvert.DeserializeObject<Dictionary<string, string>>(credentials)
    .Where(i => i.Key == "project_id")
    .Select(p => p.Value).FirstOrDefault();

var apiKey = builder.Configuration.GetValue<string>("API_KEY");

services.AddSingleton(new FirebaseAuthClient(new FirebaseAuthConfig
{
    ApiKey = apiKey,
    AuthDomain = $"{firebaseProjectName}.firebaseapp.com",
    Providers =
    [
        new EmailProvider()
    ]
}));

services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidAudience = configuration["JWT:Audience"],
            ValidIssuer = configuration["JWT:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"] ?? "")),
        };
    });

services.AddAuthorization();

services.AddInfrastructureService(configuration);

services.AddDNTCaptcha(options =>
{
    options.UseCookieStorageProvider()
        .ShowThousandsSeparators(false)
        .WithEncryptionKey("123456")
        .AbsoluteExpiration(minutes: 7)
        .InputNames(
            new DNTCaptchaComponent
            {
                CaptchaHiddenInputName = "DNTCaptchaText",
                CaptchaHiddenTokenName = "DNTCaptchaToken",
                CaptchaInputName = "DNTCaptchaInputText"
            })
        .Identifier("dntcaptha");
});

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

app.UseExceptionHandler("/Home/Error");

app.UseHsts();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseRouting();

app.UseResponseCompression();

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

app.UseMiddleware<ExceptionMiddleware>();

app.UseMiddleware<SecurityHeadersMiddleware>();

app.UseMiddleware<UserLogMiddleware>();

app.UseStatusCodePages();

app.MapRazorPages();

app.MapControllers();

app.UseSwagger();

app.UseSwaggerUI();

app.UseCors(policyBuilder =>
{
    policyBuilder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
});

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = (context) =>
    {
        var headers = context.Context.Response.GetTypedHeaders();
        headers.CacheControl = new CacheControlHeaderValue
        {
            Public = true,
            MaxAge = TimeSpan.FromHours(24)
        };
    }
});

// app.UseContentSecurityPolicy();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializerService>();

    await dbInitializer.Initialize();
}

app.Run();