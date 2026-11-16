-- 一括注文入力（得意先単価マスタ取得）
SELECT
    TT.企業ＣＤ
    ,TT.得意先ＣＤ
    ,TT.商品ＣＤ
    ,TT.商品単価税込
    ,TT.商品単価税抜
    ,TT.無効フラグ
    ,TT.作成担当者ＣＤ
    ,TT.作成日
    ,TT.修正担当者ＣＤ
    ,TT.修正日
FROM
-- ①駆動表
-- ①－１（指定されたコースにぶら下がる得意先）
    -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
    (SELECT
        CT.企業ＣＤ
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
        ,得意先ＣＤ
    FROM
        注文ヘッダ
    WHERE
        企業ＣＤ = @企業ＣＤ
        AND コースＣＤ = @コースＣＤ
        AND 喫食日 = @注文日
        AND 部署ＣＤ = @部署ＣＤ
    ) AS C
-- ②駆動表に紐付く得意先単価マスタ
    INNER JOIN 得意先単価マスタ AS TT ON TT.企業ＣＤ = C.企業ＣＤ AND TT.得意先ＣＤ = C.得意先ＣＤ AND ISNULL(TT.無効フラグ, 0) = 0


@企業ＣＤ = 40000
@部署ＣＤ = 1
@コースＣＤ = 139
@注文日 = '2018/04/10'
