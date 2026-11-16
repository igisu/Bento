-- “ü‹à“ü—Í_¿‹‡Œv
-- “ü‹àŠzi“ü‹à–¾×.“ü‹àŠz‚Ì‡Œvj‚Í•Ê“r’Šo
SELECT
    A.¿‹”Ô†
    ,A.¿‹“ú AS ¿‹“úFrom
    ,(SELECT
        ISNULL(MIN(B.¿‹“ú), '2016/08/01')
    FROM
        ¿‹‡Œv AS B
    WHERE
        B.Šé‹Æ‚b‚c = A.Šé‹Æ‚b‚c
        AND B.•”‚b‚c = A.•”‚b‚c
        AND B.¿‹æ‚b‚c = A.¿‹æ‚b‚c
        AND A.¿‹“ú < B.¿‹“ú
    ) AS ¿‹“úTo
    ,A.’÷‹æ•ª
    ,A.•”‚b‚c
    ,A.¿‹æ‚b‚c
    ,A.¿‹æ–¼
    ,A.‘O‰ñ¿‹Šz
    ,cast(0 AS decimal) AS “ü‹àŠz
    ,A.ŒJ‰zŠz
    ,A.”ƒã‹àŠz
    ,A.’²®Šz
    ,A.Á”ïÅ
    ,A.¡‰ñ¿‹Šz
    ,A.‰ñŽû—\’è“ú
    ,A.ˆóüŽÒ‚b‚c
    ,A.ˆóü“úŽž
FROM
    ¿‹‡Œv AS A
WHERE
    A.Šé‹Æ‚b‚c = 1
    AND A.¿‹“ú > '2015/07/01'
    AND A.¿‹æ‚b‚c = 4310
UNION
SELECT
    A.¿‹”Ô†
    ,A.¿‹“ú AS ¿‹“úFrom
    ,(SELECT
        ISNULL(MIN(B.¿‹“ú), '2016/08/01')
    FROM
        “dŽqŒˆÏ¿‹‡Œv AS B
    WHERE
        B.Šé‹Æ‚b‚c = A.Šé‹Æ‚b‚c
        AND B.•”‚b‚c = A.•”‚b‚c
        AND B.¿‹æ‚b‚c = A.¿‹æ‚b‚c
        AND A.¿‹“ú < B.¿‹“ú
    ) AS ¿‹“úTo
    ,A.’÷‹æ•ª
    ,A.•”‚b‚c
    ,A.¿‹æ‚b‚c
    ,A.¿‹æ–¼
    ,A.‘O‰ñ¿‹Šz
    ,cast(0 AS decimal) AS “ü‹àŠz
    ,A.ŒJ‰zŠz
    ,A.”ƒã‹àŠz
    ,A.’²®Šz
    ,A.Á”ïÅ
    ,A.¡‰ñ¿‹Šz
    ,A.‰ñŽû—\’è“ú
    ,A.ˆóüŽÒ‚b‚c
    ,A.ˆóü“úŽž
FROM
    “dŽqŒˆÏ¿‹‡Œv AS A
WHERE
    A.Šé‹Æ‚b‚c = 1
    AND A.¿‹“ú > '2015/07/01'
    AND A.¿‹æ‚b‚c = 4310
ORDER BY
    ¿‹“ú DESC
