-- 請求書印刷対象
SELECT DISTINCT
    G.請求番号
    ,G.請求日
    ,G.請求先ＣＤ
    ,G.請求先名
    ,G.今回請求額
    -- ,ISNULL(G.買上金額, 0) + ISNULL(G.調整額, 0) + ISNULL(G.消費税, 0) AS 今回請求額     -- 「当月売上額のみを請求」がONの場合はこっち（売上のみ今回請求額）
    ,ISNULL(B.請求書タイプ区分, 1) AS 請求書タイプ区分
    ,C.コースＣＤ
    ,C.コース順
FROM
    -- ①今回請求のキーと、それに対する前回請求のキーを求める（請求一覧と同様に取得しているため）
    (SELECT
        G1.企業ＣＤ
        ,G1.請求番号 AS 今回請求番号
        ,G1.請求先ＣＤ
        ,MAX(P1.請求日) AS 前回請求日
    FROM
        請求合計 AS G1
        LEFT JOIN 請求合計 AS P1
            ON G1.企業ＣＤ = P1.企業ＣＤ
            AND G1.請求先ＣＤ = P1.請求先ＣＤ
            AND P1.請求日 < G1.請求日
    WHERE
        G1.企業ＣＤ = @企業ＣＤ
        AND G1.部署ＣＤ = @部署ＣＤ
        AND G1.請求日 = @請求日
        AND G1.請求先ＣＤ >= @請求先ＣＤFrom
        AND G1.請求先ＣＤ <= @請求先ＣＤTo
        AND G1.印刷日時 IS NULL     -- 印刷済みを含める場合は条件から外す
        AND G1.締区分 = @締区分
        AND G1.締区分 <> @締区分現金未集金          -- 現金未集金は除く
    GROUP BY
        G1.企業ＣＤ
        ,G1.請求番号
        ,G1.請求先ＣＤ
    ) AS P
    -- ②今回請求の詳細
    INNER JOIN 請求合計 G ON
        G.企業ＣＤ = P.企業ＣＤ
        AND G.請求番号 = P.今回請求番号
    -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
    INNER JOIN 得意先マスタ AS S ON S.企業ＣＤ = G.企業ＣＤ AND S.得意先ＣＤ = G.請求先ＣＤ
    INNER JOIN 得意先マスタ AS J ON J.企業ＣＤ = S.企業ＣＤ AND J.得意先ＣＤ = ISNULL(S.受注得意先ＣＤ, S.得意先ＣＤ)
    INNER JOIN 部署マスタ AS B ON
        B.企業ＣＤ = G.企業ＣＤ
        AND B.部署ＣＤ = G.部署ＣＤ

    -- コースによる絞り込み用
    -- 同じ得意先に複数のコースが設定されている場合、そのいずれのコースを条件に指定しても、そのコースにぶら下がる得意先がヒットしないといけない。
    -- 検索条件にコースが指定された場合のみ、INNER JOIN以下を追加すること
    INNER JOIN
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
                -- 指定した範囲にヒットするコースＣＤのうち、最小のものを取得
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
                    1 = 1
                    AND T.コースＣＤ >= @コースＣＤFrom
                    AND T.コースＣＤ <= @コースＣＤTo
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

WHERE G.今回請求額 != 0
AND C.コースＣＤ IS NULL
ORDER BY
    --G.請求先ＣＤ,C.コースＣＤ, C.コース順
    C.コースＣＤ, C.コース順, G.請求先ＣＤ
