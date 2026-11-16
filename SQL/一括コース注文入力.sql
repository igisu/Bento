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
    ,S.商品単価税込
    ,S.商品単価税抜
    ,S.軽減税率フラグ
    ,'予測' AS 予測注文
FROM
    主要商品リストマスタ AS A
    LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND A.商品ＣＤ = S.商品ＣＤ
WHERE
    A.企業ＣＤ = @企業ＣＤ
    AND A.部署ＣＤ = @部署ＣＤ
    AND ISNULL(S.無効フラグ, 0) = 0
UNION
-- ②注文レコードから(予測用)
SELECT
    M.商品ＣＤ AS ソート用商品ＣＤ
    ,CONVERT(varchar, M.商品ＣＤ) + '_0' AS 商品ＣＤ
    ,S.商品名略称
    ,S.商品単価税込
    ,S.商品単価税抜
    ,S.軽減税率フラグ
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
    H.企業ＣＤ = @企業ＣＤ
    AND H.喫食日 = @注文日
    AND H.部署ＣＤ = @部署ＣＤ
    AND H.コースＣＤ = @コースＣＤ
UNION
-- ③主要商品リストマスタから(注文用)
SELECT
    A.商品ＣＤ AS ソート用商品ＣＤ
    ,CONVERT(varchar, A.商品ＣＤ) + '_1' AS 商品ＣＤ
    ,S.商品名略称
    ,S.商品単価税込
    ,S.商品単価税抜
    ,S.軽減税率フラグ
    ,'注文' AS 予測注文
FROM
    主要商品リストマスタ AS A
    LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND A.商品ＣＤ = S.商品ＣＤ
WHERE
    A.企業ＣＤ = @企業ＣＤ
    AND A.部署ＣＤ = @部署ＣＤ
    AND ISNULL(S.無効フラグ, 0) = 0
UNION
-- ④注文レコードから(注文用)
SELECT
    M.商品ＣＤ AS ソート用商品ＣＤ
    ,CONVERT(varchar, M.商品ＣＤ) + '_1' AS 商品ＣＤ
    ,S.商品名略称
    ,S.商品単価税込
    ,S.商品単価税抜
    ,S.軽減税率フラグ
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
    H.企業ＣＤ = @企業ＣＤ
    AND H.喫食日 = @注文日
    AND H.部署ＣＤ = @部署ＣＤ
    AND H.コースＣＤ = @コースＣＤ
ORDER BY
    ソート用商品ＣＤ
    ,商品ＣＤ



-- 一括コース注文入力
SELECT
    C.得意先ＣＤ
    ,T.得意先名
    ,ISNULL(T.税区分, @税区分内税) AS 税区分
    ,ISNULL(T.丸め処理区分, @端数区分切り捨て) AS 丸め処理区分
    ,VK.売掛現金区分名 AS 種類
    ,ISNULL(CK.注文計, 0) AS 注文計
    ,ISNULL(H.備考1, T.備考1) AS 備考
    ,A.商品ＣＤ
    ,A.商品数量
    ,CASE WHEN CK.注文計 IS NULL THEN 0 ELSE 1 END AS 注文有無フラグ
--    ,ISNULL(A.コースＣＤ, C.コースＣＤ) AS コースＣＤ                                     -- 駆動表に紐付く予測注文レコードが無い場合は取得した(=検索条件の)コース
     ,CASE WHEN A.商品数量 IS NULL THEN C.コースＣＤ ELSE A.コースＣＤ END AS コースＣＤ    -- 駆動表に紐付く予測注文レコードのコースＣＤがNULLの場合はそのまま取得したい
    ,T.休日設定
    ,CO.コース順
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
    ) AS C
    INNER JOIN 得意先マスタ AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ
    LEFT JOIN V_売掛現金区分 AS VK ON VK.売掛現金区分CD = ISNULL(T.売掛現金区分, 0)     -- デフォルトは現金
-- ②駆動表に紐付く予測と注文
-- コース注文入力とは異なり、コースを無視して(コースの結合はせずに)注文を取得する。ただし、検索条件のコースかどうかを示すためにコースCDは取得する
    LEFT JOIN
        -- ②－１ 予測を取得
        (SELECT
            ISNULL(HC.コースＣＤ, @コースＣＤ) AS コースＣＤ        -- コースＣＤは注文のものを優先し、予測から取得する場合は検索条件のコースとする
            ,M.得意先ＣＤ
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
            AND HY.喫食日 = @注文日
            AND HY.部署ＣＤ = @部署ＣＤ
            AND HY.注文区分 = @注文区分予測
        -- ②－２ 注文を取得
        UNION
        SELECT
            H.コースＣＤ
            ,M.得意先ＣＤ
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
            AND H.喫食日 = @注文日
            AND H.部署ＣＤ = @部署ＣＤ
            AND H.注文区分 = @注文区分注文
        ) AS A ON A.得意先ＣＤ = C.得意先ＣＤ
-- ③備考取得用（コース注文入力とは異なり、コースを無視して(コースの結合はせずに)注文を取得する）
    LEFT JOIN
        (SELECT
            得意先ＣＤ
            ,備考1
        FROM
            注文ヘッダ
        WHERE
            企業ＣＤ = @企業ＣＤ
            AND 注文区分 = @注文区分注文
            AND 喫食日 = @注文日
            AND 部署ＣＤ = @部署ＣＤ
        ) AS H
        ON H.得意先ＣＤ = C.得意先ＣＤ
-- ④注文計（コース注文入力とは異なり、コースを無視して(コースの結合はせずに)注文を取得する）
    LEFT JOIN
        (SELECT
            M.得意先ＣＤ
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
            M.得意先ＣＤ
        ) AS CK ON CK.得意先ＣＤ = C.得意先ＣＤ
-- ⑤コース順（①で取得していないのはなぜ？①－２からはコース順が取得できないので代わりにNULLにするとUNIONしたときに複数行できてしまうため）
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
-- 合計行
UNION
SELECT
    NULL AS 得意先ＣＤ
    ,'合計' AS 得意先名
    ,NULL AS 税区分
    ,NULL AS 丸め処理区分
    ,NULL AS 種類
    ,NULL AS 注文計
    ,NULL AS 備考
    ,NULL AS 商品ＣＤ
    ,NULL AS 商品数量
    ,NULL AS 注文有無フラグ
    ,NULL AS コースＣＤ
    ,NULL AS 休日設定
    ,999999999 AS コース順
ORDER BY
    CO.コース順
    ,C.得意先ＣＤ



@企業ＣＤ = 40000
@部署ＣＤ = 1
@コースＣＤ = 139
@注文日 = '2018/04/25'
@注文区分注文 = 0
@注文区分予測 = 1
@税区分内税 = 1
@端数区分切り捨て = 2
