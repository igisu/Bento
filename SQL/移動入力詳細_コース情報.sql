-- 移動入力詳細_コース情報
SELECT
    CM.コースＣＤ
    ,CM.コース名
    ,CM.コース区分
    ,VK.コース区分名
    ,BM.支店ＣＤ
    ,SM.支店名
    ,CM.部署ＣＤ
    ,BM.部署名
FROM
    コースマスタ AS CM
    LEFT JOIN V_コース区分 AS VK ON VK.コース区分CD = CM.コース区分
    LEFT JOIN 部署マスタ AS BM ON BM.企業ＣＤ = CM.企業ＣＤ AND BM.部署ＣＤ = CM.部署ＣＤ
    LEFT JOIN 支店マスタ AS SM ON SM.企業ＣＤ = BM.企業ＣＤ AND SM.支店ＣＤ = BM.支店ＣＤ
WHERE
    CM.企業ＣＤ = 1
    AND CM.コースＣＤ = 2


-- グループ取得
SELECT
    CG.グループ種別名
FROM
    コースマスタ AS CM
    LEFT JOIN コース別グループマスタ AS CG
        ON CG.企業ＣＤ    = CM.企業ＣＤ
        AND CG.部署ＣＤ   = CM.部署ＣＤ
        AND CG.コースＣＤ = CM.コースＣＤ
WHERE
    CM.企業ＣＤ = 1
    AND CM.コースＣＤ = 9998
ORDER BY
    CG.グループ種別ＣＤ
