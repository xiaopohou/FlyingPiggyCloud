﻿//using Exceptionless;
using QingzhenyunApis.EntityModels;
using System;
//using System.Windows.Forms;

namespace SixCloudCore.ViewModels
{
    internal partial class OfflineTaskDialogViewModel
    {

        internal class TorrentResult : ParseResult
        {
            public override void Parse(object parameter = null)
            {
                throw new NotImplementedException();
            }
            protected override bool CanParse(object parameter) => false;

            public TorrentResult(OfflineTaskParseUrl parseUrl, OfflineTaskDialogViewModel p) : base(parseUrl, p)
            {
                SourceUrl = parseUrl.Info.Name;
                Status = ParseResultStatus.Success;
            }
        }
    }
}
