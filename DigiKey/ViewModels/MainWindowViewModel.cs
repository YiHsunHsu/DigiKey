using DigiKey.Models;

namespace DigiKey.ViewModels
{
    public class MainWindowViewModel
    {
        private MainWindowModel mainWindowModel;
        public MainWindowViewModel()
        {
            mainWindowModel = new MainWindowModel();
        }
        
        public string ServerIP
        {
            get { return mainWindowModel.ServerIP; }
            set { mainWindowModel.ServerIP = value; }
        }

        public string AgencyID
        {
            get { return mainWindowModel.AgencylID; }
            set { mainWindowModel.AgencylID = value; }
        }

        public string VerificationCode
        {
            get { return mainWindowModel.VerificationCode; }
            set { mainWindowModel.VerificationCode = value; }
        }
    }
}
