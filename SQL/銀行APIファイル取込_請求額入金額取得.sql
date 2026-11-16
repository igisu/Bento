-- ‹âsAPIƒtƒ@ƒCƒ‹Žæž_¿‹Šz“ü‹àŠzŽæ“¾
SELECT
    A.¿‹”Ô†
    ,A.•”‚b‚c
    ,B.•”–¼
    ,A.‘O‰ñ¿‹Šz
    ,A.¡‰ñ¿‹Šz
    -- “ü‹à–¾×‚©‚ç“ü‹àŠz‚Ì‡Œv‚ðŽæ“¾‚·‚é‘ŠŠÖƒTƒuƒNƒGƒŠiƒXƒJƒ‰ƒTƒuƒNƒGƒŠj
    ,(SELECT
        ISNULL(SUM(N.“ü‹àŠz), 0) AS “ü‹àŠz
    FROM
        “ü‹à–¾× AS N
    WHERE
        N.Šé‹Æ‚b‚c = A.Šé‹Æ‚b‚c
        AND N.•”‚b‚c = A.•”‚b‚c
        AND N.“¾ˆÓæ‚b‚c = A.¿‹æ‚b‚c
        AND N.“ü‹à“ú > A.¿‹“ú
    ) AS ¿‹Œã“ü‹à
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
        AND A.•”‚b‚c = SG.•”‚b‚c
        AND A.¿‹æ‚b‚c = SG.¿‹æ‚b‚c
        AND A.¿‹“ú = SG.¿‹“ú
    LEFT JOIN •”ƒ}ƒXƒ^ AS B ON B.Šé‹Æ‚b‚c = A.Šé‹Æ‚b‚c AND B.•”‚b‚c = A.•”‚b‚c

@Šé‹Æ‚b‚c = 34002
@“¾ˆÓæ‚b‚c = 3735
@“ü‹à“ú = '2019/01/10'
