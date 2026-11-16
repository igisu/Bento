using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using Emu.Bento.Common;
using log4net;

namespace Emu.Bento
{
    /// <summary>
    /// 更新中ウィンドウ
    /// </summary>
    public partial class FormUpdate : BaseForm
    {
        //**************************************
        // 内部クラス／構造体
        //**************************************
        #region 内部クラス／構造体
        /// <summary>
        /// BackgroundWorker.ReportProgress の第２引数（UserState）に渡すオブジェクト
        /// </summary>
        private class MyUserState
        {
            /// <summary>
            /// ラベルに表示するメッセージ
            /// </summary>
            public string Message { get; set; }

            /// <summary>
            /// プログレスバーの上限
            /// </summary>
            public int Maximum { get; set; }

            /// <summary>
            /// プログレスバーのスタイル
            /// </summary>
            public ProgressBarStyle Style { get; set; }
        }
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// プログレスバーの現在値
        /// </summary>
        private int _progress;

        /// <summary>
        /// BackgroundWorker.ReportProgress の第２引数（UserState）に渡すオブジェクト
        /// </summary>
        private MyUserState _myUserState;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public FormUpdate()
        {
            InitializeComponent();
        }
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
        private void FormUpdate_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.icon;

                this.progressBar1.Minimum = 0;
                this.progressBar1.Maximum = 100;
                this.progressBar1.Style = ProgressBarStyle.Marquee;
                this.progressBar1.MarqueeAnimationSpeed = 50;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// フォーム 初めて表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormUpdate_Shown(object sender, EventArgs e)
        {
            this.Activate();
            // フォームが表示されたときにバックグラウンド処理を開始
            this.backgroundWorker1.RunWorkerAsync();
        }

        /// <summary>
        /// DoWorkイベントハンドラ（バックグラウンドで動作するメソッド）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = (BackgroundWorker)sender;
            _myUserState = new MyUserState();
            UpdateManager updateManager = UpdateManager.GetInstance();
            updateManager.FileReplaced += updateManager_FileReplaced;   // ファイル置換イベントをバインド
            _progress = 0;

            try
            {
                // １．圧縮ファイルを解凍
                _myUserState.Message = "解凍中...";
                _myUserState.Style = ProgressBarStyle.Marquee;      // ファイルの総数が不明なので最初はマーキー
                _myUserState.Maximum = 100;                         // 0だとマーキーが動かない
                bw.ReportProgress(_progress, _myUserState);
                if (updateManager.Extract() == false)
                {
                    // 終了処理
                    updateManager.Finish();
                    return;
                }

                // 解凍したファイルの総数を取得
                int totalFileCount = updateManager.GetExtractedFileCount();
                _myUserState.Maximum = totalFileCount;
                _myUserState.Style = ProgressBarStyle.Blocks;   // ファイル単位で進めるのでブロック

                // ２．ファイルをリプレース
                _myUserState.Message = "更新中...";
                _progress = 0;
                bw.ReportProgress(_progress, _myUserState);
                if (updateManager.Replace() == false)
                {
                    // 終了処理
                    updateManager.Finish();
                    return;
                }

                // ３．終了処理
                _myUserState.Message = "更新が完了しました。";
                bw.ReportProgress(_progress, _myUserState);
                updateManager.Finish();

                System.Threading.Thread.Sleep(2000);
            }
            finally
            {
                // イベントを削除
                updateManager.FileReplaced -= updateManager_FileReplaced;
            }
        }

        /// <summary>
        /// ファイル置換イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateManager_FileReplaced(object sender, FileReplacedEventArgs e)
        {
            _progress++;
            backgroundWorker1.ReportProgress(_progress, _myUserState);
        }

        /// <summary>
        /// ReportProgressメソッドが呼び出されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MyUserState myUserState = (MyUserState)e.UserState;

            // プログレスバーのスタイルを変更する
            if (this.progressBar1.Style != myUserState.Style)
            {
                this.progressBar1.Style = myUserState.Style;
            }

            // プログレスバーの上限を変更する
            if (this.progressBar1.Maximum != myUserState.Maximum)
            {
                this.progressBar1.Maximum = myUserState.Maximum;
            }

            // プログレスバーの値を変更する
            if (e.ProgressPercentage < this.progressBar1.Minimum)
            {
                this.progressBar1.Value = this.progressBar1.Minimum;
            }
            else if (this.progressBar1.Maximum < e.ProgressPercentage)
            {
                this.progressBar1.Value = this.progressBar1.Maximum;
            }
            else
            {
                // バーが徐々に伸びるアニメーションを無効にする
                if (this.progressBar1.Value < e.ProgressPercentage)
                {
                    // 値を増やすとき
                    if (e.ProgressPercentage < this.progressBar1.Maximum)
                    {
                        // 目的の値より一つ大きくしてから、目的の値にする
                        this.progressBar1.Value = e.ProgressPercentage + 1;
                        this.progressBar1.Value = e.ProgressPercentage;
                    }
                    else
                    {
                        // 最大値にするとき
                        // 最大値を1つ増やしてから、元に戻す
                        this.progressBar1.Maximum++;
                        this.progressBar1.Value = e.ProgressPercentage + 1;
                        this.progressBar1.Value = e.ProgressPercentage;
                        this.progressBar1.Maximum--;
                    }
                }
                else
                {
                    // 値を減らす時は、そのまま
                    this.progressBar1.Value = e.ProgressPercentage;
                }
            }

            // メッセージのテキストを変更する
            this.lblMessage.Text = myUserState.Message;
        }

        /// <summary>
        /// バックグラウンド処理が終了したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(this, Consts.APP_TITLE + "を更新中にエラーが発生しました。\n\n" + e.Error.Message, Consts.APP_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Abort;
            }
            else if (e.Cancelled)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else
            {
                this.DialogResult = DialogResult.OK;
            }

            this.Close();
        }
        #endregion
    }
}
