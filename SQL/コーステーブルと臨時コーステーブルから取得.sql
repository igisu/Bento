-- コーステーブルと臨時コーステーブルから取得

-- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
SELECT
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
            企業ＣＤ = 1
            AND ISNULL(無効フラグ, 0) = 0
            AND コースＣＤ = 6
            AND 実施日 = '2016/05/12'
--            AND 実施日 = '2016/05/13'
        ) AS RM
        ON RM.コースＣＤ = C.コースＣＤ
WHERE
    CT.企業ＣＤ = 1
    AND CT.コースＣＤ = 6
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
    RT.企業ＣＤ = 1
    AND RT.コースＣＤ = 6
    AND ISNULL(RT.無効フラグ, 0) = 0
    AND RT.実施日 = '2016/05/12'
--    AND RT.実施日 = '2016/05/13'
