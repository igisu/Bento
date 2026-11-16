-- 一括注文入力
-- 商品名取得
-- 主要商品リストマスタにある商品と、予測と注文の商品を以下のように並べる
-- ┌─────┬─────┬─────┐
-- │  商品Ａ  │  商品Ｂ  │  商品Ｃ  │
-- ├──┬──┼──┬──┼──┬──┤
-- │予測│注文│予測│注文│予測│注文│
-- └──┴──┴──┴──┴──┴──┘
-- 商品ＣＤのアンダーバー以降は
-- 0:予測 1:注文
-- ①主要商品リストマスタから(予測用)
SELECT
    A.商品ＣＤ AS ソート用商品ＣＤ
    ,CONVERT(varchar, A.商品ＣＤ) + '_0' AS 商品ＣＤ
    ,S.商品名略称
    ,'予測' AS 予測注文
FROM
    主要商品リストマスタ AS A
    INNER JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND A.商品ＣＤ = S.商品ＣＤ
WHERE
    A.企業ＣＤ = @企業ＣＤ
    AND A.部署ＣＤ = @部署ＣＤ
    AND ISNULL(A.無効フラグ, 0) = 0
    AND ISNULL(S.無効フラグ, 0) = 0
UNION
-- ②注文レコードから(予測用)
SELECT
    M.商品ＣＤ AS ソート用商品ＣＤ
    ,CONVERT(varchar, M.商品ＣＤ) + '_0' AS 商品ＣＤ
    ,S.商品名略称
    ,'予測' AS 予測注文
FROM
    注文ヘッダ AS H
    INNER JOIN 注文明細 AS M
        ON H.企業ＣＤ = M.企業ＣＤ
        AND H.注文区分 = M.注文区分
        AND H.喫食日 = M.喫食日
        AND H.得意先ＣＤ = M.得意先ＣＤ
        AND H.部署ＣＤ = M.部署ＣＤ
    LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = M.企業ＣＤ AND S.商品ＣＤ = M.商品ＣＤ
WHERE
    H.喫食日 >= @注文日From
    AND H.喫食日 <= @注文日To
    AND H.企業ＣＤ = @企業ＣＤ
    AND H.得意先ＣＤ = @得意先ＣＤ
    AND H.部署ＣＤ = @部署ＣＤ
UNION
-- ③主要商品リストマスタから(注文用)
SELECT
    A.商品ＣＤ AS ソート用商品ＣＤ
    ,CONVERT(varchar, A.商品ＣＤ) + '_1' AS 商品ＣＤ
    ,S.商品名略称
    ,'注文' AS 予測注文
FROM
    主要商品リストマスタ AS A
    INNER JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND A.商品ＣＤ = S.商品ＣＤ
WHERE
    A.企業ＣＤ = @企業ＣＤ
    AND A.部署ＣＤ = @部署ＣＤ
    AND ISNULL(A.無効フラグ, 0) = 0
    AND ISNULL(S.無効フラグ, 0) = 0
UNION
-- ④注文レコードから(注文用)
SELECT
    M.商品ＣＤ AS ソート用商品ＣＤ
    ,CONVERT(varchar, M.商品ＣＤ) + '_1' AS 商品ＣＤ
    ,S.商品名略称
    ,'注文' AS 予測注文
FROM
    注文ヘッダ AS H
    INNER JOIN 注文明細 AS M
        ON H.企業ＣＤ = M.企業ＣＤ
        AND H.注文区分 = M.注文区分
        AND H.喫食日 = M.喫食日
        AND H.得意先ＣＤ = M.得意先ＣＤ
        AND H.部署ＣＤ = M.部署ＣＤ
    LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = M.企業ＣＤ AND S.商品ＣＤ = M.商品ＣＤ
WHERE
    H.喫食日 >= @注文日From
    AND H.喫食日 <= @注文日To
    AND H.企業ＣＤ = @企業ＣＤ
    AND H.得意先ＣＤ = @得意先ＣＤ
    AND H.部署ＣＤ = @部署ＣＤ
ORDER BY
    ソート用商品ＣＤ
    ,商品ＣＤ







-- 0～30までを取得する再帰CTE（共通テーブル式）
With T(I) AS
(SELECT
    0 AS I
UNION ALL
SELECT
    I + 1
FROM
    T
WHERE
    I < 30
)
-- ② ①で取得した日付に該当する注文を取得。同時にwhere句で年月を指定する
SELECT
    A.日付
    ,S.名称 AS 祝日
    ,B.コースＣＤ
    ,CASE WHEN B.商品ＣＤ IS NULL THEN TM.備考1 ELSE B.備考1 END AS 備考1   -- 注文が無ければ得意先の備考1を初期表示
    ,B.商品ＣＤ
    ,B.商品数量
    ,0 AS 合計行
FROM
    -- ① 0～30までを取得する再帰CTEを用いて、指定した日付以降の日付を取得
    (SELECT
        CONVERT(date, DateAdd(day, I, @注文日From)) AS 日付
    FROM
        T
    ) AS A
    LEFT JOIN
        -- ②－１ 予測を取得
        (SELECT
            HC.コースＣＤ
            ,HY.喫食日
            ,HC.備考1           -- (予測ではなく)注文の備考
            ,CONVERT(varchar, M.商品ＣＤ) + '_0' AS 商品ＣＤ
            ,M.商品数量
        FROM
            注文ヘッダ AS HY
            INNER JOIN 注文明細 AS M
                ON  HY.企業ＣＤ   = M.企業ＣＤ
                AND HY.注文区分   = M.注文区分
                AND HY.喫食日     = M.喫食日
                AND HY.得意先ＣＤ = M.得意先ＣＤ
                AND HY.部署ＣＤ   = M.部署ＣＤ
            LEFT JOIN 注文ヘッダ AS HC
                ON  HY.企業ＣＤ   = HC.企業ＣＤ
                AND HY.喫食日     = HC.喫食日
                AND HY.得意先ＣＤ = HC.得意先ＣＤ
                AND HY.部署ＣＤ   = HC.部署ＣＤ
                AND HC.注文区分   = @注文区分注文
        WHERE
            HY.企業ＣＤ = @企業ＣＤ
            AND HY.喫食日 >= @注文日From
            AND HY.喫食日 <= @注文日To
            AND HY.得意先ＣＤ = @得意先ＣＤ
            AND HY.部署ＣＤ = @部署ＣＤ
            AND HY.注文区分 = @注文区分予測
        -- ②－２ 注文を取得
        UNION
        SELECT
            H.コースＣＤ
            ,H.喫食日
            ,H.備考1
            ,CONVERT(varchar, M.商品ＣＤ) + '_1' AS 商品ＣＤ
            ,M.商品数量
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
            AND H.喫食日 >= @注文日From
            AND H.喫食日 <= @注文日To
            AND H.得意先ＣＤ = @得意先ＣＤ
            AND H.部署ＣＤ = @部署ＣＤ
            AND H.注文区分 = @注文区分注文
    ) AS B
        ON B.喫食日 = A.日付
    LEFT JOIN 祝日マスタ AS S
        ON S.企業ＣＤ = @企業ＣＤ
        AND S.対象日 = A.日付
    LEFT JOIN 得意先マスタ AS TM        -- 備考取得用の得意先マスタ
        ON TM.企業ＣＤ = @企業ＣＤ
        AND TM.得意先ＣＤ = @得意先ＣＤ
WHERE
    YEAR(A.日付) = @注文年
AND MONTH(A.日付) = @注文月
UNION
SELECT
    NULL AS 日付
    ,'合計' AS 祝日
    ,-1 AS コースＣＤ
    ,NULL AS 備考1
    ,NULL AS 商品ＣＤ
    ,-1 AS 商品数量
    ,1 AS 合計行
ORDER BY
    合計行,
    日付,
    商品ＣＤ


@企業ＣＤ = 40000
@部署ＣＤ = 1
@注文区分注文 = 0
@注文区分予測 = 1
@得意先ＣＤ = 530
@注文日From = '2017/08/01'
@注文日To = '2017/08/31'
@注文年 = 2017
@注文月 = 8
