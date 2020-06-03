using SourceChord.FluentWPF;
using System;

namespace SixCloud.Core.Views
{
    /// <summary>
    /// TextInputDialog.xaml 的交互逻辑
    /// </summary>
    public sealed partial class TextInputDialog : AcrylicWindow
    {
        private bool _result;

        private string _userInput;

        public static bool Show(out string UserInput, string hintText, string title)
        {
            TextInputDialog textInputDialog = new TextInputDialog
            {
                Title = title
            };
            //textInputDialog.HintAssistant.Text = hintText;
            textInputDialog.ShowDialog();
            UserInput = textInputDialog._userInput;
            return textInputDialog._result;
        }

        public static bool Show(out string UserInput, string hintText)
        {
            TextInputDialog textInputDialog = new TextInputDialog();
            //textInputDialog.HintAssistant.Text = hintText;
            textInputDialog.ShowDialog();
            UserInput = textInputDialog._userInput;
            return textInputDialog._result;
        }

        public static bool Show(out string UserInput)
        {
            TextInputDialog textInputDialog = new TextInputDialog();
            textInputDialog.ShowDialog();
            UserInput = textInputDialog._userInput;
            return textInputDialog._result;
        }

        private TextInputDialog()
        {
            InitializeComponent();
            if (Environment.OSVersion.Version < new Version(6, 2))
            {
                SetResourceReference(BackgroundProperty, "ImmersiveSystemAccentBrushDark2");
            }
        }

        private void Confirm_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _result = true;
            _userInput = InputBox.Text;
            Close();
        }

        private void Cancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _result = false;
            _userInput = null;
            Close();
        }
    }
}
