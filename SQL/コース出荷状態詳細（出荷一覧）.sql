-- コース出荷状態詳細（出荷一覧）
SELECT
    A.企業ＣＤ
    ,A.出荷番号
    ,A.配送日
    ,A.部署ＣＤ
    ,A.得意先ＣＤ
    ,A.得意先名
    ,A.食事区分
    ,A.集計印刷名
    ,B.積載時刻
    ,A.シール食数
    ,C.現在数量
    ,A.袋数表記
FROM
    -- 駆動表兼出荷明細
    (SELECT
        SM.企業ＣＤ
        ,SM.出荷番号
        ,SM.配送日
        ,SM.部署ＣＤ
        ,SM.得意先ＣＤ
        ,SM.得意先名
        ,SM.食事区分
        ,TSM.集計印刷名
        ,TSM.印刷順
        ,SM.集計注文数 AS シール食数
        ,SM.袋数表記
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
        INNER JOIN SP_T_出荷明細 SM ON SM.企業ＣＤ = M1.企業ＣＤ AND SM.出荷番号 = M1.出荷番号
        LEFT JOIN 短冊集計設定マスタ AS TSM
            ON TSM.企業ＣＤ  = SM.企業ＣＤ
            AND TSM.部署ＣＤ = SM.部署ＣＤ
            AND TSM.食事区分ＣＤ = SM.食事区分
    ) AS A
    -- 出荷履歴
    LEFT JOIN
        (SELECT
            SH.企業ＣＤ
            ,SH.出荷番号
            ,MAX(SH.出荷日時) AS 積載時刻
        FROM
            SP_T_出荷履歴 AS SH
        WHERE
            SH.担当者ＣＤ = @担当者ＣＤ
        GROUP BY
            SH.企業ＣＤ
            ,SH.出荷番号
        ) AS B
            ON A.企業ＣＤ = B.企業ＣＤ
            AND A.出荷番号 = B.出荷番号
    -- 注文数
    LEFT JOIN
        (SELECT
            H.企業ＣＤ
            ,H.部署ＣＤ
            ,H.配送日
            ,H.得意先ＣＤ
            ,SM.食事区分
            ,SUM(M.商品数量) AS 現在数量
        FROM
            注文ヘッダ AS H
            INNER JOIN 注文明細 AS M
                ON  H.企業ＣＤ   = M.企業ＣＤ
                AND H.注文区分   = M.注文区分
                AND H.喫食日     = M.喫食日
                AND H.得意先ＣＤ = M.得意先ＣＤ
                AND H.部署ＣＤ   = M.部署ＣＤ
            INNER JOIN 商品マスタ AS SM
                ON SM.企業ＣＤ  = M.企業ＣＤ
                AND SM.商品ＣＤ = M.商品ＣＤ
        WHERE
            H.注文区分 = @注文区分注文
        --    AND H.企業ＣＤ = @企業ＣＤ
        --    AND H.部署ＣＤ = @部署ＣＤ
        --    AND H.配送日 = @配送日
        GROUP BY
            H.企業ＣＤ
            ,H.部署ＣＤ
            ,H.配送日
            ,H.得意先ＣＤ
            ,SM.食事区分
        ) AS C
            ON A.企業ＣＤ = C.企業ＣＤ
            AND A.部署ＣＤ = C.部署ＣＤ
            AND A.配送日 = C.配送日
            AND A.得意先ＣＤ = C.得意先ＣＤ
            AND A.食事区分 = C.食事区分
ORDER BY
    A.得意先ＣＤ
    ,A.印刷順

@企業ＣＤ = 40000
@配送日 = '2019/08/05'
@部署ＣＤ = 1
@コースＣＤ = 102
@注文区分注文 = 0
@担当者ＣＤ = 11








-- 以下、元ネタ




    -- 駆動表兼出荷明細
    (SELECT
        SM.企業ＣＤ
        ,SM.出荷番号
        ,SM.配送日
        ,SM.部署ＣＤ
        ,SM.得意先ＣＤ
        ,SM.得意先名
        ,SM.食事区分
        ,TSM.集計印刷名
        ,SM.集計注文数 AS シール食数
        ,SM.袋数表記
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
        INNER JOIN SP_T_出荷明細 SM ON SM.企業ＣＤ = M1.企業ＣＤ AND SM.出荷番号 = M1.出荷番号
        LEFT JOIN 短冊集計設定マスタ AS TSM
            ON TSM.企業ＣＤ  = SM.企業ＣＤ
            AND TSM.部署ＣＤ = SM.部署ＣＤ
            AND TSM.食事区分ＣＤ = SM.食事区分
    ) AS A
ORDER BY
    SM.得意先ＣＤ
    ,TSM.印刷順





-- 出荷履歴
SELECT
    SH.企業ＣＤ
    ,SH.出荷番号
    ,MAX(SH.出荷日時) AS 積載時刻
FROM
    SP_T_出荷履歴 AS SH
WHERE
    SH.担当者ＣＤ = @担当者ＣＤ
GROUP BY
    SH.企業ＣＤ
    ,SH.出荷番号




-- 注文数
SELECT
    H.企業ＣＤ
    ,H.配送日
    ,H.得意先ＣＤ
    ,SM.食事区分
    ,SUM(M.商品数量) as 数量
FROM
    注文ヘッダ AS H
    INNER JOIN 注文明細 AS M
        ON  H.企業ＣＤ   = M.企業ＣＤ
        AND H.注文区分   = M.注文区分
        AND H.喫食日     = M.喫食日
        AND H.得意先ＣＤ = M.得意先ＣＤ
        AND H.部署ＣＤ   = M.部署ＣＤ
    INNER JOIN 商品マスタ AS SM
        ON SM.企業ＣＤ  = M.企業ＣＤ
        AND SM.商品ＣＤ = M.商品ＣＤ
WHERE
    H.注文区分 = @注文区分注文
--    AND H.企業ＣＤ = @企業ＣＤ
--    AND H.部署ＣＤ = @部署ＣＤ
--    AND H.配送日 = @配送日
GROUP BY
    H.企業ＣＤ
    ,H.配送日
    ,H.得意先ＣＤ
    ,SM.食事区分

