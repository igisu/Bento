-- TP決済一覧
 SELECT
    TP.企業ＣＤ
    ,TP.TP決済番号
    ,TP.決済日
    ,TP.部署ＣＤ
    ,TP.得意先ＣＤ
    ,TP.売上明細番号
    ,TP.入金番号
    ,TP.取引日時
    ,TP.取引区分
    ,CASE
        WHEN ISNULL(TP.売上明細番号, 0) <> 0 THEN H.得意先名
        WHEN ISNULL(TP.入金番号, 0) <> 0 THEN N.得意先名
        ELSE NULL
     END AS 得意先名
    ,CASE
        WHEN ISNULL(TP.売上明細番号, 0) <> 0 THEN H.コースＣＤ
        ELSE NULL
     END AS コースＣＤ
    ,CASE
        WHEN ISNULL(TP.売上明細番号, 0) <> 0 THEN C.コース名
        ELSE NULL
     END AS コース名
    ,CASE
        WHEN ISNULL(TP.売上明細番号, 0) <> 0 THEN D.商品ＣＤ
        ELSE NULL
     END AS 商品ＣＤ
    ,CASE
        WHEN ISNULL(TP.売上明細番号, 0) <> 0 THEN S.商品名
        ELSE NULL
     END AS 商品名
    ,CASE
        WHEN ISNULL(クレジットカード会社名, '') <> '' THEN クレジットカード会社名
        WHEN ISNULL(電子マネーブランド名, '') <> '' THEN 電子マネーブランド名
        ELSE NULL
     END AS 決済方法名
    ,CASE
        WHEN ISNULL(TP.売上明細番号, 0) <> 0 THEN '売上'
        WHEN ISNULL(TP.入金番号, 0) <> 0 THEN '入金'
        ELSE NULL
     END AS 決済対象
    ,CASE
        WHEN TP.取引区分 IN ('売上', 'CL 売上') THEN TP.取引金額
        WHEN TP.取引区分 IN ('取消', 'CL 取消') THEN TP.取引金額 * -1
        ELSE TP.取引金額
     END AS 取引金額
 FROM
    TP決済情報 AS TP
    LEFT JOIN 売上明細     AS D ON D.企業ＣＤ = TP.企業ＣＤ AND D.売上日     = TP.決済日 AND D.得意先ＣＤ = TP.得意先ＣＤ AND D.部署ＣＤ = TP.部署ＣＤ AND D.明細番号 = TP.売上明細番号
    LEFT JOIN 売上ヘッダ   AS H ON H.企業ＣＤ = TP.企業ＣＤ AND H.売上日     = D.売上日  AND H.得意先ＣＤ = D.得意先ＣＤ  AND H.部署ＣＤ = D.部署ＣＤ
    LEFT JOIN 入金明細     AS N ON N.企業ＣＤ = TP.企業ＣＤ AND N.入金日     = TP.決済日 AND N.得意先ＣＤ = TP.得意先ＣＤ AND N.部署ＣＤ = TP.部署ＣＤ AND N.入金番号 = TP.入金番号
    LEFT JOIN 商品マスタ   AS S ON S.企業ＣＤ = D.企業ＣＤ  AND S.商品ＣＤ   = D.商品ＣＤ
    LEFT JOIN コースマスタ AS C ON C.企業ＣＤ = H.企業ＣＤ  AND C.コースＣＤ = H.コースＣＤ
    LEFT JOIN 部署マスタ   AS B ON B.企業ＣＤ = TP.企業ＣＤ AND B.部署ＣＤ   = TP.部署ＣＤ
 WHERE
    TP.企業ＣＤ = @企業ＣＤ
    AND B.支店ＣＤ = @支店ＣＤ
    AND TP.部署ＣＤ = @部署ＣＤ
    AND TP.売上日 >= @売上日From
    AND TP.売上日 <= @売上日To
    AND D.商品ＣＤ = @商品ＣＤ
    AND C.コースＣＤ = @コースＣＤ
    AND TP.得意先ＣＤ = @得意先ＣＤ
    AND TP.取引区分 = @取引区分
    AND (TP.クレジットカード会社名 LIKE @決済方法名 escape '#' OR TP.電子マネーブランド名 LIKE @決済方法名 escape '#'
        OR replace(replace(TP.クレジットカード会社名, ' ', ''), '　', '') LIKE @決済方法名 escape '#'
        OR replace(replace(TP.電子マネーブランド名, ' ', ''), '　', '') LIKE @決済方法名 escape '#')
 ORDER BY
    TP.取引日時 DESC, TP.得意先ＣＤ, TP.決済日;

@企業ＣＤ = 40006
@支店ＣＤ = 1
@部署ＣＤ = 1
@売上日From = 2024/01/01 0:00:00
@売上日To = 2024/10/24 0:00:00
@商品ＣＤ = 1
@コースＣＤ = 2
@得意先ＣＤ = 8957
@取引区分 = 売上
@決済方法名 = %MasterCard%