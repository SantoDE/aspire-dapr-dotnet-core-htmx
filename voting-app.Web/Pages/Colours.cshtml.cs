using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace voting_app.Web.Pages
{
    [IgnoreAntiforgeryToken]
    public class ColoursModel: PageModel 
    {
        private readonly ApiClient _apiClient;
         public List<ColourVotes> Colours { get; private set; }

        public ColoursModel(ApiClient apiClient)
        {
            Console.WriteLine("ColoursModel");
            _apiClient = apiClient;  
            
        }

        public async Task OnGet()
        {
            Console.WriteLine(_apiClient);
            var colours = await _apiClient.GetColourAsync();
            var counts = await _apiClient.GetColourVotesCount(colours);
            Colours = new List<ColourVotes>(counts);
        }

        public async Task<IActionResult> OnPostVote(string colourName)
        {
            Console.WriteLine("Vote");
            Console.WriteLine(colourName);
            Colour colour = new Colour(colourName);
            await _apiClient.VoteColourAsync(colour);
            return Partial("_VoteSuccess", colour.Name);

        }
    }
}