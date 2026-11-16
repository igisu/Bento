-- 売掛残高一覧
SELECT
    D.請求番号
    ,D.請求日
    ,D.締区分
    ,D.締め区分名
    ,D.部署ＣＤ
    ,D.部署名
    ,D.コースＣＤ
    ,D.コース名
    ,D.請求先ＣＤ
    ,D.請求先名
    ,D.今回請求額 AS 直近請求額
    ,D.請求後入金
    ,D.請求後売上
    ,(D.今回請求額 - D.請求後入金 + 請求後売上) AS 売掛残高
FROM
    (SELECT
        A.請求番号
        ,A.請求日
        ,A.締区分
        ,CASE A.締区分      -- V_締め区分に「未収金」が無いので結合できない
            WHEN 0 THEN '日締'
            WHEN 1 THEN '週締'
            WHEN 2 THEN '月締'
            WHEN 3 THEN '未収金'
            ELSE ''
        END AS 締め区分名
        ,A.部署ＣＤ
        ,B.部署名
        ,C.コースＣＤ
        ,C.コース名
        ,A.請求先ＣＤ
        ,A.請求先名
        ,A.今回請求額
        -- 入金明細から入金額の合計を取得する相関サブクエリ（スカラサブクエリ）
        ,(SELECT
            ISNULL(SUM(N.入金額), 0) AS 入金額
        FROM
            入金明細 AS N
        WHERE
            N.企業ＣＤ = A.企業ＣＤ
            AND N.部署ＣＤ = A.部署ＣＤ
            AND N.得意先ＣＤ = A.請求先ＣＤ
            AND N.入金日 > A.請求日
            AND N.入金日 <= @請求年月日To
        ) AS 請求後入金
        -- 売上明細から売上額の合計を取得する相関サブクエリ（スカラサブクエリ）
        -- 請求先にぶら下がる得意先の売上を取得する
        ,(SELECT
            ISNULL(SUM(U.商品金額合計税込), 0) AS 商品金額合計税込
        FROM
            売上明細 AS U
            INNER JOIN 得意先マスタ AS T11 ON U.企業ＣＤ = T11.企業ＣＤ AND U.得意先ＣＤ = T11.得意先ＣＤ
            INNER JOIN 得意先マスタ AS S11 ON S11.企業ＣＤ = T11.企業ＣＤ AND S11.得意先ＣＤ = ISNULL(T11.請求先ＣＤ, T11.得意先ＣＤ)
        WHERE
            S11.得意先ＣＤ = A.請求先ＣＤ
            -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる（売上の部署を無視する）
            AND ((ISNULL(S11.部署合算請求フラグ, 0) = 0 AND U.部署ＣＤ = A.部署ＣＤ) OR (ISNULL(S11.部署合算請求フラグ, 0) <> 0 AND U.部署ＣＤ = U.部署ＣＤ))
            AND U.部署ＣＤ = A.部署ＣＤ
            AND U.売上日 > A.請求日
            AND U.売上日 <= @請求年月日To
            AND U.売上区分 = @売上区分売掛
        ) AS 請求後売上
    FROM
        請求合計 AS A
        INNER JOIN
            (SELECT
                G.企業ＣＤ
                ,G.締区分
                ,G.部署ＣＤ
                ,G.請求先ＣＤ
                ,MAX(G.請求日) AS 請求日
            FROM
                請求合計 AS G
            WHERE
                G.企業ＣＤ = @企業ＣＤ
                AND G.部署ＣＤ = @部署ＣＤ
                AND G.締区分 = @締区分
                AND G.請求先ＣＤ = @請求先ＣＤ
                AND G.請求日 >= @請求年月日From
                AND G.請求日 <= @請求年月日To
            GROUP BY
                G.企業ＣＤ
                ,G.締区分
                ,G.部署ＣＤ
                ,G.請求先ＣＤ
            ) AS SG
            ON A.企業ＣＤ = SG.企業ＣＤ
            AND A.締区分 = SG.締区分
            AND A.部署ＣＤ = SG.部署ＣＤ
            AND A.請求先ＣＤ = SG.請求先ＣＤ
            AND A.請求日 = SG.請求日
        LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = A.企業ＣＤ AND B.部署ＣＤ = A.部署ＣＤ

        -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
        INNER JOIN 得意先マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND S.得意先ＣＤ = A.請求先ＣＤ
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
            AND A.部署ＣＤ = C.部署ＣＤ     -- 請求の部署と一致するものに限る
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
            AND A.部署ＣＤ = C.部署ＣＤ     -- 請求の部署と一致するものに限る
            AND J.得意先ＣＤ = C.得意先ＣＤ

    ) AS D
--WHERE (D.今回請求額 - D.請求後入金 + 請求後売上) <> 0
