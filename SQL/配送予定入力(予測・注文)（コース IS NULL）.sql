-- ”z‘——\’è“ü—Í(—\‘ªE’•¶)iƒR[ƒX IS NULLj
-- ƒR[ƒXƒe[ƒuƒ‹‚ÍŒ©‚È‚¢
SELECT
    C.“¾ˆÓæ‚b‚c
    ,T.“¾ˆÓæ–¼
    ,A.ƒR[ƒX‚b‚c
    ,A.¤•i‚b‚c
    ,A.¤•i’•¶”—Ê
    ,A.¤•i—\‘ª”—Ê
    ,CASE WHEN ISNULL(A.¤•i’•¶”—Ê, -1) >= 0 THEN 1 ELSE 0 END AS ’•¶ƒtƒ‰ƒO
    ,CASE WHEN ISNULL(A.¤•i’•¶”—Ê, -1) >= 0 THEN A.¤•i’•¶”—Ê ELSE A.¤•i—\‘ª”—Ê END AS ¤•i”—Ê
FROM
-- ‡@‹ì“®•\iƒR[ƒX‚ªNULL‚Ì—\‘ªj
    (SELECT
        “¾ˆÓæ‚b‚c
    FROM
        ’•¶ƒwƒbƒ_
    WHERE
        Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
        AND ‹iH“ú   = @’•¶“ú
        AND •”‚b‚c = @•”‚b‚c
        AND ƒR[ƒX‚b‚c IS NULL            -- ƒR[ƒX‚ª–³‚¢
        AND ’•¶‹æ•ª = @’•¶‹æ•ª—\‘ª      -- —\‘ª
    ) AS C
    LEFT JOIN “¾ˆÓæƒ}ƒXƒ^ AS T ON T.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c AND C.“¾ˆÓæ‚b‚c = T.“¾ˆÓæ‚b‚c
-- ‡A‹ì“®•\‚É•R•t‚­—\‘ª‚Æ’•¶
-- —\‘ª‚Æ’•¶‚ð‚Ps‚É‚Ü‚Æ‚ß‚é
    LEFT JOIN
       (SELECT
           B.ƒR[ƒX‚b‚c
           ,B.“¾ˆÓæ‚b‚c
           ,B.¤•i‚b‚c
           ,SUM(CASE WHEN B.’•¶‹æ•ª = @’•¶‹æ•ª’•¶ THEN B.¤•i”—Ê ELSE null END) AS ¤•i’•¶”—Ê
           ,SUM(CASE WHEN B.’•¶‹æ•ª = @’•¶‹æ•ª—\‘ª THEN B.¤•i”—Ê ELSE null END) AS ¤•i—\‘ª”—Ê
        FROM
            -- ‡A|‚P —\‘ª‚ðŽæ“¾
            (SELECT
                NULL AS ƒR[ƒX‚b‚c
                ,H.’•¶‹æ•ª
                ,M.“¾ˆÓæ‚b‚c
                ,M.¤•i‚b‚c
                ,M.¤•i”—Ê
            FROM
                ’•¶ƒwƒbƒ_ AS H
                INNER JOIN ’•¶–¾× AS M
                    ON  H.Šé‹Æ‚b‚c   = M.Šé‹Æ‚b‚c
                    AND H.’•¶‹æ•ª   = M.’•¶‹æ•ª
                    AND H.‹iH“ú     = M.‹iH“ú
                    AND H.“¾ˆÓæ‚b‚c = M.“¾ˆÓæ‚b‚c
                    AND H.•”‚b‚c   = M.•”‚b‚c
            WHERE
                H.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
                AND H.‹iH“ú   = @’•¶“ú
                AND H.•”‚b‚c = @•”‚b‚c
                AND H.’•¶‹æ•ª = @’•¶‹æ•ª—\‘ª      -- —\‘ª
                AND H.ƒR[ƒX‚b‚c IS NULL            -- ƒR[ƒX‚ª–³‚¢
            -- ‡A|‚Q ’•¶‚ðŽæ“¾
            UNION
            SELECT
                HY.ƒR[ƒX‚b‚c                       -- ’•¶‚É•R•t‚­—\‘ª‚ª‚ ‚éê‡‚ÍA‚»‚Ì—\‘ª‚ÌƒR[ƒX‚b‚c‚ðŽæ“¾
                ,HC.’•¶‹æ•ª
                ,M.“¾ˆÓæ‚b‚c
                ,M.¤•i‚b‚c
                ,M.¤•i”—Ê
            FROM
                ’•¶ƒwƒbƒ_ AS HC
                INNER JOIN ’•¶–¾× AS M
                    ON  HC.Šé‹Æ‚b‚c   = M.Šé‹Æ‚b‚c
                    AND HC.’•¶‹æ•ª   = M.’•¶‹æ•ª
                    AND HC.‹iH“ú     = M.‹iH“ú
                    AND HC.“¾ˆÓæ‚b‚c = M.“¾ˆÓæ‚b‚c
                    AND HC.•”‚b‚c   = M.•”‚b‚c
                LEFT JOIN ’•¶ƒwƒbƒ_ AS HY
                    ON  HY.Šé‹Æ‚b‚c   = HC.Šé‹Æ‚b‚c
                    AND HY.‹iH“ú     = HC.‹iH“ú
                    AND HY.“¾ˆÓæ‚b‚c = HC.“¾ˆÓæ‚b‚c
                    AND HY.•”‚b‚c   = HC.•”‚b‚c
                    AND HY.’•¶‹æ•ª   = @’•¶‹æ•ª—\‘ª
            WHERE
                HC.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
                AND HC.‹iH“ú   = @’•¶“ú
                AND HC.•”‚b‚c = @•”‚b‚c
                AND HC.’•¶‹æ•ª = @’•¶‹æ•ª’•¶     -- ’•¶
            ) AS B
        GROUP BY
           B.ƒR[ƒX‚b‚c
           ,B.“¾ˆÓæ‚b‚c
           ,B.¤•i‚b‚c
        ) AS A ON A.“¾ˆÓæ‚b‚c = C.“¾ˆÓæ‚b‚c
ORDER BY
    A.“¾ˆÓæ‚b‚c,
    A.¤•i‚b‚c



@Šé‹Æ‚b‚c = 40001
@’•¶“ú = '2018/04/25'
@•”‚b‚c = 1
@’•¶‹æ•ª’•¶ = 0
@’•¶‹æ•ª—\‘ª = 1
