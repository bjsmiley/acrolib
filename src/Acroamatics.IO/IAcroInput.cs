using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Text;

namespace Acroamatics.IO
{
	public interface IAcroInput
	{
		public PipeReader Reader { get; }
	}
}
