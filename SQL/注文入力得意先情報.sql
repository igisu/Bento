-- 注文入力得意先情報
SELECT
    T.得意先ＣＤ
    ,T.得意先名略称
    ,ISNULL(TC.Tel_TelNo, ISNULL(T.電話番号1, T.電話番号2)) AS 電話番号
    ,TC.Tel_Remarks AS 電話番号備考
    ,T.部署ＣＤ
    ,B.部署名
    ,B.支店ＣＤ
    ,S.支店名
    ,ISNULL(T.税区分, 1) AS 税区分                   -- 初期値は1:内税
    ,ISNULL(T.丸め処理区分, 2) AS 丸め処理区分       -- 初期値は2:切り捨て
    ,ISNULL(T.売掛現金区分, 0) AS 売掛現金区分       -- 初期値は0:現金
    ,T.受注得意先ＣＤ
    ,CASE WHEN H.売上日 IS NULL THEN '未配達' ELSE '配達済' END AS 配送状態
    ,T.備考1
    ,T.備考2
    ,T.備考3
    ,T.休日設定
FROM
    得意先マスタ AS T
    -- 主であり、最も若い電話番号(Tel_TelNo)をC_TelToCustから取得する。
    -- その備考(Tel_Remarks)を取得すべく、再度C_TelToCustと結合している。
    LEFT JOIN
        (SELECT
            企業ＣＤ
            ,Tel_CustNo
            ,MIN(Tel_TelNo) AS Tel_TelNo
        FROM
           C_TelToCust
        WHERE
            Tel_RepFlg = 0
        GROUP BY
            企業ＣＤ
            ,Tel_CustNo
       ) AS TTC
           ON TTC.企業ＣＤ = T.企業ＣＤ
           AND TTC.Tel_CustNo = T.得意先ＣＤ
    LEFT JOIN C_TelToCust AS TC ON TC.企業ＣＤ = TTC.企業ＣＤ AND TC.Tel_CustNo = TTC.Tel_CustNo AND TC.Tel_TelNo = TTC.Tel_TelNo
    LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = T.企業ＣＤ AND B.部署ＣＤ = T.部署ＣＤ AND ISNULL(B.無効フラグ, 0) = 0
    LEFT JOIN 支店マスタ AS S ON S.企業ＣＤ = B.企業ＣＤ AND S.支店ＣＤ = B.支店ＣＤ AND ISNULL(S.無効フラグ, 0) = 0
    LEFT JOIN 売上ヘッダ AS H ON H.企業ＣＤ = T.企業ＣＤ AND H.得意先ＣＤ = T.得意先ＣＤ AND H.売上日 = @売上日 AND H.部署ＣＤ = @部署ＣＤ
WHERE
    T.企業ＣＤ = @企業ＣＤ
    AND ISNULL(T.無効フラグ, 0) = 0
    AND T.得意先ＣＤ = @得意先ＣＤ



-- 注文入力コース取得
SELECT
    CT.コースＣＤ
    ,CM.コース名
    ,CM.部署ＣＤ
    ,CM.担当者ＣＤ
    ,T.担当者名
FROM
    コーステーブル AS CT
    LEFT JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0
    LEFT JOIN 担当者マスタ AS T ON T.企業ＣＤ = CT.企業ＣＤ AND T.担当者ＣＤ = CM.担当者ＣＤ AND ISNULL(T.無効フラグ, 0) = 0
WHERE
    CT.企業ＣＤ = @企業ＣＤ
    AND CT.得意先ＣＤ = @得意先ＣＤ
    AND ISNULL(CT.無効フラグ, 0) = 0
    AND CM.コース区分 = @コース区分
ORDER BY
    CT.コースＣＤ


-- 注文入力臨時コース取得（優先）
SELECT
    CT.コースＣＤ
    ,CM.コース名
    ,CM.部署ＣＤ
    ,CM.担当者ＣＤ
    ,T.担当者名
FROM
    臨時コーステーブル AS CT
    INNER JOIN 臨時コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND CT.実施日 = CM.実施日 AND ISNULL(CM.無効フラグ, 0) = 0
    LEFT JOIN 担当者マスタ AS T ON T.企業ＣＤ = CT.企業ＣＤ AND T.担当者ＣＤ = CM.担当者ＣＤ AND ISNULL(T.無効フラグ, 0) = 0
WHERE
    CT.企業ＣＤ = @企業ＣＤ
    AND CT.実施日 = @実施日
    AND CT.得意先ＣＤ = @得意先ＣＤ
    AND ISNULL(CT.無効フラグ, 0) = 0
    AND CM.コース区分 = @コース区分
ORDER BY
    CT.コースＣＤ
