-- ¿‹EBƒf[ƒ^ì¬
SELECT
    G.Šé‹Æ‚b‚c
    ,G.¿‹”Ô†
    ,G.¿‹“ú
    ,G.’÷‹æ•ª
    ,G.•”‚b‚c
    ,G.¿‹æ‚b‚c
    ,G.¿‹æ–¼
    ,G.‘O‰ñ¿‹Šz
    ,G.“ü‹àŠz
    ,G.ŒJ‰zŠz
    ,G.”ƒã‹àŠz
    ,G.’²®Šz
    ,G.Á”ïÅ
    ,G.¡‰ñ¿‹Šz
    ,G.‰ñŽû—\’è“ú
    ,G.ˆóüŽÒ‚b‚c
    ,G.ˆóü“úŽž
    ,G.EBì¬Ïƒtƒ‰ƒO
    ,G.ì¬’S“–ŽÒ‚b‚c
    ,G.ì¬“ú
    ,G.C³’S“–ŽÒ‚b‚c
    ,G.C³“ú
    ,T.“¾ˆÓæ–¼
    ,T.‹à—Z‹@ŠÖ‚b‚c AS ‹âs‚b‚c
    ,K.‹âs–¼ƒJƒi
    ,T.‹à—Z‹@ŠÖŽx“X‚b‚c AS Žx“X‚b‚c
    ,KB.Žx“X–¼ƒJƒi
    ,T.ŒûÀŽí•Ê
    ,T.ŒûÀ”Ô†
    ,T.ŒûÀ–¼‹`l
    ,G.¡‰ñ¿‹Šz AS ¿‹‹àŠz
    ,G.¿‹æ‚b‚c AS “¾ˆÓæ‚b‚c
FROM
    ¿‹‡Œv AS G
    INNER JOIN “¾ˆÓæƒ}ƒXƒ^ AS T
        ON G.Šé‹Æ‚b‚c = T.Šé‹Æ‚b‚c
        AND G.¿‹æ‚b‚c = T.“¾ˆÓæ‚b‚c
    INNER JOIN ‹à—Z‹@ŠÖ–¼Ì AS K ON K.‹âs‚b‚c = T.‹à—Z‹@ŠÖ‚b‚c
    INNER JOIN ‹à—Z‹@ŠÖŽx“X–¼Ì AS KB ON KB.‹âs‚b‚c = T.‹à—Z‹@ŠÖ‚b‚c AND KB.Žx“X‚b‚c = T.‹à—Z‹@ŠÖŽx“X‚b‚c
WHERE
    G.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
    AND T.’÷‹æ•ª = @’÷‹æ•ªŒŽ’÷
    AND T.ŒŽ’÷“ú = @ŒŽ’÷“ú
    AND T.Žx•¥•û–@ = @Žx•¥•û–@U‘Ö
    AND T.EB‹âsŽx“X‚b‚c = @‹âsŽx“X‚b‚c
    AND G.’÷‹æ•ª = @’÷‹æ•ªŒŽ’÷
    AND G.•”‚b‚c = @•”‚b‚c
    AND G.¿‹æ‚b‚c >= @¿‹æ‚b‚cFrom
    AND G.¿‹æ‚b‚c <= @¿‹æ‚b‚cTo
    AND G.¿‹“ú >= @¿‹”NŒŽ“úFrom
    AND G.¿‹“ú <= @¿‹”NŒŽ“úTo
--  AND G.EBì¬Ïƒtƒ‰ƒO = 0
    AND G.¡‰ñ¿‹Šz > 0
ORDER BY
    G.¿‹æ‚b‚c,
    G.¡‰ñ¿‹Šz
