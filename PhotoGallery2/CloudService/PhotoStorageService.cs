using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Ajax.Utilities;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using PhotoGallery2.Models;


namespace PhotoGallery2.CloudService
{
    public class StorageUtils
    {

        public static CloudStorageAccount StorageAccount
        {
            get
            {
                var account = CloudConfigurationManager.GetSetting("StorageAccountName");
                var key = CloudConfigurationManager.GetSetting("StorageAccountAccessKey");

                string connectionString = string.Format("DefautlEndPointProtocol=https;AccountName={0};AccountKey{1}", account, key);

                //connectionString = "UseDevelopmentStorage=true";

                return CloudStorageAccount.Parse(connectionString);
            }
        }
    }

    public class PhotoStorageService
    {

        private const string CONTAINER = "photos";

        public async Task CreateAndConfigureAsync(string containerName)
        {
            var storageAccount = StorageUtils.StorageAccount;
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);

            if (await container.CreateIfNotExistsAsync())
            {
                await container.SetPermissionsAsync(
                    new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    });
            }
        }

        public async Task UploadPhotoAsync(Photo photo, HttpPostedFileBase fileToUpload)
        {
            var container = getCloudBlobContainer();

            var fileName = string.Format("Photo-{0}{1}", Guid.NewGuid().ToString(), Path.GetExtension(fileToUpload.FileName));

            var blockBlob = container.GetBlockBlobReference(string.Format("{0}/{1}", photo.AlbumID, fileName));

            blockBlob.Properties.ContentType = fileToUpload.ContentType;

            await blockBlob.UploadFromStreamAsync(fileToUpload.InputStream);

            var uriBuilder = new UriBuilder(blockBlob.Uri)
            {
                Scheme = "http"
            };

            photo.FileName = fileName;
            photo.PhotoPath = uriBuilder.ToString();

            //Now upload the created thumbnail.

            var bitMap = Image.FromStream(fileToUpload.InputStream);
            var thumbNail = bitMap.GetThumbnailImage(160, 160, null, IntPtr.Zero);
            var thumbNailFileName = string.Format("{0}/{1}/{2}", photo.AlbumID, "thumbs", fileName);

            var memory = new MemoryStream();

            thumbNail.Save(memory, ImageFormat.Jpeg);

            var byteArray = new byte[memory.Length];
            memory.Position = 0;

            await memory.ReadAsync(byteArray, 0, (int)memory.Length);

            blockBlob = container.GetBlockBlobReference(thumbNailFileName);

            blockBlob.Properties.ContentType = fileToUpload.ContentType;

            await blockBlob.UploadFromByteArrayAsync(byteArray, 0, byteArray.Length);

            uriBuilder = new UriBuilder(blockBlob.Uri)
            {
                Scheme = "http"
            };

            await memory.FlushAsync();

            photo.ThumbnailPath = uriBuilder.ToString();
        }

        public async Task<bool> DeletePhotos(IEnumerable<Photo> photos)
        {
            var container = getCloudBlobContainer();

            foreach (var photo in photos)
            {
                await deletePhotoReference(container, string.Format("{0}/{1}", photo.AlbumID, photo.FileName));
                await deletePhotoReference(container, string.Format("{0}/{1}/{2}", photo.AlbumID, "thumbs", photo.FileName));
            }

            return true;
        }

        async Task<bool> deletePhotoReference(CloudBlobContainer container, string path)
        {
            try
            {
                var blockBlog = container.GetBlockBlobReference(path);
                return await blockBlog.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, null, null, null);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private CloudBlobContainer getCloudBlobContainer()
        {
            var storageAccount = StorageUtils.StorageAccount;
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(CONTAINER);
            return container;
        }
    }
}