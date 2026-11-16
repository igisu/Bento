-- 固定注文反映

-- 処理対象の有無チェック
SELECT
    -- 一つの得意先に複数の得意先単価適用マスタがある場合はその分が含まれることに注意
    -- 複数のコースに属している得意先がある場合はその分が含まれることに注意
    COUNT(*) AS CNT
FROM
    得意先マスタ AS T
    INNER JOIN 得意先単価適用マスタ AS TA ON
        TA.企業ＣＤ = T.企業ＣＤ
        AND TA.得意先ＣＤ = T.得意先ＣＤ

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
        ON T.企業ＣＤ = C.企業ＣＤ
        AND T.得意先ＣＤ = C.得意先ＣＤ

WHERE
    T.企業ＣＤ = @企業ＣＤ
    AND T.部署ＣＤ = @部署ＣＤ
    AND ISNULL(T.無効フラグ, 0) = 0
    AND (T.受注得意先CD IS NULL OR (T.受注得意先ＣＤ = T.得意先ＣＤ))
    AND ISNULL(TA.無効フラグ, 0) = 0
    AND TA.得意先ＣＤ >= @得意先ＣＤFrom
    AND TA.得意先ＣＤ <= @得意先ＣＤTo
    AND ISNULL(TA.固定数, 0) <> 0
    AND ISNULL(TA.商品単価税抜, -1) <> -1
    AND ISNULL(TA.商品単価税込, -1) <> -1



-- 処理対象の得意先単価適用マスタ取得
SELECT DISTINCT     -- コースによる絞り込みの際、同じ得意先に複数のコースが設定されていると複数のレコードを取得してしまうのでDISTINCTでそれを回避する
    T.得意先ＣＤ
    ,TA.商品ＣＤ
    ,TA.適用開始日
    ,TA.固定数
    ,TA.商品単価税抜
    ,TA.商品単価税込
    ,S.軽減税率フラグ
    ,ISNULL(C.明細番号, 0) AS 明細番号
    ,CASE WHEN SK.対象日 IS NOT NULL THEN ISNULL(T.祝日配送区分, 0) ELSE 1 END AS 祝日配送区分
FROM
    得意先マスタ AS T
    INNER JOIN
        (SELECT
            B1.企業ＣＤ
            ,B1.得意先ＣＤ
            ,B1.商品ＣＤ
            ,B1.適用開始日
            ,ISNULL(B1.固定数, 0) AS 固定数
            ,B1.商品単価税抜
            ,B1.商品単価税込
         FROM
            得意先単価適用マスタ AS B1
            INNER JOIN
                (SELECT
                    企業ＣＤ
                    ,得意先ＣＤ
                    ,商品ＣＤ
                    ,MAX(適用開始日) AS 適用開始日
                 FROM
                    得意先単価適用マスタ
                 WHERE
                    企業ＣＤ = @企業ＣＤ
                    AND ISNULL(無効フラグ, 0) = 0
                    AND 得意先ＣＤ >= @得意先ＣＤFrom
                    AND 得意先ＣＤ <= @得意先ＣＤTo
                    AND 適用開始日 <= @適用日
                    AND ISNULL(商品単価税抜, -1) <> -1
                    AND ISNULL(商品単価税込, -1) <> -1
                 GROUP BY
                    企業ＣＤ
                    ,得意先ＣＤ
                    ,商品ＣＤ
                ) B2 ON
                B1.企業ＣＤ = B2.企業ＣＤ
                AND B1.得意先ＣＤ = B2.得意先ＣＤ
                AND B1.商品ＣＤ   = B2.商品ＣＤ
                AND B1.適用開始日 = B2.適用開始日
        ) AS TA ON
        TA.企業ＣＤ = T.企業ＣＤ
        AND TA.得意先ＣＤ = T.得意先ＣＤ

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
        ) AS CS
        ON T.企業ＣＤ = CS.企業ＣＤ
        AND T.得意先ＣＤ = CS.得意先ＣＤ

    INNER JOIN 商品マスタ AS S ON S.企業ＣＤ = TA.企業ＣＤ AND S.商品ＣＤ = TA.商品ＣＤ
    LEFT JOIN (
        SELECT
            得意先ＣＤ
            ,MAX(明細番号) AS 明細番号
        FROM
            注文明細
        WHERE
            企業ＣＤ = @企業ＣＤ
            AND 注文区分 = @注文区分注文
            AND 喫食日 = @適用日
            AND 部署ＣＤ = @部署ＣＤ
        GROUP BY
            得意先ＣＤ
        ) AS C ON C.得意先ＣＤ = TA.得意先ＣＤ
    LEFT JOIN 祝日マスタ AS SK ON SK.企業ＣＤ = T.企業ＣＤ AND SK.対象日 = @適用日
WHERE
    T.企業ＣＤ = @企業ＣＤ
    AND T.部署ＣＤ = @部署ＣＤ
    AND ISNULL(T.無効フラグ, 0) = 0
    AND (T.受注得意先CD IS NULL OR (T.受注得意先ＣＤ = T.得意先ＣＤ))
    AND ISNULL(S.無効フラグ, 0) = 0
    AND TA.固定数 <> 0
ORDER BY
    T.得意先ＣＤ
    ,TA.商品ＣＤ



-- 最終更新日時を更新
UPDATE 得意先単価適用マスタ
SET 最終注文日時 = getdate()
    ,修正担当者ＣＤ = @担当者ＣＤ
    ,修正日 = getdate()
WHERE 企業ＣＤ = @企業ＣＤ
  AND 得意先ＣＤ = @得意先ＣＤ
  AND 商品ＣＤ   = @商品ＣＤ
  AND 適用開始日 = @適用開始日
