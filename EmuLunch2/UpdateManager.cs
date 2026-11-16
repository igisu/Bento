using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Common.Encrypt;
using Ionic.Zip;
using log4net;
using Newtonsoft.Json;

namespace Emu.Bento
{
    /// <summary>
    /// ダウンロード開始イベントデータ
    /// </summary>
    /// <returns></returns>
    internal class DownloadStartEventArgs : EventArgs
    {
    }

    /// <summary>
    /// ダウンロード完了イベントデータ
    /// </summary>
    /// <returns></returns>
    internal class DownloadCompleteEventArgs : EventArgs
    {
    }

    /// <summary>
    /// ファイル置換イベントデータ（ファイルコピー時のプログレスバー反映用）
    /// </summary>
    /// <returns></returns>
    internal class FileReplacedEventArgs : EventArgs
    {
    }

    /// <summary>
    /// アップデートマネージャー
    /// </summary>
    internal class UpdateManager
    {
        //**************************************
        // 内部クラス／構造体
        //**************************************
        #region 内部クラス／構造体
        /// <summary>
        /// アップデート確認のJSONレスポンス
        /// </summary>
        private class UpdateResponse
        {
            public string result { get; set; }
            public string message { get; set; }
            public string download_url { get; set; }
            public string request_date { get; set; }
        }

        #endregion

        //**************************************
        //* 定数
        //**************************************
        #region 定数
        #endregion

        //**************************************
        //* スタティック変数
        //**************************************
        #region スタティック変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// シングルトンインスタンス
        /// </summary>
        private static UpdateManager _instance = null;
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// update.zipのフルパス
        /// </summary>
        private string _updatezipPath;

        /// <summary>
        /// Webクライアント
        /// </summary>
        private WebClient _downloadClient;

        /// <summary>
        /// ダウンロード完了フラグ
        /// </summary>
        private bool _downloadCompleted;

        /// <summary>
        /// ダウンロードがキャンセルされた場合はtrue
        /// </summary>
        private bool _downloadCencelled;

        /// <summary>
        /// ダウンロードがエラーの場合はtrue
        /// </summary>
        private Exception _downloadError;

        /// <summary>
        /// ダウンロード先のパス（一時ファイル）
        /// </summary>
        private string _downloadPath;

        /// <summary>
        /// update.zipを解凍したフォルダ
        /// </summary>
        private string _tempFolder;
        #endregion

        //**************************************
        //* イベント定義
        //**************************************
        #region イベント定義
        /// <summary>
        /// ダウンロード開始イベント
        /// </summary>
        internal event EventHandler<DownloadStartEventArgs> DownloadStart;

        /// <summary>
        /// ダウンロード完了イベント
        /// </summary>
        internal event EventHandler<DownloadCompleteEventArgs> DownloadComplete;

        /// <summary>
        /// ファイル置換イベント
        /// </summary>
        internal event EventHandler<FileReplacedEventArgs> FileReplaced;
        #endregion

        //**************************************
        //* スタティックメソッド
        //**************************************
        #region スタティックメソッド
        /// <summary>
        /// インスタンスを返します。
        /// </summary>
        /// <returns>ライセンスマネージャーのインスタンス</returns>
        internal static UpdateManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new UpdateManager();
            }
            return _instance;
        }
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        private UpdateManager()
        {
            _updatezipPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, Consts.UPDATE_ZIP);
            _downloadClient = null;
            _downloadCompleted = false;
            _downloadCencelled = false;
            _downloadError = null;
            _downloadPath = "";
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        #region ダウンロード確認
        /// <summary>
        /// アップデートファイルをダウンロード済みかどうかを返します
        /// </summary>
        /// <returns>アップデータファイルがあればtrue。無ければfalse。</returns>
        internal bool WasDownload()
        {
            return File.Exists(_updatezipPath);
        }

        /// <summary>
        /// アップデートファイルをダウンロード中かどうかを返します
        /// </summary>
        /// <returns>ダウンロード中の場合はtrue。ダウンロード開始前 または ダウンロード済みの場合はfalse。</returns>
        internal bool IsDownLoading()
        {
            // ダウンロードが開始済み かつ 未完了の場合はtrue
            if (_downloadClient != null)
            {
                if (_downloadCompleted == false)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region ダウンロード処理
        /// <summary>
        /// 新バージョンのzipファイルがあればダウンロードします。StartDownload()とは異なり、新バージョンのzipファイルの存在をサーバーに問い合わせます。
        /// </summary>
        internal void QueryAndDownload()
        {
            try
            {
                // 以下の処理を非同期で呼び出す
                Task.Factory.StartNew(() =>
                    {
                        // update.zipがあれば削除
                        // もし同名のフォルダがあれば、ファイルのコピー(★)に失敗する。更新を回避する裏技としておく。
                        if (File.Exists(_updatezipPath))
                        {
                            File.Delete(_updatezipPath);
                            log.Info("アップデートファイルを削除");
                        }

                        // アップデート確認WebAPIを呼び出す
                        string downloadURL = "";
                        if (HasUpdate(ref downloadURL) == false)
                        {
                            log.Info("アップデート無し");
                            return;
                        }

                        // アップデートがあれば非同期ダウンロード開始（一時ファイルに保存）
                        if (Download(downloadURL) == false)
                        {
                            return;
                        }

                        log.InfoFormat("ダウンロード開始 - {0}", _downloadPath);

                        // ダウンロード終了時の処理は_downloadClient_DownloadFileCompleted()にて実施
                    });
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

        /// <summary>
        /// 新バージョンのzipファイルをダウンロードします。QueryAndDownload()とは異なり、新バージョンのzipファイルの存在をサーバーに問い合わせません。
        /// </summary>
        /// <param name="downloadURL">ダウンロード元URL</param>
        internal void StartDownload(string downloadURL)
        {
            try
            {
                // 以下の処理を非同期で呼び出す
                Task.Factory.StartNew(() =>
                    {
                        // アップデートがあれば非同期ダウンロード開始（一時ファイルに保存）
                        if (Download(downloadURL) == false)
                        {
                            return;
                        }

                        log.InfoFormat("ダウンロード開始 - {0}", _downloadPath);

                        // ダウンロード終了時の処理は_downloadClient_DownloadFileCompleted()にて実施
                    });
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

        /// <summary>
        /// ダウンロードを中止する
        /// </summary>
        internal void StopDownload()
        {
            // ダウンロードが開始済み かつ 未完了の場合はキャンセルを試みる
            if (_downloadClient != null)
            {
                if (_downloadCompleted == false)
                {
                    _downloadClient.CancelAsync();
                }
            }
        }

        /// <summary>
        /// アップデート確認
        /// </summary>
        /// <param name="downloadURL">ダウンロードURLが格納される</param>
        /// <returns>アップデートがある場合はtrue。それ以外はfalse。</returns>
        internal bool HasUpdate(ref string downloadURL)
        {
            try
            {
                downloadURL = "";
                SecretData secretData = new SecretData();

                // configファイルからURLを取得して復号する
                string encryptedURL = System.Configuration.ConfigurationManager.AppSettings["UpdateURL"];
                string url = secretData.HashedDecrypt(encryptedURL);

                bool hasUpdate = false;
                WebClient downloadClient = new WebClient();
                using (Stream sr = downloadClient.OpenRead(url))
                {
                    if (downloadClient.ResponseHeaders["Last-Modified"] != null)
                    {
                        // サーバーのファイルのタイムスタンプを取得(☆)
                        string lastModified = downloadClient.ResponseHeaders["Last-Modified"];
                        DateTime urlTimeStamp = DateTime.Parse(lastModified);
                        log.DebugFormat("Last-Modified='{0}' DateTime='{1:yyyy/MM/dd HH:mm:ss}'", lastModified, urlTimeStamp);

                        // 自身のexeファイルとのタイムスタンプと比較(☆)
                        string myFilePath = Assembly.GetExecutingAssembly().Location;
                        DateTime myTimeStamp = File.GetLastWriteTime(myFilePath);
                        log.DebugFormat("myTimeStamp='{0:yyyy/MM/dd HH:mm:ss}'", myTimeStamp);

                        // サーバーのファイルが新しい？
                        if (myTimeStamp < urlTimeStamp)
                        {
                            hasUpdate = true;
                            downloadURL = url;
                        }
                    }
                }

                return hasUpdate;
            }
            catch (EmuException)
            {
                throw;
            }
            catch (System.Net.WebException wex)
            {
                log.Warn(wex);
                History.Error(wex);
                return false;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new EmuException(ex);
            }
        }

        /// <summary>
        /// ダウンロードを開始
        /// </summary>
        /// <param name="downloadURL">ダウンロード元URL</param>
        /// <returns>ダウンロードの開始が成功した場合はtrue。それ以外はfalse。</returns>
        private bool Download(string downloadURL)
        {
            try
            {
                // ダウンロード元のURL
                Uri uri = new Uri(downloadURL);

                // ダウンロード先はTempフォルダ内の一時ファイル
                _downloadPath = Path.GetTempFileName();

                if (_downloadClient == null)
                {
                    _downloadClient = new WebClient();
                    // イベントハンドラの作成
                    _downloadClient.DownloadFileCompleted += new AsyncCompletedEventHandler(_downloadClient_DownloadFileCompleted);
                }

                // 非同期ダウンロードを開始
                _downloadClient.DownloadFileAsync(uri, _downloadPath);

                // イベントを発火
                DownloadStartEventArgs downloadStartEventArgs = new DownloadStartEventArgs();
                OnDownloadStart(downloadStartEventArgs);

                return true;
            }
            catch (Exception ex)
            {
                log.WarnFormat("ダウンロードの開始に失敗 - {0}", ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// ダウンロード開始時に発生
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDownloadStart(DownloadStartEventArgs e)
        {
            // イベントがバインドされていれば、それを呼び出す
            EventHandler<DownloadStartEventArgs> handler = DownloadStart;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// 非同期ダウンロードの完了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _downloadClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            _downloadCompleted = true;
            _downloadCencelled = e.Cancelled;
            _downloadError = e.Error;

            // ダウンロードが正常終了した場合はupdate.zipとして移動する
            if (_downloadCencelled == false && _downloadError == null)
            {
                FileInfo fi = new FileInfo(_downloadPath);
                log.InfoFormat("ダウンロード終了 サイズ={0:N0}", fi.Length);
                try
                {
                    // 上書きコピー(★)
                    File.Copy(_downloadPath, _updatezipPath, true);
                    log.Info("ダウンロードファイルをコピーしました。");
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("ファイルのコピーに失敗しました。 - {0}", ex.ToString());
                }
            }
            else
            {
                log.WarnFormat("ダウンロード失敗 - {0}", _downloadError != null ? _downloadError.ToString() : "");
            }

            // 一時ファイルを削除
            try
            {
                File.Delete(_downloadPath);
                log.Info("ダウンロードファイルを削除しました。");
            }
            catch (Exception ex)
            {
                log.ErrorFormat("ダウンロードファイルの削除に失敗しました。 - {0}", ex.ToString());
            }

            // イベントを発火
            DownloadCompleteEventArgs downloadCompleteEventArgs = new DownloadCompleteEventArgs();
            OnDownloadComplete(downloadCompleteEventArgs);
        }

        /// <summary>
        /// ダウンロード完了時に発生
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDownloadComplete(DownloadCompleteEventArgs e)
        {
            // イベントがバインドされていれば、それを呼び出す
            EventHandler<DownloadCompleteEventArgs> handler = DownloadComplete;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        #region 起動時処理（解凍・更新）
        /// <summary>
        /// バックアップファイルを削除する
        /// </summary>
        /// <returns>バックアップファイルが全て削除できた場合はtrue。それ以外はfalse。</returns>
        internal bool DeleteOldFiles()
        {
            try
            {
                // インストールフォルダにある、末尾が「_old」のファイルを全て削除する
                // 読み取り専用属性があれば解除する
                DirectoryInfo di = new DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory);
                DeleteOldFiles(di);
                log.Info("バックアップファイル削除完了");
                return true;
            }
            catch (Exception ex)
            {
                log.WarnFormat("バックアップファイルの削除に失敗しました。 - {0}", ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 指定したディレクトリにあるバックアップファイルを削除する
        /// </summary>
        /// <param name="dirInfo">ディレクトリ</param>
        private void DeleteOldFiles(DirectoryInfo dirInfo)
        {
            // 基のフォルダの属性を変更
            if ((dirInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                dirInfo.Attributes = FileAttributes.Normal;
            }

            // フォルダ内の末尾が「_old」のファイルの属性を変更して削除
            foreach (FileInfo fi in dirInfo.EnumerateFiles())
            {
                if (fi.Name.EndsWith("_old"))
                {
                    if ((fi.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        fi.Attributes = FileAttributes.Normal;
                    }
                    fi.Delete();
                }
            }

            // サブフォルダの属性を回帰的に変更
            foreach (DirectoryInfo di in dirInfo.EnumerateDirectories())
            {
                DeleteOldFiles(di);
            }
        }

        /// <summary>
        /// ダウンロード済みのアップデートファイルの有無を返します
        /// </summary>
        /// <returns>アップデートファイルがある場合はtrue。それ以外はfalse。</returns>
        internal bool HasUpdate()
        {
            return File.Exists(_updatezipPath);
        }

        /// <summary>
        /// アップデートファイル(.zip)を解凍する
        /// </summary>
        /// <returns>解凍に成功した場合はtrue。それ以外はfalse。</returns>
        internal bool Extract()
        {
            try
            {
                //// アップデートファイル(.zip)を解凍するための一時フォルダをTempフォルダに作成する
                //string tempPath = Path.GetTempPath();
                // File.Replaceに指定する２つのパスが異なるドライブだと置換に失敗するので、exe自身のフォルダに一時フォルダを作成する
                string tempPath = System.AppDomain.CurrentDomain.BaseDirectory;
                string folderName = Path.GetRandomFileName();
                _tempFolder = Path.Combine(tempPath, folderName);

                // フォルダまたはファイルが存在していれば、別の名前で試す
                while (Directory.Exists(_tempFolder) || File.Exists(_tempFolder))
                {
                    folderName = Path.GetRandomFileName();
                    _tempFolder = Path.Combine(tempPath, folderName);
                }

                // フォルダを作成
                Directory.CreateDirectory(_tempFolder);

                // そこに解凍
                log.Info("解凍開始");
                using (ZipFile zip = new ZipFile(_updatezipPath, Encoding.GetEncoding("shift_jis")))
                {
                    zip.Password = PasswordGenerator.GetRP();
                    zip.ExtractAll(_tempFolder, ExtractExistingFileAction.OverwriteSilently);
                }
                log.Info("解凍完了");
                return true;
            }
            catch (BadPasswordException ex)
            {
                log.Error("パスワードが違います。解凍できません。");
                MessageBox.Show(Consts.APP_TITLE + "を更新中にエラーが発生しました。\nパスワードが違います。解凍できません。\n\n" + ex.ToString(), Consts.APP_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                log.ErrorFormat("解凍に失敗しました。 - {0}", ex.ToString());
                MessageBox.Show(Consts.APP_TITLE + "を更新中にエラーが発生しました。\n解凍に失敗しました。\n\n" + ex.ToString(), Consts.APP_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 解凍した（一時フォルダにある）ファイルの総数を返します
        /// </summary>
        /// <returns>解凍した（一時フォルダにある）ファイルの総数</returns>
        internal int GetExtractedFileCount()
        {
            return Directory.GetFiles(_tempFolder, "*", SearchOption.AllDirectories).Length;
        }

        /// <summary>
        /// ファイルをコピー（置換）する
        /// </summary>
        /// <returns>コピーに成功した場合はtrue。それ以外はfalse。</returns>
        internal bool Replace()
        {
            string targetPath = System.AppDomain.CurrentDomain.BaseDirectory;

            // コピー元フォルダに存在するファイル／フォルダをすべてコピーする
            return CopyDirectory(_tempFolder, targetPath);
        }

        /// <summary>
        /// フォルダの中身をすべてコピー（置換）する
        /// </summary>
        /// <param name="sourcePath">コピー元フォルダ名</param>
        /// <param name="destPath">コピー先フォルダ名</param>
        /// <returns>コピーに成功した場合はtrue。それ以外はfalse。</returns>
        private bool CopyDirectory(string sourcePath, string destPath)
        {
            try
            {
                string myFilePath = Assembly.GetExecutingAssembly().Location;
                DateTime myTimeStamp = File.GetLastWriteTime(myFilePath);

                // コピー先のディレクトリがないときは作る
                if (Directory.Exists(destPath) == false)
                {
                    Directory.CreateDirectory(destPath);
                }

                // コピー元のディレクトリにあるファイルを置換
                foreach (string f in Directory.GetFiles(sourcePath))    // f はフルパス
                {
                    FileInfo srcFi = new FileInfo(f);
                    srcFi.Attributes = FileAttributes.Normal;       // コピー失敗を回避すべく、コピー元ファイルの属性を解除

                    // コピー先ファイル
                    FileInfo destFi = new FileInfo(Path.Combine(destPath, Path.GetFileName(f)));
                    if (destFi.Exists)
                    {
                        destFi.Attributes = FileAttributes.Normal;  // コピー失敗を回避すべく、コピー先ファイルの属性を解除
                    }

                    // バックアップ先ファイル
                    string backupFilePath = Path.Combine(destPath, Path.GetFileName(f) + "_old");

                    // コピー先があれば置換。無ければコピー
                    if (destFi.Exists)
                    {
                        log.InfoFormat("{0}を更新中", destFi.FullName);
                        File.Replace(f, destFi.FullName, backupFilePath);
                    }
                    else
                    {
                        log.InfoFormat("{0}をコピー中", destFi.FullName);
                        File.Copy(f, destFi.FullName, true);            // 常に上書き
                    }

                    // コピーしたファイルのタイムスタンプを更新する（サーバーにあるzipファイルのタイムスタンプと比較するので）(☆)
                    if (destFi.Name == Path.GetFileName(myFilePath))
                    {
                        destFi.LastWriteTime = DateTime.Now;
                    }

                    // イベントを発火
                    FileReplacedEventArgs fileReplacedEventArgs = new FileReplacedEventArgs();
                    OnFileReplaced(fileReplacedEventArgs);
                }

                // コピー元のディレクトリにあるディレクトリについて、再帰的に呼び出す
                foreach (string d in Directory.GetDirectories(sourcePath))
                {
                    bool ret = CopyDirectory(d, Path.Combine(destPath, Path.GetFileName(d)));
                    if (ret == false)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                log.ErrorFormat("ファイルの置換に失敗しました。 - {0}", ex.ToString());
                MessageBox.Show(Consts.APP_TITLE + "を更新中にエラーが発生しました。\nファイルの置換に失敗しました。\n\n" + ex.ToString(), Consts.APP_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        internal void Finish()
        {
            try
            {
                // アップデートファイル(.zip)を削除する
                File.Delete(_updatezipPath);

                // 一時フォルダを削除
                if (string.IsNullOrEmpty(_tempFolder) == false && Directory.Exists(_tempFolder))
                {
                    // 読み取り専用があると削除できないので、それを解除
                    DirectoryInfo di = new DirectoryInfo(_tempFolder);
                    RemoveReadonlyAttribute(di);
                    di.Delete(true);
                }
            }
            catch (Exception ex)
            {
                log.WarnFormat("終了処理に失敗しました。 - {0}", ex.ToString());
            }
        }

        /// <summary>
        /// 指定したフォルダに存在するすべてのファイルの読み取り専用属性を解除
        /// </summary>
        /// <param name="dirInfo">処理対象のディレクトリ</param>
        private void RemoveReadonlyAttribute(DirectoryInfo dirInfo)
        {
            // 基のフォルダの属性を変更
            if ((dirInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                dirInfo.Attributes = FileAttributes.Normal;
            }

            // フォルダ内のすべてのファイルの属性を変更
            foreach (FileInfo fi in dirInfo.GetFiles())
            {
                if ((fi.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    fi.Attributes = FileAttributes.Normal;
                }
            }

            // サブフォルダの属性を回帰的に変更
            foreach (DirectoryInfo di in dirInfo.GetDirectories())
            {
                RemoveReadonlyAttribute(di);
            }
        }

        /// <summary>
        /// ファイルを置換した場合に発生
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnFileReplaced(FileReplacedEventArgs e)
        {
            // イベントがバインドされていれば、それを呼び出す
            EventHandler<FileReplacedEventArgs> handler = FileReplaced;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion
        #endregion

    }
}
