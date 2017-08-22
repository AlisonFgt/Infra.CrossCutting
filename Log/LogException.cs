
using GestaoFornecedores.Infra.CrossCutting.Arquivo;
using System.Web.Mvc;

namespace GestaoFornecedores.Infra.CrossCutting.Log
{
	public class LogException : HandleErrorAttribute
	{
		public override void OnException(ExceptionContext filterContext)
		{
			base.OnException(filterContext);
			object log = Arquivos.GeraJsonLog(filterContext);
			Arquivos.SalvaJson(log, "LogException");
		}
	}
}
