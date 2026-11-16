-- 会計連携（集計対象外の抽出）

-- 売上集計
SELECT
    U.売上日
    ,U.部署ＣＤ
    ,B.部署名
    ,U.得意先ＣＤ
    ,U.得意先名
    ,U.商品ＣＤ
    ,S.商品名
    ,U.商品数量
    ,VU.売上区分名
FROM
    (SELECT
        UH.企業ＣＤ
        ,UH.売上日
        ,UH.部署ＣＤ
        ,UH.得意先ＣＤ
        ,UH.得意先名
        ,UM.明細番号
        ,UM.商品ＣＤ
        ,UM.商品数量
        ,UM.売上区分
        ,UM.チケット束フラグ
    FROM
        売上ヘッダ AS UH
        INNER JOIN 売上明細 AS UM
            ON UH.企業ＣＤ = UM.企業ＣＤ
            AND UH.売上日 = UM.売上日
            AND UH.得意先ＣＤ = UM.得意先ＣＤ
            AND UH.部署ＣＤ = UM.部署ＣＤ
    WHERE
        UH.企業ＣＤ = @企業ＣＤ
        AND UH.売上日 = @出力対象日         -- この日だけを再集計 の場合
        AND UH.売上日 >= @出力対象月一日    -- 差分 または 履歴から の場合
        AND UH.売上日 <= @出力対象日        -- 差分 または 履歴から の場合
    ) AS U
    LEFT JOIN 仕訳マスタ AS SW
        ON SW.企業ＣＤ = U.企業ＣＤ
        AND SW.部署ＣＤ = U.部署ＣＤ
        AND SW.売上区分 = U.売上区分
        AND SW.チケット束フラグ = U.チケット束フラグ
        AND SW.集計対象区分 = @集計対象区分売上
    LEFT JOIN 部署マスタ AS B
        ON B.企業ＣＤ = U.企業ＣＤ
        AND B.部署ＣＤ = U.部署ＣＤ
    LEFT JOIN 商品マスタ AS S
        ON U.企業ＣＤ = S.企業ＣＤ
        AND U.商品ＣＤ = S.商品ＣＤ
    LEFT JOIN V_売上区分 AS VU
        ON U.売上区分 = VU.売上区分CD
WHERE
    SW.仕訳ＣＤ IS NULL     -- 仕訳マスタにヒットしないもの
ORDER BY
    U.売上日
    ,U.部署ＣＤ
    ,U.得意先ＣＤ
    ,U.明細番号


-- 入金集計
SELECT
    NM.入金日
    ,NM.部署ＣＤ
    ,B.部署名
    ,NM.得意先ＣＤ
    ,NM.得意先名
    ,NM.入金方法
    ,VN.入金方法名
    ,NM.入金額
FROM
    (SELECT
        企業ＣＤ
        ,入金番号
        ,入金日
        ,部署ＣＤ
        ,得意先ＣＤ
        ,得意先名
        ,入金方法
        ,入金額
    FROM
        入金明細
    WHERE
        企業ＣＤ = @企業ＣＤ
        AND 入金日 = @出力対象日            -- この日だけを再集計 の場合
        AND 入金日 >= @出力対象月一日       -- 差分 または 履歴から の場合
        AND 入金日 <= @出力対象日           -- 差分 または 履歴から の場合
    ) AS NM
    LEFT JOIN 仕訳マスタ AS SW
        ON SW.企業ＣＤ = NM.企業ＣＤ
        AND SW.部署ＣＤ = NM.部署ＣＤ
        AND SW.入金方法 = NM.入金方法
        AND SW.集計対象区分 = @集計対象区分入金
    LEFT JOIN 部署マスタ AS B
        ON B.企業ＣＤ = NM.企業ＣＤ
        AND B.部署ＣＤ = NM.部署ＣＤ
    LEFT JOIN V_入金方法 AS VN ON VN.入金方法CD = NM.入金方法
WHERE
    SW.仕訳ＣＤ IS NULL     -- 仕訳マスタにヒットしないもの
ORDER BY
    NM.入金日
    ,NM.部署ＣＤ
    ,NM.得意先ＣＤ
    ,NM.入金番号


@企業ＣＤ = 40001
@出力対象日 = '2017/03/15'
@集計対象区分売上 = 0
@出力対象月一日 = '2017/03/01'
@集計対象区分入金 = 1
