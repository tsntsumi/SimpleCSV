//
// CSVPaserBuilder.cs
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

namespace SimpleCSV
{
    /// <summary>
    /// Builder for creating a CSVParser.
    /// </summary>
    /// <example>
    /// <code>
    /// CSVParser parser = new CSVParserBuilder()
    ///     .withSeparator('\t')
    ///     .withIgnoreQuotations(true)
    ///     .build();
    /// </code>
    /// </example>
    /// <seealso cref="CSVParser"/>
    public class CSVParserBuilder
    {
        private char separator = CSVParser.DefaultSeparator;
        private char quoteChar = CSVParser.DefaultQuoteCharacter;
        private char escapeChar = CSVParser.DefaultEscapeCharacter;
        private bool strictQuotes = CSVParser.DefaultStrictQuotes;
        private bool ignoreLeadingWhiteSpace = CSVParser.DefaultIgnoreLeadingWhiteSpace;
        private bool ignoreQuotations = CSVParser.DefaultIgnoreQuotations;
        private CSVReaderNullFieldIndicator nullFieldIndicator = CSVReaderNullFieldIndicator.Neither;

        /// <summary>
        /// Gets the separator.
        /// </summary>
        public char Separator { get { return separator; } }
        /// <summary>
        /// Gets the quoteChar.
        /// </summary>
        public char QuoteChar { get { return quoteChar; } }
        /// <summary>
        /// Gets the escapeChar.
        /// </summary>
        public char EscapeChar { get { return escapeChar; } }
        /// <summary>
        /// Gets a value of strictQuotes.
        /// </summary>
        public bool IsStrictQuotes { get { return strictQuotes; } }
        /// <summary>
        /// Gets a value of ignoreLeadingWhiteSpace.
        /// </summary>
        public bool IsIgnoreLeadingWhiteSpace { get { return ignoreLeadingWhiteSpace; } }
        /// <summary>
        /// Gets a value of ignoreQuotations.
        /// </summary>
        public bool IsIgnoreQuotations { get { return ignoreQuotations; } }
        /// <summary>
        /// Gets the null field indicator.
        /// </summary>
        public CSVReaderNullFieldIndicator NullFieldIndicator { get { return nullFieldIndicator; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleCSV.CSVPaserBuilder"/> class.
        /// </summary>
        public CSVParserBuilder()
        {
        }

        /// <summary>
        /// Sets the delimiter to use for separating entries.
        /// </summary>
        /// <returns>The CSVParserBuilder.</returns>
        /// <param name="separator">The delimiter to use for separating entries.</param>
        public CSVParserBuilder WithSeparator(char separator)
        {
            this.separator = separator;
            return this;
        }

        /// <summary>
        /// Sets the character to use for quoted elements.
        /// </summary>
        /// <returns>The CSVParserBuilder.</returns>
        /// <param name="quoteChar">The character to use for quoted element.</param>
        public CSVParserBuilder WithQuoteChar(char quoteChar)
        {
            this.quoteChar = quoteChar;
            return this;
        }

        /// <summary>
        /// Sets the character to use for escaping a separator or quote.
        /// </summary>
        /// <returns>The CSVParserBuilder.</returns>
        /// <param name="escapeChar">The character to use for escaping a separator or quote.</param>
        public CSVParserBuilder WithEscapeChar(char escapeChar)
        {
            this.escapeChar = escapeChar;
            return this;
        }

        /// <summary>
        /// Sets the strict quotes setting - if true
        /// </summary>
        /// <returns>The CSVParserBuilder.</returns>
        /// <param name="strictQuotes">If <c>true</c>, characters outside the quotes are ignored.</param>
        public CSVParserBuilder WithStrictQuotes(bool strictQuotes) {
            this.strictQuotes = strictQuotes;
            return this;
        }

        /// <summary>
        /// Sets the ignore leading whitespace setting - if true, white space
        /// in front of a quote in a field is ignored.
        /// </summary>
        /// <returns>The CSVParserBuilder.</returns>
        /// <param name="ignoreLeadingWhiteSpace">If <c>true</c>, white space in front of a quote in a field is ignored</param>
        public CSVParserBuilder WithIgnoreLeadingWhiteSpace(bool ignoreLeadingWhiteSpace)
        {
            this.ignoreLeadingWhiteSpace = ignoreLeadingWhiteSpace;
            return this;
        }

        /// <summary>
        /// Sets the ignore quotations mode - if true, quotations are ignored.
        /// </summary>
        /// <returns>The CSVParserBuilder</returns>
        /// <param name="ignoreQuotations">If <c>true</c>, quotations are ignored.</param>
        public CSVParserBuilder WithIgnoreQuotations(bool ignoreQuotations)
        {
            this.ignoreQuotations = ignoreQuotations;
            return this;
        }

        /// <summary>
        /// Sets the NullFieldIndicator.
        /// </summary>
        /// <returns>The CSVParserBuilder.</returns>
        /// <param name="fieldIndicator">CSVReaderNullFieldIndicator set to what should be considered a null field.</param>
        public CSVParserBuilder WithFieldAsNull(CSVReaderNullFieldIndicator fieldIndicator)
        {
            this.nullFieldIndicator = fieldIndicator;
            return this;
        }
        
        /// <summary>
        /// Constructs CSVParser.
        /// </summary>
        /// <returns>A new CSVParser with defined settings.</returns>
        public CSVParser Build()
        {
            return new CSVParser(
                separator,
                quoteChar,
                escapeChar,
                strictQuotes,
                ignoreLeadingWhiteSpace,
                ignoreQuotations,
                nullFieldIndicator);
        }
    }
}

