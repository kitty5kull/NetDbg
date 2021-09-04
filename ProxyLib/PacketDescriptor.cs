using System;
using System.Collections.Generic;
using System.Text;

namespace ProxyLib
{
	public class PacketDescriptor
	{
		public string Proxy { get; set; }
		public string ConnectionIdentifier { get; set; }
		public string Direction { get; set; }
		public string EncodedData { get; set; }
	}
}
