-- •”EBÝ’èƒ}ƒXƒ^Žæ“¾
SELECT
    A.Šé‹Æ‚b‚c
    ,A.•”‚b‚c
    ,A.•”‚b‚c AS •”‚b‚corg
    ,A.‹âs‚b‚c
    ,A.‹âs‚b‚c AS ‹âs‚b‚corg
    ,K.‹âs–¼
    ,A.Žx“X‚b‚c
    ,A.Žx“X‚b‚c AS Žx“X‚b‚corg
    ,KB.Žx“X–¼
    ,A.ˆÏ‘õŽÒƒR[ƒh
    ,A.ˆÏ‘õŽÒ–¼
    ,A.—a‹àŽí–Ú
    ,A.ŒûÀ”Ô†
    ,A.EBƒZƒbƒg–¼
    ,A.ì¬’S“–ŽÒ‚b‚c
    ,A1.’S“–ŽÒ–¼ AS ì¬’S“–ŽÒ–¼
    ,A.ì¬“ú
    ,A.C³’S“–ŽÒ‚b‚c
    ,A2.’S“–ŽÒ–¼ AS C³’S“–ŽÒ–¼
    ,A.C³“ú
FROM
    •”EBÝ’èƒ}ƒXƒ^ AS A
    LEFT JOIN ‹à—Z‹@ŠÖ–¼Ì AS K
        ON A.‹âs‚b‚c = K.‹âs‚b‚c
    LEFT JOIN ‹à—Z‹@ŠÖŽx“X–¼Ì AS KB
        ON A.‹âs‚b‚c = KB.‹âs‚b‚c
        AND A.Žx“X‚b‚c = KB.Žx“X‚b‚c
    LEFT JOIN ’S“–ŽÒƒ}ƒXƒ^ AS A1 ON A1.Šé‹Æ‚b‚c = A.Šé‹Æ‚b‚c AND A.ì¬’S“–ŽÒ‚b‚c = A1.’S“–ŽÒ‚b‚c
    LEFT JOIN ’S“–ŽÒƒ}ƒXƒ^ AS A2 ON A2.Šé‹Æ‚b‚c = A.Šé‹Æ‚b‚c AND A.C³’S“–ŽÒ‚b‚c = A2.’S“–ŽÒ‚b‚c
WHERE
    A.Šé‹Æ‚b‚c = 40001
    AND A.•”‚b‚c = 1
ORDER BY
    A.‹âs‚b‚c
    ,A.Žx“X‚b‚c
