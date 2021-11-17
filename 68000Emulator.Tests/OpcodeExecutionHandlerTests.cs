using PendleCodeMonkey.MC68000EmulatorLib;
using PendleCodeMonkey.MC68000EmulatorLib.Enumerations;
using Xunit;

namespace PendleCodeMonkey.MC68000Emulator.Tests
{
	public class OpcodeExecutionHandlerTests
	{
		/// <summary>
		/// Create an instance of the <see cref="Machine"/> class and populate the registers
		/// with some preset values.
		/// </summary>
		/// <returns>A new instance of the <see cref="Machine"/> class.</returns>
		private Machine CreateMachine()
		{
			var machine = new Machine();
			CPUState state = new CPUState
			{
				A0 = 0x0100,
				A1 = 0x0200,
				A2 = 0x0300,
				A3 = 0x0400,
				A4 = 0x0500,
				A5 = 0x0600,
				A6 = 0x0700,
				D0 = 0x0011,
				D1 = 0x0022,
				D2 = 0x0033,
				D3 = 0x0044,
				D4 = 0x0055,
				D5 = 0x0066,
				D6 = 0x0077,
				D7 = 0x0088,
				USP = 0x2000,
				SSP = 0x3000,
				SR = 0,
				PC = 0x4000
			};
			machine.SetCPUState(state);
			return machine;
		}

		[Theory]
		[InlineData(0x00, null, null, OpSize.Word, (byte)0, null, null, null)]                                  // D0
		[InlineData(0x01, null, null, OpSize.Word, (byte)1, null, null, null)]                                  // D1
		[InlineData(0x02, null, null, OpSize.Word, (byte)2, null, null, null)]                                  // D2
		[InlineData(0x03, null, null, OpSize.Word, (byte)3, null, null, null)]                                  // D3
		[InlineData(0x04, null, null, OpSize.Word, (byte)4, null, null, null)]                                  // D4
		[InlineData(0x05, null, null, OpSize.Word, (byte)5, null, null, null)]                                  // D5
		[InlineData(0x06, null, null, OpSize.Word, (byte)6, null, null, null)]                                  // D6
		[InlineData(0x07, null, null, OpSize.Word, (byte)7, null, null, null)]                                  // D7
		[InlineData(0x08, null, null, OpSize.Word, null, (byte)0, null, null)]                                  // A0
		[InlineData(0x09, null, null, OpSize.Word, null, (byte)1, null, null)]                                  // A1
		[InlineData(0x0A, null, null, OpSize.Word, null, (byte)2, null, null)]                                  // A2
		[InlineData(0x0B, null, null, OpSize.Word, null, (byte)3, null, null)]                                  // A3
		[InlineData(0x0C, null, null, OpSize.Word, null, (byte)4, null, null)]                                  // A4
		[InlineData(0x0D, null, null, OpSize.Word, null, (byte)5, null, null)]                                  // A5
		[InlineData(0x0E, null, null, OpSize.Word, null, (byte)6, null, null)]                                  // A6
		[InlineData(0x0F, null, null, OpSize.Word, null, (byte)7, null, null)]                                  // A7
		[InlineData(0x10, null, null, OpSize.Word, null, null, (uint)0x0100, null)]                             // (A0)
		[InlineData(0x11, null, null, OpSize.Word, null, null, (uint)0x0200, null)]                             // (A1)
		[InlineData(0x12, null, null, OpSize.Word, null, null, (uint)0x0300, null)]                             // (A2)
		[InlineData(0x13, null, null, OpSize.Word, null, null, (uint)0x0400, null)]                             // (A3)
		[InlineData(0x14, null, null, OpSize.Word, null, null, (uint)0x0500, null)]                             // (A4)
		[InlineData(0x15, null, null, OpSize.Word, null, null, (uint)0x0600, null)]                             // (A5)
		[InlineData(0x16, null, null, OpSize.Word, null, null, (uint)0x0700, null)]                             // (A6)
		[InlineData(0x17, null, null, OpSize.Word, null, null, (uint)0x2000, null)]                             // (A7)
		[InlineData(0x18, null, null, OpSize.Word, null, null, (uint)0x0100, null)]                             // (A0)+
		[InlineData(0x19, null, null, OpSize.Word, null, null, (uint)0x0200, null)]                             // (A1)+
		[InlineData(0x1A, null, null, OpSize.Word, null, null, (uint)0x0300, null)]                             // (A2)+
		[InlineData(0x1B, null, null, OpSize.Word, null, null, (uint)0x0400, null)]                             // (A3)+
		[InlineData(0x1C, null, null, OpSize.Word, null, null, (uint)0x0500, null)]                             // (A4)+
		[InlineData(0x1D, null, null, OpSize.Word, null, null, (uint)0x0600, null)]                             // (A5)+
		[InlineData(0x1E, null, null, OpSize.Word, null, null, (uint)0x0700, null)]                             // (A6)+
		[InlineData(0x1F, null, null, OpSize.Word, null, null, (uint)0x2000, null)]                             // (A7)+
		[InlineData(0x20, null, null, OpSize.Word, null, null, (uint)0x00FE, null)]                             // -(A0)
		[InlineData(0x21, null, null, OpSize.Word, null, null, (uint)0x01FE, null)]                             // -(A1)
		[InlineData(0x22, null, null, OpSize.Word, null, null, (uint)0x02FE, null)]                             // -(A2)
		[InlineData(0x23, null, null, OpSize.Word, null, null, (uint)0x03FE, null)]                             // -(A3)
		[InlineData(0x24, null, null, OpSize.Word, null, null, (uint)0x04FE, null)]                             // -(A4)
		[InlineData(0x25, null, null, OpSize.Word, null, null, (uint)0x05FE, null)]                             // -(A5)
		[InlineData(0x26, null, null, OpSize.Word, null, null, (uint)0x06FE, null)]                             // -(A6)
		[InlineData(0x27, null, null, OpSize.Word, null, null, (uint)0x1FFE, null)]                             // -(A7)
		[InlineData(0x28, (ushort)0x50, null, OpSize.Word, null, null, (uint)0x0150, null)]                     // (d16,A0)
		[InlineData(0x29, (ushort)0x52, null, OpSize.Word, null, null, (uint)0x0252, null)]                     // (d16,A1)
		[InlineData(0x2A, (ushort)0x54, null, OpSize.Word, null, null, (uint)0x0354, null)]                     // (d16,A2)
		[InlineData(0x2B, (ushort)0x56, null, OpSize.Word, null, null, (uint)0x0456, null)]                     // (d16,A3)
		[InlineData(0x2C, (ushort)0x58, null, OpSize.Word, null, null, (uint)0x0558, null)]                     // (d16,A4)
		[InlineData(0x2D, (ushort)0x5A, null, OpSize.Word, null, null, (uint)0x065A, null)]                     // (d16,A5)
		[InlineData(0x2E, (ushort)0x5C, null, OpSize.Word, null, null, (uint)0x075C, null)]                     // (d16,A6)
		[InlineData(0x2F, (ushort)0x5E, null, OpSize.Word, null, null, (uint)0x205E, null)]                     // (d16,A7)
		[InlineData(0x30, (ushort)0x50, null, OpSize.Word, null, null, (uint)0x0161, null)]                     // (d8,D0,A0)
		[InlineData(0x31, (ushort)0x1050, null, OpSize.Word, null, null, (uint)0x0272, null)]                     // (d8,D1,A1)
		[InlineData(0x32, (ushort)0x2050, null, OpSize.Word, null, null, (uint)0x0383, null)]                     // (d8,D2,A2)
		[InlineData(0x33, (ushort)0x3050, null, OpSize.Word, null, null, (uint)0x0494, null)]                     // (d8,D3,A3)
		[InlineData(0x34, (ushort)0x4050, null, OpSize.Word, null, null, (uint)0x05A5, null)]                     // (d8,D4,A4)
		[InlineData(0x35, (ushort)0x5050, null, OpSize.Word, null, null, (uint)0x06B6, null)]                     // (d8,D5,A5)
		[InlineData(0x36, (ushort)0x6050, null, OpSize.Word, null, null, (uint)0x07C7, null)]                     // (d8,D6,A6)
		[InlineData(0x37, (ushort)0x7050, null, OpSize.Word, null, null, (uint)0x20D8, null)]                     // (d8,D7,A7)
		[InlineData(0x38, (ushort)0x1FF0, null, OpSize.Word, null, null, (uint)0x1FF0, null)]                   // (xxx).w
		[InlineData(0x39, (ushort)0x0006, (ushort)0x1234, OpSize.Word, null, null, (uint)0x00061234, null)]     // (xxx).l
		[InlineData(0x3A, (ushort)0x50, null, OpSize.Word, null, null, (uint)0x4050, null)]                     // (d16,PC)
		[InlineData(0x3B, (ushort)0x50, null, OpSize.Word, null, null, (uint)0x4061, null)]                     // (d8,D0,PC)
		[InlineData(0x3C, (ushort)0x1234, null, OpSize.Word, null, null, null, (uint)0x1234)]                   // #<data>  [word]
		[InlineData(0x3C, (ushort)0x1234, (ushort)0x5678, OpSize.Long, null, null, null, (uint)0x12345678)]     // #<data>  [long]
		public void EvaluateEffectiveAddress(byte addrMode, ushort? ext1, ushort? ext2, OpSize size,
												byte? expDataReg, byte? expAddrReg, uint? expAddress, uint? expImm)
		{
			// Arrange
			var machine = CreateMachine();

			// Create an instance of the Instruction class. We pass a zero opcode value and a null InstrumentInfo object here
			// because these are not used when evaluating the effective address (so we don't need to worry about supplying
			// 'proper' values).
			// We also only supply the source effective address parameters (as the method works identically for both source and
			// destination effective addresses; therefore, we only need to test one).
			Instruction inst = new Instruction(0, null, size, addrMode, ext1, ext2);

			// Act
			var (dataRegNum, addrRegNum, address, immValue) = machine.ExecutionHandler.EvaluateEffectiveAddress(inst, EAType.Source);

			// Assert
			Assert.Equal(expDataReg, dataRegNum);
			Assert.Equal(expAddrReg, addrRegNum);
			Assert.Equal(expAddress, address);
			Assert.Equal(expImm, immValue);
		}

		[Theory]
		[InlineData(0x18, OpSize.Word, (uint)0x0102)]                             // (A0)+   [word]
		[InlineData(0x18, OpSize.Long, (uint)0x0104)]                             // (A0)+   [long]
		[InlineData(0x18, OpSize.Byte, (uint)0x0101)]                             // (A0)+   [byte]
		[InlineData(0x20, OpSize.Word, (uint)0x00FE)]                             // -(A0)   [word]
		[InlineData(0x20, OpSize.Long, (uint)0x00FC)]                             // -(A0)   [long]
		[InlineData(0x20, OpSize.Byte, (uint)0x00FF)]                             // -(A0)   [byte]
		public void EvaluateEffectiveAddress_PostIncPreDec(byte addrMode, OpSize size, uint expRegValue)
		{
			// Arrange
			var machine = CreateMachine();

			// Create an instance of the Instruction class. We pass a zero opcode value and a null InstrumentInfo object here
			// because these are not used when evaluating the effective address (so we don't need to worry about supplying
			// 'proper' values).
			// We also only supply the source addressing mode parameter and data size (as this is enough to test the postincrement and
			// predecrement modes).
			Instruction inst = new Instruction(0, null, size, addrMode);

			// Act
			var _ = machine.ExecutionHandler.EvaluateEffectiveAddress(inst, EAType.Source);

			// Assert
			Assert.Equal(expRegValue, machine.CPU.ReadAddressRegister(0));
		}

		[Theory]
		[InlineData(0x1F, OpSize.Word, (uint)0x2002)]       // (A7)+   [word]
		[InlineData(0x1F, OpSize.Long, (uint)0x2004)]       // (A7)+   [long]
		[InlineData(0x1F, OpSize.Byte, (uint)0x2002)]       // (A7)+   [byte]  (special case - postincrements by a word for byte operation on stack)
		[InlineData(0x27, OpSize.Word, (uint)0x1FFE)]       // -(A7)   [word]
		[InlineData(0x27, OpSize.Long, (uint)0x1FFC)]       // -(A7)   [long]
		[InlineData(0x27, OpSize.Byte, (uint)0x1FFE)]       // -(A7)   [byte]  (special case - predecrements by a word for byte operation on stack)
		public void EvaluateEffectiveAddress_PostIncPreDec_Stack(byte addrMode, OpSize size, uint expRegValue)
		{
			// Arrange
			var machine = CreateMachine();

			// Create an instance of the Instruction class. We pass a zero opcode value and a null InstrumentInfo object here
			// because these are not used when evaluating the effective address (so we don't need to worry about supplying
			// 'proper' values).
			// We also only supply the source addressing mode parameter and data size (as this is enough to test the postincrement and
			// predecrement modes).
			Instruction inst = new Instruction(0, null, size, addrMode);

			// Act
			var _ = machine.ExecutionHandler.EvaluateEffectiveAddress(inst, EAType.Source);

			// Assert
			Assert.Equal(expRegValue, machine.CPU.ReadAddressRegister(7));
		}

		[Theory]
		[InlineData(0x00000020, OpSize.Byte, 0x00000020)]
		[InlineData(0x00000830, OpSize.Byte, 0x00000030)]
		[InlineData(0xFFFFFF80, OpSize.Byte, 0x00000080)]
		[InlineData(0x00000020, OpSize.Word, 0x00000020)]
		[InlineData(0x000089AB, OpSize.Word, 0x000089AB)]
		[InlineData(0x22338765, OpSize.Word, 0x00008765)]
		[InlineData(0x00000020, OpSize.Long, 0x00000020)]
		[InlineData(0x0001FFFF, OpSize.Long, 0x0001FFFF)]
		[InlineData(0x12345678, OpSize.Long, 0x12345678)]
		[InlineData(0xFEDCBA98, OpSize.Long, 0xFEDCBA98)]
		public void SizedValue(uint value, OpSize size, uint expectedResult)
		{
			// Arrange
			var machine = CreateMachine();

			uint result = machine.ExecutionHandler.SizedValue(value, size);

			// Assert
			Assert.Equal(expectedResult, result);
		}


		// ********************************
		//
		// Tests for opcode handler methods
		//
		// ********************************

		[Fact]
		public void ORItoCCR()
		{
			// Arrange
			ushort[] code = new ushort[] { 0x003C, 0x0010 };  // ori #$10,ccr
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				SR = SRFlags.Zero
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(SRFlags.Zero | SRFlags.Extend, machine.CPU.SR);
		}

		[Fact]
		public void ORItoSR()
		{
			// Arrange
			ushort[] code = new ushort[] { 0x007C, 0x2000 };  // ori #$2000,sr
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				SR = SRFlags.Zero
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(SRFlags.Zero | SRFlags.SupervisorMode, machine.CPU.SR);
		}

		[Fact]
		public void ORI()
		{
			// Arrange
			ushort[] code = new ushort[] { 0x0041, 0x1234, 0x0054, 0x1234 };  // ori.w #$1234,d1 | ori.w #$1234,(a4)
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D1 = 0x00128000,
				A4 = 0x00002000
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { 0x8055 };
			machine.LoadData(data, 0x00002000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal((uint)0x00129234, machine.CPU.ReadDataRegister(1));
			Assert.Equal((uint)0x9275, machine.Memory.ReadWord(0x00002000));
		}

		[Theory]
		[InlineData(0x00000000, OpSize.Byte, SRFlags.Zero)]
		[InlineData(0x000000AA, OpSize.Byte, SRFlags.Negative)]
		[InlineData(0x00000020, OpSize.Byte, (SRFlags)0)]
		[InlineData(0x00000000, OpSize.Word, SRFlags.Zero)]
		[InlineData(0x0000AA55, OpSize.Word, SRFlags.Negative)]
		[InlineData(0x00002020, OpSize.Word, (SRFlags)0)]
		[InlineData(0x00000000, OpSize.Long, SRFlags.Zero)]
		[InlineData(0x00008000, OpSize.Long, (SRFlags)0)]
		[InlineData(0x87654321, OpSize.Long, SRFlags.Negative)]
		public void ORI_FlagsTest(uint orValue, OpSize size, SRFlags expectedFlags)
		{
			ushort[] code = size switch
			{
				OpSize.Byte => new ushort[] { 0x0001, (ushort)orValue },	// ori.b #<orValue>,d1
				OpSize.Word => new ushort[] { 0x0041, (ushort)orValue },	// ori.w #<orValue>,d1
				_ => new ushort[] { 0x0081, (ushort)((orValue & 0xFFFF0000) >> 16), (ushort)orValue },	// ori.l #<orValue>,d1
			};
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D1 = 0x00000000
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Fact]
		public void ANDItoCCR()
		{
			// Arrange
			ushort[] code = new ushort[] { 0x023C, 0x00F8 };  // and #$F8,ccr
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				SR = SRFlags.Negative | SRFlags.Carry | SRFlags.SupervisorMode
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(SRFlags.Negative | SRFlags.SupervisorMode, machine.CPU.SR);
		}

		[Fact]
		public void ANDItoSR()
		{
			// Arrange
			ushort[] code = new ushort[] { 0x027C, 0xDFFF };  // and #$DFFF,sr
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				SR = SRFlags.Negative | SRFlags.Carry | SRFlags.SupervisorMode
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(SRFlags.Negative | SRFlags.Carry, machine.CPU.SR);
		}

		[Fact]
		public void ANDI()
		{
			// Arrange
			ushort[] code = new ushort[] { 0x0242, 0x0F0F, 0x0256, 0x7070 };  // andi.w #$0F0F,d2 | andi.w #$7070,(a6)
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D2 = 0x00125555,
				A6 = 0x00002000
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { 0xFEDC };
			machine.LoadData(data, 0x00002000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal((uint)0x00120505, machine.CPU.ReadDataRegister(2));
			Assert.Equal((uint)0x7050, machine.Memory.ReadWord(0x00002000));
		}

		[Theory]
		[InlineData(0x00000000, OpSize.Byte, SRFlags.Zero)]
		[InlineData(0x000000AA, OpSize.Byte, (SRFlags)0)]
		[InlineData(0x00000020, OpSize.Byte, (SRFlags)0)]
		[InlineData(0x00000000, OpSize.Word, SRFlags.Zero)]
		[InlineData(0x0000AA55, OpSize.Word, SRFlags.Negative)]
		[InlineData(0x00002020, OpSize.Word, (SRFlags)0)]
		[InlineData(0x00000000, OpSize.Long, SRFlags.Zero)]
		[InlineData(0x00008000, OpSize.Long, (SRFlags)0)]
		[InlineData(0xFEDCBA98, OpSize.Long, SRFlags.Negative)]
		public void ANDI_FlagsTest(uint andValue, OpSize size, SRFlags expectedFlags)
		{
			ushort[] code = size switch
			{
				OpSize.Byte => new ushort[] { 0x0202, (ushort)andValue },    // andi.b #<orValue>,d2
				OpSize.Word => new ushort[] { 0x0242, (ushort)andValue },    // andi.w #<orValue>,d2
				_ => new ushort[] { 0x0282, (ushort)((andValue & 0xFFFF0000) >> 16), (ushort)andValue },  // andi.l #<orValue>,d2
			};
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D2 = 0x87658765
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}


		[Fact]
		public void SUBI()
		{
			// Arrange
			ushort[] code = new ushort[] { 0x0402, 0x0010, 0x0443, 0x2030, 0x0496, 0x3040, 0x5060 };  // subi.b #$10,d2 | subi.w #$2030,d3 | subi.l #$30405060,(a6)
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D2 = 0x00125555,
				D3 = 0x00125555,
				A6 = 0x00002000
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { 0xFEDC, 0xBA98 };
			machine.LoadData(data, 0x00002000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal((uint)0x00125545, machine.CPU.ReadDataRegister(2));
			Assert.Equal((uint)0x00123525, machine.CPU.ReadDataRegister(3));
			Assert.Equal(0xCE9C6A38, machine.Memory.ReadLong(0x00002000));
		}

		[Theory]
		[InlineData(0x00000000, SRFlags.Negative)]
		[InlineData(0x00008765, SRFlags.Zero)]
		[InlineData(0x00004321, SRFlags.Overflow)]
		[InlineData(0x0000F321, SRFlags.Carry | SRFlags.Extend | SRFlags.Negative)]
		public void SUBI_FlagsTest(uint subValue, SRFlags expectedFlags)
		{
			ushort[] code = new ushort[] { 0x0442, (ushort)subValue };    // subi.w #<orValue>,d2;
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D2 = 0x87658765
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Fact]
		public void ADDI()
		{
			// Arrange
			ushort[] code = new ushort[] { 0x0602, 0x0010, 0x0643, 0x2030, 0x0696, 0x3040, 0x5060 };  // addi.b #$10,d2 | addi.w #$2030,d3 | addi.l #$30405060,(a6)
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D2 = 0x00125555,
				D3 = 0x00125555,
				A6 = 0x00002000
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { 0xFEDC, 0xBA98 };
			machine.LoadData(data, 0x00002000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal((uint)0x00125565, machine.CPU.ReadDataRegister(2));
			Assert.Equal((uint)0x00127585, machine.CPU.ReadDataRegister(3));
			Assert.Equal((uint)0x2F1D0AF8, machine.Memory.ReadLong(0x00002000));
		}

		[Theory]
		[InlineData(0x00000000, (SRFlags)0)]
		[InlineData(0x00004000, SRFlags.Overflow | SRFlags.Negative)]
		[InlineData(0x0000CA99, SRFlags.Carry | SRFlags.Extend)]
		[InlineData(0x0000BA99, SRFlags.Zero | SRFlags.Carry | SRFlags.Extend)]
		[InlineData(0x00001234, (SRFlags)0)]
		public void ADDI_FlagsTest(uint addValue, SRFlags expectedFlags)
		{
			ushort[] code = new ushort[] { 0x0642, (ushort)addValue };    // addi.w #<orValue>,d2;
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D2 = 0x45674567
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Fact]
		public void EORItoCCR()
		{
			// Arrange
			ushort[] code = new ushort[] { 0x0A3C, 0x0019 };  // eori #$19,ccr
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				SR = SRFlags.Zero | SRFlags.Carry
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(SRFlags.Zero | SRFlags.Negative | SRFlags.Extend, machine.CPU.SR);
		}

		[Fact]
		public void EORItoSR()
		{
			// Arrange
			ushort[] code = new ushort[] { 0x0A7C, 0xA000 };  // eori #$A000,sr
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				SR = SRFlags.Zero | SRFlags.TraceMode
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(SRFlags.Zero | SRFlags.SupervisorMode, machine.CPU.SR);
		}

		[Fact]
		public void EORI()
		{
			// Arrange
			ushort[] code = new ushort[] { 0x0A47, 0x0F0F, 0x0A56, 0xF0F0 };  // eori.w #$0F0F,d7 | eori.w #$F0F0,(a6)
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D7 = 0x0012AAAA,
				A6 = 0x00002000
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { 0x8055 };
			machine.LoadData(data, 0x00002000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal((uint)0x0012A5A5, machine.CPU.ReadDataRegister(7));
			Assert.Equal((uint)0x70A5, machine.Memory.ReadWord(0x00002000));
		}

		[Theory]
		[InlineData(0x00000000, OpSize.Byte, (SRFlags)0)]
		[InlineData(0x00000078, OpSize.Byte, SRFlags.Zero)]
		[InlineData(0x00000080, OpSize.Byte, SRFlags.Negative)]
		[InlineData(0x00000000, OpSize.Word, (SRFlags)0)]
		[InlineData(0x00005678, OpSize.Word, SRFlags.Zero)]
		[InlineData(0x00008765, OpSize.Word, SRFlags.Negative)]
		[InlineData(0x00000000, OpSize.Long, (SRFlags)0)]
		[InlineData(0x12345678, OpSize.Long, SRFlags.Zero)]
		[InlineData(0x87654321, OpSize.Long, SRFlags.Negative)]
		public void EORI_FlagsTest(uint eorValue, OpSize size, SRFlags expectedFlags)
		{
			ushort[] code = size switch
			{
				OpSize.Byte => new ushort[] { 0x0A07, (ushort)eorValue },    // eori.b #<orValue>,d7
				OpSize.Word => new ushort[] { 0x0A47, (ushort)eorValue },    // eori.w #<orValue>,d7
				_ => new ushort[] { 0x0A87, (ushort)((eorValue & 0xFFFF0000) >> 16), (ushort)eorValue },  // eori.l #<orValue>,d7
			};
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D7 = 0x12345678
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(0x00000000, OpSize.Byte, (SRFlags)0)]
		[InlineData(0x00000078, OpSize.Byte, SRFlags.Zero)]
		[InlineData(0x0000007F, OpSize.Byte, SRFlags.Carry | SRFlags.Negative)]
		[InlineData(0x00000000, OpSize.Word, (SRFlags)0)]
		[InlineData(0x00005678, OpSize.Word, SRFlags.Zero)]
		[InlineData(0x0000AA55, OpSize.Word, SRFlags.Negative | SRFlags.Carry | SRFlags.Overflow)]
		[InlineData(0x00002020, OpSize.Word, (SRFlags)0)]
		[InlineData(0x00000000, OpSize.Long, (SRFlags)0)]
		[InlineData(0x02345678, OpSize.Long, (SRFlags)0)]
		[InlineData(0xFEDCBA98, OpSize.Long, SRFlags.Carry)]
		public void CMPI(uint cmpValue, OpSize size, SRFlags expectedFlags)
		{
			ushort[] code = size switch
			{
				OpSize.Byte => new ushort[] { 0x0C03, (ushort)cmpValue },    // cmpi.b #<orValue>,d3
				OpSize.Word => new ushort[] { 0x0C43, (ushort)cmpValue },    // cmpi.w #<orValue>,d4
				_ => new ushort[] { 0x0C83, (ushort)((cmpValue & 0xFFFF0000) >> 16), (ushort)cmpValue },  // cmpi.l #<orValue>,d3
			};
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D3 = 0x12345678
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Fact]
		public void MOVE()
		{
			// Arrange
			ushort[] code = new ushort[] { 0x1600, 0x3286, 0x281A };  // move.b d0,d3 | move.w d6,(a1) | move.l (a2)+,d4
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D0 = 0x0012AABC,
				D6 = 0x12345678,
				A1 = 0x00002000,
				A2 = 0x00003000
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { 0x8055, 0x40AA };
			machine.LoadData(data, 0x00003000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal((uint)0x000000BC, machine.CPU.ReadDataRegister(3));
			Assert.Equal((uint)0x5678, machine.Memory.ReadWord(0x00002000));
			Assert.Equal((uint)0x805540AA, machine.CPU.ReadDataRegister(4));
			Assert.Equal((uint)0x00003004, machine.CPU.ReadAddressRegister(2));
		}

		[Fact]
		public void MOVEA()
		{
			// Arrange
			ushort[] code = new ushort[] { 0x3246, 0x285A };  // movea.w d6,a1 | movea.l (a2)+,a4
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D6 = 0x12345678,
				A1 = 0xAA55AA55,
				A2 = 0x00003000,
				A4 = 0x55555555
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { 0x8055, 0x40AA };
			machine.LoadData(data, 0x00003000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal((uint)0xAA555678, machine.CPU.ReadAddressRegister(1));
			Assert.Equal((uint)0x805540AA, machine.CPU.ReadAddressRegister(4));
			Assert.Equal((uint)0x00003004, machine.CPU.ReadAddressRegister(2));
		}

		[Fact]
		public void MOVEfromSR()
		{
			// Arrange
			ushort[] code = new ushort[] { 0x40C1 };  // move sr,d1
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D1 = 0x12345678,
				SR = SRFlags.Overflow | SRFlags.Zero | SRFlags.SupervisorMode
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal((uint)0x12342006, machine.CPU.ReadDataRegister(1));
		}

		[Fact]
		public void MOVEtoCCR()
		{
			// Arrange
			ushort[] code = new ushort[] { 0x44DA };  // move (a2)+,ccr
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				A2 = 0x00003000,
				SR = SRFlags.Overflow | SRFlags.Zero | SRFlags.SupervisorMode
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { 0x1584 };
			machine.LoadData(data, 0x00003000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal((ushort)0x2015, (ushort)machine.CPU.SR);
			Assert.Equal((uint)0x00003001, machine.CPU.ReadAddressRegister(2));
		}

		[Fact]
		public void MOVEtoSR()
		{
			// Arrange
			ushort[] code = new ushort[] { 0x46F8, 0x2000 };  // move ($2000),sr
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				SR = SRFlags.Overflow | SRFlags.Zero
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { 0x2012 };
			machine.LoadData(data, 0x00002000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal((ushort)0x2012, (ushort)machine.CPU.SR);
			Assert.True(machine.CPU.SupervisorMode);
			Assert.True(machine.CPU.ExtendFlag);
			Assert.True(machine.CPU.OverflowFlag);
			Assert.False(machine.CPU.ZeroFlag);
			Assert.False(machine.CPU.NegativeFlag);
			Assert.False(machine.CPU.CarryFlag);
			Assert.False(machine.CPU.TraceMode);
		}

		[Theory]
		[InlineData(0x00000000, OpSize.Byte, (SRFlags)0, 0x00000000)]
		[InlineData(0x00000000, OpSize.Byte, SRFlags.Extend, 0x000000FF)]
		[InlineData(0x00000070, OpSize.Byte, (SRFlags)0, 0x00000090)]
		[InlineData(0x00000070, OpSize.Byte, SRFlags.Extend, 0x0000008F)]
		[InlineData(0x00000000, OpSize.Word, (SRFlags)0, 0x00000000)]
		[InlineData(0x00000000, OpSize.Word, SRFlags.Extend, 0x0000FFFF)]
		[InlineData(0x00007070, OpSize.Word, (SRFlags)0, 0x00008F90)]
		[InlineData(0x00007070, OpSize.Word, SRFlags.Extend, 0x00008F8F)]
		[InlineData(0x00000000, OpSize.Long, (SRFlags)0, 0x00000000)]
		[InlineData(0x00000000, OpSize.Long, SRFlags.Extend, 0xFFFFFFFF)]
		[InlineData(0x80807070, OpSize.Long, (SRFlags)0, 0x7F7F8F90)]
		[InlineData(0x80807070, OpSize.Long, SRFlags.Extend, 0x7F7F8F8F)]
		public void NEGX(uint value, OpSize size, SRFlags initFlags, uint expectedResult)
		{
			ushort[] code = size switch
			{
				OpSize.Byte => new ushort[] { 0x4001 },		// negx.b d1
				OpSize.Word => new ushort[] { 0x4041 },		// negx.w d1
				_ => new ushort[] { 0x4081 },               // negx.l d1
			};
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D1 = value,
				SR = initFlags
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(1));
		}

		[Theory]
		[InlineData(0x00000000, OpSize.Byte, (SRFlags)0, SRFlags.Zero)]
		[InlineData(0x00000000, OpSize.Byte, SRFlags.Extend, SRFlags.Negative | SRFlags.Carry | SRFlags.Extend)]
		[InlineData(0x00000070, OpSize.Byte, (SRFlags)0, SRFlags.Negative | SRFlags.Carry | SRFlags.Extend)]
		[InlineData(0x00000090, OpSize.Byte, SRFlags.Extend, SRFlags.Carry | SRFlags.Extend)]
		[InlineData(0x00000000, OpSize.Word, (SRFlags)0, SRFlags.Zero)]
		[InlineData(0x00000000, OpSize.Word, SRFlags.Extend, SRFlags.Negative | SRFlags.Carry | SRFlags.Extend)]
		[InlineData(0x00007070, OpSize.Word, (SRFlags)0, SRFlags.Negative | SRFlags.Carry | SRFlags.Extend)]
		[InlineData(0x00008070, OpSize.Word, SRFlags.Extend, SRFlags.Carry | SRFlags.Extend)]
		[InlineData(0x00000000, OpSize.Long, (SRFlags)0, SRFlags.Zero)]
		[InlineData(0x00000000, OpSize.Long, SRFlags.Extend, SRFlags.Negative | SRFlags.Carry | SRFlags.Extend)]
		[InlineData(0x70807070, OpSize.Long, (SRFlags)0, SRFlags.Negative | SRFlags.Carry | SRFlags.Extend)]
		[InlineData(0x80807070, OpSize.Long, SRFlags.Extend, SRFlags.Carry | SRFlags.Extend)]
		public void NEGX_FlagsTest(uint value, OpSize size, SRFlags initFlags, SRFlags expectedFlags)
		{
			ushort[] code = size switch
			{
				OpSize.Byte => new ushort[] { 0x4001 },     // negx.b d1
				OpSize.Word => new ushort[] { 0x4041 },     // negx.w d1
				_ => new ushort[] { 0x4081 },               // negx.l d1
			};
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D1 = value,
				SR = initFlags
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Fact]
		public void CLR()
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { 0x4207, 0x4252, 0x42B8, 0x2000 };  // clr.b d7 | clr.w (a2) | clr.l ($2000)
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D7 = 0x12345678,
				A2 = 0x00003000
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { 0xAAAA, 0x5555 };
			machine.LoadData(data, 0x00002000, false);
			data = new ushort[] { 0x2021 };
			machine.LoadData(data, 0x00003000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal((uint)0x12345600, machine.CPU.ReadDataRegister(7));
			Assert.Equal((ushort)0x0000, machine.Memory.ReadWord(0x00003000));
			Assert.Equal((uint)0x00000000, machine.Memory.ReadLong(0x00002000));
		}

		[Theory]
		[InlineData(0x00000000, OpSize.Byte, 0x00000000, SRFlags.Zero)]
		[InlineData(0x00000070, OpSize.Byte, 0x00000090, SRFlags.Negative | SRFlags.Carry | SRFlags.Extend)]
		[InlineData(0x00000000, OpSize.Word, 0x00000000, SRFlags.Zero)]
		[InlineData(0x00007070, OpSize.Word, 0x00008F90, SRFlags.Negative | SRFlags.Carry | SRFlags.Extend)]
		[InlineData(0x00008070, OpSize.Word, 0x00007F90, SRFlags.Carry | SRFlags.Extend)]
		[InlineData(0x00000000, OpSize.Long, 0x00000000, SRFlags.Zero)]
		[InlineData(0x70807070, OpSize.Long, 0x8F7F8F90, SRFlags.Negative | SRFlags.Carry | SRFlags.Extend)]
		[InlineData(0x80807070, OpSize.Long, 0x7F7F8F90, SRFlags.Carry | SRFlags.Extend)]
		public void NEG(uint value, OpSize size, uint expectedResult, SRFlags expectedFlags)
		{
			ushort[] code = size switch
			{
				OpSize.Byte => new ushort[] { 0x4401 },     // neg.b d1
				OpSize.Word => new ushort[] { 0x4441 },     // neg.w d1
				_ => new ushort[] { 0x4481 },               // neg.l d1
			};
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D1 = value
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(1));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(0x00000000, OpSize.Byte, 0x000000FF, SRFlags.Negative)]
		[InlineData(0x000000FF, OpSize.Byte, 0x00000000, SRFlags.Zero)]
		[InlineData(0x00000080, OpSize.Byte, 0x0000007F, (SRFlags)0)]
		[InlineData(0x00000000, OpSize.Word, 0x0000FFFF, SRFlags.Negative)]
		[InlineData(0x0000FFFF, OpSize.Word, 0x00000000, SRFlags.Zero)]
		[InlineData(0x00008000, OpSize.Word, 0x00007FFF, (SRFlags)0)]
		[InlineData(0x00000000, OpSize.Long, 0xFFFFFFFF, SRFlags.Negative)]
		[InlineData(0xFFFFFFFF, OpSize.Long, 0x00000000, SRFlags.Zero)]
		[InlineData(0x80000000, OpSize.Long, 0x7FFFFFFF, (SRFlags)0)]
		public void NOT(uint value, OpSize size, uint expectedResult, SRFlags expectedFlags)
		{
			ushort[] code = size switch
			{
				OpSize.Byte => new ushort[] { 0x4601 },     // not.b d1
				OpSize.Word => new ushort[] { 0x4641 },     // not.w d1
				_ => new ushort[] { 0x4681 },               // not.l d1
			};
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D1 = value
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(1));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(0x00000000, OpSize.Word, 0x00000000, SRFlags.Zero)]
		[InlineData(0x00000080, OpSize.Word, 0x0000FF80, SRFlags.Negative)]
		[InlineData(0x0000007F, OpSize.Word, 0x0000007F, (SRFlags)0)]
		[InlineData(0x12345680, OpSize.Word, 0x1234FF80, SRFlags.Negative)]
		[InlineData(0x00000000, OpSize.Long, 0x00000000, SRFlags.Zero)]
		[InlineData(0x00008000, OpSize.Long, 0xFFFF8000, SRFlags.Negative)]
		[InlineData(0x00007F00, OpSize.Long, 0x00007F00, (SRFlags)0)]
		public void EXT(uint value, OpSize size, uint expectedResult, SRFlags expectedFlags)
		{
			ushort[] code = size switch
			{
				OpSize.Word => new ushort[] { 0x4881 },     // ext.w d1
				_ => new ushort[] { 0x48C1 },               // ext.l d1
			};
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D1 = value
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(1));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(0x00000000, 0x00000000, SRFlags.Zero)]
		[InlineData(0x12348080, 0x80801234, SRFlags.Negative)]
		[InlineData(0x12345678, 0x56781234, (SRFlags)0)]
		public void SWAP(uint value, uint expectedResult, SRFlags expectedFlags)
		{
			ushort[] code = new ushort[] { 0x4846 };		// swap d6
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D6 = value
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(6));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Fact]
		public void PEA()
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { 0x4851 };  // pea (a1)
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				A1 = 0x00654321,
				USP = 0x00003000
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal((uint)0x00002FFC, machine.CPU.ReadAddressRegister(7));
			Assert.Equal((uint)0x00654321, machine.Memory.ReadLong(0x00002FFC));
		}

		[Fact]
		public void ILLEGAL()
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { 0x4AFC };  // illegal
			machine.LoadExecutableData(code, 0x0200);

			// Act and Assert
			Assert.Throws<TrapException>(() => machine.Execute());
		}

		[Theory]
		[InlineData(0x12121200, OpSize.Byte, SRFlags.Zero)]
		[InlineData(0x121212FF, OpSize.Byte, SRFlags.Negative)]
		[InlineData(0x1212127F, OpSize.Byte, (SRFlags)0)]
		[InlineData(0x12120000, OpSize.Word, SRFlags.Zero)]
		[InlineData(0x1212FFFF, OpSize.Word, SRFlags.Negative)]
		[InlineData(0x12127FFF, OpSize.Word, (SRFlags)0)]
		[InlineData(0x00000000, OpSize.Long, SRFlags.Zero)]
		[InlineData(0xFFFFFFFF, OpSize.Long, SRFlags.Negative)]
		[InlineData(0x70000000, OpSize.Long, (SRFlags)0)]
		public void TST(uint value, OpSize size, SRFlags expectedFlags)
		{
			ushort[] code = size switch
			{
				OpSize.Byte => new ushort[] { 0x4A05 },     // tst.b d5
				OpSize.Word => new ushort[] { 0x4A45 },     // tst.w d5
				_ => new ushort[] { 0x4A85 },               // tst.l d5
			};
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D5 = value
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Fact]
		public void TRAP()
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { 0x4E4A };  // trap #10
			machine.LoadExecutableData(code, 0x0200);

			// Act and Assert
			Assert.Throws<TrapException>(() => machine.Execute());
		}

		[Fact]
		public void MOVEUSP()
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { 0x4E61 };  // move a1,usp
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				A1 = 0x00654321,
				USP = 0x00003000,
				SR = SRFlags.SupervisorMode
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal((uint)0x00654321, machine.CPU.USP);
		}

		[Fact]
		public void MOVEUSP_UserMode()
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { 0x4E61 };  // move a1,usp
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				A1 = 0x00654321,
				USP = 0x00003000
			};
			machine.SetCPUState(initState);

			// Act and Assert
			Assert.Throws<TrapException>(() => machine.Execute());
		}

		[Fact]
		public void MOVEUSP_FromUSPtoReg()
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { 0x4E69 };  // move usp,a1
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				A1 = 0x00654321,
				USP = 0x00003000,
				SR = SRFlags.SupervisorMode
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal((uint)0x00003000, machine.CPU.ReadAddressRegister(1));
		}

		[Fact]
		public void RESET_UserMode()
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { 0x4E70 };  // reset
			machine.LoadExecutableData(code, 0x0200);

			// Act and Assert
			Assert.Throws<TrapException>(() => machine.Execute());
		}

		[Fact]
		public void RTE()
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { 0x4E73 };  // rte
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				SSP = 0x00003000,
				SR = SRFlags.SupervisorMode
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { 0x2012, 0x0001, 0x2244 };
			machine.LoadData(data, 0x00003000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal((uint)0x00003006, machine.CPU.SSP);
			Assert.Equal(SRFlags.SupervisorMode | SRFlags.Extend | SRFlags.Overflow, machine.CPU.SR);
			Assert.Equal((uint)0x00012244, machine.CPU.PC);
		}

		[Fact]
		public void RTE_UserMode()
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { 0x4E73 };  // rte
			machine.LoadExecutableData(code, 0x0200);

			// Act and Assert
			Assert.Throws<TrapException>(() => machine.Execute());
		}

		[Fact]
		public void RTS()
		{
			Machine machine = new Machine();
			machine.ExecutionHandler._numberOfJSRCalls = 1;			// To ensure that the RTS is actually performed.

			ushort[] code = new ushort[] { 0x4E75 };  // rts
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				USP = 0x00003000,
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { 0x0002, 0x2468 };
			machine.LoadData(data, 0x00003000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal((uint)0x00003004, machine.CPU.USP);
			Assert.Equal((uint)0x00022468, machine.CPU.PC);
		}

		[Theory]
		[InlineData(SRFlags.Overflow, true)]
		[InlineData((SRFlags)0, false)]
		public void TRAPV(SRFlags initFlags, bool shouldThrow)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { 0x4E76 };  // trapv
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				SR = initFlags,
			};
			machine.SetCPUState(initState);

			// Act
			if (!shouldThrow)
			{
				// Shouldn't throw an exception so just execute the code. If it does throw then the test will fail.
				machine.Execute();
			}
			else
			{
				// We're expecting the code to throw an exception .
				Assert.Throws<TrapException>(() => machine.Execute());
			}
		}

		[Fact]
		public void RTR()
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { 0x4E77 };  // rtr
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				SSP = 0x00003000,
				SR = SRFlags.SupervisorMode | SRFlags.Carry
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { 0x0012, 0x0002, 0x4568 };
			machine.LoadData(data, 0x00003000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal((uint)0x00003006, machine.CPU.SSP);
			Assert.Equal(SRFlags.SupervisorMode | SRFlags.Extend | SRFlags.Overflow, machine.CPU.SR);
			Assert.Equal((uint)0x00024568, machine.CPU.PC);
		}

		[Fact]
		public void JSR()
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { 0x4E91 };  // jsr (a1)
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				USP = 0x00003000,
				A1 = 0x00024680
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal((uint)0x00002FFC, machine.CPU.USP);
			Assert.Equal((uint)0x00024680, machine.CPU.PC);
			Assert.Equal((uint)0x00000202, machine.Memory.ReadLong(0x00002FFC));
		}

		[Fact]
		public void JMP()
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { 0x4EF9, 0x0001, 0x4466 };  // jmp ($00014466)
			machine.LoadExecutableData(code, 0x0200);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal((uint)0x00014466, machine.CPU.PC);
		}

		[Fact]
		public void LEA()
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { 0x43EA, 0x0040 };  // lea ($40,a2),a1
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				A2 = 0x00016666
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal((uint)0x000166A6, machine.CPU.ReadAddressRegister(1));
		}

		[Theory]
		[InlineData(0x00008000, true, true)]
		[InlineData(0x00000800, true, false)]
		[InlineData(0x00000000, false, false)]
		[InlineData(0x00000100, false, false)]
		[InlineData(0x00000400, false, false)]
		public void CHK(uint value, bool shouldThrow, bool expectedNegFlag)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { 0x4181 };  // chk d1,d0
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D0 = value,
				D1 = 0x00000400
			};
			machine.SetCPUState(initState);

			// Act
			if (!shouldThrow)
			{
				// Shouldn't throw an exception so just execute the code. If it does throw then the test will fail.
				machine.Execute();
			}
			else
			{
				// We're expecting the code to throw an exception .
				Assert.Throws<TrapException>(() => machine.Execute());
				Assert.Equal(expectedNegFlag, machine.CPU.NegativeFlag);
			}
		}

		[Theory]
		[InlineData(new ushort[] { 0x5200 }, 0x7FFF7F7E, 0x7FFF7F7F, (SRFlags)0)]     // addq.b #1,d0
		[InlineData(new ushort[] { 0x5200 }, 0x7FFF7FFF, 0x7FFF7F00, SRFlags.Zero | SRFlags.Carry | SRFlags.Extend)]     // addq.b #1,d0
		[InlineData(new ushort[] { 0x5440 }, 0x7FFF7FFE, 0x7FFF8000, SRFlags.Negative | SRFlags.Overflow)]     // addq.w #2,d0
		[InlineData(new ushort[] { 0x5440 }, 0x7FFFFFFE, 0x7FFF0000, SRFlags.Zero | SRFlags.Carry | SRFlags.Extend)]     // addq.w #2,d0
		[InlineData(new ushort[] { 0x5080 }, 0x7FFF7FFE, 0x7FFF8006, (SRFlags)0)]     // addq.l #8,d0
		[InlineData(new ushort[] { 0x5080 }, 0x7FFFFFF8, 0x80000000, SRFlags.Negative | SRFlags.Overflow)]     // addq.l #8,d0
		[InlineData(new ushort[] { 0x5080 }, 0xFFFFFFF8, 0x00000000, SRFlags.Zero | SRFlags.Carry | SRFlags.Extend)]     // addq.l #8,d0
		public void ADDQ(ushort[] code, uint initValue, uint expectedResult, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D0 = initValue
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(0));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(new ushort[] { 0x5449 }, 0x7FFF7FFE, 0x7FFF8000)]     // addq.w #2,a1
		[InlineData(new ushort[] { 0x5449 }, 0x7FFFFFFE, 0x80000000)]     // addq.w #2,a1
		[InlineData(new ushort[] { 0x5649 }, 0xFFFFFFFD, 0x00000000)]     // addq.w #3,a1
		[InlineData(new ushort[] { 0x5089 }, 0x7FFF7FFE, 0x7FFF8006)]     // addq.l #8,a1
		[InlineData(new ushort[] { 0x5089 }, 0x7FFFFFF8, 0x80000000)]     // addq.l #8,a1
		[InlineData(new ushort[] { 0x5E89 }, 0xFFFFFFF9, 0x00000000)]     // addq.l #7,a1
		public void ADDQ_AddressReg(ushort[] code, uint initValue, uint expectedResult)
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				A1 = initValue
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.ReadAddressRegister(1));
			Assert.Equal((SRFlags)0, machine.CPU.SR);
		}

		[Theory]
		[InlineData(new ushort[] { 0x5304 }, 0x7FFF7F7E, 0x7FFF7F7D, (SRFlags)0)]     // subq.b #1,d4
		[InlineData(new ushort[] { 0x5304 }, 0x7FFF7F00, 0x7FFF7FFF, SRFlags.Negative | SRFlags.Carry | SRFlags.Extend)]     // subq.b #1,d4
		[InlineData(new ushort[] { 0x5304 }, 0x7FFF7F01, 0x7FFF7F00, SRFlags.Zero)]     // subq.b #1,d4
		[InlineData(new ushort[] { 0x5744 }, 0x7FFF7FFE, 0x7FFF7FFB, (SRFlags)0)]     // subq.w #3,d4
		[InlineData(new ushort[] { 0x5744 }, 0x7FFF0002, 0x7FFFFFFF, SRFlags.Negative | SRFlags.Carry | SRFlags.Extend)]     // subq.w #3,d4
		[InlineData(new ushort[] { 0x5744 }, 0x7FFF0003, 0x7FFF0000, SRFlags.Zero)]     // subq.w #3,d4
		[InlineData(new ushort[] { 0x5184 }, 0x7FFF7FFE, 0x7FFF7FF6, (SRFlags)0)]     // subq.l #8,d4
		[InlineData(new ushort[] { 0x5184 }, 0x80000006, 0x7FFFFFFE, SRFlags.Carry | SRFlags.Extend)]     // subq.l #8,d4
		[InlineData(new ushort[] { 0x5184 }, 0x00000007, 0xFFFFFFFF, SRFlags.Negative | SRFlags.Carry | SRFlags.Extend)]     // subq.l #8,d4
		[InlineData(new ushort[] { 0x5184 }, 0x00000008, 0x00000000, SRFlags.Zero)]     // subq.l #8,d4
		public void SUBQ(ushort[] code, uint initValue, uint expectedResult, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D4 = initValue
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(4));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(new ushort[] { 0x574C }, 0x7FFF8000, 0x7FFF7FFD)]     // subq.w #3,a4
		[InlineData(new ushort[] { 0x574C }, 0x80000000, 0x7FFFFFFD)]     // subq.w #3,a4
		[InlineData(new ushort[] { 0x594C }, 0x00000003, 0xFFFFFFFF)]     // subq.w #4,a4
		[InlineData(new ushort[] { 0x518C }, 0x7FFF8000, 0x7FFF7FF8)]     // subq.l #8,a4
		[InlineData(new ushort[] { 0x518C }, 0x80000000, 0x7FFFFFF8)]     // subq.l #8,a4
		[InlineData(new ushort[] { 0x5F8C }, 0x00000006, 0xFFFFFFFF)]     // subq.l #7,a4
		public void SUBQ_AddressReg(ushort[] code, uint initValue, uint expectedResult)
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				A4 = initValue
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.ReadAddressRegister(4));
			Assert.Equal((SRFlags)0, machine.CPU.SR);
		}

		[Theory]
		[InlineData(new ushort[] { 0x50C4 }, (SRFlags)0, 0xFF)]										 // st d4
		[InlineData(new ushort[] { 0x51C4 }, (SRFlags)0, 0x00)]										 // sf d4
		[InlineData(new ushort[] { 0x54C4 }, (SRFlags)0, 0xFF)]										 // scc d4
		[InlineData(new ushort[] { 0x54C4 }, SRFlags.Carry, 0x00)]									 // scc d4
		[InlineData(new ushort[] { 0x55C4 }, (SRFlags)0, 0x00)]										 // scs d4
		[InlineData(new ushort[] { 0x55C4 }, SRFlags.Carry, 0xFF)]									 // scs d4
		[InlineData(new ushort[] { 0x57C4 }, (SRFlags)0, 0x00)]										 // seq d4
		[InlineData(new ushort[] { 0x57C4 }, SRFlags.Zero, 0xFF)]									 // seq d4
		[InlineData(new ushort[] { 0x5CC4 }, (SRFlags)0, 0xFF)]										 // sge d4
		[InlineData(new ushort[] { 0x5CC4 }, SRFlags.Negative, 0x00)]								 // sge d4
		[InlineData(new ushort[] { 0x5CC4 }, SRFlags.Negative | SRFlags.Overflow, 0xFF)]             // sge d4
		[InlineData(new ushort[] { 0x5EC4 }, (SRFlags)0, 0xFF)]										 // sgt d4
		[InlineData(new ushort[] { 0x5EC4 }, SRFlags.Zero, 0x00)]									 // sgt d4
		[InlineData(new ushort[] { 0x5EC4 }, SRFlags.Negative, 0x00)]								 // sgt d4
		[InlineData(new ushort[] { 0x5EC4 }, SRFlags.Negative | SRFlags.Overflow, 0xFF)]             // sgt d4
		[InlineData(new ushort[] { 0x52C4 }, (SRFlags)0, 0xFF)]										 // shi d4
		[InlineData(new ushort[] { 0x52C4 }, SRFlags.Zero, 0x00)]									 // shi d4
		[InlineData(new ushort[] { 0x52C4 }, SRFlags.Carry, 0x00)]									 // shi d4
		[InlineData(new ushort[] { 0x5FC4 }, (SRFlags)0, 0x00)]										 // sle d4
		[InlineData(new ushort[] { 0x5FC4 }, SRFlags.Zero, 0xFF)]									 // sle d4
		[InlineData(new ushort[] { 0x5FC4 }, SRFlags.Negative, 0xFF)]								 // sle d4
		[InlineData(new ushort[] { 0x53C4 }, (SRFlags)0, 0x00)]										 // sls d4
		[InlineData(new ushort[] { 0x53C4 }, SRFlags.Zero, 0xFF)]									 // sls d4
		[InlineData(new ushort[] { 0x53C4 }, SRFlags.Carry, 0xFF)]									 // sls d4
		[InlineData(new ushort[] { 0x5DC4 }, (SRFlags)0, 0x00)]										 // slt d4
		[InlineData(new ushort[] { 0x5DC4 }, SRFlags.Negative, 0xFF)]								 // slt d4
		[InlineData(new ushort[] { 0x5DC4 }, SRFlags.Negative | SRFlags.Overflow, 0x00)]             // slt d4
		[InlineData(new ushort[] { 0x5BC4 }, (SRFlags)0, 0x00)]										 // smi d4
		[InlineData(new ushort[] { 0x5BC4 }, SRFlags.Negative, 0xFF)]								 // smi d4
		[InlineData(new ushort[] { 0x56C4 }, (SRFlags)0, 0xFF)]										 // sne d4
		[InlineData(new ushort[] { 0x56C4 }, SRFlags.Zero, 0x00)]									 // sne d4
		[InlineData(new ushort[] { 0x5AC4 }, (SRFlags)0, 0xFF)]										 // spl d4
		[InlineData(new ushort[] { 0x5AC4 }, SRFlags.Negative, 0x00)]								 // spl d4
		[InlineData(new ushort[] { 0x58C4 }, (SRFlags)0, 0xFF)]										 // svc d4
		[InlineData(new ushort[] { 0x58C4 }, SRFlags.Overflow, 0x00)]								 // svc d4
		[InlineData(new ushort[] { 0x59C4 }, (SRFlags)0, 0x00)]										 // svs d4
		[InlineData(new ushort[] { 0x59C4 }, SRFlags.Overflow, 0xFF)]								 // svs d4
		public void Scc(ushort[] code, SRFlags initFlags, byte expectedResult)
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D4 = 0x12345678,
				SR = initFlags
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, (byte)(machine.CPU.ReadDataRegister(4) & 0x000000FF));
		}

		[Theory]
		[InlineData(new ushort[] { 0x50CC, 0x000E }, 0x01, (SRFlags)0, 0x0204)]                                      // dbt d4,$0210
		[InlineData(new ushort[] { 0x50CC, 0x000E }, 0x00, (SRFlags)0, 0x0204)]                                      // dbt d4,$0210
		[InlineData(new ushort[] { 0x51CC, 0x000E }, 0x01, (SRFlags)0, 0x0210)]                                      // dbf d4,$0210
		[InlineData(new ushort[] { 0x51CC, 0xFFEE }, 0x01, (SRFlags)0, 0x01F0)]                                      // dbf d4,$0210
		[InlineData(new ushort[] { 0x54CC, 0x000E }, 0x01, (SRFlags)0, 0x0204)]                                      // dbcc d4,$0210
		[InlineData(new ushort[] { 0x54CC, 0x000E }, 0x01, SRFlags.Carry, 0x0210)]                                   // dbcc d4,$0210
		[InlineData(new ushort[] { 0x54CC, 0x000E }, 0x00, SRFlags.Carry, 0x0204)]                                   // dbcc d4,$0210
		[InlineData(new ushort[] { 0x55CC, 0x000E }, 0x01, (SRFlags)0, 0x0210)]                                      // dbcs d4,$0210
		[InlineData(new ushort[] { 0x55CC, 0x000E }, 0x01, SRFlags.Carry, 0x0204)]                                   // dbcs d4,$0210
		[InlineData(new ushort[] { 0x57CC, 0x000E }, 0x01, (SRFlags)0, 0x0210)]                                      // dbeq d4,$0210
		[InlineData(new ushort[] { 0x57CC, 0x000E }, 0x01, SRFlags.Zero, 0x0204)]                                    // dbeq d4,$0210
		[InlineData(new ushort[] { 0x5CCC, 0x000E }, 0x01, (SRFlags)0, 0x0204)]                                      // dbge d4,$0210
		[InlineData(new ushort[] { 0x5CCC, 0x000E }, 0x01, SRFlags.Negative, 0x0210)]                                // dbge d4,$0210
		[InlineData(new ushort[] { 0x5CCC, 0x000E }, 0x01, SRFlags.Negative | SRFlags.Overflow, 0x0204)]             // dbge d4,$0210
		[InlineData(new ushort[] { 0x5ECC, 0x000E }, 0x01, (SRFlags)0, 0x0204)]                                      // dbgt d4,$0210
		[InlineData(new ushort[] { 0x5ECC, 0x000E }, 0x01, SRFlags.Zero, 0x0210)]                                    // dbgt d4,$0210
		[InlineData(new ushort[] { 0x5ECC, 0x000E }, 0x01, SRFlags.Negative, 0x0210)]                                // dbgt d4,$0210
		[InlineData(new ushort[] { 0x5ECC, 0x000E }, 0x01, SRFlags.Negative | SRFlags.Overflow, 0x0204)]             // dbgt d4,$0210
		[InlineData(new ushort[] { 0x52CC, 0x000E }, 0x01, (SRFlags)0, 0x0204)]                                      // dbhi d4,$0210
		[InlineData(new ushort[] { 0x52CC, 0x000E }, 0x01, SRFlags.Zero, 0x0210)]                                    // dbhi d4,$0210
		[InlineData(new ushort[] { 0x52CC, 0x000E }, 0x01, SRFlags.Carry, 0x0210)]                                   // dbhi d4,$0210
		[InlineData(new ushort[] { 0x5FCC, 0x000E }, 0x01, (SRFlags)0, 0x0210)]                                      // dble d4,$0210
		[InlineData(new ushort[] { 0x5FCC, 0x000E }, 0x01, SRFlags.Zero, 0x0204)]                                    // dble d4,$0210
		[InlineData(new ushort[] { 0x5FCC, 0x000E }, 0x01, SRFlags.Negative, 0x0204)]                                // dble d4,$0210
		[InlineData(new ushort[] { 0x53CC, 0x000E }, 0x01, (SRFlags)0, 0x0210)]                                      // dbls d4,$0210
		[InlineData(new ushort[] { 0x53CC, 0x000E }, 0x01, SRFlags.Zero, 0x0204)]                                    // dbls d4,$0210
		[InlineData(new ushort[] { 0x53CC, 0x000E }, 0x01, SRFlags.Carry, 0x0204)]                                   // dbls d4,$0210
		[InlineData(new ushort[] { 0x5DCC, 0x000E }, 0x01, (SRFlags)0, 0x0210)]                                      // dblt d4,$0210
		[InlineData(new ushort[] { 0x5DCC, 0x000E }, 0x01, SRFlags.Negative, 0x0204)]                                // dblt d4,$0210
		[InlineData(new ushort[] { 0x5DCC, 0x000E }, 0x01, SRFlags.Negative | SRFlags.Overflow, 0x0210)]             // dblt d4,$0210
		[InlineData(new ushort[] { 0x5BCC, 0x000E }, 0x01, (SRFlags)0, 0x0210)]                                      // dbmi d4,$0210
		[InlineData(new ushort[] { 0x5BCC, 0x000E }, 0x01, SRFlags.Negative, 0x0204)]                                // dbmi d4,$0210
		[InlineData(new ushort[] { 0x56CC, 0x000E }, 0x01, (SRFlags)0, 0x0204)]                                      // dbne d4,$0210
		[InlineData(new ushort[] { 0x56CC, 0x000E }, 0x01, SRFlags.Zero, 0x0210)]                                    // dbne d4,$0210
		[InlineData(new ushort[] { 0x5ACC, 0x000E }, 0x01, (SRFlags)0, 0x0204)]                                      // dbpl d4,$0210
		[InlineData(new ushort[] { 0x5ACC, 0x000E }, 0x01, SRFlags.Negative, 0x0210)]                                // dbpl d4,$0210
		[InlineData(new ushort[] { 0x58CC, 0x000E }, 0x01, (SRFlags)0, 0x0204)]                                      // dbvc d4,$0210
		[InlineData(new ushort[] { 0x58CC, 0x000E }, 0x01, SRFlags.Overflow, 0x0210)]                                // dbvc d4,$0210
		[InlineData(new ushort[] { 0x59CC, 0x000E }, 0x01, (SRFlags)0, 0x0210)]                                      // dbvs d4,$0210
		[InlineData(new ushort[] { 0x59CC, 0x000E }, 0x01, SRFlags.Overflow, 0x0204)]                                // dbvs d4,$0210
		public void DBcc(ushort[] code, byte initD4, SRFlags initFlags, uint expectedPC)
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D4 = initD4,
				SR = initFlags
			};
			machine.SetCPUState(initState);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedPC, machine.CPU.PC);
		}

		[Theory]
		[InlineData(new ushort[] { 0x607E }, 0x0280)]                  // bra $0280
		[InlineData(new ushort[] { 0x6000, 0xDFE }, 0x1000)]           // bra $1000
		public void BRA(ushort[] code, uint expectedPC)
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedPC, machine.CPU.PC);
		}

		[Theory]
		[InlineData(new ushort[] { 0x617E }, 0x0280, 0x0202)]                  // bsr $0280
		[InlineData(new ushort[] { 0x6100, 0xDFE }, 0x1000, 0x0204)]           // bsr $1000
		public void BSR(ushort[] code, uint expectedPC, uint expectedStackedPC)
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				USP = 0x00003000
			};
			machine.SetCPUState(initState);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedPC, machine.CPU.PC);
			Assert.Equal((uint)0x00002FFC, machine.CPU.USP);
			Assert.Equal(expectedStackedPC, machine.Memory.ReadLong(0x00002FFC));

		}

		[Theory]
		[InlineData(new ushort[] { 0x640E }, (SRFlags)0, 0x0210)]                                      // bcc $0210
		[InlineData(new ushort[] { 0x640E }, SRFlags.Carry, 0x0202)]                                   // bcc $0210
		[InlineData(new ushort[] { 0x650E }, (SRFlags)0, 0x0202)]                                      // bcs $0210
		[InlineData(new ushort[] { 0x650E }, SRFlags.Carry, 0x0210)]                                   // bcs $0210
		[InlineData(new ushort[] { 0x670E }, (SRFlags)0, 0x0202)]                                      // beq $0210
		[InlineData(new ushort[] { 0x670E }, SRFlags.Zero, 0x0210)]                                    // beq $0210
		[InlineData(new ushort[] { 0x6C0E }, (SRFlags)0, 0x0210)]                                      // bge $0210
		[InlineData(new ushort[] { 0x6C0E }, SRFlags.Negative, 0x0202)]                                // bge $0210
		[InlineData(new ushort[] { 0x6C0E }, SRFlags.Negative | SRFlags.Overflow, 0x0210)]             // bge $0210
		[InlineData(new ushort[] { 0x6E0E }, (SRFlags)0, 0x0210)]                                      // bgt $0210
		[InlineData(new ushort[] { 0x6E0E }, SRFlags.Zero, 0x0202)]                                    // bgt $0210
		[InlineData(new ushort[] { 0x6E0E }, SRFlags.Negative, 0x0202)]                                // bgt $0210
		[InlineData(new ushort[] { 0x6E0E }, SRFlags.Negative | SRFlags.Overflow, 0x0210)]             // bgt $0210
		[InlineData(new ushort[] { 0x620E }, (SRFlags)0, 0x0210)]                                      // bhi $0210
		[InlineData(new ushort[] { 0x620E }, SRFlags.Zero, 0x0202)]                                    // bhi $0210
		[InlineData(new ushort[] { 0x620E }, SRFlags.Carry, 0x0202)]                                   // bhi $0210
		[InlineData(new ushort[] { 0x6F0E }, (SRFlags)0, 0x0202)]                                      // ble $0210
		[InlineData(new ushort[] { 0x6F0E }, SRFlags.Zero, 0x0210)]                                    // ble $0210
		[InlineData(new ushort[] { 0x6F0E }, SRFlags.Negative, 0x0210)]                                // ble $0210
		[InlineData(new ushort[] { 0x630E }, (SRFlags)0, 0x0202)]                                      // bls $0210
		[InlineData(new ushort[] { 0x630E }, SRFlags.Zero, 0x0210)]                                    // bls $0210
		[InlineData(new ushort[] { 0x630E }, SRFlags.Carry, 0x0210)]                                   // bls $0210
		[InlineData(new ushort[] { 0x6D0E }, (SRFlags)0, 0x0202)]                                      // blt $0210
		[InlineData(new ushort[] { 0x6D0E }, SRFlags.Negative, 0x0210)]                                // blt $0210
		[InlineData(new ushort[] { 0x6D0E }, SRFlags.Negative | SRFlags.Overflow, 0x0202)]             // blt $0210
		[InlineData(new ushort[] { 0x6B0E }, (SRFlags)0, 0x0202)]                                      // bmi $0210
		[InlineData(new ushort[] { 0x6B0E }, SRFlags.Negative, 0x0210)]                                // bmi $0210
		[InlineData(new ushort[] { 0x660E }, (SRFlags)0, 0x0210)]                                      // bne $0210
		[InlineData(new ushort[] { 0x660E }, SRFlags.Zero, 0x0202)]                                    // bne $0210
		[InlineData(new ushort[] { 0x6A0E }, (SRFlags)0, 0x0210)]                                      // bpl $0210
		[InlineData(new ushort[] { 0x6A0E }, SRFlags.Negative, 0x0202)]                                // bpl $0210
		[InlineData(new ushort[] { 0x680E }, (SRFlags)0, 0x0210)]                                      // bvc $0210
		[InlineData(new ushort[] { 0x680E }, SRFlags.Overflow, 0x0202)]                                // bvc $0210
		[InlineData(new ushort[] { 0x690E }, (SRFlags)0, 0x0202)]                                      // bvs $0210
		[InlineData(new ushort[] { 0x690E }, SRFlags.Overflow, 0x0210)]                                // bvs $0210
		[InlineData(new ushort[] { 0x6400, 0x0DFE }, (SRFlags)0, 0x1000)]                              // bcc $1000
		[InlineData(new ushort[] { 0x6400, 0x0DFE }, SRFlags.Carry, 0x0204)]                           // bcc $1000
		[InlineData(new ushort[] { 0x6500, 0x0DFE }, (SRFlags)0, 0x0204)]                              // bcs $1000
		[InlineData(new ushort[] { 0x6500, 0x0DFE }, SRFlags.Carry, 0x1000)]                           // bcs $1000
		public void Bcc(ushort[] code, SRFlags initFlags, uint expectedPC)
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				SR = initFlags
			};
			machine.SetCPUState(initState);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedPC, machine.CPU.PC);
		}

		[Theory]
		[InlineData(new ushort[] { 0x7C50 }, (SRFlags)0, 0x00000050)]              // moveq #$50,d6
		[InlineData(new ushort[] { 0x7CF0 }, SRFlags.Negative, 0xFFFFFFF0)]        // moveq #$F0,d6
		[InlineData(new ushort[] { 0x7C00 }, SRFlags.Zero, 0x00000000)]            // moveq #$00,d6
		public void MOVEQ(ushort[] code, SRFlags expectedFlags, uint expectedResult)
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D6 = 0x12345678
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(6));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(0x00008000, 0x00000040, (SRFlags)0, 0x00000200)]
		[InlineData(0x00008030, 0x00000040, (SRFlags)0, 0x00300200)]
		[InlineData(0x04008000, 0x00000040, SRFlags.Overflow, 0x04008000)]
		[InlineData(0x00080000, 0x00000010, SRFlags.Negative, 0x00008000)]
		[InlineData(0x00000000, 0x00000010, SRFlags.Zero, 0x00000000)]
		[InlineData(0x00000002, 0x00000010, SRFlags.Zero, 0x00020000)]
		public void DIVU(uint d0Val, uint d1Val, SRFlags expectedFlags, uint expectedResult)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { 0x80C1 };  // divu d1,d0
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D0 = d0Val,
				D1 = d1Val
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(0));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Fact]
		public void DIVU_DivideByZero()
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { 0x80C1 };  // divu d1,d0
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D0 = 0x00001000,
				D1 = 0x00000000
			};
			machine.SetCPUState(initState);

			// Act and Assert
			Assert.Throws<TrapException>(() => machine.Execute());
		}

		[Theory]
		[InlineData(0x00008000, 0x00000040, (SRFlags)0, 0x00000200)]
		[InlineData(0x00008030, 0x00000040, (SRFlags)0, 0x00300200)]
		[InlineData(0x04008000, 0x00000040, SRFlags.Overflow, 0x04008000)]
		[InlineData(0xFFFFFFF8, 0x00000002, SRFlags.Negative, 0x0000FFFC)]
		[InlineData(0x00000000, 0x00000010, SRFlags.Zero, 0x00000000)]
		[InlineData(0x00000002, 0x00000010, SRFlags.Zero, 0x00020000)]
		public void DIVS(uint d0Val, uint d1Val, SRFlags expectedFlags, uint expectedResult)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { 0x81C1 };  // divs d1,d0
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D0 = d0Val,
				D1 = d1Val
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(0));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Fact]
		public void DIVS_DivideByZero()
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { 0x81C1 };  // divs d1,d0
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D0 = 0x00001000,
				D1 = 0x00000000
			};
			machine.SetCPUState(initState);

			// Act and Assert
			Assert.Throws<TrapException>(() => machine.Execute());
		}

		[Theory]
		[InlineData(0x8001, 0x00000000, 0x00000000, SRFlags.Zero, 0x00000000)]
		[InlineData(0x8001, 0x00000010, 0x00000040, (SRFlags)0, 0x00000050)]
		[InlineData(0x8001, 0x00000010, 0x00000081, SRFlags.Negative, 0x00000091)]
		[InlineData(0x8041, 0x00000000, 0x00000000, SRFlags.Zero, 0x00000000)]
		[InlineData(0x8041, 0x00002020, 0x00000202, (SRFlags)0, 0x00002222)]
		[InlineData(0x8041, 0x00001070, 0x00008043, SRFlags.Negative, 0x00009073)]
		[InlineData(0x8041, 0x12345678, 0x00002100, (SRFlags)0, 0x12347778)]
		[InlineData(0x8081, 0x00000000, 0x00000000, SRFlags.Zero, 0x00000000)]
		[InlineData(0x8081, 0x12345678, 0x65432100, (SRFlags)0, 0x77777778)]
		[InlineData(0x8081, 0x00000000, 0xF0000000, SRFlags.Negative, 0xF0000000)]
		public void OR(ushort opcode, uint d0Val, uint d1Val, SRFlags expectedFlags, uint expectedResult)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { opcode };
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D0 = d0Val,
				D1 = d1Val
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(0));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(0x9011, 0x00000000, 0x00000000, SRFlags.Zero, 0x00000000)]
		[InlineData(0x9011, 0x00000050, 0x40000000, (SRFlags)0, 0x00000010)]
		[InlineData(0x9011, 0x00000010, 0x80000000, SRFlags.Negative | SRFlags.Carry | SRFlags.Overflow | SRFlags.Extend, 0x00000090)]
		[InlineData(0x9051, 0x00000000, 0x00000000, SRFlags.Zero, 0x00000000)]
		[InlineData(0x9051, 0x00002020, 0x02020000, (SRFlags)0, 0x00001E1E)]
		[InlineData(0x9051, 0x00001070, 0x80430000, SRFlags.Negative | SRFlags.Carry | SRFlags.Overflow | SRFlags.Extend, 0x0000902D)]
		[InlineData(0x9051, 0x12345678, 0x21000000, (SRFlags)0, 0x12343578)]
		[InlineData(0x9091, 0x12345678, 0x12345678, SRFlags.Zero, 0x00000000)]
		[InlineData(0x9091, 0x12345678, 0x20000000, SRFlags.Negative | SRFlags.Carry | SRFlags.Extend, 0xF2345678)]
		[InlineData(0x9091, 0xF0000000, 0xE0000000, (SRFlags)0, 0x10000000)]
		public void SUB(ushort opcode, uint d0Val, uint a1MemVal, SRFlags expectedFlags, uint expectedResult)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { opcode };
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D0 = d0Val,
				A1 = 0x00002000
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { (ushort)((a1MemVal & 0xFFFF0000) >> 16), (ushort)(a1MemVal & 0xFFFF) };
			machine.LoadData(data, 0x00002000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(0));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(0x9509, 0x01, 0x00000000, 0x00000000, (SRFlags)0, SRFlags.Zero, 0x00000000)]
		[InlineData(0x9509, 0x01, 0x00000000, 0x00000001, SRFlags.Extend, SRFlags.Zero, 0x00000000)]
		[InlineData(0x9509, 0x01, 0x00000040, 0x00000050, (SRFlags)0, (SRFlags)0, 0x00000010)]
		[InlineData(0x9509, 0x01, 0x00000080, 0x00000010, (SRFlags)0, SRFlags.Negative | SRFlags.Carry | SRFlags.Overflow | SRFlags.Extend, 0x00000090)]
		[InlineData(0x9509, 0x01, 0x00000091, 0x00000010, SRFlags.Extend, SRFlags.Carry | SRFlags.Extend, 0x0000007E)]
		[InlineData(0x9549, 0x02, 0x00000000, 0x00000000, (SRFlags)0, SRFlags.Zero, 0x00000000)]
		[InlineData(0x9549, 0x02, 0x00000000, 0x00000001, SRFlags.Extend, SRFlags.Zero, 0x00000000)]
		[InlineData(0x9549, 0x02, 0x00000202, 0x00002020, (SRFlags)0, (SRFlags)0, 0x00001E1E)]
		[InlineData(0x9549, 0x02, 0x00008043, 0x00001070, SRFlags.Extend, SRFlags.Negative | SRFlags.Carry | SRFlags.Overflow | SRFlags.Extend, 0x0000902C)]
		[InlineData(0x9549, 0x02, 0x00002100, 0x12345678, SRFlags.Extend, (SRFlags)0, 0x12343577)]
		[InlineData(0x9589, 0x04, 0x12345678, 0x12345678, (SRFlags)0, SRFlags.Zero, 0x00000000)]
		[InlineData(0x9589, 0x04, 0x20000000, 0x12345678, (SRFlags)0, SRFlags.Negative | SRFlags.Carry | SRFlags.Extend, 0xF2345678)]
		[InlineData(0x9589, 0x04, 0xE0000000, 0xF0000000, SRFlags.Extend, (SRFlags)0, 0x0FFFFFFF)]
		public void SUBX(ushort opcode, byte numBytes, uint a1MemVal, uint a2MemVal, SRFlags initFlags, SRFlags expectedFlags, uint expectedResult)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { opcode };
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				A1 = 0x00002004,
				A2 = 0x00003004,
				SR = initFlags
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { (ushort)((a1MemVal & 0xFFFF0000) >> 16), (ushort)(a1MemVal & 0xFFFF) };
			machine.LoadData(data, 0x00002000, false);
			data = new ushort[] { (ushort)((a2MemVal & 0xFFFF0000) >> 16), (ushort)(a2MemVal & 0xFFFF) };
			machine.LoadData(data, 0x00003000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.Memory.ReadLong(0x0003000));
			Assert.Equal(expectedFlags, machine.CPU.SR);
			Assert.Equal((uint)(0x00002004 - numBytes), machine.CPU.ReadAddressRegister(1));
			Assert.Equal((uint)(0x00003004 - numBytes), machine.CPU.ReadAddressRegister(2));
		}

		[Theory]
		[InlineData(0x92C3, 0x00008000, 0x00003000, 0x00005000)]
		[InlineData(0x92C3, 0x12345678, 0x00005000, 0x12340678)]
		[InlineData(0x92C3, 0x12345678, 0x00006000, 0x1233F678)]
		[InlineData(0x93C3, 0x00800000, 0x00600000, 0x00200000)]
		[InlineData(0x93C3, 0x12345678, 0x00005000, 0x12340678)]
		[InlineData(0x93C3, 0x12345678, 0x00006000, 0x1233F678)]
		[InlineData(0x93C3, 0x12345678, 0x20000000, 0xF2345678)]
		public void SUBA(ushort opcode, uint a1Val, uint d3Val, uint expectedResult)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { opcode };
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D3 = d3Val,
				A1 = a1Val
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.ReadAddressRegister(1));
		}

		[Theory]
		[InlineData(0xB300, 0x00000000, 0x00000000, SRFlags.Zero, 0x00000000)]
		[InlineData(0xB300, 0x00000070, 0x00000040, (SRFlags)0, 0x00000030)]
		[InlineData(0xB300, 0x00000017, 0x00000081, SRFlags.Negative, 0x00000096)]
		[InlineData(0xB340, 0x00000000, 0x00000000, SRFlags.Zero, 0x00000000)]
		[InlineData(0xB340, 0x00002020, 0x00006262, (SRFlags)0, 0x00004242)]
		[InlineData(0xB340, 0x00001070, 0x00008043, SRFlags.Negative, 0x00009033)]
		[InlineData(0xB340, 0x12345678, 0x0000FFFF, SRFlags.Negative, 0x1234A987)]
		[InlineData(0xB380, 0x00000000, 0x00000000, SRFlags.Zero, 0x00000000)]
		[InlineData(0xB380, 0x12345678, 0x65432100, (SRFlags)0, 0x77777778)]
		[InlineData(0xB380, 0x70000000, 0xF0000000, SRFlags.Negative, 0x80000000)]
		public void EOR(ushort opcode, uint d0Val, uint d1Val, SRFlags expectedFlags, uint expectedResult)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { opcode };
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D0 = d0Val,
				D1 = d1Val
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(0));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(0xB509, 0x01, 0x00000000, 0x00000000, SRFlags.Zero)]
		[InlineData(0xB509, 0x01, 0x40000000, 0x50000000, (SRFlags)0)]
		[InlineData(0xB509, 0x01, 0x80000000, 0x10000000, SRFlags.Negative | SRFlags.Carry | SRFlags.Overflow)]
		[InlineData(0xB509, 0x01, 0x91000000, 0x10000000, SRFlags.Carry)]
		[InlineData(0xB549, 0x02, 0x00000000, 0x00000000, SRFlags.Zero)]
		[InlineData(0xB549, 0x02, 0x02020000, 0x20200000, (SRFlags)0)]
		[InlineData(0xB549, 0x02, 0x80430000, 0x10700000, SRFlags.Negative | SRFlags.Carry | SRFlags.Overflow)]
		[InlineData(0xB549, 0x02, 0x00002100, 0x12345678, (SRFlags)0)]
		[InlineData(0xB589, 0x04, 0x12345678, 0x12345678, SRFlags.Zero)]
		[InlineData(0xB589, 0x04, 0x20000000, 0x12345678, SRFlags.Negative | SRFlags.Carry)]
		[InlineData(0xB589, 0x04, 0xE0000000, 0xF0000000, (SRFlags)0)]
		public void CMPM(ushort opcode, byte numBytes, uint a1MemVal, uint a2MemVal, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { opcode };
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				A1 = 0x00002000,
				A2 = 0x00003000
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { (ushort)((a1MemVal & 0xFFFF0000) >> 16), (ushort)(a1MemVal & 0xFFFF) };
			machine.LoadData(data, 0x00002000, false);
			data = new ushort[] { (ushort)((a2MemVal & 0xFFFF0000) >> 16), (ushort)(a2MemVal & 0xFFFF) };
			machine.LoadData(data, 0x00003000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR);
			Assert.Equal((uint)(0x00002000 + numBytes), machine.CPU.ReadAddressRegister(1));
			Assert.Equal((uint)(0x00003000 + numBytes), machine.CPU.ReadAddressRegister(2));
		}

		[Theory]
		[InlineData(0xB411, 0x00000000, 0x00000000, SRFlags.Zero)]
		[InlineData(0xB411, 0x40000000, 0x00000050, (SRFlags)0)]
		[InlineData(0xB411, 0x80000000, 0x00000010, SRFlags.Negative | SRFlags.Carry | SRFlags.Overflow)]
		[InlineData(0xB411, 0x91000000, 0x00000010, SRFlags.Carry)]
		[InlineData(0xB451, 0x00000000, 0x00000000, SRFlags.Zero)]
		[InlineData(0xB451, 0x02020000, 0x00002020, (SRFlags)0)]
		[InlineData(0xB451, 0x80430000, 0x00001070, SRFlags.Negative | SRFlags.Carry | SRFlags.Overflow)]
		[InlineData(0xB451, 0x00002100, 0x56781234, (SRFlags)0)]
		[InlineData(0xB491, 0x12345678, 0x12345678, SRFlags.Zero)]
		[InlineData(0xB491, 0x20000000, 0x12345678, SRFlags.Negative | SRFlags.Carry)]
		[InlineData(0xB491, 0xE0000000, 0xF0000000, (SRFlags)0)]
		public void CMP(ushort opcode, uint a1MemVal, uint d2Val, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { opcode };
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				A1 = 0x00002000,
				D2 = d2Val
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { (ushort)((a1MemVal & 0xFFFF0000) >> 16), (ushort)(a1MemVal & 0xFFFF) };
			machine.LoadData(data, 0x00002000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(0xB2C3, 0x00003000, 0x00003000, SRFlags.Zero)]
		[InlineData(0xB2C3, 0x00007000, 0x00003000, (SRFlags)0)]
		[InlineData(0xB2C3, 0x12345678, 0x00005000, (SRFlags)0)]
		[InlineData(0xB2C3, 0x12345678, 0x00006000, SRFlags.Negative | SRFlags.Carry)]
		[InlineData(0xB3C3, 0x00800000, 0x00800000, SRFlags.Zero)]
		[InlineData(0xB3C3, 0x12345678, 0x00005000, (SRFlags)0)]
		[InlineData(0xB3C3, 0x12345678, 0x00006000, (SRFlags)0)]
		[InlineData(0xB3C3, 0x12345678, 0x20000000, SRFlags.Negative | SRFlags.Carry)]
		public void CMPA(ushort opcode, uint a1Val, uint d3Val, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { opcode };
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D3 = d3Val,
				A1 = a1Val
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(0x00008000, 0x00000000, SRFlags.Zero, 0x00000000)]
		[InlineData(0x00000100, 0x00000040, (SRFlags)0, 0x00004000)]
		[InlineData(0x00004000, 0x00000040, (SRFlags)0, 0x00100000)]
		[InlineData(0x0000F000, 0x00008000, (SRFlags)0, 0x78000000)]
		[InlineData(0x0000F000, 0x0000E000, SRFlags.Negative, 0xD2000000)]
		public void MULU(uint d0Val, uint d1Val, SRFlags expectedFlags, uint expectedResult)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { 0xC0C1 };  // mulu d1,d0
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D0 = d0Val,
				D1 = d1Val
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(0));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(0x00008000, 0x00000000, SRFlags.Zero, 0x00000000)]
		[InlineData(0x00000100, 0x00000040, (SRFlags)0, 0x00004000)]
		[InlineData(0x00004000, 0x00000040, (SRFlags)0, 0x00100000)]
		[InlineData(0x00008000, 0x00000100, SRFlags.Negative, 0xFF800000)]
		[InlineData(0x0000F000, 0x00008000, (SRFlags)0, 0x08000000)]
		[InlineData(0x0000F000, 0x0000E000, (SRFlags)0, 0x02000000)]
		public void MULS(uint d0Val, uint d1Val, SRFlags expectedFlags, uint expectedResult)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { 0xC1C1 };  // muls d1,d0
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D0 = d0Val,
				D1 = d1Val
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(0));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Fact]
		public void EXG()
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { 0xC141, 0xC149, 0xCD8D };  // exg d0,d1 | exg a0,a1 | exg d6,a5
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D0 = 0x12345678,
				D1 = 0x87654321,
				D6 = 0x66666666,
				A0 = 0x01010101,
				A1 = 0x55555555,
				A5 = 0xAAAAAAAA
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal((uint)0x87654321, machine.CPU.ReadDataRegister(0));
			Assert.Equal((uint)0x12345678, machine.CPU.ReadDataRegister(1));
			Assert.Equal((uint)0x55555555, machine.CPU.ReadAddressRegister(0));
			Assert.Equal((uint)0x01010101, machine.CPU.ReadAddressRegister(1));
			Assert.Equal((uint)0x66666666, machine.CPU.ReadAddressRegister(5));
			Assert.Equal((uint)0xAAAAAAAA, machine.CPU.ReadDataRegister(6));
		}

		[Theory]
		[InlineData(0xC001, 0x00000000, 0x00000000, SRFlags.Zero, 0x00000000)]
		[InlineData(0xC001, 0x0000007F, 0x00000037, (SRFlags)0, 0x00000037)]
		[InlineData(0xC001, 0x000000FE, 0x00000081, SRFlags.Negative, 0x00000080)]
		[InlineData(0xC041, 0x0000FFFF, 0x00000000, SRFlags.Zero, 0x00000000)]
		[InlineData(0xC041, 0x00002020, 0x0000F000, (SRFlags)0, 0x00002000)]
		[InlineData(0xC041, 0x0000E070, 0x00008043, SRFlags.Negative, 0x00008040)]
		[InlineData(0xC041, 0x12345678, 0x0000F0F0, (SRFlags)0, 0x12345070)]
		[InlineData(0xC081, 0x00000000, 0x00000000, SRFlags.Zero, 0x00000000)]
		[InlineData(0xC081, 0x12345678, 0x07070707, (SRFlags)0, 0x02040600)]
		[InlineData(0xC081, 0xF0000000, 0xE0000000, SRFlags.Negative, 0xE0000000)]
		public void AND(ushort opcode, uint d0Val, uint d1Val, SRFlags expectedFlags, uint expectedResult)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { opcode };
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D0 = d0Val,
				D1 = d1Val
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(0));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(0xD011, 0x00000000, 0x00000000, SRFlags.Zero, 0x00000000)]
		[InlineData(0xD011, 0x00000050, 0x10000000, (SRFlags)0, 0x00000060)]
		[InlineData(0xD011, 0x00000050, 0x40000000, SRFlags.Negative | SRFlags.Overflow, 0x00000090)]
		[InlineData(0xD011, 0x00000020, 0x80000000, SRFlags.Negative, 0x000000A0)]
		[InlineData(0xD051, 0x00000000, 0x00000000, SRFlags.Zero, 0x00000000)]
		[InlineData(0xD051, 0x00002020, 0x02020000, (SRFlags)0, 0x00002222)]
		[InlineData(0xD051, 0x00001070, 0x70430000, SRFlags.Negative | SRFlags.Overflow, 0x000080B3)]
		[InlineData(0xD051, 0x12345678, 0x21000000, (SRFlags)0, 0x12347778)]
		[InlineData(0xD091, 0x00000000, 0x00000000, SRFlags.Zero, 0x00000000)]
		[InlineData(0xD091, 0x12345678, 0x70000000, SRFlags.Negative | SRFlags.Overflow, 0x82345678)]
		[InlineData(0xD091, 0xF0000000, 0x10000000, SRFlags.Zero | SRFlags.Carry | SRFlags.Extend, 0x00000000)]
		public void ADD(ushort opcode, uint d0Val, uint a1MemVal, SRFlags expectedFlags, uint expectedResult)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { opcode };
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D0 = d0Val,
				A1 = 0x00002000
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { (ushort)((a1MemVal & 0xFFFF0000) >> 16), (ushort)(a1MemVal & 0xFFFF) };
			machine.LoadData(data, 0x00002000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(0));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(0xD509, 0x01, 0x00000000, 0x00000000, (SRFlags)0, SRFlags.Zero, 0x00000000)]
		[InlineData(0xD509, 0x01, 0x00000000, 0x00000001, SRFlags.Extend, (SRFlags)0, 0x00000002)]
		[InlineData(0xD509, 0x01, 0x00000040, 0x00000050, (SRFlags)0, SRFlags.Negative | SRFlags.Overflow, 0x00000090)]
		[InlineData(0xD509, 0x01, 0x00000080, 0x0000007F, (SRFlags)0, SRFlags.Negative, 0x000000FF)]
		[InlineData(0xD509, 0x01, 0x00000080, 0x0000007F, SRFlags.Extend, SRFlags.Zero | SRFlags.Carry | SRFlags.Extend, 0x00000000)]
		[InlineData(0xD549, 0x02, 0x00000000, 0x00000000, (SRFlags)0, SRFlags.Zero, 0x00000000)]
		[InlineData(0xD549, 0x02, 0x00000000, 0x00000001, SRFlags.Extend, (SRFlags)0, 0x00000002)]
		[InlineData(0xD549, 0x02, 0x00000202, 0x00002020, (SRFlags)0, (SRFlags)0, 0x00002222)]
		[InlineData(0xD549, 0x02, 0x00008043, 0x00001070, SRFlags.Extend, SRFlags.Negative, 0x000090B4)]
		[InlineData(0xD549, 0x02, 0x00002100, 0x12345678, SRFlags.Extend, (SRFlags)0, 0x12347779)]
		[InlineData(0xD589, 0x04, 0x00000000, 0x00000000, (SRFlags)0, SRFlags.Zero, 0x00000000)]
		[InlineData(0xD589, 0x04, 0x12345678, 0x12345678, SRFlags.Extend, (SRFlags)0, 0x2468ACF1)]
		[InlineData(0xD589, 0x04, 0x70000000, 0x12345678, (SRFlags)0, SRFlags.Negative | SRFlags.Overflow, 0x82345678)]
		[InlineData(0xD589, 0x04, 0xE0000000, 0xF0000000, SRFlags.Extend, SRFlags.Negative | SRFlags.Carry | SRFlags.Extend, 0xD0000001)]
		[InlineData(0xD589, 0x04, 0xE0000000, 0x1FFFFFFF, SRFlags.Extend, SRFlags.Zero | SRFlags.Carry | SRFlags.Extend, 0x00000000)]
		public void ADDX(ushort opcode, byte numBytes, uint a1MemVal, uint a2MemVal, SRFlags initFlags, SRFlags expectedFlags, uint expectedResult)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { opcode };
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				A1 = 0x00002004,
				A2 = 0x00003004,
				SR = initFlags
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { (ushort)((a1MemVal & 0xFFFF0000) >> 16), (ushort)(a1MemVal & 0xFFFF) };
			machine.LoadData(data, 0x00002000, false);
			data = new ushort[] { (ushort)((a2MemVal & 0xFFFF0000) >> 16), (ushort)(a2MemVal & 0xFFFF) };
			machine.LoadData(data, 0x00003000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.Memory.ReadLong(0x0003000));
			Assert.Equal(expectedFlags, machine.CPU.SR);
			Assert.Equal((uint)(0x00002004 - numBytes), machine.CPU.ReadAddressRegister(1));
			Assert.Equal((uint)(0x00003004 - numBytes), machine.CPU.ReadAddressRegister(2));
		}

		[Theory]
		[InlineData(0xD2C3, 0x00000000, 0x00000000, 0x00000000)]
		[InlineData(0xD2C3, 0x00004000, 0x00003000, 0x00007000)]
		[InlineData(0xD2C3, 0x00008000, 0x00003000, 0x0000B000)]
		[InlineData(0xD2C3, 0x12345678, 0x00005000, 0x1234A678)]
		[InlineData(0xD2C3, 0x12345678, 0x0000F000, 0x12344678)]
		[InlineData(0xD2C3, 0x00008000, 0x00008000, 0x00000000)]
		[InlineData(0xD3C3, 0x00000000, 0x00000000, 0x00000000)]
		[InlineData(0xD3C3, 0x00800000, 0x00600000, 0x00E00000)]
		[InlineData(0xD3C3, 0x12345678, 0x00005000, 0x1234A678)]
		[InlineData(0xD3C3, 0x12345678, 0x70000000, 0x82345678)]
		[InlineData(0xD3C3, 0x12345678, 0xEDCBA988, 0x00000000)]
		public void ADDA(ushort opcode, uint a1Val, uint d3Val, uint expectedResult)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { opcode };
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D3 = d3Val,
				A1 = a1Val
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.ReadAddressRegister(1));
		}

		[Theory]
		[InlineData(0xE901, 0x00000000, 0x00000000, 0x00000000, SRFlags.Zero)]    // asl.b #4,d1
		[InlineData(0xE901, 0x00000000, 0x00000001, 0x00000010, (SRFlags)0)]    // asl.b #4,d1
		[InlineData(0xE901, 0x00000000, 0x00000010, 0x00000000, SRFlags.Zero | SRFlags.Carry | SRFlags.Overflow | SRFlags.Extend)]    // asl.b #4,d1
		[InlineData(0xE121, 0x00000005, 0x00000000, 0x00000000, SRFlags.Zero)]    // asl.b d0,d1
		[InlineData(0xE121, 0x00000005, 0x00000001, 0x00000020, (SRFlags)0)]    // asl.b d0,d1
		[InlineData(0xE121, 0x00000005, 0x00000008, 0x00000000, SRFlags.Zero | SRFlags.Carry | SRFlags.Overflow | SRFlags.Extend)]    // asl.b d0,d1
		[InlineData(0xE941, 0x00000000, 0x00000000, 0x00000000, SRFlags.Zero)]    // asl.w #4,d1
		[InlineData(0xE941, 0x00000000, 0x00000201, 0x00002010, (SRFlags)0)]    // asl.w #4,d1
		[InlineData(0xE941, 0x00000000, 0x00001000, 0x00000000, SRFlags.Zero | SRFlags.Carry | SRFlags.Overflow | SRFlags.Extend)]    // asl.w #4,d1
		[InlineData(0xE161, 0x00000005, 0x00000000, 0x00000000, SRFlags.Zero)]    // asl.w d0,d1
		[InlineData(0xE161, 0x0000000B, 0x00000010, 0x00008000, SRFlags.Negative | SRFlags.Overflow)]    // asl.w d0,d1
		[InlineData(0xE161, 0x00000005, 0x00000800, 0x00000000, SRFlags.Zero | SRFlags.Carry | SRFlags.Overflow | SRFlags.Extend)]    // asl.w d0,d1
		[InlineData(0xE981, 0x00000000, 0x00000000, 0x00000000, SRFlags.Zero)]    // asl.l #4,d1
		[InlineData(0xE981, 0x00000000, 0x01020408, 0x10204080, (SRFlags)0)]    // asl.l #4,d1
		[InlineData(0xE981, 0x00000000, 0x00001000, 0x00010000, (SRFlags)0)]    // asl.l #4,d1
		[InlineData(0xE1A1, 0x00000005, 0x00000000, 0x00000000, SRFlags.Zero)]    // asl.l d0,d1
		[InlineData(0xE1A1, 0x00000010, 0x00000010, 0x00100000, (SRFlags)0)]    // asl.l d0,d1
		[InlineData(0xE1A1, 0x00000005, 0x08000000, 0x00000000, SRFlags.Zero | SRFlags.Carry | SRFlags.Overflow | SRFlags.Extend)]    // asl.l d0,d1
		public void ASL_Register(ushort opcode, uint d0Val, uint d1Val, uint expectedResult, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { opcode };
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D0 = d0Val,
				D1 = d1Val
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(1));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(0xE1D1, 0x0000, 0x0000, SRFlags.Zero)]    // asl (a1)
		[InlineData(0xE1D1, 0x0100, 0x0200, (SRFlags)0)]    // asl (a1)
		[InlineData(0xE1D1, 0x8000, 0x0000, SRFlags.Zero | SRFlags.Carry | SRFlags.Overflow | SRFlags.Extend)]    // asl (a1)
		[InlineData(0xE1D1, 0x4000, 0x8000, SRFlags.Negative | SRFlags.Overflow)]    // asl (a1)
		public void ASL_Memory(ushort opcode, ushort a1Val, ushort expectedResult, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { opcode };
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				A1 = 0x00003000
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { a1Val };
			machine.LoadData(data, 0x00003000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.Memory.ReadWord(0x00003000));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(0xE801, 0x00000000, 0x00000000, 0x00000000, SRFlags.Zero)]    // asr.b #4,d1
		[InlineData(0xE801, 0x00000000, 0x00000010, 0x00000001, (SRFlags)0)]    // asr.b #4,d1
		[InlineData(0xE801, 0x00000000, 0x00000008, 0x00000000, SRFlags.Zero | SRFlags.Carry | SRFlags.Extend)]    // asr.b #4,d1
		[InlineData(0xE801, 0x00000000, 0x00000080, 0x000000F8, SRFlags.Negative)]    // asr.b #4,d1
		[InlineData(0xE021, 0x00000005, 0x00000000, 0x00000000, SRFlags.Zero)]    // asr.b d0,d1
		[InlineData(0xE021, 0x00000005, 0x00000020, 0x00000001, (SRFlags)0)]    // asr.b d0,d1
		[InlineData(0xE021, 0x00000005, 0x00000010, 0x00000000, SRFlags.Zero | SRFlags.Carry | SRFlags.Extend)]    // asr.b d0,d1
		[InlineData(0xE841, 0x00000000, 0x00000000, 0x00000000, SRFlags.Zero)]    // asr.w #4,d1
		[InlineData(0xE841, 0x00000000, 0x00002010, 0x00000201, (SRFlags)0)]    // asr.w #4,d1
		[InlineData(0xE841, 0x00000000, 0x00000008, 0x00000000, SRFlags.Zero | SRFlags.Carry | SRFlags.Extend)]    // asr.w #4,d1
		[InlineData(0xE061, 0x00000005, 0x00000000, 0x00000000, SRFlags.Zero)]    // asr.w d0,d1
		[InlineData(0xE061, 0x0000000B, 0x00008000, 0x0000FFF0, SRFlags.Negative)]    // asr.w d0,d1
		[InlineData(0xE061, 0x0000000C, 0x00000800, 0x00000000, SRFlags.Zero | SRFlags.Carry | SRFlags.Extend)]    // asr.w d0,d1
		[InlineData(0xE881, 0x00000000, 0x00000000, 0x00000000, SRFlags.Zero)]    // asr.l #4,d1
		[InlineData(0xE881, 0x00000000, 0x10204080, 0x01020408, (SRFlags)0)]    // asr.l #4,d1
		[InlineData(0xE881, 0x00000000, 0x00000002, 0x00000000, SRFlags.Zero)]    // asr.l #4,d1
		[InlineData(0xE881, 0x00000000, 0x00000008, 0x00000000, SRFlags.Zero | SRFlags.Carry | SRFlags.Extend)]    // asr.l #4,d1
		[InlineData(0xE0A1, 0x00000005, 0x00000000, 0x00000000, SRFlags.Zero)]    // asr.l d0,d1
		[InlineData(0xE0A1, 0x00000010, 0x00100000, 0x00000010, (SRFlags)0)]    // asr.l d0,d1
		[InlineData(0xE0A1, 0x00000008, 0x80100000, 0xFF801000, SRFlags.Negative)]    // asr.l d0,d1
		[InlineData(0xE0A1, 0x00000005, 0x00000010, 0x00000000, SRFlags.Zero | SRFlags.Carry | SRFlags.Extend)]    // asr.l d0,d1
		public void ASR_Register(ushort opcode, uint d0Val, uint d1Val, uint expectedResult, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { opcode };
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D0 = d0Val,
				D1 = d1Val
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(1));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(0xE0D1, 0x0000, 0x0000, SRFlags.Zero)]    // asr (a1)
		[InlineData(0xE0D1, 0x0200, 0x0100, (SRFlags)0)]    // asr (a1)
		[InlineData(0xE0D1, 0x0001, 0x0000, SRFlags.Zero | SRFlags.Carry | SRFlags.Extend)]    // asr (a1)
		[InlineData(0xE0D1, 0x8000, 0xC000, SRFlags.Negative)]    // asr (a1)
		public void ASR_Memory(ushort opcode, ushort a1Val, ushort expectedResult, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { opcode };
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				A1 = 0x00003000
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { a1Val };
			machine.LoadData(data, 0x00003000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.Memory.ReadWord(0x00003000));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(0xE909, 0x00000000, 0x00000000, 0x00000000, SRFlags.Zero)]    // lsl.b #4,d1
		[InlineData(0xE909, 0x00000000, 0x00000001, 0x00000010, (SRFlags)0)]    // lsl.b #4,d1
		[InlineData(0xE909, 0x00000000, 0x00000010, 0x00000000, SRFlags.Zero | SRFlags.Carry | SRFlags.Extend)]    // lsl.b #4,d1
		[InlineData(0xE129, 0x00000005, 0x00000000, 0x00000000, SRFlags.Zero)]    // lsl.b d0,d1
		[InlineData(0xE129, 0x00000005, 0x00000001, 0x00000020, (SRFlags)0)]    // lsl.b d0,d1
		[InlineData(0xE129, 0x00000005, 0x00000008, 0x00000000, SRFlags.Zero | SRFlags.Carry | SRFlags.Extend)]    // lsl.b d0,d1
		[InlineData(0xE949, 0x00000000, 0x00000000, 0x00000000, SRFlags.Zero)]    // lsl.w #4,d1
		[InlineData(0xE949, 0x00000000, 0x00000201, 0x00002010, (SRFlags)0)]    // lsl.w #4,d1
		[InlineData(0xE949, 0x00000000, 0x00001000, 0x00000000, SRFlags.Zero | SRFlags.Carry | SRFlags.Extend)]    // lsl.w #4,d1
		[InlineData(0xE169, 0x00000005, 0x00000000, 0x00000000, SRFlags.Zero)]    // lsl.w d0,d1
		[InlineData(0xE169, 0x0000000B, 0x00000010, 0x00008000, SRFlags.Negative)]    // lsl.w d0,d1
		[InlineData(0xE169, 0x00000005, 0x00000800, 0x00000000, SRFlags.Zero | SRFlags.Carry | SRFlags.Extend)]    // lsl.w d0,d1
		[InlineData(0xE989, 0x00000000, 0x00000000, 0x00000000, SRFlags.Zero)]    // lsl.l #4,d1
		[InlineData(0xE989, 0x00000000, 0x01020408, 0x10204080, (SRFlags)0)]    // lsl.l #4,d1
		[InlineData(0xE989, 0x00000000, 0x00001000, 0x00010000, (SRFlags)0)]    // lsl.l #4,d1
		[InlineData(0xE1A9, 0x00000005, 0x00000000, 0x00000000, SRFlags.Zero)]    // lsl.l d0,d1
		[InlineData(0xE1A9, 0x00000010, 0x00000010, 0x00100000, (SRFlags)0)]    // lsl.l d0,d1
		[InlineData(0xE1A9, 0x00000005, 0x08000000, 0x00000000, SRFlags.Zero | SRFlags.Carry | SRFlags.Extend)]    // lsl.l d0,d1
		public void LSL_Register(ushort opcode, uint d0Val, uint d1Val, uint expectedResult, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { opcode };
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D0 = d0Val,
				D1 = d1Val
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(1));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(0xE3D1, 0x0000, 0x0000, SRFlags.Zero)]    // lsl (a1)
		[InlineData(0xE3D1, 0x0100, 0x0200, (SRFlags)0)]    // lsl (a1)
		[InlineData(0xE3D1, 0x8000, 0x0000, SRFlags.Zero | SRFlags.Carry | SRFlags.Extend)]    // lsl (a1)
		[InlineData(0xE3D1, 0x4000, 0x8000, SRFlags.Negative)]    // lsl (a1)
		public void LSL_Memory(ushort opcode, ushort a1Val, ushort expectedResult, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { opcode };
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				A1 = 0x00003000
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { a1Val };
			machine.LoadData(data, 0x00003000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.Memory.ReadWord(0x00003000));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(0xE809, 0x00000000, 0x00000000, 0x00000000, SRFlags.Zero)]    // lsr.b #4,d1
		[InlineData(0xE809, 0x00000000, 0x00000010, 0x00000001, (SRFlags)0)]    // lsr.b #4,d1
		[InlineData(0xE809, 0x00000000, 0x00000008, 0x00000000, SRFlags.Zero | SRFlags.Carry | SRFlags.Extend)]    // lsr.b #4,d1
		[InlineData(0xE809, 0x00000000, 0x00000080, 0x00000008, (SRFlags)0)]    // lsr.b #4,d1
		[InlineData(0xE029, 0x00000005, 0x00000000, 0x00000000, SRFlags.Zero)]    // lsr.b d0,d1
		[InlineData(0xE029, 0x00000005, 0x00000020, 0x00000001, (SRFlags)0)]    // lsr.b d0,d1
		[InlineData(0xE029, 0x00000005, 0x00000010, 0x00000000, SRFlags.Zero | SRFlags.Carry | SRFlags.Extend)]    // lsr.b d0,d1
		[InlineData(0xE849, 0x00000000, 0x00000000, 0x00000000, SRFlags.Zero)]    // lsr.w #4,d1
		[InlineData(0xE849, 0x00000000, 0x00002010, 0x00000201, (SRFlags)0)]    // lsr.w #4,d1
		[InlineData(0xE849, 0x00000000, 0x00000008, 0x00000000, SRFlags.Zero | SRFlags.Carry | SRFlags.Extend)]    // lsr.w #4,d1
		[InlineData(0xE069, 0x00000005, 0x00000000, 0x00000000, SRFlags.Zero)]    // lsr.w d0,d1
		[InlineData(0xE069, 0x0000000B, 0x00008000, 0x00000010, (SRFlags)0)]    // lsr.w d0,d1
		[InlineData(0xE069, 0x0000000C, 0x00000800, 0x00000000, SRFlags.Zero | SRFlags.Carry | SRFlags.Extend)]    // lsr.w d0,d1
		[InlineData(0xE889, 0x00000000, 0x00000000, 0x00000000, SRFlags.Zero)]    // lsr.l #4,d1
		[InlineData(0xE889, 0x00000000, 0x10204080, 0x01020408, (SRFlags)0)]    // lsr.l #4,d1
		[InlineData(0xE889, 0x00000000, 0x00000002, 0x00000000, SRFlags.Zero)]    // lsr.l #4,d1
		[InlineData(0xE889, 0x00000000, 0x00000008, 0x00000000, SRFlags.Zero | SRFlags.Carry | SRFlags.Extend)]    // lsr.l #4,d1
		[InlineData(0xE0A9, 0x00000005, 0x00000000, 0x00000000, SRFlags.Zero)]    // lsr.l d0,d1
		[InlineData(0xE0A9, 0x00000010, 0x00100000, 0x00000010, (SRFlags)0)]    // lsr.l d0,d1
		[InlineData(0xE0A9, 0x00000008, 0x80100000, 0x00801000, (SRFlags)0)]    // lsr.l d0,d1
		[InlineData(0xE0A9, 0x00000005, 0x00000010, 0x00000000, SRFlags.Zero | SRFlags.Carry | SRFlags.Extend)]    // lsr.l d0,d1
		public void LSR_Register(ushort opcode, uint d0Val, uint d1Val, uint expectedResult, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { opcode };
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D0 = d0Val,
				D1 = d1Val
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(1));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(0xE2D1, 0x0000, 0x0000, SRFlags.Zero)]    // lsr (a1)
		[InlineData(0xE2D1, 0x0200, 0x0100, (SRFlags)0)]    // lsr (a1)
		[InlineData(0xE2D1, 0x0001, 0x0000, SRFlags.Zero | SRFlags.Carry | SRFlags.Extend)]    // lsr (a1)
		[InlineData(0xE2D1, 0x8000, 0x4000, (SRFlags)0)]    // lsr (a1)
		public void LSR_Memory(ushort opcode, ushort a1Val, ushort expectedResult, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { opcode };
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				A1 = 0x00003000
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { a1Val };
			machine.LoadData(data, 0x00003000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.Memory.ReadWord(0x00003000));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(0xE919, 0x00000000, 0x00000000, 0x00000000, SRFlags.Zero)]    // rol.b #4,d1
		[InlineData(0xE919, 0x00000000, 0x00000001, 0x00000010, (SRFlags)0)]    // rol.b #4,d1
		[InlineData(0xE919, 0x00000000, 0x00000010, 0x00000001, SRFlags.Carry)]    // rol.b #4,d1
		[InlineData(0xE139, 0x00000005, 0x00000000, 0x00000000, SRFlags.Zero)]    // rol.b d0,d1
		[InlineData(0xE139, 0x00000005, 0x00000001, 0x00000020, (SRFlags)0)]    // rol.b d0,d1
		[InlineData(0xE139, 0x00000005, 0x00000008, 0x00000001, SRFlags.Carry)]    // rol.b d0,d1
		[InlineData(0xE139, 0x00000005, 0x00000004, 0x00000080, SRFlags.Negative)]    // rol.b d0,d1
		[InlineData(0xE959, 0x00000000, 0x00000000, 0x00000000, SRFlags.Zero)]    // rol.w #4,d1
		[InlineData(0xE959, 0x00000000, 0x00000201, 0x00002010, (SRFlags)0)]    // rol.w #4,d1
		[InlineData(0xE959, 0x00000000, 0x00001000, 0x00000001, SRFlags.Carry)]    // rol.w #4,d1
		[InlineData(0xE959, 0x00000000, 0x00000800, 0x00008000, SRFlags.Negative)]    // rol.w #4,d1
		[InlineData(0xE179, 0x00000005, 0x00000000, 0x00000000, SRFlags.Zero)]    // rol.w d0,d1
		[InlineData(0xE179, 0x0000000B, 0x00000010, 0x00008000, SRFlags.Negative)]    // rol.w d0,d1
		[InlineData(0xE179, 0x00000005, 0x00000800, 0x00000001, SRFlags.Carry )]    // rol.w d0,d1
		[InlineData(0xE999, 0x00000000, 0x00000000, 0x00000000, SRFlags.Zero)]    // rol.l #4,d1
		[InlineData(0xE999, 0x00000000, 0x01020408, 0x10204080, (SRFlags)0)]    // rol.l #4,d1
		[InlineData(0xE999, 0x00000000, 0x12345678, 0x23456781, SRFlags.Carry)]    // rol.l #4,d1
		[InlineData(0xE1B9, 0x00000005, 0x00000000, 0x00000000, SRFlags.Zero)]    // rol.l d0,d1
		[InlineData(0xE1B9, 0x00000010, 0x00000010, 0x00100000, (SRFlags)0)]    // rol.l d0,d1
		[InlineData(0xE1B9, 0x00000005, 0x08000000, 0x00000001, SRFlags.Carry)]    // rol.l d0,d1
		public void ROL_Register(ushort opcode, uint d0Val, uint d1Val, uint expectedResult, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { opcode };
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D0 = d0Val,
				D1 = d1Val
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(1));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(0xE7D1, 0x0000, 0x0000, SRFlags.Zero)]    // rol (a1)
		[InlineData(0xE7D1, 0x0100, 0x0200, (SRFlags)0)]    // rol (a1)
		[InlineData(0xE7D1, 0x8000, 0x0001, SRFlags.Carry)]    // rol (a1)
		[InlineData(0xE7D1, 0xC000, 0x8001, SRFlags.Negative | SRFlags.Carry)]    // rol (a1)
		public void ROL_Memory(ushort opcode, ushort a1Val, ushort expectedResult, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { opcode };
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				A1 = 0x00003000
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { a1Val };
			machine.LoadData(data, 0x00003000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.Memory.ReadWord(0x00003000));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(0xE819, 0x00000000, 0x00000000, 0x00000000, SRFlags.Zero)]    // ror.b #4,d1
		[InlineData(0xE819, 0x00000000, 0x00000010, 0x00000001, (SRFlags)0)]    // ror.b #4,d1
		[InlineData(0xE819, 0x00000000, 0x00000008, 0x00000080, SRFlags.Carry | SRFlags.Negative)]    // ror.b #4,d1
		[InlineData(0xE039, 0x00000005, 0x00000000, 0x00000000, SRFlags.Zero)]    // ror.b d0,d1
		[InlineData(0xE039, 0x00000005, 0x00000020, 0x00000001, (SRFlags)0)]    // ror.b d0,d1
		[InlineData(0xE039, 0x00000005, 0x00000008, 0x00000040, (SRFlags)0)]    // ror.b d0,d1
		[InlineData(0xE039, 0x00000005, 0x00000010, 0x00000080, SRFlags.Negative | SRFlags.Carry)]    // ror.b d0,d1
		[InlineData(0xE859, 0x00000000, 0x00000000, 0x00000000, SRFlags.Zero)]    // ror.w #4,d1
		[InlineData(0xE859, 0x00000000, 0x00002010, 0x00000201, (SRFlags)0)]    // ror.w #4,d1
		[InlineData(0xE859, 0x00000000, 0x00000001, 0x00001000, (SRFlags)0)]    // ror.w #4,d1
		[InlineData(0xE859, 0x00000000, 0x00000008, 0x00008000, SRFlags.Negative | SRFlags.Carry)]    // ror.w #4,d1
		[InlineData(0xE079, 0x00000005, 0x00000000, 0x00000000, SRFlags.Zero)]    // ror.w d0,d1
		[InlineData(0xE079, 0x0000000B, 0x00008000, 0x00000010, (SRFlags)0)]    // ror.w d0,d1
		[InlineData(0xE079, 0x00000005, 0x00000010, 0x00008000, SRFlags.Negative | SRFlags.Carry)]    // ror.w d0,d1
		[InlineData(0xE899, 0x00000000, 0x00000000, 0x00000000, SRFlags.Zero)]    // ror.l #4,d1
		[InlineData(0xE899, 0x00000000, 0x10204080, 0x01020408, (SRFlags)0)]    // ror.l #4,d1
		[InlineData(0xE899, 0x00000000, 0x12345678, 0x81234567, SRFlags.Negative | SRFlags.Carry)]    // ror.l #4,d1
		[InlineData(0xE0B9, 0x00000005, 0x00000000, 0x00000000, SRFlags.Zero)]    // ror.l d0,d1
		[InlineData(0xE0B9, 0x00000010, 0x00100000, 0x00000010, (SRFlags)0)]    // ror.l d0,d1
		[InlineData(0xE0B9, 0x00000005, 0x00000010, 0x80000000, SRFlags.Carry | SRFlags.Negative)]    // ror.l d0,d1
		public void ROR_Register(ushort opcode, uint d0Val, uint d1Val, uint expectedResult, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { opcode };
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D0 = d0Val,
				D1 = d1Val
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(1));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(0xE6D1, 0x0000, 0x0000, SRFlags.Zero)]    // ror (a1)
		[InlineData(0xE6D1, 0x0200, 0x0100, (SRFlags)0)]    // ror (a1)
		[InlineData(0xE6D1, 0x0001, 0x8000, SRFlags.Carry | SRFlags.Negative)]    // ror (a1)
		[InlineData(0xE6D1, 0x0003, 0x8001, SRFlags.Negative | SRFlags.Carry)]    // ror (a1)
		public void ROR_Memory(ushort opcode, ushort a1Val, ushort expectedResult, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { opcode };
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				A1 = 0x00003000
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { a1Val };
			machine.LoadData(data, 0x00003000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.Memory.ReadWord(0x00003000));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(0xE911, 0x00000000, 0x00000000, (SRFlags)0, 0x00000000, SRFlags.Zero)]    // roxl.b #4,d1
		[InlineData(0xE911, 0x00000000, 0x00000001, SRFlags.Extend, 0x00000018, (SRFlags)0)]    // roxl.b #4,d1
		[InlineData(0xE911, 0x00000000, 0x00000010, (SRFlags)0, 0x00000000, SRFlags.Carry | SRFlags.Zero | SRFlags.Extend)]    // roxl.b #4,d1
		[InlineData(0xE131, 0x00000005, 0x00000000, (SRFlags)0, 0x00000000, SRFlags.Zero)]    // roxl.b d0,d1
		[InlineData(0xE131, 0x00000005, 0x00000001, SRFlags.Extend, 0x00000030, (SRFlags)0)]    // roxl.b d0,d1
		[InlineData(0xE131, 0x00000005, 0x00000008, SRFlags.Extend, 0x00000010, SRFlags.Carry | SRFlags.Extend)]    // roxl.b d0,d1
		[InlineData(0xE131, 0x00000005, 0x00000004, SRFlags.Extend, 0x00000090, SRFlags.Negative)]    // roxl.b d0,d1
		[InlineData(0xE951, 0x00000000, 0x00000000, (SRFlags)0, 0x00000000, SRFlags.Zero)]    // roxl.w #4,d1
		[InlineData(0xE951, 0x00000000, 0x00000201, SRFlags.Extend, 0x00002018, (SRFlags)0)]    // roxl.w #4,d1
		[InlineData(0xE951, 0x00000000, 0x00001000, SRFlags.Extend, 0x00000008, SRFlags.Carry | SRFlags.Extend)]    // roxl.w #4,d1
		[InlineData(0xE951, 0x00000000, 0x00000800, (SRFlags)0, 0x00008000, SRFlags.Negative)]    // roxl.w #4,d1
		[InlineData(0xE171, 0x00000005, 0x00000000, (SRFlags)0, 0x00000000, SRFlags.Zero)]    // roxl.w d0,d1
		[InlineData(0xE171, 0x0000000B, 0x00000010, SRFlags.Extend, 0x00008400, SRFlags.Negative)]    // roxl.w d0,d1
		[InlineData(0xE171, 0x00000005, 0x00000800, (SRFlags)0, 0x00000000, SRFlags.Carry | SRFlags.Zero | SRFlags.Extend)]    // roxl.w d0,d1
		[InlineData(0xE991, 0x00000000, 0x00000000, (SRFlags)0, 0x00000000, SRFlags.Zero)]    // roxl.l #4,d1
		[InlineData(0xE991, 0x00000000, 0x01020408, SRFlags.Extend, 0x10204088, (SRFlags)0)]    // roxl.l #4,d1
		[InlineData(0xE991, 0x00000000, 0x12345678, SRFlags.Extend, 0x23456788, SRFlags.Carry | SRFlags.Extend)]    // roxl.l #4,d1
		[InlineData(0xE1B1, 0x00000005, 0x00000000, (SRFlags)0, 0x00000000, SRFlags.Zero)]    // roxl.l d0,d1
		[InlineData(0xE1B1, 0x00000010, 0x00000010, SRFlags.Extend, 0x00108000, (SRFlags)0)]    // roxl.l d0,d1
		[InlineData(0xE1B1, 0x00000005, 0x08000000, (SRFlags)0, 0x00000000, SRFlags.Zero | SRFlags.Carry | SRFlags.Extend)]    // roxl.l d0,d1
		public void ROXL_Register(ushort opcode, uint d0Val, uint d1Val, SRFlags initFlags, uint expectedResult, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { opcode };
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D0 = d0Val,
				D1 = d1Val,
				SR = initFlags
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(1));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(0xE5D1, 0x0000, (SRFlags)0, 0x0000, SRFlags.Zero)]    // roxl (a1)
		[InlineData(0xE5D1, 0x0000, SRFlags.Extend, 0x0001, (SRFlags)0)]    // roxl (a1)
		[InlineData(0xE5D1, 0x0100, (SRFlags)0, 0x0200, (SRFlags)0)]    // roxl (a1)
		[InlineData(0xE5D1, 0x8000, SRFlags.Extend, 0x0001, SRFlags.Carry | SRFlags.Extend)]    // roxl (a1)
		[InlineData(0xE5D1, 0xC000, (SRFlags)0, 0x8000, SRFlags.Negative | SRFlags.Carry | SRFlags.Extend)]    // roxl (a1)
		public void ROXL_Memory(ushort opcode, ushort a1Val, SRFlags initFlags, ushort expectedResult, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { opcode };
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				A1 = 0x00003000,
				SR=  initFlags
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { a1Val };
			machine.LoadData(data, 0x00003000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.Memory.ReadWord(0x00003000));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(0xE811, 0x00000000, 0x00000000, (SRFlags)0, 0x00000000, SRFlags.Zero)]    // roxr.b #4,d1
		[InlineData(0xE811, 0x00000000, 0x00000010, SRFlags.Extend, 0x00000011, (SRFlags)0)]    // roxr.b #4,d1
		[InlineData(0xE811, 0x00000000, 0x00000008, (SRFlags)0, 0x00000000, SRFlags.Carry | SRFlags.Zero | SRFlags.Extend)]    // roxr.b #4,d1
		[InlineData(0xE031, 0x00000005, 0x00000000, (SRFlags)0, 0x00000000, SRFlags.Zero)]    // roxr.b d0,d1
		[InlineData(0xE031, 0x00000005, 0x00000020, SRFlags.Extend, 0x00000009, (SRFlags)0)]    // roxr.b d0,d1
		[InlineData(0xE031, 0x00000005, 0x00000008, (SRFlags)0, 0x00000080, SRFlags.Negative)]    // roxr.b d0,d1
		[InlineData(0xE031, 0x00000005, 0x00000010, SRFlags.Extend, 0x00000008, SRFlags.Extend | SRFlags.Carry)]    // roxr.b d0,d1
		[InlineData(0xE851, 0x00000000, 0x00000000, (SRFlags)0, 0x00000000, SRFlags.Zero)]    // roxr.w #4,d1
		[InlineData(0xE851, 0x00000000, 0x00002010, SRFlags.Extend, 0x00001201, (SRFlags)0)]    // roxr.w #4,d1
		[InlineData(0xE851, 0x00000000, 0x00000001, (SRFlags)0, 0x00002000, (SRFlags)0)]    // roxr.w #4,d1
		[InlineData(0xE851, 0x00000000, 0x00000008, (SRFlags)0, 0x00000000, SRFlags.Zero | SRFlags.Carry | SRFlags.Extend)]    // roxr.w #4,d1
		[InlineData(0xE071, 0x00000005, 0x00000000, (SRFlags)0, 0x00000000, SRFlags.Zero)]    // roxr.w d0,d1
		[InlineData(0xE071, 0x0000000B, 0x00008000, SRFlags.Extend, 0x00000030, (SRFlags)0)]    // roxr.w d0,d1
		[InlineData(0xE071, 0x00000005, 0x00000018, (SRFlags)0, 0x00008000, SRFlags.Negative | SRFlags.Carry | SRFlags.Extend)]    // roxr.w d0,d1
		[InlineData(0xE891, 0x00000000, 0x00000000, (SRFlags)0, 0x00000000, SRFlags.Zero)]    // roxr.l #4,d1
		[InlineData(0xE891, 0x00000000, 0x10204080, SRFlags.Extend, 0x11020408, (SRFlags)0)]    // roxr.l #4,d1
		[InlineData(0xE891, 0x00000000, 0x12345678, SRFlags.Extend, 0x11234567, SRFlags.Carry | SRFlags.Extend)]    // roxr.l #4,d1
		[InlineData(0xE0B1, 0x00000005, 0x00000000, (SRFlags)0, 0x00000000, SRFlags.Zero)]    // roxr.l d0,d1
		[InlineData(0xE0B1, 0x00000010, 0x00100000, SRFlags.Extend, 0x00010010, (SRFlags)0)]    // roxr.l d0,d1
		[InlineData(0xE0B1, 0x00000005, 0x00000018, (SRFlags)0, 0x80000000, SRFlags.Carry | SRFlags.Negative | SRFlags.Extend)]    // roxr.l d0,d1
		public void ROXR_Register(ushort opcode, uint d0Val, uint d1Val, SRFlags initFlags, uint expectedResult, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { opcode };
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D0 = d0Val,
				D1 = d1Val,
				SR = initFlags
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(1));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(0xE4D1, 0x0000, (SRFlags)0, 0x0000, SRFlags.Zero)]    // roxr (a1)
		[InlineData(0xE4D1, 0x0000, SRFlags.Extend, 0x8000, SRFlags.Negative)]    // roxr (a1)
		[InlineData(0xE4D1, 0x0001, SRFlags.Extend, 0x8000, SRFlags.Negative | SRFlags.Extend | SRFlags.Carry)]    // roxr (a1)
		[InlineData(0xE4D1, 0x0200, (SRFlags)0, 0x0100, (SRFlags)0)]    // ror (a1)
		[InlineData(0xE4D1, 0x0001, (SRFlags)0, 0x0000, SRFlags.Carry | SRFlags.Zero | SRFlags.Extend)]    // roxr (a1)
		[InlineData(0xE4D1, 0x0003, (SRFlags)0, 0x0001, SRFlags.Extend | SRFlags.Carry)]    // roxr (a1)
		public void ROXR_Memory(ushort opcode, ushort a1Val, SRFlags initFlags, ushort expectedResult, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			ushort[] code = new ushort[] { opcode };
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				A1 = 0x00003000,
				SR = initFlags
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { a1Val };
			machine.LoadData(data, 0x00003000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedResult, machine.Memory.ReadWord(0x00003000));
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(new ushort[] { 0x0800, 0x0002 }, 0x00000000, 0x00000000, SRFlags.Zero)]    // btst #2,d0
		[InlineData(new ushort[] { 0x0300 }, 0x80000000, 0x0000001F, (SRFlags)0)]    // btst d1,d0
		[InlineData(new ushort[] { 0x0800, 0x0010 }, 0x00010000, 0x00000000, (SRFlags)0)]    // btst #16,d0
		[InlineData(new ushort[] { 0x0300 }, 0xFFFF0FFF, 0x0000000F, SRFlags.Zero)]    // btst d1,d0
		public void BTST_Register(ushort[] code, uint d0Val, uint d1Val, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D0 = d0Val,
				D1 = d1Val
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(new ushort[] { 0x0811, 0x0002 }, 0x0000, 0x00000000, SRFlags.Zero)]    // btst #2,(a1)
		[InlineData(new ushort[] { 0x0311 }, 0x8000, 0x0000001F, (SRFlags)0)]    // btst d1,(a1)
		[InlineData(new ushort[] { 0x0311 }, 0x0400, 0x0000001F, SRFlags.Zero)]    // btst d1,(a1)
		[InlineData(new ushort[] { 0x0811, 0x0010 }, 0x0100, 0x00000000, (SRFlags)0)]    // btst #16,(a1)
		[InlineData(new ushort[] { 0x0811, 0x0010 }, 0x0200, 0x00000000, SRFlags.Zero)]    // btst #16,(a1)
		[InlineData(new ushort[] { 0x0311 }, 0x0FFF, 0x0000000F, SRFlags.Zero)]    // btst d1,(a1)
		[InlineData(new ushort[] { 0x0311 }, 0x0FFF, 0x0000000B, (SRFlags)0)]    // btst d1,(a1)
		public void BTST_Memory(ushort[] code, ushort a1Val, uint d1Val, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				A1 = 0x00003000,
				D1 = d1Val
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { a1Val };
			machine.LoadData(data, 0x00003000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR);
		}

		[Theory]
		[InlineData(new ushort[] { 0x0840, 0x0002 }, 0x00000000, 0x00000000, 0x00000004, SRFlags.Zero)]    // bchg #2,d0
		[InlineData(new ushort[] { 0x0340 }, 0xC0000000, 0x0000001F, 0x40000000, (SRFlags)0)]    // bchg d1,d0
		[InlineData(new ushort[] { 0x0840, 0x0010 }, 0x00012345, 0x00000000, 0x00002345, (SRFlags)0)]    // bchg #16,d0
		[InlineData(new ushort[] { 0x0340 }, 0xFFFF0FFF, 0x0000000F, 0xFFFF8FFF, SRFlags.Zero)]    // bchg d1,d0
		public void BCHG_Register(ushort[] code, uint d0Val, uint d1Val, uint expectedResult, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D0 = d0Val,
				D1 = d1Val
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR);
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(0));
		}

		[Theory]
		[InlineData(new ushort[] { 0x0851, 0x0002 }, 0x0000, 0x00000000, 0x0400, SRFlags.Zero)]    // bchg #2,(a1)
		[InlineData(new ushort[] { 0x0351 }, 0xC000, 0x0000001F, 0x4000, (SRFlags)0)]    // bchg d1,(a1)
		[InlineData(new ushort[] { 0x0351 }, 0x0400, 0x0000001F, 0x8400, SRFlags.Zero)]    // bchg d1,(a1)
		[InlineData(new ushort[] { 0x0851, 0x0010 }, 0x0100, 0x00000000, 0x0000, (SRFlags)0)]    // bchg #16,(a1)
		[InlineData(new ushort[] { 0x0851, 0x0010 }, 0x0200, 0x00000000, 0x0300, SRFlags.Zero)]    // bchg #16,(a1)
		[InlineData(new ushort[] { 0x0351 }, 0x0FFF, 0x0000000F, 0x8FFF, SRFlags.Zero)]    // bchg d1,(a1)
		[InlineData(new ushort[] { 0x0351 }, 0x0FFF, 0x0000000B, 0x07FF, (SRFlags)0)]    // bchg d1,(a1)
		public void BCHG_Memory(ushort[] code, ushort a1Val, uint d1Val, ushort expectedResult, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				A1 = 0x00003000,
				D1 = d1Val
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { a1Val };
			machine.LoadData(data, 0x00003000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR);
			Assert.Equal(expectedResult, machine.Memory.ReadWord(0x00003000));
		}

		[Theory]
		[InlineData(new ushort[] { 0x0880, 0x0002 }, 0x0000FFF8, 0x00000000, 0x0000FFF8, SRFlags.Zero)]    // bclr #2,d0
		[InlineData(new ushort[] { 0x0380 }, 0xC0000000, 0x0000001F, 0x40000000, (SRFlags)0)]    // bclr d1,d0
		[InlineData(new ushort[] { 0x0880, 0x0010 }, 0x00032345, 0x00000000, 0x00022345, (SRFlags)0)]    // bclr #16,d0
		[InlineData(new ushort[] { 0x0380 }, 0xFFFF0FFF, 0x0000000F, 0xFFFF0FFF, SRFlags.Zero)]    // bclr d1,d0
		public void BCLR_Register(ushort[] code, uint d0Val, uint d1Val, uint expectedResult, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D0 = d0Val,
				D1 = d1Val
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR);
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(0));
		}

		[Theory]
		[InlineData(new ushort[] { 0x0891, 0x0002 }, 0xF000, 0x00000000, 0xF000, SRFlags.Zero)]    // bclr #2,(a1)
		[InlineData(new ushort[] { 0x0391 }, 0xC000, 0x0000001F, 0x4000, (SRFlags)0)]    // bclr d1,(a1)
		[InlineData(new ushort[] { 0x0391 }, 0x0400, 0x0000001F, 0x0400, SRFlags.Zero)]    // bclr d1,(a1)
		[InlineData(new ushort[] { 0x0891, 0x0010 }, 0xFF00, 0x00000000, 0xFE00, (SRFlags)0)]    // bclr #16,(a1)
		[InlineData(new ushort[] { 0x0891, 0x0010 }, 0x0200, 0x00000000, 0x0200, SRFlags.Zero)]    // bclr #16,(a1)
		[InlineData(new ushort[] { 0x0391 }, 0x0FFF, 0x0000000F, 0x0FFF, SRFlags.Zero)]    // bclr d1,(a1)
		[InlineData(new ushort[] { 0x0391 }, 0x0FFF, 0x0000000B, 0x07FF, (SRFlags)0)]    // bclr d1,(a1)
		public void BCLR_Memory(ushort[] code, ushort a1Val, uint d1Val, ushort expectedResult, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				A1 = 0x00003000,
				D1 = d1Val
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { a1Val };
			machine.LoadData(data, 0x00003000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR);
			Assert.Equal(expectedResult, machine.Memory.ReadWord(0x00003000));
		}

		[Theory]
		[InlineData(new ushort[] { 0x08C0, 0x0002 }, 0x0000FFF8, 0x00000000, 0x0000FFFC, SRFlags.Zero)]    // bset #2,d0
		[InlineData(new ushort[] { 0x03C0 }, 0xC0000000, 0x0000001F, 0xC0000000, (SRFlags)0)]    // bset d1,d0
		[InlineData(new ushort[] { 0x08C0, 0x0010 }, 0x00032345, 0x00000000, 0x00032345, (SRFlags)0)]    // bset #16,d0
		[InlineData(new ushort[] { 0x03C0 }, 0xFFFF0FFF, 0x0000000F, 0xFFFF8FFF, SRFlags.Zero)]    // bset d1,d0
		public void BSET_Register(ushort[] code, uint d0Val, uint d1Val, uint expectedResult, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				D0 = d0Val,
				D1 = d1Val
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR);
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(0));
		}

		[Theory]
		[InlineData(new ushort[] { 0x08D1, 0x0002 }, 0xF000, 0x00000000, 0xF400, SRFlags.Zero)]    // bclr #2,(a1)
		[InlineData(new ushort[] { 0x03D1 }, 0xC000, 0x0000001F, 0xC000, (SRFlags)0)]    // bclr d1,(a1)
		[InlineData(new ushort[] { 0x03D1 }, 0x0400, 0x0000001F, 0x8400, SRFlags.Zero)]    // bclr d1,(a1)
		[InlineData(new ushort[] { 0x08D1, 0x0010 }, 0xFF00, 0x00000000, 0xFF00, (SRFlags)0)]    // bclr #16,(a1)
		[InlineData(new ushort[] { 0x08D1, 0x0010 }, 0x0200, 0x00000000, 0x0300, SRFlags.Zero)]    // bclr #16,(a1)
		[InlineData(new ushort[] { 0x03D1 }, 0x0FFF, 0x0000000F, 0x8FFF, SRFlags.Zero)]    // bclr d1,(a1)
		[InlineData(new ushort[] { 0x03D1 }, 0x0FFF, 0x0000000B, 0x0FFF, (SRFlags)0)]    // bclr d1,(a1)
		public void BSET_Memory(ushort[] code, ushort a1Val, uint d1Val, ushort expectedResult, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				A1 = 0x00003000,
				D1 = d1Val
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { a1Val };
			machine.LoadData(data, 0x00003000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR);
			Assert.Equal(expectedResult, machine.Memory.ReadWord(0x00003000));
		}

		[Theory]
		[InlineData(new ushort[] { 0x4E50, 0x0100 }, 0x12345678, 0x000030FC)]    // link a0,#$100
		[InlineData(new ushort[] { 0x4E50, 0xFF00 }, 0x12345678, 0x00002EFC)]    // link a0,#-$100
		public void LINK(ushort[] code, uint a0Val, uint expectedSP)
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			CPUState initState = new CPUState
			{
				A0 = a0Val,
				USP = 0x00003000
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedSP, machine.CPU.USP);
			Assert.Equal((uint)0x00002FFC, machine.CPU.ReadAddressRegister(0));
			Assert.Equal((uint)0x12345678, machine.Memory.ReadLong(0x00002FFC));
		}

		[Fact]
		public void UNLK()
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(new ushort[] { 0x4E58 }, 0x0200);	// unlk a0
			CPUState initState = new CPUState
			{
				A0 = 0x00002FFC,
				USP = 0x00008000
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { 0x1234, 0x5678 };
			machine.LoadData(data, 0x00002FFC, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal((uint)0x00003000, machine.CPU.USP);
			Assert.Equal((uint)0x12345678, machine.CPU.ReadAddressRegister(0));
		}

		[Fact]
		public void STOP()
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(new ushort[] { 0x4E72, 0x2010 }, 0x0200);    // stop #$10
			CPUState initState = new CPUState
			{
				SR = SRFlags.SupervisorMode | SRFlags.Carry
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(SRFlags.SupervisorMode | SRFlags.Extend, machine.CPU.SR);
			Assert.True(machine.ExecutionStopped);
		}

		[Theory]
		[InlineData(SRFlags.Extend, 0x2010)]    // Not in supervisor mode but trying to go into supervisor mode
		[InlineData(SRFlags.SupervisorMode | SRFlags.Extend, 0x0010)]    // Already in supervisor mode but trying to come out of supervisor mode
		public void STOP_PrivilegeViolation(SRFlags initFlags, ushort operand)
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(new ushort[] { 0x4E72, operand }, 0x0200);    // stop #<operand>
			CPUState initState = new CPUState
			{
				SR = initFlags
			};
			machine.SetCPUState(initState);

			// Act and Assert
			Assert.Throws<TrapException>(() => machine.Execute());
		}

		[Theory]
		[InlineData(0x00, SRFlags.Zero)]
		[InlineData(0x7F, (SRFlags)0)]
		[InlineData(0xF0, SRFlags.Negative)]
		public void TAS(byte memVal, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(new ushort[] { 0x4AD4 }, 0x0200);    // tas (a4)
			CPUState initState = new CPUState
			{
				A4 = 0x00002000
			};
			machine.SetCPUState(initState);
			byte[] data = new byte[] { memVal };
			machine.LoadData(data, 0x00002000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR);
			Assert.Equal((byte)(memVal | 0x80), machine.Memory.ReadByte(0x00002000));
		}

		[Theory]
		[InlineData(0x00, 0x00, (SRFlags)0, 0x00, (SRFlags)0)]
		[InlineData(0x00, 0x00, SRFlags.Zero, 0x00, SRFlags.Zero)]
		[InlineData(0x00, 0x01, SRFlags.Zero, 0x01, (SRFlags)0)]
		[InlineData(0x00, 0x10, SRFlags.Extend, 0x11, (SRFlags)0)]
		[InlineData(0x04, 0x16, (SRFlags)0, 0x20, (SRFlags)0)]
		[InlineData(0x44, 0x56, (SRFlags)0, 0x00, SRFlags.Extend | SRFlags.Carry)]
		[InlineData(0x99, 0x00, SRFlags.Extend, 0x00, SRFlags.Extend | SRFlags.Carry)]
		public void ABCD_DataReg(byte d5Val, byte d6Val, SRFlags initFlags, byte expectedResult, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(new ushort[] { 0xCD05 }, 0x0200);    // abcd d5,d6
			CPUState initState = new CPUState
			{
				D5 = d5Val,
				D6 = d6Val,
				SR = initFlags
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR);
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(6));
		}

		[Theory]
		[InlineData(0x00, 0x00, (SRFlags)0, 0x00, (SRFlags)0)]
		[InlineData(0x00, 0x00, SRFlags.Zero, 0x00, SRFlags.Zero)]
		[InlineData(0x00, 0x01, SRFlags.Zero, 0x01, (SRFlags)0)]
		[InlineData(0x00, 0x10, SRFlags.Extend, 0x11, (SRFlags)0)]
		[InlineData(0x04, 0x16, (SRFlags)0, 0x20, (SRFlags)0)]
		[InlineData(0x44, 0x56, (SRFlags)0, 0x00, SRFlags.Extend | SRFlags.Carry)]
		[InlineData(0x99, 0x00, SRFlags.Extend, 0x00, SRFlags.Extend | SRFlags.Carry)]
		public void ABCD_Memory(byte a3Val, byte a4Val, SRFlags initFlags, byte expectedResult, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(new ushort[] { 0xC90B }, 0x0200);    // abcd -(a3),-(a4)
			CPUState initState = new CPUState
			{
				A3 = 0x00002001,
				A4 = 0x00002003,
				SR = initFlags
			};
			machine.SetCPUState(initState);
			byte[] data = new byte[] { a3Val, 0x00, a4Val, 0x00 };
			machine.LoadData(data, 0x00002000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR);
			Assert.Equal(expectedResult, machine.Memory.ReadByte(0x00002002));
			Assert.Equal((uint)0x00002000, machine.CPU.ReadAddressRegister(3));
			Assert.Equal((uint)0x00002002, machine.CPU.ReadAddressRegister(4));
		}

		[Theory]
		[InlineData(0x00, 0x00, (SRFlags)0, 0x00, (SRFlags)0)]
		[InlineData(0x00, 0x00, SRFlags.Zero, 0x00, SRFlags.Zero)]
		[InlineData(0x01, 0x02, SRFlags.Zero, 0x01, (SRFlags)0)]
		[InlineData(0x10, 0x22, SRFlags.Extend, 0x11, (SRFlags)0)]
		[InlineData(0x16, 0x20, (SRFlags)0, 0x04, (SRFlags)0)]
		[InlineData(0x99, 0x44, (SRFlags)0, 0x45, SRFlags.Extend | SRFlags.Carry)]
		[InlineData(0x00, 0x00, SRFlags.Extend, 0x99, SRFlags.Extend | SRFlags.Carry)]
		public void SBCD_DataReg(byte d5Val, byte d6Val, SRFlags initFlags, byte expectedResult, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(new ushort[] { 0x8D05 }, 0x0200);    // sbcd d5,d6
			CPUState initState = new CPUState
			{
				D5 = d5Val,
				D6 = d6Val,
				SR = initFlags
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR);
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(6));
		}

		[Theory]
		[InlineData(0x00, 0x00, (SRFlags)0, 0x00, (SRFlags)0)]
		[InlineData(0x00, 0x00, SRFlags.Zero, 0x00, SRFlags.Zero)]
		[InlineData(0x01, 0x02, SRFlags.Zero, 0x01, (SRFlags)0)]
		[InlineData(0x10, 0x22, SRFlags.Extend, 0x11, (SRFlags)0)]
		[InlineData(0x16, 0x20, (SRFlags)0, 0x04, (SRFlags)0)]
		[InlineData(0x99, 0x44, (SRFlags)0, 0x45, SRFlags.Extend | SRFlags.Carry)]
		[InlineData(0x00, 0x00, SRFlags.Extend, 0x99, SRFlags.Extend | SRFlags.Carry)]
		public void SBCD_Memory(byte a3Val, byte a4Val, SRFlags initFlags, byte expectedResult, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(new ushort[] { 0x890B }, 0x0200);    // sbcd -(a3),-(a4)
			CPUState initState = new CPUState
			{
				A3 = 0x00002001,
				A4 = 0x00002003,
				SR = initFlags
			};
			machine.SetCPUState(initState);
			byte[] data = new byte[] { a3Val, 0x00, a4Val, 0x00 };
			machine.LoadData(data, 0x00002000, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR);
			Assert.Equal(expectedResult, machine.Memory.ReadByte(0x00002002));
			Assert.Equal((uint)0x00002000, machine.CPU.ReadAddressRegister(3));
			Assert.Equal((uint)0x00002002, machine.CPU.ReadAddressRegister(4));
		}

		[Theory]
		[InlineData(0x00, (SRFlags)0, 0x00, (SRFlags)0)]
		[InlineData(0x00, SRFlags.Zero, 0x00, SRFlags.Zero)]
		[InlineData(0x28, SRFlags.Zero, 0x72, SRFlags.Carry | SRFlags.Extend)]
		[InlineData(0x28, SRFlags.Extend | SRFlags.Zero, 0x71, SRFlags.Carry | SRFlags.Extend)]
		[InlineData(0x99, SRFlags.Zero, 0x01, SRFlags.Extend | SRFlags.Carry)]
		[InlineData(0x99, SRFlags.Extend | SRFlags.Zero, 0x00, SRFlags.Extend | SRFlags.Carry | SRFlags.Zero)]
		public void NBCD(byte d2Val, SRFlags initFlags, byte expectedResult, SRFlags expectedFlags)
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(new ushort[] { 0x4802 }, 0x0200);    // nbcd d2
			CPUState initState = new CPUState
			{
				D2 = d2Val,
				SR = initFlags
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR);
			Assert.Equal(expectedResult, machine.CPU.ReadDataRegister(2));
		}

		[Fact]
		public void MOVEP_RegToMem_Word()
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(new ushort[] { 0x018C, 0x0100 }, 0x0200);    // movep.w d0,($100,a4)
			CPUState initState = new CPUState
			{
				D0 = 0x12345678,
				A4 = 0x00003000
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(0x56, machine.Memory.ReadByte(0x00003100));
			Assert.Equal(0x78, machine.Memory.ReadByte(0x00003102));
		}

		[Fact]
		public void MOVEP_RegToMem_Long()
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(new ushort[] { 0x01CC, 0x0100 }, 0x0200);    // movep.l d0,($100,a4)
			CPUState initState = new CPUState
			{
				D0 = 0x12345678,
				A4 = 0x00003000
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(0x12, machine.Memory.ReadByte(0x00003100));
			Assert.Equal(0x34, machine.Memory.ReadByte(0x00003102));
			Assert.Equal(0x56, machine.Memory.ReadByte(0x00003104));
			Assert.Equal(0x78, machine.Memory.ReadByte(0x00003106));
		}

		[Fact]
		public void MOVEP_MemToReg_Word()
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(new ushort[] { 0x010C, 0x0100 }, 0x0200);    // movep.w ($100,a4),d0
			CPUState initState = new CPUState
			{
				D0 = 0xAAAAAAAA,
				A4 = 0x00003000
			};
			machine.SetCPUState(initState);
			byte[] data = new byte[] { 0x56, 0x00, 0x78, 0x00 };
			machine.LoadData(data, 0x00003100, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal(0xAAAA5678, machine.CPU.ReadDataRegister(0));
		}

		[Fact]
		public void MOVEP_MemToReg_Long()
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(new ushort[] { 0x014C, 0x0100 }, 0x0200);    // movep.l ($100,a4),d0
			CPUState initState = new CPUState
			{
				D0 = 0xAAAAAAAA,
				A4 = 0x00003000
			};
			machine.SetCPUState(initState);
			byte[] data = new byte[] { 0x12, 0x00, 0x34, 0x00, 0x56, 0x00, 0x78, 0x00 };
			machine.LoadData(data, 0x00003100, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal((uint)0x12345678, machine.CPU.ReadDataRegister(0));
		}

		[Fact]
		public void MOVEM_RegToMem_Word()
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(new ushort[] { 0x48A7, 0x0E70 }, 0x0200);    // movem.w d4-d6/a1-a3,-(a7)
			CPUState initState = new CPUState
			{
				D4 = 0x12345678,
				D5 = 0xAAAAAAAA,
				D6 = 0x55555555,
				A1 = 0x87654321,
				A2 = 0x10293847,
				A3 = 0x56473829,
				USP = 0x00003000
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal((uint)0x00002FF4, machine.CPU.USP);
			Assert.Equal(0x5678, machine.Memory.ReadWord(0x00002FF4));
			Assert.Equal(0xAAAA, machine.Memory.ReadWord(0x00002FF6));
			Assert.Equal(0x5555, machine.Memory.ReadWord(0x00002FF8));
			Assert.Equal(0x4321, machine.Memory.ReadWord(0x00002FFA));
			Assert.Equal(0x3847, machine.Memory.ReadWord(0x00002FFC));
			Assert.Equal(0x3829, machine.Memory.ReadWord(0x00002FFE));
		}

		[Fact]
		public void MOVEM_RegToMem_Long()
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(new ushort[] { 0x48E7, 0x0E70 }, 0x0200);    // movem.l d4-d6/a1-a3,-(a7)
			CPUState initState = new CPUState
			{
				D4 = 0x12345678,
				D5 = 0xAAAAAAAA,
				D6 = 0x55555555,
				A1 = 0x87654321,
				A2 = 0x10293847,
				A3 = 0x56473829,
				USP = 0x00003000
			};
			machine.SetCPUState(initState);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal((uint)0x00002FE8, machine.CPU.USP);
			Assert.Equal((uint)0x12345678, machine.Memory.ReadLong(0x00002FE8));
			Assert.Equal((uint)0xAAAAAAAA, machine.Memory.ReadLong(0x00002FEC));
			Assert.Equal((uint)0x55555555, machine.Memory.ReadLong(0x00002FF0));
			Assert.Equal((uint)0x87654321, machine.Memory.ReadLong(0x00002FF4));
			Assert.Equal((uint)0x10293847, machine.Memory.ReadLong(0x00002FF8));
			Assert.Equal((uint)0x56473829, machine.Memory.ReadLong(0x00002FFC));
		}

		[Fact]
		public void MOVEM_MemToReg_Word()
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(new ushort[] { 0x4C9F, 0x0E70 }, 0x0200);    // movem.w (a7)+,d4-d6/a1-a3
			CPUState initState = new CPUState
			{
				USP = 0x00002FF4
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { 0x5678, 0xAAAA, 0x5555, 0x4321, 0x3847, 0xFFFF };
			machine.LoadData(data, 0x00002FF4, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal((uint)0x00003000, machine.CPU.USP);
			Assert.Equal((uint)0x00005678, machine.CPU.ReadDataRegister(4));
			Assert.Equal((uint)0xFFFFAAAA, machine.CPU.ReadDataRegister(5));
			Assert.Equal((uint)0x00005555, machine.CPU.ReadDataRegister(6));
			Assert.Equal((uint)0x00004321, machine.CPU.ReadAddressRegister(1));
			Assert.Equal((uint)0x00003847, machine.CPU.ReadAddressRegister(2));
			Assert.Equal((uint)0xFFFFFFFF, machine.CPU.ReadAddressRegister(3));
		}

		[Fact]
		public void MOVEM_MemToReg_Long()
		{
			Machine machine = new Machine();
			machine.LoadExecutableData(new ushort[] { 0x4CDF, 0x0E70 }, 0x0200);    // movem.l (a7)+,d4-d6/a1-a3
			CPUState initState = new CPUState
			{
				D4 = 0x12340000,
				D5 = 0xAAAA0000,
				D6 = 0x55550000,
				A1 = 0x87650000,
				A2 = 0x10290000,
				A3 = 0x56470000,
				USP = 0x00002FE8
			};
			machine.SetCPUState(initState);
			ushort[] data = new ushort[] { 0x1234, 0x5678, 0xAAAA, 0xAAAA, 0x5555, 0x5555, 0x8765, 0x4321, 0x1029, 0x3847, 0x5647, 0x3829 };
			machine.LoadData(data, 0x00002FE8, false);

			// Act
			machine.Execute();

			// Assert
			Assert.Equal((uint)0x00003000, machine.CPU.USP);
			Assert.Equal((uint)0x12345678, machine.CPU.ReadDataRegister(4));
			Assert.Equal((uint)0xAAAAAAAA, machine.CPU.ReadDataRegister(5));
			Assert.Equal((uint)0x55555555, machine.CPU.ReadDataRegister(6));
			Assert.Equal((uint)0x87654321, machine.CPU.ReadAddressRegister(1));
			Assert.Equal((uint)0x10293847, machine.CPU.ReadAddressRegister(2));
			Assert.Equal((uint)0x56473829, machine.CPU.ReadAddressRegister(3));
		}


	}
}
