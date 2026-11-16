using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Text;

namespace com.emukikaku.CtiService
{
    public class IniFile
    {
        #region member
        private string _ctiSendHost;
        private int _ctiSendPort;
        private int _ctiReceivePort;
        private string _dbServer;
        private string _dbUserId;
        private string _dbPassword;
        private string _dbDatabaseName;
        private bool _outputLog;
        private string _sqlGetTelList;
        #endregion

        #region property
        public string CTISENDHOST { get { return _ctiSendHost; } }
        public int CTISENDPORT { get { return _ctiSendPort; } }
        public int CTIRECEIVEPORT { get { return _ctiReceivePort; } }
        public string DBSERVER { get { return _dbServer; } }
        public string DBUSERID { get { return _dbUserId; } }
        public string DBPASSWORD { get { return _dbPassword; } }
        public string DBDATABASENAME { get { return _dbDatabaseName; } }
        public bool OUTPUTLOG { get { return _outputLog; } }
        public string SQL_GETTELLIST { get { return _sqlGetTelList; } }
        #endregion

        #region Constructor
        public IniFile()
        {
            initMember();
            ReadIniFile();
        }
        #endregion

        #region Initialize Member
        private void initMember()
        {
            _ctiSendHost = string.Empty;
            _ctiSendPort = 0;
            _ctiReceivePort = 0;
            _dbServer = string.Empty;
            _dbUserId = string.Empty;
            _dbPassword = string.Empty;
            _dbDatabaseName = string.Empty;
            _outputLog = true;
            _sqlGetTelList = string.Empty;
        }
        #endregion

        #region INIファイル読み込み
        /// <summary>==============================================================================
        /// INIファイルの読み込み
        /// </summary>
        private void ReadIniFile()
        {
            string file_name = "CtiService.xml";

            if (!File.Exists(file_name)) return;  // ファイルの有無をチェック

            using (XmlReader xmlReader = XmlReader.Create(file_name))
            {

                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element)
                    {
                        switch (xmlReader.LocalName)
                        {
                            case "DBSERVER":
                                this._dbServer = xmlReader.ReadString();
                                break;

                            case "DBUSERNAME":
                                this._dbUserId = xmlReader.ReadString();
                                break;

                            case "DBPASSWORD":
                                this._dbPassword = xmlReader.ReadString();
                                break;

                            case "DBNAME":
                                this._dbDatabaseName = xmlReader.ReadString();
                                break;

                            case "CTISENDHOST":
                                this._ctiSendHost = xmlReader.ReadString();
                                break;

                            case "CTISENDPORT":
                                this._ctiSendPort = Utility.toInt(xmlReader.ReadString());
                                break;

                            case "CTIRECEIVEPORT":
                                this._ctiReceivePort = Utility.toInt(xmlReader.ReadString());
                                break;

                            case "OUTPUTLOG":
                                this._outputLog = Utility.toBool(xmlReader.ReadString());
                                break;

                            case "GETSQL_TELLIST":
                                this._sqlGetTelList = xmlReader.ReadString();
                                break;
                        }
                    }
                }
            }
        }
        #endregion
    }
}
