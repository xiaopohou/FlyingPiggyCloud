using System;

namespace QingzhenyunApis.Methods.V3
{
    public sealed partial class FileSystem
    {
        /// <summary>
        /// 上传选项
        /// </summary>
        [Flags]
        public enum UploadOption
        {
            Stop = 0,
            Overwrite = 2,
            Ignore = 32
        }
    }
}
