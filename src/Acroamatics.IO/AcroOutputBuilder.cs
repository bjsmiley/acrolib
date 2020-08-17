using System;

namespace Acroamatics.IO
{
	class AcroOutputBuilder : IAcroOutputBuilder
	{
		private AcroOutputOptions options;
		private bool built;

		public IAcroOutputBuilder ConfigureOptions(Action<AcroOutputOptions> configure)
		{
			if (options == null)
				options = new AcroOutputOptions();

			configure(options);

			return this;
		}

		public IAcroOutput Build()
		{
			if (built)
				throw new InvalidOperationException("AcroOutput is already built.");

			built = true;

			if (options == null)
				return new AcroOutput();
			else
				return new AcroOutput(options);
		}
	}
}
