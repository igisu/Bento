-- 短冊集計設定空き集計区分ＣＤ検索
WITH CTE(集計区分ＣＤ) AS
(SELECT
    1
UNION ALL
SELECT
    集計区分ＣＤ + 1
FROM
    CTE
WHERE
    集計区分ＣＤ < @件数
)
SELECT
    T1.集計区分ＣＤ
FROM
    (SELECT
       集計区分ＣＤ
    FROM
        CTE
    ) T1
    LEFT JOIN
        (SELECT
            集計区分ＣＤ
        FROM
            短冊集計設定マスタ
        WHERE
            企業ＣＤ = @企業ＣＤ
            AND 部署ＣＤ = @部署ＣＤ
        ) AS TSS ON T1.集計区分ＣＤ = TSS.集計区分ＣＤ
WHERE
    TSS.集計区分ＣＤ IS NULL
ORDER BY
    集計区分ＣＤ
OPTION (MAXRECURSION 0)
