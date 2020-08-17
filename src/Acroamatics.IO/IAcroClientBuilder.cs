using System;
using System.Collections.Generic;
using System.Text;

namespace Acroamatics.IO
{
	public interface IAcroClientBuilder
	{

		public IAcroClientBuilder ConfigureOptions(Action<AcroOptions> configure);
		public IAcroClientBuilder AddInput(IAcroInput input);
		public IAcroClientBuilder AddOutput(IAcroOutput output);
		public IAcroClient Build();
	}
}
