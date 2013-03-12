//
//  Author:
//    Stefano Balocco Stefano.Balocco@gmail.com
//
//  Copyright (c) 2013, Stefano Balocco
//
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in
//       the documentation and/or other materials provided with the distribution.
//     * Neither the name of the author nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//  A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//  PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
using System;

namespace SRC.Math.PRNG
{
	public class ARC4 : SRC.Math.PRNG.Random
	{
		private short arc4_i = 0;
		private short arc4_j = 0;

		private byte[ ] S =
		{
			000,	001,	002,	003,	004,	005,	006,	007,
			008,	009,	010,	011,	012,	013,	014,	015,
			016,	017,	018,	019,	020,	021,	022,	023,
			024,	025,	026,	027,	028,	029,	030,	031,
			032,	033,	034,	035,	036,	037,	038,	039,
			040,	041,	042,	043,	044,	045,	046,	047,
			048,	049,	050,	051,	052,	053,	054,	055,
			056,	057,	058,	059,	060,	061,	062,	063,
			064,	065,	066,	067,	068,	069,	070,	071,
			072,	073,	074,	075,	076,	077,	078,	079,
			080,	081,	082,	083,	084,	085,	086,	087,
			088,	089,	090,	091,	092,	093,	094,	095,
			096,	097,	098,	099,	100,	101,	102,	103,
			104,	105,	106,	107,	108,	109,	110,	111,
			112,	113,	114,	115,	116,	117,	118,	119,
			120,	121,	122,	123,	124,	125,	126,	127,
			128,	129,	130,	131,	132,	133,	134,	135,
			136,	137,	138,	139,	140,	141,	142,	143,
			144,	145,	146,	147,	148,	149,	150,	151,
			152,	153,	154,	155,	156,	157,	158,	159,
			160,	161,	162,	163,	164,	165,	166,	167,
			168,	169,	170,	171,	172,	173,	174,	175,
			176,	177,	178,	179,	180,	181,	182,	183,
			184,	185,	186,	187,	188,	189,	190,	191,
			192,	193,	194,	195,	196,	197,	198,	199,
			200,	201,	202,	203,	204,	205,	206,	207,
			208,	209,	210,	211,	212,	213,	214,	215,
			216,	217,	218,	219,	220,	221,	222,	223,
			224,	225,	226,	227,	228,	229,	230,	231,
			232,	233,	234,	235,	236,	237,	238,	239,
			240,	241,	242,	243,	244,	245,	246,	247,
			248,	249,	250,	251,	252,	253,	254,	255
		};

		private void NextAndSwap( )
		{
			arc4_i = (short) ( ++arc4_i % 256 );
			arc4_j = (short) ( ( arc4_j + S[ arc4_i ] ) % 256 );
			S.Swap( arc4_i, arc4_j );
		}

		public ARC4( byte[] seed, int skip )
		{
			if( null == seed )
			{
				throw new ArgumentNullException( "seed" );
			}
			else if( ( 5 > seed.Length ) || ( 256 < seed.Length ) )
			{
				throw new ArgumentException( String.Format( "seed is too small or too big ({0} bytes), it should be between {1} and {2} bytes long.", seed.Length, 5, 256 ), "seed" );
			}
			int j = 0;
			for( int i = 0; i < 256; i++ )
			{
				j = ( j + S[ i ] + seed[ i % seed.Length ] ) % 256;
				S.Swap( i, j);
			}
			if( skip > 0 )
			{
				for( int k = 0; k < skip; k++ )
				{
					NextAndSwap( );
				}
			}
		}

		public ARC4( byte[] seed ) : this( seed, 0 )
		{
		}

		public override void NextBytes( byte[] buffer )
		{
			for( int i = 0; i < buffer.Length; i++ )
			{
				NextAndSwap( );
				buffer[ i ] = (byte) ( S[ ( ( S[ arc4_i ] + S[ arc4_j ] ) % 256 ) ] );
			}
		}
	}
}

