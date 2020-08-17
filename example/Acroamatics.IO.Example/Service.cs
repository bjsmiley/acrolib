using Microsoft.Extensions.Hosting;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Acroamatics.IO.Example
{
	public class Service : BackgroundService
	{
		private readonly IAcroClient client;

		private int largeCount = 0;

		public Service(IAcroClient client)
		{
			this.client = client;
		}
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while(!stoppingToken.IsCancellationRequested)
			{
				try
				{
					var result = await client.In.Reader.ReadAsync(stoppingToken);
					
					if( result.Buffer.Slice(0, 1).ToArray()[0] > 128 )
					{
						if (largeCount == int.MaxValue - 1)
							largeCount = 0;
						else
							largeCount++;

						await writeback(result.Buffer);

						client.In.Reader.AdvanceTo(result.Buffer.GetPosition(9));
					}
				}
				catch(OperationCanceledException)
				{
					break;
				}
				
			}
		}

		private ValueTask writeback(ReadOnlySequence<byte> buffer)
		{
			var newbuffer = client.Out.Pool.Rent(10);
			buffer.CopyTo(newbuffer);

			// maybe use an object pool? 
			var ctx = new OutputContext
			{
				Address = 0xAA220000,
				Length = 10,
				Buffer = newbuffer
			};

			return client.Out.Writer.WriteAsync(ctx);
		}
	}
}
