--- 主要商品マスタの商品名取得
-- 以下のように並べる
-- ┌───┬───┬───┐
-- │商品Ａ│商品Ｂ│その他│
-- └───┴───┴───┘
SELECT
    0 AS その他区分
    ,B.商品グループＣＤ
    ,B.商品ＣＤ AS ソート用商品ＣＤ
    ,CONVERT(varchar, B.商品ＣＤ) AS 商品ＣＤ
    ,B.商品名略称
FROM
    -- 出力できる商品の最大列数は16（その他を入れて17）
    (SELECT TOP(16)
        A.商品グループＣＤ
        ,A.商品ＣＤ
        ,S.商品名略称
    FROM
        主要商品リストマスタ AS A
        LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND A.商品ＣＤ = S.商品ＣＤ
    WHERE
        A.企業ＣＤ = @企業ＣＤ
        AND A.部署ＣＤ = @部署ＣＤ
        AND ISNULL(A.無効フラグ, 0) = 0
    ORDER BY
        CASE WHEN A.商品グループＣＤ IS NULL THEN 1 ELSE 0 END
        ,A.商品グループＣＤ
        ,S.弁当区分
        ,A.商品ＣＤ
    ) AS B
UNION
SELECT
    1 AS その他区分
    ,-1 AS 商品グループＣＤ
    ,-1 AS ソート用商品ＣＤ
    ,'XXX' AS 商品ＣＤ
    ,'他' AS 商品名略称
ORDER BY
    その他区分
    ,商品グループＣＤ
    ,ソート用商品ＣＤ
    ,商品ＣＤ



--- 注文に存在する商品名取得
SELECT
    0 AS その他区分
    ,CASE WHEN B.商品グループＣＤ IS NULL THEN 1 ELSE 0 END AS 有効主要商品優先
    ,B.商品グループＣＤ
    ,B.商品ＣＤ AS ソート用商品ＣＤ
    ,CONVERT(varchar, B.商品ＣＤ) AS 商品ＣＤ
    ,B.商品名略称
FROM
    -- 出力できる商品の最大列数は16（その他を入れて17）
    (SELECT DISTINCT TOP(16)
        CASE WHEN A.商品グループＣＤ IS NULL THEN 1 ELSE 0 END AS 有効主要商品優先
        ,A.商品グループＣＤ
        ,S.弁当区分
        ,M.商品ＣＤ
        ,S.商品名略称
    FROM
        注文ヘッダ AS H
        INNER JOIN 注文明細 AS M
            ON  H.企業ＣＤ   = M.企業ＣＤ
            AND H.注文区分   = M.注文区分
            AND H.喫食日     = M.喫食日
            AND H.得意先ＣＤ = M.得意先ＣＤ
            AND H.部署ＣＤ   = M.部署ＣＤ
        LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = M.企業ＣＤ AND S.商品ＣＤ = M.商品ＣＤ
        LEFT JOIN 主要商品リストマスタ AS A ON A.企業ＣＤ = S.企業ＣＤ AND A.部署ＣＤ = @部署ＣＤ AND A.商品ＣＤ = S.商品ＣＤ AND ISNULL(A.無効フラグ, 0) = 0
    WHERE
        H.企業ＣＤ = @企業ＣＤ
        AND H.喫食日 = @注文日
        AND H.部署ＣＤ = @部署ＣＤ
        AND H.コースＣＤ = @コースＣＤ
        AND M.注文区分 = @注文区分注文
    ORDER BY
        CASE WHEN A.商品グループＣＤ IS NULL THEN 1 ELSE 0 END
        ,A.商品グループＣＤ
        ,S.弁当区分
        ,M.商品ＣＤ
    ) AS B
UNION
SELECT
    1 AS その他区分
    ,0 AS 有効主要商品優先
    ,-1 AS 商品グループＣＤ
    ,-1 AS ソート用商品ＣＤ
    ,'XXX' AS 商品ＣＤ
    ,'他' AS 商品名略称
ORDER BY
    その他区分
    ,有効主要商品優先
    ,商品グループＣＤ
    ,ソート用商品ＣＤ
    ,商品ＣＤ



-- 注文元の注文を配送先の注文に集約する対応として、注文元の得意先CDは配送先の得意先CDにすり替えて取得したい。
-- ただし、注文元のコースに配送先が属していない場合は、注文元はどこにも集約させずにそのまま取得するものとする。

-- ┌─┬─────────┬───────┬─────┬──────────────┐
-- │No│コースの得意先ＣＤ│配送得意先ＣＤ│集約先ＣＤ│最終的に取得したい得意先ＣＤ│
-- ├─┼─────────┼───────┼─────┼──────────────┤
-- │ 1│             43101│              │     43101│                       43101│
-- ├─┼─────────┼───────┼─────┼──────────────┤
-- │ 2│             43104│              │     43104│                       43104│
-- ├─┼─────────┼───────┼─────┼──────────────┤
-- │ 3│             43105│         43104│     43104│                       43104│
-- ├─┼─────────┼───────┼─────┼──────────────┤
-- │ 4│             43106│         43109│          │                       43106│
-- ├─┼─────────┼───────┼─────┼──────────────┤
-- │ 5│             43107│              │     43107│                       43107│
-- └─┴─────────┴───────┴─────┴──────────────┘
-- ・コースの得意先ＣＤが配送先を持たない（つまり注文元ではない）場合は、集約先ＣＤを採用する（1,2,5）
-- ・コースの得意先ＣＤが配送先を持つ（つまり注文元になる）場合は、集約先ＣＤを採用する（3）
-- ・コースの得意先ＣＤが配送先を持つ（つまり注文元になる）が、配送先がコースに属していない（集約先ＣＤがnullになっている）場合は、コースの得意先ＣＤを採用する（4）


--- 配送集計表本体
WITH 対象コース得意先 AS (
-- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
SELECT
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
)

,駆動表 AS (
SELECT DISTINCT
    D.企業ＣＤ
    ,D.コースＣＤ
    ,D.コース得意先 AS コース得意先ＣＤ
    ,D.得意先ＣＤ
FROM
-- ①－１（指定されたコースにぶら下がる得意先(集約先)）
    (SELECT
        TH.企業ＣＤ
        ,TH.コースＣＤ
        ,TH.得意先ＣＤ AS コース得意先
        ,TH.配送得意先ＣＤ
        ,SY.集約先ＣＤ
        ,ISNULL(SY.集約先ＣＤ, TH.得意先ＣＤ) AS 得意先ＣＤ     -- 他SQLに提供する得意先ＣＤ
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
-- ①－２（指定されたコースを持つ注文の(ただしコーステーブルに無い)得意先）
UNION
SELECT
    CH.企業ＣＤ
    ,CH.コースＣＤ
    ,CH.得意先ＣＤ AS コース得意先ＣＤ
    ,CH.得意先ＣＤ
FROM
    注文ヘッダ AS CH
    LEFT JOIN 対象コース得意先 AS W
        ON CH.企業ＣＤ = W.企業ＣＤ
        AND CH.得意先ＣＤ = W.得意先ＣＤ
WHERE
    CH.企業ＣＤ = @企業ＣＤ
    AND CH.コースＣＤ = @コースＣＤ
    AND CH.喫食日 = @注文日
    AND CH.部署ＣＤ = @部署ＣＤ
    AND W.得意先ＣＤ IS NULL
)


-- 配送集計表
SELECT DISTINCT
    B.部署名,
    B.会社名称,
    B.電話番号,
    B.ＦＡＸ,
    '2022/01/28(金)' AS 配送日,
    CO.コースＣＤ,
    CO.コース名,
    D.担当者名,
    CO.コース順,
    T.得意先ＣＤ,
    T.得意先名,
    -- T.得意先名略称 AS 得意先名,
    T.電話番号1 AS 得意先電話番号,
    CASE T.締区分
        WHEN 0 THEN '日'
        WHEN 1 THEN '週'
        WHEN 2 THEN CASE T.月締日 WHEN 99 THEN '末' ELSE CONVERT(varchar, T.月締日) END
    END AS 締日,
    VK.売掛現金区分名 AS 売掛現金区分,
    A.商品ＣＤ,
    A.商品数量,
    CK.注文計 AS 合計金額,
    CASE T.売掛現金区分 WHEN 1 THEN 0 ELSE CK.注文計 END AS 現金計
FROM
-- ①駆動表
    駆動表 AS C
    LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ
-- ②駆動表に紐付く注文（注文元の数量は配送先に集約する）
    LEFT JOIN
        (SELECT
            H.コースＣＤ
            ,C.得意先ＣＤ
            ,ISNULL(CONVERT(varchar, S.商品ＣＤ), 'XXX') AS 商品ＣＤ
            ,SUM(M.商品数量) AS 商品数量
        FROM
            駆動表 AS C
            INNER JOIN 注文ヘッダ AS H
                ON C.企業ＣＤ = H.企業ＣＤ
                AND C.コースＣＤ = H.コースＣＤ
                AND C.コース得意先ＣＤ = H.得意先ＣＤ   -- 注文元の数量は配送先として計上する
            INNER JOIN 注文明細 AS M
                ON  H.企業ＣＤ   = M.企業ＣＤ
                AND H.注文区分   = M.注文区分
                AND H.喫食日     = M.喫食日
                AND H.得意先ＣＤ = M.得意先ＣＤ
                AND H.部署ＣＤ   = M.部署ＣＤ
            LEFT JOIN


                ----- 主要商品マスタの商品を表示する場合はここから -----
                -- 出力できる商品の最大列数は16（その他(XXX)を入れて17）
                (SELECT TOP(16)
                    SS.企業ＣＤ
                    ,SS.商品ＣＤ
                    ,SS.部署ＣＤ
                FROM
                    主要商品リストマスタ AS SS
                    LEFT JOIN 商品マスタ AS SM ON SM.企業ＣＤ = SS.企業ＣＤ AND SM.商品ＣＤ = SS.商品ＣＤ
                WHERE
                    SS.企業ＣＤ = @企業ＣＤ
                    AND SS.部署ＣＤ = @部署ＣＤ
                    AND ISNULL(SS.無効フラグ, 0) = 0
                ORDER BY
                    CASE WHEN SS.商品グループＣＤ IS NULL THEN 1 ELSE 0 END
                    ,SS.商品グループＣＤ
                    ,SM.弁当区分
                    ,SS.商品ＣＤ
                ) AS S
                ON S.企業ＣＤ = M.企業ＣＤ
                AND S.商品ＣＤ = M.商品ＣＤ
                AND S.部署ＣＤ = M.部署ＣＤ
                ----- ここまで -----


                ----- 注文に存在する商品を表示する場合はここから -----
                -- 出力できる商品の最大列数は16（その他を入れて17）
                (SELECT DISTINCT TOP(16)
                    H.企業ＣＤ
                    ,H.部署ＣＤ
                    ,CASE WHEN A.商品グループＣＤ IS NULL THEN 1 ELSE 0 END AS 有効主要商品優先
                    ,A.商品グループＣＤ
                    ,S.弁当区分
                    ,M.商品ＣＤ
                FROM
                    注文ヘッダ AS H
                    INNER JOIN 注文明細 AS M
                        ON  H.企業ＣＤ   = M.企業ＣＤ
                        AND H.注文区分   = M.注文区分
                        AND H.喫食日     = M.喫食日
                        AND H.得意先ＣＤ = M.得意先ＣＤ
                        AND H.部署ＣＤ   = M.部署ＣＤ
                    LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = M.企業ＣＤ AND S.商品ＣＤ = M.商品ＣＤ
                    LEFT JOIN 主要商品リストマスタ AS A ON A.企業ＣＤ = S.企業ＣＤ AND A.部署ＣＤ = @部署ＣＤ AND A.商品ＣＤ = S.商品ＣＤ AND ISNULL(A.無効フラグ, 0) = 0
                WHERE
                    H.企業ＣＤ = @企業ＣＤ
                    AND H.喫食日 = @注文日
                    AND H.部署ＣＤ = @部署ＣＤ
                    AND H.コースＣＤ = @コースＣＤ
                    AND M.注文区分 = @注文区分注文
                ORDER BY
                    CASE WHEN A.商品グループＣＤ IS NULL THEN 1 ELSE 0 END
                    ,A.商品グループＣＤ
                    ,S.弁当区分
                    ,M.商品ＣＤ
                ) AS S
                ON S.企業ＣＤ = M.企業ＣＤ
                AND S.商品ＣＤ = M.商品ＣＤ
                AND S.部署ＣＤ = M.部署ＣＤ
                ----- ここまで -----



        WHERE
            H.企業ＣＤ = @企業ＣＤ
            AND H.喫食日 = @注文日
            AND H.部署ＣＤ = @部署ＣＤ
            AND M.注文区分 = @注文区分注文
        GROUP BY
            H.コースＣＤ
            ,C.得意先ＣＤ
            ,S.商品ＣＤ
        ) AS A ON A.得意先ＣＤ = C.得意先ＣＤ AND A.コースＣＤ = C.コースＣＤ
-- ③注文計
    LEFT JOIN
        (SELECT
            H.コースＣＤ
            ,C.得意先ＣＤ
            ,SUM(M.商品金額合計税込) AS 注文計
        FROM
            駆動表 AS C
            INNER JOIN 注文ヘッダ AS H
                ON C.企業ＣＤ = H.企業ＣＤ
                AND C.コースＣＤ = H.コースＣＤ
                AND C.コース得意先ＣＤ = H.得意先ＣＤ   -- 注文元の数量は配送先として計上する
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
            AND M.注文区分 = @注文区分注文
        GROUP BY
            H.コースＣＤ
            ,C.得意先ＣＤ
        ) AS CK ON CK.得意先ＣＤ = C.得意先ＣＤ AND CK.コースＣＤ = C.コースＣＤ
-- ④コース順（①で取得していないのはなぜ？①－２からはコース順が取得できないので代わりにNULLにするとUNIONしたときに複数行できてしまうため）
    LEFT JOIN
        (SELECT
            CT.企業ＣＤ
            ,CM.コース名
            ,CM.コースＣＤ
            ,CM.担当者ＣＤ
            ,CT.得意先ＣＤ
            ,CT.コース順
        FROM
            コーステーブル AS CT
            INNER JOIN コースマスタ AS CM
                ON CM.企業ＣＤ = CT.企業ＣＤ
                AND CM.コースＣＤ = CT.コースＣＤ
                AND ISNULL(CM.無効フラグ, 0) = 0
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
            ,RM.コース名
            ,RM.コースＣＤ
            ,RM.担当者ＣＤ
            ,RT.得意先ＣＤ
            ,RT.コース順
        FROM
            臨時コーステーブル AS RT
            INNER JOIN 臨時コースマスタ AS RM
                ON RM.企業ＣＤ = RT.企業ＣＤ
                AND RM.コースＣＤ = RT.コースＣＤ
                AND RM.実施日 = RT.実施日
                AND ISNULL(RM.無効フラグ, 0) = 0
        WHERE
            RT.企業ＣＤ = @企業ＣＤ
            AND RT.コースＣＤ = @コースＣＤ
            AND ISNULL(RT.無効フラグ, 0) = 0
            AND RT.実施日 = @注文日
        ) AS CO ON C.企業ＣＤ = CO.企業ＣＤ AND C.得意先ＣＤ = CO.得意先ＣＤ
    LEFT JOIN 担当者マスタ AS D ON D.企業ＣＤ = CO.企業ＣＤ AND D.担当者ＣＤ = CO.担当者ＣＤ
    LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = C.企業ＣＤ AND B.部署ＣＤ = @部署ＣＤ
    LEFT JOIN V_売掛現金区分 AS VK ON VK.売掛現金区分CD = ISNULL(T.売掛現金区分, @売掛現金区分現金)     -- デフォルトは現金
ORDER BY
    CO.コース順,
    T.得意先ＣＤ


@企業ＣＤ = 40000
@部署ＣＤ = 1
@コースＣＤ = 800
@注文日 = '2022/01/28'
@注文区分注文 = 0
@売掛現金区分現金 = 0
