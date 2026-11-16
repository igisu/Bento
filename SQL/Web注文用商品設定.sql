-- Web注文用商品設定
SELECT
    A.企業ＣＤ
    ,A.部署ＣＤ
    ,B.部署名
    ,A.商品グループＣＤ
    ,A.商品ＣＤ
    ,A.商品ＣＤ AS 商品ＣＤorg
    ,S1.商品名
    ,A.主要商品フラグ
    ,A.WEB注文不可フラグ
    ,A.WEB注文可能期限日数
    ,A.WEB注文可能曜日
    ,cast(CASE WHEN A.WEB注文可能曜日 IS NULL THEN 0 WHEN SUBSTRING(A.WEB注文可能曜日, 1, 1) = '1' THEN 1 ELSE 0 END as bit) AS WEB注文可能日祝
    ,cast(CASE WHEN A.WEB注文可能曜日 IS NULL THEN 0 WHEN SUBSTRING(A.WEB注文可能曜日, 2, 1) = '1' THEN 1 ELSE 0 END as bit) AS WEB注文可能月曜
    ,cast(CASE WHEN A.WEB注文可能曜日 IS NULL THEN 0 WHEN SUBSTRING(A.WEB注文可能曜日, 3, 1) = '1' THEN 1 ELSE 0 END as bit) AS WEB注文可能火曜
    ,cast(CASE WHEN A.WEB注文可能曜日 IS NULL THEN 0 WHEN SUBSTRING(A.WEB注文可能曜日, 4, 1) = '1' THEN 1 ELSE 0 END as bit) AS WEB注文可能水曜
    ,cast(CASE WHEN A.WEB注文可能曜日 IS NULL THEN 0 WHEN SUBSTRING(A.WEB注文可能曜日, 5, 1) = '1' THEN 1 ELSE 0 END as bit) AS WEB注文可能木曜
    ,cast(CASE WHEN A.WEB注文可能曜日 IS NULL THEN 0 WHEN SUBSTRING(A.WEB注文可能曜日, 6, 1) = '1' THEN 1 ELSE 0 END as bit) AS WEB注文可能金曜
    ,cast(CASE WHEN A.WEB注文可能曜日 IS NULL THEN 0 WHEN SUBSTRING(A.WEB注文可能曜日, 7, 1) = '1' THEN 1 ELSE 0 END as bit) AS WEB注文可能土曜
    ,A.WEB注文開始日
    ,A.WEB注文終了日
    ,A.無効フラグ
    ,A.作成担当者ＣＤ
    ,A1.担当者名 AS 作成担当者名
    ,A.作成日
    ,A.修正担当者ＣＤ
    ,A2.担当者名 AS 修正担当者名
    ,A.修正日
FROM
    主要商品リストマスタ AS A
    LEFT JOIN 商品マスタ AS S1 ON A.企業ＣＤ = S1.企業ＣＤ AND ISNULL(S1.無効フラグ, 0) = 0 AND A.商品ＣＤ = S1.商品ＣＤ
    LEFT JOIN 部署マスタ AS B ON A.企業ＣＤ = B.企業ＣＤ AND ISNULL(B.無効フラグ, 0) = 0 AND A.部署ＣＤ = B.部署ＣＤ
    LEFT JOIN 担当者マスタ AS A1 ON A.企業ＣＤ = A1.企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ
    LEFT JOIN 担当者マスタ AS A2 ON A.企業ＣＤ = A2.企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ
WHERE
    A.企業ＣＤ = @企業ＣＤ
and A.部署ＣＤ = @部署ＣＤ
order by A.部署ＣＤ, A.商品ＣＤ
