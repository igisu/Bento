-- Žž”„ã“ü‹àˆê——i’S“–ŽÒ•Êj
SELECT
    A.•”‚b‚c
    ,B.•”–¼
    ,A.”z‘—’S“–ŽÒ‚b‚c AS ‚b‚c
    ,T.’S“–ŽÒ–¼ AS –¼‘O
    ,A.”„ã
    ,NULL AS “ü‹à
FROM
    -- ‡@‹ì“®•\
    (SELECT
        H.Šé‹Æ‚b‚c
        ,H.•”‚b‚c                 -- •”‚²‚Æ‚ÉWŒv‚·‚éê‡‚Í‚±‚¿‚ç
        ,NULL AS •”‚b‚c           -- •”‚²‚Æ‚ÉWŒv‚µ‚È‚¢i‚Â‚Ü‚è•”‚ðŒ×‚ª‚Á‚ÄWŒv‚·‚éjê‡‚Í‚±‚¿‚ç
        ,H.”z‘—’S“–ŽÒ‚b‚c
        ,SUM(M.’lˆøŒã‹àŠz) AS ”„ã
    FROM
        ”„ãƒwƒbƒ_ AS H
        INNER JOIN ”„ã–¾× AS M
            ON H.Šé‹Æ‚b‚c = M.Šé‹Æ‚b‚c
            AND H.”„ã“ú = M.”„ã“ú
            AND H.“¾ˆÓæ‚b‚c = M.“¾ˆÓæ‚b‚c
            AND H.•”‚b‚c = M.•”‚b‚c
    WHERE
        H.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
        AND H.”„ã“ú >= @Žw’è”NŒŽ“úFrom
        AND H.”„ã“ú <= @Žw’è”NŒŽ“úTo
        AND H.•”‚b‚c = @•”‚b‚c      -- •”‚²‚Æ‚ÉWŒv‚·‚éê‡‚Ì‚Ý—LŒø
        AND H.”z‘—’S“–ŽÒ‚b‚c = @’S“–ŽÒ‚b‚c
    GROUP BY
        H.Šé‹Æ‚b‚c
        ,H.•”‚b‚c                     -- •”‚²‚Æ‚ÉWŒv‚µ‚È‚¢i‚Â‚Ü‚è•”‚ðŒ×‚ª‚Á‚ÄWŒv‚·‚éjê‡‚Í‚±‚Ìs‚Í•s—v
        ,H.”z‘—’S“–ŽÒ‚b‚c
    ) AS A
    LEFT JOIN •”ƒ}ƒXƒ^ AS B ON A.Šé‹Æ‚b‚c = B.Šé‹Æ‚b‚c AND A.•”‚b‚c = B.•”‚b‚c
    LEFT JOIN ’S“–ŽÒƒ}ƒXƒ^ AS T ON A.Šé‹Æ‚b‚c = T.Šé‹Æ‚b‚c AND A.”z‘—’S“–ŽÒ‚b‚c = T.’S“–ŽÒ‚b‚c
ORDER BY
    A.”z‘—’S“–ŽÒ‚b‚c

@Šé‹Æ‚b‚c = 40000
@Žw’è”NŒŽ“úFrom = '2017/06/01'
@Žw’è”NŒŽ“úTo = '2017/06/30'
@•”‚b‚c = 1
@’S“–ŽÒ‚b‚c = 8012
