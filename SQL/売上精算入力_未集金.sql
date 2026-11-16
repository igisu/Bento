-- ”„ã¸ŽZ“ü—Í_–¢W‹à
-- Œ»‹à‚Ì“¾ˆÓæ•Š|”„‚Ì”„ã–¾×‚Ì‹àŠz‚Ì‡Œv
SELECT
    M.“¾ˆÓæ‚b‚c
    ,ISNULL(SUM(M.¤•i‹àŠz‡ŒvÅž), 0) AS ¤•i‹àŠz‡ŒvÅž
FROM
    ”„ãƒwƒbƒ_ AS H
    INNER JOIN “¾ˆÓæƒ}ƒXƒ^ AS T ON T.Šé‹Æ‚b‚c = H.Šé‹Æ‚b‚c AND T.“¾ˆÓæ‚b‚c = H.“¾ˆÓæ‚b‚c
    INNER JOIN ”„ã–¾× AS M ON M.Šé‹Æ‚b‚c = H.Šé‹Æ‚b‚c AND M.”„ã“ú = H.”„ã“ú AND M.“¾ˆÓæ‚b‚c = H.“¾ˆÓæ‚b‚c AND M.•”‚b‚c = H.•”‚b‚c
WHERE
    H.Šé‹Æ‚b‚c       = @Šé‹Æ‚b‚c
    AND H.”„ã“ú     = @¸ŽZ“ú
    AND H.•”‚b‚c   = @•”‚b‚c
    AND H.ƒR[ƒX‚b‚c = @ƒR[ƒX‚b‚c
    AND ISNULL(T.”„Š|Œ»‹à‹æ•ª, 0) = 0   -- Œ»‹à
    AND M.”„ã‹æ•ª   = @”„ã‹æ•ª”„Š|
GROUP BY
    M.“¾ˆÓæ‚b‚c
