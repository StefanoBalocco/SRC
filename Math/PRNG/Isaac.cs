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
using System.Collections.Generic;

namespace SRC.Math.PRNG
{
	public class Isaac : System.Security.Cryptography.RandomNumberGenerator
	{
		private const int SIZEL = 8;				/* log of size of rsl[] and mem[] */
		private const int SIZE = 1 << SIZEL;		/* size of rsl[] and mem[] */
		private const int MASK = ( SIZE - 1 ) << 2;	/* for pseudorandom lookup */
		private int count;							/* count through the results in rsl[] */
		private int[] rsl;							/* the results given to the user */
		private int[] mem;							/* the internal state */
		private int a;								/* accumulator */
		private int b;								/* the last result */
		private int c;								/* counter, guarantees cycle is at least 2^^40 */
		private IList<byte> buffer;					/* Buffer for the last result */

		/* initialize, or reinitialize, this instance of rand */
		public void RngInitialize( byte[] seed )
		{
			mem = new int[SIZE];
			rsl = new int[SIZE];
			bool seeded = false;
			int i;
			int a, b, c, d, e, f, g, h;
			if( null != seed )
			{
				if( ( seed.Length > SIZE ) || ( 0 != ( seed.Length % 4 ) ) )
				{
					throw new ArgumentException( String.Format( "seed is too big ({0} bytes) or the byte number isn't a 4 multiple, it should be between 0 and {1} bytes long, with 4 byte increments.", seed.Length, SIZE * 4 ), "seed" );
				}
				else
				{
					for( i = 0; ( i < ( seed.Length / 4 ) ) && ( i < SIZE ); ++i )
					{
						rsl[ i ] = seed.ToInt( i );
					}
					seeded = true;
				}
			}
			/* the golden ratio */
			a = b = c = d = e = f = g = h = unchecked( ( int ) 0x9e3779b9 );

			for( i = 0; i < 4; ++i )
			{
				a ^= b << 11;
				d += a;
				b += c;
				b ^= ( int )( ( uint ) c >> 2 );
				e += b;
				c += d;
				c ^= d << 8;
				f += c;
				d += e;
				d ^= ( int ) ( ( uint ) e >> 16 );
				g += d;
				e += f;
				e ^= f << 10;
				h += e;
				f += g;
				f ^= ( int ) ( ( uint ) g >> 4 );
				a += f;
				g += h;
				g ^= h << 8;
				b += g;
				h += a;
				h ^= ( int ) ( ( uint ) a >> 9 );
				c += h;
				a += b;
			}

			/* fill in mem[] with messy stuff */
			for( i = 0; i < SIZE; i += 8 )
			{
				if( seeded )
				{
					a += rsl[ i ];
					b += rsl[ i + 1 ];
					c += rsl[ i + 2 ];
					d += rsl[ i + 3 ];
					e += rsl[ i + 4 ];
					f += rsl[ i + 5 ];
					g += rsl[ i + 6 ];
					h += rsl[ i + 7 ];
				}
				a ^= b << 11;
				d += a;
				b += c;
				b ^= ( int ) ( ( uint ) c >> 2 );
				e += b;
				c += d;
				c ^= d << 8;
				f += c;
				d += e;
				d ^= ( int ) ( ( uint ) e >> 16 );
				g += d;
				e += f;
				e ^= f << 10;
				h += e;
				f += g;
				f ^= ( int ) ( ( uint ) g >> 4 );
				a += f;
				g += h;
				g ^= h << 8;
				b += g;
				h += a;
				h ^= ( int ) ( ( uint ) a >> 9 );
				c += h;
				a += b;
				mem[ i ] = a;
				mem[ i + 1 ] = b;
				mem[ i + 2 ] = c;
				mem[ i + 3 ] = d;
				mem[ i + 4 ] = e;
				mem[ i + 5 ] = f;
				mem[ i + 6 ] = g;
				mem[ i + 7 ] = h;
			}

			/* second pass makes all of seed affect all of mem */
			if( seeded )
			{
				for( i = 0; i < SIZE; i += 8 )
				{
					a += mem[ i ];
					b += mem[ i + 1];
					c += mem[ i + 2 ];
					d += mem[ i + 3 ];
					e += mem[ i + 4 ];
					f += mem[ i + 5 ];
					g += mem[ i + 6 ];
					h += mem[ i + 7 ];
					a ^= b << 11;
					d += a;
					b += c;
					b ^= ( int ) ( ( uint ) c >> 2 );
					e += b;
					c += d;
					c ^= d << 8;
					f += c;
					d += e;
					d ^= ( int ) ( ( uint ) e >> 16 );
					g += d;
					e += f;
					e ^= f << 10;
					h += e;
					f += g;
					f ^= ( int ) ( ( uint ) g >> 4 );
					a += f;
					g += h;
					g ^= h << 8;
					b += g;
					h += a;
					h ^= ( int ) ( ( uint ) a >> 9 );
					c += h;
					a += b;
					mem[ i ] = a;
					mem[ i + 1 ] = b;
					mem[ i + 2 ] = c;
					mem[ i + 3 ] = d;
					mem[ i + 4 ] = e;
					mem[ i + 5 ] = f;
					mem[ i + 6 ] = g;
					mem[ i + 7 ] = h;
				}
			}
			Generate( );
			count = SIZE;
		}

		public Isaac( )
		{
			System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider( );
			byte[ ] seed = new byte[ SIZE ];
			rng.GetBytes( seed );
			RngInitialize( seed );
		}

		public Isaac( byte[ ] rgb )
		{
			RngInitialize( rgb );
		}

		public Isaac( string str ) : this( ( null != str ) ? System.Text.Encoding.UTF8.GetBytes( str ) : null )
		{
		}

		/* Generate 256 results.  This is a fast (not small) implementation. */
		private void Generate( )
		{
			int i, j, x, y;

			b += ++c;
			for( i = 0, j = SIZE / 2; i < SIZE / 2; )
			{
				x = mem[ i ];
				a ^= a << 13;
				a += mem[ j++ ];
				mem[ i ] = y = mem[ ( x & MASK ) >> 2 ] + a + b;
				rsl[ i++ ] = b = mem[ ( ( y >> SIZEL ) & MASK ) >> 2 ] + x;

				x = mem[ i ];
				a ^= ( int ) ( ( uint ) a >> 6 );
				a += mem[ j++ ];
				mem[ i ] = y = mem[ ( x & MASK ) >> 2 ] + a + b;
				rsl[ i++ ] = b = mem[ ( ( y >> SIZEL ) & MASK ) >> 2 ] + x;

				x = mem[ i ];
				a ^= a << 2;
				a += mem[ j++ ];
				mem[ i ] = y = mem[ ( x & MASK ) >> 2 ] + a + b;
				rsl[ i++ ] = b = mem[ ( ( y >> SIZEL ) & MASK ) >> 2 ] + x;

				x = mem[ i ];
				a ^= ( int ) ( ( uint ) a >> 16 );
				a += mem[ j++ ];
				mem[ i ] = y = mem[ ( x & MASK ) >> 2 ] + a + b;
				rsl[ i++ ] = b = mem[ ( ( y >> SIZEL ) & MASK ) >> 2 ] + x;
			}
			for( j = 0; j < SIZE / 2; )
			{
				x = mem[ i ];
				a ^= a << 13;
				a += mem[ j++ ];
				mem[ i ] = y = mem[ ( x & MASK ) >> 2 ] + a + b;
				rsl[ i++ ] = b = mem[ ( ( y >> SIZEL ) & MASK ) >> 2 ] + x;

				x = mem[ i ];
				a ^= ( int ) ( ( uint ) a >> 6 );
				a += mem[ j++ ];
				mem[ i ] = y = mem[ ( x & MASK ) >> 2 ] + a + b;
				rsl[ i++ ] = b = mem[ ( ( y >> SIZEL ) & MASK ) >> 2 ] + x;

				x = mem[ i ];
				a ^= a << 2;
				a += mem[ j++ ];
				mem[ i ] = y = mem[ ( x & MASK ) >> 2 ] + a + b;
				rsl[ i++ ] = b = mem[ ( ( y >> SIZEL ) & MASK ) >> 2 ] + x;

				x = mem[ i ];
				a ^= ( int ) ( ( uint ) a >> 16 );
				a += mem[ j++ ];
				mem[ i ] = y = mem[ ( x & MASK ) >> 2 ] + a + b;
				rsl[ i++ ] = b = mem[ ( ( y >> SIZEL ) & MASK ) >> 2 ] + x;
			}
		}

		public override void GetBytes( byte[] bytes )
		{
			if( null == buffer )
			{
				buffer = new List<byte>( );
			}
			if( bytes.Length > buffer.Count )
			{
				while( bytes.Length > buffer.Count )
				{
					if( 0 == count-- )
					{
						Generate( );
						count = SIZE-1;
					}
					buffer.AddAll( BitConverter.GetBytes( rsl[ count ] ) );
				}
				for( int i = 0; i < bytes.Length; i++ )
				{
					bytes[ i ] = buffer[ 0 ];
					buffer.RemoveAt( 0 );
				}
			}
		}

		public override void GetNonZeroBytes( byte[] bytes )
		{
			for( int i = 0; i < bytes.Length; i++ )
			{
				byte[ ] tmpVariable = new byte[ 1 ] { 0 };
				while( 0 == tmpVariable[ 0 ] )
				{
					GetBytes( tmpVariable );
				}
				bytes[ i ] = tmpVariable[ 0 ];
			}
		}
	}
}

