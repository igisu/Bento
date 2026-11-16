-- 売上一覧
SELECT
    D.企業ＣＤ
     ,H.コースＣＤ
     ,C.コース名
     ,D.売上日
     ,D.得意先ＣＤ
     ,T.得意先名
     ,D.部署ＣＤ
     ,D.明細番号
     ,D.商品ＣＤ
     ,S.商品名
     ,D.商品グループＣＤ
     ,G.商品グループ名略称 AS 商品グループ名
     ,D.売上区分
     ,VU.売上区分名
     ,D.商品単価
     ,D.商品数量
     ,D.値引額
     ,D.値引事由区分
     ,D.値引前金額
     ,D.値引後金額
     ,D.消費税額
     ,D.商品金額合計税込
     ,D.計算税率
     ,D.チケット束フラグ
     ,D.主食ＣＤ
     ,D.副食ＣＤ
     ,D.食事区分
     ,VS.食事区分名
     ,D.分配元数量
     ,D.TP決済番号
     ,D.TP取消決済番号
     ,case when isnull(S.軽減税率フラグ, 0) <> 0 then '○' else null end as 軽減税率対象
     ,H.税区分
     ,VZ.税区分名
     ,D.作成担当者ＣＤ
     ,D.作成日
     ,D.修正担当者ＣＤ
     ,D.修正日
 FROM
     売上明細 AS D
     LEFT JOIN 売上ヘッダ         AS H  ON H.企業ＣＤ = D.企業ＣＤ AND H.売上日           = D.売上日 AND H.得意先ＣＤ = D.得意先ＣＤ AND H.部署ＣＤ = D.部署ＣＤ
     LEFT JOIN コースマスタ       AS C  ON C.企業ＣＤ = H.企業ＣＤ AND C.コースＣＤ       = H.コースＣＤ
     LEFT JOIN 部署マスタ         AS B  ON B.企業ＣＤ = D.企業ＣＤ AND B.部署ＣＤ         = D.部署ＣＤ
     LEFT JOIN 得意先マスタ       AS T  ON T.企業ＣＤ = D.企業ＣＤ AND T.得意先ＣＤ       = D.得意先ＣＤ
     LEFT JOIN V_税区分           AS VZ ON VZ.税区分CD = H.税区分
     LEFT JOIN 商品マスタ         AS S  ON S.企業ＣＤ = D.企業ＣＤ AND S.商品ＣＤ         = D.商品ＣＤ
     LEFT JOIN 商品グループマスタ AS G  ON G.企業ＣＤ = D.企業ＣＤ AND G.商品グループＣＤ = D.商品グループＣＤ
     LEFT JOIN V_売上区分         AS VU ON VU.売上区分CD = D.売上区分
     LEFT JOIN V_食事区分         AS VS ON VS.食事区分CD = D.食事区分
 WHERE
     D.企業ＣＤ = @企業ＣＤ
 and B.支店ＣＤ = @支店ＣＤ
 and D.部署ＣＤ = @部署ＣＤ
 and D.売上日 >= @売上日From
 and D.売上日 <= @売上日To
 and D.商品ＣＤ = @商品ＣＤ
 and D.商品グループＣＤ = @商品グループＣＤ
 and D.売上区分 = @売上区分
 and D.食事区分 = @食事区分
 and C.コースＣＤ = @コースＣＤ
 and ISNULL(T.請求先ＣＤ, T.得意先ＣＤ) = @請求先ＣＤ
 and D.得意先ＣＤ = @得意先ＣＤ
 and ISNULL(T.受注得意先ＣＤ, T.得意先ＣＤ) = @受注得意先ＣＤ
 and ISNULL(S.軽減税率フラグ, 0) = 1        -- 軽減税率対象
 and ISNULL(S.軽減税率フラグ, 0) = 0        -- 軽減税率対象外
order by
D.売上日 DESC, D.得意先ＣＤ, D.明細番号;
@企業ＣＤ = 40001
@部署ＣＤ = 1
@売上日From = 2016/10/13 0:00:00
@売上日To = 2016/10/13 0:00:00
@商品ＣＤ = 19
@商品グループＣＤ = 1
@売上区分 = 1
@食事区分 = 2
@コースＣＤ = 88
@請求先ＣＤ = 7888
@得意先ＣＤ = 7888
@受注得意先ＣＤ = 7888
