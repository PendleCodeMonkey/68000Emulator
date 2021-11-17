using PendleCodeMonkey.MC68000EmulatorLib;
using PendleCodeMonkey.MC68000EmulatorLib.Enumerations;
using Xunit;

namespace PendleCodeMonkey.MC68000Emulator.Tests
{
	public class CPUTests
	{
		[Fact]
		public void NewCPU_ShouldNotBeNull()
		{
			CPU cpu = new CPU();

			Assert.NotNull(cpu);
		}

		[Fact]
		public void Reset_ShouldResetState()
		{
			CPU cpu = new CPU
			{
				DataRegisters = new uint[] { 100, 200, 300, 400, 500, 600, 700, 800 },
				AddressRegisters = new uint[] { 1100, 1200, 1300, 1400, 1500, 1600, 1700 },
				PC = 0x0200,
				USP = 0x4000,
				SSP = 0x4000,
				SR = SRFlags.Carry
			};

			cpu.Reset();

			Assert.Equal((uint)0, cpu.DataRegisters[0]);
			Assert.Equal((uint)0, cpu.DataRegisters[1]);
			Assert.Equal((uint)0, cpu.DataRegisters[2]);
			Assert.Equal((uint)0, cpu.DataRegisters[3]);
			Assert.Equal((uint)0, cpu.DataRegisters[4]);
			Assert.Equal((uint)0, cpu.DataRegisters[5]);
			Assert.Equal((uint)0, cpu.DataRegisters[6]);
			Assert.Equal((uint)0, cpu.DataRegisters[7]);
			Assert.Equal((uint)0, cpu.AddressRegisters[0]);
			Assert.Equal((uint)0, cpu.AddressRegisters[1]);
			Assert.Equal((uint)0, cpu.AddressRegisters[2]);
			Assert.Equal((uint)0, cpu.AddressRegisters[3]);
			Assert.Equal((uint)0, cpu.AddressRegisters[4]);
			Assert.Equal((uint)0, cpu.AddressRegisters[5]);
			Assert.Equal((uint)0, cpu.AddressRegisters[6]);
			Assert.Equal((uint)0, cpu.PC);
			Assert.Equal((uint)0, cpu.USP);
			Assert.Equal((uint)0, cpu.SSP);
			Assert.Equal((SRFlags)0, cpu.SR);
		}

		[Fact]
		public void ReadDataRegister()
		{
			CPU cpu = new CPU
			{
				DataRegisters = new uint[] { 100, 200, 300, 400, 500, 600, 700, 800 }
			};

			uint d0 = cpu.ReadDataRegister(0);
			uint d1 = cpu.ReadDataRegister(1);
			uint d2 = cpu.ReadDataRegister(2);
			uint d3 = cpu.ReadDataRegister(3);
			uint d4 = cpu.ReadDataRegister(4);
			uint d5 = cpu.ReadDataRegister(5);
			uint d6 = cpu.ReadDataRegister(6);
			uint d7 = cpu.ReadDataRegister(7);

			Assert.Equal((uint)100, d0);
			Assert.Equal((uint)200, d1);
			Assert.Equal((uint)300, d2);
			Assert.Equal((uint)400, d3);
			Assert.Equal((uint)500, d4);
			Assert.Equal((uint)600, d5);
			Assert.Equal((uint)700, d6);
			Assert.Equal((uint)800, d7);
		}

		[Theory]
		[InlineData(0x000000AA, OpSize.Byte, 0x123456AA)]
		[InlineData(0x0000AA55, OpSize.Word, 0x1234AA55)]
		[InlineData(0x87654321, OpSize.Long, 0x87654321)]
		public void WriteDataRegister(uint value, OpSize size, uint expectedResult)
		{
			CPU cpu = new CPU
			{
				DataRegisters = new uint[] { 0x12345678, 0, 0, 0, 0, 0, 0, 0 }
			};
			cpu.WriteDataRegister(0, value, size);
			uint d0 = cpu.ReadDataRegister(0);

			// Assert
			Assert.Equal(expectedResult, d0);
		}

		[Fact]
		public void ReadAddressRegister()
		{
			CPU cpu = new CPU
			{
				AddressRegisters = new uint[] { 1100, 1200, 1300, 1400, 1500, 1600, 1700 },
				USP = 0x4000,
				SSP = 0x5000
			};

			uint a0 = cpu.ReadAddressRegister(0);
			uint a1 = cpu.ReadAddressRegister(1);
			uint a2 = cpu.ReadAddressRegister(2);
			uint a3 = cpu.ReadAddressRegister(3);
			uint a4 = cpu.ReadAddressRegister(4);
			uint a5 = cpu.ReadAddressRegister(5);
			uint a6 = cpu.ReadAddressRegister(6);
			uint a7 = cpu.ReadAddressRegister(7);           // Read A7 value whilst in user mode.
			cpu.SR = SRFlags.SupervisorMode;
			uint a7_ssp = cpu.ReadAddressRegister(7);       // Read A7 value whilst in supervisor mode.

			Assert.Equal((uint)1100, a0);
			Assert.Equal((uint)1200, a1);
			Assert.Equal((uint)1300, a2);
			Assert.Equal((uint)1400, a3);
			Assert.Equal((uint)1500, a4);
			Assert.Equal((uint)1600, a5);
			Assert.Equal((uint)1700, a6);
			Assert.Equal((uint)0x4000, a7);
			Assert.Equal((uint)0x5000, a7_ssp);
		}

		[Theory]
		[InlineData(0x000000AA, OpSize.Byte, 0x123456AA)]
		[InlineData(0x0000AA55, OpSize.Word, 0x1234AA55)]
		[InlineData(0x87654321, OpSize.Long, 0x87654321)]
		public void WriteAddressRegister(uint value, OpSize size, uint expectedResult)
		{
			CPU cpu = new CPU
			{
				AddressRegisters = new uint[] { 0, 0x12345678, 0, 0, 0, 0, 0 }
			};
			cpu.WriteAddressRegister(1, value, size);
			uint a1 = cpu.ReadAddressRegister(1);

			// Assert
			Assert.Equal(expectedResult, a1);
		}

		[Theory]
		[InlineData(0x000000AA, OpSize.Byte, 0x123456AA)]
		[InlineData(0x0000AA55, OpSize.Word, 0x1234AA55)]
		[InlineData(0x87654321, OpSize.Long, 0x87654321)]
		public void WriteAddressRegister_A7(uint value, OpSize size, uint expectedResult)
		{
			CPU cpu = new CPU
			{
				USP = 0x12345678
			};
			cpu.WriteAddressRegister(7, value, size);
			uint a7 = cpu.ReadAddressRegister(7);

			// Assert
			Assert.Equal(expectedResult, a7);
		}

		[Theory]
		[InlineData(0x00123456, OpSize.Byte, 0x00123457)]
		[InlineData(0x12341234, OpSize.Word, 0x12341236)]
		[InlineData(0x00222222, OpSize.Long, 0x00222226)]
		public void IncrementAddressRegister(uint value, OpSize size, uint expectedResult)
		{
			CPU cpu = new CPU
			{
				AddressRegisters = new uint[] { 0, value, 0, 0, 0, 0, 0 }
			};
			uint a1 = cpu.IncrementAddressRegister(1, size);

			// Assert
			Assert.Equal(expectedResult, a1);
		}

		[Theory]
		[InlineData(0x00123456, OpSize.Byte, 0x00123455)]
		[InlineData(0x12341234, OpSize.Word, 0x12341232)]
		[InlineData(0x00222222, OpSize.Long, 0x0022221E)]
		public void DecrementAddressRegister(uint value, OpSize size, uint expectedResult)
		{
			CPU cpu = new CPU
			{
				AddressRegisters = new uint[] { 0, value, 0, 0, 0, 0, 0 }
			};
			uint a1 = cpu.DecrementAddressRegister(1, size);

			// Assert
			Assert.Equal(expectedResult, a1);
		}

		[Theory]
		[InlineData(0x00123456, 2, 0x00123458)]
		[InlineData(0x12341234, 4, 0x12341238)]
		[InlineData(0x00222222, 16, 0x00222232)]
		public void IncrementPC(uint value, byte numBytes, uint expectedResult)
		{
			CPU cpu = new CPU
			{
				PC = value
			};
			cpu.IncrementPC(numBytes);

			// Assert
			Assert.Equal(expectedResult, cpu.PC);
		}

		[Theory]
		[InlineData(Condition.T, (SRFlags)0, true)]
		[InlineData(Condition.F, (SRFlags)0, false)]
		[InlineData(Condition.HI, (SRFlags)0, true)]
		[InlineData(Condition.HI, SRFlags.Zero, false)]
		[InlineData(Condition.HI, SRFlags.Carry, false)]
		[InlineData(Condition.HI, SRFlags.Carry | SRFlags.Zero, false)]
		[InlineData(Condition.LS, (SRFlags)0, false)]
		[InlineData(Condition.LS, SRFlags.Zero, true)]
		[InlineData(Condition.LS, SRFlags.Carry, true)]
		[InlineData(Condition.LS, SRFlags.Carry | SRFlags.Zero, true)]
		[InlineData(Condition.CC, (SRFlags)0, true)]
		[InlineData(Condition.CC, SRFlags.Carry, false)]
		[InlineData(Condition.CS, (SRFlags)0, false)]
		[InlineData(Condition.CS, SRFlags.Carry, true)]
		[InlineData(Condition.NE, (SRFlags)0, true)]
		[InlineData(Condition.NE, SRFlags.Zero, false)]
		[InlineData(Condition.EQ, (SRFlags)0, false)]
		[InlineData(Condition.EQ, SRFlags.Zero, true)]
		[InlineData(Condition.VC, (SRFlags)0, true)]
		[InlineData(Condition.VC, SRFlags.Overflow, false)]
		[InlineData(Condition.VS, (SRFlags)0, false)]
		[InlineData(Condition.VS, SRFlags.Overflow, true)]
		[InlineData(Condition.PL, (SRFlags)0, true)]
		[InlineData(Condition.PL, SRFlags.Negative, false)]
		[InlineData(Condition.MI, (SRFlags)0, false)]
		[InlineData(Condition.MI, SRFlags.Negative, true)]
		[InlineData(Condition.GE, (SRFlags)0, true)]
		[InlineData(Condition.GE, SRFlags.Negative, false)]
		[InlineData(Condition.GE, SRFlags.Overflow, false)]
		[InlineData(Condition.GE, SRFlags.Negative | SRFlags.Overflow, true)]
		[InlineData(Condition.LT, (SRFlags)0, false)]
		[InlineData(Condition.LT, SRFlags.Negative, true)]
		[InlineData(Condition.LT, SRFlags.Overflow, true)]
		[InlineData(Condition.LT, SRFlags.Negative | SRFlags.Overflow, false)]
		[InlineData(Condition.GT, (SRFlags)0, true)]
		[InlineData(Condition.GT, SRFlags.Zero, false)]
		[InlineData(Condition.GT, SRFlags.Negative, false)]
		[InlineData(Condition.GT, SRFlags.Overflow, false)]
		[InlineData(Condition.GT, SRFlags.Negative | SRFlags.Overflow, true)]
		[InlineData(Condition.GT, SRFlags.Zero | SRFlags.Negative | SRFlags.Overflow, false)]
		[InlineData(Condition.LE, (SRFlags)0, false)]
		[InlineData(Condition.LE, SRFlags.Zero, true)]
		[InlineData(Condition.LE, SRFlags.Negative, true)]
		[InlineData(Condition.LE, SRFlags.Overflow, true)]
		[InlineData(Condition.LE, SRFlags.Negative | SRFlags.Overflow, false)]
		[InlineData(Condition.LE, SRFlags.Zero | SRFlags.Negative | SRFlags.Overflow, true)]
		public void EvaluateCondition(Condition condition, SRFlags flags, bool expectedResult)
		{
			// Arrange
			CPU cpu = new CPU
			{
				SR = flags
			};

			// Act
			bool result = cpu.EvaluateCondition(condition);

			// Assert
			Assert.Equal(expectedResult, result);
		}


	}
}
