-- ’ZûˆóüiWŒvÝ’èj
SELECT
    TS.Šé‹Æ‚b‚c
    ,TS.•”‚b‚c
    ,TS.WŒv‹æ•ªCD
    ,TS.WŒvˆóü–¼
    ,TS.HŽ–‹æ•ªCD
    ,VS.HŽ–‹æ•ª–¼
    ,TS.‘ÜŒvŽZƒtƒ‰ƒO
    ,TS.ˆóü‡
    ,TS.ì¬’S“–ŽÒ‚b‚c
    ,A1.’S“–ŽÒ–¼ AS ì¬’S“–ŽÒ–¼
    ,TS.ì¬“ú
    ,TS.C³’S“–ŽÒ‚b‚c
    ,A2.’S“–ŽÒ–¼ AS C³’S“–ŽÒ–¼
    ,TS.C³“ú
FROM
    ’ZûWŒvÝ’èƒ}ƒXƒ^ AS TS
    LEFT JOIN V_HŽ–‹æ•ª   AS VS ON VS.HŽ–‹æ•ªCD = TS.HŽ–‹æ•ªCD
    LEFT JOIN ’S“–ŽÒƒ}ƒXƒ^ AS A1 ON A1.Šé‹Æ‚b‚c = TS.Šé‹Æ‚b‚c AND TS.ì¬’S“–ŽÒ‚b‚c = A1.’S“–ŽÒ‚b‚c
    LEFT JOIN ’S“–ŽÒƒ}ƒXƒ^ AS A2 ON A2.Šé‹Æ‚b‚c = TS.Šé‹Æ‚b‚c AND TS.C³’S“–ŽÒ‚b‚c = A2.’S“–ŽÒ‚b‚c
WHERE
    TS.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
    AND TS.•”‚b‚c = @•”‚b‚c
ORDER BY
    TS.ˆóü‡
