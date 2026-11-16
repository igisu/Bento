-- 得意先空き番号検索
WITH CTE(行連番) AS
(SELECT
    @開始番号
UNION ALL
SELECT
    行連番 + 1
FROM
    CTE
)
SELECT
    T1.digits
FROM
    (SELECT TOP @件数
        行連番 AS digits
    FROM
        CTE
    ) T1
    LEFT JOIN
        (SELECT
            得意先ＣＤ
        FROM
            得意先マスタ
        WHERE
            企業ＣＤ = @企業ＣＤ
        ) AS TOK ON T1.digits = TOK.得意先ＣＤ
WHERE
    T1.digits BETWEEN @開始番号 AND @開始番号+@件数
    AND T1.digits <= 999999999          -- 取得するのは999999999(9桁)まで
    AND TOK.得意先ＣＤ IS NULL
ORDER BY
    digits
OPTION (MAXRECURSION 0)
