using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace ProductReviewModeration
{
    public static class ProductReviewModeration
    {
        [FunctionName("Moderation")]
        public static async Task Run([BlobTrigger("productreviewimages/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
            
            //  Function to Moderate the Image added          
            bool result = await CallContentModeratorAPI(myBlob,name);   

            //Log Response
            log.LogInformation("Product Review Image - " + (result ? "Approved" : "Denied"));
        }

       
        public static string GetConentType(string fileName)
        {
            string name = fileName.ToLower();
            string contentType = "image/jpeg";
            if (name.EndsWith("png"))
            {
                contentType = "image/png";
            }
            else if (name.EndsWith("gif"))
            {
                contentType = "image/gif";
            }
            else if (name.EndsWith("bmp"))
            {
                contentType = "image/bmp";
            }
            return contentType;
        }
        static async Task<bool> CallContentModeratorAPI(Stream image,string name)
        {
            string contentType = GetConentType(name);
            using (var memoryStream = new MemoryStream())
            {
                image.Position = 0;
                image.CopyTo(memoryStream);
                memoryStream.Position = 0;

                using (var client = new HttpClient())
                {
                    var content = new StreamContent(memoryStream);
                    var url = Environment.GetEnvironmentVariable("Moderator_API_Endpoint");
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Environment.GetEnvironmentVariable("Moderator_API_Subscription_Key"));
                    content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                    var httpResponse = await client.PostAsync(url, content);

                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        Task<string> task = Task.Run<string>(async () => await httpResponse.Content.ReadAsStringAsync());
                        string result = task.Result;

                        if (String.IsNullOrEmpty(result))
                        {
                            return false;
                        }
                        else
                        {
                            dynamic json = JValue.Parse(result);
                            return (!((bool)json.IsImageAdultClassified || (bool)json.IsImageRacyClassified));
                        }

                    }
                }
            }

            return false;
        }

        
    }


}
