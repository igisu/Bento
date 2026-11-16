-- 生産予定数入力（注文から取得）
-- 売上実績から取得と同様
SELECT
    A.コースＣＤ
    ,CM.コース名
    ,A.商品ＣＤ
    ,SM.商品名
    ,ISNULL(U.入力フラグ, 1) AS 入力フラグ      -- 売上に無い場合は手入力するので入力フラグのデフォルトはON
    ,U.数量
FROM
    (SELECT
        C.コースＣＤ
        ,I.商品ＣＤ
    FROM
        -- コースマスタと生産予定テーブルと注文から取得したコース（＝全コース）と、商品マスタと生産予定テーブルと注文から取得した商品（＝全商品）の組み合わせ
        -- ①コース
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
        UNION
        SELECT
            H.コースＣＤ
        FROM
            注文ヘッダ AS H
            INNER JOIN コースマスタ AS C
                ON  C.企業ＣＤ   = H.企業ＣＤ
                AND C.コースＣＤ = H.コースＣＤ
        WHERE
            H.企業ＣＤ = @企業ＣＤ
            AND H.部署ＣＤ = @部署ＣＤ
            AND H.コースＣＤ IS NOT NULL
            AND H.喫食日 >= @注文日
            AND C.コース区分 = @コース区分      -- 曜日に応じたコース
            AND ISNULL(C.工場フラグ, 0) = 0
            AND ISNULL(C.無効フラグ, 0) = 0
        ) AS C,
        -- ②商品
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
        UNION
        -- ②－１ 弁当の主食
        SELECT
            I.主食ＣＤ AS 商品ＣＤ
        FROM
            注文明細 AS M
            INNER JOIN 注文ヘッダ AS H
                ON H.企業ＣＤ = M.企業ＣＤ
                AND H.注文区分 = M.注文区分
                AND H.喫食日 = M.喫食日
                AND H.得意先ＣＤ = M.得意先ＣＤ
                AND H.部署ＣＤ = M.部署ＣＤ
            INNER JOIN 商品マスタ AS I
                ON I.企業ＣＤ = M.企業ＣＤ
                AND ISNULL(I.無効フラグ, 0) = 0
                AND I.商品ＣＤ = M.商品ＣＤ
        WHERE
            M.企業ＣＤ = @企業ＣＤ
            AND M.部署ＣＤ = @部署ＣＤ
            AND H.コースＣＤ IS NOT NULL
            AND M.喫食日 = @注文日
            AND ISNULL(I.弁当区分, 0) = @弁当区分弁当   -- 弁当
            AND ISNULL(I.主食ＣＤ, 0) <> 0              -- 主食がある
            AND ISNULL(I.主食ＣＤ, 0) <> I.商品ＣＤ     -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
        UNION
        -- ②－２ 弁当の副食
        SELECT
            I.副食ＣＤ AS 商品ＣＤ
        FROM
            注文明細 AS M
            INNER JOIN 注文ヘッダ AS H
                ON H.企業ＣＤ = M.企業ＣＤ
                AND H.注文区分 = M.注文区分
                AND H.喫食日 = M.喫食日
                AND H.得意先ＣＤ = M.得意先ＣＤ
                AND H.部署ＣＤ = M.部署ＣＤ
            INNER JOIN 商品マスタ AS I
                ON I.企業ＣＤ = M.企業ＣＤ
                AND ISNULL(I.無効フラグ, 0) = 0
                AND I.商品ＣＤ = M.商品ＣＤ
        WHERE
            M.企業ＣＤ = @企業ＣＤ
            AND M.部署ＣＤ = @部署ＣＤ
            AND H.コースＣＤ IS NOT NULL
            AND M.喫食日 = @注文日
            AND ISNULL(I.弁当区分, 0) = @弁当区分弁当   -- 弁当
            AND ISNULL(I.副食ＣＤ, 0) <> 0              -- 副食がある
            AND ISNULL(I.副食ＣＤ, 0) <> I.商品ＣＤ     -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
        UNION
        -- ②－３ 主食のみ
        SELECT
            I.商品ＣＤ AS 商品ＣＤ
        FROM
            注文明細 AS M
            INNER JOIN 注文ヘッダ AS H
                ON H.企業ＣＤ = M.企業ＣＤ
                AND H.注文区分 = M.注文区分
                AND H.喫食日 = M.喫食日
                AND H.得意先ＣＤ = M.得意先ＣＤ
                AND H.部署ＣＤ = M.部署ＣＤ
            INNER JOIN 商品マスタ AS I
                ON I.企業ＣＤ = M.企業ＣＤ
                AND ISNULL(I.無効フラグ, 0) = 0
                AND I.商品ＣＤ = M.商品ＣＤ
        WHERE
            M.企業ＣＤ = @企業ＣＤ
            AND M.部署ＣＤ = @部署ＣＤ
            AND H.コースＣＤ IS NOT NULL
            AND M.喫食日 = @注文日
            AND ISNULL(I.弁当区分, 0) = @弁当区分主食
        UNION
        -- ②－４ 副食のみ
        SELECT
            I.商品ＣＤ AS 商品ＣＤ
        FROM
            注文明細 AS M
            INNER JOIN 注文ヘッダ AS H
                ON H.企業ＣＤ = M.企業ＣＤ
                AND H.注文区分 = M.注文区分
                AND H.喫食日 = M.喫食日
                AND H.得意先ＣＤ = M.得意先ＣＤ
                AND H.部署ＣＤ = M.部署ＣＤ
            INNER JOIN 商品マスタ AS I
                ON I.企業ＣＤ = M.企業ＣＤ
                AND ISNULL(I.無効フラグ, 0) = 0
                AND I.商品ＣＤ = M.商品ＣＤ
        WHERE
            M.企業ＣＤ = @企業ＣＤ
            AND M.部署ＣＤ = @部署ＣＤ
            AND H.コースＣＤ IS NOT NULL
            AND M.喫食日 = @注文日
            AND ISNULL(I.弁当区分, 0) = @弁当区分副食
        ) AS I
    ) AS A
    -- ③注文明細からコースと商品ごとの数量を取得
    LEFT JOIN
        (SELECT
            B.コースＣＤ
            ,B.部品ＣＤ AS 商品ＣＤ
            ,0 AS 入力フラグ
            ,SUM(B.商品注文予測数量) AS 数量
        FROM
            -- 注文数が0の場合、予測を採用するのではなく、0の注文数を採用する（注文のレコードがあれば必ず採用する）
            -- ③－１ 弁当の主食
            (SELECT
                PART1.コースＣＤ
                ,PART1.得意先ＣＤ
                ,PART1.部品ＣＤ
                ,SUM(ISNULL(PART1.商品注文数量, PART1.商品予測数量)) AS 商品注文予測数量
            FROM
                (SELECT
                    H.コースＣＤ
                    ,H.得意先ＣＤ
    --              ,M.商品ＣＤ
                    ,I.主食ＣＤ AS 部品ＣＤ
                    ,SUM(CASE WHEN M.注文区分 = 0 THEN M.商品数量 ELSE null END) AS 商品注文数量
                    ,SUM(CASE WHEN M.注文区分 = 1 THEN M.商品数量 ELSE null END) AS 商品予測数量
                FROM
                    注文明細 AS M
                    INNER JOIN 注文ヘッダ AS H
                        ON H.企業ＣＤ = M.企業ＣＤ
                        AND H.注文区分 = M.注文区分
                        AND H.喫食日 = M.喫食日
                        AND H.得意先ＣＤ = M.得意先ＣＤ
                        AND H.部署ＣＤ = M.部署ＣＤ
                    INNER JOIN 商品マスタ AS I
                        ON I.企業ＣＤ = M.企業ＣＤ
                        AND ISNULL(I.無効フラグ, 0) = 0
                        AND I.商品ＣＤ = M.商品ＣＤ
                WHERE
                    M.企業ＣＤ = @企業ＣＤ
                    AND M.部署ＣＤ = @部署ＣＤ
                    AND H.コースＣＤ IS NOT NULL
                    AND M.喫食日 = @注文日
                    AND ISNULL(I.弁当区分, 0) = @弁当区分弁当   -- 弁当
                    AND ISNULL(I.主食ＣＤ, 0) <> 0              -- 主食がある
                    AND ISNULL(I.主食ＣＤ, 0) <> I.商品ＣＤ     -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                GROUP BY
                    H.コースＣＤ
                    ,H.得意先ＣＤ
    --              ,M.商品ＣＤ
                    ,I.主食ＣＤ
                ) AS PART1
            GROUP BY
                PART1.コースＣＤ
                ,PART1.得意先ＣＤ
                ,PART1.部品ＣＤ
            UNION ALL   -- SUM()の対象にするので、同一部署、同一コース、同一部品ＣＤ、同一売上日、同一商品数量を同一視せずに全て抽出する（以下同様）
            -- ③－２ 弁当の副食
            SELECT
                PART2.コースＣＤ
                ,PART2.得意先ＣＤ
                ,PART2.部品ＣＤ
                ,SUM(ISNULL(PART2.商品注文数量, PART2.商品予測数量)) AS 商品注文予測数量
            FROM
                (SELECT
                    H.コースＣＤ
                    ,H.得意先ＣＤ
    --              ,M.商品ＣＤ
                    ,I.副食ＣＤ AS 部品ＣＤ
                    ,SUM(CASE WHEN M.注文区分 = 0 THEN M.商品数量 ELSE null END) AS 商品注文数量
                    ,SUM(CASE WHEN M.注文区分 = 1 THEN M.商品数量 ELSE null END) AS 商品予測数量
                FROM
                    注文明細 AS M
                    INNER JOIN 注文ヘッダ AS H
                        ON H.企業ＣＤ = M.企業ＣＤ
                        AND H.注文区分 = M.注文区分
                        AND H.喫食日 = M.喫食日
                        AND H.得意先ＣＤ = M.得意先ＣＤ
                        AND H.部署ＣＤ = M.部署ＣＤ
                    INNER JOIN 商品マスタ AS I
                        ON I.企業ＣＤ = M.企業ＣＤ
                        AND ISNULL(I.無効フラグ, 0) = 0
                        AND I.商品ＣＤ = M.商品ＣＤ
                WHERE
                    M.企業ＣＤ = @企業ＣＤ
                    AND M.部署ＣＤ = @部署ＣＤ
                    AND H.コースＣＤ IS NOT NULL
                    AND M.喫食日 = @注文日
                    AND ISNULL(I.弁当区分, 0) = @弁当区分弁当   -- 弁当
                    AND ISNULL(I.副食ＣＤ, 0) <> 0              -- 副食がある
                    AND ISNULL(I.副食ＣＤ, 0) <> I.商品ＣＤ     -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                GROUP BY
                    H.コースＣＤ
                    ,H.得意先ＣＤ
    --              ,M.商品ＣＤ
                    ,I.副食ＣＤ
                ) AS PART2
            GROUP BY
                PART2.コースＣＤ
                ,PART2.得意先ＣＤ
                ,PART2.部品ＣＤ
            UNION ALL
            -- ③－３ 主食のみ
            SELECT
                PART3.コースＣＤ
                ,PART3.得意先ＣＤ
                ,PART3.部品ＣＤ
                ,SUM(ISNULL(PART3.商品注文数量, PART3.商品予測数量)) AS 商品注文予測数量
            FROM
                (SELECT
                    H.コースＣＤ
                    ,H.得意先ＣＤ
    --              ,M.商品ＣＤ
                    ,I.商品ＣＤ AS 部品ＣＤ
                    ,SUM(CASE WHEN M.注文区分 = 0 THEN M.商品数量 ELSE null END) AS 商品注文数量
                    ,SUM(CASE WHEN M.注文区分 = 1 THEN M.商品数量 ELSE null END) AS 商品予測数量
                FROM
                    注文明細 AS M
                    INNER JOIN 注文ヘッダ AS H
                        ON H.企業ＣＤ = M.企業ＣＤ
                        AND H.注文区分 = M.注文区分
                        AND H.喫食日 = M.喫食日
                        AND H.得意先ＣＤ = M.得意先ＣＤ
                        AND H.部署ＣＤ = M.部署ＣＤ
                    INNER JOIN 商品マスタ AS I
                        ON I.企業ＣＤ = M.企業ＣＤ
                        AND ISNULL(I.無効フラグ, 0) = 0
                        AND I.商品ＣＤ = M.商品ＣＤ
                WHERE
                    M.企業ＣＤ = @企業ＣＤ
                    AND M.部署ＣＤ = @部署ＣＤ
                    AND H.コースＣＤ IS NOT NULL
                    AND M.喫食日 = @注文日
                    AND ISNULL(I.弁当区分, 0) = @弁当区分主食
                GROUP BY
                    H.コースＣＤ
                    ,H.得意先ＣＤ
    --              ,M.商品ＣＤ
                    ,I.商品ＣＤ
                ) AS PART3
            GROUP BY
                PART3.コースＣＤ
                ,PART3.得意先ＣＤ
                ,PART3.部品ＣＤ
            UNION ALL
            -- ③－４ 副食のみ
            SELECT
                PART4.コースＣＤ
                ,PART4.得意先ＣＤ
                ,PART4.部品ＣＤ
                ,SUM(ISNULL(PART4.商品注文数量, PART4.商品予測数量)) AS 商品注文予測数量
            FROM
                (SELECT
                    H.コースＣＤ
                    ,H.得意先ＣＤ
    --              ,M.商品ＣＤ
                    ,I.商品ＣＤ AS 部品ＣＤ
                    ,SUM(CASE WHEN M.注文区分 = 0 THEN M.商品数量 ELSE null END) AS 商品注文数量
                    ,SUM(CASE WHEN M.注文区分 = 1 THEN M.商品数量 ELSE null END) AS 商品予測数量
                FROM
                    注文明細 AS M
                    INNER JOIN 注文ヘッダ AS H
                        ON H.企業ＣＤ = M.企業ＣＤ
                        AND H.注文区分 = M.注文区分
                        AND H.喫食日 = M.喫食日
                        AND H.得意先ＣＤ = M.得意先ＣＤ
                        AND H.部署ＣＤ = M.部署ＣＤ
                    INNER JOIN 商品マスタ AS I
                        ON I.企業ＣＤ = M.企業ＣＤ
                        AND ISNULL(I.無効フラグ, 0) = 0
                        AND I.商品ＣＤ = M.商品ＣＤ
                WHERE
                    M.企業ＣＤ = @企業ＣＤ
                    AND M.部署ＣＤ = @部署ＣＤ
                    AND H.コースＣＤ IS NOT NULL
                    AND M.喫食日 = @注文日
                    AND ISNULL(I.弁当区分, 0) = @弁当区分副食
                GROUP BY
                    H.コースＣＤ
                    ,H.得意先ＣＤ
    --              ,M.商品ＣＤ
                    ,I.商品ＣＤ
                ) AS PART4
            GROUP BY
                PART4.コースＣＤ
                ,PART4.得意先ＣＤ
                ,PART4.部品ＣＤ
            ) AS B
        GROUP BY
            B.コースＣＤ
            ,B.部品ＣＤ
        ) AS U
        ON U.コースＣＤ = A.コースＣＤ
        AND U.商品ＣＤ = A.商品ＣＤ
    -- ④名称を取得するためのマスタを結合
    LEFT JOIN コースマスタ AS CM ON CM.企業ＣＤ = @企業ＣＤ AND CM.コースＣＤ = A.コースＣＤ
    LEFT JOIN 商品マスタ AS SM ON SM.企業ＣＤ = @企業ＣＤ AND SM.商品ＣＤ = A.商品ＣＤ
    -- ⑤コース別グループと結合
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
