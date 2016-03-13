//
// CSVReader.cs
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
using System.Collections.Generic;
using System.IO;

namespace SimpleCSV
{
    /// <summary>
    /// A simple CSV reader.
    /// </summary>
    public class CSVReader : IDisposable
    {
        /// <summary>
        /// The default skip lines to start reading.
        /// </summary>
        public static readonly int DefaultSkipLines = 0;

        private bool disposed = false;

        protected TextReader reader;
        protected bool linesSkiped = false;

        /// <summary>
        /// Gets the parser used by reader.
        /// </summary>
        /// <value>The parser.</value>
        public CSVParser Parser { get; private set; }

        /// <summary>
        /// Gets the number of lines in the csv file to skip before processing.
        /// </summary>
        /// <value>The number of lines.</value>
        public int SkipLines { get; private set; }

        /// <summary>
        /// Used for debugging purposes this property returns the number of lines that has been read from
        /// the reader passed into the CSVReader.
        /// </summary>
        /// <value>The lines read.</value>
        public long LinesRead { get { return Parser.LinesRead; } }

        /// <summary>
        /// Used for debugging purposes this method returns the number of records that has been read from
        /// the CSVReader.
        /// </summary>
        /// <value>The number of records (Array of string[]) read by the reader.</value>
        /// <remarks>
        /// Given the following data.
        /// <code><pre>
        /// First line in the file
        /// some other descriptive line
        /// a,b,c
        /// 
        /// a,"b\nb",c
        /// </pre></code>
        /// With a CSVReader constructed like so
        /// <code><pre>
        /// CSVReader c = new CSVReader(reader, ',', '"', 2);
        /// </pre></code>
        /// Initially, the value of RecordsRead property will be 0.<br/>
        /// After the first call to readNext() then RecordsRead will return 1.<br/>
        /// After the second call to read the blank line then RecordsRead will return 2
        /// (a blank line is considered a record with one empty field).<br/>
        /// After third call to readNext RecordsRead will return 3 because even though
        /// reads to retrieve this record it is still a single record read.<br/>
        /// Subsequent calls to readNext (since we are out of data) will not increment the number of records read.
        /// </remarks>
        public long RecordsRead { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleCSV.CSVReader"/> class using the comman for the separator.
        /// </summary>
        /// <param name="reader">The reader to an underlying CSV source.</param>
        public CSVReader(TextReader reader)
            : this(reader, CSVParser.DefaultSeparator, CSVParser.DefaultQuoteCharacter, CSVParser.DefaultEscapeCharacter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleCSV.CSVReader"/> class with supplied separator.
        /// </summary>
        /// <param name="reader">The reader to an underlying CSV source.</param>
        /// <param name="separator">The delimiter to use for separating entries.</param>
        public CSVReader(TextReader reader, char separator)
            : this(reader, separator, CSVParser.DefaultQuoteCharacter, CSVParser.DefaultEscapeCharacter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleCSV.CSVReader"/> class with supplied separator and quote char.
        /// </summary>
        /// <param name="reader">The reader to an underlying CSV source.</param>
        /// <param name="separator">The delimiter to use for separating entries.</param>
        /// <param name="quoteChar">The character to use for quoted elements.</param>
        public CSVReader(TextReader reader, char separator, char quoteChar)
            : this(reader, separator, quoteChar, CSVParser.DefaultEscapeCharacter, CSVReader.DefaultSkipLines, CSVParser.DefaultStrictQuotes)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleCSV.CSVReader"/> class with supplied separator, quote char and
        /// quote handling behavior.
        /// </summary>
        /// <param name="reader">The reader to an underlying CSV source.</param>
        /// <param name="separator">The delimiter to use for separating entries.</param>
        /// <param name="quoteChar">The character to use for quoted elements.</param>
        /// <param name="strictQuotes">Sets if characters outside the quotes are ignored.</param>
        public CSVReader(TextReader reader, char separator, char quoteChar, bool strictQuotes)
            : this(reader, separator, quoteChar, CSVParser.DefaultEscapeCharacter, CSVReader.DefaultSkipLines, strictQuotes)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleCSV.CSVReader"/> class.
        /// </summary>
        /// <param name="reader">The reader to an underlying CSV source.</param>
        /// <param name="separator">The delimiter to use for separating entries.</param>
        /// <param name="quoteChar">The character to use for quoted elements.</param>
        /// <param name="escapeChar">The character to use for escaping a separator or quote.</param>
        public CSVReader(TextReader reader, char separator, char quoteChar, char escapeChar)
            : this(reader, separator, quoteChar, escapeChar, CSVReader.DefaultSkipLines, CSVParser.DefaultStrictQuotes)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleCSV.CSVReader"/> class.
        /// </summary>
        /// <param name="reader">The reader to an underlying CSV source.</param>
        /// <param name="separator">The delimiter to use for separating entries.</param>
        /// <param name="quoteChar">The character to use for quoted elements.</param>
        /// <param name="skipLines">The line number to skip for start reading.</param>
        public CSVReader(TextReader reader, char separator, char quoteChar, int skipLines)
            : this(reader, separator, quoteChar, CSVParser.DefaultEscapeCharacter, skipLines, CSVParser.DefaultStrictQuotes)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleCSV.CSVReader"/> class.
        /// </summary>
        /// <param name="reader">The reader to an underlying CSV source.</param>
        /// <param name="separator">The delimiter to use for separating entries.</param>
        /// <param name="quoteChar">The character to use for quoted elements.</param>
        /// <param name="escapeChar">The character to use for escaping a separator or quote.</param>
        /// <param name="skipLines">The line number to skip for start reading.</param>
        public CSVReader(TextReader reader, char separator, char quoteChar, char escapeChar, int skipLines)
            : this(reader, separator, quoteChar, escapeChar, skipLines, CSVParser.DefaultStrictQuotes)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleCSV.CSVReader"/> class.
        /// </summary>
        /// <param name="reader">The reader to an underlying CSV source.</param>
        /// <param name="separator">The delimiter to use for separating entries.</param>
        /// <param name="quoteChar">The character to use for quoted elements.</param>
        /// <param name="escapeChar">The character to use for escaping a separator or quote.</param>
        /// <param name="skipLines">The line number to skip for start reading.</param>
        /// <param name="strictQuotes">Sets if characters outside the quotes are ignored.</param>
        public CSVReader(TextReader reader, char separator, char quoteChar, char escapeChar, int skipLines, bool strictQuotes)
            : this(reader, separator, quoteChar, escapeChar, skipLines, strictQuotes, CSVParser.DefaultIgnoreLeadingWhiteSpace)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleCSV.CSVReader"/> class with all data entered.
        /// </summary>
        /// <param name="reader">The reader to an underlying CSV source.</param>
        /// <param name="separator">The delimiter to use for separating entries.</param>
        /// <param name="quoteChar">The character to use for quoted elements.</param>
        /// <param name="escapeChar">The character to use for escaping a separator or quote.</param>
        /// <param name="skipLines">The line number to skip for start reading.</param>
        /// <param name="strictQuotes">Sets if characters outside the quotes are ignored.</param>
        /// <param name="ignoreLeadingWhiteSpace">If <c>true</c>, parser should ignore white space before a quote in a field.</param>
        public CSVReader(TextReader reader, char separator, char quoteChar, char escapeChar, int skipLines, bool strictQuotes,
            bool ignoreLeadingWhiteSpace)
            : this(reader, skipLines, new CSVParser(separator, quoteChar, escapeChar, strictQuotes, ignoreLeadingWhiteSpace))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleCSV.CSVReader"/> class with supplied CSVParser.
        /// </summary>
        /// <param name="reader">The reader to an underlying CSV source.</param>
        /// <param name="skipLines">The line number to skip for start reading.</param>
        /// <param name="parser">The parser to use to parse input.</param>
        public CSVReader(TextReader reader, int skipLines, CSVParser parser)
        {
            this.reader = reader;
            SkipLines = skipLines;
            Parser = parser;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="SimpleCSV.CSVReader"/> is reclaimed by garbage collection.
        /// </summary>
        ~CSVReader()
        {
            Dispose(false);
        }

        /// <summary>
        /// Reads the entire file into a List with each element being a string[] of tokens.
        /// </summary>
        /// <returns>A List of String[], with each String[] representing a line of the file.</returns>
        /// <exception cref="IOException">If bad things happen during the read.</exception>
        public IList<string[]> ReadAll()
        {
            var allElements = new List<string[]>();
            string[] nextLine;

            while ((nextLine = ReadNext()) != null)
            {
                allElements.Add(nextLine);
            }

            return allElements;
        }

        /// <summary>
        /// Reads the next record (a string array) from the reader.
        /// </summary>
        /// <returns>A string array with each comma-separated element as a separate.</returns>
        /// <exception cref="IOException">If bad things happen during the read.</exception>
        public string[] ReadNext()
        {
            if (disposed)
            {
                return null;
            }

            if (!linesSkiped)
            {
                for (int i = 0; i < SkipLines; i++)
                {
                    reader.ReadLine();
                    Parser.LinesRead++;
                }
                linesSkiped = true;
            }

            string[] record = Parser.ParseLine(reader);
            if (record != null)
            {
                RecordsRead++;
            }

            return record;
        }

        /// <summary>
        /// Closes the underlying reader.
        /// </summary>
        public void Close()
        {
            reader.Close();
        }

        /// <summary>
        /// Releases all resource used by the <see cref="SimpleCSV.CSVReader"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="SimpleCSV.CSVReader"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="SimpleCSV.CSVReader"/> in an unusable state. After
        /// calling <see cref="Dispose"/>, you must release all references to the <see cref="SimpleCSV.CSVReader"/> so
        /// the garbage collector can reclaim the memory that the <see cref="SimpleCSV.CSVReader"/> was occupying.</remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the specified disposing.
        /// </summary>
        /// <param name="disposing">If set to <c>true</c> disposing.</param>
        protected void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                if (reader != null)
                {
                    reader.Dispose();
                    reader = null;
                }
            }

            disposed = true;
        }
    }
}

