SimpleCSV
===============

Simple CSV Reader and Writer for C#.

# Reading
## Reading Records

You can loop the lines and read them.

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

If the end of line characters are CR/LF, `CSVReader` converts to LF.
When single CR appeared, `CSVReader` reads CR as it is.

# Writing
## Writing Records

You can loop the lines and write them.

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

`CSVWriter#WriteNext()` writes line end string specified `lineEnd` parameter of constructor.
The default `lineEnd` parameter is LF.

# Configuration

There are constructors and builder class to specify behavior.

## Separator and Quote Character

You can use a tab for separator.

```c#
var reader = new CSVReader(new StreamReader("file.csv"), '\t');
```

You can use a single quote character rather than double quote character.

```c#
var reader = new CSVReader(new StreamReader("file.csv"), '\t', '\'');
```

## Skip the First Few Lines

You may also skip the first few lines of the file.

```c#
var reader = new CSVReader(new StreamReader("file.csv"), '\t', '\'', 2);
```

## Use the Builder Class

You can use the CSVParserBuilder and the CSVReaderBuilder to create the CSVReader.

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

And you can also use the CSVWriterBuilder to create the CSVWriter.

```c#
var writer = new CSVWriterBuilder(new StreamWriter("file.csv"))
    .WithLineEnd("\r\n")
	.Build();
```

## Escape Character

You can use the another character rather than back quote character for escaping a separator or quote.

```c#
var parser = new CSVParserBuilder()
    .WithEscapeChar('?')
	.Build();
var reader = new CSVReaderBuilder(new StreamReader("file.csv"))
	.WithCSVParser(parser)
	.Build();
```

## Other Stuff

- Strict quotes - Characters outside the quotes are ignored.
- Ignore leading white space - White space in front of a quote in field is ignored.
- Ignore quotations - Treat quotations like any other character.
- Field as null - Which field content will be returned as null: EmptySeparators, EmptyQuotes, Both, Neither.

# CSV Samples

## For Reading

### Quote Samples

- CSV line is: `a,"bcd",e`
    - returns: `{ "a", "bcd", "e" }`
- CSV line is: `a,"b,c",d`
    - returns: `{ "a", "b,c", "d" }`
- CSV lines are: `a,"b` and: `c", d`
    - returns: `{ "a", "b\nc", "d" }`
- CSV line is: `"one",t"w"o,"three"`
    - where `WithStrictQuotes(true)`
	- returns: `{ "one", "w", "three" }`
- CSV line is: `one, t"wo, t"hree`
    - returns: `{ "one", "t\"wo, t\"hree" }`
- CSV line is: `one, t"wo, t"hree`
    - where `WithIgnoreQuotations(true)`
    - returns: `{ "one", "t\"wo", "t\"hree" }`
- CSV line is: `one, t"wo, three`
    - throws `IOException("Un-terminated quoted field at end of CSV line")`

### White Space Samples

- CSV line is: `"one", "two" , "three"`
    - where `WithIgnoreLeadingWhiteSpace(false)`
    - returns: `{ "one", " \"two\" ", " \"three\"" }`
- CSV line is: `"one", "two" , "three"`
    - where `WithStrictQuotes(true)`, `WithIgnoreLeadingWhiteSpace(false)`
	- returns: `{ "one", "two", "three" }`

### Empty Field Samples

- CSV line is: `,,,"",`
	- returns: `{ "", "", "", "", "" }`
- CSV line is: `, ,," ",`
    - returns: `{ "", " ", "", " ", "" }`
- CSV line is: `, ,,"",`
    - where `WithFieldAsNull(CSVReaderNullFieldIndicator.EmptySeparators)`
    - returns: `{ null, " ", null, "", null }`

## For Writing

### Quoting

- Fields are: `{ "abc", "d,e,f", "ghi" }`
    - CSV line is: `"abc","d,e,f","ghi"`
- Fields are: `{ "a \" b", "cde" }`
    - CSV line is: `"a "" b","cde"`
- Fields are: `{ "a \n b", "cde" }`
    - CSV lines are: `"a ` and: ` b","cde"`

### Auto Quoting

- Fields are: `{ "abc", "d,e,f", "g\"h\"i" }`
    - where `applyQuotesToAll` parameter is false, like `csvWriter.WriteNext(fields, false);`
	- CSV line is: `abc,"d,e,f","g""h""i"`
