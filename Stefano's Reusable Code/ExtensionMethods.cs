//
//  Author:
//    Stefano Balocco Stefano.Balocco@gmail.com
//
//  Copyright (c) 2011-2013, Stefano Balocco
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
using System.Runtime.Remoting.Metadata.W3cXsd2001;

#if !__MonoCS__
namespace System.Runtime.CompilerServices
{
	[ AttributeUsage( AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly ) ]
	public sealed class ExtensionAttribute : Attribute
	{
	}
}
#endif

namespace SRC
{
	public static class ExtensionMethods
	{
		private static int bytesInUint32 = 4;
		#region Checksums
		public static byte[] Adler32( this byte[] source, int offset, int length )
		{
			return Checksum.Adler32( source, offset, length );
		}

		public static byte[] Crc32( this byte[] source, int offset, int length )
		{
			return Checksum.Crc32( source, offset, length );
		}
		#endregion

		public static void AddAll<T>( this IList<T> source, T[] data )
		{
			for( int i = 0; i < data.Length; i++ )
			{
				source.Add( data[ i ] );
			}
		}

		public static T[] ArrayAppend<T>( this T[] source, T[] data )
		{
			T[] returnValue = new T[ source.Length + data.Length ];
			Array.ConstrainedCopy( source, 0, returnValue, 0, source.Length );
			Array.ConstrainedCopy( data, 0, returnValue, source.Length, data.Length );
			return returnValue;
		}

		public static T[] ArrayPrepend<T>( this T[] source, T[] data )
		{
			T[] returnValue = new T[ source.Length + data.Length ];
			Array.ConstrainedCopy( source, 0, returnValue, data.Length, source.Length );
			Array.ConstrainedCopy( data, 0, returnValue, 0, data.Length );
			return returnValue;
		}

		public static IList<int> GetFactors( this int intValue )
		{
			IList<int> returnValue = new List<int>();
			for( int i = intValue - 1; 0 < i; i-- )
			{
				if( 0 == ( intValue % i ) )
				{
					returnValue.Add( i );
				}
			}
			return returnValue;
		}

		public static byte[] PadBytes( this byte[] bytes, int bits )
		{
			byte[] returnValue = new byte[ bits / 8 ];
			int padValue = bytes.Length - ( bits / 8 );
			if( 0 < padValue )
			{
				for( int i = 0; i < padValue; i++ )
				{
					Buffer.SetByte( returnValue, i, 0 );
				}
			}
			Buffer.BlockCopy( bytes, 0, returnValue, 0 + padValue, bytes.Length );
			return returnValue;
		}

		public static T[] Reverse<T>( this T[] source )
		{
			T[] returnValue = new T[ source.Length ];
			Array.Copy( source, 0, returnValue, 0, source.Length );
			Array.Reverse( returnValue );
			return returnValue;
		}

		public static T[] SubArray<T>( this T[] source, int start, int length )
		{
			T[] returnValue = null;
			if( length > 0 )
			{
				returnValue = new T[ length ];
				returnValue.Initialize(  );
				if( source.Length < ( start + length - 1 ) )
				{
					length = source.Length - start;
				}
				Array.Copy( source, start, returnValue, 0, length );
			}
			return returnValue;
		}

		public static void Swap( this byte[] source, int first, int second )
		{
			source[ first ] ^= source[ second ];
			source[ second ] ^= source[ first ];
			source[ first ] ^= source[ second ];
		}

		public static byte[] ToByteArray( this UInt32 source )
		{
			return BitConverter.GetBytes( source );
		}

		public static byte[] ToByteArray( this UInt32[] source )
		{
			byte[] returnValue = new byte[ 4 * source.Length ];
			for( int i = 0; i < source.Length; i++ )
			{
				Array.Copy( source[ i ].ToByteArray( ), 0, returnValue, i * 4, 4 );
			}
			return returnValue;
		}

		public static UInt32[] ToUInt32Array( this byte[] source )
		{
			UInt32[] returnValue = new UInt32[ source.Length / ExtensionMethods.bytesInUint32 ];

			for( int i = 0; i < source.Length; i += 4 )
			{
				returnValue[ i / ExtensionMethods.bytesInUint32 ] = source.ToUInt32( i );
			}
			return returnValue;
		}

		public static UInt32 ToUInt32( this byte[] source, int position )
		{
			return( (UInt32) (
				source[ position++ ] |
				( source[ position++ ] << 8 ) |
				( source[ position++ ] << 16 ) |
				( source[ position ] << 24 ) ) );
		}

		public static int ToInt( this byte[] source, int position )
		{
			return( (int) (
				source[ position++ ] |
				( source[ position++ ] << 8 ) |
				( source[ position++ ] << 16 ) |
				( source[ position ] << 24 ) ) );
		}

		public static string ToHexString( this byte[] source )
		{
			return ( new SoapHexBinary( source ) ).ToString( );
		}

		public static byte[] ToByteArray( this string source )
		{
			return ( SoapHexBinary.Parse( ( ( 0 != ( source.Length % 2 ) ) ? "0" : null ) + source ) ).Value;
		}
	}
}
