-- 注文入力_注文明細取得（得意先単価マスタ含む）
SELECT
     A.企業ＣＤ
     ,A.注文区分
     ,A.喫食日
     ,A.得意先ＣＤ
     ,X.商品ＣＤ
     ,X.商品ＣＤ AS 商品ＣＤorg
     ,ST.商品名
     ,ST.弁当区分
     ,A.部署ＣＤ
     ,ISNULL(A.明細番号, B.明細番号) AS 明細番号
     ,A.製造日
     ,ISNULL(ISNULL(A.商品グループＣＤ, B.商品グループＣＤ), SS.商品グループＣＤ) AS 商品グループＣＤ
     ,ISNULL(ISNULL(A.売掛現金区分, B.売掛現金区分), ISNULL(K.売掛現金区分, 0)) AS 売掛現金区分
     ,ISNULL(ISNULL(A.商品単価, B.商品単価), CASE 2 WHEN 1 THEN T.商品単価税込 ELSE T.商品単価税抜 END) AS 商品単価
     ,A.商品数量
     ,A.商品金額合計
     ,B.商品数量 AS 予定数
     ,A.消費税額
     ,A.商品金額合計税込
     ,ISNULL(A.計算税率, CASE WHEN ISNULL(ST.軽減税率フラグ, 0) = 0 THEN TAX.消費税率 ELSE ISNULL(TAX.軽減税率, TAX.消費税率) END) AS 計算税率
     ,A.作成担当者ＣＤ
     ,A1.担当者名 AS 作成担当者名
     ,A.作成日
     ,A.修正担当者ＣＤ
     ,A2.担当者名 AS 修正担当者名
     ,A.修正日
FROM
-- ①得意先単価マスタと注文明細から、対象となる商品ＣＤを取得
    (SELECT
        TT.商品ＣＤ
    FROM
        得意先単価マスタ AS TT
        INNER JOIN 商品マスタ AS ST ON
            ST.企業ＣＤ = TT.企業ＣＤ
            AND ST.商品ＣＤ = TT.商品ＣＤ
    WHERE
        TT.企業ＣＤ = 1 AND
        TT.得意先ＣＤ = 4310 AND
        ISNULL(TT.無効フラグ, 0) = 0 AND
        ISNULL(ST.無効フラグ, 0) = 0
    UNION
    SELECT
        AA.商品ＣＤ
    FROM
        注文明細 AS AA
    WHERE
        AA.企業ＣＤ = 1 AND
        AA.喫食日 = '2016/03/07' AND
        AA.得意先ＣＤ = 4310 AND
        AA.部署ＣＤ = 1
    ) AS X
-- ② ①と得意先単価マスタを外部結合
    LEFT JOIN 得意先単価マスタ AS T ON
        T.企業ＣＤ = 1
        AND T.得意先ＣＤ = 4310
        AND ISNULL(T.無効フラグ, 0) = 0
        AND T.商品ＣＤ = X.商品ＣＤ
-- ③ ①と注文明細（注文）を外部結合
    LEFT JOIN 注文明細 AS A ON
        A.企業ＣＤ = 1
        AND A.注文区分 = 0
        AND A.喫食日 = '2016/03/07'
        AND A.得意先ＣＤ = 4310
        AND A.部署ＣＤ = 1
        AND A.商品ＣＤ = X.商品ＣＤ
-- ④ ①と注文明細（予測）を外部結合
    LEFT JOIN 注文明細 AS B  ON
        B.企業ＣＤ = 1
        AND B.注文区分 = 1
        AND B.喫食日 = '2016/03/07'
        AND B.得意先ＣＤ = 4310
        AND B.部署ＣＤ = 1
        AND B.商品ＣＤ = X.商品ＣＤ
-- ⑤ ①と商品マスタを外部結合
    LEFT JOIN 商品マスタ AS ST ON
        ST.企業ＣＤ = 1
        AND ST.商品ＣＤ = X.商品ＣＤ
-- ⑥ 税率取得用に消費税率マスタを外部結合
    LEFT JOIN (
        SELECT
            消費税率 ,
            軽減税率
        FROM
            消費税率マスタ
        WHERE
            施行日 = (SELECT max(施行日) FROM 消費税率マスタ WHERE 施行日 <= '2016/03/07')
    ) AS TAX ON 1 = 1
-- ⑦ ①と得意先マスタを外部結合
    LEFT JOIN 得意先マスタ AS K ON
        K.企業ＣＤ = 1
        AND K.得意先ＣＤ = 4310
-- ⑧ ①と主要商品マスタを外部結合
    LEFT JOIN 主要商品リストマスタ AS SS ON
        SS.企業ＣＤ = 1 AND
        SS.部署ＣＤ = 1 AND
        SS.商品ＣＤ = X.商品ＣＤ
    LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = A.企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ
    LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = A.企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ
ORDER BY
    CASE WHEN A.明細番号 IS NULL THEN 1 ELSE 0 END  -- 注文→得意先単価のみの順
    ,A.明細番号
    ,ST.弁当区分
    ,X.商品ＣＤ
