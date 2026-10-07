using FlexTravel.Api.Core;
using FlexTravel.Api.Extensions;
using FlexTravel.Api.Hubs;
using FlexTravel.Implementation;
using Microsoft.Extensions.FileProviders;
using System.Threading.RateLimiting;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var settings = new AppSettings();
builder.Configuration.Bind(settings);
settings.Validate();

builder.Services.AddSingleton(settings);
builder.Services.AddHttpContextAccessor();
builder.Services.AddApplicationUser();
builder.Services.AddJwt(settings);
builder.Services.AddFlexTravelContext(settings);
builder.Services.AddLoggers();
builder.Services.AddUseCases();
builder.Services.AddValidators();
builder.Services.AddNotifications();
builder.Services.AddWeather();
builder.Services.AddScheduledJobs();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy(FlexTravel.Api.Controllers.AuthController.SignInRateLimit, context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions { PermitLimit = 20, Window = TimeSpan.FromMinutes(1) }));
    options.OnRejected = async (context, token) =>
        await context.HttpContext.Response.WriteAsJsonAsync(
            new { message = "Too many sign-in attempts. Wait a minute and try again." }, token);
});

builder.Services.AddTransient<UseCaseHandler>();

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseMiddleware<GlobalExceptionHandler>();

var uploads = LocalFileStorage.DirectoryFor(app.Environment);
Directory.CreateDirectory(uploads);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploads),
    RequestPath = "/" + LocalFileStorage.Folder
});

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationsHub>(NotificationsHub.Path);

app.Run();
