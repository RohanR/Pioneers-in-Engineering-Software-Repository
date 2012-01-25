//
//	License: The MIT License (MIT)
//  Copyright (c) 2010 Kai Bidstrup 
//
//	http://ua6281.codeplex.com/
//

using System;
using System.Xml;
using Microsoft.SPOT;
using System.Collections;

namespace A6281 {
	[Serializable]
	public class LightSequence {
		private LightSequence() {
			this.LightSets = new LightSet[ 0 ];
		}

		public LightSequence( LightSet[] lightSets ) {
			this.LightSets = lightSets;
		}

		public static LightSequence FromLightSets( params LightSet[] lightSets ) {
			return new LightSequence( lightSets );
		}

		public static LightSequence CylonSequence( int length, int displayTimeInMilliSeconds, ChipColor backColor, ChipColor foreColor ) {
			LightSet[] tempLightSets = new LightSet[ length * 2 ];

			for ( int lightSetIndex = 0; lightSetIndex < length; lightSetIndex++ ) {
				ChipColor[] colors = new ChipColor[ length ];
				for ( int colorIndex = 0; colorIndex < length; colorIndex++ ) {
					if ( colorIndex == lightSetIndex )
						colors[ colorIndex ] = foreColor;
					else
						colors[ colorIndex ] = backColor;
				}
				tempLightSets[ lightSetIndex ] = new LightSet( displayTimeInMilliSeconds, colors );
			}

			for ( int lightSetIndex = length; lightSetIndex < length * 2; lightSetIndex++ ) {
				ChipColor[] colors = new ChipColor[ length ];
				for ( int colorIndex = 0; colorIndex < length; colorIndex++ ) {
					if ( colorIndex == lightSetIndex - length )
						colors[ length - 1 - colorIndex ] = foreColor;
					else
						colors[ length - 1 - colorIndex ] = backColor;
				}
				tempLightSets[ lightSetIndex ] = new LightSet( displayTimeInMilliSeconds, colors );
			}

			return FromLightSets( tempLightSets );
		}

		public static LightSequence LEDTestSequence( int length, int displayTimeInMilliseconds ) {
			LightSet[] tempLightSets = new LightSet[ 4 ];

			ChipColor[] colors = new ChipColor[ length ];
			for ( int colorIndex = 0; colorIndex < length; colorIndex++ ) {
				colors[ colorIndex ] = ChipColor.PureBlack;
			}
			tempLightSets[ 0 ] = new LightSet( displayTimeInMilliseconds, colors );

//			colors = new ChipColor[ length ];
			for ( int colorIndex = 0; colorIndex < length; colorIndex++ ) {
				colors[ colorIndex ] = ChipColor.PureRed;
			}
			tempLightSets[ 1 ] = new LightSet( displayTimeInMilliseconds, colors );

//			colors = new ChipColor[ length ];
			for ( int colorIndex = 0; colorIndex < length; colorIndex++ ) {
				colors[ colorIndex ] = ChipColor.PureGreen;
			}
			tempLightSets[ 2 ] = new LightSet( displayTimeInMilliseconds, colors );

//			colors = new ChipColor[ length ];
			for ( int colorIndex = 0; colorIndex < length; colorIndex++ ) {
				colors[ colorIndex ] = ChipColor.PureBlue;
			}
			tempLightSets[ 3 ] = new LightSet( displayTimeInMilliseconds, colors );

			return FromLightSets( tempLightSets );
		}

		public LightSet[] LightSets {
			get;
			set;
		}
	}
}
