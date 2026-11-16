-- 集計タイプ別売上分析表（部署別実績日計）
SELECT
    @売上日From AS 売上日From
    ,@売上日To AS 売上日To
    ,MAIN.部署ＣＤ
    ,BM.部署名
    ,MAIN.売上日
    ,'' AS 合計見出し
    ,0 AS 部署ソート順
    ,0 AS 売上日ソート順
    ,ISNULL(SYUKEI_KIN.集計実績, 0) + ISNULL(TICKET_KIN.集計実績, 0) AS 集計実績
    ,ISNULL(SYUKEI_KOSU.集計個数, 0) AS 集計個数
    ,(SELECT 商品名略称 FROM 商品マスタ WHERE 企業ＣＤ = @企業ＣＤ AND 商品ＣＤ = ISNULL(@分割集計商品ＣＤ, -1)) AS 分割集計商品名
    ,CASE WHEN @分割集計商品ＣＤ IS NULL THEN NULL ELSE ISNULL(BUNKATSU_KIN.分割集計実績, 0) END AS 分割集計実績
    ,CASE WHEN @分割集計商品ＣＤ IS NULL THEN NULL ELSE ISNULL(BUNKATSU_KOSU.分割集計個数, 0) END AS 分割集計個数
    ,ISNULL(MCDS.持出数, 0) - (ISNULL(SYUKEI_KOSU.集計個数, 0) + ISNULL(BUNKATSU_KOSU.分割集計個数, 0)) AS ロス
    ,ISNULL(MAIN.現金合計, 0) AS 現金合計
    ,ISNULL(TICKET.金額, 0) AS チケット金額
    ,ISNULL(TICKET.枚数, 0) AS チケット枚数
    ,ISNULL(MAIN.売掛合計, 0) AS 売掛合計
    ,ISNULL(MAIN.電子決済, 0) AS 電子決済
    ,ISNULL(MAIN.総売上, 0) AS 総売上
    ,ISNULL(SONOTA.その他売上, 0) AS その他売上
    ,ISNULL(MCDS.持出数, 0) AS 持出数
FROM
    -- ①駆動表（売上日＆金額合計）
    (SELECT
        部署ＣＤ
        ,売上日
        ,SUM(CASE WHEN 売上区分 IN (@売上区分現金, @売上区分バークレー)   THEN 商品金額合計税込 ELSE 0 END) AS 現金合計
        ,SUM(CASE WHEN 売上区分 IN (@売上区分売掛)                        THEN 商品金額合計税込 ELSE 0 END) AS 売掛合計
        ,SUM(CASE WHEN 売上区分 IN (@売上区分LINE)                        THEN 商品金額合計税込 ELSE 0 END) AS 電子決済
        ,SUM(CASE WHEN 売上区分 IN (@売上区分チケット, @売上区分ボーナス) THEN 0 ELSE 商品金額合計税込 END) AS 総売上
    FROM
        売上明細
    WHERE
        企業ＣＤ = @企業ＣＤ
        AND 部署ＣＤ = @部署ＣＤ
        AND 売上日 >= @売上日From
        AND 売上日 <= @売上日To
    GROUP BY
        部署ＣＤ
        ,売上日
    ) AS MAIN
    -- ②集計個数（分割集計商品ＣＤ以外を対象とし、弁当or主食or副食のみが集計対象）
    LEFT JOIN
        (SELECT
            PART.部署ＣＤ
            ,PART.売上日
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
        ) AS SYUKEI_KOSU ON MAIN.部署ＣＤ = SYUKEI_KOSU.部署ＣＤ AND MAIN.売上日 = SYUKEI_KOSU.売上日
    -- ③分割集計個数（分割集計商品ＣＤのみを対象とし、（弁当or主食or副食に加え）その他も含める点が②集計個数とは異なる）
    LEFT JOIN
        (SELECT
            PART.部署ＣＤ
            ,PART.売上日
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
        ) AS BUNKATSU_KOSU ON MAIN.部署ＣＤ = BUNKATSU_KOSU.部署ＣＤ AND MAIN.売上日 = BUNKATSU_KOSU.売上日
    -- ④集計金額（弁当or副食or主食）
    LEFT JOIN
        (SELECT
            D.部署ＣＤ
            ,D.売上日
            ,SUM(D.商品金額合計税込) AS 集計実績
        FROM
            売上明細 AS D
            INNER JOIN 商品マスタ AS I
                ON I.企業ＣＤ = D.企業ＣＤ
                AND ISNULL(I.無効フラグ, 0) = 0
                AND I.商品ＣＤ = D.商品ＣＤ
        WHERE
            D.企業ＣＤ = @企業ＣＤ
            AND D.部署ＣＤ = @部署ＣＤ
            AND D.商品ＣＤ <> @分割集計商品ＣＤ
            AND D.売上日 >= @売上日From
            AND D.売上日 <= @売上日To
            AND ISNULL(I.弁当区分, 0) IN (@弁当区分弁当, @弁当区分副食, @弁当区分主食)
            AND D.売上区分 <> @売上区分ボーナス     -- チケット回収時売上フラグに関係なくボーナスは含めない
        GROUP BY
            D.部署ＣＤ
            ,D.売上日
        ) AS SYUKEI_KIN ON MAIN.部署ＣＤ = SYUKEI_KIN.部署ＣＤ AND MAIN.売上日 = SYUKEI_KIN.売上日
    -- ⑤集計金額（チケット束売り）
    -- 管理マスタ.チケット回収時売上フラグがOFFの場合は、売上区分がチケットorボーナスの売上金額は0になっている。
    -- つまりONの場合は金額が計上されているので、OFFの場合の金額を計上する必要がある。
    -- →OFFの場合は、チケット束売り（弁当区分=8:チケット）の金額を計上する。
    -- 金額はチケット束売りの売上明細の商品金額合計税込を集計すればよい。
    LEFT JOIN
        (SELECT
            D.部署ＣＤ
            ,D.売上日
            ,SUM(D.商品金額合計税込) AS 集計実績
        FROM
            売上明細 AS D
            INNER JOIN 商品マスタ AS I
                ON I.企業ＣＤ = D.企業ＣＤ
                AND ISNULL(I.無効フラグ, 0) = 0
                AND I.商品ＣＤ = D.商品ＣＤ
            INNER JOIN 管理マスタ AS K
                ON K.企業ＣＤ = D.企業ＣＤ
        WHERE
            D.企業ＣＤ = @企業ＣＤ
            AND D.部署ＣＤ = @部署ＣＤ
            AND D.商品ＣＤ <> @分割集計商品ＣＤ
            AND D.売上日 >= @売上日From
            AND D.売上日 <= @売上日To
            AND ISNULL(I.弁当区分, 0) = @弁当区分チケット
            AND ISNULL(K.チケット回収時売上フラグ, 0) = 0   -- チケット回収時に売上にしない
        GROUP BY
            D.部署ＣＤ
            ,D.売上日
        ) AS TICKET_KIN ON MAIN.部署ＣＤ = TICKET_KIN.部署ＣＤ AND MAIN.売上日 = TICKET_KIN.売上日
    -- ⑥分割集計金額
    LEFT JOIN
        (SELECT
            D.部署ＣＤ
            ,D.売上日
            ,CASE WHEN @分割集計商品ＣＤ IS NULL THEN NULL ELSE SUM(D.商品金額合計税込) END AS 分割集計実績
        FROM
            売上明細 AS D
            INNER JOIN 商品マスタ AS I
                ON I.企業ＣＤ = D.企業ＣＤ
                AND ISNULL(I.無効フラグ, 0) = 0
                AND I.商品ＣＤ = D.商品ＣＤ
        WHERE
            D.企業ＣＤ = @企業ＣＤ
            AND D.部署ＣＤ = @部署ＣＤ
            AND D.商品ＣＤ = @分割集計商品ＣＤ
            AND D.売上日 >= @売上日From
            AND D.売上日 <= @売上日To
        GROUP BY
            D.部署ＣＤ
            ,D.売上日
        ) AS BUNKATSU_KIN ON MAIN.部署ＣＤ = BUNKATSU_KIN.部署ＣＤ AND MAIN.売上日 = BUNKATSU_KIN.売上日
    -- ⑦チケット払
    LEFT JOIN
        (SELECT
            部署ＣＤ
            ,売上日
            ,SUM(商品数量) AS 枚数
            ,SUM(商品金額合計税込) AS 金額
        FROM
            売上明細
        WHERE
            企業ＣＤ = @企業ＣＤ
            AND 部署ＣＤ = @部署ＣＤ
            AND 売上日 >= @売上日From
            AND 売上日 <= @売上日To
            AND 売上区分 IN (@売上区分チケット, @売上区分ボーナス)
        GROUP BY
            部署ＣＤ
            ,売上日
        ) AS TICKET ON MAIN.部署ＣＤ = TICKET.部署ＣＤ AND MAIN.売上日 = TICKET.売上日
    -- ⑧その他売上
    LEFT JOIN
        (SELECT
            D.部署ＣＤ
            ,D.売上日
            ,SUM(D.商品金額合計税込) AS その他売上
        FROM
            売上明細 AS D
            INNER JOIN 商品マスタ AS I
                ON I.企業ＣＤ = D.企業ＣＤ
                AND ISNULL(I.無効フラグ, 0) = 0
                AND I.商品ＣＤ = D.商品ＣＤ
        WHERE
            D.企業ＣＤ = @企業ＣＤ
            AND D.部署ＣＤ = @部署ＣＤ
            AND D.売上日 >= @売上日From
            AND D.売上日 <= @売上日To
            AND ISNULL(I.弁当区分, 0) = @弁当区分その他
        GROUP BY
            D.部署ＣＤ
            ,D.売上日
        ) AS SONOTA ON MAIN.部署ＣＤ = SONOTA.部署ＣＤ AND MAIN.売上日 = SONOTA.売上日
    -- ⑨持出数
    LEFT JOIN
        (SELECT
            C.部署ＣＤ
            ,M.持出日
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
        ) AS MCDS ON MAIN.部署ＣＤ = MCDS.部署ＣＤ AND MAIN.売上日 = MCDS.持出日
    LEFT JOIN 部署マスタ AS BM ON BM.企業ＣＤ = @企業ＣＤ AND BM.部署ＣＤ = MAIN.部署ＣＤ
ORDER BY
    部署ソート順
    ,MAIN.部署ＣＤ
    ,売上日ソート順
    ,MAIN.売上日

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
