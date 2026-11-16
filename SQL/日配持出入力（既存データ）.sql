-- 日配持出入力（既存データ）
SELECT
    M.コースＣＤ
    ,M.コース名
    ,A.商品ＣＤ
    ,A.個数
FROM
    コースマスタ AS M
    LEFT JOIN(
    (SELECT
        コースＣＤ
        ,商品ＣＤ
        ,個数
    FROM
        モバイル_持ち出し入力
    WHERE
        企業ＣＤ = 1 AND
        持出日 = '2015/03/30'
    ) AS A ON A.コースＣＤ = M.コースＣＤ
WHERE
    M.企業ＣＤ = 1
    AND M.コース区分 = 1
    AND M.部署ＣＤ = 1
    AND ISNULL(M.無効フラグ, 0) = 0
ORDER BY
    M.コースＣＤ
    ,A.商品ＣＤ
