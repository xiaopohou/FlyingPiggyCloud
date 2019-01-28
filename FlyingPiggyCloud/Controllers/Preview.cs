using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyingPiggyCloud.Controllers
{
    public abstract class Preview
    {
        public enum PreviewTask
        {
            Pdf,
            Image,
            Video
        }

        public PreviewTask PreviewType { get; protected set; }

        //protected Results.FileSystem.FileMetaData FileMetaData { get; set; }

        protected string UUID { get; set; }

        public Preview(PreviewTask previewTask) => PreviewType = previewTask;
    }

    

}
