
using Dapr.Client;
using System.Text.Json;

const string DAPR_STORE_NAME = "statestore";

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDaprClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
app.UseSwagger();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}

app.MapSubscribeHandler();
app.UseCloudEvents();

app.MapPost("/votes", async (HttpContext context) =>
{
    Console.WriteLine("Voting");
    var daprClient = context.RequestServices.GetRequiredService<DaprClient>();
    var message = await context.Request.ReadFromJsonAsync<Colour>();
    var votes = await GetCurrentVotes(daprClient, message.name);
    var newVotes = votes + 1;
    await SaveCurrentVotes(daprClient, message.name, newVotes);
    return newVotes;
}).WithTopic("pubsub", "votes").WithName("VoteColour");

Console.WriteLine("after mapPost votes");


app.MapGet("/votes", async (HttpContext context) =>
{
    Console.WriteLine("Getting votes");
    var colours = await context.Request.ReadFromJsonAsync<Colour[]>();
    var daprClient = context.RequestServices.GetRequiredService<DaprClient>();
    var colournames = new List<string>();
    foreach (var colour in colours)
    {
        colournames.Add(colour.name);
    }
    var votes = await GetCurrentVotess(daprClient, colournames.ToArray());
    await context.Response.WriteAsJsonAsync(votes);
}).WithName("GetVotes").WithOpenApi();

app.Run();

async Task<int> GetCurrentVotes(DaprClient client, string key)
{
    try {
        var votes = await client.GetStateAsync<Votes>(DAPR_STORE_NAME, key);
        
        if (votes == null) {
            Console.WriteLine("votes is null");
        } else {
            Console.WriteLine("votes is not null");
        }
        if (votes == null) {
            Console.WriteLine($"No votes found for key  {key}");
            return 0;
        }
        Console.WriteLine($"Current votes for {key}: {votes.Count}");
        return votes.Count;
    } catch (Exception e) {
        Console.WriteLine(e);
    }
    return 0;
}

async Task<Dictionary<string, int>> GetCurrentVotess(DaprClient client, string[] keys)
{
    Dictionary<string, int> votes = new();
    try {
        foreach (string colourname in keys) {
            var count = await GetCurrentVotes(client, colourname);
            votes.Add(colourname, count);
        }
    } catch (Exception e) {
        Console.WriteLine(e);
    }
    Console.WriteLine("Returning votes");
    Console.WriteLine(votes);
    return votes;
}

async Task SaveCurrentVotes(DaprClient client, string key, int count)
{   
    try {
        var vote = new Votes(key, count);
        Console.WriteLine($"Saving votes for {key}: {count}");
        Console.WriteLine(vote);
        await client.SaveStateAsync(DAPR_STORE_NAME, key, vote);
    } catch (Exception e) {
        Console.WriteLine(e);
    }
}

record Colour(string name);
record Votes(string Name, int Count);