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

            string imageFile1 = "C:\\Users\\me-na\\source\\repos\\Lab2ComputerVision\\images\\bike.jpg";
            string imageFile2 = "C:\\Users\\me-na\\source\\repos\\Lab2ComputerVision\\images\\buss.jpg";
            string imageFile3 = "C:\\Users\\me-na\\source\\repos\\Lab2ComputerVision\\images\\simulator.jpg";

            var loopiloop = true;
            while (loopiloop)
            {

                Console.WriteLine("\nWant image do you want to see? 1-2 or 3..write exit to quit program.");
                var action = Console.ReadLine();
                if (action == "1")
                {
                    if (args.Length > 0)
                    {
                        imageFile1 = args[0];
                    }
                    ApiKeyServiceClientCredentials credentials = new ApiKeyServiceClientCredentials(key);
                    cvClient = new ComputerVisionClient(credentials)
                    {
                        Endpoint = endpoint
                    };
                    await AnalyzeImage(imageFile1);
                    await GetThumbnail(imageFile1);

                }
                else if (action == "2")
                {
                    if (args.Length > 0)
                    {
                        imageFile2 = args[0];
                    }
                    ApiKeyServiceClientCredentials credentials = new ApiKeyServiceClientCredentials(key);
                    cvClient = new ComputerVisionClient(credentials)
                    {
                        Endpoint = endpoint
                    };
                    await AnalyzeImage(imageFile2);
                    await GetThumbnail(imageFile2);

                }
                else if (action == "3")
                {
                    if (args.Length > 0)
                    {
                        imageFile3 = args[0];
                    }
                    ApiKeyServiceClientCredentials credentials = new ApiKeyServiceClientCredentials(key);
                    cvClient = new ComputerVisionClient(credentials)
                    {
                        Endpoint = endpoint
                    };
                    await AnalyzeImage(imageFile3);
                    await GetThumbnail(imageFile3);

                }
                else if (action == "exit")
                {
                    loopiloop = false;
                }
            }

            

            static async Task AnalyzeImage(string imageFile)
            {
                Console.WriteLine($"\nAnalyserar bilden du har skickat in....{imageFile}");
                List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>()
            { VisualFeatureTypes.Description,
            VisualFeatureTypes.Tags,
            VisualFeatureTypes.Adult
            };
                using (var imageData = File.OpenRead(imageFile))
                {
                    var analysis = await cvClient.AnalyzeImageInStreamAsync(imageData, features);
                    // get image captions
                    foreach (var caption in analysis.Description.Captions)
                    {
                        Console.WriteLine($"\nDescription: {caption.Text} (confidence:{caption.Confidence.ToString("P")})");
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
            static async Task GetThumbnail(string imageFile)
            {
                Console.WriteLine("Generating thumbnail");

                using (var imageData = File.OpenRead(imageFile))
                {
                    var thumbnailStream = await cvClient.GenerateThumbnailInStreamAsync(100,
                   100, imageData, true);
                    string thumbnailFileName = "C:\\Users\\me-na\\source\\repos\\Lab2ComputerVision\\thumbnail\\thumbnail.jpg";
                    using (Stream thumbnailFile = File.Create(thumbnailFileName))
                    {
                        thumbnailStream.CopyTo(thumbnailFile);
                    }
                    Console.WriteLine($"Thumbnail saved in {thumbnailFileName}");
                }
            }
        }
    }
}
