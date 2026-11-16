-- 得意先単価適用バッチ

-- 対象データ取得
SELECT
    TA.企業ＣＤ
    ,TA.得意先ＣＤ
    ,TA.商品ＣＤ
    ,TA.商品単価税抜
    ,TA.商品単価税込
FROM
    得意先単価適用マスタ AS TA
WHERE
    TA.企業ＣＤ = @企業ＣＤ
    AND TA.商品単価税抜 IS NOT NULL
    AND TA.商品単価税込 IS NOT NULL
    AND TA.単価適用日時 IS NULL
    AND ISNULL(TA.無効フラグ, 0) = 0
    AND TA.適用開始日 = (SELECT
                            MAX(TB.適用開始日)
                        FROM
                            得意先単価適用マスタ AS TB
                        WHERE
                            TB.企業ＣＤ = TA.企業ＣＤ
                            AND TB.得意先ＣＤ = TA.得意先ＣＤ
                            AND TB.商品ＣＤ = TA.商品ＣＤ
                            AND TB.商品単価税抜 IS NOT NULL
                            AND TB.商品単価税込 IS NOT NULL
                            AND TB.単価適用日時 IS NULL
                            AND ISNULL(TB.無効フラグ, 0) = 0
                            AND TB.適用開始日 <= @対象日
                        )


-- 件数取得
SELECT COUNT(*) AS CNT FROM (
    SELECT
        TA.企業ＣＤ
        ,TA.得意先ＣＤ
        ,TA.商品ＣＤ
        ,TA.商品単価税抜
        ,TA.商品単価税込
    FROM
        得意先単価適用マスタ AS TA
    WHERE
        TA.企業ＣＤ = @企業ＣＤ
        AND TA.商品単価税抜 IS NOT NULL
        AND TA.商品単価税込 IS NOT NULL
        AND TA.単価適用日時 IS NULL
        AND ISNULL(TA.無効フラグ, 0) = 0
        AND TA.適用開始日 = (SELECT
                                MAX(TB.適用開始日)
                            FROM
                                得意先単価適用マスタ AS TB
                            WHERE
                                TB.企業ＣＤ = TA.企業ＣＤ
                                AND TB.得意先ＣＤ = TA.得意先ＣＤ
                                AND TB.商品ＣＤ = TA.商品ＣＤ
                                AND TB.商品単価税抜 IS NOT NULL
                                AND TB.商品単価税込 IS NOT NULL
                                AND TB.単価適用日時 IS NULL
                                AND ISNULL(TB.無効フラグ, 0) = 0
                                AND TB.適用開始日 <= @対象日
                            )
) DMY


-- 得意先単価マスタ更新
WITH TAISHO AS (
    SELECT
        TA.企業ＣＤ
        ,TA.得意先ＣＤ
        ,TA.商品ＣＤ
        ,TA.商品単価税抜
        ,TA.商品単価税込
    FROM
        得意先単価適用マスタ AS TA
    WHERE
        TA.企業ＣＤ = @企業ＣＤ
        AND TA.商品単価税抜 IS NOT NULL
        AND TA.商品単価税込 IS NOT NULL
        AND TA.単価適用日時 IS NULL
        AND ISNULL(TA.無効フラグ, 0) = 0
        AND TA.適用開始日 = (SELECT
                                MAX(TB.適用開始日)
                            FROM
                                得意先単価適用マスタ AS TB
                            WHERE
                                TB.企業ＣＤ = TA.企業ＣＤ
                                AND TB.得意先ＣＤ = TA.得意先ＣＤ
                                AND TB.商品ＣＤ = TA.商品ＣＤ
                                AND TB.商品単価税抜 IS NOT NULL
                                AND TB.商品単価税込 IS NOT NULL
                                AND TB.単価適用日時 IS NULL
                                AND ISNULL(TB.無効フラグ, 0) = 0
                                AND TB.適用開始日 <= @対象日
                            )
)
UPDATE TANKA
SET
    TANKA.商品単価税込 = TAISHO.商品単価税込
    ,TANKA.商品単価税抜 = TAISHO.商品単価税抜
    ,TANKA.修正担当者ＣＤ = @担当者ＣＤ
    ,TANKA.修正日 = getdate()
FROM
    得意先単価マスタ AS TANKA
    INNER JOIN TAISHO ON
        TAISHO.企業ＣＤ = TANKA.企業ＣＤ
        AND TAISHO.得意先ＣＤ = TANKA.得意先ＣＤ
        AND TAISHO.商品ＣＤ = TANKA.商品ＣＤ
WHERE
    TANKA.商品単価税込 <> TAISHO.商品単価税込
    OR TANKA.商品単価税抜 <> TAISHO.商品単価税抜



-- 得意先単価マスタ追加
WITH TAISHO AS (
    SELECT
        TA.企業ＣＤ
        ,TA.得意先ＣＤ
        ,TA.商品ＣＤ
        ,TA.商品単価税抜
        ,TA.商品単価税込
    FROM
        得意先単価適用マスタ AS TA
    WHERE
        TA.企業ＣＤ = @企業ＣＤ
        AND TA.商品単価税抜 IS NOT NULL
        AND TA.商品単価税込 IS NOT NULL
        AND TA.単価適用日時 IS NULL
        AND ISNULL(TA.無効フラグ, 0) = 0
        AND TA.適用開始日 = (SELECT
                                MAX(TB.適用開始日)
                            FROM
                                得意先単価適用マスタ AS TB
                            WHERE
                                TB.企業ＣＤ = TA.企業ＣＤ
                                AND TB.得意先ＣＤ = TA.得意先ＣＤ
                                AND TB.商品ＣＤ = TA.商品ＣＤ
                                AND TB.商品単価税抜 IS NOT NULL
                                AND TB.商品単価税込 IS NOT NULL
                                AND TB.単価適用日時 IS NULL
                                AND ISNULL(TB.無効フラグ, 0) = 0
                                AND TB.適用開始日 <= @対象日
                            )
)
INSERT INTO 得意先単価マスタ
SELECT
    TAISHO.企業ＣＤ
    ,TAISHO.得意先ＣＤ
    ,TAISHO.商品ＣＤ
    ,TAISHO.商品単価税込
    ,TAISHO.商品単価税抜
    ,0 AS 無効フラグ
    ,@担当者ＣＤ AS 作成担当者ＣＤ
    ,getdate() AS 作成日
    ,@担当者ＣＤ AS 修正担当者ＣＤ
    ,getdate() AS 修正日
FROM
    TAISHO
    LEFT JOIN 得意先単価マスタ TANKA ON
            TAISHO.企業ＣＤ = TANKA.企業ＣＤ
            AND TAISHO.得意先ＣＤ = TANKA.得意先ＣＤ
            AND TAISHO.商品ＣＤ = TANKA.商品ＣＤ
WHERE
    TANKA.得意先ＣＤ IS NULL



-- 注文データ更新
WITH TAISHO AS (
    SELECT
        TA.企業ＣＤ
        ,TA.得意先ＣＤ
        ,TA.商品ＣＤ
        ,TA.商品単価税抜
        ,TA.商品単価税込
    FROM
        得意先単価適用マスタ AS TA
    WHERE
        TA.企業ＣＤ = @企業ＣＤ
        AND TA.商品単価税抜 IS NOT NULL
        AND TA.商品単価税込 IS NOT NULL
        AND TA.単価適用日時 IS NULL
        AND ISNULL(TA.無効フラグ, 0) = 0
        AND TA.適用開始日 = (SELECT
                                MAX(TB.適用開始日)
                            FROM
                                得意先単価適用マスタ AS TB
                            WHERE
                                TB.企業ＣＤ = TA.企業ＣＤ
                                AND TB.得意先ＣＤ = TA.得意先ＣＤ
                                AND TB.商品ＣＤ = TA.商品ＣＤ
                                AND TB.商品単価税抜 IS NOT NULL
                                AND TB.商品単価税込 IS NOT NULL
                                AND TB.単価適用日時 IS NULL
                                AND ISNULL(TB.無効フラグ, 0) = 0
                                AND TB.適用開始日 <= @対象日
                            )
)
SELECT
    M.企業ＣＤ
    ,M.注文区分
    ,M.喫食日
    ,M.得意先ＣＤ
    ,M.商品ＣＤ
    ,M.部署ＣＤ
    ,H.税区分
    ,H.丸め処理区分
    ,TAISHO.商品単価税抜
    ,TAISHO.商品単価税込
FROM
    注文明細 AS M
    INNER JOIN 注文ヘッダ AS H ON
        H.企業ＣＤ = M.企業ＣＤ
        AND H.注文区分 = M.注文区分
        AND H.喫食日 = M.喫食日
        AND H.得意先ＣＤ = M.得意先ＣＤ
        AND H.部署ＣＤ = M.部署ＣＤ
    INNER JOIN TAISHO ON
        TAISHO.企業ＣＤ = M.企業ＣＤ
        AND TAISHO.得意先ＣＤ = M.得意先ＣＤ
        AND TAISHO.商品ＣＤ = M.商品ＣＤ
WHERE
    M.企業ＣＤ = TAISHO.企業ＣＤ
    AND M.喫食日 >= @対象日
    AND ((H.税区分 = 1 AND M.商品単価 <> TAISHO.商品単価税込)               -- 内税
      OR (H.税区分 = 2 AND M.商品単価 <> TAISHO.商品単価税抜)               -- 外税
      OR (H.税区分 = 3 AND M.商品単価 <> TAISHO.商品単価税抜))              -- 非課税



-- 単価適用日時更新
UPDATE 得意先単価適用マスタ
SET
    単価適用日時 = getdate()
    ,修正担当者ＣＤ = @担当者ＣＤ
    ,修正日 = getdate()
FROM
    得意先単価適用マスタ AS TA
WHERE
    TA.企業ＣＤ = @企業ＣＤ
    AND TA.商品単価税抜 IS NOT NULL
    AND TA.商品単価税込 IS NOT NULL
    AND TA.単価適用日時 IS NULL
    AND ISNULL(TA.無効フラグ, 0) = 0
    -- 得意先・商品が同一で適用開始日が異なるレコードが複数存在する場合、古い方も含めて適用済みとする
    AND TA.適用開始日 <= (SELECT
                            MAX(TB.適用開始日)
                        FROM
                            得意先単価適用マスタ AS TB
                        WHERE
                            TB.企業ＣＤ = TA.企業ＣＤ
                            AND TB.得意先ＣＤ = TA.得意先ＣＤ
                            AND TB.商品ＣＤ = TA.商品ＣＤ
                            AND TB.商品単価税抜 IS NOT NULL
                            AND TB.商品単価税込 IS NOT NULL
                            AND TB.単価適用日時 IS NULL
                            AND ISNULL(TB.無効フラグ, 0) = 0
                            AND TB.適用開始日 <= @対象日
                        )
