-- 売上明細表_チケット売上枚数・金額
SELECT
    A.企業ＣＤ
    ,A.精算日
    ,A.部署ＣＤ
    ,A.コースＣＤ
    ,ISNULL(SUM(A.売上数量), 0) AS チケット売上枚数
    ,ISNULL(SUM(A.売上金額), 0) AS チケット売上金額
FROM
    売上精算_売上明細 AS A
    LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND S.商品ＣＤ = A.商品ＣＤ
WHERE
    A.企業ＣＤ   = 1
    AND A.精算日     = '2016/05/19'
    AND A.部署ＣＤ   = 1
    AND A.コースＣＤ = 9996
    AND S.弁当区分 = 8
GROUP BY
    A.企業ＣＤ
    ,A.精算日
    ,A.部署ＣＤ
    ,A.コースＣＤ
