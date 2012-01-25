//
//	License: The MIT License (MIT)
//  Copyright (c) 2010 Kai Bidstrup 
//
//	http://ua6281.codeplex.com/
//

using System;
using Microsoft.SPOT;

namespace A6281 {
	public class ChipColor {

		private const ushort maxColorValue = ( ushort )0x03FF;
		private ushort red;
		private ushort green;
		private ushort blue;

		public static ChipColor PureBlack = new ChipColor( 0, 0, 0 );
		public static ChipColor PureWhite = new ChipColor( maxColorValue, maxColorValue, maxColorValue );
		public static ChipColor PureRed = new ChipColor( maxColorValue, 0, 0 );
		public static ChipColor PureGreen = new ChipColor( 0, maxColorValue, 0 );
		public static ChipColor PureBlue = new ChipColor( 0, 0, maxColorValue );
		public static ChipColor PureCyan = new ChipColor( 0, maxColorValue, maxColorValue );
		public static ChipColor PureYellow = new ChipColor( maxColorValue, maxColorValue, 0 );
		public static ChipColor PureMagenta = new ChipColor( maxColorValue, 0, maxColorValue );

		public static ChipColor FromRGB( ushort red, ushort green, ushort blue ) {
			return new ChipColor( red, green, blue );
		}

		public ChipColor Clone( ) {
			return new ChipColor( this.red, this.green, this.blue );
		}

		public ChipColor( ushort red, ushort green, ushort blue ) {
			this.red = red;
			this.green = green;
			this.blue = blue;
		}

		public ushort RedChannel {
			get {
				return ( ushort )( maxColorValue & this.red );
			}
			set {
				if ( value > maxColorValue )
					throw new Exception( "Red color value must be less than 1024" );
				this.red = value;
			}
		}

		public ushort GreenChannel {
			get {
				return ( ushort )( maxColorValue & this.green );
			}
			set {
				if ( value > maxColorValue )
					throw new Exception( "Green color value must be less than 1024" );
				this.green = value;
			}
		}

		public ushort BlueChannel {
			get {
				return ( ushort )( maxColorValue & this.blue );
			}
			set {
				if ( value > maxColorValue )
					throw new Exception( "Blue color value must be less than 1024" );
				this.blue = value;
			}
		}
	}
}
