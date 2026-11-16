-- ”„Š|•ª”z“ü—Íi•ª”zæ”„ã–¾×íœj
--DELETE M
SELECT M.*
FROM
    ”„ã–¾× AS M
    -- •ª”zŒ³‚ªŽ‚Á‚Ä‚¢‚é¤•i
    INNER JOIN
        (SELECT
            ¤•i‚b‚c
        FROM
            ”„ã–¾×
        WHERE
            Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
            AND ”„ã“ú   = @”„ã“ú
            AND “¾ˆÓæ‚b‚c = @“¾ˆÓæ‚b‚c
            AND •”‚b‚c = @•”‚b‚c
        ) AS S
        ON S.¤•i‚b‚c = M.¤•i‚b‚c
    -- •ª”zæ‚É‚È‚é“¾ˆÓæ
    INNER JOIN
        (SELECT
            “¾ˆÓæ‚b‚c
        FROM
            “¾ˆÓæƒ}ƒXƒ^
        WHERE
            Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
            AND Žó’“¾ˆÓæ‚b‚c = @“¾ˆÓæ‚b‚c
        ) AS T
        ON T.“¾ˆÓæ‚b‚c = M.“¾ˆÓæ‚b‚c
WHERE
    M.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
    AND M.”„ã“ú   = @”„ã“ú
    AND M.•”‚b‚c = @•”‚b‚c
