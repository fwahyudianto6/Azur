using Azur.Web.UI.Models;
using ImageResizer;
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
        public ActionResult Index()
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
                    lsData.Add(new StorageModel()
                    {
                        ImageUri = vBlob.Uri.ToString(),
                        ThumbnailUri = vBlob.Uri.ToString().Replace("/photos/", "/thumbnails/")
                    });
                }
            }

            ViewBag.Blobs = lsData.ToArray();
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
    }
}