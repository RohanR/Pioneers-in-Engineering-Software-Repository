//
//	License: The MIT License (MIT)
//  Copyright (c) 2010 Kai Bidstrup 
//
//	http://ua6281.codeplex.com/
//

using System;
using Microsoft.SPOT;

namespace A6281 {
	public enum ClockModeEnum {
		_800kHz = 0,
		_400kHz = 1,
		external = 2,
		_200kHz = 3
	}

	public class ChipRegisters {

		private const ushort maxCorrectionValue = ( ushort )0x003F;
		private ushort redCorrection = 120;
		private ushort greenCorrection = 100;
		private ushort blueCorrection = 100;

		public ChipRegisters() {
			this.Color = new ChipColor( 0, 0, 0 );
		}

		public ChipColor Color {
			get;
			set;
		}


		public ushort RedCorrection {
			get {
				return ( ushort )( maxCorrectionValue & this.redCorrection );
			}
			set {
				if ( value > maxCorrectionValue )
					throw new Exception( "Red correction value must be less than 64" );
				this.redCorrection = value;
			}
		}

		public ushort GreenCorrection {
			get {
				return ( ushort )( maxCorrectionValue & this.greenCorrection );
			}
			set {
				if ( value > maxCorrectionValue )
					throw new Exception( "Green correction value must be less than 64" );
				this.blueCorrection = value;
			}
		}

		public ushort BlueCorrection {
			get {
				return ( ushort )( maxCorrectionValue & this.blueCorrection );
			}
			set {
				if ( value > maxCorrectionValue )
					throw new Exception( "Blue correction value must be less than 64" );
				this.blueCorrection = value;
			}
		}

		public ClockModeEnum ClockMode {
			get;
			set;
		}

		public bool AllegroTestBit1 {
			get;
			set;
		}

		public bool AllegroTestBit2 {
			get;
			set;
		}

		public const int PacketSize = 4;

		public byte[] ColorPacket {
			get {
				byte[] result = new byte[ PacketSize ];
				InsertColorPacketData( result, 0 );
				return result;
			}
		}

		public byte[] CorrectionPacket {
			get {
				byte[] result = new byte[ PacketSize ];
				InsertCorrectionPacketData( result, 0 );
				return result;
			}
		}

		public void InsertColorPacketData( byte[] array, int startIndex ) {
			if ( array.Length < startIndex + PacketSize )
				throw new Exception( "InsertCorrectionPacketData buffer length is too short (4 needed)" );

			array[ startIndex + 0 ] = ( byte )( 0xFF & ( this.Color.GreenChannel >> 4 ) );
			array[ startIndex + 1 ] = ( byte )( 0xFF & ( ( this.Color.GreenChannel << 4 ) | ( this.Color.RedChannel >> 6 ) ) );
			array[ startIndex + 2 ] = ( byte )( 0xFF & ( ( this.Color.RedChannel << 2 ) | ( this.Color.BlueChannel >> 8 ) ) );
			array[ startIndex + 3 ] = ( byte )( 0xFF & this.Color.BlueChannel );
		}

		private object Exception( string p ) {
			throw new NotImplementedException();
		}

		public void InsertCorrectionPacketData( byte[] array, int startIndex ) {
			if ( array.Length < startIndex + PacketSize )
				throw new Exception( "InsertCorrectionPacketData buffer length is too short (4 needed)" );

			array[ startIndex ] = ( byte )( 0xFF & ( 0x40 | ( this.GreenCorrection >> 4 ) ) );
			array[ startIndex + 1 ] = ( byte )( 0xFF & ( ( this.GreenCorrection << 4 ) | ( this.RedCorrection >> 6 ) ) );
			array[ startIndex + 2 ] = ( byte )( 0xFF & ( ( this.RedCorrection << 2 ) | ( this.BlueCorrection >> 8 ) ) );
			array[ startIndex + 3 ] = ( byte )( 0xFF & ( this.BlueCorrection ) );
		}
	}
}
