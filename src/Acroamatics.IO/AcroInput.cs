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
		private readonly IBufferCollection buffers;

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
			buffers = options.BufferCollection;

			foreach(var ctx in buffers)
			{
				ctx.Buffer = new uint[ctx.End - ctx.Start + 1];
				ctx.ByteBuffer = new byte[ctx.Buffer.Length * sizeof(uint)];
			}
		}

		public AcroInput() : this(new AcroInputOptions()) { }
		

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			try
			{
				while (!stoppingToken.IsCancellationRequested)
				{
					if (!(await ReadAllBuffersAsync(stoppingToken))) break;
				}
			}
			catch (OperationCanceledException) { }
			

			await innerPipe.Writer.CompleteAsync();

		}

		private async Task<bool> ReadAllBuffersAsync(CancellationToken stoppingToken)
		{
			FlushResult result;

			foreach (var context in buffers)
			{
				result = await nextBufferAsync(context, stoppingToken);

				if (result.IsCanceled || result.IsCompleted) return false;
			}

			return true;
		}

		private async ValueTask<FlushResult> nextBufferAsync(BufferContext context, CancellationToken stoppingToken)
		{
			context.Attempts = 0;
			var needsToBeFaster = false;
			uint count = 0;
			ValueTask<FlushResult> result;

			do
			{
				// Wait an appropriate amount of time until attempting to poll the buffer
				await Task.Delay(context.WaitTime);

				// get the current state of the counter
				count = getBufferCount(context);
				context.Attempts++;

				// if count is larger than the previous count, there is new data to be read
				if(count > context.PreviousCount)
				{
					// read the buffer and send to the pipe
					result = writeToPipeAsync(context, stoppingToken);

					// if the difference between the previous and current count is >1
					// 1 or more whole buffers were missed and the next wait time needs to be 
					// smaller.
					if (count > context.PreviousCount + 1)
						needsToBeFaster = true;

					// update the previous count
					context.PreviousCount = count;

					break;
				}

			} while (true);

			// adjustments to the wait time are made below

			if (context.Attempts > context.AcceptableMaxAttempts)
			{
				// WaitTime is too small, need to increment it (frequency is too fast - slow it down)

				if(context.PreviousWaitTime == WaitTimeState.TooSmall)
				{
					// the previous buffer read was too fast as well, so increment the wait time by alot
					context.WaitTime *= 2;
				}
				else
				{
					// increment the wait time by 5 ms (might change??)
					context.WaitTime += 5; //(context.WaitTime / 2);
				}

				// update for the next buffer read that this one was too quick
				context.PreviousWaitTime = WaitTimeState.TooSmall;
			}
			else if (needsToBeFaster || context.Attempts < context.AcceptableMinAttempts)
			{
				// WaitTime is too large, need to decrement it (frequency is too slow - speed it up)

				if (context.PreviousWaitTime == WaitTimeState.TooLarge)
				{
					// the previous buffer read was too slow as well, so decrement the wait time by alot
					context.WaitTime /= 2;
				}
				else
				{
					// decrement the wait time by 5 ms (might change??)
					context.WaitTime -= 5; //(context.WaitTime / 2);
				}

				// update for the next buffer read that this previous read was too slow
				context.PreviousWaitTime = WaitTimeState.TooLarge;
			}
			else
			{
				context.PreviousWaitTime = WaitTimeState.JustRight;
			}

			return await result;

		}

		private uint getBufferCount(BufferContext context)
		{
			var space = new uint[1];
			vmerblock(space, context.Counter, 1);

			return space[0];
		}

		private ValueTask<FlushResult> writeToPipeAsync(BufferContext context, CancellationToken stoppingToken)
		{
			vmerblock(context.Buffer, context.Start, (uint)context.Buffer.Length);

			Buffer.BlockCopy(context.Buffer, 0, context.ByteBuffer, 0,context.ByteBuffer.Length);
			
			return innerPipe.Writer.WriteAsync(context.ByteBuffer, stoppingToken);
		}


		// Acro API function to read memory off CVT
		[DllImport("libio")]
		private static extern void vmerblock(uint[] buffer, uint startingAddress, uint length);
	}
}
