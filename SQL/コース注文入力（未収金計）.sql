-- コース注文入力（未収金計）
SELECT
    ISNULL(SUM(UK.商品金額合計税込), 0) AS 未収金計
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
                AND コースＣＤ = @コースＣＤ
                AND 実施日 = @注文日
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
        RT.企業ＣＤ
        ,RT.コースＣＤ
        ,RT.得意先ＣＤ
    FROM
        臨時コーステーブル AS RT
        INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0
    WHERE
        RT.企業ＣＤ = @企業ＣＤ
        AND RT.コースＣＤ = @コースＣＤ
        AND ISNULL(RT.無効フラグ, 0) = 0
        AND RT.実施日 = @注文日
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
        AND コースＣＤ = @コースＣＤ
        AND 喫食日 = @注文日
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
        AND コースＣＤ = @コースＣＤ
        AND 売上日 = @注文日
        AND 部署ＣＤ = @部署ＣＤ
    ) AS C
    LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ
-- ④注文計
    LEFT JOIN
        (SELECT
            H.コースＣＤ
            ,M.得意先ＣＤ
            ,SUM(M.商品金額合計税込) AS 注文計
        FROM
            注文ヘッダ AS H
            INNER JOIN 注文明細 AS M
                ON  H.企業ＣＤ   = M.企業ＣＤ
                AND H.注文区分   = M.注文区分
                AND H.喫食日     = M.喫食日
                AND H.得意先ＣＤ = M.得意先ＣＤ
                AND H.部署ＣＤ   = M.部署ＣＤ
        WHERE
            H.企業ＣＤ = @企業ＣＤ
            AND H.喫食日 = @注文日
            AND H.部署ＣＤ = @部署ＣＤ
            AND M.注文区分 = @注文区分注文      -- 注文
        GROUP BY
            H.コースＣＤ
            ,M.得意先ＣＤ
        ) AS CK ON CK.得意先ＣＤ = C.得意先ＣＤ AND CK.コースＣＤ = C.コースＣＤ
-- ⑤未収金計
    LEFT JOIN
        (SELECT
            H.コースＣＤ
            ,M.得意先ＣＤ
            ,SUM(M.商品金額合計税込) AS 商品金額合計税込
        FROM
            売上ヘッダ AS H
            INNER JOIN 売上明細 AS M
                ON  H.企業ＣＤ   = M.企業ＣＤ
                AND H.売上日     = M.売上日
                AND H.得意先ＣＤ = M.得意先ＣＤ
                AND H.部署ＣＤ   = M.部署ＣＤ
        WHERE
            M.企業ＣＤ = @企業ＣＤ
            AND M.売上日   = @注文日
            AND M.部署ＣＤ = @部署ＣＤ
            AND M.売上区分 = @売上区分売掛      -- 売掛
        GROUP BY
            H.コースＣＤ
            ,M.得意先ＣＤ
        ) AS UK ON UK.得意先ＣＤ = C.得意先ＣＤ AND UK.コースＣＤ = C.コースＣＤ
WHERE
    1 = 1
    AND ISNULL(T.売掛現金区分, 0) = @売掛現金区分現金   -- 現金
--    AND CK.注文計 IS NULL     -- 注文が無い
--    AND CK.注文計 IS NOT NULL -- 注文がある


@企業ＣＤ = 40000
@注文日 = '2018/04/10'
@部署ＣＤ = 1
@コースＣＤ = 1
@注文区分注文 = 0
@売上区分売掛 = 1
@売掛現金区分現金 = 0
