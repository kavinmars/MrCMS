using System;
using System.Web.Mvc;
using MrCMS.Settings;

namespace MrCMS.Website.Binders
{
    public class FileSystemSettingsModelBinder : MrCMSDefaultModelBinder
    {
        private readonly FileSystemSettings _fileSystemSettings;

        public FileSystemSettingsModelBinder(FileSystemSettings fileSystemSettings) 
        {
            _fileSystemSettings = fileSystemSettings;
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext,
                                              Type modelType)
        {
            return _fileSystemSettings;
        }
    }
}