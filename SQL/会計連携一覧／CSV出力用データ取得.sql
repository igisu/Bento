-- 会計連携CSV出力用データ取得
-- 一覧も同じ
SELECT
    K.企業ＣＤ
    ,K.集計日
    ,K.部署ＣＤ
    ,K.伝票番号
    ,K.伝票日付
    ,K.仕訳ＣＤ
    ,K.集計ＣＤ
    ,K.借方ＣＤ
    ,K.借方部門ＣＤ
    ,K.借方補助ＣＤ
    ,K.貸方ＣＤ
    ,K.貸方部門ＣＤ
    ,K.貸方補助ＣＤ
    ,K.金額
    -- 金額がマイナスの場合はマイナスフラグがONになっている
    -- CSV出力時、マイナスフラグがONなら借方と貸方を逆にして、金額をプラスにする
    -- ,CASE WHEN ISNULL(K.マイナスフラグ, 0) = 0 THEN K.借方ＣＤ     ELSE K.貸方ＣＤ     END AS 借方ＣＤ
    -- ,CASE WHEN ISNULL(K.マイナスフラグ, 0) = 0 THEN K.借方部門ＣＤ ELSE K.貸方部門ＣＤ END AS 借方部門ＣＤ
    -- ,CASE WHEN ISNULL(K.マイナスフラグ, 0) = 0 THEN K.借方補助ＣＤ ELSE K.貸方補助ＣＤ END AS 借方補助ＣＤ
    -- ,CASE WHEN ISNULL(K.マイナスフラグ, 0) = 0 THEN K.貸方ＣＤ     ELSE K.借方ＣＤ     END AS 貸方ＣＤ
    -- ,CASE WHEN ISNULL(K.マイナスフラグ, 0) = 0 THEN K.貸方部門ＣＤ ELSE K.借方部門ＣＤ END AS 貸方部門ＣＤ
    -- ,CASE WHEN ISNULL(K.マイナスフラグ, 0) = 0 THEN K.貸方補助ＣＤ ELSE K.借方補助ＣＤ END AS 貸方補助ＣＤ
    -- ,CASE WHEN ISNULL(K.マイナスフラグ, 0) = 0 THEN K.金額         ELSE (K.金額 * -1)  END AS 金額
    ,K.差分フラグ
    ,K.マイナスフラグ
    ,K.税率
    ,SW.処理内容
    ,SW.摘要
    ,B.部署名
FROM
    会計連携集計テーブル AS K
    INNER JOIN 仕訳マスタ AS SW
        ON K.企業ＣＤ = SW.企業ＣＤ
        AND K.部署ＣＤ = SW.部署ＣＤ
        AND K.仕訳ＣＤ = SW.仕訳ＣＤ
    INNER JOIN 部署マスタ AS B
        ON K.企業ＣＤ = B.企業ＣＤ
        AND K.部署ＣＤ = B.部署ＣＤ
WHERE
    K.企業ＣＤ = @企業ＣＤ
    AND K.集計日 = @出力対象日
    --AND K.伝票日付 = @出力対象日          -- 出力対象は集計日か伝票日付のどちらか
    --AND K.伝票番号 > @前回最大伝票番号    -- 差分をCSVに出力する場合は、今回追加したレコードのみを対象にする
ORDER BY
    K.部署ＣＤ
    ,K.差分フラグ
    ,K.伝票番号
