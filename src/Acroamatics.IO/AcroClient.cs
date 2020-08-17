using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Acroamatics.IO
{
	public class AcroClient : IAcroClient, IDisposable
	{
		public IAcroInput In
		{
			get
			{
				if (canInput) return @in;

				throw new NotSupportedException("Acroamatics Client was not configured with input support.");
			}
		}

		public IAcroOutput Out
		{
			get
			{
				if (canOutput) return @out;

				throw new NotSupportedException("Acroamatics Client was not configured with output support.");
			}
		}

		private bool canOutput;
		private bool canInput;
		private readonly IAcroInput @in;
		private readonly IAcroOutput @out;
		private readonly AcroOptions options;

		public AcroClient(IAcroInput input, IAcroOutput output, AcroOptions options)
		{
			this.options = options ?? new AcroOptions();
			this.@in = input;
			this.@out = output;

			canOutput = options.Direction == Direction.Out || options.Direction == Direction.InOut;
			canOutput &= @out != null;

			canInput = options.Direction == Direction.In || options.Direction == Direction.InOut;
			canInput &= @in != null;

		}

		public AcroClient(AcroOptions options) : this(null,null,options)
		{ }

		public AcroClient() : this(new AcroOptions())
		{ }

		#region IDisposable
		private bool disposedValue;
		protected virtual async void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing && !options.Hosted)
				{
					if (@in != null)
					{
						await (@in as AcroInput)?.StopAsync(CancellationToken.None);
						(@in as AcroInput)?.Dispose();
					}

					if (@out != null)
					{
						await (@out as AcroOutput)?.StopAsync(CancellationToken.None);
						(@out as AcroOutput)?.Dispose();
					}
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
