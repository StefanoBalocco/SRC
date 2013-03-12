/*
 * This implementation of Salsa20 is ported from the reference implementation
 * by D. J. Bernstein, which can be found at:
 *   http://cr.yp.to/snuffle/salsa20/ref/salsa20.c
 *
 * This work is hereby released into the Public Domain. To view a copy of the public domain dedication,
 * visit http://creativecommons.org/licenses/publicdomain/ or send a letter to
 * Creative Commons, 171 Second Street, Suite 300, San Francisco, California, 94105, USA.
 */

using System;
using System.Security.Cryptography;

namespace SRC.Cryptography
{
	/// <summary>
	/// Implements the Salsa20 stream encryption cipher, as defined at http://cr.yp.to/snuffle.html.
	/// </summary>
	/// <remarks>See <a href="http://code.logos.com/blog/2008/06/salsa20_implementation_in_c_1.html">Salsa20 Implementation in C#</a>.</remarks>
	public abstract class Salsa20 : SymmetricAlgorithm
	{
		internal abstract short Rounds
		{
			get;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Salsa20"/> class.
		/// </summary>
		/// <exception cref="CryptographicException">The implementation of the class derived from the symmetric algorithm is not valid.</exception>
		public Salsa20( )
		{
			// set legal values
			LegalBlockSizesValue = new KeySizes[ 1 ];
			LegalBlockSizesValue[ 0 ] = new KeySizes( 512, 512, 0 );

			LegalKeySizesValue = new KeySizes[ 1 ];
			// 128 key + 64 nonce or 256 key + 64 nonce
			LegalKeySizesValue[ 0 ] = new KeySizes( 192, 320, 128 );

			// set default values
			KeySizeValue = 320;
			BlockSizeValue = 512;
			FeedbackSize = BlockSizeValue;
		}

		/// <summary>
		/// Generates a random initialization vector (<see cref="SymmetricAlgorithm.IV"/>) to use for the algorithm.
		/// </summary>
		public override void GenerateIV( )
		{
			IVValue = KeyBuilder.IV( BlockSize >> 3 );
		}

		/// <summary>
		/// Generates a random key (<see cref="SymmetricAlgorithm.Key"/>) to use for the algorithm.
		/// </summary>
		public override void GenerateKey( )
		{
			KeyValue = KeyBuilder.Key( KeySizeValue >> 3 );
		}

		/// <summary>
		/// Creates a symmetric decryptor object with the specified <see cref="SymmetricAlgorithm.Key"/> property
		/// and initialization vector (<see cref="SymmetricAlgorithm.IV"/>).
		/// </summary>
		/// <param name="rgbKey">The secret key to use for the symmetric algorithm.</param>
		/// <param name="rgbIV">The initialization vector to use for the symmetric algorithm.</param>
		/// <returns>A symmetric decryptor object.</returns>
		public override ICryptoTransform CreateDecryptor( byte[] rgbKey, byte[] rgbIV )
		{
			return new Salsa20Transform( this, false, rgbKey, rgbIV, Rounds );
		}

		/// <summary>
		/// Creates a symmetric encryptor object with the specified <see cref="SymmetricAlgorithm.Key"/> property
		/// and initialization vector (<see cref="SymmetricAlgorithm.IV"/>).
		/// </summary>
		/// <param name="rgbKey">The secret key to use for the symmetric algorithm.</param>
		/// <param name="rgbIV">The initialization vector to use for the symmetric algorithm.</param>
		/// <returns>A symmetric encryptor object.</returns>
		public override ICryptoTransform CreateEncryptor( byte[] rgbKey, byte[] rgbIV )
		{
			return new Salsa20Transform( this, true, rgbKey, rgbIV, Rounds );
		}
	}

	public sealed class Salsa20_08 : Salsa20
	{
		internal override short Rounds
		{
			get
			{
				return 8;
			}
		}

		public static new Salsa20 Create( )
		{
			return new Salsa20_08( );
		}
	}

	public sealed class Salsa20_12 : Salsa20
	{
		internal override short Rounds
		{
			get
			{
				return 12;
			}
		}

		public static new Salsa20 Create( )
		{
			return new Salsa20_12( );
		}
	}

	public sealed class Salsa20_20 : Salsa20
	{
		internal override short Rounds
		{
			get
			{
				return 20;
			}
		}

		public static new Salsa20 Create( )
		{
			return new Salsa20_20( );
		}
	}

	/// <summary>
	/// Salsa20Transform is an implementation of <see cref="ICryptoTransform"/> that uses the Salsa20 algorithm.
	/// </summary>
	internal class Salsa20Transform : SymmetricTransform
	{
		private static readonly byte[] c_sigma = 
		{
			0x65,	0x78,	0x70,	0x61,
			0x6e,	0x64,	0x20,	0x33,
			0x32,	0x2d,	0x62,	0x79,
			0x74,	0x65,	0x20,	0x6b
		};
		private static readonly byte[] c_tau =
		{
			0x65,	0x78,	0x70,	0x61,
			0x6e,	0x64,	0x20,	0x31,
			0x36,	0x2d,	0x62,	0x79,
			0x74,	0x65,	0x20,	0x6b
		};

		private static uint Rotate(uint v, int c)
		{
			return (v << c) | (v >> (32 - c));
		}

		private static uint Add(uint v, uint w)
		{
			return unchecked(v + w);
		}

		private static uint AddOne(uint v)
		{
			return unchecked(v + 1);
		}

		private short rounds;
		private byte[] key;
		private byte[] salsaIV;
		private uint[] m_state;

		public Salsa20Transform( Salsa20 symmAlgo, bool encrypt, byte[] rgbKey, byte[] rgbIV, short rounds ) : base( symmAlgo, encrypt, rgbIV )
		{
			if( null == rgbKey )
			{
				throw new CryptographicException( "Key is null" );
			}
			else if( ( 24 != rgbKey.Length ) && ( 40 != rgbKey.Length ) )
			{
				throw new CryptographicException( String.Format( "Key is too small ({0} bytes), it should be {1} or {2} bytes long.", rgbKey.Length, 24, 40 ) );
			}
			if( ( 8 != rounds ) && ( 12 != rounds ) && ( 20 != rounds ) )
			{
				throw new CryptographicException( String.Format( "Invalid number of rounds ({0}), it should be {1}, {2} or {3}.", rounds, 8, 12, 20 ) );
			}
			key = rgbKey.SubArray( 0, rgbKey.Length - 8 );
			salsaIV = rgbKey.SubArray( rgbKey.Length - 8, 8 );
			this.rounds = rounds;

			m_state = new UInt32[ 16 ];
			m_state[1] = key.ToUInt32( 0 );
			m_state[2] = key.ToUInt32( 4 );
			m_state[3] = key.ToUInt32( 8 );
			m_state[4] = key.ToUInt32( 12 );

			byte[] constants = ( key.Length == 32 ? Salsa20Transform.c_sigma : Salsa20Transform.c_tau );
			int keyIndex = key.Length - 16;

			m_state[11] = key.ToUInt32( keyIndex );
			m_state[12] = key.ToUInt32( keyIndex + 4);
			m_state[13] = key.ToUInt32( keyIndex + 8);
			m_state[14] = key.ToUInt32(keyIndex + 12);
			m_state[0] = constants.ToUInt32(0);
			m_state[5] = constants.ToUInt32(4);
			m_state[10] = constants.ToUInt32(8);
			m_state[15] = constants.ToUInt32( 12 );

			m_state[6] = salsaIV.ToUInt32( 0);
			m_state[7] = salsaIV.ToUInt32( 4);
			m_state[8] = 0;
			m_state[9] = 0;
		}

		private void Hash( byte[] output, uint[] input )
		{
			uint[] state = (uint[]) input.Clone();

			for (int round = this.rounds; round > 0; round -= 2)
			{
				state[4] ^= Rotate(Add(state[0], state[12]), 7);
				state[8] ^= Rotate(Add(state[4], state[0]), 9);
				state[12] ^= Rotate(Add(state[8], state[4]), 13);
				state[0] ^= Rotate(Add(state[12], state[8]), 18);
				state[9] ^= Rotate(Add(state[5], state[1]), 7);
				state[13] ^= Rotate(Add(state[9], state[5]), 9);
				state[1] ^= Rotate(Add(state[13], state[9]), 13);
				state[5] ^= Rotate(Add(state[1], state[13]), 18);
				state[14]  ^= Rotate(Add(state[10], state[6]), 7);
				state[2] ^= Rotate(Add(state[14], state[10]), 9);
				state[6] ^= Rotate(Add(state[2], state[14]), 13);
				state[10]  ^= Rotate(Add(state[6], state[2]), 18);
				state[3] ^= Rotate(Add(state[15], state[11]), 7);
				state[7] ^= Rotate(Add(state[3], state[15]), 9);
				state[11]  ^= Rotate(Add(state[7], state[3]), 13);
				state[15]  ^= Rotate(Add(state[11], state[7]), 18);
				state[1] ^= Rotate(Add(state[0], state[3]), 7);
				state[2] ^= Rotate(Add(state[1], state[0]), 9);
				state[3] ^= Rotate(Add(state[2], state[1]), 13);
				state[0] ^= Rotate(Add(state[3], state[2]), 18);
				state[6] ^= Rotate(Add(state[5], state[4]), 7);
				state[7] ^= Rotate(Add(state[6], state[5]), 9);
				state[4] ^= Rotate(Add(state[7], state[6]), 13);
				state[5] ^= Rotate(Add(state[4], state[7]), 18);
				state[11]  ^= Rotate(Add(state[10], state[9]), 7);
				state[8] ^= Rotate(Add(state[11], state[10]), 9);
				state[9] ^= Rotate(Add(state[8], state[11]), 13);
				state[10]  ^= Rotate(Add(state[9], state[8]), 18);
				state[12]  ^= Rotate(Add(state[15], state[14]), 7);
				state[13]  ^= Rotate(Add(state[12], state[15]), 9);
				state[14]  ^= Rotate(Add(state[13], state[12]), 13);
				state[15]  ^= Rotate(Add(state[14], state[13]), 18);
			}

			for (int index = 0; index < 16; index++)
			{
				Array.Copy( Add( state[index], input[index] ).ToByteArray( ), 0, output, 4 * index, 4 );
			}
		}

		protected override void ECB( byte[] input, byte[] output )
		{
			byte[] internalBuffer = new byte[64];
			Hash( internalBuffer, m_state );
			m_state[8] = AddOne( m_state[8] );
			if( 0 == m_state[8] )
			{
				// NOTE: stopping at 2^70 bytes per nonce is user's responsibility
				m_state[9] = AddOne( m_state[ 9 ] );
			}
			int blockSize = System.Math.Min( algo.BlockSize, input.Length );
			for( int i = 0; i < blockSize; i++ )
			{
				output[ i ] = (byte) ( input[ i ] ^ internalBuffer[ i ] );
			}
		}
		/*
		public void Dispose()
		{
			if( null != m_state )
			{
				Array.Clear(m_state, 0, m_state.Length);
			}
			m_state = null;
			base.Dispose( true );
		}*/
	}
}