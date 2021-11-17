using PendleCodeMonkey.MC68000EmulatorLib.Enumerations;
using System;

namespace PendleCodeMonkey.MC68000EmulatorLib
{
	/// <summary>
	/// Implementation of the <see cref="InstructionDecoder"/> class.
	/// </summary>
	internal class InstructionDecoder
	{
		private readonly OpcodeDecoder _handler = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="InstructionDecoder"/> class.
		/// </summary>
		/// <param name="machine">The <see cref="Machine"/> instance for which this object is handling the execution of instructions.</param>
		internal InstructionDecoder(Machine machine)
		{
			Machine = machine ?? throw new ArgumentNullException(nameof(machine));
			_handler = new OpcodeDecoder();
		}

		/// <summary>
		/// Gets or sets the <see cref="Machine"/> instance for which this <see cref="InstructionDecoder"/> instance
		/// is handling the decoding of instructions.
		/// </summary>
		private Machine Machine { get; set; }

		/// <summary>
		/// Return the byte located at the Program Counter, and then increment the Program Counter.
		/// </summary>
		/// <returns>The byte located at the Program Counter.</returns>
		internal byte ReadNextPCByte()
		{
			if (Machine.IsEndOfData)
			{
				throw new InvalidOperationException("Execution has run past the end of the loaded data.");
			}
			byte value = Machine.Memory.ReadByte(Machine.CPU.PC);
			Machine.CPU.IncrementPC(1);
			return value;
		}

		/// <summary>
		/// Return the word located at the Program Counter, and then increment the Program Counter.
		/// </summary>
		/// <returns>The word located at the Program Counter.</returns>
		internal ushort ReadNextPCWord()
		{
			if (Machine.IsEndOfData)
			{
				throw new InvalidOperationException("Execution has run past the end of the loaded data.");
			}
			ushort value = Machine.Memory.ReadWord(Machine.CPU.PC);
			Machine.CPU.IncrementPC(2);
			return value;
		}


		/// <summary>
		/// Fetch the instruction located at the current Program Counter address, incrementing the
		/// Program Counter accordingly.
		/// </summary>
		/// <returns>An <see cref="Instruction"/> instance containing details about the instruction that has been fetched.</returns>
		internal Instruction FetchInstruction()
		{
			// Read the next word (which contains the instruction opcode)
			var value = ReadNextPCWord();

			// Attempt to locate info about this instruction (mainly the ID of the instruction handler)
			var instInfo = _handler.GetInstructionInfo(value);
			if (instInfo != null)
			{
				var inst = GetInstruction(value, instInfo);
				return inst;
			}

			// Return null if this is not a recognised opcode (i.e. an illegal instruction)
			return null;
		}

		/// <summary>
		/// Retrieve instruction details (decoding any operands, etc.)
		/// </summary>
		/// <param name="opcode">The 16-bit opcode value for the instruction.</param>
		/// <param name="instInfo">The <see cref="InstructionInfo"/> instance for the instruction.</param>
		/// <returns>An <see cref="Instruction"/> object containing details of the instruction.</returns>
		private Instruction GetInstruction(ushort opcode, InstructionInfo instInfo)
		{
			byte? sourceEA = null;
			byte? destEA = null;
			OpSize opSize = OpSize.Word;		// Defaults to Word sized operations.
			ushort? srcExt1 = null;
			ushort? srcExt2 = null;
			ushort? destExt1 = null;
			ushort? destExt2 = null;

			switch (instInfo.HandlerID)
			{
				case OpHandlerID.ORItoCCR:
				case OpHandlerID.ANDItoCCR:
				case OpHandlerID.EORItoCCR:
					opSize = OpSize.Byte;
					(srcExt1, srcExt2) = ReadImmediateOperandData(opSize);
					break;
				case OpHandlerID.ORItoSR:
				case OpHandlerID.ANDItoSR:
				case OpHandlerID.EORItoSR:
					(srcExt1, srcExt2) = ReadImmediateOperandData(opSize);
					break;
				case OpHandlerID.ORI:
				case OpHandlerID.ANDI:
				case OpHandlerID.SUBI:
				case OpHandlerID.ADDI:
				case OpHandlerID.EORI:
				case OpHandlerID.CMPI:
					destEA = Helpers.GetEAMode(opcode);
					opSize = Helpers.GetOpSize(opcode);
					(srcExt1, srcExt2) = ReadImmediateOperandData(opSize);
					break;
				case OpHandlerID.BTST:
				case OpHandlerID.BCHG:
				case OpHandlerID.BCLR:
				case OpHandlerID.BSET:
					if ((opcode & 0x0100) == 0)
					{
						// When bit 8 of opcode is zero then the bit number is static (i.e. is in an extension word operand)
						srcExt1 = ReadNextPCWord();
					}
					destEA = Helpers.GetEAMode(opcode);
					break;
				case OpHandlerID.MOVEP:
					// Read the displacement value (which is a word).
					srcExt1 = ReadNextPCWord();
					break;
				case OpHandlerID.MOVEA:
				case OpHandlerID.MOVE:
					sourceEA = Helpers.GetEAMode(opcode);
					destEA = Helpers.GetReversedEAMode(opcode);
					// Get the operation size (which is in an alternative format and must therefore be translated to an OpSize enum value)
					byte size = (byte)((opcode & 0x3000) >> 12);
					opSize = size switch
					{
						0x01 => OpSize.Byte,
						0x02 => OpSize.Long,
						_ => OpSize.Word,
					};
					break;
				case OpHandlerID.MOVEfromSR:
					destEA = Helpers.GetEAMode(opcode);
					break;
				case OpHandlerID.MOVEtoCCR:
					sourceEA = Helpers.GetEAMode(opcode);
					opSize = OpSize.Byte;
					break;
				case OpHandlerID.MOVEtoSR:
					sourceEA = Helpers.GetEAMode(opcode);
					break;
				case OpHandlerID.NEGX:
				case OpHandlerID.CLR:
				case OpHandlerID.NEG:
				case OpHandlerID.NOT:
				case OpHandlerID.TST:
				case OpHandlerID.ADDQ:
				case OpHandlerID.SUBQ:
				case OpHandlerID.OR:
				case OpHandlerID.SUB:
				case OpHandlerID.EOR:
				case OpHandlerID.CMP:
				case OpHandlerID.AND:
				case OpHandlerID.ADD:
					destEA = Helpers.GetEAMode(opcode);
					opSize = Helpers.GetOpSize(opcode);
					break;
				case OpHandlerID.NBCD:
				case OpHandlerID.TAS:
				case OpHandlerID.Scc:
					destEA = Helpers.GetEAMode(opcode);
					opSize = OpSize.Byte;
					break;
				case OpHandlerID.PEA:
					sourceEA = Helpers.GetEAMode(opcode);
					opSize = OpSize.Long;
					break;
				case OpHandlerID.LINK:
				case OpHandlerID.STOP:
				case OpHandlerID.DBcc:
					// Read an extension word which is as follows:
					// LINK - Displacement value.
					// STOP - Immediate operand.
					// DBcc - Displacement value.
					srcExt1 = ReadNextPCWord();
					break;
				case OpHandlerID.JSR:
				case OpHandlerID.JMP:
				case OpHandlerID.LEA:
				case OpHandlerID.CHK:
					sourceEA = Helpers.GetEAMode(opcode);
					break;
				case OpHandlerID.MOVEM:
					// Read an extension word which is the Register List Mask
					srcExt1 = ReadNextPCWord();
					opSize = (opcode & 0x0040) == 0 ? OpSize.Word : OpSize.Long;
					destEA = Helpers.GetEAMode(opcode);
					break;
				case OpHandlerID.BRA:
				case OpHandlerID.BSR:
				case OpHandlerID.Bcc:
					// If the byte displacement value held within the opcode is zero then
					// we're using a 16-bit displacement in the extension word operand.
					if ((opcode & 0x00FF) == 0)
					{
						srcExt1 = ReadNextPCWord();
					}
					break;
				case OpHandlerID.DIVU:
				case OpHandlerID.DIVS:
				case OpHandlerID.MULU:
				case OpHandlerID.MULS:
					sourceEA = Helpers.GetEAMode(opcode);
					opSize = OpSize.Word;
					break;
				case OpHandlerID.SUBA:
				case OpHandlerID.CMPA:
				case OpHandlerID.ADDA:
					sourceEA = Helpers.GetEAMode(opcode);
					opSize = (opcode & 0x0100) == 0 ? OpSize.Word : OpSize.Long;
					break;

				case OpHandlerID.ASL:
				case OpHandlerID.ASR:
				case OpHandlerID.LSL:
				case OpHandlerID.LSR:
				case OpHandlerID.ROXL:
				case OpHandlerID.ROXR:
				case OpHandlerID.ROL:
				case OpHandlerID.ROR:
					// If a memory shift then determine the effective address mode.
					if (((opcode & 0x00C0) >> 6) == 0x03)
					{
						sourceEA = Helpers.GetEAMode(opcode);
						opSize = OpSize.Word;
					}
					else
					{
						opSize = Helpers.GetOpSize(opcode);
					}
					break;

				case OpHandlerID.SUBX:
				case OpHandlerID.ADDX:
				case OpHandlerID.CMPM:
					opSize = Helpers.GetOpSize(opcode);
					break;

				case OpHandlerID.EXT:
				case OpHandlerID.SWAP:
				case OpHandlerID.ILLEGAL:
				case OpHandlerID.TRAP:
				case OpHandlerID.UNLK:
				case OpHandlerID.MOVEUSP:
				case OpHandlerID.RESET:
				case OpHandlerID.NOP:
				case OpHandlerID.RTE:
				case OpHandlerID.RTS:
				case OpHandlerID.TRAPV:
				case OpHandlerID.RTR:
				case OpHandlerID.MOVEQ:
				case OpHandlerID.SBCD:
				case OpHandlerID.ABCD:
				case OpHandlerID.EXG:
					// No additional operand extension words required for these instructions.
					break;
			}

			if (sourceEA.HasValue)
			{
				(srcExt1, srcExt2) = ReadExtensionWordData(sourceEA.Value, opSize);
			}
			if (destEA.HasValue)
			{
				(destExt1, destExt2) = ReadExtensionWordData(destEA.Value, opSize);
			}

			// Construct an Instruction object containing all the required operand info, etc.
			Instruction inst = new Instruction(opcode, instInfo, opSize, sourceEA, srcExt1, srcExt2, destEA, destExt1, destExt2);

			// and return it.
			return inst;
		}

		/// <summary>
		/// Read an immediate operand value from the current Program Counter address.
		/// </summary>
		/// <param name="size">The size of immediate operand data to be read (Word or Long).</param>
		/// <returns>
		/// A tuple containing two extension word values (both of which are optional).
		/// For Long data, both extension word values will be non-null and need to be combined to make the 32-bit operand value.
		/// </returns>
		private (ushort? extWord1, ushort? extWord2) ReadImmediateOperandData(OpSize size)
		{
			ushort ext1 = ReadNextPCWord();
			ushort? ext2 = null;
			if (size == OpSize.Long)
			{
				ext2 = ReadNextPCWord();
			}
			return (ext1, ext2);
		}

		/// <summary>
		/// Read extension word operand data for the specified effective adrdress mode.
		/// </summary>
		/// <param name="ea">The effective address mode value (see Enumerations.AddrMode).</param>
		/// <param name="size">The size of effective address operand data to be read (Word or Long).</param>
		/// <returns>
		/// A tuple containing two extension word values (both of which are optional).
		/// For Long data, both extension word values will be non-null and need to be combined to make the 32-bit operand value.
		/// </returns>
		private (ushort? extWord1, ushort? extWord2) ReadExtensionWordData(byte ea, OpSize size)
		{
			ushort? ext1 = null;
			ushort? ext2 = null;

			switch (ea & 0x38)
			{
				case 0x28:          // Address Register Indirect with Displacement
				case 0x30:          // Address Register Indirect with Index
					ext1 = ReadNextPCWord();
					break;
				case 0x38:
					switch (ea & 0x07)
					{
						case 0x00:  // Absolute Short
						case 0x02:  // PC Relative with Displacement
						case 0x03:  // PC Relative with Index
							ext1 = ReadNextPCWord();
							break;
						case 0x01:  // Absolute Long
							ext1 = ReadNextPCWord();
							ext2 = ReadNextPCWord();
							break;
						case 0x04:  // Immediate
							ext1 = ReadNextPCWord();
							if (size == OpSize.Long)
							{
								ext2 = ReadNextPCWord();
							}
							break;
					}
					break;
				default:
					// Must be one of: Data Register Direct, Address Register Direct, Address Register Indirect,
					// Address Register Indirect with Postincrement, or Address Register Indirect with Predecrement.
					// None of which require extension word data.
					break;
			}
			return (ext1, ext2);
		}
	}
}