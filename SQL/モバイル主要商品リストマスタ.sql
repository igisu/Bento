-- モバイル主要商品リストマスタ
SELECT
    B.支店ＣＤ,
    S.支店名,
    A1.部署ＣＤ ,
    B.部署名,
    A1.グループ数,
    A2.商品数
FROM
    -- グループ数取得（部署ごとのユニークなグループの数）
    (SELECT
        A.部署ＣＤ ,
        COUNT(A.グループＣＤ) AS グループ数
    FROM
        (SELECT DISTINCT
            部署ＣＤ ,
            グループＣＤ
        FROM
            モバイル_主要商品リストマスタ
        WHERE
            企業ＣＤ = 1
            AND ISNULL(無効フラグ, 0) = 0
        ) AS A
    GROUP BY
        A.部署ＣＤ
    ) AS A1
    -- 商品数取得（部署ごとの商品の出現数(=レコード数)）
    INNER JOIN
    (SELECT
        部署ＣＤ ,
        COUNT(商品ＣＤ) AS 商品数
    FROM
        モバイル_主要商品リストマスタ
    WHERE
        企業ＣＤ = 1
        AND ISNULL(無効フラグ, 0) = 0
    GROUP BY
        部署ＣＤ
    ) AS A2 ON A1.部署ＣＤ = A2.部署ＣＤ
    LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = 1 AND A1.部署ＣＤ = B.部署ＣＤ
    LEFT JOIN 支店マスタ AS S ON S.企業ＣＤ = 1 AND B.支店ＣＤ = S.支店ＣＤ
WHERE
    1 = 1
--    AND B.支店ＣＤ = 1
ORDER BY
    B.支店ＣＤ,
    A1.部署ＣＤ
