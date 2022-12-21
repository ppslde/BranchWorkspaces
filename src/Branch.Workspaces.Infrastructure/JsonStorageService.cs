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

        private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

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

            BranchWorkspace ws = _storageModel.Workspaces.SingleOrDefault(w => w.Equals(workspace));
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
                using (FileStream stream = File.Open(_file.FullName, FileMode.Open, FileAccess.Read, FileShare.None))
                    loadedModel = await JsonSerializer.DeserializeAsync<StorageModel>(stream, _serializerOptions);
            }
            else
            {
                loadedModel = new StorageModel { Info = new StorageMetaData { Location = _file.FullName } };
                using (FileStream stream = File.Open(_file.FullName, FileMode.Create, FileAccess.Write, FileShare.None))
                    await JsonSerializer.SerializeAsync(stream, loadedModel, _serializerOptions);
            }

            _storageModel = loadedModel;

            _watcher = new FileSystemWatcher(_file.DirectoryName, _file.FullName);
            _watcher.Changed += OnStorageFileChanged;
        }

        private async Task EnsureSavedAsync()
        {
            if (_storageModel == null || _watcher == null)
                return;

            _storageModel.Info.Updated = DateTimeOffset.Now;

            using (FileStream stream = File.Open(_file.FullName, FileMode.Open, FileAccess.Write, FileShare.None))
            {
                stream.SetLength(0);
                await JsonSerializer.SerializeAsync(stream, _storageModel, _serializerOptions);
            }
        }

        private void OnStorageFileChanged(object sender, FileSystemEventArgs e)
        {
        }

        public void Dispose()
        {
            _watcher?.Dispose();
        }
    }

    class StorageModel
    {
        public StorageMetaData Info { get; set; }
        public List<BranchWorkspace> Workspaces { get; set; } = new List<BranchWorkspace>();
    }

    class StorageMetaData
    {
        public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset Updated { get; set; }

        public string Location { get; set; }
    }

}
