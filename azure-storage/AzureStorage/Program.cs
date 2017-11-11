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
      BlobStorage blobStorage = new BlobStorage(storageAccountName, storageAccountKey);
      blobStorage.CreateContainer("new-container");

      Console.WriteLine("Press any key to exit.");
      Console.Read();
    }
  }
}
