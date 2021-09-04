using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProxyLib
{
	public class Encoder
	{
		public EncodingStyle Style { get; private set; }

		public static Encoder CreateFromStyleName(string style)
		{
			foreach (EncodingStyle st in Enum.GetValues(typeof(EncodingStyle)).Cast<EncodingStyle>())
			{
				if (st.ToString().ToLowerInvariant().StartsWith(style))
					return new Encoder {Style = st};
			}

			throw new ArgumentOutOfRangeException(nameof(style), $"Invalid encoding style '{style}'");
		}

		public string Encode(byte[] buffer, int len)
		{
			switch(Style)
			{
				case EncodingStyle.Ascii:
					return Encoding.ASCII.GetString(buffer, 0, len);
				case EncodingStyle.Unicode:
					return Encoding.Unicode.GetString(buffer, 0, len);
				case EncodingStyle.Base64:
					return Convert.ToBase64String(buffer, 0, len);
				case EncodingStyle.Hex:
					return EncodeHex(buffer, len);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static string EncodeHex(byte[] buffer, int len)
		{
			return EncodeHex(buffer, 0, len);
		}

		public static string EncodeHex(byte[] buffer, int offset, int len)
		{
			var sb = new StringBuilder(2 * len);
			for (int i = 0; i < len; i++)
				sb.Append($"{buffer[i + offset]:x2}");
			return sb.ToString();
		}

		public byte[] Decode(string data)
		{
			switch(Style)
			{
				case EncodingStyle.Ascii:
					return Encoding.ASCII.GetBytes(data, 0, data.Length);
				case EncodingStyle.Unicode:
					return Encoding.Unicode.GetBytes(data, 0, data.Length);
				case EncodingStyle.Base64:
					return Convert.FromBase64String(data);
				case EncodingStyle.Hex:
					return DecodeHex(data);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static void WriteHex(byte[] target, string data, int offset)
		{
			for (int i = 0, j = offset; i < data.Length; i += 2, j++)
				target[j] = Convert.ToByte(data.Substring(i, 2), 16);
		}

		public static byte[] DecodeHex(string data)
		{
			var retval = new byte[data.Length / 2];
			for (int i = 0, j = 0; i < data.Length; i += 2, j++)
				retval[j] = Convert.ToByte(data.Substring(i, 2), 16);
			return retval;
		}

		public static IEnumerable<int> IntifyData(byte[] data, int offset)
		{
			while (offset <= data.Length - 4)
			{
				yield return BitConverter.ToInt32(data, offset);
				offset += 4;
			}
		}

		public static IEnumerable<long> LongifyData(byte[] data, int offset)
		{
			while (offset <= data.Length - 8)
			{
				yield return BitConverter.ToInt64(data, offset);
				offset += 4;
			}
		}

		public static IEnumerable<float> FloatifyData(byte[] data, int offset)
		{
			while (offset <= data.Length - 4)
			{
				yield return BitConverter.ToSingle(data, offset);
				offset += 4;
			}
		}
	}
}