-- 短冊印刷
SELECT
    MAIN.喫食日
    ,MAIN.部署ＣＤ
    ,MAIN.配送日
    ,MAIN.得意先ＣＤ
    ,ISNULL(T.得意先名略称, MAIN.得意先名) AS 得意先名
    -- 喫食日、得意先、集計区分（＝食事区分）ごとの食数をスペースで区切って取得
    ,REPLACE('START'+ISNULL(
        (SELECT
            '    ' + TSM.集計印刷名 + ' ' + CAST(ISNULL(SUM(A.商品数量), 0) AS VARCHAR)
        FROM
            -- 袋計算フラグが立っている集計区分(=食事区分)は全て取得したいので、短冊集計設定マスタを駆動表にして、注文を外部結合する
            短冊集計設定マスタ AS TSM
            LEFT JOIN
                (SELECT
                    H.企業ＣＤ
                    ,H.部署ＣＤ
                    ,H.得意先ＣＤ
                    ,SM.食事区分
                    ,M.商品数量
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
                    H.企業ＣＤ = MAIN.企業ＣＤ
                    AND H.注文区分 = MAIN.注文区分
                    AND H.喫食日 = MAIN.喫食日
                    AND H.得意先ＣＤ = MAIN.得意先ＣＤ
                    AND H.部署ＣＤ = MAIN.部署ＣＤ
                ) AS A
                ON TSM.企業ＣＤ  = A.企業ＣＤ
                AND TSM.部署ＣＤ = A.部署ＣＤ
                AND TSM.食事区分ＣＤ = A.食事区分
        WHERE
            TSM.企業ＣＤ = MAIN.企業ＣＤ
            AND TSM.部署ＣＤ = MAIN.部署ＣＤ
            AND ISNULL(TSM.袋計算フラグ, 0) <> 0
            --AND ISNULL(A.商品数量, 0) <> 0            -- グループ食数表記で0注文を印字しない場合は有効にする
        GROUP BY
            A.得意先ＣＤ
            ,TSM.集計区分ＣＤ
            ,TSM.集計印刷名
            ,TSM.印刷順
        ORDER BY
            TSM.印刷順
        FOR XML PATH ('')
        ),'    '),'START    ','') AS 食事区分別食数
    ,REPLACE('START'+ISNULL(
        (SELECT
            '    ' + TSM.集計印刷名 + ' ' + CAST(ISNULL(SUM(A.商品数量), 0) AS VARCHAR)
        FROM
            -- 袋計算フラグが立っている集計区分(=食事区分)は全て取得したいので、短冊集計設定マスタを駆動表にして、注文を外部結合する
            短冊集計設定マスタ AS TSM
            LEFT JOIN
                (SELECT
                    H.企業ＣＤ
                    ,H.部署ＣＤ
                    ,H.得意先ＣＤ
                    ,SM.食事区分
                    ,M.商品数量
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
                    H.企業ＣＤ = MAIN.企業ＣＤ
                    AND H.注文区分 = MAIN.注文区分
                    AND H.喫食日 = MAIN.喫食日
                    AND H.得意先ＣＤ = MAIN.得意先ＣＤ
                    AND H.部署ＣＤ = MAIN.部署ＣＤ
                ) AS A
                ON TSM.企業ＣＤ  = A.企業ＣＤ
                AND TSM.部署ＣＤ = A.部署ＣＤ
                AND TSM.食事区分ＣＤ = A.食事区分
        WHERE
            TSM.企業ＣＤ = MAIN.企業ＣＤ
            AND TSM.部署ＣＤ = MAIN.部署ＣＤ
            AND ISNULL(TSM.袋計算フラグ, 0) <> 0
        GROUP BY
            A.得意先ＣＤ
            ,TSM.集計区分ＣＤ
            ,TSM.集計印刷名
            ,TSM.印刷順
        ORDER BY
            TSM.印刷順
        FOR XML PATH ('')
        ),'    '),'START    ','') AS 比較保存用食事区分別食数
    ,MAIN.コースＣＤ
    ,ISNULL(TC.短冊印刷コース名, CM.コース名) AS コース名
    ,CO.コース順
    ,MAIN.集計印刷名
    ,MAIN.数量
    ,MAIN.袋計算フラグ
    ,MAIN.食事区分
    ,MAIN.集計印刷名 + '  ' + cast(MAIN.数量 as varchar) + ' 食' as 食数
    ,cast(NULL AS varchar) AS 出荷番号
FROM
    -- 駆動表（兼、集計区分(=食事区分)ごとの食数）
    (SELECT
        H.企業ＣＤ
        ,H.喫食日
        ,H.部署ＣＤ
        ,H.配送日
        ,M.注文区分
        ,H.コースＣＤ
        ,H.得意先ＣＤ
        ,H.得意先名
        ,SM.食事区分
        ,TSM.集計区分ＣＤ
        ,TSM.集計印刷名
        ,TSM.印刷順
        ,TSM.袋計算フラグ
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
        INNER JOIN 短冊集計設定マスタ AS TSM
            ON TSM.企業ＣＤ  = SM.企業ＣＤ
            AND TSM.部署ＣＤ = @部署ＣＤ
            AND TSM.食事区分ＣＤ = SM.食事区分
    WHERE
        H.企業ＣＤ = @企業ＣＤ
        AND ISNULL(M.商品数量, 0) <> 0          -- 0注文はシールを印刷しない
        AND H.喫食日 = @注文日                  -- 注文日と配送日はどちらかのみ指定
        AND H.配送日 = @配送日                  -- 注文日と配送日はどちらかのみ指定
        AND H.部署ＣＤ = @部署ＣＤ
        AND H.注文区分 = @注文区分注文
        AND H.コースＣＤ IS NOT NULL            -- コースに属していない得意先の短冊は出力しなくてよい
        AND H.コースＣＤ = @コースＣＤ
        AND H.得意先ＣＤ = @得意先ＣＤ
        -- 印刷順に該当する食事区分CDを検索条件にする
        AND SM.食事区分 IN (SELECT
                                食事区分ＣＤ
                            FROM
                                短冊集計設定マスタ
                            WHERE
                                企業ＣＤ = @企業ＣＤ
                                AND 部署ＣＤ = @部署ＣＤ
                                AND 印刷順 >= @印刷順From
                                AND 印刷順 <= @印刷順To
                           )
    GROUP BY
        H.企業ＣＤ
        ,H.喫食日
        ,H.部署ＣＤ
        ,H.配送日
        ,M.注文区分
        ,H.コースＣＤ
        ,H.得意先ＣＤ
        ,H.得意先名
        ,SM.食事区分
        ,TSM.集計区分ＣＤ
        ,TSM.集計印刷名
        ,TSM.印刷順
        ,TSM.袋計算フラグ
    ) AS MAIN
    LEFT JOIN コースマスタ AS CM ON CM.企業ＣＤ = MAIN.企業ＣＤ AND CM.コースＣＤ = MAIN.コースＣＤ
    LEFT JOIN 短冊コース名マスタ AS TC ON TC.企業ＣＤ = MAIN.企業ＣＤ AND TC.コースＣＤ = MAIN.コースＣＤ
    LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = MAIN.企業ＣＤ AND T.得意先ＣＤ = MAIN.得意先ＣＤ
-- コース順を取得
    LEFT JOIN
        (SELECT
            CT.得意先ＣＤ
            ,CT.コースＣＤ
            ,CT.コース順
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
                    AND 実施日 = @実施日        -- 注文日か配送日
                ) AS RM
                ON RM.コースＣＤ = CT.コースＣＤ
        WHERE
            CT.企業ＣＤ = @企業ＣＤ
            AND ISNULL(CT.無効フラグ, 0) = 0
            AND RM.コースＣＤ IS NULL
        -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
        UNION
        SELECT
            RT.得意先ＣＤ
            ,RT.コースＣＤ
            ,RT.コース順
        FROM
            臨時コーステーブル AS RT
            INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0
        WHERE
            RT.企業ＣＤ = @企業ＣＤ
            AND ISNULL(RT.無効フラグ, 0) = 0
            AND RT.実施日 = @実施日        -- 注文日か配送日
        ) AS CO ON MAIN.得意先ＣＤ = CO.得意先ＣＤ AND MAIN.コースＣＤ = CO.コースＣＤ
ORDER BY
    MAIN.印刷順
    ,CO.コースＣＤ
    ,CO.コース順
    ,MAIN.得意先ＣＤ



@企業ＣＤ = 40000
@部署ＣＤ = 1
@コースＣＤ = 1
@得意先ＣＤ = 101
@注文日 = '2018/04/10'
@配送日 = '2018/04/10'
@注文区分注文 = 0
@実施日=  '2018/04/10'
