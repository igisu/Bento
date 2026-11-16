-- 合算請求書からの請求書印刷対象
SELECT
    G.請求番号
    ,G.請求日
    ,G.請求先ＣＤ
    ,G.請求先名
    ,G.今回請求額
    ,ISNULL(B.請求書タイプ区分, 1) AS 請求書タイプ区分
    ,C.コースＣＤ
    ,C.コース順
FROM
    請求合計 AS G
    -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
    INNER JOIN 得意先マスタ AS S ON S.企業ＣＤ = G.企業ＣＤ AND S.得意先ＣＤ = G.請求先ＣＤ
    INNER JOIN 得意先マスタ AS J ON J.企業ＣＤ = S.企業ＣＤ AND J.得意先ＣＤ = ISNULL(S.受注得意先ＣＤ, S.得意先ＣＤ)
    INNER JOIN 部署マスタ AS B ON
        B.企業ＣＤ = G.企業ＣＤ
        AND B.部署ＣＤ = G.部署ＣＤ

    -- ヒットした得意先が持つコースのうち最小値を取得する
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
                WHERE
                    ISNULL(T.無効フラグ, 0) = 0
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
            AND G.部署ＣＤ = C.部署ＣＤ     -- 請求の部署と一致するものに限る
            AND J.得意先ＣＤ = C.得意先ＣＤ
WHERE
    G.企業ＣＤ = @企業ＣＤ
    AND G.請求番号 = @請求番号
