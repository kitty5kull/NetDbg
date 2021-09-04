using System;
using System.Collections.Generic;
using System.Text;

namespace NetDbg
{
	class Layer4Address
	{
		public string Host { get; set; }
		public int Port { get; set; }

		public override string ToString()
			=> $"{Host}:{Port}";
	}

}
