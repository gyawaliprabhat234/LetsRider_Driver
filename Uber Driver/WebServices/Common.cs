using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Uber_Driver.DataModels;
using Uber_Driver.Helpers;

namespace Uber_Driver.WebServices
{
    public static class Common
    {
        public static async Task<ResponseData> WebPostService(string UrlPath, string parameter, bool IsAuthorize)
        {
            try
            {
                AppData userInfo = new AppData();
                ResponseData response = new ResponseData();
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                var client = new HttpClient();
                HttpResponseMessage result;
                if (IsAuthorize)
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + userInfo.GetToken);

                if (string.IsNullOrEmpty(parameter))
                {
                    result = await client.PostAsync(LetsRideCredentials.WebUrl + UrlPath, null).ConfigureAwait(false);

                }
                else
                {
                    var content = new StringContent(parameter);
                    HttpContent cont = content;
                    cont.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    result = await client.PostAsync(LetsRideCredentials.WebUrl + UrlPath, cont).ConfigureAwait(false);
                }

                if (result.IsSuccessStatusCode)
                {
                    response = JsonConvert.DeserializeObject<ResponseData>(await result.Content.ReadAsStringAsync());
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Problem to establish connection with server";
                }
                return response;
            }
            catch (Exception ex)
            {
                return new ResponseData() { IsSuccess = false, Message = "Error! " +ex.Message };
                
            }
        }

        public static Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }
    }
}