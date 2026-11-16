-- 配送予定入力(実績)（コース IS NULL）
SELECT
    A.得意先ＣＤ
    ,A.得意先名
    ,NULL AS コースＣＤ
    ,A.商品ＣＤ
    ,0 AS 注文フラグ
    ,A.商品数量
FROM
    (SELECT
        D.得意先ＣＤ,
        H.得意先名,
        D.商品ＣＤ,
        SUM(D.商品数量) AS 商品数量
    FROM
        売上ヘッダ AS H
        INNER JOIN 売上明細 AS D
            ON  D.企業ＣＤ   = H.企業ＣＤ
            AND D.売上日     = H.売上日
            AND D.得意先ＣＤ = H.得意先ＣＤ
            AND D.部署ＣＤ   = H.部署ＣＤ
    WHERE
        H.企業ＣＤ = @企業ＣＤ AND
        H.売上日   = @売上日 AND
        H.部署ＣＤ = @部署ＣＤ AND
        H.コースＣＤ IS NULL AND
        ISNULL(D.現金補填フラグ, 0) = 0  -- このフラグが立っているのは厳密に言うと商品の売上ではないので除外する
    GROUP BY
        D.得意先ＣＤ,
        H.得意先名,
        D.商品ＣＤ
    ) AS A
ORDER BY
    A.得意先ＣＤ,
    A.商品ＣＤ



@企業ＣＤ = 40001
@売上日 = '2018/04/25'
@部署ＣＤ = 1
