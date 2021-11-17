using System;

namespace PendleCodeMonkey.MC68000EmulatorLib.Enumerations
{
	/// <summary>
	/// Enumeration of the processor flags.
	/// </summary>
	[Flags]
	public enum SRFlags : ushort
	{
		Carry = 0x01,
		Overflow = 0x02,
		Zero = 0x04,
		Negative = 0x08,
		Extend = 0x10,
		SupervisorMode = 0x2000,
		TraceMode = 0x8000
	};

	/// <summary>
	/// Enumeration of the possible operation sizes (byte, word, or long)
	/// </summary>
	public enum OpSize : byte
	{
		Byte = 0x00,
		Word = 0x01,
		Long = 0x02
	};

	/// <summary>
	/// Enumeration of the two effective address types (source or destination)
	/// </summary>
	public enum EAType : byte
	{
		Source = 0x00,
		Destination = 0x01
	};

	/// <summary>
	/// Enumeration of the BCD operation types - Addition (for ABCD instruction) or Subtraction (for SBCD instruction)
	/// </summary>
	public enum BCDOperation : byte
	{
		Addition = 0x00,
		Subtraction = 0x01
	};

	/// <summary>
	/// Enumeration of the possible addressing modes
	/// </summary>
	public enum AddrMode : byte
	{
		DataRegister = 0x00,		// Dn
		AddressRegister = 0x08,		// An
		Address = 0x10,				// (An)
		AddressPostInc = 0x18,		// (An)+
		AddressPreDec = 0x20,		// -(An)
		AddressDisp = 0x28,			// (d,An)
		AddressIndex = 0x30,		// (d,An,Xn)
		AbsShort = 0x38,			// (xxx).W
		AbsLong = 0x39,				// (xxx).L
		PCDisp = 0x3A,				// (d,PC)
		PCIndex = 0x3B,				// (d,PC,Xn)
		Immediate = 0x3C			// #<data>
	};

	/// <summary>
	/// Enumeration of the possible condition values (for branch instructions, etc.)
	/// </summary>
	public enum Condition : byte
	{
		T = 0x00,		// True
		F = 0x01,		// False
		HI = 0x02,		// Higher
		LS = 0x03,		// Lower or Same
		CC = 0x04,		// Carry Clear
		CS = 0x05,		// Carry Set
		NE = 0x06,		// Not Equal
		EQ = 0x07,		// Equal
		VC = 0x08,		// Overflow Clear
		VS = 0x09,		// Overflow Set
		PL = 0x0A,		// Plus
		MI = 0x0B,		// Minus
		GE = 0x0C,		// Greater or Equal
		LT = 0x0D,		// Less Than
		GT = 0x0E,		// Greater Than
		LE = 0x0F		// Less or Equal
	};

	/// <summary>
	/// Enumeration of TRAP vector values.
	/// </summary>
	public enum TrapVector : ushort
	{
		BusError = 2,
		AddressError = 3,
		IllegalInstruction = 4,
		DivideByZero = 5,
		CHKInstruction = 6,
		TRAPVInstruction = 7,
		PrivilegeViolation = 8
	};

	/// <summary>
	/// Enumeration of Operation Handler identifiers.
	/// </summary>
	public enum OpHandlerID : byte
	{
		NONE,

		ORItoCCR,
		ORItoSR,
		ORI,
		ANDItoCCR,
		ANDItoSR,
		ANDI,
		SUBI,
		ADDI,
		EORItoCCR,
		EORItoSR,
		EORI,
		CMPI,
		BTST,
		BCHG,
		BCLR,
		BSET,
		MOVEP,
		MOVE,
		MOVEA,
		MOVEfromSR,
		MOVEtoCCR,
		MOVEtoSR,
		NEGX,
		CLR,
		NEG,
		NOT,
		EXT,
		NBCD,
		SWAP,
		PEA,
		ILLEGAL,
		TAS,
		TST,
		TRAP,
		LINK,
		UNLK,
		MOVEUSP,
		RESET,
		NOP,
		STOP,
		RTE,
		RTS,
		TRAPV,
		RTR,
		JSR,
		JMP,
		MOVEM,
		CHK,
		LEA,
		DBcc,
		Scc,
		ADDQ,
		SUBQ,
		BRA,
		BSR,
		Bcc,
		MOVEQ,
		DIVU,
		DIVS,
		SBCD,
		OR,
		SUBA,
		SUBX,
		SUB,
		CMPA,
		CMPM,
		EOR,
		CMP,
		MULU,
		MULS,
		ABCD,
		EXG,
		AND,
		ADDA,
		ADDX,
		ADD,
		ASR,
		ASL,
		LSR,
		LSL,
		ROXR,
		ROXL,
		ROR,
		ROL
	};
}
