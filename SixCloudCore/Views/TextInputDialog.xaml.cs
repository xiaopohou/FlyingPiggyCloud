using CustomControls.Controls;

namespace SixCloudCore.Views
{
    /// <summary>
    /// TextInputDialog.xaml 的交互逻辑
    /// </summary>
    public sealed partial class TextInputDialog : MetroWindow
    {
        private bool _result;

        private string _userInput;

        public static bool Show(out string UserInput, string hintText,string title)
        {
            TextInputDialog textInputDialog = new TextInputDialog();
            textInputDialog.Title = title;
            textInputDialog.HintAssistant.Text = hintText;
            textInputDialog.ShowDialog();
            UserInput = textInputDialog._userInput;
            return textInputDialog._result;
        }

        public static bool Show(out string UserInput,string hintText)
        {
            TextInputDialog textInputDialog = new TextInputDialog();
            textInputDialog.HintAssistant.Text = hintText;
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
