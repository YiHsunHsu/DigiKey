using DigiKey.Class;
using DigiKey.Commands;
using DigiKey.Models;
using DigiKey.ViewModelBases;
using System.Data.SqlClient;
using System.Windows.Input;

namespace DigiKey.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private MainWindowModel mainWindowModel;
        private KeyGenerator keyGenerator;
        private string _PrivateKey;
        public MainWindowViewModel()
        {
            mainWindowModel = new MainWindowModel();
            keyGenerator = new KeyGenerator();
        }
        
        public string ServerIP
        {
            get { return mainWindowModel.ServerIP; }
            set
            {
                mainWindowModel.ServerIP = value;
                OnPropertyChanged("ServerIP");
            }
        }

        public string AgencyCode
        {
            get { return mainWindowModel.AgencyCode; }
            set
            {
                mainWindowModel.AgencyCode = value;
                _PrivateKey = keyGenerator.SHA384Encode(mainWindowModel.AgencyCode);
                OnPropertyChanged("AgencyCode");
            }
        }

        public string VerificationCode
        {
            get { return mainWindowModel.VerificationCode; }
            set
            {
                mainWindowModel.VerificationCode = value;
                OnPropertyChanged("VerificationCode");
            }
        }

        private RelayCommand _register;
        public ICommand RegisterToServer
        {
            get
            {
                if (_register == null)
                {
                    _register = new RelayCommand(Register, CanRegister);
                }
                return _register;
            }
        }

        private void Register()
        {
        }

        private bool CanRegister()
        {
           if ((!string.IsNullOrEmpty(AgencyCode)) || (!string.IsNullOrEmpty(VerificationCode)))
            {
                if(!string.IsNullOrEmpty(VerificationCode))
                    return VerificationCode.Equals(_PrivateKey) ? true : false;
            }
            return false;
        }
        //private bool checkCon(string ip)
        //{
        //    SqlConnection con = new SqlConnection(@"Server=" + ip + @"\DIGIDENTAL;Database=DigiDental;User Id=sa;Password=0939566880;");
        //    try
        //    {
        //        con.Open();
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
    }
}
