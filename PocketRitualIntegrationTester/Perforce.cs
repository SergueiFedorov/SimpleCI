using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perforce.P4;
using System.Collections.Concurrent;

namespace SimpleIntegrationRunner
{
    class Perforce : IVersionControl
    {
        Server _server { get; set; }
        Repository _reposetory { get; set; }
        Connection _connection { get; set; }

        public string _username { get; }
        public string _password { get; }
        public string _address { get; }

        public Perforce(string username, string password, string address)
        {
            this._username = username;
            this._password = password;
            this._address = address;

            _server = new Server(new ServerAddress(address));
            _reposetory = new Repository(_server);

            _connection = _reposetory.Connection;
            _connection.UserName = username;
            _connection.Client = new Client();
            _connection.Client.Name = "integration";

            this.Connect();

            _reposetory.CreateClient(_connection.Client);
        }

        private void Connect()
        {
            _connection.Connect(null);
            _connection.Login(_password);
        }

        public IEnumerable<string> FetchFileList()
        {
            return null;
        }

        public void Dispose()
        {
            _connection.Disconnect();
        }

        public IEnumerable<File> FetchFilesInDirectory(string location, bool local = false)
        {
            FileSpec fileSpec;
            if (local)
            {
                fileSpec = new FileSpec(new ClientPath(location), null);
            }
            else
            {
                fileSpec = new FileSpec(new DepotPath(location), null);
            }

            var opts = new GetDepotFilesCmdOptions(GetDepotFilesCmdFlags.None, 0);
            var result = _reposetory.GetFiles(opts, fileSpec);
            return result;
        }

        public IEnumerable<string> FindFileLocation(string fileName, string directory)
        {
            var allFiles = FetchFilesInDirectory("//depot/...").Select(x => x.DepotPath.Path);

            BlockingCollection<string> foundFiles = new BlockingCollection<string>();
            allFiles.AsParallel().ForAll(x =>
            {
                if (x.StartsWith("//depot/" + directory) && x.ToLower().EndsWith(fileName.ToLower()))
                {
                    var rawDirectory = x.Substring(0, x.Length - fileName.Length);
                    foundFiles.Add(rawDirectory);
                }
            });

            return foundFiles.ToList();
        }



        public IEnumerable<string> CloneFiles(string depoDirectory)
        {
            var spec = new FileSpec(new DepotPath(depoDirectory + "..."), VersionSpec.Head);
            IList<FileSpec> list = new List<FileSpec>();
            list.Add(spec);

            var syncedFiles = _connection.Client.SyncFiles(list, null);
            if (syncedFiles == null)
            {
                return new List<string>();
            }

            return syncedFiles.Select(x => x.ClientPath.Path).ToList();
        }

        public string GetLocalDirectory()
        {
            return this._connection.Client.Root;
        }

    }
}
