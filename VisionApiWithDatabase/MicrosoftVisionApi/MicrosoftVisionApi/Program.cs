using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace MicrosoftVisionApi
{
    public class Tag
    {
        public string name { get; set; }
        public double confidence { get; set; }
        public string hint { get; set; }
    }

    public class Caption
    {
        public string text { get; set; }
        public double confidence { get; set; }
    }

    public class Description
    {
        public List<string> tags { get; set; }
        public List<Caption> captions { get; set; }
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
        public Description description { get; set; }
        public string requestId { get; set; }
        public Metadata metadata { get; set; }
    }

    static class Program
    {
        static private int imageId = 1;

        // Microsoft Computer Vision Api subscription key
        const string visionApiSubscriptionKey = "31495f44467e40e5aa8b017852219356";

        const string uriBase = "https://westeurope.api.cognitive.microsoft.com/vision/v2.0/analyze";

        static void Main()
        {
            // Upload and tag the content of an entire folder
            UploadFolder(Directory.GetFiles(@"C:/Users/alex.post/Documents/Alex Post/ImageTagging/VisionApiWithDatabase/MicrosoftVisionApi/MicrosoftVisionApi/ToTag/"));

            // Upload and tag a single file
            // UploadSingleFile("C:/Users/alex.post/Documents/Alex Post/ImageTagging/VisionApiWithDatabase/MicrosoftVisionApi/MicrosoftVisionApi/Images/Cool Collection 2 140.jpg");
        }

        /// <summary>
        /// Upload an entire folder
        /// </summary>
        /// <param name="folderPath">The path of the folder that needs to be analyzed</param>
        static void UploadFolder(string [] folderPath)
        {
            foreach (string image in folderPath)
            {
                if (File.Exists(image))
                {
                    MakeAnalysisRequest(image).Wait();

                    // Delay of 3 seconds to ensure you don't cross the free limit of 20 requests per minute
                    //Thread.Sleep(3000);
                }
                else
                {
                    Console.WriteLine("something went wrong");
                }
            }
            Console.ReadLine();
        }

        /// <summary>
        /// Upload a single file
        /// </summary>
        /// <param name="filePath">The path of the file that needs to be analyzed</param>
        static void UploadSingleFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                MakeAnalysisRequest(filePath).Wait();
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Gets the analysis of the specified image file by using the Computer Vision REST API.
        /// </summary>
        /// <param name="imageFilePath">The image file to analyze.</param>
        static async Task MakeAnalysisRequest(string imageFilePath)
        {
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = "Data Source=DESKTOP-SMB5I56;" + "Initial Catalog=ImageTagging;" + "Integrated Security=SSPI;";

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

                // Request headers
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", visionApiSubscriptionKey);

                bool description = false;

                //Supported options for visualFeatures: Tags, Description
                //Depending on the options selected the request will be executed with or without Description. The Tag option is mandatory, Description is optional
                string requestParameters = "visualFeatures=Tags, Description";
                
                if (requestParameters.Contains("Description"))
                {
                    description = true;
                }

                // Assemble the URI for the REST API Call.
                string uri = uriBase + "?" + requestParameters;

                HttpResponseMessage response;

                // Request body. Posts a locally stored JPEG image.
                byte[] byteData = GetImageAsByteArray(imageFilePath);

                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
 
                    content.Headers.ContentType =
                        new MediaTypeHeaderValue("application/octet-stream");

                    // Make the REST API call
                    response = await client.PostAsync(uri, content);
                }

                // Get the JSON response
                string contentString = await response.Content.ReadAsStringAsync();

                // Display the JSON response
                Console.WriteLine("\nResponse:\n\n{0}\n", JToken.Parse(contentString).ToString());

                // Deserialize the JSON to an ImageObject
                ImageObject image = JsonConvert.DeserializeObject<ImageObject>(contentString);

                if (description)
                {
                    foreach (Caption caption in image.description.captions)
                    {
                        Console.WriteLine("Description of the image: \n" + caption.text + "\n\n");

                        // Save image with Description
                        SqlCommand insertImageWithDescriptionCommand = new SqlCommand($"INSERT INTO Image (ID, ImageFilePath, Picture, Description) Values (@ID, @ImageFilePath, @Picture, @Description)", myConnection);
    
                        insertImageWithDescriptionCommand.Parameters.Add("@ID", SqlDbType.Int).Value = imageId;
                        insertImageWithDescriptionCommand.Parameters.Add("@ImageFilePath", SqlDbType.VarChar).Value = imageFilePath;
                        insertImageWithDescriptionCommand.Parameters.Add("@Picture", SqlDbType.VarBinary).Value = byteData;
                        insertImageWithDescriptionCommand.Parameters.Add("@Description", SqlDbType.VarChar).Value = caption.text;

                        insertImageWithDescriptionCommand.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Save image without Description
                    SqlCommand insertImageCommand = new SqlCommand($"INSERT INTO Image (ID, ImageFilePath, Picture) Values (@ID, @ImageFilePath, @Picture)", myConnection);

                    insertImageCommand.Parameters.Add("@ID", SqlDbType.Int).Value = imageId;
                    insertImageCommand.Parameters.Add("@ImageFilePath", SqlDbType.VarChar).Value = imageFilePath;
                    insertImageCommand.Parameters.Add("@Picture", SqlDbType.VarBinary).Value = byteData;

                    insertImageCommand.ExecuteNonQuery();
                }
           
                Console.WriteLine("Tags for the image: ");
                foreach (Tag tag in image.tags)
                {
                    Console.WriteLine(tag.name);
                    SqlCommand insertTagCommand = new SqlCommand($"INSERT INTO Tag (PictureID, Tag) Values (@PictureID, @Tag)", myConnection);

                    insertTagCommand.Parameters.Add("@PictureID", SqlDbType.Int).Value = imageId;
                    insertTagCommand.Parameters.Add("@Tag", SqlDbType.VarChar).Value = tag.name;

                    insertTagCommand.ExecuteNonQuery();
                }
                Console.WriteLine("-------------------------------------------------------------");
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

            // Use this block if you want to upload a local image
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }


            //Use this block to upload a url instead of local files
            //using (var webClient = new WebClient())
            //{
            //    byte[] imageBytes = webClient.DownloadData(imageFilePath);
            //    return imageBytes;
            //}
        }
    }
}