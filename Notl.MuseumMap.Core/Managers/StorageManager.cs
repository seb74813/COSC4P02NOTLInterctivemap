using Azure.Storage.Blobs;
using Notl.MuseumMap.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Azure.Storage.Blobs.Models;

namespace Notl.MuseumMap.Core.Managers
{
    public enum StorageContainerType
    {
        Accounts,
        Products,
        Messages,
        Recipe,
        Food,
    }

    /// <summary>
    /// Manages Azure Blob Storage to store files.
    /// </summary>
    public class StorageManager
    {
        public static readonly string[] SupportedImages = { ".png", ".gif", ".jpg", ".jpeg", ".bmp" };
        const int ThumbnailSize = 200;
        readonly string storageConnectionString;

        /// <summary>
        /// Constructs the storage manager with the required credentials.
        /// </summary>
        /// <param name="storageConnectionString"></param>
        public StorageManager(string storageConnectionString)
        {
            this.storageConnectionString = storageConnectionString;
        }

        /// <summary>
        /// Determines the container and path based on the type and ID passed.
        /// </summary>
        /// <param name="containerType"></param>
        /// <returns></returns>
        async Task<BlobContainerClient> GetContainerAsync(StorageContainerType containerType)
        {
            var container = new BlobContainerClient(storageConnectionString, containerType.ToString().ToLower());
            if (!await container.ExistsAsync())
            {
                // Create the container and set the default permissions
                await container.CreateIfNotExistsAsync(PublicAccessType.Blob);
            }
            return container;
        }

        /// <summary>
        /// Internal method to upload the file and generate a thumbnail (Thumbnails are only for image file types).
        /// </summary>
        /// <param name="containerType"></param>
        /// <param name="id"></param>
        /// <param name="filename"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public async Task<ImageReference> UploadFileAndCreateThumbnail(StorageContainerType containerType, Guid id, string filename, Stream stream)
        {
            var fileInfo = new FileInfo(filename);

            // Get the container and make sure it exists
            var container = await GetContainerAsync(containerType);

            var storageFile = new ImageReference { Id = id };

            // Upload the main file
            {
                var uniqueFile = $"{storageFile.Id}/{fileInfo.Name}";
                var blobClient = container.GetBlobClient(uniqueFile);
                await blobClient.DeleteIfExistsAsync();
                await blobClient.UploadAsync(stream);
                storageFile.Url = blobClient.Uri.ToString();
            }

            // Generate and upload the image thumbnail file
            if (SupportedImages.Contains(fileInfo.Extension.ToLower()))
            {
                Image original = Image.FromStream(stream);

                int rectHeight = ThumbnailSize;
                int rectWidth = ThumbnailSize;
                int newWidth, newHeight;

                //if the image is squared set it's height and width to the smallest of the desired dimensions (our box). In the current example rectHeight<rectWidth
                if (original.Height == original.Width)
                {
                    newWidth = ThumbnailSize;
                    newHeight = ThumbnailSize;
                }
                else
                {
                    //calculate aspect ratio
                    float aspect = original.Width / (float)original.Height;

                    //calculate new dimensions based on aspect ratio
                    newWidth = (int)(rectWidth * aspect);
                    newHeight = (int)(newWidth / aspect);
                    //if one of the two dimensions exceed the box dimensions
                    if (newWidth > rectWidth || newHeight > rectHeight)
                    {
                        //depending on which of the two exceeds the box dimensions set it as the box dimension and calculate the other one based on the aspect ratio
                        if (newWidth > newHeight)
                        {
                            newWidth = rectWidth;
                            newHeight = (int)(newWidth / aspect);

                        }
                        else
                        {
                            newHeight = rectHeight;
                            newWidth = (int)(newHeight * aspect);

                        }
                    }
                }

                // Resize the thumbnail
                Image thumbnail = original.GetThumbnailImage(newWidth, newHeight, () => false, IntPtr.Zero);

                using (var thumbnailStream = new MemoryStream())
                {
                    thumbnail.Save(thumbnailStream, ImageFormat.Png);
                    thumbnailStream.Seek(0, SeekOrigin.Begin);

                    var uniqueFile = $"{storageFile.Id}/thumbnail-{fileInfo.Name.Replace(fileInfo.Extension, ".png")}";
                    var thumbnailClient = container.GetBlobClient(uniqueFile);
                    await thumbnailClient.DeleteIfExistsAsync();
                    await thumbnailClient.UploadAsync(thumbnailStream);
                    storageFile.Thumbnail = thumbnailClient.Uri.ToString();
                }
            }

            return storageFile;
        }


        /// <summary>
        /// Deletes all files (image + thumbnail) based on the image ID.
        /// </summary>
        /// <param name="containerType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteStorageFolder(StorageContainerType containerType, Guid id)
        {
            // Get the container and make sure it exists
            var container = await GetContainerAsync(containerType);
            if (await container.ExistsAsync())
            {
                var blobItems = container.GetBlobs(prefix: id.ToString());
                if (blobItems != null)
                {
                    foreach (BlobItem blobItem in blobItems)
                    {
                        BlobClient blobClient = container.GetBlobClient(blobItem.Name);
                        await blobClient.DeleteIfExistsAsync();
                    }
                }
            }

        }

    }
}
