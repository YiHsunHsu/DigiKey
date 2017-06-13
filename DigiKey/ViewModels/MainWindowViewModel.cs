using DigiKey.Class;
using DigiKey.Commands;
using DigiKey.Models;
using DigiKey.ViewModelBases;
using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

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
                OnPropertyChanged("ServerStatus");
                OnPropertyChanged("CheckIPConnection");
            }
        }
        public string ServerStatus
        {
            get
            {
                if (!string.IsNullOrEmpty(ServerIP))
                {
                    if (CheckIPConnection)
                        return "/DigiKey;component/Resource/yes.png";
                }
                return "/DigiKey;component/Resource/no.png";
            }
        }
        public bool CheckIPConnection
        {
            get
            {
                return mainWindowModel.CheckIPConnection;
            }
        }
        public string AgencyCode
        {
            get { return mainWindowModel.AgencyCode; }
            set
            {
                mainWindowModel.AgencyCode = value;
                OnPropertyChanged("AgencyCode");
                OnPropertyChanged("Tip");
            }
        }
        public bool IsAgencyExist { get { return mainWindowModel.IsAgencyExist; } set { mainWindowModel.IsAgencyExist = value; } }
        public bool IsVerify { get { return mainWindowModel.IsVerify; } set { mainWindowModel.IsVerify = value; } }
        public string TrialPeriod { get { return mainWindowModel.TrialPeriod; } set { mainWindowModel.TrialPeriod = value; } }
        public bool IsTry { get { return mainWindowModel.IsTry; } set { mainWindowModel.IsTry = value; } }
        public string VerificationCode
        {
            get { return mainWindowModel.VerificationCode; }
            set
            {
                mainWindowModel.VerificationCode = value;
                OnPropertyChanged("VerificationCode");
            }
        }
        public string Tip
        {
            get
            {
                string TextTip = string.Empty;
                if (IsAgencyExist)
                {
                    TextTip = "機構代號:" + AgencyCode + "狀態:";
                    if (IsVerify)
                    {
                        if (IsTry)
                        {
                            TextTip += "為試用版，試用日期至" + TrialPeriod + "。";
                        }
                        else
                        {
                            TextTip += "啟用中。";
                        }
                    }
                    else
                    {
                        TextTip += "停用中。";
                    }
                }
                else
                {
                    TextTip = "機構代號:" + AgencyCode + "狀態:尚未註冊";
                }
                return TextTip;
            }
        }
        #region METHOD
        private bool CheckTextBox()
        {
            if (!string.IsNullOrEmpty(AgencyCode) && !string.IsNullOrEmpty(VerificationCode))
            {
                if(VerificationCode.Equals(mainWindowModel.PrivateKey))
                    return true;
            }
            return false;
        }
        #endregion
        #region 註冊鈕
        private RelayCommand registerCommand;
        public ICommand RegisterCommand
        {
            get
            {
                if (registerCommand == null)
                {
                    registerCommand = new RelayCommand(Register, CanRegister);
                }
                return registerCommand;
            }
        }
        private void Register()
        {
            mainWindowModel.InsertAgency();
            OnPropertyChanged("Tip");
        }
        private bool CanRegister()
        {
            if (CheckTextBox())
            {
                if (IsAgencyExist)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
        #region 啟用鈕
        private RelayCommand runCommand;
        public ICommand RunCommand
        {
            get
            {
                if (runCommand == null)
                {
                    runCommand = new RelayCommand(RunButton, CanRun);
                }
                return runCommand;
            }
        }
        private bool CanRun()
        {
            if (CheckTextBox())
            {
                if (IsAgencyExist)
                {
                    if ((IsVerify && IsTry) || !IsVerify)
                    {
                            return true;
                    }
                }
            }
            return false;
        }
        private void RunButton()
        {
            mainWindowModel.UpdateAgencyStatus("RUN");
            OnPropertyChanged("Tip");
        }
        #endregion
        #region 停用鈕
        private RelayCommand stopCommand;
        public ICommand StopCommand
        {
            get
            {
                if (stopCommand == null)
                {
                    stopCommand = new RelayCommand(StopButton, CanStop);
                }
                return stopCommand;
            }
        }
        private bool CanStop()
        {
            if (CheckTextBox())
            {
                if (IsAgencyExist)
                {
                    if (IsVerify)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private void StopButton()
        {
            mainWindowModel.UpdateAgencyStatus("STOP");
            OnPropertyChanged("Tip");
        }
        #endregion
        #region 試用鈕
        private RelayCommand tryCommand;
        public ICommand TryCommand
        {
            get
            {
                if (tryCommand == null)
                {
                    tryCommand = new RelayCommand(TryButton, CanTry);
                }
                return tryCommand;
            }
        }
        private bool CanTry()
        {
            if (CheckTextBox())
            {
                if (IsAgencyExist)
                {
                    if (IsVerify && !IsTry)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void TryButton()
        {
            mainWindowModel.UpdateAgencyStatus("TRY");
            OnPropertyChanged("Tip");
        }
        #endregion
    }
}
