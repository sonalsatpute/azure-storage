using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
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

    public void CreateSharedAccessPolicy()
    {
      //Create a new stored access policy and define its constraints.
      SharedAccessBlobPolicy sharedPolicy = new SharedAccessBlobPolicy()
      {
        SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
        Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.List
      };

      //Get the container's existing permissions.
      BlobContainerPermissions permissions = new BlobContainerPermissions();

      //Add the new policy to the container's permissions.
      permissions.SharedAccessPolicies.Clear();
      permissions.SharedAccessPolicies.Add("PolicyName", sharedPolicy);
      _container.SetPermissionsAsync(permissions);
    }

    //public void CreateCORSPolicy()
    //{
    //  ServiceProperties sp = new ServiceProperties();
    //  sp.Cors.CorsRules.Add(new CorsRule()
    //  {
    //    AllowedMethods = CorsHttpMethods.Get,
    //    AllowedOrigins = new List<string>() { "http://localhost:8080/" },
    //    MaxAgeInSeconds = 3600,
    //  });
    //  _container.ServiceClient.SetServicePropertiesAsync(sp);

    //  //CloudBlobClient blobClient;
    //  //blobClient.SetServicePropertiesAsync(sp);
    //}

  }
}
