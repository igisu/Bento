-- Web注文停止日

--- 日付範囲を取得する再帰CTE（共通テーブル式）
WITH R AS
(SELECT
    cast(@表示期間From AS Date ) AS 日付
UNION ALL
SELECT
    DATEADD(day, 1, 日付 )
FROM
    R
WHERE R.日付 < @表示期間To
)
SELECT
    A.企業ＣＤ
    ,A.日付
    ,SK.名称 AS 祝日名
    ,A.部署ＣＤ
    ,A.部署名
    ,CASE WHEN WS.注文停止日 IS NULL THEN 0 ELSE 1 END AS 注文停止
FROM
    -- 日付と部署をFULL JOINして取得
    (SELECT
        R.日付
        ,B.企業ＣＤ
        ,B.部署ＣＤ
        ,B.部署名
    FROM
        R                   -- 日付範囲を取得する再帰CTEを用いて、指定した範囲の日付を取得
        ,部署マスタ AS B
    WHERE
        B.企業ＣＤ = @企業ＣＤ
    ) AS A
    LEFT JOIN 祝日マスタ AS SK
        ON SK.企業ＣＤ = A.企業ＣＤ
        AND SK.対象日 = A.日付
    LEFT JOIN WEB_M_注文停止日 AS WS
        ON WS.企業ＣＤ = A.企業ＣＤ
        AND WS.部署ＣＤ = A.部署ＣＤ
        AND WS.注文停止日 = A.日付
ORDER BY
    A.日付
    ,A.部署ＣＤ
