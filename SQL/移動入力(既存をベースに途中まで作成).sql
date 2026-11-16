-- 移動入力

SELECT
    COU.工場フラグ
    ,COU.コースＣＤ
    ,COU.コース名
    ,SYOHIN.商品ＣＤ
    ,SYOHIN.商品名
    ,ISNULL(MOB_MOTI.個数,0) AS 持ち出し数
FROM
    コースマスタ AS COU
    LEFT OUTER JOIN コース別グループマスタ AS COUG
        ON COU.企業ＣＤ = COUG.企業ＣＤ
        AND COU.部署ＣＤ = COUG.部署ＣＤ
        AND COU.コースＣＤ = COUG.コースＣＤ
    LEFT OUTER JOIN 主要部品リストマスタ AS BUHIN
        ON BUHIN.企業ＣＤ = COU.企業ＣＤ
        AND BUHIN.部署ＣＤ = COU.部署ＣＤ
    LEFT JOIN 商品マスタ AS SYOHIN
        ON SYOHIN.企業ＣＤ = BUHIN.企業ＣＤ
        AND SYOHIN.商品ＣＤ = BUHIN.商品ＣＤ
    LEFT JOIN (
            SELECT
                企業ＣＤ
                ,コースＣＤ
                ,商品ＣＤ
                ,SUM(個数) AS 個数
            FROM
                モバイル_持ち出し入力
            WHERE
                企業ＣＤ = 1
            AND 持出日 = '2016/03/24'
            GROUP BY
                企業ＣＤ
                ,コースＣＤ
                ,商品ＣＤ
        ) AS MOB_MOTI
        ON  MOB_MOTI.企業ＣＤ = COU.企業ＣＤ
        AND MOB_MOTI.コースＣＤ = COU.コースＣＤ
        AND MOB_MOTI.商品ＣＤ = SYOHIN.商品ＣＤ

WHERE
    COU.企業ＣＤ = 1
    AND COU.部署ＣＤ = 1
    AND COU.コース区分 = 1
--    AND COUG.グループ種別ＣＤ = 1
    AND ISNULL(COU.無効フラグ, 0) = 0
    AND ISNULL(SYOHIN.無効フラグ, 0) = 0
    AND (BUHIN.販売期間開始 IS NULL OR BUHIN.販売期間開始 <= '2016/03/24')
    AND (BUHIN.販売期間終了 IS NULL OR BUHIN.販売期間終了 >= '2016/03/24')
