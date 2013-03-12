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
using System.Security.Cryptography;
using SRC;

namespace SRC.Cryptography
{
	public sealed class XXTEA : SymmetricAlgorithm
	{
		public XXTEA( )
		{
			LegalBlockSizesValue = new KeySizes[ 1 ];
			LegalBlockSizesValue[ 0 ] = new KeySizes( 64, 416, 32 );

			LegalKeySizesValue = new KeySizes[ 1 ];
			LegalKeySizesValue[ 0 ] = new KeySizes( 128, 128, 0 );

			KeySizeValue = 128;
			BlockSizeValue = 64;
			FeedbackSize = BlockSizeValue;
		}

		public static new XXTEA Create( )
		{
			return new XXTEA( );
		}

		public override void GenerateIV( )
		{
			IVValue = KeyBuilder.IV( BlockSizeValue >> 3 );
		}

		public override void GenerateKey()
		{
			KeyValue = KeyBuilder.Key( KeySizeValue >> 3 );
		}

		public override ICryptoTransform CreateDecryptor( byte[ ] rgbKey, byte[ ] rgbIV )
		{
			return new XXTEATransform( this, false, rgbKey, rgbIV );
		}

		public override ICryptoTransform CreateEncryptor( byte[ ] rgbKey, byte[ ] rgbIV )
		{
			return new XXTEATransform( this, true, rgbKey, rgbIV );
		}
	}

	internal class XXTEATransform : SymmetricTransform
	{
		private static UInt32 Delta = 0x9e3779b9;

		private static UInt32 MX( int j, UInt32[] key, UInt32 e, UInt32 sum, UInt32 y, UInt32 z )
		{
			return ( ( ( z >> 5 ) ^ ( y << 2 ) ) + ( ( y >> 3 ) ^ ( z << 4 ) ) ^ ( sum ^ y ) + ( key[ ( j & 3 ) ^ e ] ^ z ) );
		}

		private static UInt32[] Encrypt( UInt32[] key, UInt32[] data )
		{
			if( data.Length < 2 )
			{
				return data;
			}
			if( key.Length < 4 )
			{
				UInt32[] newKey = new UInt32[ 4 ];
				key.CopyTo( newKey, 0 );
				key = newKey;
			}
			UInt32 sum = 0;
			UInt32 z = data[ data.Length - 1 ];
			for( int i = 0; i < ( 6 + ( 52 / data.Length ) ); i++ )
			{
				sum = unchecked( sum + XXTEATransform.Delta );
				UInt32 e = ( sum >> 2 ) & 3;
				for( int j = 0; j < data.Length - 1; j++ )
				{
					z = unchecked( data[ j ] += XXTEATransform.MX( j, key, e, sum, data[ j + 1 ], z ) );
				}
				z = unchecked( data[ data.Length - 1 ] += XXTEATransform.MX( data.Length - 1, key, e, sum, data[ 0 ], z ) );
			}
			return data;
		}

		private static UInt32[] Decrypt( UInt32[] key, UInt32[] data )
		{
			if( data.Length < 2 )
			{
				return data;
			}
			if( key.Length < 4 )
			{
				UInt32[] newKey = new UInt32[ 4 ];
				key.CopyTo( newKey, 0 );
				key = newKey;
			}
			for( UInt32 sum = unchecked( (UInt32) ( ( 6 + 52 / ( data.Length ) ) * XXTEATransform.Delta ) ); sum != 0; sum = unchecked(sum - XXTEATransform.Delta ) )
			{
				int y = 0;
				UInt32 e = ( sum >> 2 ) & 3;
				for( int j = data.Length -1; j > 0; j-- )
				{
					data[ j ] = unchecked( data[ j ] - XXTEATransform.MX( j, key, e, sum, data[ y ], data[ j - 1 ] ) );
					y = j;
				}
				data[ 0 ] = unchecked( data[ 0 ] - XXTEATransform.MX( 0, key, e, sum, data[ 1 ], data[ data.Length -1 ] ) );
			}
			return data;
		}

		private UInt32[] key;

		public XXTEATransform( XXTEA symmAlgo, bool encrypt, byte[] key, byte[] iv ) : base( symmAlgo, encrypt, iv )
		{
			if( null == key )
			{
				throw new CryptographicException( "Key is null" );
			}
			else if( 16 != key.Length )
			{
				throw new CryptographicException( String.Format( "Key is too small ({0} bytes), it should be {1} bytes long.", key.Length, 16 ) );
			}
			this.key = key.ToUInt32Array( );
		}

		protected override void ECB( byte[] input, byte[] output )
		{
			byte[] transformOutput = new byte[ output.Length ];
			if( encrypt )
			{
				transformOutput = XXTEATransform.Encrypt( this.key, input.ToUInt32Array( ) ).ToByteArray( );
			}
			else
			{
				transformOutput = XXTEATransform.Decrypt( this.key, input.ToUInt32Array( ) ).ToByteArray( );
			}
			Array.Copy( transformOutput, output, output.Length );
		}
	}
}

