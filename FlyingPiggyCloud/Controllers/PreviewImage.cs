using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace FlyingPiggyCloud.Controllers
{
    public class PreviewImage : Preview, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class PreviewImageInformation
        {
            [JsonProperty(PropertyName = "fileHash")]
            public string FileHash { get; set; }

            [JsonProperty(PropertyName = "format")]
            public string Format { get; set; }

            [JsonProperty(PropertyName = "hash")]
            public string Hash { get; set; }

            [JsonProperty(PropertyName = "height")]
            public int Height { get; set; }

            [JsonProperty(PropertyName = "size")]
            public int Size { get; set; }

            [JsonProperty(PropertyName = "width")]
            public int Width { get; set; }

            [JsonProperty(PropertyName = "address")]
            public string Address { get; set; }

            [JsonProperty(PropertyName = "url")]
            public string Url { get; set; }

            [JsonProperty(PropertyName = "mime")]
            public string Mime { get; set; }
        }

        public PreviewImageInformation ImageSources { get; private set; }

        public async void LoadPreviewAddress(Action e)
        {
            FileSystemMethods fileSystemMethods = new FileSystemMethods(Properties.Settings.Default.BaseUri);
            ImageSources = await fileSystemMethods.ImagePreview(UUID);
            if(ImageSources.Address!=null)
            {
                e?.Invoke();
                OnPropertyChanged("ImageSources");
            }
        }

        public PreviewImage(string uuid) : base(PreviewTask.Video)
        {
            UUID = uuid;
        }
    }

}
