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
            if (file != null && file.ContentLength > 0)
            {
                // Make sure the user selected an image file
                if (!file.ContentType.StartsWith("image"))
                {
                    TempData["Message"] = "Only Image Files may be Uploaded!";
                }
                else
                {
                    try
                    {
                        // Save the original image in the "photos" container
                        CloudStorageAccount oStorageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
                        CloudBlobClient oBlobClient = oStorageAccount.CreateCloudBlobClient();
                        CloudBlobContainer oStorageContainer = oBlobClient.GetContainerReference("photos");

                        CloudBlockBlob oPhoto = oStorageContainer.GetBlockBlobReference(Path.GetFileName(file.FileName));
                        await oPhoto.UploadFromStreamAsync(file.InputStream);

                        // Generate a thumbnail and save it in the "thumbnails" container

                        #region Generate a Thumbnail

                        using (var outputStream = new MemoryStream())
                        {
                            file.InputStream.Seek(0L, SeekOrigin.Begin);
                            var settings = new ResizeSettings { MaxWidth = 192 };

                            ImageBuilder.Current.Build(file.InputStream, outputStream, settings);
                            outputStream.Seek(0L, SeekOrigin.Begin);
                            oStorageContainer = oBlobClient.GetContainerReference("thumbnails");

                            CloudBlockBlob thumbnail = oStorageContainer.GetBlockBlobReference(Path.GetFileName(file.FileName));
                            await thumbnail.UploadFromStreamAsync(outputStream);
                        }

                        #endregion // end of Generate a Thumbnail 

                        #region Submit to Azure's Computer Vision API

                        ComputerVisionClient oVisionClient = new ComputerVisionClient(
                            new ApiKeyServiceClientCredentials(ConfigurationManager.AppSettings["SubscriptionKey"]),
                            new System.Net.Http.DelegatingHandler[] { });
                        oVisionClient.Endpoint = ConfigurationManager.AppSettings["VisionEndpoint"];

                        VisualFeatureTypes[] features = new VisualFeatureTypes[] { VisualFeatureTypes.Description };
                        var result = await oVisionClient.AnalyzeImageAsync(oPhoto.Uri.ToString(), features);

                        // Record the image description and tags in blob metadata
                        oPhoto.Metadata.Add("Caption", result.Description.Captions[0].Text);

                        for (int i = 0; i < result.Description.Tags.Count; i++)
                        {
                            string key = String.Format("Tag{0}", i);
                            oPhoto.Metadata.Add(key, result.Description.Tags[i]);
                        }
                        await oPhoto.SetMetadataAsync();

                        #endregion // end of Submit to Azure's Computer Vision API 
                    }
                    catch (Exception oException)
                    {
                        // In case something goes wrong
                        TempData["Message"] = oException.Message;
                    }
                }
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
    }
}