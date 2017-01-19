using System;
using System.Threading.Tasks;

namespace XamarinFormsOIDCSample.Client
{
    public class ActivityPageViewModel : BaseViewModel
    {
        private string _buttonText;

        public string ButtonText
        {
            get { return _buttonText; }
            set
            {
                _buttonText = value;
                // This is very important. It indicates to the app that you've changed the content of this property
                OnPropertyChanged();
            }
        }

        
    }
}