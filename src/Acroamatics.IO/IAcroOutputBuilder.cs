using System;

namespace Acroamatics.IO
{
	public interface IAcroOutputBuilder
	{
		public IAcroOutputBuilder ConfigureOptions(Action<AcroOutputOptions> configure);
		public IAcroOutput Build();
	}
}