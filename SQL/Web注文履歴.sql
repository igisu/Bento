-- Web’•¶—š—ð
SELECT
    H.‘€ì”Ô†
    ,H.Šé‹Æ‚b‚c
    ,H.”FØƒL[
    ,H.“¾ˆÓæ‚b‚c
    ,T.“¾ˆÓæ–¼
    ,H.UA
    ,H.IPv4ƒAƒhƒŒƒX
    ,H.WEB—š—ð‹æ•ª‚b‚c
    ,VW.WEB—š—ð‹æ•ª–¼
    ,H.‘€ì“úŽž
    ,H.‘€ì“à—e
FROM
    WEB_T_’•¶—š—ð AS H
    LEFT JOIN “¾ˆÓæƒ}ƒXƒ^ as T
        ON H.Šé‹Æ‚b‚c = T.Šé‹Æ‚b‚c
        AND H.“¾ˆÓæ‚b‚c = T.“¾ˆÓæ‚b‚c
    LEFT JOIN V_WEB—š—ð‹æ•ª AS VW
        ON VW.WEB—š—ð‹æ•ª‚b‚c = H.WEB—š—ð‹æ•ª‚b‚c
WHERE
    H.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
    AND H.‘€ì“úŽž >= @‘€ì“úŽžFrom
    AND H.‘€ì“úŽž <= @‘€ì“úŽžTo
    AND H.“¾ˆÓæ‚b‚c = @“¾ˆÓæ‚b‚c
    AND H.WEB—š—ð‹æ•ª‚b‚c = @WEB—š—ð‹æ•ª‚b‚c
    AND H.UA LIKE @UA escape '#'
    AND H.IPv4ƒAƒhƒŒƒX = @IPƒAƒhƒŒƒX
ORDER BY
    ‘€ì“úŽž
    ,‘€ì”Ô†


@Šé‹Æ‚b‚c = 40000
@‘€ì“úŽžFrom = '2017/08/01 00:00:00'
@‘€ì“úŽžTo = '2017/08/31 23:59:59'
@“¾ˆÓæ‚b‚c = 530
@WEB—š—ð‹æ•ª‚b‚c = 91
@UA = '%Mozilla%'
@IPƒAƒhƒŒƒX = '172.24.252.164'
