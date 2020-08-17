using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Acroamatics.IO
{
	public class AcroInput : BackgroundService, IAcroInput
	{
		

		public PipeReader Reader { get; }

		private readonly AcroInputOptions options;
		private readonly Pipe innerPipe;

		public AcroInput(AcroInputOptions options)
		{
			this.options = options;
			if(options.PipeOptions != null)
			{
				innerPipe = new Pipe(this.options.PipeOptions);
			}
			else
			{
				innerPipe = new Pipe();
			}
			
			Reader = innerPipe.Reader;
		}

		public AcroInput() : this(new AcroInputOptions()) { }
		

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while(!stoppingToken.IsCancellationRequested)
			{
				foreach(var context in options.BufferCollection)
				{
					if (await bufferReadAsync(context, stoppingToken)) break;
				}
			}

			await innerPipe.Writer.CompleteAsync();

		}

		private async Task<bool> bufferReadAsync(BufferContext context, CancellationToken stoppingToken)
		{
			// polling read


			// load it up
			//var buffer = vmerblock(context.)
			var temp = new byte[1];

			// memory = pipe.Writer.GetMemory(size);
			try
			{
				var result = await innerPipe.Writer.WriteAsync(temp, stoppingToken);

				return result.IsCanceled || result.IsCompleted;
			}
			catch(OperationCanceledException)
			{
				return true;
			}


			
		}


		// Acro API function to read memory off CVT
		[DllImport("libio")]
		private static extern void vmerblock(byte[] buffer, uint startingAddress, uint length);
	}
}
