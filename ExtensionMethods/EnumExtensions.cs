using System;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace GestaoFornecedores.Infra.CrossCutting.ExtensionMethods
{
	public static class EnumExtensions
	{
		public static string GetDisplayName(this Enum enumValue)
		{
			try
			{
				return enumValue.GetType()
							.GetMember(enumValue.ToString())
							.First()
							.GetCustomAttribute<DisplayAttribute>()
							.GetName();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
}
