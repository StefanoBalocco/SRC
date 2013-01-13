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
using SRC.Math;

namespace SRC.Cryptography
{
	public struct DiffieHellmanMerkle
	{
		private static BigInteger prime;
		public static BigInteger Prime
		{
			get
			{
				if( null == prime )
				{
					// Source for the prime number: http://www.alpertron.com.ar/glpxm1.pl?digits=327
					prime = new BigInteger( "512760658271029132363555082556345686327932155015477028179108119504766444454819802587788422315656334405023492181360800223899332195287570357322692871093750000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001", 10 );
				}
				return prime;
			}
		}
		private static BigInteger generator;
		public static BigInteger Generator
		{
			get
			{
				if( null == generator )
				{
					// Generator calculated with Pari-gp
					generator = new BigInteger( "7" );
				}
				return generator;
			}
		}

		private BigInteger privateKey;
		private BigInteger PrivateKey
		{
			get
			{
				if( null == privateKey )
				{
					privateKey = BigInteger.ProbablePrime( 512  );
					System.Console.Out.WriteLine( privateKey.ToString() );
				}
				return privateKey;
			}
		}

		private BigInteger gamodP;
		public BigInteger GamodP
		{
			get
			{
				if( null == gamodP )
				{
					gamodP = DiffieHellmanMerkle.Generator.ModPow( PrivateKey, DiffieHellmanMerkle.Prime );
				}
				return gamodP;
			}
		}

		private BigInteger gbmodP;
		public BigInteger GbmodP
		{
			set
			{
				if( null != value )
				{
					gbmodP = value;
				}
			}
		}

		private BigInteger secret;
		public BigInteger Secret
		{
			get
			{
				if( ( null != gbmodP ) && ( null == secret ) )
				{
					secret = gbmodP.ModPow( PrivateKey, DiffieHellmanMerkle.Prime );
				}
				return secret;
			}
		}
	}
	public class test
	{
		public static void Main()
		{
			DiffieHellmanMerkle a = new DiffieHellmanMerkle();
			DiffieHellmanMerkle b = new DiffieHellmanMerkle();
			System.Console.Out.WriteLine( DiffieHellmanMerkle.Prime.ToString() );
			System.Console.Out.WriteLine( DiffieHellmanMerkle.Generator.ToString() );
			System.Console.Out.WriteLine( a.GamodP.ToString() );
			System.Console.Out.WriteLine( b.GamodP.ToString() );
			a.GbmodP = b.GamodP;
			b.GbmodP = a.GamodP;
			System.Console.Out.WriteLine( a.Secret.Equals( b.Secret ) );
			System.Console.Out.WriteLine( a.Secret.ToString() );
			System.Console.Out.WriteLine( b.Secret.ToString() );
		}
	}

}

