
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
namespace GestaoFornecedores.Infra.CrossCutting.Seguranca
{
	public static class Criptografia
	{
		private static byte[] chaveExterna = { 0x50, 0x08, 0xF1, 0xDD, 0xDE, 0x3C, 0xF2, 0x18, 0x44, 0x74, 0x19, 0x2C, 0x53, 0x49, 0xAB, 0xBC };
		private const string chaveInterna = "Q3JpcHRvZ3JhZmlhcyBjb20gUmluamRhZWwgLyBBRVM=";

		public static string Criptografa(string text)
		{
			try
			{
				if (!string.IsNullOrEmpty(text))
				{
					// Cria instancias de vetores de bytes com as chaves                
					byte[] bKey = Convert.FromBase64String(chaveInterna);
					byte[] bText = new UTF8Encoding().GetBytes(text);

					// Instancia a classe de criptografia Rijndael
					Rijndael rijndael = new RijndaelManaged();

					// Define o tamanho da chave "256 = 8 * 32"                
					// Lembre-se: chaves possíves:                
					// 128 (16 caracteres), 192 (24 caracteres) e 256 (32 caracteres)                
					rijndael.KeySize = 256;

					// Cria o espaço de memória para guardar o valor criptografado:                
					MemoryStream mStream = new MemoryStream();
					// Instancia o encriptador                 
					CryptoStream encryptor = new CryptoStream(
						mStream,
						rijndael.CreateEncryptor(bKey, chaveExterna),
						CryptoStreamMode.Write);

					// Faz a escrita dos dados criptografados no espaço de memória
					encryptor.Write(bText, 0, bText.Length);
					// Despeja toda a memória.                
					encryptor.FlushFinalBlock();
					// Pega o vetor de bytes da memória e gera a string criptografada
					return System.Web.HttpServerUtility.UrlTokenEncode(mStream.ToArray());
				}
				else
					return null;
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Erro ao criptografar", ex);
			}
		}

		public static string Descriptografa(string text)
		{
			try
			{
				if (!string.IsNullOrEmpty(text))
				{
					// Cria instancias de vetores de bytes com as chaves                
					byte[] bKey = Convert.FromBase64String(chaveInterna);
					byte[] bText = System.Web.HttpServerUtility.UrlTokenDecode(text);

					// Instancia a classe de criptografia Rijndael                
					Rijndael rijndael = new RijndaelManaged();

					// Define o tamanho da chave "256 = 8 * 32"                
					// Lembre-se: chaves possíves:                
					// 128 (16 caracteres), 192 (24 caracteres) e 256 (32 caracteres)                
					rijndael.KeySize = 256;

					// Cria o espaço de memória para guardar o valor DEScriptografado:               
					MemoryStream mStream = new MemoryStream();

					// Instancia o Decriptador                 
					CryptoStream decryptor = new CryptoStream(
						mStream,
						rijndael.CreateDecryptor(bKey, chaveExterna),
						CryptoStreamMode.Write);

					// Faz a escrita dos dados criptografados no espaço de memória   
					decryptor.Write(bText, 0, bText.Length);
					// Despeja toda a memória.                
					decryptor.FlushFinalBlock();
					// Instancia a classe de codificação para que a string venha de forma correta         
					UTF8Encoding utf8 = new UTF8Encoding();
					// Com o vetor de bytes da memória, gera a string descritografada em UTF8
					return utf8.GetString(mStream.ToArray());
				}
				else
					return null;
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Erro ao descriptografar", ex);
			}
		}
	}
}
