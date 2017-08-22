
namespace GestaoFornecedores.Infra.CrossCutting.ExtensionMethods
{
	public static class BoolExtensions
	{
		public static string GetSimOuNao(this bool value)
		{
			if (value)
				return "Sim";
			else
				return "Não";
		}
	}
}
