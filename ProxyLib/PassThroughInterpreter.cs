using System;
using System.Collections.Generic;
using System.Text;

namespace ProxyLib
{
	public class PassThroughInterpreter : IInterpreter
	{
		private readonly bool _generateDebugOutput;
		private readonly IEnumerable<string> _initCommands;
		private readonly Encoder _encoder;

		public string CurrentStatus => throw new NotImplementedException();

		public event EventHandler<string> CommandPending;
		public event EventHandler<string> OutputLinePending;

		public PassThroughInterpreter(bool generateDebugOutput = false, IEnumerable<string> initCommands = null, Encoder encoder = null)
		{
			_generateDebugOutput = generateDebugOutput;
			_initCommands = initCommands;
			_encoder = encoder;
		}

		public void Init()
		{
			foreach(var cmd in _initCommands)
				CommandPending?.Invoke(this, cmd);
		}

		public void InterpretErrorLine(string line)
		{
			OutputLinePending?.Invoke(this, line);
		}

		public void InterpretOutputLine(string line)
		{
			if (line.StartsWith("packet"))
			{
				if (_generateDebugOutput && OutputLinePending != null)
				{
					var args = line.Split(' ');
					var dto = new PacketDescriptor
					{
						Proxy = args[1],
						ConnectionIdentifier = args[2],
						Direction = args[3],
						EncodedData = args[4]
					};
					foreach(var dbg in GenerateDataDump(dto, _encoder))
						OutputLinePending.Invoke(this, dbg);
				}
				CommandPending?.Invoke(this, line);
			}
		}

		public void SendCommand(string command)
		{
			throw new NotImplementedException();
		}

		public static IEnumerable<string> GenerateDataDump(PacketDescriptor packet, Encoder encoder)
		{
			var data = encoder.Decode(packet.EncodedData);
			int dataLength = data.Length;
			char type1 = (data[0] >= 0x20 && data[0] <= 0x7f) ? (char)data[0] : '.';
			char type2 = (data[1] >= 0x20 && data[1] <= 0x7f) ? (char)data[1] : '.';

			if (data.Length <= 32)
				yield return $"{packet.Proxy}/{packet.ConnectionIdentifier} {packet.Direction:3} [0x{data[0]:x2}{data[1]:x2} '{type1}{type2}'] ({dataLength,2}) {packet.EncodedData,-64} |{GetDump(data, 0, data.Length),-32}|";
			else
			{
				yield return $"{packet.Proxy}/{packet.ConnectionIdentifier} {packet.Direction:3} [0x{data[0]:x2}{data[1]:x2} '{type1}{type2}'], Length {dataLength}";
				for (int i = 0; i < data.Length; i += 64)
				{
					int len = data.Length - i;
					if (len > 64) len = 64;
					yield return $"   ({len:00}) {Encoder.EncodeHex(data, i, len),-128} |{GetDump(data, i, len),-64}|";
				}
			}
		}

		public static string GetDump(byte[] data, int offset, int len)
		{
			StringBuilder sb = new StringBuilder();

			for (int i = offset ; i < offset + len; i++)
				sb.Append(new string(data[i] >= 0x20 && data[i] < 0x7f ? (char)data[i] : '.', 1));

			return sb.ToString();
		}

	}
}
