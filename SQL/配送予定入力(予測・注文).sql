-- 配送予定入力(予測・注文)
SELECT
    C.得意先ＣＤ
    ,T.得意先名
--    ,ISNULL(A.コースＣＤ, C.コースＣＤ) AS コースＣＤ                                     -- 駆動表に紐付く予測注文レコードが無い場合は取得した(=検索条件の)コース
    ,CASE WHEN A.商品ＣＤ IS NULL THEN C.コースＣＤ ELSE A.コースＣＤ END AS コースＣＤ    -- 駆動表に紐付く予測注文レコードのコースＣＤがNULLの場合はそのまま取得したい
    ,A.商品ＣＤ
    ,A.商品注文数量
    ,A.商品予測数量
    ,CASE WHEN ISNULL(A.商品注文数量, -1) >= 0 THEN 1 ELSE 0 END AS 注文フラグ
    ,CASE WHEN ISNULL(A.商品注文数量, -1) >= 0 THEN A.商品注文数量 ELSE A.商品予測数量 END AS 商品数量
FROM
-- ①駆動表（指定されたコースにぶら下がる得意先）
    -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
    (SELECT
        CT.企業ＣＤ
        ,CT.コースＣＤ
        ,CT.得意先ＣＤ
        ,CT.コース順
        ,CT.無効フラグ
        ,CT.作成担当者ＣＤ
        ,CT.作成日
        ,CT.修正担当者ＣＤ
        ,CT.修正日
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
        ,RT.コース順
        ,RT.無効フラグ
        ,RT.作成担当者ＣＤ
        ,RT.作成日
        ,RT.修正担当者ＣＤ
        ,RT.修正日
    FROM
        臨時コーステーブル AS RT
        INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0
    WHERE
        RT.企業ＣＤ = @企業ＣＤ
        AND RT.コースＣＤ = @コースＣＤ
        AND ISNULL(RT.無効フラグ, 0) = 0
        AND RT.実施日 = @注文日
    ) AS C
    LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = @企業ＣＤ AND C.得意先ＣＤ = T.得意先ＣＤ
-- ②駆動表に紐付く予測と注文
-- コース注文入力とは異なり、コースを無視して(コースの結合はせずに)注文を取得する。ただし、検索条件のコースかどうかを示すためにコースCDは取得する
    LEFT JOIN
       -- 予測と注文を１行にまとめる
       (SELECT
           B.コースＣＤ
           ,B.得意先ＣＤ
           ,B.商品ＣＤ
           ,SUM(CASE WHEN B.注文区分 = @注文区分注文 THEN B.商品数量 ELSE null END) AS 商品注文数量
           ,SUM(CASE WHEN B.注文区分 = @注文区分予測 THEN B.商品数量 ELSE null END) AS 商品予測数量
        FROM
            -- ②－１ 予測を取得
            (SELECT
                H.コースＣＤ
                ,H.注文区分
                ,M.得意先ＣＤ
                ,M.商品ＣＤ
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
                AND H.喫食日   = @注文日
                AND H.部署ＣＤ = @部署ＣＤ
                AND H.注文区分 = @注文区分予測      -- 予測
                AND H.コースＣＤ IS NOT NULL        -- コースがある
            -- ②－２ 注文を取得
            UNION
            SELECT
                ISNULL(HY.コースＣＤ, @コースＣＤ) AS コースＣＤ   -- コースＣＤは予測のものを優先し、注文から取得する場合は検索条件のコースとする
                ,HC.注文区分
                ,M.得意先ＣＤ
                ,M.商品ＣＤ
                ,M.商品数量
            FROM
                注文ヘッダ AS HC
                INNER JOIN 注文明細 AS M
                    ON  HC.企業ＣＤ   = M.企業ＣＤ
                    AND HC.注文区分   = M.注文区分
                    AND HC.喫食日     = M.喫食日
                    AND HC.得意先ＣＤ = M.得意先ＣＤ
                    AND HC.部署ＣＤ   = M.部署ＣＤ
                LEFT JOIN 注文ヘッダ AS HY
                    ON  HY.企業ＣＤ   = HC.企業ＣＤ
                    AND HY.喫食日     = HC.喫食日
                    AND HY.得意先ＣＤ = HC.得意先ＣＤ
                    AND HY.部署ＣＤ   = HC.部署ＣＤ
                    AND HY.注文区分   = @注文区分予測
            WHERE
                HC.企業ＣＤ = @企業ＣＤ
                AND HC.喫食日   = @注文日
                AND HC.部署ＣＤ = @部署ＣＤ
                AND HC.注文区分 = @注文区分注文     -- 注文
            ) AS B
        GROUP BY
           B.コースＣＤ
           ,B.得意先ＣＤ
           ,B.商品ＣＤ
        ) AS A ON A.得意先ＣＤ = C.得意先ＣＤ
ORDER BY
    C.コース順,
    A.商品ＣＤ


@企業ＣＤ = 40001
@注文日 = '2018/04/25'
@部署ＣＤ = 1
@コースＣＤ = 20
@注文区分注文 = 0
@注文区分予測 = 1
