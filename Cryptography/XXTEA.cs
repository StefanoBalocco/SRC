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

namespace eHTTPtunnel.Common.Cryptography
{
	//TODO: implement XXTEA as System.Security.Cryptography.SymmetricAlgorithm
	public sealed class XXTEA : SymmetricAlgorithm
	{
		public XXTEA( )
		{
			KeySizeValue = 128;
			BlockSizeValue = 64;
			FeedbackSize = 64;

			LegalBlockSizesValue = new KeySizes[ 1 ];
			LegalBlockSizesValue[ 0 ] = new KeySizes( 64, 416, 32 );

			LegalKeySizesValue = new KeySizes[ 1 ];
			LegalKeySizesValue[ 0 ] = new KeySizes( 128, 128, 0 );
		}

		public static new XXTEA Create( )
		{
			return Create( "eHTTPtunnel.Common.Cryptography.XXTEA" );
		}

		public static new XXTEA Create( string name )
		{
			return ( (XXTEA) CryptoConfig.CreateFromName( name ) );
		}

		public override void GenerateIV( )
		{
			IVValue = KeyBuilder.IV( 416 );
		}

		public override void GenerateKey()
		{
			KeyValue = KeyBuilder.Key( 128 );
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

	internal class XXTEATransform : ICryptoTransform
	{
		public XXTEATransform( XXTEA algorithm, bool encryption, byte[] key, byte[] iv )
		{
			if( null == key )
			{
				throw new CryptographicException( "Key is null" );
			}
		}
		#region IDisposable implementation
		void IDisposable.Dispose ()
		{
			throw new System.NotImplementedException ();
		}
		#endregion

		#region ICryptoTransform implementation
		int ICryptoTransform.TransformBlock (byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
		{
			throw new System.NotImplementedException ();
		}

		byte[] ICryptoTransform.TransformFinalBlock (byte[] inputBuffer, int inputOffset, int inputCount)
		{
			throw new System.NotImplementedException ();
		}

		bool ICryptoTransform.CanReuseTransform {
			get {
				throw new System.NotImplementedException ();
			}
		}

		bool ICryptoTransform.CanTransformMultipleBlocks {
			get {
				throw new System.NotImplementedException ();
			}
		}

		int ICryptoTransform.InputBlockSize {
			get {
				throw new System.NotImplementedException ();
			}
		}

		int ICryptoTransform.OutputBlockSize {
			get {
				throw new System.NotImplementedException ();
			}
		}
		#endregion
	}
}

