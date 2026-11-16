-- 請求一覧
SELECT
    A.企業ＣＤ
    ,A.請求番号
    ,A.請求日
    ,A.締区分
    ,CASE A.締区分      -- V_締め区分に「未収金」が無いので結合できない
        WHEN 0 THEN '日締'
        WHEN 1 THEN '週締'
        WHEN 2 THEN '月締'
        WHEN 3 THEN '未収金'
        ELSE ''
    END AS 締め区分名
    ,C.コースＣＤ
    ,C.コース名
    ,A.部署ＣＤ
    ,A.請求先ＣＤ
    ,A.請求先名
    ,A.前回請求額
    ,A.入金額
    ,A.繰越額
    ,A.買上金額
    ,A.調整額
    ,A.消費税
    ,A.今回請求額
    ,A.回収予定日
    ,A.印刷者ＣＤ
    ,A.印刷日時
    ,A.EB作成済フラグ
    ,A.作成担当者ＣＤ
    ,A.作成日
    ,A.修正担当者ＣＤ
    ,A.修正日
    ,A.売上編集日時
    ,SH.支払方法区分名
FROM
    (SELECT
        T.企業ＣＤ
        ,T.請求番号
        ,T.請求日
        ,T.締区分
        ,T.部署ＣＤ
        ,T.請求先ＣＤ
        ,T.請求先名
        ,T.前回請求額
        ,T.入金額
        ,T.繰越額
        ,T.買上金額
        ,T.調整額
        ,T.消費税
        ,T.今回請求額
        ,T.回収予定日
        ,T.印刷者ＣＤ
        ,T.印刷日時
        ,T.EB作成済フラグ
        ,T.作成担当者ＣＤ
        ,T.作成日
        ,T.修正担当者ＣＤ
        ,T.修正日
        ,MAX(U.修正日) AS 売上編集日時
    FROM
        -- ①今回請求のキーと、それに対する前回請求のキーを求める
        (SELECT
            T1.企業ＣＤ
            ,T1.請求番号 AS 今回請求番号
            ,T1.請求先ＣＤ
            ,MAX(P1.請求日) AS 前回請求日
        FROM
            請求合計 AS T1
            LEFT JOIN 請求合計 AS P1
                ON T1.企業ＣＤ = P1.企業ＣＤ
                AND T1.請求先ＣＤ = P1.請求先ＣＤ
                AND P1.請求日 < T1.請求日
            LEFT JOIN 部署マスタ   AS B ON B.企業ＣＤ = T1.企業ＣＤ AND B.部署ＣＤ = T1.部署ＣＤ
            LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = T1.企業ＣＤ AND T.得意先ＣＤ = T1.請求先ＣＤ
        WHERE
            T1.企業ＣＤ = @企業ＣＤ
            AND B.支店ＣＤ = @支店ＣＤ
            AND T1.部署ＣＤ = @部署ＣＤ
            AND T1.請求先ＣＤ = @請求先ＣＤ
            AND T1.請求日 >= @請求日From
            AND T1.請求日 <= @請求日To
            AND T1.回収予定日 >= @回収予定日From
            AND T1.回収予定日 <= @回収予定日To
            AND CONVERT(date, T1.修正日) = @処理日
            AND T1.今回請求額 >= @今回請求額
            AND T1.締区分 = @締区分
            AND T.支払方法 = @支払方法
            AND T.EB銀行支店ＣＤ = @EB銀行支店ＣＤ      -- 支払方法が振替の場合のみ有効
            AND T.週締曜日 = @週締曜日
            AND T.月締日 = @月締日
        GROUP BY
            T1.企業ＣＤ
            ,T1.請求番号
            ,T1.請求先ＣＤ
        ) AS P
        -- ②今回請求の詳細
        INNER JOIN 請求合計 T ON
            T.企業ＣＤ = P.企業ＣＤ
            AND T.請求番号 = P.今回請求番号
        -- ③今回の請求ヘッダ
        LEFT JOIN 請求ヘッダ H ON               -- 請求一覧は請求ヘッダが無くても表示したいのでLEFT JOIN。請求一覧ダイアログは請求ヘッダが有るものだけを表示したいのでINNER JOIN。
            H.企業ＣＤ = T.企業ＣＤ
            AND H.請求番号 = T.請求番号
        -- ④今回の請求に対する最新の売上
        LEFT JOIN 売上明細 AS U ON
            U.企業ＣＤ = H.企業ＣＤ
            AND U.得意先ＣＤ = H.得意先ＣＤ
            AND U.部署ＣＤ = H.部署ＣＤ
            AND U.売上日 > ISNULL(P.前回請求日, DATEADD(month, -3, getdate()))  -- 前回請求が無い場合は、3ヶ月前までを探す
            AND U.売上日 <= T.請求日
    GROUP BY
        T.企業ＣＤ
        ,T.請求番号
        ,T.請求日
        ,T.締区分
        ,T.部署ＣＤ
        ,T.請求先ＣＤ
        ,T.請求先名
        ,T.前回請求額
        ,T.入金額
        ,T.繰越額
        ,T.買上金額
        ,T.調整額
        ,T.消費税
        ,T.今回請求額
        ,T.回収予定日
        ,T.印刷者ＣＤ
        ,T.印刷日時
        ,T.EB作成済フラグ
        ,T.作成担当者ＣＤ
        ,T.作成日
        ,T.修正担当者ＣＤ
        ,T.修正日
    ) AS A
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
            ,CT.コースＣＤ
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
            AND A.部署ＣＤ = C.部署ＣＤ -- 請求の部署と一致するものに限る
            AND J.得意先ＣＤ = C.得意先ＣＤ

    LEFT JOIN V_支払方法区分 AS SH ON S.支払方法 = SH.支払方法区分CD
ORDER BY
    A.請求先ＣＤ
