# SimpleCSV
C#のシンプルなCSVリーダーおよびライターです。

## 読み込み
### レコードを読み込む方法

一行ずつループして読み込むことができます。

```c#
using (var csvReader = new CSVReader(new StreamReader("/path/to/file.csv")))
{
    string[] line;
    while ((line = csvReader.ReadNext()) != null)
    {
        Console.WriteLine("{0}, {1}, etc...", line[0], line[1]);
    }
}
```

SimpleCSVは、マルチラインフィールドに対応しています。ダブルクォートで囲まれたフィールドに改行コードが含まれていた場合、改行を含んだフィールドとして読み込みます。もし改行コードがCR/LFの場合、`CSVReader`はLFに変換して読み込みます。ただしCRが一つだけ現れた場合は、`CSVReader`はCRをそのまま読み込みます。

## 書き込み

一行ずつループして書き込むことができます。

```c#
using (var csvWriter = new CSVWriter(new StreamWriter("/path/to/file.csv"), '\t'))
{
    foreach (string line in list)
    {
        string[] fields = line.Split(',');
        csvWriter.WriteNext(fields);
    }
}
```

`CSVWrite#WriteNext()`メソッドはコンストラクタの`lineEnd`引数で指定された改行文字列を行の最後に書き込みます。`lineEnd`引数のデフォルトはLFです。

## 設定

振る舞いを指定するための、コンストラクタとビルダークラスがあります。

### 区切り文字と囲み文字

区切り文字としてタブコードを指定することができます。

```c#
var reader = new CSVReader(new StreamReader("file.csv"), '\t');
```

囲み文字として二重引用符ではなく引用符を指定することもできます。

```c#
var reader = new CSVReader(new StreamReader("file.csv"), '\t', '\'');
```

### 先頭の何行かをスキップ

ファイルの先頭の何行かをスキップすることもできます。

```c#
var reader = new CSVReader(new StreamReader("file.csv"), '\t', '\'', 2);
```

### ビルダークラスの使い方

`CSVReader`を生成するときに、`CSVParserBuilder`クラスと`CSVReaderBuilder`クラスを使うことができます。

```c#
var parser = new CSVParserBuilder()
    .WithSeparator('\t')
	.WithQuoteChar('\'')
	.Build();
var reader = new CSVReaderBuilder(new StreamReader("file.csv"))
    .WithSkipLines(2)
	.WithCSVParser(parser)
	.Build();
```

そして、`CSVWrite`クラスを生成するときには、`CSVWriterBuilder`クラスを使うことができます。

```c#
var writer = new CSVWriterBuilder(new StreamWriter("file.csv"))
    .WithLineEnd("\r\n")
	.Build();
```

### エスケープ文字

区切り文字と囲み文字をエスケープするのに、バッククォート以外の別の文字を使うことができます。

```c#
var parser = new CSVParserBuilder()
    .WithEscapeChar('?')
	.Build();
var reader = new CSVReaderBuilder(new StreamReader("file.csv"))
	.WithCSVParser(parser)
	.Build();
```

### その他

- 囲み文字を強制する (`strictQuotes`) - 囲み文字で囲んだ文字列の外側の文字を無視します。
- 先行する空白文字を無視する (`ignoreLeadingWhiteSpace`) - フィールドの囲み文字の前にある空白文字を無視します。
- 囲み文字を無視する (`ignoreQuotations`) - 囲み文字を他の文字と同じように扱う。
- 空のフィールドを`null`にする (`fieldAsNull`) - どういった空のフィールドを`null`として扱うかを指定します: `EmptySeparators`, `EmptyQuotes`, `Both`, `Neither`。

## CSVのサンプル
### 読み込みのサンプル
#### 囲み文字のサンプル

|CSV の行               | 条件                      | 返ってくるフィールド     |
|----------------------|--------------------------|----------------------|
|`a,"bcd",e`           |                          | `{ "a", "bcd", "e" }` |
|`a,"b,c",d`           |                          | `{ "a", "b,c", "d" }` |
|`a,"b`<br/>`c", d`    |                          | `{ "a", "b\nc", "d" }` |
|`"one",t"w"o,"three"` | `WithStrictQuotes(true)` | `{ "one", "w", "three" }` |
|`one, t"wo, t"hree`   |                          | `{ "one", "t\"wo, t\"hree" }` |
|`one, t"wo, t"hree`   | `WithIgnoreQuotations(true)` | `{ "one", "t\"wo", "t\"hree" }` |
|`one, t"wo, three`    |                          | throws `IOException("Un-terminated quoted field at end of CSV line")` |

#### 空白文字のサンプル

|CSV の行                  | 条件                                  | 返ってくるフィールド |
|-------------------------|--------------------------------------|----------------------|
|`"one", "two" , "three"` | `WithIgnoreLeadingWhiteSpace(false)` | `{ "one", " \"two\" ",`<br/>`" \"three\"" }` |
|`"one", "two" , "three"` | `WithStrictQuotes(true)`, <br/> `WithIgnoreLeadingWhiteSpace(false)` | `{ "one", "two", "three" }` |

#### 空のフィールドのサンプル

`CSVPerserBulider#WithFieldAsNull()`に`CSVReaderNullFieldIndicator`列挙体のフィールドを指定した場合の例を以下に示します。何も指定しない場合は`Neither`を指定したのと同じです。

|CSV の行               | 条件               | 返ってくるフィールド                  |
|----------------------|-------------------|-----------------------------------|
|`,,,"",`              |                   | `{ "", "", "", "", "" }`          |
|`, ,," ",`            |                   | `{ "", " ", "", " ", "" }`        |
|`, ,,"",`             | `EmptySeparators` | `{ null, " ", null, "", null }`   |
|`, ,,"",`             | `EmptyQuotes`     | `{ "", " ", "", null, "" }`       |
|`, ,,"",`             | `Both`            | `{ null, " ", null, null, null }` |

### 書き込みのサンプル
#### クォーティング

|フィールド                    | CSVの行 |
|----------------------------|--------------------------|
|`{ "abc", "d,e,f", "ghi" }` | `"abc","d,e,f","ghi"`    |
|`{ "a \" b", "cde" }`       | `"a "" b","cde"`         |
|`{ "a \n b", "cde" }`       | `"a `<br/>` b","cde"`    |

#### 自動クォーティング

`applyQuotesToAll`引数を`false`に指定した場合（`csvWriter.WriteNext(fields, false)`）。

|フィールド                        | CSVの行 |
|--------------------------------|--------------------------|
|`{ "abc", "d,e,f", "g\"h\"i" }` | `abc,"d,e,f","g""h""i"`  |

