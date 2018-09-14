using NameCaseLib.NCL;

namespace NameCaseLib.Core
{
	/// <summary>
	/// Word - класс, который служит для хранения всей информации о каждом слове
	/// </summary>
	public class Word
	{
		/// <summary>
		/// Слово в нижнем регистре, которое хранится в объекте класса
		/// </summary>
		public string Value { get; }

		/// <summary>
		/// Тип текущей записи (Фамилия/Имя/Отчество)
		/// </summary>
		public FioPart FioPart { get; set; } = FioPart.Null;

		/// <summary>
		/// Вероятность того, что текущее слово относится к мужскому или женскому полу
		/// </summary>
		public GenderProbability GenderProbability { get; set; }

		/// <summary>
		/// Окончательное решение, к какому полу относится слово
		/// </summary>
		private Gender genderSolved = Gender.Null;

		/// <summary>
		/// Маска больших букв в слове.
		/// 
		/// Содержит информацию о том, какие буквы в слове были большими, а какие мальникими:
		/// - x - маленькая буква
		/// - X - больная буква
		/// </summary>
		private LettersMask[] letterMask;

		/// <summary>
		/// Содержит true, если все слово было в верхнем регистре и false, если не было
		/// </summary>
		private bool isUpperCase = false;

		/// <summary>
		/// Массив содержит все падежи слова, полученные после склонения текущего слова
		/// </summary>
		private string[] nameCases;

		/// <summary>
		/// Номер правила, по которому было произведено склонение текущего слова
		/// </summary>
		public int Rule { get; set; } = 0;

		/// <summary>
		/// Создание нового обьекта со словом
		/// </summary>
		/// <param name="word">Слово</param>
		public Word(string word)
		{
			GenerateMask(word);
			this.Value = word.ToLower();
		}

		/// <summary>
		/// Генерирует маску, которая содержит информацию о том, какие буквы в слове были большими, а какие маленькими:
		/// - x - маленькая буква
		/// - Х - большая буква
		/// </summary>
		/// <param name="word">Слово для которого нужна маска</param>
		private void GenerateMask(string word)
		{
			isUpperCase = true;
			int length = word.Length;
			letterMask = new LettersMask[length];
			for (int i = 0; i < length; i++)
			{
				string letter = word.Substring(i, 1);
				if (letter == letter.ToLower())
				{
					isUpperCase = false;
					letterMask[i] = LettersMask.x;
				}
				else
				{
					letterMask[i] = LettersMask.X;
				}
			}
		}

		/// <summary>
		/// Возвращает все падежи слова в начальную маску
		/// </summary>
		private void ReturnMask()
		{
			int wordCount = nameCases.Length;
			if (isUpperCase)
			{
				for (int i = 0; i < wordCount; i++)
				{
					nameCases[i] = nameCases[i].ToUpper();
				}
			}
			else
			{
				for (int i = 0; i < wordCount; i++)
				{
					int lettersCount = nameCases[i].Length;
					int maskLength = letterMask.Length;
					string newStr = "";
					for (int letter = 0; letter < lettersCount; letter++)
					{
						if (letter < maskLength && letterMask[letter] == LettersMask.X)
						{
							newStr += nameCases[i].Substring(letter, 1).ToUpper();
						}
						else
						{
							newStr += nameCases[i].Substring(letter, 1);
						}
					}
					nameCases[i] = newStr;
				}
			}
		}

		/// <summary>
		/// Считывает или устанавливает все падежи
		/// </summary>
		public string[] NameCases
		{
			set
			{
				nameCases = value;
				ReturnMask();
			}
			get
			{
				return nameCases;
			}
		}

		/// <summary>
		/// Расчитывает и возвращает пол текущего слова. Или устанавливает нужный пол.
		/// </summary>
		public Gender Gender
		{
			get
			{
				if (genderSolved == Gender.Null)
				{
					genderSolved = (GenderProbability.Man > GenderProbability.Woman) ? Gender.Man : Gender.Woman;
				}
				return genderSolved;
			}
			set
			{
				genderSolved = value;
			}
		}

		/// <summary>
		/// Возвращает строку с нужным падежом текущего слова
		/// </summary>
		/// <param name="padeg">нужный падеж</param>
		/// <returns>строка с нужным падежом текущего слова</returns>
		public string GetNameCase(Padeg padeg)
		{
			return nameCases[(int)padeg];
		}

		/// <summary>
		/// Если уже был расчитан пол для всех слов системы, тогда каждому слову предается окончательное
		/// решение. Эта функция определяет было ли принято окончательное решение.
		/// </summary>
		/// <returns>true если определен и false если нет</returns>
		public bool isGenderSolved()
		{
			return genderSolved != Gender.Null;
		}
	}
}
