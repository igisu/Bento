-- 日配持出入力基準（すべて）
SELECT
    MAIN.コースＣＤ
    ,MAIN.コース名
    ,MAIN.商品ＣＤ
    ,MCDS.個数
    ,0 AS 合計行
FROM
    -- ①駆動表（コースと商品の全組み合わせ）
    (SELECT
        C.コースＣＤ,
        C.コース名,
        P.商品ＣＤ
    FROM
        -- コースマスタから取得（実施日に該当する臨時コースマスタがあればコースマスタからは取得しない。無ければ取得する）
        (SELECT
            CM.企業ＣＤ
            ,CM.部署ＣＤ
            ,CM.コースＣＤ
            ,CM.コース名
        FROM
            コースマスタ AS CM
            LEFT JOIN
                (SELECT
                    コースＣＤ
                FROM
                    臨時コースマスタ
                WHERE
                    企業ＣＤ = @企業ＣＤ
                    AND コース区分 = @コース区分
                    AND 部署ＣＤ = @部署ＣＤ
                    AND ISNULL(工場フラグ, 0) = 0
                    AND ISNULL(無効フラグ, 0) = 0
                    AND 実施日 = @持出日
                ) AS RM
                ON RM.コースＣＤ = CM.コースＣＤ
        WHERE
            CM.企業ＣＤ = @企業ＣＤ
            AND CM.コース区分 = @コース区分
            AND CM.部署ＣＤ = @部署ＣＤ
            AND ISNULL(CM.工場フラグ, 0) = 0
            AND ISNULL(CM.無効フラグ, 0) = 0
            AND RM.コースＣＤ IS NULL
        -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
        UNION
        SELECT
            RM.企業ＣＤ
            ,RM.部署ＣＤ
            ,RM.コースＣＤ
            ,RM.コース名
        FROM
            臨時コーステーブル AS RT
            INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0
        WHERE
            RM.企業ＣＤ = @企業ＣＤ
            AND RM.コース区分 = @コース区分
            AND RM.部署ＣＤ = @部署ＣＤ
            AND ISNULL(RM.工場フラグ, 0) = 0
            AND ISNULL(RT.無効フラグ, 0) = 0
            AND RM.実施日 = @持出日
        ) AS C
        INNER JOIN 主要部品リストマスタ AS P ON P.企業ＣＤ = C.企業ＣＤ AND P.部署ＣＤ = C.部署ＣＤ
        LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = P.企業ＣＤ AND S.商品ＣＤ = P.商品ＣＤ
    WHERE
        ISNULL(S.無効フラグ, 0) = 0
    ) AS MAIN
    LEFT JOIN
    -- ③保存済みのデータ
    (SELECT
        コースＣＤ
        ,商品ＣＤ
        ,個数
    FROM
        モバイル_持ち出し入力
    WHERE
        企業ＣＤ = @企業ＣＤ AND
        持出日 = @持出日
    ) AS MCDS ON MAIN.コースＣＤ = MCDS.コースＣＤ AND MAIN.商品ＣＤ = MCDS.商品ＣＤ
-- ④合計行
UNION
SELECT
    NULL AS コースＣＤ
   ,'合計' AS コース名
   ,-1 AS 商品ＣＤ
   ,-1 AS 個数
   , 1 AS 合計行
ORDER BY
    合計行,
    コースＣＤ,
    商品ＣＤ
