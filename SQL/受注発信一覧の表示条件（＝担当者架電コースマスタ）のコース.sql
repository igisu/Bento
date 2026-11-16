-- 受注発信一覧の表示条件（＝担当者架電コースマスタ）のコース
-- ログイン時に自動で開くコース注文入力のコース
 SELECT
     K.コースＣＤ
 FROM
     担当者架電コースマスタ AS K
     INNER JOIN
         (SELECT
             CM.コースＣＤ
         FROM
             コースマスタ AS CM
             LEFT JOIN
                 (SELECT
                     M.コースＣＤ
                 FROM
                     臨時コースマスタ AS M
                 WHERE
                     M.企業ＣＤ = 40001
                     AND M.実施日 = '2016/08/25'
                     AND ISNULL(M.無効フラグ, 0) = 0
                     AND M.コース区分 = 1
                 ) AS RM
                 ON RM.コースＣＤ = CM.コースＣＤ
         WHERE
             CM.企業ＣＤ = 40001
             AND ISNULL(CM.無効フラグ, 0) = 0
             AND CM.コース区分 = 1
             AND RM.コースＣＤ IS NULL
         UNION
         SELECT
             RM.コースＣＤ
         FROM
             臨時コースマスタ AS RM
         WHERE
             RM.企業ＣＤ = 40001
             AND ISNULL(RM.無効フラグ, 0) = 0
             AND RM.実施日 = '2016/08/25'
             AND RM.コース区分 = 1
         ) AS CT
         ON CT.コースＣＤ = K.コースＣＤ
 WHERE
     K.企業ＣＤ = 40001
     AND K.担当者ＣＤ = 55
 ORDER BY
     K.コースＣＤ