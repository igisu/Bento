--- 商品名取得
-- 以下のように並べる
-- ┌───┬───┬───┐
-- │商品Ａ│商品Ｂ│その他│
-- └───┴───┴───┘
SELECT
    0 AS その他区分
    ,A.商品ＣＤ AS ソート用商品ＣＤ
    ,CONVERT(varchar, A.商品ＣＤ) AS 商品ＣＤ
    ,S.商品名略称
FROM
    主要商品リストマスタ AS A
    LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND A.商品ＣＤ = S.商品ＣＤ
WHERE
    A.企業ＣＤ = @企業ＣＤ
    AND A.部署ＣＤ = @部署ＣＤ
    AND ISNULL(A.無効フラグ, 0) = 0
UNION
SELECT
    1 AS その他区分
    ,-1 AS ソート用商品ＣＤ
    ,'XXX' AS 商品ＣＤ
    ,'その他' AS 商品名略称
ORDER BY
    その他区分
    ,ソート用商品ＣＤ
    ,商品ＣＤ


--- コース注文入力
SELECT
    0 AS 明細合計区分,
    C.得意先ＣＤ,
    CASE T.受注方法 WHEN 1 THEN RIGHT('00' + CONVERT(varchar, T.電話確認時間_時), 2) + ':' + RIGHT('00' + CONVERT(varchar, T.電話確認時間_分), 2)
                    ELSE '不' END AS 発信,
    T.得意先名,
    -- T.得意先名略称 AS 得意先名,
    THR.連絡,
    VK.売掛現金区分名 AS 種類,
    ISNULL(CK.注文計, 0) AS 注文計,
    ISNULL(UK.現金計, 0) AS 現金計,
    ISNULL(H.備考1, T.備考1) AS 備考,
    A.商品ＣＤ,
    A.商品予測数量,
    A.商品注文数量,
    A.商品売上数量,
    CASE WHEN ISNULL(A.商品注文数量, -1) >= 0 THEN 1 ELSE 0 END AS 注文フラグ,
    CASE WHEN ISNULL(A.商品売上数量, -1) >= 0 THEN 1 ELSE 0 END AS 売上フラグ,
    CASE WHEN ISNULL(A.商品売上数量, -1) >= 0 THEN A.商品売上数量
         WHEN ISNULL(A.商品注文数量, -1) >= 0 THEN A.商品注文数量
         ELSE A.商品予測数量 END AS 商品数量,
    CASE WHEN CK.注文計 IS NULL THEN 0 ELSE 1 END AS 注文有無フラグ,
    CASE WHEN CO.コース順 IS NULL THEN 1 ELSE 0 END AS コースに含む順,
    CO.コース順
FROM
-- ①駆動表
-- ①－１（指定されたコースにぶら下がる得意先）
    -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
    (SELECT
        CT.企業ＣＤ
        ,CT.コースＣＤ
        ,CT.得意先ＣＤ
    FROM
        コーステーブル AS CT
        INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0
        LEFT JOIN
            (SELECT
                コースＣＤ
            FROM
                臨時コースマスタ
            WHERE
                企業ＣＤ = @企業ＣＤ
                AND ISNULL(無効フラグ, 0) = 0
                AND コースＣＤ = @コースＣＤ
                AND 実施日 = @注文日
            ) AS RM
            ON RM.コースＣＤ = CT.コースＣＤ
    WHERE
        CT.企業ＣＤ = @企業ＣＤ
        AND CT.コースＣＤ = @コースＣＤ
        AND ISNULL(CT.無効フラグ, 0) = 0
        AND RM.コースＣＤ IS NULL
    -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
    UNION
    SELECT
        RT.企業ＣＤ
        ,RT.コースＣＤ
        ,RT.得意先ＣＤ
    FROM
        臨時コーステーブル AS RT
        INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0
    WHERE
        RT.企業ＣＤ = @企業ＣＤ
        AND RT.コースＣＤ = @コースＣＤ
        AND ISNULL(RT.無効フラグ, 0) = 0
        AND RT.実施日 = @注文日
-- ①－２（指定されたコースを持つ注文の得意先）
    UNION
    SELECT
        企業ＣＤ
        ,コースＣＤ
        ,得意先ＣＤ
    FROM
        注文ヘッダ
    WHERE
        企業ＣＤ = @企業ＣＤ
        AND コースＣＤ = @コースＣＤ
        AND 喫食日 = @注文日
        AND 部署ＣＤ = @部署ＣＤ
-- ①－３（指定されたコースを持つ売上の得意先）
    UNION
    SELECT
        企業ＣＤ
        ,コースＣＤ
        ,得意先ＣＤ
    FROM
        売上ヘッダ
    WHERE
        企業ＣＤ = @企業ＣＤ
        AND コースＣＤ = @コースＣＤ
        AND 売上日 = @注文日
        AND 部署ＣＤ = @部署ＣＤ
    ) AS C
    LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ
    LEFT JOIN V_売掛現金区分 AS VK ON VK.売掛現金区分CD = ISNULL(T.売掛現金区分, @売掛現金区分現金)     -- デフォルトは現金
-- ②駆動表に紐付く注文と売上
    LEFT JOIN
        (SELECT
            A1.コースＣＤ
            ,A1.得意先ＣＤ
            ,A1.商品ＣＤ
            ,SUM(A1.商品注文数量) AS 商品注文数量
            ,SUM(A1.商品予測数量) AS 商品予測数量
            ,SUM(A1.商品売上数量) AS 商品売上数量
        FROM
            -- ②－１駆動表に紐付く注文
            (SELECT
                H.コースＣＤ
                ,M.得意先ＣＤ
                ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') AS 商品ＣＤ
                ,SUM(CASE WHEN M.注文区分 = @注文区分注文 THEN M.商品数量 ELSE null END) AS 商品注文数量
                ,SUM(CASE WHEN M.注文区分 = @注文区分予測 THEN M.商品数量 ELSE null END) AS 商品予測数量
                ,null AS 商品売上数量
            FROM
                注文ヘッダ AS H
                INNER JOIN 注文明細 AS M
                    ON  H.企業ＣＤ   = M.企業ＣＤ
                    AND H.注文区分   = M.注文区分
                    AND H.喫食日     = M.喫食日
                    AND H.得意先ＣＤ = M.得意先ＣＤ
                    AND H.部署ＣＤ   = M.部署ＣＤ
                LEFT JOIN 主要商品リストマスタ AS A
                    ON A.企業ＣＤ = M.企業ＣＤ
                    AND A.商品ＣＤ = M.商品ＣＤ
                    AND A.部署ＣＤ = M.部署ＣＤ
                    AND ISNULL(A.無効フラグ, 0) = 0
            WHERE
                H.企業ＣＤ = @企業ＣＤ
                AND H.喫食日 = @注文日
                AND H.部署ＣＤ = @部署ＣＤ
            GROUP BY
                H.コースＣＤ
                ,M.得意先ＣＤ
                ,A.商品ＣＤ
            UNION
            -- ②－２駆動表に紐付く売上
            SELECT
                H.コースＣＤ
                ,M.得意先ＣＤ
                ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') AS 商品ＣＤ
                ,null AS 商品注文数量
                ,null AS 商品予測数量
                ,SUM(M.商品数量) AS 商品売上数量
            FROM
                売上ヘッダ AS H
                INNER JOIN 売上明細 AS M
                    ON  H.企業ＣＤ   = M.企業ＣＤ
                    AND H.売上日     = M.売上日
                    AND H.得意先ＣＤ = M.得意先ＣＤ
                    AND H.部署ＣＤ   = M.部署ＣＤ
                LEFT JOIN 主要商品リストマスタ AS A
                    ON A.企業ＣＤ = M.企業ＣＤ
                    AND A.商品ＣＤ = M.商品ＣＤ
                    AND A.部署ＣＤ = M.部署ＣＤ
                    AND ISNULL(A.無効フラグ, 0) = 0
            WHERE
                M.企業ＣＤ = @企業ＣＤ
                AND M.売上日   = @注文日
                AND M.部署ＣＤ = @部署ＣＤ
            GROUP BY
                H.コースＣＤ
                ,M.得意先ＣＤ
                ,A.商品ＣＤ
            ) AS A1
        GROUP BY
            A1.コースＣＤ
            ,A1.得意先ＣＤ
            ,A1.商品ＣＤ
        ) AS A ON A.得意先ＣＤ = C.得意先ＣＤ AND A.コースＣＤ = C.コースＣＤ
-- ③備考取得用
    LEFT JOIN
        (SELECT
            得意先ＣＤ
            ,備考1
        FROM
            注文ヘッダ
        WHERE
            企業ＣＤ = @企業ＣＤ
            AND 注文区分 = @注文区分注文
            AND コースＣＤ = @コースＣＤ
            AND 喫食日 = @注文日
            AND 部署ＣＤ = @部署ＣＤ
        ) AS H
        ON H.得意先ＣＤ = C.得意先ＣＤ
-- ④注文計
    LEFT JOIN
        (SELECT
            H.コースＣＤ
            ,M.得意先ＣＤ
            ,SUM(M.商品金額合計税込) AS 注文計
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
            AND H.喫食日 = @注文日
            AND H.部署ＣＤ = @部署ＣＤ
            AND M.注文区分 = @注文区分注文      -- 注文
        GROUP BY
            H.コースＣＤ
            ,M.得意先ＣＤ
        ) AS CK ON CK.得意先ＣＤ = C.得意先ＣＤ AND CK.コースＣＤ = C.コースＣＤ
-- ⑤現金計
    LEFT JOIN
        (SELECT
            H.コースＣＤ
            ,M.得意先ＣＤ
            ,SUM(M.商品金額合計税込) AS 現金計
        FROM
            売上ヘッダ AS H
            INNER JOIN 売上明細 AS M
                ON  H.企業ＣＤ   = M.企業ＣＤ
                AND H.売上日     = M.売上日
                AND H.得意先ＣＤ = M.得意先ＣＤ
                AND H.部署ＣＤ   = M.部署ＣＤ
        WHERE
            M.企業ＣＤ = @企業ＣＤ
            AND M.売上日   = @注文日
            AND M.部署ＣＤ = @部署ＣＤ
            AND M.売上区分 = @売上区分現金      -- 現金
        GROUP BY
            H.コースＣＤ
            ,M.得意先ＣＤ
        ) AS UK ON UK.得意先ＣＤ = C.得意先ＣＤ AND UK.コースＣＤ = C.コースＣＤ
-- ⑥連絡（得意先発信履歴データ）
    LEFT JOIN
        (SELECT DISTINCT
            THR_A.得意先ＣＤ
            ,THR_A.最終発信日時
            ,THR_A.不要フラグ
            ,THR_A.保留フラグ
            ,CASE WHEN THR_A.保留フラグ = 1 THEN '保留'
                  WHEN THR_A.最終発信日時 IS NOT NULL THEN '済'
                  ELSE '未'
             END AS 連絡
        FROM
            得意先発信履歴データ AS THR_A
            INNER JOIN
                (SELECT
                    得意先ＣＤ
                    ,MAX(修正日) AS 修正日
                FROM
                    得意先発信履歴データ
                WHERE
                    企業ＣＤ = @企業ＣＤ
                    AND 発信日 = @注文日
                GROUP BY
                    得意先ＣＤ
            ) AS THR_M ON THR_M.得意先ＣＤ = THR_A.得意先ＣＤ AND THR_M.修正日 = THR_A.修正日
        WHERE
            THR_A.企業ＣＤ = @企業ＣＤ
            AND THR_A.発信日 = @注文日
        ) AS THR ON THR.得意先ＣＤ = C.得意先ＣＤ
-- ⑦コース順（①で取得していないのはなぜ？①－２からはコース順が取得できないので代わりにNULLにするとUNIONしたときに複数行できてしまうため）
    LEFT JOIN
        (SELECT
            CT.得意先ＣＤ
            ,CT.コース順
        FROM
            コーステーブル AS CT
            INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0
            LEFT JOIN
                (SELECT
                    コースＣＤ
                FROM
                    臨時コースマスタ
                WHERE
                    企業ＣＤ = @企業ＣＤ
                    AND ISNULL(無効フラグ, 0) = 0
                    AND コースＣＤ = @コースＣＤ
                    AND 実施日 = @注文日
                ) AS RM
                ON RM.コースＣＤ = CT.コースＣＤ
        WHERE
            CT.企業ＣＤ = @企業ＣＤ
            AND CT.コースＣＤ = @コースＣＤ
            AND ISNULL(CT.無効フラグ, 0) = 0
            AND RM.コースＣＤ IS NULL
        -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
        UNION
        SELECT
            RT.得意先ＣＤ
            ,RT.コース順
        FROM
            臨時コーステーブル AS RT
            INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0
        WHERE
            RT.企業ＣＤ = @企業ＣＤ
            AND RT.コースＣＤ = @コースＣＤ
            AND ISNULL(RT.無効フラグ, 0) = 0
            AND RT.実施日 = @注文日
        ) AS CO ON C.得意先ＣＤ = CO.得意先ＣＤ
WHERE
    1 = 1
--    AND CK.注文計 IS NULL     -- 注文が無い
--    AND CK.注文計 IS NOT NULL -- 注文がある
--    AND THR.連絡 = '済'       -- 連絡が「全て」以外は条件に加える
-- ⑧注文合計
UNION
SELECT
    1 AS 明細合計区分,
    NULL AS 得意先ＣＤ,
    NULL AS 発信,
    '受注合計' AS 得意先名,
    NULL AS 連絡,
    NULL AS 種類,
    NULL AS 注文計,
    NULL AS 現金計,
    NULL AS 備考,
    NULL AS 商品ＣＤ,
    NULL AS 商品予測数量,
    NULL AS 商品注文数量,
    NULL AS 商品売上数量,
    NULL AS 注文フラグ,
    NULL AS 売上フラグ,
    NULL AS 商品数量,
    -1 AS 注文有無フラグ,
    -1 AS コースに含む順,
    -1 AS コース順
-- ⑨注文＋予測合計
UNION
SELECT
    2 AS 明細合計区分,
    NULL AS 得意先ＣＤ,
    NULL AS 発信,
    '実績合計' AS 得意先名,
    NULL AS 連絡,
    NULL AS 種類,
    NULL AS 注文計,
    NULL AS 現金計,
    NULL AS 備考,
    NULL AS 商品ＣＤ,
    NULL AS 商品予測数量,
    NULL AS 商品注文数量,
    NULL AS 商品売上数量,
    NULL AS 注文フラグ,
    NULL AS 売上フラグ,
    NULL AS 商品数量,
    -1 AS 注文有無フラグ,
    -1 AS コースに含む順,
    -1 AS コース順
ORDER BY
    明細合計区分
    ,コースに含む順
    ,コース順
    ,得意先ＣＤ


-- 注文元の注文を配送先の注文に集約する（スプレッドを階層表示する）。
-- SQLの修正を最小限にすべく、元々のコース注文入力のSQLは変更せずに、
-- 自分(=得意先)が注文元なのか配送先なのかを判断して、親テーブルに含めるのか子テーブルに含めるのかを決定する。
-- 例えばコースに属する得意先が 43101～43107の場合（つまり43109は同じコースではない場合）
-- ┌─┬─────────┬───────┬───────┐
-- │No│コースの得意先ＣＤ│配送得意先ＣＤ│含めるテーブル│
-- ├─┼─────────┼───────┼───────┤
-- │ 1│             43101│              │親            │
-- ├─┼─────────┼───────┼───────┤
-- │ 2│             43104│              │親            │
-- ├─┼─────────┼───────┼───────┤
-- │ 3│             43105│         43104│子＆親        │
-- ├─┼─────────┼───────┼───────┤
-- │ 4│             43107│         43109│親            │
-- └─┴─────────┴───────┴───────┘
-- ・コースの得意先ＣＤが配送先を持たず（つまり注文元ではない）かつ配送先にもなっていない場合は、親テーブルのみに含める（1）
-- ・コースの得意先ＣＤが配送先を持たない（つまり注文元ではない）が、配送先になってる場合は、親テーブルに含めて集計対象になる（2）
-- ・コースの得意先ＣＤが配送先を持つ（つまり注文元になる）場合は、子テーブルに含めて、親テーブルの配送得意先レコードに加算する（3）
-- ・コースの得意先ＣＤが配送先を持つ（つまり注文元になる）が、配送先がコースに属していない場合は、親テーブルのみに含める（4）

--- 集計先取得
WITH 対象コース得意先 AS (
-- ①－１（指定されたコースにぶら下がる得意先）
-- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
(SELECT
    CT.企業ＣＤ
    ,CT.コースＣＤ
    ,CT.得意先ＣＤ
FROM
    コーステーブル AS CT
    INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0
    LEFT JOIN
        (SELECT
            コースＣＤ
        FROM
            臨時コースマスタ
        WHERE
            企業ＣＤ = @企業ＣＤ
            AND ISNULL(無効フラグ, 0) = 0
            AND コースＣＤ = @コースＣＤ
            AND 実施日 = @注文日
        ) AS RM
        ON RM.コースＣＤ = CT.コースＣＤ
WHERE
    CT.企業ＣＤ = @企業ＣＤ
    AND CT.コースＣＤ = @コースＣＤ
    AND ISNULL(CT.無効フラグ, 0) = 0
    AND RM.コースＣＤ IS NULL
-- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
UNION
SELECT
    RT.企業ＣＤ
    ,RT.コースＣＤ
    ,RT.得意先ＣＤ
FROM
    臨時コーステーブル AS RT
    INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0
WHERE
    RT.企業ＣＤ = @企業ＣＤ
    AND RT.コースＣＤ = @コースＣＤ
    AND ISNULL(RT.無効フラグ, 0) = 0
    AND RT.実施日 = @注文日
-- ①－２（指定されたコースを持つ注文の得意先）
UNION
SELECT
    企業ＣＤ
    ,コースＣＤ
    ,得意先ＣＤ
FROM
    注文ヘッダ
WHERE
    企業ＣＤ = @企業ＣＤ
    AND コースＣＤ = @コースＣＤ
    AND 喫食日 = @注文日
    AND 部署ＣＤ = @部署ＣＤ
-- ①－３（指定されたコースを持つ売上の得意先）
UNION
SELECT
    企業ＣＤ
    ,コースＣＤ
    ,得意先ＣＤ
FROM
    売上ヘッダ
WHERE
    企業ＣＤ = @企業ＣＤ
    AND コースＣＤ = @コースＣＤ
    AND 売上日 = @注文日
    AND 部署ＣＤ = @部署ＣＤ
)

SELECT DISTINCT
    D.企業ＣＤ
    ,D.コースＣＤ
    ,D.得意先ＣＤ
    ,D.配送先ＣＤ
FROM
-- ①－１（指定されたコースにぶら下がる得意先(集約先)）
    (SELECT
        TH.企業ＣＤ
        ,TH.コースＣＤ
        ,TH.得意先ＣＤ
        ,TH.配送得意先ＣＤ
        ,SY.集約先ＣＤ
        ,ISNULL(SY.集約先ＣＤ, TH.得意先ＣＤ) AS 配送先ＣＤ
    FROM
    -- ①－１－１（指定されたコースにぶら下がる得意先と配送先）
        (SELECT
            W.企業ＣＤ
            ,W.得意先ＣＤ
            ,T.配送得意先ＣＤ
            ,W.コースＣＤ
        FROM
            対象コース得意先 AS W
            -- 得意先と配送先を取得したいのでLEFT JOINしている
            LEFT JOIN 得意先マスタ AS T
                ON T.企業ＣＤ = W.企業ＣＤ
                AND T.得意先ＣＤ = W.得意先ＣＤ
        ) AS TH
        -- ①－１－２（指定されたコースにぶら下がる得意先のうち、注文の集約先になりうる得意先を抽出）
        LEFT JOIN
            (SELECT DISTINCT
                W.企業ＣＤ
                ,W.得意先ＣＤ
                ,T.配送得意先ＣＤ AS 集約先ＣＤ
                ,W.コースＣＤ
            FROM
                対象コース得意先 AS W
                -- 集約先になりうる得意先のみを抽出したいのでINNER JOINしている
                -- ホントは 「ISNULL(配送得意先ＣＤ, 得意先ＣＤ) AS 配送得意先ＣＤ」 として取得したかったが、インデックスが効かなくなるので
                -- T.配送得意先ＣＤ には NULL を格納せず、代わりに得意先ＣＤを格納している
                INNER JOIN 得意先マスタ AS T
                    ON T.企業ＣＤ = W.企業ＣＤ
                    AND T.配送得意先ＣＤ = W.得意先ＣＤ
            ) AS SY
            ON ISNULL(TH.配送得意先ＣＤ, TH.得意先ＣＤ) = SY.集約先ＣＤ     -- 集約先と結合するのは配送先を優先する（集約先≒配送先なので）
    ) AS D
ORDER BY
    D.企業ＣＤ
    ,D.コースＣＤ
    ,D.得意先ＣＤ
    ,D.配送先ＣＤ



@企業ＣＤ = 40000
@部署ＣＤ = 1
@コースＣＤ = 1
@注文日 = '2018/04/10'
@注文区分注文 = 0
@注文区分予測 = 1
@売上区分現金 = 0
@売掛現金区分現金 = 0
