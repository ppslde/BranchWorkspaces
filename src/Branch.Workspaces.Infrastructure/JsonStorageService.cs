using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Branch.Workspaces.Core.Interfaces;
using Branch.Workspaces.Core.Models;

namespace Branch.Workspaces.Infrastructure
{
    public class JsonStorageService : IPersistenceService
    {
        private readonly FileInfo _file;
        private FileSystemWatcher _watcher;
        private StorageModel _storageModel;

        public JsonStorageService(string storageFile)
        {
            _file = new FileInfo(storageFile);
        }
        public async Task<BranchWorkspace> LoadWorkspace(BranchWorkspace workspace)
        {
            await EnsureLoadedAsync();

            return _storageModel.Workspaces.SingleOrDefault(w => w.Equals(workspace));
        }

        public async Task SaveWorkspace(BranchWorkspace workspace)
        {
            await EnsureLoadedAsync();

            BranchWorkspace ws = _storageModel.Workspaces.SingleOrDefault(w => w.WorkDir == workspace.WorkDir);
            if (ws != null)
                _storageModel.Workspaces.Remove(ws);

            _storageModel.Workspaces.Add(workspace);

            await EnsureSavedAsync();
        }

        private async Task EnsureLoadedAsync()
        {
            if (_storageModel != null && _watcher != null)
                return;

            StorageModel loadedModel;
            if (_file.Exists)
            {
                using (FileStream stream = File.Open(_file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    loadedModel = await JsonSerializer.DeserializeAsync<StorageModel>(stream);
            }
            else
            {
                loadedModel = new StorageModel { Info = new StorageMetaData { Location = _file.FullName } };
                using (FileStream stream = File.Open(_file.FullName, FileMode.Create, FileAccess.Write, FileShare.None))
                    await JsonSerializer.SerializeAsync(stream, loadedModel);
            }

            lock (loadedModel)
            {
                _storageModel = loadedModel;
            }

            _watcher = new FileSystemWatcher(_file.DirectoryName, _file.FullName);
            _watcher.Changed += OnStorageFileChanged;
        }

        private async Task EnsureSavedAsync()
        {
            using (FileStream stream = File.Open(_file.FullName, FileMode.Open, FileAccess.Write, FileShare.None))
                await JsonSerializer.SerializeAsync(stream, _storageModel);
        }

        private void OnStorageFileChanged(object sender, FileSystemEventArgs e)
        {
        }
    }

    class StorageModel
    {
        public StorageMetaData Info { get; set; }
        public List<BranchWorkspace> Workspaces { get; set; } = new List<BranchWorkspace>();
    }

    class StorageMetaData
    {
        public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
        public string Location { get; set; }
    }

}
