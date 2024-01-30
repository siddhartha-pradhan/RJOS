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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

var configuration = builder.Configuration;

services.AddControllersWithViews();

services.AddSession(); 

services.AddCors();

services.AddRazorPages();

services.AddSwaggerGen();

// services.AddSwaggerGen(c =>
// {
//     c.SwaggerDoc("v1", new OpenApiInfo
//     {
//         Title = "RSOS", 
//         Version = "v1"
//     });
//     
//     c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme ()
//     {
//         Name = "Authorization",
//         Type = SecuritySchemeType.ApiKey,
//         Scheme = JwtBearerDefaults.AuthenticationScheme,
//         BearerFormat = "JWT",
//         In = ParameterLocation.Header,
//         Description = "Enter 'Bearer' followed by a space and then your valid JWT token."
//     });
//     
//     c.AddSecurityRequirement(new OpenApiSecurityRequirement
//     {
//         {
//             new OpenApiSecurityScheme
//             {
//                 Reference = new OpenApiReference
//                 {
//                     Type = ReferenceType.SecurityScheme,
//                     Id = "Bearer"
//                 }
//             },
//             Array.Empty<string>()
//         }
//     });
// });

services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

services.AddResponseCompression(options =>
{
    options.Providers.Add<GzipCompressionProvider>();
    options.EnableForHttps = true;
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
    Providers = new FirebaseAuthProvider[]
    {
        new EmailProvider()
    }
}));

// services
//     .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.Authority = $"https://securetoken.google.com/{firebaseProjectName}";
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidIssuer = $"https://securetoken.google.com/{firebaseProjectName}",
//             ValidateAudience = true,
//             ValidAudience = firebaseProjectName,
//             ValidateLifetime = true
//         };
//     });

services.AddInfrastructureService(configuration);

var app = builder.Build();

app.UseExceptionHandler("/Home/Error");

app.UseHsts();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseMiddleware<ExceptionMiddleware>();

app.UseResponseCompression();

app.UseAuthentication();

app.UseAuthorization();

app.UseSession(); 

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializerService>();
    
    await dbInitializer.Initialize();
}

app.Run();