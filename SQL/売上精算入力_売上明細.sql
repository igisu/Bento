-- 売上精算入力_売上明細
SELECT
    M.企業ＣＤ
    ,M.売上日
    ,M.部署ＣＤ
    ,H.コースＣＤ
    ,M.売上区分
    ,M.商品ＣＤ
    ,S.商品名略称
    ,ISNULL(SUM(M.商品数量), 0)         AS 売上数量
    ,ISNULL(SUM(M.値引額), 0)           AS 値引額
    ,ISNULL(SUM(M.値引前金額), 0)       AS 売上金額
    ,ISNULL(SUM(M.値引後金額), 0)       AS 値引後金額
    ,ISNULL(SUM(M.消費税額), 0)         AS 消費税額
    ,ISNULL(SUM(M.商品金額合計税込), 0) AS 商品金額合計税込
    ,ISNULL(SUM(CASE WHEN S.弁当区分 IN (@弁当区分弁当, @弁当区分副食) THEN M.商品数量 ELSE 0 END), 0) AS 食数
    ,NULL AS 作成担当者ＣＤ
    ,NULL AS 作成日
    ,NULL AS 修正担当者ＣＤ
    ,NULL AS 修正日
FROM
    売上ヘッダ AS H
    INNER JOIN 売上明細 AS M
        ON  M.企業ＣＤ   = H.企業ＣＤ
        AND M.売上日     = H.売上日
        AND M.得意先ＣＤ = H.得意先ＣＤ
        AND M.部署ＣＤ   = H.部署ＣＤ
    LEFT JOIN 商品マスタ AS S
        ON  S.企業ＣＤ = M.企業ＣＤ
        AND S.商品ＣＤ = M.商品ＣＤ
WHERE
    H.企業ＣＤ       = @企業ＣＤ
    AND H.売上日     = @精算日
    AND H.部署ＣＤ   = @部署ＣＤ
    AND H.コースＣＤ = @コースＣＤ
    AND M.売上区分   = @売上区分
    AND ISNULL(M.チケット束フラグ, 0) = 0   -- チケット束売りは除く
GROUP BY
    M.企業ＣＤ
    ,M.売上日
    ,M.部署ＣＤ
    ,H.コースＣＤ
    ,M.売上区分
    ,M.商品ＣＤ
    ,S.商品名略称
ORDER BY
    M.商品ＣＤ
