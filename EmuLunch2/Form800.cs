using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emu.Bento.Common;

namespace Emu.Bento
{
    /// <summary>
    /// バージョン情報
    /// </summary>
    public partial class Form800 : BaseForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const string DOWNLOAD_COMPLETED = "再起動すると最新バージョンに更新します";
        private const string QUERYING_UPDATE = "最新バージョンを確認中...";
        private const string NOW_DOWNLOADING = "ダウンロード中...";
        private const string NO_UPDATE = Consts.APP_TITLE + "は最新バージョンです";
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// アップデートマネージャー
        /// </summary>
        private UpdateManager _updateManager;

        /// <summary>
        /// アップデートファイルのダウンロード元URL
        /// </summary>
        private string _downloadURL = "";
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form800()
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
        private void Form820_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.icon;

                lblVersion.Text = Application.ProductVersion;
                btnDownload.Visible = false;
                lblDownloadStatus.Text = "";

                _updateManager = UpdateManager.GetInstance();
                _updateManager.DownloadStart += updateManager_DownloadStart;        // ダウンロード開始イベントをバインド
                _updateManager.DownloadComplete += updateManager_DownloadComplete;  // ダウンロード完了イベントをバインド
            }
            catch (EmuException ex)
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
        /// フォーム 初めて表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form820_Shown(object sender, EventArgs e)
        {
            try
            {
                // アップデートファイルをダウンロード済み？
                if (_updateManager.WasDownload())
                {
                    lblDownloadStatus.Text = DOWNLOAD_COMPLETED;
                    return;
                }

                // ダウンロード済みでなければ、最新版の存在を確認
                lblDownloadStatus.Text = QUERYING_UPDATE;
                HasUpdate();

            }
            catch (EmuException ex)
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
        /// フォームクローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form820_FormClosed(object sender, FormClosedEventArgs e)
        {
            // ダウンロード開始／完了イベントを削除する
            _updateManager.DownloadStart -= updateManager_DownloadStart;
            _updateManager.DownloadComplete -= updateManager_DownloadComplete;

            // ダウンロードを終了
            _updateManager.StopDownload();
        }

        /// <summary>
        /// ダウンロードボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                btnDownload.Visible = false;
                lblDownloadStatus.Text = NOW_DOWNLOADING;

                _updateManager.StartDownload(_downloadURL);
            }
            catch (EmuException ex)
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
        /// ダウンロード開始イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateManager_DownloadStart(object sender, DownloadStartEventArgs e)
        {
            // 別スレッドから呼び出されるかもしれない
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { updateManager_DownloadStart(sender, e); });
            }
            else
            {
                btnDownload.Visible = false;
                lblDownloadStatus.Text = NOW_DOWNLOADING;
            }
        }

        /// <summary>
        /// ダウンロード完了イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateManager_DownloadComplete(object sender, DownloadCompleteEventArgs e)
        {
            // 別スレッドから呼び出されるかもしれない
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { updateManager_DownloadComplete(sender, e); });
            }
            else
            {
                btnDownload.Visible = false;
                lblDownloadStatus.Text = DOWNLOAD_COMPLETED;

                History.Save("新バージョンのダウンロードを完了しました。");
            }
        }

        /// <summary>
        /// OKボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (EmuException ex)
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
        /// アップデートの有無を確認する
        /// </summary>
        private void HasUpdate()
        {
            try
            {
                log.Info("アップデート確認開始");

                bool hasUpdate = false;
                Task.Factory.StartNew(() =>
                    {
                        // アップデート確認WebAPIを呼び出す
                        hasUpdate = _updateManager.HasUpdate(ref _downloadURL);

                    }).ContinueWith(_ =>
                    {
                        // 最新版が見つかった？
                        if (hasUpdate)
                        {
                            lblDownloadStatus.Text = "";
                            btnDownload.Visible = true;
                        }
                        else
                        {
                            // 最新版が見つからなければ、現在のバージョンが最新版
                            lblDownloadStatus.Text = NO_UPDATE;
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());  // UIスレッドで動作させる
            }
            catch (EmuException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new EmuException(ex);
            }
        }
        #endregion
    }
}
