using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace com.emukikaku.CtiService
{
    public partial class frmTelList : Form
    {
        #region member
        private DialogResult _ChoiceResult;
        private DBAccessor _dbAccessor;
        private IniFile _iniFile;

        private string _customerName;
        private string _telNo;
        #endregion

        #region Property
        public DialogResult ChoiceResult
        {
            get
            {
                return _ChoiceResult;
            }
        }

        public string CustomerName
        {
            get
            {
                return _customerName;
            }
        }

        public string TelNo
        {
            get
            {
                return _telNo;
            }
        }
        #endregion

        #region Constructor
        public frmTelList(DBAccessor dbAccessor)
        {
            InitializeComponent();

            _dbAccessor = dbAccessor;
            dgvList.DataSource = new CtiServiceDataSet.TelListDataTable();
            _ChoiceResult = DialogResult.No;

            clear();
        }
        #endregion

        #region 電話番号リスト読み込み
        private CtiServiceDataSet.TelListDataTable readTelList(string customerNo,string customerName, string telNo)
        {
            try
            {
                _dbAccessor.open();
            }
            catch (Exception ex)
            {
                writeLog("readTelList", ex.ToString());
                return null;
            }
            string sql = string.Empty;
            try
            {
                sql = @"
                    WITH BASEDATA AS (
                      {0}
                    )
                    SELECT
                      BASE.CUSTOMER_NO
                      ,BASE.CUSTOMER_NAME
                      ,BASE.TELNO
                    FROM
                      BASEDATA BASE
                    {1}
                    ORDER BY
                      BASE.CUSTOMER_NO
                      ,BASE.CUSTOMER_NAME
                      ,BASE.TELNO
                    ";

                List<string> lstWhere = new List<string>();

                if (!customerNo.Equals(string.Empty))
                {
                    lstWhere.Add(string.Concat(" AND BASE.CUSTOMER_NO LIKE '%", customerNo, "%'"));
                }
                if (!customerName.Equals(string.Empty))
                {
                    lstWhere.Add(string.Concat(" AND BASE.CUSTOMER_NAME LIKE '%", customerName, "%'"));
                }
                if (!telNo.Equals(string.Empty))
                {
                    lstWhere.Add(string.Concat(" AND BASE.TELNO LIKE '%", telNo, "%'"));
                }
                string whereStatement = string.Empty;
                foreach (string where in lstWhere)
                {
                    whereStatement = string.Concat(whereStatement, where);
                }
                if (!whereStatement.Equals(string.Empty))
                {
                    whereStatement = string.Concat("WHERE ", whereStatement.Substring(4));
                }

                sql = string.Format(sql, _iniFile.SQL_GETTELLIST, whereStatement);
                CtiServiceDataSet.TelListDataTable dt = new CtiServiceDataSet.TelListDataTable();
                dt.Merge(_dbAccessor.fill(sql));

                return dt;
            }
            catch (Exception ex)
            {
                writeLog("readTelList", ex.ToString());
                writeLog("readTelList", sql);
                return null;
            }
            finally
            {
                _dbAccessor.close();
            }
        }
        #endregion

        #region クリア
        private void clear()
        {
            txtWhereCustomerNo.Text = string.Empty;
            txtWhereCustomerName.Text = string.Empty;
            txtWhereCustomerTelNo.Text = string.Empty;
            CtiServiceDataSet.TelListDataTable dt = (CtiServiceDataSet.TelListDataTable)dgvList.DataSource;
            dt.Clear();

        }
        #endregion

        #region 検索ボタン押下
        private void btnSearch_Click(object sender, EventArgs e)
        {
            CtiServiceDataSet.TelListDataTable dt = (CtiServiceDataSet.TelListDataTable)dgvList.DataSource;
            dt.Clear();

            CtiServiceDataSet.TelListDataTable dtResult = readTelList(txtWhereCustomerNo.Text,txtWhereCustomerName.Text,txtWhereCustomerTelNo.Text);
            if (dtResult != null && !dtResult.Rows.Count.Equals(0))
            {
                dt.Merge(dtResult);
                dgvList.Focus();
            }
        }
        #endregion

        #region キャンセルボタン
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region OK
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (dgvList.CurrentRow == null)
            {
                showMessage("行が選択されていません。");
                return;
            }

            choiceCustomer((CtiServiceDataSet.TelListRow)((DataRowView)dgvList.CurrentRow.DataBoundItem).Row);
        }
        #endregion

        #region メッセージ表示
        private DialogResult showMessage(string message)
        {
            return showMessage(message, MessageBoxButtons.OK);
        }

        private DialogResult showMessage(string message, MessageBoxButtons buttons)
        {
            return MessageBox.Show(message, "CTI Service", buttons);
        }
        #endregion

        #region メンバ設定
        public void setDBAccessor(DBAccessor dbaccessor)
        {
            _dbAccessor = dbaccessor;
        }

        public void setIniFile(IniFile iniFile)
        {
            _iniFile = iniFile;
        }
        #endregion

        #region WriteLog
        private void writeLog(string tag, string message)
        {
            if (!_iniFile.OUTPUTLOG) return;
            Utility.writeLog(tag, message);
        }
        #endregion

        #region 得意先選択
        private void dgvList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvList.CurrentRow == null) return;
            choiceCustomer((CtiServiceDataSet.TelListRow)((DataRowView)dgvList.CurrentRow.DataBoundItem).Row);
        }
        #endregion

        #region 全体共通キー検出
        private void all_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }
        #endregion

        #region キー検出
        private void dgvList_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Enter:
                    if (dgvList.CurrentRow == null) return;
                    choiceCustomer((CtiServiceDataSet.TelListRow)((DataRowView)dgvList.CurrentRow.DataBoundItem).Row);
                    break;
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }
        #endregion

        #region 顧客決定
        private void choiceCustomer(CtiService.CtiServiceDataSet.TelListRow row)
        {
            _customerName = row.CUSTOMER_NAME;
            _telNo = row.TELNO;
            _ChoiceResult = DialogResult.OK;
            this.Close();
        }
        #endregion
    }
}