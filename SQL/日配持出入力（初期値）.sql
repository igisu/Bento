-- 日配持出入力（初期値）
SELECT
    M.コースＣＤ,
    M.コース名,
    PART.商品ＣＤ,
    PART.部品ＣＤ,
    PART.商品注文数量,
    PART.商品予測数量,
    CASE WHEN PART.商品注文数量 <> 0 THEN 1 ELSE 0 END AS 注文フラグ,
    CASE WHEN PART.商品注文数量 <> 0 THEN PART.商品注文数量 ELSE PART.商品予測数量 END AS 商品数量
FROM
    コースマスタ AS M
    LEFT JOIN
    (SELECT
        CM.コースＣＤ,
        D.商品ＣＤ,
        I.主食ＣＤ AS 部品ＣＤ,
        SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE 0 END) AS 商品注文数量,
        SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE 0 END) AS 商品予測数量
    FROM
        コースマスタ AS CM
        INNER JOIN コーステーブル AS CT
            ON CT.企業ＣＤ = CM.企業ＣＤ
            AND ISNULL(CT.無効フラグ, 0) = 0
            AND CT.コースＣＤ = CM.コースＣＤ
        INNER JOIN 注文ヘッダ AS H
            ON H.企業ＣＤ    = CM.企業ＣＤ
            AND H.配送日     = '2016/03/14'
            AND H.得意先ＣＤ = CT.得意先ＣＤ
            AND H.部署ＣＤ   = CM.部署ＣＤ
        INNER JOIN 注文明細 AS D
            ON D.企業ＣＤ    = CM.企業ＣＤ
            AND D.注文区分   = H.注文区分
            AND D.喫食日     = H.配送日
            AND D.得意先ＣＤ = H.得意先ＣＤ
            AND D.部署ＣＤ   = H.部署ＣＤ
        INNER JOIN 商品マスタ AS I
            ON I.企業ＣＤ = D.企業ＣＤ
            AND ISNULL(I.無効フラグ, 0) = 0
            AND I.商品ＣＤ = D.商品ＣＤ
    WHERE
        CM.企業ＣＤ = 1
        AND CM.コース区分 = 1
        AND CM.部署ＣＤ = 1
        AND ISNULL(CM.無効フラグ, 0) = 0
        AND ISNULL(I.主食ＣＤ, 0) <> 0
    GROUP BY
        CM.コースＣＤ,
        D.商品ＣＤ,
        I.主食ＣＤ
    UNION
    SELECT
        CM.コースＣＤ,
        D.商品ＣＤ,
        I.副食ＣＤ AS 部品ＣＤ,
        SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE 0 END) AS 商品注文数量,
        SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE 0 END) AS 商品予測数量
    FROM
        コースマスタ AS CM
            INNER JOIN コーステーブル AS CT
                ON CT.企業ＣＤ = CM.企業ＣＤ
                AND ISNULL(CT.無効フラグ, 0) = 0
                AND CT.コースＣＤ = CM.コースＣＤ
            INNER JOIN 注文ヘッダ AS H
                ON H.企業ＣＤ    = CM.企業ＣＤ
                AND H.配送日     = '2015/04/14'
                AND H.得意先ＣＤ = CT.得意先ＣＤ
                AND H.部署ＣＤ   = CM.部署ＣＤ
            INNER JOIN 注文明細 AS D
                ON D.企業ＣＤ    = CM.企業ＣＤ
                AND D.注文区分   = H.注文区分
                AND D.喫食日     = H.配送日
                AND D.得意先ＣＤ = H.得意先ＣＤ
                AND D.部署ＣＤ   = H.部署ＣＤ
            INNER JOIN 商品マスタ AS I
                ON I.企業ＣＤ = D.企業ＣＤ
                AND ISNULL(I.無効フラグ, 0) = 0
                AND I.商品ＣＤ = D.商品ＣＤ
    WHERE
        CM.企業ＣＤ = 1
        AND CM.コース区分 = 1
        AND CM.部署ＣＤ = 1
        AND ISNULL(CM.無効フラグ, 0) = 0
        AND ISNULL(I.副食ＣＤ, 0) <> 0
    GROUP BY
        CM.コースＣＤ,
        D.商品ＣＤ,
        I.副食ＣＤ
    UNION
    SELECT
        CM.コースＣＤ,
        D.商品ＣＤ,
        I.商品ＣＤ AS 部品ＣＤ,
        SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE 0 END) AS 商品注文数量,
        SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE 0 END) AS 商品予測数量
    FROM
        コースマスタ AS CM
            INNER JOIN コーステーブル AS CT
                ON CT.企業ＣＤ = CM.企業ＣＤ
                AND ISNULL(CT.無効フラグ, 0) = 0
                AND CT.コースＣＤ = CM.コースＣＤ
            INNER JOIN 注文ヘッダ AS H
                ON H.企業ＣＤ    = CM.企業ＣＤ
                AND H.配送日     = '2015/04/14'
                AND H.得意先ＣＤ = CT.得意先ＣＤ
                AND H.部署ＣＤ   = CM.部署ＣＤ
            INNER JOIN 注文明細 AS D
                ON D.企業ＣＤ    = CM.企業ＣＤ
                AND D.注文区分   = H.注文区分
                AND D.喫食日     = H.配送日
                AND D.得意先ＣＤ = H.得意先ＣＤ
                AND D.部署ＣＤ   = H.部署ＣＤ
            INNER JOIN 商品マスタ AS I
                ON I.企業ＣＤ = D.企業ＣＤ
                AND ISNULL(I.無効フラグ, 0) = 0
                AND I.商品ＣＤ = D.商品ＣＤ
    WHERE
        CM.企業ＣＤ = 1
        AND CM.コース区分 = 1
        AND CM.部署ＣＤ = 1
        AND ISNULL(CM.無効フラグ, 0) = 0
        AND ISNULL(I.主食ＣＤ, 0) = 0
        AND ISNULL(I.副食ＣＤ, 0) = 0
    GROUP BY
        CM.コースＣＤ,
        D.商品ＣＤ,
        I.商品ＣＤ
    ) AS PART ON M.コースＣＤ = PART.コースＣＤ
WHERE
    M.企業ＣＤ = 1
AND M.コース区分 = 1
AND M.部署ＣＤ = 1
AND ISNULL(M.無効フラグ, 0) = 0
