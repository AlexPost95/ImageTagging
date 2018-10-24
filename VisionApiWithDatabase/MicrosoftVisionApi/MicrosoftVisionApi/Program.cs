using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MicrosoftVisionApi
{
    public class Tag
    {
        public string name { get; set; }
        public double confidence { get; set; }
    }

    public class Metadata
    {
        public int width { get; set; }
        public int height { get; set; }
        public string format { get; set; }
    }

    public class ImageObject
    {
        public List<Tag> tags { get; set; }
        public string requestId { get; set; }
        public Metadata metadata { get; set; }
    }

    static class Program
    {
        static private int imageId = 0;

        const string subscriptionKey = "31495f44467e40e5aa8b017852219356";

        const string uriBase =
            "https://westeurope.api.cognitive.microsoft.com/vision/v2.0/analyze";

        static void Main()
        {
            string[] filePaths = Directory.GetFiles(@"C:/Users/alexp/Documents/Overig/ImageTagging/VisionApiWithDatabase/ImagesToBeTagged");

            foreach (string image in filePaths)
            {
                if (File.Exists(image))
                {
                    MakeAnalysisRequest(image).Wait();
                }
                else
                {
                    Console.WriteLine("something went wrong");
                }
            }
            Console.ReadLine();
        }

        /// <summary>
        /// Gets the analysis of the specified image file by using
        /// the Computer Vision REST API.
        /// </summary>
        /// <param name="imageFilePath">The image file to analyze.</param>
        static async Task MakeAnalysisRequest(string imageFilePath)
        {
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = "Data Source=LAPTOP-OBPEB7SF;" + "Initial Catalog=ImageTagging;" + "Integrated Security=SSPI;";

            try
            {
                myConnection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            try
            {
                HttpClient client = new HttpClient();

                // Request headers.
                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", subscriptionKey);

                // Request parameters. A third optional parameter is "details".
                string requestParameters =
                    "visualFeatures=Tags";
                //"visualFeatures=Categories,Description,Color";

                // Assemble the URI for the REST API Call.
                string uri = uriBase + "?" + requestParameters;

                HttpResponseMessage response;

                // Request body. Posts a locally stored JPEG image.
                byte[] byteData = GetImageAsByteArray(imageFilePath);

                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    // This example uses content type "application/octet-stream".
                    // The other content types you can use are "application/json"
                    // and "multipart/form-data".
                    content.Headers.ContentType =
                        new MediaTypeHeaderValue("application/octet-stream");

                    // Make the REST API call.
                    response = await client.PostAsync(uri, content);
                }

                // Get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();

                // Display the JSON response.
                Console.WriteLine("\nResponse:\n\n{0}\n", JToken.Parse(contentString).ToString());

                ImageObject image = JsonConvert.DeserializeObject<ImageObject>(contentString);
                SqlCommand insertImageCommand = new SqlCommand($"INSERT INTO Image (ID, Picture) Values ('{imageId}', '{imageFilePath}')", myConnection);
                insertImageCommand.ExecuteNonQuery();

                foreach (Tag tag in image.tags)
                {
                    Console.WriteLine(tag.name);
                    SqlCommand insertTagCommand = new SqlCommand($"INSERT INTO Tag (PictureID, Tag) Values ('{imageId}', '{tag.name}')", myConnection);
                    insertTagCommand.ExecuteNonQuery();
                }
                imageId++;
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
            }
        }

        /// <summary>
        /// Returns the contents of the specified file as a byte array.
        /// </summary>
        /// <param name="imageFilePath">The image file to read.</param>
        /// <returns>The byte array of the image data.</returns>
        static byte[] GetImageAsByteArray(string imageFilePath)
        {

            //string someUrl = "https://wallpaperbrowse.com/media/images/3848765-wallpaper-images-download.jpg";
            //using (var webClient = new WebClient())
            //{
            //    byte[] imageBytes = webClient.DownloadData(someUrl);
            //    return imageBytes;
            //}


            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                //Console.WriteLine(binaryReader.ReadBytes((int)fileStream.Length));
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }
    }
}