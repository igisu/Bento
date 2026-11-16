-- 生産予定数入力（生産予定から取得）
SELECT
    A.コースＣＤ
    ,CM.コース名
    ,A.商品ＣＤ
    ,SM.商品名
    ,ISNULL(Y.入力フラグ, 1) AS 入力フラグ  -- 生産予定が無い場合は、これから入力するということなので、入力フラグのデフォルトはON
    ,Y.数量
FROM
    -- ①駆動表
    -- コースマスタと生産予定テーブルから取得したコース（＝全コース）と、商品マスタと生産予定テーブルから取得した商品（＝全商品）の組み合わせ
    (SELECT
        C.コースＣＤ
        ,I.商品ＣＤ
    FROM
        (SELECT
            コースＣＤ
        FROM
            コースマスタ
        WHERE
            企業ＣＤ = @企業ＣＤ
            AND 部署ＣＤ = @部署ＣＤ
            AND コース区分 = @コース区分
            AND ISNULL(工場フラグ, 0) = 0
            AND ISNULL(無効フラグ, 0) = 0
        UNION
        SELECT
            コースＣＤ
        FROM
            生産予定
        WHERE
            企業ＣＤ = @企業ＣＤ
            AND 部署ＣＤ = @部署ＣＤ
            AND 喫食日 = @注文日
        ) AS C,
        (SELECT
            商品ＣＤ
        FROM
            商品マスタ
        WHERE
            企業ＣＤ = @企業ＣＤ
            AND ISNULL(無効フラグ, 0) = 0
            AND 弁当区分 IN (@弁当区分副食, @弁当区分主食)  -- 副食と主食のみ
        UNION
        SELECT
            商品ＣＤ
        FROM
            生産予定
        WHERE
            企業ＣＤ = @企業ＣＤ
            AND 部署ＣＤ = @部署ＣＤ
            AND 喫食日 = @注文日
        ) AS I
    ) AS A
    -- ②生産予定から数量を取得
    LEFT JOIN
        (SELECT
            商品ＣＤ
            ,コースＣＤ
            ,入力フラグ
            ,数量
        FROM
            生産予定
        WHERE
            企業ＣＤ = @企業ＣＤ
            AND 部署ＣＤ = @部署ＣＤ
            AND 喫食日 = @注文日
        ) AS Y
        ON Y.コースＣＤ = A.コースＣＤ
        AND Y.商品ＣＤ = A.商品ＣＤ
    -- ③名称を取得するためのマスタを結合
    LEFT JOIN コースマスタ AS CM ON CM.企業ＣＤ = @企業ＣＤ AND CM.コースＣＤ = A.コースＣＤ
    LEFT JOIN 商品マスタ AS SM ON SM.企業ＣＤ = @企業ＣＤ AND SM.商品ＣＤ = A.商品ＣＤ
    -- ④コース別グループと結合
--    INNER JOIN コース別グループマスタ AS CG ON
--        CG.企業ＣＤ = @企業ＣＤ
--        AND CG.部署ＣＤ = @部署ＣＤ
--        AND CG.グループ種別ＣＤ = @グループ種別ＣＤ
--        AND CG.コースＣＤ = A.コースＣＤ
    WHERE
        CM.コース区分 = @コース区分
ORDER BY
    SM.弁当区分
    ,A.商品ＣＤ
