namespace voting_app.Web;

using Dapr.Client;

public class ApiClient
{
    private readonly DaprClient _client;

    public ApiClient(DaprClient client)
    {
        _client = client;
    }
    public async Task<Colour[]> GetColourAsync(CancellationToken cancellationToken = default)
    {
        var req = _client.CreateInvokeMethodRequest(HttpMethod.Get, "colours", "/");
        var response = await _client.InvokeMethodWithResponseAsync(req);
        var colours = await response.Content.ReadFromJsonAsync<Colour[]>();
        Console.WriteLine(colours.Length);
        return colours;
    }

    public async Task VoteColourAsync(Colour colour, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Sending shit");
        Console.WriteLine(colour);
        CancellationTokenSource source = new CancellationTokenSource();
        cancellationToken = source.Token;
        await _client.PublishEventAsync("pubsub", "votes", colour, cancellationToken);
    }
    
    public async Task VoteColourAsync(String colour, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Sending shit from string");
        Console.WriteLine("Passed string is " + colour);
        var c = new Colour(colour);
        Console.WriteLine(c);
        CancellationTokenSource source = new CancellationTokenSource();
        cancellationToken = source.Token;
        await _client.PublishEventAsync("pubsub", "votes", c, cancellationToken);
    }

    public async Task<ColourVotes[]> GetColourVotesCount(Colour[] colours, CancellationToken cancellationToken = default)
    {
        var req = _client.CreateInvokeMethodRequest(HttpMethod.Get, "votes", "/votes", colours);
        var response = await _client.InvokeMethodWithResponseAsync(req);
        var colourDictionary = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
        var colourVotesArray = colourDictionary.Select(kvp => new ColourVotes(kvp.Key, kvp.Value)).ToArray();
        return colourVotesArray;
    }
    
    
}
public record Colour(string Name);
public record ColourVotes(string Name, int Count);