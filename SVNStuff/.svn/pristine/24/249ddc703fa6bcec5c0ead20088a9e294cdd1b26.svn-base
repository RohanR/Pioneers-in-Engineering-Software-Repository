//
//	License: The MIT License (MIT)
//  Copyright (c) 2010 Kai Bidstrup 
//
//	http://ua6281.codeplex.com/
//

using System;
using Microsoft.SPOT;

namespace A6281 {
	[Serializable]
	public class LightSet {
		public LightSet( ChipColor[] colors ) {
			this.Colors = colors;
			this.DisplayTimeInMilliSeconds = 100;
		}

		public LightSet( int displayTimeInMilliseconds, ChipColor[] colors ) {
			this.Colors = colors.Clone() as ChipColor[];
			this.DisplayTimeInMilliSeconds = displayTimeInMilliseconds;
		}

		public static LightSet FromChipColors( params ChipColor[] colors ) {
			return new LightSet( colors );
		}

		public static LightSet FromChipColors( int displayTimeInMilliSeconds, params ChipColor[] colors ) {
			return new LightSet( displayTimeInMilliSeconds, colors );
		}

		public static LightSet FromChipColor( int displayTimeInMilliSeconds, int count, ChipColor color ) {
			ChipColor[] colors = new ChipColor[ count ];
			for ( int index = 0; index < count; index++ ) {
				colors[ index ] = color.Clone();
			}
			return new LightSet( displayTimeInMilliSeconds, colors );
		}

		public ChipColor[] Colors {
			get;
			set;
		}

		public int DisplayTimeInMilliSeconds {
			get;
			set;
		}
	}
}
