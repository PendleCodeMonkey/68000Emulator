using PendleCodeMonkey.MC68000EmulatorLib;
using System;

namespace PendleCodeMonkey.MC68000EmulatorConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			EmulatorTest_8BitSort();

			Console.ReadKey();
		}

		private static void EmulatorTest_8BitSort()
		{
			Machine machine = new Machine();

			ushort[] code = new ushort[] { 0x41F8, 0x2000, 0x4240, 0x1018, 0x43F0, 0x00FF, 0x4241, 0x2448, 0x101A, 0xB012, 0x650A,
											0x1212, 0x1541, 0xFFFF, 0x1480, 0x5241, 0xB3CA, 0x62EC, 0x4A41, 0x66E4, 0x4E75};
			machine.LoadExecutableData(code, 0x1000);
			CPUState initState = new CPUState
			{
				USP = 0x00004000
			};
			machine.SetCPUState(initState);

			// Load unsorted 8-bit values into memory
			byte[] data = new byte[51];
			data[0] = 50;
			for (int i = 0; i < 50; i++)
			{
				data[i + 1] = i % 2 == 0 ? (byte)((100 - i) * 2) : (byte)((50 - i) * 2);
			}
			machine.LoadData(data, 0x2000, false);

			// Dump out the unsorted 8-bit value list.
			Console.WriteLine("Unsorted:");
			for (int i = 0; i < 50; i++)
			{
				Console.Write($"{data[i + 1]} ");
			}
			Console.WriteLine();
			Console.WriteLine();

			// Execute the 68000 code (which should sort the 8-bit values that have been loaded into memory).
			machine.Execute();

			// Dump out the sorted 8-bit value list (to show that the 68000 code has successfully sorted the values).
			var memDump = machine.DumpMemory(0x00002000, 51);
			Console.WriteLine("Sorted:");
			for (int i = 0; i < 50; i++)
			{
				Console.Write($"{memDump[i + 1]} ");
			}
			Console.WriteLine();
			Console.WriteLine();

			// Dump out the final CPU status.
			Console.WriteLine("CPU status dump:");
			string dump = machine.Dump();
			Console.WriteLine(dump);
		}
	}
}
