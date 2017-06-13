using DigiKey.Class;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DigiKey.Models
{
    public class MainWindowModel
    {
        private KeyGenerator keyGenerator;
        private string sqlcmd = string.Empty;
        public MainWindowModel()
        {
            keyGenerator = new KeyGenerator();
        }
        /// <summary>
        /// 連線字串
        /// </summary>
        private string ConnectionString;
        /// <summary>
        /// SQL Connection 設定
        /// </summary>
        private SqlConnection sqlConnection;
        public SqlConnection SqlConnection
        {
            get
            {
                if (sqlConnection == null)
                {
                    sqlConnection = new SqlConnection(ConnectionString);
                    sqlConnection.Open();
                }
                else if (sqlConnection.State == ConnectionState.Closed)
                {
                    sqlConnection.Open();
                }
                else if (sqlConnection.State == ConnectionState.Broken)
                {
                    sqlConnection.Close();
                    sqlConnection.Open();
                }
                return sqlConnection;
            }
        }
        /// <summary>
        /// 伺服器位置
        /// </summary>
        private string serverIP;
        public string ServerIP
        {
            get
            {
                return serverIP;
            }
            set
            {
                serverIP = value;
                if (ValidatorHelper.IsIP(serverIP))
                {
                    ConnectionString = @"server=" + serverIP + @"\DigiDental;database=DigiDental;uid=sa;pwd=0939566880;Connection Timeout=1;";
                    try
                    {
                        sqlConnection = new SqlConnection(ConnectionString);
                        sqlConnection.Open();
                        CheckIPConnection = true;
                    }
                    catch
                    {
                        CheckIPConnection = false;
                    }
                }
                else
                {
                    CheckIPConnection = false;
                }
            }
        }
        /// <summary>
        /// 測試Connection是否可用
        /// </summary>
        public bool CheckIPConnection { get; set; }
        /// <summary>
        /// 機構代碼
        /// </summary>
        private string agencyCode;
        public string AgencyCode
        {
            get { return agencyCode; }
            set
            {
                agencyCode = value;
                if (CheckIPConnection && agencyCode.Length == 10)
                {
                    GetAgencySetting();
                }
                else
                {
                    IsAgencyExist = false;
                }
            }
        }
        /// <summary>
        /// 判斷Agency_Code是否已經存在
        /// </summary>
        public bool IsAgencyExist { get; set; }
        /// <summary>
        /// 是否可已驗證
        /// </summary>
        public bool IsVerify { get; set; }
        /// <summary>
        /// 試用期限
        /// </summary>
        public string TrialPeriod { get; set; }
        /// <summary>
        /// 是否為試用版
        /// </summary>
        public bool IsTry { get; set; }
        /// <summary>
        /// 加密金鑰
        /// </summary>
        public string PrivateKey { get { return keyGenerator.SHA384Encode(AgencyCode); } }
        /// <summary>
        /// 驗證碼
        /// </summary>
        public string VerificationCode { get; set; }
        /// <summary>
        /// 取得機構代碼目前設定
        /// </summary>
        private void GetAgencySetting()
        {
            sqlcmd = @"SELECT * 
                    FROM Agencys 
                    WHERE Agency_Code = @Agency_Code";
            SqlParameter[] parameters = {
                                        new SqlParameter("@Agency_Code", SqlDbType.VarChar)
                                        };
            parameters[0].Value = agencyCode;
            DataTable dt = ExecuteQuery(sqlcmd, parameters).Tables[0];
            if (dt.Rows.Count > 0)
            {
                IsAgencyExist = true;
                IsVerify = bool.Parse(dt.Rows[0]["Agency_IsVerify"].ToString());
                TrialPeriod = string.IsNullOrEmpty(dt.Rows[0]["Agency_TrialPeriod"].ToString()) ? string.Empty : DateTime.Parse(dt.Rows[0]["Agency_TrialPeriod"].ToString()).ToShortDateString();
                IsTry = bool.Parse(dt.Rows[0]["Agency_IsTry"].ToString());
            }
            else
            {
                IsAgencyExist = false;
            }
        }
        /// <summary>
        /// 建立註冊碼
        /// </summary>
        /// <returns></returns>
        public bool InsertAgency()
        {
            try
            {
                sqlcmd = @"INSERT INTO Agencys(Agency_Code, Agency_ServerIP, Agency_VerificationCode, Agency_IsVerify)
                        VALUES(@Agency_Code, @Agency_ServerIP, @Agency_VerificationCode, '1')
                            SELECT * FROM Agencys WHERE Agency_Code = @Agency_Code";
                SqlParameter[] parameters = {
                                        new SqlParameter("@Agency_Code", SqlDbType.VarChar),
                                        new SqlParameter("@Agency_ServerIP", SqlDbType.VarChar),
                                        new SqlParameter("@Agency_VerificationCode", SqlDbType.VarChar)
                                        };
                parameters[0].Value = AgencyCode;
                parameters[1].Value = ServerIP;
                parameters[2].Value = VerificationCode;
                DataTable dt = ExecuteQuery(sqlcmd, parameters).Tables[0];
                if(dt.Rows.Count > 0)
                {
                    IsAgencyExist = true;
                    IsVerify = bool.Parse(dt.Rows[0]["Agency_IsVerify"].ToString());
                    TrialPeriod = string.IsNullOrEmpty(dt.Rows[0]["Agency_TrialPeriod"].ToString()) ? string.Empty : DateTime.Parse(dt.Rows[0]["Agency_TrialPeriod"].ToString()).ToShortDateString();
                    IsTry = bool.Parse(dt.Rows[0]["Agency_IsTry"].ToString());
                }
                else
                {
                    IsAgencyExist = false;
                }
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// 更新 Agency 狀態
        /// </summary>
        /// <param name="Status"></param>
        /// <returns></returns>
        public bool UpdateAgencyStatus(string Status)
        {
            try
            {
                switch (Status)
                {
                    case "RUN"://啟用
                        sqlcmd = @"UPDATE Agencys 
                                SET Agency_IsVerify = 1,
                                    Agency_TrialPeriod = NULL,
                                    Agency_IsTry = 0,
                                    UpdateTime = DEFAULT
                                WHERE Agency_Code = @Agency_Code";
                        break;
                    case "STOP"://停用
                        sqlcmd = @"UPDATE Agencys 
                                SET Agency_IsVerify = 0,
                                    Agency_TrialPeriod = NULL,
                                    Agency_IsTry = 0,
                                    UpdateTime = DEFAULT
                                WHERE Agency_Code = @Agency_Code";
                        break;
                    case "TRY"://試用
                        sqlcmd = @"UPDATE Agencys 
                                SET Agency_IsVerify = 1,
                                    Agency_TrialPeriod = DATEADD(MONTH,1, GETDATE()),
                                    Agency_IsTry = 1,
                                    UpdateTime = DEFAULT
                                WHERE Agency_Code = @Agency_Code";
                        break;
                }
                sqlcmd += @" SELECT * FROM Agencys WHERE Agency_Code = @Agency_Code";

                SqlParameter[] parameters = {
                                        new SqlParameter("@Agency_Code", SqlDbType.VarChar)
                                        };
                parameters[0].Value = AgencyCode;
                DataTable dt = ExecuteQuery(sqlcmd, parameters).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    IsAgencyExist = true;
                    IsVerify = bool.Parse(dt.Rows[0]["Agency_IsVerify"].ToString());
                    TrialPeriod = string.IsNullOrEmpty(dt.Rows[0]["Agency_TrialPeriod"].ToString()) ? string.Empty : DateTime.Parse(dt.Rows[0]["Agency_TrialPeriod"].ToString()).ToShortDateString();
                    IsTry = bool.Parse(dt.Rows[0]["Agency_IsTry"].ToString());
                }
                else
                {
                    IsAgencyExist = false;
                }
                return true;
            }
            catch { return false; }
        }
        #region DATA ACCESS
        /// <summary>
        /// Execute sqlstring command by inline sql
        /// </summary>
        /// <param name="Sqlcmd"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected DataSet ExecuteQuery(string Sqlcmd, IDataParameter[] parameters)
        {
            DataSet dataSet = new DataSet();
            //SqlConnection.Open();
            SqlDataAdapter sqlDA = new SqlDataAdapter();
            sqlDA.SelectCommand = ExecuteQueryCommand(Sqlcmd, parameters);
            sqlDA.Fill(dataSet);
            //SqlConnection.Close();

            return dataSet;
        }
        private SqlCommand ExecuteQueryCommand(string Sqlcmd, IDataParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(Sqlcmd, SqlConnection);
            foreach (SqlParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }

            return command;
        }

        /// <summary>
        /// Execute sqlstring command by inline sql
        /// </summary>
        /// <param name="Sqlcmd"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected void ExecuteNonQuery(string Sqlcmd, IDataParameter[] parameters)
        {
            try
            {
                ExecuteNonQueryCommand(Sqlcmd, parameters);
            }
            catch
            {
                SqlConnection.Close();
            }
        }
        private int ExecuteNonQueryCommand(string Sqlcmd, IDataParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(Sqlcmd, SqlConnection);
            foreach (SqlParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
            return command.ExecuteNonQuery();
        }
        #endregion
    }
}