-- Web得意先詳細（自動ログイン(セッション)一覧）
SELECT
    ID
    ,認証キー
    ,企業ＣＤ
    ,得意先ＣＤ
    ,UA
    ,IPv4アドレス
    ,最終ログイン日時
FROM
    WEB_T_自動ログイン
WHERE
    企業ＣＤ = @企業ＣＤ
    AND 得意先ＣＤ = @得意先ＣＤ
ORDER BY
    最終ログイン日時 DESC












-- IPアドレスごとの最終ログイン日時取得版
SELECT
    LA.ID
    ,LA.認証キー
    ,LA.企業ＣＤ
    ,LA.得意先ＣＤ
    ,LA.UA
    ,LA.IPv4アドレス
    ,LA.最終ログイン日時
FROM
    WEB_T_自動ログイン AS LA
    INNER JOIN
        (SELECT
            得意先ＣＤ
            ,企業ＣＤ
            ,IPv4アドレス
            ,max(最終ログイン日時) AS 最終ログイン日時
        FROM
            WEB_T_自動ログイン
        WHERE
            企業ＣＤ = @企業ＣＤ
        GROUP BY
            得意先ＣＤ
            ,企業ＣＤ
            ,IPv4アドレス
        ) AS LB
        ON LA.得意先ＣＤ = LB.得意先ＣＤ
        AND LA.企業ＣＤ = LB.企業ＣＤ
        AND LA.IPv4アドレス = LB.IPv4アドレス
        AND LA.最終ログイン日時 = LB.最終ログイン日時
WHERE
    LA.企業ＣＤ = @企業ＣＤ
    AND LA.得意先ＣＤ = @得意先ＣＤ
ORDER BY
    LA.ID


@企業ＣＤ = 40000
@得意先ＣＤ = 530
