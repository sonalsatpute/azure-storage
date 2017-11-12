using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;

namespace AzureStorage
{
  public class AzureBlobStorageContainer
  {
    private readonly string _storageAccountName;
    private readonly string _storageAccountKey;
    private readonly string _containerName;
    private CloudBlobContainer _container;

    public AzureBlobStorageContainer(string storageAccountName, string storageAccountKey, string containerName)
    {
      _storageAccountName = storageAccountName;
      _storageAccountKey = storageAccountKey;
      _containerName = containerName;
    }

    public void CreateIfNotExists(string containerName)
    {
      var storageCredentials = new StorageCredentials(_storageAccountName, _storageAccountKey);
      var cloudStorageAccount = new CloudStorageAccount(storageCredentials, true);
      var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
      _container = cloudBlobClient.GetContainerReference(containerName);
      _container.CreateIfNotExistsAsync();
    }

    public void ListAttributes()
    {
      _container.FetchAttributesAsync();
      Console.WriteLine("Container name " + _container.StorageUri.PrimaryUri.ToString());
      Console.WriteLine("Last modified " + _container.Properties.LastModified.ToString());
    }

    public void SetMetadata()
    {
      _container.Metadata.Clear();
      _container.Metadata.Add("author", "sonal satpute");
      _container.Metadata["authoredOn"] = DateTime.Now.ToString();
      _container.SetMetadataAsync();
    }

    public void ListMetadata()
    {
      Console.WriteLine("Metadata:\n");
      foreach (var item in _container.Metadata)
      {
        Console.WriteLine("Key " + item.Key);
        Console.WriteLine("Value " + item.Value + "\n\n");
      }
    }

    public void UploadBlob(string path)
    {
      FileInfo info = new FileInfo(path);
      CloudBlockBlob blockBlob = _container.GetBlockBlobReference(info.Name);

      using (var fileStream = File.OpenRead(path))
      {
        blockBlob.UploadFromStreamAsync(fileStream);
      }
    }

    public void CopyBlob(string blobName, string newBlobName)
    {
      CloudBlockBlob blockBlob = _container.GetBlockBlobReference(blobName);
      CloudBlockBlob copyToBlockBlob = _container.GetBlockBlobReference(newBlobName);
      copyToBlockBlob.StartCopyAsync(new Uri(blockBlob.Uri.AbsoluteUri));
    }

    public void UploadBlobSubdirectory(string path)
    {
      FileInfo info = new FileInfo(path);

      CloudBlobDirectory directory = _container.GetDirectoryReference("parent-directory");
      CloudBlobDirectory subdirectory = directory.GetDirectoryReference("child-directory");
      CloudBlockBlob blockBlob = subdirectory.GetBlockBlobReference(info.Name);
      
      using (var fileStream =File.OpenRead(path))
      {
        blockBlob.UploadFromStreamAsync(fileStream);
      }

    }
  }
}
