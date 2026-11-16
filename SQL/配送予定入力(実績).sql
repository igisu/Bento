-- 配送予定入力(実績)
SELECT
    C.得意先ＣＤ
    ,T.得意先名
    ,NULL AS コースＣＤ
    ,A.商品ＣＤ
    ,0 AS 注文フラグ
    ,A.商品数量
FROM
-- ①駆動表
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
-- ②駆動表に紐付く売上（売上のコースは見ない）
    LEFT JOIN (
        SELECT
            D.得意先ＣＤ,
            D.商品ＣＤ,
            SUM(D.商品数量) AS 商品数量
        FROM
            売上ヘッダ AS H
            INNER JOIN 売上明細 AS D
                ON  H.企業ＣＤ   = D.企業ＣＤ
                AND H.売上日     = D.売上日
                AND H.得意先ＣＤ = D.得意先ＣＤ
                AND H.部署ＣＤ   = D.部署ＣＤ
        WHERE
            H.企業ＣＤ = @企業ＣＤ AND
            H.売上日   = @売上日 AND
            H.部署ＣＤ = @部署ＣＤ AND
            ISNULL(D.現金補填フラグ, 0) = 0  -- このフラグが立っているのは厳密に言うと商品の売上ではないので除外する
        GROUP BY
            D.得意先ＣＤ,
            D.商品ＣＤ
        ) AS A ON A.得意先ＣＤ = C.得意先ＣＤ
WHERE
    C.企業ＣＤ = @企業ＣＤ
    AND ISNULL(C.無効フラグ, 0) = 0
    AND C.コースＣＤ = @コースＣＤ
ORDER BY
    C.コース順,
    A.商品ＣＤ


@企業ＣＤ = 40001
@注文日 = '2018/04/25'
@売上日 = '2018/04/25'
@部署ＣＤ = 1
@コースＣＤ = 20
