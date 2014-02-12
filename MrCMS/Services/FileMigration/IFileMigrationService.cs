using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Policy;
using MrCMS.DataAccess;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Tasks;
using MrCMS.Website;
using Newtonsoft.Json;
using Ninject;
using Site = MrCMS.Entities.Multisite.Site;

namespace MrCMS.Services.FileMigration
{
    public interface IFileMigrationService
    {
        void MigrateFiles();
    }

    public class FileMigrationService : IFileMigrationService
    {
        private readonly IEnumerable<IFileSystem> _allFileSystems;
        private readonly FileSystemSettings _fileSystemSettings;
        private readonly IDbContext _dbContext;
        private readonly Site _site;

        public FileMigrationService(IKernel kernel, FileSystemSettings fileSystemSettings, IDbContext dbContext, Site site)
        {
            _allFileSystems =
                TypeHelper.GetAllTypesAssignableFrom<IFileSystem>()
                          .Select(type => kernel.Get(type) as IFileSystem)
                          .ToList();
            _fileSystemSettings = fileSystemSettings;
            _dbContext = dbContext;
            _site = site;
        }

        public IFileSystem CurrentFileSystem
        {
            get
            {
                var storageType = _fileSystemSettings.StorageType;
                return _allFileSystems.FirstOrDefault(system => system.GetType().FullName == storageType);
            }
        }

        public void MigrateFiles()
        {
            var mediaFiles = _dbContext.Query<MediaFile>().Where(file => file.Site.Id == _site.Id).Include(file => file.ResizedImages).ToList();
            var filesToMove =
                mediaFiles.Where(mediaFile => mediaFile.GetFileSystem(_allFileSystems) != CurrentFileSystem)
                          .Select(file => new MoveFileData
                                              {
                                                  FileId = file.Id,
                                                  From = file.GetFileSystem(_allFileSystems).GetType().FullName,
                                                  To = CurrentFileSystem.GetType().FullName
                                              }).ToList();

            foreach (var queuedTask in filesToMove.Select(moveFileData => new QueuedTask
                                                                              {
                                                                                  Data = JsonConvert.SerializeObject(moveFileData),
                                                                                  Type = typeof(MoveFile).FullName,
                                                                                  Status = TaskExecutionStatus.Pending
                                                                              }))
            {
                CurrentRequestData.QueuedTasks.Add(queuedTask);
            }
        }
    }

    public class MoveFile : AdHocTask
    {
        private readonly IEnumerable<IFileSystem> _fileSystems;
        private readonly IDbContext _dbContext;

        public MoveFile(IKernel kernel, IDbContext dbContext)
        {
            _fileSystems =
                TypeHelper.GetAllTypesAssignableFrom<IFileSystem>()
                          .Select(type => kernel.Get(type) as IFileSystem)
                          .ToList();
            _dbContext = dbContext;
        }

        public override int Priority
        {
            get { return 0; }
        }
        private MoveFileData FileData { get; set; }

        protected override void OnExecute()
        {
            _dbContext.Transact(session =>
                                  {
                                      var file = _dbContext.Get<MediaFile>(FileData.FileId);
                                      var from = _fileSystems.FirstOrDefault(system => system.GetType().FullName == FileData.From);
                                      var to = _fileSystems.FirstOrDefault(system => system.GetType().FullName == FileData.To);

                                      // remove resized images (they will be regenerated on the to system)
                                      foreach (var resizedImage in file.ResizedImages.ToList())
                                      {
                                          from.Delete(resizedImage.Url);
                                          file.ResizedImages.Remove(resizedImage);
                                          session.Delete(resizedImage);
                                      }

                                      var existingUrl = file.FileUrl;
                                      using (var readStream = @from.GetReadStream(existingUrl))
                                      {
                                          file.FileUrl = to.SaveFile(readStream, GetNewFilePath(file),
                                                                     file.ContentType);
                                      }
                                      from.Delete(existingUrl);

                                      session.Update(file);
                                  });
        }

        private string GetNewFilePath(MediaFile file)
        {
            var fileUrl = file.FileUrl;
            var id = file.Site.Id;
            var indexOf = file.FileUrl.IndexOf(string.Format("/{0}/", id), StringComparison.OrdinalIgnoreCase);
            var newFilePath = fileUrl.Substring(indexOf + 1);
            return newFilePath;
        }

        public override string GetData()
        {
            return JsonConvert.SerializeObject(FileData);
        }

        public override void SetData(string data)
        {
            FileData = JsonConvert.DeserializeObject<MoveFileData>(data);
        }
    }

    public class MoveFileData
    {
        public int FileId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}