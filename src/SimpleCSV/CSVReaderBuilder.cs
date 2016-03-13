//
// CSVReaderBuilder.cs
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

namespace SimpleCSV
{
    /// <summary>
    /// Builder for creating a CSVReader.
    /// </summary>
    /// <example>
    /// <code>
    /// CSVParser parser = new CSVParserBuilder()
    ///     .WithSeparator('\t')
    ///     .WithIgnoreQuotations(true)
    ///     .Build();
    /// CSVReader reader = new CSVReaderBuilder(new StringReader(csv))
    ///     .WithSkipLines(1)
    ///     .WithCSVParser(parser)
    ///     .Build();
    /// </code>
    /// </example>
    /// <seealso cref="CSVReader"/>
    public class CSVReaderBuilder
    {
        private readonly CSVParserBuilder parserBuilder = new CSVParserBuilder();
        private readonly TextReader reader;
        private int skipLines = CSVReader.DefaultSkipLines;
        private CSVParser csvParser = null;
        private CSVReaderNullFieldIndicator nullFieldIndicator = CSVReaderNullFieldIndicator.Neither;

        /// <summary>
        /// Used by unit tests.
        /// </summary>
        /// <value>The reader.</value>
        public TextReader Reader { get { return reader; } }
        /// <summary>
        /// Used by unit tests.
        /// </summary>
        /// <value>The number of lines to skip.</value>
        public int SkipLines { get { return skipLines; } }
        /// <summary>
        /// Used by unit tests.
        /// </summary>
        /// <value>The csvParser.</value>
        public CSVParser CsvParser { get { return csvParser; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleCSV.CSVReaderBuilder"/> class
        /// with the reader to an underlying CSV source.
        /// </summary>
        /// <param name="reader">Reader.</param>
        public CSVReaderBuilder(TextReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader", "Reader may not be null");
            }
            this.reader = reader;
        }

        /// <summary>
        /// Sets the line number to skip for start reading.
        /// </summary>
        /// <returns>The CSVReaderBuilder with skipLines set.</returns>
        /// <param name="skipLines">The line number to skip for start reading.</param>
        public CSVReaderBuilder WithSkipLines(int skipLines)
        {
            this.skipLines = skipLines <= 0 ? 0 : skipLines;
            return this;
        }

        /// <summary>
        /// Sets the parser to use to parse the input.
        /// </summary>
        /// <returns>The CSVReaderBuilder with the CSVParser set.</returns>
        /// <param name="csvParser">Ther parser to use to parse the input.</param>
        public CSVReaderBuilder WithCSVParser(CSVParser csvParser)
        {
            this.csvParser = csvParser;
            return this;
        }

        /// <summary>
        /// Checks to see if it should treat an field with two separators, two quotes, or both as a null field.
        /// </summary>
        /// <returns>The CSVReaderBuilder based on this criteria.</returns>
        /// <param name="indicator">CSVReaderNullFieldIndicator set to what should be considered a null field.</param>
        public CSVReaderBuilder WithFieldAsNull(CSVReaderNullFieldIndicator indicator)
        {
            this.nullFieldIndicator = indicator;
            return this;
        }

        /// <summary>
        /// Create the CSVReader.
        /// </summary>
        /// <returns>The CSVReader based on the set criteria.</returns>
        public CSVReader Build()
        {
            CSVParser parser = (csvParser != null ? csvParser : 
                parserBuilder.WithFieldAsNull(nullFieldIndicator).Build());
            return new CSVReader(reader, skipLines, parser);
        }
    }
}
