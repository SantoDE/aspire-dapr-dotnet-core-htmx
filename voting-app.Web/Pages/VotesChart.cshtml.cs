using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace voting_app.Web.Pages
{
    [IgnoreAntiforgeryToken]
    public class VotesChartModel: PageModel 
    {
        private readonly ApiClient _apiClient;
        public List<ColourVotes> Votes { get; private set; }
        public VotesChartModel(ApiClient apiClient)
        {
            Console.WriteLine("VotesChartModel");
            _apiClient = apiClient;
        }
        public async Task OnGet()
        {
            Console.WriteLine("In OnGet Chart");
            var colours = await _apiClient.GetColourAsync();
            var counts = await _apiClient.GetColourVotesCount(colours);
            Votes = counts.ToList();
        }
    }
}

public record ColourVotes(string Name, int Count);