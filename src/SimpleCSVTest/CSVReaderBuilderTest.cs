//
// CSVReaderBuilderTest.cs
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
    public class CSVReaderBuilderTest
    {
        private CSVReaderBuilder builder;
        private TextReader reader;

        [SetUp]
        public void SetUp()
        {
            reader = new StringReader("");
            builder = new CSVReaderBuilder(reader);
        }

        [Test]
        public void TestDefaultBuilder()
        {
            Assert.AreSame(reader, builder.Reader);
            Assert.Null(builder.CsvParser);
            Assert.AreEqual(CSVReader.DefaultSkipLines, builder.SkipLines);

            CSVReader csvReader = builder.Build();
            Assert.AreEqual(CSVReader.DefaultSkipLines, csvReader.SkipLines);
        }

        [TestCase(ExpectedException = typeof(ArgumentNullException))]
        public void TestNullReader()
        {
            builder = new CSVReaderBuilder(null);
        }

        [Test]
        public void TestWithCSVParserNull()
        {
            builder.WithCSVParser(new CSVParser());
            builder.WithCSVParser(null);
            Assert.Null(builder.CsvParser);
        }

        [Test]
        public void TestWithCSVParser()
        {
            CSVParser csvParser = new CSVParser();
            builder.WithCSVParser(csvParser);
            Assert.AreSame(csvParser, builder.CsvParser);

            CSVReader actual = builder.Build();
            Assert.AreSame(csvParser, actual.Parser);
        }

        [Test]
        public void TestWithSkipLines()
        {
            builder.WithSkipLines(99);

            Assert.AreEqual(99, builder.SkipLines);

            CSVReader actual = builder.Build();
            Assert.AreEqual(99, actual.SkipLines);
        }

        [Test]
        public void TestWithSkipLinesZero()
        {
            builder.WithSkipLines(0);

            Assert.AreEqual(0, builder.SkipLines);

            CSVReader actual = builder.Build();
            Assert.AreEqual(0, actual.SkipLines);
        }

        [Test]
        public void TestWithSkipLinesNegative()
        {
            builder.WithSkipLines(-1);

            Assert.AreEqual(0, builder.SkipLines);

            CSVReader actual = builder.Build();
            Assert.AreEqual(0, actual.SkipLines);
        }

        [Test]
        public void TestWithNullFieldIndicator()
        {
            CSVReader reader = builder.WithFieldAsNull(CSVReaderNullFieldIndicator.EmptySeparators).Build();

            Assert.AreEqual(CSVReaderNullFieldIndicator.EmptySeparators, reader.Parser.NullFieldIndicator);
        }
    }
}

