-- 売上精算入力_回収現金
-- 入金入力区分=1＆入金方法=現金の入金明細の入金額
SELECT
    N.得意先ＣＤ
    ,T.得意先名
    ,ISNULL(SUM(N.入金額), 0) AS 入金額
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
    INNER JOIN 入金明細 AS N ON N.企業ＣＤ = JT.企業ＣＤ AND N.得意先ＣＤ = JT.得意先ＣＤ
    LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = N.企業ＣＤ AND T.得意先ＣＤ = N.得意先ＣＤ
WHERE
    N.入金日 = @精算日
    AND N.部署ＣＤ = @部署ＣＤ
    AND N.入金入力区分 = @入金入力区分モバイル  -- モバイル
    AND N.入金方法 = @入金方法現金      -- 現金
    AND ISNULL(N.入金額, 0) <> 0
GROUP BY
    N.得意先ＣＤ
    ,T.得意先名
ORDER BY
    N.得意先ＣＤ
