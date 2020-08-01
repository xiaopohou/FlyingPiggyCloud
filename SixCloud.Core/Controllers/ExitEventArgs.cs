using QingzhenyunApis.EntityModels;
using System;

namespace SixCloud.Core.Controllers
{
    internal class ExitEventArgs : EventArgs
    {
        public UserInformation CurrentUser { get; }

        public ExitEventArgs(UserInformation currentUser)
        {
            CurrentUser = currentUser;
        }
    }
}