-- 一括移動入力_移動元／先に共通する商品かどうか
SELECT
    MOTO.商品ＣＤ
FROM
    (SELECT
        P.商品ＣＤ
    FROM
        コースマスタ AS CM
        INNER JOIN 主要部品リストマスタ AS P ON P.企業ＣＤ = CM.企業ＣＤ AND P.部署ＣＤ = CM.部署ＣＤ
        INNER JOIN 商品マスタ AS S ON S.企業ＣＤ = P.企業ＣＤ AND S.商品ＣＤ = P.商品ＣＤ
    WHERE
        CM.企業ＣＤ = @企業ＣＤ
        AND CM.コースＣＤ = @移動元コースＣＤ
        AND P.商品ＣＤ = @商品ＣＤ
        AND ISNULL(CM.無効フラグ, 0) = 0
        AND ISNULL(S.無効フラグ, 0) = 0
    ) AS MOTO
    -- 双方の条件で検索した結果(=商品CD)で結合する
    INNER JOIN
        (SELECT
            P.商品ＣＤ
        FROM
            コースマスタ AS CM
            INNER JOIN 主要部品リストマスタ AS P ON P.企業ＣＤ = CM.企業ＣＤ AND P.部署ＣＤ = CM.部署ＣＤ
            INNER JOIN 商品マスタ AS S ON S.企業ＣＤ = P.企業ＣＤ AND S.商品ＣＤ = P.商品ＣＤ
        WHERE
            CM.企業ＣＤ = @企業ＣＤ
            AND CM.コースＣＤ = @移動先コースＣＤ
            AND P.商品ＣＤ = @商品ＣＤ
            AND ISNULL(CM.無効フラグ, 0) = 0
            AND ISNULL(S.無効フラグ, 0) = 0
        ) AS SAKI
        ON MOTO.商品ＣＤ = SAKI.商品ＣＤ
