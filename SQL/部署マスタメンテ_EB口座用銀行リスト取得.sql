-- 部署マスタメンテ_EB口座用銀行リスト取得
SELECT
    -1 AS 銀行ＣＤ,
    '' AS 銀行名,
     0 AS 表示順
UNION
SELECT
    A.銀行ＣＤ,
    B.銀行名,
    1 AS 表示順
FROM
    (SELECT DISTINCT
        銀行ＣＤ
    FROM
        EBレコードフォーマット
    ) AS A
    INNER JOIN 金融機関名称 AS B ON A.銀行ＣＤ = B.銀行ＣＤ
ORDER BY
    表示順,
    銀行ＣＤ
