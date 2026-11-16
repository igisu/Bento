-- 生産予定数入力（モバイル_持ち出し入力削除）
--SELECT M.*
DELETE FROM M
FROM
    モバイル_持ち出し入力 AS M
    INNER JOIN
        (SELECT
            @企業ＣＤ AS 企業ＣＤ
            ,C.コースＣＤ
            ,I.商品ＣＤ
            ,@注文日 AS 持出日
        FROM
            -- コースマスタと生産予定テーブルと売上から取得したコース（＝全コース）と、商品マスタと生産予定テーブルと売上から取得した商品（＝全商品）の組み合わせ
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
                売上ヘッダ AS H
                LEFT JOIN 祝日マスタ AS SK ON SK.企業ＣＤ = H.企業ＣＤ AND SK.対象日 = H.売上日
                INNER JOIN コースマスタ AS C
                    ON  C.企業ＣＤ   = H.企業ＣＤ
                    AND C.コースＣＤ = H.コースＣＤ
            WHERE
                H.企業ＣＤ = @企業ＣＤ
                AND H.部署ＣＤ = @部署ＣＤ
                AND H.コースＣＤ IS NOT NULL
                AND H.売上日 >= @期間開始日
                AND H.売上日 <= @期間終了日
                AND DATEPART(WEEKDAY, H.売上日) = DATEPART(WEEKDAY, @期間終了日)
                AND SK.対象日 IS NULL       -- 祝日は除く
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
                売上明細 AS M
                INNER JOIN 売上ヘッダ AS H
                    ON H.企業ＣＤ = M.企業ＣＤ
                    AND H.売上日 = M.売上日
                    AND H.得意先ＣＤ = M.得意先ＣＤ
                    AND H.部署ＣＤ = M.部署ＣＤ
                LEFT JOIN 祝日マスタ AS SK
                    ON SK.企業ＣＤ = M.企業ＣＤ
                    AND SK.対象日 = M.売上日
                INNER JOIN 商品マスタ AS I
                    ON I.企業ＣＤ = M.企業ＣＤ
                    AND ISNULL(I.無効フラグ, 0) = 0
                    AND I.商品ＣＤ = M.商品ＣＤ
            WHERE
                M.企業ＣＤ = @企業ＣＤ
                AND M.部署ＣＤ = @部署ＣＤ
                AND H.コースＣＤ IS NOT NULL
                AND M.売上日 >= @期間開始日
                AND M.売上日 <= @期間終了日
                AND DATEPART(WEEKDAY, M.売上日) = DATEPART(WEEKDAY, @期間終了日)
                AND SK.対象日 IS NULL       -- 祝日は除く
                AND ISNULL(I.弁当区分, 0) = @弁当区分弁当   -- 弁当
                AND ISNULL(M.主食ＣＤ, 0) <> 0              -- 主食がある
                AND ISNULL(M.主食ＣＤ, 0) <> M.商品ＣＤ     -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
            UNION
            -- ②－２ 弁当の副食
            SELECT
                I.副食ＣＤ AS 商品ＣＤ
            FROM
                売上明細 AS M
                INNER JOIN 売上ヘッダ AS H
                    ON H.企業ＣＤ = M.企業ＣＤ
                    AND H.売上日 = M.売上日
                    AND H.得意先ＣＤ = M.得意先ＣＤ
                    AND H.部署ＣＤ = M.部署ＣＤ
                LEFT JOIN 祝日マスタ AS SK
                    ON SK.企業ＣＤ = M.企業ＣＤ
                    AND SK.対象日 = M.売上日
                INNER JOIN 商品マスタ AS I
                    ON I.企業ＣＤ = M.企業ＣＤ
                    AND ISNULL(I.無効フラグ, 0) = 0
                    AND I.商品ＣＤ = M.商品ＣＤ
            WHERE
                M.企業ＣＤ = @企業ＣＤ
                AND M.部署ＣＤ = @部署ＣＤ
                AND H.コースＣＤ IS NOT NULL
                AND M.売上日 >= @期間開始日
                AND M.売上日 <= @期間終了日
                AND DATEPART(WEEKDAY, M.売上日) = DATEPART(WEEKDAY, @期間終了日)
                AND SK.対象日 IS NULL       -- 祝日は除く
                AND ISNULL(I.弁当区分, 0) = @弁当区分弁当   -- 弁当
                AND ISNULL(M.副食ＣＤ, 0) <> 0              -- 副食がある
                AND ISNULL(M.副食ＣＤ, 0) <> M.商品ＣＤ     -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
            UNION
            -- ②－３ 主食のみ
            SELECT
                I.商品ＣＤ AS 商品ＣＤ
            FROM
                売上明細 AS M
                INNER JOIN 売上ヘッダ AS H
                    ON H.企業ＣＤ = M.企業ＣＤ
                    AND H.売上日 = M.売上日
                    AND H.得意先ＣＤ = M.得意先ＣＤ
                    AND H.部署ＣＤ = M.部署ＣＤ
                LEFT JOIN 祝日マスタ AS SK
                    ON SK.企業ＣＤ = M.企業ＣＤ
                    AND SK.対象日 = M.売上日
                INNER JOIN 商品マスタ AS I
                    ON I.企業ＣＤ = M.企業ＣＤ
                    AND ISNULL(I.無効フラグ, 0) = 0
                    AND I.商品ＣＤ = M.商品ＣＤ
            WHERE
                M.企業ＣＤ = @企業ＣＤ
                AND M.部署ＣＤ = @部署ＣＤ
                AND H.コースＣＤ IS NOT NULL
                AND M.売上日 >= @期間開始日
                AND M.売上日 <= @期間終了日
                AND DATEPART(WEEKDAY, M.売上日) = DATEPART(WEEKDAY, @期間終了日)
                AND SK.対象日 IS NULL       -- 祝日は除く
                AND ISNULL(I.弁当区分, 0) = @弁当区分主食
            UNION
            -- ②－４ 副食のみ
            SELECT
                I.商品ＣＤ AS 商品ＣＤ
            FROM
                売上明細 AS M
                INNER JOIN 売上ヘッダ AS H
                    ON H.企業ＣＤ = M.企業ＣＤ
                    AND H.売上日 = M.売上日
                    AND H.得意先ＣＤ = M.得意先ＣＤ
                    AND H.部署ＣＤ = M.部署ＣＤ
                LEFT JOIN 祝日マスタ AS SK
                    ON SK.企業ＣＤ = M.企業ＣＤ
                    AND SK.対象日 = M.売上日
                INNER JOIN 商品マスタ AS I
                    ON I.企業ＣＤ = M.企業ＣＤ
                    AND ISNULL(I.無効フラグ, 0) = 0
                    AND I.商品ＣＤ = M.商品ＣＤ
            WHERE
                M.企業ＣＤ = @企業ＣＤ
                AND M.部署ＣＤ = @部署ＣＤ
                AND H.コースＣＤ IS NOT NULL
                AND M.売上日 >= @期間開始日
                AND M.売上日 <= @期間終了日
                AND DATEPART(WEEKDAY, M.売上日) = DATEPART(WEEKDAY, @期間終了日)
                AND SK.対象日 IS NULL       -- 祝日は除く
                AND ISNULL(I.弁当区分, 0) = @弁当区分副食
            ) AS I
        ) AS A
        ON A.企業ＣＤ = M.企業ＣＤ
        AND A.コースＣＤ = M.コースＣＤ
        AND A.商品ＣＤ = M.商品ＣＤ
        AND A.持出日 = M.持出日
    -- ④コース別グループと結合
--    INNER JOIN コース別グループマスタ AS CG ON
--        CG.企業ＣＤ = @企業ＣＤ
--        AND CG.部署ＣＤ = @部署ＣＤ
--        AND CG.グループ種別ＣＤ = @グループ種別ＣＤ
--        AND CG.コースＣＤ = A.コースＣＤ
