-- システム接続端末登録
INSERT INTO システム接続端末(
    企業ＣＤ
    ,MACアドレス
    ,IPv4アドレス
    ,起動日時
    ,ログイン担当者ＣＤ
)
SELECT
    @企業ＣＤ
    ,@MACアドレス
    ,@IPv4アドレス
    ,getdate()
    ,@ログイン担当者ＣＤ
WHERE
    (SELECT COUNT(*) FROM システム接続端末 WHERE 企業ＣＤ = @企業ＣＤ) < @システム接続端末数
