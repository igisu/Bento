-- 領収書_税率ごとの内訳取得用の売上明細取得
SELECT
    G.請求番号
    ,G.請求先名
    ,URI.売上日
    ,URI.値引後金額
    ,URI.計算税率
FROM
    -- ①今回請求のキーと、それに対する前回請求のキーを求める
    (SELECT
        G1.企業ＣＤ
        ,G1.請求番号 AS 今回請求番号
        ,G1.請求先ＣＤ
        ,MAX(P1.請求日) AS 前回請求日
    FROM
        請求合計 AS G1
        LEFT JOIN 請求合計 AS P1
            ON G1.企業ＣＤ = P1.企業ＣＤ
            AND G1.請求先ＣＤ = P1.請求先ＣＤ
            AND P1.請求日 < G1.請求日
    WHERE
        G1.企業ＣＤ = @企業ＣＤ
        AND G1.部署ＣＤ = @部署ＣＤ
        AND G1.請求先ＣＤ = @請求先ＣＤ
        AND G1.請求日 >= @請求日From
        AND G1.請求日 <= @請求日To
        AND G1.回収予定日 >= @回収予定日From
        AND G1.回収予定日 <= @回収予定日To
        AND CONVERT(date, G1.修正日) = @処理日
        AND G1.今回請求額 >= @今回請求額
        AND G1.締区分 = @締区分
    GROUP BY
        G1.企業ＣＤ
        ,G1.請求番号
        ,G1.請求先ＣＤ
    ) AS P
    -- ②今回請求の詳細
    INNER JOIN 請求合計 G ON
        G.企業ＣＤ = P.企業ＣＤ
        AND G.請求番号 = P.今回請求番号
    INNER JOIN 得意先マスタ AS T
        ON  T.企業ＣＤ = G.企業ＣＤ
        AND T.得意先ＣＤ = G.請求先ＣＤ
    -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
    INNER JOIN 得意先マスタ AS J
        ON J.企業ＣＤ = T.企業ＣＤ
        AND J.得意先ＣＤ = ISNULL(T.受注得意先ＣＤ, T.得意先ＣＤ)
    -- ③今回の請求ヘッダ（売上がある請求ヘッダのみ取得する）（部署ごとの最新の売上を取得する必要があるので合計にはしない）
    --   計算税率ごとにレコードがあるのでそれを同一視する（計算税率ごとのレコードを取得しない）
    LEFT JOIN
        (SELECT DISTINCT
             企業ＣＤ
             ,請求番号
             ,部署ＣＤ
             ,得意先ＣＤ
             ,請求先ＣＤ
        FROM
            請求ヘッダ
        WHERE
            売上金額税込 <> 0
        ) AS H ON
        H.企業ＣＤ = G.企業ＣＤ
        AND H.請求番号 = G.請求番号
    -- ④今回の請求に対する最新の売上
    LEFT JOIN
        (SELECT
            UH.企業ＣＤ
            ,UH.得意先ＣＤ
            ,UH.部署ＣＤ
            ,UH.売上日
            ,UH.納品書伝票番号
            ,UM.明細番号
            ,UM.商品ＣＤ
            ,UM.商品単価
            ,UM.値引後金額
            ,UM.商品数量
            ,UM.計算税率
        FROM
            売上ヘッダ AS UH
            INNER JOIN 売上明細 AS UM ON
                UM.企業ＣＤ = UH.企業ＣＤ
                AND UM.得意先ＣＤ = UH.得意先ＣＤ
                AND UM.部署ＣＤ = UH.部署ＣＤ
                AND UM.売上日 = UH.売上日
        WHERE
            UM.売上区分 = @売上区分売掛        -- 売掛のみ
            AND ISNULL(UM.商品数量, 0) <> 0
--            AND (ISNULL(UM.商品単価, 0) <> 0 OR ISNULL(UM.値引額, 0) <> 0)        -- 単価が0でも、値引が0でなければ出力する
        ) AS URI ON
        URI.企業ＣＤ = H.企業ＣＤ
        AND URI.得意先ＣＤ = H.得意先ＣＤ
        AND URI.部署ＣＤ = H.部署ＣＤ
        AND URI.売上日 > ISNULL(P.前回請求日, '2000/01/01')
        AND URI.売上日 <= G.請求日

    -- コースによる絞り込み用
    -- 同じ得意先に複数のコースが設定されている場合、そのいずれのコースを条件に指定しても、そのコースにぶら下がる得意先がヒットしないといけない。
    INNER JOIN
        (SELECT DISTINCT
            CM.企業ＣＤ
            ,CM.部署ＣＤ
            ,CM.コース名
            ,CT.コースＣＤ
            ,CT.得意先ＣＤ
            ,CT.コース順
        FROM
            コースマスタ AS CM
            INNER JOIN コーステーブル AS CT ON
                CM.企業ＣＤ = CT.企業ＣＤ
                AND CM.コースＣＤ =  CT.コースＣＤ
        WHERE
            CT.コースＣＤ = @コースＣＤ
        ) AS C
        ON J.企業ＣＤ = C.企業ＣＤ
        AND G.部署ＣＤ = C.部署ＣＤ     -- 請求の部署と一致するものに限る
        AND J.得意先ＣＤ = C.得意先ＣＤ
WHERE
    1 = 1
    AND T.支払方法 = @支払方法
    AND T.EB銀行支店ＣＤ = @EB銀行支店ＣＤ      -- 支払方法が振替の場合のみ有効
    AND T.週締曜日 = @週締曜日
    AND T.月締日 = @月締日



@企業ＣＤ = 40001
@請求番号 = 69
@売上区分売掛 = 1
