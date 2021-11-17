using PendleCodeMonkey.MC68000EmulatorLib;
using PendleCodeMonkey.MC68000EmulatorLib.Enumerations;
using Xunit;

namespace PendleCodeMonkey.MC68000Emulator.Tests
{
	public class MachineTests
	{
		[Fact]
		public void NewMachine_ShouldNotBeNull()
		{
			Machine machine = new Machine();

			Assert.NotNull(machine);
		}

		[Fact]
		public void NewMachine_ShouldHaveCPU()
		{
			Machine machine = new Machine();

			Assert.NotNull(machine.CPU);
		}

		[Fact]
		public void NewMachine_ShouldHaveMemory()
		{
			Machine machine = new Machine();

			Assert.NotNull(machine.Memory);
		}

		[Fact]
		public void NewMachine_ShouldHaveExecutionHandler()
		{
			Machine machine = new Machine();

			Assert.NotNull(machine.ExecutionHandler);
		}

		[Fact]
		public void NewMachine_ShouldHaveInstructionDecoder()
		{
			Machine machine = new Machine();

			Assert.NotNull(machine.Decoder);
		}

		[Fact]
		public void LoadData_ShouldSucceedWhenDataFitsInMemory()
		{
			Machine machine = new Machine();
			var success = machine.LoadData(new byte[] { 1, 2, 3, 4, 5, 6 }, 0x2000);

			Assert.True(success);
		}

		[Fact]
		public void LoadData_ShouldFailWhenDataExceedsMemoryLimit()
		{
			Machine machine = new Machine();
			var success = machine.LoadData(new byte[] { 1, 2, 3, 4, 5, 6 }, 0xFFFFFC);

			Assert.False(success);
		}

		[Fact]
		public void LoadExecutableData_ShouldSetPCToStartOfLoadedData()
		{
			Machine machine = new Machine();
			var _ = machine.LoadExecutableData(new byte[] { 1, 2, 3, 4, 5, 6 }, 0x2000);

			Assert.Equal((uint)0x2000, machine.CPU.PC);
		}

		[Fact]
		public void IsEndOfData_ShouldBeFalseWhenPCIsWithinLoadedData()
		{
			Machine machine = new Machine();
			var _ = machine.LoadExecutableData(new byte[] { 1, 2, 3, 4, 5, 6 }, 0x2000);

			Assert.False(machine.IsEndOfData);
		}

		[Fact]
		public void IsEndOfData_ShouldBeTrueWhenPCIsPassedEndOfLoadedData()
		{
			Machine machine = new Machine();
			var _ = machine.LoadExecutableData(new byte[] { 1, 2, 3, 4, 5, 6 }, 0x2000);
			machine.CPU.PC = 0x2008;

			Assert.True(machine.IsEndOfData);
		}

		[Fact]
		public void DumpMemory_ShouldReturnCorrectMemoryBlock()
		{
			Machine machine = new Machine();
			var _ = machine.LoadData(new byte[] { 1, 2, 3, 4, 5, 6 }, 0x2000);

			var dump = machine.DumpMemory(0x2002, 0x0010);

			Assert.Equal(0x0010, dump.Length);
			Assert.Equal(0x05, dump[2]);
		}

		[Fact]
		public void GetCPUState_ShouldReturnCorrectCPUState()
		{
			Machine machine = new Machine();
			machine.CPU.DataRegisters = new uint[] { 100, 200, 300, 400, 500, 600, 700, 800 };
			machine.CPU.AddressRegisters = new uint[] { 1100, 1200, 1300, 1400, 1500, 1600, 1700 };
			machine.CPU.PC = 0x0200;
			machine.CPU.USP = 0x4000;
			machine.CPU.SSP = 0x4000;
			machine.CPU.SR = SRFlags.Carry;

			CPUState state = machine.GetCPUState();

			Assert.Equal(machine.CPU.DataRegisters[0], state.D0);
			Assert.Equal(machine.CPU.DataRegisters[1], state.D1);
			Assert.Equal(machine.CPU.DataRegisters[2], state.D2);
			Assert.Equal(machine.CPU.DataRegisters[3], state.D3);
			Assert.Equal(machine.CPU.DataRegisters[4], state.D4);
			Assert.Equal(machine.CPU.DataRegisters[5], state.D5);
			Assert.Equal(machine.CPU.DataRegisters[6], state.D6);
			Assert.Equal(machine.CPU.DataRegisters[7], state.D7);
			Assert.Equal(machine.CPU.AddressRegisters[0], state.A0);
			Assert.Equal(machine.CPU.AddressRegisters[1], state.A1);
			Assert.Equal(machine.CPU.AddressRegisters[2], state.A2);
			Assert.Equal(machine.CPU.AddressRegisters[3], state.A3);
			Assert.Equal(machine.CPU.AddressRegisters[4], state.A4);
			Assert.Equal(machine.CPU.AddressRegisters[5], state.A5);
			Assert.Equal(machine.CPU.AddressRegisters[6], state.A6);
			Assert.Equal(machine.CPU.PC, state.PC);
			Assert.Equal(machine.CPU.USP, state.USP);
			Assert.Equal(machine.CPU.SSP, state.SSP);
			Assert.Equal(machine.CPU.SR, state.SR);
		}

		[Fact]
		public void SetCPUState_ShouldCorrectlyUpdateCPUState()
		{
			Machine machine = new Machine();
			machine.CPU.DataRegisters = new uint[] { 100, 200, 300, 400, 500, 600, 700, 800 };
			machine.CPU.AddressRegisters = new uint[] { 1100, 1200, 1300, 1400, 1500, 1600, 1700 };
			machine.CPU.PC = 0x0200;
			machine.CPU.USP = 0x4000;
			machine.CPU.SSP = 0x4000;
			machine.CPU.SR = SRFlags.Carry;

			// Set the state of some settings (NOTE: all others should remain unchanged)
			CPUState newState = new CPUState
			{
				D2 = 0x2030,
				D5 = 0x1050,
				A3 = 0xABCD,
				A6 = 0xDCBA,
				PC = 0x8000,
				USP = 0x432100,
				SSP = 0x100100,
				SR = SRFlags.Overflow
			};
			machine.SetCPUState(newState);

			Assert.Equal((uint)100, machine.CPU.DataRegisters[0]);
			Assert.Equal((uint)200, machine.CPU.DataRegisters[1]);
			Assert.Equal((uint)0x2030, machine.CPU.DataRegisters[2]);
			Assert.Equal((uint)400, machine.CPU.DataRegisters[3]);
			Assert.Equal((uint)500, machine.CPU.DataRegisters[4]);
			Assert.Equal((uint)0x1050, machine.CPU.DataRegisters[5]);
			Assert.Equal((uint)700, machine.CPU.DataRegisters[6]);
			Assert.Equal((uint)800, machine.CPU.DataRegisters[7]);
			Assert.Equal((uint)1100, machine.CPU.AddressRegisters[0]);
			Assert.Equal((uint)1200, machine.CPU.AddressRegisters[1]);
			Assert.Equal((uint)1300, machine.CPU.AddressRegisters[2]);
			Assert.Equal((uint)0xABCD, machine.CPU.AddressRegisters[3]);
			Assert.Equal((uint)1500, machine.CPU.AddressRegisters[4]);
			Assert.Equal((uint)1600, machine.CPU.AddressRegisters[5]);
			Assert.Equal((uint)0xDCBA, machine.CPU.AddressRegisters[6]);
			Assert.Equal((uint)0x8000, machine.CPU.PC);
			Assert.Equal((uint)0x432100, machine.CPU.USP);
			Assert.Equal((uint)0x100100, machine.CPU.SSP);
			Assert.Equal(SRFlags.Overflow, machine.CPU.SR);
		}

		[Fact]
		public void PushLong()
		{
			Machine machine = new Machine();
//			var _ = machine.LoadData(new byte[] { 1, 2, 3, 4, 5, 6 }, 0x2000);
			machine.CPU.USP = 0x4000;

			machine.PushLong(0x01020304);

			Assert.Equal((uint)0x3FFC, machine.CPU.USP);
			Assert.Equal(0x01, machine.Memory.Data[0x3FFC]);
			Assert.Equal(0x02, machine.Memory.Data[0x3FFD]);
			Assert.Equal(0x03, machine.Memory.Data[0x3FFE]);
			Assert.Equal(0x04, machine.Memory.Data[0x3FFF]);
		}

		[Fact]
		public void PushWord()
		{
			Machine machine = new Machine();
			machine.CPU.USP = 0x4000;

			machine.PushWord(0x2030);

			Assert.Equal((uint)0x3FFE, machine.CPU.USP);
			Assert.Equal(0x20, machine.Memory.Data[0x3FFE]);
			Assert.Equal(0x30, machine.Memory.Data[0x3FFF]);
		}

		[Fact]
		public void PopLong()
		{
			Machine machine = new Machine();
			var _ = machine.LoadData(new byte[] { 1, 2, 3, 4, 5, 6 }, 0x2000);
			machine.CPU.USP = 0x2000;

			uint value = machine.PopLong();

			Assert.Equal((uint)0x2004, machine.CPU.USP);
			Assert.Equal((uint)0x01020304, value);
		}

		[Fact]
		public void PopWord()
		{
			Machine machine = new Machine();
			var _ = machine.LoadData(new byte[] { 1, 2, 3, 4, 5, 6 }, 0x2000);
			machine.CPU.USP = 0x2000;

			ushort value = machine.PopWord();

			Assert.Equal((uint)0x2002, machine.CPU.USP);
			Assert.Equal((uint)0x0102, value);
		}


	}
}
