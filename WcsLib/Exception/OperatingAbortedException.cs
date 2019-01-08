using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcsLib.Exception
{
    public class OperatingAbortedException:System.Exception
    {
        public OperatingAbortedException(string message):base(message)
        {

        }
    }
}
