using System;

namespace PendleCodeMonkey.MC68000EmulatorLib
{
	/// <summary>
	/// Implementation of the <see cref="TrapException"/> class.
	/// </summary>
	public class TrapException : Exception
	{
		/// <summary>
		/// Gets the TRAP vector value.
		/// </summary>
		public ushort Vector { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TrapException"/> class.
		/// </summary>
		protected TrapException() : base()
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="TrapException"/> class.
		/// </summary>
		/// <param name="vector">The TRAP vector value.</param>
		public TrapException(ushort vector) : base(String.Format("A TRAP #{0} exception was thrown.", vector))
		{
			Vector = vector;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TrapException"/> class.
		/// </summary>
		/// <param name="vector">The TRAP vector value.</param>
		/// <param name="message">The exception message.</param>
		public TrapException(ushort vector, string message) : base(message)
		{
			Vector = vector;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TrapException"/> class.
		/// </summary>
		/// <param name="vector">The TRAP vector value.</param>
		/// <param name="message">The exception message.</param>
		/// <param name="innerException">Inner <see cref="Exception"/> object.</param>
		public TrapException(ushort vector, string message, Exception innerException) : base(message, innerException)
		{
			Vector = vector;
		}
	}
}
