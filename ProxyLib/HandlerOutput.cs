using System;
using System.Collections.Generic;
using System.Text;

namespace ProxyLib
{
	public class HandlerOutput
	{
		public List<string> DebugOutput;
		public List<byte[]> ForwardPackets;
		public List<byte[]> ReflectPackets;
		public int ConsumedBytes { get; set; }

		private HandlerOutput()
		{
			ForwardPackets = new List<byte[]>();
			ReflectPackets = new List<byte[]>();
			DebugOutput = new List<string>();
		}

		public static HandlerOutput CreateForward(byte[] data, string DebugOutput = null, int consumedBytes = 0)
		{
			var retval = CreateEmpty(DebugOutput, consumedBytes);
			retval.ConsumedBytes = consumedBytes;
			retval.ForwardPackets.Add(data);
			return retval;
		}

		public static HandlerOutput CreateEmpty(string debugOutput = null, int consumedBytes = 0)
		{
			var retval = new HandlerOutput();
			retval.ConsumedBytes = consumedBytes;
			if (!string.IsNullOrEmpty(debugOutput))
				retval.DebugOutput.Add(debugOutput);
			return retval;
		}

	}
}
