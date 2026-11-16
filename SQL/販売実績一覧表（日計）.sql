-- 販売実績一覧表（日計）
-------
-- 明細
-------
SELECT
    '2018年02月05日(月曜日)' AS 精算日
    ,'販売実績一覧表（日計）' AS タイトル
    ,C.部署ＣＤ
    ,BM.部署名
    ,C.コースＣＤ
    ,C.コース名
    ,UB.配送担当者ＣＤ AS 担当者ＣＤ
    ,UB.配送担当者名 AS 担当者名
    ,UC.持出数副食
    ,UC.持出数主食
    ,UC.工場追加数副食 + もらった数副食 AS 貰い副食
    ,UC.工場追加数主食 + もらった数主食 AS 貰い主食
    ,UC.やった数副食 AS 渡し副食
    ,UC.やった数主食 AS 渡し主食
    ,UC.販売数副食
    ,UC.販売数主食
    ,UC.残数副食
    ,UC.残数主食
    ,UA.弁当数量 + UA.副食数量 AS 食数
    ,UB.現金売上
    ,UB.掛売売上
    ,UB.E売上
    ,UB.その他
    ,UB.実質売上
    ,CASE WHEN (UA.弁当数量 + UA.副食数量) = 0 THEN 0 ELSE FLOOR(UB.実質売上 / (UA.弁当数量 + UA.副食数量)) END AS 平均単価
    ,0 AS 合計行フラグ
    ,C.部署ＣＤ AS ソート用部署ＣＤ
    ,C.コースＣＤ AS ソート用コースＣＤ
FROM
    -- ①駆動表
    -- コースマスタから取得（実施日に該当する臨時コースマスタがあればコースマスタからは取得しない。無ければ取得する）
    (SELECT
        CM.部署ＣＤ
        ,CM.コースＣＤ
        ,CM.コース名
        ,CM.担当者ＣＤ
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
        RM.部署ＣＤ
        ,RM.コースＣＤ
        ,RM.コース名
        ,RM.担当者ＣＤ
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
    ) AS C
    -- ②売上精算_売上明細から弁当・副食の数量（＝食数）を取得
    LEFT JOIN
        (SELECT
            UM.部署ＣＤ
            ,UM.コースＣＤ
            ,SUM(CASE WHEN SM.弁当区分 = @弁当区分弁当 THEN UM.売上数量 ELSE 0 END) AS 弁当数量
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
            UM.部署ＣＤ
            ,UM.コースＣＤ
        ) AS UA
    ON UA.部署ＣＤ = C.部署ＣＤ AND UA.コースＣＤ = C.コースＣＤ
    -- ③売上精算_ヘッダから金額を取得
    LEFT JOIN
        (SELECT
            B.部署ＣＤ
            ,B.コースＣＤ
            ,B.配送担当者ＣＤ
            ,B.配送担当者名
            ,B.現金売上
            ,B.掛売売上
            ,B.E売上
            ,B.その他
            ,B.現金売上 + B.掛売売上 + B.E売上 + B.その他 AS 実質売上
        FROM
            (SELECT
                部署ＣＤ
                ,コースＣＤ
                ,配送担当者ＣＤ
                ,配送担当者名
                ,ISNULL(現金_売上合計, 0) - ISNULL(現金_値引合計, 0) AS 現金売上
                ,ISNULL(掛売_売上合計, 0) - ISNULL(掛売_値引合計, 0) AS 掛売売上
                ,ISNULL(E_売上合計, 0)    - ISNULL(E_値引合計, 0)    AS E売上
                ,ISNULL(合計_チケット売上, 0) + ISNULL(合計_バークレー売上, 0) AS その他
            FROM
                売上精算_ヘッダ
            WHERE
                企業ＣＤ = @企業ＣＤ
                AND 精算日 = @精算日
                AND 部署ＣＤ = @部署ＣＤ
            ) AS B
        ) AS UB
    ON UB.部署ＣＤ = C.部署ＣＤ AND UB.コースＣＤ = C.コースＣＤ
    -- ④売上精算_持出明細から持ち出し数を取得
    LEFT JOIN
        (SELECT
            M.精算日
            ,M.部署ＣＤ
            ,M.コースＣＤ
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.持出個数, 0)       ELSE 0 END) AS 持出数副食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.持出個数, 0)       ELSE 0 END) AS 持出数主食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.工場追加数, 0)     ELSE 0 END) AS 工場追加数副食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.工場追加数, 0)     ELSE 0 END) AS 工場追加数主食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.もらった数, 0)     ELSE 0 END) AS もらった数副食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.もらった数, 0)     ELSE 0 END) AS もらった数主食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.やった数, 0)       ELSE 0 END) AS やった数副食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.やった数, 0)       ELSE 0 END) AS やった数主食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.残数, 0)           ELSE 0 END) AS 残数副食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.残数, 0)           ELSE 0 END) AS 残数主食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.売上予定実績数, 0) ELSE 0 END) AS 販売数副食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.売上予定実績数, 0) ELSE 0 END) AS 販売数主食
        FROM
            売上精算_持出明細 AS M
            LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = M.企業ＣＤ AND S.商品ＣＤ = M.商品ＣＤ
        WHERE
            M.企業ＣＤ = @企業ＣＤ
            AND M.精算日 = @精算日
            AND M.部署ＣＤ = @部署ＣＤ
        GROUP BY
            M.精算日
            ,M.部署ＣＤ
            ,M.コースＣＤ
        ) AS UC
    ON UC.部署ＣＤ = C.部署ＣＤ AND UC.コースＣＤ = C.コースＣＤ
    LEFT JOIN 部署マスタ AS BM ON BM.企業ＣＤ = @企業ＣＤ AND BM.部署ＣＤ = C.部署ＣＤ
UNION
-----------------
-- 部署ごとの小計
-----------------
SELECT
    '2018年02月05日(月曜日)' AS 精算日
    ,'販売実績一覧表（日計）' AS タイトル
    ,C.部署ＣＤ
    ,BM.部署名
    ,NULL AS コースＣＤ
    ,BM.部署名 + ' 計'  AS コース名
    ,NULL AS 担当者ＣＤ
    ,NULL AS 担当者名
    ,SUM(UC.持出数副食) AS 持出数副食
    ,SUM(UC.持出数主食) AS 持出数主食
    ,SUM(UC.工場追加数副食 + もらった数副食) AS 貰い副食
    ,SUM(UC.工場追加数主食 + もらった数主食) AS 貰い主食
    ,SUM(UC.やった数副食) AS 渡し副食
    ,SUM(UC.やった数主食) AS 渡し主食
    ,SUM(UC.販売数副食) AS 販売数副食
    ,SUM(UC.販売数主食) AS 販売数主食
    ,SUM(UC.残数副食) AS 残数副食
    ,SUM(UC.残数主食) AS 残数主食
    ,SUM(UA.弁当数量 + UA.副食数量) AS 食数
    ,SUM(UB.現金売上) AS 現金売上
    ,SUM(UB.掛売売上) AS 掛売売上
    ,SUM(UB.E売上) AS E売上
    ,SUM(UB.その他) AS その他
    ,SUM(UB.実質売上) AS 実質売上
    ,CASE WHEN (SUM(UA.弁当数量 + UA.副食数量)) = 0 THEN 0 ELSE FLOOR(SUM(UB.実質売上) / SUM(UA.弁当数量 + UA.副食数量)) END AS 平均単価
    ,1 AS 合計行フラグ
    ,C.部署ＣＤ AS ソート用部署ＣＤ
    ,999999999 AS ソート用コースＣＤ
FROM
    -- ①駆動表
    -- コースマスタから取得（実施日に該当する臨時コースマスタがあればコースマスタからは取得しない。無ければ取得する）
    (SELECT
        CM.部署ＣＤ
        ,CM.コースＣＤ
        ,CM.コース名
        ,CM.担当者ＣＤ
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
        RM.部署ＣＤ
        ,RM.コースＣＤ
        ,RM.コース名
        ,RM.担当者ＣＤ
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
    ) AS C
    -- ②売上精算_売上明細から弁当・副食の数量（＝食数）を取得
    LEFT JOIN
        (SELECT
            UM.部署ＣＤ
            ,UM.コースＣＤ
            ,SUM(CASE WHEN SM.弁当区分 = @弁当区分弁当 THEN UM.売上数量 ELSE 0 END) AS 弁当数量
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
            UM.部署ＣＤ
            ,UM.コースＣＤ
        ) AS UA
    ON UA.部署ＣＤ = C.部署ＣＤ AND UA.コースＣＤ = C.コースＣＤ
    -- ③売上精算_ヘッダから金額を取得
    LEFT JOIN
        (SELECT
            B.部署ＣＤ
            ,B.コースＣＤ
            ,B.配送担当者ＣＤ
            ,B.配送担当者名
            ,B.現金売上
            ,B.掛売売上
            ,B.E売上
            ,B.その他
            ,B.現金売上 + B.掛売売上 + B.E売上 + B.その他 AS 実質売上
        FROM
            (SELECT
                部署ＣＤ
                ,コースＣＤ
                ,配送担当者ＣＤ
                ,配送担当者名
                ,ISNULL(現金_売上合計, 0) - ISNULL(現金_値引合計, 0) AS 現金売上
                ,ISNULL(掛売_売上合計, 0) - ISNULL(掛売_値引合計, 0) AS 掛売売上
                ,ISNULL(E_売上合計, 0)    - ISNULL(E_値引合計, 0)    AS E売上
                ,ISNULL(合計_チケット売上, 0) + ISNULL(合計_バークレー売上, 0) AS その他
            FROM
                売上精算_ヘッダ
            WHERE
                企業ＣＤ = @企業ＣＤ
                AND 精算日 = @精算日
                AND 部署ＣＤ = @部署ＣＤ
            ) AS B
        ) AS UB
    ON UB.部署ＣＤ = C.部署ＣＤ AND UB.コースＣＤ = C.コースＣＤ
    -- ④売上精算_持出明細から持ち出し数を取得
    LEFT JOIN
        (SELECT
            M.精算日
            ,M.部署ＣＤ
            ,M.コースＣＤ
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.持出個数, 0)       ELSE 0 END) AS 持出数副食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.持出個数, 0)       ELSE 0 END) AS 持出数主食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.工場追加数, 0)     ELSE 0 END) AS 工場追加数副食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.工場追加数, 0)     ELSE 0 END) AS 工場追加数主食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.もらった数, 0)     ELSE 0 END) AS もらった数副食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.もらった数, 0)     ELSE 0 END) AS もらった数主食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.やった数, 0)       ELSE 0 END) AS やった数副食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.やった数, 0)       ELSE 0 END) AS やった数主食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.残数, 0)           ELSE 0 END) AS 残数副食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.残数, 0)           ELSE 0 END) AS 残数主食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.売上予定実績数, 0) ELSE 0 END) AS 販売数副食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.売上予定実績数, 0) ELSE 0 END) AS 販売数主食
        FROM
            売上精算_持出明細 AS M
            LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = M.企業ＣＤ AND S.商品ＣＤ = M.商品ＣＤ
        WHERE
            M.企業ＣＤ = @企業ＣＤ
            AND M.精算日 = @精算日
            AND M.部署ＣＤ = @部署ＣＤ
        GROUP BY
            M.精算日
            ,M.部署ＣＤ
            ,M.コースＣＤ
        ) AS UC
    ON UC.部署ＣＤ = C.部署ＣＤ AND UC.コースＣＤ = C.コースＣＤ
    LEFT JOIN 部署マスタ AS BM ON BM.企業ＣＤ = @企業ＣＤ AND BM.部署ＣＤ = C.部署ＣＤ
GROUP BY
    C.部署ＣＤ
    ,BM.部署名
UNION
---------
-- 総合計
---------
SELECT
    '2018年02月05日(月曜日)' AS 精算日
    ,'販売実績一覧表（日計）' AS タイトル
    ,NULL AS 部署ＣＤ
    ,NULL AS 部署名
    ,NULL AS コースＣＤ
    ,'総 合 計'  AS コース名
    ,NULL AS 担当者ＣＤ
    ,NULL AS 担当者名
    ,SUM(UC.持出数副食) AS 持出数副食
    ,SUM(UC.持出数主食) AS 持出数主食
    ,SUM(UC.工場追加数副食 + もらった数副食) AS 貰い副食
    ,SUM(UC.工場追加数主食 + もらった数主食) AS 貰い主食
    ,SUM(UC.やった数副食) AS 渡し副食
    ,SUM(UC.やった数主食) AS 渡し主食
    ,SUM(UC.販売数副食) AS 販売数副食
    ,SUM(UC.販売数主食) AS 販売数主食
    ,SUM(UC.残数副食) AS 残数副食
    ,SUM(UC.残数主食) AS 残数主食
    ,SUM(UA.弁当数量 + UA.副食数量) AS 食数
    ,SUM(UB.現金売上) AS 現金売上
    ,SUM(UB.掛売売上) AS 掛売売上
    ,SUM(UB.E売上) AS E売上
    ,SUM(UB.その他) AS その他
    ,SUM(UB.実質売上) AS 実質売上
    ,CASE WHEN (SUM(UA.弁当数量 + UA.副食数量)) = 0 THEN 0 ELSE FLOOR(SUM(UB.実質売上) / SUM(UA.弁当数量 + UA.副食数量)) END AS 平均単価
    ,1 AS 合計行フラグ
    ,999999999 AS ソート用部署ＣＤ
    ,999999999 AS ソート用コースＣＤ
FROM
    -- ①駆動表
    -- コースマスタから取得（実施日に該当する臨時コースマスタがあればコースマスタからは取得しない。無ければ取得する）
    (SELECT
        CM.部署ＣＤ
        ,CM.コースＣＤ
        ,CM.コース名
        ,CM.担当者ＣＤ
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
        RM.部署ＣＤ
        ,RM.コースＣＤ
        ,RM.コース名
        ,RM.担当者ＣＤ
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
    ) AS C
    -- ②売上精算_売上明細から弁当・副食の数量（＝食数）を取得
    LEFT JOIN
        (SELECT
            UM.部署ＣＤ
            ,UM.コースＣＤ
            ,SUM(CASE WHEN SM.弁当区分 = @弁当区分弁当 THEN UM.売上数量 ELSE 0 END) AS 弁当数量
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
            UM.部署ＣＤ
            ,UM.コースＣＤ
        ) AS UA
    ON UA.部署ＣＤ = C.部署ＣＤ AND UA.コースＣＤ = C.コースＣＤ
    -- ③売上精算_ヘッダから金額を取得
    LEFT JOIN
        (SELECT
            B.部署ＣＤ
            ,B.コースＣＤ
            ,B.配送担当者ＣＤ
            ,B.配送担当者名
            ,B.現金売上
            ,B.掛売売上
            ,B.E売上
            ,B.その他
            ,B.現金売上 + B.掛売売上 + B.E売上+ B.その他 AS 実質売上
        FROM
            (SELECT
                部署ＣＤ
                ,コースＣＤ
                ,配送担当者ＣＤ
                ,配送担当者名
                ,ISNULL(現金_売上合計, 0) - ISNULL(現金_値引合計, 0) AS 現金売上
                ,ISNULL(掛売_売上合計, 0) - ISNULL(掛売_値引合計, 0) AS 掛売売上
                ,ISNULL(E_売上合計, 0)    - ISNULL(E_値引合計, 0)    AS E売上
                ,ISNULL(合計_チケット売上, 0) + ISNULL(合計_バークレー売上, 0) AS その他
            FROM
                売上精算_ヘッダ
            WHERE
                企業ＣＤ = @企業ＣＤ
                AND 精算日 = @精算日
                AND 部署ＣＤ = @部署ＣＤ
            ) AS B
        ) AS UB
    ON UB.部署ＣＤ = C.部署ＣＤ AND UB.コースＣＤ = C.コースＣＤ
    -- ④売上精算_持出明細から持ち出し数を取得
    LEFT JOIN
        (SELECT
            M.精算日
            ,M.部署ＣＤ
            ,M.コースＣＤ
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.持出個数, 0)       ELSE 0 END) AS 持出数副食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.持出個数, 0)       ELSE 0 END) AS 持出数主食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.工場追加数, 0)     ELSE 0 END) AS 工場追加数副食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.工場追加数, 0)     ELSE 0 END) AS 工場追加数主食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.もらった数, 0)     ELSE 0 END) AS もらった数副食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.もらった数, 0)     ELSE 0 END) AS もらった数主食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.やった数, 0)       ELSE 0 END) AS やった数副食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.やった数, 0)       ELSE 0 END) AS やった数主食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.残数, 0)           ELSE 0 END) AS 残数副食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.残数, 0)           ELSE 0 END) AS 残数主食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.売上予定実績数, 0) ELSE 0 END) AS 販売数副食
            ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.売上予定実績数, 0) ELSE 0 END) AS 販売数主食
        FROM
            売上精算_持出明細 AS M
            LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = M.企業ＣＤ AND S.商品ＣＤ = M.商品ＣＤ
        WHERE
            M.企業ＣＤ = @企業ＣＤ
            AND M.精算日 = @精算日
            AND M.部署ＣＤ = @部署ＣＤ
        GROUP BY
            M.精算日
            ,M.部署ＣＤ
            ,M.コースＣＤ
        ) AS UC
    ON UC.部署ＣＤ = C.部署ＣＤ AND UC.コースＣＤ = C.コースＣＤ
ORDER BY
    ソート用部署ＣＤ
    ,ソート用コースＣＤ



@企業ＣＤ = 40000
@コース区分 = 1
@部署ＣＤ = 1
@精算日 = '2018/02/05'
@弁当区分弁当 = 0
@弁当区分副食 = 1
@弁当区分主食 = 2
