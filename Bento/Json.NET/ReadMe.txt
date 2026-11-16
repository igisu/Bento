2019/04/01現在のJson.NET(Newtonsoft.Json.dll)の最新版は 12.0.1 だが、
それを参照すると、ActiveReportsNET11(C:\Program Files (x86)\ActiveReportsNET11\Bin\v11.0.9393.1\Newtonsoft.Json.dll)
で参照しているバージョン(7.0.1)とバッティングしてしまう。

したがって、ActiveReportsNET11で参照しているdllファイルをコピーして参照することにする
→これだとうまくいかなかった(※)ので、同じバージョンのJson.NETをダウンロード、解凍して解決。
  たぶんxmlファイルが必要なのかもしれない。

(※)Microsoft.CSharp.RuntimeBinder.RuntimeBinderException: 'Newtonsoft.Json.Linq.JObject' に 'order_slip_number' の定義がありません


(2019/04/04)
よく見てみると、ActiveReportsNET11 の Newtonsoft.Json.dll は、.NET 3.5 でビルドされているみたい。
試しに .NET 3.5 用の Newtonsoft.Json.dll と Newtonsoft.Json.xml を参照設定してみると、やはり(※)の例外が発生。
xmlの有無は関係なかった。

.NET 4.0 向けの Newtonsoft.Json.dll（と Newtonsoft.Json.xml）を参照設定してみると正常に動作。
ActiveReportも問題無く動作。
