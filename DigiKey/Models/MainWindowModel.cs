namespace DigiKey.Models
{
    public class MainWindowModel
    {
        /// <summary>
        /// 診所伺服器位置
        /// </summary>
        public string ServerIP { get; set; }
        /// <summary>
        /// 機構代碼
        /// </summary>
        public string AgencylID { get; set; }
        /// <summary>
        /// 驗證碼
        /// </summary>
        public string VerificationCode { get; set; }
    }
}