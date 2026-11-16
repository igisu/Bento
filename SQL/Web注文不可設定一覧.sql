-- Web’•¶•s‰ÂÝ’èˆê——
SELECT
    A.Šé‹Æ‚b‚c
    ,A.“¾ˆÓæ‚b‚c
    ,T.“¾ˆÓæ–¼
    ,A.’•¶•s‰Â“ú
    ,CASE WHEN H.“¾ˆÓæ‚b‚c IS NULL THEN '' ELSE '—L' END AS ’•¶—L–³
    ,A.ì¬’S“–ŽÒ‚b‚c
    ,A.ì¬“ú
    ,A.C³’S“–ŽÒ‚b‚c
    ,A.C³“ú
FROM
    WEB_M_’•¶•s‰ÂÝ’è AS A
    INNER JOIN “¾ˆÓæƒ}ƒXƒ^ AS T
        ON A.Šé‹Æ‚b‚c = T.Šé‹Æ‚b‚c
        AND A.“¾ˆÓæ‚b‚c = T.“¾ˆÓæ‚b‚c
        AND ISNULL(T.–³Œøƒtƒ‰ƒO, 0) = 0
    LEFT JOIN ’•¶ƒwƒbƒ_ AS H
        ON A.Šé‹Æ‚b‚c = H.Šé‹Æ‚b‚c
        AND A.’•¶•s‰Â“ú = H.‹iH“ú
        AND T.“¾ˆÓæ‚b‚c = H.“¾ˆÓæ‚b‚c
        AND T.•”‚b‚c = H.•”‚b‚c
        AND H.’•¶‹æ•ª = @’•¶‹æ•ª’•¶
WHERE
    A.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
    AND T.•”‚b‚c = @•”‚b‚c
    AND A.’•¶•s‰Â“ú >= @’•¶•s‰Â“úFrom
    AND A.’•¶•s‰Â“ú <= @’•¶•s‰Â“úTo
    AND A.“¾ˆÓæ‚b‚c = @“¾ˆÓæ‚b‚c
ORDER BY
    A.“¾ˆÓæ‚b‚c
    ,A.’•¶•s‰Â“ú


@Šé‹Æ‚b‚c = 40000
@•”‚b‚c = 1
@“¾ˆÓæ‚b‚c = 530
@’•¶•s‰Â“úFrom = '2017/08/01'
@’•¶•s‰Â“úTo = '2017/08/07'
@’•¶‹æ•ª’•¶ = 0
