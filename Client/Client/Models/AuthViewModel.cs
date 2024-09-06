using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Client.Models
{
    public class AuthViewModel : INotifyPropertyChanged
    {
        private ICommand register;
        private ICommand login;
        private Window window;

        public ICommand Register { get => register; }
        public ICommand Login { get => login; }

        public string? LoginUsername { get; set; }
        public string? LoginPassword { private get; set; } 
        public string? RegisterUsername { get; set; }
        public string? RegisterPassword { private get; set; }
        public string? RegisterPasswordConfirm { private get; set; }


        public event PropertyChangedEventHandler? PropertyChanged;

        public AuthViewModel(Window window)
        {
            register = new CommandHandler(RegisterUser, CanRegisterUser);
            login = new CommandHandler(LoginUser, CanLoginUser);
            this.window = window;
        }

        private async Task RegisterUser()
        {
            await ConnectionHandler.Instance.Register(RegisterUsername, RegisterPassword, msg => MessageBox.Show(msg));
        }

        private bool CanRegisterUser()
        {
            return !string.IsNullOrEmpty(RegisterUsername) 
                && !string.IsNullOrEmpty(RegisterPassword)
                && !string.IsNullOrEmpty(RegisterPasswordConfirm) && RegisterPassword == RegisterPasswordConfirm;
        }

        private async Task LoginUser()
        {
            await ConnectionHandler.Instance.Login(LoginUsername, LoginPassword, (msg, success) =>
            {
                MessageBox.Show(msg);
                if(success)
                {
                    DataWindow dataWindow = new DataWindow();
                    dataWindow.Show();
                    window.Close();
                }
            });
        }

        private bool CanLoginUser()
        {
            return !string.IsNullOrEmpty(LoginUsername)
                && !string.IsNullOrEmpty(LoginPassword);
        }
    }
}
