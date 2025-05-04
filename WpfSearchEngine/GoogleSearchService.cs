using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Markup;
using Newtonsoft.Json;

public class GoogleSearchService
{
    private readonly string _apiKey;
    private readonly string _searchEngineId;
    private readonly HttpClient _httpClient;

    public GoogleSearchService(string apiKey, string searchEngineId)
    {
        _apiKey = apiKey;
        _searchEngineId = searchEngineId;
        _httpClient = new HttpClient();
    }

    public async Task<List<SearchResult>> SearchAsync(string query, int totalResults)
    {
        var results = new List<SearchResult>();
        int maxPerRequest = 10; // حداکثر نتایج در هر درخواست
        int remainingResults = totalResults;

        try
        {
            for (int startIndex = 1; remainingResults > 0; startIndex += maxPerRequest)
            {
                int resultsThisRequest = Math.Min(maxPerRequest, remainingResults);

               
                var url = $"https://www.googleapis.com/customsearch/v1?" +
                    $"key={_apiKey}&cx={_searchEngineId}&" +
                    $"q={WebUtility.UrlEncode(query)}&" +
                    $"num={resultsThisRequest}&start={startIndex}";

                using (var httpClient = new HttpClient())
                {
                    var response = await _httpClient.GetStringAsync(url);
                    var data = JsonConvert.DeserializeObject<GoogleSearchResponse>(response);

                    foreach (var item in data.Items)
                    {
                        results.Add(new SearchResult
                        {
                            Title = item.Title,
                            Url = item.Link,
                            Description = item.Snippet
                        });
                    }
                    remainingResults -= data.Items.Count;

                    // اگر نتایج کمتری از حد انتظار برگردانده شد
                    if (data.Items.Count < resultsThisRequest)
                    {
                        break;
                    }
                }


                // تاخیر بین درخواست‌ها برای جلوگیری از محدودیت Rate Limit
                await Task.Delay(2000);
            }
              

           
        }
        catch (Exception ex)
        {
            // Log error
            Console.WriteLine($"Error in Google Search: {ex.Message}");
        }

        return results;
    }
}

public class GoogleSearchResponse
{
    public List<GoogleSearchItem> Items { get; set; }
}

public class GoogleSearchItem
{
    public string Title { get; set; }
    public string Link { get; set; }
    public string Snippet { get; set; }
}

public class SearchResult
{
    public string Title { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }
}