-- コース売上一覧（対象商品）
-- 以下のように並べる
-- ┌─────┬─────┬─────┐
-- │  商品Ａ  │  商品Ｂ  │  合計    │
-- ├──┬──┼──┬──┼──┬──┤
-- │個数│金額│個数│金額│個数│金額│
-- └──┴──┴──┴──┴──┴──┘
-- 商品ＣＤのアンダーバー以降は
-- 0:個数 1:金額

-- ①個数
SELECT
    0 AS 合計区分
    ,T.商品ＣＤ AS ソート用商品ＣＤ
    ,CONVERT(VARCHAR, T.商品ＣＤ) + '_0' AS 商品ＣＤ
    ,ISNULL(S.商品名略称, CONVERT(VARCHAR, T.商品ＣＤ)) AS 商品名略称
    ,'個数' AS 個数金額
FROM
-- 主要商品リストマスタと売上明細にある商品
    (SELECT
        A.企業ＣＤ,
        A.商品ＣＤ
    FROM
        主要商品リストマスタ AS A
    WHERE
        A.企業ＣＤ = @企業ＣＤ
        AND A.部署ＣＤ = @部署ＣＤ
        AND ISNULL(A.無効フラグ, 0) = 0
    UNION
    SELECT
        M.企業ＣＤ,
        M.商品ＣＤ
    FROM
        売上明細 AS M
    WHERE
        M.企業ＣＤ = @企業ＣＤ AND
        M.売上日   = @売上日 AND
        M.部署ＣＤ = @部署ＣＤ
    ) AS T
    LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = T.企業ＣＤ AND S.商品ＣＤ = T.商品ＣＤ
UNION
-- ②金額
SELECT
    0 AS 合計区分
    ,T.商品ＣＤ AS ソート用商品ＣＤ
    ,CONVERT(VARCHAR, T.商品ＣＤ) + '_1' AS 商品ＣＤ
    ,ISNULL(S.商品名略称, CONVERT(VARCHAR, T.商品ＣＤ)) AS 商品名略称
    ,'金額' AS 個数金額
FROM
-- 主要商品リストマスタと売上明細にある商品
    (SELECT
        A.企業ＣＤ,
        A.商品ＣＤ
    FROM
        主要商品リストマスタ AS A
    WHERE
        A.企業ＣＤ = @企業ＣＤ
        AND A.部署ＣＤ = @部署ＣＤ
        AND ISNULL(A.無効フラグ, 0) = 0
    UNION
    SELECT
        M.企業ＣＤ,
        M.商品ＣＤ
    FROM
        売上明細 AS M
    WHERE
        M.企業ＣＤ = @企業ＣＤ AND
        M.売上日   = @売上日 AND
        M.部署ＣＤ = @部署ＣＤ
    ) AS T
    LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = T.企業ＣＤ AND S.商品ＣＤ = T.商品ＣＤ
UNION
-- ③合計個数
SELECT
    1 AS 合計区分
    ,-1 AS ソート用商品ＣＤ
    , 'XXX_0' AS 商品ＣＤ
    ,'合計' AS 商品名略称
    ,'個数' AS 個数金額
-- ④合計金額
UNION
SELECT
    1 AS 合計区分
    ,-1 AS ソート用商品ＣＤ
    , 'XXX_1' AS 商品ＣＤ
    ,'合計' AS 商品名略称
    ,'金額' AS 個数金額
ORDER BY
    合計区分
    ,ソート用商品ＣＤ
    ,商品ＣＤ";
