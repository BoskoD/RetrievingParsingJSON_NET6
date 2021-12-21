using Google.Apis.Services;
using Google.Apis.YouTube.v3;


namespace AzureFriday
{
    public class YouTubeResourceClient
    {
        public YouTubeService YouTubeService { get; set; }

        public YouTubeResourceClient(string apiKey)
        {
            YouTubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = apiKey
            });
        }

        public async Task GetYouTubeResource()
        {
            var baseAddress = "https://www.youtube.com";
            var request = YouTubeService.Search.List("snippet");
            request.Q = "C# Advanced|Microservices .NET|Cloud Design Patterns|Advanced DevOps";
            request.RegionCode = "us"; //https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2
            request.MaxResults = 100;

            var result = await request.ExecuteAsync();
            foreach (var videos in result.Items)
            {
                Console.WriteLine($"Title: {videos.Snippet.Title}\nVideo: {baseAddress}/watch?v={videos.Id.VideoId}");
                Console.WriteLine();
            }
        }
        public async Task RunAsync()
        { 
            await GetYouTubeResource();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("You tube pull completed!");
            Console.ResetColor();
        }
    }
}
