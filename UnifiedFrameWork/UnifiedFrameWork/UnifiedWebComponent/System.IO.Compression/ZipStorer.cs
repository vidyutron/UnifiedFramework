using System;
using System.Collections.Generic;
using System.Text;

namespace System.IO.Compression
{
	internal sealed class ZipStorer : IDisposable
	{
		public enum CompressionMethod : ushort
		{
			Store,
			Deflate = 8
		}

		public struct ZipFileEntry
		{
			public ZipStorer.CompressionMethod Method;

			public string FilenameInZip;

			public uint FileSize;

			public uint CompressedSize;

			public uint HeaderOffset;

			public uint FileOffset;

			public uint HeaderSize;

			public uint Crc32;

			public DateTime ModifyTime;

			public string Comment;

			public bool EncodeUTF8;

			public override string ToString()
			{
				return this.FilenameInZip;
			}
		}

		private static uint[] crcTable = ZipStorer.GenerateCrc32Table();

		private static Encoding defaultEncoding = Encoding.GetEncoding(437);

		private List<ZipStorer.ZipFileEntry> files = new List<ZipStorer.ZipFileEntry>();

		private Stream zipFileStream;

		private string comment = string.Empty;

		private byte[] centralDirectoryImage;

		private ushort existingFileCount;

		private FileAccess access;

		private bool encodeUtf8;

		private bool forceDeflating;

		public bool EncodeUtf8
		{
			get
			{
				return this.encodeUtf8;
			}
		}

		public bool ForceDeflating
		{
			get
			{
				return this.forceDeflating;
			}
		}

		public static ZipStorer Create(Stream zipStream, string fileComment)
		{
			return new ZipStorer
			{
				comment = fileComment,
				zipFileStream = zipStream,
				access = FileAccess.Write
			};
		}

		public static ZipStorer Open(Stream stream, FileAccess access)
		{
			if (!stream.CanSeek && access != FileAccess.Read)
			{
				throw new InvalidOperationException("Stream cannot seek");
			}
			ZipStorer zipStorer = new ZipStorer();
			zipStorer.zipFileStream = stream;
			zipStorer.access = access;
			if (zipStorer.ReadFileInfo())
			{
				return zipStorer;
			}
			throw new InvalidDataException();
		}

		public void AddFile(ZipStorer.CompressionMethod compressionMethod, string sourceFile, string fileNameInZip, string fileEntryComment)
		{
			if (this.access == FileAccess.Read)
			{
				throw new InvalidOperationException("Writing is not allowed");
			}
			using (FileStream fileStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
			{
				this.AddStream(compressionMethod, fileStream, fileNameInZip, File.GetLastWriteTime(sourceFile), fileEntryComment);
			}
		}

		public void AddStream(ZipStorer.CompressionMethod compressionMethod, Stream sourceStream, string fileNameInZip, DateTime modificationTimeStamp, string fileEntryComment)
		{
			if (this.access == FileAccess.Read)
			{
				throw new InvalidOperationException("Writing is not allowed");
			}
			ZipStorer.ZipFileEntry item = default(ZipStorer.ZipFileEntry);
			item.Method = compressionMethod;
			item.EncodeUTF8 = this.EncodeUtf8;
			item.FilenameInZip = ZipStorer.NormalizeFileName(fileNameInZip);
			item.Comment = ((fileEntryComment == null) ? string.Empty : fileEntryComment);
			item.Crc32 = 0u;
			item.HeaderOffset = (uint)this.zipFileStream.Position;
			item.ModifyTime = modificationTimeStamp;
			this.WriteLocalHeader(ref item);
			item.FileOffset = (uint)this.zipFileStream.Position;
			this.Store(ref item, sourceStream);
			sourceStream.Close();
			this.UpdateCrcAndSizes(ref item);
			this.files.Add(item);
		}

		public void Close()
		{
			if (this.access != FileAccess.Read)
			{
				uint offset = (uint)this.zipFileStream.Position;
				uint num = 0u;
				if (this.centralDirectoryImage != null)
				{
					this.zipFileStream.Write(this.centralDirectoryImage, 0, this.centralDirectoryImage.Length);
				}
				for (int i = 0; i < this.files.Count; i++)
				{
					long position = this.zipFileStream.Position;
					this.WriteCentralDirRecord(this.files[i]);
					num += (uint)(this.zipFileStream.Position - position);
				}
				if (this.centralDirectoryImage != null)
				{
					this.WriteEndRecord(num + (uint)this.centralDirectoryImage.Length, offset);
				}
				else
				{
					this.WriteEndRecord(num, offset);
				}
			}
			if (this.zipFileStream != null)
			{
				this.zipFileStream.Flush();
				this.zipFileStream.Dispose();
				this.zipFileStream = null;
			}
		}

		public List<ZipStorer.ZipFileEntry> ReadCentralDirectory()
		{
			if (this.centralDirectoryImage == null)
			{
				throw new InvalidOperationException("Central directory currently does not exist");
			}
			List<ZipStorer.ZipFileEntry> list = new List<ZipStorer.ZipFileEntry>();
			ushort num2;
			ushort num3;
			ushort num4;
			for (int i = 0; i < this.centralDirectoryImage.Length; i += (int)(46 + num2 + num3 + num4))
			{
				uint num = BitConverter.ToUInt32(this.centralDirectoryImage, i);
				if (num != 33639248u)
				{
					break;
				}
				bool flag = (BitConverter.ToUInt16(this.centralDirectoryImage, i + 8) & 2048) != 0;
				ushort method = BitConverter.ToUInt16(this.centralDirectoryImage, i + 10);
				uint dosTime = BitConverter.ToUInt32(this.centralDirectoryImage, i + 12);
				uint crc = BitConverter.ToUInt32(this.centralDirectoryImage, i + 16);
				uint compressedSize = BitConverter.ToUInt32(this.centralDirectoryImage, i + 20);
				uint fileSize = BitConverter.ToUInt32(this.centralDirectoryImage, i + 24);
				num2 = BitConverter.ToUInt16(this.centralDirectoryImage, i + 28);
				num3 = BitConverter.ToUInt16(this.centralDirectoryImage, i + 30);
				num4 = BitConverter.ToUInt16(this.centralDirectoryImage, i + 32);
				uint headerOffset = BitConverter.ToUInt32(this.centralDirectoryImage, i + 42);
				uint headerSize = (uint)(46 + num2 + num3 + num4);
				Encoding encoding = flag ? Encoding.UTF8 : ZipStorer.defaultEncoding;
				ZipStorer.ZipFileEntry item = default(ZipStorer.ZipFileEntry);
				item.Method = (ZipStorer.CompressionMethod)method;
				item.FilenameInZip = encoding.GetString(this.centralDirectoryImage, i + 46, (int)num2);
				item.FileOffset = this.GetFileOffset(headerOffset);
				item.FileSize = fileSize;
				item.CompressedSize = compressedSize;
				item.HeaderOffset = headerOffset;
				item.HeaderSize = headerSize;
				item.Crc32 = crc;
				item.ModifyTime = ZipStorer.DosTimeToDateTime(dosTime);
				if (num4 > 0)
				{
					item.Comment = encoding.GetString(this.centralDirectoryImage, i + 46 + (int)num2 + (int)num3, (int)num4);
				}
				list.Add(item);
			}
			return list;
		}

		public bool ExtractFile(ZipStorer.ZipFileEntry zipFileEntry, string destinationFileName)
		{
			string directoryName = Path.GetDirectoryName(destinationFileName);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			if (Directory.Exists(destinationFileName))
			{
				return true;
			}
			bool result = false;
			using (Stream stream = new FileStream(destinationFileName, FileMode.Create, FileAccess.Write))
			{
				result = this.ExtractFile(zipFileEntry, stream);
			}
			File.SetCreationTime(destinationFileName, zipFileEntry.ModifyTime);
			File.SetLastWriteTime(destinationFileName, zipFileEntry.ModifyTime);
			return result;
		}

		public bool ExtractFile(ZipStorer.ZipFileEntry zipFileEntry, Stream destinationStream)
		{
			if (!destinationStream.CanWrite)
			{
				throw new InvalidOperationException("Stream cannot be written");
			}
			byte[] array = new byte[4];
			this.zipFileStream.Seek((long)((ulong)zipFileEntry.HeaderOffset), SeekOrigin.Begin);
			this.zipFileStream.Read(array, 0, 4);
			if (BitConverter.ToUInt32(array, 0) != 67324752u)
			{
				return false;
			}
			Stream stream;
			if (zipFileEntry.Method == ZipStorer.CompressionMethod.Store)
			{
				stream = this.zipFileStream;
			}
			else
			{
				if (zipFileEntry.Method != ZipStorer.CompressionMethod.Deflate)
				{
					return false;
				}
				stream = new DeflateStream(this.zipFileStream, CompressionMode.Decompress, true);
			}
			byte[] array2 = new byte[16384];
			this.zipFileStream.Seek((long)((ulong)zipFileEntry.FileOffset), SeekOrigin.Begin);
			int num2;
			for (uint num = zipFileEntry.FileSize; num > 0u; num -= (uint)num2)
			{
				num2 = stream.Read(array2, 0, (int)Math.Min((long)((ulong)num), (long)array2.Length));
				destinationStream.Write(array2, 0, num2);
			}
			destinationStream.Flush();
			if (zipFileEntry.Method == ZipStorer.CompressionMethod.Deflate)
			{
				stream.Dispose();
			}
			return true;
		}

		public void Dispose()
		{
			this.Close();
		}

		private static uint[] GenerateCrc32Table()
		{
			uint[] array = new uint[256];
			for (int i = 0; i < array.Length; i++)
			{
				uint num = (uint)i;
				for (int j = 0; j < 8; j++)
				{
					if ((num & 1u) != 0u)
					{
						num = (3988292384u ^ num >> 1);
					}
					else
					{
						num >>= 1;
					}
				}
				array[i] = num;
			}
			return array;
		}

		private static uint DateTimeToDosTime(DateTime dateTime)
		{
			return (uint)(dateTime.Second / 2 | dateTime.Minute << 5 | dateTime.Hour << 11 | dateTime.Day << 16 | dateTime.Month << 21 | dateTime.Year - 1980 << 25);
		}

		private static DateTime DosTimeToDateTime(uint dosTime)
		{
			return new DateTime((int)((dosTime >> 25) + 1980u), (int)(dosTime >> 21 & 15u), (int)(dosTime >> 16 & 31u), (int)(dosTime >> 11 & 31u), (int)(dosTime >> 5 & 63u), (int)((dosTime & 31u) * 2u));
		}

		private static string NormalizeFileName(string fileNameToNormalize)
		{
			string text = fileNameToNormalize.Replace('\\', '/');
			int num = text.IndexOf(':');
			if (num >= 0)
			{
				text = text.Remove(0, num + 1);
			}
			return text.Trim(new char[]
			{
				'/'
			});
		}

		private uint GetFileOffset(uint headerOffset)
		{
			byte[] array = new byte[2];
			this.zipFileStream.Seek((long)((ulong)(headerOffset + 26u)), SeekOrigin.Begin);
			this.zipFileStream.Read(array, 0, 2);
			ushort num = BitConverter.ToUInt16(array, 0);
			this.zipFileStream.Read(array, 0, 2);
			ushort num2 = BitConverter.ToUInt16(array, 0);
			return (uint)((long)(30 + num + num2) + (long)((ulong)headerOffset));
		}

		private void WriteLocalHeader(ref ZipStorer.ZipFileEntry zipFileEntry)
		{
			long position = this.zipFileStream.Position;
			Encoding encoding = zipFileEntry.EncodeUTF8 ? Encoding.UTF8 : ZipStorer.defaultEncoding;
			byte[] bytes = encoding.GetBytes(zipFileEntry.FilenameInZip);
			this.zipFileStream.Write(new byte[]
			{
				80,
				75,
				3,
				4,
				20,
				0
			}, 0, 6);
			this.zipFileStream.Write(BitConverter.GetBytes(zipFileEntry.EncodeUTF8 ? 2048 : 0), 0, 2);
			this.zipFileStream.Write(BitConverter.GetBytes((ushort)zipFileEntry.Method), 0, 2);
			this.zipFileStream.Write(BitConverter.GetBytes(ZipStorer.DateTimeToDosTime(zipFileEntry.ModifyTime)), 0, 4);
			Stream arg_C2_0 = this.zipFileStream;
			byte[] buffer = new byte[12];
			arg_C2_0.Write(buffer, 0, 12);
			this.zipFileStream.Write(BitConverter.GetBytes((ushort)bytes.Length), 0, 2);
			this.zipFileStream.Write(BitConverter.GetBytes(0), 0, 2);
			this.zipFileStream.Write(bytes, 0, bytes.Length);
			zipFileEntry.HeaderSize = (uint)(this.zipFileStream.Position - position);
		}

		private void WriteCentralDirRecord(ZipStorer.ZipFileEntry zipFileEntry)
		{
			Encoding encoding = zipFileEntry.EncodeUTF8 ? Encoding.UTF8 : ZipStorer.defaultEncoding;
			byte[] bytes = encoding.GetBytes(zipFileEntry.FilenameInZip);
			byte[] bytes2 = encoding.GetBytes(zipFileEntry.Comment);
			this.zipFileStream.Write(new byte[]
			{
				80,
				75,
				1,
				2,
				23,
				11,
				20,
				0
			}, 0, 8);
			this.zipFileStream.Write(BitConverter.GetBytes(zipFileEntry.EncodeUTF8 ? 2048 : 0), 0, 2);
			this.zipFileStream.Write(BitConverter.GetBytes((ushort)zipFileEntry.Method), 0, 2);
			this.zipFileStream.Write(BitConverter.GetBytes(ZipStorer.DateTimeToDosTime(zipFileEntry.ModifyTime)), 0, 4);
			this.zipFileStream.Write(BitConverter.GetBytes(zipFileEntry.Crc32), 0, 4);
			this.zipFileStream.Write(BitConverter.GetBytes(zipFileEntry.CompressedSize), 0, 4);
			this.zipFileStream.Write(BitConverter.GetBytes(zipFileEntry.FileSize), 0, 4);
			this.zipFileStream.Write(BitConverter.GetBytes((ushort)bytes.Length), 0, 2);
			this.zipFileStream.Write(BitConverter.GetBytes(0), 0, 2);
			this.zipFileStream.Write(BitConverter.GetBytes((ushort)bytes2.Length), 0, 2);
			this.zipFileStream.Write(BitConverter.GetBytes(0), 0, 2);
			this.zipFileStream.Write(BitConverter.GetBytes(0), 0, 2);
			this.zipFileStream.Write(BitConverter.GetBytes(0), 0, 2);
			this.zipFileStream.Write(BitConverter.GetBytes(33024), 0, 2);
			this.zipFileStream.Write(BitConverter.GetBytes(zipFileEntry.HeaderOffset), 0, 4);
			this.zipFileStream.Write(bytes, 0, bytes.Length);
			this.zipFileStream.Write(bytes2, 0, bytes2.Length);
		}

		private void WriteEndRecord(uint size, uint offset)
		{
			Encoding encoding = this.EncodeUtf8 ? Encoding.UTF8 : ZipStorer.defaultEncoding;
			byte[] bytes = encoding.GetBytes(this.comment);
			this.zipFileStream.Write(new byte[]
			{
				80,
				75,
				5,
				6,
				0,
				0,
				0,
				0
			}, 0, 8);
			this.zipFileStream.Write(BitConverter.GetBytes((int)((ushort)this.files.Count + this.existingFileCount)), 0, 2);
			this.zipFileStream.Write(BitConverter.GetBytes((int)((ushort)this.files.Count + this.existingFileCount)), 0, 2);
			this.zipFileStream.Write(BitConverter.GetBytes(size), 0, 4);
			this.zipFileStream.Write(BitConverter.GetBytes(offset), 0, 4);
			this.zipFileStream.Write(BitConverter.GetBytes((ushort)bytes.Length), 0, 2);
			this.zipFileStream.Write(bytes, 0, bytes.Length);
		}

		private void Store(ref ZipStorer.ZipFileEntry zipFileEntry, Stream sourceStream)
		{
			byte[] array = new byte[16384];
			uint num = 0u;
			long position = this.zipFileStream.Position;
			long position2 = sourceStream.Position;
			Stream stream;
			if (zipFileEntry.Method == ZipStorer.CompressionMethod.Store)
			{
				stream = this.zipFileStream;
			}
			else
			{
				stream = new DeflateStream(this.zipFileStream, CompressionMode.Compress, true);
			}
			zipFileEntry.Crc32 = 4294967295u;
			int num2;
			do
			{
				num2 = sourceStream.Read(array, 0, array.Length);
				num += (uint)num2;
				if (num2 > 0)
				{
					stream.Write(array, 0, num2);
					uint num3 = 0u;
					while ((ulong)num3 < (ulong)((long)num2))
					{
						zipFileEntry.Crc32 = (ZipStorer.crcTable[(int)((UIntPtr)((zipFileEntry.Crc32 ^ (uint)array[(int)((UIntPtr)num3)]) & 255u))] ^ zipFileEntry.Crc32 >> 8);
						num3 += 1u;
					}
				}
			}
			while (num2 == array.Length);
			stream.Flush();
			if (zipFileEntry.Method == ZipStorer.CompressionMethod.Deflate)
			{
				stream.Dispose();
			}
			zipFileEntry.Crc32 ^= 4294967295u;
			zipFileEntry.FileSize = num;
			zipFileEntry.CompressedSize = (uint)(this.zipFileStream.Position - position);
			if (zipFileEntry.Method == ZipStorer.CompressionMethod.Deflate && !this.ForceDeflating && sourceStream.CanSeek && zipFileEntry.CompressedSize > zipFileEntry.FileSize)
			{
				zipFileEntry.Method = ZipStorer.CompressionMethod.Store;
				this.zipFileStream.Position = position;
				this.zipFileStream.SetLength(position);
				sourceStream.Position = position2;
				this.Store(ref zipFileEntry, sourceStream);
			}
		}

		private void UpdateCrcAndSizes(ref ZipStorer.ZipFileEntry zipFileEntry)
		{
			long position = this.zipFileStream.Position;
			this.zipFileStream.Position = (long)((ulong)(zipFileEntry.HeaderOffset + 8u));
			this.zipFileStream.Write(BitConverter.GetBytes((ushort)zipFileEntry.Method), 0, 2);
			this.zipFileStream.Position = (long)((ulong)(zipFileEntry.HeaderOffset + 14u));
			this.zipFileStream.Write(BitConverter.GetBytes(zipFileEntry.Crc32), 0, 4);
			this.zipFileStream.Write(BitConverter.GetBytes(zipFileEntry.CompressedSize), 0, 4);
			this.zipFileStream.Write(BitConverter.GetBytes(zipFileEntry.FileSize), 0, 4);
			this.zipFileStream.Position = position;
		}

		private bool ReadFileInfo()
		{
			if (this.zipFileStream.Length < 22L)
			{
				return false;
			}
			try
			{
				this.zipFileStream.Seek(-17L, SeekOrigin.End);
				BinaryReader binaryReader = new BinaryReader(this.zipFileStream);
				while (true)
				{
					this.zipFileStream.Seek(-5L, SeekOrigin.Current);
					uint num = binaryReader.ReadUInt32();
					if (num == 101010256u)
					{
						break;
					}
					if (this.zipFileStream.Position <= 0L)
					{
						goto Block_5;
					}
				}
				this.zipFileStream.Seek(6L, SeekOrigin.Current);
				ushort num2 = binaryReader.ReadUInt16();
				int num3 = binaryReader.ReadInt32();
				uint num4 = binaryReader.ReadUInt32();
				ushort num5 = binaryReader.ReadUInt16();
				bool result;
				if (this.zipFileStream.Position + (long)((ulong)num5) != this.zipFileStream.Length)
				{
					result = false;
					return result;
				}
				this.existingFileCount = num2;
				this.centralDirectoryImage = new byte[num3];
				this.zipFileStream.Seek((long)((ulong)num4), SeekOrigin.Begin);
				this.zipFileStream.Read(this.centralDirectoryImage, 0, num3);
				this.zipFileStream.Seek((long)((ulong)num4), SeekOrigin.Begin);
				result = true;
				return result;
				Block_5:;
			}
			catch (IOException)
			{
			}
			return false;
		}
	}
}
