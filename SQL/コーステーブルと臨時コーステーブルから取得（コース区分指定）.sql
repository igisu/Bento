-- コーステーブルと臨時コーステーブルから取得（コース区分指定）

-- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
SELECT
    1 AS 区分
    ,CT.コースＣＤ
    ,CT.得意先ＣＤ
FROM
    コーステーブル AS CT
    INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0
    LEFT JOIN
        (SELECT
            コースＣＤ
        FROM
            INNER JOIN 臨時コースマスタ
        WHERE
            企業ＣＤ = @企業ＣＤ
            AND 実施日 = @実施日
            AND 得意先ＣＤ = @得意先ＣＤ
            AND ISNULL(無効フラグ, 0) = 0
            AND コース区分 = @コース区分
        ) AS R
        ON R.コースＣＤ = CT.コースＣＤ
WHERE
    CT.企業ＣＤ = @企業ＣＤ
    AND ISNULL(CT.無効フラグ, 0) = 0
    AND CT.得意先ＣＤ = @得意先ＣＤ
    AND CM.コース区分 = @コース区分
    AND R.コースＣＤ IS NULL
-- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
UNION
SELECT
    0 AS 区分
    ,RT.コースＣＤ
    ,RT.得意先ＣＤ
FROM
    臨時コーステーブル AS RT
    INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0
WHERE
    RT.企業ＣＤ = @企業ＣＤ
    AND RT.実施日 = @実施日
    AND RT.得意先ＣＤ = @得意先ＣＤ
    AND ISNULL(RT.無効フラグ, 0) = 0
    AND RM.コース区分 = @コース区分
