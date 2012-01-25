//
//	License: The MIT License (MIT)
//  Copyright (c) 2010 Kai Bidstrup 
//
//	http://ua6281.codeplex.com/
//

using System;
using System.Text;
using System.Threading;

using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace A6281 {
	public class Single : IDisposable {
		protected SPI.Configuration configuration;
		protected SPI serialPeripheralInterface;
		protected OutputPort latchOutput;
		protected OutputPort outputEnableOutput;

		public Single( Cpu.Pin chipSelectPin, Cpu.Pin latchPin, Cpu.Pin outputEnablePin, SPI.SPI_module spiModule ) {
			try {
				this.configuration = new SPI.Configuration( chipSelectPin
					, true
					, 0
					, 0
					, false
					, true
					, 800
					, spiModule );

				this.serialPeripheralInterface = new SPI( this.configuration );

				this.latchOutput = new OutputPort( latchPin, false );
				this.outputEnableOutput = new OutputPort( outputEnablePin, true );
				this.Registers = new ChipRegisters();
			}
			catch ( Exception ex ) {
				Debug.Print( ex.Message );
			}
		}

		public ChipRegisters Registers {
			get;
			set;
		}

		public bool On {
			get {
				return !this.outputEnableOutput.Read();
			}
			set {
				this.outputEnableOutput.Write( !value );
			}
		}

		public void SetCorrectionImmediate() {
			byte[] packet = this.Registers.CorrectionPacket;
			this.serialPeripheralInterface.Write( packet );
			Thread.Sleep( 1 );
			this.latchOutput.Write( true );
			Thread.Sleep( 5 );
			this.latchOutput.Write( false );
		}

		public void SetColorImmediate( ushort r, ushort g, ushort b ) {
			try {
				this.Registers.Color.RedChannel = r;
				this.Registers.Color.BlueChannel = g;
				this.Registers.Color.GreenChannel = b;

				byte[] packet = this.Registers.ColorPacket;

				this.serialPeripheralInterface.Write( packet );

				Thread.Sleep( 1 );
				this.latchOutput.Write( true );
				Thread.Sleep( 5 );
				this.latchOutput.Write( false );
			}
			catch ( Exception ex ) {
				Debug.Print( ex.Message );
			}
		}

		#region IDisposable Members

		public void Dispose() {
			this.outputEnableOutput.Dispose();
			this.outputEnableOutput = null;
			this.latchOutput.Dispose();
			this.latchOutput = null;
			this.serialPeripheralInterface.Dispose();
			this.serialPeripheralInterface = null;
			GC.SuppressFinalize( this );
		}

		#endregion
	}
}
