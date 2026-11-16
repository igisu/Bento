-- 月次得意先別実績一覧
SELECT
    0 AS 明細合計区分
    ,C.コースＣＤ
    ,C.コース名
    ,A.得意先ＣＤ
    ,T.得意先名
    ,A.商品ＣＤ
    ,A.値
FROM
    -- 個数
    (SELECT
        U.企業ＣＤ
        ,U.部署ＣＤ
        ,U.得意先ＣＤ
        ,J.得意先ＣＤ AS 受注得意先ＣＤ
        ,ISNULL(CONVERT(varchar, L.商品ＣＤ), 'XXX') + '_0' AS 商品ＣＤ
        ,SUM(U.今月数量) AS 値
    FROM
        月次売上集計 AS U
        LEFT JOIN 主要商品リストマスタ AS L
            ON U.企業ＣＤ = L.企業ＣＤ
            AND U.商品ＣＤ = L.商品ＣＤ
            AND U.部署ＣＤ = L.部署ＣＤ
            AND ISNULL(L.無効フラグ, 0) = 0
        -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
        INNER JOIN 得意先マスタ AS T ON T.企業ＣＤ = U.企業ＣＤ AND T.得意先ＣＤ = U.得意先ＣＤ
        INNER JOIN 得意先マスタ AS J ON J.企業ＣＤ = T.企業ＣＤ AND J.得意先ＣＤ = ISNULL(T.受注得意先ＣＤ, T.得意先ＣＤ)
    WHERE
        U.企業ＣＤ = @企業ＣＤ
        AND U.集計年 = @集計年
        AND U.集計月 = @集計月
        AND U.部署ＣＤ = @部署ＣＤ
        AND U.得意先ＣＤ = @得意先ＣＤ
    GROUP BY
        U.企業ＣＤ
        ,U.部署ＣＤ
        ,U.得意先ＣＤ
        ,J.得意先ＣＤ
        ,L.商品ＣＤ
    UNION
    -- 金額
    SELECT
        U.企業ＣＤ
        ,U.部署ＣＤ
        ,U.得意先ＣＤ
        ,J.得意先ＣＤ AS 受注得意先ＣＤ
        ,ISNULL(CONVERT(varchar, L.商品ＣＤ), 'XXX') + '_1' AS 商品ＣＤ
        ,SUM(U.今月売上額) AS 値
    FROM
        月次売上集計 AS U
        LEFT JOIN 主要商品リストマスタ AS L
            ON U.企業ＣＤ = L.企業ＣＤ
            AND U.商品ＣＤ = L.商品ＣＤ
            AND U.部署ＣＤ = L.部署ＣＤ
            AND ISNULL(L.無効フラグ, 0) = 0
        -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
        INNER JOIN 得意先マスタ AS T ON T.企業ＣＤ = U.企業ＣＤ AND T.得意先ＣＤ = U.得意先ＣＤ
        INNER JOIN 得意先マスタ AS J ON J.企業ＣＤ = T.企業ＣＤ AND J.得意先ＣＤ = ISNULL(T.受注得意先ＣＤ, T.得意先ＣＤ)
    WHERE
        U.企業ＣＤ = @企業ＣＤ
        AND U.集計年 = @集計年
        AND U.集計月 = @集計月
        AND U.部署ＣＤ = @部署ＣＤ
        AND U.得意先ＣＤ = @得意先ＣＤ
    GROUP BY
        U.企業ＣＤ
        ,U.部署ＣＤ
        ,U.得意先ＣＤ
        ,J.得意先ＣＤ
        ,L.商品ＣＤ
    UNION
    -- 値引額
    SELECT
        U.企業ＣＤ
        ,U.部署ＣＤ
        ,U.得意先ＣＤ
        ,J.得意先ＣＤ AS 受注得意先ＣＤ
        ,'YYY' + '_1' AS 商品ＣＤ
        ,SUM(ISNULL(U.今月値引額, 0)) AS 値
    FROM
        月次売上集計 AS U
        -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
        INNER JOIN 得意先マスタ AS T ON T.企業ＣＤ = U.企業ＣＤ AND T.得意先ＣＤ = U.得意先ＣＤ
        INNER JOIN 得意先マスタ AS J ON J.企業ＣＤ = T.企業ＣＤ AND J.得意先ＣＤ = ISNULL(T.受注得意先ＣＤ, T.得意先ＣＤ)
    WHERE
        U.企業ＣＤ = @企業ＣＤ
        AND U.集計年 = @集計年
        AND U.集計月 = @集計月
        AND U.部署ＣＤ = @部署ＣＤ
        AND U.得意先ＣＤ = @得意先ＣＤ
    GROUP BY
        U.企業ＣＤ
        ,U.部署ＣＤ
        ,U.得意先ＣＤ
        ,J.得意先ＣＤ
    ) AS A

    -- コースによる絞り込み用
    -- 同じ得意先に複数のコースが設定されている場合、そのいずれのコースを条件に指定しても、そのコースにぶら下がる得意先がヒットしないといけない。
    INNER JOIN
        (SELECT DISTINCT
            CM.企業ＣＤ
            ,CM.部署ＣＤ
            ,CM.コース名
            ,CM.コースＣＤ
            ,CT.得意先ＣＤ
            ,CT.コース順
        FROM
            コースマスタ AS CM
            INNER JOIN コーステーブル AS CT ON
                CM.企業ＣＤ = CT.企業ＣＤ
                AND CM.コースＣＤ =  CT.コースＣＤ
        WHERE
            CT.コースＣＤ = @コースＣＤ
        ) AS C
        ON A.企業ＣＤ = C.企業ＣＤ
        AND A.部署ＣＤ = C.部署ＣＤ     -- 月次売上集計の部署と一致するものに限る
        AND A.受注得意先ＣＤ = C.得意先ＣＤ

    -- 検索条件にコースが指定されていない場合は、ヒットした得意先が持つコースのうち最小値を取得する
    -- （コース注文一覧／コース売上一覧とは異なり、コースが指定されていない＝コースがnullの注文/売上 ではなく、コースを条件に加えない ということ）
    LEFT JOIN
        (SELECT
            CM.企業ＣＤ
            ,CM.部署ＣＤ
            ,CM.コース名
            ,CT1.コースＣＤ
            ,CT1.得意先ＣＤ
            ,CT1.コース順
        FROM
            コーステーブル AS CT1
            INNER JOIN
                (SELECT
                    M.企業ＣＤ
                    ,M.部署ＣＤ
                    ,T.得意先ＣＤ
                    ,MIN(T.コースＣＤ) AS コースＣＤ
                FROM
                    コースマスタ AS M
                    LEFT JOIN コーステーブル AS T ON
                        T.企業ＣＤ = M.企業ＣＤ
                        AND T.コースＣＤ = M.コースＣＤ
                GROUP BY
                    M.企業ＣＤ
                    ,M.部署ＣＤ
                    ,T.得意先ＣＤ
                ) AS CT2
                    ON CT2.企業ＣＤ = CT1.企業ＣＤ
                    AND CT2.得意先ＣＤ = CT1.得意先ＣＤ
                    AND CT2.コースＣＤ = CT1.コースＣＤ
            INNER JOIN コースマスタ AS CM
                ON CM.企業ＣＤ = CT1.企業ＣＤ
                AND CM.コースＣＤ =  CT1.コースＣＤ
        ) AS C
        ON A.企業ＣＤ = C.企業ＣＤ
        AND A.部署ＣＤ = C.部署ＣＤ     -- 月次売上集計の部署と一致するものに限る
        AND A.受注得意先ＣＤ = C.得意先ＣＤ


    LEFT JOIN 得意先マスタ AS T
        ON T.企業ＣＤ = A.企業ＣＤ
        AND T.得意先ＣＤ = A.得意先ＣＤ
UNION
-- 合計
SELECT
    1 AS 明細合計区分
    ,NULL AS コースＣＤ
    ,'' AS コース名
    ,NULL AS 得意先ＣＤ
    ,'合計' AS 得意先名
    ,NULL AS 商品ＣＤ
    ,NULL AS 値
ORDER BY
    明細合計区分
    ,コースＣＤ
    ,得意先ＣＤ


@企業ＣＤ = 40001
@集計年 = 2017
@集計月 = 3
@部署ＣＤ = 1
@得意先ＣＤ = 3796
@コースＣＤ = 1
