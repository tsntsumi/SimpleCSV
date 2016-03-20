CSV Parser Performance Test
===========================

This project tests parsing performance. We will use the [worldcitiespop.txt](http://www.maxmind.com/download/worldcities/worldcitiespop.txt.gz) as the input file by [Maxmind](http://www.maxmind.com/). It contains more than 3 million rows.

Please download [worldcitiespop.txt](http://www.maxmind.com/download/worldcities/worldcitiespop.txt.gz) and place it under PerformanceTest folder before executing the main class Performance.cs.

And we can also test a NuGet package CsvHelper parsing performance. If you don't need compare with CsvHelper performance, Please comment out the declearation below.

```c#
private List<AbstractParser> parsers = new List<AbstractParser>() {
    new SimpleCSVParser(),
    new ReadLineAndSplitParser(),
    // new CsvHelperParser(),    // CsvHelper Parser performance test class
};
```

Notice: The input file is **not** [RFC 4180](https://www.rfc-editor.org/rfc/rfc4180.txt) compliant.

Statistics
----------

- CPU: 2.8 GHz Intel Core i7
- Memory: 16 GB 1600 MHz DDR3
- OS: Mac OS X El Capitan (10.11.3)
- Mono: version 4.2.3

###Processing 3,173,959 rows of non [RFC 4180](https://www.rfc-editor.org/rfc/rfc4180.txt) compliant input.

| Parser             | Average | % Slower than best |
|--------------------|---------|--------------------|
| ReadLine and Split | 3812 ms | Best time!         |
| SimpleCSV          | 6730 ms | 76%                |
| CsvHelper          | 8623 ms | 126%               |
