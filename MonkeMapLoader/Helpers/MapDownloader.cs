using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using VmodMonkeMapLoader.Models.Downloader;
using System.IO;

namespace VmodMonkeMapLoader.Helpers
{
    public static class MapDownloader
    {
        static HttpClient client = new HttpClient();
        static WebClient webClient = new WebClient();
        public static async void GetMaps()
        {
            try
            {
                StringContent httpContent = new StringContent("{\"paginationInfo\":{\"pageSize\":100,\"pageNumber\":0,\"orderBy\":3,\"isDescending\":true},\"onlyVerified\":true}", System.Text.Encoding.UTF8, "application/json"); // make this paginate properly

                HttpResponseMessage response = await client.PostAsync("https://monkemaphub.com/api/maps", httpContent);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                MapResponse mapResponse = JsonConvert.DeserializeObject<MapResponse>(responseBody);

                Debug.Log(mapResponse.Data.Maps.Length);
                DownloadMap(mapResponse.Data.Maps[0]);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public static async void DownloadMap(OnlineMapInfo map)
        {
            var dirPath = Path.Combine(Path.GetDirectoryName(typeof(MapFileUtils).Assembly.Location), Constants.CustomMapsFolderName, map.FileName);
            Debug.Log(dirPath);

            webClient.DownloadProgressChanged -= WebClient_DownloadProgressChanged;
            webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;

            webClient.DownloadFileCompleted -= WebClient_DownloadFileCompleted;
            webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;

            webClient.DownloadFileAsync(new Uri("https://monkemaphub.com/" + map.FileURL), dirPath);

        }

        private static void WebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Debug.Log("DONE DOWNLOADING REFRESH");
        }

        private static void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // Displays the operation identifier, and the transfer progress.
            Console.WriteLine("{0}    downloaded {1} of {2} bytes. {3} % complete...",
                (string)e.UserState,
                e.BytesReceived,
                e.TotalBytesToReceive,
                e.ProgressPercentage);
        }
    }
}
