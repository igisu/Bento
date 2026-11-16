-- 生産予定数入力（コース取得）
SELECT
    C.コースＣＤ
    ,CM.コース名
FROM
    (SELECT
        コースＣＤ
    FROM
        コースマスタ
    WHERE
        企業ＣＤ = @企業ＣＤ
        AND 部署ＣＤ = @部署ＣＤ
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
    ) AS C
    LEFT JOIN コースマスタ AS CM ON CM.企業ＣＤ = @企業ＣＤ AND CM.コースＣＤ = C.コースＣＤ
    -- コース別グループと結合
--    INNER JOIN コース別グループマスタ AS CG ON
--        CG.企業ＣＤ = @企業ＣＤ
--        AND CG.部署ＣＤ = @部署ＣＤ
--        AND CG.グループ種別ＣＤ = @グループ種別ＣＤ
--        AND CG.コースＣＤ = C.コースＣＤ
    WHERE
        CM.コース区分 = @コース区分
ORDER BY
    C.コースＣＤ
