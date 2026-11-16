-- 請求レコード追加_計算税率コンボの選択肢
SELECT
    -1 AS 消費税率
    ,'' AS 表示用消費税率
UNION
SELECT
    0 AS 消費税率               -- 非課税
    ,'0.00' AS 表示用消費税率
UNION
SELECT
    消費税率                    -- 最新の通常税率
    ,cast(消費税率 as varchar) as 表示用消費税率
FROM
    消費税率マスタ
WHERE
    施行日 = (SELECT MAX(施行日) FROM 消費税率マスタ)
UNION
SELECT
    軽減税率 AS 消費税率        -- 最新の軽減税率
    ,cast(軽減税率 as varchar) as 表示用消費税率
FROM
    消費税率マスタ
WHERE
    施行日 = (SELECT MAX(施行日) FROM 消費税率マスタ)
    AND 軽減税率 IS NOT NULL
ORDER BY
    消費税率
