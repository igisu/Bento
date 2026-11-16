-- コース売上一覧
--- 売上現金個数
SELECT
    0 AS 明細合計区分,
    C.得意先ＣＤ,
    T.得意先名,
    CONVERT(varchar, C.得意先ＣＤ) + ' ' + ISNULL(V.売掛現金区分名, ' ') + CHAR(10) + ISNULL(T.得意先名, '') AS 表示用得意先名,
    VT.売掛現金区分CD AS 売掛現金区分,
    VT.売掛現金区分名 AS タイプ,
    A.商品ＣＤ,
    A.個数 AS 値,
    CASE WHEN CO.コース順 IS NULL THEN 1 ELSE 0 END AS コースに含む順,
    CO.コース順
FROM
    -- ①駆動表
    -- ①－１（指定されたコースにぶら下がる得意先）
    -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
    (SELECT
        CT.企業ＣＤ
        ,CT.コースＣＤ
        ,CT.得意先ＣＤ
    FROM
        コーステーブル AS CT
        INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0
        LEFT JOIN
            (SELECT
                コースＣＤ
            FROM
                臨時コースマスタ
            WHERE
                企業ＣＤ = @企業ＣＤ
                AND ISNULL(無効フラグ, 0) = 0
                AND 実施日 = @売上日
            ) AS RM
            ON RM.コースＣＤ = CT.コースＣＤ
    WHERE
        CT.企業ＣＤ = @企業ＣＤ
        AND ISNULL(CT.無効フラグ, 0) = 0
        AND RM.コースＣＤ IS NULL
    -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
    UNION
    SELECT
        RT.企業ＣＤ
        ,RT.コースＣＤ
        ,RT.得意先ＣＤ
    FROM
        臨時コーステーブル AS RT
        INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0
    WHERE
        RT.企業ＣＤ = @企業ＣＤ
        AND ISNULL(RT.無効フラグ, 0) = 0
        AND RT.実施日 = @売上日
    -- ①－２（指定されたコースを持つ注文の得意先）
    UNION
    SELECT
        企業ＣＤ
        ,コースＣＤ
        ,得意先ＣＤ
    FROM
        注文ヘッダ
    WHERE
        企業ＣＤ = @企業ＣＤ
        AND 喫食日 = @売上日
        AND 部署ＣＤ = @部署ＣＤ
    -- ①－３（指定されたコースを持つ売上の得意先）
    UNION
    SELECT
        企業ＣＤ
        ,コースＣＤ
        ,得意先ＣＤ
    FROM
        売上ヘッダ
    WHERE
        企業ＣＤ = @企業ＣＤ
        AND 売上日 = @売上日
        AND 部署ＣＤ = @部署ＣＤ
    ) AS C
    LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ
    LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分
    LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分現金
    LEFT JOIN (
        SELECT
            H.コースＣＤ,
            T.受注得意先ＣＤ AS 得意先ＣＤ,
            CONVERT(varchar, M.商品ＣＤ) + '_0' AS 商品ＣＤ,
            SUM(M.商品数量) AS 個数
        FROM
            売上明細 AS M
            -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
            -- （分配することによって分配先のコースは分配元と同じになっているので、分配元と分配先が一緒に抽出される）
            INNER JOIN (
                SELECT
                    企業ＣＤ
                    ,得意先ＣＤ
                    ,ISNULL(受注得意先ＣＤ, 得意先ＣＤ) AS 受注得意先ＣＤ
                FROM
                    得意先マスタ
                ) AS T
                ON T.企業ＣＤ  = M.企業ＣＤ
                AND T.得意先ＣＤ = M.得意先ＣＤ
            INNER JOIN 売上ヘッダ AS H
                ON  H.企業ＣＤ   = M.企業ＣＤ
                AND H.売上日     = M.売上日
                AND H.得意先ＣＤ = M.得意先ＣＤ
                AND H.部署ＣＤ   = M.部署ＣＤ
        WHERE
            M.企業ＣＤ = @企業ＣＤ AND
            M.売上区分 NOT IN (@売上区分売掛, @売上区分LINE) AND        -- 現金、チケット、バークレー、ボーナスは現金と見なす
            M.売上日   = @売上日 AND
            M.部署ＣＤ = @部署ＣＤ
        GROUP BY
            H.コースＣＤ,
            T.受注得意先ＣＤ,
            M.商品ＣＤ
        ) AS A ON A.コースＣＤ = C.コースＣＤ AND A.得意先ＣＤ = C.得意先ＣＤ
-- ②コース順（①で取得していないのはなぜ？①－２からはコース順が取得できないので代わりにNULLにするとUNIONしたときに複数行できてしまうため）
    LEFT JOIN
        (SELECT
            CT.得意先ＣＤ
            ,CT.コース順
        FROM
            コーステーブル AS CT
            INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0
            LEFT JOIN
                (SELECT
                    コースＣＤ
                FROM
                    臨時コースマスタ
                WHERE
                    企業ＣＤ = @企業ＣＤ
                    AND ISNULL(無効フラグ, 0) = 0
                    AND コースＣＤ = @コースＣＤ
                    AND 実施日 = @売上日
                ) AS RM
                ON RM.コースＣＤ = CT.コースＣＤ
        WHERE
            CT.企業ＣＤ = @企業ＣＤ
            AND CT.コースＣＤ = @コースＣＤ
            AND ISNULL(CT.無効フラグ, 0) = 0
            AND RM.コースＣＤ IS NULL
        -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
        UNION
        SELECT
            RT.得意先ＣＤ
            ,RT.コース順
        FROM
            臨時コーステーブル AS RT
            INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0
        WHERE
            RT.企業ＣＤ = @企業ＣＤ
            AND RT.コースＣＤ = @コースＣＤ
            AND ISNULL(RT.無効フラグ, 0) = 0
            AND RT.実施日 = @売上日
        ) AS CO ON C.得意先ＣＤ = CO.得意先ＣＤ
WHERE
    C.企業ＣＤ = @企業ＣＤ
    AND C.コースＣＤ = @コースＣＤ
UNION
--- 売上現金金額
SELECT
    0 AS 明細合計区分,
    C.得意先ＣＤ,
    T.得意先名,
    CONVERT(varchar, C.得意先ＣＤ) + ' ' + ISNULL(V.売掛現金区分名, ' ') + CHAR(10) + ISNULL(T.得意先名, '') AS 表示用得意先名,
    VT.売掛現金区分CD AS 売掛現金区分,
    VT.売掛現金区分名 AS タイプ,
    A.商品ＣＤ,
    A.金額 AS 値,
    CASE WHEN CO.コース順 IS NULL THEN 1 ELSE 0 END AS コースに含む順,
    CO.コース順
FROM
    -- ①駆動表
    -- ①－１（指定されたコースにぶら下がる得意先）
    -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
    (SELECT
        CT.企業ＣＤ
        ,CT.コースＣＤ
        ,CT.得意先ＣＤ
    FROM
        コーステーブル AS CT
        INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0
        LEFT JOIN
            (SELECT
                コースＣＤ
            FROM
                臨時コースマスタ
            WHERE
                企業ＣＤ = @企業ＣＤ
                AND ISNULL(無効フラグ, 0) = 0
                AND 実施日 = @売上日
            ) AS RM
            ON RM.コースＣＤ = CT.コースＣＤ
    WHERE
        CT.企業ＣＤ = @企業ＣＤ
        AND ISNULL(CT.無効フラグ, 0) = 0
        AND RM.コースＣＤ IS NULL
    -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
    UNION
    SELECT
        RT.企業ＣＤ
        ,RT.コースＣＤ
        ,RT.得意先ＣＤ
    FROM
        臨時コーステーブル AS RT
        INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0
    WHERE
        RT.企業ＣＤ = @企業ＣＤ
        AND ISNULL(RT.無効フラグ, 0) = 0
        AND RT.実施日 = @売上日
    -- ①－２（指定されたコースを持つ注文の得意先）
    UNION
    SELECT
        企業ＣＤ
        ,コースＣＤ
        ,得意先ＣＤ
    FROM
        注文ヘッダ
    WHERE
        企業ＣＤ = @企業ＣＤ
        AND 喫食日 = @売上日
        AND 部署ＣＤ = @部署ＣＤ
    -- ①－３（指定されたコースを持つ売上の得意先）
    UNION
    SELECT
        企業ＣＤ
        ,コースＣＤ
        ,得意先ＣＤ
    FROM
        売上ヘッダ
    WHERE
        企業ＣＤ = @企業ＣＤ
        AND 売上日 = @売上日
        AND 部署ＣＤ = @部署ＣＤ
    ) AS C
    LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ
    LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分
    LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分現金
    LEFT JOIN (
        SELECT
            H.コースＣＤ,
            T.受注得意先ＣＤ AS 得意先ＣＤ,
            CONVERT(varchar, M.商品ＣＤ) + '_1' AS 商品ＣＤ,
            SUM(M.商品金額合計税込) AS 金額
        FROM
            売上明細 AS M
            -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
            -- （分配することによって分配先のコースは分配元と同じになっているので、分配元と分配先が一緒に抽出される）
            INNER JOIN (
                SELECT
                    企業ＣＤ
                    ,得意先ＣＤ
                    ,ISNULL(受注得意先ＣＤ, 得意先ＣＤ) AS 受注得意先ＣＤ
                FROM
                    得意先マスタ
                ) AS T
                ON T.企業ＣＤ  = M.企業ＣＤ
                AND T.得意先ＣＤ = M.得意先ＣＤ
            INNER JOIN 売上ヘッダ AS H
                ON  H.企業ＣＤ   = M.企業ＣＤ
                AND H.売上日     = M.売上日
                AND H.得意先ＣＤ = M.得意先ＣＤ
                AND H.部署ＣＤ   = M.部署ＣＤ
        WHERE
            M.企業ＣＤ = @企業ＣＤ AND
            M.売上区分 NOT IN (@売上区分売掛, @売上区分LINE) AND        -- 現金、チケット、バークレー、ボーナスは現金と見なす
            M.売上日   = @売上日 AND
            M.部署ＣＤ = @部署ＣＤ
        GROUP BY
            H.コースＣＤ,
            T.受注得意先ＣＤ,
            M.商品ＣＤ
        ) AS A ON A.コースＣＤ = C.コースＣＤ AND A.得意先ＣＤ = C.得意先ＣＤ
-- ②コース順（①で取得していないのはなぜ？①－２からはコース順が取得できないので代わりにNULLにするとUNIONしたときに複数行できてしまうため）
    LEFT JOIN
        (SELECT
            CT.得意先ＣＤ
            ,CT.コース順
        FROM
            コーステーブル AS CT
            INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0
            LEFT JOIN
                (SELECT
                    コースＣＤ
                FROM
                    臨時コースマスタ
                WHERE
                    企業ＣＤ = @企業ＣＤ
                    AND ISNULL(無効フラグ, 0) = 0
                    AND コースＣＤ = @コースＣＤ
                    AND 実施日 = @売上日
                ) AS RM
                ON RM.コースＣＤ = CT.コースＣＤ
        WHERE
            CT.企業ＣＤ = @企業ＣＤ
            AND CT.コースＣＤ = @コースＣＤ
            AND ISNULL(CT.無効フラグ, 0) = 0
            AND RM.コースＣＤ IS NULL
        -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
        UNION
        SELECT
            RT.得意先ＣＤ
            ,RT.コース順
        FROM
            臨時コーステーブル AS RT
            INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0
        WHERE
            RT.企業ＣＤ = @企業ＣＤ
            AND RT.コースＣＤ = @コースＣＤ
            AND ISNULL(RT.無効フラグ, 0) = 0
            AND RT.実施日 = @売上日
        ) AS CO ON C.得意先ＣＤ = CO.得意先ＣＤ
WHERE
    C.企業ＣＤ = @企業ＣＤ
    AND C.コースＣＤ = @コースＣＤ
UNION
--- 売上売掛個数
SELECT
    0 AS 明細合計区分,
    C.得意先ＣＤ,
    T.得意先名,
    CONVERT(varchar, C.得意先ＣＤ) + ' ' + ISNULL(V.売掛現金区分名, ' ') + CHAR(10) + ISNULL(T.得意先名, '') AS 表示用得意先名,
    VT.売掛現金区分CD AS 売掛現金区分,
    VT.売掛現金区分名 AS タイプ,
    A.商品ＣＤ,
    A.個数 AS 値,
    CASE WHEN CO.コース順 IS NULL THEN 1 ELSE 0 END AS コースに含む順,
    CO.コース順
FROM
    -- ①駆動表
    -- ①－１（指定されたコースにぶら下がる得意先）
    -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
    (SELECT
        CT.企業ＣＤ
        ,CT.コースＣＤ
        ,CT.得意先ＣＤ
    FROM
        コーステーブル AS CT
        INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0
        LEFT JOIN
            (SELECT
                コースＣＤ
            FROM
                臨時コースマスタ
            WHERE
                企業ＣＤ = @企業ＣＤ
                AND ISNULL(無効フラグ, 0) = 0
                AND 実施日 = @売上日
            ) AS RM
            ON RM.コースＣＤ = CT.コースＣＤ
    WHERE
        CT.企業ＣＤ = @企業ＣＤ
        AND ISNULL(CT.無効フラグ, 0) = 0
        AND RM.コースＣＤ IS NULL
    -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
    UNION
    SELECT
        RT.企業ＣＤ
        ,RT.コースＣＤ
        ,RT.得意先ＣＤ
    FROM
        臨時コーステーブル AS RT
        INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0
    WHERE
        RT.企業ＣＤ = @企業ＣＤ
        AND ISNULL(RT.無効フラグ, 0) = 0
        AND RT.実施日 = @売上日
    -- ①－２（指定されたコースを持つ注文の得意先）
    UNION
    SELECT
        企業ＣＤ
        ,コースＣＤ
        ,得意先ＣＤ
    FROM
        注文ヘッダ
    WHERE
        企業ＣＤ = @企業ＣＤ
        AND 喫食日 = @売上日
        AND 部署ＣＤ = @部署ＣＤ
    -- ①－３（指定されたコースを持つ売上の得意先）
    UNION
    SELECT
        企業ＣＤ
        ,コースＣＤ
        ,得意先ＣＤ
    FROM
        売上ヘッダ
    WHERE
        企業ＣＤ = @企業ＣＤ
        AND 売上日 = @売上日
        AND 部署ＣＤ = @部署ＣＤ
    ) AS C
    LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ
    LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分
    LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分売掛
    LEFT JOIN (
        SELECT
            H.コースＣＤ,
            T.受注得意先ＣＤ AS 得意先ＣＤ,
            CONVERT(varchar, M.商品ＣＤ) + '_0' AS 商品ＣＤ,
            SUM(M.商品数量) AS 個数
        FROM
            売上明細 AS M
            -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
            -- （分配することによって分配先のコースは分配元と同じになっているので、分配元と分配先が一緒に抽出される）
            INNER JOIN (
                SELECT
                    企業ＣＤ
                    ,得意先ＣＤ
                    ,ISNULL(受注得意先ＣＤ, 得意先ＣＤ) AS 受注得意先ＣＤ
                FROM
                    得意先マスタ
                ) AS T
                ON T.企業ＣＤ  = M.企業ＣＤ
                AND T.得意先ＣＤ = M.得意先ＣＤ
            INNER JOIN 売上ヘッダ AS H
                ON  H.企業ＣＤ   = M.企業ＣＤ
                AND H.売上日     = M.売上日
                AND H.得意先ＣＤ = M.得意先ＣＤ
                AND H.部署ＣＤ   = M.部署ＣＤ
        WHERE
            M.企業ＣＤ = @企業ＣＤ AND
            M.売上区分 IN (@売上区分売掛, @売上区分LINE) AND        -- LINE決済は売掛と見なす
            M.売上日   = @売上日 AND
            M.部署ＣＤ = @部署ＣＤ
        GROUP BY
            H.コースＣＤ,
            T.受注得意先ＣＤ,
            M.商品ＣＤ
        ) AS A ON A.コースＣＤ = C.コースＣＤ AND A.得意先ＣＤ = C.得意先ＣＤ
-- ②コース順（①で取得していないのはなぜ？①－２からはコース順が取得できないので代わりにNULLにするとUNIONしたときに複数行できてしまうため）
    LEFT JOIN
        (SELECT
            CT.得意先ＣＤ
            ,CT.コース順
        FROM
            コーステーブル AS CT
            INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0
            LEFT JOIN
                (SELECT
                    コースＣＤ
                FROM
                    臨時コースマスタ
                WHERE
                    企業ＣＤ = @企業ＣＤ
                    AND ISNULL(無効フラグ, 0) = 0
                    AND コースＣＤ = @コースＣＤ
                    AND 実施日 = @売上日
                ) AS RM
                ON RM.コースＣＤ = CT.コースＣＤ
        WHERE
            CT.企業ＣＤ = @企業ＣＤ
            AND CT.コースＣＤ = @コースＣＤ
            AND ISNULL(CT.無効フラグ, 0) = 0
            AND RM.コースＣＤ IS NULL
        -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
        UNION
        SELECT
            RT.得意先ＣＤ
            ,RT.コース順
        FROM
            臨時コーステーブル AS RT
            INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0
        WHERE
            RT.企業ＣＤ = @企業ＣＤ
            AND RT.コースＣＤ = @コースＣＤ
            AND ISNULL(RT.無効フラグ, 0) = 0
            AND RT.実施日 = @売上日
        ) AS CO ON C.得意先ＣＤ = CO.得意先ＣＤ
WHERE
    C.企業ＣＤ = @企業ＣＤ
    AND C.コースＣＤ = @コースＣＤ
UNION
--- 売上売掛金額
SELECT
    0 AS 明細合計区分,
    C.得意先ＣＤ,
    T.得意先名,
    CONVERT(varchar, C.得意先ＣＤ) + ' ' + ISNULL(V.売掛現金区分名, ' ') + CHAR(10) + ISNULL(T.得意先名, '') AS 表示用得意先名,
    VT.売掛現金区分CD AS 売掛現金区分,
    VT.売掛現金区分名 AS タイプ,
    A.商品ＣＤ,
    A.金額 AS 値,
    CASE WHEN CO.コース順 IS NULL THEN 1 ELSE 0 END AS コースに含む順,
    CO.コース順
FROM
    -- ①駆動表
    -- ①－１（指定されたコースにぶら下がる得意先）
    -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
    (SELECT
        CT.企業ＣＤ
        ,CT.コースＣＤ
        ,CT.得意先ＣＤ
    FROM
        コーステーブル AS CT
        INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0
        LEFT JOIN
            (SELECT
                コースＣＤ
            FROM
                臨時コースマスタ
            WHERE
                企業ＣＤ = @企業ＣＤ
                AND ISNULL(無効フラグ, 0) = 0
                AND 実施日 = @売上日
            ) AS RM
            ON RM.コースＣＤ = CT.コースＣＤ
    WHERE
        CT.企業ＣＤ = @企業ＣＤ
        AND ISNULL(CT.無効フラグ, 0) = 0
        AND RM.コースＣＤ IS NULL
    -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
    UNION
    SELECT
        RT.企業ＣＤ
        ,RT.コースＣＤ
        ,RT.得意先ＣＤ
    FROM
        臨時コーステーブル AS RT
        INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0
    WHERE
        RT.企業ＣＤ = @企業ＣＤ
        AND ISNULL(RT.無効フラグ, 0) = 0
        AND RT.実施日 = @売上日
    -- ①－２（指定されたコースを持つ注文の得意先）
    UNION
    SELECT
        企業ＣＤ
        ,コースＣＤ
        ,得意先ＣＤ
    FROM
        注文ヘッダ
    WHERE
        企業ＣＤ = @企業ＣＤ
        AND 喫食日 = @売上日
        AND 部署ＣＤ = @部署ＣＤ
    -- ①－３（指定されたコースを持つ売上の得意先）
    UNION
    SELECT
        企業ＣＤ
        ,コースＣＤ
        ,得意先ＣＤ
    FROM
        売上ヘッダ
    WHERE
        企業ＣＤ = @企業ＣＤ
        AND 売上日 = @売上日
        AND 部署ＣＤ = @部署ＣＤ
    ) AS C
    LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ
    LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分
    LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分売掛
    LEFT JOIN (
        SELECT
            H.コースＣＤ,
            T.受注得意先ＣＤ AS 得意先ＣＤ,
            CONVERT(varchar, M.商品ＣＤ) + '_1' AS 商品ＣＤ,
            SUM(M.商品金額合計税込) AS 金額
        FROM
            売上明細 AS M
            -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
            -- （分配することによって分配先のコースは分配元と同じになっているので、分配元と分配先が一緒に抽出される）
            INNER JOIN (
                SELECT
                    企業ＣＤ
                    ,得意先ＣＤ
                    ,ISNULL(受注得意先ＣＤ, 得意先ＣＤ) AS 受注得意先ＣＤ
                FROM
                    得意先マスタ
                ) AS T
                ON T.企業ＣＤ  = M.企業ＣＤ
                AND T.得意先ＣＤ = M.得意先ＣＤ
            INNER JOIN 売上ヘッダ AS H
                ON  H.企業ＣＤ   = M.企業ＣＤ
                AND H.売上日     = M.売上日
                AND H.得意先ＣＤ = M.得意先ＣＤ
                AND H.部署ＣＤ   = M.部署ＣＤ
        WHERE
            M.企業ＣＤ = @企業ＣＤ AND
            M.売上区分 IN (@売上区分売掛, @売上区分LINE) AND        -- LINE決済は売掛と見なす
            M.売上日   = @売上日 AND
            M.部署ＣＤ = @部署ＣＤ
        GROUP BY
            H.コースＣＤ,
            T.受注得意先ＣＤ,
            M.商品ＣＤ
        ) AS A ON A.コースＣＤ = C.コースＣＤ AND A.得意先ＣＤ = C.得意先ＣＤ
-- ②コース順（①で取得していないのはなぜ？①－２からはコース順が取得できないので代わりにNULLにするとUNIONしたときに複数行できてしまうため）
    LEFT JOIN
        (SELECT
            CT.得意先ＣＤ
            ,CT.コース順
        FROM
            コーステーブル AS CT
            INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0
            LEFT JOIN
                (SELECT
                    コースＣＤ
                FROM
                    臨時コースマスタ
                WHERE
                    企業ＣＤ = @企業ＣＤ
                    AND ISNULL(無効フラグ, 0) = 0
                    AND コースＣＤ = @コースＣＤ
                    AND 実施日 = @売上日
                ) AS RM
                ON RM.コースＣＤ = CT.コースＣＤ
        WHERE
            CT.企業ＣＤ = @企業ＣＤ
            AND CT.コースＣＤ = @コースＣＤ
            AND ISNULL(CT.無効フラグ, 0) = 0
            AND RM.コースＣＤ IS NULL
        -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
        UNION
        SELECT
            RT.得意先ＣＤ
            ,RT.コース順
        FROM
            臨時コーステーブル AS RT
            INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0
        WHERE
            RT.企業ＣＤ = @企業ＣＤ
            AND RT.コースＣＤ = @コースＣＤ
            AND ISNULL(RT.無効フラグ, 0) = 0
            AND RT.実施日 = @売上日
        ) AS CO ON C.得意先ＣＤ = CO.得意先ＣＤ
WHERE
    C.企業ＣＤ = @企業ＣＤ
    AND C.コースＣＤ = @コースＣＤ
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
    NULL AS 値,
    -1 AS コースに含む順,
    -1 AS コース順
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
    NULL AS 値,
    -1 AS コースに含む順,
    -1 AS コース順
FROM
    V_売掛現金区分 AS VT
WHERE
    VT.売掛現金区分CD = @売掛現金区分売掛
ORDER BY
    明細合計区分,
    コースに含む順,
    コース順,
    得意先ＣＤ,
    売掛現金区分,
    商品ＣＤ
