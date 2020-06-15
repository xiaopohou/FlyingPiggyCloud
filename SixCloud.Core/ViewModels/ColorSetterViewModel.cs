using SixCloud.Core.Controllers;
using SixCloud.Core.Views.Dialogs;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SixCloud.Core.ViewModels
{
    internal class ColorSetterViewModel : ViewModelBase
    {
        private Color newAccentColor;
        private Color newForegroundColor;
        private Color newBackgroundColor;

        private Color oldAccentColor;
        private Color oldForegroundColor;
        private Color oldBackgroundColor;

        private Window dialog;
        private bool isUserDefinedAccentColor = false;
        private bool isUserDefinedForegroundColor = false;
        private bool isUserDefinedBackgroundColor = false;

        public bool IsUserDefinedAccentColor
        {
            get => isUserDefinedAccentColor;
            set
            {
                isUserDefinedAccentColor = value;
                if (!value)
                {
                    NewAccentColor = SourceChord.FluentWPF.AccentColors.ImmersiveSystemAccent;
                }
            }
        }
        public Color NewAccentColor
        {
            get => newAccentColor;
            set
            {
                newAccentColor = value;
                ColorSetter.AccentColor = value;
            }
        }

        public bool IsUserDefinedForegroundColor
        {
            get => isUserDefinedForegroundColor;
            set
            {
                isUserDefinedForegroundColor = value;
                if (!value)
                {
                    NewForegroundColor = Color.FromArgb(0xFF, 0x5A, 0x5A, 0x5A);
                }
            }
        }
        public Color NewForegroundColor
        {
            get => newForegroundColor;
            set
            {
                newForegroundColor = value;
                ColorSetter.ForegroundColor = value;
            }
        }

        public bool IsUserDefinedBackgroundColor
        {
            get => isUserDefinedBackgroundColor;
            set
            {
                isUserDefinedBackgroundColor = value;
                if (!value)
                {
                    NewBackgroundColor = Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0);
                }
            }
        }
        public Color NewBackgroundColor
        {
            get => newBackgroundColor;
            set
            {
                newBackgroundColor = value;
                ColorSetter.BackgroundColor = value;
            }
        }

        public void InitializeComponent(Window owner)
        {
            dialog = new ColorSetterDialog(owner);
            dialog.DataContext = this;
            dialog.Show();
            if (LocalProperties.AccentColor != default)
            {
                IsUserDefinedAccentColor = true;
                OnPropertyChanged(nameof(IsUserDefinedAccentColor));
            }
            if (LocalProperties.ForegroundColor != default)
            {
                IsUserDefinedForegroundColor = true;
                OnPropertyChanged(nameof(IsUserDefinedForegroundColor));
            }
            if (LocalProperties.BackgroundColor != default)
            {
                IsUserDefinedBackgroundColor = true;
                OnPropertyChanged(nameof(IsUserDefinedBackgroundColor));
            }
        }

        public ColorSetterViewModel()
        {
            oldAccentColor = ColorSetter.AccentColor;
            newAccentColor = oldAccentColor;
            oldForegroundColor = ColorSetter.ForegroundColor;
            newForegroundColor = oldForegroundColor;
            oldBackgroundColor = ColorSetter.BackgroundColor;
            newBackgroundColor = oldBackgroundColor;

            ConfirmCommand = new DependencyCommand(Confirm);
            CancelCommand = new DependencyCommand(Cancel);
        }

        #region Commands
        public DependencyCommand ConfirmCommand { get; }
        private void Confirm(object parameter)
        {
            LocalProperties.AccentColor = IsUserDefinedAccentColor ? newAccentColor as Color? : null;
            LocalProperties.ForegroundColor = IsUserDefinedForegroundColor ? newForegroundColor as Color? : null;
            LocalProperties.BackgroundColor = IsUserDefinedBackgroundColor ? newBackgroundColor as Color? : null;
            dialog.Close();
        }

        public DependencyCommand CancelCommand { get; }
        private void Cancel(object parameter)
        {
            ColorSetter.AccentColor = oldAccentColor;
            ColorSetter.ForegroundColor = oldForegroundColor;
            ColorSetter.BackgroundColor = oldBackgroundColor;
            dialog.Close();
        }
        #endregion
    }
}
