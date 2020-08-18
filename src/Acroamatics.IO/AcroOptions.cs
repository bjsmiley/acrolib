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
		/// <summary>
		/// The 32-bit buffer used when reading from the Acro CVT. No need to
		/// configure, it is managed by <see cref="AcroInput"/>.
		/// </summary>
		public uint[] Buffer { get; set; }
		/// <summary>
		/// The 8-bit buffer used when writing to to <see cref="PipeWriter"/>. No
		/// need to configure, it is managed by <see cref="AcroInput"/>.
		/// </summary>
		public byte[] ByteBuffer { get; set; }
		/// <summary>
		/// The number of attempts reading from address <see cref="Counter"/> before 
		/// the count increments. No need need to configure, it is managed by <see cref="AcroInput"/>.
		/// </summary>
		public int Attempts { get; set; } = 0;
		/// <summary>
		/// The value stored at <see cref="Counter"/> address after the previous read.
		/// No need need to configure, it is managed by <see cref="AcroInput"/>.
		/// </summary>
		public uint PreviousCount { get; set; } = 0;
		/// <summary>
		/// The final status of the buffer read. 
		/// No need need to configure, it is managed by <see cref="AcroInput"/>.
		/// </summary>
		public WaitTimeState PreviousWaitTime { get; set; } = WaitTimeState.JustRight;
		/// <summary>
		/// The address where the buffer starts in the CVT.
		/// </summary>
		public uint Start { get; set; }
		/// <summary>
		/// The address where the buffer ends in the CVT.
		/// </summary>
		public uint End { get; set; }
		/// <summary>
		/// The address where the buffer counter exists in the CVT.
		/// </summary>
		public uint Counter { get; set; }
		/// <summary>
		/// The initial wait time (in ms) before reading from the buffer. 
		/// Recommended to configure as a small amount (NOT 0). Default
		/// is 1.
		/// </summary>
		public int WaitTime { get; set; } = 1;
		/// <summary>
		/// The acceptable maximum number of attempts to read the buffer
		/// before <see cref="AcroInput"/> considers the read to be ineffiecent,
		/// and pointlessly fast.
		/// </summary>
		public int AcceptableMaxAttempts { get; set; } = 10;
		/// <summary>
		/// The acceptable minimum number of attempts to read the buffer before
		/// <see cref="AcroInput"/> considers the read to be ineffiecent,
		/// and too slow.
		/// </summary>
		public int AcceptableMinAttempts { get; set; } = 4;

	}

	public enum WaitTimeState
	{
		TooSmall,
		TooLarge,
		JustRight
	}

	public class OutputContext
	{
		public uint[] Buffer { get; set; }
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
