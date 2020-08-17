using System;

namespace Acroamatics.IO
{
	class AcroInputBuilder : IAcroInputBuilder
	{
		private AcroInputOptions options;
		private bool built;

		public IAcroInputBuilder ConfigureOptions(Action<AcroInputOptions> configure)
		{
			if (options == null)
				options = new AcroInputOptions();

			configure(options);

			return this;
		}

		public IAcroInput Build()
		{
			if (built)
				throw new InvalidOperationException("AcroInput is already built.");
			
			built = true;

			if (options == null)
				return new AcroInput();
			else
				return new AcroInput(options);
		}
	}
}
