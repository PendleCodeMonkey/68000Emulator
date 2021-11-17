using PendleCodeMonkey.MC68000EmulatorLib.Enumerations;
using System.Collections.Generic;

namespace PendleCodeMonkey.MC68000EmulatorLib
{
	/// <summary>
	/// Implementation of the <see cref="OpcodeDecoder"/> class.
	/// </summary>
	internal class OpcodeDecoder
	{
		/// <summary>
		/// Dictionary of instructions.
		/// </summary>
		internal static Dictionary<byte, List<InstructionInfo>> Instructions
		{ get; } = new Dictionary<byte, List<InstructionInfo>>()
		{
			{
				0x00, new List<InstructionInfo>()
				{
					{ new InstructionInfo(0b0000000000111100, 0b1111111111111111, "ORItoCCR", OpHandlerID.ORItoCCR) },
					{ new InstructionInfo(0b0000000001111100, 0b1111111111111111, "ORItoSR", OpHandlerID.ORItoSR) },
					{ new InstructionInfo(0b0000000000000000, 0b1111111100000000, "ORI", OpHandlerID.ORI) },
					{ new InstructionInfo(0b0000001000111100, 0b1111111111111111, "ANDItoCCR", OpHandlerID.ANDItoCCR) },
					{ new InstructionInfo(0b0000001001111100, 0b1111111111111111, "ANDItoSR", OpHandlerID.ANDItoSR) },
					{ new InstructionInfo(0b0000001000000000, 0b1111111100000000, "ANDI", OpHandlerID.ANDI) },
					{ new InstructionInfo(0b0000010000000000, 0b1111111100000000, "SUBI", OpHandlerID.SUBI) },
					{ new InstructionInfo(0b0000011000000000, 0b1111111100000000, "ADDI", OpHandlerID.ADDI) },
					{ new InstructionInfo(0b0000101000111100, 0b1111111111111111, "EORItoCCR", OpHandlerID.EORItoCCR) },
					{ new InstructionInfo(0b0000101001111100, 0b1111111111111111, "EORItoSR", OpHandlerID.EORItoSR) },
					{ new InstructionInfo(0b0000101000000000, 0b1111111100000000, "EORI", OpHandlerID.EORI) },
					{ new InstructionInfo(0b0000110000000000, 0b1111111100000000, "CMPI", OpHandlerID.CMPI) },
					{ new InstructionInfo(0b0000000100001000, 0b1111000100111000, "MOVEP", OpHandlerID.MOVEP) },
					{ new InstructionInfo(0b0000100000000000, 0b1111111111000000, "BTST", OpHandlerID.BTST) },
					{ new InstructionInfo(0b0000100001000000, 0b1111111111000000, "BCHG", OpHandlerID.BCHG) },
					{ new InstructionInfo(0b0000100010000000, 0b1111111111000000, "BCLR", OpHandlerID.BCLR) },
					{ new InstructionInfo(0b0000100011000000, 0b1111111111000000, "BSET", OpHandlerID.BSET) },
					{ new InstructionInfo(0b0000000100000000, 0b1111000111000000, "BTST", OpHandlerID.BTST) },
					{ new InstructionInfo(0b0000000101000000, 0b1111000111000000, "BCHG", OpHandlerID.BCHG) },
					{ new InstructionInfo(0b0000000110000000, 0b1111000111000000, "BCLR", OpHandlerID.BCLR) },
					{ new InstructionInfo(0b0000000111000000, 0b1111000111000000, "BSET", OpHandlerID.BSET) }
				}
			},
			{
				0x01, new List<InstructionInfo>()
				{
					{ new InstructionInfo(0b0001000000000000, 0b1111000000000000, "MOVE", OpHandlerID.MOVE) }
				}
			},
			{
				0x02, new List<InstructionInfo>()
				{
					{ new InstructionInfo(0b0010000001000000, 0b1111000111000000, "MOVEA", OpHandlerID.MOVEA) },
					{ new InstructionInfo(0b0010000000000000, 0b1111000000000000, "MOVE", OpHandlerID.MOVE) }
				}
			},
			{
				0x03, new List<InstructionInfo>()
				{
					{ new InstructionInfo(0b0011000001000000, 0b1111000111000000, "MOVEA", OpHandlerID.MOVEA) },
					{ new InstructionInfo(0b0011000000000000, 0b1111000000000000, "MOVE", OpHandlerID.MOVE) }
				}
			},
			{
				0x04, new List<InstructionInfo>()
				{
					{ new InstructionInfo(0b0100000011000000, 0b1111111111000000, "MOVEfromSR", OpHandlerID.MOVEfromSR) },
					{ new InstructionInfo(0b0100010011000000, 0b1111111111000000, "MOVEtoCCR", OpHandlerID.MOVEtoCCR) },
					{ new InstructionInfo(0b0100011011000000, 0b1111111111000000, "MOVEtoSR", OpHandlerID.MOVEtoSR) },
					{ new InstructionInfo(0b0100000000000000, 0b1111111100000000, "NEGX", OpHandlerID.NEGX) },
					{ new InstructionInfo(0b0100001000000000, 0b1111111100000000, "CLR", OpHandlerID.CLR) },
					{ new InstructionInfo(0b0100010000000000, 0b1111111100000000, "NEG", OpHandlerID.NEG) },
					{ new InstructionInfo(0b0100011000000000, 0b1111111100000000, "NOT", OpHandlerID.NOT) },
					{ new InstructionInfo(0b0100100010000000, 0b1111111110111000, "EXT", OpHandlerID.EXT) },
					{ new InstructionInfo(0b0100100000000000, 0b1111111111000000, "NBCD", OpHandlerID.NBCD) },
					{ new InstructionInfo(0b0100100001000000, 0b1111111111111000, "SWAP", OpHandlerID.SWAP) },
					{ new InstructionInfo(0b0100100001000000, 0b1111111111000000, "PEA", OpHandlerID.PEA) },
					{ new InstructionInfo(0b0100101011111100, 0b1111111111111111, "ILLEGAL", OpHandlerID.ILLEGAL) },
					{ new InstructionInfo(0b0100101011000000, 0b1111111111000000, "TAS", OpHandlerID.TAS) },
					{ new InstructionInfo(0b0100101000000000, 0b1111111100000000, "TST", OpHandlerID.TST) },
					{ new InstructionInfo(0b0100111001000000, 0b1111111111110000, "TRAP", OpHandlerID.TRAP) },
					{ new InstructionInfo(0b0100111001010000, 0b1111111111111000, "LINK", OpHandlerID.LINK) },
					{ new InstructionInfo(0b0100111001011000, 0b1111111111111000, "UNLK", OpHandlerID.UNLK) },
					{ new InstructionInfo(0b0100111001100000, 0b1111111111110000, "MOVEUSP", OpHandlerID.MOVEUSP) },
					{ new InstructionInfo(0b0100111001110000, 0b1111111111111111, "RESET", OpHandlerID.RESET) },
					{ new InstructionInfo(0b0100111001110001, 0b1111111111111111, "NOP", OpHandlerID.NOP) },
					{ new InstructionInfo(0b0100111001110010, 0b1111111111111111, "STOP", OpHandlerID.STOP) },
					{ new InstructionInfo(0b0100111001110011, 0b1111111111111111, "RTE", OpHandlerID.RTE) },
					{ new InstructionInfo(0b0100111001110101, 0b1111111111111111, "RTS", OpHandlerID.RTS) },
					{ new InstructionInfo(0b0100111001110110, 0b1111111111111111, "TRAPV", OpHandlerID.TRAPV) },
					{ new InstructionInfo(0b0100111001110111, 0b1111111111111111, "RTR", OpHandlerID.RTR) },
					{ new InstructionInfo(0b0100111010000000, 0b1111111111000000, "JSR", OpHandlerID.JSR) },
					{ new InstructionInfo(0b0100111011000000, 0b1111111111000000, "JMP", OpHandlerID.JMP) },
					{ new InstructionInfo(0b0100100010000000, 0b1111101110000000, "MOVEM", OpHandlerID.MOVEM) },
					{ new InstructionInfo(0b0100000110000000, 0b1111000111000000, "CHK", OpHandlerID.CHK) },
					{ new InstructionInfo(0b0100000111000000, 0b1111000111000000, "LEA", OpHandlerID.LEA) }
				}
			},
			{
				0x05, new List<InstructionInfo>()
				{
					{ new InstructionInfo(0b0101000011001000, 0b1111000011111000, "DBcc", OpHandlerID.DBcc) },
					{ new InstructionInfo(0b0101000011000000, 0b1111000011000000, "Scc", OpHandlerID.Scc) },
					{ new InstructionInfo(0b0101000000000000, 0b1111000100000000, "ADDQ", OpHandlerID.ADDQ) },
					{ new InstructionInfo(0b0101000100000000, 0b1111000100000000, "SUBQ", OpHandlerID.SUBQ) }
			   }
			},
			{
				0x06, new List<InstructionInfo>()
				{
					{ new InstructionInfo(0b0110000000000000, 0b1111111100000000, "BRA", OpHandlerID.BRA) },
					{ new InstructionInfo(0b0110000100000000, 0b1111111100000000, "BSR", OpHandlerID.BSR) },
					{ new InstructionInfo(0b0110000000000000, 0b1111000000000000, "Bcc", OpHandlerID.Bcc) }
			   }
			},
			{
				0x07, new List<InstructionInfo>()
				{
					{ new InstructionInfo(0b0111000000000000, 0b1111000100000000, "MOVEQ", OpHandlerID.MOVEQ) }
			   }
			},
			{
				0x08, new List<InstructionInfo>()
				{
					{ new InstructionInfo(0b1000000011000000, 0b1111000111000000, "DIVU", OpHandlerID.DIVU) },
					{ new InstructionInfo(0b1000000111000000, 0b1111000111000000, "DIVS", OpHandlerID.DIVS) },
					{ new InstructionInfo(0b1000000100000000, 0b1111000111110000, "SBCD", OpHandlerID.SBCD) },
					{ new InstructionInfo(0b1000000000000000, 0b1111000000000000, "OR", OpHandlerID.OR) }
			   }
			},
			{
				0x09, new List<InstructionInfo>()
				{
					{ new InstructionInfo(0b1001000011000000, 0b1111000011000000, "SUBA", OpHandlerID.SUBA) },
					{ new InstructionInfo(0b1001000100000000, 0b1111000100110000, "SUBX", OpHandlerID.SUBX) },
					{ new InstructionInfo(0b1001000000000000, 0b1111000000000000, "SUB", OpHandlerID.SUB) }
			   }
			},
			{
				0x0B, new List<InstructionInfo>()
				{
					{ new InstructionInfo(0b1011000011000000, 0b1111000011000000, "CMPA", OpHandlerID.CMPA) },
					{ new InstructionInfo(0b1011000100001000, 0b1111000100111000, "CMPM", OpHandlerID.CMPM) },
					{ new InstructionInfo(0b1011000100000000, 0b1111000100000000, "EOR", OpHandlerID.EOR) },
					{ new InstructionInfo(0b1011000000000000, 0b1111000100000000, "CMP", OpHandlerID.CMP) }
			   }
			},
			{
				0x0C, new List<InstructionInfo>()
				{
					{ new InstructionInfo(0b1100000011000000, 0b1111000111000000, "MULU", OpHandlerID.MULU) },
					{ new InstructionInfo(0b1100000111000000, 0b1111000111000000, "MULS", OpHandlerID.MULS) },
					{ new InstructionInfo(0b1100000100000000, 0b1111000111110000, "ABCD", OpHandlerID.ABCD) },
					{ new InstructionInfo(0b1100000100000000, 0b1111000100110000, "EXG", OpHandlerID.EXG) },
					{ new InstructionInfo(0b1100000000000000, 0b1111000000000000, "AND", OpHandlerID.AND) }
			   }
			},
			{
				0x0D, new List<InstructionInfo>()
				{
					{ new InstructionInfo(0b1101000011000000, 0b1111000011000000, "ADDA", OpHandlerID.ADDA) },
					{ new InstructionInfo(0b1101000100000000, 0b1111000100110000, "ADDX", OpHandlerID.ADDX) },
					{ new InstructionInfo(0b1101000000000000, 0b1111000000000000, "ADD", OpHandlerID.ADD) }
			   }
			},
			{
				0x0E, new List<InstructionInfo>()
				{
					{ new InstructionInfo(0b1110000011000000, 0b1111111111000000, "ASR", OpHandlerID.ASR) },
					{ new InstructionInfo(0b1110000111000000, 0b1111111111000000, "ASL", OpHandlerID.ASL) },
					{ new InstructionInfo(0b1110001011000000, 0b1111111111000000, "LSR", OpHandlerID.LSR) },
					{ new InstructionInfo(0b1110001111000000, 0b1111111111000000, "LSL", OpHandlerID.LSL) },
					{ new InstructionInfo(0b1110010011000000, 0b1111111111000000, "ROXR", OpHandlerID.ROXR) },
					{ new InstructionInfo(0b1110010111000000, 0b1111111111000000, "ROXL", OpHandlerID.ROXL) },
					{ new InstructionInfo(0b1110011011000000, 0b1111111111000000, "ROR", OpHandlerID.ROR) },
					{ new InstructionInfo(0b1110011111000000, 0b1111111111000000, "ROL", OpHandlerID.ROL) },
					{ new InstructionInfo(0b1110000000000000, 0b1111000100011000, "ASR", OpHandlerID.ASR) },
					{ new InstructionInfo(0b1110000100000000, 0b1111000100011000, "ASL", OpHandlerID.ASL) },
					{ new InstructionInfo(0b1110000000001000, 0b1111000100011000, "LSR", OpHandlerID.LSR) },
					{ new InstructionInfo(0b1110000100001000, 0b1111000100011000, "LSL", OpHandlerID.LSL) },
					{ new InstructionInfo(0b1110000000010000, 0b1111000100011000, "ROXR", OpHandlerID.ROXR) },
					{ new InstructionInfo(0b1110000100010000, 0b1111000100011000, "ROXL", OpHandlerID.ROXL) },
					{ new InstructionInfo(0b1110000000011000, 0b1111000100011000, "ROR", OpHandlerID.ROR) },
					{ new InstructionInfo(0b1110000100011000, 0b1111000100011000, "ROL", OpHandlerID.ROL) }
			   }
			}
		};

		/// <summary>
		/// Attempt to retrieve an <see cref="InstructionInfo"/> instance corresponding to
		/// the specified opcode value.
		/// </summary>
		/// <param name="opcode">The 16-bit instruction opcode value.</param>
		/// <returns>
		/// The <see cref="InstructionInfo"/> instance that corresponds to the specified
		/// opcode value (or null if the opcode is invalid).
		/// </returns>
		internal InstructionInfo GetInstructionInfo(ushort opcode)
		{
			byte group = (byte)((opcode & 0xF000) >> 12);
			if (Instructions.ContainsKey(group))
			{
				foreach (var inst in Instructions[group])
				{
					if ((opcode & inst.OpcodeMask) == inst.OpcodeValue)
					{
						return inst;
					}
				}
			}

			return null;
		}
	}
}
