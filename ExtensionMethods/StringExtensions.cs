using System;
using System.Text;
using System.Text.RegularExpressions;

namespace GestaoFornecedores.Infra.CrossCutting.ExtensionMethods
{
	public static class StringExtensions
	{
		private static readonly Regex
		rxSimbolos = new Regex(@"\W", RegexOptions.Compiled);

		/// <summary>
		/// Remove todos os símbolos e espaços de uma string.
		/// Útil para armazenar campos com máscara.
		/// </summary>
		/// <param name="str">A string</param>
		public static string RemoveSimbolos(this String str)
		{
			if (String.IsNullOrEmpty(str))
				return str;
			return rxSimbolos.Replace(str, String.Empty);
		}

		public static string Criptografa(this String str)
		{
			if (String.IsNullOrEmpty(str))
				return str;

			System.Security.Cryptography.SHA256Managed crypt = new System.Security.Cryptography.SHA256Managed();
			System.Text.StringBuilder hash = new System.Text.StringBuilder();
			byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(str), 0, Encoding.UTF8.GetByteCount(str));
			foreach (byte theByte in crypto)
			{
				hash.Append(theByte.ToString("x2"));
			}
			return hash.ToString();
		}

		public static decimal PercentualToDecimal(this String str)
		{
			var strCorrigida = str;
			strCorrigida = strCorrigida.Replace("%", string.Empty);
			strCorrigida = strCorrigida.Replace(".", string.Empty);
			strCorrigida = strCorrigida.Replace(",", ".");
			strCorrigida = strCorrigida.Trim();
			decimal decimalDoPercentual = 0.00m;
			decimal.TryParse(strCorrigida, out decimalDoPercentual);

			return decimalDoPercentual;
		}

		public static decimal ValorToDecimal(this String str)
		{
			var strCorrigida = str;
			strCorrigida = strCorrigida.Replace("R$", string.Empty);
			strCorrigida = strCorrigida.Replace(".", string.Empty);
			strCorrigida = strCorrigida.Replace(",", ".");
			strCorrigida = strCorrigida.Trim();
			decimal decimalDoValor = 0.00m;
			decimal.TryParse(strCorrigida, out decimalDoValor);

			return decimalDoValor;
		}

		private static readonly Regex
			rxCnpj = new Regex(@"^(\d{2}.\d{3}.\d{3}/\d{4}-\d{2})|(\d{14})$", RegexOptions.Compiled);

		public static bool IsCnpjValido(this String str)
		{
			if (str == null)
				return false;

			var cnpj = Convert.ToString(str);

			if (!rxCnpj.IsMatch(cnpj))
				return false;
			return CnpjValido(cnpj);
		}

		private static bool CnpjValido(string cnpj)
		{

			int[] mt1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
			int[] mt2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
			int soma; int resto; string digito; string TempCNPJ;

			cnpj = cnpj.RemoveSimbolos();

			if (cnpj.Length != 14)
				return false;

			if (cnpj.Equals("00000000000000") || cnpj.Equals("11111111111111") ||
				 cnpj.Equals("22222222222222") || cnpj.Equals("33333333333333") ||
				 cnpj.Equals("44444444444444") || cnpj.Equals("55555555555555") ||
				 cnpj.Equals("66666666666666") || cnpj.Equals("77777777777777") ||
				 cnpj.Equals("88888888888888") || cnpj.Equals("99999999999999"))
				return false;

			TempCNPJ = cnpj.Substring(0, 12);
			soma = 0;

			for (int i = 0; i < 12; i++)
				soma += int.Parse(TempCNPJ[i].ToString()) * mt1[i];

			resto = (soma % 11);
			if (resto < 2)
				resto = 0;
			else
				resto = 11 - resto;

			digito = resto.ToString();

			TempCNPJ = TempCNPJ + digito;
			soma = 0;
			for (int i = 0; i < 13; i++)
				soma += int.Parse(TempCNPJ[i].ToString()) * mt2[i];

			resto = (soma % 11);
			if (resto < 2)
				resto = 0;
			else
				resto = 11 - resto;
			digito = digito + resto.ToString();

			return cnpj.EndsWith(digito);
		}

		private static readonly Regex
			rxEmailValido = new Regex(@"^([a-zA-Z0-9_.-]+@[a-zA-Z0-9_-]+[.][a-zA-Z0-9_.-]+)([,;][a-zA-Z0-9_.-]+@[a-zA-Z0-9_-]+[.][a-zA-Z0-9_.-]+)*$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

		public static bool IsEmailValido(this String str)
		{
			if (str == null)
				return false;

			var cnpj = Convert.ToString(str);

			if (rxEmailValido.IsMatch(cnpj))
				return true;

			return false;

		}
	}
}
