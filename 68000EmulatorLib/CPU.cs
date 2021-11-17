using PendleCodeMonkey.MC68000EmulatorLib.Enumerations;
using System.Diagnostics;

namespace PendleCodeMonkey.MC68000EmulatorLib
{
	public class CPU
	{
		public CPU()
		{
			Reset();
		}

		/// <summary>
		/// Gets or sets the data register values.
		/// </summary>
		internal uint[] DataRegisters { get; set; } = new uint[8];

		/// <summary>
		/// Gets or sets the address register values.
		/// </summary>
		internal uint[] AddressRegisters { get; set; } = new uint[7];

		/// <summary>
		/// Gets or sets the User Stack Pointer value.
		/// </summary>
		internal uint USP { get; set; }

		/// <summary>
		/// Gets or sets the Supervisor Stack Pointer value.
		/// </summary>
		internal uint SSP { get; set; }

		/// <summary>
		/// Gets or sets the value of the Program Counter.
		/// </summary>
		internal uint PC { get; set; }

		/// <summary>
		/// Gets or sets the Status Register value.
		/// </summary>
		internal SRFlags SR { get; set; }


		// **********************
		// Flag helper properties
		// **********************

		/// <summary>
		/// Gets or sets the value of the Carry flag.
		/// </summary>
		internal bool CarryFlag
		{
			get => SR.HasFlag(SRFlags.Carry);
			set
			{
				if (value)
				{
					SR |= SRFlags.Carry;
				}
				else
				{
					SR &= ~SRFlags.Carry;
				}
			}
		}


		/// <summary>
		/// Gets or sets the value of the Overflow flag.
		/// </summary>
		internal bool OverflowFlag
		{
			get => SR.HasFlag(SRFlags.Overflow);
			set
			{
				if (value)
				{
					SR |= SRFlags.Overflow;
				}
				else
				{
					SR &= ~SRFlags.Overflow;
				}
			}
		}

		/// <summary>
		/// Gets or sets the value of the Zero flag.
		/// </summary>
		internal bool ZeroFlag
		{
			get => SR.HasFlag(SRFlags.Zero);
			set
			{
				if (value)
				{
					SR |= SRFlags.Zero;
				}
				else
				{
					SR &= ~SRFlags.Zero;
				}
			}
		}


		/// <summary>
		/// Gets or sets the value of the Negative flag.
		/// </summary>
		internal bool NegativeFlag
		{
			get => SR.HasFlag(SRFlags.Negative);
			set
			{
				if (value)
				{
					SR |= SRFlags.Negative;
				}
				else
				{
					SR &= ~SRFlags.Negative;
				}
			}
		}

		/// <summary>
		/// Gets or sets the value of the Extend flag.
		/// </summary>
		internal bool ExtendFlag
		{
			get => SR.HasFlag(SRFlags.Extend);
			set
			{
				if (value)
				{
					SR |= SRFlags.Extend;
				}
				else
				{
					SR &= ~SRFlags.Extend;
				}
			}
		}

		/// <summary>
		/// Gets or sets the value of the Supervisor Mode flag.
		/// </summary>
		internal bool SupervisorMode
		{
			get => SR.HasFlag(SRFlags.SupervisorMode);
			set
			{
				if (value)
				{
					SR |= SRFlags.SupervisorMode;
				}
				else
				{
					SR &= ~SRFlags.SupervisorMode;
				}
			}
		}

		/// <summary>
		/// Gets or sets the value of the Trace Mode flag.
		/// </summary>
		internal bool TraceMode
		{
			get => SR.HasFlag(SRFlags.TraceMode);
			set
			{
				if (value)
				{
					SR |= SRFlags.TraceMode;
				}
				else
				{
					SR &= ~SRFlags.TraceMode;
				}
			}
		}


		/// <summary>
		/// Reset the CPU settings to their default state.
		/// </summary>
		internal void Reset()
		{
			for (int i = 0; i < DataRegisters.Length; i++)
			{
				DataRegisters[i] = 0;
			}
			for (int i = 0; i < AddressRegisters.Length; i++)
			{
				AddressRegisters[i] = 0;
			}
			USP = 0;
			SSP = 0;
			PC = 0;
			SR = 0;
		}

		/// <summary>
		/// Read the value of the specified data register.
		/// </summary>
		/// <param name="reg">The number of the data register (0 to 7).</param>
		/// <returns>The value of the specified data register.</returns>
		internal uint ReadDataRegister(int reg)
		{
			Debug.Assert(reg >= 0 && reg < 8, "Invalid data register index.");
			if (reg >= 0 && reg < 8)
			{
				return DataRegisters[reg];
			}

			return 0;
		}

		/// <summary>
		/// Set the value of the specified data register.
		/// </summary>
		/// <remarks>
		/// When a data size is specified, only the bits corresponding to that size of data
		/// are affected (for example, if a data size of OpSize.Byte is specified then only the
		/// lower 8 bits of the data register are modified, with the upper 24 bits remaining unaltered).
		/// </remarks>
		/// <param name="reg">The number of the data register (0 to 7).</param>
		/// <param name="value">The value to be written to the data register.</param>
		/// <param name="dataSize">The size of data to be written [optional] - default is Long.</param>
		internal void WriteDataRegister(int reg, uint value, OpSize? dataSize = null)
		{
			Debug.Assert(reg >= 0 && reg < 8, "Invalid data register index.");
			if (reg >= 0 && reg < 8)
			{
				OpSize size = dataSize ?? OpSize.Long;      // default to the full 32-bit value.
				DataRegisters[reg] = size switch
				{
					OpSize.Byte => (DataRegisters[reg] & 0xFFFFFF00) | (value & 0x000000FF),
					OpSize.Word => (DataRegisters[reg] & 0xFFFF0000) | (value & 0x0000FFFF),
					_ => value,
				};
			}
		}

		/// <summary>
		/// Read the value of the specified address register.
		/// </summary>
		/// <param name="reg">The number of the address register (0 to 7).</param>
		/// <returns>The value of the specified address register.</returns>
		internal uint ReadAddressRegister(int reg)
		{
			if (reg >= 0 && reg < 7)
			{
				return AddressRegisters[reg];
			}
			else if (reg == 7)
			{
				return SupervisorMode ? SSP : USP;
			}

			Debug.Assert(false, "Invalid address register index.");
			return 0;
		}

		/// <summary>
		/// Set the value of the specified address register.
		/// </summary>
		/// <remarks>
		/// When a data size is specified, only the bits corresponding to that size of data
		/// are affected (for example, if a data size of OpSize.Byte is specified then only the
		/// lower 8 bits of the address register are modified, with the upper 24 bits remaining unaltered).
		/// </remarks>
		/// <param name="reg">The number of the address register (0 to 7).</param>
		/// <param name="value">The value to be written to the address register.</param>
		/// <param name="dataSize">The size of data to be written [optional] - default is Long.</param>
		internal void WriteAddressRegister(int reg, uint value, OpSize? dataSize = null)
		{
			OpSize size = dataSize ?? OpSize.Long;      // default to the full 32-bit value.
			Debug.Assert(reg >= 0 && reg < 8, "Invalid address register index.");
			if (reg >= 0 && reg < 7)
			{
				AddressRegisters[reg] = size switch
				{
					OpSize.Byte => (AddressRegisters[reg] & 0xFFFFFF00) | (value & 0x000000FF),
					OpSize.Word => (AddressRegisters[reg] & 0xFFFF0000) | (value & 0x0000FFFF),
					_ => value,
				};
			}
			else if (reg == 7)
			{
				if (SupervisorMode)
				{
					SSP = size switch
					{
						OpSize.Byte => (SSP & 0xFFFFFF00) | (value & 0x000000FF),
						OpSize.Word => (SSP & 0xFFFF0000) | (value & 0x0000FFFF),
						_ => value,
					};
				}
				else
				{
					USP = size switch
					{
						OpSize.Byte => (USP & 0xFFFFFF00) | (value & 0x000000FF),
						OpSize.Word => (USP & 0xFFFF0000) | (value & 0x0000FFFF),
						_ => value,
					};
				}
			}
		}

		/// <summary>
		/// Increment the specified address register by the number of bytes
		/// corresponding to the specified data size.
		/// </summary>
		/// <param name="regNum">The number of the address register (0 to 7).</param>
		/// <param name="size">The size indicating the number of bytes by which the register should be incremented.</param>
		/// <returns>The incremented address register value.</returns>
		internal uint IncrementAddressRegister(byte regNum, OpSize size)
		{
			uint value = ReadAddressRegister(regNum);
			int numBytes = size == OpSize.Byte ? 1 : (size == OpSize.Long ? 4 : 2);
			value += (uint)numBytes;
			WriteAddressRegister(regNum, value);
			return value;
		}

		/// <summary>
		/// Decrement the specified address register by the number of bytes
		/// corresponding to the specified data size.
		/// </summary>
		/// <param name="regNum">The number of the address register (0 to 7).</param>
		/// <param name="size">The size indicating the number of bytes by which the register should be decremented.</param>
		/// <returns>The decremented address register value.</returns>
		internal uint DecrementAddressRegister(byte regNum, OpSize size)
		{
			uint value = ReadAddressRegister(regNum);
			int numBytes = size == OpSize.Byte ? 1 : (size == OpSize.Long ? 4 : 2);
			value -= (uint)numBytes;
			WriteAddressRegister(regNum, value);
			return value;
		}

		/// <summary>
		/// Evaluate the result of the specified condition.
		/// </summary>
		/// <param name="condition">The enumerated value of the condition to be evaluated.</param>
		/// <returns><c>true</c> if the specified condition is met, otherwise <c>false</c>.</returns>
		internal bool EvaluateCondition(Condition condition)
		{
			return condition switch
			{
				// True
				Condition.T => true,
				// False
				Condition.F => false,
				// Higher
				Condition.HI => !ZeroFlag && !CarryFlag,
				// Lower or Same
				Condition.LS => ZeroFlag || CarryFlag,
				// Carry Clear
				Condition.CC => !CarryFlag,
				// Carry Set
				Condition.CS => CarryFlag,
				// Not Equal
				Condition.NE => !ZeroFlag,
				// Equal
				Condition.EQ => ZeroFlag,
				// Overflow Clear
				Condition.VC => !OverflowFlag,
				// Overflow Set
				Condition.VS => OverflowFlag,
				// Plus
				Condition.PL => !NegativeFlag,
				// Minus
				Condition.MI => NegativeFlag,
				// Greater or Equal
				Condition.GE => NegativeFlag == OverflowFlag,
				// Less Than
				Condition.LT => NegativeFlag != OverflowFlag,
				// Greater Than
				Condition.GT => !ZeroFlag && (NegativeFlag == OverflowFlag),
				// Less or Equal
				Condition.LE => ZeroFlag || (NegativeFlag != OverflowFlag),
				_ => false,
			};
		}


		/// <summary>
		/// Increment the value of the Program Counter.
		/// </summary>
		/// <param name="numBytes">The number of bytes by which the program counter should be incremented.</param>
		internal void IncrementPC(byte numBytes) => PC += numBytes;

		/// <summary>
		/// Reset external devices (called as a result of executing a 68000 RESET instruction).
		/// </summary>
		internal void ResetExternalDevices()
		{
			// Currently does nothing.
		}
	}
}
