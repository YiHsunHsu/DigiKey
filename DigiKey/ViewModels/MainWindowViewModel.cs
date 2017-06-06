using DigiKey.Models;
using DigiKey.ViewModelBases;
using System.Data.SqlClient;

namespace DigiKey.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private MainWindowModel mainWindowModel;
        public MainWindowViewModel()
        {
            mainWindowModel = new MainWindowModel();
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
            set { mainWindowModel.AgencyCode = value; }
        }

        public string VerificationCode
        {
            get { return mainWindowModel.VerificationCode; }
            set { mainWindowModel.VerificationCode = value; }
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
