-- 売上精算入力_得意先チケット使用
-- 売上区分がチケット
SELECT
    0 AS 合計行
    ,A.企業ＣＤ
    ,A.コースＣＤ
    ,A.得意先ＣＤ
    ,A.得意先名
    ,A.商品単価
    ,A.商品数量
    ,A.商品金額合計税込
    ,C.コース順
FROM
    (SELECT
        H.企業ＣＤ
        ,H.コースＣＤ
        ,H.得意先ＣＤ
        ,H.得意先名
        ,M.商品単価
        ,SUM(M.商品数量)         AS 商品数量
        ,SUM(M.商品金額合計税込) AS 商品金額合計税込
    FROM
        売上ヘッダ AS H
        INNER JOIN 売上明細 AS M
            ON  M.企業ＣＤ   = H.企業ＣＤ
            AND M.売上日     = H.売上日
            AND M.得意先ＣＤ = H.得意先ＣＤ
            AND M.部署ＣＤ   = H.部署ＣＤ
    WHERE
        H.企業ＣＤ       = @企業ＣＤ
        AND H.売上日     = @精算日
        AND H.部署ＣＤ   = @部署ＣＤ
        AND H.コースＣＤ = @コースＣＤ
        AND M.売上区分 = 2
    GROUP BY
        H.企業ＣＤ
        ,H.コースＣＤ
        ,H.得意先ＣＤ
        ,H.得意先名
        ,M.商品単価
    ) AS A
    LEFT JOIN コーステーブル AS C
        ON  C.企業ＣＤ   = A.企業ＣＤ
        AND C.コースＣＤ = A.コースＣＤ
        AND C.得意先ＣＤ = A.得意先ＣＤ
UNION
SELECT
    1 AS 合計行
    ,A.企業ＣＤ
    ,NULL AS コースＣＤ
    ,NULL AS 得意先ＣＤ
    ,'合計' AS 得意先名
    ,NULL AS 商品単価
    ,SUM(A.商品数量) AS 商品数量
    ,SUM(A.商品金額合計税込) AS 商品金額合計税込
    ,NULL AS コース順
FROM
    (SELECT
        H.企業ＣＤ
        ,H.コースＣＤ
        ,H.得意先ＣＤ
        ,H.得意先名
        ,M.商品単価
        ,SUM(M.商品数量)         AS 商品数量
        ,SUM(M.商品金額合計税込) AS 商品金額合計税込
    FROM
        売上ヘッダ AS H
        INNER JOIN 売上明細 AS M
            ON  M.企業ＣＤ   = H.企業ＣＤ
            AND M.売上日     = H.売上日
            AND M.得意先ＣＤ = H.得意先ＣＤ
            AND M.部署ＣＤ   = H.部署ＣＤ
    WHERE
        H.企業ＣＤ       = @企業ＣＤ
        AND H.売上日     = @精算日
        AND H.部署ＣＤ   = @部署ＣＤ
        AND H.コースＣＤ = @コースＣＤ
        AND M.売上区分 = 2
    GROUP BY
        H.企業ＣＤ
        ,H.コースＣＤ
        ,H.得意先ＣＤ
        ,H.得意先名
        ,M.商品単価
    ) AS A
GROUP BY
    A.企業ＣＤ
ORDER BY
    合計行
    ,商品単価
    ,得意先ＣＤ
