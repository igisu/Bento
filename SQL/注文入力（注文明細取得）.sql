-- ’•¶“ü—Íi’•¶–¾×Žæ“¾j
SELECT
    M.Šé‹Æ‚b‚c
    ,0 AS ’•¶‹æ•ª
    ,M.‹iH“ú
    ,M.“¾ˆÓæ‚b‚c
    ,M.¤•i‚b‚c
    ,M.¤•i‚b‚c AS ¤•i‚b‚corg
    ,S.¤•i–¼
    ,M.•”‚b‚c
    ,ISNULL(M0.–¾×”Ô†         ,M1.–¾×”Ô†)               AS –¾×”Ô†
    ,M0.»‘¢“ú
    ,ISNULL(M0.¤•iƒOƒ‹[ƒv‚b‚c ,M1.¤•iƒOƒ‹[ƒv‚b‚c)       AS ¤•iƒOƒ‹[ƒv‚b‚c
    ,ISNULL(M0.”„Š|Œ»‹à‹æ•ª     ,ISNULL(K.”„Š|Œ»‹à‹æ•ª, 0)) AS ”„Š|Œ»‹à‹æ•ª     -- ‰Šú’l‚Í0:Œ»‹à
    ,ISNULL(M0.¤•i’P‰¿         ,M1.¤•i’P‰¿)               AS ¤•i’P‰¿
    ,M1.¤•i”—Ê AS —\’è”
    ,M0.¤•i”—Ê
    ,M0.¤•i‹àŠz‡Œv
    ,M0.Á”ïÅŠz
    ,M0.¤•i‹àŠz‡ŒvÅž
    ,ISNULL(M0.ŒvŽZÅ—¦, CASE WHEN ISNULL(S.ŒyŒ¸Å—¦ƒtƒ‰ƒO, 0) = 0 THEN TAX.Á”ïÅ—¦ ELSE ISNULL(TAX.ŒyŒ¸Å—¦, TAX.Á”ïÅ—¦) END) AS ŒvŽZÅ—¦
    ,M0.ì¬’S“–ŽÒ‚b‚c
    ,A1.’S“–ŽÒ–¼ AS ì¬’S“–ŽÒ–¼
    ,M0.ì¬“ú
    ,M0.C³’S“–ŽÒ‚b‚c
    ,A2.’S“–ŽÒ–¼ AS C³’S“–ŽÒ–¼
    ,M0.C³“ú
FROM
    -- ‡@‹ì“®•\i’•¶‚Æ—\‘ª‚Ì’•¶–¾×j
    (SELECT
        Šé‹Æ‚b‚c
        ,‹iH“ú
        ,“¾ˆÓæ‚b‚c
        ,¤•i‚b‚c
        ,•”‚b‚c
    FROM
        ’•¶–¾×
    WHERE
        Šé‹Æ‚b‚c = 1
        AND ’•¶‹æ•ª = 0        -- ’•¶
        AND ‹iH“ú = '2016/06/16'
        AND “¾ˆÓæ‚b‚c = 4311
        AND •”‚b‚c = 1
    UNION
    SELECT
        Šé‹Æ‚b‚c
        ,‹iH“ú
        ,“¾ˆÓæ‚b‚c
        ,¤•i‚b‚c
        ,•”‚b‚c
    FROM
        ’•¶–¾×
    WHERE
        Šé‹Æ‚b‚c = 1
        AND ’•¶‹æ•ª = 1        -- —\‘ª
        AND ‹iH“ú = '2016/06/16'
        AND “¾ˆÓæ‚b‚c = 4311
        AND •”‚b‚c = 1
    ) AS M
    -- ‡A’•¶–¾×i’•¶j
    LEFT JOIN
        (SELECT
            Šé‹Æ‚b‚c
            ,’•¶‹æ•ª
            ,‹iH“ú
            ,“¾ˆÓæ‚b‚c
            ,¤•i‚b‚c
            ,•”‚b‚c
            ,–¾×”Ô†
            ,»‘¢“ú
            ,¤•iƒOƒ‹[ƒv‚b‚c
            ,”„Š|Œ»‹à‹æ•ª
            ,¤•i’P‰¿
            ,¤•i”—Ê
            ,¤•i‹àŠz‡Œv
            ,Á”ïÅŠz
            ,¤•i‹àŠz‡ŒvÅž
            ,ŒvŽZÅ—¦
            ,ì¬’S“–ŽÒ‚b‚c
            ,ì¬“ú
            ,C³’S“–ŽÒ‚b‚c
            ,C³“ú
        FROM
            ’•¶–¾×
        WHERE
            Šé‹Æ‚b‚c = 1
            AND ’•¶‹æ•ª = 0            -- ’•¶
            AND ‹iH“ú = '2016/06/16'
            AND “¾ˆÓæ‚b‚c = 4311
            AND •”‚b‚c = 1
        ) AS M0
        ON M0.Šé‹Æ‚b‚c    = M.Šé‹Æ‚b‚c
        AND M0.‹iH“ú     = M.‹iH“ú
        AND M0.“¾ˆÓæ‚b‚c = M.“¾ˆÓæ‚b‚c
        AND M0.¤•i‚b‚c   = M.¤•i‚b‚c
        AND M0.•”‚b‚c   = M.•”‚b‚c
    -- ‡A’•¶–¾×i—\‘ªj
    LEFT JOIN
        (SELECT
            Šé‹Æ‚b‚c
            ,’•¶‹æ•ª
            ,‹iH“ú
            ,“¾ˆÓæ‚b‚c
            ,¤•i‚b‚c
            ,•”‚b‚c
            ,–¾×”Ô†
            ,»‘¢“ú
            ,¤•iƒOƒ‹[ƒv‚b‚c
            ,”„Š|Œ»‹à‹æ•ª
            ,¤•i’P‰¿
            ,¤•i”—Ê
            ,¤•i‹àŠz‡Œv
            ,Á”ïÅŠz
            ,¤•i‹àŠz‡ŒvÅž
            ,ŒvŽZÅ—¦
            ,ì¬’S“–ŽÒ‚b‚c
            ,ì¬“ú
            ,C³’S“–ŽÒ‚b‚c
            ,C³“ú
        FROM
            ’•¶–¾×
        WHERE
            Šé‹Æ‚b‚c = 1
            AND ’•¶‹æ•ª = 1            -- —\‘ª
            AND ‹iH“ú = '2016/06/16'
            AND “¾ˆÓæ‚b‚c = 4311
            AND •”‚b‚c = 1
        ) AS M1
        ON M1.Šé‹Æ‚b‚c    = M.Šé‹Æ‚b‚c
        AND M1.‹iH“ú     = M.‹iH“ú
        AND M1.“¾ˆÓæ‚b‚c = M.“¾ˆÓæ‚b‚c
        AND M1.¤•i‚b‚c   = M.¤•i‚b‚c
        AND M1.•”‚b‚c   = M.•”‚b‚c
    LEFT JOIN “¾ˆÓæƒ}ƒXƒ^ AS K  ON K.Šé‹Æ‚b‚c = M.Šé‹Æ‚b‚c AND K.“¾ˆÓæ‚b‚c = M.“¾ˆÓæ‚b‚c
    LEFT JOIN ¤•iƒ}ƒXƒ^   AS S  ON S.Šé‹Æ‚b‚c = M.Šé‹Æ‚b‚c AND S.¤•i‚b‚c = M.¤•i‚b‚c
    LEFT JOIN ’S“–ŽÒƒ}ƒXƒ^ AS A1 ON A1.Šé‹Æ‚b‚c = M0.Šé‹Æ‚b‚c AND M0.ì¬’S“–ŽÒ‚b‚c = A1.’S“–ŽÒ‚b‚c
    LEFT JOIN ’S“–ŽÒƒ}ƒXƒ^ AS A2 ON A2.Šé‹Æ‚b‚c = M0.Šé‹Æ‚b‚c AND M0.C³’S“–ŽÒ‚b‚c = A2.’S“–ŽÒ‚b‚c
    LEFT JOIN (
        SELECT
            Á”ïÅ—¦,
            ŒyŒ¸Å—¦
        FROM
            Á”ïÅ—¦ƒ}ƒXƒ^
        WHERE
            Ž{s“ú = (SELECT MAX(Ž{s“ú) FROM Á”ïÅ—¦ƒ}ƒXƒ^ WHERE Ž{s“ú <= '2016/06/16')
    ) AS TAX ON 1 = 1
ORDER BY
    –¾×”Ô†
