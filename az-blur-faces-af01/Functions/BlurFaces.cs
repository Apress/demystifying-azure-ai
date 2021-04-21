using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;
using az_blur_faces_af01.Interfaces;
using Newtonsoft.Json;
using az_blur_faces_af01.Models;
using System.Net.Http;

namespace BlurFaces.Functions
{
    public class BlurFaces
    {
        private readonly IImageModifier _imageModifier;

        private readonly HttpClient _httpClient;
        public BlurFaces(IImageModifier imageModifier, HttpClient httpClient)
        {
            _imageModifier = imageModifier ?? throw new ArgumentException(nameof(imageModifier));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        [FunctionName(nameof(BlurFaces))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
            {
                string request = await new StreamReader(req.Body).ReadToEndAsync();

                var imageToProcess = JsonConvert.DeserializeObject<ImageModel>(request);

                //Retrieve Image base 64 string
                imageToProcess.Data = Convert.ToBase64String(await _httpClient.GetByteArrayAsync(imageToProcess.Url));

                imageToProcess.Name = Path.GetFileName(imageToProcess.Url);

                ImageModel processedImage;

                try
                {
                    processedImage = _imageModifier.BlurFaces(imageToProcess);

                    return new ContentResult
                    {
                        Content = JsonConvert.SerializeObject(processedImage),
                        ContentType = "application/json",
                        StatusCode = 200
                    };
                    
                }
                catch(Exception ex)
                {
                    log.LogError(ex.ToString());

                    return new ContentResult
                    {
                        StatusCode = 502,
                        Content = JsonConvert.SerializeObject(imageToProcess),
                        ContentType = "application/json"
                        
                    };

                }


            }
    }
}