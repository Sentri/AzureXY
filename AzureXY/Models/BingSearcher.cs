using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web.Configuration;

namespace AzureXY.Models
{
    public class BingSearcher
    {

        public static BingSearchViewModel ImageSearch(string query)
        {
            string accountKey = WebConfigurationManager.AppSettings["BingKey"];
            string rootUrl = "https://api.datamarket.azure.com/Bing/Search";
            var bingContainer = new Bing.BingSearchContainer(new Uri(rootUrl));
            bingContainer.Credentials = new NetworkCredential(accountKey, accountKey);

            var imageQuery = bingContainer.Image(query, null, null, null, null, null, null);
            imageQuery = imageQuery.AddQueryOption("$top", 12);
            var imageResults = imageQuery.Execute();

            var model = new BingSearchViewModel();
            foreach (var result in imageResults)
            {
                model.Results.Add(new BingSearchImageResult() {
                    ImageURL = result.MediaUrl,
                    ThumbnailURL = result.Thumbnail.MediaUrl,
                    ThumbnailWidth = result.Thumbnail.Width,
                    ThumbnailHeight = result.Thumbnail.Height,
                });
            }
            return model;
        }
    }

    public class BingSearchViewModel
    {
        public BingSearchViewModel()
        {
            Results = new List<BingSearchImageResult>();
        }

        public List<BingSearchImageResult> Results { get; set; }

    }

    public class BingSearchImageResult
    {
        public string ThumbnailURL { get; set; }
        public int? ThumbnailWidth { get; set; }
        public int? ThumbnailHeight { get; set; }
        public string ImageURL { get; set; }
    }
}
