using System;
using az_blur_faces_af01.Helpers;
using az_blur_faces_af01.Interfaces;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: FunctionsStartup(typeof(az_blur_faces_af01.Startup))]

namespace az_blur_faces_af01
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddTransient<IImageModifier, ImageModifier>();
        
        }
    }
}

