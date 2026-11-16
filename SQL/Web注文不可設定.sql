-- Web注文不可設定

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
    ,@得意先ＣＤ AS 得意先ＣＤ
    ,S.名称 AS 祝日
    ,CASE WHEN B.注文不可日 IS NULL THEN  0 ELSE    1 END AS 注文不可
    ,CASE WHEN H.得意先ＣＤ IS NULL THEN '' ELSE '有' END AS 注文有無
FROM
    -- ① 0～30までを取得する再帰CTEを用いて、指定した日付以降の日付を取得
    (SELECT
        CONVERT(date, DateAdd(day, I, @対象年月日)) AS 日付
    FROM
        T
    ) AS A
    LEFT JOIN WEB_M_注文不可設定 AS B
        ON B.注文不可日 = A.日付
        AND B.企業ＣＤ = @企業ＣＤ
        AND B.得意先ＣＤ = @得意先ＣＤ
    LEFT JOIN 得意先マスタ AS T
        ON T.企業ＣＤ = @企業ＣＤ
        AND T.得意先ＣＤ = @得意先ＣＤ
        AND ISNULL(T.無効フラグ, 0) = 0
    LEFT JOIN 注文ヘッダ AS H
        ON H.企業ＣＤ = @企業ＣＤ
        AND H.注文区分 = @注文区分注文
        AND H.喫食日 = A.日付
        AND H.得意先ＣＤ = T.得意先ＣＤ
        AND H.部署ＣＤ = T.部署ＣＤ
    LEFT JOIN 祝日マスタ AS S
        ON S.企業ＣＤ = @企業ＣＤ
        AND S.対象日 = A.日付
WHERE
    YEAR(A.日付) = @対象年
    AND MONTH(A.日付) = @対象月
ORDER BY
    日付


@企業ＣＤ = 40000
@部署ＣＤ = 1
@得意先ＣＤ = 530
@対象年月日 = '2017/08/01'
@対象年 = 2017
@対象月 = 8
@注文区分注文 = 0
