using PendleCodeMonkey.MC68000EmulatorLib;
using PendleCodeMonkey.MC68000EmulatorLib.Enumerations;
using System;
using Xunit;

namespace PendleCodeMonkey.MC68000Emulator.Tests
{
	public class InstructionDecoderTests
	{
		[Fact]
		public void NewDecoder_ShouldNotBeNull()
		{
			InstructionDecoder decoder = new InstructionDecoder(new Machine());

			Assert.NotNull(decoder);
		}

		[Fact]
		public void ReadNextPCByte_ShouldReturnValueWhenPCIsWithinLoadedData()
		{
			Machine machine = new Machine();
			InstructionDecoder decoder = new InstructionDecoder(machine);
			var _ = machine.LoadExecutableData(new byte[] { 1, 2, 3, 4, 5, 6 }, 0x2000);

			var value = decoder.ReadNextPCByte();

			Assert.Equal(1, value);
		}

		[Fact]
		public void ReadNextPCByte_ShouldThrowExceptionWhenPCIsPassedEndOfLoadedData()
		{
			Machine machine = new Machine();
			InstructionDecoder decoder = new InstructionDecoder(machine);
			var _ = machine.LoadExecutableData(new byte[] { 1, 2, 3, 4, 5, 6 }, 0x2000);
			machine.CPU.PC = 0x2008;

			Assert.Throws<InvalidOperationException>(() => decoder.ReadNextPCByte());
		}

		[Fact]
		public void ReadNextPCWord_ShouldReturnValueWhenPCIsWithinLoadedData()
		{
			Machine machine = new Machine();
			InstructionDecoder decoder = new InstructionDecoder(machine);
			var _ = machine.LoadExecutableData(new byte[] { 1, 2, 3, 4, 5, 6 }, 0x2000);

			var value = decoder.ReadNextPCWord();

			Assert.Equal(0x0102, value);
		}

		[Fact]
		public void ReadNextPCWord_ShouldThrowExceptionWhenPCIsPassedEndOfLoadedData()
		{
			Machine machine = new Machine();
			InstructionDecoder decoder = new InstructionDecoder(machine);
			var _ = machine.LoadExecutableData(new byte[] { 1, 2, 3, 4, 5, 6 }, 0x2000);
			machine.CPU.PC = 0x2008;

			Assert.Throws<InvalidOperationException>(() => decoder.ReadNextPCWord());
		}

		[Fact]
		public void FetchInstruction_ShouldFetchValidInstruction()
		{
			Machine machine = new Machine();
			InstructionDecoder decoder = new InstructionDecoder(machine);
			var _ = machine.LoadExecutableData(new byte[] { 0x30, 0x3C, 0x00, 0x32 }, 0x2000);		// instruction is: move.w #50,d0

			var instruction = decoder.FetchInstruction();

			Assert.NotNull(instruction);
			Assert.Equal(0x303C, instruction.Opcode);
			Assert.Equal(OpSize.Word, instruction.Size);
			Assert.Equal((byte)AddrMode.Immediate, instruction.SourceAddrMode.Value);
			Assert.Equal(0x0032, instruction.SourceExtWord1.Value);
			Assert.Equal((byte)0, instruction.DestAddrMode.Value);
			Assert.Equal(OpHandlerID.MOVE, instruction.Info.HandlerID);
		}

	}
}
