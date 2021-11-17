using PendleCodeMonkey.MC68000EmulatorLib.Enumerations;

namespace PendleCodeMonkey.MC68000EmulatorLib
{
	/// <summary>
	/// Implementation of the <see cref="Instruction"/> class.
	/// </summary>
	class Instruction
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Instruction"/> class.
		/// </summary>
		/// <param name="opcode">The 8-bit opcode value for the instruction.</param>
		/// <param name="info">An <see cref="InstructionInfo"/> instance giving info about the instruction.</param>
		/// <param name="byteOperand">The value of the 8-bit operand (if any).</param>
		/// <param name="wordOperand">The value of the 16-bit operand (if any).</param>
		/// <param name="displacement">The value of the 8-bit displacement (if any).</param>
		internal Instruction(ushort opcode, InstructionInfo info, OpSize? size = null, byte? srcAddrMode = null, ushort? srcExtWord1 = null, ushort? srcExtWord2 = null,
							byte? destAddrMode = null, ushort? destExtWord1 = null, ushort? destExtWord2 = null)
		{
			Opcode = opcode;
			Info = info;
			Size = size;
			SourceAddrMode = srcAddrMode;
			SourceExtWord1 = srcExtWord1;
			SourceExtWord2 = srcExtWord2;
			DestAddrMode = destAddrMode;
			DestExtWord1 = destExtWord1;
			DestExtWord2 = destExtWord2;
		}


		/// <summary>
		/// The 16-bit opcode value for this instruction.
		/// </summary>
		internal ushort Opcode { get; set; }

		/// <summary>
		/// An <see cref="InstructionInfo"/> instance giving info about the instruction.
		/// </summary>
		internal InstructionInfo Info { get; set; }

		/// <summary>
		/// The size of the operation [byte, word, or long] (if any).
		/// </summary>
		internal OpSize? Size { get; set; }

		/// <summary>
		/// The value of the source addressing mode (if any).
		/// </summary>
		internal byte? SourceAddrMode { get; set; }

		/// <summary>
		/// The value of the first source extension word (if any).
		/// </summary>
		internal ushort? SourceExtWord1 { get; set; }

		/// <summary>
		/// The value of the second source extension word (if any).
		/// </summary>
		internal ushort? SourceExtWord2 { get; set; }

		/// <summary>
		/// The value of the destination addressing mode (if any).
		/// </summary>
		internal byte? DestAddrMode { get; set; }

		/// <summary>
		/// The value of the first destination extension word (if any).
		/// </summary>
		internal ushort? DestExtWord1 { get; set; }

		/// <summary>
		/// The value of the second destination extension word (if any).
		/// </summary>
		internal ushort? DestExtWord2 { get; set; }
	}
}
