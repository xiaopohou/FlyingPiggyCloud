//using Exceptionless;
using QingzhenyunApis.EntityModels;
using System.Collections.Generic;
using System.Windows;
//using System.Windows.Forms;

namespace SixCloud.Core.ViewModels
{
    internal partial class OfflineTaskDialogViewModel
    {
        internal abstract class ParseResult : ViewModelBase
        {
            protected OfflineTaskParseUrl parseResult;
            protected readonly OfflineTaskDialogViewModel parent;

            internal enum ParseResultStatus
            {
                /// <summary>
                /// 解析中
                /// </summary>
                Parsing,
                /// <summary>
                /// 成功
                /// </summary>
                Success,
                /// <summary>
                /// 需要密码
                /// </summary>
                PasswordRequired,
                /// <summary>
                /// 不合法的地址
                /// </summary>
                InvalidUrl
            }

            public string Name => parseResult.Info.Name;

            public string FriendlyInfo => Status switch
            {
                ParseResultStatus.Success => FindLocalizationResource("Lang-Succeeded"),
                ParseResultStatus.Parsing => FindLocalizationResource("Lang-Parsing"),
                ParseResultStatus.PasswordRequired => FindLocalizationResource("Lang-PasswordRequested"),
                ParseResultStatus.InvalidUrl => FindLocalizationResource("Lang-ParseFailed"),
                _ => FindLocalizationResource("Lang-ExceptionalStatus"),
            };

            //public Visibility FriendlyErrorInfoVisibility => FriendlyErrorInfo == null ? Visibility.Collapsed : Visibility.Visible;

            public string Identity => parseResult.Hash;

            public long Size => parseResult.Info.Size;

            public IList<OfflineTaskParseFile> Files => parseResult.Info.DataList;

            /// <summary>
            /// 任务解析状态
            /// </summary>
            public ParseResultStatus Status { get; protected set; } = ParseResultStatus.Parsing;

            public Visibility PasswordBoxVisibility => Status == ParseResultStatus.PasswordRequired ? Visibility.Visible : Visibility.Collapsed;

            public string SourceUrl { get; set; }

            public bool AllowEdit => Status != ParseResultStatus.Success;

            public string Icon => Status switch
            {
                ParseResultStatus.Parsing => "\uf519",
                ParseResultStatus.PasswordRequired => "\uf084",
                ParseResultStatus.InvalidUrl => "\uf06a",
                ParseResultStatus.Success => "\uf058",
                _ => "\uf519",
            };

            public string SharePassword { get; set; }

            /// <summary>
            /// 仅未成功的任务可调用此Command
            /// </summary>
            public DependencyCommand ParseCommand { get; set; }
            public abstract void Parse(object parameter = null);
            protected virtual bool CanParse(object parameter)
            {
                return Status != ParseResultStatus.Success;
            }

            public DependencyCommand CancelCommand { get; set; }
            protected void Cancel(object parameter)
            {
                parent.ParseResults.Remove(this);
                parent.UrlParseResultConfirmCommand.OnCanExecutedChanged(this, null);
            }


            protected ParseResult(OfflineTaskParseUrl parseUrl, OfflineTaskDialogViewModel p)
            {
                parseResult = parseUrl;
                parent = p;
                ParseCommand = new DependencyCommand(Parse, CanParse);
                CancelCommand = new DependencyCommand(Cancel, DependencyCommand.AlwaysCan);
            }
        }
    }
}
