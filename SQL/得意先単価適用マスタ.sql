-- 得意先単価適用マスタ
SELECT
    A.企業ＣＤ
    ,A.得意先ＣＤ
    ,T.得意先名
    ,B.部署名
    ,T.丸め処理区分
    ,A.商品ＣＤ
    ,S.商品名
    ,ISNULL(S.軽減税率フラグ, 0) AS 軽減税率フラグ
    ,CASE WHEN ISNULL(S.軽減税率フラグ, 0) <> 0 THEN '対象' ELSE '' END AS 軽減税率
    ,S.商品単価税込 AS 標準単価
    ,A.適用開始日
    ,A.商品単価税抜
    ,A.商品単価税込
    ,A.固定数
    ,A.単価適用日時
    ,A.最終注文日時
    ,A.無効フラグ
    ,A.作成担当者ＣＤ
    ,A1.担当者名 AS 作成担当者名
    ,A.作成日
    ,A.修正担当者ＣＤ
    ,A2.担当者名 AS 修正担当者名
    ,A.修正日
FROM
    得意先単価適用マスタ AS A
    LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = A.企業ＣＤ AND T.得意先ＣＤ = A.得意先ＣＤ
    LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = T.企業ＣＤ AND B.部署ＣＤ = T.部署ＣＤ
    LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND S.商品ＣＤ = A.商品ＣＤ
    LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = A.企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ
    LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = A.企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ

    -- コースによる絞り込み用
    -- 同じ得意先に複数のコースが設定されている場合、そのいずれのコースを条件に指定しても、そのコースにぶら下がる得意先がヒットしないといけない。
    -- 検索条件にコースが指定された場合のみ、INNER JOIN以下を追加すること
    INNER JOIN
        (SELECT DISTINCT
            CM.企業ＣＤ
            ,CM.部署ＣＤ
            ,CM.コース名
            ,CT.コースＣＤ
            ,CT.得意先ＣＤ
            ,CT.コース順
        FROM
            コースマスタ AS CM
            INNER JOIN コーステーブル AS CT ON
                CM.企業ＣＤ = CT.企業ＣＤ
                AND CM.コースＣＤ =  CT.コースＣＤ
        WHERE
            1 = 1
            AND CT.コースＣＤ >= @コースＣＤFrom
            AND CT.コースＣＤ <= @コースＣＤTo
        ) AS C
        ON A.企業ＣＤ = C.企業ＣＤ
        AND A.得意先ＣＤ = C.得意先ＣＤ

WHERE
    A.企業ＣＤ = @企業ＣＤ
    AND T.部署ＣＤ = @部署ＣＤ
    AND A.得意先ＣＤ = @得意先ＣＤ
    AND A.商品ＣＤ = @商品ＣＤ
    AND A.適用開始日 >= @適用開始日From
    AND A.適用開始日 <= @適用開始日To
    AND ISNULL(A.固定数, 0) <> 0
