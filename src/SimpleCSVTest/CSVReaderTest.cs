//
// CSVReaderTest.cs
//
// Author:
//       tsntsumi <tsntsumi@tsntsumi.com>
//
// Copyright (c) 2016 tsntsumi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using SimpleCSV;

namespace SimpleCSVTest
{
    public class CSVReaderTest
    {
        CSVReader csvr;
        StringBuilder strBuilder;

        [SetUp]
        public void SetUp()
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("a,b,c").Append("\n");   // standard case
            strBuilder.Append("a,\"b,b,b\",c").Append("\n");  // quoted elements
            strBuilder.Append(",,").Append("\n"); // empty elements
            strBuilder.Append("a,\"PO Box 123,\nKippax,ACT. 2615.\nAustralia\",d.\n");
            strBuilder.Append("\"Glen \"\"The Man\"\" Smith\",Athlete,Developer\n"); // Test quoted quote chars
            strBuilder.Append("\"\"\"\"\"\",\"test\"\n"); // """""","test"  representing:  "", test
            strBuilder.Append("\"a\nb\",b,\"\nd\",e\n");
            csvr = new CSVReader(new StringReader(strBuilder.ToString()));
        }

        [Test]
        public void TestParseLine()
        {
            // test normal case
            string[] nextLine = csvr.ReadNext();
            Assert.AreEqual("a", nextLine[0]);
            Assert.AreEqual("b", nextLine[1]);
            Assert.AreEqual("c", nextLine[2]);

            // test quoted commas
            nextLine = csvr.ReadNext();
            Assert.AreEqual("a", nextLine[0]);
            Assert.AreEqual("b,b,b", nextLine[1]);
            Assert.AreEqual("c", nextLine[2]);

            // test empty elements
            nextLine = csvr.ReadNext();
            Assert.AreEqual(3, nextLine.Length);

            // test multiline quoted
            nextLine = csvr.ReadNext();
            Assert.AreEqual(3, nextLine.Length);

            // test quoted quote chars
            nextLine = csvr.ReadNext();
            Assert.AreEqual("Glen \"The Man\" Smith", nextLine[0]);

            nextLine = csvr.ReadNext();
            Assert.AreEqual("\"\"", nextLine[0]); // check the tricky situation
            Assert.AreEqual("test", nextLine[1]); // make sure we didn't ruin the next field..

            nextLine = csvr.ReadNext();
            Assert.AreEqual(4, nextLine.Length);

            //test end of stream
            Assert.IsNull(csvr.ReadNext());
        }

        [Test]
        public void ReaderCanHandleNullInString() 
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("a,\0b,c");

            StringReader reader = new StringReader(sb.ToString());

            CSVReader defaultReader = new CSVReader(reader);

            String[] nextLine = defaultReader.ReadNext();
            Assert.AreEqual(3, nextLine.Length);
            Assert.AreEqual("a", nextLine[0]);
            Assert.AreEqual("\0b", nextLine[1]);
            Assert.AreEqual(0, nextLine[1][0]);
            Assert.AreEqual("c", nextLine[2]);
        }

        [Test]
        public void TestParseLineStrictQuote() 
        {
            csvr = new CSVReader(new StringReader(strBuilder.ToString()), ',', '\"', true);

            // test normal case
            String[] nextLine = csvr.ReadNext();
            Assert.AreEqual("", nextLine[0]);
            Assert.AreEqual("", nextLine[1]);
            Assert.AreEqual("", nextLine[2]);

            // test quoted commas
            nextLine = csvr.ReadNext();
            Assert.AreEqual("", nextLine[0]);
            Assert.AreEqual("b,b,b", nextLine[1]);
            Assert.AreEqual("", nextLine[2]);

            // test empty elements
            nextLine = csvr.ReadNext();
            Assert.AreEqual(3, nextLine.Length);

            // test multiline quoted
            nextLine = csvr.ReadNext();
            Assert.AreEqual(3, nextLine.Length);

            // test quoted quote chars
            nextLine = csvr.ReadNext();
            Assert.AreEqual("Glen \"The Man\" Smith", nextLine[0]);

            nextLine = csvr.ReadNext();
            Assert.True(nextLine[0] == "\"\""); // check the tricky situation
            Assert.True(nextLine[1] == "test"); // make sure we didn't ruin the next field..

            nextLine = csvr.ReadNext();
            Assert.AreEqual(4, nextLine.Length);
            Assert.AreEqual("a\nb", nextLine[0]);
            Assert.AreEqual("", nextLine[1]);
            Assert.AreEqual("\nd", nextLine[2]);
            Assert.AreEqual("", nextLine[3]);

            //test end of stream
            Assert.IsNull(csvr.ReadNext());
        }

        [Test]
        public void TestParseAll() 
        {
            Assert.AreEqual(7, csvr.ReadAll().Count);
        }

        [Test]
        public void TestOptionalConstructors() 
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("a\tb\tc").Append("\n");   // tab separated case
            sb.Append("a\t'b\tb\tb'\tc").Append("\n");  // single quoted elements
            CSVReader c = new CSVReader(new StringReader(sb.ToString()), '\t', '\'');

            String[] nextLine = c.ReadNext();
            Assert.AreEqual(3, nextLine.Length);

            nextLine = c.ReadNext();
            Assert.AreEqual(3, nextLine.Length);
        }

        [Test]
        public void ParseQuotedStringWithDefinedSeperator() 
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("a\tb\tc").Append("\n");   // tab separated case
            sb.Append("a\t\"b\tb\tb\"\tc").Append("\n");  // quoted elements

            CSVReader c = new CSVReader(new StringReader(sb.ToString()), '\t');

            String[] nextLine = c.ReadNext();
            Assert.AreEqual(3, nextLine.Length);

            nextLine = c.ReadNext();
            Assert.AreEqual(3, nextLine.Length);
        }

        [Test]
        public void TestSkippingLines() 
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Skip this line\t with tab").Append("\n");   // should skip this
            sb.Append("And this line too").Append("\n");   // and this
            sb.Append("a\t'b\tb\tb'\tc").Append("\n");  // single quoted elements
            CSVReader c = new CSVReader(new StringReader(sb.ToString()), '\t', '\'', 2);

            String[] nextLine = c.ReadNext();
            Assert.AreEqual(3, nextLine.Length);

            Assert.AreEqual("a", nextLine[0]);
        }

        /// <summary>
        /// Tests methods to get the number of lines and records read.
        /// </summary>
        [Test]
        public void LinesAndRecordsRead() 
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Skip this line\t with tab").Append("\n");   // should skip this
            sb.Append("And this line too").Append("\n");   // and this
            sb.Append("a,b,c").Append("\n");  // second line
            sb.Append("\n");                  // no data here just a blank line
            sb.Append("a,\"b\nb\",c");

            CSVReader c = new CSVReader(new StringReader(sb.ToString()), ',', '"', 2);

            Assert.AreEqual(0, c.LinesRead);
            Assert.AreEqual(0, c.RecordsRead);

            String[] nextLine = c.ReadNext();
            Assert.AreEqual(3, nextLine.Length);

            Assert.AreEqual(3, c.LinesRead);
            Assert.AreEqual(1, c.RecordsRead);

            nextLine = c.ReadNext();
            Assert.AreEqual(1, nextLine.Length);
            Assert.AreEqual(0, nextLine[0].Length);

            Assert.AreEqual(4, c.LinesRead);
            Assert.AreEqual(2, c.RecordsRead);  // A blank line is considered a record with a single element

            nextLine = c.ReadNext();
            Assert.AreEqual(3, nextLine.Length);

            Assert.AreEqual(6, c.LinesRead);
            Assert.AreEqual(3, c.RecordsRead);  // two lines read to get a single record.

            nextLine = c.ReadNext();  // reading after all the data has been read.
            Assert.IsNull(nextLine);

            Assert.AreEqual(6, c.LinesRead);
            Assert.AreEqual(3, c.RecordsRead);
        }

        /// <summary>
        /// Tests option to skip the first few lines of a file.
        /// </summary>
        [Test]
        public void TestSkippingLinesWithDifferentEscape() 
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Skip this line?t with tab").Append("\n");   // should skip this
            sb.Append("And this line too").Append("\n");   // and this
            sb.Append("a\t'b\tb\tb'\t'c'").Append("\n");  // single quoted elements
            CSVReader c = new CSVReader(new StringReader(sb.ToString()), '\t', '\'', '?', 2);

            String[] nextLine = c.ReadNext();

            Assert.AreEqual(3, nextLine.Length);

            Assert.AreEqual("a", nextLine[0]);
            Assert.AreEqual("b\tb\tb", nextLine[1]);
            Assert.AreEqual("c", nextLine[2]);
        }

        [Test]
        public void TestNormalParsedLine() 
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("a,1234567,c").Append("\n");// a,1234,c

            CSVReader c = new CSVReader(new StringReader(sb.ToString()));

            String[] nextLine = c.ReadNext();
            Assert.AreEqual(3, nextLine.Length);

            Assert.AreEqual("a", nextLine[0]);
            Assert.AreEqual("1234567", nextLine[1]);
            Assert.AreEqual("c", nextLine[2]);
        }

        [Test]
        public void TestASingleQuoteAsDataElement() 
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("a,'''',c").Append("\n");// a,',c

            CSVReader c = new CSVReader(new StringReader(sb.ToString()), ',', '\'');

            String[] nextLine = c.ReadNext();
            Assert.AreEqual(3, nextLine.Length);

            Assert.AreEqual("a", nextLine[0]);
            Assert.AreEqual(1, nextLine[1].Length);
            Assert.AreEqual("\'", nextLine[1]);
            Assert.AreEqual("c", nextLine[2]);
        }

        [Test]
        public void TestASingleQuoteAsDataElementWithEmptyField() 
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("a,'',c").Append("\n");// a,,c

            CSVReader c = new CSVReader(new StringReader(sb.ToString()), ',', '\'');

            String[] nextLine = c.ReadNext();
            Assert.AreEqual(3, nextLine.Length);

            Assert.AreEqual("a", nextLine[0]);
            Assert.AreEqual(0, nextLine[1].Length);
            Assert.AreEqual("", nextLine[1]);
            Assert.AreEqual("c", nextLine[2]);
        }

        [Test]
        public void TestSpacesAtEndOfString() 
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("\"a\",\"b\",\"c\"   ");

            CSVReader c = new CSVReader(new StringReader(sb.ToString()), CSVParser.DefaultSeparator, CSVParser.DefaultQuoteCharacter, true);

            String[] nextLine = c.ReadNext();
            Assert.AreEqual(3, nextLine.Length);

            Assert.AreEqual("a", nextLine[0]);
            Assert.AreEqual("b", nextLine[1]);
            Assert.AreEqual("c", nextLine[2]);
        }

        [Test]
        public void TestEscapedQuote() 
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("a,\"123\\\"4567\",c").Append("\n");// a,123"4",c

            CSVReader c = new CSVReader(new StringReader(sb.ToString()));

            String[] nextLine = c.ReadNext();
            Assert.AreEqual(3, nextLine.Length);

            Assert.AreEqual("123\"4567", nextLine[1]);
        }

        [Test]
        public void TestEscapedEscape() 
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("a,\"123\\\\4567\",c").Append("\n");// a,123"4",c

            CSVReader c = new CSVReader(new StringReader(sb.ToString()));

            String[] nextLine = c.ReadNext();
            Assert.AreEqual(3, nextLine.Length);

            Assert.AreEqual("123\\4567", nextLine[1]);
        }

        /// <summary>
        /// Test a line where one of the elements is two single quotes and the
        /// quote character is the default double quote.  The expected result is two
        /// single quotes.
        /// </summary>
        [Test]
        public void TestSingleQuoteWhenDoubleQuoteIsQuoteChar() 
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("a,'',c").Append("\n");// a,'',c

            CSVReader c = new CSVReader(new StringReader(sb.ToString()));

            String[] nextLine = c.ReadNext();
            Assert.AreEqual(3, nextLine.Length);

            Assert.AreEqual("a", nextLine[0]);
            Assert.AreEqual(2, nextLine[1].Length);
            Assert.AreEqual("''", nextLine[1]);
            Assert.AreEqual("c", nextLine[2]);
        }

        /// <summary>
        /// Test a normal line with three elements and all elements are quoted.
        /// </summary>
        [Test]
        public void TestQuotedParsedLine() 
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("\"a\",\"1234567\",\"c\"").Append("\n"); // "a","1234567","c"

            CSVReader c = new CSVReader(new StringReader(sb.ToString()), CSVParser.DefaultSeparator, CSVParser.DefaultQuoteCharacter, true);

            String[] nextLine = c.ReadNext();
            Assert.AreEqual(3, nextLine.Length);

            Assert.AreEqual("a", nextLine[0]);
            Assert.AreEqual(1, nextLine[0].Length);

            Assert.AreEqual("1234567", nextLine[1]);
            Assert.AreEqual("c", nextLine[2]);
        }

        [Test]
        public void TestOutOfPlaceQuotes() 
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("a,b,c,ddd\\\"eee\nf,g,h,\"iii,jjj\"");

            CSVReader c = new CSVReader(new StringReader(sb.ToString()));

            String[] nextLine = c.ReadNext();

            Assert.AreEqual("a", nextLine[0]);
            Assert.AreEqual("b", nextLine[1]);
            Assert.AreEqual("c", nextLine[2]);
            Assert.AreEqual("ddd\"eee", nextLine[3]);
        }

        [Test]
        public void quoteAndEscapeMustBeDifferent()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("a,b,c,ddd\\\"eee\nf,g,h,\"iii,jjj\"");

            Assert.Throws<ArgumentException>(() => new CSVReader(new StringReader(sb.ToString()), CSVParser.DefaultSeparator, CSVParser.DefaultQuoteCharacter,
                CSVParser.DefaultQuoteCharacter, CSVReader.DefaultSkipLines, CSVParser.DefaultStrictQuotes,
                CSVParser.DefaultIgnoreLeadingWhiteSpace));
        }

        [Test]
        public void SeparatorAndEscapeMustBeDifferent()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("a,b,c,ddd\\\"eee\nf,g,h,\"iii,jjj\"");

            Assert.Throws<ArgumentException>(() =>
                new CSVReader(new StringReader(sb.ToString()), CSVParser.DefaultSeparator, CSVParser.DefaultQuoteCharacter,
                    CSVParser.DefaultSeparator, CSVReader.DefaultSkipLines, CSVParser.DefaultStrictQuotes,
                    CSVParser.DefaultIgnoreLeadingWhiteSpace)
            );
        }

        [Test]
        public void separatorAndQuoteMustBeDifferent() {
            StringBuilder sb = new StringBuilder();

            sb.Append("a,b,c,ddd\\\"eee\nf,g,h,\"iii,jjj\"");

            Assert.Throws<ArgumentException>(() =>
                new CSVReader(new StringReader(sb.ToString()), CSVParser.DefaultSeparator, CSVParser.DefaultSeparator,
                    CSVParser.DefaultEscapeCharacter, CSVReader.DefaultSkipLines, CSVParser.DefaultStrictQuotes,
                    CSVParser.DefaultIgnoreLeadingWhiteSpace)
            );
        }

        [Test]
        public void TestEmptyField()
        {
            CSVReader csvReader = new CSVReader(new StringReader("\"\",a\n\"\",b\n"));

            String[] firstRow = csvReader.ReadNext();
            Assert.AreEqual(2, firstRow.Length);
            Assert.True(firstRow[0] == "");
            Assert.AreEqual("a", firstRow[1]);

            String[] secondRow = csvReader.ReadNext();
            Assert.AreEqual(2, secondRow.Length);
            Assert.True(secondRow[0] == "");
            Assert.AreEqual("b", secondRow[1]);
        }

        [Test]
        public void ByDefaultEmptyFieldsAreBlank() 
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(",,,\"\",");

            StringReader stringReader = new StringReader(sb.ToString());

            CSVReader csvReader = new CSVReader(stringReader);

            String[] row = csvReader.ReadNext();

            Assert.AreEqual(5, row.Length);
            Assert.AreEqual("", row[0]);
            Assert.AreEqual("", row[1]);
            Assert.AreEqual("", row[2]);
            Assert.AreEqual("", row[3]);
            Assert.AreEqual("", row[4]);
        }

        [Test]
        public void TreatEmptyFieldsAsNull() 
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(",,,\"\",");

            StringReader stringReader = new StringReader(sb.ToString());

            CSVReader csvReader = new CSVReader(stringReader, CSVReader.DefaultSkipLines,
                                      new CSVParser(',', '"', '\\', CSVParser.DefaultStrictQuotes,
                                          CSVParser.DefaultIgnoreLeadingWhiteSpace, CSVParser.DefaultIgnoreQuotations,
                                          CSVReaderNullFieldIndicator.EmptySeparators));

            string[] item = csvReader.ReadNext();

            Assert.AreEqual(5, item.Length);
            Assert.IsNull(item[0]);
            Assert.IsNull(item[1]);
            Assert.IsNull(item[2]);
            Assert.AreEqual("", item[3]);
            Assert.IsNull(item[4]);
        }

        [Test]
        public void TreatEmptyDelimitedFieldsAsNull() 
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(",,,\"\",");

            StringReader stringReader = new StringReader(sb.ToString());

            CSVReader csvReader = new CSVReader(stringReader, CSVReader.DefaultSkipLines,
                new CSVParser(',', '"', '\\', CSVParser.DefaultStrictQuotes,
                    CSVParser.DefaultIgnoreLeadingWhiteSpace, CSVParser.DefaultIgnoreQuotations,
                    CSVReaderNullFieldIndicator.EmptyQuotes));

            string[] item = csvReader.ReadNext();

            Assert.AreEqual(5, item.Length);
            Assert.AreEqual("", item[0]);
            Assert.AreEqual("", item[1]);
            Assert.AreEqual("", item[2]);
            Assert.IsNull(item[3]);
            Assert.AreEqual("", item[4]);
        }

        [Test]
        public void TreatEmptyFieldsDelimitedOrNotAsNull() 
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(",,,\"\",");

            StringReader stringReader = new StringReader(sb.ToString());

            CSVReader csvReader = new CSVReader(stringReader, CSVReader.DefaultSkipLines,
                                      new CSVParser(',', '"', '\\', CSVParser.DefaultStrictQuotes,
                                          CSVParser.DefaultIgnoreLeadingWhiteSpace, CSVParser.DefaultIgnoreQuotations,
                                          CSVReaderNullFieldIndicator.Both));

            string[] item = csvReader.ReadNext();

            Assert.AreEqual(5, item.Length);
            Assert.IsNull(item[0]);
            Assert.IsNull(item[1]);
            Assert.IsNull(item[2]);
            Assert.IsNull(item[3]);
            Assert.IsNull(item[4]);
        }
    }
}

