using System.ComponentModel;

namespace NameCaseLib.NCL
{
	/// <summary>
	/// Перечисление пола человека
	/// </summary>
	public enum Gender
	{
		/// <summary>
		/// Пол не определен
		/// </summary>
		[Description("Неопределен")]
		Null = 0,
		/// <summary>
		/// Мужской пол
		/// </summary>
		[Description("Мужской")]
		Man = 1,
		/// <summary>
		/// Женский пол
		/// </summary>
		[Description("Женский")]
		Woman = 2
	}
}
