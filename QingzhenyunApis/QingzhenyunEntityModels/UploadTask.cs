namespace SixCloud.Models
{
    public interface IUploadTask
    {
        void Pause();

        RecoverableUploadTaskArchive Save();

        void Abort();

        void Start();

        bool IsRunning { get; }
    }
}
