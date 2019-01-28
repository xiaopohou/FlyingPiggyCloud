using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlyingPiggyCloud.Controllers.Results.FileSystem;

namespace FlyingPiggyCloud.Controllers
{
    public class PreviewVideo : Preview, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class PreviewVideoInformation
        {
            public string[] ClearTexts { get; }

            public int[] Clears { get; }

            public string EncodeKey { get; }

            public ObservableCollection<Dictionary<string, string>> Preview { get; private set; }
        }

        public PreviewVideoInformation VideoSources { get; private set; }

        private async void LoadPreviewAddress()
        {
            FileSystemMethods fileSystemMethods = new FileSystemMethods(Properties.Settings.Default.BaseUri);
            VideoSources = await fileSystemMethods.VideoPreview(UUID);
            
            OnPropertyChanged("VideoSources");
        }

        public PreviewVideo(string uuid):base(PreviewTask.Video)
        {
            UUID = uuid;
            LoadPreviewAddress();
        }
    }

}
