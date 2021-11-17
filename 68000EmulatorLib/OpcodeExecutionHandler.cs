using PendleCodeMonkey.MC68000EmulatorLib.Enumerations;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PendleCodeMonkey.MC68000EmulatorLib
{
	/// <summary>
	/// Implementation of the <see cref="OpcodeExecutionHandler"/> class.
	/// </summary>
	class OpcodeExecutionHandler
	{
		private Dictionary<OpHandlerID, Action<Instruction>> _handlers = new Dictionary<OpHandlerID, Action<Instruction>>();

		private readonly uint[] _bit = new uint[] { 0x00000001, 0x00000002, 0x00000004, 0x00000008, 0x00000010, 0x00000020, 0x00000040, 0x00000080,
													0x00000100, 0x00000200, 0x00000400, 0x00000800, 0x00001000, 0x00002000, 0x00004000, 0x00008000,
													0x00010000, 0x00020000, 0x00040000, 0x00080000, 0x00100000, 0x00200000, 0x00400000, 0x00800000,
													0x01000000, 0x02000000, 0x04000000, 0x08000000, 0x10000000, 0x20000000, 0x40000000, 0x80000000 };

		private readonly object lockObj = new object();     // Object that is locked for TAS instruction.

		internal int _numberOfJSRCalls = 0;

		/// <summary>
		/// Initializes a new instance of the <see cref="OpcodeExecutionHandler"/> class.
		/// </summary>
		/// <param name="machine">The <see cref="Machine"/> instance for which this object is handling the execution of instructions.</param>
		public OpcodeExecutionHandler(Machine machine)
		{
			Machine = machine ?? throw new ArgumentNullException(nameof(machine));

			InitOpcodeHandlers();
		}

		/// <summary>
		/// Gets or sets the <see cref="Machine"/> instance for which this <see cref="OpcodeExecutionHandler"/> instance
		/// is handling the execution of instructions
		/// </summary>
		private Machine Machine { get; set; }


		/// <summary>
		/// Initialize the dictionary of Opcode handlers.
		/// </summary>
		/// <remarks>
		/// Maps an enumerated operation handler ID to an Action that performs the operation.
		/// </remarks>
		private void InitOpcodeHandlers()
		{
			_handlers.Add(OpHandlerID.ORItoCCR, ORItoCCR);
			_handlers.Add(OpHandlerID.ORItoSR, ORItoSR);
			_handlers.Add(OpHandlerID.ORI, ORI);
			_handlers.Add(OpHandlerID.ANDItoCCR, ANDItoCCR);
			_handlers.Add(OpHandlerID.ANDItoSR, ANDItoSR);
			_handlers.Add(OpHandlerID.ANDI, ANDI);
			_handlers.Add(OpHandlerID.SUBI, SUBI);
			_handlers.Add(OpHandlerID.ADDI, ADDI);
			_handlers.Add(OpHandlerID.EORItoCCR, EORItoCCR);
			_handlers.Add(OpHandlerID.EORItoSR, EORItoSR);
			_handlers.Add(OpHandlerID.EORI, EORI);
			_handlers.Add(OpHandlerID.CMPI, CMPI);
			_handlers.Add(OpHandlerID.MOVE, MOVE);
			_handlers.Add(OpHandlerID.MOVEA, MOVEA);
			_handlers.Add(OpHandlerID.MOVEfromSR, MOVEfromSR);
			_handlers.Add(OpHandlerID.MOVEtoCCR, MOVEtoCCR);
			_handlers.Add(OpHandlerID.MOVEtoSR, MOVEtoSR);
			_handlers.Add(OpHandlerID.NEGX, NEGX);
			_handlers.Add(OpHandlerID.CLR, CLR);
			_handlers.Add(OpHandlerID.NEG, NEG);
			_handlers.Add(OpHandlerID.NOT, NOT);
			_handlers.Add(OpHandlerID.EXT, EXT);
			_handlers.Add(OpHandlerID.SWAP, SWAP);
			_handlers.Add(OpHandlerID.PEA, PEA);
			_handlers.Add(OpHandlerID.ILLEGAL, ILLEGAL);
			_handlers.Add(OpHandlerID.TST, TST);
			_handlers.Add(OpHandlerID.TRAP, TRAP);
			_handlers.Add(OpHandlerID.MOVEUSP, MOVEUSP);
			_handlers.Add(OpHandlerID.RESET, RESET);
			_handlers.Add(OpHandlerID.NOP, NOP);
			_handlers.Add(OpHandlerID.RTE, RTE);
			_handlers.Add(OpHandlerID.RTS, RTS);
			_handlers.Add(OpHandlerID.TRAPV, TRAPV);
			_handlers.Add(OpHandlerID.RTR, RTR);
			_handlers.Add(OpHandlerID.JSR, JSR);
			_handlers.Add(OpHandlerID.JMP, JMP);
			_handlers.Add(OpHandlerID.LEA, LEA);
			_handlers.Add(OpHandlerID.CHK, CHK);
			_handlers.Add(OpHandlerID.ADDQ, ADDQ);
			_handlers.Add(OpHandlerID.SUBQ, SUBQ);
			_handlers.Add(OpHandlerID.Scc, Scc);
			_handlers.Add(OpHandlerID.DBcc, DBcc);
			_handlers.Add(OpHandlerID.BRA, BRA);
			_handlers.Add(OpHandlerID.BSR, BSR);
			_handlers.Add(OpHandlerID.Bcc, Bcc);
			_handlers.Add(OpHandlerID.MOVEQ, MOVEQ);
			_handlers.Add(OpHandlerID.DIVU, DIVU);
			_handlers.Add(OpHandlerID.DIVS, DIVS);
			_handlers.Add(OpHandlerID.OR, OR);
			_handlers.Add(OpHandlerID.SUB, SUB);
			_handlers.Add(OpHandlerID.SUBX, SUBX);
			_handlers.Add(OpHandlerID.SUBA, SUBA);
			_handlers.Add(OpHandlerID.EOR, EOR);
			_handlers.Add(OpHandlerID.CMPM, CMPM);
			_handlers.Add(OpHandlerID.CMP, CMP);
			_handlers.Add(OpHandlerID.CMPA, CMPA);
			_handlers.Add(OpHandlerID.MULU, MULU);
			_handlers.Add(OpHandlerID.MULS, MULS);
			_handlers.Add(OpHandlerID.EXG, EXG);
			_handlers.Add(OpHandlerID.AND, AND);
			_handlers.Add(OpHandlerID.ADD, ADD);
			_handlers.Add(OpHandlerID.ADDX, ADDX);
			_handlers.Add(OpHandlerID.ADDA, ADDA);
			_handlers.Add(OpHandlerID.ASL, ASL_ASR_LSL_LSR);
			_handlers.Add(OpHandlerID.ASR, ASL_ASR_LSL_LSR);
			_handlers.Add(OpHandlerID.LSL, ASL_ASR_LSL_LSR);
			_handlers.Add(OpHandlerID.LSR, ASL_ASR_LSL_LSR);
			_handlers.Add(OpHandlerID.ROL, ROL_ROR_ROXL_ROXR);
			_handlers.Add(OpHandlerID.ROR, ROL_ROR_ROXL_ROXR);
			_handlers.Add(OpHandlerID.ROXL, ROL_ROR_ROXL_ROXR);
			_handlers.Add(OpHandlerID.ROXR, ROL_ROR_ROXL_ROXR);
			_handlers.Add(OpHandlerID.BTST, BTST_BCHG_BCLR_BSET);
			_handlers.Add(OpHandlerID.BCHG, BTST_BCHG_BCLR_BSET);
			_handlers.Add(OpHandlerID.BCLR, BTST_BCHG_BCLR_BSET);
			_handlers.Add(OpHandlerID.BSET, BTST_BCHG_BCLR_BSET);
			_handlers.Add(OpHandlerID.LINK, LINK);
			_handlers.Add(OpHandlerID.UNLK, UNLK);
			_handlers.Add(OpHandlerID.STOP, STOP);
			_handlers.Add(OpHandlerID.TAS, TAS);
			_handlers.Add(OpHandlerID.ABCD, ABCD_SBCD);
			_handlers.Add(OpHandlerID.SBCD, ABCD_SBCD);
			_handlers.Add(OpHandlerID.NBCD, NBCD);
			_handlers.Add(OpHandlerID.MOVEP, MOVEP);
			_handlers.Add(OpHandlerID.MOVEM, MOVEM);
		}

		/// <summary>
		/// Get a sized operand data value.
		/// </summary>
		/// <param name="size">The size of the operand data (Word or Long).</param>
		/// <param name="ext1">The first extension word value (can be null).</param>
		/// <param name="ext2">The second extension word value (can be null).</param>
		/// <returns>A 32-bit value containing the operand data (or null if no value is available).</returns>
		private uint? GetSizedOperandValue(OpSize size, ushort? ext1, ushort? ext2)
		{
			uint? value = null;
			if (ext1.HasValue)
			{
				if (size == OpSize.Long)
				{
					if (ext2.HasValue)
					{
						value = (uint)((ext1.Value << 16) + ext2.Value);
					}
				}
				else
				{
					value = ext1.Value;
				}
			}

			return value;
		}

		/// <summary>
		/// Sets the state of the flags for the specified instruction type based on supplied result, source, and destination values.
		/// </summary>
		/// <param name="opID">The type of the instruction.</param>
		/// <param name="size">The size of the result, source, and destination values.</param>
		/// <param name="result">The result of the operation performed by the instruction.</param>
		/// <param name="source">The source value supplied to the operation performed by the instruction.</param>
		/// <param name="dest">The destination value supplied to the operation performed by the instruction.</param>
		private void SetFlags(OpHandlerID opID, OpSize size, uint result, uint source = 0, uint dest = 0)
		{
			var msb = Helpers.SizeMSB(size);
			var mask = Helpers.SizeMask(size);
			bool srcMsbSet = (source & msb) != 0;
			bool destMsbSet = (dest & msb) != 0;
			bool resultMsbSet = (result & msb) != 0;
			switch (opID)
			{
				case OpHandlerID.ORI:
				case OpHandlerID.ANDI:
				case OpHandlerID.EORI:
				case OpHandlerID.MOVE:
				case OpHandlerID.MOVEQ:
				case OpHandlerID.DIVU:
				case OpHandlerID.DIVS:
				case OpHandlerID.OR:
				case OpHandlerID.EOR:
				case OpHandlerID.MULU:
				case OpHandlerID.MULS:
				case OpHandlerID.AND:
					Machine.CPU.NegativeFlag = resultMsbSet;
					Machine.CPU.ZeroFlag = (result & mask) == 0;
					Machine.CPU.OverflowFlag = false;
					Machine.CPU.CarryFlag = false;
					break;
				case OpHandlerID.SUBI:
				case OpHandlerID.SUBQ:
				case OpHandlerID.SUB:
				case OpHandlerID.SUBX:
					Machine.CPU.NegativeFlag = resultMsbSet;
					Machine.CPU.ZeroFlag = (result & mask) == 0;
					Machine.CPU.OverflowFlag = (srcMsbSet == resultMsbSet) && (destMsbSet != resultMsbSet);
					Machine.CPU.CarryFlag = (srcMsbSet && resultMsbSet) || (srcMsbSet && !destMsbSet) || (!destMsbSet && resultMsbSet);
					Machine.CPU.ExtendFlag = Machine.CPU.CarryFlag;
					break;
				case OpHandlerID.ADDI:
				case OpHandlerID.ADDQ:
				case OpHandlerID.ADD:
				case OpHandlerID.ADDX:
					Machine.CPU.NegativeFlag = resultMsbSet;
					Machine.CPU.ZeroFlag = (result & mask) == 0;
					Machine.CPU.OverflowFlag = (srcMsbSet == destMsbSet) && (srcMsbSet != resultMsbSet);
					Machine.CPU.CarryFlag = (srcMsbSet && !resultMsbSet) || (srcMsbSet && destMsbSet) || (destMsbSet && !resultMsbSet);
					Machine.CPU.ExtendFlag = Machine.CPU.CarryFlag;
					break;
				case OpHandlerID.CMPI:
				case OpHandlerID.CMPM:
				case OpHandlerID.CMP:
				case OpHandlerID.CMPA:
					Machine.CPU.NegativeFlag = resultMsbSet;
					Machine.CPU.ZeroFlag = (result & mask) == 0;
					Machine.CPU.OverflowFlag = (srcMsbSet == resultMsbSet) && (destMsbSet != resultMsbSet);
					Machine.CPU.CarryFlag = (srcMsbSet && resultMsbSet) || (srcMsbSet && !destMsbSet) || (!destMsbSet && resultMsbSet);
					break;
				case OpHandlerID.NEGX:
					Machine.CPU.NegativeFlag = resultMsbSet;
					Machine.CPU.ZeroFlag = (result & mask) == 0;
					Machine.CPU.OverflowFlag = srcMsbSet && resultMsbSet;
					Machine.CPU.CarryFlag = Machine.CPU.ExtendFlag = srcMsbSet || resultMsbSet;
					break;
				case OpHandlerID.NEG:
					Machine.CPU.NegativeFlag = resultMsbSet;
					Machine.CPU.ZeroFlag = (result & mask) == 0;
					Machine.CPU.OverflowFlag = srcMsbSet && resultMsbSet;
					Machine.CPU.CarryFlag = Machine.CPU.ExtendFlag = (result & mask) != 0;
					break;
				case OpHandlerID.NOT:
				case OpHandlerID.EXT:
				case OpHandlerID.SWAP:
				case OpHandlerID.TST:
					Machine.CPU.NegativeFlag = resultMsbSet;
					Machine.CPU.ZeroFlag = (result & mask) == 0;
					Machine.CPU.OverflowFlag = Machine.CPU.CarryFlag = false;
					break;
				case OpHandlerID.ASL:
				case OpHandlerID.ASR:
				case OpHandlerID.LSL:
				case OpHandlerID.LSR:
					Machine.CPU.NegativeFlag = resultMsbSet;
					Machine.CPU.ZeroFlag = (result & mask) == 0;
					if (source != 0)
					{
						Machine.CPU.CarryFlag = Machine.CPU.ExtendFlag = dest != 0;
					}
					else
					{
						Machine.CPU.CarryFlag = false;
					}
					break;
				case OpHandlerID.ROL:
				case OpHandlerID.ROR:
					Machine.CPU.NegativeFlag = resultMsbSet;
					Machine.CPU.ZeroFlag = (result & mask) == 0;
					Machine.CPU.CarryFlag = source != 0 && dest != 0;
					Machine.CPU.OverflowFlag = false;
					break;
				case OpHandlerID.ROXL:
				case OpHandlerID.ROXR:
					Machine.CPU.NegativeFlag = resultMsbSet;
					Machine.CPU.ZeroFlag = (result & mask) == 0;
					if (source != 0)
					{
						Machine.CPU.CarryFlag = Machine.CPU.ExtendFlag = dest != 0;
					}
					else
					{
						Machine.CPU.CarryFlag = false;
					}
					Machine.CPU.OverflowFlag = false;
					break;

			}
		}

		/// <summary>
		/// Perform a Binary Coded Decimal (BCD) operation (i.e. addition or subtraction).
		/// </summary>
		/// <param name="opType">The type of BCD operation (Addition or Subtraction).</param>
		/// <param name="src">The source value to be used in the calculation.</param>
		/// <param name="dest">The destination value to be used in the calculation.</param>
		/// <returns>The result of the BCD operation.</returns>
		private uint BCDCalculation(BCDOperation opType, uint src, uint dest)
		{
			int loVal;
			int hiVal;
			bool carry;
			if (opType == BCDOperation.Addition)
			{
				loVal = (int)((src & 0x000F) + (dest & 0x000F) + (Machine.CPU.ExtendFlag ? 1 : 0));
				carry = loVal > 9;
				if (loVal > 9)
				{
					loVal -= 10;
				}
				hiVal = (int)(((src >> 4) & 0x000F) + ((dest >> 4) & 0x000F) + (carry ? 1 : 0));
				carry = hiVal > 9;
				if (hiVal > 9)
				{
					hiVal -= 10;
				}
			}
			else
			{
				loVal = (int)((dest & 0x000F) - (src & 0x000F) - (Machine.CPU.ExtendFlag ? 1 : 0));
				carry = loVal < 0;
				if (loVal < 0)
				{
					loVal += 10;
				}
				hiVal = (int)(((dest >> 4) & 0x000F) - ((src >> 4) & 0x000F) - (carry ? 1 : 0));
				carry = hiVal < 0;
				if (hiVal < 0)
				{
					hiVal += 10;
				}
			}

			var result = (hiVal << 4) + loVal;
			Machine.CPU.CarryFlag = Machine.CPU.ExtendFlag = carry;
			if (result != 0)
			{
				Machine.CPU.ZeroFlag = false;
			}

			return (uint)(result & 0x000000FF);
		}

		/// <summary>
		/// Memory to Register functionality for the MOVEM instruction.
		/// </summary>
		/// <param name="regMask">16-bit register mask.</param>
		/// <param name="address">Address at which to start reading values into the registers.</param>
		/// <param name="size">The size of the values to be transferred (Word or Long).</param>
		/// <returns>The address at which the transfer completed.</returns>
		private uint MOVEM_MemToReg(ushort regMask, uint address, OpSize size)
		{
			for (int n = 0; n < 16; n++)
			{
				if ((regMask & _bit[n]) != 0)
				{
					uint value = size == OpSize.Long ? Machine.Memory.ReadLong(address) : (uint)Helpers.SignExtendValue(Machine.Memory.ReadWord(address));
					if (n < 8)
					{
						Machine.CPU.WriteDataRegister(n, value, OpSize.Long);
					}
					else
					{
						Machine.CPU.WriteAddressRegister(n - 8, value, OpSize.Long);
					}
					address += (uint)(size == OpSize.Long ? 4 : 2);
				}
			}
			return address;
		}

		/// <summary>
		/// Register to Memory functionality for the MOVEM instruction (for all but predecrement addressing mode).
		/// </summary>
		/// <param name="regMask">16-bit register mask.</param>
		/// <param name="address">Address at which to start writing register values.</param>
		/// <param name="size">The size of the values to be transferred (Word or Long).</param>
		/// <returns>The address at which the transfer completed.</returns>
		private uint MOVEM_RegToMem(ushort regMask, uint address, OpSize size)
		{
			for (int n = 0; n < 16; n++)
			{
				if ((regMask & _bit[n]) != 0)
				{
					if (n < 8)
					{
						if (size == OpSize.Long)
						{
							Machine.Memory.WriteLong(address, Machine.CPU.ReadDataRegister(n));
						}
						else
						{
							Machine.Memory.WriteWord(address, (ushort)(Machine.CPU.ReadDataRegister(n) & 0x0000FFFF));
						}
					}
					else
					{
						if (size == OpSize.Long)
						{
							Machine.Memory.WriteLong(address, Machine.CPU.ReadAddressRegister(n));
						}
						else
						{
							Machine.Memory.WriteWord(address, (ushort)(Machine.CPU.ReadAddressRegister(n) & 0x0000FFFF));
						}
					}
					address += (uint)(size == OpSize.Long ? 4 : 2);
				}
			}
			return address;
		}

		/// <summary>
		/// Register to Memory functionality for the MOVEM instruction (for predecrement addressing mode).
		/// </summary>
		/// <param name="regMask">16-bit register mask.</param>
		/// <param name="address">Address at which to start writing register values.</param>
		/// <param name="size">The size of the values to be transferred (Word or Long).</param>
		/// <returns>The address at which the transfer completed.</returns>
		private uint MOVEM_RegToMemPreDec(ushort regMask, uint address, OpSize size)
		{
			// Increment address because it has already been predecremented once prior to calling this method.
			address += (uint)(size == OpSize.Long ? 4 : 2);
			for (int n = 0; n < 16; n++)
			{
				if ((regMask & _bit[n]) != 0)
				{
					address -= (uint)(size == OpSize.Long ? 4 : 2);
					if (n < 8)
					{
						if (size == OpSize.Long)
						{
							Machine.Memory.WriteLong(address, Machine.CPU.ReadAddressRegister(7 - n));
						}
						else
						{
							Machine.Memory.WriteWord(address, (ushort)(Machine.CPU.ReadAddressRegister(7 - n) & 0x0000FFFF));
						}
					}
					else
					{
						if (size == OpSize.Long)
						{
							Machine.Memory.WriteLong(address, Machine.CPU.ReadDataRegister(15 - n));
						}
						else
						{
							Machine.Memory.WriteWord(address, (ushort)(Machine.CPU.ReadDataRegister(15 - n) & 0x0000FFFF));
						}
					}
				}
			}
			return address;
		}



		/// <summary>
		/// Execute the specified instruction.
		/// </summary>
		/// <param name="instruction">The <see cref="Instruction"/> instance of the instruction to be executed.</param>
		internal void Execute(Instruction instruction)
		{
			if (_handlers.ContainsKey(instruction.Info.HandlerID))
			{
				_handlers[instruction.Info.HandlerID]?.Invoke(instruction);          // Call the handler Action.
			}
		}

		/// <summary>
		/// Read the specified effective address value for the supplied instruction.
		/// </summary>
		/// <param name="instruction">The <see cref="Instruction"/> instance.</param>
		/// <param name="eaType">The type of effective address data to be retrieved (Source or Destination).</param>
		/// <returns>32-bit value containing the effective address value (or null if no value available).</returns>
		private uint? ReadEAValue(Instruction instruction, EAType eaType)
		{
			uint? value = null;
			OpSize size = instruction.Size ?? OpSize.Word;
			var (dataRegNum, addrRegNum, address, immValue) = EvaluateEffectiveAddress(instruction, eaType);
			if (dataRegNum.HasValue)
			{
				value = Machine.CPU.ReadDataRegister(dataRegNum.Value);
			}
			else if (addrRegNum.HasValue)
			{
				value = Machine.CPU.ReadAddressRegister(addrRegNum.Value);
			}
			else if (address.HasValue)
			{
				value = size switch
				{
					OpSize.Byte => Machine.Memory.ReadByte(address.Value),
					OpSize.Long => Machine.Memory.ReadLong(address.Value),
					_ => Machine.Memory.ReadWord(address.Value),
				};
			}
			else if (immValue.HasValue)
			{
				value = immValue.Value;
			}

			if (value.HasValue)
			{
				return SizedValue(value.Value, size);
			}
			return null;
		}

		/// <summary>
		/// Write the specified effective address value for the supplied instruction.
		/// </summary>
		/// <param name="instruction">The <see cref="Instruction"/> instance.</param>
		/// <param name="value">The data to be written.</param>
		/// <param name="eaType">The type of effective address data to be written (Source or Destination).</param>
		private void WriteEAValue(Instruction instruction, uint value, EAType eaType)
		{
			OpSize size = instruction.Size ?? OpSize.Word;
			var (dataRegNum, addrRegNum, address, immValue) = EvaluateEffectiveAddress(instruction, eaType);
			if (dataRegNum.HasValue)
			{
				Machine.CPU.WriteDataRegister(dataRegNum.Value, value, size);
			}
			else if (addrRegNum.HasValue)
			{
				Machine.CPU.WriteAddressRegister(addrRegNum.Value, value, size);
			}
			else if (address.HasValue)
			{
				switch (size)
				{
					case OpSize.Byte:
						Machine.Memory.WriteByte(address.Value, (byte)(value & 0x000000FF));
						break;
					case OpSize.Word:
						Machine.Memory.WriteWord(address.Value, (ushort)(value & 0x0000FFFF));
						break;
					default:
						Machine.Memory.WriteLong(address.Value, value);
						break;
				}
			}
			else if (immValue.HasValue)
			{
				Debug.Assert(false, "Immediate addressing mode cannot be used for Write operations.");
			}
		}

		/// <summary>
		/// Returns the supplied value to the specified data size. 
		/// </summary>
		/// <param name="value">The data value.</param>
		/// <param name="size">The size at which the value should be returned (Byte, Word, or Long).</param>
		/// <returns>The value after being restricted to the specified size.</returns>
		internal uint SizedValue(uint value, OpSize size)
		{
			return size switch
			{
				OpSize.Byte => value & 0x000000FF,
				OpSize.Word => value & 0x0000FFFF,
				_ => value,
			};
		}

		/// <summary>
		/// Evaluate the specified effective address (EA).
		/// </summary>
		/// <param name="instruction">The <see cref="Instruction"/> instance.</param>
		/// <param name="eaType">The type of effective address to be evaluated (Source or Destination).</param>
		/// <returns>
		/// A tuple consisting of the following:
		///		dataRegNum - The data register number (if the EA mode is Data Register Direct) or null if any other EA mode.
		///		addrRegNum - The address register number (if the EA mode is Address Register Direct) or null if any other EA mode.
		///		address - The memory address (if the EA mode is one of the indirect memory access modes) or null if any other EA mode.
		///		immValue - The immediate operand data (if the EA mode is Immediate) or null if any other EA mode.
		/// </returns>
		internal (byte? dataRegNum, byte? addrRegNum, uint? address, uint? immValue) EvaluateEffectiveAddress(Instruction instruction, EAType eaType)
		{
			ushort? ea = eaType == EAType.Source ? instruction.SourceAddrMode : instruction.DestAddrMode;
			ushort? ext1 = eaType == EAType.Source ? instruction.SourceExtWord1 : instruction.DestExtWord1;
			ushort? ext2 = eaType == EAType.Source ? instruction.SourceExtWord2 : instruction.DestExtWord2;

			byte? dRegNum = null;
			byte? aRegNum = null;
			uint? address = null;
			uint? immVal = null;
			if (ea.HasValue)
			{
				OpSize size = instruction.Size ?? OpSize.Word;
				uint sizeInBytes = (uint)(size == OpSize.Byte ? 1 : size == OpSize.Long ? 4 : 2);

				// Get register number (for addressing modes that use a register)
				ushort regNum = (ushort)(ea & 0x0007);
				switch (ea & 0x0038)
				{
					case (byte)AddrMode.DataRegister:
						dRegNum = (byte)regNum;
						break;
					case (byte)AddrMode.AddressRegister:
						aRegNum = (byte)regNum;
						break;
					case (byte)AddrMode.Address:
						address = Machine.CPU.ReadAddressRegister(regNum);
						break;
					case (byte)AddrMode.AddressPostInc:
						// Special case: If working with SP (i.e. A7) and a size of 1 byte then use a 2 byte increment (to keep SP address even).
						if (regNum == 7 && sizeInBytes == 1)
						{
							sizeInBytes = 2;
						}
						address = Machine.CPU.ReadAddressRegister(regNum);
						Machine.CPU.WriteAddressRegister(regNum, address.Value + sizeInBytes);
						break;
					case (byte)AddrMode.AddressPreDec:
						// Special case: If working with SP (i.e. A7) and a size of 1 byte then use a 2 byte decrement (to keep SP address even).
						if (regNum == 7 && sizeInBytes == 1)
						{
							sizeInBytes = 2;
						}
						address = Machine.CPU.ReadAddressRegister(regNum) - sizeInBytes;
						Machine.CPU.WriteAddressRegister(regNum, address.Value);
						break;
					case (byte)AddrMode.AddressDisp:
						Debug.Assert(ext1.HasValue, "Required extension word is not available");
						if (ext1.HasValue)
						{
							address = (uint)(Machine.CPU.ReadAddressRegister(regNum) + (short)ext1.Value);
						}
						break;
					case (byte)AddrMode.AddressIndex:
						Debug.Assert(ext1.HasValue, "Required extension word is not available");
						if (ext1.HasValue)
						{
							byte disp = (byte)(ext1.Value & 0x00FF);
							byte indexRegNum = (byte)((ext1.Value & 0x7000) >> 12);
							OpSize indexSize = (ext1.Value & 0x0800) == 0 ? OpSize.Word : OpSize.Long;
							uint indexValue = Machine.CPU.ReadDataRegister(indexRegNum);
							if (indexSize == OpSize.Word)
							{
								indexValue = (indexValue & 0x0000FFFF) | ((indexValue & 0x00008000) == 0 ? 0x0 : 0xFFFF0000);
							}
							address = (uint)(Machine.CPU.ReadAddressRegister(regNum) + (int)indexValue + (sbyte)disp);
						}
						break;
					case 0x0038:
						switch (ea)
						{
							case (byte)AddrMode.AbsShort:
								Debug.Assert(ext1.HasValue, "Required extension word is not available");
								if (ext1.HasValue)
								{
									address = ext1.Value | ((ext1.Value & 0x8000) == 0 ? 0x0 : 0xFFFF0000);
								}
								break;
							case (byte)AddrMode.AbsLong:
								Debug.Assert(ext1.HasValue && ext2.HasValue, "Required extension word is not available");
								if (ext1.HasValue && ext2.HasValue)
								{
									address = (uint)((ext1.Value << 16) + ext2.Value);
								}
								break;
							case (byte)AddrMode.PCDisp:
								Debug.Assert(ext1.HasValue, "Required extension word is not available");
								if (ext1.HasValue)
								{
									address = (uint)(Machine.CPU.PC + (short)ext1.Value);
								}
								break;
							case (byte)AddrMode.PCIndex:
								Debug.Assert(ext1.HasValue, "Required extension word is not available");
								if (ext1.HasValue)
								{
									byte disp = (byte)(ext1.Value & 0x00FF);
									byte indexRegNum = (byte)((ext1.Value & 0x7000) >> 12);
									OpSize indexSize = (ext1.Value & 0x0800) == 0 ? OpSize.Word : OpSize.Long;
									uint indexValue = Machine.CPU.ReadDataRegister(indexRegNum);
									if (indexSize == OpSize.Word)
									{
										indexValue = (indexValue & 0x0000FFFF) | ((indexValue & 0x00008000) == 0 ? 0x0 : 0xFFFF0000);
									}
									address = (uint)(Machine.CPU.PC + (int)indexValue + (sbyte)disp);
								}
								break;
							case (byte)AddrMode.Immediate:
								Debug.Assert(ext1.HasValue, "Required extension word is not available");
								if (ext1.HasValue)
								{
									if (size == OpSize.Long)
									{
										Debug.Assert(ext2.HasValue, "Required extension word is not available");
										if (ext2.HasValue)
										{
											immVal = (uint)((ext1.Value << 16) + ext2.Value);
										}
									}
									else
									{
										immVal = ext1.Value;
									}
								}
								break;
						}
						break;
				}
			}

			return (dRegNum, aRegNum, address, immVal);
		}

		// *************************
		//
		// Operation handler methods
		//
		// *************************

		private void ORItoCCR(Instruction inst)
		{
			// SourceExtWord1 holds the immediate operand value.
			if (inst.SourceExtWord1.HasValue)
			{
				ushort value = (ushort)Machine.CPU.SR;
				value |= (ushort)(inst.SourceExtWord1.Value & 0x001F);
				Machine.CPU.SR = (SRFlags)value;
			}
		}

		private void ORItoSR(Instruction inst)
		{
			// SourceExtWord1 holds the immediate operand value.
			if (inst.SourceExtWord1.HasValue)
			{
				ushort value = (ushort)Machine.CPU.SR;
				value |= inst.SourceExtWord1.Value;
				Machine.CPU.SR = (SRFlags)value;
			}
		}

		private void ORI(Instruction inst)
		{
			OpSize opSize = inst.Size ?? OpSize.Word;
			uint? value = GetSizedOperandValue(opSize, inst.SourceExtWord1, inst.SourceExtWord2);
			if (value.HasValue)
			{
				var destValue = ReadEAValue(inst, EAType.Destination);
				if (destValue.HasValue)
				{
					destValue |= value.Value;
					WriteEAValue(inst, destValue.Value, EAType.Destination);
					SetFlags(inst.Info.HandlerID, opSize, destValue.Value);
				}
			}
		}

		private void ANDItoCCR(Instruction inst)
		{
			// SourceExtWord1 holds the immediate operand value.
			if (inst.SourceExtWord1.HasValue)
			{
				ushort value = (ushort)Machine.CPU.SR;
				value &= (ushort)((inst.SourceExtWord1.Value & 0x001F) | 0xFFE0);
				Machine.CPU.SR = (SRFlags)value;
			}
		}

		private void ANDItoSR(Instruction inst)
		{
			// SourceExtWord1 holds the immediate operand value.
			if (inst.SourceExtWord1.HasValue)
			{
				ushort value = (ushort)Machine.CPU.SR;
				value &= inst.SourceExtWord1.Value;
				Machine.CPU.SR = (SRFlags)value;
			}
		}

		private void ANDI(Instruction inst)
		{
			OpSize opSize = inst.Size ?? OpSize.Word;
			uint? value = GetSizedOperandValue(opSize, inst.SourceExtWord1, inst.SourceExtWord2);
			if (value.HasValue)
			{
				var destValue = ReadEAValue(inst, EAType.Destination);
				if (destValue.HasValue)
				{
					destValue &= value.Value;
					WriteEAValue(inst, destValue.Value, EAType.Destination);
					SetFlags(inst.Info.HandlerID, opSize, destValue.Value);
				}
			}
		}

		private void SUBI(Instruction inst)
		{
			OpSize opSize = inst.Size ?? OpSize.Word;
			uint? srcValue = GetSizedOperandValue(opSize, inst.SourceExtWord1, inst.SourceExtWord2);
			if (srcValue.HasValue)
			{
				var destValue = ReadEAValue(inst, EAType.Destination);
				if (destValue.HasValue)
				{
					uint result = destValue.Value - srcValue.Value;
					WriteEAValue(inst, result, EAType.Destination);
					SetFlags(inst.Info.HandlerID, opSize, result, srcValue.Value, destValue.Value);
				}
			}
		}

		private void ADDI(Instruction inst)
		{
			OpSize opSize = inst.Size ?? OpSize.Word;
			uint? srcValue = GetSizedOperandValue(opSize, inst.SourceExtWord1, inst.SourceExtWord2);
			if (srcValue.HasValue)
			{
				var destValue = ReadEAValue(inst, EAType.Destination);
				if (destValue.HasValue)
				{
					uint result = destValue.Value + srcValue.Value;
					WriteEAValue(inst, result, EAType.Destination);
					SetFlags(inst.Info.HandlerID, opSize, result, srcValue.Value, destValue.Value);
				}
			}
		}

		private void EORItoCCR(Instruction inst)
		{
			// SourceExtWord1 holds the immediate operand value.
			if (inst.SourceExtWord1.HasValue)
			{
				ushort value = (ushort)Machine.CPU.SR;
				value ^= (ushort)(inst.SourceExtWord1.Value & 0x001F);
				Machine.CPU.SR = (SRFlags)value;
			}
		}

		private void EORItoSR(Instruction inst)
		{
			// SourceExtWord1 holds the immediate operand value.
			if (inst.SourceExtWord1.HasValue)
			{
				ushort value = (ushort)Machine.CPU.SR;
				value ^= inst.SourceExtWord1.Value;
				Machine.CPU.SR = (SRFlags)value;
			}
		}

		private void EORI(Instruction inst)
		{
			OpSize opSize = inst.Size ?? OpSize.Word;
			uint? value = GetSizedOperandValue(opSize, inst.SourceExtWord1, inst.SourceExtWord2);
			if (value.HasValue)
			{
				var destValue = ReadEAValue(inst, EAType.Destination);
				if (destValue.HasValue)
				{
					destValue ^= value.Value;
					WriteEAValue(inst, destValue.Value, EAType.Destination);
					SetFlags(inst.Info.HandlerID, opSize, destValue.Value);
				}
			}
		}

		private void CMPI(Instruction inst)
		{
			OpSize opSize = inst.Size ?? OpSize.Word;
			uint? srcValue = GetSizedOperandValue(opSize, inst.SourceExtWord1, inst.SourceExtWord2);
			if (srcValue.HasValue)
			{
				var destValue = ReadEAValue(inst, EAType.Destination);
				if (destValue.HasValue)
				{
					uint result = destValue.Value - srcValue.Value;
					SetFlags(inst.Info.HandlerID, opSize, result, srcValue.Value, destValue.Value);
				}
			}
		}

		private void MOVE(Instruction inst)
		{
			var value = ReadEAValue(inst, EAType.Source);
			if (value.HasValue)
			{
				WriteEAValue(inst, value.Value, EAType.Destination);
				OpSize size = inst.Size ?? OpSize.Word;
				SetFlags(inst.Info.HandlerID, size, value.Value);
			}
		}

		private void MOVEA(Instruction inst)
		{
			uint? value = ReadEAValue(inst, EAType.Source);
			if (value.HasValue)
			{
				OpSize size = inst.Size ?? OpSize.Word;
				int regNum = (inst.Opcode & 0x0E00) >> 9;
				Machine.CPU.WriteAddressRegister(regNum, value.Value, size);
			}
		}

		private void MOVEfromSR(Instruction inst)
		{
			WriteEAValue(inst, (uint)Machine.CPU.SR, EAType.Destination);
		}

		private void MOVEtoCCR(Instruction inst)
		{
			var value = ReadEAValue(inst, EAType.Source);
			if (value.HasValue)
			{
				ushort srValue = (ushort)Machine.CPU.SR;
				srValue = (ushort)((srValue & 0xFFE0) | ((ushort)value.Value & 0x001F));
				Machine.CPU.SR = (SRFlags)srValue;
			}
		}

		private void MOVEtoSR(Instruction inst)
		{
			var value = ReadEAValue(inst, EAType.Source);
			if (value.HasValue)
			{
				Machine.CPU.SR = (SRFlags)((ushort)value.Value & 0xF71F);
			}
		}

		private void NEGX(Instruction inst)
		{
			var value = ReadEAValue(inst, EAType.Destination);
			if (value.HasValue)
			{
				OpSize size = inst.Size ?? OpSize.Word;
				int val = Helpers.SignExtendValue(value.Value, size);
				int result = 0 - (val + (Machine.CPU.ExtendFlag ? 1 : 0));
				WriteEAValue(inst, (uint)result, EAType.Destination);
				SetFlags(inst.Info.HandlerID, size, (uint)result, value.Value);
			}
		}

		private void CLR(Instruction inst)
		{
			var value = ReadEAValue(inst, EAType.Destination);
			if (value.HasValue)
			{
				WriteEAValue(inst, 0, EAType.Destination);
				Machine.CPU.ZeroFlag = true;
				Machine.CPU.NegativeFlag = Machine.CPU.OverflowFlag = Machine.CPU.CarryFlag = false;
			}
		}

		private void NEG(Instruction inst)
		{
			var value = ReadEAValue(inst, EAType.Destination);
			if (value.HasValue)
			{
				OpSize size = inst.Size ?? OpSize.Word;
				int val = Helpers.SignExtendValue(value.Value, size);
				int result = 0 - val;
				WriteEAValue(inst, (uint)result, EAType.Destination);
				SetFlags(inst.Info.HandlerID, size, (uint)result, value.Value);
			}
		}

		private void NOT(Instruction inst)
		{
			var value = ReadEAValue(inst, EAType.Destination);
			if (value.HasValue)
			{
				OpSize size = inst.Size ?? OpSize.Word;
				int val = Helpers.SignExtendValue(value.Value, size);
				int result = ~val;
				WriteEAValue(inst, (uint)result, EAType.Destination);
				SetFlags(inst.Info.HandlerID, size, (uint)result, value.Value);
			}
		}

		private void EXT(Instruction inst)
		{
			byte regNum = (byte)(inst.Opcode & 0x0007);
			OpSize size = (inst.Opcode & 0x0040) == 0 ? OpSize.Word : OpSize.Long;
			uint value = Machine.CPU.ReadDataRegister(regNum);
			int extValue = Helpers.SignExtendValue(value, size == OpSize.Word ? OpSize.Byte : OpSize.Word);
			Machine.CPU.WriteDataRegister(regNum, (uint)extValue, size);
			SetFlags(inst.Info.HandlerID, size, (uint)extValue);
		}

		private void SWAP(Instruction inst)
		{
			byte regNum = (byte)(inst.Opcode & 0x0007);
			uint value = Machine.CPU.ReadDataRegister(regNum);
			uint lowerWord = value & 0x0000FFFF;
			value = (value >> 16) | (lowerWord << 16);
			Machine.CPU.WriteDataRegister(regNum, value, OpSize.Long);
			SetFlags(inst.Info.HandlerID, OpSize.Long, value);
		}

		private void PEA(Instruction inst)
		{
			var (_, _, address, _) = EvaluateEffectiveAddress(inst, EAType.Source);
			if (address.HasValue)
			{
				Machine.PushLong(address.Value);
			}
		}

		private void ILLEGAL(Instruction inst)
		{
			Helpers.RaiseTRAPException(TrapVector.IllegalInstruction);
		}

		private void TST(Instruction inst)
		{
			var value = ReadEAValue(inst, EAType.Destination);
			if (value.HasValue)
			{
				OpSize size = inst.Size ?? OpSize.Word;
				SetFlags(inst.Info.HandlerID, size, value.Value);
			}
		}

		private void TRAP(Instruction inst)
		{
			ushort vector = (ushort)(inst.Opcode & 0x000F);
			Helpers.RaiseTRAPException((ushort)(vector + 32));
		}

		private void MOVEUSP(Instruction inst)
		{
			if (!Machine.CPU.SupervisorMode)
			{
				Helpers.RaiseTRAPException(TrapVector.PrivilegeViolation);
				return;
			}
			byte regNum = (byte)(inst.Opcode & 0x0007);
			if ((inst.Opcode & 0x0008) == 0)
			{
				Machine.CPU.USP = Machine.CPU.ReadAddressRegister(regNum);
			}
			else
			{
				Machine.CPU.WriteAddressRegister(regNum, Machine.CPU.USP);
			}
		}

		private void RESET(Instruction inst)
		{
			if (Machine.CPU.SupervisorMode)
			{
				Machine.CPU.ResetExternalDevices();
			}
			else
			{
				Helpers.RaiseTRAPException(TrapVector.PrivilegeViolation);
			}
		}

		private void NOP(Instruction _)
		{
			// No operation to be performed for NOP (obviously!)
		}

		private void RTE(Instruction inst)
		{
			if (Machine.CPU.SupervisorMode)
			{
				ushort sr = Machine.PopWord();
				Machine.CPU.PC = Machine.PopLong();
				Machine.CPU.SR = (SRFlags)sr;
			}
			else
			{
				Helpers.RaiseTRAPException(TrapVector.PrivilegeViolation);
			}
		}

		private void RTS(Instruction inst)
		{
			//			Machine.CPU.PC = Machine.PopLong();

			// if no JSR/BSR instruction has been executed then this RTS marks the termination of the code execution.
			if (_numberOfJSRCalls == 0)
			{
				Machine.IsEndOfExecution = true;
				return;
			}
			Machine.CPU.PC = Machine.PopLong();

			_numberOfJSRCalls--;
		}

		private void TRAPV(Instruction _)
		{
			if (Machine.CPU.OverflowFlag)
			{
				Helpers.RaiseTRAPException(TrapVector.TRAPVInstruction);
			}
		}

		private void RTR(Instruction _)
		{
			ushort ccr = Machine.PopWord();
			ushort srValue = (ushort)Machine.CPU.SR;
			srValue = (ushort)((srValue & 0xFFE0) | (ccr & 0x001F));
			Machine.CPU.SR = (SRFlags)srValue;
			Machine.CPU.PC = Machine.PopLong();
		}

		private void JSR(Instruction inst)
		{
			var (_, _, address, _) = EvaluateEffectiveAddress(inst, EAType.Source);
			if (address.HasValue)
			{
				Machine.PushLong(Machine.CPU.PC);
				Machine.CPU.PC = address.Value;
				_numberOfJSRCalls++;
			}
		}

		private void JMP(Instruction inst)
		{
			var (_, _, address, _) = EvaluateEffectiveAddress(inst, EAType.Source);
			if (address.HasValue)
			{
				Machine.CPU.PC = address.Value;
			}
		}

		private void LEA(Instruction inst)
		{
			var (_, _, address, _) = EvaluateEffectiveAddress(inst, EAType.Source);
			if (address.HasValue)
			{
				int regNum = (inst.Opcode & 0x0E00) >> 9;
				Machine.CPU.WriteAddressRegister(regNum, address.Value, OpSize.Long);
			}
		}

		private void CHK(Instruction inst)
		{
			var value = ReadEAValue(inst, EAType.Source);
			if (value.HasValue)
			{
				OpSize size = inst.Size ?? OpSize.Word;
				int regNum = (inst.Opcode & 0x0E00) >> 9;
				uint dRegValue = Machine.CPU.ReadDataRegister(regNum);
				int signedDVal = Helpers.SignExtendValue(dRegValue, size);
				int signedEAVal = Helpers.SignExtendValue(value.Value, size);
				if (signedDVal < 0)
				{
					Machine.CPU.NegativeFlag = true;
					Helpers.RaiseTRAPException(TrapVector.CHKInstruction);
				}
				else if (signedDVal > signedEAVal)
				{
					Machine.CPU.NegativeFlag = false;
					Helpers.RaiseTRAPException(TrapVector.CHKInstruction);
				}
			}
		}

		private void ADDQ(Instruction inst)
		{
			int addVal = (inst.Opcode & 0x0E00) >> 9;
			if (addVal == 0)
			{
				addVal = 8;
			}

			// When being applied to an address register, we work with the entire 32-bit value regardless
			// of the size that has been specified. This operation also doesn't affect the flags.
			if ((inst.Opcode & 0x0038) == (int)AddrMode.AddressRegister)
			{
				int regNum = inst.Opcode & 0x0007;
				uint aRegVal = Machine.CPU.ReadAddressRegister(regNum);
				uint result = (uint)(aRegVal + addVal);
				Machine.CPU.WriteAddressRegister(regNum, result);
				return;
			}

			var value = ReadEAValue(inst, EAType.Destination);
			if (value.HasValue)
			{
				OpSize size = inst.Size ?? OpSize.Word;
				uint result = (uint)(value.Value + addVal);
				WriteEAValue(inst, result, EAType.Destination);
				SetFlags(inst.Info.HandlerID, size, result, value.Value);
			}
		}

		private void SUBQ(Instruction inst)
		{
			int subVal = (inst.Opcode & 0x0E00) >> 9;
			if (subVal == 0)
			{
				subVal = 8;
			}

			// When being applied to an address register, we work with the entire 32-bit value regardless
			// of the size that has been specified. This operation also doesn't affect the flags.
			if ((inst.Opcode & 0x0038) == (int)AddrMode.AddressRegister)
			{
				int regNum = inst.Opcode & 0x0007;
				uint aRegVal = Machine.CPU.ReadAddressRegister(regNum);
				uint result = (uint)(aRegVal - subVal);
				Machine.CPU.WriteAddressRegister(regNum, result);
				return;
			}

			var value = ReadEAValue(inst, EAType.Destination);
			if (value.HasValue)
			{
				OpSize size = inst.Size ?? OpSize.Word;
				uint result = (uint)(value.Value - subVal);
				WriteEAValue(inst, result, EAType.Destination);
				SetFlags(inst.Info.HandlerID, size, result, value.Value);
			}
		}

		private void Scc(Instruction inst)
		{
			var value = ReadEAValue(inst, EAType.Destination);
			if (value.HasValue)
			{
				int cond = (inst.Opcode & 0x0F00) >> 8;
				if (Machine.CPU.EvaluateCondition((Condition)cond))
				{
					WriteEAValue(inst, 0x000000FF, EAType.Destination);
				}
				else
				{
					WriteEAValue(inst, 0x00000000, EAType.Destination);
				}
			}
		}

		private void DBcc(Instruction inst)
		{
			int cond = (inst.Opcode & 0x0F00) >> 8;
			if (!Machine.CPU.EvaluateCondition((Condition)cond))
			{
				int dRegNum = inst.Opcode & 0x0007;
				uint dRegVal = Machine.CPU.ReadDataRegister(dRegNum);
				int newDRegVal = Helpers.SignExtendValue(dRegVal, OpSize.Word) - 1;
				Machine.CPU.WriteDataRegister(dRegNum, (uint)newDRegVal, OpSize.Word);
				if (newDRegVal != -1)
				{
					// Note: extra -2 to account for PC pointing at the next instruction, not on the extension word for the
					// current instruction (as the displacement for DBcc instructions assumes)
					int disp = Helpers.SignExtendValue(inst.SourceExtWord1.Value, OpSize.Word) - 2;
					Machine.CPU.PC = (uint)(Machine.CPU.PC + disp);
				}
			}
		}

		private void BRA(Instruction inst)
		{
			uint pc = Machine.CPU.PC;
			int disp = inst.Opcode & 0x00FF;
			if (disp == 0)
			{
				// Byte displacement is zero so use the extension word value as a 16-bit displacement.
				disp = Helpers.SignExtendValue(inst.SourceExtWord1.Value, OpSize.Word);

				// Step PC back a word as it should be pointing immediately after the instruction opcode word
				// for the displacement to be correct (whereas it will currently be pointing at the location immediately
				// after the extension word)
				pc -= 2;
			}
			else
			{
				disp = Helpers.SignExtendValue((uint)disp, OpSize.Byte);
			}

			Machine.CPU.PC = (uint)(pc + disp);
		}

		private void BSR(Instruction inst)
		{
			uint pc = Machine.CPU.PC;
			int disp = inst.Opcode & 0x00FF;
			if (disp == 0)
			{
				// Byte displacement is zero so use the extension word value as a 16-bit displacement.
				disp = Helpers.SignExtendValue(inst.SourceExtWord1.Value, OpSize.Word);

				// Step PC back a word as it should be pointing immediately after the instruction opcode word
				// for the displacement to be correct (whereas it will currently be pointing at the location immediately
				// after the extension word)
				pc -= 2;
			}
			else
			{
				disp = Helpers.SignExtendValue((uint)disp, OpSize.Byte);
			}

			Machine.PushLong(Machine.CPU.PC);
			Machine.CPU.PC = (uint)(pc + disp);
			_numberOfJSRCalls++;
		}

		private void Bcc(Instruction inst)
		{
			int cond = (inst.Opcode & 0x0F00) >> 8;
			if (Machine.CPU.EvaluateCondition((Condition)cond))
			{
				uint pc = Machine.CPU.PC;
				int disp = inst.Opcode & 0x00FF;
				if (disp == 0)
				{
					// Byte displacement is zero so use the extension word value as a 16-bit displacement.
					disp = Helpers.SignExtendValue(inst.SourceExtWord1.Value, OpSize.Word);

					// Step PC back a word as it should be pointing immediately after the instruction opcode word
					// for the displacement to be correct (whereas it will currently be pointing at the location immediately
					// after the extension word)
					pc -= 2;
				}
				else
				{
					disp = Helpers.SignExtendValue((uint)disp, OpSize.Byte);
				}

				Machine.CPU.PC = (uint)(pc + disp);
			}
		}

		private void MOVEQ(Instruction inst)
		{
			int dRegNum = (inst.Opcode & 0x0E00) >> 9;
			int data = Helpers.SignExtendValue((uint)(inst.Opcode & 0x00FF), OpSize.Byte);
			Machine.CPU.WriteDataRegister(dRegNum, (uint)data);
			SetFlags(inst.Info.HandlerID, OpSize.Long, (uint)data);
		}

		private void DIVU(Instruction inst)
		{
			int dRegNum = (inst.Opcode & 0x0E00) >> 9;
			uint dRegVal = Machine.CPU.ReadDataRegister(dRegNum);
			var value = ReadEAValue(inst, EAType.Source);
			if (value.HasValue)
			{
				if (value.Value == 0)
				{
					Helpers.RaiseTRAPException(TrapVector.DivideByZero);
					return;
				}

				var res = dRegVal / value.Value;
				if (res > 0xFFFF)
				{
					Machine.CPU.OverflowFlag = true;
					return;
				}
				var remainder = (dRegVal % value.Value) & 0xFFFF;
				var result = (res & 0xFFFF) | (remainder << 16);
				Machine.CPU.WriteDataRegister(dRegNum, result);
				SetFlags(inst.Info.HandlerID, OpSize.Word, res);
			}
		}

		private void DIVS(Instruction inst)
		{
			int dRegNum = (inst.Opcode & 0x0E00) >> 9;
			uint dRegVal = Machine.CPU.ReadDataRegister(dRegNum);
			var value = ReadEAValue(inst, EAType.Source);
			if (value.HasValue)
			{
				if (value.Value == 0)
				{
					Helpers.RaiseTRAPException(TrapVector.DivideByZero);
					return;
				}
				var signedVal = Helpers.SignExtendValue(value.Value, OpSize.Word);

				var res = (int)dRegVal / signedVal;
				if (res < -32768 || res > 32767)
				{
					Machine.CPU.OverflowFlag = true;
					return;
				}
				var remainder = (int)(dRegVal % signedVal) & 0xFFF;
				var result = (res & 0xFFFF) | (remainder << 16);
				Machine.CPU.WriteDataRegister(dRegNum, (uint)result);
				SetFlags(inst.Info.HandlerID, OpSize.Word, (uint)res);
			}
		}

		private void OR(Instruction inst)
		{
			int dRegNum = (inst.Opcode & 0x0E00) >> 9;
			uint dRegVal = Machine.CPU.ReadDataRegister(dRegNum);
			var value = ReadEAValue(inst, EAType.Destination);
			if (value.HasValue)
			{
				var result = dRegVal | value.Value;
				OpSize size = inst.Size ?? OpSize.Word;
				bool dnDest = (inst.Opcode & 0x0100) == 0;
				if (dnDest)
				{
					Machine.CPU.WriteDataRegister(dRegNum, result, size);
				}
				else
				{
					WriteEAValue(inst, result, EAType.Destination);
				}
				SetFlags(inst.Info.HandlerID, size, result);
			}
		}

		private void SUB(Instruction inst)
		{
			OpSize size = inst.Size ?? OpSize.Word;
			int dRegNum = (inst.Opcode & 0x0E00) >> 9;
			int dRegVal = Helpers.SignExtendValue(Machine.CPU.ReadDataRegister(dRegNum), size);
			var value = ReadEAValue(inst, EAType.Destination);
			if (value.HasValue)
			{
				bool dnDest = (inst.Opcode & 0x0100) == 0;
				int signedVal = Helpers.SignExtendValue(value.Value, size);
				var result = dnDest ? (dRegVal - signedVal) : (signedVal - dRegVal);
				if (dnDest)
				{
					Machine.CPU.WriteDataRegister(dRegNum, (uint)result, size);
					SetFlags(inst.Info.HandlerID, size, (uint)result, value.Value, (uint)dRegVal);
				}
				else
				{
					WriteEAValue(inst, (uint)result, EAType.Destination);
					SetFlags(inst.Info.HandlerID, size, (uint)result, (uint)dRegVal, value.Value);
				}
			}
		}

		private void SUBX(Instruction inst)
		{
			OpSize size = inst.Size ?? OpSize.Word;
			byte rX = (byte)(inst.Opcode & 0x0007);
			byte rY = (byte)((inst.Opcode & 0x0E00) >> 9);
			bool usingDataReg = (inst.Opcode & 0x0008) == 0;
			if (usingDataReg)
			{
				int rXVal = Helpers.SignExtendValue(Machine.CPU.ReadDataRegister(rX), size);
				int rYVal = Helpers.SignExtendValue(Machine.CPU.ReadDataRegister(rY), size);
				var result = rYVal - rXVal - (Machine.CPU.ExtendFlag ? 1 : 0);
				Machine.CPU.WriteDataRegister(rY, (uint)result, size);
				SetFlags(inst.Info.HandlerID, size, (uint)result, (uint)rXVal, (uint)rYVal);
			}
			else
			{
				// Predecrement the source and destination address registers by the number of bytes specified
				// by the data size.
				uint rXAddr = Machine.CPU.DecrementAddressRegister(rX, size);
				uint rYAddr = Machine.CPU.DecrementAddressRegister(rY, size);
				int rXVal;
				int rYVal;
				switch (size)
				{
					case OpSize.Byte:
						rXVal = Helpers.SignExtendValue(Machine.Memory.ReadByte(rXAddr), size);
						rYVal = Helpers.SignExtendValue(Machine.Memory.ReadByte(rYAddr), size);
						break;
					case OpSize.Long:
						rXVal = Helpers.SignExtendValue(Machine.Memory.ReadLong(rXAddr), size);
						rYVal = Helpers.SignExtendValue(Machine.Memory.ReadLong(rYAddr), size);
						break;
					default:
						rXVal = Helpers.SignExtendValue(Machine.Memory.ReadWord(rXAddr), size);
						rYVal = Helpers.SignExtendValue(Machine.Memory.ReadWord(rYAddr), size);
						break;
				}
				var result = rYVal - rXVal - (Machine.CPU.ExtendFlag ? 1 : 0);
				switch (size)
				{
					case OpSize.Byte:
						Machine.Memory.WriteByte(rYAddr, (byte)(result & 0x000000FF));
						break;
					case OpSize.Long:
						Machine.Memory.WriteLong(rYAddr, (uint)result);
						break;
					default:
						Machine.Memory.WriteWord(rYAddr, (ushort)(result & 0x0000FFFF));
						break;
				}

				SetFlags(inst.Info.HandlerID, size, (uint)result, (uint)rXVal, (uint)rYVal);
			}
		}

		private void SUBA(Instruction inst)
		{
			var value = ReadEAValue(inst, EAType.Source);
			if (value.HasValue)
			{
				OpSize size = inst.Size ?? OpSize.Word;
				int signedVal = Helpers.SignExtendValue(value.Value, size);
				int regNum = (inst.Opcode & 0x0E00) >> 9;
				uint val = Machine.CPU.ReadAddressRegister(regNum);
				val = (uint)(val - signedVal);
				Machine.CPU.WriteAddressRegister(regNum, val, OpSize.Long);
			}
		}

		private void EOR(Instruction inst)
		{
			int dRegNum = (inst.Opcode & 0x0E00) >> 9;
			uint dRegVal = Machine.CPU.ReadDataRegister(dRegNum);
			var value = ReadEAValue(inst, EAType.Destination);
			if (value.HasValue)
			{
				var result = value.Value ^ dRegVal;
				OpSize size = inst.Size ?? OpSize.Word;
				WriteEAValue(inst, result, EAType.Destination);
				SetFlags(inst.Info.HandlerID, size, result);
			}
		}

		private void CMPM(Instruction inst)
		{
			OpSize size = inst.Size ?? OpSize.Word;
			byte rX = (byte)(inst.Opcode & 0x0007);
			byte rY = (byte)((inst.Opcode & 0x0E00) >> 9);

			uint rXAddr = Machine.CPU.ReadAddressRegister(rX);
			uint rYAddr = Machine.CPU.ReadAddressRegister(rY);
			int rXVal;
			int rYVal;
			switch (size)
			{
				case OpSize.Byte:
					rXVal = Helpers.SignExtendValue(Machine.Memory.ReadByte(rXAddr), size);
					rYVal = Helpers.SignExtendValue(Machine.Memory.ReadByte(rYAddr), size);
					break;
				case OpSize.Long:
					rXVal = Helpers.SignExtendValue(Machine.Memory.ReadLong(rXAddr), size);
					rYVal = Helpers.SignExtendValue(Machine.Memory.ReadLong(rYAddr), size);
					break;
				default:
					rXVal = Helpers.SignExtendValue(Machine.Memory.ReadWord(rXAddr), size);
					rYVal = Helpers.SignExtendValue(Machine.Memory.ReadWord(rYAddr), size);
					break;
			}

			// Now we've read the values from memory, postincrement both address registers.
			Machine.CPU.IncrementAddressRegister(rX, size);
			Machine.CPU.IncrementAddressRegister(rY, size);

			// Subtract to perform the comparison.
			int result = rYVal - rXVal;
			SetFlags(inst.Info.HandlerID, size, (uint)result, (uint)rXVal, (uint)rYVal);
		}

		private void CMP(Instruction inst)
		{
			OpSize size = inst.Size ?? OpSize.Word;
			byte dRegNum = (byte)((inst.Opcode & 0x0E00) >> 9);
			int dRegVal = Helpers.SignExtendValue(Machine.CPU.ReadDataRegister(dRegNum), size);

			var value = ReadEAValue(inst, EAType.Destination);
			if (value.HasValue)
			{
				int signedVal = Helpers.SignExtendValue(value.Value, size);
				var result = dRegVal - signedVal;

				SetFlags(inst.Info.HandlerID, size, (uint)result, value.Value, (uint)dRegVal);
			}
		}

		private void CMPA(Instruction inst)
		{
			var value = ReadEAValue(inst, EAType.Source);
			if (value.HasValue)
			{
				OpSize size = inst.Size ?? OpSize.Word;
				int signedVal = Helpers.SignExtendValue(value.Value, size);
				int regNum = (inst.Opcode & 0x0E00) >> 9;
				uint val = Machine.CPU.ReadAddressRegister(regNum);
				var result = val - signedVal;
				SetFlags(inst.Info.HandlerID, size, (uint)result, value.Value, val);
			}
		}

		private void MULU(Instruction inst)
		{
			int dRegNum = (inst.Opcode & 0x0E00) >> 9;
			uint dRegVal = (Machine.CPU.ReadDataRegister(dRegNum) & 0x0000FFFF);
			var value = ReadEAValue(inst, EAType.Source);
			if (value.HasValue)
			{
				var res = value.Value * dRegVal;
				Machine.CPU.WriteDataRegister(dRegNum, res, OpSize.Long);
				SetFlags(inst.Info.HandlerID, OpSize.Long, res);
			}
		}

		private void MULS(Instruction inst)
		{
			int dRegNum = (inst.Opcode & 0x0E00) >> 9;
			int dRegVal = Helpers.SignExtendValue(Machine.CPU.ReadDataRegister(dRegNum), OpSize.Word);
			var value = ReadEAValue(inst, EAType.Source);
			if (value.HasValue)
			{
				int signedValue = Helpers.SignExtendValue(value.Value, OpSize.Word);
				var res = signedValue * dRegVal;
				Machine.CPU.WriteDataRegister(dRegNum, (uint)res, OpSize.Long);
				SetFlags(inst.Info.HandlerID, OpSize.Long, (uint)res);
			}
		}

		private void EXG(Instruction inst)
		{
			byte rX = (byte)(inst.Opcode & 0x0007);
			byte rY = (byte)((inst.Opcode & 0x0E00) >> 9);
			byte mode = (byte)((inst.Opcode & 0x00F8) >> 3);
			uint x;
			uint y;
			switch (mode)
			{
				case 0x08:      // Data Register <-> Data Register
					x = Machine.CPU.ReadDataRegister(rX);
					y = Machine.CPU.ReadDataRegister(rY);
					Machine.CPU.WriteDataRegister(rX, y);
					Machine.CPU.WriteDataRegister(rY, x);
					break;
				case 0x09:      // Address Register <-> Address Register
					x = Machine.CPU.ReadAddressRegister(rX);
					y = Machine.CPU.ReadAddressRegister(rY);
					Machine.CPU.WriteAddressRegister(rX, y);
					Machine.CPU.WriteAddressRegister(rY, x);
					break;
				case 0x11:      // Data Register <-> Address Register
					x = Machine.CPU.ReadAddressRegister(rX);
					y = Machine.CPU.ReadDataRegister(rY);
					Machine.CPU.WriteAddressRegister(rX, y);
					Machine.CPU.WriteDataRegister(rY, x);
					break;
				default:
					Debug.Assert(false, "Invalid operating mode for EXG instruction.");
					break;
			}
		}

		private void AND(Instruction inst)
		{
			int dRegNum = (inst.Opcode & 0x0E00) >> 9;
			uint dRegVal = Machine.CPU.ReadDataRegister(dRegNum);
			var value = ReadEAValue(inst, EAType.Destination);
			if (value.HasValue)
			{
				var result = dRegVal & value.Value;
				OpSize size = inst.Size ?? OpSize.Word;
				bool dnDest = (inst.Opcode & 0x0100) == 0;
				if (dnDest)
				{
					Machine.CPU.WriteDataRegister(dRegNum, result, size);
				}
				else
				{
					WriteEAValue(inst, result, EAType.Destination);
				}
				SetFlags(inst.Info.HandlerID, size, result);
			}
		}

		private void ADD(Instruction inst)
		{
			OpSize size = inst.Size ?? OpSize.Word;
			int dRegNum = (inst.Opcode & 0x0E00) >> 9;
			int dRegVal = Helpers.SignExtendValue(Machine.CPU.ReadDataRegister(dRegNum), size);
			var value = ReadEAValue(inst, EAType.Destination);
			if (value.HasValue)
			{
				bool dnDest = (inst.Opcode & 0x0100) == 0;
				int signedVal = Helpers.SignExtendValue(value.Value, size);
				var result = dRegVal + signedVal;
				if (dnDest)
				{
					Machine.CPU.WriteDataRegister(dRegNum, (uint)result, size);
					SetFlags(inst.Info.HandlerID, size, (uint)result, value.Value, (uint)dRegVal);
				}
				else
				{
					WriteEAValue(inst, (uint)result, EAType.Destination);
					SetFlags(inst.Info.HandlerID, size, (uint)result, (uint)dRegVal, value.Value);
				}
			}
		}

		private void ADDX(Instruction inst)
		{
			OpSize size = inst.Size ?? OpSize.Word;
			byte rX = (byte)(inst.Opcode & 0x0007);
			byte rY = (byte)((inst.Opcode & 0x0E00) >> 9);
			bool usingDataReg = (inst.Opcode & 0x0008) == 0;
			if (usingDataReg)
			{
				int rXVal = Helpers.SignExtendValue(Machine.CPU.ReadDataRegister(rX), size);
				int rYVal = Helpers.SignExtendValue(Machine.CPU.ReadDataRegister(rY), size);
				var result = rYVal + rXVal + (Machine.CPU.ExtendFlag ? 1 : 0);
				Machine.CPU.WriteDataRegister(rY, (uint)result, size);
				SetFlags(inst.Info.HandlerID, size, (uint)result, (uint)rXVal, (uint)rYVal);
			}
			else
			{
				// Predecrement the source and destination address registers by the number of bytes specified
				// by the data size.
				uint rXAddr = Machine.CPU.DecrementAddressRegister(rX, size);
				uint rYAddr = Machine.CPU.DecrementAddressRegister(rY, size);
				int rXVal;
				int rYVal;
				switch (size)
				{
					case OpSize.Byte:
						rXVal = Helpers.SignExtendValue(Machine.Memory.ReadByte(rXAddr), size);
						rYVal = Helpers.SignExtendValue(Machine.Memory.ReadByte(rYAddr), size);
						break;
					case OpSize.Long:
						rXVal = Helpers.SignExtendValue(Machine.Memory.ReadLong(rXAddr), size);
						rYVal = Helpers.SignExtendValue(Machine.Memory.ReadLong(rYAddr), size);
						break;
					default:
						rXVal = Helpers.SignExtendValue(Machine.Memory.ReadWord(rXAddr), size);
						rYVal = Helpers.SignExtendValue(Machine.Memory.ReadWord(rYAddr), size);
						break;
				}
				var result = rYVal + rXVal + (Machine.CPU.ExtendFlag ? 1 : 0);
				switch (size)
				{
					case OpSize.Byte:
						Machine.Memory.WriteByte(rYAddr, (byte)(result & 0x000000FF));
						break;
					case OpSize.Long:
						Machine.Memory.WriteLong(rYAddr, (uint)result);
						break;
					default:
						Machine.Memory.WriteWord(rYAddr, (ushort)(result & 0x0000FFFF));
						break;
				}

				SetFlags(inst.Info.HandlerID, size, (uint)result, (uint)rXVal, (uint)rYVal);
			}
		}

		private void ADDA(Instruction inst)
		{
			var value = ReadEAValue(inst, EAType.Source);
			if (value.HasValue)
			{
				OpSize size = inst.Size ?? OpSize.Word;
				int signedVal = Helpers.SignExtendValue(value.Value, size);
				int regNum = (inst.Opcode & 0x0E00) >> 9;
				uint val = Machine.CPU.ReadAddressRegister(regNum);
				val = (uint)(val + signedVal);
				Machine.CPU.WriteAddressRegister(regNum, val, OpSize.Long);
			}
		}

		private void ASL_ASR_LSL_LSR(Instruction inst)
		{
			bool directionLeft = (inst.Opcode & 0x0100) != 0;       // Determine direction of shift (i.e. ASL or ASR).
			bool logicalShift;
			byte sizeBits = (byte)((inst.Opcode & 0x00C0) >> 6);
			if (sizeBits == 0x03)
			{
				// Shift on memory (using Effective Address)
				logicalShift = (inst.Opcode & 0x0E00) != 0;        // Determine if logical shift (i.e. LSL or LSR).
				var value = ReadEAValue(inst, EAType.Source);
				if (value.HasValue)
				{
					if (directionLeft)
					{
						var result = value.Value << 1;
						WriteEAValue(inst, result, EAType.Source);
						SetFlags(inst.Info.HandlerID, OpSize.Word, result, 1, value.Value & 0x8000);
						Machine.CPU.OverflowFlag = logicalShift ? false : (result & 0x8000) != (value.Value & 0x8000);
					}
					else
					{
						var msb = value.Value & Helpers.SizeMSB(OpSize.Word);
						var result = (value.Value >> 1) | (logicalShift ? 0 : msb);
						WriteEAValue(inst, result, EAType.Source);
						SetFlags(inst.Info.HandlerID, OpSize.Word, result, 1, value.Value & 0x0001);
					}
				}
			}
			else
			{
				logicalShift = (inst.Opcode & 0x0018) != 0;        // Determine if logical shift (i.e. LSL or LSR).
				OpSize size = inst.Size ?? OpSize.Word;
				uint sizeMask = Helpers.SizeMask(size);
				uint signMask = Helpers.SizeMSB(size);
				byte dRegNum = (byte)(inst.Opcode & 0x0007);
				uint dRegVal = Machine.CPU.ReadDataRegister(dRegNum) & sizeMask;

				// Determine if a data register holds the shift amount.
				bool dRegShift = (inst.Opcode & 0x0020) != 0;
				int shift = (inst.Opcode & 0x0E00) >> 9;
				int shiftAmt;
				if (dRegShift)
				{
					// The shift value holds the number of the data register that holds the number of bits to shift by.
					shiftAmt = (int)(Machine.CPU.ReadDataRegister(shift) & 0x003F);
				}
				else
				{
					shiftAmt = shift != 0 ? shift : 8;
				}
				uint bitShiftedOut = 0;
				bool msbChanged = false;
				if (directionLeft)
				{
					for (int s = 0; s < shiftAmt; s++)
					{
						bitShiftedOut = dRegVal & signMask;
						dRegVal <<= 1;
						if ((dRegVal & signMask) != bitShiftedOut)
						{
							msbChanged = true;
						}
					}
				}
				else
				{
					var msb = dRegVal & signMask;
					for (int s = 0; s < shiftAmt; s++)
					{
						bitShiftedOut = dRegVal & 0x00000001;
						dRegVal >>= 1;
						if (!logicalShift)
						{
							dRegVal |= msb;
						}
					}
				}

				dRegVal &= sizeMask;
				Machine.CPU.WriteDataRegister(dRegNum, dRegVal, size);
				SetFlags(inst.Info.HandlerID, size, dRegVal, (uint)shiftAmt, bitShiftedOut);
				Machine.CPU.OverflowFlag = logicalShift ? false : msbChanged;
			}
		}

		private void ROL_ROR_ROXL_ROXR(Instruction inst)
		{
			bool directionLeft = (inst.Opcode & 0x0100) != 0;       // Determine direction of rotation (i.e. ROL or ROR).
			bool withExtend;
			byte sizeBits = (byte)((inst.Opcode & 0x00C0) >> 6);
			if (sizeBits == 0x03)
			{
				// Rotate on memory (using Effective Address)
				withExtend = (inst.Opcode & 0x0E00) == 0x0400;        // Determine if rotate with Extend (i.e. ROXL or ROXR).
				var value = ReadEAValue(inst, EAType.Source);
				if (value.HasValue)
				{
					if (directionLeft)
					{
						var bitShiftedOut = value.Value & 0x00008000;
						var result = value.Value << 1;
						bool introduceBit = withExtend ? Machine.CPU.ExtendFlag : bitShiftedOut != 0;
						if (introduceBit)
						{
							result |= 0x00000001;
						}
						WriteEAValue(inst, result, EAType.Source);
						SetFlags(inst.Info.HandlerID, OpSize.Word, result, 1, bitShiftedOut);
						Machine.CPU.OverflowFlag = false;
					}
					else
					{
						var bitShiftedOut = value.Value & 0x00000001;
						var result = (value.Value >> 1);
						bool introduceBit = withExtend ? Machine.CPU.ExtendFlag : bitShiftedOut != 0;
						if (introduceBit)
						{
							result |= 0x00008000;
						}
						WriteEAValue(inst, result, EAType.Source);
						SetFlags(inst.Info.HandlerID, OpSize.Word, result, 1, bitShiftedOut);
					}
				}
			}
			else
			{
				withExtend = (inst.Opcode & 0x0018) == 0x0010;        // Determine if rotate with Extend (i.e. ROXL or ROXR).
				OpSize size = inst.Size ?? OpSize.Word;
				uint sizeMask = Helpers.SizeMask(size);
				uint msb = Helpers.SizeMSB(size);
				byte dRegNum = (byte)(inst.Opcode & 0x0007);
				uint dRegVal = Machine.CPU.ReadDataRegister(dRegNum) & sizeMask;

				// Determine if a data register holds the rotation amount.
				bool dRegRotate = (inst.Opcode & 0x0020) != 0;
				int rotate = (inst.Opcode & 0x0E00) >> 9;
				int rotateAmt;
				if (dRegRotate)
				{
					// The rotate value holds the number of the data register that holds the number of bits to rotate by.
					rotateAmt = (int)(Machine.CPU.ReadDataRegister(rotate) & 0x003F);
				}
				else
				{
					rotateAmt = rotate != 0 ? rotate : 8;
				}
				uint bitShiftedOut = 0;
				if (directionLeft)
				{
					bool xFlag = Machine.CPU.ExtendFlag;
					for (int r = 0; r < rotateAmt; r++)
					{
						bitShiftedOut = dRegVal & msb;
						dRegVal <<= 1;
						bool introduceBit = withExtend ? xFlag : bitShiftedOut != 0;
						if (introduceBit)
						{
							dRegVal |= 0x00000001;
						}
						xFlag = bitShiftedOut != 0;
					}
				}
				else
				{
					bool xFlag = Machine.CPU.ExtendFlag;
					for (int r = 0; r < rotateAmt; r++)
					{
						bitShiftedOut = dRegVal & 0x00000001;
						dRegVal >>= 1;
						bool introduceBit = withExtend ? xFlag : bitShiftedOut != 0;
						if (introduceBit)
						{
							dRegVal |= msb;
						}
						xFlag = bitShiftedOut != 0;
					}
				}

				dRegVal &= sizeMask;
				Machine.CPU.WriteDataRegister(dRegNum, dRegVal, size);
				SetFlags(inst.Info.HandlerID, size, dRegVal, (uint)rotateAmt, bitShiftedOut);
			}
		}

		private void BTST_BCHG_BCLR_BSET(Instruction inst)
		{
			uint bitNum;
			if ((inst.Opcode & 0x0100) != 0)       // Determine if dynamic (i.e. bit number specified in a register)
			{
				int regNum = (inst.Opcode & 0x0E00) >> 9;
				bitNum = Machine.CPU.ReadDataRegister(regNum);
			}
			else
			{
				bitNum = (uint)inst.SourceExtWord1;
			}

			// Determine if the destination is a memory address. If it is then we work with a single byte.
			var (_, _, address, _) = EvaluateEffectiveAddress(inst, EAType.Destination);
			if (address.HasValue)
			{
				bitNum &= 0x00000007;
				inst.Size = OpSize.Byte;
			}
			else
			{
				bitNum &= 0x0000001F;
				inst.Size = OpSize.Long;
			}

			var value = ReadEAValue(inst, EAType.Destination);
			if (value.HasValue)
			{
				// Determine which operation we're performing (BTST, BCHG, BCLR, or BSET)
				byte operation = (byte)((inst.Opcode & 0x00C0) >> 6);

				// Test the specified bit and set the Zero flag accordingly.
				Machine.CPU.ZeroFlag = (value.Value & _bit[bitNum]) == 0;

				// Modify the specified bit as necessary for the instruction being executed (but do nothing more for
				// BTST as we've already performed the test, which is all that is needed for this instruction)
				uint result = value.Value;
				switch (operation)
				{
					case 0x01:      // BCHG
						result ^= _bit[bitNum];
						break;
					case 0x02:      // BCLR
						result &= ~_bit[bitNum];
						break;
					case 0x03:      // BSET
						result |= _bit[bitNum];
						break;
					default:
						// BTST operation so nothing more to do.
						break;
				}

				// For anything other than a BTST instruction we need to update the destination.
				if (operation != 0x00)
				{
					WriteEAValue(inst, result, EAType.Destination);
				}
			}
		}

		private void LINK(Instruction inst)
		{
			if (inst.SourceExtWord1.HasValue)
			{
				byte regNum = (byte)(inst.Opcode & 0x0007);
				uint regValue = Machine.CPU.ReadAddressRegister(regNum);
				Machine.PushLong(regValue);
				uint sp = Machine.CPU.ReadAddressRegister(7);
				Machine.CPU.WriteAddressRegister(regNum, sp);
				int disp = Helpers.SignExtendValue((uint)inst.SourceExtWord1, OpSize.Word);
				Machine.CPU.WriteAddressRegister(7, (uint)((int)sp + disp));
			}
		}

		private void UNLK(Instruction inst)
		{
			byte regNum = (byte)(inst.Opcode & 0x0007);
			Machine.CPU.WriteAddressRegister(7, Machine.CPU.ReadAddressRegister(regNum));
			Machine.CPU.WriteAddressRegister(regNum, Machine.PopLong());
		}

		private void STOP(Instruction inst)
		{
			var data = inst.SourceExtWord1;
			if (data.HasValue)
			{
				// If not already in Supervisor mode or if the supplied data would take the system out of
				// Supervisor mode then raise an exception.
				if (!Machine.CPU.SupervisorMode || ((data.Value & (ushort)SRFlags.SupervisorMode) == 0))
				{
					Helpers.RaiseTRAPException(TrapVector.PrivilegeViolation);
					return;
				}
				Machine.CPU.SR = (SRFlags)data.Value;
				Machine.StopExecution();
			}
		}

		private void TAS(Instruction inst)
		{
			lock (lockObj)
			{
				var value = ReadEAValue(inst, EAType.Destination);
				if (value.HasValue)
				{
					Machine.CPU.NegativeFlag = (value.Value & 0x00000080) != 0;
					Machine.CPU.ZeroFlag = (value.Value & 0x000000FF) == 0;
					Machine.CPU.CarryFlag = Machine.CPU.OverflowFlag = false;
					var result = value.Value | 0x00000080;
					WriteEAValue(inst, result, EAType.Destination);
				}
			}
		}

		private void ABCD_SBCD(Instruction inst)
		{
			// Determine if we're handling an ABCD instruction or a SBCD instruction by analyzing the top 4 bits of the
			// opcode value.
			BCDOperation opType = ((inst.Opcode & 0xF000) == 0xC000) ? BCDOperation.Addition : BCDOperation.Subtraction;
			byte rSrc = (byte)(inst.Opcode & 0x0007);
			byte rDest = (byte)((inst.Opcode & 0x0E00) >> 9);

			if ((inst.Opcode & 0x0008) == 0)
			{
				// Working with data registers
				uint srcVal = Machine.CPU.ReadDataRegister(rSrc) & 0x000000FF;
				uint destVal = Machine.CPU.ReadDataRegister(rDest) & 0x000000FF;
				var result = BCDCalculation(opType, srcVal, destVal);
				Machine.CPU.WriteDataRegister(rDest, result, OpSize.Byte);
			}
			else
			{
				// Working with memory addresses, so predecrement both address registers by 1 byte.
				var srcAddr = Machine.CPU.DecrementAddressRegister(rSrc, OpSize.Byte);
				var destAddr = Machine.CPU.DecrementAddressRegister(rDest, OpSize.Byte);
				uint srcVal = Machine.Memory.ReadByte(srcAddr);
				uint destVal = Machine.Memory.ReadByte(destAddr);
				var result = BCDCalculation(opType, srcVal, destVal);
				Machine.Memory.WriteByte(destAddr, (byte)(result & 0x000000FF));
			}
		}

		private void NBCD(Instruction inst)
		{
			var value = ReadEAValue(inst, EAType.Destination);
			if (value.HasValue)
			{
				var loVal = 10 - (value.Value & 0x0000000F) - (Machine.CPU.ExtendFlag ? 1 : 0);
				bool carry = loVal < 10;
				if (loVal >= 10)
				{
					loVal = 0;
				}
				var hiVal = 10 - ((value.Value >> 4) & 0x0000000F) - (carry ? 1 : 0);
				carry = hiVal < 10;
				if (hiVal >= 10)
				{
					hiVal = 0;
				}

				var result = (hiVal << 4) + loVal;
				WriteEAValue(inst, (uint)(result & 0x000000FF), EAType.Destination);
				Machine.CPU.CarryFlag = Machine.CPU.ExtendFlag = carry;
				if (result != 0)
				{
					Machine.CPU.ZeroFlag = false;
				}
			}
		}

		private void MOVEP(Instruction inst)
		{
			byte aRegNum = (byte)(inst.Opcode & 0x0007);
			byte dRegNum = (byte)((inst.Opcode & 0x0E00) >> 9);
			OpSize size = (inst.Opcode & 0x0040) == 0 ? OpSize.Word : OpSize.Long;
			bool memToReg = (inst.Opcode & 0x0080) == 0;
			int disp = Helpers.SignExtendValue((uint)inst.SourceExtWord1, OpSize.Word);
			uint address = (uint)((int)Machine.CPU.ReadAddressRegister(aRegNum) + disp);
			if (memToReg)
			{
				if (size == OpSize.Word)
				{
					int val = Machine.Memory.ReadByte(address) << 8;
					val |= Machine.Memory.ReadByte(address + 2);
					Machine.CPU.WriteDataRegister(dRegNum, (uint)val, size);
				}
				else
				{
					var val = Machine.Memory.ReadByte(address) << 24;
					val |= Machine.Memory.ReadByte(address + 2) << 16;
					val |= Machine.Memory.ReadByte(address + 4) << 8;
					val |= Machine.Memory.ReadByte(address + 6);
					Machine.CPU.WriteDataRegister(dRegNum, (uint)val, size);
				}
			}
			else
			{
				uint val = Machine.CPU.ReadDataRegister(dRegNum);
				if (size == OpSize.Word)
				{
					Machine.Memory.WriteByte(address, (byte)((val >> 8) & 0x000000FF));
					Machine.Memory.WriteByte(address + 2, (byte)(val & 0x000000FF));
				}
				else
				{
					Machine.Memory.WriteByte(address, (byte)((val >> 24) & 0x000000FF));
					Machine.Memory.WriteByte(address + 2, (byte)((val >> 16) & 0x000000FF));
					Machine.Memory.WriteByte(address + 4, (byte)((val >> 8) & 0x000000FF));
					Machine.Memory.WriteByte(address + 6, (byte)(val & 0x000000FF));
				}
			}
		}

		private void MOVEM(Instruction inst)
		{
			var (_, _, address, _) = EvaluateEffectiveAddress(inst, EAType.Destination);
			if (address.HasValue)
			{
				if (inst.SourceExtWord1.HasValue)
				{
					ushort regMask = inst.SourceExtWord1.Value;
					OpSize size = inst.Size ?? OpSize.Long;
					bool regToMem = (inst.Opcode & 0x0400) == 0;

					if (regToMem)
					{
						if (((inst.Opcode >> 3) & 0x0007) == 0x0004)
						{
							// Predecrement addressing mode
							var newAddr = MOVEM_RegToMemPreDec(regMask, address.Value, size);
							Machine.CPU.WriteAddressRegister(inst.Opcode & 0x0007, newAddr);
						}
						else
						{
							_ = MOVEM_RegToMem(regMask, address.Value, size);
						}
					}
					else
					{
						var newAddr = MOVEM_MemToReg(regMask, address.Value, size);
						// If postincrement addressing then update the address register.
						if (((inst.Opcode >> 3) & 0x0007) == 0x0003)
						{
							Machine.CPU.WriteAddressRegister(inst.Opcode & 0x0007, newAddr);
						}
					}

				}
			}
		}


	}
}
