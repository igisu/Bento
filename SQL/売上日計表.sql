-- 売上集計表(売上日計表)
SELECT
    @チケット回収時売上フラグ AS 総売上見出しフラグ
    ,'2016年09月15日(木曜日)' AS 精算日
    ,'売上日計表' AS タイトル
    ,C.コースＣＤ
    ,C.コース名
    ,UB.配送担当者ＣＤ AS 担当者ＣＤ
    ,UB.配送担当者名 AS 担当者名
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
    -- コースマスタから取得（実施日に該当する臨時コースマスタがあればコースマスタからは取得しない。無ければ取得する）
    (SELECT
        CM.コースＣＤ
        ,CM.コース名
    FROM
        コースマスタ AS CM
        LEFT JOIN
            (SELECT
                コースＣＤ
            FROM
                臨時コースマスタ
            WHERE
                企業ＣＤ = @企業ＣＤ
                AND コース区分 = @コース区分
                AND 部署ＣＤ = @部署ＣＤ
                AND ISNULL(工場フラグ, 0) = 0
                AND ISNULL(無効フラグ, 0) = 0
                AND 実施日 = @精算日
            ) AS RM
            ON RM.コースＣＤ = CM.コースＣＤ
    WHERE
        CM.企業ＣＤ = @企業ＣＤ
        AND CM.コース区分 = @コース区分
        AND CM.部署ＣＤ = @部署ＣＤ
        AND ISNULL(CM.工場フラグ, 0) = 0
        AND ISNULL(CM.無効フラグ, 0) = 0
        AND RM.コースＣＤ IS NULL
    -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
    UNION
    SELECT
        RM.コースＣＤ
        ,RM.コース名
    FROM
        臨時コーステーブル AS RT
        INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0
    WHERE
        RM.企業ＣＤ = @企業ＣＤ
        AND RM.コース区分 = @コース区分
        AND RM.部署ＣＤ = @部署ＣＤ
        AND ISNULL(RM.工場フラグ, 0) = 0
        AND ISNULL(RT.無効フラグ, 0) = 0
        AND RM.実施日 = @精算日
    UNION
    -- 集計対象の売上精算_ヘッダにあるコースも取得
    SELECT
        UH.コースＣＤ
        ,ISNULL(RM.コース名, CM.コース名) AS コース名   -- 臨時コース名を優先して取得
    FROM
        売上精算_ヘッダ AS UH
        INNER JOIN コースマスタ AS CM ON UH.コースＣＤ = CM.コースＣＤ
        LEFT JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = UH.企業ＣＤ AND RM.コースＣＤ = UH.コースＣＤ AND RM.実施日 = UH.精算日 AND ISNULL(RM.無効フラグ, 0) = 0
    WHERE
        UH.企業ＣＤ = @企業ＣＤ
        AND UH.精算日 = @精算日
        AND UH.部署ＣＤ = @部署ＣＤ
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
            AND UM.精算日 = @精算日
            AND UM.部署ＣＤ = @部署ＣＤ
        GROUP BY
            UM.コースＣＤ
        ) AS UA
    ON UA.コースＣＤ = C.コースＣＤ
    -- ③売上精算_ヘッダから金額を取得
    LEFT JOIN
        (SELECT
            B.コースＣＤ
            ,B.配送担当者ＣＤ
            ,B.配送担当者名
            ,B.件数
            ,B.現金売上
            ,B.チケット
            ,B.チケット代
            ,B.回収現金
            ,B.現金合計
            ,B.掛売合計
            ,B.E合計 + B.E_チケット束代 AS E合計
            ,CASE WHEN ISNULL(K.チケット回収時売上フラグ, 0) = 0
             THEN B.現金売上 + B.チケット代 + B.掛売合計 + B.E合計 + B.E_チケット束代
             ELSE B.現金売上 + B.チケット + B.掛売合計 + B.E合計 + B.E_チケット束代     -- 電子決済のチケット束代は常に加算する
             END AS 総売上
            ,B.総値引
            ,B.バークレー
            ,CASE WHEN ISNULL(K.チケット回収時売上フラグ, 0) = 0
             THEN B.現金売上 + B.チケット代 + B.掛売合計 + B.E合計 + B.E_チケット束代 - B.総値引 + B.バークレー
             ELSE B.現金売上 + B.チケット + B.掛売合計 + B.E合計 + B.E_チケット束代 - B.総値引 + B.バークレー       -- 電子決済のチケット束代は常に加算する
             END AS 実質売上
        FROM
            (SELECT
                企業ＣＤ
                ,コースＣＤ
                ,配送担当者ＣＤ
                ,配送担当者名
                ,ISNULL(得意先数, 0) AS 件数
                ,ISNULL(現金_売上合計, 0) AS 現金売上
                ,ISNULL(現金_チケット束代, 0) AS チケット代
                ,ISNULL(現金_回収現金, 0) AS 回収現金
                ,ISNULL(現金_売上合計, 0) + ISNULL(現金_チケット束代, 0) + ISNULL(現金_回収現金, 0) AS 現金合計
                ,ISNULL(掛売_売上合計, 0) AS 掛売合計
                ,ISNULL(E_売上合計, 0) AS E合計
                ,ISNULL(E_チケット束代, 0) AS E_チケット束代
                ,ISNULL(現金_値引合計, 0) + ISNULL(掛売_値引合計, 0) + ISNULL(E_値引合計, 0) AS 総値引
                ,ISNULL(合計_チケット売上, 0) AS チケット
                ,ISNULL(合計_バークレー売上, 0) AS バークレー
            FROM
                売上精算_ヘッダ
            WHERE
                企業ＣＤ = @企業ＣＤ
                AND 精算日 = @精算日
                AND 部署ＣＤ = @部署ＣＤ
            ) AS B
            INNER JOIN 管理マスタ AS K ON K.企業ＣＤ = B.企業ＣＤ
        ) AS UB
    ON UB.コースＣＤ = C.コースＣＤ
ORDER BY
    C.コースＣＤ
