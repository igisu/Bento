-- 日配持出入力基準（コース×商品）
SELECT
    CM.コースＣＤ,
    P.商品ＣＤ
FROM
    コースマスタ AS CM
    INNER JOIN 主要部品リストマスタ AS P ON P.企業ＣＤ = CM.企業ＣＤ AND P.部署ＣＤ = CM.部署ＣＤ
    LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = P.企業ＣＤ AND S.商品ＣＤ = P.商品ＣＤ
WHERE
    CM.企業ＣＤ = 1
    AND CM.コース区分 = 1
    AND CM.部署ＣＤ = 1
    AND ISNULL(CM.無効フラグ, 0) = 0
    AND ISNULL(S.無効フラグ, 0) = 0
