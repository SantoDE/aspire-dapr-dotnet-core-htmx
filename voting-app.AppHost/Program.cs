using Aspire.Hosting.Dapr;
var builder = DistributedApplication.CreateBuilder(args);

var stateStore = builder.AddDaprStateStore("statestore");
var pubSub = builder.AddDaprPubSub("pubsub");

DaprSidecarOptions sidecarOptionsVote = new()
{
    EnableApiLogging = true,
    LogLevel = "DEBUG",
    AppId = "votes"
};

DaprSidecarOptions sidecarOptionsColours = new()
{
    EnableApiLogging = true,
    LogLevel = "DEBUG",
    AppId = "colours"
};

DaprSidecarOptions sidecarOptionWeb = new()
{
    EnableApiLogging = true,
    LogLevel = "DEBUG",
    AppId = "web"
};

var voteApi = builder.AddProject<Projects.voting_app_VotesService>("voteservice")
    .WithDaprSidecar(sidecarOptionsVote)
    .WithReference(stateStore)
    .WithReference(pubSub)
    .WithExternalHttpEndpoints();

var colourApi = builder.AddProject<Projects.voting_app_ColourService>("colourservice")
    .WithExternalHttpEndpoints()
    .WithDaprSidecar(sidecarOptionsColours)
    .WithReference(pubSub)
    .WithReference(voteApi);

var frontend = builder.AddProject<Projects.voting_app_Web>("web")
    .WithExternalHttpEndpoints()
    .WithDaprSidecar(sidecarOptionWeb);

builder.Build().Run();
