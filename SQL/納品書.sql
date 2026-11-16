-- 納品書
SELECT
    A.納品書伝票番号
    ,0 AS ページ番号
    ,'納    品    書(控)' AS 見出し控え
    ,'納    品    書' AS 見出し
    -- ,'納品書兼請求書(控)' AS 見出し控え
    -- ,'納品書兼請求書' AS 見出し
    ,T.郵便番号 AS 得意先郵便番号
    ,T.住所1 AS 得意先住所1
    ,T.住所2 AS 得意先住所2
    ,T.得意先名 + ' ' + ISNULL(VS.請求書敬称名, '様') AS 得意先名
    ,DATALENGTH(T.得意先名 + ' ' + ISNULL(VS.請求書敬称名, '様')) AS 得意先名長さ
    ,CASE WHEN T.お届け先住所1 IS NULL
        THEN T.住所1 + ISNULL(T.住所2, '')
        ELSE T.お届け先住所1 + ISNULL(T.お届け先住所2 ,'')
    END AS 納品先住所
    ,ISNULL(T.お届け先電話番号1, ISNULL(お届け先電話番号2, ISNULL(電話番号1, 電話番号2))) AS 納品先電話番号
    ,ISNULL(A.税区分, @税区分内税) AS 税区分
    ,CASE ISNULL(A.税区分, @税区分内税)
        WHEN @税区分内税 THEN '全て税込です'
        WHEN @税区分外税 THEN '全て税別です'
        ELSE ''
     END AS 税区分備考
    ,ISNULL(A.丸め処理区分, @丸め処理区分切り捨て) AS 丸め処理区分
    ,ISNULL(T.売掛現金区分, @売掛現金区分現金) AS 売掛現金区分
    ,A.企業ＣＤ
    ,A.注文区分
    ,A.得意先ＣＤ
    ,A.部署ＣＤ
    ,A.配送日
    ,RIGHT('0000' + convert(varchar, year(A.配送日)), 4) + '年' +
     RIGHT('00' + convert(varchar, month(A.配送日)) ,2) + '月' +
     RIGHT('00' + convert(varchar, day(A.配送日)), 2) + '日(' +
     CASE DATEPART(WEEKDAY, A.配送日)
        WHEN 1 THEN '日'
        WHEN 2 THEN '月'
        WHEN 3 THEN '火'
        WHEN 4 THEN '水'
        WHEN 5 THEN '木'
        WHEN 6 THEN '金'
        WHEN 7 THEN '土'
     END  + ')' AS 納品日
    ,A.明細件数
    ,((A.明細件数 - 1) / @行数) + 1 AS ページ数         -- @行数 = 7
    -- 金額は出力しない
    --,A.小計
    --,A.合計金額
    ,NULL AS 小計
    ,NULL AS 合計金額
    ,NULL AS 消費税             -- 消費税は内税でも外税でも計算し直す
    ,A.作成日 AS 注文日付
    ,B.会社名称 AS 自社名称
    ,B.郵便番号 AS 自社郵便番号
    ,B.住所 AS 自社住所
    ,B.電話番号 AS 自社電話番号
    ,B.ＦＡＸ AS 自社FAX
    ,B.登録番号
    ,@金額印字フラグ AS 金額印字フラグ
    ,@内消費税見出し AS 内消費税見出し
    ,@見出しフラグ AS 納品書兼請求書フラグ
FROM
    (SELECT
        H.企業ＣＤ
        ,H.注文区分
        ,H.得意先ＣＤ
        ,H.部署ＣＤ
        ,H.配送日
        ,H.コースＣＤ
        ,H.税区分
        ,H.丸め処理区分
        ,H.納品書伝票番号
        ,MIN(H.作成日) AS 作成日
        ,COUNT(M.明細番号) AS 明細件数
        ,SUM(M.商品金額合計) AS 小計
        ,SUM(M.消費税額) AS 消費税
        ,SUM(M.商品金額合計税込) AS 合計金額
    FROM
        注文ヘッダ AS H
        INNER JOIN 注文明細 AS M
            ON  H.企業ＣＤ   = M.企業ＣＤ
            AND H.注文区分   = M.注文区分
            AND H.喫食日     = M.喫食日
            AND H.得意先ＣＤ = M.得意先ＣＤ
            AND H.部署ＣＤ   = M.部署ＣＤ
    WHERE
        H.企業ＣＤ = 1
        AND H.注文区分 = 0
        AND H.部署ＣＤ = @部署ＣＤ
        AND H.配送日 = '2016/08/03'
        AND H.コースＣＤ >= @コースＣＤFrom
        AND H.コースＣＤ <= @コースＣＤTo
        AND H.得意先ＣＤ >= @得意先ＣＤFrom
        AND H.得意先ＣＤ <= @得意先ＣＤTo
        AND H.納品書伝票番号 IS NULL
    GROUP BY
        H.企業ＣＤ
        ,H.注文区分
        ,H.得意先ＣＤ
        ,H.部署ＣＤ
        ,H.配送日
        ,H.コースＣＤ
        ,H.税区分
        ,H.丸め処理区分
        ,H.納品書伝票番号
    ) AS A
    LEFT JOIN 得意先マスタ AS T
        ON T.企業ＣＤ = A.企業ＣＤ
        AND T.得意先ＣＤ = A.得意先ＣＤ
    LEFT JOIN コーステーブル AS C
        ON C.企業ＣＤ = A.企業ＣＤ
        AND C.コースＣＤ = A.コースＣＤ
        AND C.得意先ＣＤ = A.得意先ＣＤ
    LEFT JOIN V_請求書敬称 AS VS
        ON VS.請求書敬称CD = T.請求書敬称
    LEFT JOIN 部署マスタ AS B
        ON B.企業ＣＤ = A.企業ＣＤ
        AND B.部署ＣＤ = A.部署ＣＤ
WHERE
    ISNULL(T.納品書発行区分, 0) <> 0    -- 「得意先マスタの設定を使用する」チェックボックスがONの場合に追加する（納品書発行区分がONの得意先のみ印刷対象にする）
ORDER BY
    A.コースＣＤ
    ,C.コース順
    ,CASE WHEN A.納品書伝票番号 IS NULL THEN 1 ELSE 0 END
    ,A.納品書伝票番号

    A.得意先ＣＤ
    ,CASE WHEN A.納品書伝票番号 IS NULL THEN 1 ELSE 0 END
    ,A.納品書伝票番号



-- 明細
SELECT
    0 AS ページ番号
    ,H.企業ＣＤ
    ,H.注文区分
    ,H.得意先ＣＤ
    ,H.部署ＣＤ
    ,H.配送日
    ,H.コースＣＤ
    ,H.納品書伝票番号
    ,M.商品ＣＤ
    ,S.商品名
    ,M.喫食日
    ,M.明細番号
    ,M.商品数量 AS 数量
    ,M.商品単価 AS 単価
    -- 金額は出力しない
    --,M.商品金額合計 AS 金額
    --,M.計算税率
    ,NULL AS 金額
    ,NULL AS 計算税率
    ,0 AS 明細ソート用番号
FROM
    注文ヘッダ AS H
    INNER JOIN 注文明細 AS M
        ON  H.企業ＣＤ   = M.企業ＣＤ
        AND H.注文区分   = M.注文区分
        AND H.喫食日     = M.喫食日
        AND H.得意先ＣＤ = M.得意先ＣＤ
        AND H.部署ＣＤ   = M.部署ＣＤ
    LEFT JOIN 商品マスタ AS S ON
        S.企業ＣＤ = M.企業ＣＤ
        AND S.商品ＣＤ = M.商品ＣＤ
    LEFT JOIN 得意先マスタ AS T
        ON T.企業ＣＤ = H.企業ＣＤ
        AND T.得意先ＣＤ = H.得意先ＣＤ
WHERE
    H.企業ＣＤ = 1
    AND H.注文区分 = 0
    AND H.部署ＣＤ = @部署ＣＤ
    AND H.配送日 = '2016/08/03'
    AND H.コースＣＤ >= @コースＣＤFrom
    AND H.コースＣＤ <= @コースＣＤTo
    AND H.得意先ＣＤ >= @得意先ＣＤFrom
    AND H.得意先ＣＤ <= @得意先ＣＤTo
    AND ISNULL(T.納品書発行区分, 0) <> 0    -- 「得意先マスタの設定を使用する」チェックボックスがONの場合に追加する（納品書発行区分がONの得意先のみ印刷対象にする）
ORDER BY
    H.得意先ＣＤ
    ,CASE WHEN H.納品書伝票番号 IS NULL THEN 1 ELSE 0 END
    ,H.納品書伝票番号
    ,M.喫食日
    ,M.明細番号



-- 注文ヘッダの納品書伝票番号を更新
SELECT
    ISNULL(MAX(A.納品書伝票番号), 0) + 1 as 納品書伝票番号
FROM
    (SELECT
        ISNULL(MAX(納品書伝票番号), 0) AS 納品書伝票番号
    FROM
        注文ヘッダ
    WHERE
        企業ＣＤ = @企業ＣＤ
    UNION
    SELECT
        ISNULL(MAX(納品書伝票番号), 0) AS 納品書伝票番号
    FROM
        売上ヘッダ
    WHERE
        企業ＣＤ = @企業ＣＤ
    ) AS A

UPDATE 注文ヘッダ
SET  納品書伝票番号 = @納品書伝票番号
WHERE 企業ＣＤ     = @企業ＣＤ
  AND 注文区分     = @注文区分
  AND 得意先ＣＤ   = @得意先ＣＤ
  AND 部署ＣＤ     = @部署ＣＤ
  AND 配送日       = @配送日            -- 喫食日は条件に加えない
  AND 納品書伝票番号 IS NULL



-- 売上ヘッダの納品書伝票番号の更新
UPDATE 売上ヘッダ
SET 納品書伝票番号 = CH.納品書伝票番号
FROM
    売上ヘッダ AS UH
    INNER JOIN 注文ヘッダ AS CH
        ON CH.企業ＣＤ = UH.企業ＣＤ
        AND CH.喫食日 = UH.売上日
        and CH.得意先ＣＤ = UH.得意先ＣＤ
        AND CH.部署ＣＤ = UH.部署ＣＤ
WHERE
    CH.企業ＣＤ = 1
    AND CH.注文区分 = @注文区分注文
    AND CH.得意先ＣＤ = @得意先ＣＤ
    AND CH.部署ＣＤ = @部署ＣＤ
    AND CH.配送日 = @配送日
    AND UH.納品書伝票番号 IS NULL





--*--*--*--*--*--*--*
-- これより売上ベース
--*--*--*--*--*--*--*



-- 納品書（売上ベース）
SELECT
    A.納品書伝票番号
    ,0 AS ページ番号
    ,'納    品    書(控)' AS 見出し控え
    ,'納    品    書' AS 見出し
    -- ,'納品書兼請求書(控)' AS 見出し控え
    -- ,'納品書兼請求書' AS 見出し
    ,T.郵便番号 AS 得意先郵便番号
    ,T.住所1 AS 得意先住所1
    ,T.住所2 AS 得意先住所2
    ,T.得意先名 + ' ' + ISNULL(VS.請求書敬称名, '様') AS 得意先名
    ,DATALENGTH(T.得意先名 + ' ' + ISNULL(VS.請求書敬称名, '様')) AS 得意先名長さ
    ,CASE WHEN T.お届け先住所1 IS NULL
        THEN T.住所1 + ISNULL(T.住所2, '')
        ELSE T.お届け先住所1 + ISNULL(T.お届け先住所2 ,'')
    END AS 納品先住所
    ,ISNULL(T.お届け先電話番号1, ISNULL(お届け先電話番号2, ISNULL(電話番号1, 電話番号2))) AS 納品先電話番号
    ,ISNULL(A.税区分, @税区分内税) AS 税区分
    ,CASE ISNULL(A.税区分, @税区分内税)
        WHEN @税区分内税 THEN '全て税込です'
        WHEN @税区分外税 THEN '全て税別です'
        ELSE ''
     END AS 税区分備考
    ,ISNULL(A.丸め処理区分, @丸め処理区分切り捨て) AS 丸め処理区分
    ,ISNULL(T.売掛現金区分, @売掛現金区分現金) AS 売掛現金区分
    ,A.企業ＣＤ
    ,A.得意先ＣＤ
    ,A.部署ＣＤ
    ,A.配送日
    ,RIGHT('0000' + convert(varchar, year(A.配送日)), 4) + '年' +
     RIGHT('00' + convert(varchar, month(A.配送日)) ,2) + '月' +
     RIGHT('00' + convert(varchar, day(A.配送日)), 2) + '日(' +
     CASE DATEPART(WEEKDAY, A.配送日)
        WHEN 1 THEN '日'
        WHEN 2 THEN '月'
        WHEN 3 THEN '火'
        WHEN 4 THEN '水'
        WHEN 5 THEN '木'
        WHEN 6 THEN '金'
        WHEN 7 THEN '土'
     END  + ')' AS 納品日
    ,A.明細件数
    ,((A.明細件数 - 1) / @行数) + 1 AS ページ数         -- @行数 = 7
    -- 金額は出力しない
    --,A.小計
    --,A.合計金額
    ,NULL AS 小計
    ,NULL AS 合計金額
    ,NULL AS 消費税             -- 消費税は内税でも外税でも計算し直す
    ,A.作成日 AS 注文日付
    ,B.会社名称 AS 自社名称
    ,B.郵便番号 AS 自社郵便番号
    ,B.住所 AS 自社住所
    ,B.電話番号 AS 自社電話番号
    ,B.ＦＡＸ AS 自社FAX
    ,B.登録番号
    ,@金額印字フラグ AS 金額印字フラグ
    ,@内消費税見出し AS 内消費税見出し
    ,@見出しフラグ AS 納品書兼請求書フラグ
FROM
    (SELECT
        H.企業ＣＤ
        ,H.得意先ＣＤ
        ,H.部署ＣＤ
        ,H.売上日 AS 配送日
        ,H.コースＣＤ
        ,H.税区分
        ,H.丸め処理区分
        ,H.納品書伝票番号
        ,MIN(H.作成日) AS 作成日
        ,COUNT(M.明細番号) AS 明細件数
        ,SUM(M.値引後金額) AS 小計
        ,SUM(M.消費税額) AS 消費税
        ,SUM(M.商品金額合計税込) AS 合計金額
    FROM
        売上ヘッダ AS H
        INNER JOIN 売上明細 AS M
            ON  H.企業ＣＤ   = M.企業ＣＤ
            AND H.売上日     = M.売上日
            AND H.得意先ＣＤ = M.得意先ＣＤ
            AND H.部署ＣＤ   = M.部署ＣＤ
    WHERE
        H.企業ＣＤ = 1
        AND H.部署ＣＤ = @部署ＣＤ
        AND H.売上日 = '2016/08/03'
        AND H.コースＣＤ >= @コースＣＤFrom
        AND H.コースＣＤ <= @コースＣＤTo
        AND H.得意先ＣＤ >= @得意先ＣＤFrom
        AND H.得意先ＣＤ <= @得意先ＣＤTo
        AND H.納品書伝票番号 IS NULL
    GROUP BY
        H.企業ＣＤ
        ,H.得意先ＣＤ
        ,H.部署ＣＤ
        ,H.売上日
        ,H.コースＣＤ
        ,H.税区分
        ,H.丸め処理区分
        ,H.納品書伝票番号
    ) AS A
    LEFT JOIN 得意先マスタ AS T
        ON T.企業ＣＤ = A.企業ＣＤ
        AND T.得意先ＣＤ = A.得意先ＣＤ
    LEFT JOIN コーステーブル AS C
        ON C.企業ＣＤ = A.企業ＣＤ
        AND C.コースＣＤ = A.コースＣＤ
        AND C.得意先ＣＤ = A.得意先ＣＤ
    LEFT JOIN V_請求書敬称 AS VS
        ON VS.請求書敬称CD = T.請求書敬称
    LEFT JOIN 部署マスタ AS B
        ON B.企業ＣＤ = A.企業ＣＤ
        AND B.部署ＣＤ = A.部署ＣＤ
WHERE
    1 = 1
    AND ISNULL(T.納品書発行区分, 0) <> 0    -- 「得意先マスタの設定を使用する」チェックボックスがONの場合に追加する（納品書発行区分がONの得意先のみ印刷対象にする）
ORDER BY
    A.コースＣＤ
    ,C.コース順
    ,CASE WHEN A.納品書伝票番号 IS NULL THEN 1 ELSE 0 END
    ,A.納品書伝票番号

    A.得意先ＣＤ
    ,CASE WHEN A.納品書伝票番号 IS NULL THEN 1 ELSE 0 END
    ,A.納品書伝票番号



-- 明細（売上ベース）
SELECT
    0 AS ページ番号
    ,H.企業ＣＤ
    ,H.得意先ＣＤ
    ,H.部署ＣＤ
    ,H.売上日 AS 配送日
    ,H.コースＣＤ
    ,H.納品書伝票番号
    ,M.商品ＣＤ
    ,S.商品名
    ,M.売上日 AS 喫食日
    ,M.明細番号
    ,M.商品数量 AS 数量
    ,M.商品単価 AS 単価
    -- 金額は出力しない
    --,M.値引後金額 AS 金額
    --,M.計算税率
    ,NULL AS 金額
    ,NULL AS 計算税率
    ,0 AS 明細ソート用番号
FROM
    売上ヘッダ AS H
    INNER JOIN 売上明細 AS M
        ON  H.企業ＣＤ   = M.企業ＣＤ
        AND H.売上日     = M.売上日
        AND H.得意先ＣＤ = M.得意先ＣＤ
        AND H.部署ＣＤ   = M.部署ＣＤ
    LEFT JOIN 商品マスタ AS S ON
        S.企業ＣＤ = M.企業ＣＤ
        AND S.商品ＣＤ = M.商品ＣＤ
    LEFT JOIN 得意先マスタ AS T
        ON T.企業ＣＤ = H.企業ＣＤ
        AND T.得意先ＣＤ = H.得意先ＣＤ
WHERE
    H.企業ＣＤ = 1
    AND H.部署ＣＤ = @部署ＣＤ
    AND H.売上日 = '2016/08/03'
    AND H.コースＣＤ >= @コースＣＤFrom
    AND H.コースＣＤ <= @コースＣＤTo
    AND H.得意先ＣＤ >= @得意先ＣＤFrom
    AND H.得意先ＣＤ <= @得意先ＣＤTo
    AND ISNULL(T.納品書発行区分, 0) <> 0    -- 「得意先マスタの設定を使用する」チェックボックスがONの場合に追加する（納品書発行区分がONの得意先のみ印刷対象にする）
ORDER BY
    H.得意先ＣＤ
    ,CASE WHEN H.納品書伝票番号 IS NULL THEN 1 ELSE 0 END
    ,H.納品書伝票番号
    ,M.売上日
    ,M.明細番号



-- 売上ヘッダの納品書伝票番号を更新
SELECT
    ISNULL(MAX(A.納品書伝票番号), 0) + 1 as 納品書伝票番号
FROM
    (SELECT
        ISNULL(MAX(納品書伝票番号), 0) AS 納品書伝票番号
    FROM
        注文ヘッダ
    WHERE
        企業ＣＤ = @企業ＣＤ
    UNION
    SELECT
        ISNULL(MAX(納品書伝票番号), 0) AS 納品書伝票番号
    FROM
        売上ヘッダ
    WHERE
        企業ＣＤ = @企業ＣＤ
    ) AS A

UPDATE 売上ヘッダ
SET  納品書伝票番号 = @納品書伝票番号
WHERE 企業ＣＤ     = @企業ＣＤ
  AND 得意先ＣＤ   = @得意先ＣＤ
  AND 部署ＣＤ     = @部署ＣＤ
  AND 売上日       = @売上日
  AND 納品書伝票番号 IS NULL
