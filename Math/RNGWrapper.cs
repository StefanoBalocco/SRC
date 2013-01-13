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
using SRC;

namespace SRC.Math
{
	public class RNGWrapper : Singleton<RNGWrapper>
	{
		private System.Security.Cryptography.RandomNumberGenerator random = System.Security.Cryptography.RandomNumberGenerator.Create(  );

		public int Next()
		{
			byte[] bytes = new byte[ 4 ];
			NextBytes( bytes );
			return System.Math.Abs( BitConverter.ToInt32( bytes, 0 ) );
		}

		public int Next( int maximum )
		{
			return Next() % maximum;
		}

		public int Next( int minimum, int maximum )
		{
			return minimum + Next( maximum - minimum );
		}

		public void NextBytes( byte[] bytes )
		{
			random.GetBytes( bytes );
		}

		public byte NextByte( )
		{
			byte[] returnValue = new byte[ 1 ];
			random.GetBytes( returnValue );
			return returnValue[ 0 ];
		}

		public byte NextByte( byte maximum )
		{
			return (byte) ( NextByte() % maximum );
		}

		public byte NextByte( byte minimum, byte maximum )
		{
			return (byte) ( minimum + NextByte( (byte) ( maximum - minimum ) ) );
		}

		public double NextDouble()
		{
			byte[] bytes = new byte[ 8 ];
			NextBytes( bytes );
			return BitConverter.ToDouble( bytes, 0 );
		}

		public BigInteger GetBytes( int size )
		{
			
			byte[] bytes = new byte[ size ];
			NextBytes( bytes );
			return new BigInteger( bytes );
		}
	}
}

