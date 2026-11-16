-- 受注発信一覧（初期登録）
INSERT INTO 得意先発信履歴データ (
    企業ＣＤ
    ,得意先ＣＤ
    ,発信日
    ,電話番号
    ,最終発信日時
    ,担当者ＣＤ
    ,処理中フラグ
    ,不要フラグ
    ,保留フラグ
    ,コメント
    ,電話番号備考
    ,作成担当者ＣＤ
    ,作成日
    ,修正担当者ＣＤ
    ,修正日
) SELECT DISTINCT
    K.企業ＣＤ,
    T.得意先ＣＤ,
    '2016/06/27' AS 発信日,
    TC.Tel_TelNo AS 電話番号,
    NULL AS 最終発信日時,
    1 AS 担当者ＣＤ,
    0 AS 処理中フラグ,
    ISNULL(CASE WHEN SK.対象日 IS NOT NULL THEN T.祝日配送区分 ELSE SUBSTRING(T.休日設定, DATEPART(WEEKDAY, '2016/06/27'), 1) END, 0) AS 不要フラグ,
    0 AS 保留フラグ,
    NULL AS コメント,
    TC.Tel_Remarks AS 電話番号備考,
    1 AS 作成担当者ＣＤ,
    getdate() AS 作成日,
    1 AS 修正担当者ＣＤ,
    getdate() AS 修正日
FROM
    担当者架電コースマスタ AS K
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
                    INNER JOIN 臨時コースマスタ
                WHERE
                    企業ＣＤ = 1
                    AND ISNULL(無効フラグ, 0) = 0
                    AND 実施日 = '2016/06/27'
                    AND コース区分 = @コース区分
                ) AS RM
                ON RM.コースＣＤ = C.コースＣＤ
        WHERE
            C.企業ＣＤ = 1
            AND ISNULL(C.無効フラグ, 0) = 0
            AND CM.コース区分 = @コース区分
            AND RM.コースＣＤ IS NULL
        -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
        UNION
        SELECT
            R.企業ＣＤ
            ,R.コースＣＤ
            ,R.得意先ＣＤ
            ,R.コース順
        FROM
            臨時コーステーブル AS R
            INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = R.企業ＣＤ AND RM.コースＣＤ = R.コースＣＤ AND R.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0
        WHERE
            R.企業ＣＤ = 1
            AND ISNULL(R.無効フラグ, 0) = 0
            AND R.実施日 = '2016/06/27'
            AND RM.コース区分 = @コース区分
        ) AS CT
        ON CT.コースＣＤ = K.コースＣＤ
    LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = CT.企業ＣＤ AND T.得意先ＣＤ = CT.得意先ＣＤ
    LEFT JOIN C_TelToCust  AS TC ON TC.企業ＣＤ = T.企業ＣＤ AND TC.Tel_CustNo = T.得意先ＣＤ AND TC.Tel_RepFlg = 0
    LEFT JOIN 祝日マスタ AS SK ON SK.企業ＣＤ = T.企業ＣＤ AND SK.対象日 = '2016/06/27'
    LEFT JOIN 得意先発信履歴データ AS H ON H.企業ＣＤ = K.企業ＣＤ AND H.得意先ＣＤ = CT.得意先ＣＤ AND H.発信日 = '2016/06/27'
WHERE
    K.企業ＣＤ = 1
    AND K.担当者ＣＤ = 1
    AND T.受注方法 = 1      -- 電話要
    AND H.発信日 IS NULL
    AND TC.Tel_TelNo IS NOT NULL
