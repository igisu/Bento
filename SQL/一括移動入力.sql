-- 一括移動入力
-- 移動元（のコースが持つ部署）の主要部品と、移動先（のコースが持つ部署）の主要部品で、双方ともに持つ部品に限り、移動ができる。

SELECT
    MI.企業ＣＤ
    ,MI.コースＣＤ
    ,MI.相手コースＣＤ
    ,MI.相手コースＣＤ AS 相手コースＣＤorg
    ,CM.コース名 AS 相手コース名
    ,MI.商品ＣＤ
    ,MI.商品ＣＤ as 商品ＣＤorg
    ,S.商品名略称
    ,MI.移動日
    ,MI.ＳＥＱ
    ,MI.ＳＥＱ as ＳＥＱorg
    ,MI.個数
    ,MI.確認フラグ
    ,MI.確認日時
    ,MI.相手確認フラグ
    ,MI.相手確認日時
    ,MI.作成担当者ＣＤ
    ,A1.担当者名 AS 作成担当者名
    ,MI.作成日
    ,MI.修正担当者ＣＤ
    ,A2.担当者名 AS 修正担当者名
    ,MI.修正日
FROM
    モバイル_移動入力 AS MI
    LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = MI.企業ＣＤ AND S.商品ＣＤ = MI.商品ＣＤ
    LEFT JOIN コースマスタ AS CM ON MI.企業ＣＤ = CM.企業ＣＤ AND MI.相手コースＣＤ = CM.コースＣＤ
    LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = MI.企業ＣＤ AND MI.作成担当者ＣＤ = A1.担当者ＣＤ
    LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = MI.企業ＣＤ AND MI.修正担当者ＣＤ = A2.担当者ＣＤ
WHERE
    MI.企業ＣＤ = @企業ＣＤ
    AND MI.コースＣＤ = @移動元コースＣＤ
    AND MI.移動日 = @移動日
ORDER BY
    MI.ＳＥＱ
