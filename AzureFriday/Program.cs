using AzureFriday;
using Microsoft.Extensions.Configuration;


// Build a config object, using env vars and JSON providers.
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

// Get values from the config given their key and their target type.
Settings settings = config.GetRequiredSection("Settings").Get<Settings>();

// Azure Friday target resources
AzureFridayResourceClient azureFridayEpisodes = new();
await azureFridayEpisodes.RunAsync();

// You tube target resources 
YouTubeResourceClient youTubeClient = new(settings.ApiKey);
await youTubeClient.RunAsync();

//await Task.WhenAll(
//    azureFridayEpisodes.RunAsync(), 
//    youTubeClient.RunAsync());