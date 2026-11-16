-- 会計連携

--- 最新集計日を取得（廃止）
SELECT
    ISNULL(MAX(集計日), '2000/01/01') AS 最新集計日
FROM
    会計連携集計テーブル
WHERE
    企業ＣＤ = @企業ＣＤ



--- 最新集計日に該当するレコードを削除（廃止）
DELETE FROM
    会計連携集計テーブル
WHERE
    企業ＣＤ = @企業ＣＤ AND
    集計日 = (SELECT
                  MAX(集計日)
              FROM
                  会計連携集計テーブル
              WHERE
                  企業ＣＤ = @企業ＣＤ
             )


--- 最大伝票番号を取得
SELECT
    ISNULL(MAX(伝票番号), 0) AS 最大伝票番号
FROM
    会計連携集計テーブル
WHERE
    企業ＣＤ = @企業ＣＤ
    AND 集計日 = @出力対象日


--- 出力対象日のレコードを削除（この日だけを再集計する の場合）
DELETE FROM 会計連携集計テーブル
WHERE
    企業ＣＤ = @企業ＣＤ
    AND 集計日 = @出力対象日


--- 売上集計（当日分）
-- 集計ＣＤ使用フラグOFF
SELECT
    @出力対象日 AS 集計日
    ,SW.部署ＣＤ
    ,@出力対象日 AS 伝票日付
    ,SW.仕訳ＣＤ
    ,SW.集計ＣＤ
    ,SW.処理内容
    ,SW.借方ＣＤ
    ,SW.借方部門ＣＤ
    ,SW.借方補助ＣＤ    -- リテラル または [部署ＣＤ], [集計ＣＤ], [借方部門ＣＤ], [貸方部門ＣＤ]
    ,SW.貸方ＣＤ
    ,SW.貸方部門ＣＤ
    ,SW.貸方補助ＣＤ    -- リテラル または [部署ＣＤ], [集計ＣＤ], [借方部門ＣＤ], [貸方部門ＣＤ]
    ,SW.摘要
    ,SW.税率
    --,SW.売上区分
    --,SW.チケット束フラグ
    ,ISNULL(UM.金額, 0) AS 金額
    ,0 AS 差分フラグ
FROM
    仕訳マスタ AS SW
    LEFT JOIN
        (SELECT
            企業ＣＤ
            ,部署ＣＤ
            ,売上区分
            ,チケット束フラグ
            ,SUM(商品金額合計税込) AS 金額
        FROM
            売上明細
        WHERE
            企業ＣＤ = @企業ＣＤ
            AND 売上日 = @出力対象日
        GROUP BY
            企業ＣＤ
            ,部署ＣＤ
            ,売上区分
            ,チケット束フラグ
        ) AS UM
        ON SW.企業ＣＤ = UM.企業ＣＤ
        AND SW.部署ＣＤ = UM.部署ＣＤ
        AND SW.売上区分 = UM.売上区分
        AND SW.チケット束フラグ = UM.チケット束フラグ
WHERE
    SW.企業ＣＤ = @企業ＣＤ
    AND SW.集計対象区分 = @集計対象区分売上
    AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 0    -- 得意先マスタの集計CDと結合しない
UNION
-- 集計ＣＤ使用フラグON
SELECT
    @出力対象日 AS 集計日
    ,SW.部署ＣＤ
    ,@出力対象日 AS 伝票日付
    ,SW.仕訳ＣＤ
    ,SW.集計ＣＤ
    ,SW.処理内容
    ,SW.借方ＣＤ
    ,SW.借方部門ＣＤ
    ,SW.借方補助ＣＤ
    ,SW.貸方ＣＤ
    ,SW.貸方部門ＣＤ
    ,SW.貸方補助ＣＤ
    ,SW.摘要
    ,SW.税率
    --,SW.売上区分
    --,SW.チケット束フラグ
    ,ISNULL(UM.金額, 0) AS 金額
    ,0 AS 差分フラグ
FROM
    仕訳マスタ AS SW
    LEFT JOIN
        (SELECT
            M.企業ＣＤ
            ,M.部署ＣＤ
            ,T.集計ＣＤ
            ,M.売上区分
            ,M.チケット束フラグ
            ,SUM(M.商品金額合計税込) AS 金額
        FROM
            売上明細 AS M
            INNER JOIN 得意先マスタ AS T
            ON M.企業ＣＤ = T.企業ＣＤ
            AND M.得意先ＣＤ = T.得意先ＣＤ
        WHERE
            M.企業ＣＤ = @企業ＣＤ
            AND M.売上日 = @出力対象日
        GROUP BY
            M.企業ＣＤ
            ,M.部署ＣＤ
            ,T.集計ＣＤ
            ,M.売上区分
            ,M.チケット束フラグ
        ) AS UM
        ON SW.企業ＣＤ = UM.企業ＣＤ
        AND SW.部署ＣＤ = UM.部署ＣＤ
        AND SW.集計ＣＤ = UM.集計ＣＤ
        AND SW.売上区分 = UM.売上区分
        AND SW.チケット束フラグ = UM.チケット束フラグ
WHERE
    SW.企業ＣＤ = @企業ＣＤ
    AND SW.集計対象区分 = @集計対象区分売上
    AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 1    -- 得意先マスタの集計CDと結合する
ORDER BY
    SW.部署ＣＤ
    ,SW.仕訳ＣＤ



--- 売上集計（前日までの差分）
SELECT
    @出力対象日 AS 集計日
    ,SW.部署ＣＤ
    ,A.伝票日付
    ,SW.仕訳ＣＤ
    ,SW.集計ＣＤ
    ,SW.処理内容
    ,SW.借方ＣＤ
    ,SW.借方部門ＣＤ
    ,SW.借方補助ＣＤ
    ,SW.貸方ＣＤ
    ,SW.貸方部門ＣＤ
    ,SW.貸方補助ＣＤ
    ,SW.摘要
    ,SW.税率
    ,A.金額
    ,1 AS 差分フラグ
FROM
    -- ①両方に存在（売上－集計テーブル）
    (SELECT
        A1.部署ＣＤ
        ,A1.伝票日付
        ,A1.仕訳ＣＤ
        ,A1.金額 - A2.金額 AS 金額
    FROM
        -- 売上（集計ＣＤ使用フラグOFF）
        (SELECT
            SW.部署ＣＤ
            ,UM.売上日 AS 伝票日付
            ,SW.仕訳ＣＤ
            ,ISNULL(UM.金額, 0) AS 金額
        FROM
            仕訳マスタ AS SW
            LEFT JOIN
                (SELECT
                    企業ＣＤ
                    ,部署ＣＤ
                    ,売上日
                    ,売上区分
                    ,チケット束フラグ
                    ,SUM(商品金額合計税込) AS 金額
                FROM
                    売上明細
                WHERE
                    企業ＣＤ = @企業ＣＤ
                    AND 売上日 >= @出力対象月一日
                    AND 売上日 < @集計用出力対象日      -- 当日の差分を抽出する場合は 売上日 <= @出力対象日 つまり 売上日 < @出力対象日の翌日 となる
                GROUP BY
                    企業ＣＤ
                    ,部署ＣＤ
                    ,売上日
                    ,売上区分
                    ,チケット束フラグ
                ) AS UM
                ON SW.企業ＣＤ = UM.企業ＣＤ
                AND SW.部署ＣＤ = UM.部署ＣＤ
                AND SW.売上区分 = UM.売上区分
                AND SW.チケット束フラグ = UM.チケット束フラグ
        WHERE
            SW.企業ＣＤ = @企業ＣＤ
            AND SW.集計対象区分 = @集計対象区分売上
            AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 0    -- 得意先マスタの集計CDと結合しない
            AND UM.売上日 IS NOT NULL
        UNION
        -- 売上（集計ＣＤ使用フラグON）
        SELECT
            SW.部署ＣＤ
            ,UM.売上日 AS 伝票日付
            ,SW.仕訳ＣＤ
            ,ISNULL(UM.金額, 0) AS 金額
        FROM
            仕訳マスタ AS SW
            LEFT JOIN
                (SELECT
                    M.企業ＣＤ
                    ,M.部署ＣＤ
                    ,M.売上日
                    ,T.集計ＣＤ
                    ,M.売上区分
                    ,M.チケット束フラグ
                    ,SUM(M.商品金額合計税込) AS 金額
                FROM
                    売上明細 AS M
                    INNER JOIN 得意先マスタ AS T
                    ON M.企業ＣＤ = T.企業ＣＤ
                    AND M.得意先ＣＤ = T.得意先ＣＤ
                WHERE
                    M.企業ＣＤ = @企業ＣＤ
                    AND M.売上日 >= @出力対象月一日
                    AND M.売上日 < @集計用出力対象日        -- 当日の差分を抽出する場合は 売上日 <= @出力対象日 つまり 売上日 < @出力対象日の翌日 となる
                GROUP BY
                    M.企業ＣＤ
                    ,M.部署ＣＤ
                    ,M.売上日
                    ,T.集計ＣＤ
                    ,M.売上区分
                    ,M.チケット束フラグ
                ) AS UM
                ON SW.企業ＣＤ = UM.企業ＣＤ
                AND SW.部署ＣＤ = UM.部署ＣＤ
                AND SW.集計ＣＤ = UM.集計ＣＤ
                AND SW.売上区分 = UM.売上区分
                AND SW.チケット束フラグ = UM.チケット束フラグ
        WHERE
            SW.企業ＣＤ = @企業ＣＤ
            AND SW.集計対象区分 = @集計対象区分売上
            AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 1    -- 得意先マスタの集計CDと結合する
            AND UM.売上日 IS NOT NULL
        ) AS A1
        INNER JOIN
            -- 会計連携集計テーブルを前日までの差分取得用に集計
            (SELECT
                K.部署ＣＤ
                ,K.伝票日付
                ,K.仕訳ＣＤ
                ,SUM(K.金額) AS 金額
            FROM
                会計連携集計テーブル AS K
                INNER JOIN 仕訳マスタ AS SW
                    ON K.企業ＣＤ = SW.企業ＣＤ
                    AND K.部署ＣＤ = SW.部署ＣＤ
                    AND K.仕訳ＣＤ = SW.仕訳ＣＤ
            WHERE
                K.企業ＣＤ = @企業ＣＤ
                AND K.集計日 >= @出力対象月一日
                AND K.集計日 < @集計用出力対象日            -- 当日の差分を抽出する場合は 売上日 <= @出力対象日 つまり 売上日 < @出力対象日の翌日 となる
                AND SW.集計対象区分 = @集計対象区分売上
            GROUP BY
                K.部署ＣＤ
                ,K.伝票日付
                ,K.仕訳ＣＤ
            ) AS A2
            ON A1.部署ＣＤ = A2.部署ＣＤ
            AND A1.伝票日付 = A2.伝票日付
            AND A1.仕訳ＣＤ = A2.仕訳ＣＤ
    WHERE
        (A1.金額 - A2.金額) <> 0
    UNION
    -- ②売上に存在（売上のみ）
    SELECT
        A1.部署ＣＤ
        ,A1.伝票日付
        ,A1.仕訳ＣＤ
        ,A1.金額
    FROM
        -- 売上（集計ＣＤ使用フラグOFF）
        (SELECT
            SW.部署ＣＤ
            ,UM.売上日 AS 伝票日付
            ,SW.仕訳ＣＤ
            ,ISNULL(UM.金額, 0) AS 金額
        FROM
            仕訳マスタ AS SW
            LEFT JOIN
                (SELECT
                    企業ＣＤ
                    ,部署ＣＤ
                    ,売上日
                    ,売上区分
                    ,チケット束フラグ
                    ,SUM(商品金額合計税込) AS 金額
                FROM
                    売上明細
                WHERE
                    企業ＣＤ = @企業ＣＤ
                    AND 売上日 >= @出力対象月一日
                    AND 売上日 < @集計用出力対象日      -- 当日の差分を抽出する場合は 売上日 <= @出力対象日 つまり 売上日 < @出力対象日の翌日 となる
                GROUP BY
                    企業ＣＤ
                    ,部署ＣＤ
                    ,売上日
                    ,売上区分
                    ,チケット束フラグ
                ) AS UM
                ON SW.企業ＣＤ = UM.企業ＣＤ
                AND SW.部署ＣＤ = UM.部署ＣＤ
                AND SW.売上区分 = UM.売上区分
                AND SW.チケット束フラグ = UM.チケット束フラグ
        WHERE
            SW.企業ＣＤ = @企業ＣＤ
            AND SW.集計対象区分 = @集計対象区分売上
            AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 0    -- 得意先マスタの集計CDと結合しない
            AND UM.売上日 IS NOT NULL
        UNION
        -- 売上（集計ＣＤ使用フラグON）
        SELECT
            SW.部署ＣＤ
            ,UM.売上日 AS 伝票日付
            ,SW.仕訳ＣＤ
            ,ISNULL(UM.金額, 0) AS 金額
        FROM
            仕訳マスタ AS SW
            LEFT JOIN
                (SELECT
                    M.企業ＣＤ
                    ,M.部署ＣＤ
                    ,M.売上日
                    ,T.集計ＣＤ
                    ,M.売上区分
                    ,M.チケット束フラグ
                    ,SUM(M.商品金額合計税込) AS 金額
                FROM
                    売上明細 AS M
                    INNER JOIN 得意先マスタ AS T
                    ON M.企業ＣＤ = T.企業ＣＤ
                    AND M.得意先ＣＤ = T.得意先ＣＤ
                WHERE
                    M.企業ＣＤ = @企業ＣＤ
                    AND M.売上日 >= @出力対象月一日
                    AND M.売上日 < @集計用出力対象日        -- 当日の差分を抽出する場合は 売上日 <= @出力対象日 つまり 売上日 < @出力対象日の翌日 となる
                GROUP BY
                    M.企業ＣＤ
                    ,M.部署ＣＤ
                    ,M.売上日
                    ,T.集計ＣＤ
                    ,M.売上区分
                    ,M.チケット束フラグ
                ) AS UM
                ON SW.企業ＣＤ = UM.企業ＣＤ
                AND SW.部署ＣＤ = UM.部署ＣＤ
                AND SW.集計ＣＤ = UM.集計ＣＤ
                AND SW.売上区分 = UM.売上区分
                AND SW.チケット束フラグ = UM.チケット束フラグ
        WHERE
            SW.企業ＣＤ = @企業ＣＤ
            AND SW.集計対象区分 = @集計対象区分売上
            AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 1    -- 得意先マスタの集計CDと結合する
            AND UM.売上日 IS NOT NULL
        ) AS A1
        LEFT JOIN
            -- 会計連携集計テーブルを前日までの差分取得用に集計
            (SELECT
                K.部署ＣＤ
                ,K.伝票日付
                ,K.仕訳ＣＤ
                ,SUM(K.金額) AS 金額
            FROM
                会計連携集計テーブル AS K
                INNER JOIN 仕訳マスタ AS SW
                    ON K.企業ＣＤ = SW.企業ＣＤ
                    AND K.部署ＣＤ = SW.部署ＣＤ
                    AND K.仕訳ＣＤ = SW.仕訳ＣＤ
            WHERE
                K.企業ＣＤ = @企業ＣＤ
                AND K.集計日 >= @出力対象月一日
                AND K.集計日 < @集計用出力対象日        -- 当日の差分を抽出する場合は 売上日 <= @出力対象日 つまり 売上日 < @出力対象日の翌日 となる
                AND SW.集計対象区分 = @集計対象区分売上
            GROUP BY
                K.部署ＣＤ
                ,K.伝票日付
                ,K.仕訳ＣＤ
            ) AS A2
            ON A1.部署ＣＤ = A2.部署ＣＤ
            AND A1.伝票日付 = A2.伝票日付
            AND A1.仕訳ＣＤ = A2.仕訳ＣＤ
    WHERE
        A2.仕訳ＣＤ IS NULL
    UNION
    -- ③集計テーブルに存在（集計値をマイナスとして）
    SELECT
        A2.部署ＣＤ
        ,A2.伝票日付
        ,A2.仕訳ＣＤ
        ,A2.金額 * -1 AS 金額
    FROM
        -- 会計連携集計テーブルを前日までの差分取得用に集計
        (SELECT
            K.部署ＣＤ
            ,K.伝票日付
            ,K.仕訳ＣＤ
            ,SUM(K.金額) AS 金額
        FROM
            会計連携集計テーブル AS K
            INNER JOIN 仕訳マスタ AS SW
                ON K.企業ＣＤ = SW.企業ＣＤ
                AND K.部署ＣＤ = SW.部署ＣＤ
                AND K.仕訳ＣＤ = SW.仕訳ＣＤ
        WHERE
            K.企業ＣＤ = @企業ＣＤ
            AND K.集計日 >= @出力対象月一日
            AND K.集計日 < @集計用出力対象日        -- 当日の差分を抽出する場合は 売上日 <= @出力対象日 つまり 売上日 < @出力対象日の翌日 となる
            AND SW.集計対象区分 = @集計対象区分売上
        GROUP BY
            K.部署ＣＤ
            ,K.伝票日付
            ,K.仕訳ＣＤ
        ) AS A2
        LEFT JOIN
            -- 売上（集計ＣＤ使用フラグOFF）
            (SELECT
                SW.部署ＣＤ
                ,UM.売上日 AS 伝票日付
                ,SW.仕訳ＣＤ
                ,ISNULL(UM.金額, 0) AS 金額
            FROM
                仕訳マスタ AS SW
                LEFT JOIN
                    (SELECT
                        企業ＣＤ
                        ,部署ＣＤ
                        ,売上日
                        ,売上区分
                        ,チケット束フラグ
                        ,SUM(商品金額合計税込) AS 金額
                    FROM
                        売上明細
                    WHERE
                        企業ＣＤ = @企業ＣＤ
                        AND 売上日 >= @出力対象月一日
                        AND 売上日 < @集計用出力対象日      -- 当日の差分を抽出する場合は 売上日 <= @出力対象日 つまり 売上日 < @出力対象日の翌日 となる
                    GROUP BY
                        企業ＣＤ
                        ,部署ＣＤ
                        ,売上日
                        ,売上区分
                        ,チケット束フラグ
                    ) AS UM
                    ON SW.企業ＣＤ = UM.企業ＣＤ
                    AND SW.部署ＣＤ = UM.部署ＣＤ
                    AND SW.売上区分 = UM.売上区分
                    AND SW.チケット束フラグ = UM.チケット束フラグ
            WHERE
                SW.企業ＣＤ = @企業ＣＤ
                AND SW.集計対象区分 = @集計対象区分売上
                AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 0    -- 得意先マスタの集計CDと結合しない
                AND UM.売上日 IS NOT NULL
            UNION
            -- 売上（集計ＣＤ使用フラグON）
            SELECT
                SW.部署ＣＤ
                ,UM.売上日 AS 伝票日付
                ,SW.仕訳ＣＤ
                ,ISNULL(UM.金額, 0) AS 金額
            FROM
                仕訳マスタ AS SW
                LEFT JOIN
                    (SELECT
                        M.企業ＣＤ
                        ,M.部署ＣＤ
                        ,M.売上日
                        ,T.集計ＣＤ
                        ,M.売上区分
                        ,M.チケット束フラグ
                        ,SUM(M.商品金額合計税込) AS 金額
                    FROM
                        売上明細 AS M
                        INNER JOIN 得意先マスタ AS T
                        ON M.企業ＣＤ = T.企業ＣＤ
                        AND M.得意先ＣＤ = T.得意先ＣＤ
                    WHERE
                        M.企業ＣＤ = @企業ＣＤ
                        AND M.売上日 >= @出力対象月一日
                        AND M.売上日 < @集計用出力対象日        -- 当日の差分を抽出する場合は 売上日 <= @出力対象日 つまり 売上日 < @出力対象日の翌日 となる
                    GROUP BY
                        M.企業ＣＤ
                        ,M.部署ＣＤ
                        ,M.売上日
                        ,T.集計ＣＤ
                        ,M.売上区分
                        ,M.チケット束フラグ
                    ) AS UM
                    ON SW.企業ＣＤ = UM.企業ＣＤ
                    AND SW.部署ＣＤ = UM.部署ＣＤ
                    AND SW.集計ＣＤ = UM.集計ＣＤ
                    AND SW.売上区分 = UM.売上区分
                    AND SW.チケット束フラグ = UM.チケット束フラグ
            WHERE
                SW.企業ＣＤ = @企業ＣＤ
                AND SW.集計対象区分 = @集計対象区分売上
                AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 1    -- 得意先マスタの集計CDと結合する
                AND UM.売上日 IS NOT NULL
            ) AS A1
            ON A2.部署ＣＤ = A1.部署ＣＤ
            AND A2.伝票日付 = A1.伝票日付
            AND A2.仕訳ＣＤ = A1.仕訳ＣＤ
    WHERE
        A1.仕訳ＣＤ IS NULL
        AND A2.金額 <> 0
    ) AS A
    INNER JOIN 仕訳マスタ AS SW
        ON SW.企業ＣＤ = @企業ＣＤ
        AND A.部署ＣＤ = SW.部署ＣＤ
        AND A.仕訳ＣＤ = SW.仕訳ＣＤ
ORDER BY
    SW.部署ＣＤ
    ,A.伝票日付
    ,SW.仕訳ＣＤ



--- 入金集計（当日分）
-- 集計ＣＤ使用フラグOFF
SELECT
    @出力対象日 AS 集計日
    ,SW.部署ＣＤ
    ,@出力対象日 AS 伝票日付
    ,SW.仕訳ＣＤ
    ,SW.集計ＣＤ
    ,SW.処理内容
    ,SW.借方ＣＤ
    ,SW.借方部門ＣＤ
    ,SW.借方補助ＣＤ    -- リテラル または [部署ＣＤ], [集計ＣＤ], [借方部門ＣＤ], [貸方部門ＣＤ]
    ,SW.貸方ＣＤ
    ,SW.貸方部門ＣＤ
    ,SW.貸方補助ＣＤ    -- リテラル または [部署ＣＤ], [集計ＣＤ], [借方部門ＣＤ], [貸方部門ＣＤ]
    ,SW.摘要
    ,SW.税率
    --,SW.入金方法
    ,ISNULL(NM.金額, 0) AS 金額
    ,0 AS 差分フラグ
FROM
    仕訳マスタ AS SW
    LEFT JOIN
        (SELECT
            企業ＣＤ
            ,部署ＣＤ
            ,入金方法
            ,SUM(入金額) AS 金額
        FROM
            入金明細
        WHERE
            企業ＣＤ = @企業ＣＤ
            AND 入金日 = @出力対象日
        GROUP BY
            企業ＣＤ
            ,部署ＣＤ
            ,入金方法
        ) AS NM
        ON SW.企業ＣＤ = NM.企業ＣＤ
        AND SW.部署ＣＤ = NM.部署ＣＤ
        AND SW.入金方法 = NM.入金方法
WHERE
    SW.企業ＣＤ = @企業ＣＤ
    AND SW.集計対象区分 = @集計対象区分入金
    AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 0    -- 得意先マスタの集計CDと結合しない
UNION
-- 集計ＣＤ使用フラグON
SELECT
    @出力対象日 AS 集計日
    ,SW.部署ＣＤ
    ,@出力対象日 AS 伝票日付
    ,SW.仕訳ＣＤ
    ,SW.集計ＣＤ
    ,SW.処理内容
    ,SW.借方ＣＤ
    ,SW.借方部門ＣＤ
    ,SW.借方補助ＣＤ
    ,SW.貸方ＣＤ
    ,SW.貸方部門ＣＤ
    ,SW.貸方補助ＣＤ
    ,SW.摘要
    ,SW.税率
    --,SW.入金方法
    ,ISNULL(NM.金額, 0) AS 金額
    ,0 AS 差分フラグ
FROM
    仕訳マスタ AS SW
    LEFT JOIN
        (SELECT
            M.企業ＣＤ
            ,M.部署ＣＤ
            ,T.集計ＣＤ
            ,M.入金方法
            ,SUM(M.入金額) AS 金額
        FROM
            入金明細 AS M
            INNER JOIN 得意先マスタ AS T
            ON M.企業ＣＤ = T.企業ＣＤ
            AND M.得意先ＣＤ = T.得意先ＣＤ
        WHERE
            M.企業ＣＤ = @企業ＣＤ
            AND M.入金日 = @出力対象日
        GROUP BY
            M.企業ＣＤ
            ,M.部署ＣＤ
            ,T.集計ＣＤ
            ,M.入金方法
        ) AS NM
        ON SW.企業ＣＤ = NM.企業ＣＤ
        AND SW.部署ＣＤ = NM.部署ＣＤ
        AND SW.集計ＣＤ = NM.集計ＣＤ
        AND SW.入金方法 = NM.入金方法
WHERE
    SW.企業ＣＤ = @企業ＣＤ
    AND SW.集計対象区分 = @集計対象区分入金
    AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 1    -- 得意先マスタの集計CDと結合する
ORDER BY
    SW.部署ＣＤ
    ,SW.仕訳ＣＤ



--- 入金集計（前日までの差分）
SELECT
    @出力対象日 AS 集計日
    ,SW.部署ＣＤ
    ,A.伝票日付
    ,SW.仕訳ＣＤ
    ,SW.集計ＣＤ
    ,SW.処理内容
    ,SW.借方ＣＤ
    ,SW.借方部門ＣＤ
    ,SW.借方補助ＣＤ
    ,SW.貸方ＣＤ
    ,SW.貸方部門ＣＤ
    ,SW.貸方補助ＣＤ
    ,SW.摘要
    ,SW.税率
    ,A.金額
    ,1 AS 差分フラグ
FROM
    -- ①両方に存在（入金－集計テーブル）
    (SELECT
        A1.部署ＣＤ
        ,A1.伝票日付
        ,A1.仕訳ＣＤ
        ,A1.金額 - A2.金額 AS 金額
    FROM
        -- 入金（集計ＣＤ使用フラグOFF）
        (SELECT
            SW.部署ＣＤ
            ,NM.入金日 AS 伝票日付
            ,SW.仕訳ＣＤ
            ,ISNULL(NM.金額, 0) AS 金額
        FROM
            仕訳マスタ AS SW
            LEFT JOIN
                (SELECT
                    企業ＣＤ
                    ,部署ＣＤ
                    ,入金日
                    ,入金方法
                    ,SUM(入金額) AS 金額
                FROM
                    入金明細
                WHERE
                    企業ＣＤ = @企業ＣＤ
                    AND 入金日 >= @出力対象月一日
                    AND 入金日 < @集計用出力対象日      -- 当日の差分を抽出する場合は 入金日 <= @出力対象日 つまり 入金日 < @出力対象日の翌日 となる
                GROUP BY
                    企業ＣＤ
                    ,部署ＣＤ
                    ,入金日
                    ,入金方法
                ) AS NM
                ON SW.企業ＣＤ = NM.企業ＣＤ
                AND SW.部署ＣＤ = NM.部署ＣＤ
                AND SW.入金方法 = NM.入金方法
        WHERE
            SW.企業ＣＤ = @企業ＣＤ
            AND SW.集計対象区分 = @集計対象区分入金
            AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 0    -- 得意先マスタの集計CDと結合しない
            AND NM.入金日 IS NOT NULL
        UNION
        -- 入金（集計ＣＤ使用フラグON）
        SELECT
            SW.部署ＣＤ
            ,NM.入金日 AS 伝票日付
            ,SW.仕訳ＣＤ
            ,ISNULL(NM.金額, 0) AS 金額
        FROM
            仕訳マスタ AS SW
            LEFT JOIN
                (SELECT
                    M.企業ＣＤ
                    ,M.部署ＣＤ
                    ,M.入金日
                    ,T.集計ＣＤ
                    ,M.入金方法
                    ,SUM(M.入金額) AS 金額
                FROM
                    入金明細 AS M
                    INNER JOIN 得意先マスタ AS T
                    ON M.企業ＣＤ = T.企業ＣＤ
                    AND M.得意先ＣＤ = T.得意先ＣＤ
                WHERE
                    M.企業ＣＤ = @企業ＣＤ
                    AND M.入金日 >= @出力対象月一日
                    AND M.入金日 < @集計用出力対象日        -- 当日の差分を抽出する場合は 入金日 <= @出力対象日 つまり 入金日 < @出力対象日の翌日 となる
                GROUP BY
                    M.企業ＣＤ
                    ,M.部署ＣＤ
                    ,M.入金日
                    ,T.集計ＣＤ
                    ,M.入金方法
                ) AS NM
                ON SW.企業ＣＤ = NM.企業ＣＤ
                AND SW.部署ＣＤ = NM.部署ＣＤ
                AND SW.集計ＣＤ = NM.集計ＣＤ
                AND SW.入金方法 = NM.入金方法
        WHERE
            SW.企業ＣＤ = @企業ＣＤ
            AND SW.集計対象区分 = @集計対象区分入金
            AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 1    -- 得意先マスタの集計CDと結合する
            AND NM.入金日 IS NOT NULL
        ) AS A1
        INNER JOIN
            -- 会計連携集計テーブルを前日までの差分取得用に集計
            (SELECT
                K.部署ＣＤ
                ,K.伝票日付
                ,K.仕訳ＣＤ
                ,SUM(K.金額) AS 金額
            FROM
                会計連携集計テーブル AS K
                INNER JOIN 仕訳マスタ AS SW
                    ON K.企業ＣＤ = SW.企業ＣＤ
                    AND K.部署ＣＤ = SW.部署ＣＤ
                    AND K.仕訳ＣＤ = SW.仕訳ＣＤ
            WHERE
                K.企業ＣＤ = @企業ＣＤ
                AND K.集計日 >= @出力対象月一日
                AND K.集計日 < @集計用出力対象日        -- 当日の差分を抽出する場合は 入金日 <= @出力対象日 つまり 入金日 < @出力対象日の翌日 となる
                AND SW.集計対象区分 = @集計対象区分入金
            GROUP BY
                K.部署ＣＤ
                ,K.伝票日付
                ,K.仕訳ＣＤ
            ) AS A2
            ON A1.部署ＣＤ = A2.部署ＣＤ
            AND A1.伝票日付 = A2.伝票日付
            AND A1.仕訳ＣＤ = A2.仕訳ＣＤ
    WHERE
        (A1.金額 - A2.金額) <> 0
    UNION
    -- ②入金に存在（入金のみ）
    SELECT
        A1.部署ＣＤ
        ,A1.伝票日付
        ,A1.仕訳ＣＤ
        ,A1.金額
    FROM
        -- 入金（集計ＣＤ使用フラグOFF）
        (SELECT
            SW.部署ＣＤ
            ,NM.入金日 AS 伝票日付
            ,SW.仕訳ＣＤ
            ,ISNULL(NM.金額, 0) AS 金額
        FROM
            仕訳マスタ AS SW
            LEFT JOIN
                (SELECT
                    企業ＣＤ
                    ,部署ＣＤ
                    ,入金日
                    ,入金方法
                    ,SUM(入金額) AS 金額
                FROM
                    入金明細
                WHERE
                    企業ＣＤ = @企業ＣＤ
                    AND 入金日 >= @出力対象月一日
                    AND 入金日 < @集計用出力対象日      -- 当日の差分を抽出する場合は 入金日 <= @出力対象日 つまり 入金日 < @出力対象日の翌日 となる
                GROUP BY
                    企業ＣＤ
                    ,部署ＣＤ
                    ,入金日
                    ,入金方法
                ) AS NM
                ON SW.企業ＣＤ = NM.企業ＣＤ
                AND SW.部署ＣＤ = NM.部署ＣＤ
                AND SW.入金方法 = NM.入金方法
        WHERE
            SW.企業ＣＤ = @企業ＣＤ
            AND SW.集計対象区分 = @集計対象区分入金
            AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 0    -- 得意先マスタの集計CDと結合しない
            AND NM.入金日 IS NOT NULL
        UNION
        -- 入金（集計ＣＤ使用フラグON）
        SELECT
            SW.部署ＣＤ
            ,NM.入金日 AS 伝票日付
            ,SW.仕訳ＣＤ
            ,ISNULL(NM.金額, 0) AS 金額
        FROM
            仕訳マスタ AS SW
            LEFT JOIN
                (SELECT
                    M.企業ＣＤ
                    ,M.部署ＣＤ
                    ,M.入金日
                    ,T.集計ＣＤ
                    ,M.入金方法
                    ,SUM(M.入金額) AS 金額
                FROM
                    入金明細 AS M
                    INNER JOIN 得意先マスタ AS T
                    ON M.企業ＣＤ = T.企業ＣＤ
                    AND M.得意先ＣＤ = T.得意先ＣＤ
                WHERE
                    M.企業ＣＤ = @企業ＣＤ
                    AND M.入金日 >= @出力対象月一日
                    AND M.入金日 < @集計用出力対象日        -- 当日の差分を抽出する場合は 入金日 <= @出力対象日 つまり 入金日 < @出力対象日の翌日 となる
                GROUP BY
                    M.企業ＣＤ
                    ,M.部署ＣＤ
                    ,M.入金日
                    ,T.集計ＣＤ
                    ,M.入金方法
                ) AS NM
                ON SW.企業ＣＤ = NM.企業ＣＤ
                AND SW.部署ＣＤ = NM.部署ＣＤ
                AND SW.集計ＣＤ = NM.集計ＣＤ
                AND SW.入金方法 = NM.入金方法
        WHERE
            SW.企業ＣＤ = @企業ＣＤ
            AND SW.集計対象区分 = @集計対象区分入金
            AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 1    -- 得意先マスタの集計CDと結合する
            AND NM.入金日 IS NOT NULL
        ) AS A1
        LEFT JOIN
            -- 会計連携集計テーブルを前日までの差分取得用に集計
            (SELECT
                K.部署ＣＤ
                ,K.伝票日付
                ,K.仕訳ＣＤ
                ,SUM(K.金額) AS 金額
            FROM
                会計連携集計テーブル AS K
                INNER JOIN 仕訳マスタ AS SW
                    ON K.企業ＣＤ = SW.企業ＣＤ
                    AND K.部署ＣＤ = SW.部署ＣＤ
                    AND K.仕訳ＣＤ = SW.仕訳ＣＤ
            WHERE
                K.企業ＣＤ = @企業ＣＤ
                AND K.集計日 >= @出力対象月一日
                AND K.集計日 < @集計用出力対象日        -- 当日の差分を抽出する場合は 入金日 <= @出力対象日 つまり 入金日 < @出力対象日の翌日 となる
                AND SW.集計対象区分 = @集計対象区分入金
            GROUP BY
                K.部署ＣＤ
                ,K.伝票日付
                ,K.仕訳ＣＤ
            ) AS A2
            ON A1.部署ＣＤ = A2.部署ＣＤ
            AND A1.伝票日付 = A2.伝票日付
            AND A1.仕訳ＣＤ = A2.仕訳ＣＤ
    WHERE
        A2.仕訳ＣＤ IS NULL
    UNION
    -- ③集計テーブルに存在（集計値をマイナスとして）
    SELECT
        A2.部署ＣＤ
        ,A2.伝票日付
        ,A2.仕訳ＣＤ
        ,A2.金額 * -1 AS 金額
    FROM
        -- 会計連携集計テーブルを前日までの差分取得用に集計
        (SELECT
            K.部署ＣＤ
            ,K.伝票日付
            ,K.仕訳ＣＤ
            ,SUM(K.金額) AS 金額
        FROM
            会計連携集計テーブル AS K
            INNER JOIN 仕訳マスタ AS SW
                ON K.企業ＣＤ = SW.企業ＣＤ
                AND K.部署ＣＤ = SW.部署ＣＤ
                AND K.仕訳ＣＤ = SW.仕訳ＣＤ
        WHERE
            K.企業ＣＤ = @企業ＣＤ
            AND K.集計日 >= @出力対象月一日
            AND K.集計日 < @集計用出力対象日        -- 当日の差分を抽出する場合は 入金日 <= @出力対象日 つまり 入金日 < @出力対象日の翌日 となる
            AND SW.集計対象区分 = @集計対象区分入金
        GROUP BY
            K.部署ＣＤ
            ,K.伝票日付
            ,K.仕訳ＣＤ
        ) AS A2
        LEFT JOIN
            -- 入金（集計ＣＤ使用フラグOFF）
            (SELECT
                SW.部署ＣＤ
                ,NM.入金日 AS 伝票日付
                ,SW.仕訳ＣＤ
                ,ISNULL(NM.金額, 0) AS 金額
            FROM
                仕訳マスタ AS SW
                LEFT JOIN
                    (SELECT
                        企業ＣＤ
                        ,部署ＣＤ
                        ,入金日
                        ,入金方法
                        ,SUM(入金額) AS 金額
                    FROM
                        入金明細
                    WHERE
                        企業ＣＤ = @企業ＣＤ
                        AND 入金日 >= @出力対象月一日
                        AND 入金日 < @集計用出力対象日      -- 当日の差分を抽出する場合は 入金日 <= @出力対象日 つまり 入金日 < @出力対象日の翌日 となる
                    GROUP BY
                        企業ＣＤ
                        ,部署ＣＤ
                        ,入金日
                        ,入金方法
                    ) AS NM
                    ON SW.企業ＣＤ = NM.企業ＣＤ
                    AND SW.部署ＣＤ = NM.部署ＣＤ
                    AND SW.入金方法 = NM.入金方法
            WHERE
                SW.企業ＣＤ = @企業ＣＤ
                AND SW.集計対象区分 = @集計対象区分入金
                AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 0    -- 得意先マスタの集計CDと結合しない
                AND NM.入金日 IS NOT NULL
            UNION
            -- 入金（集計ＣＤ使用フラグON）
            SELECT
                SW.部署ＣＤ
                ,NM.入金日 AS 伝票日付
                ,SW.仕訳ＣＤ
                ,ISNULL(NM.金額, 0) AS 金額
            FROM
                仕訳マスタ AS SW
                LEFT JOIN
                    (SELECT
                        M.企業ＣＤ
                        ,M.部署ＣＤ
                        ,M.入金日
                        ,T.集計ＣＤ
                        ,M.入金方法
                        ,SUM(M.入金額) AS 金額
                    FROM
                        入金明細 AS M
                        INNER JOIN 得意先マスタ AS T
                        ON M.企業ＣＤ = T.企業ＣＤ
                        AND M.得意先ＣＤ = T.得意先ＣＤ
                    WHERE
                        M.企業ＣＤ = @企業ＣＤ
                        AND M.入金日 >= @出力対象月一日
                        AND M.入金日 < @集計用出力対象日        -- 当日の差分を抽出する場合は 入金日 <= @出力対象日 つまり 入金日 < @出力対象日の翌日 となる
                    GROUP BY
                        M.企業ＣＤ
                        ,M.部署ＣＤ
                        ,M.入金日
                        ,T.集計ＣＤ
                        ,M.入金方法
                    ) AS NM
                    ON SW.企業ＣＤ = NM.企業ＣＤ
                    AND SW.部署ＣＤ = NM.部署ＣＤ
                    AND SW.集計ＣＤ = NM.集計ＣＤ
                    AND SW.入金方法 = NM.入金方法
            WHERE
                SW.企業ＣＤ = @企業ＣＤ
                AND SW.集計対象区分 = @集計対象区分入金
                AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 1    -- 得意先マスタの集計CDと結合する
                AND NM.入金日 IS NOT NULL
            ) AS A1
            ON A2.部署ＣＤ = A1.部署ＣＤ
            AND A2.伝票日付 = A1.伝票日付
            AND A2.仕訳ＣＤ = A1.仕訳ＣＤ
    WHERE
        A1.仕訳ＣＤ IS NULL
        AND A2.金額 <> 0
    ) AS A
    INNER JOIN 仕訳マスタ AS SW
        ON SW.企業ＣＤ = @企業ＣＤ
        AND A.部署ＣＤ = SW.部署ＣＤ
        AND A.仕訳ＣＤ = SW.仕訳ＣＤ
ORDER BY
    SW.部署ＣＤ
    ,A.伝票日付
    ,SW.仕訳ＣＤ



@企業ＣＤ = 40001
@出力対象日 = '2017/03/15'
@集計用出力対象日 = '2017/03/15'
@集計対象区分売上 = 0
@出力対象月一日 = '2017/03/01'
@集計対象区分入金 = 1
