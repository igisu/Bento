using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 得意先マスタ詳細
    /// </summary>
    public partial class Form700D : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        // タブのインデックス
        private const int TAB_INDEX_KIHON    = 0;   // 基本情報タブ
        private const int TAB_INDEX_SHIHARAI = 1;   // 支払情報タブ
        private const int TAB_INDEX_BUNPAI   = 2;   // 分配先タブ
        private const int TAB_INDEX_FUZOKU   = 3;   // 付属情報タブ
        private const int TAB_INDEX_TANKA    = 4;   // 得意先単価タブ
        private const int TAB_INDEX_HAISO    = 5;   // 注文元タブ

        // （基本情報）CTI連携・電話番号登録
        private const int TEL_KBN_COLUMN    = 0;   // 区分列インデックス
        private const int TEL_NUMBER_COLUMN = 1;   // 電話番号ボタン列インデックス
        private const int TEL_REMARK_COLUMN = 2;   // 備考列インデックス
        private const int TEL_DELETE_COLUMN = 3;   // 削除ボタン列インデックス

        // （支払情報）振込依頼人名
        private const int FURI_NAME_COLUMN   = 0;   // 振込依頼人名列インデックス
        private const int FURI_DELETE_COLUMN = 1;   // 削除ボタン列インデックス

        // （分配先）分配先
        private const int BUNPAI_TOKUISAKI_CD_COLUMN   = 0;     // 得意先ＣＤ列インデックス
        private const int BUNPAI_REFERENCE_COLUMN      = 1;     // 参照ボタン列インデックス
        private const int BUNPAI_TOKUISAKI_NAME_COLUMN = 2;     // 得意先名列インデックス
        private const int BUNPAI_TOKUISAKI_KANA_COLUMN = 3;     // 得意先名カナ列インデックス
        private const int BUNPAI_TOKUISAKI_TEL_COLUMN  = 4;     // 電話番号列インデックス
        private const int BUNPAI_BUSYO_COLUMN          = 5;     // 部署名列インデックス
        private const int BUNPAI_DELETE_COLUMN         = 6;     // 削除ボタン列インデックス
        private const int BUNPAI_INVALID_COLUMN        = 7;     // 無効フラグ列インデックス

        // （得意先単価）得意先単価
        private const int TANKA_SYOHIN_CD_COLUMN      = 0;    // 商品ＣＤ列インデックス
        private const int TANKA_REFERENCE_COLUMN      = 1;    // 参照ボタン列インデックス
        private const int TANKA_SYOHIN_NAME_COLUMN    = 2;    // 商品名列インデックス
        private const int TANKA_SYOHIN_RYAKU_COLUMN   = 3;    // 商品名略称列インデックス
        private const int TANKA_BUSYO_GROUP_COLUMN    = 4;    // 部署グループ列インデックス
        private const int TANKA_HYOJUN_KOMI_COLUMN    = 5;    // 標準単価税込列インデックス
        private const int TANKA_HYOJUN_NUKI_COLUMN    = 6;    // 標準単価税抜列インデックス
        private const int TANKA_TOKUISAKI_KOMI_COLUMN = 7;    // 得意先単価税込列インデックス
        private const int TANKA_TOKUISAKI_NUKI_COLUMN = 8;    // 得意先単価税抜列インデックス
        private const int TANKA_INVALID_COLUMN        = 9;    // 無効フラグ列インデックス
        private const int TANKA_DELETE_COLUMN         = 10;   // 削除ボタン列インデックス

        // （注文元）注文元
        private const int CHUMONMOTO_TOKUISAKI_CD_COLUMN   = 0; // 得意先ＣＤ列インデックス
        private const int CHUMONMOTO_TOKUISAKI_NAME_COLUMN = 1; // 得意先名列インデックス
        private const int CHUMONMOTO_TOKUISAKI_KANA_COLUMN = 2; // 得意先名カナ列インデックス
        private const int CHUMONMOTO_TOKUISAKI_TEL_COLUMN  = 3; // 電話番号列インデックス
        private const int CHUMONMOTO_BUSYO_COLUMN          = 4; // 部署名列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 保存用得意先マスタ
        /// </summary>
        private Tokuisaki _tokuisaki;

        /// <summary>
        /// 初期状態用得意先マスタ
        /// </summary>
        private Tokuisaki _origin;

        /// <summary>
        /// （今日時点の）消費税
        /// </summary>
        private Syohizei _syohizei;

        /// <summary>
        /// 新規レコードか否か
        /// </summary>
        private bool _IsNewRecord;

        /// <summary>
        /// コピーして新規登録の場合はtrue
        /// </summary>
        private bool _IsCopy;

        /// <summary>
        /// データソース列値変更イベント実行中フラグ
        /// </summary>
        private bool _isDtColumnChanging = false;

        /// <summary>
        /// Spreadにて非編集モードから編集モードへのF2キーが押下されたか
        /// </summary>
        private bool _isF2forSpreadEdit = false;

        /// <summary>
        /// スプレッドのコンボボックス（F2,Enter制御用）
        /// </summary>
        private FarPoint.Win.FpCombo _fpCombo;

        /// <summary>
        /// スプレッドのコンボボックスでEnterキーが押下されたか
        /// </summary>
        private bool _isEnterforFpCombo = false;

        /// <summary>
        /// 得意先履歴画面
        /// </summary>
        private Form700DH _formRireki = null;

        /// <summary>
        /// コース一覧画面
        /// </summary>
        private Form700DC _formCourse = null;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="tokuisaki">得意先マスタ</param>
        public Form700D(Tokuisaki tokuisaki) : this(tokuisaki, false)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="tokuisaki">得意先マスタ</param>
        /// <param name="isCopy">コピーして新規登録の場合はtrue</param>
        public Form700D(Tokuisaki tokuisaki, bool isCopy)
        {
            try
            {
                InitializeComponent();

                // データをいったんコピーしておく
                _tokuisaki = new Tokuisaki(tokuisaki);

                _IsNewRecord = true;
                _IsCopy = isCopy;
                if (_tokuisaki.得意先ＣＤ != null)
                {
                    _IsNewRecord = isCopy;
                }
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
        //* イベント
        //**************************************
        #region イベント
        /// <summary>
        /// フォームロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form700D_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.database_gear_ico;

                //*********************************************************
                // 以下の項目は必須とはせずに未入力の警告のみとする。
                // ・得意先名
                // ・売掛現金区分
                // ・税区分
                // ・丸め処理区分
                // ※ホントは必須にすべきだが、未登録の電話番号からの着信で
                //   新規得意先として登録する際は必須にはしていないので。
                //*********************************************************
                errorProvider2.Icon = Properties.Resources.warning_ico;     // 未入力警告表示用ErrorProviderのアイコンを変更

                // 未入力警告のアイコンのみを表示したいので、必須入力エラーアイコンを消す必要がある。
                // しかしその手段が無いので、アイコンの位置を遠くにすることで非表示に見せる。
                errorProvider1.SetIconPadding(txtTokuisakiName, this.Width * (-1));
                errorProvider1.SetIconPadding(cboUrigen, this.Width * (-1));
                errorProvider1.SetIconPadding(cboZei, this.Width * (-1));
                errorProvider1.SetIconPadding(cboMarume, this.Width * (-1));

                // 反対に必須入力エラーアイコンを表示したい項目については、未入力警告のアイコンを非表示にする
                errorProvider2.SetIconPadding(txtTokuisakiCD, this.Width * (-1));
                errorProvider2.SetIconPadding(cboBusyo, this.Width * (-1));
                errorProvider2.SetIconPadding(txtHaisoCD, this.Width * (-1));

                // 権限によりボタンを制御
                btnSave.Enabled = this.HasAuthEdit;
                btnDelete.Enabled = this.HasAuthEdit;
                btnCopy.Enabled = this.HasAuthEdit;

                //*****************
                // スプレッドの設定
                //*****************
                // （※）デザイン時にExcel互換の入力マップを読み込んでいる
                // Spreadデフォルトの動作は、非編集モードでの入力は常にセルへの挿入なので、これを上書きにする
                fpSpread_Tel.EditModeReplace = true;                           // 編集開始時に入力済みデータを選択状態にします(☆)
                // この設定により、非編集モード時のF2キー押下でセル値が選択されるようになった。
                // しかし、ExcelのF2キーの動作はセル値の末尾にカーソルがあるので、Spreadでも同様に動作にすべく
                // F2キーのデフォルト動作を無効にして処理を実装する
                FarPoint.Win.Spread.InputMap im = fpSpread_Tel.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused);  // 非編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);   // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                //im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生しない）

                im = fpSpread_Tel.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenAncestorOfFocused);                     // 編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生する）

                // CTI連携・電話番号登録スプレッドの区分コンボボックス列を作成
                FarPoint.Win.Spread.CellType.ComboBoxCellType cbKubun = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
                ComboUtil.Set(cbKubun,
                    new string[] { "主", "副" },
                    new bool[] {false, true});
                cbKubun.AutoCompleteSource = AutoCompleteSource.ListItems;
                cbKubun.AutoCompleteMode = AutoCompleteMode.Suggest;
                fpSpread_Tel_Sheet1.Columns[TEL_KBN_COLUMN].CellType = cbKubun;

                // （※）デザイン時にExcel互換の入力マップを読み込んでいる
                // Spreadデフォルトの動作は、非編集モードでの入力は常にセルへの挿入なので、これを上書きにする
                fpSpread_Furikomi.EditModeReplace = true;                           // 編集開始時に入力済みデータを選択状態にします(☆)
                // この設定により、非編集モード時のF2キー押下でセル値が選択されるようになった。
                // しかし、ExcelのF2キーの動作はセル値の末尾にカーソルがあるので、Spreadでも同様に動作にすべく
                // F2キーのデフォルト動作を無効にして処理を実装する
                im = fpSpread_Furikomi.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused);  // 非編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);   // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                //im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生しない）

                im = fpSpread_Furikomi.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenAncestorOfFocused);                     // 編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生する）

                // （※）デザイン時にExcel互換の入力マップを読み込んでいる
                // Spreadデフォルトの動作は、非編集モードでの入力は常にセルへの挿入なので、これを上書きにする
                fpSpread_Bunpai.EditModeReplace = true;                           // 編集開始時に入力済みデータを選択状態にします(☆)
                // この設定により、非編集モード時のF2キー押下でセル値が選択されるようになった。
                // しかし、ExcelのF2キーの動作はセル値の末尾にカーソルがあるので、Spreadでも同様に動作にすべく
                // F2キーのデフォルト動作を無効にして処理を実装する
                im = fpSpread_Bunpai.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused);  // 非編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);   // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                //im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生しない）

                im = fpSpread_Bunpai.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenAncestorOfFocused);                     // 編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生する）

                // （※）デザイン時にExcel互換の入力マップを読み込んでいる
                // Spreadデフォルトの動作は、非編集モードでの入力は常にセルへの挿入なので、これを上書きにする
                fpSpread_TokuisakiTanka.EditModeReplace = true;                           // 編集開始時に入力済みデータを選択状態にします(☆)
                // この設定により、非編集モード時のF2キー押下でセル値が選択されるようになった。
                // しかし、ExcelのF2キーの動作はセル値の末尾にカーソルがあるので、Spreadでも同様に動作にすべく
                // F2キーのデフォルト動作を無効にして処理を実装する
                im = fpSpread_TokuisakiTanka.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused);  // 非編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);   // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                //im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生しない）

                im = fpSpread_TokuisakiTanka.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenAncestorOfFocused);                     // 編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生する）

                // （※）デザイン時にExcel互換の入力マップを読み込んでいる
                // Spreadデフォルトの動作は、非編集モードでの入力は常にセルへの挿入なので、これを上書きにする
                fpSpread_ChumonMoto.EditModeReplace = true;                           // 編集開始時に入力済みデータを選択状態にします(☆)
                // この設定により、非編集モード時のF2キー押下でセル値が選択されるようになった。
                // しかし、ExcelのF2キーの動作はセル値の末尾にカーソルがあるので、Spreadでも同様に動作にすべく
                // F2キーのデフォルト動作を無効にして処理を実装する
                im = fpSpread_ChumonMoto.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused);  // 非編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);   // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                //im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生しない）

                im = fpSpread_ChumonMoto.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenAncestorOfFocused);                     // 編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生する）

                cboBusyo.SelectedValueChanged -= cboBusyo_SelectedValueChanged;     // 部署ＣＤコンボイベントをいったん削除

                // コンボボックスの選択肢生成
                ComboUtil.SetJotaiKubun(cboJotai);
                ComboUtil.SetJotaiRiyuKubun(cboJotaiRiyu);
                ComboUtil.SetBusyo(cboBusyo);
                ComboUtil.SetUriGenKubun(cboUrigen);
                ComboUtil.SetZeiKubun(cboZei);
                ComboUtil.SetMarumeKubun(cboMarume);
                ComboUtil.SetShimeKubun(cboShime);
                ComboUtil.Set(cboSyushimeYobi,
                            new string[] { "",
                                           string.Format("{0}:日曜日", (int)DayOfWeek.Sunday),
                                           string.Format("{0}:月曜日", (int)DayOfWeek.Monday),
                                           string.Format("{0}:火曜日", (int)DayOfWeek.Tuesday),
                                           string.Format("{0}:水曜日", (int)DayOfWeek.Wednesday),
                                           string.Format("{0}:木曜日", (int)DayOfWeek.Thursday),
                                           string.Format("{0}:金曜日", (int)DayOfWeek.Friday),
                                           string.Format("{0}:土曜日", (int)DayOfWeek.Saturday) },
                            new int[] { -1,
                                        (int)DayOfWeek.Sunday,
                                        (int)DayOfWeek.Monday,
                                        (int)DayOfWeek.Tuesday,
                                        (int)DayOfWeek.Wednesday,
                                        (int)DayOfWeek.Thursday,
                                        (int)DayOfWeek.Friday,
                                        (int)DayOfWeek.Saturday});
                ComboUtil.SetShiharaiSight(cboShiharaiSight);
                ComboUtil.SetShiharaiHoho(cboShiharaiHoho);
                ComboUtil.SetKozaSyubetsu(cboKozaSyubetsu);
                ComboUtil.SetJuchuHoho(choJucyuHoho);
                ComboUtil.SetYokiKaisyu(cboKaisyu);
                ComboUtil.SetKeisyo(cboKeisyo);

                // 新規の場合は削除不可
                if (_IsNewRecord)
                {
                    lblMode.Text = MODE_CAPTION_NEW;
                    lblMode.BackColor = BACK_COLOR_NEW;
                    btnSave.Text = "登録(F9)";
                    btnDelete.Enabled = false;
                    btnCopy.Enabled = false;
                    btnRireki.Enabled = false;      // 履歴表示も不可
                    btnCourse.Enabled = false;      // コース表示も不可

                    // 状態の初期値を30:未承認にする
                    _tokuisaki.状態区分 = (int)Consts.JotaiKubun.MiSyonin;

                    if (_IsCopy == false)
                    {
                        // 部署CDコンボの初期値を設定
                        _tokuisaki.部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
                    }
                }
                else
                {
                    lblMode.Text = MODE_CAPTION_EDIT;
                    lblMode.BackColor = BACK_COLOR_EDIT;
                    btnSave.Text = "更新(F9)";
                    // 既存の場合は得意先ＣＤ変更不可
                    txtTokuisakiCD.Enabled = false;
                    btnAkiBango.Enabled = false;
                }

                // ゆうちょ以外の場合は記号番号を非活性にする
                // ※Tokuisakiクラスの中でクリアする
                if (_tokuisaki.金融機関ＣＤ != Consts.YUCHO)
                {
                    txtKigoBango.Enabled = false;
                }

                // 分配先スプレッドの最終列（無効フラグ）を非表示にする
                fpSpread_Bunpai_Sheet1.Columns[BUNPAI_INVALID_COLUMN].Visible = false;

                // 消費税を取得
                _syohizei = Syohizei.GetSyohizei(DateTime.Today);

                // 振替用EB口座の選択肢を、部署にぶら下がるものに再設定する
                SetBusyoEB(cboEBKoza, _tokuisaki.部署ＣＤ);

                //*****************************************************************
                // 「コピーして新規作成」の場合、以下の一覧は空データを取得する
                // ・CTI連携・電話番号
                // ・振込依頼人名
                // ・分配先
                // ・注文元
                // ※つまり得意先単価だけはコピー元の得意先ＣＤのレコードを取得する
                //*****************************************************************

                // CTI連携・電話番号を取得
                GetTelToCust(_tokuisaki.得意先ＣＤ);

                // 振込依頼人名を取得
                GetTokuisakiFurikomiIraiNin(_tokuisaki.得意先ＣＤ);

                // 分配先を取得
                GetBunpai(_tokuisaki.得意先ＣＤ);

                // 得意先単価を取得
                GetTokuisakiTanka(_tokuisaki.得意先ＣＤ);

                // 注文元を取得
                GetChumonMoto(_tokuisaki.得意先ＣＤ);

                // コピーして新規作成の場合は得意先マスタのコピー不要の項目をクリア
                if (_IsCopy)
                {
                    _tokuisaki.stopOverwrite = false;
                    _tokuisaki.得意先ＣＤ = null;       // コピー元の得意先ＣＤをクリアする前に、これを条件とする必要なデータは取得済み
                    _tokuisaki.得意先ＣＤorg = null;
                    _tokuisaki.状態区分 = (int)Consts.JotaiKubun.MiSyonin;      // 状態の初期値を30:未承認にする
                    _tokuisaki.承認日 = null;
                    _tokuisaki.承認者ＣＤ = null;
                    _tokuisaki.承認者名 = null;
                    _tokuisaki.状態理由区分 = null;
                    _tokuisaki.失客日 = null;
                    _tokuisaki.請求先ＣＤ = null;
                    _tokuisaki.請求先名 = null;
                    _tokuisaki.支払方法 = null;
                    _tokuisaki.EB銀行支店ＣＤ = null;
                    _tokuisaki.振込手数料フラグ = false;
                    _tokuisaki.部署合算請求フラグ = false;
                    _tokuisaki.金融機関ＣＤ = null;
                    _tokuisaki.金融機関名 = null;
                    _tokuisaki.金融機関支店ＣＤ = null;
                    _tokuisaki.金融機関支店名 = null;
                    _tokuisaki.記号番号 = null;
                    _tokuisaki.口座種別 = null;
                    _tokuisaki.口座番号 = null;
                    _tokuisaki.口座名義人 = null;
                    _tokuisaki.チケット枚数 = null;
                    _tokuisaki.サービスチケット枚数 = null;
                    _tokuisaki.営業担当者ＣＤ = null;
                    _tokuisaki.営業担当者名 = null;
                    _tokuisaki.獲得営業者ＣＤ = null;
                    _tokuisaki.獲得営業者名 = null;
                    _tokuisaki.登録担当者ＣＤ = null;
                    _tokuisaki.登録担当者名 = null;
                    _tokuisaki.受注得意先ＣＤ = null;
                    _tokuisaki.受注得意先名 = null;
                    _tokuisaki.受注方法 = null;
                    _tokuisaki.電話確認時間_時 = null;
                    _tokuisaki.電話確認時間_分 = null;
                    _tokuisaki.配送得意先ＣＤ = null;
                    _tokuisaki.配送先名 = null;
                    _tokuisaki.味噌汁区分 = null;
                    _tokuisaki.ふりかけ区分 = null;
                    _tokuisaki.祝日配送区分 = null;
                    _tokuisaki.納品書発行区分 = null;
                    _tokuisaki.空き容器回収区分 = null;
                    _tokuisaki.請求書敬称 = null;
                    _tokuisaki.誕生日1 = null;
                    _tokuisaki.誕生日2 = null;
                    _tokuisaki.休日設定 = null;
                    _tokuisaki.集計ＣＤ = null;
                    _tokuisaki.備考１ = null;
                    _tokuisaki.備考２ = null;
                    _tokuisaki.備考３ = null;
                    _tokuisaki.受注時特記事項 = null;
                    _tokuisaki.作成担当者ＣＤ = null;
                    _tokuisaki.作成担当者名 = null;
                    _tokuisaki.作成日 = null;
                    _tokuisaki.修正担当者ＣＤ = null;
                    _tokuisaki.修正担当者名 = null;
                    _tokuisaki.修正日 = null;
                }

                // そもそも自分が分配先（＝受注得意先ＣＤが指定されている）なら、分配先スプレッドは非活性にする（分配先の分配先を指定できないようにする）(△)
                if (_tokuisaki.受注得意先ＣＤ != null)
                {
                    fpSpread_Bunpai.Enabled = false;
                }

                // 初期状態として保持
                _origin = new Tokuisaki(_tokuisaki);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                tokuisakiBindingSource.DataSource = _tokuisaki;
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
            finally
            {
                Application.DoEvents();         // バインドしたことで発生するSelectedValueChangedをメッセージキューから消す
                cboBusyo.SelectedValueChanged +=  new EventHandler(cboBusyo_SelectedValueChanged);   // 部署ＣＤコンボイベントを追加
            }
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form700D_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F4:                       // 得意先履歴ボタン押下
                    btnRireki.PerformClick();
                    break;

                case Keys.F7:                       // コース一覧ボタン押下
                    btnCourse.PerformClick();
                    break;

                case Keys.F9:                       // 更新ボタン押下
                    // スプレッドが編集途中の場合は強制的に編集を終了(確定)する
                    if (fpSpread_Tel.EditMode == true)
                    {
                        fpSpread_Tel.StopCellEditing();
                        Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                    }
                    if (fpSpread_Furikomi.EditMode == true)
                    {
                        fpSpread_Furikomi.StopCellEditing();
                        Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                    }
                    if (fpSpread_Bunpai.EditMode == true)
                    {
                        fpSpread_Bunpai.StopCellEditing();
                        Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                    }
                    if (fpSpread_TokuisakiTanka.EditMode == true)
                    {
                        fpSpread_TokuisakiTanka.StopCellEditing();
                        Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                    }

                    WriteBinding(this.ActiveControl);   // 入力中の内容を強制的に反映する

                    btnSave.PerformClick();
                    break;

                case Keys.F11:              // 削除
                    // スプレッドが編集途中の場合は強制的に編集を終了(確定)する
                    if (fpSpread_Tel.EditMode == true)
                    {
                        fpSpread_Tel.StopCellEditing();
                        Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                    }
                    if (fpSpread_Furikomi.EditMode == true)
                    {
                        fpSpread_Furikomi.StopCellEditing();
                        Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                    }
                    if (fpSpread_Bunpai.EditMode == true)
                    {
                        fpSpread_Bunpai.StopCellEditing();
                        Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                    }
                    if (fpSpread_TokuisakiTanka.EditMode == true)
                    {
                        fpSpread_TokuisakiTanka.StopCellEditing();
                        Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                    }

                    WriteBinding(this.ActiveControl);   // 入力中の内容を強制的に反映する

                    btnDelete.PerformClick();
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
        /// フォームクロージング
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form700D_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // このフォーム以外の要因で閉じられようとしている場合は閉じる
                if (e.CloseReason != CloseReason.UserClosing)
                {
                    return;
                }

                // 登録or削除済み？
                if (this.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }

                // フォームが閉じられようとするとき、スプレッドが編集途中の場合は強制的に編集を終了(確定)する
                if (fpSpread_Tel.EditMode == true)
                {
                    fpSpread_Tel.StopCellEditing();
                    Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                }
                if (fpSpread_Furikomi.EditMode == true)
                {
                    fpSpread_Furikomi.StopCellEditing();
                    Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                }
                if (fpSpread_Bunpai.EditMode == true)
                {
                    fpSpread_Bunpai.StopCellEditing();
                    Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                }
                if (fpSpread_TokuisakiTanka.EditMode == true)
                {
                    fpSpread_TokuisakiTanka.StopCellEditing();
                    Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                }

                WriteBinding(this.ActiveControl);   // 入力中の内容を強制的に反映する

                // 初期状態と異なる？
                if (HasChangeTokuisaki() || HasChangeTelToCust() || HasChangeTokuisakiFurikomiIraiNin() || HasChangeBunpai() || HasChangeTokuisakiTanka())
                {
                    DialogResult r = MessageBox.Show(this, string.Format("変更されています。{0}しますか？", _IsNewRecord ? "登録" : "更新"), this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    switch (r)
                    {
                        case DialogResult.Yes:
                            // 得意先マスタの入力チェック
                            if (CanSaveTokuisaki() == false)
                            {
                                e.Cancel = true;
                                return;
                            }

                            // 電話テーブルの入力チェック
                            if (CanSaveTelToCust() == false)
                            {
                                e.Cancel = true;
                                return;
                            }

                            // 得意先振込依頼人名テーブルの入力チェック
                            if (CanSaveTokuisakiFurikomiIraiNin() == false)
                            {
                                e.Cancel = true;
                                return;
                            }

                            // 分配先の入力チェック
                            if (CanSaveBunpai() == false)
                            {
                                e.Cancel = true;
                                return;
                            }

                            // 得意先単価マスタの入力チェック
                            if (CanSaveTokuisakiTanka() == false)
                            {
                                e.Cancel = true;
                                return;
                            }

                            if (Save())
                            {
                                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                            }
                            else
                            {
                                e.Cancel = true;
                            }
                            return;

                        case DialogResult.Cancel:
                            e.Cancel = true;
                            return;
                    }
                }
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
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
        /// フォームクローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form700D_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 閉じられようとしている場合は得意先履歴も閉じる
            if (_formRireki != null && _formRireki.IsDisposed == false)
            {
                _formRireki.FormClosed -= FormRireki_FormClosed;    // 念のためイベントの発生を解除
                _formRireki.Close();
            }

            // コース一覧も同様
            if (_formCourse != null && _formCourse.IsDisposed == false)
            {
                _formCourse.FormClosed -= FormCourse_FormClosed;    // 念のためイベントの発生を解除
                _formCourse.Close();
            }
        }

        /// <summary>
        /// 得意先番号 キー押下（F1）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTokuisakiCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnAkiBango.PerformClick();
            }
        }

        /// <summary>
        /// 空き番号ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAkiBango_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 空き番号検索ダイアログを表示
                Form700DN formAkiBangoDlg = new Form700DN();
                SubFormOpenInit(formAkiBangoDlg);
                formAkiBangoDlg.FormClosed += new FormClosedEventHandler(Form700DNTokuisakiCD_FormClosed);
                formAkiBangoDlg.ShowDialog();
                if (formAkiBangoDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.ProcessTabKey(true);                   // 次のコントロールへ
                }
                else
                {
                    // テキストボックスのF1キーにて表示したダイアログが閉じられたときは、フォーカスは元のテキストボックスのままだが
                    // 背景色が消えているので、一旦ボタンにフォーカスを移したのち、元のテキストボックスにフォーカスを戻す
                    ((Button)sender).Select();
                    activeControl.Select();
                }
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
        /// 空き番号検索ダイアログクローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form700DNTokuisakiCD_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                // 親画面を閉じた後に子画面で閉じられた場合、親画面では何もしない
                if (this.IsDisposed)
                {
                    return;
                }

                this.Enabled = true;
                this.Activate();
                Form700DN form = (Form700DN)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtSyoninsyaCD, null);  // エラーを消す

                    int tokuisakiCD = form.空き番号;
                    txtTokuisakiCD.Text = tokuisakiCD.ToString();
                    _tokuisaki.得意先ＣＤ = tokuisakiCD;       // 入力内容を確定させる
                }
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
        /// 得意先名 変換確定（1バイトカタカナ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTokuisakiName_ImeCompositedKata(object sender, Common.Control.TextBoxAF.ImeCompositionEventArgs e)
        {
            try
            {
                // 得意先に入力された読み（カタカナ）を得意先名カナに追加
                _tokuisaki.得意先名カナ += e.InputString;

                // MaxLengthを超えないようにする
                if (_tokuisaki.得意先名カナ.Length > txtTokuisakiKana.MaxLength)
                {
                    _tokuisaki.得意先名カナ = _tokuisaki.得意先名カナ.Substring(0, txtTokuisakiKana.MaxLength);
                }

                // コントロールに反映
                ReadBinding(txtTokuisakiKana);
            }
            catch (Exception)
            {
                // 例外が発生しても無視する
            }
        }

        /// <summary>
        /// 状態コンボ変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboJotai_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                // 状態が承認の場合は承認日と承認者を、
                //       失客の場合は状態理由と失客日をそれぞれ活性にする
                cboJotaiRiyu.Enabled = false;
                dtpSyonin.Enabled = false;
                dtpShitsukyaku.Enabled = false;
                txtSyoninsyaCD.Enabled = false;
                lblSyoninsyaName.Enabled = false;
                btnSyoninsyaCD.Enabled = false;

                switch ((int?)cboJotai.SelectedValue)
                {
                    case (int)Consts.JotaiKubun.Syonin:
                        dtpSyonin.Enabled = true;
                        txtSyoninsyaCD.Enabled = true;
                        lblSyoninsyaName.Enabled = true;
                        btnSyoninsyaCD.Enabled = true;
                        break;

                    case (int)Consts.JotaiKubun.Shikkyaku:
                        cboJotaiRiyu.Enabled = true;
                        dtpShitsukyaku.Enabled = true;
                        break;
                }
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
        /// 承認者CDキー押下（F1）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSyoninsyaCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnSyoninsyaCD.PerformClick();
            }
        }

        /// <summary>
        /// 承認者参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSyoninsyaCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 担当者選択ダイアログを表示
                Form904 formTantosyaDlg = new Form904();
                SubFormOpenInit(formTantosyaDlg);
                formTantosyaDlg.FormClosed += new FormClosedEventHandler(Form904SyoninsyaCD_FormClosed);
                formTantosyaDlg.ShowDialog();
                if (formTantosyaDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.ProcessTabKey(true);                   // 次のコントロールへ
                }
                else
                {
                    // テキストボックスのF1キーにて表示したダイアログが閉じられたときは、フォーカスは元のテキストボックスのままだが
                    // 背景色が消えているので、一旦ボタンにフォーカスを移したのち、元のテキストボックスにフォーカスを戻す
                    ((Button)sender).Select();
                    activeControl.Select();
                }
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
        /// 担当者選択ダイアログクローズ（承認者ＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form904SyoninsyaCD_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                // 親画面を閉じた後に子画面で閉じられた場合、親画面では何もしない
                if (this.IsDisposed)
                {
                    return;
                }

                this.Enabled = true;
                this.Activate();
                Form904 form = (Form904)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtSyoninsyaCD, null);  // エラーを消す

                    Tantousya tantousya = form.SelectedTantousya;
                    txtSyoninsyaCD.Text = tantousya.担当者ＣＤ.ToString();
                    _tokuisaki.承認者ＣＤ = tantousya.担当者ＣＤ;       // 入力内容を確定させる
                    lblSyoninsyaName.Text = tantousya.担当者名;
                }
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
        /// 部署コンボ変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboBusyo_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                // 振替用EB口座の選択肢を、部署にぶら下がるものに再設定する
                SetBusyoEB(cboEBKoza, (int)cboBusyo.SelectedValue);
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
        /// CTI連携・電話番号登録Spread KeyDownイベント前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Tel_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                // 通常モード（＝セル編集ができる）SpreadはEscapeキーのKeyDownイベントが発生しないのでこのイベントで処理する
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
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
        /// CTI連携・電話番号登録Spreadキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Tel_KeyDown(object sender, KeyEventArgs e)
        {
            // F2？
            if (e.KeyCode == Keys.F2)
            {
                _isF2forSpreadEdit = true;      // 編集モードに移行するF2キーが押下された

                // ↓キー押下されたことにするが、あまり意味が無い
                //e.SuppressKeyPress = true;
                //e.Handled = true;

                // 非編集モードから編集モードへの移行なので、常にelseが成立
                if (fpSpread_Tel.EditMode)
                {
                    fpSpread_Tel.CancelCellEditing();
                }
                else
                {
                    fpSpread_Tel.StartCellEditing(e, false);   // セル編集を開始

                    // ↓ここでやっても意味が無い
                    //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_Tel.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread_Tel_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread_Tel_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列を超える場合は次の行へ移動する
                    if (newColumnIndex >= TEL_DELETE_COLUMN - 1)
                    {
                        newColumnIndex = 0;
                        newRowIndex++;
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread_Tel_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread_Tel_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread_Tel_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread_Tel.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }

            // Deleteキー
            if (e.KeyCode == Keys.Delete)
            {
                int colIndex = -1;
                switch (fpSpread_Tel_Sheet1.ActiveColumnIndex)
                {
                    case TEL_NUMBER_COLUMN:      // 電話番号
                    case TEL_REMARK_COLUMN:      // 備考
                        colIndex = fpSpread_Tel_Sheet1.ActiveColumnIndex;
                        break;

                    default:
                        colIndex = -1;
                        break;
                }

                if (colIndex >= 0)
                {
                    fpSpread_Tel_Sheet1.Cells[fpSpread_Tel_Sheet1.ActiveRowIndex, colIndex].Value = null;     // dt_TelToCust_ColumnChangedが発生
                }
            }
        }

        /// <summary>
        /// CTI連携・電話番号登録Spread編集モードON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Tel_EditModeOn(object sender, EventArgs e)
        {
            if (fpSpread_Tel.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                // 編集モードに移行するF2キーが押下されたことによる編集モード開始？
                if (_isF2forSpreadEdit)
                {
                    // ☆で編集開始時に入力済みデータを選択状態にしているので、F2キーでのセル編集開始時は選択状態では無く末尾にカーソルを表示する
                    ((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_Tel.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }

                //セルの編集開始のタイミングで各Keyイベントをハンドルします
                fpSpread_Tel.EditingControl.KeyDown += new KeyEventHandler(this.fpSpread_TelEdit_KeyDown);
            }

            if (fpSpread_Tel.EditingControl is FarPoint.Win.FpCombo)
            {
                // コンボボックスの場合は、Enterキーでのドロップダウンを制御する
                _fpCombo = (FarPoint.Win.FpCombo)fpSpread_Tel.EditingControl;
                _fpCombo.KeyDown += new KeyEventHandler(FpCombo_Tel_KeyDown);
                _fpCombo.DropDown += new FarPoint.Win.DropDownEventHandler(FpCombo_Tel_DropDown);
            }
        }

        /// <summary>
        /// CTI連携・電話番号登録Spread編集モードOFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Tel_EditModeOff(object sender, EventArgs e)
        {
            if (fpSpread_Tel.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                // 省略された日付文字列を補正する
                ParseDateText(fpSpread_Tel, fpSpread_Tel_Sheet1);

                //セルの編集終了のタイミングで各Keyイベントのハンドルを解除します
                fpSpread_Tel.EditingControl.KeyDown -= new KeyEventHandler(this.fpSpread_TelEdit_KeyDown);
            }

            if (fpSpread_Tel.EditingControl is FarPoint.Win.FpCombo)
            {
                // ドロップダウンについても各イベントのハンドルを解除します
                _fpCombo.KeyDown -= new KeyEventHandler(FpCombo_Tel_KeyDown);
                _fpCombo.DropDown -= new FarPoint.Win.DropDownEventHandler(FpCombo_Tel_DropDown);
            }
        }

        /// <summary>
        /// CTI連携・電話番号登録Spread編集モードでのキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_TelEdit_KeyDown(object sender, KeyEventArgs e)
        {
            // F2？
            if (e.KeyCode == Keys.F2)
            {
                // 編集モードに移行するF2キーが押下された場合も呼ばれるので、その場合は何もしない
                if (_isF2forSpreadEdit)
                {
                    _isF2forSpreadEdit = false;
                }
                else
                {
                    // ↓キー押下されたことにするが、あまり意味が無い
                    //e.SuppressKeyPress = true;
                    //e.Handled = true;

                    // 編集モードから非編集モードへの移行なので、常にifが成立
                    if (fpSpread_Tel.EditMode)
                    {
                        fpSpread_Tel.CancelCellEditing();
                    }
                    else
                    {
                        fpSpread_Tel.StartCellEditing(e, false);
                        //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_Tel.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                    }
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread_Tel_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread_Tel_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列を超える場合は次の行へ移動する
                    if (newColumnIndex >= TEL_DELETE_COLUMN - 1)
                    {
                        newColumnIndex = 0;
                        newRowIndex++;
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread_Tel_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread_Tel_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread_Tel_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread_Tel.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }
        }

        /// <summary>
        /// CTI連携・電話番号登録Spreadコンボボックスキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FpCombo_Tel_KeyDown(object sender, KeyEventArgs e)
        {
            // F2？
            if (e.KeyCode == Keys.F2)
            {
                // 編集モードに移行するF2キーが押下された場合に呼ばれた場合は、リストを表示する
                if (_isF2forSpreadEdit)
                {
                    _isF2forSpreadEdit = false;
                    _fpCombo.DroppedDown = true;
                }
                else
                {
                    // ↓キー押下されたことにするが、あまり意味が無い
                    //e.SuppressKeyPress = true;
                    //e.Handled = true;

                    // 編集モードから非編集モードへの移行なので、常にifが成立
                    if (fpSpread_Tel.EditMode)
                    {
                        fpSpread_Tel.CancelCellEditing();
                    }
                    else
                    {
                        fpSpread_Tel.StartCellEditing(e, false);
                        //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_Tel.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                    }
                }
            }

            //Enterキーが押された場合にフラグを立てる
            _isEnterforFpCombo = (e.KeyCode == Keys.Enter);
        }

        /// <summary>
        /// CTI連携・電話番号登録Spreadコンボボックスドロップダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FpCombo_Tel_DropDown(object sender, FarPoint.Win.DropDownEventArgs e)
        {
            //Enterキーを契機とする場合は、DropDown処理をキャンセルする
            if (_isEnterforFpCombo)
            {
                _isEnterforFpCombo = false;
                e.Cancel = true;
                // 次のセルへ移動
                SendKeys.Send("{Tab}");
            }
        }

        /// <summary>
        /// CTI連携・電話番号登録Spreadボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Tel_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                // 削除ボタン？
                if (e.Column == TEL_DELETE_COLUMN)
                {
                    // 最終行（=新規行）の場合は何もしない
                    if (e.Row == fpSpread_Tel_Sheet1.Rows.Count - 1)
                    {
                        return;
                    }
                    DataRow dr = ((DataRowView)telBindingSource.Current).Row;
                    dr.Delete();
                }
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
        /// CTI連携・電話番号登録Spread編集完了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Tel_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            // 既存の行のRowStateが、アクティブセルが他の行に移らないと変わらないのを、強制的に変更する
            DataRow dr = ((DataRowView)telBindingSource.Current).Row;
            dr.EndEdit();
        }

        /// <summary>
        /// 請求先CDキー押下（F1）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSeikyuCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnSeikyuCD.PerformClick();
            }
        }

        /// <summary>
        /// 請求先CD参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSeikyuCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 得意先選択ダイアログを表示
                Form900 formTokuisakiDlg = new Form900();
                SubFormOpenInit(formTokuisakiDlg);
                formTokuisakiDlg.FormClosed += new FormClosedEventHandler(Form900SeikyusakiCD_FormClosed);
                formTokuisakiDlg.ShowDialog();
                if (formTokuisakiDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.ProcessTabKey(true);                   // 次のコントロールへ
                }
                else
                {
                    // テキストボックスのF1キーにて表示したダイアログが閉じられたときは、フォーカスは元のテキストボックスのままだが
                    // 背景色が消えているので、一旦ボタンにフォーカスを移したのち、元のテキストボックスにフォーカスを戻す
                    ((Button)sender).Select();
                    activeControl.Select();
                }
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
        /// 得意先選択ダイアログクローズ（請求先ＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form900SeikyusakiCD_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                // 親画面を閉じた後に子画面で閉じられた場合、親画面では何もしない
                if (this.IsDisposed)
                {
                    return;
                }

                this.Enabled = true;
                this.Activate();
                Form900 form = (Form900)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtSeikyuCD, null);  // エラーを消す

                    Tokuisaki tokuisaki = form.SelectedTokuisaki;
                    txtSeikyuCD.Text = tokuisaki.得意先ＣＤ.ToString();
                    _tokuisaki.請求先ＣＤ = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    lblSeikyuName.Text = tokuisaki.得意先名;
                }
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
        /// 丸め区分 検証後
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboMarume_Validated(object sender, EventArgs e)
        {
            try
            {
                // 得意先単価の税抜き単価を算出し直す
                DataSet ds = (DataSet)tankaBindingSource.DataSource;
                if (ds == null)
                {
                    return;
                }

                DataTable dt = ds.Tables["得意先単価マスタ"];
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        CalcTokuisakiTankaZeiNuki(dr, _tokuisaki.丸め処理区分);
                    }
                }
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
        /// 締め区分コンボ変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboShime_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                switch ((int?)cboShime.SelectedValue)
                {
                    case (int)Consts.ShimeKubun.Weekly:     // 週締め
                        // 週締め曜日のみ入力可能
                        txtTsukishimebi.Enabled = false;
                        cboSyushimeYobi.Enabled = true;
                        break;

                    case (int)Consts.ShimeKubun.Monthly:    // 月締め
                        // 月締め日のみ入力可能
                        txtTsukishimebi.Enabled = true;
                        cboSyushimeYobi.Enabled = false;
                        break;

                    default:                                // 上記以外（日締め・未選択）
                        // どちらも入力不可
                        txtTsukishimebi.Enabled = false;
                        cboSyushimeYobi.Enabled = false;
                        break;
                }
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
        /// 支払方法コンボ変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboShiharaiHoho_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                // 支払方法が振替の場合は、振替用EB口座を入力可能
                cboEBKoza.Enabled = ((int?)cboShiharaiHoho.SelectedValue == (int)Consts.ShiharaiHohoKubun.Furikae);
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
        /// 金融機関ＣＤキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBankCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnBankCD.PerformClick();
            }
        }

        /// <summary>
        /// 金融機関ＣＤ参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBankCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 金融機関選択ダイアログを表示
                Form905 formBankDlg = new Form905();
                SubFormOpenInit(formBankDlg);
                formBankDlg.FormClosed += new FormClosedEventHandler(Form905BankCD_FormClosed);
                formBankDlg.ShowDialog();
                if (formBankDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.ProcessTabKey(true);                   // 次のコントロールへ
                }
                else
                {
                    // テキストボックスのF1キーにて表示したダイアログが閉じられたときは、フォーカスは元のテキストボックスのままだが
                    // 背景色が消えているので、一旦ボタンにフォーカスを移したのち、元のテキストボックスにフォーカスを戻す
                    ((Button)sender).Select();
                    activeControl.Select();
                }
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
        /// 金融機関選択ダイアログクローズ（金融機関ＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form905BankCD_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                // 親画面を閉じた後に子画面で閉じられた場合、親画面では何もしない
                if (this.IsDisposed)
                {
                    return;
                }

                this.Enabled = true;
                this.Activate();
                Form905 form = (Form905)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtBankCD, null);  // エラーを消す

                    // 金融機関支店CDも消す
                    errorProvider1.SetError(txtBranchCD, null);
                    txtBranchCD.Text = "";
                    lblBranchName.Text = "";
                    _tokuisaki.金融機関支店ＣＤ = null;

                    Bank bank = form.SelectedBank;
                    txtBankCD.Text = bank.銀行ＣＤ;

                    txtKigoBango.Enabled = (txtBankCD.Text == Consts.YUCHO);    // // ゆうちょ以外の場合は記号番号を非活性にする

                    _tokuisaki.金融機関ＣＤ = bank.銀行ＣＤ;       // 入力内容を確定させる
                    lblBankName.Text = bank.銀行名;
                }
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
        /// 金融機関CD検証後イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBankCD_Validated(object sender, EventArgs e)
        {
            try
            {
                bool enabled = true;
                // ゆうちょ以外の場合は記号番号を非活性にする
                // ※Tokuisakiクラスの中でクリアする
                if (txtBankCD.Text != Consts.YUCHO)
                {
                    enabled = false;
                }
                txtKigoBango.Enabled = enabled;
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
        /// 金融機関支店CDキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBranchCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnBranchCD.PerformClick();
            }
        }

        /// <summary>
        /// 金融機関支店ＣＤ参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBranchCD_Click(object sender, EventArgs e)
        {
            try
            {
                // 金融機関CD1が未入力の場合は何もしない
                if (string.IsNullOrEmpty(txtBankCD.Text) == false && string.IsNullOrEmpty(lblBankName.Text) == false)
                {
                    Control activeControl = this.ActiveControl;
                    // 金融機関支店選択ダイアログを表示
                    Form906 formBranchDlg = new Form906(txtBankCD.Text, lblBankName.Text);
                    SubFormOpenInit(formBranchDlg);
                    formBranchDlg.FormClosed += new FormClosedEventHandler(Form906BranchCD_FormClosed);
                    formBranchDlg.ShowDialog();
                    if (formBranchDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
                    {
                        this.ProcessTabKey(true);                   // 次のコントロールへ
                    }
                    else
                    {
                        // テキストボックスのF1キーにて表示したダイアログが閉じられたときは、フォーカスは元のテキストボックスのままだが
                        // 背景色が消えているので、一旦ボタンにフォーカスを移したのち、元のテキストボックスにフォーカスを戻す
                        ((Button)sender).Select();
                        activeControl.Select();
                    }
                }
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
        /// 金融機関支店選択ダイアログクローズ（金融機関支店ＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form906BranchCD_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                // 親画面を閉じた後に子画面で閉じられた場合、親画面では何もしない
                if (this.IsDisposed)
                {
                    return;
                }

                this.Enabled = true;
                this.Activate();
                Form906 form = (Form906)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtBranchCD, null);   // エラーを消す
                    BankBranch bankBranch = form.SelectedBankBranch;
                    txtBranchCD.Text = bankBranch.支店ＣＤ;
                    _tokuisaki.金融機関支店ＣＤ = bankBranch.支店ＣＤ;         // 入力内容を確定させる
                    lblBranchName.Text = bankBranch.支店名; ;
                }
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
        /// 振込依頼人名Spread KeyDownイベント前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Furikomi_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                // 通常モード（＝セル編集ができる）SpreadはEscapeキーのKeyDownイベントが発生しないのでこのイベントで処理する
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
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
        /// 振込依頼人名Spreadキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Furikomi_KeyDown(object sender, KeyEventArgs e)
        {
            // F2？
            if (e.KeyCode == Keys.F2)
            {
                _isF2forSpreadEdit = true;      // 編集モードに移行するF2キーが押下された

                // ↓キー押下されたことにするが、あまり意味が無い
                //e.SuppressKeyPress = true;
                //e.Handled = true;

                // 非編集モードから編集モードへの移行なので、常にelseが成立
                if (fpSpread_Furikomi.EditMode)
                {
                    fpSpread_Furikomi.CancelCellEditing();
                }
                else
                {
                    fpSpread_Furikomi.StartCellEditing(e, false);   // セル編集を開始

                    // ↓ここでやっても意味が無い
                    //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_Furikomi.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread_Furikomi_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread_Furikomi_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列を超える場合は次の行へ移動する
                    if (newColumnIndex >= FURI_DELETE_COLUMN - 1)
                    {
                        newColumnIndex = 0;
                        newRowIndex++;
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread_Furikomi_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread_Furikomi_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread_Furikomi_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread_Furikomi.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }

            // Deleteキー
            if (e.KeyCode == Keys.Delete)
            {
                int colIndex = -1;
                switch (fpSpread_Furikomi_Sheet1.ActiveColumnIndex)
                {
                    case FURI_NAME_COLUMN:      // 振込依頼人名
                        colIndex = fpSpread_Furikomi_Sheet1.ActiveColumnIndex;
                        break;

                    default:
                        colIndex = -1;
                        break;
                }

                if (colIndex >= 0)
                {
                    fpSpread_Furikomi_Sheet1.Cells[fpSpread_Furikomi_Sheet1.ActiveRowIndex, colIndex].Value = null;     // dt_TokuisakiFurikomiIraiNin_ColumnChangedが発生
                }
            }
        }

        /// <summary>
        /// 振込依頼人名Spread編集モードON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Furikomi_EditModeOn(object sender, EventArgs e)
        {
            if (fpSpread_Furikomi.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                // 編集モードに移行するF2キーが押下されたことによる編集モード開始？
                if (_isF2forSpreadEdit)
                {
                    // ☆で編集開始時に入力済みデータを選択状態にしているので、F2キーでのセル編集開始時は選択状態では無く末尾にカーソルを表示する
                    ((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_Furikomi.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }

                //セルの編集開始のタイミングで各Keyイベントをハンドルします
                fpSpread_Furikomi.EditingControl.KeyDown += new KeyEventHandler(this.fpSpread_FurikomiEdit_KeyDown);
            }
        }

        /// <summary>
        /// 振込依頼人名Spread編集モードOFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Furikomi_EditModeOff(object sender, EventArgs e)
        {
            if (fpSpread_Furikomi.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                // 省略された日付文字列を補正する
                ParseDateText(fpSpread_Furikomi, fpSpread_Furikomi_Sheet1);

                //セルの編集終了のタイミングで各Keyイベントのハンドルを解除します
                fpSpread_Furikomi.EditingControl.KeyDown -= new KeyEventHandler(this.fpSpread_FurikomiEdit_KeyDown);
            }
        }

        /// <summary>
        /// 振込依頼人名Spread編集モードでのキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_FurikomiEdit_KeyDown(object sender, KeyEventArgs e)
        {
            // F2？
            if (e.KeyCode == Keys.F2)
            {
                // 編集モードに移行するF2キーが押下された場合も呼ばれるので、その場合は何もしない
                if (_isF2forSpreadEdit)
                {
                    _isF2forSpreadEdit = false;
                }
                else
                {
                    // ↓キー押下されたことにするが、あまり意味が無い
                    //e.SuppressKeyPress = true;
                    //e.Handled = true;

                    // 編集モードから非編集モードへの移行なので、常にifが成立
                    if (fpSpread_Furikomi.EditMode)
                    {
                        fpSpread_Furikomi.CancelCellEditing();
                    }
                    else
                    {
                        fpSpread_Furikomi.StartCellEditing(e, false);
                        //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_Furikomi.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                    }
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread_Furikomi_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread_Furikomi_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列を超える場合は次の行へ移動する
                    if (newColumnIndex >= FURI_DELETE_COLUMN - 1)
                    {
                        newColumnIndex = 0;
                        newRowIndex++;
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread_Furikomi_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread_Furikomi_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread_Furikomi_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread_Furikomi.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }
        }

        /// <summary>
        /// 振込依頼人名Spreadボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Furikomi_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                // 削除ボタン？
                if (e.Column == FURI_DELETE_COLUMN)
                {
                    // 最終行（=新規行）の場合は何もしない
                    if (e.Row == fpSpread_Furikomi_Sheet1.Rows.Count - 1)
                    {
                        return;
                    }
                    DataRow dr = ((DataRowView)furikomiBindingSource.Current).Row;
                    dr.Delete();
                }
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
        /// 振込依頼人名Spread編集完了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Furikomi_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            // 既存の行のRowStateが、アクティブセルが他の行に移らないと変わらないのを、強制的に変更する
            DataRow dr = ((DataRowView)furikomiBindingSource.Current).Row;
            dr.EndEdit();
        }

        /// <summary>
        /// 分配先Spread KeyDownイベント前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Bunpai_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                // 通常モード（＝セル編集ができる）SpreadはEscapeキーのKeyDownイベントが発生しないのでこのイベントで処理する
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
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
        /// 分配先Spreadキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Bunpai_KeyDown(object sender, KeyEventArgs e)
        {
            // F1？
            if (e.KeyCode == Keys.F1)
            {
                OpenForm900BySpread();          // 得意先選択ダイアログを表示
            }

            // F2？
            if (e.KeyCode == Keys.F2)
            {
                _isF2forSpreadEdit = true;      // 編集モードに移行するF2キーが押下された

                // ↓キー押下されたことにするが、あまり意味が無い
                //e.SuppressKeyPress = true;
                //e.Handled = true;

                // 非編集モードから編集モードへの移行なので、常にelseが成立
                if (fpSpread_Bunpai.EditMode)
                {
                    fpSpread_Bunpai.CancelCellEditing();
                }
                else
                {
                    fpSpread_Bunpai.StartCellEditing(e, false);   // セル編集を開始

                    // ↓ここでやっても意味が無い
                    //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_Bunpai.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread_Bunpai_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread_Bunpai_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列を超える場合は次の行へ移動する
                    if (newColumnIndex >= BUNPAI_DELETE_COLUMN - 1)
                    {
                        newColumnIndex = 0;
                        newRowIndex++;
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread_Bunpai_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread_Bunpai_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread_Bunpai_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread_Bunpai.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }

            // Deleteキー
            if (e.KeyCode == Keys.Delete)
            {
                int colIndex = -1;
                switch (fpSpread_Bunpai_Sheet1.ActiveColumnIndex)
                {
                    case BUNPAI_TOKUISAKI_CD_COLUMN:      // 得意先ＣＤ
                        colIndex = fpSpread_Bunpai_Sheet1.ActiveColumnIndex;
                        break;

                    default:
                        colIndex = -1;
                        break;
                }

                if (colIndex >= 0)
                {
                    fpSpread_Bunpai_Sheet1.Cells[fpSpread_Bunpai_Sheet1.ActiveRowIndex, colIndex].Value = null;     // dt_Bunpai_ColumnChangedが発生
                }
            }
        }

        /// <summary>
        /// 分配先Spread編集モードON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Bunpai_EditModeOn(object sender, EventArgs e)
        {
            if (fpSpread_Bunpai.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                // 編集モードに移行するF2キーが押下されたことによる編集モード開始？
                if (_isF2forSpreadEdit)
                {
                    // ☆で編集開始時に入力済みデータを選択状態にしているので、F2キーでのセル編集開始時は選択状態では無く末尾にカーソルを表示する
                    ((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_Bunpai.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }

                //セルの編集開始のタイミングで各Keyイベントをハンドルします
                fpSpread_Bunpai.EditingControl.KeyDown += new KeyEventHandler(this.fpSpread_BunpaiEdit_KeyDown);
            }
        }

        /// <summary>
        /// 分配先Spread編集モードOFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Bunpai_EditModeOff(object sender, EventArgs e)
        {
            if (fpSpread_Bunpai.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                // 省略された日付文字列を補正する
                ParseDateText(fpSpread_Bunpai, fpSpread_Bunpai_Sheet1);

                //セルの編集終了のタイミングで各Keyイベントのハンドルを解除します
                fpSpread_Bunpai.EditingControl.KeyDown -= new KeyEventHandler(this.fpSpread_BunpaiEdit_KeyDown);
            }
        }

        /// <summary>
        /// 分配先Spread編集モードでのキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_BunpaiEdit_KeyDown(object sender, KeyEventArgs e)
        {
            // F2？
            if (e.KeyCode == Keys.F2)
            {
                // 編集モードに移行するF2キーが押下された場合も呼ばれるので、その場合は何もしない
                if (_isF2forSpreadEdit)
                {
                    _isF2forSpreadEdit = false;
                }
                else
                {
                    // ↓キー押下されたことにするが、あまり意味が無い
                    //e.SuppressKeyPress = true;
                    //e.Handled = true;

                    // 編集モードから非編集モードへの移行なので、常にifが成立
                    if (fpSpread_Bunpai.EditMode)
                    {
                        fpSpread_Bunpai.CancelCellEditing();
                    }
                    else
                    {
                        fpSpread_Bunpai.StartCellEditing(e, false);
                        //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_Bunpai.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                    }
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread_Bunpai_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread_Bunpai_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列を超える場合は次の行へ移動する
                    if (newColumnIndex >= BUNPAI_DELETE_COLUMN - 1)
                    {
                        newColumnIndex = 0;
                        newRowIndex++;
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread_Bunpai_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread_Bunpai_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread_Bunpai_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread_Bunpai.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }
        }

        /// <summary>
        /// 分配先Spreadボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Bunpai_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                // 参照ボタン？
                if (e.Column == BUNPAI_REFERENCE_COLUMN)
                {
                    OpenForm900BySpread();
                }

                // 削除ボタン？
                if (e.Column == BUNPAI_DELETE_COLUMN)
                {
                    // 最終行（=新規行）の場合は何もしない
                    if (e.Row == fpSpread_Bunpai_Sheet1.Rows.Count - 1)
                    {
                        return;
                    }
                    DataRow dr = ((DataRowView)bunpaiBindingSource.Current).Row;
                    dr.Delete();
                }
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
        /// 分配先Spreadから得意先選択ダイアログを表示する
        /// </summary>
        private void OpenForm900BySpread()
        {
            try
            {
                // 得意先選択ダイアログを表示
                Form900 formTokuisakiDlg = new Form900();
                SubFormOpenInit(formTokuisakiDlg);
                formTokuisakiDlg.FormClosed += new FormClosedEventHandler(Form900TokuisakiCD_FormClosed);
                formTokuisakiDlg.ShowDialog();
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
        /// 得意先選択ダイアログクローズ（得意先ＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form900TokuisakiCD_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                // 親画面を閉じた後に子画面で閉じられた場合、親画面では何もしない
                if (this.IsDisposed)
                {
                    return;
                }

                this.Enabled = true;
                this.Activate();
                Form900 form = (Form900)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                    int newColumnIndex = fpSpread_Bunpai_Sheet1.ActiveColumnIndex;
                    do
                    {
                        newColumnIndex++;
                    } while (fpSpread_Bunpai_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                             fpSpread_Bunpai_Sheet1.Columns[newColumnIndex].CanFocus == false);
                    fpSpread_Bunpai_Sheet1.SetActiveCell(fpSpread_Bunpai_Sheet1.ActiveRowIndex, newColumnIndex);

                    Tokuisaki tokuisaki = form.SelectedTokuisaki;
                    fpSpread_Bunpai_Sheet1.Cells[fpSpread_Bunpai_Sheet1.ActiveRowIndex, BUNPAI_TOKUISAKI_CD_COLUMN].Value = tokuisaki.得意先ＣＤ;   // 得意先CDセルに値を設定→dt_Bunpai_ColumnChanged()が呼ばれる
                }
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
        /// 分配先Spread編集完了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Bunpai_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            // 既存の行のRowStateが、アクティブセルが他の行に移らないと変わらないのを、強制的に変更する
            DataRow dr = ((DataRowView)bunpaiBindingSource.Current).Row;
            dr.EndEdit();
        }

        /// <summary>
        /// 営業担当者CDキー押下（F1）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtEigyoCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnEigyoCD.PerformClick();
            }
        }

        /// <summary>
        /// 営業担当者参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEigyoCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 担当者選択ダイアログを表示
                Form904 formTantosyaDlg = new Form904();
                SubFormOpenInit(formTantosyaDlg);
                formTantosyaDlg.FormClosed += new FormClosedEventHandler(Form904EigyoCD_FormClosed);
                formTantosyaDlg.ShowDialog();
                if (formTantosyaDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.ProcessTabKey(true);                   // 次のコントロールへ
                }
                else
                {
                    // テキストボックスのF1キーにて表示したダイアログが閉じられたときは、フォーカスは元のテキストボックスのままだが
                    // 背景色が消えているので、一旦ボタンにフォーカスを移したのち、元のテキストボックスにフォーカスを戻す
                    ((Button)sender).Select();
                    activeControl.Select();
                }
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
        /// 担当者選択ダイアログクローズ（営業担当者CD）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form904EigyoCD_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                // 親画面を閉じた後に子画面で閉じられた場合、親画面では何もしない
                if (this.IsDisposed)
                {
                    return;
                }

                this.Enabled = true;
                this.Activate();
                Form904 form = (Form904)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtEigyoCD, null);  // エラーを消す

                    Tantousya tantousya = form.SelectedTantousya;
                    txtEigyoCD.Text = tantousya.担当者ＣＤ.ToString();
                    _tokuisaki.営業担当者ＣＤ = tantousya.担当者ＣＤ;       // 入力内容を確定させる
                    lblEigyoName.Text = tantousya.担当者名;
                }
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
        /// 獲得営業者CDキー押下（F1）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtKakutokuCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnKakutokuCD.PerformClick();
            }
        }

        /// <summary>
        /// 獲得営業者参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnKakutokuCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 担当者選択ダイアログを表示
                Form904 formTantosyaDlg = new Form904();
                SubFormOpenInit(formTantosyaDlg);
                formTantosyaDlg.FormClosed += new FormClosedEventHandler(Form904KakutokuCD_FormClosed);
                formTantosyaDlg.ShowDialog();
                if (formTantosyaDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.ProcessTabKey(true);                   // 次のコントロールへ
                }
                else
                {
                    // テキストボックスのF1キーにて表示したダイアログが閉じられたときは、フォーカスは元のテキストボックスのままだが
                    // 背景色が消えているので、一旦ボタンにフォーカスを移したのち、元のテキストボックスにフォーカスを戻す
                    ((Button)sender).Select();
                    activeControl.Select();
                }
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
        /// 担当者選択ダイアログクローズ（獲得営業者ＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form904KakutokuCD_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                // 親画面を閉じた後に子画面で閉じられた場合、親画面では何もしない
                if (this.IsDisposed)
                {
                    return;
                }

                this.Enabled = true;
                this.Activate();
                Form904 form = (Form904)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtKakutokuCD, null);  // エラーを消す

                    Tantousya tantousya = form.SelectedTantousya;
                    txtKakutokuCD.Text = tantousya.担当者ＣＤ.ToString();
                    _tokuisaki.獲得営業者ＣＤ = tantousya.担当者ＣＤ;       // 入力内容を確定させる
                    lblKakutokuName.Text = tantousya.担当者名;
                }
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
        /// 登録担当者CDキー押下（F1）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTorokuCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnTorokuCD.PerformClick();
            }
        }

        /// <summary>
        /// 登録担当者参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTorokuCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 担当者選択ダイアログを表示
                Form904 formTantosyaDlg = new Form904();
                SubFormOpenInit(formTantosyaDlg);
                formTantosyaDlg.FormClosed += new FormClosedEventHandler(Form904TorokuCD_FormClosed);
                formTantosyaDlg.ShowDialog();
                if (formTantosyaDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.ProcessTabKey(true);                   // 次のコントロールへ
                }
                else
                {
                    // テキストボックスのF1キーにて表示したダイアログが閉じられたときは、フォーカスは元のテキストボックスのままだが
                    // 背景色が消えているので、一旦ボタンにフォーカスを移したのち、元のテキストボックスにフォーカスを戻す
                    ((Button)sender).Select();
                    activeControl.Select();
                }
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
        /// 担当者選択ダイアログクローズ（登録担当者ＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form904TorokuCD_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                // 親画面を閉じた後に子画面で閉じられた場合、親画面では何もしない
                if (this.IsDisposed)
                {
                    return;
                }

                this.Enabled = true;
                this.Activate();
                Form904 form = (Form904)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtSyoninsyaCD, null);  // エラーを消す

                    Tantousya tantousya = form.SelectedTantousya;
                    txtTorokuCD.Text = tantousya.担当者ＣＤ.ToString();
                    _tokuisaki.登録担当者ＣＤ = tantousya.担当者ＣＤ;       // 入力内容を確定させる
                    lblTorokuName.Text = tantousya.担当者名;
                }
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
        /// 配送先CDキー押下（F1）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtHaisoCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnHaisoCD.PerformClick();
            }
        }

        /// <summary>
        /// 配送先CD参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHaisoCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 得意先選択ダイアログを表示
                Form900 formTokuisakiDlg = new Form900();
                SubFormOpenInit(formTokuisakiDlg);
                formTokuisakiDlg.FormClosed += new FormClosedEventHandler(Form900HaisosakiCD_FormClosed);
                formTokuisakiDlg.ShowDialog();
                if (formTokuisakiDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.ProcessTabKey(true);                   // 次のコントロールへ
                }
                else
                {
                    // テキストボックスのF1キーにて表示したダイアログが閉じられたときは、フォーカスは元のテキストボックスのままだが
                    // 背景色が消えているので、一旦ボタンにフォーカスを移したのち、元のテキストボックスにフォーカスを戻す
                    ((Button)sender).Select();
                    activeControl.Select();
                }
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
        /// 得意先選択ダイアログクローズ（配送得意先ＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form900HaisosakiCD_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                // 親画面を閉じた後に子画面で閉じられた場合、親画面では何もしない
                if (this.IsDisposed)
                {
                    return;
                }

                this.Enabled = true;
                this.Activate();
                Form900 form = (Form900)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtSeikyuCD, null);  // エラーを消す

                    Tokuisaki tokuisaki = form.SelectedTokuisaki;
                    txtHaisoCD.Text = tokuisaki.得意先ＣＤ.ToString();
                    _tokuisaki.配送得意先ＣＤ = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    lblHaisoName.Text = tokuisaki.得意先名;
                }
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
        /// 得意先単価Spread KeyDownイベント前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_TokuisakiTanka_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                // 通常モード（＝セル編集ができる）SpreadはEscapeキーのKeyDownイベントが発生しないのでこのイベントで処理する
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
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
        /// 得意先単価Spreadキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_TokuisakiTanka_KeyDown(object sender, KeyEventArgs e)
        {
            // F1？
            if (e.KeyCode == Keys.F1)
            {
                OpenForm901BySpread();          // 商品選択ダイアログを表示
            }

            // F2？
            if (e.KeyCode == Keys.F2)
            {
                _isF2forSpreadEdit = true;      // 編集モードに移行するF2キーが押下された

                // ↓キー押下されたことにするが、あまり意味が無い
                //e.SuppressKeyPress = true;
                //e.Handled = true;

                // 非編集モードから編集モードへの移行なので、常にelseが成立
                if (fpSpread_TokuisakiTanka.EditMode)
                {
                    fpSpread_TokuisakiTanka.CancelCellEditing();
                }
                else
                {
                    fpSpread_TokuisakiTanka.StartCellEditing(e, false);   // セル編集を開始

                    // ↓ここでやっても意味が無い
                    //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_TokuisakiTanka.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread_TokuisakiTanka_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread_TokuisakiTanka_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列を超える場合は次の行へ移動する
                    if (newColumnIndex >= TANKA_DELETE_COLUMN - 1)
                    {
                        newColumnIndex = 0;
                        newRowIndex++;
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread_TokuisakiTanka_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread_TokuisakiTanka_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread_TokuisakiTanka_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread_TokuisakiTanka.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }

            // Deleteキー
            if (e.KeyCode == Keys.Delete)
            {
                int colIndex = -1;
                switch (fpSpread_TokuisakiTanka_Sheet1.ActiveColumnIndex)
                {
                    case TANKA_SYOHIN_CD_COLUMN:        // 商品ＣＤ
                    case TANKA_TOKUISAKI_KOMI_COLUMN:   // 得意先単価税込
                    case TANKA_TOKUISAKI_NUKI_COLUMN:   // 得意先単価税抜
                        colIndex = fpSpread_TokuisakiTanka_Sheet1.ActiveColumnIndex;
                        break;

                    default:
                        colIndex = -1;
                        break;
                }

                if (colIndex >= 0)
                {
                    fpSpread_TokuisakiTanka_Sheet1.Cells[fpSpread_TokuisakiTanka_Sheet1.ActiveRowIndex, colIndex].Value = null;     // dt_TokuisakiTanka_ColumnChangedが発生
                }
            }
        }

        /// <summary>
        /// 得意先単価Spread編集モードON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_TokuisakiTanka_EditModeOn(object sender, EventArgs e)
        {
            if (fpSpread_TokuisakiTanka.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                // 編集モードに移行するF2キーが押下されたことによる編集モード開始？
                if (_isF2forSpreadEdit)
                {
                    // ☆で編集開始時に入力済みデータを選択状態にしているので、F2キーでのセル編集開始時は選択状態では無く末尾にカーソルを表示する
                    ((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_TokuisakiTanka.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }

                //セルの編集開始のタイミングで各Keyイベントをハンドルします
                fpSpread_TokuisakiTanka.EditingControl.KeyDown += new KeyEventHandler(this.fpSpread_TokuisakiTankaEdit_KeyDown);
            }
        }

        /// <summary>
        /// 得意先単価Spread編集モードOFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_TokuisakiTanka_EditModeOff(object sender, EventArgs e)
        {
            if (fpSpread_TokuisakiTanka.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                // 省略された日付文字列を補正する
                ParseDateText(fpSpread_TokuisakiTanka, fpSpread_TokuisakiTanka_Sheet1);

                //セルの編集終了のタイミングで各Keyイベントのハンドルを解除します
                fpSpread_TokuisakiTanka.EditingControl.KeyDown -= new KeyEventHandler(this.fpSpread_TokuisakiTankaEdit_KeyDown);
            }
        }

        /// <summary>
        /// 得意先単価Spread編集モードでのキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_TokuisakiTankaEdit_KeyDown(object sender, KeyEventArgs e)
        {
            // F2？
            if (e.KeyCode == Keys.F2)
            {
                // 編集モードに移行するF2キーが押下された場合も呼ばれるので、その場合は何もしない
                if (_isF2forSpreadEdit)
                {
                    _isF2forSpreadEdit = false;
                }
                else
                {
                    // ↓キー押下されたことにするが、あまり意味が無い
                    //e.SuppressKeyPress = true;
                    //e.Handled = true;

                    // 編集モードから非編集モードへの移行なので、常にifが成立
                    if (fpSpread_TokuisakiTanka.EditMode)
                    {
                        fpSpread_TokuisakiTanka.CancelCellEditing();
                    }
                    else
                    {
                        fpSpread_TokuisakiTanka.StartCellEditing(e, false);
                        //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_TokuisakiTanka.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                    }
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread_TokuisakiTanka_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread_TokuisakiTanka_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列を超える場合は次の行へ移動する
                    if (newColumnIndex >= TANKA_DELETE_COLUMN - 1)
                    {
                        newColumnIndex = 0;
                        newRowIndex++;
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread_TokuisakiTanka_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread_TokuisakiTanka_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread_TokuisakiTanka_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread_TokuisakiTanka.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }
        }
        /// <summary>
        /// 得意先単価Spreadボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_TokuisakiTanka_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                // 参照ボタン？
                if (e.Column == TANKA_REFERENCE_COLUMN)
                {
                    OpenForm901BySpread();
                }

                // 無効チェックボックス？
                if (e.Column == TANKA_INVALID_COLUMN)
                {
                    if (fpSpread_TokuisakiTanka_Sheet1.Cells[e.Row, TANKA_INVALID_COLUMN].Value as bool? == true)
                    {
                        fpSpread_TokuisakiTanka_Sheet1.Rows[e.Row].BackColor = SystemColors.ControlLight;
                        fpSpread_TokuisakiTanka_Sheet1.Rows[e.Row].ForeColor = SystemColors.GrayText;
                    }
                    else
                    {
                        fpSpread_TokuisakiTanka_Sheet1.Rows[e.Row].ResetBackColor();
                        fpSpread_TokuisakiTanka_Sheet1.Rows[e.Row].ResetForeColor();
                    }
                }

                // 削除ボタン？
                if (e.Column == TANKA_DELETE_COLUMN)
                {
                    // 最終行（=新規行）の場合は何もしない
                    if (e.Row == fpSpread_TokuisakiTanka_Sheet1.Rows.Count - 1)
                    {
                        return;
                    }
                    DataRow dr = ((DataRowView)tankaBindingSource.Current).Row;
                    dr.Delete();
                }
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
        /// 得意先単価Spreadから商品選択ダイアログを表示する
        /// </summary>
        private void OpenForm901BySpread()
        {
            try
            {
                // 商品選択ダイアログを表示
                Form901 formSyohinDlg = new Form901();
                SubFormOpenInit(formSyohinDlg);
                formSyohinDlg.FormClosed += new FormClosedEventHandler(Form901SyohinCD_FormClosed);
                formSyohinDlg.ShowDialog();
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
        /// 商品選択ダイアログクローズ（商品CD）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form901SyohinCD_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                // 親画面を閉じた後に子画面で閉じられた場合、親画面では何もしない
                if (this.IsDisposed)
                {
                    return;
                }

                this.Enabled = true;
                this.Activate();
                Form901 form = (Form901)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                    int newColumnIndex = fpSpread_TokuisakiTanka_Sheet1.ActiveColumnIndex;
                    do
                    {
                        newColumnIndex++;
                    } while (fpSpread_TokuisakiTanka_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                             fpSpread_TokuisakiTanka_Sheet1.Columns[newColumnIndex].CanFocus == false);
                    fpSpread_TokuisakiTanka_Sheet1.SetActiveCell(fpSpread_TokuisakiTanka_Sheet1.ActiveRowIndex, newColumnIndex);

                    Syohin syohin = form.SelectedSyohin;
                    fpSpread_TokuisakiTanka_Sheet1.Cells[fpSpread_TokuisakiTanka_Sheet1.ActiveRowIndex, TANKA_SYOHIN_CD_COLUMN].Value = syohin.商品ＣＤ;   // 商品CDセルに値を設定→dt_TokuisakiTanka_ColumnChanged()が呼ばれる
                }
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
        /// 得意先単価Spread編集完了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_TokuisakiTanka_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            // 既存の行のRowStateが、アクティブセルが他の行に移らないと変わらないのを、強制的に変更する
            DataRow dr = ((DataRowView)tankaBindingSource.Current).Row;
            dr.EndEdit();
        }

        /// <summary>
        /// コピーして新規作成ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                // 表示中の得意先マスタをコピーする
                Tokuisaki tokuisaki = new Tokuisaki(_tokuisaki);

                // 得意先マスタ詳細画面を開く
                // ※コピー不要の項目をクリア（または取得しない）するのは Form700D_Load（とそこから呼ぶ取得処理）で実施している
                Form700D form = new Form700D(tokuisaki, true);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                form.MdiParent = this.MdiParent;
                form.StartPosition = FormStartPosition.WindowsDefaultLocation;  // コピー元の詳細画面と重なって表示されるのを防ぐ
                form.Show();
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
        /// 履歴表示ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRireki_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formRireki == null || _formRireki.IsDisposed)
                {
                    _formRireki = null;
                    _formRireki = new Form700DH((int)_tokuisaki.得意先ＣＤ);
                    _formRireki.FormClosed += new FormClosedEventHandler(FormRireki_FormClosed);
                }
                _formRireki.Show();
                _formRireki.Activate();
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
        /// 得意先履歴クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormRireki_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                _formRireki = null;
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
        /// コース一覧ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCourse_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formCourse == null || _formCourse.IsDisposed)
                {
                    _formCourse = null;
                    _formCourse = new Form700DC((int)_tokuisaki.得意先ＣＤ);
                    _formCourse.FormClosed += new FormClosedEventHandler(FormCourse_FormClosed);
                }
                _formCourse.Show();
                _formCourse.Activate();
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
        /// コース一覧クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormCourse_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                _formCourse = null;
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
        /// 登録ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // 得意先マスタの入力チェック
                if (CanSaveTokuisaki() == false)
                {
                    return;
                }

                // 電話テーブルの入力チェック
                if (CanSaveTelToCust() == false)
                {
                    return;
                }

                // 得意先振込依頼人名テーブルの入力チェック
                if (CanSaveTokuisakiFurikomiIraiNin() == false)
                {
                    return;
                }

                // ┌─┬───┬───────────┐
                // │  │これは│これであってはならない│
                // ├─┼───┼───────────┤
                // │ 1│分配元│分配先                │
                // ├─┼───┼───────────┤
                // │ 2│分配先│分配元                │
                // ├─┼───┼───────────┤
                // │ 3│注文元│配送先                │
                // ├─┼───┼───────────┤
                // │ 4│配送先│注文元                │
                // ├─┼───┼───────────┤
                // │ 5│分配元│注文元                │
                // ├─┼───┼───────────┤
                // │ 6│分配元│配送先                │
                // ├─┼───┼───────────┤
                // │ 7│分配先│注文元                │
                // ├─┼───┼───────────┤
                // │ 8│分配先│配送先                │
                // ├─┼───┼───────────┤
                // │ 9│注文元│分配元                │
                // ├─┼───┼───────────┤
                // │10│注文元│分配先                │
                // ├─┼───┼───────────┤
                // │11│配送先│分配元                │
                // ├─┼───┼───────────┤
                // │12│配送先│分配先                │
                // └─┴───┴───────────┘

                // 分配先の入力チェック
                if (CanSaveBunpai() == false)
                {
                    return;
                }

                // 得意先単価マスタの入力チェック
                if (CanSaveTokuisakiTanka() == false)
                {
                    return;
                }

                // 注文元一覧を最新化しておく
                GetChumonMoto(_tokuisaki.得意先ＣＤ);

                // 配送先(と注文元)のチェック
                if (CanSaveHaiso() == false)
                {
                    return;
                }

                // 分配と配送の入力チェック
                if (CanSaveBunpaiHaiso() == false)
                {
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                if (Save())
                {
                    // 再読み込み
                    Reload();
                }
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
            finally
            {
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 削除ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // 削除対象のルートがほかのテーブルで使用されているかチェック
                if (CanDelete() == false)
                {
                    return;
                }

                if (MessageBox.Show(this, "このデータを削除しますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }

                if (Delete())
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }
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
        /// 振替用EB口座コンボボックス設定
        /// </summary>
        /// <param name="cbo">振替用EB口座コンボボックス</param>
        /// <param name="busyoCD">部署ＣＤ</param>
        private void SetBusyoEB(ComboBox cbo, int? busyoCD)
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     '' AS 銀行ＣＤ" +
                                      "\r\n     ,'' AS 支店ＣＤ" +
                                      "\r\n     ,'' AS 銀行支店ＣＤ" +
                                      "\r\n     ,'' AS EBセット名" +
                                      "\r\n UNION" +
                                      "\r\n SELECT" +
                                      "\r\n     A.銀行ＣＤ" +
                                      "\r\n     ,A.支店ＣＤ" +
                                      "\r\n     ,A.銀行ＣＤ + A.支店ＣＤ AS 銀行支店ＣＤ" +
                                      "\r\n     ,A.EBセット名" +
                                      "\r\n FROM" +
                                      "\r\n     部署EB設定マスタ AS A" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND A.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     銀行ＣＤ" +
                                      "\r\n     ,支店ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = busyoCD ?? -1;  // null の場合は -1 で検索（新規の場合は0件を取得）

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("部署EB設定マスタ");
                    adp.Fill(dt);

                    cbo.DisplayMember = "EBセット名";
                    cbo.ValueMember = "銀行支店ＣＤ";
                    cbo.DataSource = dt;
                }
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
        /// 電話テーブル取得
        /// </summary>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        private void GetTelToCust(int? tokuisakiCD)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     A.企業ＣＤ" +
                                      "\r\n     ,A.Tel_TelNo" +
                                      "\r\n     ,A.Tel_TelNo AS Tel_TelNoORG" +
                                      "\r\n     ,A.Tel_CustNo" +
                                      "\r\n     ,A.Tel_CustNo AS Tel_CustNoORG" +
                                      "\r\n     ,A.Tel_RepFlg" +
                                      "\r\n     ,A.Tel_DelFlg" +
                                      "\r\n     ,A.Tel_Remarks" +
                                      "\r\n     ,A.Tel_NewDate" +
                                      "\r\n     ,A.Tel_UpdDate" +
                                      "\r\n FROM" +
                                      "\r\n     C_TelToCust AS A" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ = @企業ＣＤ AND" +
                                      "\r\n     A.Tel_CustNo = @Tel_CustNo" +
                                      "\r\n ORDER BY" +
                                      "\r\n     A.Tel_RepFlg, A.Tel_TelNo";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@Tel_CustNo", SqlDbType.Int).Value = _IsCopy ? -1 : (tokuisakiCD ?? -1);  // コピーして新規作成 または 得意先ＣＤが null の場合は -1 で検索（新規の場合は0件を取得）

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("C_TelToCust");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    dt.ColumnChanged += new DataColumnChangeEventHandler(dt_TelToCust_ColumnChanged);

                    telBindingSource.DataSource = ds;
                    telBindingSource.DataMember = "C_TelToCust";

                    log.Info("電話テーブルを検索しました。");
                }

                fpSpread_Tel_Sheet1.BindDataColumn(TEL_KBN_COLUMN, "Tel_RepFlg");
                fpSpread_Tel_Sheet1.BindDataColumn(TEL_NUMBER_COLUMN, "Tel_TelNo");
                fpSpread_Tel_Sheet1.BindDataColumn(TEL_REMARK_COLUMN, "Tel_Remarks");

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
        /// 電話テーブルデータソース列値変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dt_TelToCust_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            // イベントの再帰呼び出しを回避する
            if (_isDtColumnChanging)
            {
                return;
            }

            try
            {
                _isDtColumnChanging = true;

                // 特に何もしない
                // 将来必要になったときのためにメソッドだけ用意している
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
            finally
            {
                _isDtColumnChanging = false;
            }
        }

        /// <summary>
        /// 得意先振込依頼人名テーブル取得
        /// </summary>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        private void GetTokuisakiFurikomiIraiNin(int? tokuisakiCD)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     A.企業ＣＤ" +
                                      "\r\n     ,A.得意先ＣＤ" +
                                      "\r\n     ,A.依頼人ＣＤ" +
                                      "\r\n     ,A.振込依頼人名" +
                                      "\r\n     ,A.作成担当者ＣＤ" +
                                      "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                      "\r\n     ,A.作成日" +
                                      "\r\n     ,A.修正担当者ＣＤ" +
                                      "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                      "\r\n     ,A.修正日" +
                                      "\r\n FROM" +
                                      "\r\n     得意先振込依頼人名 AS A" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n AND A.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     A.依頼人ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _IsCopy ? -1 : (tokuisakiCD ?? -1);  // コピーして新規作成 または 得意先ＣＤが null の場合は -1 で検索（新規の場合は0件を取得）

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("得意先振込依頼人名テーブル");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    dt.ColumnChanged += new DataColumnChangeEventHandler(dt_TokuisakiFurikomiIraiNin_ColumnChanged);

                    furikomiBindingSource.DataSource = ds;
                    furikomiBindingSource.DataMember = "得意先振込依頼人名テーブル";

                    log.Info("得意先振込依頼人名テーブルを検索しました。");
                }

                fpSpread_Furikomi_Sheet1.BindDataColumn(FURI_NAME_COLUMN, "振込依頼人名");
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
        /// 得意先振込依頼人名テーブルデータソース列値変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dt_TokuisakiFurikomiIraiNin_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            // イベントの再帰呼び出しを回避する
            if (_isDtColumnChanging)
            {
                return;
            }

            try
            {
                _isDtColumnChanging = true;

                // 特に何もしない
                // 将来必要になったときのためにメソッドだけ用意している
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
            finally
            {
                _isDtColumnChanging = false;
            }
        }

        /// <summary>
        /// 分配先取得
        /// </summary>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        private void GetBunpai(int? tokuisakiCD)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n      A.企業ＣＤ" +
                                      "\r\n      ,A.得意先ＣＤ" +
                                      "\r\n      ,A.得意先ＣＤ AS 得意先ＣＤorg" +
                                      "\r\n      ,A.得意先名" +
                                      "\r\n      ,A.得意先名カナ" +
                                      "\r\n      ,A.得意先名略称" +
                                      "\r\n      ,A.郵便番号" +
                                      "\r\n      ,A.住所1" +
                                      "\r\n      ,A.住所2" +
                                      "\r\n      ,A.電話番号1" +
                                      "\r\n      ,A.ＦＡＸ1" +
                                      "\r\n      ,A.電話番号2" +
                                      "\r\n      ,A.ＦＡＸ2" +
                                      "\r\n      ,A.お届け先郵便番号" +
                                      "\r\n      ,A.お届け先住所1" +
                                      "\r\n      ,A.お届け先住所2" +
                                      "\r\n      ,A.お届け先電話番号1" +
                                      "\r\n      ,A.お届け先ＦＡＸ1" +
                                      "\r\n      ,A.お届け先電話番号2" +
                                      "\r\n      ,A.お届け先ＦＡＸ2" +
                                      "\r\n      ,A.部署ＣＤ" +
                                      "\r\n      ,A.売掛現金区分" +
                                      "\r\n      ,A.電話確認区分" +
                                      "\r\n      ,A.締区分" +
                                      "\r\n      ,A.月締日" +
                                      "\r\n      ,A.週締曜日" +
                                      "\r\n      ,A.支払サイト" +
                                      "\r\n      ,A.支払日" +
                                      "\r\n      ,A.請求先ＣＤ" +
                                      "\r\n      ,A.支払方法" +
                                      "\r\n      ,A.振込手数料フラグ" +
                                      "\r\n      ,A.部署合算請求フラグ" +
                                      "\r\n      ,A.金融機関ＣＤ" +
                                      "\r\n      ,A.金融機関支店ＣＤ" +
                                      "\r\n      ,A.記号番号" +
                                      "\r\n      ,A.口座種別" +
                                      "\r\n      ,A.口座番号" +
                                      "\r\n      ,A.口座名義人" +
                                      "\r\n      ,A.チケット枚数" +
                                      "\r\n      ,A.サービスチケット枚数" +
                                      "\r\n      ,A.営業担当者ＣＤ" +
                                      "\r\n      ,A.獲得営業者ＣＤ" +
                                      "\r\n      ,A.登録担当者ＣＤ" +
                                      "\r\n      ,A.受注得意先ＣＤ" +
                                      "\r\n      ,A.受注方法" +
                                      "\r\n      ,A.電話確認時間_時" +
                                      "\r\n      ,A.電話確認時間_分" +
                                      "\r\n      ,A.味噌汁区分" +
                                      "\r\n      ,A.ふりかけ区分" +
                                      "\r\n      ,A.納品書発行区分" +
                                      "\r\n      ,A.空き容器回収区分" +
                                      "\r\n      ,A.請求書敬称" +
                                      "\r\n      ,A.備考１" +
                                      "\r\n      ,A.備考２" +
                                      "\r\n      ,A.備考３" +
                                      "\r\n      ,A.状態区分" +
                                      "\r\n      ,A.承認日" +
                                      "\r\n      ,A.承認者ＣＤ" +
                                      "\r\n      ,A.状態理由区分" +
                                      "\r\n      ,A.休日設定" +
                                      "\r\n      ,A.新規登録日" +
                                      "\r\n      ,A.税区分" +
                                      "\r\n      ,A.丸め処理区分" +
                                      "\r\n      ,A.祝日配送区分" +
                                      "\r\n      ,A.誕生日1" +
                                      "\r\n      ,A.誕生日2" +
                                      "\r\n      ,A.失客日" +
                                      "\r\n      ,A.EB銀行支店ＣＤ" +
                                      "\r\n      ,A.受注時特記事項" +
                                      "\r\n      ,A.請求書郵便マークフラグ" +
                                      "\r\n      ,A.集計ＣＤ" +
                                      "\r\n      ,A.配送得意先ＣＤ" +
                                      "\r\n      ,A.無効フラグ" +
                                      "\r\n      ,A.作成担当者ＣＤ" +
                                      "\r\n      ,A1.担当者名 AS 作成担当者名" +
                                      "\r\n      ,A.作成日" +
                                      "\r\n      ,A.修正担当者ＣＤ" +
                                      "\r\n      ,A2.担当者名 AS 修正担当者名" +
                                      "\r\n      ,A.修正日" +
                                      "\r\n      ,B.部署名" +
                                      "\r\n FROM" +
                                      "\r\n      得意先マスタ AS A" +
                                      "\r\n      LEFT JOIN 担当者マスタ     AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                      "\r\n      LEFT JOIN 担当者マスタ     AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                      "\r\n      LEFT JOIN 部署マスタ       AS B  ON  B.企業ＣＤ = @企業ＣＤ AND ISNULL(B.無効フラグ, 0) = 0 AND A.部署ＣＤ = B.部署ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n      A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n  AND A.受注得意先ＣＤ = @受注得意先ＣＤ" +
                                      "\r\n ORDER BY A.得意先ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@受注得意先ＣＤ", SqlDbType.Int).Value = _IsCopy ? -1 : (tokuisakiCD ?? -1);  // コピーして新規作成 または 得意先ＣＤが null の場合は -1 で検索（新規の場合は0件を取得）

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("分配先");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    dt.ColumnChanged += new DataColumnChangeEventHandler(dt_Bunpai_ColumnChanged);

                    bunpaiBindingSource.DataSource = ds;
                    bunpaiBindingSource.DataMember = "分配先";

                    log.Info("分配先を検索しました。");
                }

                fpSpread_Bunpai_Sheet1.BindDataColumn(BUNPAI_TOKUISAKI_CD_COLUMN, "得意先ＣＤ");
                fpSpread_Bunpai_Sheet1.BindDataColumn(BUNPAI_TOKUISAKI_NAME_COLUMN, "得意先名");
                fpSpread_Bunpai_Sheet1.BindDataColumn(BUNPAI_TOKUISAKI_KANA_COLUMN, "得意先名カナ");
                fpSpread_Bunpai_Sheet1.BindDataColumn(BUNPAI_TOKUISAKI_TEL_COLUMN, "電話番号1");
                fpSpread_Bunpai_Sheet1.BindDataColumn(BUNPAI_BUSYO_COLUMN, "部署名");
                fpSpread_Bunpai_Sheet1.BindDataColumn(BUNPAI_INVALID_COLUMN, "無効フラグ");

                // 無効フラグの値に応じて行の背景色を変える
                for (int rowIndex = 0; rowIndex < fpSpread_Bunpai_Sheet1.Rows.Count; rowIndex++)
                {
                    fpSpread_Bunpai_Sheet1.Rows[rowIndex].ResetBackColor();
                    fpSpread_Bunpai_Sheet1.Rows[rowIndex].ResetForeColor();
                    if (fpSpread_Bunpai_Sheet1.Cells[rowIndex, BUNPAI_INVALID_COLUMN].Value as bool? == true)
                    {
                        fpSpread_Bunpai_Sheet1.Rows[rowIndex].BackColor = SystemColors.ControlLight;
                        fpSpread_Bunpai_Sheet1.Rows[rowIndex].ForeColor = SystemColors.GrayText;
                    }
                }
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
        /// 分配先データソース列値変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dt_Bunpai_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            // イベントの再帰呼び出しを回避する
            if (_isDtColumnChanging)
            {
                return;
            }

            try
            {
                fpSpread_Bunpai.SuspendLayout();      // 得意先ＣＤを入力しても得意先名などが描画されないことがあるのでそれを回避する

                _isDtColumnChanging = true;

                Tokuisaki bunpaiChild = new Tokuisaki(e.Row);

                // 得意先CDの場合は得意先名を取得
                if (e.Column.ColumnName == "得意先ＣＤ")
                {
                    e.Row.SetColumnError(e.Column, null);
                    e.Row["得意先名"] = null;
                    e.Row["得意先名カナ"] = null;
                    e.Row["電話番号1"] = null;
                    e.Row["部署名"] = null;
                    e.Row["無効フラグ"] = DBNull.Value;
                    e.Row["修正日"] = DBNull.Value;
                    if (bunpaiChild.得意先ＣＤ != null)
                    {
                        Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)bunpaiChild.得意先ＣＤ);
                        if (tokuisaki != null)
                        {
                            e.Row["得意先名"] = tokuisaki.得意先名;
                            e.Row["得意先名カナ"] = tokuisaki.得意先名カナ;
                            e.Row["電話番号1"] = tokuisaki.電話番号1;
                            e.Row["部署名"] = tokuisaki.部署名;
                            e.Row["無効フラグ"] = tokuisaki.無効フラグ;     // 無効フラグが立っているものが追加されることはないので背景色は変更不要
                        }
                        else
                        {
                            e.Row.SetColumnError(e.Column, "得意先が見つかりません。");
                        }
                    }
                }
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
            finally
            {
                _isDtColumnChanging = false;
                fpSpread_Bunpai.ResumeLayout(true);       // 得意先ＣＤを入力しても得意先名などが描画されないことがあるのでそれを回避する
            }
        }

        /// <summary>
        /// 得意先単価マスタ取得
        /// </summary>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        private void GetTokuisakiTanka(int? tokuisakiCD)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    string sql = "\r\n SELECT" +
                                 "\r\n     A.企業ＣＤ" +
                                 "\r\n     ,{0} AS 得意先ＣＤ" +
                                 "\r\n     ,A.商品ＣＤ" +
                                 "\r\n     ,A.商品ＣＤ AS 商品ＣＤorg" +
                                 "\r\n     ,S.商品名" +
                                 "\r\n     ,S.商品名略称" +
                                 "\r\n     ,BG.部署グループ名" +
                                 "\r\n     ,S.商品単価税込 AS 標準単価税込" +
                                 "\r\n     ,S.商品単価税抜 AS 標準単価税抜" +
                                 "\r\n     ,S.軽減税率フラグ" +
                                 "\r\n     ,A.商品単価税込" +
                                 "\r\n     ,A.商品単価税抜" +
                                 "\r\n     ,A.無効フラグ" +
                                 "\r\n     ,A.作成担当者ＣＤ" +
                                 "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                 "\r\n     ,A.作成日" +
                                 "\r\n     ,A.修正担当者ＣＤ" +
                                 "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                 "\r\n     ,A.修正日" +
                                 "\r\n FROM" +
                                 "\r\n     得意先単価マスタ AS A" +
                                 "\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = @企業ＣＤ AND A.商品ＣＤ = S.商品ＣＤ" +
                                 "\r\n     LEFT JOIN V_部署グループ AS BG ON S.部署グループ = BG.部署グループCD" +
                                 "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                 "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                 "\r\n WHERE" +
                                 "\r\n     A.企業ＣＤ   = @企業ＣＤ" +
                                 "\r\n AND A.得意先ＣＤ = @得意先ＣＤ" +
                                 "\r\n ORDER BY" +
                                 "\r\n     A.商品ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD ?? -1;  // null の場合は -1 で検索（新規の場合は0件を取得）

                    cmd.CommandText = string.Format(sql, _IsCopy == true ? "null" : "A.得意先ＣＤ");    // コピーして新規作成の場合は得意先ＣＤを取得しない

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("得意先単価マスタ");
                    adp.Fill(dt);

                    // コピーして新規作成の場合は、取得した明細を全て追加状態にする
                    if (_IsCopy)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (dr.RowState == DataRowState.Unchanged)
                            {
                                dr.SetAdded();
                            }
                        }
                    }

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    dt.ColumnChanged += new DataColumnChangeEventHandler(dt_TokuisakiTanka_ColumnChanged);

                    tankaBindingSource.DataSource = ds;
                    tankaBindingSource.DataMember = "得意先単価マスタ";

                    log.Info("得意先単価マスタを検索しました。");
                }

                fpSpread_TokuisakiTanka_Sheet1.BindDataColumn(TANKA_SYOHIN_CD_COLUMN,      "商品ＣＤ");
                fpSpread_TokuisakiTanka_Sheet1.BindDataColumn(TANKA_SYOHIN_NAME_COLUMN,    "商品名");
                fpSpread_TokuisakiTanka_Sheet1.BindDataColumn(TANKA_SYOHIN_RYAKU_COLUMN,   "商品名略称");
                fpSpread_TokuisakiTanka_Sheet1.BindDataColumn(TANKA_BUSYO_GROUP_COLUMN,    "部署グループ名");
                fpSpread_TokuisakiTanka_Sheet1.BindDataColumn(TANKA_HYOJUN_KOMI_COLUMN,    "標準単価税込");
                fpSpread_TokuisakiTanka_Sheet1.BindDataColumn(TANKA_HYOJUN_NUKI_COLUMN,    "標準単価税抜");
                fpSpread_TokuisakiTanka_Sheet1.BindDataColumn(TANKA_TOKUISAKI_KOMI_COLUMN, "商品単価税込");
                fpSpread_TokuisakiTanka_Sheet1.BindDataColumn(TANKA_TOKUISAKI_NUKI_COLUMN, "商品単価税抜");
                fpSpread_TokuisakiTanka_Sheet1.BindDataColumn(TANKA_INVALID_COLUMN,        "無効フラグ");

                // 無効フラグの値に応じて行の背景色を変える
                for (int rowIndex = 0; rowIndex < fpSpread_TokuisakiTanka_Sheet1.Rows.Count; rowIndex++)
                {
                    fpSpread_TokuisakiTanka_Sheet1.Rows[rowIndex].ResetBackColor();
                    fpSpread_TokuisakiTanka_Sheet1.Rows[rowIndex].ResetForeColor();
                    if (fpSpread_TokuisakiTanka_Sheet1.Cells[rowIndex, TANKA_INVALID_COLUMN].Value as bool? == true)
                    {
                        fpSpread_TokuisakiTanka_Sheet1.Rows[rowIndex].BackColor = SystemColors.ControlLight;
                        fpSpread_TokuisakiTanka_Sheet1.Rows[rowIndex].ForeColor = SystemColors.GrayText;
                    }
                }
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
        /// 得意先単価マスタデータソース列値変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dt_TokuisakiTanka_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            // イベントの再帰呼び出しを回避する
            if (_isDtColumnChanging)
            {
                return;
            }

            try
            {
                fpSpread_TokuisakiTanka.SuspendLayout();      // 商品ＣＤを入力しても単価が描画されないことがあるのでそれを回避する

                _isDtColumnChanging = true;

                TokuisakiTanka tokuisakiTanka = new TokuisakiTanka(e.Row);

                // 商品CDの場合は商品～標準単価を取得
                if (e.Column.ColumnName == "商品ＣＤ")
                {
                    e.Row.SetColumnError(e.Column, null);
                    e.Row["商品名"] = null;
                    e.Row["商品名略称"] = null;
                    e.Row["部署グループ名"] = null;
                    e.Row["標準単価税込"] = DBNull.Value;
                    e.Row["標準単価税抜"] = DBNull.Value;
                    e.Row["商品単価税込"] = DBNull.Value;
                    e.Row["商品単価税抜"] = DBNull.Value;
                    e.Row["無効フラグ"] = DBNull.Value;
                    if (tokuisakiTanka.商品ＣＤ != null)
                    {
                        Syohin syohin = Syohin.GetSyohin((int)tokuisakiTanka.商品ＣＤ);
                        if (syohin != null)
                        {
                            e.Row["商品名"] = syohin.商品名;
                            e.Row["商品名略称"] = syohin.商品名略称;
                            e.Row["部署グループ名"] = syohin.部署グループ名;
                            e.Row["標準単価税込"] = syohin.商品単価税込;
                            e.Row["標準単価税抜"] = syohin.商品単価税抜;
                            e.Row["軽減税率フラグ"] = syohin.軽減税率フラグ;    // 税抜き単価算出用
                        }
                        else
                        {
                            e.Row.SetColumnError(e.Column, "商品が見つかりません。");
                        }
                    }
                }

                // 単価の場合はエラーチェック
                if (e.Column.ColumnName == "商品単価税込")
                {
                    e.Row.SetColumnError(e.Column, tokuisakiTanka["商品単価税込"]);
                    if (tokuisakiTanka.商品単価税込 != null)
                    {
                        // 税抜き単価を算出
                        CalcTokuisakiTankaZeiNuki(e.Row, _tokuisaki.丸め処理区分);
                    }
                }

                if (e.Column.ColumnName == "商品単価税抜")
                {
                    e.Row.SetColumnError(e.Column, tokuisakiTanka["商品単価税抜"]);
                }

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
            finally
            {
                _isDtColumnChanging = false;
                fpSpread_TokuisakiTanka.ResumeLayout(true);      // 商品ＣＤを入力しても単価が描画されないことがあるのでそれを回避する
            }
        }

        /// <summary>
        /// 注文元取得
        /// </summary>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        private void GetChumonMoto(int? tokuisakiCD)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n      A.企業ＣＤ" +
                                      "\r\n      ,A.得意先ＣＤ" +
                                      "\r\n      ,A.得意先ＣＤ AS 得意先ＣＤorg" +
                                      "\r\n      ,A.得意先名" +
                                      "\r\n      ,A.得意先名カナ" +
                                      "\r\n      ,A.得意先名略称" +
                                      "\r\n      ,A.郵便番号" +
                                      "\r\n      ,A.住所1" +
                                      "\r\n      ,A.住所2" +
                                      "\r\n      ,A.電話番号1" +
                                      "\r\n      ,A.ＦＡＸ1" +
                                      "\r\n      ,A.電話番号2" +
                                      "\r\n      ,A.ＦＡＸ2" +
                                      "\r\n      ,A.お届け先郵便番号" +
                                      "\r\n      ,A.お届け先住所1" +
                                      "\r\n      ,A.お届け先住所2" +
                                      "\r\n      ,A.お届け先電話番号1" +
                                      "\r\n      ,A.お届け先ＦＡＸ1" +
                                      "\r\n      ,A.お届け先電話番号2" +
                                      "\r\n      ,A.お届け先ＦＡＸ2" +
                                      "\r\n      ,A.部署ＣＤ" +
                                      "\r\n      ,A.売掛現金区分" +
                                      "\r\n      ,A.電話確認区分" +
                                      "\r\n      ,A.締区分" +
                                      "\r\n      ,A.月締日" +
                                      "\r\n      ,A.週締曜日" +
                                      "\r\n      ,A.支払サイト" +
                                      "\r\n      ,A.支払日" +
                                      "\r\n      ,A.請求先ＣＤ" +
                                      "\r\n      ,A.支払方法" +
                                      "\r\n      ,A.振込手数料フラグ" +
                                      "\r\n      ,A.部署合算請求フラグ" +
                                      "\r\n      ,A.金融機関ＣＤ" +
                                      "\r\n      ,A.金融機関支店ＣＤ" +
                                      "\r\n      ,A.記号番号" +
                                      "\r\n      ,A.口座種別" +
                                      "\r\n      ,A.口座番号" +
                                      "\r\n      ,A.口座名義人" +
                                      "\r\n      ,A.チケット枚数" +
                                      "\r\n      ,A.サービスチケット枚数" +
                                      "\r\n      ,A.営業担当者ＣＤ" +
                                      "\r\n      ,A.獲得営業者ＣＤ" +
                                      "\r\n      ,A.登録担当者ＣＤ" +
                                      "\r\n      ,A.受注得意先ＣＤ" +
                                      "\r\n      ,A.受注方法" +
                                      "\r\n      ,A.電話確認時間_時" +
                                      "\r\n      ,A.電話確認時間_分" +
                                      "\r\n      ,A.味噌汁区分" +
                                      "\r\n      ,A.ふりかけ区分" +
                                      "\r\n      ,A.納品書発行区分" +
                                      "\r\n      ,A.空き容器回収区分" +
                                      "\r\n      ,A.請求書敬称" +
                                      "\r\n      ,A.備考１" +
                                      "\r\n      ,A.備考２" +
                                      "\r\n      ,A.備考３" +
                                      "\r\n      ,A.状態区分" +
                                      "\r\n      ,A.承認日" +
                                      "\r\n      ,A.承認者ＣＤ" +
                                      "\r\n      ,A.状態理由区分" +
                                      "\r\n      ,A.休日設定" +
                                      "\r\n      ,A.新規登録日" +
                                      "\r\n      ,A.税区分" +
                                      "\r\n      ,A.丸め処理区分" +
                                      "\r\n      ,A.祝日配送区分" +
                                      "\r\n      ,A.誕生日1" +
                                      "\r\n      ,A.誕生日2" +
                                      "\r\n      ,A.失客日" +
                                      "\r\n      ,A.EB銀行支店ＣＤ" +
                                      "\r\n      ,A.受注時特記事項" +
                                      "\r\n      ,A.請求書郵便マークフラグ" +
                                      "\r\n      ,A.集計ＣＤ" +
                                      "\r\n      ,A.配送得意先ＣＤ" +
                                      "\r\n      ,A.無効フラグ" +
                                      "\r\n      ,A.作成担当者ＣＤ" +
                                      "\r\n      ,A1.担当者名 AS 作成担当者名" +
                                      "\r\n      ,A.作成日" +
                                      "\r\n      ,A.修正担当者ＣＤ" +
                                      "\r\n      ,A2.担当者名 AS 修正担当者名" +
                                      "\r\n      ,A.修正日" +
                                      "\r\n      ,B.部署名" +
                                      "\r\n FROM" +
                                      "\r\n      得意先マスタ AS A" +
                                      "\r\n      LEFT JOIN 担当者マスタ     AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                      "\r\n      LEFT JOIN 担当者マスタ     AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                      "\r\n      LEFT JOIN 部署マスタ       AS B  ON  B.企業ＣＤ = @企業ＣＤ AND ISNULL(B.無効フラグ, 0) = 0 AND A.部署ＣＤ = B.部署ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n      A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n  AND A.配送得意先ＣＤ = @配送得意先ＣＤ" +
                                      "\r\n  AND A.得意先ＣＤ <> @配送得意先ＣＤ" +     // -- 配送得意先ＣＤに自分自身の得意先ＣＤが設定されることになった。そのため、自分自身のレコードは取得しない(★)
                                      "\r\n ORDER BY A.得意先ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@配送得意先ＣＤ", SqlDbType.Int).Value = _IsCopy ? -1 : (tokuisakiCD ?? -1);  // コピーして新規作成 または 得意先ＣＤが null の場合は -1 で検索（新規の場合は0件を取得）

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("注文元");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    chumonMotoBindingSource.DataSource = ds;
                    chumonMotoBindingSource.DataMember = "注文元";

                    log.Info("注文元を検索しました。");
                }

                fpSpread_ChumonMoto_Sheet1.BindDataColumn(CHUMONMOTO_TOKUISAKI_CD_COLUMN,   "得意先ＣＤ");
                fpSpread_ChumonMoto_Sheet1.BindDataColumn(CHUMONMOTO_TOKUISAKI_NAME_COLUMN, "得意先名");
                fpSpread_ChumonMoto_Sheet1.BindDataColumn(CHUMONMOTO_TOKUISAKI_KANA_COLUMN, "得意先名カナ");
                fpSpread_ChumonMoto_Sheet1.BindDataColumn(CHUMONMOTO_TOKUISAKI_TEL_COLUMN,  "電話番号1");
                fpSpread_ChumonMoto_Sheet1.BindDataColumn(CHUMONMOTO_BUSYO_COLUMN,          "部署名");
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
        /// 得意先単価マスタの税抜き単価を算出
        /// </summary>
        /// <param name="dr">得意先単価マスタレコード</param>
        /// <param name="marumeKubun">端数区分（丸め区分）</param>
        private void CalcTokuisakiTankaZeiNuki(DataRow dr, int? marumeKubun)
        {
            try
            {
                // 消費税の丸め区分を決定
                if (marumeKubun == null || marumeKubun < 0)
                {
                    marumeKubun = (int)Rounding.HasuKubun.Floor;
                }

                // 単価の丸め区分を決定（消費税を切り捨てたら、単価は切り上げる）
                int hasuKubun = (int)Rounding.HasuKubun.Round;
                switch (marumeKubun)
                {
                    case (int)Rounding.HasuKubun.Ceiling:
                        hasuKubun = (int)Rounding.HasuKubun.Floor;
                        break;

                    case (int)Rounding.HasuKubun.Floor:
                        hasuKubun = (int)Rounding.HasuKubun.Ceiling;
                        break;
                }
                decimal taxRate = _syohizei.GetTaxRate(dr["軽減税率フラグ"] as bool?);
                decimal? price = dr["商品単価税込"] as decimal?;
                if (price == null)
                {
                    dr["商品単価税抜"] = DBNull.Value;
                    return;
                }
                price = price / (1m + taxRate);
                price = Rounding.Round((decimal)price, hasuKubun);
                dr["商品単価税抜"] = price;
                dr.SetColumnError("商品単価税抜", null);   // 税抜き単価のエラーをクリア
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
        /// 得意先マスタが変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChangeTokuisaki()
        {
            if (_tokuisaki.Equals(_origin))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 電話テーブルが変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChangeTelToCust()
        {
            // 電話テーブルを比較
            DataSet ds = (DataSet)telBindingSource.DataSource;
            if (ds == null)
            {
                return false;
            }

            DataTable dt = ds.Tables["C_TelToCust"];

            foreach (DataRow dr in dt.Rows)
            {
                switch (dr.RowState)
                {
                    case DataRowState.Added:        // 追加
                    case DataRowState.Modified:     // 変更
                    case DataRowState.Deleted:      // 削除
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 得意先振込依頼人名テーブルが変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChangeTokuisakiFurikomiIraiNin()
        {
            // 得意先振込依頼人名テーブルを比較
            DataSet ds = (DataSet)furikomiBindingSource.DataSource;
            if (ds == null)
            {
                return false;
            }

            DataTable dt = ds.Tables["得意先振込依頼人名テーブル"];

            foreach (DataRow dr in dt.Rows)
            {
                switch (dr.RowState)
                {
                    case DataRowState.Added:        // 追加
                    case DataRowState.Modified:     // 変更
                    case DataRowState.Deleted:      // 削除
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 分配先テーブルが変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChangeBunpai()
        {
            // 分配先テーブルを比較
            DataSet ds = (DataSet)bunpaiBindingSource.DataSource;
            if (ds == null)
            {
                return false;
            }

            DataTable dt = ds.Tables["分配先"];

            foreach (DataRow dr in dt.Rows)
            {
                switch (dr.RowState)
                {
                    case DataRowState.Added:        // 追加
                    case DataRowState.Modified:     // 変更
                    case DataRowState.Deleted:      // 削除
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 得意先単価マスタが変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChangeTokuisakiTanka()
        {
            // 得意先単価マスタを比較
            DataSet ds = (DataSet)tankaBindingSource.DataSource;
            if (ds == null)
            {
                return false;
            }

            DataTable dt = ds.Tables["得意先単価マスタ"];

            foreach (DataRow dr in dt.Rows)
            {
                switch (dr.RowState)
                {
                    case DataRowState.Added:        // 追加
                    case DataRowState.Modified:     // 変更
                    case DataRowState.Deleted:      // 削除
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 得意先マスタが登録可能か？
        /// </summary>
        /// <returns>登録可能な場合はtrue</returns>
        private bool CanSaveTokuisaki()
        {
            try
            {
                if (this.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "編集できる権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                if (_tokuisaki.HasErrors)
                {
                    MessageBox.Show(this, _tokuisaki.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 記号番号が入力されていれば、支店CDと口座番号を算出し直す
                if (_tokuisaki.記号番号 != null)
                {
                    _tokuisaki.記号番号 = _tokuisaki.記号番号;
                }

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
        /// 電話テーブルが登録可能か？
        /// </summary>
        /// <returns>登録可能な場合はtrue</returns>
        private bool CanSaveTelToCust()
        {
            bool hasError = false;

            try
            {
                DataSet ds = (DataSet)telBindingSource.DataSource;
                DataTable dt = ds.Tables["C_TelToCust"];

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    dr.ClearErrors();

                    // 削除された行についてはエラーチェックは不要
                    if (dr.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }

                    TelToCust telToCust = new TelToCust(dr);

                    // 得意先CDがない場合は新規扱い
                    if (telToCust.Tel_CustNo == null)
                    {
                        telToCust.Tel_CustNo = _tokuisaki.得意先ＣＤ;
                    }

                    // エラーがある？
                    if (telToCust.HasErrors)
                    {
                        dr.SetColumnError("Tel_RepFlg", telToCust["Tel_RepFlg"]);
                        dr.SetColumnError("Tel_TelNo", telToCust["Tel_TelNo"]);
                        hasError = true;
                    }
                }

                if (hasError)
                {
                    MessageBox.Show(this, "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

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
        /// 得意先振込依頼人名テーブルが登録可能か？
        /// </summary>
        /// <returns>登録可能な場合はtrue</returns>
        private bool CanSaveTokuisakiFurikomiIraiNin()
        {
            bool hasError = false;

            try
            {
                DataSet ds = (DataSet)furikomiBindingSource.DataSource;
                DataTable dt = ds.Tables["得意先振込依頼人名テーブル"];

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    dr.ClearErrors();

                    // 削除された行についてはエラーチェックは不要
                    if (dr.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }

                    TokuisakiFurikomiIraiNin tokuisakiFurikomiIraiNin = new TokuisakiFurikomiIraiNin(dr);

                    // エラーがある？
                    if (tokuisakiFurikomiIraiNin.HasErrors)
                    {
                        dr.SetColumnError("振込依頼人名", tokuisakiFurikomiIraiNin["振込依頼人名"]);
                        hasError = true;
                    }
                }

                if (hasError)
                {
                    MessageBox.Show(this, "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

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
        /// 分配先が登録可能か？
        /// </summary>
        /// <returns>登録可能な場合はtrue</returns>
        private bool CanSaveBunpai()
        {
            bool hasError = false;

            try
            {
                DataSet ds = (DataSet)bunpaiBindingSource.DataSource;
                DataTable dt = ds.Tables["分配先"];

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    dr.ClearErrors();

                    // 削除された行についてはエラーチェックは不要
                    if (dr.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }

                    Tokuisaki bunpaiChild = new Tokuisaki(dr);

                    // 得意先CDが未入力？
                    if (bunpaiChild.得意先ＣＤ == null)
                    {
                        dr.SetColumnError("得意先ＣＤ", bunpaiChild["得意先ＣＤ"]);
                        hasError = true;
                    }
                    else
                    {
                        // 得意先ＣＤに該当するマスタが存在するか？（無効であっても検索）
                        Tokuisaki target = Tokuisaki.GetTokuisakiReload((int)bunpaiChild.得意先ＣＤ);
                        if (target == null)
                        {
                            dr.SetColumnError("得意先ＣＤ", "得意先が見つかりません。");
                            hasError = true;
                        }
                        else
                        {
                            //*****************************************************
                            // 登録する得意先を親、分配先を子として、以下をチェック
                            //*****************************************************
                            // ┌─┬───┬───────────┐
                            // │  │これは│これであってはならない│
                            // ├─┼───┼───────────┤
                            // │ 1│分配元│分配先                │
                            // ├─┼───┼───────────┤
                            // │ 2│分配先│分配元                │
                            // └─┴───┴───────────┘
                            // 子として追加できる得意先の受注顧客がnullまたは親と同じであること（既に誰かの子でないこと）
                            if (target.受注得意先ＣＤ != null && target.受注得意先ＣＤ != _tokuisaki.得意先ＣＤ)
                            {
                                dr.SetColumnError("得意先ＣＤ", "別の得意先の分配先です。分配先として指定することはできません。");
                                hasError = true;
                            }

                            // 親を子として指定できない
                            if (target.得意先ＣＤ == _tokuisaki.得意先ＣＤ)
                            {
                                dr.SetColumnError("得意先ＣＤ", "自分自身を分配先として指定することはできません。");
                                hasError = true;
                            }

                            // 子として追加する得意先ＣＤが、どの得意先の受注得意先ＣＤにも登録されていないこと（親となる得意先は除く）（既に誰かの親でないこと）
                            if (IsBunpaiParent((int)target.得意先ＣＤ))
                            {
                                dr.SetColumnError("得意先ＣＤ", "別の得意先の受注顧客です。分配先として指定することはできません。");
                                hasError = true;
                            }

                            // 分配先に対する分配先（つまり孫）が入力できないようにForm_Loadで制御している(△)
                        }
                    }
                }

                if (hasError)
                {
                    tabControl1.SelectedIndex = TAB_INDEX_BUNPAI;
                    MessageBox.Show(this, "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

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
        /// 分配先として指定した得意先CDが、他のどの得意先の受注得意先ＣＤにも登録されているかどうかを返す
        /// </summary>
        /// <param name="tokuisakiCD">分配先として指定した得意先CD</param>
        /// <returns>他の得意先の受注得意先ＣＤに登録されている場合はtrue</returns>
        private bool IsBunpaiParent(int tokuisakiCD)
        {
            bool isParent = false;

            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     COUNT(得意先ＣＤ) AS CNT" +
                                      "\r\n FROM" +
                                      "\r\n     得意先マスタ" +
                                      "\r\n WHERE" +
                                      "\r\n     企業ＣＤ= @企業ＣＤ" +
                                      "\r\n     AND 受注得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n     AND 得意先ＣＤ <> @得意先ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD;

                    log.Debug(cmd.CommandTextWithParameters());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        if ((int)reader["CNT"] != 0)
                        {
                            isParent = true;
                        }
                    }
                }

                return isParent;
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
        /// 得意先単価マスタが登録可能か？
        /// </summary>
        /// <returns>登録可能な場合はtrue</returns>
        private bool CanSaveTokuisakiTanka()
        {
            bool hasError = false;

            try
            {
                DataSet ds = (DataSet)tankaBindingSource.DataSource;
                DataTable dt = ds.Tables["得意先単価マスタ"];

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    dr.ClearErrors();

                    // 削除された行についてはエラーチェックは不要
                    if (dr.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }

                    TokuisakiTanka tokuisakiTanka = new TokuisakiTanka(dr);

                    // 得意先CDがない場合は新規扱い
                    if (tokuisakiTanka.得意先ＣＤ == null)
                    {
                        tokuisakiTanka.得意先ＣＤ = _tokuisaki.得意先ＣＤ;
                    }

                    // エラーがある？
                    if (tokuisakiTanka.HasErrors)
                    {
                        dr.SetColumnError("商品ＣＤ", tokuisakiTanka["商品ＣＤ"]);
                        dr.SetColumnError("商品単価税込", tokuisakiTanka["商品単価税込"]);
                        dr.SetColumnError("商品単価税抜", tokuisakiTanka["商品単価税抜"]);
                        hasError = true;
                    }
                    else
                    {
                        // 商品ＣＤ該当するマスタが存在するか？
                        if (tokuisakiTanka.商品ＣＤ != null)
                        {
                            Syohin syohin = Syohin.GetSyohin((int)tokuisakiTanka.商品ＣＤ);
                            if (syohin == null)
                            {
                                dr.SetColumnError("商品ＣＤ", "商品が見つかりません。");
                                hasError = true;
                            }
                        }
                    }
                }

                if (hasError)
                {
                    MessageBox.Show(this, "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

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
        /// 配送先が登録可能か？
        /// </summary>
        /// <returns>登録可能な場合はtrue</returns>
        private bool CanSaveHaiso()
        {
            try
            {
                // 注文元一覧を取得
                DataSet ds = (DataSet)chumonMotoBindingSource.DataSource;
                DataTable dt = ds.Tables["注文元"];

                // 配送先を指定している（つまり注文元）？
                if (_tokuisaki.配送得意先ＣＤ != null)
                {
                    // 自分が注文元の場合は、他の誰かの配送先であってはダメ（注文元が既に誰かの配送先でないこと）
                    // ※配送先に自分自身を指定しているかのチェックはTokuisaki#Errorにて実施
                    // ↑しないことになった(2022/06/30)
                    // ┌─┬───┬───────────┐
                    // │  │これは│これであってはならない│
                    // ├─┼───┼───────────┤
                    // │ 3│注文元│配送先                │
                    // └─┴───┴───────────┘
                    if (dt.Rows.Count != 0)
                    {
                        if (_tokuisaki.配送得意先ＣＤ != _tokuisaki.得意先ＣＤ)     // 配送得意先ＣＤに自分自身の得意先ＣＤを設定するのは許可する(★)
                        {
                            tabControl1.SelectedIndex = TAB_INDEX_FUZOKU;
                            txtHaisoCD.Focus();
                            MessageBox.Show(this, "この得意先は他の得意先によって配送先として指定されています。配送先の配送先を指定することはできません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return false;
                        }
                    }
                }

                // 自分が配送先（＝配送先を指定していない） かつ 自分が注文元（＝配送先を指定している） はあり得ない
                // ┌─┬───┬───────────┐
                // │  │これは│これであってはならない│
                // ├─┼───┼───────────┤
                // │ 4│配送先│注文元                │
                // └─┴───┴───────────┘

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
        /// 分配と配送の入力チェック
        /// </summary>
        /// <returns>登録可能な場合はtrue</returns>
        private bool CanSaveBunpaiHaiso()
        {
            bool hasError = false;

            try
            {
                DataSet dsBunpai = (DataSet)bunpaiBindingSource.DataSource;
                DataTable dtBunpai = dsBunpai.Tables["分配先"];

                DataSet dsHaiso = (DataSet)chumonMotoBindingSource.DataSource;
                DataTable dtHaiso = dsHaiso.Tables["注文元"];

                // ┌─┬───┬───────────┐
                // │  │これは│これであってはならない│
                // ├─┼───┼───────────┤
                // │ 5│分配元│注文元                │
                // └─┴───┴───────────┘
                // 分配先一覧がある場合は配送得意先ＣＤを設定してはダメ
                if (dtBunpai.Rows.Count != 0)
                {
                    if (_tokuisaki.配送得意先ＣＤ != _tokuisaki.得意先ＣＤ)     // 配送得意先ＣＤに自分自身の得意先ＣＤを設定するのは許可する(★)
                    {
                        MessageBox.Show(this, "分配先と配送先を同時に指定することはできません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }

                // ┌─┬───┬───────────┐
                // │ 6│分配元│配送先                │
                // └─┴───┴───────────┘
                // 分配先一覧がある場合は注文元一覧があってはダメ
                if (dtBunpai.Rows.Count != 0)
                {
                    if (dtHaiso.Rows.Count != 0)
                    {
                        tabControl1.SelectedIndex = TAB_INDEX_BUNPAI;
                        MessageBox.Show(this, "注文元がある場合は、分配先を指定できません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }

                // ┌─┬───┬───────────┐
                // │ 7│分配先│注文元                │
                // └─┴───┴───────────┘
                // 受注得意先ＣＤを設定している場合は配送得意先ＣＤを設定してはダメ
                if (_tokuisaki.受注得意先ＣＤ != null)
                {
                    if (_tokuisaki.配送得意先ＣＤ != _tokuisaki.得意先ＣＤ)     // 配送得意先ＣＤに自分自身の得意先ＣＤを設定するのは許可する(★)
                    {
                        tabControl1.SelectedIndex = TAB_INDEX_FUZOKU;
                        txtHaisoCD.Focus();
                        MessageBox.Show(this, "受注顧客がある場合は、配送先を指定できません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }

                // ┌─┬───┬───────────┐
                // │ 8│分配先│配送先                │
                // └─┴───┴───────────┘
                // 受注得意先ＣＤを設定している場合は注文元一覧があってはダメ
                // ※どちらも他の得意先によって設定されるものなので、この条件は成立しないはず
                if (_tokuisaki.受注得意先ＣＤ != null)
                {
                    if (dtHaiso.Rows.Count != 0)
                    {
                        MessageBox.Show(this, "受注顧客と注文元が同時に成立しています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }

                // 分配先として設定しようとしている得意先は、注文元でも配送先でもダメ
                foreach (DataRow dr in dtBunpai.Rows)
                {
                    dr.ClearErrors();

                    // 削除された行についてはエラーチェックは不要
                    if (dr.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }

                    Tokuisaki bunpaiChild = new Tokuisaki(dr);

                    // 得意先ＣＤに該当するマスタが存在するか？（無効であっても検索）
                    Tokuisaki target = Tokuisaki.GetTokuisakiReload((int)bunpaiChild.得意先ＣＤ);

                    // 得意先が存在していることはCanSaveBunpai()で確認済み

                    // 子として追加できる得意先の配送先がnullであること（既に誰かの注文元でないこと）
                    if (target.配送得意先ＣＤ != target.得意先ＣＤ)     // 配送得意先ＣＤに自分自身の得意先ＣＤを設定するのは許可する(★)
                    {
                        dr.SetColumnError("得意先ＣＤ", "別の得意先の注文元です。分配先として指定することはできません。");
                        hasError = true;
                    }

                    if (IsHaisoChild((int)target.得意先ＣＤ))
                    {
                        dr.SetColumnError("得意先ＣＤ", "別の得意先の配送先です。分配先として指定することはできません。");
                        hasError = true;
                    }
                }

                if (hasError)
                {
                    tabControl1.SelectedIndex = TAB_INDEX_BUNPAI;
                    MessageBox.Show(this, "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // ┌─┬───┬───────────┐
                // │ 9│注文元│分配元                │
                // └─┴───┴───────────┘
                // 配送得意先ＣＤを設定している場合は分配先一覧があってはダメ
                // つまり5と同じ

                // ┌─┬───┬───────────┐
                // │10│注文元│分配先                │
                // └─┴───┴───────────┘
                // 配送得意先ＣＤを設定している場合は受注得意先ＣＤを設定してはダメ
                // つまり7と同じ

                // ┌─┬───┬───────────┐
                // │11│配送先│分配元                │
                // └─┴───┴───────────┘
                // 注文元一覧がある場合は分配先一覧があってはダメ
                // つまり6と同じ

                // ┌─┬───┬───────────┐
                // │12│配送先│分配先                │
                // └─┴───┴───────────┘
                // 注文元一覧がある場合は受注得意先ＣＤを設定してはダメ
                // つまり8と同じ

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
        /// 分配先として指定した得意先CDが、他のどの得意先の配送得意先ＣＤにも登録されているかどうかを返す
        /// </summary>
        /// <param name="tokuisakiCD">分配先として指定した得意先CD</param>
        /// <returns>他の得意先の配送得意先ＣＤに登録されている場合はtrue</returns>
        private bool IsHaisoChild(int tokuisakiCD)
        {
            bool isChild = false;

            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     COUNT(得意先ＣＤ) AS CNT" +
                                      "\r\n FROM" +
                                      "\r\n     得意先マスタ" +
                                      "\r\n WHERE" +
                                      "\r\n     企業ＣＤ= @企業ＣＤ" +
                                      "\r\n     AND 配送得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n     AND 配送得意先ＣＤ <> 得意先ＣＤ";      // 配送得意先ＣＤに自分自身の得意先ＣＤを設定するのは許可する(★)

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD;

                    log.Debug(cmd.CommandTextWithParameters());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        if ((int)reader["CNT"] != 0)
                        {
                            isChild = true;
                        }
                    }
                }

                return isChild;
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
        /// DB保存
        /// </summary>
        /// <returns>ダイアログを閉じる場合はtrue</returns>
        private bool Save()
        {
            History.ProcessType historyProcess = History.ProcessType.Create;

            if (_IsNewRecord == false)
            {
                historyProcess = History.ProcessType.Update;
            }

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                // トランザクション開始
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    // 得意先マスタ保存
                    if (SaveTokuisaki(con, tran) == false)
                    {
                        return false;
                    }

                    // 電話テーブル保存
                    if (SaveTelToCust(con, tran) == false)
                    {
                        return false;
                    }

                    // 得意先振込依頼人名テーブル保存
                    if (SaveTokuisakiFurikomiIraiNin(con, tran) == false)
                    {
                        return false;
                    }

                    // 分配先保存
                    if (SaveBunpai(con, tran, (_tokuisaki.無効フラグ ?? false)) == false)
                    {
                        return false;
                    }

                    // 得意先単価マスタ保存
                    if (SaveTokuisakiTanka(con, tran) == false)
                    {
                        return false;
                    }

                    // 得意先履歴テーブル登録
                    if (SaveTokuisakiRireki(con, tran) == false)
                    {
                        return false;
                    }

                    // コミット
                    tran.Commit();
                    log.InfoFormat("得意先マスタと得意先振込依頼人名テーブルと得意先単価マスタを{0}しました。", historyProcess.DisplayName());
                    MessageBox.Show(this, string.Format("{0}しました。", historyProcess.DisplayName()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        }

        /// <summary>
        /// 得意先マスタ保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveTokuisaki(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                bool ret = false;
                History.ProcessType historyProcess = History.ProcessType.Create;

                if (_IsNewRecord)
                {
                    ret = _tokuisaki.Insert(con, tran);
                }
                else
                {
                    ret = _tokuisaki.Update(con, tran);
                    historyProcess = History.ProcessType.Update;
                }

                if (ret == false)
                {
                    MessageBox.Show(this, _tokuisaki.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(con, tran, this.Text, "得意先マスタ", _tokuisaki.Code, historyProcess);
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
        /// 電話テーブル保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveTelToCust(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                DataSet ds = (DataSet)telBindingSource.DataSource;
                DataTable dt = ds.Tables["C_TelToCust"];

                History.ProcessType historyProcess = History.ProcessType.Create;
                bool ret = false;

                // １件も無い場合は、電話番号１と２を追加する
                // ※dt.Rows.Count == 0 だと、スプレッドから削除したレコードも数えてしまう
                if (telBindingSource.Count == 0)
                {
                    string[] telno = new string[] { _tokuisaki.電話番号1 == null ? "" : _tokuisaki.電話番号1.Replace("-", ""),
                                                    _tokuisaki.電話番号2 == null ? "" : _tokuisaki.電話番号2.Replace("-", "") };
                    bool[] repFlg = new bool[] { false, true };
                    for (int i = 0; i < telno.Length; i++)
                    {
                        if (string.IsNullOrEmpty(telno[i]))
                        {
                            continue;
                        }
                        // 数字以外の文字がある場合も無視する
                        if (Regex.IsMatch(telno[i], @"^[0-9]*$") == false)
                        {
                            continue;
                        }
                        TelToCust telToCust = new TelToCust();
                        telToCust.Tel_TelNo = telno[i];
                        telToCust.Tel_CustNo = _tokuisaki.得意先ＣＤ;
                        telToCust.Tel_RepFlg = repFlg[i];
                        telToCust.Tel_DelFlg = false;
                        DataRow dr = dt.NewRow();
                        telToCust.SetDataRow(dr);
                        dt.Rows.Add(dr);
                    }
                }

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    TelToCust telToCust = new TelToCust(dr);

                    switch (dr.RowState)
                    {
                        case DataRowState.Added:        // 追加
                            telToCust.Tel_CustNo = _tokuisaki.得意先ＣＤ;
                            ret = telToCust.Insert(con, tran);
                            historyProcess = History.ProcessType.Create;
                            break;

                        case DataRowState.Modified:     // 変更
                            ret = telToCust.Update(con, tran);
                            historyProcess = History.ProcessType.Update;
                            break;

                        case DataRowState.Deleted:      // 削除
                            ret = telToCust.Delete(con, tran);
                            historyProcess = History.ProcessType.Delete;
                            break;

                        default:
                            continue;
                    }

                    if (ret == false)
                    {
                        MessageBox.Show(this, telToCust.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    History.Save(con, tran, this.Text, "電話テーブル", telToCust.Code, historyProcess);
                }
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
        /// 得意先振込依頼人名テーブル保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveTokuisakiFurikomiIraiNin(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                DataSet ds = (DataSet)furikomiBindingSource.DataSource;
                DataTable dt = ds.Tables["得意先振込依頼人名テーブル"];

                bool ret = false;
                History.ProcessType historyProcess = History.ProcessType.Create;

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    TokuisakiFurikomiIraiNin tokuisakiFurikomiIraiNin = new TokuisakiFurikomiIraiNin(dr);

                    switch (dr.RowState)
                    {
                        case DataRowState.Added:        // 追加
                            tokuisakiFurikomiIraiNin.得意先ＣＤ = _tokuisaki.得意先ＣＤ;
                            ret = tokuisakiFurikomiIraiNin.Insert(con, tran);
                            historyProcess = History.ProcessType.Create;
                            break;

                        case DataRowState.Modified:     // 変更
                            ret = tokuisakiFurikomiIraiNin.Update(con, tran);
                            historyProcess = History.ProcessType.Update;
                            break;

                        case DataRowState.Deleted:      // 削除
                            ret = tokuisakiFurikomiIraiNin.Delete(con, tran);
                            historyProcess = History.ProcessType.Delete;
                            break;

                        default:
                            continue;
                    }

                    if (ret == false)
                    {
                        MessageBox.Show(this, tokuisakiFurikomiIraiNin.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    History.Save(con, tran, this.Text, "得意先振込依頼人名テーブル", tokuisakiFurikomiIraiNin.Code, historyProcess);
                }
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
        /// 分配先保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="isInvalid">得意先が無効かどうか</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveBunpai(SqlConnection con, SqlTransaction tran, bool isInvalid)
        {
            try
            {
                DataSet ds = (DataSet)bunpaiBindingSource.DataSource;
                DataTable dt = ds.Tables["分配先"];

                History.ProcessType historyProcess = History.ProcessType.Create;
                string sql = "";
                bool ret = false;

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    Tokuisaki bunpaiChild = new Tokuisaki(dr);

                    switch (dr.RowState)
                    {
                        case DataRowState.Added:        // 追加

                            // 子(=分配先)を追加しても、親(=得意先)が無効になるなら、子の追加(受注得意先CDの登録)はしない
                            if (isInvalid)
                            {
                                continue;
                            }

                            sql = "\r\n UPDATE 得意先マスタ" +
                                  "\r\n SET 受注得意先ＣＤ = @受注得意先ＣＤ" +
                                  "\r\n WHERE 企業ＣＤ     = @企業ＣＤ" +
                                  "\r\n   AND 得意先ＣＤ   = @得意先ＣＤ" +
                                  "\r\n   AND 受注得意先ＣＤ IS NULL";

                            // 受注得意先ＣＤ設定時に受注得意先名を取得しないようにする
                            // （異なるトランザクションで得意先マスタを参照することによるロックを回避するため）
                            bunpaiChild.stopOverwrite = true;

                            bunpaiChild.受注得意先ＣＤ = _tokuisaki.得意先ＣＤ;
                            historyProcess = History.ProcessType.Update;
                            ret = SaveBunpaiMain(con, tran, sql, bunpaiChild, historyProcess);
                            if (ret == false)
                            {
                                return false;
                            }
                            break;

                        case DataRowState.Modified:     // 変更
                            // 変更前をクリアし、変更後を更新
                            sql = "\r\n UPDATE 得意先マスタ" +
                                  "\r\n SET 受注得意先ＣＤ = @受注得意先ＣＤ" +
                                  "\r\n WHERE 企業ＣＤ     = @企業ＣＤ" +
                                  "\r\n   AND 得意先ＣＤ   = @得意先ＣＤorg";

                            // 受注得意先ＣＤ設定時に受注得意先名を取得しないようにする
                            // （異なるトランザクションで得意先マスタを参照することによるロックを回避するため）
                            bunpaiChild.stopOverwrite = true;

                            bunpaiChild.受注得意先ＣＤ = null;
                            historyProcess = History.ProcessType.Update;
                            ret = SaveBunpaiMain(con, tran, sql, bunpaiChild, historyProcess);
                            if (ret == false)
                            {
                                return false;
                            }

                            // 子(=分配先)を変更しても、親(=得意先)が無効になるなら、変更後の子の登録(受注得意先CDの登録)はしない
                            if (isInvalid)
                            {
                                continue;
                            }

                            sql = "\r\n UPDATE 得意先マスタ" +
                                  "\r\n SET 受注得意先ＣＤ = @受注得意先ＣＤ" +
                                  "\r\n WHERE 企業ＣＤ     = @企業ＣＤ" +
                                  "\r\n   AND 得意先ＣＤ   = @得意先ＣＤ" +
                                  "\r\n   AND 受注得意先ＣＤ IS NULL";

                            // 受注得意先ＣＤ設定時に受注得意先名を取得しないようにする
                            // （異なるトランザクションで得意先マスタを参照することによるロックを回避するため）
                            bunpaiChild.stopOverwrite = true;

                            bunpaiChild.受注得意先ＣＤ = _tokuisaki.得意先ＣＤ;
                            historyProcess = History.ProcessType.Update;
                            ret = SaveBunpaiMain(con, tran, sql, bunpaiChild, historyProcess);
                            if (ret == false)
                            {
                                return false;
                            }
                            break;

                        case DataRowState.Deleted:      // 削除
                            sql = "\r\n UPDATE 得意先マスタ" +
                                  "\r\n SET 受注得意先ＣＤ = @受注得意先ＣＤ" +
                                  "\r\n WHERE 企業ＣＤ     = @企業ＣＤ" +
                                  "\r\n   AND 得意先ＣＤ   = @得意先ＣＤorg";

                            // 受注得意先ＣＤ設定時に受注得意先名を取得しないようにする
                            // （異なるトランザクションで得意先マスタを参照することによるロックを回避するため）
                            bunpaiChild.stopOverwrite = true;

                            bunpaiChild.受注得意先ＣＤ = null;
                            historyProcess = History.ProcessType.Update;
                            ret = SaveBunpaiMain(con, tran, sql, bunpaiChild, historyProcess);
                            if (ret == false)
                            {
                                return false;
                            }
                            break;

                        case DataRowState.Unchanged:    // 変更なし
                            // 親(=得意先)が無効になったら、子(=分配先)を消す（受注得意先CDを消す）
                            if (isInvalid)
                            {
                                sql = "\r\n UPDATE 得意先マスタ" +
                                      "\r\n SET 受注得意先ＣＤ = @受注得意先ＣＤ" +
                                      "\r\n WHERE 企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n   AND 得意先ＣＤ   = @得意先ＣＤorg";

                                // 受注得意先ＣＤ設定時に受注得意先名を取得しないようにする
                                // （異なるトランザクションで得意先マスタを参照することによるロックを回避するため）
                                bunpaiChild.stopOverwrite = true;

                                bunpaiChild.受注得意先ＣＤ = null;
                                historyProcess = History.ProcessType.Update;
                                ret = SaveBunpaiMain(con, tran, sql, bunpaiChild, historyProcess);
                                if (ret == false)
                                {
                                    return false;
                                }
                            }
                            break;

                        default:
                            continue;
                    }
                }
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
        /// 分配先保存主処理
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="sql">実行SQL</param>
        /// <param name="bunpaiChild">分配先得意先マスタ</param>
        /// <param name="historyProcess">処理タイプ</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveBunpaiMain(SqlConnection con, SqlTransaction tran, string sql, Tokuisaki bunpaiChild, History.ProcessType historyProcess)
        {
            try
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = sql;

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = bunpaiChild.得意先ＣＤ;

                // 更新または削除の場合のみ
                if (bunpaiChild.得意先ＣＤorg != null)
                {
                    cmd.Parameters.Add("@得意先ＣＤorg", SqlDbType.Int).Value = bunpaiChild.得意先ＣＤorg;
                }

                if (bunpaiChild.受注得意先ＣＤ != null)
                {
                    cmd.Parameters.Add("@受注得意先ＣＤ", SqlDbType.Int).Value = bunpaiChild.受注得意先ＣＤ;
                }
                else
                {
                    cmd.Parameters.Add("@受注得意先ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                }

                log.Debug(cmd.CommandTextWithParameters());

                if (cmd.ExecuteNonQuery() != 1)
                {
                    tran.Rollback();
                    MessageBox.Show(this, "別の端末で更新済みです。再度検索してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                log.InfoFormat("得意先マスタ（分配先）を{0}しました。", historyProcess.DisplayName());

                History.Save(con, tran, this.Text, "得意先マスタ（分配先）", bunpaiChild.Code, historyProcess);

                return true;
            }
            catch (SqlException ex)
            {
                log.Error(ex);
                tran.Rollback();
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show(this, "得意先マスタが重複しています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    History.Error(ex);
                    MessageBox.Show(this, string.Format("{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(this, string.Format("{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 得意先単価マスタ保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveTokuisakiTanka(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                DataSet ds = (DataSet)tankaBindingSource.DataSource;
                DataTable dt = ds.Tables["得意先単価マスタ"];

                History.ProcessType historyProcess = History.ProcessType.Create;
                bool ret = false;

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    TokuisakiTanka tokuisakiTanka = new TokuisakiTanka(dr);

                    switch (dr.RowState)
                    {
                        case DataRowState.Added:        // 追加
                            tokuisakiTanka.得意先ＣＤ = _tokuisaki.得意先ＣＤ;
                            ret = tokuisakiTanka.Insert(con, tran);
                            historyProcess = History.ProcessType.Create;
                            break;

                        case DataRowState.Modified:     // 変更
                            ret = tokuisakiTanka.Update(con, tran);
                            historyProcess = History.ProcessType.Update;
                            break;

                        case DataRowState.Deleted:      // 削除
                            ret = tokuisakiTanka.Delete(con, tran);
                            historyProcess = History.ProcessType.Delete;
                            break;

                        default:
                            continue;
                    }

                    if (ret == false)
                    {
                        MessageBox.Show(this, tokuisakiTanka.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    History.Save(con, tran, this.Text, "得意先単価マスタ", tokuisakiTanka.Code, historyProcess);
                }
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
        /// 得意先履歴テーブル保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveTokuisakiRireki(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                History.ProcessType historyProcess = History.ProcessType.Create;

                // 得意先履歴テーブルが持つ項目が変更されていれば登録する
                if (TokuisakiRireki.IsSaveTarget(_tokuisaki, _origin) == false)
                {
                    return true;
                }

                TokuisakiRireki tokuisakiRireki = new TokuisakiRireki(_tokuisaki);
                bool ret = tokuisakiRireki.Insert(con, tran);

                if (ret == false)
                {
                    MessageBox.Show(this, tokuisakiRireki.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(con, tran, this.Text, "得意先履歴テーブル", tokuisakiRireki.Code, historyProcess);
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
        /// （保存後の）再読み込み
        /// </summary>
        private void Reload()
        {
            try
            {
                _tokuisaki = Tokuisaki.GetTokuisakiReload((int)_tokuisaki.得意先ＣＤ);
                _origin = new Tokuisaki(_tokuisaki);

                _IsNewRecord = false;
                _IsCopy = false;

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                tokuisakiBindingSource.DataSource = _tokuisaki;

                lblMode.Text = MODE_CAPTION_EDIT;
                lblMode.BackColor = BACK_COLOR_EDIT;
                btnSave.Text = "更新(F9)";
                // 既存の場合は得意先ＣＤ変更不可
                txtTokuisakiCD.Enabled = false;
                btnAkiBango.Enabled = false;

                btnDelete.Enabled = this.HasAuthEdit;
                btnCopy.Enabled = this.HasAuthEdit;
                btnRireki.Enabled = true;
                btnCourse.Enabled = true;

                // CTI連携・電話番号を取得
                GetTelToCust(_tokuisaki.得意先ＣＤ);

                // 振込依頼人名を取得
                GetTokuisakiFurikomiIraiNin(_tokuisaki.得意先ＣＤ);

                // 分配先を取得
                GetBunpai(_tokuisaki.得意先ＣＤ);

                // 得意先単価を取得
                GetTokuisakiTanka(_tokuisaki.得意先ＣＤ);
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
        /// 削除可能か？
        /// </summary>
        /// <returns></returns>
        private bool CanDelete()
        {
            try
            {
                if (this.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "編集できる権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

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
        /// DB削除
        /// </summary>
        /// <returns>ダイアログを閉じる場合はtrue</returns>
        private bool Delete()
        {
            History.ProcessType historyProcess = History.ProcessType.Delete;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    // 得意先マスタ削除
                    if (DeleteTokuisaki(con, tran) == false)
                    {
                        return false;
                    }

                    // 電話テーブル削除
                    if (DeleteTelToCust(con, tran) == false)
                    {
                        return false;
                    }

                    // 得意先振込依頼人名テーブル削除
                    if (DeleteTokuisakiFurikomiIraiNin(con, tran) == false)
                    {
                        return false;
                    }

                    // 分配先削除
                    if (DeleteBunpai(con, tran) == false)
                    {
                        return false;
                    }

                    // 得意先単価マスタ削除
                    if (DeleteTokuisakiTanka(con, tran) == false)
                    {
                        return false;
                    }

                    // 得意先単価適用マスタ削除
                    if (DeleteTokuisaki(con, tran, "得意先単価適用マスタ") == false)
                    {
                        return false;
                    }

                    // コーステーブル削除
                    if (DeleteTokuisaki(con, tran, "コーステーブル") == false)
                    {
                        return false;
                    }

                    tran.Commit();
                    log.InfoFormat("得意先マスタと得意先振込依頼人名テーブルと得意先単価マスタを{0}しました。", historyProcess.DisplayName());
                    MessageBox.Show(this, string.Format("{0}しました。", historyProcess.DisplayName()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        }

        /// <summary>
        /// 得意先マスタ削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool DeleteTokuisaki(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                History.ProcessType historyProcess = History.ProcessType.Delete;

                bool ret = _tokuisaki.Delete(con, tran);

                if (ret == false)
                {
                    MessageBox.Show(this, _tokuisaki.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(con, tran, this.Text, "得意先マスタ", _tokuisaki.Code, historyProcess);
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
        /// 電話テーブル削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool DeleteTelToCust(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                // 全削除
                DataSet ds = (DataSet)telBindingSource.DataSource;
                DataTable dt = ds.Tables["C_TelToCust"];

                History.ProcessType historyProcess = History.ProcessType.Delete;

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr.RowState == DataRowState.Added)  // 画面上にて追加されたばかりのレコードは削除しない
                    {
                        continue;
                    }

                    TelToCust telToCust = new TelToCust(dr);

                    bool ret = telToCust.Delete(con, tran);
                    if (ret == false)
                    {
                        MessageBox.Show(this, telToCust.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    History.Save(con, tran, this.Text, "電話テーブル", telToCust.Code, historyProcess);
                }
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
        /// 得意先振込依頼人名テーブル削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool DeleteTokuisakiFurikomiIraiNin(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                // 全削除
                DataSet ds = (DataSet)furikomiBindingSource.DataSource;
                DataTable dt = ds.Tables["得意先振込依頼人名テーブル"];

                History.ProcessType historyProcess = History.ProcessType.Delete;

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr.RowState == DataRowState.Added)  // 画面上にて追加されたばかりのレコードは削除しない
                    {
                        continue;
                    }

                    TokuisakiFurikomiIraiNin tokuisakiFurikomiIraiNin = new TokuisakiFurikomiIraiNin(dr);

                    bool ret = tokuisakiFurikomiIraiNin.Delete(con, tran);
                    if (ret == false)
                    {
                        MessageBox.Show(this, tokuisakiFurikomiIraiNin.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    History.Save(con, tran, this.Text, "得意先振込依頼人名テーブル", tokuisakiFurikomiIraiNin.Code, historyProcess);
                }
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
        /// 分配先削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool DeleteBunpai(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                DataSet ds = (DataSet)bunpaiBindingSource.DataSource;
                DataTable dt = ds.Tables["分配先"];

                History.ProcessType historyProcess = History.ProcessType.Update;

                string  sql = "\r\n UPDATE 得意先マスタ" +
                              "\r\n SET 受注得意先ＣＤ = NULL" +
                              "\r\n WHERE 企業ＣＤ     = @企業ＣＤ" +
                              "\r\n   AND 得意先ＣＤ   = @得意先ＣＤorg";

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr.RowState == DataRowState.Added)  // 画面上にて追加されたばかりのレコードは更新しない
                    {
                        continue;
                    }

                    Tokuisaki bunpaiChild = new Tokuisaki(dr);

                    try
                    {
                        SqlCommand cmd = con.CreateCommand();
                        cmd.Transaction = tran;
                        cmd.CommandText = sql;

                        cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                        cmd.Parameters.Add("@得意先ＣＤorg", SqlDbType.Int).Value = bunpaiChild.得意先ＣＤorg;

                        log.Debug(cmd.CommandTextWithParameters());

                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            tran.Rollback();
                            MessageBox.Show(this, "別の端末で更新済みです。再度検索してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }

                        log.InfoFormat("得意先マスタ（分配先）を{0}しました。", historyProcess.DisplayName());

                        History.Save(con, tran, this.Text, "得意先マスタ（分配先）", bunpaiChild.Code, historyProcess);
                    }
                    catch (SqlException ex)
                    {
                        log.Error(ex);
                        tran.Rollback();
                        if (ex.Number == 2627 || ex.Number == 2601)
                        {
                            MessageBox.Show(this, "得意先マスタが重複しています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            History.Error(ex);
                            MessageBox.Show(this, string.Format("{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        tran.Rollback();
                        History.Error(ex);
                        MessageBox.Show(this, string.Format("{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
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
        /// 得意先単価マスタ削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool DeleteTokuisakiTanka(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                // 全削除
                DataSet ds = (DataSet)tankaBindingSource.DataSource;
                DataTable dt = ds.Tables["得意先単価マスタ"];

                History.ProcessType historyProcess = History.ProcessType.Create;

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr.RowState == DataRowState.Added)  // 画面上にて追加されたばかりのレコードは更新しない
                    {
                        continue;
                    }

                    TokuisakiTanka tokuisakiTanka = new TokuisakiTanka(dr);

                    bool ret = tokuisakiTanka.Delete(con, tran);
                    if (ret == false)
                    {
                        MessageBox.Show(this, tokuisakiTanka.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    History.Save(con, tran, this.Text, "得意先単価マスタ", tokuisakiTanka.Code, historyProcess);
                }
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
        /// 指定したテーブルから商品を削除します
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="tableName">テーブル名</param>
        /// <returns>成功した場合は True</returns>
        private bool DeleteTokuisaki(SqlConnection con, SqlTransaction tran, string tableName)
        {
            History.ProcessType historyProcess = History.ProcessType.Delete;

            try
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                string sql = "\r\n DELETE {0}" +
                             "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                             "\r\n   AND 得意先ＣＤ = @得意先ＣＤ";
                cmd.CommandText = string.Format(sql, tableName);

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = (int)_tokuisaki.得意先ＣＤorg;

                log.Debug(cmd.CommandTextWithParameters());

                cmd.ExecuteNonQuery();

                log.InfoFormat("{0}を{1}しました。", tableName, historyProcess.DisplayName());
                History.Save(con, tran, this.Text, tableName, _tokuisaki.Code, History.ProcessType.Delete);
                return true;
            }
            catch (SqlException ex)
            {
                log.Error(ex);
                tran.Rollback();
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show(string.Format("{0}が重複しています。", tableName), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    History.Error(ex);
                    MessageBox.Show(string.Format("{0}の{1}に失敗しました。\n{2}", tableName, historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("{0}の{1}に失敗しました。\n{2}", tableName, historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        #endregion
    }
}
