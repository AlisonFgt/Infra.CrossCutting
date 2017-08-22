
using GestaoFornecedores.Infra.CrossCutting.Arquivo;
using System.Web.Mvc;
namespace GestaoFornecedores.Infra.CrossCutting.Log
{
	public class LogAction : ActionFilterAttribute, IActionFilter
	{
		public string LogMessage { get; set; }

		void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
		{
			object log = Arquivos.GeraJsonLog(filterContext);
			Arquivos.SalvaJson(log, "LogActionExecuting");
		}

		void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
		{
			object log = Arquivos.GeraJsonLog(filterContext);
			Arquivos.SalvaJson(log, "LogActionExecuted");
		}
	}
}
