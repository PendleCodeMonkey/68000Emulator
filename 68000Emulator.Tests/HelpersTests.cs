using PendleCodeMonkey.MC68000EmulatorLib;
using PendleCodeMonkey.MC68000EmulatorLib.Enumerations;
using Xunit;

namespace PendleCodeMonkey.MC68000Emulator.Tests
{
	public class HelpersTests
	{
		[Theory]
		[InlineData(0x0000, 0x00)]
		[InlineData(0x0001, 0x01)]
		[InlineData(0x0030, 0x30)]
		[InlineData(0x003F, 0x3F)]
		[InlineData(0x0040, 0x00)]
		[InlineData(0x0087, 0x07)]
		[InlineData(0x00FF, 0x3F)]
		[InlineData(0x0136, 0x36)]
		[InlineData(0xFFC0, 0x00)]
		public void GetEAMode(ushort value, byte expectedResult)
		{
			byte result = Helpers.GetEAMode(value);

			// Assert
			Assert.Equal(expectedResult, result);
		}

		[Theory]
		[InlineData(0x0000, 0x00)]
		[InlineData(0x0800, 0x04)]
		[InlineData(0x0300, 0x21)]
		[InlineData(0xFFFF, 0x3F)]
		[InlineData(0xF20F, 0x01)]
		[InlineData(0xF03F, 0x00)]
		public void GetReversedEAMode(ushort value, byte expectedResult)
		{
			byte result = Helpers.GetReversedEAMode(value);

			// Assert
			Assert.Equal(expectedResult, result);
		}

		[Theory]
		[InlineData(0x0000, OpSize.Byte)]
		[InlineData(0x0040, OpSize.Word)]
		[InlineData(0x0080, OpSize.Long)]
		[InlineData(0xFFFF, (OpSize)3)]
		public void GetOpSize(ushort value, OpSize expectedResult)
		{
			OpSize result = Helpers.GetOpSize(value);

			// Assert
			Assert.Equal(expectedResult, result);
		}

		[Theory]
		[InlineData(OpSize.Byte, 0x000000FF)]
		[InlineData(OpSize.Word, 0x0000FFFF)]
		[InlineData(OpSize.Long, 0xFFFFFFFF)]
		public void SizeMask(OpSize size, uint expectedResult)
		{
			uint result = Helpers.SizeMask(size);

			// Assert
			Assert.Equal(expectedResult, result);
		}

		[Theory]
		[InlineData(OpSize.Byte, 0x00000080)]
		[InlineData(OpSize.Word, 0x00008000)]
		[InlineData(OpSize.Long, 0x80000000)]
		public void SizeMSB(OpSize size, uint expectedResult)
		{
			uint result = Helpers.SizeMSB(size);

			// Assert
			Assert.Equal(expectedResult, result);
		}

		[Theory]
		[InlineData(0x00000020, OpSize.Byte, 0x00000020)]
		[InlineData(0x00000080, OpSize.Byte, -128)]
		[InlineData(0x11223380, OpSize.Byte, -128)]
		[InlineData(0x34567890, OpSize.Byte, -112)]
		[InlineData(0xFFFFFFFF, OpSize.Byte, -1)]
		[InlineData(0xFFFFFF00, OpSize.Byte, 0)]
		[InlineData(0x00000020, OpSize.Word, 0x00000020)]
		[InlineData(0x00008000, OpSize.Word, -32768)]
		[InlineData(0x22338000, OpSize.Word, -32768)]
		[InlineData(0x12345678, OpSize.Word, 0x00005678)]
		[InlineData(0xFFFFFFFF, OpSize.Word, -1)]
		[InlineData(0xFFFF0000, OpSize.Word, 0)]
		[InlineData(0x00000020, OpSize.Long, 0x00000020)]
		[InlineData(0x00008000, OpSize.Long, 32768)]
		[InlineData(0x12345678, OpSize.Long, 0x12345678)]
		[InlineData(0xFFFFFFFF, OpSize.Long, -1)]
		[InlineData(0xFFFF0000, OpSize.Long, -65536)]
		public void SignExtendValue(uint value, OpSize size, int expectedResult)
		{
			int result = Helpers.SignExtendValue(value, size);

			// Assert
			Assert.Equal(expectedResult, result);
		}

		[Fact]
		public void RaiseTRAPException()
		{
			// Assert
			Assert.Throws<TrapException>(() => Helpers.RaiseTRAPException(8));
			Assert.Throws<TrapException>(() => Helpers.RaiseTRAPException(TrapVector.AddressError));
		}

	}
}
