using System;
using System.IO;
using System.Threading.Tasks;
using az_blur_faces_af01.Interfaces;
using az_blur_faces_af01.Models;
using ImageProcessorCore;

namespace az_blur_faces_af01.Helpers
{
    public class ImageModifier : IImageModifier
    {

        public ImageModel BlurFaces(ImageModel image)
        {
            using(MemoryStream ms = new MemoryStream(
                Convert.FromBase64String(image.Data)
            ))
            {
                var imageToProcess = new ImageProcessorCore.Image<Color, uint>(ms);

                foreach(var faceRectangle in image.FaceRectangles)
                {
                    imageToProcess.BoxBlur(
                        20, new Rectangle(
                        faceRectangle.Left,
                        faceRectangle.Top,
                        faceRectangle.Width,
                        faceRectangle.Height
                        )
                    );

                }
                
                using(MemoryStream processedImage = new MemoryStream())
                {
                    imageToProcess.Save(processedImage);
                    processedImage.Position = 0;
                    image.Data = Convert.ToBase64String(processedImage.ToArray());

                }
                
                

            }
            return image;
   
        }
    }
}