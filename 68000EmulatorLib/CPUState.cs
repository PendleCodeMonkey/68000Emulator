using PendleCodeMonkey.MC68000EmulatorLib.Enumerations;

namespace PendleCodeMonkey.MC68000EmulatorLib
{
	/// <summary>
	/// Implementation of the <see cref="CPUState"/> class.
	/// </summary>
	/// <remarks>
	/// This class holds information that can be used to get or set individual elements
	/// of the CPU state. The number of CPU state elements is quite large and therefore it
	/// is preferable to have a single object for handling this state info rather than having
	/// a method (a constructor, for example) with a dozen or more arguments.
	/// </remarks>
	public class CPUState
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CPUState"/> class.
		/// </summary>
		public CPUState()
		{
		}

		/// <summary>
		/// Gets or sets the value of the D0 register.
		/// </summary>
		public uint? D0 { get; set; }

		/// <summary>
		/// Gets or sets the value of the D1 register.
		/// </summary>
		public uint? D1 { get; set; }

		/// <summary>
		/// Gets or sets the value of the D2 register.
		/// </summary>
		public uint? D2 { get; set; }

		/// <summary>
		/// Gets or sets the value of the D3 register.
		/// </summary>
		public uint? D3 { get; set; }

		/// <summary>
		/// Gets or sets the value of the D4 register.
		/// </summary>
		public uint? D4 { get; set; }

		/// <summary>
		/// Gets or sets the value of the D5 register.
		/// </summary>
		public uint? D5 { get; set; }

		/// <summary>
		/// Gets or sets the value of the D6 register.
		/// </summary>
		public uint? D6 { get; set; }

		/// <summary>
		/// Gets or sets the value of the D7 register.
		/// </summary>
		public uint? D7 { get; set; }

		/// <summary>
		/// Gets or sets the value of the A0 register.
		/// </summary>
		public uint? A0 { get; set; }

		/// <summary>
		/// Gets or sets the value of the A1 register.
		/// </summary>
		public uint? A1 { get; set; }

		/// <summary>
		/// Gets or sets the value of the A2 register.
		/// </summary>
		public uint? A2 { get; set; }

		/// <summary>
		/// Gets or sets the value of the A3 register.
		/// </summary>
		public uint? A3 { get; set; }

		/// <summary>
		/// Gets or sets the value of the A4 register.
		/// </summary>
		public uint? A4 { get; set; }

		/// <summary>
		/// Gets or sets the value of the A5 register.
		/// </summary>
		public uint? A5 { get; set; }

		/// <summary>
		/// Gets or sets the value of the A6 register.
		/// </summary>
		public uint? A6 { get; set; }

		/// <summary>
		/// Gets or sets the value of the A7 register.
		/// </summary>
		public uint? A7 { get; set; }

		/// <summary>
		/// Gets or sets the value of the USP register.
		/// </summary>
		public uint? USP { get; set; }

		/// <summary>
		/// Gets or sets the value of the SSP register.
		/// </summary>
		public uint? SSP { get; set; }

		/// <summary>
		/// Gets or sets the value of the Status Register.
		/// </summary>
		public SRFlags? SR { get; set; }

		/// <summary>
		/// Gets or sets the value of the Program Counter.
		/// </summary>
		public uint? PC { get; set; }



		/// <summary>
		/// Transfer values from this <see cref="CPUState"/> instance into the settings in
		/// specified <see cref="CPU"/> instance.
		/// </summary>
		/// <remarks>
		/// Only non-null values in this <see cref="CPUState"/> instance are transferred to the <see cref="CPU"/>.
		/// </remarks>
		/// <param name="cpu">The <see cref="CPU"/> instance into which the state values should be transferred.</param>
		public void TransferStateToCPU(CPU cpu)
		{
			if (D0.HasValue)
			{
				cpu.WriteDataRegister(0, D0.Value);
			}
			if (D1.HasValue)
			{
				cpu.WriteDataRegister(1, D1.Value);
			}
			if (D2.HasValue)
			{
				cpu.WriteDataRegister(2, D2.Value);
			}
			if (D3.HasValue)
			{
				cpu.WriteDataRegister(3, D3.Value);
			}
			if (D4.HasValue)
			{
				cpu.WriteDataRegister(4, D4.Value);
			}
			if (D5.HasValue)
			{
				cpu.WriteDataRegister(5, D5.Value);
			}
			if (D6.HasValue)
			{
				cpu.WriteDataRegister(6, D6.Value);
			}
			if (D7.HasValue)
			{
				cpu.WriteDataRegister(7, D7.Value);
			}

			if (A0.HasValue)
			{
				cpu.WriteAddressRegister(0, A0.Value);
			}
			if (A1.HasValue)
			{
				cpu.WriteAddressRegister(1, A1.Value);
			}
			if (A2.HasValue)
			{
				cpu.WriteAddressRegister(2, A2.Value);
			}
			if (A3.HasValue)
			{
				cpu.WriteAddressRegister(3, A3.Value);
			}
			if (A4.HasValue)
			{
				cpu.WriteAddressRegister(4, A4.Value);
			}
			if (A5.HasValue)
			{
				cpu.WriteAddressRegister(5, A5.Value);
			}
			if (A6.HasValue)
			{
				cpu.WriteAddressRegister(6, A6.Value);
			}
			if (A7.HasValue)
			{
				cpu.WriteAddressRegister(7, A7.Value);
			}

			if (USP.HasValue)
			{
				cpu.USP = USP.Value;
			}
			if (SSP.HasValue)
			{
				cpu.SSP = SSP.Value;
			}

			if (SR.HasValue)
			{
				cpu.SR = SR.Value;
			}

			if (PC.HasValue)
			{
				cpu.PC = PC.Value;
			}
		}

		/// <summary>
		/// Transfer values from the specified <see cref="CPU"/> instance into this <see cref="CPUState"/> instance.
		/// </summary>
		/// <param name="cpu">The <see cref="CPU"/> instance from which the state values should be transferred.</param>
		public void TransferStateFromCPU(CPU cpu)
		{
			D0 = cpu.ReadDataRegister(0);
			D1 = cpu.ReadDataRegister(1);
			D2 = cpu.ReadDataRegister(2);
			D3 = cpu.ReadDataRegister(3);
			D4 = cpu.ReadDataRegister(4);
			D5 = cpu.ReadDataRegister(5);
			D6 = cpu.ReadDataRegister(6);
			D7 = cpu.ReadDataRegister(7);

			A0 = cpu.ReadAddressRegister(0);
			A1 = cpu.ReadAddressRegister(1);
			A2 = cpu.ReadAddressRegister(2);
			A3 = cpu.ReadAddressRegister(3);
			A4 = cpu.ReadAddressRegister(4);
			A5 = cpu.ReadAddressRegister(5);
			A6 = cpu.ReadAddressRegister(6);
			A7 = cpu.ReadAddressRegister(7);

			USP = cpu.USP;
			SSP = cpu.SSP;

			SR = cpu.SR;

			PC = cpu.PC;
		}

	}
}
