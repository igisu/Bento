-- 注文一覧（コース IS NULL）
-- コーステーブルは見ない
-- ※配送日で検索する場合は、WHERE句にある注文ヘッダの喫食日を配送日にする
--- 注文現金個数
SELECT
    0 AS 明細合計区分,
    MAIN.得意先ＣＤ,
    T.得意先名,
    CONVERT(varchar, MAIN.得意先ＣＤ) + ' ' + ISNULL(V.売掛現金区分名, ' ') + CHAR(10) + ISNULL(T.得意先名, '') AS 表示用得意先名,
    VT.売掛現金区分CD AS 売掛現金区分,
    VT.売掛現金区分名 AS タイプ,
    A.商品ＣＤ,
    A.個数 AS 値
FROM
    -- 駆動表（注文明細）
    (SELECT
        M.得意先ＣＤ
        ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_C0' AS 商品ＣＤ
    FROM
        注文ヘッダ AS H
        INNER JOIN 注文明細 AS M
            ON  H.企業ＣＤ   = M.企業ＣＤ
            AND H.注文区分   = M.注文区分
            AND H.喫食日     = M.喫食日
            AND H.得意先ＣＤ = M.得意先ＣＤ
            AND H.部署ＣＤ   = M.部署ＣＤ
        LEFT JOIN 主要商品リストマスタ AS A
            ON A.企業ＣＤ = M.企業ＣＤ
            AND A.商品ＣＤ = M.商品ＣＤ
            AND A.部署ＣＤ = M.部署ＣＤ
            AND ISNULL(A.無効フラグ, 0) = 0
    WHERE
        H.企業ＣＤ = @企業ＣＤ AND
        H.注文区分 = @注文区分注文 AND
        H.喫食日 = @喫食日 AND
        H.部署ＣＤ = @部署ＣＤ AND
        H.コースＣＤ IS NULL
    ) AS MAIN
    LEFT JOIN
    -- 注文明細（現金）（レコードがヒットしなくても得意先名とタイプは表示される）
    (SELECT
        H.得意先ＣＤ
        ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_C0' AS 商品ＣＤ
        ,SUM(M.商品数量) AS 個数
    FROM
        注文ヘッダ AS H
        INNER JOIN 注文明細 AS M
            ON  H.企業ＣＤ   = M.企業ＣＤ
            AND H.注文区分   = M.注文区分
            AND H.喫食日     = M.喫食日
            AND H.得意先ＣＤ = M.得意先ＣＤ
            AND H.部署ＣＤ   = M.部署ＣＤ
        LEFT JOIN 主要商品リストマスタ AS A
            ON A.企業ＣＤ = M.企業ＣＤ
            AND A.商品ＣＤ = M.商品ＣＤ
            AND A.部署ＣＤ = M.部署ＣＤ
            AND ISNULL(A.無効フラグ, 0) = 0
    WHERE
        H.企業ＣＤ = @企業ＣＤ AND
        H.注文区分 = @注文区分注文 AND
        H.喫食日 = @喫食日 AND
        H.部署ＣＤ = @部署ＣＤ AND
        H.コースＣＤ IS NULL AND
        M.売掛現金区分 = @売掛現金区分現金
    GROUP BY
        H.得意先ＣＤ
        ,A.商品ＣＤ
    ) AS A
        ON MAIN.得意先ＣＤ = A.得意先ＣＤ
        AND MAIN.商品ＣＤ   = A.商品ＣＤ
    LEFT JOIN 得意先マスタ  AS T ON T.企業ＣＤ = @企業ＣＤ AND T.得意先ＣＤ = MAIN.得意先ＣＤ
    LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分
    LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分現金
UNION
--- 注文現金金額
SELECT
    0 AS 明細合計区分,
    MAIN.得意先ＣＤ,
    T.得意先名,
    CONVERT(varchar, MAIN.得意先ＣＤ) + ' ' + ISNULL(V.売掛現金区分名, ' ') + CHAR(10) + ISNULL(T.得意先名, '') AS 表示用得意先名,
    VT.売掛現金区分CD AS 売掛現金区分,
    VT.売掛現金区分名 AS タイプ,
    A.商品ＣＤ,
    A.金額 AS 値
FROM
    -- 駆動表（注文明細）
    (SELECT
        M.得意先ＣＤ
        ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_C1' AS 商品ＣＤ
    FROM
        注文ヘッダ AS H
        INNER JOIN 注文明細 AS M
            ON  H.企業ＣＤ   = M.企業ＣＤ
            AND H.注文区分   = M.注文区分
            AND H.喫食日     = M.喫食日
            AND H.得意先ＣＤ = M.得意先ＣＤ
            AND H.部署ＣＤ   = M.部署ＣＤ
        LEFT JOIN 主要商品リストマスタ AS A
            ON A.企業ＣＤ = M.企業ＣＤ
            AND A.商品ＣＤ = M.商品ＣＤ
            AND A.部署ＣＤ = M.部署ＣＤ
            AND ISNULL(A.無効フラグ, 0) = 0
    WHERE
        H.企業ＣＤ = @企業ＣＤ AND
        H.注文区分 = @注文区分注文 AND
        H.喫食日 = @喫食日 AND
        H.部署ＣＤ = @部署ＣＤ AND
        H.コースＣＤ IS NULL
    ) AS MAIN
    LEFT JOIN
    -- 注文明細（現金）（レコードがヒットしなくても得意先名とタイプは表示される）
    (SELECT
        H.得意先ＣＤ
        ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_C1' AS 商品ＣＤ
        ,SUM(M.商品金額合計税込) AS 金額
    FROM
        注文ヘッダ AS H
        INNER JOIN 注文明細 AS M
            ON  H.企業ＣＤ   = M.企業ＣＤ
            AND H.注文区分   = M.注文区分
            AND H.喫食日     = M.喫食日
            AND H.得意先ＣＤ = M.得意先ＣＤ
            AND H.部署ＣＤ   = M.部署ＣＤ
        LEFT JOIN 主要商品リストマスタ AS A
            ON A.企業ＣＤ = M.企業ＣＤ
            AND A.商品ＣＤ = M.商品ＣＤ
            AND A.部署ＣＤ = M.部署ＣＤ
            AND ISNULL(A.無効フラグ, 0) = 0
    WHERE
        H.企業ＣＤ = @企業ＣＤ AND
        H.注文区分 = @注文区分注文 AND
        H.喫食日 = @喫食日 AND
        H.部署ＣＤ = @部署ＣＤ AND
        H.コースＣＤ IS NULL AND
        M.売掛現金区分 = @売掛現金区分現金
    GROUP BY
        H.得意先ＣＤ
        ,A.商品ＣＤ
    ) AS A
        ON MAIN.得意先ＣＤ = A.得意先ＣＤ
        AND MAIN.商品ＣＤ   = A.商品ＣＤ
    LEFT JOIN 得意先マスタ  AS T ON T.企業ＣＤ = @企業ＣＤ AND T.得意先ＣＤ = MAIN.得意先ＣＤ
    LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分
    LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分現金
UNION
--- 注文売掛個数
SELECT
    0 AS 明細合計区分,
    MAIN.得意先ＣＤ,
    T.得意先名,
    CONVERT(varchar, MAIN.得意先ＣＤ) + ' ' + ISNULL(V.売掛現金区分名, ' ') + CHAR(10) + ISNULL(T.得意先名, '') AS 表示用得意先名,
    VT.売掛現金区分CD AS 売掛現金区分,
    VT.売掛現金区分名 AS タイプ,
    A.商品ＣＤ,
    A.個数 AS 値
FROM
    -- 駆動表（注文明細）
    (SELECT
        M.得意先ＣＤ
        ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_C0' AS 商品ＣＤ
    FROM
        注文ヘッダ AS H
        INNER JOIN 注文明細 AS M
            ON  H.企業ＣＤ   = M.企業ＣＤ
            AND H.注文区分   = M.注文区分
            AND H.喫食日     = M.喫食日
            AND H.得意先ＣＤ = M.得意先ＣＤ
            AND H.部署ＣＤ   = M.部署ＣＤ
        LEFT JOIN 主要商品リストマスタ AS A
            ON A.企業ＣＤ = M.企業ＣＤ
            AND A.商品ＣＤ = M.商品ＣＤ
            AND A.部署ＣＤ = M.部署ＣＤ
            AND ISNULL(A.無効フラグ, 0) = 0
    WHERE
        H.企業ＣＤ = @企業ＣＤ AND
        H.注文区分 = @注文区分注文 AND
        H.喫食日 = @喫食日 AND
        H.部署ＣＤ = @部署ＣＤ AND
        H.コースＣＤ IS NULL
    ) AS MAIN
    LEFT JOIN
    -- 注文明細（売掛）（レコードがヒットしなくても得意先名とタイプは表示される）
    (SELECT
        H.得意先ＣＤ
        ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_C0' AS 商品ＣＤ
        ,SUM(M.商品数量) AS 個数
    FROM
        注文ヘッダ AS H
        INNER JOIN 注文明細 AS M
            ON  H.企業ＣＤ   = M.企業ＣＤ
            AND H.注文区分   = M.注文区分
            AND H.喫食日     = M.喫食日
            AND H.得意先ＣＤ = M.得意先ＣＤ
            AND H.部署ＣＤ   = M.部署ＣＤ
        LEFT JOIN 主要商品リストマスタ AS A
            ON A.企業ＣＤ = M.企業ＣＤ
            AND A.商品ＣＤ = M.商品ＣＤ
            AND A.部署ＣＤ = M.部署ＣＤ
            AND ISNULL(A.無効フラグ, 0) = 0
    WHERE
        H.企業ＣＤ = @企業ＣＤ AND
        H.注文区分 = @注文区分注文 AND
        H.喫食日 = @喫食日 AND
        H.部署ＣＤ = @部署ＣＤ AND
        H.コースＣＤ IS NULL AND
        M.売掛現金区分 <> @売掛現金区分現金     -- 売掛、LINE決済
    GROUP BY
        H.得意先ＣＤ
        ,A.商品ＣＤ
    ) AS A
        ON MAIN.得意先ＣＤ = A.得意先ＣＤ
        AND MAIN.商品ＣＤ   = A.商品ＣＤ
    LEFT JOIN 得意先マスタ  AS T ON T.企業ＣＤ = @企業ＣＤ AND T.得意先ＣＤ = MAIN.得意先ＣＤ
    LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分
    LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分売掛
UNION
--- 注文売掛金額
SELECT
    0 AS 明細合計区分,
    MAIN.得意先ＣＤ,
    T.得意先名,
    CONVERT(varchar, MAIN.得意先ＣＤ) + ' ' + ISNULL(V.売掛現金区分名, ' ') + CHAR(10) + ISNULL(T.得意先名, '') AS 表示用得意先名,
    VT.売掛現金区分CD AS 売掛現金区分,
    VT.売掛現金区分名 AS タイプ,
    A.商品ＣＤ,
    A.金額 AS 値
FROM
    -- 駆動表（注文明細）
    (SELECT
        M.得意先ＣＤ
        ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_C1' AS 商品ＣＤ
    FROM
        注文ヘッダ AS H
        INNER JOIN 注文明細 AS M
            ON  H.企業ＣＤ   = M.企業ＣＤ
            AND H.注文区分   = M.注文区分
            AND H.喫食日     = M.喫食日
            AND H.得意先ＣＤ = M.得意先ＣＤ
            AND H.部署ＣＤ   = M.部署ＣＤ
        LEFT JOIN 主要商品リストマスタ AS A
            ON A.企業ＣＤ = M.企業ＣＤ
            AND A.商品ＣＤ = M.商品ＣＤ
            AND A.部署ＣＤ = M.部署ＣＤ
            AND ISNULL(A.無効フラグ, 0) = 0
    WHERE
        H.企業ＣＤ = @企業ＣＤ AND
        H.注文区分 = @注文区分注文 AND
        H.喫食日 = @喫食日 AND
        H.部署ＣＤ = @部署ＣＤ AND
        H.コースＣＤ IS NULL
    ) AS MAIN
    LEFT JOIN
    -- 注文明細（売掛）（レコードがヒットしなくても得意先名とタイプは表示される）
    (SELECT
        H.得意先ＣＤ
        ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_C1' AS 商品ＣＤ
        ,SUM(M.商品金額合計税込) AS 金額
    FROM
        注文ヘッダ AS H
        INNER JOIN 注文明細 AS M
            ON  H.企業ＣＤ   = M.企業ＣＤ
            AND H.注文区分   = M.注文区分
            AND H.喫食日     = M.喫食日
            AND H.得意先ＣＤ = M.得意先ＣＤ
            AND H.部署ＣＤ   = M.部署ＣＤ
        LEFT JOIN 主要商品リストマスタ AS A
            ON A.企業ＣＤ = M.企業ＣＤ
            AND A.商品ＣＤ = M.商品ＣＤ
            AND A.部署ＣＤ = M.部署ＣＤ
            AND ISNULL(A.無効フラグ, 0) = 0
    WHERE
        H.企業ＣＤ = @企業ＣＤ AND
        H.注文区分 = @注文区分注文 AND
        H.喫食日 = @喫食日 AND
        H.部署ＣＤ = @部署ＣＤ AND
        H.コースＣＤ IS NULL AND
        M.売掛現金区分 <> @売掛現金区分現金     -- 売掛、LINE決済
    GROUP BY
        H.得意先ＣＤ
        ,A.商品ＣＤ
    ) AS A
        ON MAIN.得意先ＣＤ = A.得意先ＣＤ
        AND MAIN.商品ＣＤ   = A.商品ＣＤ
    LEFT JOIN 得意先マスタ  AS T ON T.企業ＣＤ = @企業ＣＤ AND T.得意先ＣＤ = MAIN.得意先ＣＤ
    LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分
    LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分売掛
UNION
--- 売上現金個数
SELECT
    0 AS 明細合計区分,
    MAIN.得意先ＣＤ,
    T.得意先名,
    CONVERT(varchar, MAIN.得意先ＣＤ) + ' ' + ISNULL(V.売掛現金区分名, ' ') + CHAR(10) + ISNULL(T.得意先名, '') AS 表示用得意先名,
    VT.売掛現金区分CD AS 売掛現金区分,
    VT.売掛現金区分名 AS タイプ,
    A.商品ＣＤ,
    A.個数 AS 値
FROM
    -- 駆動表（売上明細）
    (SELECT DISTINCT
        M.得意先ＣＤ
        ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_U0' AS 商品ＣＤ
    FROM
        売上明細 AS M
        INNER JOIN 売上ヘッダ AS H
            ON  H.企業ＣＤ   = M.企業ＣＤ
            AND H.売上日     = M.売上日
            AND H.得意先ＣＤ = M.得意先ＣＤ
            AND H.部署ＣＤ   = M.部署ＣＤ
        LEFT JOIN 主要商品リストマスタ AS A
            ON A.企業ＣＤ = M.企業ＣＤ
            AND A.商品ＣＤ = M.商品ＣＤ
            AND A.部署ＣＤ = M.部署ＣＤ
            AND ISNULL(A.無効フラグ, 0) = 0
    WHERE
        H.企業ＣＤ = @企業ＣＤ AND
        H.売上日 = @喫食日 AND
        H.部署ＣＤ = @部署ＣＤ AND
        H.コースＣＤ IS NULL
    ) AS MAIN
    LEFT JOIN
    -- 売上明細（現金）（レコードがヒットしなくても得意先名とタイプは表示される）
    (SELECT
        M.得意先ＣＤ
        ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_U0' AS 商品ＣＤ
        ,SUM(M.商品数量) AS 個数
    FROM
        売上明細 AS M
        INNER JOIN 売上ヘッダ AS H
            ON  H.企業ＣＤ   = M.企業ＣＤ
            AND H.売上日     = M.売上日
            AND H.得意先ＣＤ = M.得意先ＣＤ
            AND H.部署ＣＤ   = M.部署ＣＤ
        LEFT JOIN 主要商品リストマスタ AS A
            ON A.企業ＣＤ = M.企業ＣＤ
            AND A.商品ＣＤ = M.商品ＣＤ
            AND A.部署ＣＤ = M.部署ＣＤ
            AND ISNULL(A.無効フラグ, 0) = 0
    WHERE
        M.企業ＣＤ = @企業ＣＤ AND
        M.売上区分 NOT IN (@売上区分売掛, @売上区分LINE) AND        -- 現金、チケット、バークレー、ボーナスは現金と見なす
        M.売上日 = @喫食日 AND
        M.部署ＣＤ = @部署ＣＤ AND
        H.コースＣＤ IS NULL
    GROUP BY
        M.得意先ＣＤ
        ,A.商品ＣＤ
    ) AS A
        ON MAIN.得意先ＣＤ = A.得意先ＣＤ
        AND MAIN.商品ＣＤ   = A.商品ＣＤ
    LEFT JOIN 得意先マスタ  AS T ON T.企業ＣＤ = @企業ＣＤ AND T.得意先ＣＤ = MAIN.得意先ＣＤ
    LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分
    LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分現金
UNION
--- 売上現金金額
SELECT
    0 AS 明細合計区分,
    MAIN.得意先ＣＤ,
    T.得意先名,
    CONVERT(varchar, MAIN.得意先ＣＤ) + ' ' + ISNULL(V.売掛現金区分名, ' ') + CHAR(10) + ISNULL(T.得意先名, '') AS 表示用得意先名,
    VT.売掛現金区分CD AS 売掛現金区分,
    VT.売掛現金区分名 AS タイプ,
    A.商品ＣＤ,
    A.金額 AS 値
FROM
    -- 駆動表（売上明細）
    (SELECT DISTINCT
        M.得意先ＣＤ
        ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_U1' AS 商品ＣＤ
    FROM
        売上明細 AS M
        INNER JOIN 売上ヘッダ AS H
            ON  H.企業ＣＤ   = M.企業ＣＤ
            AND H.売上日     = M.売上日
            AND H.得意先ＣＤ = M.得意先ＣＤ
            AND H.部署ＣＤ   = M.部署ＣＤ
        LEFT JOIN 主要商品リストマスタ AS A
            ON A.企業ＣＤ = M.企業ＣＤ
            AND A.商品ＣＤ = M.商品ＣＤ
            AND A.部署ＣＤ = M.部署ＣＤ
            AND ISNULL(A.無効フラグ, 0) = 0
    WHERE
        H.企業ＣＤ = @企業ＣＤ AND
        H.売上日 = @喫食日 AND
        H.部署ＣＤ = @部署ＣＤ AND
        H.コースＣＤ IS NULL
    ) AS MAIN
    LEFT JOIN
    -- 売上明細（現金）（レコードがヒットしなくても得意先名とタイプは表示される）
    (SELECT
        M.得意先ＣＤ
        ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_U1' AS 商品ＣＤ
        ,SUM(M.商品金額合計税込) AS 金額
    FROM
        売上明細 AS M
        INNER JOIN 売上ヘッダ AS H
            ON  H.企業ＣＤ   = M.企業ＣＤ
            AND H.売上日     = M.売上日
            AND H.得意先ＣＤ = M.得意先ＣＤ
            AND H.部署ＣＤ   = M.部署ＣＤ
        LEFT JOIN 主要商品リストマスタ AS A
            ON A.企業ＣＤ = M.企業ＣＤ
            AND A.商品ＣＤ = M.商品ＣＤ
            AND A.部署ＣＤ = M.部署ＣＤ
            AND ISNULL(A.無効フラグ, 0) = 0
    WHERE
        M.企業ＣＤ = @企業ＣＤ AND
        M.売上区分 NOT IN (@売上区分売掛, @売上区分LINE) AND        -- 現金、チケット、バークレー、ボーナスは現金と見なす
        M.売上日 = @喫食日 AND
        M.部署ＣＤ = @部署ＣＤ AND
        H.コースＣＤ IS NULL
    GROUP BY
        M.得意先ＣＤ
        ,A.商品ＣＤ
    ) AS A
        ON MAIN.得意先ＣＤ = A.得意先ＣＤ
        AND MAIN.商品ＣＤ   = A.商品ＣＤ
    LEFT JOIN 得意先マスタ  AS T ON T.企業ＣＤ = @企業ＣＤ AND T.得意先ＣＤ = MAIN.得意先ＣＤ
    LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分
    LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分現金
UNION
--- 売上売掛個数
SELECT
    0 AS 明細合計区分,
    MAIN.得意先ＣＤ,
    T.得意先名,
    CONVERT(varchar, MAIN.得意先ＣＤ) + ' ' + ISNULL(V.売掛現金区分名, ' ') + CHAR(10) + ISNULL(T.得意先名, '') AS 表示用得意先名,
    VT.売掛現金区分CD AS 売掛現金区分,
    VT.売掛現金区分名 AS タイプ,
    A.商品ＣＤ,
    A.個数 AS 値
FROM
    -- 駆動表（売上明細）
    (SELECT DISTINCT
        M.得意先ＣＤ
        ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_U0' AS 商品ＣＤ
    FROM
        売上明細 AS M
        INNER JOIN 売上ヘッダ AS H
            ON  H.企業ＣＤ   = M.企業ＣＤ
            AND H.売上日     = M.売上日
            AND H.得意先ＣＤ = M.得意先ＣＤ
            AND H.部署ＣＤ   = M.部署ＣＤ
        LEFT JOIN 主要商品リストマスタ AS A
            ON A.企業ＣＤ = M.企業ＣＤ
            AND A.商品ＣＤ = M.商品ＣＤ
            AND A.部署ＣＤ = M.部署ＣＤ
            AND ISNULL(A.無効フラグ, 0) = 0
    WHERE
        H.企業ＣＤ = @企業ＣＤ AND
        H.売上日 = @喫食日 AND
        H.部署ＣＤ = @部署ＣＤ AND
        H.コースＣＤ IS NULL
    ) AS MAIN
    LEFT JOIN
    -- 売上明細（売掛）（レコードがヒットしなくても得意先名とタイプは表示される）
    (SELECT
        M.得意先ＣＤ
        ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_U0' AS 商品ＣＤ
        ,SUM(M.商品数量) AS 個数
    FROM
        売上明細 AS M
        INNER JOIN 売上ヘッダ AS H
            ON  H.企業ＣＤ   = M.企業ＣＤ
            AND H.売上日     = M.売上日
            AND H.得意先ＣＤ = M.得意先ＣＤ
            AND H.部署ＣＤ   = M.部署ＣＤ
        LEFT JOIN 主要商品リストマスタ AS A
            ON A.企業ＣＤ = M.企業ＣＤ
            AND A.商品ＣＤ = M.商品ＣＤ
            AND A.部署ＣＤ = M.部署ＣＤ
            AND ISNULL(A.無効フラグ, 0) = 0
    WHERE
        M.企業ＣＤ = @企業ＣＤ AND
        M.売上区分 IN (@売上区分売掛, @売上区分LINE) AND        -- LINE決済は売掛と見なす
        M.売上日 = @喫食日 AND
        M.部署ＣＤ = @部署ＣＤ AND
        H.コースＣＤ IS NULL
    GROUP BY
        M.得意先ＣＤ
        ,A.商品ＣＤ
    ) AS A
        ON MAIN.得意先ＣＤ = A.得意先ＣＤ
        AND MAIN.商品ＣＤ   = A.商品ＣＤ
    LEFT JOIN 得意先マスタ  AS T ON T.企業ＣＤ = @企業ＣＤ AND T.得意先ＣＤ = MAIN.得意先ＣＤ
    LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分
    LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分売掛
UNION
--- 売上売掛金額
SELECT
    0 AS 明細合計区分,
    MAIN.得意先ＣＤ,
    T.得意先名,
    CONVERT(varchar, MAIN.得意先ＣＤ) + ' ' + ISNULL(V.売掛現金区分名, ' ') + CHAR(10) + ISNULL(T.得意先名, '') AS 表示用得意先名,
    VT.売掛現金区分CD AS 売掛現金区分,
    VT.売掛現金区分名 AS タイプ,
    A.商品ＣＤ,
    A.金額 AS 値
FROM
    -- 駆動表（売上明細）
    (SELECT DISTINCT
        M.得意先ＣＤ
        ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_U1' AS 商品ＣＤ
    FROM
        売上明細 AS M
        INNER JOIN 売上ヘッダ AS H
            ON  H.企業ＣＤ   = M.企業ＣＤ
            AND H.売上日     = M.売上日
            AND H.得意先ＣＤ = M.得意先ＣＤ
            AND H.部署ＣＤ   = M.部署ＣＤ
        LEFT JOIN 主要商品リストマスタ AS A
            ON A.企業ＣＤ = M.企業ＣＤ
            AND A.商品ＣＤ = M.商品ＣＤ
            AND A.部署ＣＤ = M.部署ＣＤ
            AND ISNULL(A.無効フラグ, 0) = 0
    WHERE
        H.企業ＣＤ = @企業ＣＤ AND
        H.売上日 = @喫食日 AND
        H.部署ＣＤ = @部署ＣＤ AND
        H.コースＣＤ IS NULL
    ) AS MAIN
    LEFT JOIN
    -- 売上明細（売掛）（レコードがヒットしなくても得意先名とタイプは表示される）
    (SELECT
        M.得意先ＣＤ
        ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_U1' AS 商品ＣＤ
        ,SUM(M.商品金額合計税込) AS 金額
    FROM
        売上明細 AS M
        INNER JOIN 売上ヘッダ AS H
            ON  H.企業ＣＤ   = M.企業ＣＤ
            AND H.売上日     = M.売上日
            AND H.得意先ＣＤ = M.得意先ＣＤ
            AND H.部署ＣＤ   = M.部署ＣＤ
        LEFT JOIN 主要商品リストマスタ AS A
            ON A.企業ＣＤ = M.企業ＣＤ
            AND A.商品ＣＤ = M.商品ＣＤ
            AND A.部署ＣＤ = M.部署ＣＤ
            AND ISNULL(A.無効フラグ, 0) = 0
    WHERE
        M.企業ＣＤ = @企業ＣＤ AND
        M.売上区分 IN (@売上区分売掛, @売上区分LINE) AND        -- LINE決済は売掛と見なす
        M.売上日 = @喫食日 AND
        M.部署ＣＤ = @部署ＣＤ AND
        H.コースＣＤ IS NULL
    GROUP BY
        M.得意先ＣＤ
        ,A.商品ＣＤ
    ) AS A
        ON MAIN.得意先ＣＤ = A.得意先ＣＤ
        AND MAIN.商品ＣＤ   = A.商品ＣＤ
    LEFT JOIN 得意先マスタ  AS T ON T.企業ＣＤ = @企業ＣＤ AND T.得意先ＣＤ = MAIN.得意先ＣＤ
    LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分
    LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分売掛
UNION
--- 現金合計
SELECT
    1 AS 明細合計区分,
    -1 AS 得意先ＣＤ,
    NULL AS 得意先名,
    '合計' AS 表示用得意先名,
    VT.売掛現金区分CD AS 売掛現金区分,
    VT.売掛現金区分名 AS タイプ,
    NULL AS 商品ＣＤ,
    NULL AS 値
FROM
    V_売掛現金区分 AS VT
WHERE
    VT.売掛現金区分CD = @売掛現金区分現金
UNION
--- 売掛合計
SELECT
    1 AS 明細合計区分,
    -1 AS 得意先ＣＤ,
    NULL AS 得意先名,
    '合計' AS 表示用得意先名,
    VT.売掛現金区分CD AS 売掛現金区分,
    VT.売掛現金区分名 AS タイプ,
    NULL AS 商品ＣＤ,
    NULL AS 値
FROM
    V_売掛現金区分 AS VT
WHERE
    VT.売掛現金区分CD = @売掛現金区分売掛
ORDER BY
    明細合計区分,
    得意先ＣＤ,
    売掛現金区分,
    商品ＣＤ

@売掛現金区分現金 = 0
@売掛現金区分売掛 = 1
@売掛現金区分LINE = 5
@売上区分売掛 = 1
@売上区分LINE = 5
