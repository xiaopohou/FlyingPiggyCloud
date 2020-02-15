using System;
using System.Globalization;
using System.Windows.Controls;

namespace SixCloud.ViewModels.ValidationRules
{
    internal class ParsingUrl : ValidationRule
    {
        public OfflineTaskDialogViewModel.ParseResult ParseResultDataContext { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return ParseResultDataContext.Status switch
            {
                OfflineTaskDialogViewModel.ParseResult.ParseResultStatus.Success => new ValidationResult(true, null),
                OfflineTaskDialogViewModel.ParseResult.ParseResultStatus.Parsing => new ValidationResult(true, "解析中请稍等"),
                OfflineTaskDialogViewModel.ParseResult.ParseResultStatus.PasswordRequired => new ValidationResult(false, "需要密码"),
                OfflineTaskDialogViewModel.ParseResult.ParseResultStatus.InvalidUrl => new ValidationResult(false, "地址解析失败"),
                _ => throw new InvalidOperationException("意料外的任务状态"),
            };
        }
    }
}
