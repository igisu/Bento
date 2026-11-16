-- 売上精算入力_持出数管理取得
-- 移動入力詳細（移動元）とほぼ同じ

SELECT
    MAIN.企業ＣＤ,
    NULL AS 精算日,
    NULL AS 部署ＣＤ,
    MAIN.コースＣＤ,
    MAIN.商品ＣＤ,
    MAIN.商品名略称,
    MCDS.個数 AS 持出個数,
    TAKE_KOJO.個数 AS 工場追加数,
    TAKE_IPPAN.個数 AS もらった数,
    GIVE.個数 AS やった数,
    -- 持出の個数＋工場から受け取った個数＋工場以外から受け取った個数－渡した個数－売上数 を残数とする
    ISNULL(MCDS.個数, 0) +
    ISNULL(TAKE_KOJO.個数, 0) +
    ISNULL(TAKE_IPPAN.個数, 0) -
    ISNULL(GIVE.個数, 0) -
    ISNULL(INIT.売上予定実績数, 0)
    AS 残数,
    INIT.売上予定実績数,
    INIT.ボーナス消費数,
    NULL AS 作成担当者ＣＤ,
    NULL AS 作成担当者名,
    NULL AS 作成日,
    NULL AS 修正担当者ＣＤ,
    NULL AS 修正担当者名,
    NULL AS 修正日
FROM
    -- ①駆動表（コースに該当する商品）
    (SELECT
        CM.企業ＣＤ,
        CM.コースＣＤ,
        CM.コース名,
        P.商品ＣＤ,
        S.商品名略称
    FROM
        コースマスタ AS CM
        INNER JOIN 主要部品リストマスタ AS P ON P.企業ＣＤ = CM.企業ＣＤ AND P.部署ＣＤ = CM.部署ＣＤ
        LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = P.企業ＣＤ AND S.商品ＣＤ = P.商品ＣＤ
    WHERE
        CM.企業ＣＤ = @企業ＣＤ
        AND CM.コースＣＤ = @コースＣＤ
        AND ISNULL(CM.無効フラグ, 0) = 0
        AND ISNULL(S.無効フラグ, 0) = 0
    ) AS MAIN
    LEFT JOIN
    -- ②売上の部品（各商品を分割した部品ごとに合計する）
    (SELECT
        PART.コースＣＤ,
        PART.部品ＣＤ,
        SUM(PART.売上予定実績数) AS 売上予定実績数,
        SUM(PART.ボーナス) AS ボーナス消費数
    FROM
        -- ②－１ 売上の主食
        (SELECT
            CM.コースＣＤ,
--            D.商品ＣＤ,
            D.主食ＣＤ AS 部品ＣＤ,
            SUM(D.商品数量) AS 売上予定実績数,
            SUM(CASE WHEN D.売上区分  = @売上区分ボーナス THEN D.商品数量 ELSE 0 END) AS ボーナス
        FROM
            コースマスタ AS CM
            INNER JOIN 売上ヘッダ AS H
                ON H.企業ＣＤ    = CM.企業ＣＤ
                AND H.売上日     = @精算日
                AND H.部署ＣＤ   = CM.部署ＣＤ
                AND H.コースＣＤ = CM.コースＣＤ
            INNER JOIN 売上明細 AS D
                ON D.企業ＣＤ    = H.企業ＣＤ
                AND D.売上日     = H.売上日
                AND D.得意先ＣＤ = H.得意先ＣＤ
                AND D.部署ＣＤ   = H.部署ＣＤ
        WHERE
            CM.企業ＣＤ = @企業ＣＤ
            AND ISNULL(CM.無効フラグ, 0) = 0
            AND ISNULL(D.主食ＣＤ, 0) <> 0
        GROUP BY
            CM.コースＣＤ,
--            D.商品ＣＤ,
            D.主食ＣＤ
        UNION
        -- ②－２ 売上の副食
        SELECT
            CM.コースＣＤ,
--            D.商品ＣＤ,
            D.副食ＣＤ AS 部品ＣＤ,
            SUM(D.商品数量) AS 売上予定実績数,
            SUM(CASE WHEN D.売上区分  = @売上区分ボーナス THEN D.商品数量 ELSE 0 END) AS ボーナス
        FROM
            コースマスタ AS CM
            INNER JOIN 売上ヘッダ AS H
                ON H.企業ＣＤ    = CM.企業ＣＤ
                AND H.売上日     = @精算日
                AND H.部署ＣＤ   = CM.部署ＣＤ
                AND H.コースＣＤ = CM.コースＣＤ
            INNER JOIN 売上明細 AS D
                ON D.企業ＣＤ    = H.企業ＣＤ
                AND D.売上日     = H.売上日
                AND D.得意先ＣＤ = H.得意先ＣＤ
                AND D.部署ＣＤ   = H.部署ＣＤ
        WHERE
            CM.企業ＣＤ = @企業ＣＤ
            AND ISNULL(CM.無効フラグ, 0) = 0
            AND ISNULL(D.副食ＣＤ, 0) <> 0
        GROUP BY
            CM.コースＣＤ,
--            D.商品ＣＤ,
            D.副食ＣＤ
        UNION
        -- ②－３ 売上の主食でも副食でもないもの
        SELECT
            CM.コースＣＤ,
--            D.商品ＣＤ,
            D.商品ＣＤ AS 部品ＣＤ,
            SUM(D.商品数量) AS 売上予定実績数,
            SUM(CASE WHEN D.売上区分  = @売上区分ボーナス THEN D.商品数量 ELSE 0 END) AS ボーナス
        FROM
            コースマスタ AS CM
            INNER JOIN 売上ヘッダ AS H
                ON H.企業ＣＤ    = CM.企業ＣＤ
                AND H.売上日     = @精算日
                AND H.部署ＣＤ   = CM.部署ＣＤ
                AND H.コースＣＤ = CM.コースＣＤ
            INNER JOIN 売上明細 AS D
                ON D.企業ＣＤ    = H.企業ＣＤ
                AND D.売上日     = H.売上日
                AND D.得意先ＣＤ = H.得意先ＣＤ
                AND D.部署ＣＤ   = H.部署ＣＤ
        WHERE
            CM.企業ＣＤ = @企業ＣＤ
            AND ISNULL(CM.無効フラグ, 0) = 0
            AND ISNULL(D.主食ＣＤ, 0) = 0
            AND ISNULL(D.副食ＣＤ, 0) = 0
        GROUP BY
            CM.コースＣＤ,
--            D.商品ＣＤ,
            D.商品ＣＤ
        ) AS PART
    GROUP BY
        PART.コースＣＤ,
        PART.部品ＣＤ
    ) AS INIT ON MAIN.コースＣＤ = INIT.コースＣＤ AND MAIN.商品ＣＤ = INIT.部品ＣＤ
    LEFT JOIN
    -- ③持ち出し数
    (SELECT
        コースＣＤ
        ,商品ＣＤ
        ,SUM(個数) AS 個数
    FROM
        モバイル_持ち出し入力
    WHERE
        企業ＣＤ = @企業ＣＤ AND
        持出日 = @精算日
    GROUP BY
        コースＣＤ
        ,商品ＣＤ
    ) AS MCDS ON MAIN.コースＣＤ = MCDS.コースＣＤ AND MAIN.商品ＣＤ = MCDS.商品ＣＤ
    LEFT JOIN
    -- ④工場からもらった個数
    (SELECT
        MI.相手コースＣＤ
        ,MI.商品ＣＤ
        ,SUM(MI.個数) AS 個数
    FROM
        モバイル_移動入力 AS MI
        LEFT JOIN コースマスタ AS CM
        ON MI.企業ＣＤ = CM.企業ＣＤ
        AND MI.コースＣＤ = CM.コースＣＤ
    WHERE
        CM.企業ＣＤ = @企業ＣＤ
        AND ISNULL(CM.無効フラグ, 0) = 0
        AND ISNULL(CM.工場フラグ, 0) <> 0
        AND MI.移動日 = @精算日
    GROUP BY
        MI.相手コースＣＤ
        ,MI.商品ＣＤ
    ) AS TAKE_KOJO ON MAIN.コースＣＤ = TAKE_KOJO.相手コースＣＤ AND MAIN.商品ＣＤ = TAKE_KOJO.商品ＣＤ
    LEFT JOIN
    -- ⑤工場以外からもらった個数
    (SELECT
        MI.相手コースＣＤ
        ,MI.商品ＣＤ
        ,SUM(MI.個数) AS 個数
    FROM
        モバイル_移動入力 AS MI
        LEFT JOIN コースマスタ AS CM
        ON MI.企業ＣＤ = CM.企業ＣＤ
        AND MI.コースＣＤ = CM.コースＣＤ
    WHERE
        CM.企業ＣＤ = @企業ＣＤ
        AND ISNULL(CM.無効フラグ, 0) = 0
        AND ISNULL(CM.工場フラグ, 0) = 0
        AND MI.移動日 = @精算日
    GROUP BY
        MI.相手コースＣＤ
        ,MI.商品ＣＤ
    ) AS TAKE_IPPAN ON MAIN.コースＣＤ = TAKE_IPPAN.相手コースＣＤ AND MAIN.商品ＣＤ = TAKE_IPPAN.商品ＣＤ
    LEFT JOIN
    -- ⑥渡した個数
    (SELECT
        コースＣＤ
        ,商品ＣＤ
        ,SUM(個数) AS 個数
    FROM
        モバイル_移動入力
    WHERE
        企業ＣＤ = @企業ＣＤ
        AND 移動日 = @精算日
    GROUP BY
        コースＣＤ
        ,商品ＣＤ
    ) AS GIVE ON MAIN.コースＣＤ = GIVE.コースＣＤ AND MAIN.商品ＣＤ = GIVE.商品ＣＤ
ORDER BY
    商品ＣＤ
