//using Exceptionless;
using QingzhenyunApis.Exceptions;
using QingzhenyunApis.Methods.V3;
using System;
//using System.Windows.Forms;

namespace SixCloud.Core.ViewModels
{
    internal partial class OfflineTaskDialogViewModel
    {
        internal class UrlResult : ParseResult
        {
            public override async void Parse(object parameter = null)
            {
                if (SourceUrl == null)
                {
                    throw new InvalidOperationException("Url为空");
                }

                try
                {
                    parseResult = await OfflineDownloader.Parse(SourceUrl, password: SharePassword);
                    Status = ParseResultStatus.Success;
                }
                catch (NeedPasswordException)
                {
                    Status = ParseResultStatus.PasswordRequired;
                }
                catch (UnsupportUrlException)
                {
                    Status = ParseResultStatus.InvalidUrl;
                }
                catch (RequestFailedException ex)
                {
                    if (ex.Code == "SHARE_IS_EMPTY")
                    {
                        Status = ParseResultStatus.InvalidUrl;
                    }
                    else
                    {
                        Status = ParseResultStatus.InvalidUrl;
                    }
                }
                catch (InvalidOperationException ex)
                {
                    ex.ToSentry().TreatedBy(nameof(OfflineTaskDialogViewModel)).AttachExtraInfo("urlTask", this).Submit();
                }

                OnPropertyChanged(nameof(PasswordBoxVisibility));
                OnPropertyChanged(nameof(Status));
                OnPropertyChanged(nameof(AllowEdit));
                OnPropertyChanged(nameof(Icon));
                OnPropertyChanged(nameof(FriendlyInfo));
                parent.UrlParseResultConfirmCommand.OnCanExecutedChanged(this, null);
                ParseCommand.OnCanExecutedChanged(this, null);
            }

            public UrlResult(string source, OfflineTaskDialogViewModel p) : base(null, p)
            {
                SourceUrl = source;
                ParseCommand = new DependencyCommand(Parse, CanParse);
                CancelCommand = new DependencyCommand(Cancel, DependencyCommand.AlwaysCan);
            }
        }
    }
}
