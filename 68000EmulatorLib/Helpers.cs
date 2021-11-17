using PendleCodeMonkey.MC68000EmulatorLib.Enumerations;

namespace PendleCodeMonkey.MC68000EmulatorLib
{
	/// <summary>
	/// Implementation of the <see cref="Helpers"/> static class.
	/// </summary>
	public static class Helpers
	{
		/// <summary>
		///	Extract the effective address mode from the supplied opcode value.
		/// </summary>
		/// <remarks>
		/// This should be called to retrieve address mode values that are located in
		/// bits 0 to 5 of the opcode.
		/// </remarks>
		/// <param name="opcode">The opcode from which the effective address mode should be extracted.</param>
		/// <returns>The extracted effective address mode value.</returns>
		public static byte GetEAMode(ushort opcode) => (byte)(opcode & 0x003F);

		/// <summary>
		///	Extract the effective address mode from the supplied opcode value.
		/// </summary>
		/// <remarks>
		/// This helper method extracts 'reversed' effective address mode info from a supplied opcode (for
		/// example, as is used to specify the destination effective address mode in a MOVE instruction).
		/// This should be called to retrieve address mode values that are located in
		/// bits 6 to 11 of the opcode.
		/// </remarks>
		/// <param name="opcode">The opcode from which the effective address mode should be extracted.</param>
		/// <returns>The extracted effective address mode value.</returns>
		public static byte GetReversedEAMode(ushort opcode) => (byte)(((opcode & 0x01C0) >> 3) | (opcode & 0x0E00) >> 9);

		/// <summary>
		///	Extract the operation size (byte, word, or long) from the supplied opcode value.
		/// </summary>
		/// <remarks>
		/// This should be called to retrieve operation size values that are located in
		/// bits 6 and 7 of the opcode.
		/// </remarks>
		/// <param name="opcode">The opcode from which the operation size should be extracted.</param>
		/// <returns>The extracted operation size value.</returns>
		public static OpSize GetOpSize(ushort opcode)
		{
			return (OpSize)((opcode & 0x00C0) >> 6);
		}

		/// <summary>
		/// Retrieve a bit mask for the specified data size.
		/// </summary>
		/// <remarks>
		/// The returned value can be used to mask out only data of the specified size (e.g. passing a size
		/// of Byte returns a value of 0x000000FF, which can be used to mask out the lower 8 bits - i.e. the lower byte).
		/// </remarks>
		/// <param name="size">The data size (Byte, Word, or Long).</param>
		/// <returns>The bit mask for the specified data size.</returns>
		public static uint SizeMask(OpSize size)
		{
			return size switch
			{
				OpSize.Byte => 0x000000FF,
				OpSize.Long => 0xFFFFFFFF,
				_ => 0x0000FFFF,
			};
		}

		/// <summary>
		/// Retrieve a value containing only the most significant bit for the specified data size.
		/// </summary>
		/// <param name="size">The data size (Byte, Word, or Long).</param>
		/// <returns>A value containing only the most significant bit for the specified data size.</returns>
		public static uint SizeMSB(OpSize size)
		{
			return size switch
			{
				OpSize.Byte => 0x00000080,
				OpSize.Long => 0x80000000,
				_ => 0x00008000,
			};
		}

		/// <summary>
		/// Retrieve the specified sized value as a 32-bit sign-extended integer.
		/// </summary>
		/// <param name="value">The value to be sign-extended.</param>
		/// <param name="size">The data size (Byte, Word, or Long).</param>
		/// <returns>The 32-bit sign-extended integer.</returns>
		public static int SignExtendValue(uint value, OpSize size = OpSize.Word)
		{
			int newValue = (int)value;
			switch (size)
			{
				case OpSize.Byte:
					if ((value & 0x80) == 0x80)
					{
						newValue = (int)(newValue | 0xFFFFFF00);
					}
					else
					{
						newValue &= 0x000000ff;
					}
					break;
				case OpSize.Long:
					// No need to do anything if value is a long
					break;
				default:
					if ((value & 0x8000) == 0x8000)
					{
						newValue = (int)(newValue | 0xFFFF0000);
					}
					else
					{
						newValue &= 0x0000ffff;
					}
					break;
			}

			return newValue;
		}

		/// <summary>
		/// Raise an exception as a result of a failure during the execution of a 68000 operation (a 68000 processor TRAP).
		/// </summary>
		/// <param name="vector">The 68000 TRAP vector value.</param>
		public static void RaiseTRAPException(ushort vector)
		{
			throw new TrapException(vector);
		}

		/// <summary>
		/// Raise an exception as a result of a failure during the execution of a 68000 operation (a 68000 processor TRAP).
		/// </summary>
		/// <param name="vector">An enumerated 68000 TRAP vector value.</param>
		public static void RaiseTRAPException(TrapVector vector)
		{
			RaiseTRAPException((ushort)vector);
		}

	}
}
