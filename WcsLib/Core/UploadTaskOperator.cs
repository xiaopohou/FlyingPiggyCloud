using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcsLib.Core
{
    public class UploadTaskOperator
    {
        internal Action userCommand;

        public void Cancle()
        {
            userCommand = new Action(() =>
            {
                throw new Exception.OperatingAbortedException("上传任务被用户取消");
            });
        }
    }
}
