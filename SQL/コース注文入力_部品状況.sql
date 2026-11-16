-- コース注文入力_部品状況
SELECT
    A.行番号
    ,A.見出し
    ,B.商品ＣＤ
    ,B.商品名略称
    ,B.数量
FROM
    (SELECT
        0 AS 行番号
        ,'持出' AS 見出し
    UNION
    SELECT
        1 AS 行番号
        ,'移動' AS 見出し
    UNION
    SELECT
        2 AS 行番号
        ,'受注' AS 見出し
    ) AS A
    LEFT JOIN
    -- 持出行
    (SELECT
        0 AS 行番号
         ,'持出' AS 見出し
        ,MAIN.商品ＣＤ
        ,MAIN.商品名略称
        ,ISNULL(MCDS.個数, 0) AS 数量
    FROM
        -- ①駆動表（指定されたコースの商品）
        (SELECT
            P.商品ＣＤ
            ,S.商品名略称
        FROM
            主要部品リストマスタ AS P
            LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = P.企業ＣＤ AND S.商品ＣＤ = P.商品ＣＤ
        WHERE
            P.企業ＣＤ = @企業ＣＤ
            AND P.部署ＣＤ = @部署ＣＤ
            AND ISNULL(S.無効フラグ, 0) = 0
        ) AS MAIN
        LEFT JOIN
        -- ③持ち出し数
        (SELECT
            商品ＣＤ
            ,SUM(個数) AS 個数
        FROM
            モバイル_持ち出し入力
        WHERE
            企業ＣＤ = @企業ＣＤ
            AND コースＣＤ = @コースＣＤ
            AND 持出日 = @注文日
        GROUP BY
            商品ＣＤ
        ) AS MCDS ON MAIN.商品ＣＤ = MCDS.商品ＣＤ
    UNION
    -- 移動行
    SELECT
        1 AS 行番号
         ,'移動' AS 見出し
        ,MAIN.商品ＣＤ
        ,MAIN.商品名略称
        -- 受け取った個数－渡した個数を移動数とする
        ,ISNULL(TAKE.個数, 0) - ISNULL(GIVE.個数, 0) AS 数量
    FROM
        -- ①駆動表（指定されたコースの商品）
        (SELECT
            P.商品ＣＤ
            ,S.商品名略称
        FROM
            主要部品リストマスタ AS P
            LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = P.企業ＣＤ AND S.商品ＣＤ = P.商品ＣＤ
        WHERE
            P.企業ＣＤ = @企業ＣＤ
            AND P.部署ＣＤ = @部署ＣＤ
            AND ISNULL(S.無効フラグ, 0) = 0
        ) AS MAIN
        LEFT JOIN
        -- ④もらった個数
        (SELECT
            商品ＣＤ
            ,SUM(個数) AS 個数
        FROM
            モバイル_移動入力
        WHERE
            企業ＣＤ = @企業ＣＤ
            AND 相手コースＣＤ = @コースＣＤ
            AND 移動日 = @注文日
        GROUP BY
            商品ＣＤ
        ) AS TAKE ON MAIN.商品ＣＤ = TAKE.商品ＣＤ
        LEFT JOIN
        -- ⑥渡した個数
        (SELECT
            コースＣＤ
            ,商品ＣＤ
            ,SUM(個数) AS 個数
        FROM
            モバイル_移動入力
        WHERE
            企業ＣＤ = @企業ＣＤ
            AND コースＣＤ = @コースＣＤ
            AND 移動日 = @注文日
        GROUP BY
            コースＣＤ
            ,商品ＣＤ
        ) AS GIVE ON MAIN.商品ＣＤ = GIVE.商品ＣＤ
    UNION
    -- 受注行
    SELECT
        2 AS 行番号
         ,'受注' AS 見出し
        ,MAIN.商品ＣＤ
        ,MAIN.商品名略称
        ,ISNULL(INIT.数量, 0) AS 数量
    FROM
        -- ①駆動表（指定されたコースの商品）
        (SELECT
            P.商品ＣＤ
            ,S.商品名略称
        FROM
            主要部品リストマスタ AS P
            LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = P.企業ＣＤ AND S.商品ＣＤ = P.商品ＣＤ
        WHERE
            P.企業ＣＤ = @企業ＣＤ
            AND P.部署ＣＤ = @部署ＣＤ
            AND ISNULL(S.無効フラグ, 0) = 0
        ) AS MAIN
        LEFT JOIN
        (SELECT
            PARTSUM.部品ＣＤ
            ,SUM(PARTSUM.数量) AS 数量
        FROM
            -- ②予測と注文と売上の部品（各商品を分割した部品ごとに合計する）
            (SELECT
                PART.得意先ＣＤ
                ,PART.部品ＣＤ
                ,CASE WHEN ISNULL(SUM(PART.商品売上数量), -1) >= 0 THEN SUM(PART.商品売上数量)
                      ELSE ISNULL(SUM(PART.商品注文予測数量), 0) END
                 AS 数量
            FROM
                -- ②－１ 予測と注文の弁当の主食
                (SELECT
                    PART1.得意先ＣＤ
                    ,PART1.部品ＣＤ
                    ,SUM(ISNULL(PART1.商品注文数量, PART1.商品予測数量)) AS 商品注文予測数量
                    ,null AS 商品売上数量
                FROM
                    (SELECT
                        H.得意先ＣＤ
        --                ,D.商品ＣＤ
                        ,I.主食ＣＤ AS 部品ＣＤ
                        ,SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量
                        ,SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量
                    FROM
                        注文ヘッダ AS H
                        INNER JOIN 注文明細 AS D
                            ON D.企業ＣＤ    = H.企業ＣＤ
                            AND D.注文区分   = H.注文区分
                            AND D.喫食日     = H.喫食日
                            AND D.得意先ＣＤ = H.得意先ＣＤ
                            AND D.部署ＣＤ   = H.部署ＣＤ
                        -- 売上には存在しないこと（売上に存在する注文は取得しない）
                        -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                        LEFT JOIN
                            (SELECT
                               H.企業ＣＤ,
                               H.売上日,
                               H.得意先ＣＤ,
                               H.部署ＣＤ,
                               H.コースＣＤ,
                               D.商品ＣＤ,
                               D.明細番号
                            FROM
                                -- 喫食日売上ヘッダ
                                -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                (SELECT
                                    T1.企業ＣＤ,
                                    T1.売上日,
                                    T1.得意先ＣＤ,
                                    T1.部署ＣＤ,
                                    T1.コースＣＤ
                                FROM
                                    (SELECT
                                        企業ＣＤ,
                                        部署ＣＤ,
                                        コースＣＤ,
                                        得意先ＣＤ,
                                        喫食日 AS 売上日
                                    FROM
                                        注文ヘッダ
                                    WHERE
                                        企業ＣＤ     = @企業ＣＤ
                                        AND 配送日   = @注文日
                                        AND 部署ＣＤ = @部署ＣＤ
                                        AND コースＣＤ = @コースＣＤ
                                    ) T2 -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                    INNER JOIN 売上ヘッダ T1 ON
                                        T1.企業ＣＤ       = T2.企業ＣＤ
                                        AND T1.得意先ＣＤ = T2.得意先ＣＤ
                                        AND T1.売上日     = T2.売上日
                                        AND T1.部署ＣＤ   = T2.部署ＣＤ
                                        AND T1.コースＣＤ = T2.コースＣＤ
                                UNION
                                SELECT
                                    T1.企業ＣＤ,
                                    T1.売上日,
                                    T1.得意先ＣＤ,
                                    T1.部署ＣＤ,
                                    T1.コースＣＤ
                                FROM
                                    売上ヘッダ T1
                                WHERE
                                    T1.企業ＣＤ     = @企業ＣＤ
                                    AND T1.売上日   = @注文日
                                    AND T1.部署ＣＤ = @部署ＣＤ
                                    AND T1.コースＣＤ = @コースＣＤ
                                    AND NOT EXISTS (
                                        SELECT 'X'
                                        FROM
                                            (SELECT
                                                企業ＣＤ,
                                                部署ＣＤ,
                                                コースＣＤ,
                                                得意先ＣＤ,
                                                喫食日 AS 売上日
                                            FROM
                                                注文ヘッダ
                                            WHERE
                                                企業ＣＤ     = @企業ＣＤ
                                                AND 配送日   = @注文日
                                                AND 部署ＣＤ = @部署ＣＤ
                                                AND コースＣＤ = @コースＣＤ
                                            ) T2 -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                        WHERE
                                            T1.企業ＣＤ       = T2.企業ＣＤ
                                            AND T1.得意先ＣＤ = T2.得意先ＣＤ
                                            AND T1.売上日     = T2.売上日
                                            AND T1.部署ＣＤ   = T2.部署ＣＤ
                                            AND T1.コースＣＤ = T2.コースＣＤ)
                                ) AS H
                                INNER JOIN 売上明細 AS D
                                    ON D.企業ＣＤ    = H.企業ＣＤ
                                    AND D.売上日     = H.売上日
                                    AND D.得意先ＣＤ = H.得意先ＣＤ
                                    AND D.部署ＣＤ   = H.部署ＣＤ
                            ) AS KHD
                            ON H.企業ＣＤ = KHD.企業ＣＤ
                            AND H.コースＣＤ = KHD.コースＣＤ
                            AND D.喫食日 = KHD.売上日
                            AND D.得意先ＣＤ = KHD.得意先ＣＤ
                            AND D.部署ＣＤ = KHD.部署ＣＤ
                            AND D.商品ＣＤ = KHD.商品ＣＤ
                        INNER JOIN 商品マスタ AS I
                            ON I.企業ＣＤ = D.企業ＣＤ
                            AND ISNULL(I.無効フラグ, 0) = 0
                            AND I.商品ＣＤ = D.商品ＣＤ
                    WHERE
                        H.企業ＣＤ = @企業ＣＤ
                        AND H.部署ＣＤ = @部署ＣＤ
                        AND H.配送日 = @注文日
                        AND H.コースＣＤ = @コースＣＤ
                        AND ISNULL(I.弁当区分, 0) = @弁当区分弁当   -- 弁当
                        AND ISNULL(I.主食ＣＤ, 0) <> 0              -- 主食がある
                        AND ISNULL(I.主食ＣＤ, 0) <> I.商品ＣＤ     -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                        AND KHD.明細番号 IS NULL
                    GROUP BY
                        H.得意先ＣＤ
        --                ,D.商品ＣＤ
                        ,I.主食ＣＤ
                    ) AS PART1
                GROUP BY
                    PART1.得意先ＣＤ
                    ,PART1.部品ＣＤ
                UNION ALL   -- SUM()の対象にするので、同一部署、同一コース、同一部品ＣＤ、同一売上日、同一商品数量を同一視せずに全て抽出する（以下同様）
                -- ②－２ 予測と注文の弁当の副食
                SELECT
                    PART2.得意先ＣＤ
                    ,PART2.部品ＣＤ
                    ,SUM(ISNULL(PART2.商品注文数量, PART2.商品予測数量)) AS 商品注文予測数量
                    ,null AS 商品売上数量
                FROM
                    (SELECT
                        H.得意先ＣＤ
        --                ,D.商品ＣＤ
                        ,I.副食ＣＤ AS 部品ＣＤ
                        ,SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量
                        ,SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量
                    FROM
                        注文ヘッダ AS H
                        INNER JOIN 注文明細 AS D
                            ON D.企業ＣＤ    = H.企業ＣＤ
                            AND D.注文区分   = H.注文区分
                            AND D.喫食日     = H.喫食日
                            AND D.得意先ＣＤ = H.得意先ＣＤ
                            AND D.部署ＣＤ   = H.部署ＣＤ
                        -- 売上には存在しないこと（売上に存在する注文は取得しない）
                        -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                        LEFT JOIN
                            (SELECT
                               H.企業ＣＤ,
                               H.売上日,
                               H.得意先ＣＤ,
                               H.部署ＣＤ,
                               H.コースＣＤ,
                               D.商品ＣＤ,
                               D.明細番号
                            FROM
                                -- 喫食日売上ヘッダ
                                -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                (SELECT
                                    T1.企業ＣＤ,
                                    T1.売上日,
                                    T1.得意先ＣＤ,
                                    T1.部署ＣＤ,
                                    T1.コースＣＤ
                                FROM
                                    (SELECT
                                        企業ＣＤ,
                                        部署ＣＤ,
                                        コースＣＤ,
                                        得意先ＣＤ,
                                        喫食日 AS 売上日
                                    FROM
                                        注文ヘッダ
                                    WHERE
                                        企業ＣＤ     = @企業ＣＤ
                                        AND 配送日   = @注文日
                                        AND 部署ＣＤ = @部署ＣＤ
                                        AND コースＣＤ = @コースＣＤ
                                    ) T2 -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                    INNER JOIN 売上ヘッダ T1 ON
                                        T1.企業ＣＤ       = T2.企業ＣＤ
                                        AND T1.得意先ＣＤ = T2.得意先ＣＤ
                                        AND T1.売上日     = T2.売上日
                                        AND T1.部署ＣＤ   = T2.部署ＣＤ
                                        AND T1.コースＣＤ = T2.コースＣＤ
                                UNION
                                SELECT
                                    T1.企業ＣＤ,
                                    T1.売上日,
                                    T1.得意先ＣＤ,
                                    T1.部署ＣＤ,
                                    T1.コースＣＤ
                                FROM
                                    売上ヘッダ T1
                                WHERE
                                    T1.企業ＣＤ     = @企業ＣＤ
                                    AND T1.売上日   = @注文日
                                    AND T1.部署ＣＤ = @部署ＣＤ
                                    AND T1.コースＣＤ = @コースＣＤ
                                    AND NOT EXISTS (
                                        SELECT 'X'
                                        FROM
                                            (SELECT
                                                企業ＣＤ,
                                                部署ＣＤ,
                                                コースＣＤ,
                                                得意先ＣＤ,
                                                喫食日 AS 売上日
                                            FROM
                                                注文ヘッダ
                                            WHERE
                                                企業ＣＤ     = @企業ＣＤ
                                                AND 配送日   = @注文日
                                                AND 部署ＣＤ = @部署ＣＤ
                                                AND コースＣＤ = @コースＣＤ
                                            ) T2 -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                        WHERE
                                            T1.企業ＣＤ       = T2.企業ＣＤ
                                            AND T1.得意先ＣＤ = T2.得意先ＣＤ
                                            AND T1.売上日     = T2.売上日
                                            AND T1.部署ＣＤ   = T2.部署ＣＤ
                                            AND T1.コースＣＤ = T2.コースＣＤ)
                                ) AS H
                                INNER JOIN 売上明細 AS D
                                    ON D.企業ＣＤ    = H.企業ＣＤ
                                    AND D.売上日     = H.売上日
                                    AND D.得意先ＣＤ = H.得意先ＣＤ
                                    AND D.部署ＣＤ   = H.部署ＣＤ
                            ) AS KHD
                            ON H.企業ＣＤ = KHD.企業ＣＤ
                            AND H.コースＣＤ = KHD.コースＣＤ
                            AND D.喫食日 = KHD.売上日
                            AND D.得意先ＣＤ = KHD.得意先ＣＤ
                            AND D.部署ＣＤ = KHD.部署ＣＤ
                            AND D.商品ＣＤ = KHD.商品ＣＤ
                        INNER JOIN 商品マスタ AS I
                            ON I.企業ＣＤ = D.企業ＣＤ
                            AND ISNULL(I.無効フラグ, 0) = 0
                            AND I.商品ＣＤ = D.商品ＣＤ
                    WHERE
                        H.企業ＣＤ = @企業ＣＤ
                        AND H.部署ＣＤ = @部署ＣＤ
                        AND H.配送日 = @注文日
                        AND H.コースＣＤ = @コースＣＤ
                        AND ISNULL(I.弁当区分, 0) = @弁当区分弁当   -- 弁当
                        AND ISNULL(I.副食ＣＤ, 0) <> 0              -- 副食がある
                        AND ISNULL(I.副食ＣＤ, 0) <> I.商品ＣＤ     -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                        AND KHD.明細番号 IS NULL
                    GROUP BY
                        H.得意先ＣＤ
        --                ,D.商品ＣＤ
                        ,I.副食ＣＤ
                    ) AS PART2
                GROUP BY
                    PART2.得意先ＣＤ
                    ,PART2.部品ＣＤ
                UNION ALL
                -- ②－３ 予測と注文の主食のみ
                SELECT
                    PART3.得意先ＣＤ
                    ,PART3.部品ＣＤ
                    ,SUM(ISNULL(PART3.商品注文数量, PART3.商品予測数量)) AS 商品注文予測数量
                    ,null AS 商品売上数量
                FROM
                    (SELECT
                        H.得意先ＣＤ
        --                ,D.商品ＣＤ
                        ,I.主食ＣＤ AS 部品ＣＤ
                        ,SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量
                        ,SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量
                    FROM
                        注文ヘッダ AS H
                        INNER JOIN 注文明細 AS D
                            ON D.企業ＣＤ    = H.企業ＣＤ
                            AND D.注文区分   = H.注文区分
                            AND D.喫食日     = H.喫食日
                            AND D.得意先ＣＤ = H.得意先ＣＤ
                            AND D.部署ＣＤ   = H.部署ＣＤ
                        -- 売上には存在しないこと（売上に存在する注文は取得しない）
                        -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                        LEFT JOIN
                            (SELECT
                               H.企業ＣＤ,
                               H.売上日,
                               H.得意先ＣＤ,
                               H.部署ＣＤ,
                               H.コースＣＤ,
                               D.商品ＣＤ,
                               D.明細番号
                            FROM
                                -- 喫食日売上ヘッダ
                                -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                (SELECT
                                    T1.企業ＣＤ,
                                    T1.売上日,
                                    T1.得意先ＣＤ,
                                    T1.部署ＣＤ,
                                    T1.コースＣＤ
                                FROM
                                    (SELECT
                                        企業ＣＤ,
                                        部署ＣＤ,
                                        コースＣＤ,
                                        得意先ＣＤ,
                                        喫食日 AS 売上日
                                    FROM
                                        注文ヘッダ
                                    WHERE
                                        企業ＣＤ     = @企業ＣＤ
                                        AND 配送日   = @注文日
                                        AND 部署ＣＤ = @部署ＣＤ
                                        AND コースＣＤ = @コースＣＤ
                                    ) T2 -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                    INNER JOIN 売上ヘッダ T1 ON
                                        T1.企業ＣＤ       = T2.企業ＣＤ
                                        AND T1.得意先ＣＤ = T2.得意先ＣＤ
                                        AND T1.売上日     = T2.売上日
                                        AND T1.部署ＣＤ   = T2.部署ＣＤ
                                        AND T1.コースＣＤ = T2.コースＣＤ
                                UNION
                                SELECT
                                    T1.企業ＣＤ,
                                    T1.売上日,
                                    T1.得意先ＣＤ,
                                    T1.部署ＣＤ,
                                    T1.コースＣＤ
                                FROM
                                    売上ヘッダ T1
                                WHERE
                                    T1.企業ＣＤ     = @企業ＣＤ
                                    AND T1.売上日   = @注文日
                                    AND T1.部署ＣＤ = @部署ＣＤ
                                    AND T1.コースＣＤ = @コースＣＤ
                                    AND NOT EXISTS (
                                        SELECT 'X'
                                        FROM
                                            (SELECT
                                                企業ＣＤ,
                                                部署ＣＤ,
                                                コースＣＤ,
                                                得意先ＣＤ,
                                                喫食日 AS 売上日
                                            FROM
                                                注文ヘッダ
                                            WHERE
                                                企業ＣＤ     = @企業ＣＤ
                                                AND 配送日   = @注文日
                                                AND 部署ＣＤ = @部署ＣＤ
                                                AND コースＣＤ = @コースＣＤ
                                            ) T2 -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                        WHERE
                                            T1.企業ＣＤ       = T2.企業ＣＤ
                                            AND T1.得意先ＣＤ = T2.得意先ＣＤ
                                            AND T1.売上日     = T2.売上日
                                            AND T1.部署ＣＤ   = T2.部署ＣＤ
                                            AND T1.コースＣＤ = T2.コースＣＤ)
                                ) AS H
                                INNER JOIN 売上明細 AS D
                                    ON D.企業ＣＤ    = H.企業ＣＤ
                                    AND D.売上日     = H.売上日
                                    AND D.得意先ＣＤ = H.得意先ＣＤ
                                    AND D.部署ＣＤ   = H.部署ＣＤ
                            ) AS KHD
                            ON H.企業ＣＤ = KHD.企業ＣＤ
                            AND H.コースＣＤ = KHD.コースＣＤ
                            AND D.喫食日 = KHD.売上日
                            AND D.得意先ＣＤ = KHD.得意先ＣＤ
                            AND D.部署ＣＤ = KHD.部署ＣＤ
                            AND D.商品ＣＤ = KHD.商品ＣＤ
                        INNER JOIN 商品マスタ AS I
                            ON I.企業ＣＤ = D.企業ＣＤ
                            AND ISNULL(I.無効フラグ, 0) = 0
                            AND I.商品ＣＤ = D.商品ＣＤ
                    WHERE
                        H.企業ＣＤ = @企業ＣＤ
                        AND H.部署ＣＤ = @部署ＣＤ
                        AND H.配送日 = @注文日
                        AND H.コースＣＤ = @コースＣＤ
                        AND ISNULL(I.弁当区分, 0) = @弁当区分主食   -- 弁当区分主食
                        AND KHD.明細番号 IS NULL
                    GROUP BY
                        H.得意先ＣＤ
        --                ,D.商品ＣＤ
                        ,I.主食ＣＤ
                    ) AS PART3
                GROUP BY
                    PART3.得意先ＣＤ
                    ,PART3.部品ＣＤ
                UNION ALL
                -- ②－４ 予測と注文の副食のみ
                SELECT
                    PART4.得意先ＣＤ
                    ,PART4.部品ＣＤ
                    ,SUM(ISNULL(PART4.商品注文数量, PART4.商品予測数量)) AS 商品注文予測数量
                    ,null AS 商品売上数量
                FROM
                    (SELECT
                        H.得意先ＣＤ
        --                ,D.商品ＣＤ
                        ,I.副食ＣＤ AS 部品ＣＤ
                        ,SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量
                        ,SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量
                    FROM
                        注文ヘッダ AS H
                        INNER JOIN 注文明細 AS D
                            ON D.企業ＣＤ    = H.企業ＣＤ
                            AND D.注文区分   = H.注文区分
                            AND D.喫食日     = H.喫食日
                            AND D.得意先ＣＤ = H.得意先ＣＤ
                            AND D.部署ＣＤ   = H.部署ＣＤ
                        -- 売上には存在しないこと（売上に存在する注文は取得しない）
                        -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                        LEFT JOIN
                            (SELECT
                               H.企業ＣＤ,
                               H.売上日,
                               H.得意先ＣＤ,
                               H.部署ＣＤ,
                               H.コースＣＤ,
                               D.商品ＣＤ,
                               D.明細番号
                            FROM
                                -- 喫食日売上ヘッダ
                                -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                (SELECT
                                    T1.企業ＣＤ,
                                    T1.売上日,
                                    T1.得意先ＣＤ,
                                    T1.部署ＣＤ,
                                    T1.コースＣＤ
                                FROM
                                    (SELECT
                                        企業ＣＤ,
                                        部署ＣＤ,
                                        コースＣＤ,
                                        得意先ＣＤ,
                                        喫食日 AS 売上日
                                    FROM
                                        注文ヘッダ
                                    WHERE
                                        企業ＣＤ     = @企業ＣＤ
                                        AND 配送日   = @注文日
                                        AND 部署ＣＤ = @部署ＣＤ
                                        AND コースＣＤ = @コースＣＤ
                                    ) T2 -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                    INNER JOIN 売上ヘッダ T1 ON
                                        T1.企業ＣＤ       = T2.企業ＣＤ
                                        AND T1.得意先ＣＤ = T2.得意先ＣＤ
                                        AND T1.売上日     = T2.売上日
                                        AND T1.部署ＣＤ   = T2.部署ＣＤ
                                        AND T1.コースＣＤ = T2.コースＣＤ
                                UNION
                                SELECT
                                    T1.企業ＣＤ,
                                    T1.売上日,
                                    T1.得意先ＣＤ,
                                    T1.部署ＣＤ,
                                    T1.コースＣＤ
                                FROM
                                    売上ヘッダ T1
                                WHERE
                                    T1.企業ＣＤ     = @企業ＣＤ
                                    AND T1.売上日   = @注文日
                                    AND T1.部署ＣＤ = @部署ＣＤ
                                    AND T1.コースＣＤ = @コースＣＤ
                                    AND NOT EXISTS (
                                        SELECT 'X'
                                        FROM
                                            (SELECT
                                                企業ＣＤ,
                                                部署ＣＤ,
                                                コースＣＤ,
                                                得意先ＣＤ,
                                                喫食日 AS 売上日
                                            FROM
                                                注文ヘッダ
                                            WHERE
                                                企業ＣＤ     = @企業ＣＤ
                                                AND 配送日   = @注文日
                                                AND 部署ＣＤ = @部署ＣＤ
                                                AND コースＣＤ = @コースＣＤ
                                            ) T2 -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                        WHERE
                                            T1.企業ＣＤ       = T2.企業ＣＤ
                                            AND T1.得意先ＣＤ = T2.得意先ＣＤ
                                            AND T1.売上日     = T2.売上日
                                            AND T1.部署ＣＤ   = T2.部署ＣＤ
                                            AND T1.コースＣＤ = T2.コースＣＤ)
                                ) AS H
                                INNER JOIN 売上明細 AS D
                                    ON D.企業ＣＤ    = H.企業ＣＤ
                                    AND D.売上日     = H.売上日
                                    AND D.得意先ＣＤ = H.得意先ＣＤ
                                    AND D.部署ＣＤ   = H.部署ＣＤ
                            ) AS KHD
                            ON H.企業ＣＤ = KHD.企業ＣＤ
                            AND H.コースＣＤ = KHD.コースＣＤ
                            AND D.喫食日 = KHD.売上日
                            AND D.得意先ＣＤ = KHD.得意先ＣＤ
                            AND D.部署ＣＤ = KHD.部署ＣＤ
                            AND D.商品ＣＤ = KHD.商品ＣＤ
                        INNER JOIN 商品マスタ AS I
                            ON I.企業ＣＤ = D.企業ＣＤ
                            AND ISNULL(I.無効フラグ, 0) = 0
                            AND I.商品ＣＤ = D.商品ＣＤ
                    WHERE
                        H.企業ＣＤ = @企業ＣＤ
                        AND H.部署ＣＤ = @部署ＣＤ
                        AND H.配送日 = @注文日
                        AND H.コースＣＤ = @コースＣＤ
                        AND ISNULL(I.弁当区分, 0) = @弁当区分副食   -- 弁当区分副食
                        AND KHD.明細番号 IS NULL
                    GROUP BY
                        H.得意先ＣＤ
        --                ,D.商品ＣＤ
                        ,I.副食ＣＤ
                    ) AS PART4
                GROUP BY
                    PART4.得意先ＣＤ
                    ,PART4.部品ＣＤ
                UNION ALL
                -- ②－５ 予測と注文の主食でも副食でもないもの
                SELECT
                    PART5.得意先ＣＤ
                    ,PART5.部品ＣＤ
                    ,SUM(ISNULL(PART5.商品注文数量, PART5.商品予測数量)) AS 商品注文予測数量
                    ,null AS 商品売上数量
                FROM
                    (SELECT
                        H.得意先ＣＤ
        --                ,D.商品ＣＤ
                        ,I.商品ＣＤ AS 部品ＣＤ
                        ,SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量
                        ,SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量
                    FROM
                        注文ヘッダ AS H
                        INNER JOIN 注文明細 AS D
                            ON D.企業ＣＤ    = H.企業ＣＤ
                            AND D.注文区分   = H.注文区分
                            AND D.喫食日     = H.喫食日
                            AND D.得意先ＣＤ = H.得意先ＣＤ
                            AND D.部署ＣＤ   = H.部署ＣＤ
                        -- 売上には存在しないこと（売上に存在する注文は取得しない）
                        -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                        LEFT JOIN
                            (SELECT
                               H.企業ＣＤ,
                               H.売上日,
                               H.得意先ＣＤ,
                               H.部署ＣＤ,
                               H.コースＣＤ,
                               D.商品ＣＤ,
                               D.明細番号
                            FROM
                                -- 喫食日売上ヘッダ
                                -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                (SELECT
                                    T1.企業ＣＤ,
                                    T1.売上日,
                                    T1.得意先ＣＤ,
                                    T1.部署ＣＤ,
                                    T1.コースＣＤ
                                FROM
                                    (SELECT
                                        企業ＣＤ,
                                        部署ＣＤ,
                                        コースＣＤ,
                                        得意先ＣＤ,
                                        喫食日 AS 売上日
                                    FROM
                                        注文ヘッダ
                                    WHERE
                                        企業ＣＤ     = @企業ＣＤ
                                        AND 配送日   = @注文日
                                        AND 部署ＣＤ = @部署ＣＤ
                                        AND コースＣＤ = @コースＣＤ
                                    ) T2 -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                    INNER JOIN 売上ヘッダ T1 ON
                                        T1.企業ＣＤ       = T2.企業ＣＤ
                                        AND T1.得意先ＣＤ = T2.得意先ＣＤ
                                        AND T1.売上日     = T2.売上日
                                        AND T1.部署ＣＤ   = T2.部署ＣＤ
                                        AND T1.コースＣＤ = T2.コースＣＤ
                                UNION
                                SELECT
                                    T1.企業ＣＤ,
                                    T1.売上日,
                                    T1.得意先ＣＤ,
                                    T1.部署ＣＤ,
                                    T1.コースＣＤ
                                FROM
                                    売上ヘッダ T1
                                WHERE
                                    T1.企業ＣＤ     = @企業ＣＤ
                                    AND T1.売上日   = @注文日
                                    AND T1.部署ＣＤ = @部署ＣＤ
                                    AND T1.コースＣＤ = @コースＣＤ
                                    AND NOT EXISTS (
                                        SELECT 'X'
                                        FROM
                                            (SELECT
                                                企業ＣＤ,
                                                部署ＣＤ,
                                                コースＣＤ,
                                                得意先ＣＤ,
                                                喫食日 AS 売上日
                                            FROM
                                                注文ヘッダ
                                            WHERE
                                                企業ＣＤ     = @企業ＣＤ
                                                AND 配送日   = @注文日
                                                AND 部署ＣＤ = @部署ＣＤ
                                                AND コースＣＤ = @コースＣＤ
                                            ) T2 -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                        WHERE
                                            T1.企業ＣＤ       = T2.企業ＣＤ
                                            AND T1.得意先ＣＤ = T2.得意先ＣＤ
                                            AND T1.売上日     = T2.売上日
                                            AND T1.部署ＣＤ   = T2.部署ＣＤ
                                            AND T1.コースＣＤ = T2.コースＣＤ)
                                ) AS H
                                INNER JOIN 売上明細 AS D
                                    ON D.企業ＣＤ    = H.企業ＣＤ
                                    AND D.売上日     = H.売上日
                                    AND D.得意先ＣＤ = H.得意先ＣＤ
                                    AND D.部署ＣＤ   = H.部署ＣＤ
                            ) AS KHD
                            ON H.企業ＣＤ = KHD.企業ＣＤ
                            AND H.コースＣＤ = KHD.コースＣＤ
                            AND D.喫食日 = KHD.売上日
                            AND D.得意先ＣＤ = KHD.得意先ＣＤ
                            AND D.部署ＣＤ = KHD.部署ＣＤ
                            AND D.商品ＣＤ = KHD.商品ＣＤ
                        INNER JOIN 商品マスタ AS I
                            ON I.企業ＣＤ = D.企業ＣＤ
                            AND ISNULL(I.無効フラグ, 0) = 0
                            AND I.商品ＣＤ = D.商品ＣＤ
                    WHERE
                        H.企業ＣＤ = @企業ＣＤ
                        AND H.部署ＣＤ = @部署ＣＤ
                        AND H.配送日 = @注文日
                        AND H.コースＣＤ = @コースＣＤ
                        AND ISNULL(I.主食ＣＤ, 0) = 0
                        AND ISNULL(I.副食ＣＤ, 0) = 0
                        AND KHD.明細番号 IS NULL
                    GROUP BY
                        H.得意先ＣＤ
        --                ,D.商品ＣＤ
                        ,I.商品ＣＤ
                    ) AS PART5
                GROUP BY
                    PART5.得意先ＣＤ
                    ,PART5.部品ＣＤ
                UNION ALL
                -- ②－６ 予測と注文のその他の主食
                SELECT
                    PART6.得意先ＣＤ
                    ,PART6.部品ＣＤ
                    ,SUM(ISNULL(PART6.商品注文数量, PART6.商品予測数量)) AS 商品注文予測数量
                    ,null AS 商品売上数量
                FROM
                    (SELECT
                        H.得意先ＣＤ
        --                ,D.商品ＣＤ
                        ,I.主食ＣＤ AS 部品ＣＤ
                        ,SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量
                        ,SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量
                    FROM
                        注文ヘッダ AS H
                        INNER JOIN 注文明細 AS D
                            ON D.企業ＣＤ    = H.企業ＣＤ
                            AND D.注文区分   = H.注文区分
                            AND D.喫食日     = H.喫食日
                            AND D.得意先ＣＤ = H.得意先ＣＤ
                            AND D.部署ＣＤ   = H.部署ＣＤ
                        -- 売上には存在しないこと（売上に存在する注文は取得しない）
                        -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                        LEFT JOIN
                            (SELECT
                               H.企業ＣＤ,
                               H.売上日,
                               H.得意先ＣＤ,
                               H.部署ＣＤ,
                               H.コースＣＤ,
                               D.商品ＣＤ,
                               D.明細番号
                            FROM
                                -- 喫食日売上ヘッダ
                                -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                (SELECT
                                    T1.企業ＣＤ,
                                    T1.売上日,
                                    T1.得意先ＣＤ,
                                    T1.部署ＣＤ,
                                    T1.コースＣＤ
                                FROM
                                    (SELECT
                                        企業ＣＤ,
                                        部署ＣＤ,
                                        コースＣＤ,
                                        得意先ＣＤ,
                                        喫食日 AS 売上日
                                    FROM
                                        注文ヘッダ
                                    WHERE
                                        企業ＣＤ     = @企業ＣＤ
                                        AND 配送日   = @注文日
                                        AND 部署ＣＤ = @部署ＣＤ
                                        AND コースＣＤ = @コースＣＤ
                                    ) T2 -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                    INNER JOIN 売上ヘッダ T1 ON
                                        T1.企業ＣＤ       = T2.企業ＣＤ
                                        AND T1.得意先ＣＤ = T2.得意先ＣＤ
                                        AND T1.売上日     = T2.売上日
                                        AND T1.部署ＣＤ   = T2.部署ＣＤ
                                        AND T1.コースＣＤ = T2.コースＣＤ
                                UNION
                                SELECT
                                    T1.企業ＣＤ,
                                    T1.売上日,
                                    T1.得意先ＣＤ,
                                    T1.部署ＣＤ,
                                    T1.コースＣＤ
                                FROM
                                    売上ヘッダ T1
                                WHERE
                                    T1.企業ＣＤ     = @企業ＣＤ
                                    AND T1.売上日   = @注文日
                                    AND T1.部署ＣＤ = @部署ＣＤ
                                    AND T1.コースＣＤ = @コースＣＤ
                                    AND NOT EXISTS (
                                        SELECT 'X'
                                        FROM
                                            (SELECT
                                                企業ＣＤ,
                                                部署ＣＤ,
                                                コースＣＤ,
                                                得意先ＣＤ,
                                                喫食日 AS 売上日
                                            FROM
                                                注文ヘッダ
                                            WHERE
                                                企業ＣＤ     = @企業ＣＤ
                                                AND 配送日   = @注文日
                                                AND 部署ＣＤ = @部署ＣＤ
                                                AND コースＣＤ = @コースＣＤ
                                            ) T2 -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                        WHERE
                                            T1.企業ＣＤ       = T2.企業ＣＤ
                                            AND T1.得意先ＣＤ = T2.得意先ＣＤ
                                            AND T1.売上日     = T2.売上日
                                            AND T1.部署ＣＤ   = T2.部署ＣＤ
                                            AND T1.コースＣＤ = T2.コースＣＤ)
                                ) AS H
                                INNER JOIN 売上明細 AS D
                                    ON D.企業ＣＤ    = H.企業ＣＤ
                                    AND D.売上日     = H.売上日
                                    AND D.得意先ＣＤ = H.得意先ＣＤ
                                    AND D.部署ＣＤ   = H.部署ＣＤ
                            ) AS KHD
                            ON H.企業ＣＤ = KHD.企業ＣＤ
                            AND H.コースＣＤ = KHD.コースＣＤ
                            AND D.喫食日 = KHD.売上日
                            AND D.得意先ＣＤ = KHD.得意先ＣＤ
                            AND D.部署ＣＤ = KHD.部署ＣＤ
                            AND D.商品ＣＤ = KHD.商品ＣＤ
                        INNER JOIN 商品マスタ AS I
                            ON I.企業ＣＤ = D.企業ＣＤ
                            AND ISNULL(I.無効フラグ, 0) = 0
                            AND I.商品ＣＤ = D.商品ＣＤ
                    WHERE
                        H.企業ＣＤ = @企業ＣＤ
                        AND H.部署ＣＤ = @部署ＣＤ
                        AND H.配送日 = @注文日
                        AND H.コースＣＤ = @コースＣＤ
                        AND ISNULL(I.弁当区分, 0) IN (@弁当区分チケット, @弁当区分その他) -- チケットとその他
                        AND ISNULL(I.主食ＣＤ, 0) <> 0              -- 主食がある
                        AND ISNULL(I.主食ＣＤ, 0) <> I.商品ＣＤ     -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                        AND KHD.明細番号 IS NULL
                    GROUP BY
                        H.得意先ＣＤ
        --                ,D.商品ＣＤ
                        ,I.主食ＣＤ
                    ) AS PART6
                GROUP BY
                    PART6.得意先ＣＤ
                    ,PART6.部品ＣＤ
                UNION ALL
                -- ②－７ 予測と注文のその他の副食
                SELECT
                    PART7.得意先ＣＤ
                    ,PART7.部品ＣＤ
                    ,SUM(ISNULL(PART7.商品注文数量, PART7.商品予測数量)) AS 商品注文予測数量
                    ,null AS 商品売上数量
                FROM
                    (SELECT
                        H.得意先ＣＤ
        --                ,D.商品ＣＤ
                        ,I.副食ＣＤ AS 部品ＣＤ
                        ,SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量
                        ,SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量
                    FROM
                        注文ヘッダ AS H
                        INNER JOIN 注文明細 AS D
                            ON D.企業ＣＤ    = H.企業ＣＤ
                            AND D.注文区分   = H.注文区分
                            AND D.喫食日     = H.喫食日
                            AND D.得意先ＣＤ = H.得意先ＣＤ
                            AND D.部署ＣＤ   = H.部署ＣＤ
                        -- 売上には存在しないこと（売上に存在する注文は取得しない）
                        -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                        LEFT JOIN
                            (SELECT
                               H.企業ＣＤ,
                               H.売上日,
                               H.得意先ＣＤ,
                               H.部署ＣＤ,
                               H.コースＣＤ,
                               D.商品ＣＤ,
                               D.明細番号
                            FROM
                                -- 喫食日売上ヘッダ
                                -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                (SELECT
                                    T1.企業ＣＤ,
                                    T1.売上日,
                                    T1.得意先ＣＤ,
                                    T1.部署ＣＤ,
                                    T1.コースＣＤ
                                FROM
                                    (SELECT
                                        企業ＣＤ,
                                        部署ＣＤ,
                                        コースＣＤ,
                                        得意先ＣＤ,
                                        喫食日 AS 売上日
                                    FROM
                                        注文ヘッダ
                                    WHERE
                                        企業ＣＤ     = @企業ＣＤ
                                        AND 配送日   = @注文日
                                        AND 部署ＣＤ = @部署ＣＤ
                                        AND コースＣＤ = @コースＣＤ
                                    ) T2 -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                    INNER JOIN 売上ヘッダ T1 ON
                                        T1.企業ＣＤ       = T2.企業ＣＤ
                                        AND T1.得意先ＣＤ = T2.得意先ＣＤ
                                        AND T1.売上日     = T2.売上日
                                        AND T1.部署ＣＤ   = T2.部署ＣＤ
                                        AND T1.コースＣＤ = T2.コースＣＤ
                                UNION
                                SELECT
                                    T1.企業ＣＤ,
                                    T1.売上日,
                                    T1.得意先ＣＤ,
                                    T1.部署ＣＤ,
                                    T1.コースＣＤ
                                FROM
                                    売上ヘッダ T1
                                WHERE
                                    T1.企業ＣＤ     = @企業ＣＤ
                                    AND T1.売上日   = @注文日
                                    AND T1.部署ＣＤ = @部署ＣＤ
                                    AND T1.コースＣＤ = @コースＣＤ
                                    AND NOT EXISTS (
                                        SELECT 'X'
                                        FROM
                                            (SELECT
                                                企業ＣＤ,
                                                部署ＣＤ,
                                                コースＣＤ,
                                                得意先ＣＤ,
                                                喫食日 AS 売上日
                                            FROM
                                                注文ヘッダ
                                            WHERE
                                                企業ＣＤ     = @企業ＣＤ
                                                AND 配送日   = @注文日
                                                AND 部署ＣＤ = @部署ＣＤ
                                                AND コースＣＤ = @コースＣＤ
                                            ) T2 -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                        WHERE
                                            T1.企業ＣＤ       = T2.企業ＣＤ
                                            AND T1.得意先ＣＤ = T2.得意先ＣＤ
                                            AND T1.売上日     = T2.売上日
                                            AND T1.部署ＣＤ   = T2.部署ＣＤ
                                            AND T1.コースＣＤ = T2.コースＣＤ)
                                ) AS H
                                INNER JOIN 売上明細 AS D
                                    ON D.企業ＣＤ    = H.企業ＣＤ
                                    AND D.売上日     = H.売上日
                                    AND D.得意先ＣＤ = H.得意先ＣＤ
                                    AND D.部署ＣＤ   = H.部署ＣＤ
                            ) AS KHD
                            ON H.企業ＣＤ = KHD.企業ＣＤ
                            AND H.コースＣＤ = KHD.コースＣＤ
                            AND D.喫食日 = KHD.売上日
                            AND D.得意先ＣＤ = KHD.得意先ＣＤ
                            AND D.部署ＣＤ = KHD.部署ＣＤ
                            AND D.商品ＣＤ = KHD.商品ＣＤ
                        INNER JOIN 商品マスタ AS I
                            ON I.企業ＣＤ = D.企業ＣＤ
                            AND ISNULL(I.無効フラグ, 0) = 0
                            AND I.商品ＣＤ = D.商品ＣＤ
                    WHERE
                        H.企業ＣＤ = @企業ＣＤ
                        AND H.部署ＣＤ = @部署ＣＤ
                        AND H.配送日 = @注文日
                        AND H.コースＣＤ = @コースＣＤ
                        AND ISNULL(I.弁当区分, 0) IN (@弁当区分チケット, @弁当区分その他) -- チケットとその他
                        AND ISNULL(I.副食ＣＤ, 0) <> 0              -- 副食がある
                        AND ISNULL(I.副食ＣＤ, 0) <> I.商品ＣＤ     -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                        AND KHD.明細番号 IS NULL
                    GROUP BY
                        H.得意先ＣＤ
        --                ,D.商品ＣＤ
                        ,I.副食ＣＤ
                    ) AS PART7
                GROUP BY
                    PART7.得意先ＣＤ
                    ,PART7.部品ＣＤ
                UNION ALL
                -- ②－８ 売上の弁当の主食
                SELECT
                    H.得意先ＣＤ
                    ,D.主食ＣＤ AS 部品ＣＤ
                    ,null AS 商品注文予測数量
                    ,SUM(D.商品数量) AS 商品売上数量
                FROM
                    -- 喫食日売上ヘッダ
                    -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    (SELECT
                        T1.企業ＣＤ,
                        T1.売上日,
                        T1.得意先ＣＤ,
                        T1.部署ＣＤ
                    FROM
                        (SELECT
                            企業ＣＤ,
                            部署ＣＤ,
                            コースＣＤ,
                            得意先ＣＤ,
                            喫食日 AS 売上日
                        FROM
                            注文ヘッダ
                        WHERE
                            企業ＣＤ     = @企業ＣＤ
                            AND 配送日   = @注文日
                            AND 部署ＣＤ = @部署ＣＤ
                            AND コースＣＤ = @コースＣＤ
                        ) T2 -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                        INNER JOIN 売上ヘッダ T1 ON
                            T1.企業ＣＤ       = T2.企業ＣＤ
                            AND T1.得意先ＣＤ = T2.得意先ＣＤ
                            AND T1.売上日     = T2.売上日
                            AND T1.部署ＣＤ   = T2.部署ＣＤ
                            AND T1.コースＣＤ = T2.コースＣＤ
                    UNION
                    SELECT
                        T1.企業ＣＤ,
                        T1.売上日,
                        T1.得意先ＣＤ,
                        T1.部署ＣＤ
                    FROM
                        売上ヘッダ T1
                    WHERE
                        T1.企業ＣＤ     = @企業ＣＤ
                        AND T1.売上日   = @注文日
                        AND T1.部署ＣＤ = @部署ＣＤ
                        AND T1.コースＣＤ = @コースＣＤ
                        AND NOT EXISTS (
                            SELECT 'X'
                            FROM
                                (SELECT
                                    企業ＣＤ,
                                    部署ＣＤ,
                                    コースＣＤ,
                                    得意先ＣＤ,
                                    喫食日 AS 売上日
                                FROM
                                    注文ヘッダ
                                WHERE
                                    企業ＣＤ     = @企業ＣＤ
                                    AND 配送日   = @注文日
                                    AND 部署ＣＤ = @部署ＣＤ
                                    AND コースＣＤ = @コースＣＤ
                                ) T2 -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                            WHERE
                                T1.企業ＣＤ       = T2.企業ＣＤ
                                AND T1.得意先ＣＤ = T2.得意先ＣＤ
                                AND T1.売上日     = T2.売上日
                                AND T1.部署ＣＤ   = T2.部署ＣＤ
                                AND T1.コースＣＤ = T2.コースＣＤ)
                    ) AS H
                    INNER JOIN 売上明細 AS D
                        ON D.企業ＣＤ    = H.企業ＣＤ
                        AND D.売上日     = H.売上日
                        AND D.得意先ＣＤ = H.得意先ＣＤ
                        AND D.部署ＣＤ   = H.部署ＣＤ
                    INNER JOIN 商品マスタ AS I
                        ON I.企業ＣＤ = D.企業ＣＤ
                        AND ISNULL(I.無効フラグ, 0) = 0
                        AND I.商品ＣＤ = D.商品ＣＤ
                WHERE
                    ISNULL(I.弁当区分, 0) = @弁当区分弁当   -- 弁当
                    AND ISNULL(D.主食ＣＤ, 0) <> 0          -- 主食がある
                    AND ISNULL(D.主食ＣＤ, 0) <> D.商品ＣＤ -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                GROUP BY
                    H.得意先ＣＤ,
                    D.主食ＣＤ
                UNION ALL
                -- ②－９ 売上の弁当の副食
                SELECT
                    H.得意先ＣＤ
                    ,D.副食ＣＤ AS 部品ＣＤ
                    ,null AS 商品注文予測数量
                    ,SUM(D.商品数量) AS 商品売上数量
                FROM
                    -- 喫食日売上ヘッダ
                    -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    (SELECT
                        T1.企業ＣＤ,
                        T1.売上日,
                        T1.得意先ＣＤ,
                        T1.部署ＣＤ
                    FROM
                        (SELECT
                            企業ＣＤ,
                            部署ＣＤ,
                            コースＣＤ,
                            得意先ＣＤ,
                            喫食日 AS 売上日
                        FROM
                            注文ヘッダ
                        WHERE
                            企業ＣＤ     = @企業ＣＤ
                            AND 配送日   = @注文日
                            AND 部署ＣＤ = @部署ＣＤ
                            AND コースＣＤ = @コースＣＤ
                        ) T2 -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                        INNER JOIN 売上ヘッダ T1 ON
                            T1.企業ＣＤ       = T2.企業ＣＤ
                            AND T1.得意先ＣＤ = T2.得意先ＣＤ
                            AND T1.売上日     = T2.売上日
                            AND T1.部署ＣＤ   = T2.部署ＣＤ
                            AND T1.コースＣＤ = T2.コースＣＤ
                    UNION
                    SELECT
                        T1.企業ＣＤ,
                        T1.売上日,
                        T1.得意先ＣＤ,
                        T1.部署ＣＤ
                    FROM
                        売上ヘッダ T1
                    WHERE
                        T1.企業ＣＤ     = @企業ＣＤ
                        AND T1.売上日   = @注文日
                        AND T1.部署ＣＤ = @部署ＣＤ
                        AND T1.コースＣＤ = @コースＣＤ
                        AND NOT EXISTS (
                            SELECT 'X'
                            FROM
                                (SELECT
                                    企業ＣＤ,
                                    部署ＣＤ,
                                    コースＣＤ,
                                    得意先ＣＤ,
                                    喫食日 AS 売上日
                                FROM
                                    注文ヘッダ
                                WHERE
                                    企業ＣＤ     = @企業ＣＤ
                                    AND 配送日   = @注文日
                                    AND 部署ＣＤ = @部署ＣＤ
                                    AND コースＣＤ = @コースＣＤ
                                ) T2 -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                            WHERE
                                T1.企業ＣＤ       = T2.企業ＣＤ
                                AND T1.得意先ＣＤ = T2.得意先ＣＤ
                                AND T1.売上日     = T2.売上日
                                AND T1.部署ＣＤ   = T2.部署ＣＤ
                                AND T1.コースＣＤ = T2.コースＣＤ)
                    ) AS H
                    INNER JOIN 売上明細 AS D
                        ON D.企業ＣＤ    = H.企業ＣＤ
                        AND D.売上日     = H.売上日
                        AND D.得意先ＣＤ = H.得意先ＣＤ
                        AND D.部署ＣＤ   = H.部署ＣＤ
                    INNER JOIN 商品マスタ AS I
                        ON I.企業ＣＤ = D.企業ＣＤ
                        AND ISNULL(I.無効フラグ, 0) = 0
                        AND I.商品ＣＤ = D.商品ＣＤ
                WHERE
                    ISNULL(I.弁当区分, 0) = @弁当区分弁当   -- 弁当
                    AND ISNULL(D.副食ＣＤ, 0) <> 0          -- 副食がある
                    AND ISNULL(D.副食ＣＤ, 0) <> D.商品ＣＤ -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                GROUP BY
                    H.得意先ＣＤ,
                    D.副食ＣＤ
                UNION ALL
                -- ②－１０ 売上の主食のみ
                SELECT
                    H.得意先ＣＤ
                    ,D.主食ＣＤ AS 部品ＣＤ
                    ,null AS 商品注文予測数量
                    ,SUM(D.商品数量) AS 商品売上数量
                FROM
                    -- 喫食日売上ヘッダ
                    -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    (SELECT
                        T1.企業ＣＤ,
                        T1.売上日,
                        T1.得意先ＣＤ,
                        T1.部署ＣＤ
                    FROM
                        (SELECT
                            企業ＣＤ,
                            部署ＣＤ,
                            コースＣＤ,
                            得意先ＣＤ,
                            喫食日 AS 売上日
                        FROM
                            注文ヘッダ
                        WHERE
                            企業ＣＤ     = @企業ＣＤ
                            AND 配送日   = @注文日
                            AND 部署ＣＤ = @部署ＣＤ
                            AND コースＣＤ = @コースＣＤ
                        ) T2 -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                        INNER JOIN 売上ヘッダ T1 ON
                            T1.企業ＣＤ       = T2.企業ＣＤ
                            AND T1.得意先ＣＤ = T2.得意先ＣＤ
                            AND T1.売上日     = T2.売上日
                            AND T1.部署ＣＤ   = T2.部署ＣＤ
                            AND T1.コースＣＤ = T2.コースＣＤ
                    UNION
                    SELECT
                        T1.企業ＣＤ,
                        T1.売上日,
                        T1.得意先ＣＤ,
                        T1.部署ＣＤ
                    FROM
                        売上ヘッダ T1
                    WHERE
                        T1.企業ＣＤ     = @企業ＣＤ
                        AND T1.売上日   = @注文日
                        AND T1.部署ＣＤ = @部署ＣＤ
                        AND T1.コースＣＤ = @コースＣＤ
                        AND NOT EXISTS (
                            SELECT 'X'
                            FROM
                                (SELECT
                                    企業ＣＤ,
                                    部署ＣＤ,
                                    コースＣＤ,
                                    得意先ＣＤ,
                                    喫食日 AS 売上日
                                FROM
                                    注文ヘッダ
                                WHERE
                                    企業ＣＤ     = @企業ＣＤ
                                    AND 配送日   = @注文日
                                    AND 部署ＣＤ = @部署ＣＤ
                                    AND コースＣＤ = @コースＣＤ
                                ) T2 -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                            WHERE
                                T1.企業ＣＤ       = T2.企業ＣＤ
                                AND T1.得意先ＣＤ = T2.得意先ＣＤ
                                AND T1.売上日     = T2.売上日
                                AND T1.部署ＣＤ   = T2.部署ＣＤ
                                AND T1.コースＣＤ = T2.コースＣＤ)
                    ) AS H
                    INNER JOIN 売上明細 AS D
                        ON D.企業ＣＤ    = H.企業ＣＤ
                        AND D.売上日     = H.売上日
                        AND D.得意先ＣＤ = H.得意先ＣＤ
                        AND D.部署ＣＤ   = H.部署ＣＤ
                    INNER JOIN 商品マスタ AS I
                        ON I.企業ＣＤ = D.企業ＣＤ
                        AND ISNULL(I.無効フラグ, 0) = 0
                        AND I.商品ＣＤ = D.商品ＣＤ
                WHERE
                    ISNULL(I.弁当区分, 0) = @弁当区分主食
                GROUP BY
                    H.得意先ＣＤ,
                    D.主食ＣＤ
                UNION ALL
                -- ②－１１ 売上の副食のみ
                SELECT
                    H.得意先ＣＤ
                    ,D.副食ＣＤ AS 部品ＣＤ
                    ,null AS 商品注文予測数量
                    ,SUM(D.商品数量) AS 商品売上数量
                FROM
                    -- 喫食日売上ヘッダ
                    -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    (SELECT
                        T1.企業ＣＤ,
                        T1.売上日,
                        T1.得意先ＣＤ,
                        T1.部署ＣＤ
                    FROM
                        (SELECT
                            企業ＣＤ,
                            部署ＣＤ,
                            コースＣＤ,
                            得意先ＣＤ,
                            喫食日 AS 売上日
                        FROM
                            注文ヘッダ
                        WHERE
                            企業ＣＤ     = @企業ＣＤ
                            AND 配送日   = @注文日
                            AND 部署ＣＤ = @部署ＣＤ
                            AND コースＣＤ = @コースＣＤ
                        ) T2 -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                        INNER JOIN 売上ヘッダ T1 ON
                            T1.企業ＣＤ       = T2.企業ＣＤ
                            AND T1.得意先ＣＤ = T2.得意先ＣＤ
                            AND T1.売上日     = T2.売上日
                            AND T1.部署ＣＤ   = T2.部署ＣＤ
                            AND T1.コースＣＤ = T2.コースＣＤ
                    UNION
                    SELECT
                        T1.企業ＣＤ,
                        T1.売上日,
                        T1.得意先ＣＤ,
                        T1.部署ＣＤ
                    FROM
                        売上ヘッダ T1
                    WHERE
                        T1.企業ＣＤ     = @企業ＣＤ
                        AND T1.売上日   = @注文日
                        AND T1.部署ＣＤ = @部署ＣＤ
                        AND T1.コースＣＤ = @コースＣＤ
                        AND NOT EXISTS (
                            SELECT 'X'
                            FROM
                                (SELECT
                                    企業ＣＤ,
                                    部署ＣＤ,
                                    コースＣＤ,
                                    得意先ＣＤ,
                                    喫食日 AS 売上日
                                FROM
                                    注文ヘッダ
                                WHERE
                                    企業ＣＤ     = @企業ＣＤ
                                    AND 配送日   = @注文日
                                    AND 部署ＣＤ = @部署ＣＤ
                                    AND コースＣＤ = @コースＣＤ
                                ) T2 -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                            WHERE
                                T1.企業ＣＤ       = T2.企業ＣＤ
                                AND T1.得意先ＣＤ = T2.得意先ＣＤ
                                AND T1.売上日     = T2.売上日
                                AND T1.部署ＣＤ   = T2.部署ＣＤ
                                AND T1.コースＣＤ = T2.コースＣＤ)
                    ) AS H
                    INNER JOIN 売上明細 AS D
                        ON D.企業ＣＤ    = H.企業ＣＤ
                        AND D.売上日     = H.売上日
                        AND D.得意先ＣＤ = H.得意先ＣＤ
                        AND D.部署ＣＤ   = H.部署ＣＤ
                    INNER JOIN 商品マスタ AS I
                        ON I.企業ＣＤ = D.企業ＣＤ
                        AND ISNULL(I.無効フラグ, 0) = 0
                        AND I.商品ＣＤ = D.商品ＣＤ
                WHERE
                    ISNULL(I.弁当区分, 0) = @弁当区分副食
                GROUP BY
                    H.得意先ＣＤ,
                    D.副食ＣＤ
                UNION ALL
                -- ②－１２ 売上の主食でも副食でもないもの
                SELECT
                    H.得意先ＣＤ
                    ,D.商品ＣＤ AS 部品ＣＤ
                    ,null AS 商品注文予測数量
                    ,SUM(D.商品数量) AS 商品売上数量
                FROM
                    -- 喫食日売上ヘッダ
                    -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    (SELECT
                        T1.企業ＣＤ,
                        T1.売上日,
                        T1.得意先ＣＤ,
                        T1.部署ＣＤ
                    FROM
                        (SELECT
                            企業ＣＤ,
                            部署ＣＤ,
                            コースＣＤ,
                            得意先ＣＤ,
                            喫食日 AS 売上日
                        FROM
                            注文ヘッダ
                        WHERE
                            企業ＣＤ     = @企業ＣＤ
                            AND 配送日   = @注文日
                            AND 部署ＣＤ = @部署ＣＤ
                            AND コースＣＤ = @コースＣＤ
                        ) T2 -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                        INNER JOIN 売上ヘッダ T1 ON
                            T1.企業ＣＤ       = T2.企業ＣＤ
                            AND T1.得意先ＣＤ = T2.得意先ＣＤ
                            AND T1.売上日     = T2.売上日
                            AND T1.部署ＣＤ   = T2.部署ＣＤ
                            AND T1.コースＣＤ = T2.コースＣＤ
                    UNION
                    SELECT
                        T1.企業ＣＤ,
                        T1.売上日,
                        T1.得意先ＣＤ,
                        T1.部署ＣＤ
                    FROM
                        売上ヘッダ T1
                    WHERE
                        T1.企業ＣＤ     = @企業ＣＤ
                        AND T1.売上日   = @注文日
                        AND T1.部署ＣＤ = @部署ＣＤ
                        AND T1.コースＣＤ = @コースＣＤ
                        AND NOT EXISTS (
                            SELECT 'X'
                            FROM
                                (SELECT
                                    企業ＣＤ,
                                    部署ＣＤ,
                                    コースＣＤ,
                                    得意先ＣＤ,
                                    喫食日 AS 売上日
                                FROM
                                    注文ヘッダ
                                WHERE
                                    企業ＣＤ     = @企業ＣＤ
                                    AND 配送日   = @注文日
                                    AND 部署ＣＤ = @部署ＣＤ
                                    AND コースＣＤ = @コースＣＤ
                                ) T2 -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                            WHERE
                                T1.企業ＣＤ       = T2.企業ＣＤ
                                AND T1.得意先ＣＤ = T2.得意先ＣＤ
                                AND T1.売上日     = T2.売上日
                                AND T1.部署ＣＤ   = T2.部署ＣＤ
                                AND T1.コースＣＤ = T2.コースＣＤ)
                    ) AS H
                    INNER JOIN 売上明細 AS D
                        ON D.企業ＣＤ    = H.企業ＣＤ
                        AND D.売上日     = H.売上日
                        AND D.得意先ＣＤ = H.得意先ＣＤ
                        AND D.部署ＣＤ   = H.部署ＣＤ
                WHERE
                    ISNULL(D.主食ＣＤ, 0) = 0
                    AND ISNULL(D.副食ＣＤ, 0) = 0
                GROUP BY
                    H.得意先ＣＤ,
                    D.商品ＣＤ
                UNION ALL
                -- ②－１３ 売上のその他の主食
                SELECT
                    H.得意先ＣＤ
                    ,D.主食ＣＤ AS 部品ＣＤ
                    ,null AS 商品注文予測数量
                    ,SUM(D.商品数量) AS 商品売上数量
                FROM
                    -- 喫食日売上ヘッダ
                    -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    (SELECT
                        T1.企業ＣＤ,
                        T1.売上日,
                        T1.得意先ＣＤ,
                        T1.部署ＣＤ
                    FROM
                        (SELECT
                            企業ＣＤ,
                            部署ＣＤ,
                            コースＣＤ,
                            得意先ＣＤ,
                            喫食日 AS 売上日
                        FROM
                            注文ヘッダ
                        WHERE
                            企業ＣＤ     = @企業ＣＤ
                            AND 配送日   = @注文日
                            AND 部署ＣＤ = @部署ＣＤ
                            AND コースＣＤ = @コースＣＤ
                        ) T2 -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                        INNER JOIN 売上ヘッダ T1 ON
                            T1.企業ＣＤ       = T2.企業ＣＤ
                            AND T1.得意先ＣＤ = T2.得意先ＣＤ
                            AND T1.売上日     = T2.売上日
                            AND T1.部署ＣＤ   = T2.部署ＣＤ
                            AND T1.コースＣＤ = T2.コースＣＤ
                    UNION
                    SELECT
                        T1.企業ＣＤ,
                        T1.売上日,
                        T1.得意先ＣＤ,
                        T1.部署ＣＤ
                    FROM
                        売上ヘッダ T1
                    WHERE
                        T1.企業ＣＤ     = @企業ＣＤ
                        AND T1.売上日   = @注文日
                        AND T1.部署ＣＤ = @部署ＣＤ
                        AND T1.コースＣＤ = @コースＣＤ
                        AND NOT EXISTS (
                            SELECT 'X'
                            FROM
                                (SELECT
                                    企業ＣＤ,
                                    部署ＣＤ,
                                    コースＣＤ,
                                    得意先ＣＤ,
                                    喫食日 AS 売上日
                                FROM
                                    注文ヘッダ
                                WHERE
                                    企業ＣＤ     = @企業ＣＤ
                                    AND 配送日   = @注文日
                                    AND 部署ＣＤ = @部署ＣＤ
                                    AND コースＣＤ = @コースＣＤ
                                ) T2 -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                            WHERE
                                T1.企業ＣＤ       = T2.企業ＣＤ
                                AND T1.得意先ＣＤ = T2.得意先ＣＤ
                                AND T1.売上日     = T2.売上日
                                AND T1.部署ＣＤ   = T2.部署ＣＤ
                                AND T1.コースＣＤ = T2.コースＣＤ)
                    ) AS H
                    INNER JOIN 売上明細 AS D
                        ON D.企業ＣＤ    = H.企業ＣＤ
                        AND D.売上日     = H.売上日
                        AND D.得意先ＣＤ = H.得意先ＣＤ
                        AND D.部署ＣＤ   = H.部署ＣＤ
                    INNER JOIN 商品マスタ AS I
                        ON I.企業ＣＤ = D.企業ＣＤ
                        AND ISNULL(I.無効フラグ, 0) = 0
                        AND I.商品ＣＤ = D.商品ＣＤ
                WHERE
                    ISNULL(I.弁当区分, 0) IN (@弁当区分チケット, @弁当区分その他) -- チケットとその他
                    AND ISNULL(D.主食ＣＤ, 0) <> 0          -- 主食がある
                    AND ISNULL(D.主食ＣＤ, 0) <> D.商品ＣＤ -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                GROUP BY
                    H.得意先ＣＤ,
                    D.主食ＣＤ
                UNION ALL
                -- ②－１４ 売上のその他の副食
                SELECT
                    H.得意先ＣＤ
                    ,D.副食ＣＤ AS 部品ＣＤ
                    ,null AS 商品注文予測数量
                    ,SUM(D.商品数量) AS 商品売上数量
                FROM
                    -- 喫食日売上ヘッダ
                    -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    (SELECT
                        T1.企業ＣＤ,
                        T1.売上日,
                        T1.得意先ＣＤ,
                        T1.部署ＣＤ
                    FROM
                        (SELECT
                            企業ＣＤ,
                            部署ＣＤ,
                            コースＣＤ,
                            得意先ＣＤ,
                            喫食日 AS 売上日
                        FROM
                            注文ヘッダ
                        WHERE
                            企業ＣＤ     = @企業ＣＤ
                            AND 配送日   = @注文日
                            AND 部署ＣＤ = @部署ＣＤ
                            AND コースＣＤ = @コースＣＤ
                        ) T2 -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                        INNER JOIN 売上ヘッダ T1 ON
                            T1.企業ＣＤ       = T2.企業ＣＤ
                            AND T1.得意先ＣＤ = T2.得意先ＣＤ
                            AND T1.売上日     = T2.売上日
                            AND T1.部署ＣＤ   = T2.部署ＣＤ
                            AND T1.コースＣＤ = T2.コースＣＤ
                    UNION
                    SELECT
                        T1.企業ＣＤ,
                        T1.売上日,
                        T1.得意先ＣＤ,
                        T1.部署ＣＤ
                    FROM
                        売上ヘッダ T1
                    WHERE
                        T1.企業ＣＤ     = @企業ＣＤ
                        AND T1.売上日   = @注文日
                        AND T1.部署ＣＤ = @部署ＣＤ
                        AND T1.コースＣＤ = @コースＣＤ
                        AND NOT EXISTS (
                            SELECT 'X'
                            FROM
                                (SELECT
                                    企業ＣＤ,
                                    部署ＣＤ,
                                    コースＣＤ,
                                    得意先ＣＤ,
                                    喫食日 AS 売上日
                                FROM
                                    注文ヘッダ
                                WHERE
                                    企業ＣＤ     = @企業ＣＤ
                                    AND 配送日   = @注文日
                                    AND 部署ＣＤ = @部署ＣＤ
                                    AND コースＣＤ = @コースＣＤ
                                ) T2 -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                            WHERE
                                T1.企業ＣＤ       = T2.企業ＣＤ
                                AND T1.得意先ＣＤ = T2.得意先ＣＤ
                                AND T1.売上日     = T2.売上日
                                AND T1.部署ＣＤ   = T2.部署ＣＤ
                                AND T1.コースＣＤ = T2.コースＣＤ)
                    ) AS H
                    INNER JOIN 売上明細 AS D
                        ON D.企業ＣＤ    = H.企業ＣＤ
                        AND D.売上日     = H.売上日
                        AND D.得意先ＣＤ = H.得意先ＣＤ
                        AND D.部署ＣＤ   = H.部署ＣＤ
                    INNER JOIN 商品マスタ AS I
                        ON I.企業ＣＤ = D.企業ＣＤ
                        AND ISNULL(I.無効フラグ, 0) = 0
                        AND I.商品ＣＤ = D.商品ＣＤ
                WHERE
                    ISNULL(I.弁当区分, 0) IN (@弁当区分チケット, @弁当区分その他) -- チケットとその他
                    AND ISNULL(D.副食ＣＤ, 0) <> 0          -- 副食がある
                    AND ISNULL(D.副食ＣＤ, 0) <> D.商品ＣＤ -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                GROUP BY
                    H.得意先ＣＤ,
                    D.副食ＣＤ
                ) AS PART
            GROUP BY
                PART.得意先ＣＤ,
                PART.部品ＣＤ
            ) AS PARTSUM
        GROUP BY
            PARTSUM.部品ＣＤ
        ) AS INIT ON MAIN.商品ＣＤ = INIT.部品ＣＤ
    ) AS B
    ON A.行番号 = B.行番号
ORDER BY
    行番号
    ,商品ＣＤ


@企業ＣＤ = 40000
@注文日 = 2018/04/27 0:00:00
@部署ＣＤ = 1
@コースＣＤ = 132
@弁当区分弁当 = 0
@弁当区分副食 = 1
@弁当区分主食 = 2
@弁当区分チケット = 8
@弁当区分その他 = 9
