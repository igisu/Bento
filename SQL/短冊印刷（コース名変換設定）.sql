-- 短冊印刷（コース名変換設定）
SELECT
    TC.企業ＣＤ
    ,TC.コースＣＤ
    ,CM.コース名
    ,TC.短冊印刷コース名
    ,TC.作成担当者ＣＤ
    ,A1.担当者名 AS 作成担当者名
    ,TC.作成日
    ,TC.修正担当者ＣＤ
    ,A2.担当者名 AS 修正担当者名
    ,TC.修正日
FROM
    短冊コース名マスタ AS TC
    INNER JOIN コースマスタ AS CM ON TC.企業ＣＤ = CM.企業ＣＤ AND TC.コースＣＤ = CM.コースＣＤ
    LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = TC.企業ＣＤ AND TC.作成担当者ＣＤ = A1.担当者ＣＤ
    LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = TC.企業ＣＤ AND TC.修正担当者ＣＤ = A2.担当者ＣＤ
WHERE
    TC.企業ＣＤ = @企業ＣＤ
ORDER BY
    TC.コースＣＤ
