using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web.Configuration;
using Newtonsoft.Json;

namespace AzureXY.Models
{
    public class BingSearcher
    {

        public static BingSearchViewModel ImageSearch(string query, string callback)
        {
            string accountKey = WebConfigurationManager.AppSettings["BingKey"];
            string rootUrl = "https://api.datamarket.azure.com/Bing/Search";
            var bingContainer = new Bing.BingSearchContainer(new Uri(rootUrl));
            bingContainer.Credentials = new NetworkCredential(accountKey, accountKey);

            var imageQuery = bingContainer.Image(query, null, null, null, null, null, null);
            imageQuery = imageQuery.AddQueryOption("$top", 12);
            var imageResults = imageQuery.Execute();

            var model = new BingSearchViewModel();
            if (callback != null && callback.Length > 0)
            {
                model.Callback = callback;
            }
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
            Callback = null;
        }

        public List<BingSearchImageResult> Results { get; set; }
        public string Callback { get; set; }
        public string JSON
        {
            get
            {
                return JsonConvert.SerializeObject(Results);
            }
        }
        public string JSONP
        {
            get
            {
                var str = JsonConvert.SerializeObject(Results);
                return Callback + "(" + str + ")";
            }
        }

    }

    public class BingSearchImageResult
    {
        public string ThumbnailURL { get; set; }
        public int? ThumbnailWidth { get; set; }
        public int? ThumbnailHeight { get; set; }
        public string ImageURL { get; set; }
    }
}
