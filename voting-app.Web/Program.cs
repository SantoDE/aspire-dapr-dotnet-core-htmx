using voting_app.Web;
using voting_app.Web.Pages;
using Dapr.Client;
using Google.Api;
using voting_app.Web.Components;
using Results = voting_app.Web.Components.Results;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
//builder.Services.AddRazorPages();
builder.Services
	.AddRazorComponents();
builder.Services.AddOutputCache();
builder.Services.AddDaprClient();
builder.Services.AddSingleton<ApiClient>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

/*app.MapRazorComponents<App>()
   .AddHtmxorComponentEndpoints(app);
   */

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAntiforgery();

app.UseOutputCache();

app.MapGet("/colours", async (ApiClient client) =>
{
	var colours = await client.GetColourAsync();
	return RazorExtensions.Component<Colours>(colours);
});

app.MapPut("/colour/{name}/vote", async (string name, ApiClient client) =>
{
	await client.VoteColourAsync(name);
	var message = "Voted for colour " + name;
	return RazorExtensions.Component<SuccessMessage>(message);
});

app.MapGet("/results", async (ApiClient client) =>
{
	var colours = await client.GetColourAsync();
	var counts = await client.GetColourVotesCount(colours);
	return RazorExtensions.Component<Results>(counts);
});

app.Run();
