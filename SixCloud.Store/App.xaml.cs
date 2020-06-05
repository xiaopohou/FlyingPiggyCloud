using SixCloud.Store.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SixCloud.Store
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //Core.Core.Initialize();
            LibVLCSharp.Shared.Core.Initialize();
            new LoginWebViewModel();
        }
    }
}
