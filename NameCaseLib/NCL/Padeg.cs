using System.ComponentModel;

namespace NameCaseLib
{
	/// <summary>
	/// Содержит перечисление падежей русского и украинского языков
	/// </summary>
	public enum Padeg
	{
		/// <summary>
		/// Именительный падеж
		/// </summary>
		[Description("Именительный")]
		IMENITLN = 0,
		/// <summary>
		/// Родительный падеж
		/// </summary>
		[Description("Родительный")]
		RODITLN = 1,
		/// <summary>
		/// Дательный падеж
		/// </summary>
		[Description("Дательный")]
		DATELN = 2,
		/// <summary>
		/// Винительный падеж
		/// </summary>
		[Description("Винительный")]
		VINITELN = 3,
		/// <summary>
		/// Творительный падеж
		/// </summary>
		[Description("Творительный")]
		TVORITELN = 4,
		/// <summary>
		/// Предложный падеж
		/// </summary>
		[Description("Предложный")]
		PREDLOGN = 5,
		/// <summary>
		/// Називний відмінок
		/// </summary>
		[Description("Називний")]
		UaNazyvnyi = 0,
		/// <summary>
		/// Родовий відмінок
		/// </summary>
		[Description("Родовий")]
		UaRodovyi = 1,
		/// <summary>
		/// Давальний відмінок
		/// </summary>
		[Description("Давальний")]
		UaDavalnyi = 2,
		/// <summary>
		/// Знахідний відмінок
		/// </summary>
		[Description("Знахідний")]
		UaZnahidnyi = 3,
		/// <summary>
		/// Орудний відмінок
		/// </summary>
		[Description("Орудний")]
		UaOrudnyi = 4,
		/// <summary>
		/// Місцевий відмінок
		/// </summary>
		[Description("Місцевий")]
		UaMiszevyi = 5,
		/// <summary>
		/// Кличний відмінок
		/// </summary>
		[Description("Кличний")]
		UaKlychnyi = 6
	}
}
