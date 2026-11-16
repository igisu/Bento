-- 請求書（カレンダー縦）（複数ある場合の明細部）

--- 日付範囲取得の基準日付を取得
SELECT
    G.請求日
    ,ISNULL(P.前回請求日, '2000/01/01') AS 前回請求日
FROM
    -- ①今回請求のキーと、それに対する前回請求のキーを求める
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
        AND G1.請求番号 = @請求番号
    GROUP BY
        G1.企業ＣＤ
        ,G1.請求番号
        ,G1.請求先ＣＤ
    ) AS P
    -- ②今回請求の詳細
    INNER JOIN 請求合計 G ON
        G.企業ＣＤ = P.企業ＣＤ
        AND G.請求番号 = P.今回請求番号





--- 日付範囲を取得する再帰CTE（共通テーブル式）
WITH R AS
(SELECT
    cast(@表示期間From AS Date ) AS 日付
UNION ALL
SELECT
    DATEADD(day, 1, 日付 )
FROM
    R
WHERE R.日付 < @表示期間To
)
SELECT
    0 AS ページ番号
    ,0 AS 明細小計区分
    ,@合算請求フラグ AS 合算請求フラグ
    ,@売上のみフラグ AS 売上のみフラグ
    ,A.日付
    ,RIGHT('00' + cast(month(A.日付) as varchar) ,2) + '/' +
     RIGHT('00' + cast(day(A.日付) as varchar), 2) + '(' +
     CASE DATEPART(WEEKDAY, A.日付)
        WHEN 1 THEN '日'
        WHEN 2 THEN '月'
        WHEN 3 THEN '火'
        WHEN 4 THEN '水'
        WHEN 5 THEN '木'
        WHEN 6 THEN '金'
        WHEN 7 THEN '土'
     END  + ')' AS 表示用日付
    ,A.請求番号
    ,A.請求日
    ,A.締区分
    ,A.部署ＣＤ
    ,A.請求先ＣＤ AS 得意先ＣＤ
    ,A.請求先名 + ' ' + ISNULL(VS.請求書敬称名, '様') AS 得意先名
    ,DATALENGTH(A.請求先名 + ' ' + ISNULL(VS.請求書敬称名, '')) AS 得意先名長さ
    ,NULL AS 前回請求額
    ,NULL AS 入金額
    ,NULL AS 繰越額
    ,NULL AS 買上金額
    ,NULL AS 調整額
    ,NULL AS 消費税
    ,NULL AS 今回請求額
    ,NULL AS 売上のみ前回請求額
    ,NULL AS 売上のみ入金額
    ,NULL AS 売上のみ繰越額
    ,NULL AS 売上のみ今回請求額
    ,NULL AS 回収予定日
    ,@回収予定日印字フラグ AS 回収予定日印字フラグ
    ,ISNULL(A.前回請求日, DATEADD(d, -1, A.最古売上日)) AS 前回請求日
    ,ISNULL(DATEADD(d, 1, A.前回請求日), A.最古売上日) AS 請求期間From
    ,A.請求日 AS 請求期間To
    ,T.郵便番号 AS 得意先郵便番号
    ,T.住所1 AS 得意先住所1
    ,T.住所2 AS 得意先住所2
    ,T.電話番号1 AS 得意先電話番号
    ,T.ＦＡＸ1 AS 得意先ＦＡＸ
    ,T.締区分
    ,CASE T.締区分
        WHEN 1 THEN T.週締曜日
        WHEN 2 THEN T.月締日
        ELSE DAY(A.請求日)  -- 日締
     END AS 締日
    ,T.支払サイト AS サイト
    ,T.支払日
    ,ISNULL(T.支払方法,-1) AS 支払方法
    ,T.振込手数料フラグ             -- 負担をお願いする場合はfalse
    ,T.請求書郵便マークフラグ
    ,ISNULL(T.税区分, @税区分内税) AS 税区分
    ,ISNULL(T.丸め処理区分, @丸め処理区分切り捨て) AS 丸め処理区分
    ,B.会社名称 AS 自社名称
    ,B.郵便番号 AS 自社郵便番号
    ,B.住所 AS 自社住所
    ,B.電話番号 AS 自社電話番号
    ,B.ＦＡＸ AS 自社ＦＡＸ
    ,B.登録番号
    ,B.金融機関ＣＤ1
    ,KKM1.銀行名 AS 銀行名1
    ,B.金融機関支店ＣＤ1
    ,KKSM1.支店名 AS 支店名1
    ,B.口座種別1
    ,VK1.口座種別名 AS 口座種別名1
    ,B.口座番号1
    ,B.口座名義人1
    ,B.金融機関ＣＤ2
    ,KKM2.銀行名 AS 銀行名2
    ,B.金融機関支店ＣＤ2
    ,KKSM2.支店名 AS 支店名2
    ,B.口座種別2
    ,VK2.口座種別名 AS 口座種別名2
    ,B.口座番号2
    ,B.口座名義人2
    ,B.請求書備考1
    ,B.請求書備考2
    ,@コースＣＤ AS コースＣＤ
    ,A.得意先ＣＤ AS 明細得意先ＣＤ
    ,T2.得意先名 AS 明細得意先名
    ,A.商品ＣＤ
    ,cast(A.商品ＣＤ as varchar) + '_' + cast(A.商品単価 as varchar) AS 商品キー
    ,S.商品名
    ,A.商品単価
    ,A.売上日
    ,A.値引後金額
    ,A.計算税率
    ,A.商品数量
    ,@内消費税見出し AS 内消費税見出し
    ,RIGHT('**********' + RIGHT(T.口座番号, 3), LEN(T.口座番号)) AS 振替口座番号
    ,KKM3.銀行名 AS 振替銀行名
    ,KKSM3.支店名 AS 振替支店名
    ,@振替口座印字フラグ AS 振替口座印字フラグ
FROM
    (SELECT
        X.日付
        ,X.企業ＣＤ
        ,X.請求番号
        ,X.請求日
        ,X.締区分
        ,X.部署ＣＤ
        ,X.請求先ＣＤ
        ,X.請求先名
        ,X.前回請求額
        ,X.入金額
        ,X.繰越額
        ,X.買上金額
        ,X.調整額
        ,X.消費税
        ,X.今回請求額
        ,X.回収予定日
        ,X.前回請求日
        ,X.最古売上日
        ,X.得意先ＣＤ
        ,Y.商品ＣＤ
        ,Y.商品単価
        ,Y.売上日
        ,Y.値引後金額
        ,Y.計算税率
        ,Y.商品数量
    FROM
        -- 日付と請求合計をFULL JOINして取得
        (SELECT DISTINCT
            R.日付
            ,G.企業ＣＤ
            ,G.請求番号
            ,G.請求日
            ,G.締区分
            ,G.部署ＣＤ
            ,G.請求先ＣＤ
            ,G.請求先名
            ,G.前回請求額
            ,G.入金額
            ,G.繰越額
            ,G.買上金額
            ,G.調整額
            ,G.消費税
            ,G.今回請求額
            ,G.回収予定日
            ,P.前回請求日
            ,H.得意先ＣＤ
            -- ,UM_OLD.最古売上日
            ,P.最古売上日
        FROM
            R,              -- 日付範囲を取得する再帰CTEを用いて、指定した範囲の日付を取得
            -- ①今回請求のキーと、それに対する前回請求のキーを求める
            (SELECT
                G1.企業ＣＤ
                ,G1.請求番号 AS 今回請求番号
                ,G1.請求先ＣＤ
                ,UM_OLD.最古売上日
                ,MAX(P1.請求日) AS 前回請求日
            FROM
                請求合計 AS G1
                LEFT JOIN 請求合計 AS P1
                    ON G1.企業ＣＤ = P1.企業ＣＤ
                    AND G1.請求先ＣＤ = P1.請求先ＣＤ
                    AND P1.請求日 < G1.請求日
                -- ⑤前回請求日が無い場合の、最古の売上日（全てのレコードにおいて同じ値になるので、駆動表に含めることで高速化を図る）
                LEFT JOIN
                    (SELECT
                        OH.企業ＣＤ
                        ,OH.請求先ＣＤ
                        ,MIN(OM.売上日) AS 最古売上日
                    FROM
                        請求ヘッダ AS OH
                        INNER JOIN 売上明細 AS OM ON
                            OH.企業ＣＤ = OM.企業ＣＤ
                            AND OH.得意先ＣＤ = OM.得意先ＣＤ
                            AND OH.部署ＣＤ = OM.部署ＣＤ
                    WHERE
                        OH.企業ＣＤ = @企業ＣＤ
                        AND OH.請求番号 = @請求番号
                        AND OM.売上区分 = @売上区分売掛        -- 売掛のみ
                        AND ISNULL(OM.商品数量, 0) <> 0
                    GROUP BY
                        OH.企業ＣＤ
                        ,OH.請求先ＣＤ
                    ) AS UM_OLD ON
                    UM_OLD.企業ＣＤ = G1.企業ＣＤ
                    AND UM_OLD.請求先ＣＤ = G1.請求先ＣＤ
            WHERE
                G1.企業ＣＤ = @企業ＣＤ
                AND G1.請求番号 = @請求番号
            GROUP BY
                G1.企業ＣＤ
                ,G1.請求番号
                ,G1.請求先ＣＤ
                ,UM_OLD.最古売上日
            ) AS P
            -- ②今回請求の詳細
            INNER JOIN 請求合計 G ON
                G.企業ＣＤ = P.企業ＣＤ
                AND G.請求番号 = P.今回請求番号
            -- ③今回の請求ヘッダ（売上がある請求ヘッダのみ取得する）（部署ごとの最新の売上を取得する必要があるので合計にはしない）
            --   計算税率ごとにレコードがあるのでそれを同一視する（計算税率ごとのレコードを取得しない）
            LEFT JOIN
                (SELECT DISTINCT
                     企業ＣＤ
                     ,請求番号
                     ,部署ＣＤ
                     ,得意先ＣＤ
                     ,請求先ＣＤ
                FROM
                    請求ヘッダ
                WHERE
                    売上金額税込 <> 0         -- 改ページに必要なので、売上が無くても得意先ＣＤを取得する必要がある
                ) AS H ON
                H.企業ＣＤ = G.企業ＣＤ
                AND H.請求番号 = G.請求番号
            -- ⑤前回請求日が無い場合の、最古の売上日（ここでの取得は廃止する）
            -- LEFT JOIN
            --     (SELECT
            --         OH.企業ＣＤ
            --         ,OH.請求先ＣＤ
            --         ,MIN(OM.売上日) AS 最古売上日
            --     FROM
            --         請求ヘッダ AS OH
            --         INNER JOIN 売上明細 AS OM ON
            --         OH.企業ＣＤ = OM.企業ＣＤ
            --         AND OH.得意先ＣＤ = OM.得意先ＣＤ
            --         AND OH.部署ＣＤ = OM.部署ＣＤ
            --     WHERE
            --         OH.請求番号 = @請求番号
            --         AND OM.売上区分 = @売上区分売掛        -- 売掛のみ
            --         AND ISNULL(OM.商品数量, 0) <> 0
            --     GROUP BY
            --         OH.企業ＣＤ
            --         ,OH.請求先ＣＤ
            --     ) AS UM_OLD ON
            --     UM_OLD.企業ＣＤ = H.企業ＣＤ
            --     AND UM_OLD.請求先ＣＤ = H.請求先ＣＤ
        ) AS X
         -- 売上
        LEFT JOIN
            (SELECT
                G.企業ＣＤ
                ,G.請求番号
                ,URI.得意先ＣＤ
                ,URI.商品ＣＤ
                ,URI.商品単価
                ,URI.売上日
                ,SUM(URI.値引後金額) AS 値引後金額
                ,URI.計算税率
                ,SUM(URI.商品数量) AS 商品数量
            FROM
                -- ①今回請求のキーと、それに対する前回請求のキーを求める
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
                    AND G1.請求番号 = @請求番号
                GROUP BY
                    G1.企業ＣＤ
                    ,G1.請求番号
                    ,G1.請求先ＣＤ
                ) AS P
                -- ②今回請求の詳細
                INNER JOIN 請求合計 G ON
                    G.企業ＣＤ = P.企業ＣＤ
                    AND G.請求番号 = P.今回請求番号
                -- ③今回の請求ヘッダ（売上がある請求ヘッダのみ取得する）（部署ごとの最新の売上を取得する必要があるので合計にはしない）
                --   計算税率ごとにレコードがあるのでそれを同一視する（計算税率ごとのレコードを取得しない）
                LEFT JOIN
                    (SELECT DISTINCT
                         企業ＣＤ
                         ,請求番号
                         ,部署ＣＤ
                         ,得意先ＣＤ
                         ,請求先ＣＤ
                    FROM
                        請求ヘッダ
                    WHERE
                        売上金額税込 <> 0         -- 改ページに必要なので、売上が無くても得意先ＣＤを取得する必要がある
                    ) AS H ON
                    H.企業ＣＤ = G.企業ＣＤ
                    AND H.請求番号 = G.請求番号
                -- ④今回の請求に対する最新の売上
                LEFT JOIN
                    (SELECT
                        UH.企業ＣＤ
                        ,UH.得意先ＣＤ
                        ,UH.部署ＣＤ
                        ,UH.売上日
                        ,UM.商品ＣＤ
                        ,UM.商品単価
                        ,ISNULL(SUM(UM.値引後金額), 0) AS 値引後金額
                        ,UM.計算税率
                        ,ISNULL(SUM(UM.商品数量), 0) AS 商品数量
                    FROM
                        売上ヘッダ AS UH
                        INNER JOIN 売上明細 AS UM ON
                            UM.企業ＣＤ = UH.企業ＣＤ
                            AND UM.得意先ＣＤ = UH.得意先ＣＤ
                            AND UM.部署ＣＤ = UH.部署ＣＤ
                            AND UM.売上日 = UH.売上日
                    WHERE
                        UM.売上区分 = @売上区分売掛        -- 売掛のみ
--                      AND (ISNULL(UM.商品単価, 0) <> 0 OR ISNULL(UM.値引額, 0) <> 0)        -- 単価が0でも、値引が0でなければ出力する
                    GROUP BY
                        UH.企業ＣＤ
                        ,UH.得意先ＣＤ
                        ,UH.部署ＣＤ
                        ,UH.売上日
                        ,UM.商品ＣＤ
                        ,UM.商品単価
                        ,UM.計算税率
                    ) AS URI ON
                        URI.企業ＣＤ = H.企業ＣＤ
                        AND URI.得意先ＣＤ = H.得意先ＣＤ
                        AND URI.部署ＣＤ = H.部署ＣＤ
                        AND URI.売上日 > ISNULL(P.前回請求日, '2000/01/01')
                        AND URI.売上日 <= G.請求日
                        AND URI.商品数量 <> 0
             GROUP BY
                 G.企業ＣＤ
                 ,G.請求番号
                 ,URI.得意先ＣＤ
                 ,URI.商品ＣＤ
                 ,URI.商品単価
                 ,URI.売上日
                 ,URI.計算税率
             ) AS Y ON
                 X.企業ＣＤ = Y.企業ＣＤ
                 AND X.請求番号 = Y.請求番号
                 AND X.得意先ＣＤ = Y.得意先ＣＤ
                 AND X.日付 = Y.売上日
    ) AS A
    LEFT JOIN 商品マスタ AS S ON
        S.企業ＣＤ = A.企業ＣＤ
        AND S.商品ＣＤ = A.商品ＣＤ
    LEFT JOIN 得意先マスタ AS T
        ON  T.企業ＣＤ = A.企業ＣＤ
        AND T.得意先ＣＤ = A.請求先ＣＤ
    LEFT JOIN 部署マスタ AS B ON
        B.企業ＣＤ = A.企業ＣＤ
        AND B.部署ＣＤ = A.部署ＣＤ
    LEFT JOIN V_請求書敬称 AS VS ON
        VS.請求書敬称CD = T.請求書敬称
    LEFT JOIN 金融機関名称 AS KKM1 ON
        KKM1.銀行ＣＤ = B.金融機関ＣＤ1
    LEFT JOIN 金融機関支店名称 AS KKSM1 ON
        KKSM1.銀行ＣＤ = B.金融機関ＣＤ1
        AND KKSM1.支店ＣＤ = B.金融機関支店ＣＤ1
    LEFT JOIN V_口座種別 AS VK1 ON
        VK1.口座種別CD = B.口座種別1
    LEFT JOIN 金融機関名称 AS KKM2 ON
        KKM2.銀行ＣＤ = B.金融機関ＣＤ2
    LEFT JOIN 金融機関支店名称 AS KKSM2 ON
        KKSM2.銀行ＣＤ = B.金融機関ＣＤ2
        AND KKSM2.支店ＣＤ = B.金融機関支店ＣＤ2
    LEFT JOIN V_口座種別 AS VK2 ON
        VK2.口座種別CD = B.口座種別2
    LEFT JOIN 得意先マスタ AS T2
        ON  T2.企業ＣＤ = A.企業ＣＤ
        AND T2.得意先ＣＤ = A.得意先ＣＤ
    LEFT JOIN 金融機関名称 AS KKM3 ON
        KKM3.銀行ＣＤ = T.金融機関ＣＤ
    LEFT JOIN 金融機関支店名称 AS KKSM3 ON
        KKSM3.銀行ＣＤ = T.金融機関ＣＤ
        AND KKSM3.支店ＣＤ = T.金融機関支店ＣＤ
ORDER BY
    A.得意先ＣＤ
    ,A.日付
    ,A.商品ＣＤ



@表示期間From = '2018/04/01'
@表示期間To = '2018/04/30'
@企業ＣＤ = 40000
@請求番号 = 120465
@売上区分売掛 = 1
@合算請求フラグ = 0
@売上のみフラグ = 0         -- 0(というかfalse)固定
@回収予定日印字フラグ = 0   -- 0(というかfalse)固定
@コースＣＤ = 1001
@内消費税見出し = 0
@税区分内税 = 1
@丸め処理区分切り捨て = 2




@表示期間From = '2019/09/01'
@表示期間To = '2019/09/30'
@企業ＣＤ = 40001
@請求番号 = 69
