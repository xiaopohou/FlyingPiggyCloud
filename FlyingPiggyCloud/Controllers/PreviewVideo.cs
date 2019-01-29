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
            public string[] ClearTexts { get; set; }

            [JsonProperty(PropertyName = "clears")]
            public int[] Clears { get; set; }

            [JsonProperty(PropertyName = "encodeKey")]
            public string EncodeKey { get; set; }

            [JsonProperty(PropertyName = "preview")]
            public ObservableCollection<Dictionary<string, string>> Preview { get; set; }
        }

        public PreviewVideoInformation VideoSources { get; set; }

        public string CurrentToken { get; private set; }

        public async void LoadPreviewAddress(Action action)
        {
            FileSystemMethods fileSystemMethods = new FileSystemMethods(Properties.Settings.Default.BaseUri);
            var x = await fileSystemMethods.VideoPreview(UUID);
            VideoSources = x.Result;
            CurrentToken = x.Token;
            action?.Invoke();
            OnPropertyChanged("VideoSources");
        }

        public PreviewVideo(string uuid):base(PreviewTask.Video)
        {
            UUID = uuid;
        }
    }

}
