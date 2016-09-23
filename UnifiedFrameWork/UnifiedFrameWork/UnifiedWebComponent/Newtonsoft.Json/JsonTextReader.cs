using Newtonsoft.Json.Utilities;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Numerics;

namespace Newtonsoft.Json
{
	internal class JsonTextReader : JsonReader, IJsonLineInfo
	{
		private const char UnicodeReplacementChar = '�';

		private readonly TextReader _reader;

		private char[] _chars;

		private int _charsUsed;

		private int _charPos;

		private int _lineStartPos;

		private int _lineNumber;

		private bool _isEndOfFile;

		private StringBuffer _buffer;

		private StringReference _stringReference;

		public int LineNumber
		{
			get
			{
				if (base.CurrentState == JsonReader.State.Start && this.LinePosition == 0)
				{
					return 0;
				}
				return this._lineNumber;
			}
		}

		public int LinePosition
		{
			get
			{
				return this._charPos - this._lineStartPos;
			}
		}

		public JsonTextReader(TextReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			this._reader = reader;
			this._lineNumber = 1;
			this._chars = new char[1025];
		}

		private StringBuffer GetBuffer()
		{
			if (this._buffer == null)
			{
				this._buffer = new StringBuffer(1025);
			}
			else
			{
				this._buffer.Position = 0;
			}
			return this._buffer;
		}

		private void OnNewLine(int pos)
		{
			this._lineNumber++;
			this._lineStartPos = pos - 1;
		}

		private void ParseString(char quote)
		{
			this._charPos++;
			this.ShiftBufferIfNeeded();
			this.ReadStringIntoBuffer(quote);
			if (this._readType == ReadType.ReadAsBytes)
			{
				byte[] value;
				if (this._stringReference.Length == 0)
				{
					value = new byte[0];
				}
				else
				{
					value = Convert.FromBase64CharArray(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length);
				}
				base.SetToken(JsonToken.Bytes, value);
				return;
			}
			if (this._readType == ReadType.ReadAsString)
			{
				string value2 = this._stringReference.ToString();
				base.SetToken(JsonToken.String, value2);
				this._quoteChar = quote;
				return;
			}
			string text = this._stringReference.ToString();
			if (this._dateParseHandling != DateParseHandling.None)
			{
				DateParseHandling dateParseHandling;
				if (this._readType == ReadType.ReadAsDateTime)
				{
					dateParseHandling = DateParseHandling.DateTime;
				}
				else if (this._readType == ReadType.ReadAsDateTimeOffset)
				{
					dateParseHandling = DateParseHandling.DateTimeOffset;
				}
				else
				{
					dateParseHandling = this._dateParseHandling;
				}
				object value3;
				if (DateTimeUtils.TryParseDateTime(text, dateParseHandling, base.DateTimeZoneHandling, out value3))
				{
					base.SetToken(JsonToken.Date, value3);
					return;
				}
			}
			base.SetToken(JsonToken.String, text);
			this._quoteChar = quote;
		}

		private static void BlockCopyChars(char[] src, int srcOffset, char[] dst, int dstOffset, int count)
		{
			Buffer.BlockCopy(src, srcOffset * 2, dst, dstOffset * 2, count * 2);
		}

		private void ShiftBufferIfNeeded()
		{
			int num = this._chars.Length;
			if ((double)(num - this._charPos) <= (double)num * 0.1)
			{
				int num2 = this._charsUsed - this._charPos;
				if (num2 > 0)
				{
					JsonTextReader.BlockCopyChars(this._chars, this._charPos, this._chars, 0, num2);
				}
				this._lineStartPos -= this._charPos;
				this._charPos = 0;
				this._charsUsed = num2;
				this._chars[this._charsUsed] = '\0';
			}
		}

		private int ReadData(bool append)
		{
			return this.ReadData(append, 0);
		}

		private int ReadData(bool append, int charsRequired)
		{
			if (this._isEndOfFile)
			{
				return 0;
			}
			if (this._charsUsed + charsRequired >= this._chars.Length - 1)
			{
				if (append)
				{
					int num = Math.Max(this._chars.Length * 2, this._charsUsed + charsRequired + 1);
					char[] array = new char[num];
					JsonTextReader.BlockCopyChars(this._chars, 0, array, 0, this._chars.Length);
					this._chars = array;
				}
				else
				{
					int num2 = this._charsUsed - this._charPos;
					if (num2 + charsRequired + 1 >= this._chars.Length)
					{
						char[] array2 = new char[num2 + charsRequired + 1];
						if (num2 > 0)
						{
							JsonTextReader.BlockCopyChars(this._chars, this._charPos, array2, 0, num2);
						}
						this._chars = array2;
					}
					else if (num2 > 0)
					{
						JsonTextReader.BlockCopyChars(this._chars, this._charPos, this._chars, 0, num2);
					}
					this._lineStartPos -= this._charPos;
					this._charPos = 0;
					this._charsUsed = num2;
				}
			}
			int count = this._chars.Length - this._charsUsed - 1;
			int num3 = this._reader.Read(this._chars, this._charsUsed, count);
			this._charsUsed += num3;
			if (num3 == 0)
			{
				this._isEndOfFile = true;
			}
			this._chars[this._charsUsed] = '\0';
			return num3;
		}

		private bool EnsureChars(int relativePosition, bool append)
		{
			return this._charPos + relativePosition < this._charsUsed || this.ReadChars(relativePosition, append);
		}

		private bool ReadChars(int relativePosition, bool append)
		{
			if (this._isEndOfFile)
			{
				return false;
			}
			int num = this._charPos + relativePosition - this._charsUsed + 1;
			int num2 = 0;
			do
			{
				int num3 = this.ReadData(append, num - num2);
				if (num3 == 0)
				{
					break;
				}
				num2 += num3;
			}
			while (num2 < num);
			return num2 >= num;
		}

		[DebuggerStepThrough]
		public override bool Read()
		{
			this._readType = ReadType.Read;
			if (!this.ReadInternal())
			{
				base.SetToken(JsonToken.None);
				return false;
			}
			return true;
		}

		public override byte[] ReadAsBytes()
		{
			return base.ReadAsBytesInternal();
		}

		public override decimal? ReadAsDecimal()
		{
			return base.ReadAsDecimalInternal();
		}

		public override int? ReadAsInt32()
		{
			return base.ReadAsInt32Internal();
		}

		public override string ReadAsString()
		{
			return base.ReadAsStringInternal();
		}

		public override DateTime? ReadAsDateTime()
		{
			return base.ReadAsDateTimeInternal();
		}

		public override DateTimeOffset? ReadAsDateTimeOffset()
		{
			return base.ReadAsDateTimeOffsetInternal();
		}

		internal override bool ReadInternal()
		{
			while (true)
			{
				switch (this._currentState)
				{
				case JsonReader.State.Start:
				case JsonReader.State.Property:
				case JsonReader.State.ArrayStart:
				case JsonReader.State.Array:
				case JsonReader.State.ConstructorStart:
				case JsonReader.State.Constructor:
					goto IL_46;
				case JsonReader.State.Complete:
				case JsonReader.State.Closed:
				case JsonReader.State.Error:
					continue;
				case JsonReader.State.ObjectStart:
				case JsonReader.State.Object:
					goto IL_4D;
				case JsonReader.State.PostValue:
					if (this.ParsePostValue())
					{
						return true;
					}
					continue;
				case JsonReader.State.Finished:
					goto IL_5E;
				}
				break;
			}
			throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
			IL_46:
			return this.ParseValue();
			IL_4D:
			return this.ParseObject();
			IL_5E:
			if (!this.EnsureChars(0, false))
			{
				return false;
			}
			this.EatWhitespace(false);
			if (this._isEndOfFile)
			{
				return false;
			}
			if (this._chars[this._charPos] == '/')
			{
				this.ParseComment();
				return true;
			}
			throw JsonReaderException.Create(this, "Additional text encountered after finished reading JSON content: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
		}

		private void ReadStringIntoBuffer(char quote)
		{
			int num = this._charPos;
			int charPos = this._charPos;
			int num2 = this._charPos;
			StringBuffer stringBuffer = null;
			char c2;
			while (true)
			{
				char c = this._chars[num++];
				if (c <= '\r')
				{
					if (c != '\0')
					{
						if (c != '\n')
						{
							if (c == '\r')
							{
								this._charPos = num - 1;
								this.ProcessCarriageReturn(true);
								num = this._charPos;
							}
						}
						else
						{
							this._charPos = num - 1;
							this.ProcessLineFeed();
							num = this._charPos;
						}
					}
					else if (this._charsUsed == num - 1)
					{
						num--;
						if (this.ReadData(true) == 0)
						{
							break;
						}
					}
				}
				else if (c != '"' && c != '\'')
				{
					if (c == '\\')
					{
						this._charPos = num;
						if (!this.EnsureChars(0, true))
						{
							goto Block_10;
						}
						int writeToPosition = num - 1;
						c2 = this._chars[num];
						char c3 = c2;
						char c4;
						if (c3 <= '\\')
						{
							if (c3 <= '\'')
							{
								if (c3 != '"' && c3 != '\'')
								{
									goto Block_14;
								}
							}
							else if (c3 != '/')
							{
								if (c3 != '\\')
								{
									goto Block_16;
								}
								num++;
								c4 = '\\';
								goto IL_2F8;
							}
							c4 = c2;
							num++;
						}
						else if (c3 <= 'f')
						{
							if (c3 != 'b')
							{
								if (c3 != 'f')
								{
									goto Block_19;
								}
								num++;
								c4 = '\f';
							}
							else
							{
								num++;
								c4 = '\b';
							}
						}
						else
						{
							if (c3 != 'n')
							{
								switch (c3)
								{
								case 'r':
									num++;
									c4 = '\r';
									goto IL_2F8;
								case 't':
									num++;
									c4 = '\t';
									goto IL_2F8;
								case 'u':
									num++;
									this._charPos = num;
									c4 = this.ParseUnicode();
									if (StringUtils.IsLowSurrogate(c4))
									{
										c4 = '�';
									}
									else if (StringUtils.IsHighSurrogate(c4))
									{
										bool flag;
										do
										{
											flag = false;
											if (this.EnsureChars(2, true) && this._chars[this._charPos] == '\\' && this._chars[this._charPos + 1] == 'u')
											{
												char writeChar = c4;
												this._charPos += 2;
												c4 = this.ParseUnicode();
												if (!StringUtils.IsLowSurrogate(c4))
												{
													if (StringUtils.IsHighSurrogate(c4))
													{
														writeChar = '�';
														flag = true;
													}
													else
													{
														writeChar = '�';
													}
												}
												if (stringBuffer == null)
												{
													stringBuffer = this.GetBuffer();
												}
												this.WriteCharToBuffer(stringBuffer, writeChar, num2, writeToPosition);
												num2 = this._charPos;
											}
											else
											{
												c4 = '�';
											}
										}
										while (flag);
									}
									num = this._charPos;
									goto IL_2F8;
								}
								goto Block_21;
							}
							num++;
							c4 = '\n';
						}
						IL_2F8:
						if (stringBuffer == null)
						{
							stringBuffer = this.GetBuffer();
						}
						this.WriteCharToBuffer(stringBuffer, c4, num2, writeToPosition);
						num2 = num;
					}
				}
				else if (this._chars[num - 1] == quote)
				{
					goto Block_30;
				}
			}
			this._charPos = num;
			throw JsonReaderException.Create(this, "Unterminated string. Expected delimiter: {0}.".FormatWith(CultureInfo.InvariantCulture, quote));
			Block_10:
			this._charPos = num;
			throw JsonReaderException.Create(this, "Unterminated string. Expected delimiter: {0}.".FormatWith(CultureInfo.InvariantCulture, quote));
			Block_14:
			Block_16:
			Block_19:
			Block_21:
			num++;
			this._charPos = num;
			throw JsonReaderException.Create(this, "Bad JSON escape sequence: {0}.".FormatWith(CultureInfo.InvariantCulture, "\\" + c2));
			Block_30:
			num--;
			if (charPos == num2)
			{
				this._stringReference = new StringReference(this._chars, charPos, num - charPos);
			}
			else
			{
				if (stringBuffer == null)
				{
					stringBuffer = this.GetBuffer();
				}
				if (num > num2)
				{
					stringBuffer.Append(this._chars, num2, num - num2);
				}
				this._stringReference = new StringReference(stringBuffer.GetInternalBuffer(), 0, stringBuffer.Position);
			}
			num++;
			this._charPos = num;
		}

		private void WriteCharToBuffer(StringBuffer buffer, char writeChar, int lastWritePosition, int writeToPosition)
		{
			if (writeToPosition > lastWritePosition)
			{
				buffer.Append(this._chars, lastWritePosition, writeToPosition - lastWritePosition);
			}
			buffer.Append(writeChar);
		}

		private char ParseUnicode()
		{
			if (this.EnsureChars(4, true))
			{
				string s = new string(this._chars, this._charPos, 4);
				char c = Convert.ToChar(int.Parse(s, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo));
				char result = c;
				this._charPos += 4;
				return result;
			}
			throw JsonReaderException.Create(this, "Unexpected end while parsing unicode character.");
		}

		private void ReadNumberIntoBuffer()
		{
			int num = this._charPos;
			while (true)
			{
				char c = this._chars[num++];
				if (c <= 'F')
				{
					if (c != '\0')
					{
						switch (c)
						{
						case '+':
						case '-':
						case '.':
						case '0':
						case '1':
						case '2':
						case '3':
						case '4':
						case '5':
						case '6':
						case '7':
						case '8':
						case '9':
						case 'A':
						case 'B':
						case 'C':
						case 'D':
						case 'E':
						case 'F':
							continue;
						}
						break;
					}
					if (this._charsUsed != num - 1)
					{
						goto IL_FD;
					}
					num--;
					this._charPos = num;
					if (this.ReadData(true) == 0)
					{
						return;
					}
				}
				else if (c != 'X')
				{
					switch (c)
					{
					case 'a':
					case 'b':
					case 'c':
					case 'd':
					case 'e':
					case 'f':
						break;
					default:
						if (c != 'x')
						{
							goto Block_6;
						}
						break;
					}
				}
			}
			Block_6:
			goto IL_107;
			IL_FD:
			this._charPos = num - 1;
			return;
			IL_107:
			this._charPos = num - 1;
		}

		private void ClearRecentString()
		{
			if (this._buffer != null)
			{
				this._buffer.Position = 0;
			}
			this._stringReference = default(StringReference);
		}

		private bool ParsePostValue()
		{
			char c;
			while (true)
			{
				c = this._chars[this._charPos];
				char c2 = c;
				if (c2 <= ')')
				{
					if (c2 <= '\r')
					{
						if (c2 != '\0')
						{
							switch (c2)
							{
							case '\t':
								break;
							case '\n':
								this.ProcessLineFeed();
								continue;
							case '\v':
							case '\f':
								goto IL_15A;
							case '\r':
								this.ProcessCarriageReturn(false);
								continue;
							default:
								goto IL_15A;
							}
						}
						else
						{
							if (this._charsUsed != this._charPos)
							{
								this._charPos++;
								continue;
							}
							if (this.ReadData(false) == 0)
							{
								break;
							}
							continue;
						}
					}
					else if (c2 != ' ')
					{
						if (c2 != ')')
						{
							goto IL_15A;
						}
						goto IL_FA;
					}
					this._charPos++;
					continue;
				}
				if (c2 <= '/')
				{
					if (c2 == ',')
					{
						goto IL_11A;
					}
					if (c2 == '/')
					{
						goto IL_112;
					}
				}
				else
				{
					if (c2 == ']')
					{
						goto IL_E2;
					}
					if (c2 == '}')
					{
						goto IL_CA;
					}
				}
				IL_15A:
				if (!char.IsWhiteSpace(c))
				{
					goto IL_178;
				}
				this._charPos++;
			}
			this._currentState = JsonReader.State.Finished;
			return false;
			IL_CA:
			this._charPos++;
			base.SetToken(JsonToken.EndObject);
			return true;
			IL_E2:
			this._charPos++;
			base.SetToken(JsonToken.EndArray);
			return true;
			IL_FA:
			this._charPos++;
			base.SetToken(JsonToken.EndConstructor);
			return true;
			IL_112:
			this.ParseComment();
			return true;
			IL_11A:
			this._charPos++;
			base.SetStateBasedOnCurrent();
			return false;
			IL_178:
			throw JsonReaderException.Create(this, "After parsing a value an unexpected character was encountered: {0}.".FormatWith(CultureInfo.InvariantCulture, c));
		}

		private bool ParseObject()
		{
			while (true)
			{
				char c = this._chars[this._charPos];
				char c2 = c;
				if (c2 <= '\r')
				{
					if (c2 != '\0')
					{
						switch (c2)
						{
						case '\t':
							break;
						case '\n':
							this.ProcessLineFeed();
							continue;
						case '\v':
						case '\f':
							goto IL_D7;
						case '\r':
							this.ProcessCarriageReturn(false);
							continue;
						default:
							goto IL_D7;
						}
					}
					else
					{
						if (this._charsUsed != this._charPos)
						{
							this._charPos++;
							continue;
						}
						if (this.ReadData(false) == 0)
						{
							break;
						}
						continue;
					}
				}
				else if (c2 != ' ')
				{
					if (c2 == '/')
					{
						goto IL_A5;
					}
					if (c2 != '}')
					{
						goto IL_D7;
					}
					goto IL_8D;
				}
				this._charPos++;
				continue;
				IL_D7:
				if (!char.IsWhiteSpace(c))
				{
					goto IL_F5;
				}
				this._charPos++;
			}
			return false;
			IL_8D:
			base.SetToken(JsonToken.EndObject);
			this._charPos++;
			return true;
			IL_A5:
			this.ParseComment();
			return true;
			IL_F5:
			return this.ParseProperty();
		}

		private bool ParseProperty()
		{
			char c = this._chars[this._charPos];
			char c2;
			if (c == '"' || c == '\'')
			{
				this._charPos++;
				c2 = c;
				this.ShiftBufferIfNeeded();
				this.ReadStringIntoBuffer(c2);
			}
			else
			{
				if (!this.ValidIdentifierChar(c))
				{
					throw JsonReaderException.Create(this, "Invalid property identifier character: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
				}
				c2 = '\0';
				this.ShiftBufferIfNeeded();
				this.ParseUnquotedProperty();
			}
			string value = this._stringReference.ToString();
			this.EatWhitespace(false);
			if (this._chars[this._charPos] != ':')
			{
				throw JsonReaderException.Create(this, "Invalid character after parsing property name. Expected ':' but got: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
			}
			this._charPos++;
			base.SetToken(JsonToken.PropertyName, value);
			this._quoteChar = c2;
			this.ClearRecentString();
			return true;
		}

		private bool ValidIdentifierChar(char value)
		{
			return char.IsLetterOrDigit(value) || value == '_' || value == '$';
		}

		private void ParseUnquotedProperty()
		{
			int charPos = this._charPos;
			char c2;
			while (true)
			{
				char c = this._chars[this._charPos];
				if (c == '\0')
				{
					if (this._charsUsed != this._charPos)
					{
						goto IL_42;
					}
					if (this.ReadData(true) == 0)
					{
						break;
					}
				}
				else
				{
					c2 = this._chars[this._charPos];
					if (!this.ValidIdentifierChar(c2))
					{
						goto IL_87;
					}
					this._charPos++;
				}
			}
			throw JsonReaderException.Create(this, "Unexpected end while parsing unquoted property name.");
			IL_42:
			this._stringReference = new StringReference(this._chars, charPos, this._charPos - charPos);
			return;
			IL_87:
			if (char.IsWhiteSpace(c2) || c2 == ':')
			{
				this._stringReference = new StringReference(this._chars, charPos, this._charPos - charPos);
				return;
			}
			throw JsonReaderException.Create(this, "Invalid JavaScript property identifier character: {0}.".FormatWith(CultureInfo.InvariantCulture, c2));
		}

		private bool ParseValue()
		{
			char c;
			while (true)
			{
				c = this._chars[this._charPos];
				char c2 = c;
				if (c2 <= 'I')
				{
					if (c2 <= '\r')
					{
						if (c2 != '\0')
						{
							switch (c2)
							{
							case '\t':
								break;
							case '\n':
								this.ProcessLineFeed();
								continue;
							case '\v':
							case '\f':
								goto IL_29C;
							case '\r':
								this.ProcessCarriageReturn(false);
								continue;
							default:
								goto IL_29C;
							}
						}
						else
						{
							if (this._charsUsed != this._charPos)
							{
								this._charPos++;
								continue;
							}
							if (this.ReadData(false) == 0)
							{
								break;
							}
							continue;
						}
					}
					else
					{
						switch (c2)
						{
						case ' ':
							break;
						case '!':
							goto IL_29C;
						case '"':
							goto IL_122;
						default:
							switch (c2)
							{
							case '\'':
								goto IL_122;
							case '(':
							case '*':
							case '+':
							case '.':
								goto IL_29C;
							case ')':
								goto IL_25A;
							case ',':
								goto IL_250;
							case '-':
								goto IL_1C4;
							case '/':
								goto IL_1FA;
							default:
								if (c2 != 'I')
								{
									goto IL_29C;
								}
								goto IL_1BC;
							}
							break;
						}
					}
					this._charPos++;
					continue;
				}
				if (c2 <= 'f')
				{
					if (c2 == 'N')
					{
						goto IL_1B4;
					}
					switch (c2)
					{
					case '[':
						goto IL_221;
					case '\\':
						break;
					case ']':
						goto IL_238;
					default:
						if (c2 == 'f')
						{
							goto IL_133;
						}
						break;
					}
				}
				else
				{
					if (c2 == 'n')
					{
						goto IL_13B;
					}
					switch (c2)
					{
					case 't':
						goto IL_12B;
					case 'u':
						goto IL_202;
					default:
						if (c2 == '{')
						{
							goto IL_20A;
						}
						break;
					}
				}
				IL_29C:
				if (!char.IsWhiteSpace(c))
				{
					goto IL_2BA;
				}
				this._charPos++;
			}
			return false;
			IL_122:
			this.ParseString(c);
			return true;
			IL_12B:
			this.ParseTrue();
			return true;
			IL_133:
			this.ParseFalse();
			return true;
			IL_13B:
			if (this.EnsureChars(1, true))
			{
				char c3 = this._chars[this._charPos + 1];
				if (c3 == 'u')
				{
					this.ParseNull();
				}
				else
				{
					if (c3 != 'e')
					{
						throw JsonReaderException.Create(this, "Unexpected character encountered while parsing value: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
					}
					this.ParseConstructor();
				}
				return true;
			}
			throw JsonReaderException.Create(this, "Unexpected end.");
			IL_1B4:
			this.ParseNumberNaN();
			return true;
			IL_1BC:
			this.ParseNumberPositiveInfinity();
			return true;
			IL_1C4:
			if (this.EnsureChars(1, true) && this._chars[this._charPos + 1] == 'I')
			{
				this.ParseNumberNegativeInfinity();
			}
			else
			{
				this.ParseNumber();
			}
			return true;
			IL_1FA:
			this.ParseComment();
			return true;
			IL_202:
			this.ParseUndefined();
			return true;
			IL_20A:
			this._charPos++;
			base.SetToken(JsonToken.StartObject);
			return true;
			IL_221:
			this._charPos++;
			base.SetToken(JsonToken.StartArray);
			return true;
			IL_238:
			this._charPos++;
			base.SetToken(JsonToken.EndArray);
			return true;
			IL_250:
			base.SetToken(JsonToken.Undefined);
			return true;
			IL_25A:
			this._charPos++;
			base.SetToken(JsonToken.EndConstructor);
			return true;
			IL_2BA:
			if (char.IsNumber(c) || c == '-' || c == '.')
			{
				this.ParseNumber();
				return true;
			}
			throw JsonReaderException.Create(this, "Unexpected character encountered while parsing value: {0}.".FormatWith(CultureInfo.InvariantCulture, c));
		}

		private void ProcessLineFeed()
		{
			this._charPos++;
			this.OnNewLine(this._charPos);
		}

		private void ProcessCarriageReturn(bool append)
		{
			this._charPos++;
			if (this.EnsureChars(1, append) && this._chars[this._charPos] == '\n')
			{
				this._charPos++;
			}
			this.OnNewLine(this._charPos);
		}

		private bool EatWhitespace(bool oneOrMore)
		{
			bool flag = false;
			bool flag2 = false;
			while (!flag)
			{
				char c = this._chars[this._charPos];
				char c2 = c;
				if (c2 != '\0')
				{
					if (c2 != '\n')
					{
						if (c2 != '\r')
						{
							if (c == ' ' || char.IsWhiteSpace(c))
							{
								flag2 = true;
								this._charPos++;
							}
							else
							{
								flag = true;
							}
						}
						else
						{
							this.ProcessCarriageReturn(false);
						}
					}
					else
					{
						this.ProcessLineFeed();
					}
				}
				else if (this._charsUsed == this._charPos)
				{
					if (this.ReadData(false) == 0)
					{
						flag = true;
					}
				}
				else
				{
					this._charPos++;
				}
			}
			return !oneOrMore || flag2;
		}

		private void ParseConstructor()
		{
			if (!this.MatchValueWithTrailingSeperator("new"))
			{
				throw JsonReaderException.Create(this, "Unexpected content while parsing JSON.");
			}
			this.EatWhitespace(false);
			int charPos = this._charPos;
			char c;
			while (true)
			{
				c = this._chars[this._charPos];
				if (c == '\0')
				{
					if (this._charsUsed != this._charPos)
					{
						goto IL_59;
					}
					if (this.ReadData(true) == 0)
					{
						break;
					}
				}
				else
				{
					if (!char.IsLetterOrDigit(c))
					{
						goto IL_8E;
					}
					this._charPos++;
				}
			}
			throw JsonReaderException.Create(this, "Unexpected end while parsing constructor.");
			IL_59:
			int charPos2 = this._charPos;
			this._charPos++;
			goto IL_118;
			IL_8E:
			if (c == '\r')
			{
				charPos2 = this._charPos;
				this.ProcessCarriageReturn(true);
			}
			else if (c == '\n')
			{
				charPos2 = this._charPos;
				this.ProcessLineFeed();
			}
			else if (char.IsWhiteSpace(c))
			{
				charPos2 = this._charPos;
				this._charPos++;
			}
			else
			{
				if (c != '(')
				{
					throw JsonReaderException.Create(this, "Unexpected character while parsing constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, c));
				}
				charPos2 = this._charPos;
			}
			IL_118:
			this._stringReference = new StringReference(this._chars, charPos, charPos2 - charPos);
			string value = this._stringReference.ToString();
			this.EatWhitespace(false);
			if (this._chars[this._charPos] != '(')
			{
				throw JsonReaderException.Create(this, "Unexpected character while parsing constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
			}
			this._charPos++;
			this.ClearRecentString();
			base.SetToken(JsonToken.StartConstructor, value);
		}

		private void ParseNumber()
		{
			this.ShiftBufferIfNeeded();
			char c = this._chars[this._charPos];
			int charPos = this._charPos;
			this.ReadNumberIntoBuffer();
			this._stringReference = new StringReference(this._chars, charPos, this._charPos - charPos);
			bool flag = char.IsDigit(c) && this._stringReference.Length == 1;
			bool flag2 = c == '0' && this._stringReference.Length > 1 && this._stringReference.Chars[this._stringReference.StartIndex + 1] != '.' && this._stringReference.Chars[this._stringReference.StartIndex + 1] != 'e' && this._stringReference.Chars[this._stringReference.StartIndex + 1] != 'E';
			object value;
			JsonToken newToken;
			if (this._readType == ReadType.ReadAsInt32)
			{
				if (flag)
				{
					value = (int)(c - '0');
				}
				else if (flag2)
				{
					string text = this._stringReference.ToString();
					int num = text.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt32(text, 16) : Convert.ToInt32(text, 8);
					value = num;
				}
				else
				{
					int num2;
					ParseResult parseResult = ConvertUtils.Int32TryParse(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length, out num2);
					if (parseResult == ParseResult.Success)
					{
						value = num2;
					}
					else
					{
						if (parseResult == ParseResult.Overflow)
						{
							throw JsonReaderException.Create(this, "JSON integer {0} is too large or small for an Int32.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()));
						}
						throw JsonReaderException.Create(this, "Input string '{0}' is not a valid integer.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()));
					}
				}
				newToken = JsonToken.Integer;
			}
			else if (this._readType == ReadType.ReadAsDecimal)
			{
				if (flag)
				{
					value = c - 48m;
				}
				else if (flag2)
				{
					string text2 = this._stringReference.ToString();
					long value2 = text2.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(text2, 16) : Convert.ToInt64(text2, 8);
					value = Convert.ToDecimal(value2);
				}
				else
				{
					string s = this._stringReference.ToString();
					decimal num3;
					if (!decimal.TryParse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out num3))
					{
						throw JsonReaderException.Create(this, "Input string '{0}' is not a valid decimal.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()));
					}
					value = num3;
				}
				newToken = JsonToken.Float;
			}
			else if (flag)
			{
				value = (long)((ulong)c - 48uL);
				newToken = JsonToken.Integer;
			}
			else if (flag2)
			{
				string text3 = this._stringReference.ToString();
				value = (text3.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(text3, 16) : Convert.ToInt64(text3, 8));
				newToken = JsonToken.Integer;
			}
			else
			{
				long num4;
				ParseResult parseResult2 = ConvertUtils.Int64TryParse(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length, out num4);
				if (parseResult2 == ParseResult.Success)
				{
					value = num4;
					newToken = JsonToken.Integer;
				}
				else if (parseResult2 == ParseResult.Overflow)
				{
					string value3 = this._stringReference.ToString();
					value = BigInteger.Parse(value3, CultureInfo.InvariantCulture);
					newToken = JsonToken.Integer;
				}
				else
				{
					string text4 = this._stringReference.ToString();
					if (this._floatParseHandling == FloatParseHandling.Decimal)
					{
						decimal num5;
						if (!decimal.TryParse(text4, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out num5))
						{
							throw JsonReaderException.Create(this, "Input string '{0}' is not a valid decimal.".FormatWith(CultureInfo.InvariantCulture, text4));
						}
						value = num5;
					}
					else
					{
						double num6;
						if (!double.TryParse(text4, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out num6))
						{
							throw JsonReaderException.Create(this, "Input string '{0}' is not a valid number.".FormatWith(CultureInfo.InvariantCulture, text4));
						}
						value = num6;
					}
					newToken = JsonToken.Float;
				}
			}
			this.ClearRecentString();
			base.SetToken(newToken, value);
		}

		private void ParseComment()
		{
			this._charPos++;
			if (!this.EnsureChars(1, false))
			{
				throw JsonReaderException.Create(this, "Unexpected end while parsing comment.");
			}
			bool flag;
			if (this._chars[this._charPos] == '*')
			{
				flag = false;
			}
			else
			{
				if (this._chars[this._charPos] != '/')
				{
					throw JsonReaderException.Create(this, "Error parsing comment. Expected: *, got {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
				}
				flag = true;
			}
			this._charPos++;
			int charPos = this._charPos;
			bool flag2 = false;
			while (!flag2)
			{
				char c = this._chars[this._charPos];
				if (c <= '\n')
				{
					if (c != '\0')
					{
						if (c == '\n')
						{
							if (flag)
							{
								this._stringReference = new StringReference(this._chars, charPos, this._charPos - charPos);
								flag2 = true;
							}
							this.ProcessLineFeed();
							continue;
						}
					}
					else
					{
						if (this._charsUsed != this._charPos)
						{
							this._charPos++;
							continue;
						}
						if (this.ReadData(true) != 0)
						{
							continue;
						}
						if (!flag)
						{
							throw JsonReaderException.Create(this, "Unexpected end while parsing comment.");
						}
						this._stringReference = new StringReference(this._chars, charPos, this._charPos - charPos);
						flag2 = true;
						continue;
					}
				}
				else
				{
					if (c == '\r')
					{
						if (flag)
						{
							this._stringReference = new StringReference(this._chars, charPos, this._charPos - charPos);
							flag2 = true;
						}
						this.ProcessCarriageReturn(true);
						continue;
					}
					if (c == '*')
					{
						this._charPos++;
						if (!flag && this.EnsureChars(0, true) && this._chars[this._charPos] == '/')
						{
							this._stringReference = new StringReference(this._chars, charPos, this._charPos - charPos - 1);
							this._charPos++;
							flag2 = true;
							continue;
						}
						continue;
					}
				}
				this._charPos++;
			}
			base.SetToken(JsonToken.Comment, this._stringReference.ToString());
			this.ClearRecentString();
		}

		private bool MatchValue(string value)
		{
			if (!this.EnsureChars(value.Length - 1, true))
			{
				return false;
			}
			for (int i = 0; i < value.Length; i++)
			{
				if (this._chars[this._charPos + i] != value[i])
				{
					return false;
				}
			}
			this._charPos += value.Length;
			return true;
		}

		private bool MatchValueWithTrailingSeperator(string value)
		{
			return this.MatchValue(value) && (!this.EnsureChars(0, false) || this.IsSeperator(this._chars[this._charPos]) || this._chars[this._charPos] == '\0');
		}

		private bool IsSeperator(char c)
		{
			if (c <= ')')
			{
				switch (c)
				{
				case '\t':
				case '\n':
				case '\r':
					break;
				case '\v':
				case '\f':
					goto IL_B8;
				default:
					if (c != ' ')
					{
						if (c != ')')
						{
							goto IL_B8;
						}
						if (base.CurrentState == JsonReader.State.Constructor || base.CurrentState == JsonReader.State.ConstructorStart)
						{
							return true;
						}
						return false;
					}
					break;
				}
				return true;
			}
			if (c <= '/')
			{
				if (c != ',')
				{
					if (c != '/')
					{
						goto IL_B8;
					}
					if (!this.EnsureChars(1, false))
					{
						return false;
					}
					char c2 = this._chars[this._charPos + 1];
					return c2 == '*' || c2 == '/';
				}
			}
			else if (c != ']' && c != '}')
			{
				goto IL_B8;
			}
			return true;
			IL_B8:
			if (char.IsWhiteSpace(c))
			{
				return true;
			}
			return false;
		}

		private void ParseTrue()
		{
			if (this.MatchValueWithTrailingSeperator(JsonConvert.True))
			{
				base.SetToken(JsonToken.Boolean, true);
				return;
			}
			throw JsonReaderException.Create(this, "Error parsing boolean value.");
		}

		private void ParseNull()
		{
			if (this.MatchValueWithTrailingSeperator(JsonConvert.Null))
			{
				base.SetToken(JsonToken.Null);
				return;
			}
			throw JsonReaderException.Create(this, "Error parsing null value.");
		}

		private void ParseUndefined()
		{
			if (this.MatchValueWithTrailingSeperator(JsonConvert.Undefined))
			{
				base.SetToken(JsonToken.Undefined);
				return;
			}
			throw JsonReaderException.Create(this, "Error parsing undefined value.");
		}

		private void ParseFalse()
		{
			if (this.MatchValueWithTrailingSeperator(JsonConvert.False))
			{
				base.SetToken(JsonToken.Boolean, false);
				return;
			}
			throw JsonReaderException.Create(this, "Error parsing boolean value.");
		}

		private void ParseNumberNegativeInfinity()
		{
			if (!this.MatchValueWithTrailingSeperator(JsonConvert.NegativeInfinity))
			{
				throw JsonReaderException.Create(this, "Error parsing negative infinity value.");
			}
			if (this._floatParseHandling == FloatParseHandling.Decimal)
			{
				throw new JsonReaderException("Cannot read -Infinity as a decimal.");
			}
			base.SetToken(JsonToken.Float, double.NegativeInfinity);
		}

		private void ParseNumberPositiveInfinity()
		{
			if (!this.MatchValueWithTrailingSeperator(JsonConvert.PositiveInfinity))
			{
				throw JsonReaderException.Create(this, "Error parsing positive infinity value.");
			}
			if (this._floatParseHandling == FloatParseHandling.Decimal)
			{
				throw new JsonReaderException("Cannot read Infinity as a decimal.");
			}
			base.SetToken(JsonToken.Float, double.PositiveInfinity);
		}

		private void ParseNumberNaN()
		{
			if (!this.MatchValueWithTrailingSeperator(JsonConvert.NaN))
			{
				throw JsonReaderException.Create(this, "Error parsing NaN value.");
			}
			if (this._floatParseHandling == FloatParseHandling.Decimal)
			{
				throw new JsonReaderException("Cannot read NaN as a decimal.");
			}
			base.SetToken(JsonToken.Float, double.NaN);
		}

		public override void Close()
		{
			base.Close();
			if (base.CloseInput && this._reader != null)
			{
				this._reader.Close();
			}
			if (this._buffer != null)
			{
				this._buffer.Clear();
			}
		}

		public bool HasLineInfo()
		{
			return true;
		}
	}
}
