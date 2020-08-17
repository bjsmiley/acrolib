using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Pipelines;
using System.Threading.Tasks;
using System.Threading.Channels;

namespace Acroamatics.IO
{
	/// <summary>
	/// Acroamatics IO Client Interface
	/// </summary>
	public interface IAcroClient
	{
		/// <summary>
		/// Gets the readable half of this client.
		/// </summary>
		public IAcroInput In { get; }

		/// <summary>
		/// Gets the writable half of this client.
		/// </summary>
		public IAcroOutput Out { get; }
	}
}
