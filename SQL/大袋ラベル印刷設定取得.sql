-- ‘å‘Üƒ‰ƒxƒ‹ˆóüÝ’èŽæ“¾
SELECT
    OS.Šé‹Æ‚b‚c
    ,OS.•”‚b‚c
    ,B.•”–¼
    ,OS.‘å‘Üƒ‰ƒxƒ‹ˆóüŽí•Ê‚b‚c
    ,VP.‘å‘Üƒ‰ƒxƒ‹ˆóüŽí•Ê–¼
    ,OS.‘å‘Üƒ‰ƒxƒ‹HŽ–‹æ•ª‚b‚c
    ,OS.‘å‘Üƒ‰ƒxƒ‹HŽ–‹æ•ª‚b‚c AS ‘å‘Üƒ‰ƒxƒ‹HŽ–‹æ•ª‚b‚corg
    ,VS.‘å‘Üƒ‰ƒxƒ‹HŽ–‹æ•ª–¼
    ,OS.¤•i‚b‚c
    ,OS.¤•i‚b‚c AS ¤•i‚b‚corg
    ,S.¤•i–¼
    ,OS.ì¬’S“–ŽÒ‚b‚c
    ,A1.’S“–ŽÒ–¼ AS ì¬’S“–ŽÒ–¼
    ,OS.ì¬“ú
    ,OS.C³’S“–ŽÒ‚b‚c
    ,A2.’S“–ŽÒ–¼ AS C³’S“–ŽÒ–¼
    ,OS.C³“ú
FROM
    ‘å‘Üƒ‰ƒxƒ‹ˆóüÝ’è AS OS
    LEFT JOIN V_‘å‘Üƒ‰ƒxƒ‹ˆóüŽí•Ê AS VP ON OS.‘å‘Üƒ‰ƒxƒ‹ˆóüŽí•Ê‚b‚c = VP.‘å‘Üƒ‰ƒxƒ‹ˆóüŽí•Ê‚b‚c
    LEFT JOIN V_‘å‘Üƒ‰ƒxƒ‹HŽ–‹æ•ª AS VS ON OS.‘å‘Üƒ‰ƒxƒ‹HŽ–‹æ•ª‚b‚c = VS.‘å‘Üƒ‰ƒxƒ‹HŽ–‹æ•ª‚b‚c
    LEFT JOIN •”ƒ}ƒXƒ^ AS B ON OS.Šé‹Æ‚b‚c = B.Šé‹Æ‚b‚c AND OS.•”‚b‚c = B.•”‚b‚c
    LEFT JOIN ¤•iƒ}ƒXƒ^ AS S ON OS.Šé‹Æ‚b‚c = S.Šé‹Æ‚b‚c AND OS.¤•i‚b‚c = S.¤•i‚b‚c
    LEFT JOIN ’S“–ŽÒƒ}ƒXƒ^ AS A1 ON OS.Šé‹Æ‚b‚c = A1.Šé‹Æ‚b‚c AND OS.ì¬’S“–ŽÒ‚b‚c = A1.’S“–ŽÒ‚b‚c
    LEFT JOIN ’S“–ŽÒƒ}ƒXƒ^ AS A2 ON OS.Šé‹Æ‚b‚c = A2.Šé‹Æ‚b‚c AND OS.C³’S“–ŽÒ‚b‚c = A2.’S“–ŽÒ‚b‚c
WHERE
    OS.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
    AND OS.•”‚b‚c = @•”‚b‚c
    AND OS.‘å‘Üƒ‰ƒxƒ‹ˆóüŽí•Ê‚b‚c = @‘å‘Üƒ‰ƒxƒ‹ˆóüŽí•Ê‚b‚c
ORDER BY
    OS.‘å‘Üƒ‰ƒxƒ‹HŽ–‹æ•ª‚b‚c
    ,OS.¤•i‚b‚c
