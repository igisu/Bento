-- コース区分取得

-- 0～30までを取得する再帰CTE（共通テーブル式）
With T(I) AS
(SELECT
    0 AS I
UNION ALL
SELECT
    I + 1
FROM
    T
WHERE
    I < datediff(day, '2019/01/01', '2019/01/31')
)

SELECT
    A.日付
    ,DATEPART(WEEKDAY, A.日付) AS 曜日
    ,HO.名称
    ,CASE DATEPART(WEEKDAY, A.日付)
        WHEN 1 THEN 3       -- 日曜
        WHEN 7 THEN 2       -- 土曜
        ELSE CASE WHEN HO.名称 IS NULL THEN 1 ELSE 4 END    -- 祝日or平日
    END AS コース区分
FROM
    -- ① 0～30までを取得する再帰CTEを用いて、指定した日付以降の日付を取得
    (SELECT
        CONVERT(date, DateAdd(day, I, '2019/01/01')) AS 日付
    FROM
        T
    ) AS A
    LEFT JOIN 祝日マスタ AS HO ON 対象日 = A.日付
