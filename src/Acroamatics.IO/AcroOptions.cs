using System.IO.Pipelines;
using System.Threading.Channels;

namespace Acroamatics.IO
{
	public class AcroOptions
	{
		public Direction Direction { get; set; } = Direction.InOut;
		public bool Hosted { get; set; } = true;
	}

	public class AcroInputOptions
	{
		public IBufferCollection BufferCollection { get; set; } = new System.Collections.ObjectModel.Collection<BufferContext>() as IBufferCollection;
		public PipeOptions PipeOptions { get; set; }
	}

	public class AcroOutputOptions
	{
		public UnboundedChannelOptions ChannelOptions { get; set; } = new UnboundedChannelOptions { SingleReader = true };
		public bool UseArrayPool { get; set; } = true;
	}

	public interface IBufferCollection : System.Collections.Generic.ICollection<BufferContext>
	{ }

	public class BufferContext
	{
		public uint Start { get; set; }
		public uint End { get; set; }
		public uint Counter { get; set; }
		public uint CurrentCount { get; set; } = 0;
		public int Interval { get; set; } = 0;
	}

	public class OutputContext
	{
		public byte[] Buffer { get; set; }
		public uint Address { get; set; }
		public int Length { get; set; }
	}

	public enum Direction
	{
		In,
		Out,
		InOut
	}
}
