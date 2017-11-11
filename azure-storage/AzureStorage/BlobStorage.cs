using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;

namespace AzureStorage
{
  public class BlobStorage
  {
    private readonly string _storageAccountName;
    private readonly string _storageAccountKey;

    public BlobStorage(string storageAccountName, string storageAccountKey)
    {
      _storageAccountName = storageAccountName;
      _storageAccountKey = storageAccountKey;
    }

    public void CreateContainer(string containerName)
    {
      var storageCredentials = new StorageCredentials(_storageAccountName, _storageAccountKey);
      var cloudStorageAccount = new CloudStorageAccount(storageCredentials, true);
      var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
      var container = cloudBlobClient.GetContainerReference(containerName);
      container.CreateIfNotExistsAsync();
    }
  }
}
