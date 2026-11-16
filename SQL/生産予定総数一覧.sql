-- ¶ŽY—\’è‘”ˆê——

-- •”ˆê——
SELECT
    A.•”‚b‚c
    ,B.•”–¼
FROM
    (SELECT DISTINCT
        Šé‹Æ‚b‚c
        ,•”‚b‚c
    FROM
        ¶ŽY—\’è
    WHERE
        Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
        AND ‹iH“ú = @‹iH“ú
    ) AS A
    INNER JOIN •”ƒ}ƒXƒ^ AS B
        ON B.Šé‹Æ‚b‚c = A.Šé‹Æ‚b‚c
        AND B.•”‚b‚c = A.•”‚b‚c
ORDER BY
    A.•”‚b‚c


-- ¶ŽY—\’è
SELECT
    Y.‹iH“ú
    ,Y.¤•i‚b‚c
    ,Y.•”‚b‚c
    ,S.¤•i–¼
    ,SUM(Y.”—Ê) AS ”—Ê
FROM
    ¶ŽY—\’è AS Y
    LEFT JOIN ¤•iƒ}ƒXƒ^ AS S
        ON Y.Šé‹Æ‚b‚c = S.Šé‹Æ‚b‚c
        AND Y.¤•i‚b‚c = S.¤•i‚b‚c
WHERE
    Y.Šé‹Æ‚b‚c = @Šé‹Æ‚b‚c
    AND Y.‹iH“ú = @‹iH“ú
GROUP BY
    Y.‹iH“ú
    ,Y.¤•i‚b‚c
    ,Y.•”‚b‚c
    ,S.¤•i–¼
ORDER BY
    Y.¤•i‚b‚c
    ,Y.•”‚b‚c


@Šé‹Æ‚b‚c = 40000
@‹iH“ú = '2018/05/02'
