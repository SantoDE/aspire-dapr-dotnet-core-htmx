using Dapr.Client;

const string PUBSUB_NAME = "pubsub";
const string TOPIC_NAME = "votes";

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

//app.UseHttpsRedirection();

var colours = new Colour[] {
    new Colour("blue"),
    new Colour("green"),
    new Colour("red"),
    new Colour("yellow"),
};

app.MapGet("/", () =>
{
    return Results.Ok(colours);
})
.WithName("GetColours");

app.MapPost("/vote", async (HttpContext context, Colour colour) =>
{
    CancellationTokenSource source = new CancellationTokenSource();
    CancellationToken cancellationToken = source.Token;
    var daprClient = context.RequestServices.GetRequiredService<DaprClient>();
    await daprClient.PublishEventAsync(PUBSUB_NAME, TOPIC_NAME, colour, cancellationToken);
    return Results.Ok(colour);
})
.WithName("VoteColour")
.WithOpenApi();

app.Run();

record Colour(string Name);
record BeerVoteResponse(string Name, int Count);