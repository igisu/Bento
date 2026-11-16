-- ¿‹‘i–¾×ji¿‹æ‚É‚Ô‚ç‰º‚ª‚é“¾ˆÓæ‚ªŽ©•ª‚µ‚©‚¢‚È‚¢ê‡j
-- “¾ˆÓæ–¼s‚ðo—Í‚¹‚¸A‘O‰ñ¿‹Šz`¡‰ñ¿‹Šz‚ÆÅIs‚Ì‡Œvs‚ðo—Í‚·‚é‚Ì‚ªA•¡”‚ ‚éê‡‚Ì–¾×•”‚Æ‚Ìˆá‚¢
-- –¾×
SELECT
    @‡ŽZ¿‹ƒtƒ‰ƒO AS ‡ŽZ¿‹ƒtƒ‰ƒO
    ,@”„ã‚Ì‚Ýƒtƒ‰ƒO AS ”„ã‚Ì‚Ýƒtƒ‰ƒO
    ,G.¿‹”Ô†
    ,G.¿‹“ú
    ,G.’÷‹æ•ª
    ,G.•”‚b‚c
    ,G.¿‹æ‚b‚c AS “¾ˆÓæ‚b‚c
    ,G.¿‹æ–¼ + ' ' + ISNULL(VS.¿‹‘ŒhÌ–¼, '—l') AS “¾ˆÓæ–¼
    ,DATALENGTH(G.¿‹æ–¼ + ' ' + ISNULL(VS.¿‹‘ŒhÌ–¼, '')) AS “¾ˆÓæ–¼’·‚³
    ,G.‘O‰ñ¿‹Šz
    ,G.“ü‹àŠz
    ,G.ŒJ‰zŠz
    ,G.”ƒã‹àŠz
    ,G.’²®Šz
    ,G.Á”ïÅ
    ,G.¡‰ñ¿‹Šz
    ,'*****' AS ”„ã‚Ì‚Ý‘O‰ñ¿‹Šz
    ,'*****' AS ”„ã‚Ì‚Ý“ü‹àŠz
    ,'*****' AS ”„ã‚Ì‚ÝŒJ‰zŠz
    ,ISNULL(G.”ƒã‹àŠz, 0) + ISNULL(G.’²®Šz, 0) + ISNULL(G.Á”ïÅ, 0) AS ”„ã‚Ì‚Ý¡‰ñ¿‹Šz
    ,G.‰ñŽû—\’è“ú
    ,@‰ñŽû—\’è“úˆóŽšƒtƒ‰ƒO AS ‰ñŽû—\’è“úˆóŽšƒtƒ‰ƒO
    ,ISNULL(P.‘O‰ñ¿‹“ú, DATEADD(d, -1, P.ÅŒÃ”„ã“ú)) AS ‘O‰ñ¿‹“ú
    ,ISNULL(DATEADD(d, 1, P.‘O‰ñ¿‹“ú), P.ÅŒÃ”„ã“ú) AS ¿‹ŠúŠÔFrom
    ,G.¿‹“ú AS ¿‹ŠúŠÔTo
    ,T.—X•Ö”Ô† AS “¾ˆÓæ—X•Ö”Ô†
    ,T.ZŠ1 AS “¾ˆÓæZŠ1
    ,T.ZŠ2 AS “¾ˆÓæZŠ2
    ,T.“d˜b”Ô†1 AS “¾ˆÓæ“d˜b”Ô†
    ,T.‚e‚`‚w1 AS “¾ˆÓæ‚e‚`‚w
    ,T.’÷‹æ•ª
    ,CASE T.’÷‹æ•ª
        WHEN 1 THEN T.T’÷—j“ú
        WHEN 2 THEN T.ŒŽ’÷“ú
        ELSE DAY(G.¿‹“ú)  -- “ú’÷
     END AS ’÷“ú
    ,T.Žx•¥ƒTƒCƒg AS ƒTƒCƒg
    ,T.Žx•¥“ú
    ,ISNULL(T.Žx•¥•û–@, -1) AS Žx•¥•û–@
    ,T.UžŽè”—¿ƒtƒ‰ƒO             -- •‰’S‚ð‚¨Šè‚¢‚·‚éê‡‚Ífalse
    ,T.¿‹‘—X•Öƒ}[ƒNƒtƒ‰ƒO
    ,ISNULL(T.Å‹æ•ª, @Å‹æ•ª“àÅ) AS Å‹æ•ª
    ,ISNULL(T.ŠÛ‚ßˆ—‹æ•ª, @ŠÛ‚ßˆ—‹æ•ªØ‚èŽÌ‚Ä) AS ŠÛ‚ßˆ—‹æ•ª
    ,B.‰ïŽÐ–¼Ì AS Ž©ŽÐ–¼Ì
    ,B.—X•Ö”Ô† AS Ž©ŽÐ—X•Ö”Ô†
    ,B.ZŠ AS Ž©ŽÐZŠ
    ,B.“d˜b”Ô† AS Ž©ŽÐ“d˜b”Ô†
    ,B.‚e‚`‚w AS Ž©ŽÐ‚e‚`‚w
    ,B.“o˜^”Ô†
    ,B.‹à—Z‹@ŠÖ‚b‚c1
    ,KKM1.‹âs–¼ AS ‹âs–¼1
    ,B.‹à—Z‹@ŠÖŽx“X‚b‚c1
    ,KKSM1.Žx“X–¼ AS Žx“X–¼1
    ,B.ŒûÀŽí•Ê1
    ,VK1.ŒûÀŽí•Ê–¼ AS ŒûÀŽí•Ê–¼1
    ,B.ŒûÀ”Ô†1
    ,B.ŒûÀ–¼‹`l1
    ,B.‹à—Z‹@ŠÖ‚b‚c2
    ,KKM2.‹âs–¼ AS ‹âs–¼2
    ,B.‹à—Z‹@ŠÖŽx“X‚b‚c2
    ,KKSM2.Žx“X–¼ AS Žx“X–¼2
    ,B.ŒûÀŽí•Ê2
    ,VK2.ŒûÀŽí•Ê–¼ AS ŒûÀŽí•Ê–¼2
    ,B.ŒûÀ”Ô†2
    ,B.ŒûÀ–¼‹`l2
    ,B.¿‹‘”õl1
    ,B.¿‹‘”õl2
    ,@ƒR[ƒX‚b‚c AS ƒR[ƒX‚b‚c
    ,H.“¾ˆÓæ‚b‚c AS –¾×“¾ˆÓæ‚b‚c
    ,URI.”„ã“ú
    ,URI.”[•i‘“`•[”Ô† AS “`•[”Ô†
    ,URI.–¾×”Ô†
    ,URI.¤•i‚b‚c
    ,S.¤•i–¼
    ,URI.¤•i’P‰¿
    ,URI.¤•i”—Ê
    ,URI.’lˆøŒã‹àŠz
    ,URI.ŒvŽZÅ—¦
    ,@“àÁ”ïÅŒ©o‚µ AS “àÁ”ïÅŒ©o‚µ
    ,RIGHT('**********' + RIGHT(T.ŒûÀ”Ô†, 3), LEN(T.ŒûÀ”Ô†)) AS U‘ÖŒûÀ”Ô†
    ,KKM3.‹âs–¼ AS U‘Ö‹âs–¼
    ,KKSM3.Žx“X–¼ AS U‘ÖŽx“X–¼
    ,@U‘ÖŒûÀˆóŽšƒtƒ‰ƒO AS U‘ÖŒûÀˆóŽšƒtƒ‰ƒO
FROM
    -- ‡@¡‰ñ¿‹‚ÌƒL[‚ÆA‚»‚ê‚É‘Î‚·‚é‘O‰ñ¿‹‚ÌƒL[‚ð‹‚ß‚é
    (SELECT
        G1.Šé‹Æ‚b‚c
        ,G1.¿‹”Ô† AS ¡‰ñ¿‹”Ô†
        ,G1.¿‹æ‚b‚c
        ,UM_OLD.ÅŒÃ”„ã“ú
        ,MAX(P1.¿‹“ú) AS ‘O‰ñ¿‹“ú
    FROM
        ¿‹‡Œv AS G1
        LEFT JOIN ¿‹‡Œv AS P1
            ON G1.Šé‹Æ‚b‚c = P1.Šé‹Æ‚b‚c
            AND G1.¿‹æ‚b‚c = P1.¿‹æ‚b‚c
            AND P1.¿‹“ú < G1.¿‹“ú
        -- ‡D‘O‰ñ¿‹“ú‚ª–³‚¢ê‡‚ÌAÅŒÃ‚Ì”„ã“úi‘S‚Ä‚ÌƒŒƒR[ƒh‚É‚¨‚¢‚Ä“¯‚¶’l‚É‚È‚é‚Ì‚ÅA‹ì“®•\‚ÉŠÜ‚ß‚é‚±‚Æ‚Å‚‘¬‰»‚ð}‚éj
        LEFT JOIN
            (SELECT
                OH.Šé‹Æ‚b‚c
                ,OH.¿‹æ‚b‚c
                ,MIN(OM.”„ã“ú) AS ÅŒÃ”„ã“ú
            FROM
                ¿‹ƒwƒbƒ_ AS OH
                INNER JOIN ”„ã–¾× AS OM ON
                    OH.Šé‹Æ‚b‚c = OM.Šé‹Æ‚b‚c
                    AND OH.“¾ˆÓæ‚b‚c = OM.“¾ˆÓæ‚b‚c
                    AND OH.•”‚b‚c = OM.•”‚b‚c
            WHERE
                OH.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
                AND OH.¿‹”Ô† = @¿‹”Ô†
                AND OM.”„ã‹æ•ª = @”„ã‹æ•ª”„Š|        -- ”„Š|‚Ì‚Ý
                AND ISNULL(OM.¤•i”—Ê, 0) <> 0
            GROUP BY
                OH.Šé‹Æ‚b‚c
                ,OH.¿‹æ‚b‚c
            ) AS UM_OLD ON
            UM_OLD.Šé‹Æ‚b‚c = G1.Šé‹Æ‚b‚c
            AND UM_OLD.¿‹æ‚b‚c = G1.¿‹æ‚b‚c
    WHERE
        G1.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
        AND G1.¿‹”Ô† = @¿‹”Ô†
    GROUP BY
        G1.Šé‹Æ‚b‚c
        ,G1.¿‹”Ô†
        ,G1.¿‹æ‚b‚c
        ,UM_OLD.ÅŒÃ”„ã“ú
    ) AS P
    -- ‡A¡‰ñ¿‹‚ÌÚ×
    INNER JOIN ¿‹‡Œv G ON
        G.Šé‹Æ‚b‚c = P.Šé‹Æ‚b‚c
        AND G.¿‹”Ô† = P.¡‰ñ¿‹”Ô†
    LEFT JOIN “¾ˆÓæƒ}ƒXƒ^ AS T
        ON  T.Šé‹Æ‚b‚c = G.Šé‹Æ‚b‚c
        AND T.“¾ˆÓæ‚b‚c = G.¿‹æ‚b‚c
    LEFT JOIN •”ƒ}ƒXƒ^ AS B ON
        B.Šé‹Æ‚b‚c = G.Šé‹Æ‚b‚c
        AND B.•”‚b‚c = G.•”‚b‚c
    -- ‡B¡‰ñ‚Ì¿‹ƒwƒbƒ_i”„ã‚ª‚ ‚é¿‹ƒwƒbƒ_‚Ì‚ÝŽæ“¾‚·‚éji•”‚²‚Æ‚ÌÅV‚Ì”„ã‚ðŽæ“¾‚·‚é•K—v‚ª‚ ‚é‚Ì‚Å‡Œv‚É‚Í‚µ‚È‚¢j
     LEFT JOIN
        (SELECT DISTINCT
            Šé‹Æ‚b‚c
            ,¿‹”Ô†
            ,“¾ˆÓæ‚b‚c
            ,•”‚b‚c
            ,¿‹æ‚b‚c
        FROM
            ¿‹ƒwƒbƒ_
        WHERE
            ”„ã‹àŠzÅž <> 0
        ) AS H ON
        H.Šé‹Æ‚b‚c = G.Šé‹Æ‚b‚c
        AND H.¿‹”Ô† = G.¿‹”Ô†
        -- ‡C¡‰ñ‚Ì¿‹‚É‘Î‚·‚éÅV‚Ì”„ã
    LEFT JOIN
        (SELECT
            UH.Šé‹Æ‚b‚c
            ,UH.“¾ˆÓæ‚b‚c
            ,UH.•”‚b‚c
            ,UH.”„ã“ú
            ,UH.”[•i‘“`•[”Ô†
            ,UM.–¾×”Ô†
            ,UM.¤•i‚b‚c
            ,UM.¤•i’P‰¿
            ,UM.’lˆøŒã‹àŠz
            ,UM.¤•i”—Ê
            ,UM.ŒvŽZÅ—¦
        FROM
            ”„ãƒwƒbƒ_ AS UH
            INNER JOIN ”„ã–¾× AS UM ON
                UM.Šé‹Æ‚b‚c = UH.Šé‹Æ‚b‚c
                AND UM.“¾ˆÓæ‚b‚c = UH.“¾ˆÓæ‚b‚c
                AND UM.•”‚b‚c = UH.•”‚b‚c
                AND UM.”„ã“ú = UH.”„ã“ú
        WHERE
            UM.”„ã‹æ•ª = @”„ã‹æ•ª”„Š|        -- ”„Š|‚Ì‚Ý
            AND ISNULL(UM.¤•i”—Ê, 0) <> 0
--            AND (ISNULL(UM.¤•i’P‰¿, 0) <> 0 OR ISNULL(UM.’lˆøŠz, 0) <> 0)        -- ’P‰¿‚ª0‚Å‚àA’lˆø‚ª0‚Å‚È‚¯‚ê‚Îo—Í‚·‚é
        ) AS URI ON
        URI.Šé‹Æ‚b‚c = H.Šé‹Æ‚b‚c
        AND URI.“¾ˆÓæ‚b‚c = H.“¾ˆÓæ‚b‚c
        AND URI.•”‚b‚c = H.•”‚b‚c
        AND URI.”„ã“ú > ISNULL(P.‘O‰ñ¿‹“ú, '2000/01/01')
        AND URI.”„ã“ú <= G.¿‹“ú
    -- ‡D‘O‰ñ¿‹“ú‚ª–³‚¢ê‡‚ÌAÅŒÃ‚Ì”„ã“úi‚±‚±‚Å‚ÌŽæ“¾‚Í”pŽ~‚·‚éj
    -- LEFT JOIN
    --     (SELECT
    --         OH.Šé‹Æ‚b‚c
    --         ,OH.¿‹æ‚b‚c
    --         ,MIN(OM.”„ã“ú) AS ÅŒÃ”„ã“ú
    --     FROM
    --         ¿‹ƒwƒbƒ_ AS OH
    --         INNER JOIN ”„ã–¾× AS OM ON
    --         OH.Šé‹Æ‚b‚c = OM.Šé‹Æ‚b‚c
    --         AND OH.“¾ˆÓæ‚b‚c = OM.“¾ˆÓæ‚b‚c
    --         AND OH.•”‚b‚c = OM.•”‚b‚c
    --     WHERE
    --         OH.¿‹”Ô† = @¿‹”Ô†
    --         AND OM.”„ã‹æ•ª = @”„ã‹æ•ª”„Š|        -- ”„Š|‚Ì‚Ý
    --         AND ISNULL(OM.¤•i”—Ê, 0) <> 0
    --     GROUP BY
    --         OH.Šé‹Æ‚b‚c
    --         ,OH.¿‹æ‚b‚c
    --     ) AS UM_OLD ON
    --     UM_OLD.Šé‹Æ‚b‚c = H.Šé‹Æ‚b‚c
    --     AND UM_OLD.¿‹æ‚b‚c = H.¿‹æ‚b‚c
    LEFT JOIN ¤•iƒ}ƒXƒ^ AS S ON
        S.Šé‹Æ‚b‚c = URI.Šé‹Æ‚b‚c
        AND S.¤•i‚b‚c = URI.¤•i‚b‚c
    LEFT JOIN V_¿‹‘ŒhÌ AS VS ON
        VS.¿‹‘ŒhÌCD = T.¿‹‘ŒhÌ
    LEFT JOIN ‹à—Z‹@ŠÖ–¼Ì AS KKM1 ON
        KKM1.‹âs‚b‚c = B.‹à—Z‹@ŠÖ‚b‚c1
    LEFT JOIN ‹à—Z‹@ŠÖŽx“X–¼Ì AS KKSM1 ON
        KKSM1.‹âs‚b‚c = B.‹à—Z‹@ŠÖ‚b‚c1
        AND KKSM1.Žx“X‚b‚c = B.‹à—Z‹@ŠÖŽx“X‚b‚c1
    LEFT JOIN V_ŒûÀŽí•Ê AS VK1 ON
        VK1.ŒûÀŽí•ÊCD = B.ŒûÀŽí•Ê1
    LEFT JOIN ‹à—Z‹@ŠÖ–¼Ì AS KKM2 ON
        KKM2.‹âs‚b‚c = B.‹à—Z‹@ŠÖ‚b‚c2
    LEFT JOIN ‹à—Z‹@ŠÖŽx“X–¼Ì AS KKSM2 ON
        KKSM2.‹âs‚b‚c = B.‹à—Z‹@ŠÖ‚b‚c2
        AND KKSM2.Žx“X‚b‚c = B.‹à—Z‹@ŠÖŽx“X‚b‚c2
    LEFT JOIN V_ŒûÀŽí•Ê AS VK2 ON
        VK2.ŒûÀŽí•ÊCD = B.ŒûÀŽí•Ê2
    LEFT JOIN ‹à—Z‹@ŠÖ–¼Ì AS KKM3 ON
        KKM3.‹âs‚b‚c = T.‹à—Z‹@ŠÖ‚b‚c
    LEFT JOIN ‹à—Z‹@ŠÖŽx“X–¼Ì AS KKSM3 ON
        KKSM3.‹âs‚b‚c = T.‹à—Z‹@ŠÖ‚b‚c
        AND KKSM3.Žx“X‚b‚c = T.‹à—Z‹@ŠÖŽx“X‚b‚c
ORDER BY
    –¾×“¾ˆÓæ‚b‚c
    ,”„ã“ú
    ,–¾×”Ô†
    ,¤•i‚b‚c


@”„ã‹æ•ª”„Š| = 1
@‡ŽZ¿‹ƒtƒ‰ƒO = 0
@”„ã‚Ì‚Ýƒtƒ‰ƒO = 0
@‰ñŽû—\’è“úˆóŽšƒtƒ‰ƒO = 1
@ƒR[ƒX‚b‚c = NULL
@Å‹æ•ª“àÅ = 1
@ŠÛ‚ßˆ—‹æ•ªØ‚èŽÌ‚Ä = 2
@“àÁ”ïÅŒ©o‚µ = 0
