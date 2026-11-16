-- 売上精算入力_使用チケット
--┌─────┬─────┐
--│チケット  │枚数と金額│
--├─────┼─────┤
--│バークレー│金額のみ  │
--├─────┼─────┤
--│ボーナス  │枚数だけ  │
--└─────┴─────┘
-- 値引額については、GetTotalNebiki()で売上区分を問わず全ての売上の値引の合計を取得したあと、
-- 実質売上(合計_実質総売上)計算時に値引額を減算しているため、ここでは値引前の金額（＝値引額を加算した金額）を求める
-- としていたが、バークレーには値引後の金額を表示することにする。
SELECT
     ISNULL(SUM(CASE WHEN M.売上区分 = 2 THEN M.商品数量                               ELSE 0 END), 0) AS チケット枚数
    ,ISNULL(SUM(CASE WHEN M.売上区分 = 2 THEN M.商品金額合計税込 + ISNULL(M.値引額, 0) ELSE 0 END), 0) AS チケット金額
    ,ISNULL(SUM(CASE WHEN M.売上区分 = 3 THEN M.商品金額合計税込                       ELSE 0 END), 0) AS バークレー金額
    ,ISNULL(SUM(CASE WHEN M.売上区分 = 4 THEN M.商品数量                               ELSE 0 END), 0) AS ボーナス枚数
FROM
    売上ヘッダ AS H
    INNER JOIN 売上明細 AS M
        ON  M.企業ＣＤ   = H.企業ＣＤ
        AND M.売上日     = H.売上日
        AND M.得意先ＣＤ = H.得意先ＣＤ
        AND M.部署ＣＤ   = H.部署ＣＤ
WHERE
    H.企業ＣＤ       = 1
    AND H.売上日     = '2016/03/16'
    AND H.部署ＣＤ   = 7
    AND H.コースＣＤ = 9997
    AND M.売上区分 IN (2, 3, 4)
