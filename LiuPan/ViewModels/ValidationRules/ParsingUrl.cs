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
            switch (ParseResultDataContext.Status)
            {
                case OfflineTaskDialogViewModel.ParseResult.ParseResultStatus.Success:
                    return new ValidationResult(true, null);
                case OfflineTaskDialogViewModel.ParseResult.ParseResultStatus.Parsing:
                    return new ValidationResult(true, "解析中请稍等");
                case OfflineTaskDialogViewModel.ParseResult.ParseResultStatus.PasswordRequired:
                    return new ValidationResult(false, "需要密码");
                case OfflineTaskDialogViewModel.ParseResult.ParseResultStatus.InvalidUrl:
                    return new ValidationResult(false, "地址解析失败");
            }

            throw new InvalidOperationException("意料外的任务状态");
        }
    }
}
