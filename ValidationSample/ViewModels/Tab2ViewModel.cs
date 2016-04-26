using System;

namespace ValidationSample.ViewModels
{
    public class Tab2ViewModel : ViewModelBase
    {
        public Tab2ViewModel()
        {
            AddValidationHandler("TestText2", () => String.IsNullOrEmpty(TestText2) ? "Cannot be empty" : null);
        }

        private string _testText2;        
        public string TestText2
        {
            get { return _testText2; }
            set
            {
                _testText2 = value;
                NotifyPropertyChanged("TestText2");
            }
        }
    }
}
