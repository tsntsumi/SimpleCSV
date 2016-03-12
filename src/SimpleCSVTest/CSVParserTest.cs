//
// CSVParserTest.cs
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
using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using SimpleCSV;

namespace SimpleCSVTest
{
    [TestFixture()]
    public class CSVParserTest
    {
        CSVParser csvParser;

        [SetUp]
        public void SetUp()
        {
            csvParser = new CSVParser();
        }

        [Test()]
        public void TestParseLine() 
        {
            string[] items = csvParser.ParseLine(new StringReader("This, is, a, test."));
            Assert.AreEqual(4, items.Length);
            Assert.AreEqual("This", items[0]);
            Assert.AreEqual(" is", items[1]);
            Assert.AreEqual(" a", items[2]);
            Assert.AreEqual(" test.", items[3]);
        }

        [Test]
        public void ParseSimpleString()
        {
            string[] items = csvParser.ParseLine(new StringReader("a,b,c"));
            Assert.AreEqual(3, items.Length);
            Assert.AreEqual("a", items[0]);
            Assert.AreEqual("b", items[1]);
            Assert.AreEqual("c", items[2]);
        }


        [Test]
        public void ParseSimpleQuotedString()
        {
            string[] items = csvParser.ParseLine(new StringReader("\"a\",\"b\",\"c\""));
            Assert.AreEqual(3, items.Length);
            Assert.AreEqual("a", items[0]);
            Assert.AreEqual("b", items[1]);
            Assert.AreEqual("c", items[2]);
        }

        [Test]
        public void ParseSimpleQuotedStringWithSpaces() 
        {
            CSVParser parser = new CSVParser(CSVParser.DefaultSeparator, CSVParser.DefaultQuoteCharacter, CSVParser.DefaultEscapeCharacter,
                true, false);

            string[] items = parser.ParseLine(new StringReader(" \"a\" , \"b\" , \"c\" "));
            Assert.AreEqual(3, items.Length);
            Assert.AreEqual("a", items[0]);
            Assert.AreEqual("b", items[1]);
            Assert.AreEqual("c", items[2]);
        }

        /// <summary>
        /// Tests quotes in the middle of an element.
        /// </summary>
        [Test]
        public void TestParsedLineWithInternalQuota() 
        {
            string[] items = csvParser.ParseLine(new StringReader("a,123\"4\"567,c"));
            Assert.AreEqual(3, items.Length);
            Assert.AreEqual("123\"4\"567", items[1]);
        }

        [Test]
        public void ParseQuotedStringWithCommas()
        {
            string[] items = csvParser.ParseLine(new StringReader("a,\"b,b,b\",c"));
            Assert.AreEqual("a", items[0]);
            Assert.AreEqual("b,b,b", items[1]);
            Assert.AreEqual("c", items[2]);
            Assert.AreEqual(3, items.Length);
        }

        [Test]
        public void ParseQuotedStringWithDefinedSeperator() 
        {
            csvParser = new CSVParser(':');

            string[] items = csvParser.ParseLine(new StringReader("a:\"b:b:b\":c"));
            Assert.AreEqual("a", items[0]);
            Assert.AreEqual("b:b:b", items[1]);
            Assert.AreEqual("c", items[2]);
            Assert.AreEqual(3, items.Length);
        }

        [Test]
        public void ParseQuotedStringWithDefinedSeperatorAndQuote() 
        {
            csvParser = new CSVParser(':', '\'');

            string[] items = csvParser.ParseLine(new StringReader("a:'b:b:b':c"));
            Assert.AreEqual("a", items[0]);
            Assert.AreEqual("b:b:b", items[1]);
            Assert.AreEqual("c", items[2]);
            Assert.AreEqual(3, items.Length);
        }

        [Test]
        public void ParseEmptyElements()
        {
            string[] items = csvParser.ParseLine(new StringReader(",,"));
            Assert.AreEqual(3, items.Length);
            Assert.AreEqual("", items[0]);
            Assert.AreEqual("", items[1]);
            Assert.AreEqual("", items[2]);
        }

        [Test]
        public void ParseMultiLinedQuoted()
        {
            string[] items = csvParser.ParseLine(new StringReader("a,\"PO Box 123,\nKippax,ACT. 2615.\nAustralia\",d.\n"));
            Assert.AreEqual(3, items.Length);
            Assert.AreEqual("a", items[0]);
            Assert.AreEqual("PO Box 123,\nKippax,ACT. 2615.\nAustralia", items[1]);
            Assert.AreEqual("d.", items[2]);
        }

        [Test]
        public void ParseMultiLinedQuotedwithCarriageReturns()
        {
            string[] items = csvParser.ParseLine(new StringReader("a,\"PO Box 123,\r\nKippax,ACT. 2615.\r\nAustralia\",d.\n"));
            Assert.AreEqual(3, items.Length);
            Assert.AreEqual("a", items[0]);
            Assert.AreEqual("PO Box 123,\r\nKippax,ACT. 2615.\r\nAustralia", items[1]);
            Assert.AreEqual("d.", items[2]);
        }

        [Test]
        public void TestADoubleQuoteAsDataElement()
        {

            String[] nextLine = csvParser.ParseLine(new StringReader("a,\"\"\"\",c"));// a,"""",c

            Assert.AreEqual(3, nextLine.Length);

            Assert.AreEqual("a", nextLine[0]);
            Assert.AreEqual(1, nextLine[1].Length);
            Assert.AreEqual("\"", nextLine[1]);
            Assert.AreEqual("c", nextLine[2]);

        }

        [Test]
        public void TestEscapedDoubleQuoteAsDataElement() 
        {
            String[] nextLine = csvParser.ParseLine(new StringReader("\"test\",\"this,test,is,good\",\"\\\"test\\\"\",\"\\\"quote\\\"\""));
            // "test","this,test,is,good","\"test\",\"quote\""

            Assert.AreEqual(4, nextLine.Length);

            Assert.AreEqual("test", nextLine[0]);
            Assert.AreEqual("this,test,is,good", nextLine[1]);
            Assert.AreEqual("\"test\"", nextLine[2]);
            Assert.AreEqual("\"quote\"", nextLine[3]);

        }

        [Test]
        public void ParseQuotedQuoteCharacters() 
        {
            String[] nextLine = csvParser.ParseLine(new StringReader("\"Glen \"\"The Man\"\" Smith\",Athlete,Developer\n"));
            Assert.AreEqual(3, nextLine.Length);
            Assert.AreEqual("Glen \"The Man\" Smith", nextLine[0]);
            Assert.AreEqual("Athlete", nextLine[1]);
            Assert.AreEqual("Developer", nextLine[2]);
        }

        [Test]
        public void ParseMultipleQuotes() 
        {
            String[] nextLine = csvParser.ParseLine(new StringReader("\"\"\"\"\"\",\"test\"\n")); // """""","test"  representing:  "", test
            Assert.AreEqual("\"\"", nextLine[0]); // check the tricky situation
            Assert.AreEqual("test", nextLine[1]); // make sure we didn't ruin the next field..
            Assert.AreEqual(2, nextLine.Length);
        }

        [Test]
        public void ParseTrickyString() 
        {
            String[] nextLine = csvParser.ParseLine(new StringReader("\"a\nb\",b,\"\nd\",e\n"));
            Assert.AreEqual(4, nextLine.Length);
            Assert.AreEqual("a\nb", nextLine[0]);
            Assert.AreEqual("b", nextLine[1]);
            Assert.AreEqual("\nd", nextLine[2]);
            Assert.AreEqual("e", nextLine[3]);
        }

        private String SetUpMultiLineInsideQuotes() 
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Small test,\"This is a test across \ntwo lines.\"");

            return sb.ToString();
        }

        [Test]
        public void TestAMultiLineInsideQuotes() 
        {
            String testString = SetUpMultiLineInsideQuotes();

            String[] nextLine = csvParser.ParseLine(new StringReader(testString));
            Assert.AreEqual(2, nextLine.Length);
            Assert.AreEqual("Small test", nextLine[0]);
            Assert.AreEqual("This is a test across \ntwo lines.", nextLine[1]);
        }

        [Test]
        public void TestStrictQuoteSimple() 
        {
            csvParser = new CSVParser(',', '"', '\\', true);
            String testString = "\"a\",\"b\",\"c\"";

            String[] nextLine = csvParser.ParseLine(new StringReader(testString));
            Assert.AreEqual(3, nextLine.Length);
            Assert.AreEqual("a", nextLine[0]);
            Assert.AreEqual("b", nextLine[1]);
            Assert.AreEqual("c", nextLine[2]);
        }

        [Test]
        public void TestNotStrictQuoteSimple() 
        {
            csvParser = new CSVParser(',', '"', '\\', false);
            String testString = "\"a\",\"b\",\"c\"";

            String[] nextLine = csvParser.ParseLine(new StringReader(testString));
            Assert.AreEqual(3, nextLine.Length);
            Assert.AreEqual("a", nextLine[0]);
            Assert.AreEqual("b", nextLine[1]);
            Assert.AreEqual("c", nextLine[2]);
        }

        [Test]
        public void TestStrictQuoteWithSpacesAndTabs() 
        {
            csvParser = new CSVParser(',', '"', '\\', true);
            String testString = " \t      \"a\",\"b\"      \t       ,   \"c\"   ";

            String[] nextLine = csvParser.ParseLine(new StringReader(testString));
            Assert.AreEqual(3, nextLine.Length);
            Assert.AreEqual("a", nextLine[0]);
            Assert.AreEqual("b", nextLine[1]);
            Assert.AreEqual("c", nextLine[2]);
        }

        /// <summary>
        /// Shows that without the strict quotes SimpleCSV will read until the separator or the end of the line.
        /// </summary>
        [Test]
        public void TestNotStrictQuoteWithSpacesAndTabs() 
        {
            csvParser = new CSVParser(',', '\"', '\\', false);
            String testString = " \t      \"a\",\"b\"      \t       ,   \"c\"   ";

            String[] nextLine = csvParser.ParseLine(new StringReader(testString));
            Assert.AreEqual(3, nextLine.Length);
            Assert.AreEqual("a", nextLine[0]);
            Assert.AreEqual("b\"      \t       ", nextLine[1]);
            Assert.AreEqual("c\"   ", nextLine[2]);
        }

        [Test]
        public void TestStrictQuoteWithGarbage()
        {
            csvParser = new CSVParser(',', '\"', '\\', true);
            String testString = "abc',!@#\",\\\"\"   xyz,";

            String[] nextLine = csvParser.ParseLine(new StringReader(testString));
            Assert.AreEqual(3, nextLine.Length);
            Assert.AreEqual("", nextLine[0]);
            Assert.AreEqual(",\"", nextLine[1]);
            Assert.AreEqual("", nextLine[2]);
        }

        [Test]
        public void TestCanIgnoreQuotations() 
        {
            csvParser = new CSVParser(CSVParser.DefaultSeparator,
                CSVParser.DefaultQuoteCharacter,
                CSVParser.DefaultEscapeCharacter,
                CSVParser.DefaultStrictQuotes,
                CSVParser.DefaultIgnoreLeadingWhiteSpace,
                true);
            String testString = "Bob,test\",Beaumont,TX";

            String[] nextLine = csvParser.ParseLine(new StringReader(testString));
            Assert.AreEqual(4, nextLine.Length);
            Assert.AreEqual("Bob", nextLine[0]);
            Assert.AreEqual("test", nextLine[1]);
            Assert.AreEqual("Beaumont", nextLine[2]);
            Assert.AreEqual("TX", nextLine[3]);
        }

        [TestCase(ExpectedException = typeof(IOException))]
        public void TestFalseIgnoreQuotations()
        {
            csvParser = new CSVParser(CSVParser.DefaultSeparator,
                CSVParser.DefaultQuoteCharacter,
                CSVParser.DefaultEscapeCharacter,
                CSVParser.DefaultStrictQuotes,
                CSVParser.DefaultIgnoreLeadingWhiteSpace,
                false);
            String testString = "Bob,test\",Beaumont,TX";

            String[] nextLine = csvParser.ParseLine(new StringReader(testString));
            Assert.AreEqual(4, nextLine.Length);
        }

        [Test]
        public void TestQuoteWithinField()
        {
            csvParser = new CSVParser(';',
                CSVParser.DefaultQuoteCharacter,
                CSVParser.DefaultEscapeCharacter,
                CSVParser.DefaultStrictQuotes,
                CSVParser.DefaultIgnoreLeadingWhiteSpace,
                true);
            String testString = "RPO;2012;P; ; ; ;SDX;ACCESSORY WHEEL, 16\", ALUMINUM, DESIGN 1";

            String[] nextLine = csvParser.ParseLine(new StringReader(testString));
            Assert.AreEqual(8, nextLine.Length);
            Assert.AreEqual("RPO", nextLine[0]);
            Assert.AreEqual("2012", nextLine[1]);
            Assert.AreEqual("P", nextLine[2]);
            Assert.AreEqual(" ", nextLine[3]);
            Assert.AreEqual(" ", nextLine[4]);
            Assert.AreEqual(" ", nextLine[5]);
            Assert.AreEqual("SDX", nextLine[6]);
            Assert.AreEqual("ACCESSORY WHEEL, 16\", ALUMINUM, DESIGN 1", nextLine[7]);
        }

        [Test]
        public void TestEscapedQuote() 
        {
            csvParser = new CSVParser(',', '\'');

            String[] nextLine = csvParser.ParseLine(new StringReader("865,0,'AmeriKKKa\\'s_Most_Wanted','',294,0,0,0.734338696798625,'20081002052147',242429208,18448"));

            Assert.AreEqual(11, nextLine.Length);

            Assert.AreEqual("865", nextLine[0]);
            Assert.AreEqual("0", nextLine[1]);
            Assert.AreEqual("AmeriKKKa's_Most_Wanted", nextLine[2]);
            Assert.AreEqual("", nextLine[3]);
            Assert.AreEqual("18448", nextLine[10]);
        }

        [Test]
        public void TestEscapedCharacterBeforeACharacterThatDidNotNeedEscaping()
        {
            csvParser = new CSVParser(';');
            String[] nextLine = csvParser.ParseLine(new StringReader("field1;\\=field2;\"\"\"field3\"\"\"")); // field1;\=field2;"""field3"""

            Assert.AreEqual(3, nextLine.Length);

            Assert.AreEqual("field1", nextLine[0]);
            Assert.AreEqual("=field2", nextLine[1]);
            Assert.AreEqual("\"field3\"", nextLine[2]);
        }

        [Test]
        public void TestEscapedQuoteCharacterAtFirstOfField() 
        {
            String[] nextLine = csvParser.ParseLine(new StringReader("\"804503689\",\"London\",\"\"London\"shop\",\"address\",\"116.453182\",\"39.918884\""));
            // "804503689","London",""London"shop","address","116.453182","39.918884"

            Assert.AreEqual(6, nextLine.Length);

            Assert.AreEqual("804503689", nextLine[0]);
            Assert.AreEqual("London", nextLine[1]);
            Assert.AreEqual("\"London\"shop", nextLine[2]);
            Assert.AreEqual("address", nextLine[3]);
            Assert.AreEqual("116.453182", nextLine[4]);
            Assert.AreEqual("39.918884", nextLine[5]);
        }

        [TestCase(ExpectedException = typeof(IOException))]
        public void AnIOExceptionThrownifStringEndsInsideAQuotedString()
        {
            csvParser.ParseLine(new StringReader("This,is a \"bad line to parse."));
        }

        [Test]
        public void ParseLineAllowsQuotesAcrossMultipleLines() 
        {
            String[] nextLine = csvParser.ParseLine(new StringReader("This,\"is a \"good\" line\\\\ to parse\nbecause we are using parseQuotedField(multi:true).\""));

            Assert.AreEqual(2, nextLine.Length);
            Assert.AreEqual("This", nextLine[0]);
            Assert.AreEqual("is a \"good\" line\\ to parse\nbecause we are using parseQuotedField(multi:true).", nextLine[1]);
        }

        [Test]
        public void SpacesAtEndOfQuotedStringDoNotCountIfStrictQuotesIsTrue() 
        {
            CSVParser parser = new CSVParser(CSVParser.DefaultSeparator, CSVParser.DefaultQuoteCharacter, CSVParser.DefaultEscapeCharacter, true);
            String[] nextLine = parser.ParseLine(new StringReader("\"Line with\", \"spaces at end\"  "));

            Assert.AreEqual(2, nextLine.Length);
            Assert.AreEqual("Line with", nextLine[0]);
            Assert.AreEqual("spaces at end", nextLine[1]);
        }

        [Test]
        public void ReturnNullWhenNullPassedIn() 
        {
            String[] nextLine = csvParser.ParseLine(null);
            Assert.IsNull(nextLine);
        }

        [Test]
        public void ReturnNullWhenEofPassedIn() 
        {
            String[] nextLine = csvParser.ParseLine(new StringReader(""));
            Assert.IsNull(nextLine);
        }

        [Test]
        public void WhitespaceBeforeEscape()
        {
            String[] nextItem = csvParser.ParseLine(new StringReader("\"this\", \"is\",\"a test\"")); //"this", "is","a test"
            Assert.AreEqual("this", nextItem[0]);
            Assert.AreEqual("is", nextItem[1]);
            Assert.AreEqual("a test", nextItem[2]);
        }

        [Test]
        public void TestWithoutQuotes() 
        {
            CSVParser testParser = new CSVParser('\t');
            String[] nextItem = testParser.ParseLine(new StringReader("zo\"\"har\"\"at\t10-04-1980\t29\tC:\\\\foo.txt"));
            Assert.AreEqual(4, nextItem.Length);
            Assert.AreEqual("zo\"har\"at", nextItem[0]);
            Assert.AreEqual("10-04-1980", nextItem[1]);
            Assert.AreEqual("29", nextItem[2]);
            Assert.AreEqual("C:\\foo.txt", nextItem[3]);
        }

        [TestCase(ExpectedException = typeof(ArgumentException))]
        public void QuoteAndEscapeCannotBeTheSame()
        {
            new CSVParser(CSVParser.DefaultSeparator, CSVParser.DefaultQuoteCharacter, CSVParser.DefaultQuoteCharacter);
        }

        [Test]
        public void QuoteAndEscapeCanBeTheSameIfNull() 
        {
            new CSVParser(CSVParser.DefaultSeparator, CSVParser.NullCharacter, CSVParser.NullCharacter);
        }

        [TestCase(ExpectedException = typeof(ArgumentException))]
        public void SeparatorCharacterCannotBeNull()
        {
            new CSVParser(CSVParser.NullCharacter);
        }

        [TestCase(ExpectedException = typeof(ArgumentException))]
        public void SeparatorAndEscapeCannotBeTheSame()
        {
            new CSVParser(CSVParser.DefaultSeparator, CSVParser.DefaultQuoteCharacter, CSVParser.DefaultSeparator);
        }

        [TestCase(ExpectedException = typeof(ArgumentException))]
        public void SeparatorAndQuoteCannotBeTheSame()
        {
            new CSVParser(CSVParser.DefaultSeparator, CSVParser.DefaultSeparator, CSVParser.DefaultEscapeCharacter);
        }

        [Test]
        public void ParserHandlesNullInString() 
        {
            String[] nextLine = csvParser.ParseLine(new StringReader("because we are using\0 parseLineMulti."));

            Assert.AreEqual(1, nextLine.Length);
            Assert.AreEqual("because we are using\0 parseLineMulti.", nextLine[0]);
        }

        [Test]
        public void DefaultEmptyFieldsAreBlank()
        {
            String[] item = csvParser.ParseLine(new StringReader(",,,\"\","));

            Assert.AreEqual(5, item.Length);
            Assert.AreEqual("", item[0]);
            Assert.AreEqual("", item[1]);
            Assert.AreEqual("", item[2]);
            Assert.AreEqual("", item[3]);
            Assert.AreEqual("", item[4]);
        }

        [Test]
        public void TreatEmptyFieldsAsNull()
        {
            CSVParser parser = new CSVParser(CSVParser.DefaultSeparator, CSVParser.DefaultQuoteCharacter, CSVParser.DefaultEscapeCharacter,
                                   CSVParser.DefaultStrictQuotes, CSVParser.DefaultIgnoreLeadingWhiteSpace, CSVParser.DefaultIgnoreQuotations,
                                   CSVReaderNullFieldIndicator.EmptySeparators);

            String[] item = parser.ParseLine(new StringReader(", ,,\"\","));

            Assert.AreEqual(5, item.Length);
            Assert.IsNull(item[0]);
            Assert.AreEqual(" ", item[1]);
            Assert.IsNull(item[2]);
            Assert.AreEqual("", item[3]);
            Assert.IsNull(item[4]);
        }

        [Test]
        public void TreatEmptyDelimitedFieldsAsNull()
        {
            CSVParser parser = new CSVParser(CSVParser.DefaultSeparator, CSVParser.DefaultQuoteCharacter, CSVParser.DefaultEscapeCharacter,
                CSVParser.DefaultStrictQuotes, CSVParser.DefaultIgnoreLeadingWhiteSpace, CSVParser.DefaultIgnoreQuotations,
                CSVReaderNullFieldIndicator.EmptyQuotes);

            String[] item = parser.ParseLine(new StringReader(",\" \",,\"\","));

            Assert.AreEqual(5, item.Length);
            Assert.AreEqual("", item[0]);
            Assert.AreEqual(" ", item[1]);
            Assert.AreEqual("", item[2]);
            Assert.IsNull(item[3]);
            Assert.AreEqual("", item[4]);
        }

        [Test]
        public void TreatEmptyFieldsDelimitedOrNotAsNull() 
        {
            CSVParser parser = new CSVParser(CSVParser.DefaultSeparator, CSVParser.DefaultQuoteCharacter, CSVParser.DefaultEscapeCharacter,
                CSVParser.DefaultStrictQuotes, CSVParser.DefaultIgnoreLeadingWhiteSpace, CSVParser.DefaultIgnoreQuotations,
                CSVReaderNullFieldIndicator.Both);

            String[] item = parser.ParseLine(new StringReader(",,,\"\","));

            Assert.AreEqual(5, item.Length);
            Assert.IsNull(item[0]);
            Assert.IsNull(item[1]);
            Assert.IsNull(item[2]);
            Assert.IsNull(item[3]);
            Assert.IsNull(item[4]);
        }

        [Test]
        public void TestStrictQuotesEndsFieldAtQuote() 
        {
            CSVParser parser = new CSVParser(CSVParser.DefaultSeparator, CSVParser.DefaultQuoteCharacter, CSVParser.DefaultEscapeCharacter, true);

            String[] nextLine = parser.ParseLine(new StringReader("\"one\",\"t\"wo,\"three\""));

            Assert.AreEqual(3, nextLine.Length);

            Assert.AreEqual("one", nextLine[0]);
            Assert.AreEqual("t", nextLine[1]);
            Assert.AreEqual("three", nextLine[2]);
        }

        [Test]
        public void TestStrictQuotesEndsFieldAtQuoteWithEscapedQuoteInMiddle() 
        {
            CSVParser parser = new CSVParser(CSVParser.DefaultSeparator, CSVParser.DefaultQuoteCharacter, CSVParser.DefaultEscapeCharacter, true);

            String[] nextLine = parser.ParseLine(new StringReader("\"one\",\"t\"\"w\"o,\"three\""));

            Assert.AreEqual(3, nextLine.Length);

            Assert.AreEqual("one", nextLine[0]);
            Assert.AreEqual("t\"w", nextLine[1]);
            Assert.AreEqual("three", nextLine[2]);
        }

        [Test]
        public void TestNotStrictQuotesAllowsEmbeddedEscapedQuote()
        {
            CSVParser parser = new CSVParser(CSVParser.DefaultSeparator, CSVParser.DefaultQuoteCharacter, CSVParser.DefaultEscapeCharacter, false);

            String[] nextLine = parser.ParseLine(new StringReader("\"one\",\"t\"\"wo\",\"three\""));

            Assert.AreEqual(3, nextLine.Length);

            Assert.AreEqual("one", nextLine[0]);
            Assert.AreEqual("t\"wo", nextLine[1]);
            Assert.AreEqual("three", nextLine[2]);
        }

        [Test]
        public void TestNotStrictQuotesAllowsEmbeddedQuote() 
        {
            CSVParser parser = new CSVParser(CSVParser.DefaultSeparator, CSVParser.DefaultQuoteCharacter, CSVParser.DefaultEscapeCharacter, false);

            String[] nextLine = parser.ParseLine(new StringReader("\"one\",t\"\"wo,\"three\""));

            Assert.AreEqual(3, nextLine.Length);

            Assert.AreEqual("one", nextLine[0]);
            Assert.AreEqual("t\"wo", nextLine[1]);
            Assert.AreEqual("three", nextLine[2]);
        }

        [Test]
        public void ParsingEmptyDoubleQuoteField()
        {
            CSVParser parser = new CSVParser(CSVParser.DefaultSeparator, CSVParser.DefaultQuoteCharacter, CSVParser.DefaultEscapeCharacter, false);

            String[] nextLine = parser.ParseLine(new StringReader("\"\",2"));

            Assert.AreEqual(2, nextLine.Length);

            Assert.AreEqual("", nextLine[0]);
            Assert.AreEqual("2", nextLine[1]);
        }

        [Test]
        public void TestMultipleLines()
        {
            var reader = new StringReader("a,b,c\na,\"b\nc\",d");
            string[] items;
            items = csvParser.ParseLine(reader);
            Assert.AreEqual("a", items[0]);
            Assert.AreEqual("b", items[1]);
            Assert.AreEqual("c", items[2]);
            items = csvParser.ParseLine(reader);
            Assert.AreEqual("a", items[0]);
            Assert.AreEqual("b\nc", items[1]);
            Assert.AreEqual("d", items[2]);
        }
    }
}

