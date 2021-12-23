using System.Text.Json;
using System.Text.Json.Nodes;


namespace AzureFriday
{
    public class AzureFridayResourceClient
    {
        public string MainUrl { get; set; }
        public string EpisodeUrl { get; set; }
        public HttpClient Client { get; set; }

        readonly List<string> EpisodeEntryIds = new();
        readonly List<string> Essentials = new();


        public AzureFridayResourceClient()
        {
            MainUrl = "/api/hierarchy/shows/azure-friday/episodes?page={0}&pageSize=30&orderBy=uploaddate%20desc"; // mainUrl used to pull data
            EpisodeUrl = "/api/video/public/v1/entries/{0}"; // episode metadata endpoint
            Client = new HttpClient
            {
                BaseAddress = new Uri("https://docs.microsoft.com")
            };
        }


        public async Task GetEntryIds()
        {
            int pageNum = 0;
            int pageSize = 30;

            /// <summary>
            /// Retrieve and parse episode ids and populate 
            /// <see cref="episodeEntryIds"/>
            /// </summary>
            for (int i = 0; i < pageSize; i++)
            {
                string jsonString = await Client.GetStringAsync(String.Format(MainUrl, pageNum));
                var jsonObject = JsonNode.Parse(jsonString);
                EpisodeEntryIds.AddRange(from JsonObject? item in jsonObject["episodes"].AsArray()
                                         let show = JsonSerializer.Deserialize<AzureFridayShow>(item)
                                         select show.entryId);
                i++;
                pageNum++;
            }
        }

        public async Task GetEpisodes(List<string> episodeIds)
        {
            /// <summary>
            /// Get episode ids and pull episode metadata to populate episode video list 
            /// <see cref="essentials"/>
            /// </summary>
            foreach (var item in episodeIds)
            {
                string episodes = await Client.GetStringAsync(string.Format(EpisodeUrl, item));
                var episodeMetadataObject = JsonNode.Parse(episodes);
                var videos = JsonSerializer.Deserialize<AzuerEpisodeMedia>(episodeMetadataObject);
                var essentialMetadataInfo = $"{ videos.title}|{ videos.publicVideo["mediumQualityVideoUrl"]}";
                Essentials.Add(essentialMetadataInfo);
                Console.WriteLine($"Title: {videos.title}\nVideo: {videos.publicVideo["mediumQualityVideoUrl"]}");
            }
        }

        private static void ExportToExcel(List<string> essentials)
        {
            // Set a variable to the Documents path.
            string docFullPath = Path.Combine(Environment.GetFolderPath
                (Environment.SpecialFolder.MyDocuments), "AzureResources.txt");

            if (!File.Exists(docFullPath))
            {
                using StreamWriter sw = File.CreateText(docFullPath);
            }
                
            // Write the string array to a new file named "AzureResources.txt".
            using StreamWriter outputFile = new(docFullPath);
            foreach (string line in essentials)
                outputFile.WriteLine($"{line}\n");
        }

        public async Task RunAsync()
        {
            await GetEntryIds();
            await GetEpisodes(EpisodeEntryIds);
            ExportToExcel(Essentials);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine("Azure pull completed!");
            Console.ResetColor();
            Console.WriteLine();
        }
    }


    public record AzuerEpisodeMedia
    {
        public string title { get; set; }
        public JsonObject publicVideo { get; set; }
    }

    public record AzureFridayShow
    {
        public string title { get; init; }
        public string url { get; init; }
        public string description { get; init; }
        public string entryId { get; init; }
    }
}
