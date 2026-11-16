-- 売上集計表(売上月計表)(日付範囲指定の売上集計表)
SELECT
    @チケット回収時売上フラグ AS 総売上見出しフラグ
    ,'2017年2月分' AS 精算日            -- 日付範囲指定の場合は「2024年10月26日(土曜日)～2024年11月01日(金曜日)」
    ,'売上月計表' AS タイトル           -- 日付範囲指定の場合は「売上集計表」
    ,C.コースＣＤ
    ,CM.コース名
    ,CM.担当者ＣＤ
    ,TM.担当者名
    ,UA.弁当数量
    ,UA.主食数量
    ,UA.副食数量
    ,UB.件数
    ,UA.弁当数量 + UA.副食数量 AS 食数
    ,UB.現金売上
    ,UB.チケット
    ,UB.チケット代
    ,UB.回収現金
    ,UB.現金合計
    ,UB.掛売合計
    ,UB.E合計
    ,UB.総売上
    ,UB.総値引
    ,UB.バークレー
    ,UB.実質売上
    ,CASE WHEN (UA.弁当数量 + UA.副食数量) = 0 THEN 0 ELSE FLOOR(UB.総売上 / (UA.弁当数量 + UA.副食数量)) END AS 平均単価
    ,0 AS 合計行フラグ
FROM
    -- ①駆動表
    -- コースマスタから取得（売上日計表とは異なり、臨時コーステーブルからは取得しない）
    (SELECT
        コースＣＤ
    FROM
        コースマスタ
    WHERE
        企業ＣＤ = @企業ＣＤ
        AND 部署ＣＤ = @部署ＣＤ
        AND ISNULL(工場フラグ, 0) = 0
        AND ISNULL(無効フラグ, 0) = 0
    UNION
    -- 集計対象の売上精算_ヘッダにあるコースも取得
    SELECT
        コースＣＤ
    FROM
        売上精算_ヘッダ
    WHERE
        企業ＣＤ = @企業ＣＤ
        AND 精算日 >= @精算日From
        AND 精算日 <= @精算日To
        AND 部署ＣＤ = @部署ＣＤ
    ) AS C
    -- ②売上精算_売上明細から弁当・主食・副食の数量を取得
    LEFT JOIN
        (SELECT
            UM.コースＣＤ
            ,SUM(CASE WHEN SM.弁当区分 = @弁当区分弁当 THEN UM.売上数量 ELSE 0 END) AS 弁当数量
            ,SUM(CASE WHEN SM.弁当区分 = @弁当区分主食 THEN UM.売上数量 ELSE 0 END) AS 主食数量
            ,SUM(CASE WHEN SM.弁当区分 = @弁当区分副食 THEN UM.売上数量 ELSE 0 END) AS 副食数量
        FROM
            売上精算_売上明細 AS UM
            INNER JOIN 商品マスタ AS SM
                ON UM.企業ＣＤ = SM.企業ＣＤ
                AND UM.商品ＣＤ = SM.商品ＣＤ
        WHERE
            UM.企業ＣＤ = @企業ＣＤ
            AND UM.精算日 >= @精算日From
            AND UM.精算日 <= @精算日To
            AND UM.部署ＣＤ = @部署ＣＤ
        GROUP BY
            UM.コースＣＤ
        ) AS UA
    ON UA.コースＣＤ = C.コースＣＤ
    -- ③売上精算_ヘッダから金額を取得
    LEFT JOIN
        (SELECT
            B.コースＣＤ
            ,SUM(B.件数) AS 件数
            ,SUM(B.現金売上) AS 現金売上
            ,SUM(B.チケット) AS チケット
            ,SUM(B.チケット代) AS チケット代
            ,SUM(B.回収現金) AS 回収現金
            ,SUM(B.現金合計) AS 現金合計
            ,SUM(B.掛売合計) AS 掛売合計
            ,SUM(B.E合計) AS E合計
            ,CASE WHEN ISNULL(K.チケット回収時売上フラグ, 0) = 0
             THEN SUM(B.現金売上) + SUM(B.チケット代) + SUM(B.掛売合計) + SUM(B.E合計)
             ELSE SUM(B.現金売上) + SUM(B.チケット) + SUM(B.掛売合計) + SUM(B.E合計)
             END AS 総売上
            ,SUM(B.総値引) AS 総値引
            ,SUM(B.バークレー) AS バークレー
            ,CASE WHEN ISNULL(K.チケット回収時売上フラグ, 0) = 0
             THEN SUM(B.現金売上) + SUM(B.チケット代) + SUM(B.掛売合計) + SUM(B.E合計) - SUM(B.総値引) + SUM(B.バークレー)
             ELSE SUM(B.現金売上) + SUM(B.チケット) + SUM(B.掛売合計) + SUM(B.E合計) - SUM(B.総値引) + SUM(B.バークレー)
             END AS 実質売上
        FROM
            (SELECT
                企業ＣＤ
                ,コースＣＤ
                ,ISNULL(得意先数, 0) AS 件数
                ,ISNULL(現金_売上合計, 0) AS 現金売上
                ,ISNULL(現金_チケット束代, 0) AS チケット代
                ,ISNULL(現金_回収現金, 0) AS 回収現金
                ,ISNULL(現金_売上合計, 0) + ISNULL(現金_チケット束代, 0) + ISNULL(現金_回収現金, 0) AS 現金合計
                ,ISNULL(掛売_売上合計, 0) AS 掛売合計
                ,ISNULL(E_売上合計, 0) AS E合計
                ,ISNULL(現金_値引合計, 0) + ISNULL(掛売_値引合計, 0) + ISNULL(E_値引合計, 0) AS 総値引
                ,ISNULL(合計_チケット売上, 0) AS チケット
                ,ISNULL(合計_バークレー売上, 0) AS バークレー
            FROM
                売上精算_ヘッダ
            WHERE
                企業ＣＤ = @企業ＣＤ
                AND 精算日 >= @精算日From
                AND 精算日 <= @精算日To
                AND 部署ＣＤ = @部署ＣＤ
            ) AS B
            INNER JOIN 管理マスタ AS K ON K.企業ＣＤ = B.企業ＣＤ
        GROUP BY
            B.企業ＣＤ
            ,B.コースＣＤ
            ,K.チケット回収時売上フラグ
        ) AS UB
    ON UB.コースＣＤ = C.コースＣＤ
    LEFT JOIN コースマスタ AS CM ON CM.企業ＣＤ = @企業ＣＤ AND CM.コースＣＤ = C.コースＣＤ
    LEFT JOIN 担当者マスタ AS TM ON TM.企業ＣＤ = CM.企業ＣＤ AND TM.担当者ＣＤ = CM.担当者ＣＤ
ORDER BY
    C.コースＣＤ
