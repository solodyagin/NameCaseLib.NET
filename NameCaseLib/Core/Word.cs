namespace NameCaseLib
{
	/// <summary>
	/// Word - класс, который служит для хранения всей информации о каждом слове
	/// </summary>
	public class Word
	{
		/// <summary>
		/// Слово в нижнем регистре, которое хранится в объекте класса
		/// </summary>
		public readonly string Value;

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
		private Gender _genderSolved = Gender.Null;

		/// <summary>
		/// Маска больших букв в слове.
		/// 
		/// Содержит информацию о том, какие буквы в слове были большими, а какие мальникими:
		/// - x - маленькая буква
		/// - X - больная буква
		/// </summary>
		private LettersMask[] _letterMask;

		/// <summary>
		/// Содержит true, если все слово было в верхнем регистре и false, если не было
		/// </summary>
		private bool _isUpperCase = false;

		/// <summary>
		/// Массив содержит все падежи слова, полученные после склонения текущего слова
		/// </summary>
		private string[] _nameCases;

		/// <summary>
		/// Номер правила, по которому было произведено склонение текущего слова
		/// </summary>
		public int Rule { get; set; } = 0;

		/// <summary>
		/// Номер позиции слова в ФИО
		/// </summary>
		public int Position = 1;

		/// <summary>
		/// Создание нового обьекта со словом
		/// </summary>
		/// <param name="value">Слово</param>
		public Word(string value)
		{
			GenerateMask(value);
			Value = value.ToLower();
		}

		/// <summary>
		/// Генерирует маску, которая содержит информацию о том, какие буквы в слове были большими, а какие маленькими:
		/// - x - маленькая буква
		/// - Х - большая буква
		/// </summary>
		/// <param name="word">Слово для которого нужна маска</param>
		private void GenerateMask(string word)
		{
			_isUpperCase = true;
			_letterMask = new LettersMask[word.Length];
			for (int i = 0; i < word.Length; i++)
			{
				string letter = word.Substring(i, 1);
				if (letter == letter.ToLower())
				{
					_isUpperCase = false;
					_letterMask[i] = LettersMask.x;
				}
				else
				{
					_letterMask[i] = LettersMask.X;
				}
			}
		}

		/// <summary>
		/// Возвращает все падежи слова в начальную маску
		/// </summary>
		private void ReturnMask()
		{
			if (_isUpperCase)
			{
				for (int i = 0; i < _nameCases.Length; i++)
					_nameCases[i] = _nameCases[i].ToUpper();
			}
			else
			{
				for (int i = 0; i < _nameCases.Length; i++)
				{
					string newStr = "";
					for (int letter = 0; letter < _nameCases[i].Length; letter++)
					{
						if (letter < _letterMask.Length && _letterMask[letter] == LettersMask.X)
							newStr += _nameCases[i].Substring(letter, 1).ToUpper();
						else
							newStr += _nameCases[i].Substring(letter, 1);
					}
					_nameCases[i] = newStr;
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
				_nameCases = value;
				ReturnMask();
			}
			get => _nameCases;
		}

		/// <summary>
		/// Расчитывает и возвращает пол текущего слова. Или устанавливает нужный пол.
		/// </summary>
		public Gender Gender
		{
			get
			{
				if (_genderSolved == Gender.Null)
					_genderSolved = GenderProbability.Man > GenderProbability.Woman ? Gender.Man : Gender.Woman;
				return _genderSolved;
			}
			set => _genderSolved = value;
		}

		/// <summary>
		/// Возвращает строку с нужным падежом текущего слова
		/// </summary>
		/// <param name="padeg">нужный падеж</param>
		/// <returns>строка с нужным падежом текущего слова</returns>
		public string GetNameCase(Padeg padeg)
		{
			return _nameCases != null ? _nameCases[(int)padeg] : "";
		}

		/// <summary>
		/// Если уже был расчитан пол для всех слов системы, тогда каждому слову предается окончательное
		/// решение. Эта функция определяет было ли принято окончательное решение.
		/// </summary>
		/// <returns>true если определен и false если нет</returns>
		public bool isGenderSolved()
		{
			return _genderSolved != Gender.Null;
		}
	}
}
