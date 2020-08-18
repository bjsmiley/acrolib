using Microsoft.Extensions.Hosting;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Acroamatics.IO.Example
{
	public class Service : BackgroundService
	{
		private readonly IAcroClient client;

		private int found = 0;

		public Service(IAcroClient client)
		{
			this.client = client;
		}
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			var packet = new byte[32]; // pretending all packets are 32 bytes long

			while(!stoppingToken.IsCancellationRequested)
			{
				try
				{
					var result = await client.In.Reader.ReadAsync(stoppingToken);

					var buffer = result.Buffer;

					var position = readItems(buffer, packet);

					if (result.IsCompleted) 
						break;

					client.In.Reader.AdvanceTo(position, buffer.End);
			
				}
				catch(OperationCanceledException)
				{
					break;
				}
				
			}
		}

		private SequencePosition readItems(ReadOnlySequence<byte> sequence, byte[] packet)
		{
			var reader = new SequenceReader<byte>(sequence);

			while(!reader.End)
			{
				var isFound = reader.TryAdvanceToAny(new byte[] { 0x69, 0x42, 0xFA, 0x00,0x01,0x03 }, false);

				if (!isFound)
					break;
				
				if(reader.Remaining >= 32)
				{
					var packetSequence = reader.Sequence.Slice(reader.Position, 32);

					found++;

					reader.Advance(32);

					packetSequence.CopyTo(packet);

					writeBack(packet).AsTask().Wait();
				}
				else
				{
					break;
				}
			}

			return reader.Position;
		}

		private ValueTask writeBack(byte[] packet)
		{
			var writablePacket = MemoryMarshal.Cast<byte, uint>(packet);

			// maybe use an object pool?
			// maybe use the array pool?
			var ctx = new OutputContext
			{
				Address = 0xAA220000,
				Length = 10,
				Buffer = writablePacket.ToArray()
			};
			return client.Out.Writer.WriteAsync(ctx);
		}
	}
}
