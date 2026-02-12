using Microsoft.AspNetCore.Mvc;
using MindfulMomentsApp.Models;
using System.Net.Http;
using System.Text.Json;

namespace MindfulMomentsApp.Helpers
{
    public class Quotes : ViewComponent
    {
        private readonly HttpClient _httpClient;

        public Quotes()
        {
            _httpClient = new HttpClient();
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string apiUrl = "https://www.positive-api.online/phrases";
            string quoteText = "Unable to load quote.";

            try
            {
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    var quotes = JsonSerializer.Deserialize<List<QuoteModel>>(json);

                    if (quotes != null && quotes.Count > 0)
                    {
                        var random = new Random();
                        var selected = quotes[random.Next(quotes.Count)];
                        quoteText = selected.text;
                    }
                }
            }
            catch
            {
                quoteText = "Keep going. Everything you need will come to you at the perfect time.";
            }

            return View("Default", quoteText);
        }
    }
}
