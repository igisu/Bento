-- 集計タイプ別売上分析表（担当者別実績集計）
SELECT
    @売上日From AS 売上日From
    ,@売上日To AS 売上日To
    ,USH.部署ＣＤ
    ,BM.部署名
    ,USH.配送担当者ＣＤ
    ,USH.配送担当者名
    ,0 AS 部署ソート順
    ,0 AS 配送担当者ＣＤソート順
    ,ISNULL(SUM(SYUKEI_KIN.集計実績), 0) + ISNULL(SUM(TICKET_KIN.集計実績), 0) AS 集計実績
    ,ISNULL(SUM(SYUKEI_KOSU.集計個数), 0) AS 集計個数
    ,(SELECT 商品名略称 FROM 商品マスタ WHERE 企業ＣＤ = @企業ＣＤ AND 商品ＣＤ = ISNULL(@分割集計商品ＣＤ, -1)) AS 分割集計商品名
    ,CASE WHEN @分割集計商品ＣＤ IS NULL THEN NULL ELSE ISNULL(SUM(BUNKATSU_KIN.分割集計実績), 0) END AS 分割集計実績
    ,CASE WHEN @分割集計商品ＣＤ IS NULL THEN NULL ELSE ISNULL(SUM(BUNKATSU_KOSU.分割集計個数), 0) END AS 分割集計個数
    ,ISNULL(SUM(MCDS.持出数), 0) - (ISNULL(SUM(SYUKEI_KOSU.集計個数), 0) + ISNULL(SUM(BUNKATSU_KOSU.分割集計個数), 0)) AS ロス
    ,ISNULL(SUM(MAIN.現金合計), 0) AS 現金合計
    ,ISNULL(SUM(TICKET.金額), 0) AS チケット金額
    ,ISNULL(SUM(TICKET.枚数), 0) AS チケット枚数
    ,ISNULL(SUM(MAIN.売掛合計), 0) AS 売掛合計
    ,ISNULL(SUM(MAIN.電子決済), 0) AS 電子決済
    ,ISNULL(SUM(MAIN.総売上), 0) AS 総売上
    ,ISNULL(SUM(SONOTA.その他売上), 0) AS その他売上
    ,ISNULL(SUM(MCDS.持出数), 0) AS 持出数
FROM
    -- キー（一人の配送担当者が複数のコースを担当することがある。このタイプの売上分析表ではそれを配送担当者ごとに集計する）
    (SELECT DISTINCT
        企業ＣＤ
        ,精算日 AS 売上日
        ,部署ＣＤ
        ,コースＣＤ
        ,配送担当者ＣＤ
        ,配送担当者名
    FROM
        売上精算_ヘッダ
    WHERE
        企業ＣＤ = @企業ＣＤ
        AND 部署ＣＤ = @部署ＣＤ
        AND 精算日 >= @売上日From
        AND 精算日 <= @売上日To
    ) AS USH
    -- ①駆動表（売上日＆金額合計）
    LEFT JOIN
        (SELECT
            H.企業ＣＤ
            ,H.部署ＣＤ
            ,H.売上日
            ,H.コースＣＤ
            ,SUM(CASE WHEN D.売上区分 IN (@売上区分現金, @売上区分バークレー)   THEN D.商品金額合計税込 ELSE 0 END) AS 現金合計
            ,SUM(CASE WHEN D.売上区分 IN (@売上区分売掛)                        THEN D.商品金額合計税込 ELSE 0 END) AS 売掛合計
            ,SUM(CASE WHEN D.売上区分 IN (@売上区分LINE)                        THEN D.商品金額合計税込 ELSE 0 END) AS 電子決済
            ,SUM(CASE WHEN D.売上区分 IN (@売上区分チケット, @売上区分ボーナス) THEN 0 ELSE D.商品金額合計税込 END) AS 総売上
        FROM
            売上ヘッダ AS H
            INNER JOIN 売上明細 AS D
                ON D.企業ＣＤ    = H.企業ＣＤ
                AND D.売上日     = H.売上日
                AND D.得意先ＣＤ = H.得意先ＣＤ
                AND D.部署ＣＤ   = H.部署ＣＤ
        WHERE
            H.企業ＣＤ = @企業ＣＤ
            AND H.部署ＣＤ = @部署ＣＤ
            AND H.売上日 >= @売上日From
            AND H.売上日 <= @売上日To
        GROUP BY
            H.企業ＣＤ
            ,H.部署ＣＤ
            ,H.売上日
            ,H.コースＣＤ
        ) AS MAIN ON USH.企業ＣＤ = MAIN.企業ＣＤ AND USH.部署ＣＤ = MAIN.部署ＣＤ AND USH.売上日 = MAIN.売上日 AND ISNULL(USH.コースＣＤ, -1) = ISNULL(MAIN.コースＣＤ, -1)
    -- ②集計個数（分割集計商品ＣＤ以外を対象とし、弁当or主食or副食のみが集計対象）
    LEFT JOIN
        (SELECT
            PART.部署ＣＤ
            ,PART.売上日
            ,PART.コースＣＤ
            ,SUM(PART.商品売上数量) AS 集計個数
        FROM
            -- ②－８ 売上の弁当の主食
            (SELECT
                H.部署ＣＤ
                ,H.売上日
                ,H.コースＣＤ
                ,H.得意先ＣＤ
                ,D.主食ＣＤ AS 部品ＣＤ
                ,SUM(D.商品数量) AS 商品売上数量
            FROM
                売上ヘッダ AS H
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
                H.企業ＣＤ    = @企業ＣＤ
                AND H.売上日 >= @売上日From
                AND H.売上日 <= @売上日To
                AND H.部署ＣＤ = @部署ＣＤ
                AND D.商品ＣＤ <> @分割集計商品ＣＤ
                AND ISNULL(I.弁当区分, 0) = @弁当区分弁当   -- 弁当
                AND ISNULL(D.主食ＣＤ, 0) <> 0              -- 主食がある
                AND ISNULL(D.主食ＣＤ, 0) <> D.商品ＣＤ     -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
            GROUP BY
                H.部署ＣＤ
                ,H.売上日
                ,H.コースＣＤ
                ,H.得意先ＣＤ
                ,D.主食ＣＤ
            UNION ALL
            -- ②－９ 売上の弁当の副食
            SELECT
                H.部署ＣＤ
                ,H.売上日
                ,H.コースＣＤ
                ,H.得意先ＣＤ
                ,D.副食ＣＤ AS 部品ＣＤ
                ,SUM(D.商品数量) AS 商品売上数量
            FROM
                売上ヘッダ AS H
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
                H.企業ＣＤ    = @企業ＣＤ
                AND H.売上日 >= @売上日From
                AND H.売上日 <= @売上日To
                AND H.部署ＣＤ = @部署ＣＤ
                AND D.商品ＣＤ <> @分割集計商品ＣＤ
                AND ISNULL(I.弁当区分, 0) = @弁当区分弁当   -- 弁当
                AND ISNULL(D.副食ＣＤ, 0) <> 0              -- 副食がある
                AND ISNULL(D.副食ＣＤ, 0) <> D.商品ＣＤ     -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
            GROUP BY
                H.部署ＣＤ
                ,H.売上日
                ,H.コースＣＤ
                ,H.得意先ＣＤ
                ,D.副食ＣＤ
            UNION ALL
            -- ②－１０ 売上の主食のみ
            SELECT
                H.部署ＣＤ
                ,H.売上日
                ,H.コースＣＤ
                ,H.得意先ＣＤ
                ,D.主食ＣＤ AS 部品ＣＤ
                ,SUM(D.商品数量) AS 商品売上数量
            FROM
                売上ヘッダ AS H
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
                H.企業ＣＤ    = @企業ＣＤ
                AND H.売上日 >= @売上日From
                AND H.売上日 <= @売上日To
                AND H.部署ＣＤ = @部署ＣＤ
                AND D.商品ＣＤ <> @分割集計商品ＣＤ
                AND ISNULL(I.弁当区分, 0) = @弁当区分主食
            GROUP BY
                H.部署ＣＤ
                ,H.売上日
                ,H.コースＣＤ
                ,H.得意先ＣＤ
                ,D.主食ＣＤ
            UNION ALL
            -- ②－１１ 売上の副食のみ
            SELECT
                H.部署ＣＤ
                ,H.売上日
                ,H.コースＣＤ
                ,H.得意先ＣＤ
                ,D.副食ＣＤ AS 部品ＣＤ
                ,SUM(D.商品数量) AS 商品売上数量
            FROM
                売上ヘッダ AS H
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
                H.企業ＣＤ    = @企業ＣＤ
                AND H.売上日 >= @売上日From
                AND H.売上日 <= @売上日To
                AND H.部署ＣＤ = @部署ＣＤ
                AND D.商品ＣＤ <> @分割集計商品ＣＤ
                AND ISNULL(I.弁当区分, 0) = @弁当区分副食
            GROUP BY
                H.部署ＣＤ
                ,H.売上日
                ,H.コースＣＤ
                ,H.得意先ＣＤ
                ,D.副食ＣＤ
            UNION ALL
            -- ②－１５ 売上のチケットの主食
            -- 管理マスタ.チケット回収時売上フラグがOFFの場合は、売上区分がチケットorボーナスの売上金額は0になっている。
            -- つまりONの場合は金額が計上されているので、OFFの場合の金額を計上する必要がある。
            -- →OFFの場合は、チケット束売り（弁当区分=8:チケット）の金額と個数を計上する。
            -- 金額はチケット束売りの売上明細の商品金額合計税込を集計すればよいが、個数は主食と副食をそれぞれ集計する必要がある。
            -- チケット束売りに主食・副食が設定されていることはほぼないが、もしあればそれを集計する。
            SELECT
                H.部署ＣＤ
                ,H.売上日
                ,H.コースＣＤ
                ,H.得意先ＣＤ
                ,D.主食ＣＤ AS 部品ＣＤ
                ,SUM(D.商品数量) AS 商品売上数量
            FROM
                売上ヘッダ AS H
                INNER JOIN 売上明細 AS D
                    ON D.企業ＣＤ    = H.企業ＣＤ
                    AND D.売上日     = H.売上日
                    AND D.得意先ＣＤ = H.得意先ＣＤ
                    AND D.部署ＣＤ   = H.部署ＣＤ
                INNER JOIN 商品マスタ AS I
                    ON I.企業ＣＤ = D.企業ＣＤ
                    AND ISNULL(I.無効フラグ, 0) = 0
                    AND I.商品ＣＤ = D.商品ＣＤ
                INNER JOIN 管理マスタ AS K
                    ON K.企業ＣＤ = H.企業ＣＤ
            WHERE
                H.企業ＣＤ    = @企業ＣＤ
                AND H.売上日 >= @売上日From
                AND H.売上日 <= @売上日To
                AND H.部署ＣＤ = @部署ＣＤ
                AND D.商品ＣＤ <> @分割集計商品ＣＤ
                AND ISNULL(I.弁当区分, 0) = @弁当区分チケット   -- チケット
                AND ISNULL(D.主食ＣＤ, 0) <> 0                  -- 主食がある
                AND ISNULL(D.主食ＣＤ, 0) <> D.商品ＣＤ         -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                AND ISNULL(K.チケット回収時売上フラグ, 0) = 0   -- チケット回収時に売上にしない
            GROUP BY
                H.部署ＣＤ
                ,H.売上日
                ,H.コースＣＤ
                ,H.得意先ＣＤ
                ,D.主食ＣＤ
            UNION ALL
            -- ②－１６ 売上のチケットの副食
            -- 管理マスタ.チケット回収時売上フラグがOFFの場合は、売上区分がチケットorボーナスの売上金額は0になっている。
            -- つまりONの場合は金額が計上されているので、OFFの場合の金額を計上する必要がある。
            -- →OFFの場合は、チケット束売り（弁当区分=8:チケット）の金額と個数を計上する。
            -- 金額はチケット束売りの売上明細の商品金額合計税込を集計すればよいが、個数は主食と副食をそれぞれ集計する必要がある。
            -- チケット束売りに主食・副食が設定されていることはほぼないが、もしあればそれを集計する。
            SELECT
                H.部署ＣＤ
                ,H.売上日
                ,H.コースＣＤ
                ,H.得意先ＣＤ
                ,D.副食ＣＤ AS 部品ＣＤ
                ,SUM(D.商品数量) AS 商品売上数量
            FROM
                売上ヘッダ AS H
                INNER JOIN 売上明細 AS D
                    ON D.企業ＣＤ    = H.企業ＣＤ
                    AND D.売上日     = H.売上日
                    AND D.得意先ＣＤ = H.得意先ＣＤ
                    AND D.部署ＣＤ   = H.部署ＣＤ
                INNER JOIN 商品マスタ AS I
                    ON I.企業ＣＤ = D.企業ＣＤ
                    AND ISNULL(I.無効フラグ, 0) = 0
                    AND I.商品ＣＤ = D.商品ＣＤ
                INNER JOIN 管理マスタ AS K
                    ON K.企業ＣＤ = H.企業ＣＤ
            WHERE
                H.企業ＣＤ    = @企業ＣＤ
                AND H.売上日 >= @売上日From
                AND H.売上日 <= @売上日To
                AND H.部署ＣＤ = @部署ＣＤ
                AND D.商品ＣＤ <> @分割集計商品ＣＤ
                AND ISNULL(I.弁当区分, 0) = @弁当区分チケット   -- チケット
                AND ISNULL(D.副食ＣＤ, 0) <> 0                  -- 副食がある
                AND ISNULL(D.副食ＣＤ, 0) <> D.商品ＣＤ         -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                AND ISNULL(K.チケット回収時売上フラグ, 0) = 0   -- チケット回収時に売上にしない
            GROUP BY
                H.部署ＣＤ
                ,H.売上日
                ,H.コースＣＤ
                ,H.得意先ＣＤ
                ,D.副食ＣＤ
            ) AS PART
        GROUP BY
            PART.部署ＣＤ
            ,PART.売上日
            ,PART.コースＣＤ
        ) AS SYUKEI_KOSU ON MAIN.部署ＣＤ = SYUKEI_KOSU.部署ＣＤ AND MAIN.売上日 = SYUKEI_KOSU.売上日 AND ISNULL(MAIN.コースＣＤ, -1) = ISNULL(SYUKEI_KOSU.コースＣＤ, -1)
    -- ③分割集計個数（分割集計商品ＣＤのみを対象とし、（弁当or主食or副食に加え）その他も含める点が②集計個数とは異なる）
    LEFT JOIN
        (SELECT
            PART.部署ＣＤ
            ,PART.売上日
            ,PART.コースＣＤ
            ,CASE WHEN @分割集計商品ＣＤ IS NULL THEN NULL ELSE SUM(PART.商品売上数量) END AS 分割集計個数
        FROM
            -- ③－８ 売上の弁当の主食
            (SELECT
                H.部署ＣＤ
                ,H.売上日
                ,H.コースＣＤ
                ,H.得意先ＣＤ
                ,D.主食ＣＤ AS 部品ＣＤ
                ,SUM(D.商品数量) AS 商品売上数量
            FROM
                売上ヘッダ AS H
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
                H.企業ＣＤ    = @企業ＣＤ
                AND H.売上日 >= @売上日From
                AND H.売上日 <= @売上日To
                AND H.部署ＣＤ = @部署ＣＤ
                AND D.商品ＣＤ = @分割集計商品ＣＤ
                AND ISNULL(I.弁当区分, 0) = @弁当区分弁当   -- 弁当
                AND ISNULL(D.主食ＣＤ, 0) <> 0              -- 主食がある
                AND ISNULL(D.主食ＣＤ, 0) <> D.商品ＣＤ     -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
            GROUP BY
                H.部署ＣＤ
                ,H.売上日
                ,H.コースＣＤ
                ,H.得意先ＣＤ
                ,D.主食ＣＤ
            UNION ALL
            -- ③－９ 売上の弁当の副食
            SELECT
                H.部署ＣＤ
                ,H.売上日
                ,H.コースＣＤ
                ,H.得意先ＣＤ
                ,D.副食ＣＤ AS 部品ＣＤ
                ,SUM(D.商品数量) AS 商品売上数量
            FROM
                売上ヘッダ AS H
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
                H.企業ＣＤ    = @企業ＣＤ
                AND H.売上日 >= @売上日From
                AND H.売上日 <= @売上日To
                AND H.部署ＣＤ = @部署ＣＤ
                AND D.商品ＣＤ = @分割集計商品ＣＤ
                AND ISNULL(I.弁当区分, 0) = @弁当区分弁当   -- 弁当
                AND ISNULL(D.副食ＣＤ, 0) <> 0              -- 副食がある
                AND ISNULL(D.副食ＣＤ, 0) <> D.商品ＣＤ     -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
            GROUP BY
                H.部署ＣＤ
                ,H.売上日
                ,H.コースＣＤ
                ,H.得意先ＣＤ
                ,D.副食ＣＤ
            UNION ALL
            -- ③－１０ 売上の主食のみ
            SELECT
                H.部署ＣＤ
                ,H.売上日
                ,H.コースＣＤ
                ,H.得意先ＣＤ
                ,D.主食ＣＤ AS 部品ＣＤ
                ,SUM(D.商品数量) AS 商品売上数量
            FROM
                売上ヘッダ AS H
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
                H.企業ＣＤ    = @企業ＣＤ
                AND H.売上日 >= @売上日From
                AND H.売上日 <= @売上日To
                AND H.部署ＣＤ = @部署ＣＤ
                AND D.商品ＣＤ = @分割集計商品ＣＤ
                AND ISNULL(I.弁当区分, 0) = @弁当区分主食
            GROUP BY
                H.部署ＣＤ
                ,H.売上日
                ,H.コースＣＤ
                ,H.得意先ＣＤ
                ,D.主食ＣＤ
            UNION ALL
            -- ③－１１ 売上の副食のみ
            SELECT
                H.部署ＣＤ
                ,H.売上日
                ,H.コースＣＤ
                ,H.得意先ＣＤ
                ,D.副食ＣＤ AS 部品ＣＤ
                ,SUM(D.商品数量) AS 商品売上数量
            FROM
                売上ヘッダ AS H
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
                H.企業ＣＤ    = @企業ＣＤ
                AND H.売上日 >= @売上日From
                AND H.売上日 <= @売上日To
                AND H.部署ＣＤ = @部署ＣＤ
                AND D.商品ＣＤ = @分割集計商品ＣＤ
                AND ISNULL(I.弁当区分, 0) = @弁当区分副食
            GROUP BY
                H.部署ＣＤ
                ,H.売上日
                ,H.コースＣＤ
                ,H.得意先ＣＤ
                ,D.副食ＣＤ
            UNION ALL
            -- ③－１２ 売上の主食でも副食でもないもの
            SELECT
                H.部署ＣＤ
                ,H.売上日
                ,H.コースＣＤ
                ,H.得意先ＣＤ
                ,D.商品ＣＤ AS 部品ＣＤ
                ,SUM(D.商品数量) AS 商品売上数量
            FROM
                売上ヘッダ AS H
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
                H.企業ＣＤ    = @企業ＣＤ
                AND H.売上日 >= @売上日From
                AND H.売上日 <= @売上日To
                AND H.部署ＣＤ = @部署ＣＤ
                AND D.商品ＣＤ = @分割集計商品ＣＤ
                AND ISNULL(D.主食ＣＤ, 0) = 0
                AND ISNULL(D.副食ＣＤ, 0) = 0
            GROUP BY
                H.部署ＣＤ
                ,H.売上日
                ,H.コースＣＤ
                ,H.得意先ＣＤ
                ,D.商品ＣＤ
            UNION ALL
            -- ③－１３ 売上のその他の主食
            SELECT
                H.部署ＣＤ
                ,H.売上日
                ,H.コースＣＤ
                ,H.得意先ＣＤ
                ,D.主食ＣＤ AS 部品ＣＤ
                ,SUM(D.商品数量) AS 商品売上数量
            FROM
                売上ヘッダ AS H
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
                H.企業ＣＤ    = @企業ＣＤ
                AND H.売上日 >= @売上日From
                AND H.売上日 <= @売上日To
                AND H.部署ＣＤ = @部署ＣＤ
                AND D.商品ＣＤ = @分割集計商品ＣＤ
                AND ISNULL(I.弁当区分, 0) = @弁当区分その他 -- その他
                AND ISNULL(D.主食ＣＤ, 0) <> 0              -- 主食がある
                AND ISNULL(D.主食ＣＤ, 0) <> D.商品ＣＤ     -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
            GROUP BY
                H.部署ＣＤ
                ,H.売上日
                ,H.コースＣＤ
                ,H.得意先ＣＤ
                ,D.主食ＣＤ
            UNION ALL
            -- ③－１４ 売上のその他の副食
            SELECT
                H.部署ＣＤ
                ,H.売上日
                ,H.コースＣＤ
                ,H.得意先ＣＤ
                ,D.副食ＣＤ AS 部品ＣＤ
                ,SUM(D.商品数量) AS 商品売上数量
            FROM
                売上ヘッダ AS H
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
                H.企業ＣＤ    = @企業ＣＤ
                AND H.売上日 >= @売上日From
                AND H.売上日 <= @売上日To
                AND H.部署ＣＤ = @部署ＣＤ
                AND D.商品ＣＤ = @分割集計商品ＣＤ
                AND ISNULL(I.弁当区分, 0) = @弁当区分その他 -- その他
                AND ISNULL(D.副食ＣＤ, 0) <> 0              -- 副食がある
                AND ISNULL(D.副食ＣＤ, 0) <> D.商品ＣＤ     -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
            GROUP BY
                H.部署ＣＤ
                ,H.売上日
                ,H.コースＣＤ
                ,H.得意先ＣＤ
                ,D.副食ＣＤ
            ) AS PART
        GROUP BY
            PART.部署ＣＤ
            ,PART.売上日
            ,PART.コースＣＤ
        ) AS BUNKATSU_KOSU ON MAIN.部署ＣＤ = BUNKATSU_KOSU.部署ＣＤ AND MAIN.売上日 = BUNKATSU_KOSU.売上日 AND ISNULL(MAIN.コースＣＤ, -1) = ISNULL(BUNKATSU_KOSU.コースＣＤ, -1)
    -- ④集計金額（弁当or副食or主食）
    LEFT JOIN
        (SELECT
            H.部署ＣＤ
            ,H.売上日
            ,H.コースＣＤ
            ,SUM(D.商品金額合計税込) AS 集計実績
        FROM
            売上ヘッダ AS H
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
            H.企業ＣＤ = @企業ＣＤ
            AND H.部署ＣＤ = @部署ＣＤ
            AND H.売上日 >= @売上日From
            AND H.売上日 <= @売上日To
            AND D.商品ＣＤ <> @分割集計商品ＣＤ
            AND ISNULL(I.弁当区分, 0) IN (@弁当区分弁当, @弁当区分副食, @弁当区分主食)
        GROUP BY
            H.部署ＣＤ
            ,H.売上日
            ,H.コースＣＤ
        ) AS SYUKEI_KIN ON MAIN.部署ＣＤ = SYUKEI_KIN.部署ＣＤ AND MAIN.売上日 = SYUKEI_KIN.売上日 AND ISNULL(MAIN.コースＣＤ, -1) = ISNULL(SYUKEI_KIN.コースＣＤ, -1)
    -- ⑤集計金額（チケット束売り）
    -- 管理マスタ.チケット回収時売上フラグがOFFの場合は、売上区分がチケットorボーナスの売上金額は0になっている。
    -- つまりONの場合は金額が計上されているので、OFFの場合の金額を計上する必要がある。
    -- →OFFの場合は、チケット束売り（弁当区分=8:チケット）の金額を計上する。
    -- 金額はチケット束売りの売上明細の商品金額合計税込を集計すればよい。
    LEFT JOIN
        (SELECT
            H.部署ＣＤ
            ,H.売上日
            ,H.コースＣＤ
            ,SUM(D.商品金額合計税込) AS 集計実績
        FROM
            売上ヘッダ AS H
            INNER JOIN 売上明細 AS D
                ON D.企業ＣＤ    = H.企業ＣＤ
                AND D.売上日     = H.売上日
                AND D.得意先ＣＤ = H.得意先ＣＤ
                AND D.部署ＣＤ   = H.部署ＣＤ
            INNER JOIN 商品マスタ AS I
                ON I.企業ＣＤ = D.企業ＣＤ
                AND ISNULL(I.無効フラグ, 0) = 0
                AND I.商品ＣＤ = D.商品ＣＤ
            INNER JOIN 管理マスタ AS K
                ON K.企業ＣＤ = D.企業ＣＤ
        WHERE
            H.企業ＣＤ = @企業ＣＤ
            AND H.部署ＣＤ = @部署ＣＤ
            AND H.売上日 >= @売上日From
            AND H.売上日 <= @売上日To
            AND D.商品ＣＤ <> @分割集計商品ＣＤ
            AND ISNULL(I.弁当区分, 0) = @弁当区分チケット
            AND ISNULL(K.チケット回収時売上フラグ, 0) = 0   -- チケット回収時に売上にしない
        GROUP BY
            H.部署ＣＤ
            ,H.売上日
            ,H.コースＣＤ
        ) AS TICKET_KIN ON MAIN.部署ＣＤ = TICKET_KIN.部署ＣＤ AND MAIN.売上日 = TICKET_KIN.売上日 AND ISNULL(MAIN.コースＣＤ, -1) = ISNULL(TICKET_KIN.コースＣＤ, -1)
    -- ⑥分割集計金額
    LEFT JOIN
        (SELECT
            H.部署ＣＤ
            ,H.売上日
            ,H.コースＣＤ
            ,CASE WHEN @分割集計商品ＣＤ IS NULL THEN NULL ELSE SUM(D.商品金額合計税込) END AS 分割集計実績
        FROM
            売上ヘッダ AS H
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
            H.企業ＣＤ = @企業ＣＤ
            AND H.部署ＣＤ = @部署ＣＤ
            AND H.売上日 >= @売上日From
            AND H.売上日 <= @売上日To
            AND D.商品ＣＤ = @分割集計商品ＣＤ
        GROUP BY
            H.部署ＣＤ
            ,H.売上日
            ,H.コースＣＤ
        ) AS BUNKATSU_KIN ON MAIN.部署ＣＤ = BUNKATSU_KIN.部署ＣＤ AND MAIN.売上日 = BUNKATSU_KIN.売上日 AND ISNULL(MAIN.コースＣＤ, -1) = ISNULL(BUNKATSU_KIN.コースＣＤ, -1)
    -- ⑦チケット払
    LEFT JOIN
        (SELECT
            H.部署ＣＤ
            ,H.売上日
            ,H.コースＣＤ
            ,SUM(D.商品数量) AS 枚数
            ,SUM(D.商品金額合計税込) AS 金額
        FROM
            売上ヘッダ AS H
            INNER JOIN 売上明細 AS D
                ON D.企業ＣＤ    = H.企業ＣＤ
                AND D.売上日     = H.売上日
                AND D.得意先ＣＤ = H.得意先ＣＤ
                AND D.部署ＣＤ   = H.部署ＣＤ
        WHERE
            H.企業ＣＤ = @企業ＣＤ
            AND H.部署ＣＤ = @部署ＣＤ
            AND H.売上日 >= @売上日From
            AND H.売上日 <= @売上日To
            AND D.売上区分 IN (@売上区分チケット, @売上区分ボーナス)
        GROUP BY
            H.部署ＣＤ
            ,H.売上日
            ,H.コースＣＤ
        ) AS TICKET ON MAIN.部署ＣＤ = TICKET.部署ＣＤ AND MAIN.売上日 = TICKET.売上日 AND ISNULL(MAIN.コースＣＤ, -1) = ISNULL(TICKET.コースＣＤ, -1)
    -- ⑧その他売上
    LEFT JOIN
        (SELECT
            H.部署ＣＤ
            ,H.売上日
            ,H.コースＣＤ
            ,SUM(D.商品金額合計税込) AS その他売上
        FROM
            売上ヘッダ AS H
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
            H.企業ＣＤ = @企業ＣＤ
            AND H.部署ＣＤ = @部署ＣＤ
            AND H.売上日 >= @売上日From
            AND H.売上日 <= @売上日To
            AND ISNULL(I.弁当区分, 0) = @弁当区分その他
        GROUP BY
            H.部署ＣＤ
            ,H.売上日
            ,H.コースＣＤ
        ) AS SONOTA ON MAIN.部署ＣＤ = SONOTA.部署ＣＤ AND MAIN.売上日 = SONOTA.売上日 AND ISNULL(MAIN.コースＣＤ, -1) = ISNULL(SONOTA.コースＣＤ, -1)
    -- ⑨持出数
    LEFT JOIN
        (SELECT
            C.部署ＣＤ
            ,M.持出日
            ,C.コースＣＤ
            ,SUM(M.個数) AS 持出数
        FROM
            モバイル_持ち出し入力 AS M
            INNER JOIN コースマスタ AS C ON M.企業ＣＤ = C.企業ＣＤ AND M.コースＣＤ = C.コースＣＤ
        WHERE
            M.企業ＣＤ = @企業ＣＤ
            AND M.持出日 >= @売上日From
            AND M.持出日 <= @売上日To
            AND C.部署ＣＤ = @部署ＣＤ
        GROUP BY
            C.部署ＣＤ
            ,M.持出日
            ,C.コースＣＤ
        ) AS MCDS ON MAIN.部署ＣＤ = MCDS.部署ＣＤ AND MAIN.売上日 = MCDS.持出日 AND ISNULL(MAIN.コースＣＤ, -1) = ISNULL(MCDS.コースＣＤ, -1)
    LEFT JOIN 部署マスタ AS BM ON BM.企業ＣＤ = USH.企業ＣＤ AND BM.部署ＣＤ = USH.部署ＣＤ
GROUP BY
    USH.部署ＣＤ
    ,BM.部署名
    ,USH.配送担当者ＣＤ
    ,USH.配送担当者名
ORDER BY
    部署ソート順
    ,USH.部署ＣＤ
    ,配送担当者ＣＤソート順
    ,USH.配送担当者ＣＤ



-- 部署計・総合計はプログラム側で算出

@企業ＣＤ = 40001
@売上日From = 2019/01/15
@売上日To = 2019/01/21
@部署ＣＤ = 1
@分割集計商品ＣＤ = 3000
@弁当区分弁当 = 0
@弁当区分副食 = 1
@弁当区分主食 = 2
@弁当区分チケット = 8
@弁当区分その他 = 9
@売上区分現金 = 0
@売上区分売掛 = 1
@売上区分チケット = 2
@売上区分バークレー = 3
@売上区分ボーナス = 4
@売上区分LINE = 5
