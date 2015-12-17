using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleIntegrationRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Perforce perforce = new Perforce("USERNAME", "PASSWORD", "ADDRESS"))
            {
                var fileLocations = perforce.FindFileLocation("webservice.sln", "main");
                var webServiceLocation = fileLocations.FirstOrDefault();

                var allFilesForWebService = perforce.FetchFilesInDirectory(webServiceLocation + "...");

                if (string.IsNullOrEmpty(webServiceLocation))
                {
                    Console.WriteLine("Solution file could not be found");
                }

                var syncedFiles = perforce.CloneFiles(webServiceLocation);
                var localFileLocations = perforce.GetLocalDirectory();
            }

            Console.Read();
        }
    }
}
