using System.Collections.Generic;
using Newtonsoft.Json;

namespace az_blur_faces_af01.Models
{
    public class ImageModel
    {
        [JsonRequired]
        public string Url {get; set;}
        public string Data { get; set; }

        public string Name {get; set;}

        public List<FaceRectangle>  FaceRectangles {get; set;}

    }
}