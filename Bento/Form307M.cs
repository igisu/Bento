using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 大袋ラベル手動印刷
    /// </summary>
    public partial class Form307M : BaseForm
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 大袋ラベル印刷条件
        /// </summary>
        private BigBagLabelPrintCriteria _bigBagLabelPrintCriteria;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="bigBagLabelPrintCriteria">大袋ラベル印刷条件</param>
        public Form307M(BigBagLabelPrintCriteria bigBagLabelPrintCriteria)
        {
            InitializeComponent();

            _bigBagLabelPrintCriteria = bigBagLabelPrintCriteria;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        /// <summary>
        /// 大袋ラベル印刷種別
        /// </summary>
        public Consts.BigBagPrintKind BigBagPrintKind
        {
            get
            {
                return _bigBagLabelPrintCriteria.印刷種別ＣＤ;
            }
        }

        /// <summary>
        /// 印刷したかどうか
        /// </summary>
        public bool IsPrinted { get; set; }
        #endregion

        //**************************************
        //* イベント
        //**************************************
        #region イベント
        /// <summary>
        /// フォームロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form307M_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.lorry_go_ico;

                IsPrinted = false;

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);
                ComboUtil.SetBigBagSyokujiKubun(cboSyokujiKbn);

                // ラジオボタンを初期化
                if (_bigBagLabelPrintCriteria.印刷種別ＣＤ == Consts.BigBagPrintKind.Set)
                {
                    rdoSet.Checked = true;
                }
                else
                {
                    rdoUnit.Checked = true;
                }

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                bigBagLabelPrintCriteriaBindingSource.DataSource = _bigBagLabelPrintCriteria;
            }
            catch (BentoException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form307M_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F10:                      // 処理開始ボタン押下
                    WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する
                    btnPrint.PerformClick();
                    break;

                // 終了はフォームのCancelButtonなのでEscキーとしていたが、
                // (ボタンのクリックではなく)キー押下でフォームを閉じたときに親フォームがアクティブにならないという問題がある。
                // これを回避するには当該キー押下時に当該ボタンにフォーカスすればよいのだが、
                // CancelButtonだとイベントが発生せずEscキー押下を検知できず、ボタンにフォーカスする契機が無い。
                case Keys.Escape:
                    btnQuit.Focus();
                    btnQuit.PerformClick();
                    break;
            }
        }

        /// <summary>
        /// 印刷ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_bigBagLabelPrintCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_bigBagLabelPrintCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _bigBagLabelPrintCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 注文日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                if (_bigBagLabelPrintCriteria.注文日 > warnDate)
                {
                    if (MessageBox.Show(this, "注文日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }
                }

                //*****************************************************************
                // 大袋ラベル印刷条件とバインドしていない項目を明示的にバインドする
                //*****************************************************************
                // ラジオボタンの値を大袋ラベル印刷条件に格納する
                if (rdoSet.Checked)
                {
                    _bigBagLabelPrintCriteria.印刷種別ＣＤ = Consts.BigBagPrintKind.Set;
                }
                else
                {
                    _bigBagLabelPrintCriteria.印刷種別ＣＤ = Consts.BigBagPrintKind.Unit;
                }

                // 食事区分名をコンボボックスから取得
                _bigBagLabelPrintCriteria.食事区分名 = cboSyokujiKbn.Text.Substring(cboSyokujiKbn.Text.IndexOf(":") + 1);

                // 大袋ラベル印刷設定が存在しない？
                if (BigBagLabelSettei.HasBigBagLabelSettei((int)_bigBagLabelPrintCriteria.部署ＣＤ, _bigBagLabelPrintCriteria.印刷種別ＣＤ,
                                                            (int)_bigBagLabelPrintCriteria.食事区分ＣＤ) == false)
                {
                    if (MessageBox.Show(this, "印刷対象の設定が存在しません。\nラベルの印刷対象ではないようです。\n続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }
                }

                // 帳票に該当するプリンタを取得。未選択ならダイアログを開く。そのあと印刷処理を実行
                DecidePrinter(Consts.Reports.BigBag, PrintMain);
            }
            catch (BentoException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 終了ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuit_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (BentoException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 印刷処理
        /// </summary>
        /// <param name="printerName">プリンタ名</param>
        private void PrintMain(string printerName)
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;

                List<BigBagLabel> bigBagLabelList = new List<BigBagLabel>();
                for (int i = 0; i < _bigBagLabelPrintCriteria.印刷枚数; i++)
                {
                    BigBagLabel bigBagLabel = new BigBagLabel();
                    bigBagLabel.喫食日 = _bigBagLabelPrintCriteria.注文日;
                    bigBagLabel.大袋ラベル食事区分名 = _bigBagLabelPrintCriteria.食事区分名;
                    bigBagLabel.商品数量 = (decimal)_bigBagLabelPrintCriteria.食数;
                    bigBagLabel.食数 = (int)_bigBagLabelPrintCriteria.食数;
                    bigBagLabelList.Add(bigBagLabel);
                }

                // 大袋ラベル印刷履歴を出力
                if(SaveBigBagLabelHistory(bigBagLabelList.Count) == false)
                {
                    return;
                }

                History.Save(this.Text, "大袋ラベル手動印刷", string.Format("{0:yyyy/MM/dd} {1} {2} {3} 枚数={4}",
                            _bigBagLabelPrintCriteria.注文日, _bigBagLabelPrintCriteria.部署名, _bigBagLabelPrintCriteria.食事区分名,
                            _bigBagLabelPrintCriteria.印刷種別名, _bigBagLabelPrintCriteria.印刷枚数), History.ProcessType.Output);

                Report307A report307A = new Report307A();
                report307A.DataSource = bigBagLabelList;
                report307A.Document.Printer.DocumentName = "大袋ラベル";
                report307A.Document.Printer.PrinterName = printerName;

                // プレビューを表示
                FormPreview form = new FormPreview();
                form.MdiParent = this.MdiParent;
                form.Title = "大袋ラベル";
                form.Show();
                form.ActiveReportViewer.LoadDocument(report307A);
                form.FormClosed += new FormClosedEventHandler(formPreview_FormClosed);

                log.InfoFormat("大袋ラベル({0})を印刷しました。件数={1}", _bigBagLabelPrintCriteria.印刷種別名, bigBagLabelList.Count);

                IsPrinted = true;
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
            finally
            {
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 大袋ラベル印刷履歴を出力
        /// </summary>
        /// <param name="count">印刷枚数</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveBigBagLabelHistory(int count)
        {
            try
            {
                BigBagLabelHistory bigBagLabelHistory = new BigBagLabelHistory();
                bigBagLabelHistory.部署ＣＤ = _bigBagLabelPrintCriteria.部署ＣＤ;
                bigBagLabelHistory.大袋ラベル印刷種別ＣＤ = (int)_bigBagLabelPrintCriteria.印刷種別ＣＤ;
                bigBagLabelHistory.大袋ラベル食事区分ＣＤ = _bigBagLabelPrintCriteria.食事区分ＣＤ;
                bigBagLabelHistory.喫食日 = _bigBagLabelPrintCriteria.注文日;
                // 印刷実行番号はInsert実行時に取得している
                bigBagLabelHistory.印刷枚数 = count;

                bool ret = bigBagLabelHistory.Insert();

                if (ret == false)
                {
                    MessageBox.Show(this, bigBagLabelHistory.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // 操作履歴に保存
                History.Save(this.Text, "大袋ラベル印刷履歴", bigBagLabelHistory.Code, History.ProcessType.Create);

                return true;
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// プレビュー画面クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void formPreview_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                FormPreview form = (FormPreview)sender;
                form.Dispose();
                form = null;
            }
            catch (BentoException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
