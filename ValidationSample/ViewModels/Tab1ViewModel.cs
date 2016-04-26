using System;

namespace ValidationSample.ViewModels
{
    public class Tab1ViewModel : ViewModelBase
    {
        public Tab1ViewModel()
        {
            AddValidationHandler("TestText1", () => String.IsNullOrEmpty(TestText1) ? "Cannot be empty" : null);
        }

        private string _testText1;
        public string TestText1
        {
            get { return _testText1; }
            set
            {
                _testText1 = value;
                NotifyPropertyChanged("TestText1");
            }
        }
    }
}
