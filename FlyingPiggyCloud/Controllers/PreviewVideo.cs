using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlyingPiggyCloud.Controllers.Results.FileSystem;
using Newtonsoft.Json;

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
            [JsonProperty(PropertyName = "clearTexts")]
            public string[] ClearTexts { get; }

            [JsonProperty(PropertyName = "clears")]
            public int[] Clears { get; }

            [JsonProperty(PropertyName = "encodeKey")]
            public string EncodeKey { get; }

            [JsonProperty(PropertyName = "preview")]
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
