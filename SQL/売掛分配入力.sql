-- ”„Š|•ª”z“ü—Í
-- •ª”zŒ³_¤•i”—Êi‰Šú’lj
SELECT
    0 AS s‹æ•ª
    ,M.“¾ˆÓæ‚b‚c
    ,CONVERT(varchar, M.“¾ˆÓæ‚b‚c) AS ”„ã‹æ•ª•t‚«“¾ˆÓæ‚b‚c
    ,'•ª”zŒ³‰Šú’l' AS “¾ˆÓæ–¼
    ,CONVERT(varchar, M.¤•i‚b‚c) + '_0' AS ¤•i‚b‚c
    ,M.”„ã‹æ•ª
    ,ISNULL(M.¤•i”—Ê, 0) AS ’l
FROM
    ”„ã–¾× AS M
WHERE
    M.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
    AND M.”„ã“ú   = @”„ã“ú
    AND M.“¾ˆÓæ‚b‚c = @“¾ˆÓæ‚b‚c
    AND M.•”‚b‚c = @•”‚b‚c
    AND M.”„ã‹æ•ª = @”„ã‹æ•ª”„Š|
UNION
-- •ª”zŒ³_•ª”zŒ³”—Êi‰Šú’lj
SELECT
    0 AS s‹æ•ª
    ,M.“¾ˆÓæ‚b‚c
    ,CONVERT(varchar, M.“¾ˆÓæ‚b‚c) AS ”„ã‹æ•ª•t‚«“¾ˆÓæ‚b‚c
    ,'•ª”zŒ³‰Šú’l' AS “¾ˆÓæ–¼
    ,CONVERT(varchar, M.¤•i‚b‚c) + '_1' AS ¤•i‚b‚c
    ,M.”„ã‹æ•ª
    ,ISNULL(M.•ª”zŒ³”—Ê, 0) AS ’l
FROM
    ”„ã–¾× AS M
WHERE
    M.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
    AND M.”„ã“ú   = @”„ã“ú
    AND M.“¾ˆÓæ‚b‚c = @“¾ˆÓæ‚b‚c
    AND M.•”‚b‚c = @•”‚b‚c
    AND M.”„ã‹æ•ª = @”„ã‹æ•ª”„Š|
UNION
-- •ª”zŒ³”Ž®s
SELECT
    1 AS s‹æ•ª
    ,M.“¾ˆÓæ‚b‚c
    ,CONVERT(varchar, M.“¾ˆÓæ‚b‚c) AS ”„ã‹æ•ª•t‚«“¾ˆÓæ‚b‚c
    ,'•ª”zŒ³”Ž®' AS “¾ˆÓæ–¼
    ,CONVERT(varchar, M.¤•i‚b‚c) + '_0' AS ¤•i‚b‚c
    ,M.”„ã‹æ•ª
    ,NULL AS ’l
FROM
    ”„ã–¾× AS M
WHERE
    M.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
    AND M.”„ã“ú   = @”„ã“ú
    AND M.“¾ˆÓæ‚b‚c = @“¾ˆÓæ‚b‚c
    AND M.•”‚b‚c = @•”‚b‚c
    AND M.”„ã‹æ•ª = @”„ã‹æ•ª”„Š|
UNION
SELECT
    1 AS s‹æ•ª
    ,M.“¾ˆÓæ‚b‚c
    ,CONVERT(varchar, M.“¾ˆÓæ‚b‚c) AS ”„ã‹æ•ª•t‚«“¾ˆÓæ‚b‚c
    ,'•ª”zŒ³”Ž®' AS “¾ˆÓæ–¼
    ,CONVERT(varchar, M.¤•i‚b‚c) + '_1' AS ¤•i‚b‚c
    ,M.”„ã‹æ•ª
    ,NULL AS ’l
FROM
    ”„ã–¾× AS M
WHERE
    M.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
    AND M.”„ã“ú   = @”„ã“ú
    AND M.“¾ˆÓæ‚b‚c = @“¾ˆÓæ‚b‚c
    AND M.•”‚b‚c = @•”‚b‚c
    AND M.”„ã‹æ•ª = @”„ã‹æ•ª”„Š|
UNION
-- •ª”zæŒÂ”
SELECT
    3 AS s‹æ•ª
    ,T.“¾ˆÓæ‚b‚c
    ,A.”„ã‹æ•ª•t‚«“¾ˆÓæ‚b‚c
    ,T.“¾ˆÓæ–¼
    ,A.¤•i‚b‚c
    ,A.”„ã‹æ•ª
    ,A.ŒÂ” AS ’l
FROM
    (SELECT
        “¾ˆÓæ‚b‚c
        ,“¾ˆÓæ–¼
    FROM
        “¾ˆÓæƒ}ƒXƒ^
    WHERE
        Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
        AND Žó’“¾ˆÓæ‚b‚c = @“¾ˆÓæ‚b‚c
    ) AS T
    LEFT JOIN (
        SELECT
            M.“¾ˆÓæ‚b‚c
            ,CONVERT(varchar, M.“¾ˆÓæ‚b‚c) + '_' + CONVERT(varchar, M.”„ã‹æ•ª)  AS ”„ã‹æ•ª•t‚«“¾ˆÓæ‚b‚c
            ,M.”„ã‹æ•ª
            ,CONVERT(varchar, M.¤•i‚b‚c) + '_0' AS ¤•i‚b‚c
            ,SUM(M.¤•i”—Ê) AS ŒÂ”
        FROM
            ”„ã–¾× AS M
        WHERE
            M.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
            AND M.”„ã“ú   = @”„ã“ú
            AND M.•”‚b‚c = @•”‚b‚c
        GROUP BY
            M.“¾ˆÓæ‚b‚c
            ,M.”„ã‹æ•ª
            ,M.¤•i‚b‚c
        ) AS A ON A.“¾ˆÓæ‚b‚c = T.“¾ˆÓæ‚b‚c
UNION
-- •ª”zæ”„ã‹æ•ª
SELECT
    3 AS s‹æ•ª
    ,T.“¾ˆÓæ‚b‚c
    ,A.”„ã‹æ•ª•t‚«“¾ˆÓæ‚b‚c
    ,T.“¾ˆÓæ–¼
    ,A.¤•i‚b‚c
    ,A.”„ã‹æ•ª
    ,A.”„ã‹æ•ª AS ’l
FROM
    (SELECT
        “¾ˆÓæ‚b‚c
        ,“¾ˆÓæ–¼
    FROM
        “¾ˆÓæƒ}ƒXƒ^
    WHERE
        Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
        AND Žó’“¾ˆÓæ‚b‚c = @“¾ˆÓæ‚b‚c
    ) AS T
    LEFT JOIN (
        SELECT DISTINCT
            M.“¾ˆÓæ‚b‚c
            ,CONVERT(varchar, M.“¾ˆÓæ‚b‚c) + '_' + CONVERT(varchar, M.”„ã‹æ•ª)  AS ”„ã‹æ•ª•t‚«“¾ˆÓæ‚b‚c
            ,M.”„ã‹æ•ª
            ,CONVERT(varchar, M.¤•i‚b‚c) + '_1' AS ¤•i‚b‚c
        FROM
            ”„ã–¾× AS M
        WHERE
            M.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
            AND M.”„ã“ú   = @”„ã“ú
            AND M.•”‚b‚c = @•”‚b‚c
        ) AS A ON A.“¾ˆÓæ‚b‚c = T.“¾ˆÓæ‚b‚c
ORDER BY
    s‹æ•ª
    ,“¾ˆÓæ‚b‚c
    ,”„ã‹æ•ª•t‚«“¾ˆÓæ‚b‚c
    ,¤•i‚b‚c
    ,”„ã‹æ•ª
