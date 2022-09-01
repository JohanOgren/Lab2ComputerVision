using System;
using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.IO;

namespace Lab2ComputerVision
{
    internal class Program
    {
        private static ComputerVisionClient cvClient;
        static string key;
        static string endpoint;

        static async Task Main(string[] args)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();
            endpoint = configuration["endpoint"];
            key = configuration["key"];

            string imageFile = "C:\\Users\\me-na\\source\\repos\\Lab2ComputerVision\\images\\bike.jpg";
            
            if (args.Length > 0)
            {
                imageFile = args[0];
            }
            ApiKeyServiceClientCredentials credentials = new ApiKeyServiceClientCredentials(key);
            cvClient = new ComputerVisionClient(credentials)
            {
                Endpoint = endpoint
            };
            await AnalyzeImage(imageFile);
            Console.ReadKey();

        }

        static async Task AnalyzeImage(string imageFile)
        {
            Console.WriteLine($"Analyserar bilden du har skickat in....{imageFile}");
            List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>()
            { VisualFeatureTypes.Description,
            VisualFeatureTypes.Tags,
            VisualFeatureTypes.Adult
            };
            using (var imageData = File.OpenRead(imageFile))
            {
                var analysis = await cvClient.AnalyzeImageInStreamAsync(imageData, features);
                foreach (var caption in analysis.Description.Captions)
                {
                    Console.WriteLine($"Description: {caption.Text} (confidence:{caption.Confidence.ToString("P")})");
                }

                if (analysis.Tags.Count > 0)
                {
                    Console.WriteLine("Tags:");
                    foreach (var tag in analysis.Tags)
                    {
                        Console.WriteLine($" -{tag.Name} (confidence:{tag.Confidence.ToString("P")})");
                    }
                }
                string ratings = $"Ratings:\n -Adult: {analysis.Adult.IsAdultContent}\n -Racy:{analysis.Adult.IsRacyContent}\n -Gore: {analysis.Adult.IsGoryContent}";
                Console.WriteLine(ratings);

            }
        }
    }
}
