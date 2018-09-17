using System.ComponentModel;

namespace NameCaseLib.NCL
{
	/// <summary>
	/// Перечисление для идентификации части ФИО
	/// </summary>
	public enum FioPart
	{
		/// <summary>
		/// Слово не идентифицировано
		/// </summary>
		[Description("Не идентифицировано")]
		Null = 0,
		/// <summary>
		/// Имя
		/// </summary>
		[Description("Имя")]
		Name = 1,
		/// <summary>
		/// Фамилия
		/// </summary>
		[Description("Фамилия")]
		SurName = 2,
		/// <summary>
		/// Отчество
		/// </summary>
		[Description("Отчество")]
		PatrName = 3
	}
}
