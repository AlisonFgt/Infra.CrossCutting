using GestaoFornecedores.Infra.CrossCutting.ExtensionMethods;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace GestaoFornecedores.Infra.CrossCutting.Excel
{
	public class Excel
	{
		public static string ExcelContentType
		{
			get
			{ return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; }
		}

		public static DataTable ListToDataTable<T>(List<T> data)
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
			DataTable dataTable = new DataTable();

			for (int i = 0; i < properties.Count; i++)
			{
				PropertyDescriptor property = properties[i];
				dataTable.Columns.Add(property.DisplayName == null ? property.Name : property.DisplayName, property.PropertyType.IsEnum == true ? typeof(String) : Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
			}

			object[] values = new object[properties.Count];
			foreach (T item in data)
			{
				for (int i = 0; i < values.Length; i++)
				{
					if (properties[i].PropertyType.IsEnum)
						values[i] = ((Enum)properties[i].GetValue(item)).GetDisplayName();
					else
						values[i] = properties[i].GetValue(item);
				}

				dataTable.Rows.Add(values);
			}
			return dataTable;
		}

		public static byte[] ExportaExcel(DataTable tabela, string titulo = "", bool mostraNroLinha = false, params string[] ignorarColunas)
		{
			byte[] result = null;
			using (ExcelPackage package = new ExcelPackage())
			{
				ExcelWorksheet workSheet = package.Workbook.Worksheets.Add(String.Format("{0} Data", titulo));
				int linhaInicial = String.IsNullOrEmpty(titulo) ? 1 : 3;

				if (mostraNroLinha)
				{
					DataColumn dataColumn = tabela.Columns.Add("Linha", typeof(int));
					dataColumn.SetOrdinal(0);
					int index = 1;
					foreach (DataRow item in tabela.Rows)
					{
						item[0] = index;
						index++;
					}
				}

				// Se tiver titulo deixar a linha maior para destacar
				if (linhaInicial.Equals(3))
					AplicarEstiloTitulo(workSheet.Row(1));

				// Adicione o conteúdo ao arquivo Excel
				workSheet.Cells["A" + linhaInicial].LoadFromDataTable(tabela, true);

				// Aplique a largura das células com conteúdo pequeno
				int columnIndex = 1;
				foreach (DataColumn column in tabela.Columns)
				{
					ExcelRange columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.End.Row, columnIndex];
					int maxLength = columnCells.Max(cell => cell.Value == null ? 1 : cell.Value.ToString().Count());
					if (maxLength < 150)
					{
						workSheet.Column(columnIndex).AutoFit();
					}
					columnIndex++;
				}

				// Cabeçalho do formato - negrito e estilo
				using (ExcelRange r = workSheet.Cells[linhaInicial, 1, linhaInicial, tabela.Columns.Count])
				{
					AplicarBorda(r);
					AplicarEstiloCabecalho(r);
				}

				// Formatar células - adicionar bordas
				using (ExcelRange r = workSheet.Cells[linhaInicial + 1, 1, linhaInicial + tabela.Rows.Count, tabela.Columns.Count])
				{
					AplicarBorda(r);
				}

				// Remover colunas ignoradas
				for (int i = tabela.Columns.Count - 1; i >= 0; i--)
				{
					if (i == 0 && mostraNroLinha)
					{
						continue;
					}
					if (ignorarColunas.Contains(tabela.Columns[i].ColumnName))
					{
						workSheet.DeleteColumn(i + 1);
					}
				}

				if (!String.IsNullOrEmpty(titulo))
				{
					workSheet.Cells["A1"].Value = titulo;
					workSheet.Cells["A1"].Style.Font.Size = 20;

					workSheet.InsertColumn(1, 1);
					workSheet.InsertRow(1, 1);
					workSheet.Column(1).Width = 5;
				}

				result = package.GetAsByteArray();
			}

			return result;
		}

		private static void AplicarEstiloTitulo(ExcelRow excelRow)
		{
			excelRow.Height = 30;
			excelRow.Style.Font.Bold = true;
		}

		private static void AplicarEstiloCabecalho(ExcelRange r)
		{
			r.Style.Font.Color.SetColor(System.Drawing.Color.White);
			r.Style.Font.Bold = true;
			r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
			r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#2970bc"));
		}

		private static void AplicarBorda(ExcelRange r)
		{
			r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
			r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
			r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
			r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

			r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
			r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
			r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
			r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
		}

		public static byte[] ExportaExcel<T>(List<T> data, string Heading = "", bool showSlno = false, params string[] ColumnsToTake)
		{
			return ExportaExcel(ListToDataTable<T>(data), Heading, showSlno, ColumnsToTake);
		}
	}
}
