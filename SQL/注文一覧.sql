-- ’•¶ˆê——
SELECT
      M.Šé‹Æ‚b‚c
     ,H.ƒR[ƒX‚b‚c
     ,C.ƒR[ƒX–¼
     ,M.‹iH“ú
     ,M.“¾ˆÓæ‚b‚c
     ,T.“¾ˆÓæ–¼
     ,M.•”‚b‚c
     ,M.–¾×”Ô†
     ,H.”z‘—“ú
     ,M.»‘¢“ú
     ,M.¤•i‚b‚c
     ,S.¤•i–¼
     ,M.¤•iƒOƒ‹[ƒv‚b‚c
     ,G.¤•iƒOƒ‹[ƒv–¼—ªÌ AS ¤•iƒOƒ‹[ƒv–¼
     ,M.”„Š|Œ»‹à‹æ•ª
     ,VU.”„Š|Œ»‹à‹æ•ª–¼
     ,M.¤•i’P‰¿
     ,M.¤•i”—Ê
     ,M.¤•i‹àŠz‡Œv
     ,M.Á”ïÅŠz
     ,M.¤•i‹àŠz‡ŒvÅž
     ,M.ŒvŽZÅ—¦
     ,case when isnull(S.ŒyŒ¸Å—¦ƒtƒ‰ƒO, 0) <> 0 then '›' else null end as ŒyŒ¸Å—¦‘ÎÛ
     ,H.Å‹æ•ª
     ,VZ.Å‹æ•ª–¼
     ,M.ì¬’S“–ŽÒ‚b‚c
     ,A1.’S“–ŽÒ–¼ AS ì¬’S“–ŽÒ–¼
     ,M.ì¬“ú
     ,M.C³’S“–ŽÒ‚b‚c
     ,A2.’S“–ŽÒ–¼ AS C³’S“–ŽÒ–¼
     ,M.C³“ú
     ,H.ì¬’S“–ŽÒ‚b‚c AS ƒwƒbƒ_ì¬’S“–ŽÒ‚b‚c
     ,A3.’S“–ŽÒ–¼ AS ƒwƒbƒ_ì¬’S“–ŽÒ–¼
     ,H.ì¬“ú AS ƒwƒbƒ_ì¬“úŽž
     ,H.C³’S“–ŽÒ‚b‚c AS ƒwƒbƒ_C³’S“–ŽÒ‚b‚c
     ,A4.’S“–ŽÒ–¼ AS ƒwƒbƒ_C³’S“–ŽÒ–¼
     ,H.C³“ú AS ƒwƒbƒ_C³“úŽž
 FROM
     ’•¶–¾× AS M
     LEFT JOIN ’•¶ƒwƒbƒ_         AS H  ON H.Šé‹Æ‚b‚c = M.Šé‹Æ‚b‚c AND H.’•¶‹æ•ª         = M.’•¶‹æ•ª AND H.‹iH“ú = M.‹iH“ú AND H.“¾ˆÓæ‚b‚c = M.“¾ˆÓæ‚b‚c AND H.•”‚b‚c = M.•”‚b‚c
     LEFT JOIN ƒR[ƒXƒ}ƒXƒ^       AS C  ON C.Šé‹Æ‚b‚c = H.Šé‹Æ‚b‚c AND C.ƒR[ƒX‚b‚c       = H.ƒR[ƒX‚b‚c
     LEFT JOIN •”ƒ}ƒXƒ^         AS B  ON B.Šé‹Æ‚b‚c = M.Šé‹Æ‚b‚c AND B.•”‚b‚c         = M.•”‚b‚c
     LEFT JOIN “¾ˆÓæƒ}ƒXƒ^       AS T  ON T.Šé‹Æ‚b‚c = M.Šé‹Æ‚b‚c AND T.“¾ˆÓæ‚b‚c       = M.“¾ˆÓæ‚b‚c
     LEFT JOIN V_Å‹æ•ª           AS VZ ON VZ.Å‹æ•ªCD = H.Å‹æ•ª
     LEFT JOIN ¤•iƒ}ƒXƒ^         AS S  ON S.Šé‹Æ‚b‚c = M.Šé‹Æ‚b‚c AND S.¤•i‚b‚c         = M.¤•i‚b‚c
     LEFT JOIN ¤•iƒOƒ‹[ƒvƒ}ƒXƒ^ AS G  ON G.Šé‹Æ‚b‚c = M.Šé‹Æ‚b‚c AND G.¤•iƒOƒ‹[ƒv‚b‚c = M.¤•iƒOƒ‹[ƒv‚b‚c
     LEFT JOIN V_”„Š|Œ»‹à‹æ•ª     AS VU ON VU.”„Š|Œ»‹à‹æ•ªCD = M.”„Š|Œ»‹à‹æ•ª
     LEFT JOIN ’S“–ŽÒƒ}ƒXƒ^       AS A1 ON A1.Šé‹Æ‚b‚c = M.Šé‹Æ‚b‚c AND M.ì¬’S“–ŽÒ‚b‚c = A1.’S“–ŽÒ‚b‚c
     LEFT JOIN ’S“–ŽÒƒ}ƒXƒ^       AS A2 ON A2.Šé‹Æ‚b‚c = M.Šé‹Æ‚b‚c AND M.C³’S“–ŽÒ‚b‚c = A2.’S“–ŽÒ‚b‚c
     LEFT JOIN ’S“–ŽÒƒ}ƒXƒ^       AS A3 ON A3.Šé‹Æ‚b‚c = H.Šé‹Æ‚b‚c AND H.ì¬’S“–ŽÒ‚b‚c = A3.’S“–ŽÒ‚b‚c
     LEFT JOIN ’S“–ŽÒƒ}ƒXƒ^       AS A4 ON A4.Šé‹Æ‚b‚c = H.Šé‹Æ‚b‚c AND H.C³’S“–ŽÒ‚b‚c = A4.’S“–ŽÒ‚b‚c
 WHERE
     M.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
 AND M.’•¶‹æ•ª = @’•¶‹æ•ª’•¶
 and B.Žx“X‚b‚c = @Žx“X‚b‚c
 and M.•”‚b‚c = @•”‚b‚c
 and M.‹iH“ú >= @‹iH“úFrom
 and M.‹iH“ú <= @‹iH“úTo
 and H.”z‘—“ú >= @”z‘—“úFrom
 and H.”z‘—“ú <= @”z‘—“úTo
 and M.»‘¢“ú >= @»‘¢“úFrom
 and M.»‘¢“ú <= @»‘¢“úTo
 and M.¤•i‚b‚c = @¤•i‚b‚c
 and M.¤•iƒOƒ‹[ƒv‚b‚c = @¤•iƒOƒ‹[ƒv‚b‚c
 and C.ƒR[ƒX‚b‚c = @ƒR[ƒX‚b‚c
 and ISNULL(T.¿‹æ‚b‚c, T.“¾ˆÓæ‚b‚c) = @¿‹æ‚b‚c
 and M.“¾ˆÓæ‚b‚c = @“¾ˆÓæ‚b‚c
 and ISNULL(T.Žó’“¾ˆÓæ‚b‚c, T.“¾ˆÓæ‚b‚c) = @Žó’“¾ˆÓæ‚b‚c
 and (M.ì¬’S“–ŽÒ‚b‚c = @“ü—ÍŽÒ‚b‚c OR M.C³’S“–ŽÒ‚b‚c = @“ü—ÍŽÒ‚b‚c)
 and H.ì¬“ú >= @ì¬“úŽžFrom
 and H.ì¬“ú <= @ì¬“úŽžTo
 and H.C³“ú >= @C³“úŽžFrom
 and H.C³“ú <= @C³“úŽžTo
 and ISNULL(S.ŒyŒ¸Å—¦ƒtƒ‰ƒO, 0) = 1        -- ŒyŒ¸Å—¦‘ÎÛ
 and ISNULL(S.ŒyŒ¸Å—¦ƒtƒ‰ƒO, 0) = 0        -- ŒyŒ¸Å—¦‘ÎÛŠO
order by
M.‹iH“ú DESC, M.“¾ˆÓæ‚b‚c, M.–¾×”Ô†;
@Šé‹Æ‚b‚c = 40001
@’•¶‹æ•ª’•¶ = 0
@Žx“X‚b‚c = 1
@•”‚b‚c = 1
@‹iH“úFrom = 2016/09/01 0:00:00
@‹iH“úTo = 2016/09/30 0:00:00
@”z‘—“úFrom = 2016/09/01 0:00:00
@”z‘—“úTo = 2016/09/30 0:00:00
@»‘¢“úFrom = 2016/09/01 0:00:00
@»‘¢“úTo = 2016/09/30 0:00:00
@¤•i‚b‚c = 28
@¤•iƒOƒ‹[ƒv‚b‚c = 1
@ƒR[ƒX‚b‚c = 28
@¿‹æ‚b‚c = 5
@“¾ˆÓæ‚b‚c = 5
@Žó’“¾ˆÓæ‚b‚c = 5
@“ü—ÍŽÒ‚b‚c = 999999
@ì¬“úŽžFrom = 2016/09/01 0:00:00
@ì¬“úŽžTo = 2016/09/30 0:00:00
@C³“úŽžFrom = 2016/09/01 0:00:00
@C³“úŽžTo = 2016/09/30 0:00:00
