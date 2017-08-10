using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmHelpers;
using ImageSearch.Services;
using System.Net.Http;
using Newtonsoft.Json;
using ImageSearch.Model;
using Plugin.Media;
using Acr.UserDialogs;
using Plugin.Media.Abstractions;
using ImageSearch.Model.BingSearch;
using Plugin.Connectivity;

namespace ImageSearch.ViewModel
{
    public class ImageSearchViewModel
    {
        public ObservableRangeCollection<ImageResult> Images { get; }

        public ImageSearchViewModel()
        {
            Images = new ObservableRangeCollection<ImageResult>();
        }
        
        public async Task<bool> SearchForImagesAsync(string query)
        {
            if(string.IsNullOrWhiteSpace(query))
            {
                await UserDialogs.Instance.AlertAsync("Please search for cute things");
                return false;
            }

            if(!CrossConnectivity.Current.IsConnected)
            {
                await UserDialogs.Instance.AlertAsync("On interwebs :(");
                return false;
            }

            //Bing Image API
            //var url = $"https://api.cognitive.microsoft.com/bing/v5.0/images/" + 
            //	      $"search?q={query}" +
            //		  $"&count=20&offset=0&mkt=en-us&safeSearch=Strict";

            //Google Image API
            var url =
                string.Format(
                    "https://www.googleapis.com/customsearch/v1?key={0}&cx={1}&q={2}&searchType=image&alt=json&num=10&start=1",
                    CognitiveServicesKeys.GoogleSearchApi, 
                    CognitiveServicesKeys.GoogleCx, 
                    query);

            try
            {
                //var headerKey = "Ocp-Apim-Subscription-Key";
                //var headerValue = CognitiveServicesKeys.BingSearch;

                var client = new HttpClient();
                //client.DefaultRequestHeaders.Add(
                //    headerKey, headerValue);

                var json = await client.GetStringAsync(url);

                var result = JsonConvert.DeserializeObject<Shared.Services.GoogleSearch.SearchResult>(
                    json);

                var images = result.Images.Select(i => new ImageResult
                {
                    ContextLink = i.DisplayLink,
                    FileFormat = i.Mime,
                    ImageLink = i.Link,
                    ThumbnailLink = i.Link,
                    Title = i.Title
                });

                Images.ReplaceRange(images);
               
            }
            catch (Exception ex)
            {
                await UserDialogs.Instance.AlertAsync("Something went terribly wrong, please open a ticket with support.");
                return false;
            }

			return true;
        }

        public async Task<string> AnalyzeImageAsync(string imageUrl)
        {
            var result = string.Empty;
            try
            {
                using (var client = new HttpClient())
                {
                    var stream = await client.GetStreamAsync(imageUrl);

                    var emotion = await EmotionService.GetAverageHappinessScoreAsync(stream);

                    result = EmotionService.GetHappinessMessage(emotion);
                }
            }
            catch(Exception ex)
            {
                result =  "Unable to analyze image";
            }

            //await UserDialogs.Instance.AlertAsync(result);

            //Return the percentage
            return result;
        }

        public async Task ShowImageMessage(string imageType, double percentage)
        {
            await UserDialogs.Instance.AlertAsync(string.Format("You're {0}% happy so here's one of the {1} you got, aww", percentage, imageType));
        }

        public async Task<double> TakePhotoAndAnalyzeAsync(bool useCamera = true)
        {
            string result = "Error";
            double emotion = 100.0;
            MediaFile file = null;
            IProgressDialog progress;

            try
            {
                await CrossMedia.Current.Initialize();

                if (useCamera)
                {
                    file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        Directory = "Sample",
                        Name = "face.jpg",
                        PhotoSize = PhotoSize.Medium
                    });
                }
                else
                {
                    file = await CrossMedia.Current.PickPhotoAsync();
                }
               

                if (file == null)
                    result = "No photo taken.";
                else
                {
                    emotion = await EmotionService.GetAverageHappinessScoreAsync(file.GetStream());

                    result = EmotionService.GetHappinessMessage((float)emotion);
                }
            }
            catch(Exception ex)
            {
                result =  ex.Message;
            }

            //await UserDialogs.Instance.AlertAsync(result);

            emotion = emotion * 100;
            emotion = Math.Round(emotion, 2);
            return emotion;
        }
    }
}
