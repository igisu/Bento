-- Web得意先一覧
SELECT
    A.企業ＣＤ
    ,A.得意先ＣＤ
    ,T.得意先名
    ,A.ログインＰＷ
    ,A.メールアドレス
    ,A.個別注文締切時間
    ,A.個別注文締切分
    ,A.無効フラグ
    ,A.作成担当者ＣＤ
    ,A.作成日
    ,A.修正担当者ＣＤ
    ,A.修正日
    ,L.IPv4アドレス
    ,L.最終ログイン日時
    ,A1.担当者名 AS 作成担当者名
    ,A.作成日
    ,A.修正担当者ＣＤ
    ,A2.担当者名 AS 修正担当者名
    ,A.修正日
FROM
    WEB_M_得意先 AS A
    LEFT JOIN
        (SELECT
            LA.ID
            ,LA.企業ＣＤ
            ,LA.得意先ＣＤ
            ,LA.IPv4アドレス
            ,LA.最終ログイン日時
            ,cast(LA.最終ログイン日時 AS date) AS 最終ログイン日
        FROM
            WEB_T_自動ログイン AS LA
            INNER JOIN
                (SELECT
                    得意先ＣＤ
                    ,企業ＣＤ
                    ,max(最終ログイン日時) AS 最終ログイン日時
                FROM
                    WEB_T_自動ログイン
                WHERE
                    企業ＣＤ = @企業ＣＤ
                GROUP BY
                    得意先ＣＤ
                    ,企業ＣＤ
                ) AS LB
                ON LA.得意先ＣＤ = LB.得意先ＣＤ
                AND LA.企業ＣＤ = LB.企業ＣＤ
                AND LA.最終ログイン日時 = LB.最終ログイン日時
           ) AS L ON A.企業ＣＤ = L.企業ＣＤ AND A.得意先ＣＤ = L.得意先ＣＤ
    LEFT JOIN 得意先マスタ AS T ON A.企業ＣＤ = T.企業ＣＤ AND A.得意先ＣＤ = T.得意先ＣＤ
    LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = A.企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ
    LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = A.企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ
WHERE
    A.企業ＣＤ = @企業ＣＤ
    AND T.部署ＣＤ = @部署ＣＤ
    AND A.得意先ＣＤ = @得意先ＣＤ
    AND L.最終ログイン日 >= @最終ログイン日From
    AND L.最終ログイン日 <= @最終ログイン日To
    AND L.IPv4アドレス = @IPアドレス
    AND ISNULL(A.無効フラグ, 0) = 0
ORDER BY
    A.得意先ＣＤ



@企業ＣＤ = 40000
@部署ＣＤ = 1
@得意先ＣＤ = 530
@最終ログイン日From = '2017/08/01'
@最終ログイン日To = '2017/08/07'
@IPアドレス = '172.24.252.164'
