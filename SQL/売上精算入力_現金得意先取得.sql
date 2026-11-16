-- 売上精算入力_現金得意先取得
-- コースにぶら下がる、売掛現金区分=現金の得意先を抽出
SELECT
    A.得意先ＣＤ
FROM
    -- 駆動表① 売上が存在する得意先
    (SELECT
        U1.得意先ＣＤ
    FROM
        売上ヘッダ AS U1
        INNER JOIN 売上明細 AS UM1
            ON UM1.企業ＣＤ = U1.企業ＣＤ
            AND UM1.売上日 = U1.売上日
            AND UM1.得意先ＣＤ = U1.得意先ＣＤ
            AND UM1.部署ＣＤ = U1.部署ＣＤ
        LEFT JOIN
            -- 前回締日
            (SELECT
                H1.得意先ＣＤ
                ,H1.部署ＣＤ
                ,MAX(H1.請求日) AS 前回締日
            FROM
                請求ヘッダ AS H1
                INNER JOIN 請求合計 AS G1 ON H1.企業ＣＤ = G1.企業ＣＤ AND H1.請求番号 = G1.請求番号
            WHERE
                H1.企業ＣＤ = @企業ＣＤ
                AND H1.請求日 < @精算日                 -- 今回締日（入力された請求日）
            GROUP BY
                H1.得意先ＣＤ
                ,H1.部署ＣＤ
            ) AS M11 ON
                M11.得意先ＣＤ = U1.得意先ＣＤ
                AND M11.部署ＣＤ = U1.部署ＣＤ
    WHERE
        U1.企業ＣＤ = @企業ＣＤ
        -- 前回請求が有るものは前回請求日以降
        -- 無ければ過去の売上全て
        -- ※請求締処理の対象ということは必ず掛売になっているはずだが、売上が無かったときのガードの日付を設けておく
        AND U1.売上日 > ISNULL(M11.前回締日, '2000/01/01')
        AND U1.売上日 <= @精算日           -- 今回締日（入力された請求日）
        AND UM1.売上区分 = @売上区分売掛        -- 売掛
    UNION
    -- 駆動表② 入金が存在する得意先
    SELECT
        N1.得意先ＣＤ
    FROM
        入金明細 AS N1
        LEFT JOIN
            -- 前回締日
            (SELECT
                G2.請求先ＣＤ
                ,G2.部署ＣＤ
                ,MAX(G2.請求日) AS 前回締日
            FROM
                請求ヘッダ AS H2
                INNER JOIN 請求合計 AS G2 ON H2.企業ＣＤ = G2.企業ＣＤ AND H2.請求番号 = G2.請求番号
            WHERE
                H2.企業ＣＤ = @企業ＣＤ
                AND H2.請求日 < @精算日                 -- 今回締日（入力された請求日）
            GROUP BY
                G2.請求先ＣＤ
                ,G2.部署ＣＤ
            ) AS M21 ON
                M21.請求先ＣＤ = N1.得意先ＣＤ
                AND M21.部署ＣＤ = N1.部署ＣＤ
    WHERE
        N1.企業ＣＤ = @企業ＣＤ
        AND N1.入金日 > ISNULL(M21.前回締日, '2000/01/01')
        AND N1.入金日 <= @精算日                -- 今回締日（入力された請求日）
    ) AS A
INNER JOIN
    (SELECT
        T.得意先ＣＤ
    FROM
        -- コースに紐付く得意先の入金を取得する
        -- 分配した場合は、基本的に分配先はコースに属していないので、コースに属している分配元(=受注得意先)を経由して入金を取得する
        (SELECT DISTINCT
            T.企業ＣＤ
            ,T.得意先ＣＤ
        FROM
            (SELECT
                CT.企業ＣＤ
                ,CT.得意先ＣＤ
            FROM
                コーステーブル AS CT
                INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0
                LEFT JOIN
                    (SELECT
                        コースＣＤ
                    FROM
                        臨時コースマスタ
                    WHERE
                        企業ＣＤ = @企業ＣＤ
                        AND ISNULL(無効フラグ, 0) = 0
                        AND コースＣＤ = @コースＣＤ
                        AND 実施日 = @精算日
                    ) AS RM
                    ON RM.コースＣＤ = CT.コースＣＤ
            WHERE
                CT.企業ＣＤ = @企業ＣＤ
                AND CT.コースＣＤ = @コースＣＤ
                AND ISNULL(CT.無効フラグ, 0) = 0
                AND RM.コースＣＤ IS NULL
            -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
            UNION
            SELECT
                RT.企業ＣＤ
                ,RT.得意先ＣＤ
            FROM
                臨時コーステーブル AS RT
                INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0
            WHERE
                RT.企業ＣＤ = @企業ＣＤ
                AND RT.コースＣＤ = @コースＣＤ
                AND ISNULL(RT.無効フラグ, 0) = 0
                AND RT.実施日 = @精算日
            ) AS CT
            INNER JOIN 得意先マスタ AS T ON T.企業ＣＤ = CT.企業ＣＤ AND ISNULL(T.受注得意先ＣＤ, T.得意先ＣＤ) = CT.得意先ＣＤ
        ) AS JT
        INNER JOIN 得意先マスタ AS T
            ON  T.企業ＣＤ = JT.企業ＣＤ
            AND T.得意先ＣＤ = JT.得意先ＣＤ
    WHERE
        T.売掛現金区分 = @売掛現金区分現金
    ) AS B
        ON A.得意先ＣＤ = B.得意先ＣＤ
ORDER BY
    A.得意先ＣＤ



@売掛現金区分現金 = 0
