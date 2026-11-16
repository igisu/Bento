-- ‡ŽZ¿‹‘
SELECT
    G.¿‹”Ô†
    ,G.•”‚b‚c
    ,G.¿‹æ–¼
    ,G.¿‹æ–¼ + ' ' + ISNULL(VS.¿‹‘ŒhÌ–¼, '—l') AS “¾ˆÓæ–¼
    ,DATALENGTH(G.¿‹æ–¼ + ' ' + ISNULL(VS.¿‹‘ŒhÌ–¼, '')) AS “¾ˆÓæ–¼’·‚³
    ,G.‘O‰ñ¿‹Šz
    ,G.“ü‹àŠz
    ,G.ŒJ‰zŠz
    ,G.”ƒã‹àŠz
    ,G.’²®Šz
    ,G.Á”ïÅ
    ,G.¡‰ñ¿‹Šz
    ,G.‰ñŽû—\’è“ú
    ,@‰ñŽû—\’è“úˆóŽšƒtƒ‰ƒO AS ‰ñŽû—\’è“úˆóŽšƒtƒ‰ƒO
    ,T.—X•Ö”Ô† AS “¾ˆÓæ—X•Ö”Ô†
    ,T.ZŠ1 AS “¾ˆÓæZŠ1
    ,T.ZŠ2 AS “¾ˆÓæZŠ2
    ,T.“d˜b”Ô†1 AS “¾ˆÓæ“d˜b”Ô†
    ,T.‚e‚`‚w1 AS “¾ˆÓæ‚e‚`‚w
    ,T.UžŽè”—¿ƒtƒ‰ƒO             -- •‰’S‚ð‚¨Šè‚¢‚·‚éê‡‚Ítrue
    ,T.¿‹‘—X•Öƒ}[ƒNƒtƒ‰ƒO
    ,ISNULL(B.¿‹‘ƒ^ƒCƒv‹æ•ª, 1) AS ¿‹‘ƒ^ƒCƒv‹æ•ª
    ,B.‰ïŽÐ–¼Ì AS Ž©ŽÐ–¼Ì
    ,B.—X•Ö”Ô† AS Ž©ŽÐ—X•Ö”Ô†
    ,B.ZŠ AS Ž©ŽÐZŠ
    ,B.“d˜b”Ô† AS Ž©ŽÐ“d˜b”Ô†
    ,B.‚e‚`‚w AS Ž©ŽÐ‚e‚`‚w
    ,B.“o˜^”Ô†
    ,B.‹à—Z‹@ŠÖ‚b‚c1
    ,KKM1.‹âs–¼ AS ‹âs–¼1
    ,B.‹à—Z‹@ŠÖŽx“X‚b‚c1
    ,KKSM1.Žx“X–¼ AS Žx“X–¼1
    ,B.ŒûÀŽí•Ê1
    ,VK1.ŒûÀŽí•Ê–¼ AS ŒûÀŽí•Ê–¼1
    ,B.ŒûÀ”Ô†1
    ,B.ŒûÀ–¼‹`l1
    ,B.‹à—Z‹@ŠÖ‚b‚c2
    ,KKM2.‹âs–¼ AS ‹âs–¼2
    ,B.‹à—Z‹@ŠÖŽx“X‚b‚c2
    ,KKSM2.Žx“X–¼ AS Žx“X–¼2
    ,B.ŒûÀŽí•Ê2
    ,VK2.ŒûÀŽí•Ê–¼ AS ŒûÀŽí•Ê–¼2
    ,B.ŒûÀ”Ô†2
    ,B.ŒûÀ–¼‹`l2
    ,B.¿‹‘”õl1
    ,B.¿‹‘”õl2
    ,'' AS ‡ŽZ‘ÎÛ¿‹”Ô†
FROM
    ¿‹‡Œv AS G
    LEFT JOIN “¾ˆÓæƒ}ƒXƒ^ AS T
        ON  T.Šé‹Æ‚b‚c = G.Šé‹Æ‚b‚c
        AND T.“¾ˆÓæ‚b‚c = G.¿‹æ‚b‚c
    LEFT JOIN •”ƒ}ƒXƒ^ AS B ON
        B.Šé‹Æ‚b‚c = G.Šé‹Æ‚b‚c
        AND B.•”‚b‚c = G.•”‚b‚c
    LEFT JOIN V_¿‹‘ŒhÌ AS VS ON
        VS.¿‹‘ŒhÌCD = T.¿‹‘ŒhÌ
    LEFT JOIN ‹à—Z‹@ŠÖ–¼Ì AS KKM1 ON
        KKM1.‹âs‚b‚c = B.‹à—Z‹@ŠÖ‚b‚c1
    LEFT JOIN ‹à—Z‹@ŠÖŽx“X–¼Ì AS KKSM1 ON
        KKSM1.‹âs‚b‚c = B.‹à—Z‹@ŠÖ‚b‚c1
        AND KKSM1.Žx“X‚b‚c = B.‹à—Z‹@ŠÖŽx“X‚b‚c1
    LEFT JOIN V_ŒûÀŽí•Ê AS VK1 ON
        VK1.ŒûÀŽí•ÊCD = B.ŒûÀŽí•Ê1
    LEFT JOIN ‹à—Z‹@ŠÖ–¼Ì AS KKM2 ON
        KKM2.‹âs‚b‚c = B.‹à—Z‹@ŠÖ‚b‚c2
    LEFT JOIN ‹à—Z‹@ŠÖŽx“X–¼Ì AS KKSM2 ON
        KKSM2.‹âs‚b‚c = B.‹à—Z‹@ŠÖ‚b‚c2
        AND KKSM2.Žx“X‚b‚c = B.‹à—Z‹@ŠÖŽx“X‚b‚c2
    LEFT JOIN V_ŒûÀŽí•Ê AS VK2 ON
        VK2.ŒûÀŽí•ÊCD = B.ŒûÀŽí•Ê2
WHERE
    G.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
    AND G.¿‹”Ô† = @¿‹”Ô†
