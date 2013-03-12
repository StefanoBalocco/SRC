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
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using SRC;
using SRC.Math.PRNG;
using SRC.Cryptography;
using NUnit.Framework;

namespace SRC.Tests
{
	[TestFixture]
	public class Tests
	{
		[Test]
		public static void DHM()
		{
			DiffieHellmanMerkle a = new DiffieHellmanMerkle();
			DiffieHellmanMerkle b = new DiffieHellmanMerkle();
			a.GbmodP = b.GamodP;
			b.GbmodP = a.GamodP;
			Assert.AreEqual( a.Secret, b.Secret );
		}

		[TestCase( "00000000000000000000000000000000", "0000000000000000", Result="ab043705808c5d57" )]
		[TestCase( "0102040810204080fffefcf8f0e0c080", "0000000000000000", Result="d1e78be2c746728a" )]
		[TestCase( "9e3779b99b9773e9b979379e6b695156", "ffffffffffffffff", Result="67ed0ea8e8973fc5" )]
		[TestCase( "0102040810204080fffefcf8f0e0c080", "fffefcf8f0e0c080", Result="8c3707c01c7fccc4" )]
		[TestCase( "ffffffffffffffffffffffffffffffff", "157c13a850ba5e57306d7791", Result="b2601cefb078b772abccba6a" )]
		[TestCase( "9e3779b99b9773e9b979379e6b695156", "157c13a850ba5e57306d7791", Result="579016d143ed6247ac6710dd" )]
		[TestCase( "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "0102040810204080fffefcf8f0e0c080", Result="c0a19f06ebb0d63925aa27f74cc6b2d0" )]
		[TestCase( "9e3779b99b9773e9b979379e6b695156", "0102040810204080fffefcf8f0e0c080", Result="01b815fd2e4894d13555da434c9d868a" )]
		[TestCase( "0102040810204080fffefcf8f0e0c080", "157c13a850ba5e57306d77916fa2c37be1949616", Result="51f0ffeb46012a245e0c6c4fa097db27caec698d" )]
		[TestCase( "9e3779b99b9773e9b979379e6b695156", "690342f45054a708c475c91db77761bc01b815fd2e4894d1", Result="759e5b212ee58be734d610248e1daa1c9d0647d428b4f95a" )]
		[TestCase( "9e3779b99b9773e9b979379e6b695156", "3555da434c9d868a1431e73e73372fc0688e09ce11d00b6fd936a764", Result="8e63ae7d8a119566990eb756f16abf94ff87359803ca12fbaa03fdfb" )]
		[TestCase( "0102040810204080fffefcf8f0e0c080", "db9af3c96e36a30c643c6e97f4d75b7a4b51a40e9d8759e581e3c40b341b4436", Result="5ef1b6e010a2227ba337374b59beffc5263503054745fb513000641e2c7dd107" )]
		public string Test_XXTEA( string key, string input )
		{
			int dataLength = input.ToByteArray( ).Length;
			byte[] output = new byte[ dataLength ];
			using( SymmetricAlgorithm symmetricAlgorithm = XXTEA.Create( ) )
			{
				symmetricAlgorithm.Key = key.ToByteArray( );
				symmetricAlgorithm.Mode = CipherMode.ECB;
				symmetricAlgorithm.Padding = PaddingMode.None;
				symmetricAlgorithm.BlockSize = 8 * dataLength;
				symmetricAlgorithm.IV = new byte[ dataLength ];
				using( ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateEncryptor( ) )
				{
					using( Stream memoryStream = new MemoryStream( input.ToByteArray( ), false ) )
					{
						using( CryptoStream streamEncrypted = new CryptoStream( memoryStream, cryptoTransform, CryptoStreamMode.Read ) )
						{
							streamEncrypted.Read( output, 0, output.Length );
						}
					}
				}
				using( ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateDecryptor( ) )
				{
					using( Stream memoryStream = new MemoryStream( output, false ) )
					{
						byte[] decrypted = new byte[ dataLength ];
						using( CryptoStream streamEncrypted = new CryptoStream( memoryStream, cryptoTransform, CryptoStreamMode.Read ) )
						{
							streamEncrypted.Read( decrypted, 0, decrypted.Length );
						}
						Assert.AreEqual( input.ToByteArray(), decrypted );
					}
				}
			}
			return output.ToHexString( ).ToLower( );
		}

		[TestCase( "Secret", "Attack at dawn", Result="45A01F645FC35B383552544B9BF5" )]
		public string Test_ARC4( string key, string input )
		{
			byte[] output = new byte[ input.Length ];
			using( SymmetricAlgorithm symmetricAlgorithm = SRC.Cryptography.ARC4.Create( ) )
			{
				symmetricAlgorithm.Key = Encoding.ASCII.GetBytes( key );
				symmetricAlgorithm.Mode = CipherMode.ECB;
				symmetricAlgorithm.Padding = PaddingMode.None;
				symmetricAlgorithm.BlockSize = 8;
				symmetricAlgorithm.IV = new byte[ 1 ];
				using( ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateEncryptor( ) )
				{
					using( Stream memoryStream = new MemoryStream( Encoding.ASCII.GetBytes( input ), false ) )
					{
						using( CryptoStream streamEncrypted = new CryptoStream( memoryStream, cryptoTransform, CryptoStreamMode.Read ) )
						{
							streamEncrypted.Read( output, 0, output.Length );
						}
					}
				}
				using( ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateDecryptor( ) )
				{
					using( Stream memoryStream = new MemoryStream( output, false ) )
					{
						byte[] decrypted = new byte[ input.Length ];
						using( CryptoStream streamEncrypted = new CryptoStream( memoryStream, cryptoTransform, CryptoStreamMode.Read ) )
						{
							streamEncrypted.Read( decrypted, 0, decrypted.Length );
						}
						Assert.AreEqual( Encoding.ASCII.GetBytes( input ), decrypted );
					}
				}
			}
			return output.ToHexString( );
		}

		[TestCase(
			"80000000000000000000000000000000" +
			"0000000000000000",
			"A9C9F888AB552A2D1BBFF9F36BEBEB33" +
			"7A8B4B107C75B63BAE26CB9A235BBA9D" +
			"784F38BEFC3ADF4CD3E266687EA7B9F0" +
			"9BA650AE81EAC6063AE31FF12218DDC5",
			"BB5B6BB2CC8B8A0222DCCC1753ED4AEB" +
			"23377ACCBD5D4C0B69A8A03BB115EF71" +
			"871BC10559080ACA7C68F0DEF32A80DD" +
			"BAF497259BB76A3853A7183B51CC4B9F",
			"4436CDC0BE39559F5E5A6B79FBDB2CAE" +
			"4782910F27FFC2391E05CFC78D601AD8" +
			"CD7D87B074169361D997D1BED9729C0D" +
			"EB23418E0646B7997C06AA84E7640CE3",
			"BEE85903BEA506B05FC04795836FAAAC" +
			"7F93F785D473EB762576D96B4A65FFE4" +
			"63B34AAE696777FC6351B67C3753B89B" +
			"A6B197BD655D1D9CA86E067F4D770220"
		)] // Set 1, vector#  0
		[TestCase(
			"00400000000000000000000000000000" +
			"0000000000000000",
			"EEB20BFB12025D2EE2BF33356644DCEF" +
			"467D377176FA74B3C110377A40CFF1BF" +
			"37EBD52A51750FB04B80C50AFD082354" +
			"9230B006F5994EBAAA521C7788F5E31C",
			"6F397C82EC0D708CBE01F7FFAC0109EE" +
			"E7D2C564046CE22B8F74DF12A111CBED" +
			"9697A492C9147BFBB26613D8FFC29762" +
			"DA009207E2038F7BCE7FBB53BF1D6128",
			"278CD2F0E90E66BCEF73D0FEB66FB5AF" +
			"F2F2083C1B6C462E1F1E6D864F6A7473" +
			"C0988F721AD673C23C4E70DDF67505AC" +
			"017B84DFF1983BD1ED81F8D64C8D9347",
			"EB55A9195DEE506F1C56E99DF24AE40C" +
			"F7F942B577BA241692AC85EACEE58B38" +
			"CE2F05F0E2C492D7FFAA07EF6CA36916" +
			"34BA12B68476C95F583F2723C498A6E7"
		)] // Set 1, vector# 9
		[TestCase(
			"00002000000000000000000000000000" +
			"0000000000000000",
			"714DA982330B4B52E88CD0AC151E77AB" +
			"72EECEA2023139DA39FCCC3ABC12F83F" +
			"455733EDC22808318F10499EA0FCEEB4" +
			"0F61EF121C39F62D92CA62DA885BDF21",
			"2EE0006A6B8F39086D981B9F332330C6" +
			"25531E002B8A28E7AEDA658D4A59558E" +
			"788CC2A24B073FA4E523F4BDA4EFF218" +
			"6AE54BADEA96283266035DCAB57CC935",
			"BD63C4E505BB28A14D2C25DA1A233905" +
			"578560105F7DF219653B8B3FB0436933" +
			"3B84259A1F2E866BC3F90EDC7192B03B" +
			"D83EEEB33CB9FD63D65BE8F5A8B93905",
			"9DDBAAFAF7E490A32059C79D76854C17" +
			"0F3AEFAB39D3B16F964C9A36AB07DBA7" +
			"A66AF2D1C73DE7732DB4E0D51ABB71B6" +
			"56EF23A130C4C1DD901523019FC75ACD"
		)] // Set 1, vector# 18
		public void Test_Salsa20_08( string key, string result_0_63, string result_192_255, string result_256_319, string result_448_511 )
		{
			byte[] output = new byte[ 512 ];
			using( SymmetricAlgorithm symmetricAlgorithm = Salsa20_08.Create( ) )
			{
				symmetricAlgorithm.Key = key.ToByteArray( );
				symmetricAlgorithm.Mode = CipherMode.ECB;
				symmetricAlgorithm.Padding = PaddingMode.None;
				symmetricAlgorithm.BlockSize = 512;
				byte[] iv = new byte[ 64 ];
				Array.Clear( iv, 0, symmetricAlgorithm.BlockSize >> 3 );
				symmetricAlgorithm.IV = iv;
				byte[] input = new byte[ 512 ];
				using( ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateEncryptor( ) )
				{
					using( Stream memoryStream = new MemoryStream( input, false ) )
					{
						using( Stream streamEncrypted = new CryptoStream( memoryStream, cryptoTransform, CryptoStreamMode.Read ) )
						{
							streamEncrypted.Read( output, 0, output.Length );
						}
					}
				}
				using( ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateDecryptor( ) )
				{
					using( Stream memoryStream = new MemoryStream( output, false ) )
					{
						byte[] decrypted = new byte[ output.Length ];
						using( Stream streamEncrypted = new CryptoStream( memoryStream, cryptoTransform, CryptoStreamMode.Read ) )
						{
							streamEncrypted.Read( decrypted, 0, decrypted.Length );
						}
						Assert.AreEqual( input, decrypted );
					}
				}
			}
			Assert.AreEqual( output.SubArray( 0, 64 ).ToHexString( ), result_0_63 );
			Assert.AreEqual( output.SubArray( 192, 64 ).ToHexString( ), result_192_255 );
			Assert.AreEqual( output.SubArray( 256, 64 ).ToHexString( ), result_256_319 );
			Assert.AreEqual( output.SubArray( 448, 64 ).ToHexString( ), result_448_511 );
			//return output.ToHexString( );
		}

		[TestCase(
			"800000000000000000000000000000000000000000000000",
			"FC207DBFC76C5E1774961E7A5AAD09069B2225AC1CE0FE7A0CE77003E7E5BDF8B31AF821000813E6C56B8C1771D6EE7039B2FBD0A68E8AD70A3944B677937897",
			"4B62A4881FA1AF9560586510D5527ED48A51ECAFA4DECEEBBDDC10E9918D44AB26B10C0A31ED242F146C72940C6E9C3753F641DA84E9F68B4F9E76B6C48CA5AC",
			"F52383D9DEFB20810325F7AEC9EADE34D9D883FEE37E05F74BF40875B2D0BE79ED8886E5BFF556CEA8D1D9E86B1F68A964598C34F177F8163E271B8D2FEB5996",
			"A52ED8C37014B10EC0AA8E05B5CEEE123A1017557FB3B15C53E6C5EA8300BF74264A73B5315DC821AD2CAB0F3BB2F152BDAEA3AEE97BA04B8E72A7B40DCC6BA4"
		)]
		public void Test_Salsa20_12( string key, string result_0_63, string result_192_255, string result_256_319, string result_448_511 )
		{
			byte[] output = new byte[ 512 ];
			using( SymmetricAlgorithm symmetricAlgorithm = Salsa20_12.Create( ) )
			{
				symmetricAlgorithm.Key = key.ToByteArray( );
				symmetricAlgorithm.Mode = CipherMode.ECB;
				symmetricAlgorithm.Padding = PaddingMode.None;
				symmetricAlgorithm.BlockSize = 512;
				byte[] iv = new byte[ 64 ];
				Array.Clear( iv, 0, symmetricAlgorithm.BlockSize >> 3 );
				symmetricAlgorithm.IV = iv;
				byte[] input = new byte[ 512 ];
				using( ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateEncryptor( ) )
				{
					using( Stream memoryStream = new MemoryStream( input, false ) )
					{
						using( Stream streamEncrypted = new CryptoStream( memoryStream, cryptoTransform, CryptoStreamMode.Read ) )
						{
							streamEncrypted.Read( output, 0, output.Length );
						}
					}
				}
				using( ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateDecryptor( ) )
				{
					using( Stream memoryStream = new MemoryStream( output, false ) )
					{
						byte[] decrypted = new byte[ output.Length ];
						using( Stream streamEncrypted = new CryptoStream( memoryStream, cryptoTransform, CryptoStreamMode.Read ) )
						{
							streamEncrypted.Read( decrypted, 0, decrypted.Length );
						}
						Assert.AreEqual( input, decrypted );
					}
				}
			}
			Assert.AreEqual( output.SubArray( 0, 64 ).ToHexString( ), result_0_63 );
			Assert.AreEqual( output.SubArray( 192, 64 ).ToHexString( ), result_192_255 );
			Assert.AreEqual( output.SubArray( 256, 64 ).ToHexString( ), result_256_319 );
			Assert.AreEqual( output.SubArray( 448, 64 ).ToHexString( ), result_448_511 );
			//return output.ToHexString( );
		}

		[TestCase(
			"800000000000000000000000000000000000000000000000",
			"4DFA5E481DA23EA09A31022050859936DA52FCEE218005164F267CB65F5CFD7F2B4F97E0FF16924A52DF269515110A07F9E460BC65EF95DA58F740B7D1DBB0AA",
			"DA9C1581F429E0A00F7D67E23B730676783B262E8EB43A25F55FB90B3E753AEF8C6713EC66C51881111593CCB3E8CB8F8DE124080501EEEB389C4BCB6977CF95",
			"7D5789631EB4554400E1E025935DFA7B3E9039D61BDC58A8697D36815BF1985CEFDF7AE112E5BB81E37ECF0616CE7147FC08A93A367E08631F23C03B00A8DA2F",
			"B375703739DACED4DD4059FD71C3C47FC2F9939670FAD4A46066ADCC6A5645783308B90FFB72BE04A6B147CBE38CC0C3B9267C296A92A7C69873F9F263BE9703"
		)]
		[TestCase(
			"121212121212121212121212121212120000000000000000",
			"05835754A1333770BBA8262F8A84D0FD70ABF58CDB83A54172B0C07B6CCA5641060E3097D2B19F82E918CB697D0F347DC7DAE05C14355D09B61B47298FE89AEB",
			"5525C22F425949A5E51A4EAFA18F62C6E01A27EF78D79B073AEBEC436EC8183BC683CD3205CF80B795181DAFF3DC98486644C6310F09D865A7A75EE6D5105F92",
			"2EE7A4F9C576EADE7EE325334212196CB7A61D6FA693238E6E2C8B53B900FF1A133A6E53F58AC89D6A695594CE03F7758DF9ABE981F23373B3680C7A4AD82680",
			"CB7A0595F3A1B755E9070E8D3BACCF9574F881E4B9D91558E19317C4C254988F42184584E5538C63D964F8EF61D86B09D983998979BA3F44BAF527128D3E5393"
		)]
		[TestCase(
			"000000000000000000000000000000008000000000000000",
			"B66C1E4446DD9557E578E223B0B768017B23B267BB0234AE4626BF443F219776436FB19FD0E8866FCD0DE9A9538F4A09CA9AC0732E30BCF98E4F13E4B9E201D9",
			"462920041C5543954D6230C531042B999A289542FEB3C129C5286E1A4B4CF1187447959785434BEF0D05C6EC8950E469BBA6647571DDD049C72D81AC8B75D027",
			"DD84E3F631ADDC4450B9813729BD8E7CC8909A1E023EE539F12646CFEC03239A68F3008F171CDAE514D20BCD584DFD44CBF25C05D028E51870729E4087AA025B",
			"5AC8474899B9E28211CC7137BD0DF290D3E926EB32D8F9C92D0FB1DE4DBE452DE3800E554B348E8A3D1B9C59B9C77B090B8E3A0BDAC520E97650195846198E9D"
		)]
		[TestCase(
			"80000000000000000000000000000000000000000000000000000000000000000000000000000000",
			"E3BE8FDD8BECA2E3EA8EF9475B29A6E7003951E1097A5C38D23B7A5FAD9F6844B22C97559E2723C7CBBD3FE4FC8D9A0744652A83E72A9C461876AF4D7EF1A117",
			"57BE81F47B17D9AE7C4FF15429A73E10ACF250ED3A90A93C711308A74C6216A9ED84CD126DA7F28E8ABF8BB63517E1CA98E712F4FB2E1A6AED9FDC73291FAA17",
			"958211C4BA2EBD5838C635EDB81F513A91A294E194F1C039AEEC657DCE40AA7E7C0AF57CACEFA40C9F14B71A4B3456A63E162EC7D8D10B8FFB1810D71001B618",
			"696AFCFD0CDDCC83C7E77F11A649D79ACDC3354E9635FF137E929933A0BD6F5377EFA105A3A4266B7C0D089D08F1E855CC32B15B93784A36E56A76CC64BC8477"
		)]
		[TestCase(
			"3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F0000000000000000",
			"18B631E89190A2C763AD5F1DBC57B565EAD588F7DC85C3DD75E7D7E74C1D4429E2FB3C6CB687A620EB7050CCD49B54D0F147302BFB7ADC6D1EB235A60338D190",
			"FE2017B0E26C72416B6789071D0EABE48DA7531CAD058597AB3742792C79167844C84243B910FCA131C4EB3D39BD6341842F96F4059261438A81423586EEE459",
			"5FA44FAD6149C7E80BA6A98A8C861993F7D39F1CAEAD07CEB96CBB9BD9153C978B8957C82F88EC2EDD1BCC207627CDB7029AFC907BBEAFAA14444F66CB9A20EA",
			"CF4DD50E4D99B8A26A9ED0F8CEE5FC10E8410C7071CCFD6939C09AE576C3A5EDD2F03412E40C8BAD8DC72FAFD2ED76A1AF3BDD674EC5428BD400E2D4AE9026EF"
		)]
		[TestCase(
			"00000000000000000000000000000000000000000000000000000000000000000000200000000000",
			"621F3014E0ADC8022868C3D9070BC49E48BC6B504AFF11CB17957F0EBFB7612F7FCB67C60A2FBD7A4BD7C312E8F50AF3CA7520821D73DB47189DAD557C436DDC",
			"42C8DFE869C90018825E2037BB5E2EBBC4A4A42660AFEA8A2E385AFBBC63EF3098D052FF4A52ED12107EE71C1AEC271E6870538FCEAA1191B4224A6FFDCE5327",
			"4214DA4FAF0DF7FC2955D81403C9D49EE87116B1975C5823E28D9A08C5B1189DC52BCBEF065B637F1870980CB778B75ADDA41613F5F4728AD8D8D189FBF0E76D",
			"4CA854257ECE95E67383FC8665C3A8238B87255F815CA4DEC2D57DB72924C60CB20A7EE40C559406AAAB25BE5F47184DD187ED7EA191133F3000CB88DCBAC433"
		)]
		public void Test_Salsa20_20( string key, string result_0_63, string result_192_255, string result_256_319, string result_448_511 )
		{
			byte[] output = new byte[ 512 ];
			using( SymmetricAlgorithm symmetricAlgorithm = Salsa20_20.Create( ) )
			{
				symmetricAlgorithm.Key = key.ToByteArray( );
				symmetricAlgorithm.Mode = CipherMode.ECB;
				symmetricAlgorithm.Padding = PaddingMode.None;
				symmetricAlgorithm.BlockSize = 512;
				byte[] iv = new byte[ 64 ];
				Array.Clear( iv, 0, symmetricAlgorithm.BlockSize >> 3 );
				symmetricAlgorithm.IV = iv;
				byte[] input = new byte[ 512 ];
				using( ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateEncryptor( ) )
				{
					using( Stream memoryStream = new MemoryStream( input, false ) )
					{
						using( Stream streamEncrypted = new CryptoStream( memoryStream, cryptoTransform, CryptoStreamMode.Read ) )
						{
							streamEncrypted.Read( output, 0, output.Length );
						}
					}
				}
				using( ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateDecryptor( ) )
				{
					using( Stream memoryStream = new MemoryStream( output, false ) )
					{
						byte[] decrypted = new byte[ output.Length ];
						using( Stream streamEncrypted = new CryptoStream( memoryStream, cryptoTransform, CryptoStreamMode.Read ) )
						{
							streamEncrypted.Read( decrypted, 0, decrypted.Length );
						}
						Assert.AreEqual( input, decrypted );
					}
				}
			}
			Assert.AreEqual( output.SubArray( 0, 64 ).ToHexString( ), result_0_63 );
			Assert.AreEqual( output.SubArray( 192, 64 ).ToHexString( ), result_192_255 );
			Assert.AreEqual( output.SubArray( 256, 64 ).ToHexString( ), result_256_319 );
			Assert.AreEqual( output.SubArray( 448, 64 ).ToHexString( ), result_448_511 );
			//return output.ToHexString( );
		}

		[Test]
		public void Isaac( )
		{
			SRC.Math.PRNG.Random random = new Isaac( );
			byte[] buffer = new byte[ 256 ];
			random.NextBytes( buffer );
		}
	}
}

