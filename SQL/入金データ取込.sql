-- “ü‹àƒf[ƒ^Žæž
SELECT
    CAST('2015/06/15' AS DATETIME) as “ü‹à“ú
    ,A.¿‹”Ô†
    ,A.•”‚b‚c
    ,B.•”–¼
    ,A.¿‹æ‚b‚c
    ,A.¿‹æ–¼
    ,NULL AS ŠÇ—No
    ,A.¡‰ñ¿‹Šz
    ,CAST (0 AS DECIMAL) AS “ü‹àŠz
    ,'' AS “E—v
    ,CAST (0 AS DECIMAL) AS ”„Š|Žc
FROM
    ¿‹‡Œv AS A
    LEFT JOIN •”ƒ}ƒXƒ^ AS B ON B.Šé‹Æ‚b‚c = A.Šé‹Æ‚b‚c AND B.•”‚b‚c = A.•”‚b‚c
WHERE
    A.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
    AND A.¿‹”Ô† IN (1711,1712,1713)
ORDER BY
    A.¿‹”Ô†
