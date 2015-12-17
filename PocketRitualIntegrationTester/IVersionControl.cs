using Perforce.P4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleIntegrationRunner
{
    interface IVersionControl : IDisposable
    {
        IEnumerable<string> FetchFileList();
        IEnumerable<File> FetchFilesInDirectory(string location, bool local = false);
        IEnumerable<string> FindFileLocation(string fileName, string directory);
        IEnumerable<string> CloneFiles(string depoDirectory);
        string GetLocalDirectory();
    }
}
