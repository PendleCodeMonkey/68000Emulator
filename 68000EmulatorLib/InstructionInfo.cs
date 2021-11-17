using PendleCodeMonkey.MC68000EmulatorLib.Enumerations;

namespace PendleCodeMonkey.MC68000EmulatorLib
{
	class InstructionInfo
	{
		public InstructionInfo(ushort value, ushort mask, string mnemonic, OpHandlerID handlerID)
		{
			OpcodeValue = value;
			OpcodeMask = mask;
			Mnemonic = mnemonic;
			HandlerID = handlerID;
		}

		internal ushort OpcodeValue { get; set; }

		internal ushort OpcodeMask { get; set; }

		internal string Mnemonic { get; set; }

		internal OpHandlerID HandlerID { get; set; }
	}
}
