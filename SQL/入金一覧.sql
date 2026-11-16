-- 入金一覧
SELECT
    A.企業ＣＤ
    ,A.入金番号
    ,A.入金入力区分
    ,A.入金日
    ,A.部署ＣＤ
    ,B.部署名
    ,C.コースＣＤ
    ,C.コース名
    ,A.得意先ＣＤ
    ,A.得意先名
    ,A.依頼人ＣＤ
    ,A.振込依頼人名
    ,A.入金方法
    ,VN.入金方法名
    ,ISNULL(A.入金額, 0) AS 入金額
    ,A.入金摘要区分
    ,CONVERT(varchar, A.入金摘要区分) + '月分' AS 摘要
    ,A.対応請求番号
    ,A.備考
    ,A.作成担当者ＣＤ
    ,A1.担当者名 AS 作成担当者名
    ,A.作成日
    ,A.修正担当者ＣＤ
    ,A2.担当者名 AS 修正担当者名
    ,A.修正日
FROM
    入金明細 AS A
    -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
    INNER JOIN 得意先マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND S.得意先ＣＤ = A.得意先ＣＤ
    INNER JOIN 得意先マスタ AS J ON J.企業ＣＤ = S.企業ＣＤ AND J.得意先ＣＤ = ISNULL(S.受注得意先ＣＤ, S.得意先ＣＤ)

    -- コースによる絞り込み用
    -- 同じ得意先に複数のコースが設定されている場合、そのいずれのコースを条件に指定しても、そのコースにぶら下がる得意先がヒットしないといけない。
    -- 検索条件にコースが指定された場合のみ、INNER JOIN以下を追加すること
    INNER JOIN
        (SELECT DISTINCT
            CM.企業ＣＤ
            ,CM.部署ＣＤ
            ,CM.コース名
            ,CM.コースＣＤ
            ,CT.得意先ＣＤ
            ,CT.コース順
        FROM
            コースマスタ AS CM
            INNER JOIN コーステーブル AS CT ON
                CM.企業ＣＤ = CT.企業ＣＤ
                AND CM.コースＣＤ =  CT.コースＣＤ
        WHERE
            CT.コースＣＤ = @コースＣＤ
        ) AS C
        ON J.企業ＣＤ = C.企業ＣＤ
        AND A.部署ＣＤ = C.部署ＣＤ     -- 入金明細の部署と一致するものに限る
        AND J.得意先ＣＤ = C.得意先ＣＤ

    -- 検索条件にコースが指定されていない場合は、ヒットした得意先が持つコースのうち最小値を取得する
    LEFT JOIN
        (SELECT
            CM.企業ＣＤ
            ,CM.部署ＣＤ
            ,CM.コース名
            ,CT1.コースＣＤ
            ,CT1.得意先ＣＤ
            ,CT1.コース順
        FROM
            コーステーブル AS CT1
            INNER JOIN
                (SELECT
                    M.企業ＣＤ
                    ,M.部署ＣＤ
                    ,T.得意先ＣＤ
                    ,MIN(T.コースＣＤ) AS コースＣＤ
                FROM
                    コースマスタ AS M
                    LEFT JOIN コーステーブル AS T ON
                        T.企業ＣＤ = M.企業ＣＤ
                        AND T.コースＣＤ = M.コースＣＤ
                GROUP BY
                    M.企業ＣＤ
                    ,M.部署ＣＤ
                    ,T.得意先ＣＤ
                ) AS CT2
                    ON CT2.企業ＣＤ = CT1.企業ＣＤ
                    AND CT2.得意先ＣＤ = CT1.得意先ＣＤ
                    AND CT2.コースＣＤ = CT1.コースＣＤ
            INNER JOIN コースマスタ AS CM
                ON CM.企業ＣＤ = CT1.企業ＣＤ
                AND CM.コースＣＤ =  CT1.コースＣＤ
        ) AS C
        ON J.企業ＣＤ = C.企業ＣＤ
        AND A.部署ＣＤ = C.部署ＣＤ     -- 入金明細の部署と一致するものに限る
        AND J.得意先ＣＤ = C.得意先ＣＤ

    LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = A.企業ＣＤ AND B.部署ＣＤ = A.部署ＣＤ
    LEFT JOIN V_入金方法 AS VN ON VN.入金方法CD = A.入金方法
    LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = A.企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ
    LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = A.企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ
WHERE
    A.企業ＣＤ = @企業ＣＤ
    AND B.支店ＣＤ = @支店ＣＤ
    AND A.部署ＣＤ = @部署ＣＤ
    AND A.入金日 >= @入金日From
    AND A.入金日 <= @入金日To
    AND A.入金方法 = @入金方法
ORDER BY
    A.入金日