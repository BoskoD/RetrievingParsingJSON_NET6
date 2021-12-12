using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Nodes;


/// <summary>
/// Main url for the episodes
/// </summary>
var mainUrl = "/api/hierarchy/shows/azure-friday/episodes?page={0}&pageSize=30&orderBy=uploaddate%20desc";
List<string> episodeEntryIds = new(); // store episode ids
HttpClient client = new();
client.BaseAddress = new Uri("https://docs.microsoft.com");
int pageNum = 0;
int pageSize = 30;

List<string> essentials = new();

//var totalCount = (int?)jsonObject["totalCount"].AsValue(); //349 episodes

for (int i = 0; i < pageSize; i++)
{
    string jsonString = await client.GetStringAsync(String.Format(mainUrl, pageNum));
    var jsonObject = JsonNode.Parse(jsonString);

    foreach (JsonObject item in jsonObject["episodes"].AsArray())
    {
        var show = JsonSerializer.Deserialize<AzureFridayShow>(item);
        episodeEntryIds.Add(show.entryId);
    }
    i++;
    pageNum++;
}


/// <summary>
/// Episode metadata
/// </summary>
var episodeUrl = "/api/video/public/v1/entries/{0}";

foreach (var item in episodeEntryIds)
{
    string episodes = await client.GetStringAsync(string.Format(episodeUrl, item));
    var episodeMetadataObject = JsonNode.Parse(episodes);
    var videos = JsonSerializer.Deserialize<AzuerEpisodeMedia>(episodeMetadataObject);
    var essentialMetadataInfo = $"{ videos.title}|{ videos.publicVideo["mediumQualityVideoUrl"]}";
    essentials.Add(essentialMetadataInfo);
    Console.WriteLine($"Title: {videos.title}\n Video: {videos.publicVideo["mediumQualityVideoUrl"]}");
}


// Set a variable to the Documents path.
string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

// Write the string array to a new file named "AzureResources.txt".
using (StreamWriter outputFile = new(Path.Combine(docPath, "AzureResources.txt")))
{
    foreach (string line in essentials)
        outputFile.WriteLine($"{line}\n");
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
