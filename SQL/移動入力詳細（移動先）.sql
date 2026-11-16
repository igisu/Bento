-- 移動入力詳細（移動先）
-- 移動元（のコースが持つ部署）の主要部品と、移動先（のコースが持つ部署）の主要部品で、双方ともに持つ部品に限り、移動ができる。

SELECT
    MI.企業ＣＤ
    ,MI.コースＣＤ
    ,MI.相手コースＣＤ
    ,MI.商品ＣＤ
    ,S.商品名略称
    ,MI.個数
    ,MI.移動日
    ,MI.ＳＥＱ
    ,MI.確認フラグ
    ,MI.確認日時
    ,MI.相手確認フラグ
    ,MI.相手確認日時
    ,CASE WHEN ISNULL(MI.相手確認日時, '2000/01/01') >= ISNULL(MI.確認日時, '2000/01/01') THEN MI.相手確認日時 ELSE MI.確認日時 END AS 表示用確認日時
    ,MI.作成担当者ＣＤ
    ,A1.担当者名 AS 作成担当者名
    ,MI.作成日
    ,MI.修正担当者ＣＤ
    ,A2.担当者名 AS 修正担当者名
    ,MI.修正日
FROM
    モバイル_移動入力 AS MI
    LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = MI.企業ＣＤ AND S.商品ＣＤ = MI.商品ＣＤ
    LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = MI.企業ＣＤ AND MI.作成担当者ＣＤ = A1.担当者ＣＤ
    LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = MI.企業ＣＤ AND MI.修正担当者ＣＤ = A2.担当者ＣＤ
WHERE
    MI.企業ＣＤ = @企業ＣＤ
    AND MI.コースＣＤ = @移動元コースＣＤ
    AND MI.相手コースＣＤ = @移動先コースＣＤ
    AND MI.移動日 = @移動日
ORDER BY
    MI.ＳＥＱ
