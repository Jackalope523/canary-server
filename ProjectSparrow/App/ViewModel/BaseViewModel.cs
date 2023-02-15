using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace App.ViewModel
{
    internal class BaseViewModel : INotifyPropertyChanged
    {
        private bool isBusy;
        private string title;

        public bool IsBusy 
        { 
            get => isBusy;
            set
            {
                if (isBusy == value) return;

                isBusy = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotBusy));
            }
        }
        public bool IsNotBusy => !IsBusy;
        public string Title
        {
            get => title;
            set
            {
                if (title == value) return;

                title = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));  
        }
    }
}
