using System;
using System.Collections.Generic;
using System.Text;

namespace Acroamatics.IO
{
	public class AcroClientBuilder : IAcroClientBuilder
	{

		private AcroOptions options;
		private bool isbuilt;
		private IAcroInput input;
		private IAcroOutput output;


		public IAcroClientBuilder ConfigureOptions(Action<AcroOptions> configure)
		{
			if (options == null)
				options = new AcroOptions();

			configure(options);

			return this;

		}

		public IAcroClientBuilder AddInput(IAcroInput input)
		{
			this.input = input;

			return this;
		}

		public IAcroClientBuilder AddOutput(IAcroOutput output)
		{
			this.output = output;

			return this;
		}

		public IAcroClient Build()
		{
			if (isbuilt)
				throw new InvalidOperationException("AcroClient is already built.");

			isbuilt = true;

			return new AcroClient(input, output, options);


		}


	}
}
