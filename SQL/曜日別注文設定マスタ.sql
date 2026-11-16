-- 曜日別注文設定マスタ
SELECT
    A.企業ＣＤ
    ,A.得意先ＣＤ
    ,A.得意先ＣＤ AS 得意先ＣＤorg
    ,T.得意先名
    ,A.商品ＣＤ
    ,A.商品ＣＤ AS 商品ＣＤorg
    ,S.商品名
    ,ISNULL(K.商品単価税抜, S.商品単価税抜) AS 商品単価税抜
    ,ISNULL(K.商品単価税込, S.商品単価税込) AS 商品単価税込
    ,S.軽減税率フラグ
    ,CASE WHEN K.商品単価税込 IS NULL THEN 1 ELSE 0 END AS 単価淡色フラグ
    ,A.注文数_日
    ,A.注文数_月
    ,A.注文数_火
    ,A.注文数_水
    ,A.注文数_木
    ,A.注文数_金
    ,A.注文数_土
    ,A.注文数_祝
    ,A.作成担当者ＣＤ
    ,A1.担当者名 AS 作成担当者名
    ,A.作成日
    ,A.修正担当者ＣＤ
    ,A2.担当者名 AS 修正担当者名
    ,A.修正日
    ,WEK.有効フラグ AS 平日有効
    ,SAT.有効フラグ AS 土曜有効
    ,SUN.有効フラグ AS 日曜有効
    ,HOL.有効フラグ AS 祝日有効
    ,T.休日設定
    ,T.祝日配送区分
    ,WT.得意先ＣＤ AS WEB得意先ＣＤ
FROM
    曜日別注文設定マスタ AS A
    LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = A.企業ＣＤ AND T.得意先ＣＤ = A.得意先ＣＤ
    LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND S.商品ＣＤ = A.商品ＣＤ
    LEFT JOIN 得意先単価マスタ AS K ON K.企業ＣＤ = A.企業ＣＤ AND K.得意先ＣＤ = A.得意先ＣＤ AND K.商品ＣＤ = A.商品ＣＤ AND ISNULL(K.無効フラグ, 0) = 0
    LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = A.企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ
    LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = A.企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ
    LEFT JOIN WEB_M_得意先 AS WT ON WT.企業ＣＤ = A.企業ＣＤ AND WT.得意先ＣＤ = A.得意先ＣＤ AND ISNULL(WT.無効フラグ, 0) = 0
    LEFT JOIN
        -- 平日
        (SELECT DISTINCT
            CM.企業ＣＤ
            ,CT.得意先ＣＤ
            ,1 AS 有効フラグ
        FROM
            コースマスタ AS CM
            INNER JOIN コーステーブル CT ON CT.企業ＣＤ = CM.企業ＣＤ AND CT.コースＣＤ = CM.コースＣＤ
        WHERE
            CM.コース区分 = @コース区分平日     -- 1
            AND ISNULL(CM.無効フラグ, 0) = 0
            AND ISNULL(CT.無効フラグ, 0) = 0
        ) AS WEK
        ON WEK.企業ＣＤ = A.企業ＣＤ
        AND WEK.得意先ＣＤ = A.得意先ＣＤ
    LEFT JOIN
        -- 土曜
        (SELECT DISTINCT
            CM.企業ＣＤ
            ,CT.得意先ＣＤ
            ,1 AS 有効フラグ
        FROM
            コースマスタ AS CM
            INNER JOIN コーステーブル CT ON CT.企業ＣＤ = CM.企業ＣＤ AND CT.コースＣＤ = CM.コースＣＤ
        WHERE
            CM.コース区分 = @コース区分土曜     -- 2
            AND ISNULL(CM.無効フラグ, 0) = 0
            AND ISNULL(CT.無効フラグ, 0) = 0
        ) AS SAT
        ON SAT.企業ＣＤ = A.企業ＣＤ
        AND SAT.得意先ＣＤ = A.得意先ＣＤ
    LEFT JOIN
        -- 日曜
        (SELECT DISTINCT
            CM.企業ＣＤ
            ,CT.得意先ＣＤ
            ,1 AS 有効フラグ
        FROM
            コースマスタ AS CM
            INNER JOIN コーステーブル CT ON CT.企業ＣＤ = CM.企業ＣＤ AND CT.コースＣＤ = CM.コースＣＤ
        WHERE
            CM.コース区分 = @コース区分日曜     -- 3
            AND ISNULL(CM.無効フラグ, 0) = 0
            AND ISNULL(CT.無効フラグ, 0) = 0
        ) AS SUN
        ON SUN.企業ＣＤ = A.企業ＣＤ
        AND SUN.得意先ＣＤ = A.得意先ＣＤ
    LEFT JOIN
        -- 祝日
        (SELECT DISTINCT
            CM.企業ＣＤ
            ,CT.得意先ＣＤ
            ,1 AS 有効フラグ
        FROM
            コースマスタ AS CM
            INNER JOIN コーステーブル CT ON CT.企業ＣＤ = CM.企業ＣＤ AND CT.コースＣＤ = CM.コースＣＤ
        WHERE
            CM.コース区分 = @コース区分祝日     -- 4
            AND ISNULL(CM.無効フラグ, 0) = 0
            AND ISNULL(CT.無効フラグ, 0) = 0
        ) AS HOL
        ON HOL.企業ＣＤ = A.企業ＣＤ
        AND HOL.得意先ＣＤ = A.得意先ＣＤ

    -- コースによる絞り込み用
    -- 同じ得意先に複数のコースが設定されている場合、そのいずれのコースを条件に指定しても、そのコースにぶら下がる得意先がヒットしないといけない。
    -- 検索条件にコースが指定された場合のみ、INNER JOIN以下を追加すること
    INNER JOIN
        (SELECT DISTINCT
            CM.企業ＣＤ
            ,CM.部署ＣＤ
            ,CM.コース名
            ,CT.コースＣＤ
            ,CT.得意先ＣＤ
            ,CT.コース順
        FROM
            コースマスタ AS CM
            INNER JOIN コーステーブル AS CT ON
                CM.企業ＣＤ = CT.企業ＣＤ
                AND CM.コースＣＤ =  CT.コースＣＤ
        WHERE
            CT.コースＣＤ = @コースＣＤ
        ) AS C
        ON A.企業ＣＤ = C.企業ＣＤ
        AND A.得意先ＣＤ = C.得意先ＣＤ

WHERE
    A.企業ＣＤ = @企業ＣＤ
    AND T.部署ＣＤ = @部署ＣＤ
    AND A.得意先ＣＤ = @得意先ＣＤ
    AND A.商品ＣＤ = @商品ＣＤ






--- 得意先と有効フラグのみ
SELECT
    A.得意先ＣＤ
    ,WEK.有効フラグ AS 平日有効
    ,SAT.有効フラグ AS 土曜有効
    ,SUN.有効フラグ AS 日曜有効
    ,HOL.有効フラグ AS 祝日有効
    ,A.休日設定
    ,A.祝日配送区分
FROM
    得意先マスタ AS A
    LEFT JOIN
-- 平日
        (SELECT DISTINCT
            CM.企業ＣＤ
            ,CT.得意先ＣＤ
            ,1 AS 有効フラグ
        FROM
            コースマスタ AS CM
            INNER JOIN コーステーブル CT ON CT.企業ＣＤ = CM.企業ＣＤ AND CT.コースＣＤ = CM.コースＣＤ
        WHERE
            CM.コース区分 = @コース区分平日     -- 1
            AND ISNULL(CM.無効フラグ, 0) = 0
            AND ISNULL(CT.無効フラグ, 0) = 0
        ) AS WEK
        ON WEK.企業ＣＤ = A.企業ＣＤ
        AND WEK.得意先ＣＤ = A.得意先ＣＤ
    LEFT JOIN
-- 土曜
        (SELECT DISTINCT
            CM.企業ＣＤ
            ,CT.得意先ＣＤ
            ,1 AS 有効フラグ
        FROM
            コースマスタ AS CM
            INNER JOIN コーステーブル CT ON CT.企業ＣＤ = CM.企業ＣＤ AND CT.コースＣＤ = CM.コースＣＤ
        WHERE
            CM.コース区分 = @コース区分土曜     -- 2
            AND ISNULL(CM.無効フラグ, 0) = 0
            AND ISNULL(CT.無効フラグ, 0) = 0
        ) AS SAT
        ON SAT.企業ＣＤ = A.企業ＣＤ
        AND SAT.得意先ＣＤ = A.得意先ＣＤ
    LEFT JOIN
-- 日曜
        (SELECT DISTINCT
            CM.企業ＣＤ
            ,CT.得意先ＣＤ
            ,1 AS 有効フラグ
        FROM
            コースマスタ AS CM
            INNER JOIN コーステーブル CT ON CT.企業ＣＤ = CM.企業ＣＤ AND CT.コースＣＤ = CM.コースＣＤ
        WHERE
            CM.コース区分 = @コース区分日曜     -- 3
            AND ISNULL(CM.無効フラグ, 0) = 0
            AND ISNULL(CT.無効フラグ, 0) = 0
        ) AS SUN
        ON SUN.企業ＣＤ = A.企業ＣＤ
        AND SUN.得意先ＣＤ = A.得意先ＣＤ
    LEFT JOIN
-- 祝日
        (SELECT DISTINCT
            CM.企業ＣＤ
            ,CT.得意先ＣＤ
            ,1 AS 有効フラグ
        FROM
            コースマスタ AS CM
            INNER JOIN コーステーブル CT ON CT.企業ＣＤ = CM.企業ＣＤ AND CT.コースＣＤ = CM.コースＣＤ
        WHERE
            CM.コース区分 = @コース区分祝日     -- 4
            AND ISNULL(CM.無効フラグ, 0) = 0
            AND ISNULL(CT.無効フラグ, 0) = 0
        ) AS HOL
        ON HOL.企業ＣＤ = A.企業ＣＤ
        AND HOL.得意先ＣＤ = A.得意先ＣＤ
WHERE
    A.企業ＣＤ = @企業ＣＤ
    AND A.得意先ＣＤ = @得意先ＣＤ
