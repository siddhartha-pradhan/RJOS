using Application.Interfaces.Services;
using Data.Dependency;
using Firebase.Auth;
using Firebase.Auth.Providers;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.ResponseCompression;
using Newtonsoft.Json;
using RJOS.Middlewares;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

var configuration = builder.Configuration;

services.AddInfrastructureService(configuration);

services.AddControllersWithViews();

services.AddSession(); 

services.AddCors();

services.AddRazorPages();

services.AddSwaggerGen();

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

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

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