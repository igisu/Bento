-- 得意先別売上推移（対象年月）

-- 集計年月日が2017/05/01
-- 表示年月が12の場合、以下のように並べる
-- ┌───┬───┬───┬─┬───┬───┬──┐
-- │2016/6│2016/7│2016/8│～│2017/4│2017/5│合計│
-- └───┴───┴───┴─┴───┴───┴──┘

-- 0～@表示月数-1までを取得する再帰CTE（共通テーブル式）
With T(I) AS
(SELECT
    0 AS I
UNION ALL
SELECT
    I + 1
FROM
    T
WHERE
    I < @表示月数 - 1
)
-- ② ①で取得した日付の年月を取得
SELECT
    0 AS 合計区分
    ,(YEAR(A.日付) * 100) + MONTH(A.日付) AS 年月
    ,cast(YEAR(A.日付) AS varchar) + '/' + cast(MONTH(A.日付) AS varchar) AS 表示用年月
FROM
    -- ① 0～@表示月数-1までを取得する再帰CTEを用いて、指定した日付以前の日付を取得
    (SELECT
        CONVERT(date, DateAdd(month, I * -1, @集計年月日)) AS 日付
    FROM
        T
    ) AS A
UNION
SELECT
    1 AS 合計区分
    ,999999 AS 年月
    ,'合計' AS 表示用年月
ORDER BY
    合計区分
    ,年月


@表示月数 = 12
@集計年月日 = '2017/05/01'
