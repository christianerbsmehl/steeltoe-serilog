using Serilog;
using Steeltoe.Extensions.Logging.DynamicSerilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();

builder.Logging.ClearProviders();

// Works always (no Steeltoe)
//builder.Logging.AddSerilog(new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger());

// Doesn't work with Steeltoe 3.2.0
//builder.Logging.AddDynamicSerilog(new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration));

// Doesn't work either with Steeltoe 3.2.0
builder.WebHost.ConfigureLogging((hostContext, loggingBuilder) =>
    loggingBuilder.AddDynamicSerilog(new LoggerConfiguration().ReadFrom.Configuration(hostContext.Configuration)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapGet("log", async context =>
{
    var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("TestLogger");

    using (logger.BeginScope("MyScope"))
    {
        logger.LogInformation("Log should have a {value} property which value is 'value'.", "value");
    }

    await context.Response.WriteAsync("Check logs.");
});

app.Run();
