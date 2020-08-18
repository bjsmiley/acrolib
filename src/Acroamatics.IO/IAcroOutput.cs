using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace Acroamatics.IO
{
	public interface IAcroOutput
	{
		public ChannelWriter<OutputContext> Writer { get; }
		public ArrayPool<uint> Pool { get; }
	}
}
