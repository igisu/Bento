-- ”„Š|•ª”z“ü—Íi”„ã–¾×‚ª‘¶Ý‚µ‚È‚¢”„ãƒwƒbƒ_‚ðíœj
--DELETE H
SELECT H.*
FROM
    ”„ãƒwƒbƒ_ AS H
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
        ON T.“¾ˆÓæ‚b‚c = H.“¾ˆÓæ‚b‚c
    LEFT JOIN ”„ã–¾× AS M
        ON H.Šé‹Æ‚b‚c    = M.Šé‹Æ‚b‚c
        AND H.”„ã“ú     = M.”„ã“ú
        AND H.“¾ˆÓæ‚b‚c = M.“¾ˆÓæ‚b‚c
        AND H.•”‚b‚c   = M.•”‚b‚c
WHERE
    H.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
    AND H.”„ã“ú   = @”„ã“ú
    AND H.•”‚b‚c = @•”‚b‚c
    AND M.Šé‹Æ‚b‚c IS NULL
