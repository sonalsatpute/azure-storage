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

      Console.WriteLine(Configuration["StorageConnectionString"]);
    }
  }
}
