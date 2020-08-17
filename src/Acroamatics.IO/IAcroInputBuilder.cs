using System;

namespace Acroamatics.IO
{
	public interface IAcroInputBuilder
	{
		public IAcroInputBuilder ConfigureOptions(Action<AcroInputOptions> configure);
		public IAcroInput Build();
	}
}