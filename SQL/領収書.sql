-- 領収書
SELECT
    A.請求番号
    ,A.請求先ＣＤ AS 得意先ＣＤ
    ,C.コースＣＤ
    ,A.請求先名 + ' ' + ISNULL(VS.請求書敬称名, '様') AS 得意先名
    ,DATALENGTH(A.請求先名 + ' ' + ISNULL(VS.請求書敬称名, '様')) AS 得意先長さ
    ,S.電話番号1 AS 得意先電話番号
    ,ISNULL(S.税区分, @税区分内税) AS 税区分
    ,ISNULL(S.丸め処理区分, @丸め処理区分切り捨て) AS 丸め処理区分
    ,CASE WHEN ISNULL(A.消費税, 0) = 0 THEN
          CASE WHEN ISNULL(A.買上金額, 0) = 0 THEN A.消費税 ELSE NULL END       -- 内税と思われる場合はNULLを返してプログラム側で消費税額を算出する
         ELSE A.消費税
         END AS 消費税

    -- 売上額を出力する場合、外税の場合は買上金額に消費税額を加算したものを出力する（内税の場合は消費税は加算せず、買上金額をそのまま出力する）
    ,CASE WHEN ISNULL(A.消費税, 0) = 0 THEN
          CASE WHEN ISNULL(A.買上金額, 0) = 0 THEN ISNULL(A.買上金額, 0) + ISNULL(A.消費税, 0) ELSE ISNULL(A.買上金額, 0) END
         ELSE ISNULL(A.買上金額, 0) + ISNULL(A.消費税, 0)
         END AS 今回請求額

    -- 請求額を出力する場合
    ,ISNULL(A.今回請求額, 0) AS 今回請求額

    ,ISNULL(A.繰越額, 0) AS 繰越額
    ,A.請求日
    ,A.回収予定日 AS 支払日
    ,CASE A.締区分 WHEN @締区分月 THEN CAST(CAST(SUBSTRING(CONVERT(VARCHAR,A.請求日,112),5,2) AS INT) AS VARCHAR) + '月分お弁当代として'
                   ELSE 'お弁当代として' END AS 但書
    ,B.会社名称 AS 自社名称
    ,B.郵便番号 AS 自社郵便番号
    ,B.住所 AS 自社住所
    ,B.電話番号 AS 自社電話番号
    ,B.ＦＡＸ AS 自社ＦＡＸ
    ,B.登録番号
    ,G.印影 AS 印鑑画像
    ,'2019/09/25' AS 発行日
FROM
    請求合計 AS A
    -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
    INNER JOIN 得意先マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND S.得意先ＣＤ = A.請求先ＣＤ
    INNER JOIN 得意先マスタ AS J ON J.企業ＣＤ = S.企業ＣＤ AND J.得意先ＣＤ = ISNULL(S.受注得意先ＣＤ, S.得意先ＣＤ)

    -- コースによる絞り込み用
    -- 同じ得意先に複数のコースが設定されている場合、そのいずれのコースを条件に指定しても、そのコースにぶら下がる得意先がヒットしないといけない。
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
            AND A.部署ＣＤ = C.部署ＣＤ     -- 請求の部署と一致するものに限る
            AND J.得意先ＣＤ = C.得意先ＣＤ

    LEFT JOIN V_請求書敬称 AS VS ON
        VS.請求書敬称CD = S.請求書敬称
    LEFT JOIN 部署マスタ AS B ON
        B.企業ＣＤ = A.企業ＣＤ
        AND B.部署ＣＤ = A.部署ＣＤ
    LEFT JOIN 部署画像マスタ AS G ON
        G.企業ＣＤ = A.企業ＣＤ
        AND G.部署ＣＤ = A.部署ＣＤ

WHERE
    A.企業ＣＤ = @企業ＣＤ
    AND A.部署ＣＤ = @部署ＣＤ
    AND A.請求先ＣＤ = @請求先ＣＤ
    AND A.請求日 >= @請求日From
    AND A.請求日 <= @請求日To
    AND A.回収予定日 >= @回収予定日From
    AND A.回収予定日 <= @回収予定日To
    AND CONVERT(date, A.修正日) = @処理日
    AND A.今回請求額 >= @今回請求額
    AND A.締区分 = @締区分
    AND S.支払方法 = @支払方法
    AND S.EB銀行支店ＣＤ = @EB銀行支店ＣＤ      -- 支払方法が振替の場合のみ有効
    AND S.週締曜日 = @週締曜日
    AND S.月締日 = @月締日

@税区分内税 = 1
@丸め処理区分切り捨て = 2
