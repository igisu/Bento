-- コース出荷状態
SELECT
    A.配送日
    ,A.部署ＣＤ
    ,A.コースＣＤ
    ,A.コース名
    ,A.出荷数
    ,B.担当者ＣＤ
    ,B.担当者名
    ,B.出荷完了日時
    ,B.個数
FROM
    -- 駆動表兼出荷数（個口数の合計）
    (SELECT
        SM.企業ＣＤ
        ,SM.配送日
        ,SM.部署ＣＤ
        ,SM.コースＣＤ
        ,SM.コース名
        ,SUM(SM.個口数) AS 出荷数
    FROM
        -- 指定した配送日・コースで、同じ得意先・食事区分の最新のレコードを抽出する
        -- ※短冊を複数回出力することがあるので、最新のレコードを優先する必要がある
        (SELECT
            MAX(出荷番号) AS 出荷番号
            ,企業ＣＤ
            ,部署ＣＤ
            ,得意先ＣＤ
            ,食事区分
        FROM
            SP_T_出荷明細
        WHERE
            企業ＣＤ = @企業ＣＤ
            AND 配送日 >= @配送日From
            AND 配送日 <= @配送日To
            AND 部署ＣＤ = @部署ＣＤ
            AND コースＣＤ = @コースＣＤ
        GROUP BY
            企業ＣＤ
            ,部署ＣＤ
            ,得意先ＣＤ
            ,食事区分
        ) AS M1
        INNER JOIN SP_T_出荷明細 SM ON SM.企業ＣＤ = M1.企業ＣＤ AND SM.出荷番号 = M1.出荷番号
    GROUP BY
        SM.企業ＣＤ
        ,SM.配送日
        ,SM.部署ＣＤ
        ,SM.コースＣＤ
        ,SM.コース名
    ) AS A
    LEFT JOIN
        -- 出荷履歴
        (SELECT
            SH.企業ＣＤ
        --    ,SH.出荷番号
            ,SM.配送日
            ,SM.コースＣＤ
            ,SM.コース名
            ,SH.担当者ＣＤ
            ,T.担当者名
            ,MAX(SH.出荷完了日時) AS 出荷完了日時
            ,COUNT(SH.出荷個口番号) AS 個数
        FROM
            -- 指定した配送日・コースで、同じ得意先・食事区分の最新のレコードを抽出する
            -- ※短冊を複数回出力することがあるので、最新のレコードを優先する必要がある
            (SELECT
                MAX(出荷番号) AS 出荷番号
                ,企業ＣＤ
                ,部署ＣＤ
                ,得意先ＣＤ
                ,食事区分
            FROM
                SP_T_出荷明細
            WHERE
                企業ＣＤ = @企業ＣＤ
                AND 配送日 >= @配送日From
                AND 配送日 <= @配送日To
                AND 部署ＣＤ = @部署ＣＤ
                AND コースＣＤ = @コースＣＤ
            GROUP BY
                企業ＣＤ
                ,部署ＣＤ
                ,得意先ＣＤ
                ,食事区分
            ) AS M1
            INNER JOIN SP_T_出荷明細 AS SM ON SM.企業ＣＤ = M1.企業ＣＤ AND SM.出荷番号 = M1.出荷番号
            INNER JOIN SP_T_出荷履歴 AS SH ON SH.企業ＣＤ = SM.企業ＣＤ AND SH.出荷番号 = SM.出荷番号
            LEFT JOIN 担当者マスタ AS T ON SH.企業ＣＤ = T.企業ＣＤ AND SH.担当者ＣＤ = T.担当者ＣＤ
        GROUP BY
            SH.企業ＣＤ
        --    ,SH.出荷番号
            ,SM.配送日
            ,SM.コースＣＤ
            ,SM.コース名
            ,SH.担当者ＣＤ
            ,T.担当者名
        ) AS B ON A.企業ＣＤ = B.企業ＣＤ AND A.配送日 = B.配送日 AND A.コースＣＤ = B.コースＣＤ
ORDER BY
    A.配送日
    ,A.コースＣＤ
    ,B.出荷完了日時 DESC


@企業ＣＤ = 40000
@配送日 = '2019/08/05'
@部署ＣＤ = 1
















-- 以下、ボツ
SELECT
    A.企業ＣＤ
    ,A.出荷番号
    ,A.コースＣＤ
    ,A.コース名
    ,A.食事区分
    ,B.出荷数
    ,C.担当者ＣＤ
    ,C.担当者名
    ,C.出荷完了日時
    ,C.個数
FROM
    -- ①駆動表
    (SELECT
        M.企業ＣＤ
        ,M.出荷番号
        ,M.コースＣＤ
        ,M.コース名
        ,M.食事区分
    FROM
        -- 指定した配送日・コースで、同じ得意先・食事区分の最新のレコードを抽出する
        -- ※短冊を複数回出力することがあるので、最新のレコードを優先する必要がある
        (SELECT
            MAX(出荷番号) AS 出荷番号
            ,企業ＣＤ
            ,部署ＣＤ
            ,得意先ＣＤ
            ,食事区分
        FROM
            SP_T_出荷明細
        WHERE
            企業ＣＤ = @企業ＣＤ
            AND 配送日 = @配送日
            AND 部署ＣＤ = @部署ＣＤ
            AND コースＣＤ = @コースＣＤ
        GROUP BY
            企業ＣＤ
            ,部署ＣＤ
            ,得意先ＣＤ
            ,食事区分
        ) AS M1
        INNER JOIN SP_T_出荷明細 M
            ON M.企業ＣＤ = M1.企業ＣＤ
            AND M.出荷番号 = M1.出荷番号
    ) AS A
    -- ②－１ 駆動表に紐付く出荷明細
    LEFT JOIN
        (SELECT
            SM.企業ＣＤ
            ,SM.出荷番号
            ,SM.コースＣＤ
            ,SM.コース名
            ,SM.コース順
            ,SM.食事区分
            ,SUM(SM.個口数) AS 出荷数
        FROM
            SP_T_出荷明細 AS SM
        GROUP BY
            SM.企業ＣＤ
            ,SM.出荷番号
            ,SM.コースＣＤ
            ,SM.コース名
            ,SM.コース順
            ,SM.食事区分
        ) AS B
            ON A.企業ＣＤ = B.企業ＣＤ
            AND A.出荷番号 = B.出荷番号
    -- ②－２ 駆動表に紐付く出荷履歴
    LEFT JOIN
        (SELECT
            SH.企業ＣＤ
            ,SH.出荷番号
            ,SM.コースＣＤ
            ,SM.コース名
            ,SM.コース順
            ,SM.食事区分
            ,SH.担当者ＣＤ
            ,T.担当者名
            ,MAX(SH.出荷完了日時) AS 出荷完了日時
            ,COUNT(SH.出荷個口番号) AS 個数
        FROM
            SP_T_出荷履歴 AS SH
            INNER JOIN SP_T_出荷明細 AS SM
                ON SH.企業ＣＤ = SM.企業ＣＤ
                AND SH.出荷番号 = SM.出荷番号
            LEFT JOIN 担当者マスタ AS T ON SH.企業ＣＤ = T.企業ＣＤ AND SH.担当者ＣＤ = T.担当者ＣＤ
        GROUP BY
            SH.企業ＣＤ
            ,SH.出荷番号
            ,SM.コースＣＤ
            ,SM.コース名
            ,SM.コース順
            ,SM.食事区分
            ,SH.担当者ＣＤ
            ,T.担当者名
        ) AS C
            ON A.企業ＣＤ = C.企業ＣＤ
            AND A.出荷番号 = C.出荷番号
ORDER BY
    A.出荷番号
    ,A.コースＣＤ
    ,A.食事区分
