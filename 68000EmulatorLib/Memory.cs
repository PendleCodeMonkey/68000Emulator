using PendleCodeMonkey.MC68000EmulatorLib.Enumerations;
using System;

namespace PendleCodeMonkey.MC68000EmulatorLib
{
	/// <summary>
	/// Implementation of the <see cref="Memory"/> class.
	/// </summary>
	class Memory
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Memory"/> class.
		/// </summary>
		/// <param name="memSize">The number of bytes of memory to be allocated.</param>
		internal Memory(uint memSize)
		{
			Data = new byte[memSize];
		}

		/// <summary>
		/// Gets or sets the byte array that holds the memory contents.
		/// </summary>
		internal byte[] Data { get; set; }

		/// <summary>
		/// Load data into the specified address, optionally clearing all memory before doing so.
		/// </summary>
		/// <param name="data">The data to be loaded.</param>
		/// <param name="loadAddress">The address at which the data sound be loaded.</param>
		/// <param name="clearBeforeLoad"><c>true</c> if all memory should be cleared before loading, otherwise <c>false</c>.</param>
		/// <returns><c>true</c> if the data was loaded into memory, otherwise <c>false</c>.</returns>
		public bool LoadData(byte[] data, uint loadAddress, bool clearBeforeLoad = true)
		{
			// Check that the data being loaded will actually fit at the specified load address.
			if (loadAddress + data.Length > Data.Length)
			{
				return false;
			}

			Span<byte> machineMemorySpan = Data;
			if (clearBeforeLoad)
			{
				machineMemorySpan.Fill(0);
			}
			Span<byte> dataSpan = data;
			Span<byte> loadMemorySpan = machineMemorySpan.Slice((int)loadAddress, dataSpan.Length);
			dataSpan.CopyTo(loadMemorySpan);
			return true;
		}

		/// <summary>
		/// Clear all of the <see cref="Memory"/> instance's data.
		/// </summary>
		public void Clear() => Data.AsSpan().Fill(0);

		/// <summary>
		/// Return the specified block of memory.
		/// </summary>
		/// <param name="address">Start address of the requested block of memory.</param>
		/// <param name="length">Length (in bytes) of the block of memory to be retrieved.</param>
		/// <returns>Read-only copy of the requested memory.</returns>
		public ReadOnlySpan<byte> DumpMemory(uint address, uint length)
		{
			if (address + length > Data.Length)
			{
				return null;
			}

			ReadOnlySpan<byte> dumpMem = Data.AsSpan().Slice((int)address, (int)length);
			return dumpMem;
		}

		/// <summary>
		/// Read the byte value at the specified address.
		/// </summary>
		/// <param name="address">The address of the memory to be read.</param>
		/// <returns>The value that was read from the specified address.</returns>
		public byte ReadByte(uint address)
		{
			if (address >= Data.Length)
			{
				Helpers.RaiseTRAPException(TrapVector.AddressError);
			}
			return Data[address];
		}

		/// <summary>
		/// Read the 16-bit value at the specified address.
		/// </summary>
		/// <param name="address">The address of the memory to be read.</param>
		/// <returns>The 16-bit value that was read from the specified address.</returns>
		public ushort ReadWord(uint address)
		{
			if ((address % 0x00000002) != 0 || address > Data.Length - 2)
			{
				Helpers.RaiseTRAPException(TrapVector.AddressError);
			}
			return (ushort)((Data[address] << 8) + Data[address + 1]);
		}

		/// <summary>
		/// Read the 32-bit value at the specified address.
		/// </summary>
		/// <param name="address">The address of the memory to be read.</param>
		/// <returns>The 32-bit value that was read from the specified address.</returns>
		public uint ReadLong(uint address)
		{
			if ((address % 0x00000002) != 0 || address > Data.Length - 4)
			{
				Helpers.RaiseTRAPException(TrapVector.AddressError);
			}
			return (uint)((Data[address] << 24) + (Data[address + 1] << 16) + (Data[address + 2] << 8) + Data[address + 3]);
		}

		/// <summary>
		/// Write a byte value to the specified address.
		/// </summary>
		/// <param name="address">The address at which the value should be written.</param>
		/// <param name="value">The value to be written to the specified address.</param>
		public void WriteByte(uint address, byte value)
		{
			if (address >= Data.Length)
			{
				Helpers.RaiseTRAPException(TrapVector.AddressError);
			}
			Data[address] = value;
		}

		/// <summary>
		/// Write a 16-bit value to the specified address.
		/// </summary>
		/// <remarks>
		/// The 16-bit value is written to memory as high byte followed by low byte.
		/// </remarks>
		/// <param name="address">The address at which the value should be written.</param>
		/// <param name="value">The value to be written to the specified address.</param>
		public void WriteWord(uint address, ushort value)
		{
			if ((address % 0x00000002) != 0 || address > Data.Length - 2)
			{
				Helpers.RaiseTRAPException(TrapVector.AddressError);
			}
			Data[address] = (byte)((value >> 8) & 0xFF);
			Data[address + 1] = (byte)(value & 0xFF);
		}

		/// <summary>
		/// Write a 32-bit value to the specified address.
		/// </summary>
		/// <remarks>
		/// Each byte of the 32-bit value is written to memory in sequence from the highest byte to the lowest byte.
		/// </remarks>
		/// <param name="address">The address at which the value should be written.</param>
		/// <param name="value">The value to be written to the specified address.</param>
		public void WriteLong(uint address, uint value)
		{
			if ((address % 0x00000002) != 0 || address > Data.Length - 4)
			{
				Helpers.RaiseTRAPException(TrapVector.AddressError);
			}
			Data[address] = (byte)((value >> 24) & 0xFF);
			Data[address + 1] = (byte)((value >> 16) & 0xFF);
			Data[address + 2] = (byte)((value >> 8) & 0xFF);
			Data[address + 3] = (byte)(value & 0xFF);
		}
	}
}
