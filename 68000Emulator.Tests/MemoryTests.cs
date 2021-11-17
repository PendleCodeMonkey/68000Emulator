using PendleCodeMonkey.MC68000EmulatorLib;
using System;
using System.Linq;
using Xunit;

namespace PendleCodeMonkey.MC68000Emulator.Tests
{
	public class MemoryTests
	{
		private uint _memorySize = 0x10000;     // Only allocate 64K of memory for unit testing purposes.

		[Fact]
		public void NewMemory_ShouldNotBeNull()
		{
			Memory memory = new Memory(_memorySize);

			Assert.NotNull(memory);
		}

		[Fact]
		public void NewMemory_ShouldBeCorrectlyAllocated()
		{
			Memory memory = new Memory(_memorySize);

			Assert.NotNull(memory.Data);
			Assert.Equal((int)_memorySize, memory.Data.Length);
		}

		[Fact]
		public void LoadData_SucceedsWhenDataFitsInMemory()
		{
			Memory memory = new Memory(_memorySize);
			var success = memory.LoadData(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x0 }, 0x2000);

			Assert.True(success);
		}

		[Fact]
		public void LoadData_FailsWhenDataExceedsMemoryLimit()
		{
			Memory memory = new Memory(_memorySize);
			var success = memory.LoadData(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x0 }, 0xFFFC);

			Assert.False(success);
		}

		[Fact]
		public void Clear_ShouldClearAllMemory()
		{
			Memory memory = new Memory(_memorySize);
			Span<byte> machineMemorySpan = memory.Data;
			machineMemorySpan.Fill(0xAA);

			memory.Clear();

			Assert.False(memory.Data.Where(x => x > 0).Any());
		}

		[Fact]
		public void DumpMemory_ShouldReturnRequestedMemoryBlock()
		{
			Memory memory = new Memory(_memorySize);
			_ = memory.LoadData(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 }, 0x2000);

			var dump = memory.DumpMemory(0x2001, 0x0004);

			Assert.Equal(4, dump.Length);
			Assert.Equal(0x02, dump[0]);
			Assert.Equal(0x03, dump[1]);
			Assert.Equal(0x04, dump[2]);
			Assert.Equal(0x05, dump[3]);
		}

		[Fact]
		public void ReadByte_ShouldReturnCorrectData()
		{
			Memory memory = new Memory(_memorySize);
			var _ = memory.LoadData(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x00 }, 0x2000);

			byte value = memory.ReadByte(0x2002);

			Assert.Equal(0x03, value);
		}

		[Fact]
		public void ReadByte_ShouldThrowExceptionWhenAddressOutOfRange()
		{
			Memory memory = new Memory(_memorySize);
			var _ = memory.LoadData(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x00 }, 0x2000);

			Assert.Throws<TrapException>(() => memory.ReadByte(_memorySize));
		}

		[Fact]
		public void ReadWord_ShouldReturnCorrectData()
		{
			Memory memory = new Memory(_memorySize);
			var _ = memory.LoadData(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x00 }, 0x2000);

			ushort value = memory.ReadWord(0x2002);

			Assert.Equal(0x0304, value);
		}

		[Fact]
		public void ReadWord_ShouldThrowExceptionWhenAddressOutOfRange()
		{
			Memory memory = new Memory(_memorySize);
			var _ = memory.LoadData(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x00 }, 0x2000);

			Assert.Throws<TrapException>(() => memory.ReadWord(_memorySize));
		}

		[Fact]
		public void ReadWord_ShouldThrowExceptionWhenAddressIsNotEven()
		{
			Memory memory = new Memory(_memorySize);
			var _ = memory.LoadData(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x00 }, 0x2000);

			Assert.Throws<TrapException>(() => memory.ReadWord(0x2003));
		}

		[Fact]
		public void ReadLong_ShouldReturnCorrectData()
		{
			Memory memory = new Memory(_memorySize);
			var _ = memory.LoadData(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x00 }, 0x2000);

			uint value = memory.ReadLong(0x2002);

			Assert.Equal((uint)0x03040500, value);
		}

		[Fact]
		public void ReadLong_ShouldThrowExceptionWhenAddressOutOfRange()
		{
			Memory memory = new Memory(_memorySize);
			var _ = memory.LoadData(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x00 }, 0x2000);

			Assert.Throws<TrapException>(() => memory.ReadLong(_memorySize));
		}

		[Fact]
		public void ReadLong_ShouldThrowExceptionWhenAddressIsNotEven()
		{
			Memory memory = new Memory(_memorySize);
			var _ = memory.LoadData(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x00 }, 0x2000);

			Assert.Throws<TrapException>(() => memory.ReadLong(0x2003));
		}

		[Fact]
		public void WriteByte_ShouldWriteCorrectData()
		{
			Memory memory = new Memory(_memorySize);
			var _ = memory.LoadData(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x00 }, 0x2000);

			memory.WriteByte(0x2002, 0x20);

			Assert.Equal(0x20, memory.Data[0x2002]);
		}

		[Fact]
		public void WriteByte_ShouldThrowExceptionWhenAddressOutOfRange()
		{
			Memory memory = new Memory(_memorySize);
			var _ = memory.LoadData(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x00 }, 0x2000);

			Assert.Throws<TrapException>(() => memory.WriteByte(_memorySize, 0x20));
		}


		[Fact]
		public void WriteWord_ShouldWriteCorrectData()
		{
			Memory memory = new Memory(_memorySize);
			var _ = memory.LoadData(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x00 }, 0x2000);

			memory.WriteWord(0x2002, 0x3040);

			Assert.Equal(0x30, memory.Data[0x2002]);
			Assert.Equal(0x40, memory.Data[0x2003]);
		}

		[Fact]
		public void WriteWord_ShouldThrowExceptionWhenAddressOutOfRange()
		{
			Memory memory = new Memory(_memorySize);
			var _ = memory.LoadData(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x00 }, 0x2000);

			Assert.Throws<TrapException>(() => memory.WriteWord(_memorySize, 0x2030));
		}

		[Fact]
		public void WriteWord_ShouldThrowExceptionWhenAddressIsNotEven()
		{
			Memory memory = new Memory(_memorySize);
			var _ = memory.LoadData(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x00 }, 0x2000);

			Assert.Throws<TrapException>(() => memory.WriteWord(0x2003, 0x2030));
		}

		[Fact]
		public void WriteLong_ShouldWriteCorrectData()
		{
			Memory memory = new Memory(_memorySize);
			var _ = memory.LoadData(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x00 }, 0x2000);

			memory.WriteLong(0x2002, 0x30405060);

			Assert.Equal(0x30, memory.Data[0x2002]);
			Assert.Equal(0x40, memory.Data[0x2003]);
			Assert.Equal(0x50, memory.Data[0x2004]);
			Assert.Equal(0x60, memory.Data[0x2005]);
		}

		[Fact]
		public void WriteLong_ShouldThrowExceptionWhenAddressOutOfRange()
		{
			Memory memory = new Memory(_memorySize);
			var _ = memory.LoadData(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x00 }, 0x2000);

			Assert.Throws<TrapException>(() => memory.WriteLong(_memorySize, 0x20304050));
		}

		[Fact]
		public void WriteLong_ShouldThrowExceptionWhenAddressIsNotEven()
		{
			Memory memory = new Memory(_memorySize);
			var _ = memory.LoadData(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x00 }, 0x2000);

			Assert.Throws<TrapException>(() => memory.WriteLong(0x2003, 0x20304050));
		}

	}
}
