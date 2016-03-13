//
// CSVParserBuilderTest.cs
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
using NUnit.Framework;
using SimpleCSV;

namespace SimpleCSVTest
{
    public class CSVParserBuilderTest
    {
        private CSVParserBuilder builder;

        [SetUp]
        public void SetUp()
        {
            builder = new CSVParserBuilder();
        }

        [Test]
        public void TestDefaultBuilder()
        {
            Assert.AreEqual(CSVParser.DefaultSeparator, builder.Separator);
            Assert.AreEqual(CSVParser.DefaultQuoteCharacter, builder.QuoteChar);
            Assert.AreEqual(CSVParser.DefaultEscapeCharacter, builder.EscapeChar);
            Assert.AreEqual(CSVParser.DefaultStrictQuotes, builder.IsStrictQuotes);
            Assert.AreEqual(CSVParser.DefaultIgnoreLeadingWhiteSpace, builder.IsIgnoreLeadingWhiteSpace);
            Assert.AreEqual(CSVParser.DefaultIgnoreQuotations, builder.IsIgnoreQuotations);
            Assert.AreEqual(CSVReaderNullFieldIndicator.Neither, builder.NullFieldIndicator);

            CSVParser parser = builder.Build();
            Assert.AreEqual(CSVParser.DefaultSeparator, parser.Separator);
            Assert.AreEqual(CSVParser.DefaultQuoteCharacter, parser.QuoteChar);
            Assert.AreEqual(CSVParser.DefaultEscapeCharacter, parser.EscapeChar);
            Assert.AreEqual(CSVParser.DefaultStrictQuotes, parser.IsStrictQuotes);
            Assert.AreEqual(CSVParser.DefaultIgnoreLeadingWhiteSpace, parser.IsIgnoreLeadingWhiteSpace);
            Assert.AreEqual(CSVParser.DefaultIgnoreQuotations, parser.IsIgnoreQuotations);
            Assert.AreEqual(CSVReaderNullFieldIndicator.Neither, parser.NullFieldIndicator);
        }

        [Test]
        public void TestWithSeparator()
        {
            char expected = '1';
            builder.WithSeparator(expected);
            Assert.AreEqual(expected, builder.Separator);
            Assert.AreEqual(expected, builder.Build().Separator);
        }

        [Test]
        public void TestWithQuoteChar()
        {
            char expected = '2';
            builder.WithQuoteChar(expected);
            Assert.AreEqual(expected, builder.QuoteChar);
            Assert.AreEqual(expected, builder.Build().QuoteChar);
        }

        [Test]
        public void TestWithEscapeChar()
        {
            char expected = '3';
            builder.WithEscapeChar(expected);
            Assert.AreEqual(expected, builder.EscapeChar);
            Assert.AreEqual(expected, builder.Build().EscapeChar);
        }

        [Test]
        public void TestWithStrictQuotes()
        {
            bool expected = true;
            builder.WithStrictQuotes(expected);
            Assert.AreEqual(expected, builder.IsStrictQuotes);
            Assert.AreEqual(expected, builder.Build().IsStrictQuotes);
        }

        [Test]
        public void TestWithIgnoreLeadingWhiteSpace()
        {
            bool expected = true;
            builder.WithIgnoreLeadingWhiteSpace(expected);
            Assert.AreEqual(expected, builder.IsIgnoreLeadingWhiteSpace);
            Assert.AreEqual(expected, builder.Build().IsIgnoreLeadingWhiteSpace);
        }

        [Test]
        public void TestWithIgnoreQuotations()
        {
            bool expected = true;
            builder.WithIgnoreQuotations(expected);
            Assert.AreEqual(expected, builder.IsIgnoreQuotations);
            Assert.AreEqual(expected, builder.Build().IsIgnoreQuotations);
        }
    }
}

