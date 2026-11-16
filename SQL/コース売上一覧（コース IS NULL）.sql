-- ”„ãˆê——iƒR[ƒX IS NULLj
-- ƒR[ƒXƒe[ƒuƒ‹‚ÍŒ©‚È‚¢
--- ”„ãŒ»‹àŒÂ”
SELECT
    0 AS –¾×‡Œv‹æ•ª,
    MAIN.“¾ˆÓæ‚b‚c,
    T.“¾ˆÓæ–¼,
    CONVERT(varchar, MAIN.“¾ˆÓæ‚b‚c) + ' ' + ISNULL(V.”„Š|Œ»‹à‹æ•ª–¼, ' ') + CHAR(10) + ISNULL(T.“¾ˆÓæ–¼, '') AS •\Ž¦—p“¾ˆÓæ–¼,
    VT.”„Š|Œ»‹à‹æ•ªCD AS ”„Š|Œ»‹à‹æ•ª,
    VT.”„Š|Œ»‹à‹æ•ª–¼ AS ƒ^ƒCƒv,
    A.¤•i‚b‚c,
    A.ŒÂ” AS ’l
FROM
    -- ‹ì“®•\i”„ã–¾×j
    (SELECT DISTINCT
        M.Šé‹Æ‚b‚c
        ,M.”„ã“ú
        ,M.“¾ˆÓæ‚b‚c
        ,M.•”‚b‚c
        ,CONVERT(varchar, M.¤•i‚b‚c) + '_0' AS ¤•i‚b‚c
    FROM
        ”„ã–¾× AS M
        INNER JOIN ”„ãƒwƒbƒ_ AS H
            ON  H.Šé‹Æ‚b‚c   = M.Šé‹Æ‚b‚c
            AND H.”„ã“ú     = M.”„ã“ú
            AND H.“¾ˆÓæ‚b‚c = M.“¾ˆÓæ‚b‚c
            AND H.•”‚b‚c   = M.•”‚b‚c
        LEFT JOIN Žå—v¤•iƒŠƒXƒgƒ}ƒXƒ^ AS A
            ON A.Šé‹Æ‚b‚c = M.Šé‹Æ‚b‚c
            AND A.¤•i‚b‚c = M.¤•i‚b‚c
            AND A.•”‚b‚c = M.•”‚b‚c
            AND ISNULL(A.–³Œøƒtƒ‰ƒO, 0) = 0
    WHERE
        H.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c AND
        H.”„ã“ú = @”„ã“ú AND
        H.•”‚b‚c = @•”‚b‚c AND
        H.ƒR[ƒX‚b‚c IS NULL
    ) AS MAIN
    LEFT JOIN
    -- ”„ã–¾×iŒ»‹àjiƒŒƒR[ƒh‚ªƒqƒbƒg‚µ‚È‚­‚Ä‚à“¾ˆÓæ–¼‚Æƒ^ƒCƒv‚Í•\Ž¦‚³‚ê‚éj
    (SELECT
        M.Šé‹Æ‚b‚c
        ,M.”„ã“ú
        ,T.Žó’“¾ˆÓæ‚b‚c AS “¾ˆÓæ‚b‚c
        ,M.•”‚b‚c
        ,CONVERT(varchar, M.¤•i‚b‚c) + '_0' AS ¤•i‚b‚c
        ,SUM(M.¤•i”—Ê) AS ŒÂ”
    FROM
        ”„ã–¾× AS M
        -- •ª”zæ‚Ì”„ã‚Í•ª”zŒ³‚Æ‚µ‚ÄŒvã‚µ‚½‚¢‚Ì‚ÅA“¾ˆÓæ‚b‚c‚ð•ª”zŒ³‚Ì‚»‚ê‚É‚·‚è‘Ö‚¦‚é
        -- i•ª”z‚·‚é‚±‚Æ‚É‚æ‚Á‚Ä•ª”zæ‚ÌƒR[ƒX‚Í•ª”zŒ³‚Æ“¯‚¶‚É‚È‚Á‚Ä‚¢‚é‚Ì‚ÅA•ª”zŒ³‚Æ•ª”zæ‚ªˆê‚É’Šo‚³‚ê‚éj
        INNER JOIN (
            SELECT
                Šé‹Æ‚b‚c
                ,“¾ˆÓæ‚b‚c
                ,ISNULL(Žó’“¾ˆÓæ‚b‚c, “¾ˆÓæ‚b‚c) AS Žó’“¾ˆÓæ‚b‚c
            FROM
                “¾ˆÓæƒ}ƒXƒ^
            ) AS T
            ON T.Šé‹Æ‚b‚c  = M.Šé‹Æ‚b‚c
            AND T.“¾ˆÓæ‚b‚c = M.“¾ˆÓæ‚b‚c
        INNER JOIN ”„ãƒwƒbƒ_ AS H
            ON  H.Šé‹Æ‚b‚c   = M.Šé‹Æ‚b‚c
            AND H.”„ã“ú     = M.”„ã“ú
            AND H.“¾ˆÓæ‚b‚c = M.“¾ˆÓæ‚b‚c
            AND H.•”‚b‚c   = M.•”‚b‚c
    WHERE
        M.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c AND
        M.”„ã‹æ•ª NOT IN (@”„ã‹æ•ª”„Š|, @”„ã‹æ•ªLINE) AND        -- Œ»‹àAƒ`ƒPƒbƒgAƒo[ƒNƒŒ[Aƒ{[ƒiƒX‚ÍŒ»‹à‚ÆŒ©‚È‚·
        M.”„ã“ú   = @”„ã“ú AND
        M.•”‚b‚c = @•”‚b‚c AND
        H.ƒR[ƒX‚b‚c IS NULL
    GROUP BY
        M.Šé‹Æ‚b‚c
        ,M.”„ã“ú
        ,T.Žó’“¾ˆÓæ‚b‚c
        ,M.•”‚b‚c
        ,M.¤•i‚b‚c
    ) AS A
        ON MAIN.Šé‹Æ‚b‚c    = A.Šé‹Æ‚b‚c
        AND MAIN.”„ã“ú     = A.”„ã“ú
        AND MAIN.“¾ˆÓæ‚b‚c = A.“¾ˆÓæ‚b‚c
        AND MAIN.¤•i‚b‚c   = A.¤•i‚b‚c
        AND MAIN.•”‚b‚c   = A.•”‚b‚c
    LEFT JOIN “¾ˆÓæƒ}ƒXƒ^ AS T ON T.Šé‹Æ‚b‚c = MAIN.Šé‹Æ‚b‚c AND T.“¾ˆÓæ‚b‚c = MAIN.“¾ˆÓæ‚b‚c
    LEFT JOIN V_”„Š|Œ»‹à‹æ•ª AS V ON V.”„Š|Œ»‹à‹æ•ªCD = T.”„Š|Œ»‹à‹æ•ª
    LEFT JOIN V_”„Š|Œ»‹à‹æ•ª AS VT ON VT.”„Š|Œ»‹à‹æ•ªCD = @”„Š|Œ»‹à‹æ•ªŒ»‹à
UNION
--- ”„ãŒ»‹à‹àŠz
SELECT
    0 AS –¾×‡Œv‹æ•ª,
    MAIN.“¾ˆÓæ‚b‚c,
    T.“¾ˆÓæ–¼,
    CONVERT(varchar, MAIN.“¾ˆÓæ‚b‚c) + ' ' + ISNULL(V.”„Š|Œ»‹à‹æ•ª–¼, ' ') + CHAR(10) + ISNULL(T.“¾ˆÓæ–¼, '') AS •\Ž¦—p“¾ˆÓæ–¼,
    VT.”„Š|Œ»‹à‹æ•ªCD AS ”„Š|Œ»‹à‹æ•ª,
    VT.”„Š|Œ»‹à‹æ•ª–¼ AS ƒ^ƒCƒv,
    A.¤•i‚b‚c,
    A.‹àŠz AS ’l
FROM
    -- ‹ì“®•\i”„ã–¾×j
    (SELECT DISTINCT
        M.Šé‹Æ‚b‚c
        ,M.”„ã“ú
        ,M.“¾ˆÓæ‚b‚c
        ,M.•”‚b‚c
        ,CONVERT(varchar, M.¤•i‚b‚c) + '_1' AS ¤•i‚b‚c
    FROM
        ”„ã–¾× AS M
        INNER JOIN ”„ãƒwƒbƒ_ AS H
            ON  H.Šé‹Æ‚b‚c   = M.Šé‹Æ‚b‚c
            AND H.”„ã“ú     = M.”„ã“ú
            AND H.“¾ˆÓæ‚b‚c = M.“¾ˆÓæ‚b‚c
            AND H.•”‚b‚c   = M.•”‚b‚c
        LEFT JOIN Žå—v¤•iƒŠƒXƒgƒ}ƒXƒ^ AS A
            ON A.Šé‹Æ‚b‚c = M.Šé‹Æ‚b‚c
            AND A.¤•i‚b‚c = M.¤•i‚b‚c
            AND A.•”‚b‚c = M.•”‚b‚c
            AND ISNULL(A.–³Œøƒtƒ‰ƒO, 0) = 0
    WHERE
        H.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c AND
        H.”„ã“ú = @”„ã“ú AND
        H.•”‚b‚c = @•”‚b‚c AND
        H.ƒR[ƒX‚b‚c IS NULL
    ) AS MAIN
    LEFT JOIN
    -- ”„ã–¾×iŒ»‹àjiƒŒƒR[ƒh‚ªƒqƒbƒg‚µ‚È‚­‚Ä‚à“¾ˆÓæ–¼‚Æƒ^ƒCƒv‚Í•\Ž¦‚³‚ê‚éj
    (SELECT
        M.Šé‹Æ‚b‚c
        ,M.”„ã“ú
        ,T.Žó’“¾ˆÓæ‚b‚c AS “¾ˆÓæ‚b‚c
        ,M.•”‚b‚c
        ,CONVERT(varchar, M.¤•i‚b‚c) + '_1' AS ¤•i‚b‚c
        ,SUM(M.¤•i‹àŠz‡ŒvÅž) AS ‹àŠz
    FROM
        ”„ã–¾× AS M
        -- •ª”zæ‚Ì”„ã‚Í•ª”zŒ³‚Æ‚µ‚ÄŒvã‚µ‚½‚¢‚Ì‚ÅA“¾ˆÓæ‚b‚c‚ð•ª”zŒ³‚Ì‚»‚ê‚É‚·‚è‘Ö‚¦‚é
        -- i•ª”z‚·‚é‚±‚Æ‚É‚æ‚Á‚Ä•ª”zæ‚ÌƒR[ƒX‚Í•ª”zŒ³‚Æ“¯‚¶‚É‚È‚Á‚Ä‚¢‚é‚Ì‚ÅA•ª”zŒ³‚Æ•ª”zæ‚ªˆê‚É’Šo‚³‚ê‚éj
        INNER JOIN (
            SELECT
                Šé‹Æ‚b‚c
                ,“¾ˆÓæ‚b‚c
                ,ISNULL(Žó’“¾ˆÓæ‚b‚c, “¾ˆÓæ‚b‚c) AS Žó’“¾ˆÓæ‚b‚c
            FROM
                “¾ˆÓæƒ}ƒXƒ^
            ) AS T
            ON T.Šé‹Æ‚b‚c  = M.Šé‹Æ‚b‚c
            AND T.“¾ˆÓæ‚b‚c = M.“¾ˆÓæ‚b‚c
        INNER JOIN ”„ãƒwƒbƒ_ AS H
            ON  H.Šé‹Æ‚b‚c   = M.Šé‹Æ‚b‚c
            AND H.”„ã“ú     = M.”„ã“ú
            AND H.“¾ˆÓæ‚b‚c = M.“¾ˆÓæ‚b‚c
            AND H.•”‚b‚c   = M.•”‚b‚c
    WHERE
        M.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c AND
        M.”„ã‹æ•ª NOT IN (@”„ã‹æ•ª”„Š|, @”„ã‹æ•ªLINE) AND        -- Œ»‹àAƒ`ƒPƒbƒgAƒo[ƒNƒŒ[Aƒ{[ƒiƒX‚ÍŒ»‹à‚ÆŒ©‚È‚·
        M.”„ã“ú   = @”„ã“ú AND
        M.•”‚b‚c = @•”‚b‚c AND
        H.ƒR[ƒX‚b‚c IS NULL
    GROUP BY
        M.Šé‹Æ‚b‚c
        ,M.”„ã“ú
        ,T.Žó’“¾ˆÓæ‚b‚c
        ,M.•”‚b‚c
        ,M.¤•i‚b‚c
    ) AS A
        ON MAIN.Šé‹Æ‚b‚c    = A.Šé‹Æ‚b‚c
        AND MAIN.”„ã“ú     = A.”„ã“ú
        AND MAIN.“¾ˆÓæ‚b‚c = A.“¾ˆÓæ‚b‚c
        AND MAIN.¤•i‚b‚c   = A.¤•i‚b‚c
        AND MAIN.•”‚b‚c   = A.•”‚b‚c
    LEFT JOIN “¾ˆÓæƒ}ƒXƒ^ AS T ON T.Šé‹Æ‚b‚c = MAIN.Šé‹Æ‚b‚c AND T.“¾ˆÓæ‚b‚c = MAIN.“¾ˆÓæ‚b‚c
    LEFT JOIN V_”„Š|Œ»‹à‹æ•ª AS V ON V.”„Š|Œ»‹à‹æ•ªCD = T.”„Š|Œ»‹à‹æ•ª
    LEFT JOIN V_”„Š|Œ»‹à‹æ•ª AS VT ON VT.”„Š|Œ»‹à‹æ•ªCD = @”„Š|Œ»‹à‹æ•ªŒ»‹à
UNION
--- ”„ã”„Š|ŒÂ”
SELECT
    0 AS –¾×‡Œv‹æ•ª,
    MAIN.“¾ˆÓæ‚b‚c,
    T.“¾ˆÓæ–¼,
    CONVERT(varchar, MAIN.“¾ˆÓæ‚b‚c) + ' ' + ISNULL(V.”„Š|Œ»‹à‹æ•ª–¼, ' ') + CHAR(10) + ISNULL(T.“¾ˆÓæ–¼, '') AS •\Ž¦—p“¾ˆÓæ–¼,
    VT.”„Š|Œ»‹à‹æ•ªCD AS ”„Š|Œ»‹à‹æ•ª,
    VT.”„Š|Œ»‹à‹æ•ª–¼ AS ƒ^ƒCƒv,
    A.¤•i‚b‚c,
    A.ŒÂ” AS ’l
FROM
    -- ‹ì“®•\i”„ã–¾×j
    (SELECT DISTINCT
        M.Šé‹Æ‚b‚c
        ,M.”„ã“ú
        ,M.“¾ˆÓæ‚b‚c
        ,M.•”‚b‚c
        ,CONVERT(varchar, M.¤•i‚b‚c) + '_0' AS ¤•i‚b‚c
    FROM
        ”„ã–¾× AS M
        INNER JOIN ”„ãƒwƒbƒ_ AS H
            ON  H.Šé‹Æ‚b‚c   = M.Šé‹Æ‚b‚c
            AND H.”„ã“ú     = M.”„ã“ú
            AND H.“¾ˆÓæ‚b‚c = M.“¾ˆÓæ‚b‚c
            AND H.•”‚b‚c   = M.•”‚b‚c
        LEFT JOIN Žå—v¤•iƒŠƒXƒgƒ}ƒXƒ^ AS A
            ON A.Šé‹Æ‚b‚c = M.Šé‹Æ‚b‚c
            AND A.¤•i‚b‚c = M.¤•i‚b‚c
            AND A.•”‚b‚c = M.•”‚b‚c
            AND ISNULL(A.–³Œøƒtƒ‰ƒO, 0) = 0
    WHERE
        H.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c AND
        H.”„ã“ú = @”„ã“ú AND
        H.•”‚b‚c = @•”‚b‚c AND
        H.ƒR[ƒX‚b‚c IS NULL
    ) AS MAIN
    LEFT JOIN
    -- ”„ã–¾×i”„Š|jiƒŒƒR[ƒh‚ªƒqƒbƒg‚µ‚È‚­‚Ä‚à“¾ˆÓæ–¼‚Æƒ^ƒCƒv‚Í•\Ž¦‚³‚ê‚éj
    (SELECT
        M.Šé‹Æ‚b‚c
        ,M.”„ã“ú
        ,T.Žó’“¾ˆÓæ‚b‚c AS “¾ˆÓæ‚b‚c
        ,M.•”‚b‚c
        ,CONVERT(varchar, M.¤•i‚b‚c) + '_0' AS ¤•i‚b‚c
        ,SUM(M.¤•i”—Ê) AS ŒÂ”
    FROM
        ”„ã–¾× AS M
        -- •ª”zæ‚Ì”„ã‚Í•ª”zŒ³‚Æ‚µ‚ÄŒvã‚µ‚½‚¢‚Ì‚ÅA“¾ˆÓæ‚b‚c‚ð•ª”zŒ³‚Ì‚»‚ê‚É‚·‚è‘Ö‚¦‚é
        -- i•ª”z‚·‚é‚±‚Æ‚É‚æ‚Á‚Ä•ª”zæ‚ÌƒR[ƒX‚Í•ª”zŒ³‚Æ“¯‚¶‚É‚È‚Á‚Ä‚¢‚é‚Ì‚ÅA•ª”zŒ³‚Æ•ª”zæ‚ªˆê‚É’Šo‚³‚ê‚éj
        INNER JOIN (
            SELECT
                Šé‹Æ‚b‚c
                ,“¾ˆÓæ‚b‚c
                ,ISNULL(Žó’“¾ˆÓæ‚b‚c, “¾ˆÓæ‚b‚c) AS Žó’“¾ˆÓæ‚b‚c
            FROM
                “¾ˆÓæƒ}ƒXƒ^
            ) AS T
            ON T.Šé‹Æ‚b‚c  = M.Šé‹Æ‚b‚c
            AND T.“¾ˆÓæ‚b‚c = M.“¾ˆÓæ‚b‚c
        INNER JOIN ”„ãƒwƒbƒ_ AS H
            ON  H.Šé‹Æ‚b‚c   = M.Šé‹Æ‚b‚c
            AND H.”„ã“ú     = M.”„ã“ú
            AND H.“¾ˆÓæ‚b‚c = M.“¾ˆÓæ‚b‚c
            AND H.•”‚b‚c   = M.•”‚b‚c
    WHERE
        M.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c AND
        M.”„ã‹æ•ª IN (@”„ã‹æ•ª”„Š|, @”„ã‹æ•ªLINE) AND        -- LINEŒˆÏ‚Í”„Š|‚ÆŒ©‚È‚·
        M.”„ã“ú   = @”„ã“ú AND
        M.•”‚b‚c = @•”‚b‚c AND
        H.ƒR[ƒX‚b‚c IS NULL
    GROUP BY
        M.Šé‹Æ‚b‚c
        ,M.”„ã“ú
        ,T.Žó’“¾ˆÓæ‚b‚c
        ,M.•”‚b‚c
        ,M.¤•i‚b‚c
    ) AS A
        ON MAIN.Šé‹Æ‚b‚c    = A.Šé‹Æ‚b‚c
        AND MAIN.”„ã“ú     = A.”„ã“ú
        AND MAIN.“¾ˆÓæ‚b‚c = A.“¾ˆÓæ‚b‚c
        AND MAIN.¤•i‚b‚c   = A.¤•i‚b‚c
        AND MAIN.•”‚b‚c   = A.•”‚b‚c
    LEFT JOIN “¾ˆÓæƒ}ƒXƒ^ AS T ON T.Šé‹Æ‚b‚c = MAIN.Šé‹Æ‚b‚c AND T.“¾ˆÓæ‚b‚c = MAIN.“¾ˆÓæ‚b‚c
    LEFT JOIN V_”„Š|Œ»‹à‹æ•ª AS V ON V.”„Š|Œ»‹à‹æ•ªCD = T.”„Š|Œ»‹à‹æ•ª
    LEFT JOIN V_”„Š|Œ»‹à‹æ•ª AS VT ON VT.”„Š|Œ»‹à‹æ•ªCD = @”„Š|Œ»‹à‹æ•ª”„Š|
UNION
--- ”„ã”„Š|‹àŠz
SELECT
    0 AS –¾×‡Œv‹æ•ª,
    MAIN.“¾ˆÓæ‚b‚c,
    T.“¾ˆÓæ–¼,
    CONVERT(varchar, MAIN.“¾ˆÓæ‚b‚c) + ' ' + ISNULL(V.”„Š|Œ»‹à‹æ•ª–¼, ' ') + CHAR(10) + ISNULL(T.“¾ˆÓæ–¼, '') AS •\Ž¦—p“¾ˆÓæ–¼,
    VT.”„Š|Œ»‹à‹æ•ªCD AS ”„Š|Œ»‹à‹æ•ª,
    VT.”„Š|Œ»‹à‹æ•ª–¼ AS ƒ^ƒCƒv,
    A.¤•i‚b‚c,
    A.‹àŠz AS ’l
FROM
    -- ‹ì“®•\i”„ã–¾×j
    (SELECT DISTINCT
        M.Šé‹Æ‚b‚c
        ,M.”„ã“ú
        ,M.“¾ˆÓæ‚b‚c
        ,M.•”‚b‚c
        ,CONVERT(varchar, M.¤•i‚b‚c) + '_1' AS ¤•i‚b‚c
    FROM
        ”„ã–¾× AS M
        INNER JOIN ”„ãƒwƒbƒ_ AS H
            ON  H.Šé‹Æ‚b‚c   = M.Šé‹Æ‚b‚c
            AND H.”„ã“ú     = M.”„ã“ú
            AND H.“¾ˆÓæ‚b‚c = M.“¾ˆÓæ‚b‚c
            AND H.•”‚b‚c   = M.•”‚b‚c
        LEFT JOIN Žå—v¤•iƒŠƒXƒgƒ}ƒXƒ^ AS A
            ON A.Šé‹Æ‚b‚c = M.Šé‹Æ‚b‚c
            AND A.¤•i‚b‚c = M.¤•i‚b‚c
            AND A.•”‚b‚c = M.•”‚b‚c
            AND ISNULL(A.–³Œøƒtƒ‰ƒO, 0) = 0
    WHERE
        H.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c AND
        H.”„ã“ú = @”„ã“ú AND
        H.•”‚b‚c = @•”‚b‚c AND
        H.ƒR[ƒX‚b‚c IS NULL
    ) AS MAIN
    LEFT JOIN
    -- ”„ã–¾×i”„Š|jiƒŒƒR[ƒh‚ªƒqƒbƒg‚µ‚È‚­‚Ä‚à“¾ˆÓæ–¼‚Æƒ^ƒCƒv‚Í•\Ž¦‚³‚ê‚éj
    (SELECT
        M.Šé‹Æ‚b‚c
        ,M.”„ã“ú
        ,T.Žó’“¾ˆÓæ‚b‚c AS “¾ˆÓæ‚b‚c
        ,M.•”‚b‚c
        ,CONVERT(varchar, M.¤•i‚b‚c) + '_1' AS ¤•i‚b‚c
        ,SUM(M.¤•i‹àŠz‡ŒvÅž) AS ‹àŠz
    FROM
        ”„ã–¾× AS M
        -- •ª”zæ‚Ì”„ã‚Í•ª”zŒ³‚Æ‚µ‚ÄŒvã‚µ‚½‚¢‚Ì‚ÅA“¾ˆÓæ‚b‚c‚ð•ª”zŒ³‚Ì‚»‚ê‚É‚·‚è‘Ö‚¦‚é
        -- i•ª”z‚·‚é‚±‚Æ‚É‚æ‚Á‚Ä•ª”zæ‚ÌƒR[ƒX‚Í•ª”zŒ³‚Æ“¯‚¶‚É‚È‚Á‚Ä‚¢‚é‚Ì‚ÅA•ª”zŒ³‚Æ•ª”zæ‚ªˆê‚É’Šo‚³‚ê‚éj
        INNER JOIN (
            SELECT
                Šé‹Æ‚b‚c
                ,“¾ˆÓæ‚b‚c
                ,ISNULL(Žó’“¾ˆÓæ‚b‚c, “¾ˆÓæ‚b‚c) AS Žó’“¾ˆÓæ‚b‚c
            FROM
                “¾ˆÓæƒ}ƒXƒ^
            ) AS T
            ON T.Šé‹Æ‚b‚c  = M.Šé‹Æ‚b‚c
            AND T.“¾ˆÓæ‚b‚c = M.“¾ˆÓæ‚b‚c
        INNER JOIN ”„ãƒwƒbƒ_ AS H
            ON  H.Šé‹Æ‚b‚c   = M.Šé‹Æ‚b‚c
            AND H.”„ã“ú     = M.”„ã“ú
            AND H.“¾ˆÓæ‚b‚c = M.“¾ˆÓæ‚b‚c
            AND H.•”‚b‚c   = M.•”‚b‚c
    WHERE
        M.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c AND
        M.”„ã‹æ•ª IN (@”„ã‹æ•ª”„Š|, @”„ã‹æ•ªLINE) AND        -- LINEŒˆÏ‚Í”„Š|‚ÆŒ©‚È‚·
        M.”„ã“ú   = @”„ã“ú AND
        M.•”‚b‚c = @•”‚b‚c AND
        H.ƒR[ƒX‚b‚c IS NULL
    GROUP BY
        M.Šé‹Æ‚b‚c
        ,M.”„ã“ú
        ,T.Žó’“¾ˆÓæ‚b‚c
        ,M.•”‚b‚c
        ,M.¤•i‚b‚c
    ) AS A
        ON MAIN.Šé‹Æ‚b‚c    = A.Šé‹Æ‚b‚c
        AND MAIN.”„ã“ú     = A.”„ã“ú
        AND MAIN.“¾ˆÓæ‚b‚c = A.“¾ˆÓæ‚b‚c
        AND MAIN.¤•i‚b‚c   = A.¤•i‚b‚c
        AND MAIN.•”‚b‚c   = A.•”‚b‚c
    LEFT JOIN “¾ˆÓæƒ}ƒXƒ^ AS T ON T.Šé‹Æ‚b‚c = MAIN.Šé‹Æ‚b‚c AND T.“¾ˆÓæ‚b‚c = MAIN.“¾ˆÓæ‚b‚c
    LEFT JOIN V_”„Š|Œ»‹à‹æ•ª AS V ON V.”„Š|Œ»‹à‹æ•ªCD = T.”„Š|Œ»‹à‹æ•ª
    LEFT JOIN V_”„Š|Œ»‹à‹æ•ª AS VT ON VT.”„Š|Œ»‹à‹æ•ªCD = @”„Š|Œ»‹à‹æ•ª”„Š|
UNION
--- Œ»‹à‡Œv
SELECT
    1 AS –¾×‡Œv‹æ•ª,
    -1 AS “¾ˆÓæ‚b‚c,
    NULL AS “¾ˆÓæ–¼,
    '‡Œv' AS •\Ž¦—p“¾ˆÓæ–¼,
    VT.”„Š|Œ»‹à‹æ•ªCD AS ”„Š|Œ»‹à‹æ•ª,
    VT.”„Š|Œ»‹à‹æ•ª–¼ AS ƒ^ƒCƒv,
    NULL AS ¤•i‚b‚c,
    NULL AS ’l
FROM
    V_”„Š|Œ»‹à‹æ•ª AS VT
WHERE
    VT.”„Š|Œ»‹à‹æ•ªCD = @”„Š|Œ»‹à‹æ•ªŒ»‹à
UNION
--- ”„Š|‡Œv
SELECT
    1 AS –¾×‡Œv‹æ•ª,
    -1 AS “¾ˆÓæ‚b‚c,
    NULL AS “¾ˆÓæ–¼,
    '‡Œv' AS •\Ž¦—p“¾ˆÓæ–¼,
    VT.”„Š|Œ»‹à‹æ•ªCD AS ”„Š|Œ»‹à‹æ•ª,
    VT.”„Š|Œ»‹à‹æ•ª–¼ AS ƒ^ƒCƒv,
    NULL AS ¤•i‚b‚c,
    NULL AS ’l
FROM
    V_”„Š|Œ»‹à‹æ•ª AS VT
WHERE
    VT.”„Š|Œ»‹à‹æ•ªCD = @”„Š|Œ»‹à‹æ•ª”„Š|
ORDER BY
    –¾×‡Œv‹æ•ª,
    “¾ˆÓæ‚b‚c,
    ”„Š|Œ»‹à‹æ•ª,
    ¤•i‚b‚c

@”„Š|Œ»‹à‹æ•ªŒ»‹à = 0
@”„Š|Œ»‹à‹æ•ª”„Š| = 1
@”„Š|Œ»‹à‹æ•ªLINE = 5
@”„ã‹æ•ª”„Š| = 1
@”„ã‹æ•ªLINE = 5
