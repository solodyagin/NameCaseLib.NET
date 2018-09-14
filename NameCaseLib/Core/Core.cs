using System;
using System.Reflection;
using NameCaseLib.NCL;

namespace NameCaseLib.Core
{
	/// <summary>
	/// Базовый класс с набором основных функций
	/// </summary>
	public abstract class Core
	{
		/// <summary>
		/// Возвращает текущую версию библиотеки
		/// </summary>
		public static string Version { get; } = "0.4.2";

		/// <summary>
		/// Версия языкового файла
		/// </summary>
		protected static string languageBuild;

		/// <summary>
		/// Готовность системы:
		/// - Все слова идентифицированы (известно к какой части ФИО относится слово)
		/// - У всех слов определен пол
		/// Если все сделано стоит флаг true, при добавлении нового слова флаг сбрасывается на false
		/// </summary>
		private bool ready = false;

		/// <summary>
		/// Если все текущие слова были просклонены и в каждом слове уже есть результат склонения,
		/// тогда true. Если было добавлено новое слово, то флаг сбрасывается на false
		/// </summary>
		private bool finished = false;

		/// <summary>
		///  Массив содержит елементы типа Word. Это все слова, которые нужно обработать и просклонять
		/// </summary>
		private WordArray words;

		/// <summary>
		/// Текущее слово, с которым сейчас идет работа
		/// </summary>
		protected string workingWord;

		/// <summary>
		/// Номер последнего использованного правила, устанавливается методом Rule()
		/// </summary>
		private int lastRule = 0;

		/// <summary>
		/// Массив содержит результат склонения слова во всех падежах
		/// </summary>
		protected string[] lastResult;

		/// <summary>
		/// Количество падежей в языке
		/// </summary>
		protected int caseCount;

		/// <summary>
		/// Метод очищает результаты последнего склонения слова. Нужен при склонении нескольких слов.
		/// </summary>
		private void Reset()
		{
			lastRule = 0;
			lastResult = new string[caseCount];
		}

		/// <summary>
		/// Устанавливает флаги о том, что система не готова и слова еще не были просклонены
		/// </summary>
		private void NotReady()
		{
			ready = false;
			finished = false;
		}

		/// <summary>
		/// Сбрасывает всю информацию на начальную. Очищает все слова добавленные в систему.
		/// После выполнения система готова работать сначала. 
		/// </summary>
		public Core FullReset()
		{
			words = new WordArray();
			Reset();
			NotReady();
			return this;
		}

		/// <summary>
		/// Устанавливает последнее правило
		/// </summary>
		/// <param name="ruleID">Правило</param>
		protected void Rule(int ruleID)
		{
			lastRule = ruleID;
		}

		/// <summary>
		/// Считывает и устанавливает последнее правило
		/// </summary>
		protected int GetRule()
		{
			return lastRule;
		}

		/// <summary>
		/// Устанавливает слово текущим для работы системы. Очищает кеш слова.
		/// </summary>
		/// <param name="word">слово, которое нужно установить</param>
		protected void SetWorkingWord(string word)
		{
			Reset();
			workingWord = word;
		}

		/// <summary>
		/// Если не нужно склонять слово, то делает результат таким же, как и именительный падеж
		/// </summary>
		protected void MakeResultTheSame()
		{
			lastResult = new string[caseCount];
			for (int i = 0; i < caseCount; i++)
			{
				lastResult[i] = workingWord;
			}
		}

		/// <summary>
		/// Вырезает определенное количество последних букв текущего слова
		/// </summary>
		/// <param name="length">Количество букв</param>
		/// <returns>Подстроку, содержащую определенное количество букв</returns>
		protected string Last(int length)
		{
			string result = "";
			int startIndex = workingWord.Length - length;
			if (startIndex >= 0)
			{
				result = workingWord.Substring(workingWord.Length - length, length);
			}
			else
			{
				result = workingWord;
			}
			return result;
		}

		/// <summary>
		/// Получает указаное количество букв с конца слова
		/// </summary>
		/// <param name="word">Слово</param>
		/// <param name="length">Количество букв</param>
		/// <returns>Полученая строка</returns>
		protected string Last(string word, int length)
		{
			string result = "";
			int startIndex = word.Length - length;
			if (startIndex >= 0)
			{
				result = word.Substring(word.Length - length, length);
			}
			else
			{
				result = word;
			}
			return result;
		}

		/// <summary>
		/// Вырезает stopAfter букв начиная с length с конца
		/// </summary>
		/// <param name="length">На сколько букв нужно оступить от конца</param>
		/// <param name="stopAfter">Сколько букв нужно вырезать</param>
		/// <returns>Искомая строка</returns>
		protected string Last(int length, int stopAfter)
		{
			string result = "";
			int startIndex = workingWord.Length - length;
			if (startIndex >= 0)
			{
				result = workingWord.Substring(workingWord.Length - length, stopAfter);
			}
			else
			{
				result = workingWord;
			}
			return result;
		}

		/// <summary>
		/// Извлекает последние буквы из указанного слова
		/// </summary>
		/// <param name="word">Слово</param>
		/// <param name="length">Сколько букв с конца</param>
		/// <param name="stopAfter">Сколько нужно взять</param>
		/// <returns>Полученая подстрока</returns>
		protected string Last(string word, int length, int stopAfter)
		{
			string result = "";
			int startIndex = word.Length - length;
			if (startIndex >= 0)
			{
				result = word.Substring(word.Length - length, stopAfter);
			}
			else
			{
				result = word;
			}
			return result;
		}

		/// <summary>
		/// Над текущим словом выполняются правила в указанном порядке.
		/// </summary>
		/// <param name="gender">Пол</param>
		/// <param name="rulesArray">Порядок правил</param>
		/// <returns>Если правило было использовао true если нет тогда false</returns>
		protected bool RulesChain(Gender gender, int[] rulesArray)
		{
			if (gender != Gender.Null)
			{
				int rulesLength = rulesArray.Length;
				string rulesName = (gender == Gender.Man ? "Man" : "Woman");
				Type classType = this.GetType();
				for (int i = 0; i < rulesLength; i++)
				{
					string methodName = string.Format("{0}Rule{1}", rulesName, rulesArray[i]);
					bool res = (bool)classType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(this, null);
					if (res)
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Проверяет входит ли буква в список букв
		/// </summary>
		/// <param name="needle">буква</param>
		/// <param name="letters">список букв</param>
		/// <returns>true если входит в список и false если не входит</returns>
		protected bool In(string needle, string letters)
		{
			return needle != "" && letters.IndexOf(needle) >= 0;
		}

		/// <summary>
		/// Ищет окончание в списке окончаний
		/// </summary>
		/// <param name="needle">окончание</param>
		/// <param name="haystack">список окончаний</param>
		/// <returns>true если найдено и false если нет</returns>
		protected bool In(string needle, string[] haystack)
		{
			int length = haystack.Length;
			if (needle != "")
			{
				for (int i = 0; i < length; i++)
				{
					if (haystack[i] == needle)
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Проверяет равенство имени
		/// </summary>
		/// <param name="needle">имя</param>
		/// <param name="name">имя, с которым сравнить</param>
		/// <returns>true - имена совпали</returns>
		protected bool InNames(string needle, string name)
		{
			return needle == name;
		}

		/// <summary>
		/// Проверяет входит ли имя в список имен
		/// </summary>
		/// <param name="needle">имя</param>
		/// <param name="names">список имен</param>
		/// <returns>true если входит</returns>
		protected bool InNames(string needle, string[] names)
		{
			int length = names.Length;
			for (int i = 0; i < length; i++)
			{
				if (needle == names[i])
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Склоняем слово во все падежи используя окончания
		/// </summary>
		/// <param name="word">Слово</param>
		/// <param name="endings">окончания</param>
		/// <param name="replaceLast">сколько последних букв нужно убрать</param>
		protected void WordForms(string word, string[] endings, int replaceLast)
		{
			// Сохраняем именительный падеж
			lastResult = new string[caseCount];
			lastResult[0] = workingWord;

			if (word.Length >= replaceLast)
			{
				// убираем лишние буквы
				word = word.Substring(0, word.Length - replaceLast);
			}
			else
			{
				word = "";
			}
			// Добавляем окончания
			for (int i = 1; i < caseCount; i++)
			{
				lastResult[i] = word + endings[i - 1];
			}
		}

		/// <summary>
		/// Создает список слов во всех падежах используя окончания для каждого падежа
		/// </summary>
		/// <param name="word">слово</param>
		/// <param name="endings">окончания</param>
		protected void WordForms(string word, string[] endings)
		{
			WordForms(word, endings, 0);
		}

		/// <summary>
		/// Устанавливает имя
		/// </summary>
		/// <param name="name">Имя</param>
		/// <returns></returns>
		public Core SetName(string name)
		{
			if (name.Trim() != "")
			{
				Word tmpWord = new Word(name);
				tmpWord.FioPart = FioPart.Name;
				words.AddWord(tmpWord);
				NotReady();
			}
			return this;
		}

		/// <summary>
		/// Устанавливает фамилию
		/// </summary>
		/// <param name="name">Фамилия</param>
		/// <returns></returns>
		public Core SetSurName(string name)
		{
			if (name.Trim() != "")
			{
				Word tmpWord = new Word(name);
				tmpWord.FioPart = FioPart.SurName;
				words.AddWord(tmpWord);
				NotReady();
			}
			return this;
		}

		/// <summary>
		/// Устанавливает отчество
		/// </summary>
		/// <param name="name">Отчество</param>
		/// <returns></returns>
		public Core SetPatrName(string name)
		{
			if (name.Trim() != "")
			{
				Word tmpWord = new Word(name);
				tmpWord.FioPart = FioPart.PatrName;
				words.AddWord(tmpWord);
				NotReady();
			}
			return this;
		}

		/// <summary>
		/// Устанавливает пол
		/// </summary>
		/// <param name="gender">Пол</param>
		/// <returns></returns>
		public Core SetGender(Gender gender)
		{
			int length = words.Length;
			for (int i = 0; i < length; i++)
			{
				words.GetWord(i).Gender = gender;
			}
			return this;
		}

		/// <summary>
		/// Устанавливает ФИО
		/// </summary>
		/// <param name="surName">Фамилия</param>
		/// <param name="name">Имя</param>
		/// <param name="patrName">Отчество</param>
		/// <returns></returns>
		public Core SetFullName(string surName, string name, string patrName)
		{
			SetName(name);
			SetSurName(surName);
			SetPatrName(patrName);
			return this;
		}

		/// <summary>
		/// Установить фамилию человека
		/// </summary>
		/// <param name="name">Фамилия</param>
		/// <returns></returns>
		public Core SetLastName(string name)
		{
			return SetSurName(name);
		}

		/// <summary>
		/// Идентифицирует нужное слово
		/// </summary>
		/// <param name="word">Слово</param>
		private void PrepareFioPart(Word word)
		{
			if (word.FioPart == FioPart.Null)
			{
				DetectFioPart(word);
			}
		}

		/// <summary>
		/// Идентифицирует все существующие слова
		/// </summary>
		private void PrepareAllFioParts()
		{
			int length = words.Length;
			for (int i = 0; i < length; i++)
			{
				PrepareFioPart(words.GetWord(i));
			}
		}

		/// <summary>
		/// Предварительно определяет пол в слове
		/// </summary>
		/// <param name="word">Слово для определения</param>
		private void PrepareGender(Word word)
		{
			if (!word.isGenderSolved())
			{
				switch (word.FioPart)
				{
					case FioPart.Name: GenderByName(word); break;
					case FioPart.SurName: GenderBySurName(word); break;
					case FioPart.PatrName: GenderByPatrName(word); break;
				}
			}
		}

		/// <summary>
		/// Принимает решение о текущем поле человека
		/// </summary>
		private void SolveGender()
		{
			// Ищем, может где-то пол уже установлен
			int length = words.Length;
			for (int i = 0; i < length; i++)
			{
				if (words.GetWord(i).isGenderSolved())
				{
					SetGender(words.GetWord(i).Gender);
					return;
				}
			}

			// Если нет, тогда определяем у каждого слова и потом суммируем
			GenderProbability probability = new GenderProbability(0, 0);

			for (int i = 0; i < length; i++)
			{
				Word word = words.GetWord(i);
				PrepareGender(word);
				probability = probability + word.GenderProbability;
			}

			if (probability.Man > probability.Woman)
			{
				SetGender(Gender.Man);
			}
			else
			{
				SetGender(Gender.Woman);
			}
		}

		/// <summary>
		/// Выполняет все необходимые подготовления для склонения.
		/// Все слова идентифицируются. Определяется пол.
		/// </summary>
		private void PrepareEverything()
		{
			if (!ready)
			{
				PrepareAllFioParts();
				SolveGender();
				ready = true;
			}
		}

		/// <summary>
		/// По указаным словам определяется пол
		/// </summary>
		/// <returns>Пол</returns>
		public Gender GenderAutoDetect()
		{
			PrepareEverything();
			if (words.Length > 0)
			{
				return words.GetWord(0).Gender;
			}
			else
			{
				return Gender.Null;
			}
		}

		/// <summary>
		/// Разделяет слова на части и готовит к дальнейшему склонению
		/// </summary>
		/// <param name="fullname">Строка содержащая полное имя</param>
		private void SplitFullName(string fullname)
		{
			string[] arr = fullname.Trim().Split(new Char[] { ' ' });
			int length = arr.Length;

			words = new WordArray();
			for (int i = 0; i < length; i++)
			{
				if (arr[i] != "")
				{
					words.AddWord(new Word(arr[i]));
				}
			}
		}

		/// <summary>
		/// Склоняет слово по нужным правилам
		/// </summary>
		/// <param name="word">Слово</param>
		protected void WordCase(Word word)
		{
			Gender gender = word.Gender;
			string genderName = (gender == Gender.Man ? "Man" : "Woman");

			string fioPartName = "";
			switch (word.FioPart)
			{
				case FioPart.Name: fioPartName = "Name"; break;
				case FioPart.SurName: fioPartName = "SurName"; break;
				case FioPart.PatrName: fioPartName = "PatrName"; break;
			}

			string methodName = genderName + fioPartName;
			SetWorkingWord(word.Value);

			bool res = (bool)this.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(this, null);
			if (res)
			{
				word.NameCases = lastResult;
				word.Rule = lastRule;
			}
			else
			{
				MakeResultTheSame();
				word.NameCases = lastResult;
				word.Rule = -1;
			}
		}

		/// <summary>
		/// Производит склонение всех слов
		/// </summary>
		private void AllWordCases()
		{
			if (!finished)
			{
				PrepareEverything();
				int length = words.Length;

				for (int i = 0; i < length; i++)
				{
					WordCase(words.GetWord(i));
				}

				finished = true;
			}
		}

		/// <summary>
		/// Возвращает массив, содержащий все падежи имени
		/// </summary>
		/// <returns>Возвращает массив со всеми падежами имени</returns>
		public string[] GetNameCase()
		{
			AllWordCases();
			return words.GetByFioPart(FioPart.Name).NameCases;
		}

		/// <summary>
		/// Возвращает имя в определенном падеже
		/// </summary>
		/// <param name="caseNum">Падеж</param>
		/// <returns>Имя в определенном падеже</returns>
		public string GetNameCase(Padeg caseNum)
		{
			AllWordCases();
			return words.GetByFioPart(FioPart.Name).GetNameCase(caseNum);
		}

		/// <summary>
		/// Возвращает массив, содержащий все падежи фамилии
		/// </summary>
		/// <returns>Возвращает массив со всеми падежами фамилии</returns>
		public string[] GetSurNameCase()
		{
			AllWordCases();
			return words.GetByFioPart(FioPart.SurName).NameCases;
		}

		/// <summary>
		/// Возвращает фамилию в определенном падеже
		/// </summary>
		/// <param name="caseNum">Падеж</param>
		/// <returns>Фамилия в определенном падеже</returns>
		public string GetSurNameCase(Padeg caseNum)
		{
			AllWordCases();
			return words.GetByFioPart(FioPart.SurName).GetNameCase(caseNum);
		}

		/// <summary>
		/// Возвращает массив, содержащий все падежи отчества
		/// </summary>
		/// <returns>Возвращает массив со всеми падежами отчества</returns>
		public string[] GetPatrNameCase()
		{
			AllWordCases();
			return words.GetByFioPart(FioPart.PatrName).NameCases;
		}

		/// <summary>
		/// Возвращает отчество в определенном падеже
		/// </summary>
		/// <param name="caseNum">Падеж</param>
		/// <returns>Отчество в определенном падеже</returns>
		public string GetPatrNameCase(Padeg caseNum)
		{
			AllWordCases();
			return words.GetByFioPart(FioPart.PatrName).GetNameCase(caseNum);
		}

		/// <summary>
		/// Выполняет склонение имени
		/// </summary>
		/// <param name="name">Имя</param>
		/// <param name="gender">Пол</param>
		/// <returns>Массив со всеми падежами</returns>
		public string[] QName(string name, Gender gender)
		{
			FullReset();
			SetName(name);
			if (gender != Gender.Null)
			{
				SetGender(gender);
			}
			return GetNameCase();
		}

		/// <summary>
		/// Выполняет склонение имени
		/// </summary>
		/// <param name="name">Имя</param>
		/// <returns>Массив со всеми падежами</returns>
		public string[] QName(string name)
		{
			return QName(name, Gender.Null);
		}

		/// <summary>
		/// Выполняет склонение имени
		/// </summary>
		/// <param name="name">Имя</param>
		/// <param name="caseNum">Падеж</param>
		/// <param name="gender">Пол</param>
		/// <returns>Имя в указаном падеже</returns>
		public string QName(string name, Padeg caseNum, Gender gender)
		{
			FullReset();
			SetName(name);
			if (gender != Gender.Null)
			{
				SetGender(gender);
			}
			return GetNameCase(caseNum);
		}

		/// <summary>
		/// Выполняет склонение имени
		/// </summary>
		/// <param name="name">Имя</param>
		/// <param name="caseNum">Падеж</param>
		/// <returns>Имя в указаном падеже</returns>
		public string QName(string name, Padeg caseNum)
		{
			return QName(name, caseNum, Gender.Null);
		}

		/// <summary>
		/// Выполняет склонение фамилии
		/// </summary>
		/// <param name="surName">Фамилия</param>
		/// <param name="gender">Пол</param>
		/// <returns>Массив со всеми падежами</returns>
		public string[] QSurName(string surName, Gender gender)
		{
			FullReset();
			SetSurName(surName);
			if (gender != Gender.Null)
			{
				SetGender(gender);
			}
			return GetSurNameCase();
		}

		/// <summary>
		/// Выполняет склонение фамилии
		/// </summary>
		/// <param name="surName">Фамилия</param>
		/// <returns>Массив со всеми падежами</returns>
		public string[] QSurName(string surName)
		{
			return QSurName(surName, Gender.Null);
		}

		/// <summary>
		/// Выполняет склонение фамилии
		/// </summary>
		/// <param name="surName">Фамилия</param>
		/// <param name="caseNum">Падеж</param>
		/// <param name="gender">Пол</param>
		/// <returns>Фамилия в указаном падеже</returns>
		public string QSurName(string surName, Padeg caseNum, Gender gender)
		{
			FullReset();
			SetSurName(surName);
			if (gender != Gender.Null)
			{
				SetGender(gender);
			}
			return GetSurNameCase(caseNum);
		}

		/// <summary>
		/// Выполняет склонение фамилии
		/// </summary>
		/// <param name="surName">Фамилия</param>
		/// <param name="caseNum">Падеж</param>
		/// <returns>Фамилия в указаном падеже</returns>
		public string QSurName(string surName, Padeg caseNum)
		{
			return QSurName(surName, caseNum, Gender.Null);
		}

		/// <summary>
		/// Выполняет склонение отчества
		/// </summary>
		/// <param name="patrName">Отчество</param>
		/// <param name="gender">Пол</param>
		/// <returns>Массив со всеми падежами</returns>
		public string[] QPatrName(string patrName, Gender gender)
		{
			FullReset();
			SetPatrName(patrName);
			if (gender != Gender.Null)
			{
				SetGender(gender);
			}
			return GetPatrNameCase();
		}

		/// <summary>
		/// Выполняет склонение отчества
		/// </summary>
		/// <param name="patrName">Отчество</param>
		/// <returns>Массив со всеми падежами</returns>
		public string[] QPatrName(string patrName)
		{
			return QPatrName(patrName, Gender.Null);
		}

		/// <summary>
		/// Выполняет склонение отчества
		/// </summary>
		/// <param name="patrName">Отчество</param>
		/// <param name="caseNum">Падеж</param>
		/// <param name="gender">Пол</param>
		/// <returns>Фамилия в указаном падеже</returns>
		public string QPatrName(string patrName, Padeg caseNum, Gender gender)
		{
			FullReset();
			SetPatrName(patrName);
			if (gender != Gender.Null)
			{
				SetGender(gender);
			}
			return GetPatrNameCase(caseNum);
		}

		/// <summary>
		/// Выполняет склонение отчества
		/// </summary>
		/// <param name="patrName">Отчество</param>
		/// <param name="caseNum">Падеж</param>
		/// <returns>Фамилия в указаном падеже</returns>
		public string QPatrName(string patrName, Padeg caseNum)
		{
			return QPatrName(patrName, caseNum, Gender.Null);
		}

		/// <summary>
		/// Соединяет все слова, имеющиеся в системе, в одну строку в определенном падеже
		/// </summary>
		/// <param name="caseNum">Падеж</param>
		/// <returns>Соединённая строка</returns>
		private string ConnectedCase(Padeg caseNum)
		{
			int length = words.Length;
			string result = "";
			for (int i = 0; i < length; i++)
			{
				result += words.GetWord(i).GetNameCase(caseNum) + " ";
			}
			return result.TrimEnd();
		}

		/// <summary>
		/// Соединяет все слова, имеющиеся в системе, в массив со всеми падежами
		/// </summary>
		/// <returns>Массив со всеми падежами</returns>
		private string[] ConnectedCases()
		{
			string[] res = new string[caseCount];
			for (int i = 0; i < caseCount; i++)
			{
				res[i] = ConnectedCase((Padeg)i);
			}

			return res;
		}

		/// <summary>
		/// Выполняет склонение полного имени
		/// </summary>
		/// <param name="fullName">Полное имя</param>
		/// <param name="gender">Пол</param>
		/// <returns>Массив со всеми падежами</returns>
		public string[] Q(string fullName, Gender gender)
		{
			FullReset();
			SplitFullName(fullName);
			if (gender != Gender.Null)
			{
				SetGender(gender);
			}
			AllWordCases();
			return ConnectedCases();
		}

		/// <summary>
		/// Выполняет склонение полного имени
		/// </summary>
		/// <param name="fullName">Полное имя</param>
		/// <returns>Массив со всеми падежами</returns>
		public string[] Q(string fullName)
		{
			return Q(fullName, Gender.Null);
		}

		/// <summary>
		/// Выполняет склонение полного имени
		/// </summary>
		/// <param name="fullName">Полное имя</param>
		/// <param name="caseNum">Падеж</param>
		/// <param name="gender">Пол</param>
		/// <returns>Строка в указаном падеже</returns>
		public string Q(string fullName, Padeg caseNum, Gender gender)
		{
			FullReset();
			SplitFullName(fullName);
			if (gender != Gender.Null)
			{
				SetGender(gender);
			}
			AllWordCases();
			return ConnectedCase(caseNum);
		}

		/// <summary>
		/// Выполняет склонение полного имени
		/// </summary>
		/// <param name="fullName">Полное имя</param>
		/// <param name="caseNum">Падеж</param>
		/// <returns>Строка в указаном падеже</returns>
		public string Q(string fullName, Padeg caseNum)
		{
			return Q(fullName, caseNum, Gender.Null);
		}

		/// <summary>
		/// Возвращает массив всех слов
		/// </summary>
		/// <returns>Массив всех слов</returns>
		public WordArray GetWordsArray()
		{
			return words;
		}

		/// <summary>
		/// Склонение имени по правилам мужских имен
		/// </summary>
		/// <returns>true - склонение было произведено, false - правило не найдено</returns>
		abstract protected bool ManName();

		/// <summary>
		/// Склонение имени по правилам женских имен
		/// </summary>
		/// <returns>true - склонение было произведено, false - правило не найдено</returns>
		abstract protected bool WomanName();

		/// <summary>
		/// Склонение фамилию по правилам мужских имен
		/// </summary>
		/// <returns>true - склонение было произведено, false - правило не найдено</returns>
		abstract protected bool ManSurName();

		/// <summary>
		/// Склонение фамилию по правилам женских имен
		/// </summary>
		/// <returns>true - склонение было произведено, false - правило не найдено</returns>
		abstract protected bool WomanSurName();

		/// <summary>
		/// Склонение отчества по правилам мужских имен
		/// </summary>
		/// <returns>true - склонение было произведено, false - правило не найдено</returns>
		abstract protected bool ManPatrName();

		/// <summary>
		/// Склонение отчества по правилам женских имен
		/// </summary>
		/// <returns>true - склонение было произведено, false - правило не найдено</returns>
		abstract protected bool WomanPatrName();

		/// <summary>
		/// Определяет пол человека по его имени
		/// </summary>
		/// <param name="word">Имя</param>
		abstract protected void GenderByName(Word word);

		/// <summary>
		/// Определяет пол человека по его фамилии
		/// </summary>
		/// <param name="word">Фамилия</param>
		abstract protected void GenderBySurName(Word word);

		/// <summary>
		/// Определяет пол человека по его отчеству
		/// </summary>
		/// <param name="word">Отчество</param>
		abstract protected void GenderByPatrName(Word word);

		/// <summary>
		/// Идентифицирует чем является слово: имен, фамилией или отчеством
		/// </summary>
		/// <param name="word">Слово, которое нужно идентифицировать</param>
		abstract protected void DetectFioPart(Word word);

		/// <summary>
		/// Возвращает текущую версию языкового файла
		/// </summary>
		static public string LanguageVersion
		{
			get
			{
				return languageBuild;
			}
		}
	}
}
