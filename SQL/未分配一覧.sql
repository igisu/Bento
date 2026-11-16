-- 未分配一覧
SELECT
    M.売上日
    ,M.部署ＣＤ
    ,B.部署名
    ,M.得意先ＣＤ
    ,H.得意先名
    ,H.コースＣＤ
    ,C.コース名
    ,H.配送担当者ＣＤ
    ,A1.担当者名 AS 配送担当者名
    ,M.商品ＣＤ
    ,S.商品名
    ,M.売上数
    ,M.未分配個数
    ,M.分配未着手
    ,M.分配未完了
    ,M.分配完了
FROM
    (SELECT DISTINCT
        UM.企業ＣＤ
        ,UM.売上日
        ,UM.得意先ＣＤ
        ,UM.部署ＣＤ
        ,UM.明細番号
        ,UM.商品ＣＤ
        ,UM.商品グループＣＤ
        ,UM.売上区分
        ,UM.商品単価
        ,UM.商品数量
        ,UM.値引額
        ,UM.値引事由区分
        ,UM.値引前金額
        ,UM.値引後金額
        ,UM.消費税額
        ,UM.商品金額合計税込
        ,UM.計算税率
        ,UM.チケット束フラグ
        ,UM.現金補填フラグ
        ,UM.主食ＣＤ
        ,UM.副食ＣＤ
        ,UM.食事区分
        ,UM.分配元数量
        ,UM.作成担当者ＣＤ
        ,UM.作成日
        ,UM.修正担当者ＣＤ
        ,UM.修正日
        ,ISNULL(UM.分配元数量, 0) + UM.商品数量 AS 売上数
        ,UM.商品数量 AS 未分配個数
        ,CASE WHEN ISNULL(UM.分配元数量, 0) = 0 THEN 1 ELSE 0 END AS 分配未着手
        ,CASE WHEN ISNULL(UM.商品数量, 0) <> 0 AND ISNULL(UM.分配元数量, 0) > 0 THEN 1 ELSE 0 END AS 分配未完了
        ,CASE WHEN ISNULL(UM.商品数量, 0) = 0 AND ISNULL(UM.分配元数量, 0) > 0 THEN 1 ELSE 0 END AS 分配完了
    FROM
        売上明細 AS UM
        -- 分配先が存在する得意先（＝分配元）を抽出する（受注得意先として指定されている得意先を表示対象とする）
        INNER JOIN 得意先マスタ AS T
            ON T.企業ＣＤ = UM.企業ＣＤ
            AND T.受注得意先ＣＤ = UM.得意先ＣＤ
    WHERE
        UM.企業ＣＤ = @企業ＣＤ
        AND UM.売上区分 = @売上区分売掛
        AND ISNULL(UM.商品数量, 0) + ISNULL(UM.分配元数量, 0) <> 0  -- そもそも売上が無ければ表示する必要は無い
        AND T.受注得意先ＣＤ IS NOT NULL
    ) AS M
    INNER JOIN 売上ヘッダ AS H
        ON  H.企業ＣＤ   = M.企業ＣＤ
        AND H.売上日     = M.売上日
        AND H.得意先ＣＤ = M.得意先ＣＤ
        AND H.部署ＣＤ   = M.部署ＣＤ
    LEFT JOIN 部署マスタ   AS B  ON B.企業ＣＤ  = M.企業ＣＤ AND B.部署ＣＤ = M.部署ＣＤ
    LEFT JOIN コースマスタ AS C  ON C.企業ＣＤ  = H.企業ＣＤ AND C.コースＣＤ = H.コースＣＤ
    LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = H.企業ＣＤ AND H.配送担当者ＣＤ = A1.担当者ＣＤ
    LEFT JOIN 商品マスタ   AS S  ON S.企業ＣＤ  = M.企業ＣＤ AND S.商品ＣＤ = M.商品ＣＤ
WHERE
    1 = 1
    AND M.部署ＣＤ = @部署ＣＤ
    AND M.売上日 >= @売上日From
    AND M.売上日 <= @売上日To
    AND H.コースＣＤ >= @コースＣＤFrom
    AND H.コースＣＤ <= @コースＣＤTo
    AND M.分配未完了 = 1
    AND M.分配完了 = 0
ORDER BY
    M.売上日
    ,M.部署ＣＤ
    ,M.得意先ＣＤ
    ,H.コースＣＤ
