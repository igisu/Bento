-- ‹âsAPIƒtƒ@ƒCƒ‹Žæž_’¼‹ßŒŽ‚ÉŠÜ‚Ü‚ê‚é‘S‚Ä‚Ì•”
-- ‚±‚ê‚ª•¡”‘¶Ý‚µ‚½‚çA•”‚ª•¡”‚ ‚é‚Ì‚Å’ˆÓ‚·‚éŽ|‚ÌŒx‚ð•\Ž¦
SELECT DISTINCT
    A.•”‚b‚c
FROM
    ¿‹‡Œv AS A
    INNER JOIN
        (SELECT
            G.Šé‹Æ‚b‚c
            ,G.•”‚b‚c
            ,G.¿‹æ‚b‚c
            ,MAX(G.¿‹“ú) AS ¿‹“ú
        FROM
            ¿‹‡Œv AS G
            INNER JOIN “¾ˆÓæƒ}ƒXƒ^ AS T
                ON G.Šé‹Æ‚b‚c = T.Šé‹Æ‚b‚c
                AND G.¿‹æ‚b‚c = T.“¾ˆÓæ‚b‚c
                AND G.•”‚b‚c = T.•”‚b‚c
        WHERE
            G.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
            AND G.¿‹æ‚b‚c = @“¾ˆÓæ‚b‚c
            AND G.¿‹“ú < @“ü‹à“ú
        GROUP BY
            G.Šé‹Æ‚b‚c
            ,G.•”‚b‚c
            ,G.¿‹æ‚b‚c
        ) AS SG
        ON A.Šé‹Æ‚b‚c = SG.Šé‹Æ‚b‚c
        AND A.¿‹æ‚b‚c = SG.¿‹æ‚b‚c
        AND YEAR(A.¿‹“ú) = YEAR(SG.¿‹“ú)
        AND MONTH(A.¿‹“ú) = MONTH(SG.¿‹“ú)
