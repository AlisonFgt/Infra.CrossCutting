using System.Web;

namespace GestaoFornecedores.Infra.CrossCutting.Helpers
{
	public class HttpHelper
	{
		public static string GetUrlBase()
		{
			return string.Format("{0}://{1}/", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority);
		}
	}
}
