using System;
using System.Threading.Tasks;
using az_blur_faces_af01.Models;
using ImageProcessorCore;

namespace az_blur_faces_af01.Interfaces
{
    public interface IImageModifier
    {
        ImageModel BlurFaces(ImageModel image);
        
    }

}

