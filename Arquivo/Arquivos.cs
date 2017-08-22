using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace GestaoFornecedores.Infra.CrossCutting.Arquivo
{
	public class Arquivos
	{
		private static readonly string _dirArquivos = ConfigurationManager.AppSettings["Arquivos"];

		public static void SalvaJson(object json, string pasta)
		{
			var diretorio = Path.Combine(_dirArquivos, pasta, String.Format("{0:yyyyMM}", DateTime.Now));
			var diretorioArquivo = Path.Combine(diretorio, String.Format("{0:dd-MM-yyyy}", DateTime.Now) + ".json");
			List<object> Logs = new List<object>();
			CriaDiretorioArquivos(diretorio);

			if (File.Exists(diretorioArquivo))
			{
				using (var streamReader = new StreamReader(diretorioArquivo))
				{
					string log = streamReader.ReadToEnd();
					var deserializedObject = JsonConvert.DeserializeObject<List<object>>(log);
					deserializedObject.Add(json);
					Logs = deserializedObject;
				}
			}
			else
				Logs.Add(json);

			System.IO.File.WriteAllText(diretorioArquivo, JsonConvert.SerializeObject(Logs.ToArray(), Formatting.Indented));
		}

		private static void CriaDiretorioArquivos(string diretorio)
		{
			if (!Directory.Exists(diretorio))
				Directory.CreateDirectory(diretorio);
		}

		public static void SalvaArquivo(HttpPostedFileBase file, string nomeArquivo)
		{
			if (file != null)
			{
				CriaDiretorioArquivos();
				file.SaveAs(Path.Combine(_dirArquivos, nomeArquivo));
			}
		}

		public static void SalvaArquivo(HttpPostedFileBase file)
		{
			if (file != null)
			{
				CriaDiretorioArquivos();
				file.SaveAs(_dirArquivos + Path.GetFileName(file.FileName));
			}
		}

		private static void CriaDiretorioArquivos()
		{
			if (!Directory.Exists(_dirArquivos))
				Directory.CreateDirectory(_dirArquivos);
		}

		public static void ApagaArquivo(string nomeArquivo)
		{
			var caminhoENomeArquivo = Path.Combine(_dirArquivos, nomeArquivo);

			if (File.Exists(caminhoENomeArquivo))
				File.Delete(caminhoENomeArquivo);
		}

		public static byte[] Download(string nomeArquivo)
		{
			nomeArquivo = Path.Combine(_dirArquivos, nomeArquivo);

			if (File.Exists(nomeArquivo))
			{
				FileStream fs = File.OpenRead(nomeArquivo);
				byte[] data = new byte[fs.Length];
				int br = fs.Read(data, 0, data.Length);
				if (br != fs.Length)
					throw new IOException(nomeArquivo);

				return data;
			}
			return null;
		}

		internal static object GeraJsonLog(ExceptionContext filterContext)
		{
			return new
			{
				Data = String.Format("{0:dd/MM/yyyy HH:mm:ss}", DateTime.Now),
				Controller = filterContext.Controller.ToString(),
				Mensagem = filterContext.Exception.Message,
				StackTrace = filterContext.Exception.StackTrace,
				InnerException = filterContext.Exception.InnerException != null ? filterContext.Exception.InnerException.InnerException.Message : null,
				InnerStackTrace = filterContext.Exception.InnerException != null ? filterContext.Exception.InnerException.StackTrace.ToString() : null
			};
		}

		internal static object GeraJsonLog(ActionExecutingContext filterContext)
		{
			return new
			{
				Data = String.Format("{0:dd/MM/yyyy HH:mm:ss}", DateTime.Now),
				Controller = filterContext.Controller.ToString(),
				Parametros = filterContext.ActionParameters,
				Trace = filterContext.HttpContext.Trace
			};
		}

		internal static object GeraJsonLog(ActionExecutedContext filterContext)
		{
			return new
			{
				Data = String.Format("{0:dd/MM/yyyy HH:mm:ss}", DateTime.Now),
				Controller = filterContext.Controller.ToString(),
				Trace = filterContext.HttpContext.Trace
			};
		}
	}
}
