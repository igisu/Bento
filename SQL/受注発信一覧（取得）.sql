-- 受注発信一覧（取得）
SELECT
    H.企業ＣＤ
    ,CT.コースＣＤ
    ,H.得意先ＣＤ
    ,T.得意先名
    ,H.電話番号
    ,CASE WHEN CH.配送日 IS NULL THEN '未' ELSE '済' END AS 注文状況
    ,CASE WHEN UH.売上日 IS NULL THEN '未' ELSE '済' END AS 配送状況
    ,VC.コース区分名
    ,CM.コース名
    ,CM.部署ＣＤ
    ,T.電話確認時間_時
    ,T.電話確認時間_分
    ,RIGHT('00' + CONVERT(varchar, T.電話確認時間_時), 2) + ':' + RIGHT('00' + CONVERT(varchar, T.電話確認時間_分), 2) AS 電話確認時間
    ,H.発信日
    ,H.最終発信日時
    ,H.担当者ＣＤ
    ,H.処理中フラグ
    ,H.不要フラグ
    ,H.保留フラグ
    ,H.コメント
    ,H.電話番号備考
    ,H.作成担当者ＣＤ
    ,H.作成日
    ,H.修正担当者ＣＤ
    ,H.修正日
FROM
    -- ①得意先発信履歴データは得意先ＣＤ、担当者架電コースマスタはコースＣＤしか持たないので、完全外部結合して②と内部結合する
    (SELECT
        H.企業ＣＤ
        ,H.得意先ＣＤ
        ,H.発信日
        ,H.電話番号
        ,H.最終発信日時
        ,H.担当者ＣＤ
        ,H.処理中フラグ
        ,H.不要フラグ
        ,H.保留フラグ
        ,H.コメント
        ,H.電話番号備考
        ,H.作成担当者ＣＤ
        ,H.作成日
        ,H.修正担当者ＣＤ
        ,H.修正日
        ,K.コースＣＤ
    FROM
        得意先発信履歴データ AS H
        LEFT JOIN 担当者架電コースマスタ AS K ON K.企業ＣＤ = H.企業ＣＤ
    WHERE
        H.企業ＣＤ = @企業ＣＤ
        AND H.発信日 = @注文日
        AND K.担当者ＣＤ = 1
    ) AS H
    -- ②コースＣＤと得意先ＣＤの組み合わせ
    INNER JOIN
        -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
        (SELECT
            C.企業ＣＤ
            ,C.コースＣＤ
            ,C.得意先ＣＤ
            ,C.コース順
        FROM
            コーステーブル AS C
            INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = C.企業ＣＤ AND CM.コースＣＤ = C.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0
            LEFT JOIN
                (SELECT
                    コースＣＤ
                FROM
                    臨時コースマスタ
                WHERE
                    企業ＣＤ = @企業ＣＤ
                    AND ISNULL(無効フラグ, 0) = 0
                    AND 実施日 = @注文日
                    AND コース区分 = @コース区分
                ) AS RM
                ON RM.コースＣＤ = C.コースＣＤ
        WHERE
            C.企業ＣＤ = @企業ＣＤ
            AND ISNULL(C.無効フラグ, 0) = 0
            AND CM.コース区分 = @コース区分
            AND RM.コースＣＤ IS NULL
        -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
        UNION
        SELECT
            RT.企業ＣＤ
            ,RT.コースＣＤ
            ,RT.得意先ＣＤ
            ,RT.コース順
        FROM
            臨時コーステーブル AS RT
            INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0
        WHERE
            RT.企業ＣＤ = @企業ＣＤ
            AND ISNULL(RT.無効フラグ, 0) = 0
            AND RT.実施日 = @注文日
            AND RM.コース区分 = @コース区分
        ) AS CT
        ON CT.コースＣＤ = H.コースＣＤ AND CT.得意先ＣＤ = H.得意先ＣＤ
    LEFT JOIN 得意先マスタ AS T  ON  T.企業ＣＤ = H.企業ＣＤ AND T.得意先ＣＤ = H.得意先ＣＤ
    LEFT JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ
    LEFT JOIN V_コース区分 AS VC ON VC.コース区分CD = CM.コース区分
    LEFT JOIN 注文ヘッダ   AS CH ON CH.企業ＣＤ = H.企業ＣＤ AND CH.注文区分 = @注文区分注文 AND CH.喫食日 = H.発信日 AND CH.得意先ＣＤ = H.得意先ＣＤ AND CH.部署ＣＤ = CM.部署ＣＤ
    LEFT JOIN 売上ヘッダ   AS UH ON UH.企業ＣＤ = H.企業ＣＤ AND UH.得意先ＣＤ = H.得意先ＣＤ AND UH.売上日 = H.発信日 AND UH.部署ＣＤ = CM.部署ＣＤ
WHERE
    1 = 1
--    AND ISNULL(H.不要フラグ, 0) <> 0
--    AND CH.配送日 IS NULL
--    AND UH.売上日 IS NULL
ORDER BY
    T.電話確認時間_時
    ,T.電話確認時間_分
    ,CT.コースＣＤ
    ,CT.コース順
    ,H.電話番号
