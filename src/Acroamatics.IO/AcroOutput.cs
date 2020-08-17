using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Acroamatics.IO
{
	public class AcroOutput : BackgroundService, IAcroOutput
	{

		public ChannelWriter<OutputContext> Writer { get; }

		public ArrayPool<byte> Pool
		{
			get
			{

				return pool ?? throw new NotSupportedException("Acroamatics Client Output was not configured with MemoryPool support.");
			}
		}

		private readonly AcroOutputOptions options;
		private readonly Channel<OutputContext> channel;
		private readonly ArrayPool<byte> pool;

		public AcroOutput(AcroOutputOptions options)
		{
			this.options = options;
			channel = Channel.CreateUnbounded<OutputContext>(this.options.ChannelOptions);
			Writer = channel.Writer;
			if (options.UseArrayPool) pool = ArrayPool<byte>.Shared;
		}

		public AcroOutput() : this(new AcroOutputOptions()) { }


		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			
			while(!stoppingToken.IsCancellationRequested)
			{
				try
				{
					var ctx = await channel.Reader.ReadAsync(stoppingToken);

					vmewblock(ctx.Buffer, ctx.Address, ctx.Length);

					pool?.Return(ctx.Buffer);

					// use a object pool and return here?
				}
				catch(OperationCanceledException)
				{
					break;
				}
			}

		}

		

		// Acro API function to write buffers to memory CVT
		[DllImport("libio")]
		private static extern void vmewblock(byte[] buffer, uint startingAddress, int length);
	}
}
