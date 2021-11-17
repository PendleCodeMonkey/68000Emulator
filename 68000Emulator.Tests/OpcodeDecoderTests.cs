using PendleCodeMonkey.MC68000EmulatorLib;
using PendleCodeMonkey.MC68000EmulatorLib.Enumerations;
using Xunit;

namespace PendleCodeMonkey.MC68000Emulator.Tests
{
	public class OpcodeDecoderTests
	{
		[Theory]
		[InlineData(0x003C, OpHandlerID.ORItoCCR)]
		[InlineData(0x007C, OpHandlerID.ORItoSR)]
		[InlineData(0x0041, OpHandlerID.ORI)]
		[InlineData(0x023C, OpHandlerID.ANDItoCCR)]
		[InlineData(0x027C, OpHandlerID.ANDItoSR)]
		[InlineData(0x0242, OpHandlerID.ANDI)]
		[InlineData(0x0441, OpHandlerID.SUBI)]
		[InlineData(0x0642, OpHandlerID.ADDI)]
		[InlineData(0x0A3C, OpHandlerID.EORItoCCR)]
		[InlineData(0x0A7C, OpHandlerID.EORItoSR)]
		[InlineData(0x0A42, OpHandlerID.EORI)]
		[InlineData(0x0C43, OpHandlerID.CMPI)]
		[InlineData(0x0800, OpHandlerID.BTST)]
		[InlineData(0x0111, OpHandlerID.BTST)]
		[InlineData(0x0840, OpHandlerID.BCHG)]
		[InlineData(0x0151, OpHandlerID.BCHG)]
		[InlineData(0x0880, OpHandlerID.BCLR)]
		[InlineData(0x0191, OpHandlerID.BCLR)]
		[InlineData(0x08C0, OpHandlerID.BSET)]
		[InlineData(0x01D1, OpHandlerID.BSET)]
		[InlineData(0x0189, OpHandlerID.MOVEP)]
		[InlineData(0x1280, OpHandlerID.MOVE)]
		[InlineData(0x3280, OpHandlerID.MOVE)]
		[InlineData(0x2280, OpHandlerID.MOVE)]
		[InlineData(0x3250, OpHandlerID.MOVEA)]
		[InlineData(0x40C0, OpHandlerID.MOVEfromSR)]
		[InlineData(0x44FC, OpHandlerID.MOVEtoCCR)]
		[InlineData(0x46FC, OpHandlerID.MOVEtoSR)]
		[InlineData(0x4041, OpHandlerID.NEGX)]
		[InlineData(0x4242, OpHandlerID.CLR)]
		[InlineData(0x4443, OpHandlerID.NEG)]
		[InlineData(0x4644, OpHandlerID.NOT)]
		[InlineData(0x4885, OpHandlerID.EXT)]
		[InlineData(0x4800, OpHandlerID.NBCD)]
		[InlineData(0x4841, OpHandlerID.SWAP)]
		[InlineData(0x4851, OpHandlerID.PEA)]
		[InlineData(0x4AFC, OpHandlerID.ILLEGAL)]
		[InlineData(0x4AC0, OpHandlerID.TAS)]
		[InlineData(0x4A42, OpHandlerID.TST)]
		[InlineData(0x4E44, OpHandlerID.TRAP)]
		[InlineData(0x4E50, OpHandlerID.LINK)]
		[InlineData(0x4E58, OpHandlerID.UNLK)]
		[InlineData(0x4E62, OpHandlerID.MOVEUSP)]
		[InlineData(0x4E70, OpHandlerID.RESET)]
		[InlineData(0x4E71, OpHandlerID.NOP)]
		[InlineData(0x4E72, OpHandlerID.STOP)]
		[InlineData(0x4E73, OpHandlerID.RTE)]
		[InlineData(0x4E75, OpHandlerID.RTS)]
		[InlineData(0x4E76, OpHandlerID.TRAPV)]
		[InlineData(0x4E77, OpHandlerID.RTR)]
		[InlineData(0x4EB8, OpHandlerID.JSR)]
		[InlineData(0x4EF8, OpHandlerID.JMP)]
		[InlineData(0x4891, OpHandlerID.MOVEM)]
		[InlineData(0x4391, OpHandlerID.CHK)]
		[InlineData(0x43D2, OpHandlerID.LEA)]
		[InlineData(0x57CC, OpHandlerID.DBcc)]
		[InlineData(0x56C0, OpHandlerID.Scc)]
		[InlineData(0x5C48, OpHandlerID.ADDQ)]
		[InlineData(0x5940, OpHandlerID.SUBQ)]
		[InlineData(0x6000, OpHandlerID.BRA)]
		[InlineData(0x6100, OpHandlerID.BSR)]
		[InlineData(0x6900, OpHandlerID.Bcc)]
		[InlineData(0x7004, OpHandlerID.MOVEQ)]
		[InlineData(0x82C0, OpHandlerID.DIVU)]
		[InlineData(0x87C2, OpHandlerID.DIVS)]
		[InlineData(0x8300, OpHandlerID.SBCD)]
		[InlineData(0x8242, OpHandlerID.OR)]
		[InlineData(0x92C0, OpHandlerID.SUBA)]
		[InlineData(0x9742, OpHandlerID.SUBX)]
		[InlineData(0x9A44, OpHandlerID.SUB)]
		[InlineData(0xB2C1, OpHandlerID.CMPA)]
		[InlineData(0xB348, OpHandlerID.CMPM)]
		[InlineData(0xB141, OpHandlerID.EOR)]
		[InlineData(0xB441, OpHandlerID.CMP)]
		[InlineData(0xC2C0, OpHandlerID.MULU)]
		[InlineData(0xC7C2, OpHandlerID.MULS)]
		[InlineData(0xCD05, OpHandlerID.ABCD)]
		[InlineData(0xC141, OpHandlerID.EXG)]
		[InlineData(0xC240, OpHandlerID.AND)]
		[InlineData(0xD2C0, OpHandlerID.ADDA)]
		[InlineData(0xD742, OpHandlerID.ADDX)]
		[InlineData(0xDA44, OpHandlerID.ADD)]
		[InlineData(0xE640, OpHandlerID.ASR)]
		[InlineData(0xE262, OpHandlerID.ASR)]
		[InlineData(0xE0D1, OpHandlerID.ASR)]
		[InlineData(0xE740, OpHandlerID.ASL)]
		[InlineData(0xE362, OpHandlerID.ASL)]
		[InlineData(0xE1D2, OpHandlerID.ASL)]
		[InlineData(0xE648, OpHandlerID.LSR)]
		[InlineData(0xE26A, OpHandlerID.LSR)]
		[InlineData(0xE2D1, OpHandlerID.LSR)]
		[InlineData(0xE748, OpHandlerID.LSL)]
		[InlineData(0xE36A, OpHandlerID.LSL)]
		[InlineData(0xE3D2, OpHandlerID.LSL)]
		[InlineData(0xE658, OpHandlerID.ROR)]
		[InlineData(0xE27A, OpHandlerID.ROR)]
		[InlineData(0xE6D1, OpHandlerID.ROR)]
		[InlineData(0xE758, OpHandlerID.ROL)]
		[InlineData(0xE37A, OpHandlerID.ROL)]
		[InlineData(0xE7D2, OpHandlerID.ROL)]
		[InlineData(0xE650, OpHandlerID.ROXR)]
		[InlineData(0xE272, OpHandlerID.ROXR)]
		[InlineData(0xE4D1, OpHandlerID.ROXR)]
		[InlineData(0xE750, OpHandlerID.ROXL)]
		[InlineData(0xE372, OpHandlerID.ROXL)]
		[InlineData(0xE5D2, OpHandlerID.ROXL)]
		public void GetInstructionInfo_ReturnsCorrectData(ushort opcode, OpHandlerID expectedHandlerID)
		{
			OpcodeDecoder decoder = new OpcodeDecoder();

			var info = decoder.GetInstructionInfo(opcode);

			// Assert
			Assert.NotNull(info);
			Assert.Equal(expectedHandlerID, info.HandlerID);
		}

		[Fact]
		public void GetInstructionInfo_ReturnsNullForInvalidOpcode()
		{
			OpcodeDecoder decoder = new OpcodeDecoder();

			var info = decoder.GetInstructionInfo(0xA000);

			// Assert
			Assert.Null(info);
		}

	}
}
