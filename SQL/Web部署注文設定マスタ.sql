-- Web•”’•¶Ý’èƒ}ƒXƒ^
SELECT
    A.Šé‹Æ‚b‚c
    ,A.•”‚b‚c
    ,B.•”–¼
    ,W.Žó•tŠúŒÀŒŽ”
    ,W.’•¶‰Â”\ŠúŒÀ“ú”
    ,W.’•¶’÷ØŽžŠÔ
    ,W.’•¶’÷Ø•ª
    ,W.ì¬’S“–ŽÒ‚b‚c
    ,W.ì¬“ú
    ,W.C³’S“–ŽÒ‚b‚c
    ,W.C³“ú
FROM
    (SELECT
        Šé‹Æ‚b‚c
        ,•”‚b‚c
    FROM
        •”ƒ}ƒXƒ^
    WHERE
        Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
        AND ISNULL(–³Œøƒtƒ‰ƒO, 0) = 0
    UNION
    SELECT
        Šé‹Æ‚b‚c
        ,•”‚b‚c
    FROM
        WEB_M_•”’•¶Ý’è
    WHERE
        Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
    ) AS A
    LEFT JOIN WEB_M_•”’•¶Ý’è AS W ON A.Šé‹Æ‚b‚c = W.Šé‹Æ‚b‚c AND A.•”‚b‚c = W.•”‚b‚c
    LEFT JOIN •”ƒ}ƒXƒ^ AS B ON A.Šé‹Æ‚b‚c = B.Šé‹Æ‚b‚c AND A.•”‚b‚c = B.•”‚b‚c
ORDER BY
    A.•”‚b‚c
