//
// CSVReaderNullFieldIndicator.cs
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
using System.Text;

namespace SimpleCSV
{
    /// <summary>
    /// A simple CSV parser.
    /// This just implements splitting a single line into fields.
    /// </summary>
    public class CSVParser
    {
        /// <summary>
        /// The default separator to use if none is supplied to the constructor.
        /// </summary>
        public static readonly char DefaultSeparator = ',';
        /// <summary>
        /// The default quote character to use if none is supplied to the constructor.
        /// </summary>
        public static readonly char DefaultQuoteCharacter = '"';
        /// <summary>
        /// The default escape character to use if none is supplied to the constructor.
        /// </summary>
        public static readonly char DefaultEscapeCharacter = '\\';
        /// <summary>
        /// The default strict quotes behavior to use if none is supplied to the constructor.
        /// </summary>
        public static readonly bool DefaultStrictQuotes = false;
        /// <summary>
        /// The default ignore leading whitespace behavior to use if none is supplied to the constructor.
        /// </summary>
        public static readonly bool DefaultIgnoreLeadingWhiteSpace = true;
        /// <summary>
        /// The default ignore quotations behavior to use if none is supplied to the constructor.
        /// </summary>
        public static readonly bool DefaultIgnoreQuotations = false;
        /// <summary>
        /// The "null" character - if a value is set to this then it is ignored.
        /// </summary>
        public static readonly char NullCharacter = '\0';
        /// <summary>
        /// The default null field indicator to use if none is supplied to the constructor.
        /// </summary>
        public static readonly CSVReaderNullFieldIndicator DefaultNullFieldIndicator = CSVReaderNullFieldIndicator.Neither;
        /// <summary>
        /// In most cases we know the size of the line we want to read. 
        /// </summary>
        public static readonly int ReadBufferSize = 128;

        /// <summary>
        /// This is the character that the CSVParser will treat as the separator.
        /// </summary>
        public char Separator { get; private set; }
        /// <summary>
        /// This is the character that the CSVParser will treat as the quotation character.
        /// </summary>
        public char QuoteChar { get; private set; }
        /// <summary>
        /// This is the character that the CSVParser will treat as the escape character.
        /// </summary>
        public char EscapeChar { get; private set; }
        /// <summary>
        /// Determines if the field is between quotes (true) or between separators (false).
        /// </summary>
        public bool IsStrictQuotes { get; private set; }
        /// <summary>
        /// Ignore any leading white space at the start of the field.
        /// </summary>
        public bool IsIgnoreLeadingWhiteSpace { get; private set; }
        /// <summary>
        /// Skip over quotation characters when parsing.
        /// </summary>
        public bool IsIgnoreQuotations { get; private set; }
        /// <summary>
        /// Denotes what field contents will cause the parser to return null: EmptySeparators, EmptyQuotes, Both, Neither (default).
        /// </summary>
        public CSVReaderNullFieldIndicator NullFieldIndicator { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleCSV.CSVParser"/> class using comma for separator.
        /// </summary>
        public CSVParser()
            : this(DefaultSeparator, DefaultQuoteCharacter, DefaultEscapeCharacter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleCSV.CSVParser"/> class with supplied separator.
        /// </summary>
        /// <param name="separator">The delimiter to use for separating entries.</param>
        public CSVParser(char separator)
            : this(separator, DefaultQuoteCharacter, DefaultEscapeCharacter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleCSV.CSVParser"/> class with supplied separator and quote char.
        /// </summary>
        /// <param name="separator">The delimiter to use for separating entries.</param>
        /// <param name="quoteChar">The character to use for quoted elements.</param>
        public CSVParser(char separator, char quoteChar)
            : this(separator, quoteChar, DefaultEscapeCharacter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleCSV.CSVParser"/> class with supplied separator, quote char and escape char.
        /// </summary>
        /// <param name="separator">The delimiter to use for separating entries.</param>
        /// <param name="quoteChar">The character to use for quoted elements.</param>
        /// <param name="escapeChar">The character to use for escaping a separator or quote.</param>
        public CSVParser(char separator, char quoteChar, char escapeChar)
            : this(separator, quoteChar, escapeChar, DefaultStrictQuotes)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleCSV.CSVParser"/> class with supplied separator, quote char and escape char.
        /// Allows setting the "strict quotes" flag.
        /// </summary>
        /// <param name="separator">The delimiter to use for separating entries.</param>
        /// <param name="quoteChar">The character to use for quoted elements.</param>
        /// <param name="escapeChar">The character to use for escaping a separator or quote.</param>
        /// <param name="strictQuotes">If set to <c>true</c> strict quotes.</param>
        public CSVParser(char separator, char quoteChar, char escapeChar, bool strictQuotes)
            : this(separator, quoteChar, escapeChar, strictQuotes, DefaultIgnoreLeadingWhiteSpace)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleCSV.CSVParser"/> class with supplied separator, quote char and escape char.
        /// Allows setting the "strict quotes" and "ignore leading whitespace" flags.
        /// </summary>
        /// <param name="separator">The delimiter to use for separating entries.</param>
        /// <param name="quoteChar">The character to use for quoted elements.</param>
        /// <param name="escapeChar">The character to use for escaping a separator or quote.</param>
        /// <param name="strictQuotes">If <c>true</c>, characters outside the quotes are ignored.</param>
        /// <param name="ignoreLeadingWhiteSpace">If true, white space in front of a quote in a field is ignored.</param>
        public CSVParser(char separator, char quoteChar, char escapeChar, bool strictQuotes, bool ignoreLeadingWhiteSpace)
            : this(separator, quoteChar, escapeChar, strictQuotes, ignoreLeadingWhiteSpace, DefaultIgnoreQuotations)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleCSV.CSVParser"/> class with supplied separator, quote char and escape char.
        /// Allows setting the "strict quotes", "ignore leading whitespace" and "ignore quotations" flags.
        /// </summary>
        /// <param name="separator">The delimiter to use for separating entries.</param>
        /// <param name="quoteChar">The character to use for quoted elements.</param>
        /// <param name="escapeChar">The character to use for escaping a separator or quote.</param>
        /// <param name="strictQuotes">If <c>true</c>, characters outside the quotes are ignored.</param>
        /// <param name="ignoreLeadingWhiteSpace">If true, white space in front of a quote in a field is ignored.</param>
        /// <param name="ignoreQuotations">If set to <c>true</c> ignore quotations.</param>
        public CSVParser(char separator, char quoteChar, char escapeChar, bool strictQuotes, bool ignoreLeadingWhiteSpace, bool ignoreQuotations)
            : this(separator, quoteChar, escapeChar, strictQuotes, ignoreLeadingWhiteSpace, ignoreQuotations, DefaultNullFieldIndicator)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleCSV.CSVParser"/> class with supplied separator, quote char and escape char.
        /// Allows setting the "strict quotes", "ignore leadig whitespace", "ignore quotations" flags and null field indicator.
        /// </summary>
        /// <param name="separator">The delimiter to use for separating entries.</param>
        /// <param name="quoteChar">The character to use for quoted elements.</param>
        /// <param name="escapeChar">The character to use for escaping a separator or quote.</param>
        /// <param name="strictQuotes">If <c>true</c>, characters outside the quotes are ignored.</param>
        /// <param name="ignoreLeadingWhiteSpace">If <c>true</c>, white space in front of a quote in a field is ignored.</param>
        /// <param name="ignoreQuotations">If set to <c>true</c>, treat quotations like any other character.</param>
        /// <param name="nullFieldIndicator">which field content will be returned as null: 
        ///                                     <c>EmptySeparators</c>, <c>EmptyQuotes</c>, <c>Both</c>, <c>Neither</c> (default).</param>
        public CSVParser(char separator, char quoteChar, char escapeChar, bool strictQuotes,
            bool ignoreLeadingWhiteSpace,
            bool ignoreQuotations,
            CSVReaderNullFieldIndicator nullFieldIndicator)
        {
            if (AnyCharactersAreTheSame(separator, quoteChar, escapeChar)) {
                throw new ArgumentException("The separator, quote, and escape characters must be different!");
            }
            if (separator == NullCharacter) {
                throw new ArgumentException("The separator character must be defined!");
            }
            this.Separator = separator;
            this.QuoteChar = quoteChar;
            this.EscapeChar = escapeChar;
            this.IsStrictQuotes = strictQuotes;
            this.IsIgnoreLeadingWhiteSpace = ignoreLeadingWhiteSpace;
            this.IsIgnoreQuotations = ignoreQuotations;
            this.NullFieldIndicator = nullFieldIndicator;
        }

        /// <summary>
        /// Checks to see if any two of the three characters are the same.
        /// </summary>
        /// <returns><c>true</c>, if any two of the three are the same, <c>false</c> otherwise.</returns>
        /// <param name="separator">Separator.</param>
        /// <param name="quoteChar">Quotation character.</param>
        /// <param name="escapeChar">Escape character.</param>
        private bool AnyCharactersAreTheSame(char separator, char quoteChar, char escapeChar)
        {
            return IsSameCharacter(separator, quoteChar) || IsSameCharacter(separator, escapeChar) || IsSameCharacter(quoteChar, escapeChar);
        }

        /// <summary>
        /// Checks that the two characters are the same and are not the defined NullCharacter.
        /// </summary>
        /// <returns><c>true</c> if both characters are the same and are not the defined NullCharacter; otherwise, <c>false</c>.</returns>
        /// <param name="c1">First character.</param>
        /// <param name="c2">Second character.</param>
        private bool IsSameCharacter(char c1, char c2) {
            return c1 != NullCharacter && c1 == c2;
        }

        /// <summary>
        /// Parses an incoming line and returns an array of fields.
        /// </summary>
        /// <returns>The comma separated list of fields, or null if line is null.</returns>
        /// <param name="reader">The reader to parse.</param>
        /// <exception cref="IOException">If bad things happen during the read.</exception>
        public string[] ParseLine(TextReader reader)
        {
            int nextChar;

            if (reader == null || (nextChar = Advance(reader)) < 0)
            {
                return null;
            }

            var fields = new List<string>();

            for (;;)
            {
                string field;

                if (IsStrictQuotes)
                {
                    nextChar = SkipGarbage(nextChar, reader);
                }
                if (nextChar == QuoteChar)
                {
                    nextChar = ParseQuotedField(reader, true, out field);
                }
                else
                {
                    nextChar = ParseField(nextChar, reader, out field);
                }
                fields.Add(field);
                if (nextChar == '\n')
                {
                    nextChar = Advance(reader);
                    break;
                }
                if (nextChar < 0)
                {
                    break;
                }
                nextChar = Advance(reader);
            }

            return fields.ToArray();
        }

        /// <summary>
        /// Skip gabage.
        /// </summary>
        /// <returns>The next character.</returns>
        /// <param name="nextChar">Next char.</param>
        /// <param name="reader">Reader to skip.</param>
        protected int SkipGarbage(int nextChar, TextReader reader)
        {
            while (nextChar >= 0)
            {
                if (nextChar == '\n' || nextChar == QuoteChar || nextChar == Separator)
                {
                    break;
                }
                nextChar = Advance(reader);
            }
            return nextChar;
        }

        protected int SkipWhiteSpaces(int nextChar, TextReader reader)
        {
            while (nextChar >= 0)
            {
                if (!Char.IsWhiteSpace((char)nextChar))
                {
                    break;
                }
                nextChar = Advance(reader);
            }
            return nextChar;
        }

        /// <summary>
        /// Parses the quoted field.
        /// </summary>
        /// <returns>The next character.</returns>
        /// <param name="reader">Reader to parse.</param>
        /// <param name="multi">Enable multi-line.</param>
        /// <param name="field">read field.</param>
        protected int ParseQuotedField(TextReader reader, bool multi, out string field)
        {
            var sb = new StringBuilder(ReadBufferSize);
            int nextChar = Advance(reader);

            for (;;)
            {
                if (nextChar < 0)
                {
                    throw new IOException("Un-terminated quoted field at end of CSV line");
                }
                if (nextChar == '\n')
                {
                    if (!multi)
                    {
                        throw new IOException("Un-terminated quoted field at end of CSV line");
                    }
                    // do nothing
                }
                else if (nextChar == EscapeChar)
                {
                    nextChar = Advance(reader);
                }
                else if (nextChar != QuoteChar)
                {
                    // do nothing
                }
                else if (reader.Peek() == QuoteChar) // && nextChar == QuoteChar  // quote char escapes quote char.
                {
                    nextChar = reader.Read();
                }
                else if (IsStrictQuotes)    // && nextChar == QuoteChar
                {
                    nextChar = SkipTrailingCharacters(reader);
                    break;
                }
                else
                {
                    var trailings = new StringBuilder();
                    nextChar = CollectTrailingCharacters(nextChar, reader, multi, trailings);
                    if (trailings.Length > 0)
                    {
                        sb.Append(trailings.ToString());
                    }
                    break;
                }
                sb.Append((char)nextChar);
                nextChar = reader.Read();
            }

            field = sb.ToString();
            if (field == "" &&
                (NullFieldIndicator == CSVReaderNullFieldIndicator.Both ||
                 NullFieldIndicator == CSVReaderNullFieldIndicator.EmptyQuotes))
            {
                field = null;
            }

            return nextChar;
        }

        /// <summary>
        /// Parses the field.
        /// </summary>
        /// <returns>The next character.</returns>
        /// <param name="nextChar">Next char.</param>
        /// <param name="reader">Reader.</param>
        /// <param name="field">read field.</param>
        protected int ParseField(int nextChar, TextReader reader, out string field)
        {
            StringBuilder sb = new StringBuilder(ReadBufferSize);

            while (Char.IsWhiteSpace((char)nextChar))
            {
                sb.Append((char)nextChar);
                nextChar = reader.Read();
            }

            if (nextChar == QuoteChar && IsIgnoreLeadingWhiteSpace)
            {
                return ParseQuotedField(reader, true, out field);
            }

            while (nextChar >= 0)
            {
                if (nextChar == '\n' || nextChar == Separator)
                {
                    break;
                }
                if (nextChar == EscapeChar)
                {
                    nextChar = Advance(reader);
                }
                else if (nextChar != QuoteChar)
                {
                    // do nothing
                }
                else if (reader.Peek() == QuoteChar)
                {
                    nextChar = Advance(reader);
                }
                else if (IsIgnoreQuotations)
                {
                    nextChar = Advance(reader);
                    if (nextChar == Separator || nextChar == '\n' || nextChar < 0)
                    {
                        break;
                    }
                    sb.Append((char)QuoteChar);
                }
                else
                {
                    sb.Append((char)nextChar);
                    string quoted;
                    nextChar = ParseQuotedField(reader, false, out quoted);
                    sb.Append(quoted);
                    break;
                }
                sb.Append((char)nextChar);
                nextChar = Advance(reader);
            }

            field = sb.ToString();

            if (field == "" &&
                (NullFieldIndicator == CSVReaderNullFieldIndicator.Both ||
                 NullFieldIndicator == CSVReaderNullFieldIndicator.EmptySeparators))
            {
                field = null;
            }

            return nextChar;
        }

        /// <summary>
        /// Advance the specified reader.
        /// </summary>
        /// <returns>The next character.</returns>
        /// <param name="reader">Reader.</param>
        protected int Advance(TextReader reader)
        {
            int nextChar = reader.Read();
            if (nextChar == '\r' && reader.Peek() == '\n')
            {
                nextChar = reader.Read();
            }
            return nextChar;
        }

        /// <summary>
        /// Skips the trailing characters.
        /// </summary>
        /// <returns>The next character.</returns>
        /// <param name="reader">Reader.</param>
        protected int SkipTrailingCharacters(TextReader reader)
        {
            int nextChar = Advance(reader);
            while (nextChar >= 0 && nextChar != '\n' && nextChar != Separator)
            {
                nextChar = Advance(reader);
            }
            return nextChar;
        }

        /// <summary>
        /// Collects the trailing characters.
        /// </summary>
        /// <returns>The next character.</returns>
        /// <param name="reader">Reader.</param>
        /// <param name="trailings">Trailings.</param>
        protected int CollectTrailingCharacters(int nextChar, TextReader reader, bool multi, StringBuilder trailings)
        {
            while (nextChar >= 0)
            {
                if (nextChar == QuoteChar)
                {
                    nextChar = Advance(reader);
                    if (nextChar == Separator || nextChar == '\n' || nextChar < 0)
                    {
                        break;
                    }
                    trailings.Append((char)QuoteChar);
                }
                else if (nextChar == EscapeChar)
                {
                    nextChar = Advance(reader);
                }
                else if (nextChar == Separator)
                {
                    break;
                }
                else if (nextChar == '\n' && !multi)
                {
                    break;
                }
                trailings.Append((char)nextChar);
                nextChar = Advance(reader);
            }
            return nextChar;
        }
    }
}

