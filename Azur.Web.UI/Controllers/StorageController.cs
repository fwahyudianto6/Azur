using Azur.Web.UI.Models;
using ImageResizer;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Azur.Web.UI.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Azur.Web.UI.Controllers
{
    public class StorageController : Controller
    {
        /// <summary>
        /// Azure Storage and Cognitive Services Page
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string id)
        {
            // Pass a list of blob URIs in ViewBag
            CloudStorageAccount oStorageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudBlobClient oBlobClient = oStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer oStorageContainer = oBlobClient.GetContainerReference("photos");
            List<StorageModel> lsData = new List<StorageModel>();

            foreach (IListBlobItem item in oStorageContainer.ListBlobs())
            {
                var vBlob = item as CloudBlockBlob;

                if (vBlob != null)
                {
                    vBlob.FetchAttributes(); // Get blob metadata

                    if (String.IsNullOrEmpty(id) || HasMatchingMetadata(vBlob, id))
                    {
                        var caption = vBlob.Metadata.ContainsKey("Caption") ? vBlob.Metadata["Caption"] : vBlob.Name;

                        lsData.Add(new StorageModel()
                        {
                            ImageUri = vBlob.Uri.ToString(),
                            ThumbnailUri = vBlob.Uri.ToString().Replace("/photos/", "/thumbnails/"),
                            Caption = caption
                        });
                    }
                }
            }

            ViewBag.Blobs = lsData.ToArray();
            ViewBag.Search = id; // Prevent search box from losing its content
            return View();
        }

        /// <summary>
        /// Upload Image Async
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Upload(HttpPostedFileBase file)
        {
            // Save the original image in the "photos" container
            CloudStorageAccount oStorageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudBlobClient oBlobClient = oStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer oStorageContainer = oBlobClient.GetContainerReference("photos");

            CloudBlockBlob oPhoto = oStorageContainer.GetBlockBlobReference(Path.GetFileName(file.FileName));

            string strThumbUri = string.Empty;
            const string uriBase = "https://southeastasia.api.cognitive.microsoft.com/vision/v1.0/analyze";
            
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    // Make sure the user selected an image file
                    if (!file.ContentType.StartsWith("image"))
                    {
                        TempData["Message"] = "Only Image Files may be Uploaded!";
                    }
                    else
                    {
                        await oPhoto.UploadFromStreamAsync(file.InputStream);

                        #region Generate a Thumbnail

                        using (var outputStream = new MemoryStream())
                        {
                            file.InputStream.Seek(0L, SeekOrigin.Begin);
                            var settings = new ResizeSettings { MaxWidth = 192 };

                            ImageBuilder.Current.Build(file.InputStream, outputStream, settings);
                            outputStream.Seek(0L, SeekOrigin.Begin);
                            oStorageContainer = oBlobClient.GetContainerReference("thumbnails");

                            CloudBlockBlob oThumbnail = oStorageContainer.GetBlockBlobReference(Path.GetFileName(file.FileName));
                            await oThumbnail.UploadFromStreamAsync(outputStream);

                            strThumbUri = oThumbnail.Uri.ToString();
                            oThumbnail = null;
                        }

                        #endregion // end of Generate a Thumbnail 

                        #region Submit to Azure's Computer Vision API

                        ComputerVisionClient oVisionClient = new ComputerVisionClient(
                            new ApiKeyServiceClientCredentials(ConfigurationManager.AppSettings["SubscriptionKey"]),
                            new System.Net.Http.DelegatingHandler[] { });
                        oVisionClient.Endpoint = ConfigurationManager.AppSettings["VisionEndpoint"];

                        VisualFeatureTypes[] oVisualFeatures = new VisualFeatureTypes[]
                        {
                            VisualFeatureTypes.Objects,
                            VisualFeatureTypes.Tags,
                            VisualFeatureTypes.Description,
                            VisualFeatureTypes.Faces,
                            VisualFeatureTypes.ImageType,
                            VisualFeatureTypes.Color,
                            VisualFeatureTypes.Adult,
                            VisualFeatureTypes.Categories,
                            VisualFeatureTypes.Brands
                        };
                        var vResponse = await oVisionClient.AnalyzeImageAsync(oPhoto.Uri.ToString(), oVisualFeatures);

                        // Record the image description and tags in blob metadata
                        oPhoto.Metadata.Add("Caption", vResponse.Description.Captions[0].Text);

                        for (int i = 0; i < vResponse.Description.Tags.Count; i++)
                        {
                            string key = String.Format("Tag{0}", i);
                            oPhoto.Metadata.Add(key, vResponse.Description.Tags[i]);
                        }
                        await oPhoto.SetMetadataAsync();

                        #endregion // end of Submit to Azure's Computer Vision API   

                        #region Save Database

                        if (vResponse != null)
                        {
                            HttpClient oHttpClient = new HttpClient();
                            HttpResponseMessage oHttpResponseMessage;

                            // Request headers.
                            oHttpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["SubscriptionKey"]);

                            // Request parameters. A third optional parameter is "details".
                            string strParams = "visualFeatures=Categories,Tags,Description,Faces,ImageType,Color,Adult&details=&language=en";

                            // Request body. Posts a locally stored JPEG image.
                            string strBody = "{\"url\":\"" + oPhoto.Uri.ToString() + "\"}";   // https://azurui.blob.core.windows.net/photos/Combine.jpg
                            byte[] byteData = Encoding.UTF8.GetBytes(strBody);

                            using (ByteArrayContent content = new ByteArrayContent(byteData))
                            {
                                content.Headers.ContentType =
                                    new MediaTypeHeaderValue("application/json");
                                // Make the REST API call.
                                oHttpResponseMessage = await oHttpClient.PostAsync(
                                    string.Format("{0}?{1}", uriBase, strParams), content);
                            }

                            // Get the JSON response.
                            string strResponse = await oHttpResponseMessage.Content.ReadAsStringAsync();

                            if (oHttpResponseMessage.IsSuccessStatusCode)
                            {
                                DataAccess oDataAccess = new DataAccess();
                                StorageModel oStorageModel = new StorageModel(
                                    oPhoto.Name,
                                    vResponse.Description.Captions[0].Text,
                                    oPhoto.Uri.ToString(),
                                    strThumbUri,
                                    strResponse);

                                oDataAccess.AddStorage(oStorageModel);
                                oStorageModel = null;
                            }
                        }

                        #endregion // end of Save Database 
                    }
                }
            }
            catch (Exception oException)
            {
                // In case something goes wrong
                TempData["Message"] = oException.Message;
            }
            finally
            {
                oStorageAccount = null;
                oBlobClient = null;
                oStorageContainer = null;
                oPhoto = null;
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Search Image by Term
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Search(string term)
        {
            return RedirectToAction("Index", new { id = term });
        }

        /// <summary>
        /// Hashing Metadata
        /// </summary>
        /// <param name="blob"></param>
        /// <param name="term"></param>
        /// <returns></returns>
        private bool HasMatchingMetadata(CloudBlockBlob blob, string term)
        {
            foreach (var item in blob.Metadata)
            {
                if (item.Key.StartsWith("Tag") && item.Value.Equals(term, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }

            return false;
        }

        public ActionResult Detail(string uri)
        {
            List<StorageModel> lsData = new List<StorageModel>();

            if (!String.IsNullOrEmpty(uri))
            {
                DataAccess oDataAccess = new DataAccess();
                StorageModel oStorageModel = new StorageModel();

                oStorageModel = oDataAccess.GetStorage().Find(Models => Models.ImageUri == uri);

                lsData.Add(new StorageModel()
                {
                    ImageUri = oStorageModel.ImageUri,
                    ThumbnailUri = oStorageModel.ImageUri.Replace("/photos/", "/thumbnails/"),
                    Caption = oStorageModel.Caption,
                    Response = oStorageModel.Response
                });
            }

            ViewBag.Blobs = lsData.ToArray();
            ViewBag.Search = uri; // Prevent search box from losing its content
            return View();
        }
    }
}