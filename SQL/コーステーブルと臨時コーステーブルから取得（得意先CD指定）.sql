-- コーステーブルと臨時コーステーブルから取得（得意先CD指定）
-- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
SELECT
    1 AS 区分
    ,CT.コースＣＤ
    ,CT.得意先ＣＤ
    ,CM.部署ＣＤ
FROM
    コーステーブル AS CT
    INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0
    LEFT JOIN
        (SELECT
            T.コースＣＤ
        FROM
            臨時コーステーブル AS T
            INNER JOIN 臨時コースマスタ AS M ON M.企業ＣＤ = T.企業ＣＤ AND M.コースＣＤ = T.コースＣＤ AND T.実施日 = M.実施日 AND ISNULL(M.無効フラグ, 0) = 0
        WHERE
            T.企業ＣＤ = @企業ＣＤ
            AND T.実施日 = @実施日
            AND T.得意先ＣＤ = @得意先ＣＤ
            AND ISNULL(T.無効フラグ, 0) = 0
            AND M.コース区分 = @コース区分
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
    ,RM.部署ＣＤ
FROM
    臨時コーステーブル AS RT
    INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0
WHERE
    RT.企業ＣＤ = @企業ＣＤ
    AND RT.実施日 = @実施日
    AND RT.得意先ＣＤ = @得意先ＣＤ
    AND ISNULL(RT.無効フラグ, 0) = 0
    AND RM.コース区分 = @コース区分
ORDER BY
    区分
    ,コースＣＤ

@企業ＣＤ= 40000
@得意先ＣＤ = 30166
@実施日 = '2018/04/11'
@コース区分 = 3
