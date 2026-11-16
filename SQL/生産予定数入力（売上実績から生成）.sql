-- 生産予定数入力（売上実績から生成）
-- すべての部署について生成するので、部署は無視する
INSERT INTO 生産予定 (
    企業ＣＤ
    ,部署ＣＤ
    ,喫食日
    ,商品ＣＤ
    ,コースＣＤ
    ,入力フラグ
    ,数量
    ,作成担当者ＣＤ
    ,作成日
    ,修正担当者ＣＤ
    ,修正日
) SELECT
    A.企業ＣＤ
    ,A.部署ＣＤ
    ,A.喫食日
    ,A.商品ＣＤ
    ,A.コースＣＤ
    ,0 AS 入力フラグ
    ,A.数量
    ,@作成担当者ＣＤ AS 作成担当者ＣＤ
    ,getdate()
    ,@修正担当者ＣＤ AS 修正担当者ＣＤ
    ,getdate()
FROM
    (SELECT
        U.企業ＣＤ
        ,U.部署ＣＤ
        ,U.喫食日
        ,U.商品ＣＤ
        ,U.コースＣＤ
        ,0 AS 入力フラグ
        ,U.数量
    FROM
        -- ①売上明細からコースと商品ごとの数量を取得
        (SELECT
            C.企業ＣＤ
            ,C.部署ＣＤ
            ,@注文日 AS 喫食日
            ,C.コースＣＤ
            ,C.部品ＣＤ AS 商品ＣＤ
            ,CEILING(AVG(C.数量)) AS 数量
        FROM
            -- ごはんとしての商品と、弁当の部品(=主食)のごはんとを、同じ売上日で合計する（副食も同様）
            (SELECT
                B.企業ＣＤ
                ,B.部署ＣＤ
                ,B.コースＣＤ
                ,B.部品ＣＤ
                ,B.売上日
                ,SUM(B.商品数量) AS 数量
            FROM
                -- ①－１ 弁当の主食
                (SELECT
                    M.企業ＣＤ
                    ,M.部署ＣＤ
                    ,H.コースＣＤ
        --              ,M.商品ＣＤ
                    ,I.主食ＣＤ AS 部品ＣＤ
                    ,M.売上日
                    ,M.商品数量
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
                    INNER JOIN 商品マスタ AS P          -- 部品本体が無効でないこと
                        ON P.企業ＣＤ = I.企業ＣＤ
                        AND ISNULL(P.無効フラグ, 0) = 0
                        AND P.商品ＣＤ = I.主食ＣＤ
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
                UNION ALL   -- SUM()の対象にするので、同一部署、同一コース、同一部品ＣＤ、同一売上日、同一商品数量を同一視せずに全て抽出する（以下同様）
                -- ①－２ 弁当の副食
                SELECT
                    M.企業ＣＤ
                    ,M.部署ＣＤ
                    ,H.コースＣＤ
        --            ,M.商品ＣＤ
                    ,I.副食ＣＤ AS 部品ＣＤ
                    ,M.売上日
                    ,M.商品数量
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
                    INNER JOIN 商品マスタ AS P          -- 部品本体が無効でないこと
                        ON P.企業ＣＤ = I.企業ＣＤ
                        AND ISNULL(P.無効フラグ, 0) = 0
                        AND P.商品ＣＤ = I.副食ＣＤ
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
                UNION ALL
                -- ①－３ 主食のみ
                SELECT
                    M.企業ＣＤ
                    ,M.部署ＣＤ
                    ,H.コースＣＤ
        --            ,M.商品ＣＤ
                    ,I.商品ＣＤ AS 部品ＣＤ
                    ,M.売上日
                    ,M.商品数量
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
                UNION ALL
                -- ①－４ 副食のみ
                SELECT
                    M.企業ＣＤ
                    ,M.部署ＣＤ
                    ,H.コースＣＤ
        --            ,M.商品ＣＤ
                    ,I.商品ＣＤ AS 部品ＣＤ
                    ,M.売上日
                    ,M.商品数量
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
                ) AS B
            GROUP BY
                B.企業ＣＤ
                ,B.部署ＣＤ
                ,B.コースＣＤ
                ,B.部品ＣＤ
                ,B.売上日
            ) AS C
        GROUP BY
            C.企業ＣＤ
            ,C.部署ＣＤ
            ,C.コースＣＤ
            ,C.部品ＣＤ
        ) AS U
        -- ②生産予定に無いレコードを抽出するための結合
        LEFT JOIN 生産予定 AS S
            ON  S.企業ＣＤ   = U.企業ＣＤ
            AND S.部署ＣＤ   = U.部署ＣＤ
            AND S.喫食日     = U.喫食日
            AND S.商品ＣＤ   = U.商品ＣＤ
            AND S.コースＣＤ = U.コースＣＤ
        -- ③コース区分の条件用
        INNER JOIN コースマスタ AS C
            ON  C.企業ＣＤ   = U.企業ＣＤ
            AND C.コースＣＤ = U.コースＣＤ
        -- ④コース別グループと結合
--        INNER JOIN コース別グループマスタ AS CG ON
--            CG.企業ＣＤ = @企業ＣＤ
--            AND CG.部署ＣＤ = @部署ＣＤ
--            AND CG.グループ種別ＣＤ = @グループ種別ＣＤ
--            AND CG.コースＣＤ = U.コースＣＤ
        WHERE S.作成担当者ＣＤ IS NULL          -- 生産予定に無いレコード
        AND   C.コース区分 = @コース区分        -- 曜日に応じたコース
        AND   ISNULL(C.工場フラグ, 0) = 0
        AND   ISNULL(C.無効フラグ, 0) = 0
    ) AS A
