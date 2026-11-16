-- ”„Š|•ª”z“ü—Íi“¯ˆê”„ã‹æ•ªA“¯ˆê¤•i‚Ì‘¶Ýƒ`ƒFƒbƒNj
SELECT
    count(*) AS CNT     -- 1ˆÈã‚È‚ç“¯ˆê”„ã‹æ•ªA“¯ˆê¤•i‚ª•¡”‘¶Ý
FROM
    (SELECT
        ¤•i‚b‚c,
        count(¤•i‚b‚c) AS ¤•i”
    FROM
        ”„ã–¾×
    WHERE
        Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
        AND ”„ã“ú = @”„ã“ú
        AND “¾ˆÓæ‚b‚c = @“¾ˆÓæ‚b‚c
        AND •”‚b‚c = @•”‚b‚c
        AND ”„ã‹æ•ª = @”„ã‹æ•ª”„Š|
    GROUP BY
        ¤•i‚b‚c
    HAVING
        count(¤•i‚b‚c) > 1
    ) AS A
