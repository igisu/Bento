-- 注文入力_コース状況取得

SELECT
    C.得意先ＣＤ
    ,T.得意先名略称
    ,CASE WHEN UH.得意先ＣＤ IS NULL THEN '' ELSE '○' END AS 売上
    ,CASE WHEN CH.得意先ＣＤ IS NULL THEN '' ELSE '○' END AS 配送
    ,VJ.受注方法区分名
FROM
    (SELECT
        CT.企業ＣＤ
        ,CT.得意先ＣＤ
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
                AND コースＣＤ = @コースＣＤ
                AND 実施日 = @配送日
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
        ,RT.コース順
    FROM
        臨時コーステーブル AS RT
        INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0
    WHERE
        RT.企業ＣＤ = @企業ＣＤ
        AND RT.コースＣＤ = @コースＣＤ
        AND ISNULL(RT.無効フラグ, 0) = 0
        AND RT.実施日 = @配送日
    ) AS C
    LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ
    LEFT JOIN V_受注方法区分 AS VJ ON VJ.受注方法区分CD = T.受注方法
    LEFT JOIN 売上ヘッダ AS UH ON
        UH.企業ＣＤ = C.企業ＣＤ
        AND UH.売上日 = @注文日
        AND UH.得意先ＣＤ = C.得意先ＣＤ
        AND UH.部署ＣＤ = @部署ＣＤ
    LEFT JOIN コース配送履歴 AS CH ON
        CH.企業ＣＤ = C.企業ＣＤ
        AND CH.配送日 = @配送日
        AND CH.得意先ＣＤ = C.得意先ＣＤ
        AND CH.部署ＣＤ = @部署ＣＤ
ORDER BY
    C.コース順


@企業ＣＤ = 40001
@コースＣＤ = 1
@配送日 = '2019/10/15'
@注文日 = '2019/10/15'
@部署ＣＤ = 1
