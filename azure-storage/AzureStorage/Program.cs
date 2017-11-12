using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace AzureStorage
{
  class Program
  {
    static void Main(string[] args)
    {
      var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("config.json");

      IConfigurationRoot Configuration = builder.Build();

      string storageAccountName = Configuration["StorageAccountName"];
      string storageAccountKey = Configuration["StorageAccountKey"];
      AzureBlobStorageContainer container = new AzureBlobStorageContainer(storageAccountName, storageAccountKey, "new-container");
      container.CreateIfNotExists("new-container");
      container.ListAttributes();
      container.SetMetadata();
      container.ListMetadata();
      container.UploadBlob("532_OD_Changes.pdf");
      container.CopyBlob("532_OD_Changes.pdf", "Copy_Of_532_OD_Changes.pdf");
      container.UploadBlobSubdirectory("532_OD_Changes.pdf");

      Console.WriteLine("Press any key to exit.");
      Console.Read();
    }
  }
}
