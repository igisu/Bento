using System.Drawing;

namespace Emu.Bento.Common
{
    /// <summary>
    /// 選択時のハイライト表示をしない列ヘッダーレンダラー
    /// </summary>
    class NoHilightEnhancedColumnHeaderRenderer : FarPoint.Win.Spread.CellType.EnhancedColumnHeaderRenderer
    {
        /// <summary>
        /// 指定した外観設定を使用して、編集モードでない列ヘッダ セルを指定したグラフィック インタフェースに描画します。
        /// </summary>
        /// <param name="g">列ヘッダ セルを描画するためのグラフィック デバイス インタフェース</param>
        /// <param name="r">列ヘッダセルを描画するための矩形領域の位置とサイズ</param>
        /// <param name="appearance">列ヘッダセルの外観設定</param>
        /// <param name="value">列ヘッダセルのレンダラーコントロールの名前を含むオブジェクト。</param>
        /// <param name="isSelected">列ヘッダセルが選択されているかどうか</param>
        /// <param name="isLocked">列ヘッダセルがロックされているかどうか</param>
        /// <param name="zoomFactor">列ヘッダセルの表示倍率を表す数値</param>
        public override void PaintCell(Graphics g, Rectangle r, FarPoint.Win.Spread.Appearance appearance, object value, bool isSelected, bool isLocked, float zoomFactor)
        {
            // セルを選択した時の行列ヘッダのハイライト表示を抑制したい
            // https://dev.grapecity.co.jp/support/kb/detail.asp?id=29700

            // 選択の解除
            appearance.ContainsSelection = false;
            // 既定の動作
            base.PaintCell(g, r, appearance, value, isSelected, isLocked, zoomFactor);
        }
    }
}
