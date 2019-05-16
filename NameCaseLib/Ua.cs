﻿using System;
using System.Linq;

namespace NameCaseLib
{
	/// <summary>
	/// Украинские правила склонений ФИО. 
	/// Правила определения пола человека по ФИО для украинского языка
	/// Система разделения фамилий имен и отчеств для украинского языка 
	/// </summary>
	public class Ua : Core
	{
		/// <summary>
		/// Версия языкового файла
		/// </summary>
		public static string LanguageBuild = "20180918-1"; // Формат: ГодМесяцДень-номерИзменения

		/// <summary>
		/// Количество падежей в языке
		/// </summary>
		protected new int caseCount = 7;

		/// <summary>
		/// Список гласных украинского языка
		/// </summary>
		private readonly string vowels = "аеиоуіїєюя";

		/// <summary>
		/// Список согласных украинского языка
		/// </summary>
		private readonly string consonant = "бвгджзйклмнпрстфхцчшщ";

		/// <summary>
		/// Українські шиплячі приголосні 
		/// </summary>
		private readonly string shyplyachi = "жчшщ";

		/// <summary>
		/// Українські нешиплячі приголосні 
		/// </summary>
		private readonly string neshyplyachi = "бвгдзклмнпрстфхц";

		/// <summary>
		/// Українські завжди м’які звуки 
		/// </summary>
		private readonly string myaki = "ьюяєї";

		/// <summary>
		/// Українські губні звуки
		/// </summary>
		private readonly string gubni = "мвпбф";

		/// <summary>
		/// Создание текущего объекта
		/// </summary>
		public Ua()
		{
			base.caseCount = caseCount;
		}

		/// <summary>
		/// Чергування українських приголосних
		/// Чергування "г", "к", "х" —» "з", "ц", "с"
		/// <param name="letter">літера, яку необхідно перевірити на чергування</param>
		/// </summary>
		/// <returns>літера, де вже відбулося чергування</returns>
		private string inverseGKH(string letter)
		{
			switch (letter)
			{
				case "г": return "з";
				case "к": return "ц";
				case "х": return "с";
			}
			return letter;
		}

		/// <summary>
		/// Перевіряє чи символ є апострофом чи не є
		/// <param name="letter">симпол для перевірки</param>
		/// </summary>
		/// <returns>true якщо символ є апострофом</returns> 
		private bool isApostrof(string letter)
		{
			if (In(letter, " " + consonant + vowels))
				return false;
			return true;
		}

		/// <summary>
		/// Чергування українських приголосних
		/// Чергування г к —» ж ч
		/// @param string letter літера, яку необхідно перевірити на чергування
		/// <returns>літера, де вже відбулося чергування</returns> 
		/// </summary>
		private string inverse2(string letter)
		{
			switch (letter)
			{
				case "к": return "ч";
				case "г": return "ж";
			}
			return letter;
		}

		/// <summary>
		/// <b>Визначення групи для іменників 2-ї відміни</b>
		/// 1 - тверда
		/// 2 - мішана
		/// 3 - м’яка
		/// 
		/// <b>Правило:</b>
		/// - Іменники з основою на твердий нешиплячий належать до твердої групи: 
		///   береза, дорога, Дніпро, шлях, віз, село, яблуко.
		/// - Іменники з основою на твердий шиплячий належать до мішаної групи: 
		///   пожеж-а, пущ-а, тиш-а, алич-а, вуж, кущ, плющ, ключ, плече, прізвище.
		/// - Іменники з основою на будь-який м"який чи пом"якше­ний належать до м"якої групи: 
		///   земля [земл"а], зоря [зор"а], армія [арм"ійа], сім"я [с"імйа], серпень, фахівець, 
		///   трамвай, су­зір"я [суз"ірйа], насіння [насін"н"а], узвишшя Іузвиш"ш"а
		/// <param name="word">іменник, групу якого необхідно визначити</param>
		/// </summary>
		/// <returns>номер групи іменника</returns> 
		private int detect2Group(string word)
		{
			string osnova = word;
			string stack = "";
			// Ріжемо слово поки не зустрінемо приголосний і записуемо в стек всі голосні які зустріли
			while (In(Last(osnova, 1), vowels + "ь"))
			{
				stack = Last(osnova, 1) + stack;
				osnova = osnova.Substring(0, osnova.Length - 1);
			}
			int stacksize = stack.Length;
			string last = "Z"; // нульове закінчення
			if (stacksize > 0)
				last = stack.Substring(0, 1);

			string osnovaEnd = Last(osnova, 1);
			if (In(osnovaEnd, neshyplyachi) && !In(last, myaki))
				return 1;
			else if (In(osnovaEnd, shyplyachi) && !In(last, myaki))
				return 2;
			else
				return 3;
		}

		/// <summary>
		/// Шукаємо першу з кінця літеру з переліку
		/// </summary>
		/// <param name="word">Слово</param>
		/// <param name="vowels">Перелік літер</param>
		/// <returns>Перша літера з кінця</returns>
		private string FirstLastVowel(string word, string vowels)
		{
			for (int i = word.Length - 1; i > 0; i--)
			{
				string letter = word.Substring(i, 1);
				if (In(letter, vowels))
					return letter;
			}
			return "";
		}

		/// <summary>
		/// Отримуємо основу слова за правилами української мови
		/// </summary>
		/// <param name="word">Слово</param>
		/// <returns>Основа слова</returns>
		private string getOsnova(string word)
		{
			string osnova = word;
			// Ріжемо слово поки не зустрінемо приголосний
			while (In(Last(osnova, 1), vowels + "ь"))
				osnova = osnova.Substring(0, osnova.Length - 1);
			return osnova;
		}

		/// <summary>
		/// Українські чоловічі та жіночі імена, що в називному відмінку однини закінчуються на "а" ("я"),
		/// відмінються як відповідні іменники І відміни.
		/// <ul>
		/// <li>Примітка 1. Кінцеві приголосні основи "г", "к", "х" у жіночих іменах 
		///   у давальному та місцевому відмінках однини перед закінченням "і" 
		///   змінюються на "з", "ц", "с": Ольга - Ользі, Палажка - Палажці, Солоха - Солосі.</li>
		/// <li>Примітка 2. У жіночих іменах типу Одарка, Параска в родовому відмінку множини 
		///   в кінці основи між приголосними з"являється звук "о": Одарок, Парасок. </li>
		/// </ul>
		/// </summary>
		/// <returns>true - якщо було задіяно правило з переліку, false - якщо правило не знайдено</returns>
		protected bool ManRule1()
		{
			// Предпоследний символ
			string beforeLast = Last(2, 1);

			// Останні літера або "а"
			if (Last(1) == "а")
			{
				WordForms(workingWord, new[] { beforeLast + "и", inverseGKH(beforeLast) + "і", beforeLast + "у", beforeLast + "ою", inverseGKH(beforeLast) + "і", beforeLast + "о" }, 2);
				Rule(101);
				return true;
			}
			// Остання літера "я"
			else if (Last(1) == "я")
			{
				// Перед останньою літерою стоїть я
				if (beforeLast == "і")
				{
					WordForms(workingWord, new[] { "ї", "ї", "ю", "єю", "ї", "є" }, 1);
					Rule(102);
					return true;
				}
				else
				{
					WordForms(workingWord, new[] { beforeLast + "і", inverseGKH(beforeLast) + "і", beforeLast + "ю", beforeLast + "ею", inverseGKH(beforeLast) + "і", beforeLast + "е" }, 2);
					Rule(103);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Імена, що в називному відмінку закінчуються на "р", у родовому мають закінчення "а": 
		/// Віктор - Віктора, Макар - Макара, але: Ігор - Ігоря, Лазар - Лазаря.
		/// </summary>
		/// <returns>true - якщо було задіяно правило з переліку, false - якщо правило не знайдено</returns>
		protected bool ManRule2()
		{
			if (Last(1) == "р")
			{
				if (InNames(workingWord, new[] { "ігор", "лазар" }))
				{
					WordForms(workingWord, new[] { "я", "еві", "я", "ем", "еві", "е" });
					Rule(201);
					return true;
				}
				else
				{
					// Сидір
					string osnova = workingWord;
					// В украинском алфавите букве "і" соответсвует код Юникода = 0x0456
					if (new[] { char.ConvertFromUtf32(0x0456), "i" }.Contains(Last(osnova, 2, 1)))
						osnova = osnova.Substring(0, osnova.Length - 2) + "о" + Last(osnova, 1);
					//WordForms(osnova, new[] { "а", "ові", "а", "ом", "ові", "е" });
					WordForms(osnova, new[] { "а", "у", "а", "ом", "і", "е" });
					Rule(202);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Українські чоловічі імена, що в називному відмінку однини закінчуються на приголосний та "о", 
		/// відмінюються як відповідні іменники ІІ відміни.
		/// </summary>
		/// <returns>true - якщо було задіяно правило з переліку, false - якщо правило не знайдено</returns> 
		protected bool ManRule3()
		{
			// Предпоследний символ
			string beforeLast = Last(2, 1);

			if (In(Last(1), consonant + "оь"))
			{
				int group = detect2Group(workingWord);
				string osnova = getOsnova(workingWord);
				// В іменах типу Антін, Нестір, Нечипір, Прокіп, Сидір, Тиміш, Федір голосний "і" виступає тільки в 
				// називному відмінку, у непрямих - "о": Антона, Антонові
				// Чергування "і" -» "о" всередині
				string osLast = Last(osnova, 1);
				if (osLast != "й" && Last(osnova, 2, 1) == "і" && !In(Last(osnova, 4), new[] { "світ", "цвіт" }) && !InNames(workingWord, "гліб") && !In(Last(2), new[] { "ік", "іч" }))
					osnova = osnova.Substring(0, osnova.Length - 2) + "о" + Last(osnova, 1);

				// Випадання букви "е" при відмінюванні слів типу Орел
				//if (osnova != "" && osnova.Substring(0, 1) == "о" && FirstLastVowel(osnova, vowels + "гк") == "е" && Last(2) != "сь")
				//{
				//	int delim = osnova.LastIndexOf("е");
				//	osnova = osnova.Substring(0, delim) + osnova.Substring(delim + 1, osnova.Length - delim);
				//}
				if (InNames(workingWord, new[] { "орел" }))
				{
					int delim = osnova.LastIndexOf("е");
					osnova = osnova.Substring(0, delim) + osnova.Substring(delim + 1, osnova.Length - delim);
				}

				if (group == 1)
				{
					// Тверда група
					// Слова що закінчуються на "ок"
					if (Last(2) == "ок" && Last(3) != "оок")
					{
						WordForms(workingWord, new[] { "ка", "кові", "ка", "ком", "кові", "че" }, 2);
						Rule(301);
						return true;
					}
					// Російські прізвища на "ов", "ев", "єв"
					else if (In(Last(2), new[] { "ов", "ев", "єв" }) && !InNames(workingWord, new[] { "лев", "остромов" }))
					{
						WordForms(osnova, new[] { osLast + "а", osLast + "у", osLast + "а", osLast + "им", osLast + "у", inverse2(osLast) + "е" }, 1);
						Rule(302);
						return true;
					}
					// Російські прізвища на "ін"
					else if (In(Last(2), new[] { "ін" }))
					{
						WordForms(workingWord, new[] { "а", "у", "а", "ом", "у", "е" });
						Rule(303);
						return true;
					}
					else
					{
						//WordForms(osnova, new[] { osLast + "а", osLast + "ові", osLast + "а", osLast + "ом", osLast + "ові", inverse2(osLast) + "е" }, 1);
						WordForms(osnova, new[] { osLast + "а", osLast + "у", osLast + "а", osLast + "ом", osLast + "і", inverse2(osLast) + "е" }, 1);
						Rule(304);
						return true;
					}
				}
				if (group == 2)
				{
					// Мішана група
					WordForms(osnova, new[] { "а", "еві", "а", "ем", "еві", "е" });
					Rule(305);
					return true;
				}
				if (group == 3)
				{
					// М’яка група
					// Соловей
					if (Last(2) == "ей" && In(Last(3, 1), gubni))
					{
						osnova = workingWord.Substring(0, workingWord.Length - 2) + "’";
						WordForms(osnova, new[] { "я", "єві", "я", "єм", "єві", "ю" });
						Rule(306);
						return true;
					}
					else if (Last(1) == "й" || beforeLast == "і")
					{
						WordForms(workingWord, new[] { "я", "єві", "я", "єм", "єві", "ю" }, 1);
						Rule(307);
						return true;
					}
					// Швець
					else if (workingWord == "швець")
					{
						WordForms(workingWord, new[] { "евця", "евцеві", "евця", "евцем", "евцеві", "евцю" }, 4);
						Rule(308);
						return true;
					}
					// Слова що закінчуються на "ець"
					else if (Last(3) == "ець")
					{
						WordForms(workingWord, new[] { "ця", "цеві", "ця", "цем", "цеві", "цю" }, 3);
						Rule(309);
						return true;
					}
					// Слова що закінчуються на "єць", "яць"
					else if (In(Last(3), new[] { "єць", "яць" }))
					{
						WordForms(workingWord, new[] { "йця", "йцеві", "йця", "йцем", "йцеві", "йцю" }, 3);
						Rule(310);
						return true;
					}
					else
					{
						WordForms(osnova, new[] { "я", "еві", "я", "ем", "еві", "ю" });
						Rule(311);
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Якщо слово закінчується на "і", то відмінюємо як множину
		/// </summary>
		/// <returns>true - якщо було задіяно правило з переліку, false - якщо правило не знайдено</returns>
		protected bool ManRule4()
		{
			if (Last(1) == "і")
			{
				WordForms(workingWord, new[] { "их", "им", "их", "ими", "их", "і" }, 1);
				Rule(4);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Якщо слово закінчується на "ий" або "ой"
		/// </summary>
		/// <returns>true - якщо було задіяно правило з переліку, false - якщо правило не знайдено</returns> 
		protected bool ManRule5()
		{
			if (In(Last(2), new[] { "ий", "ой" }))
			{
				WordForms(workingWord, new[] { "ого", "ому", "ого", "им", "ому", "ий" }, 2);
				Rule(5);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Українські чоловічі та жіночі імена, що в називному відмінку однини закінчуються на "а" ("я"), 
		/// відмінються як відповідні іменники І відміни.  
		/// - Примітка 1. Кінцеві приголосні основи "г", "к", "х" у жіночих іменах 
		///   у давальному та місцевому відмінках однини перед закінченням "і" 
		///   змінюються на "з", "ц", "с": Ольга - Ользі, Палажка - Палажці, Солоха - Солосі.
		/// - Примітка 2. У жіночих іменах типу Одарка, Параска в родовому відмінку множини 
		///   в кінці основи між приголосними з"являється звук "о": Одарок, Парасок 
		/// </summary>
		/// <returns>true - якщо було задіяно правило з переліку, false - якщо правило не знайдено</returns> 
		protected bool WomanRule1()
		{
			// Предпоследний символ
			string beforeLast = Last(2, 1);

			// Якщо закінчується на "ніга" -» "нога"
			if (Last(4) == "ніга")
			{
				string osnova = workingWord.Substring(0, workingWord.Length - 3) + "о";
				WordForms(osnova, new[] { "ги", "зі", "гу", "гою", "зі", "го" });
				Rule(101);
				return true;
			}

			// Останні літера або "а"
			else if (Last(1) == "а")
			{
				WordForms(workingWord, new[] { beforeLast + "и", inverseGKH(beforeLast) + "і", beforeLast + "у", beforeLast + "ою", inverseGKH(beforeLast) + "і", beforeLast + "о" }, 2);
				Rule(102);
				return true;
			}
			// Остання літера "я"
			else if (Last(1) == "я")
			{
				if (In(beforeLast, vowels) || isApostrof(beforeLast))
				{
					WordForms(workingWord, new[] { "ї", "ї", "ю", "єю", "ї", "є" }, 1);
					Rule(103);
					return true;
				}
				else
				{
					WordForms(workingWord, new[] { beforeLast + "і", inverseGKH(beforeLast) + "і", beforeLast + "ю", beforeLast + "ею", inverseGKH(beforeLast) + "і", beforeLast + "е" }, 2);
					Rule(104);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Українські жіночі імена, що в називному відмінку однини закінчуються на приголосний, 
		/// відмінюються як відповідні іменники ІІІ відміни
		/// </summary>
		/// <returns>true - якщо було задіяно правило з переліку, false - якщо правило не знайдено</returns> 
		protected bool WomanRule2()
		{
			if (In(Last(1), consonant + "ь"))
			{
				string osnova = getOsnova(workingWord);
				string apostrof = "";
				string duplicate = "";
				string osLast = Last(osnova, 1);
				string osbeforeLast = Last(osnova, 2, 1);

				// Чи треба ставити апостроф
				if (In(osLast, "мвпбф") && In(osbeforeLast, vowels))
					apostrof = "’";

				// Чи треба подвоювати
				if (In(osLast, "дтзсцлн"))
					duplicate = osLast;

				// Відмінюємо
				if (Last(1) == "ь")
				{
					WordForms(osnova, new[] { "і", "і", "ь", duplicate + apostrof + "ю", "і", "е" });
					Rule(201);
					return true;
				}
				else
				{
					WordForms(osnova, new[] { "і", "і", "", duplicate + apostrof + "ю", "і", "е" });
					Rule(202);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Якщо слово на ськ або це російське прізвище
		/// </summary>
		/// <returns>true - якщо було задіяно правило з переліку, false - якщо правило не знайдено</returns> 
		protected bool WomanRule3()
		{
			// Предпоследний символ
			string beforeLast = Last(2, 1);

			// Донская
			if (Last(2) == "ая")
			{
				WordForms(workingWord, new[] { "ої", "ій", "ую", "ою", "ій", "ая" }, 2);
				Rule(301);
				return true;
			}

			// Ті що на ськ
			if (Last(1) == "а" && (In(Last(2, 1), "чнв") || In(Last(3, 2), new[] { "ьк" })))
			{
				WordForms(workingWord, new[] { beforeLast + "ої", beforeLast + "ій", beforeLast + "у", beforeLast + "ою", beforeLast + "ій", beforeLast + "о" }, 2);
				Rule(302);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Функція намагається застосувати ланцюг правил для чоловічих імен
		/// </summary>
		/// <returns>true - якщо було задіяно правило з переліку, false - якщо правило не знайдено</returns>
		protected override bool ManName()
		{
			return RulesChain(Gender.Man, new int[] { 1, 2, 3 });
		}

		/// <summary>
		/// Функція намагається застосувати ланцюг правил для жіночих імен
		/// </summary>
		/// <returns>true - якщо було задіяно правило з переліку, false - якщо правило не знайдено</returns>
		protected override bool WomanName()
		{
			return RulesChain(Gender.Woman, new int[] { 1, 2 });
		}

		/// <summary>
		/// Функція намагається застосувати ланцюг правил для чоловічих прізвищ
		/// </summary>
		/// <returns>true - якщо було задіяно правило з переліку, false - якщо правило не знайдено</returns>
		protected override bool ManSurName()
		{
			return RulesChain(Gender.Man, new int[] { 5, 1, 2, 3, 4 });
		}

		/// <summary>
		/// Функція намагається застосувати ланцюг правил для жіночих прізвищ
		/// </summary>
		/// <returns>true - якщо було задіяно правило з переліку, false - якщо правило не знайдено</returns>
		protected override bool WomanSurName()
		{
			return RulesChain(Gender.Woman, new int[] { 3, 1 });
		}

		/// <summary>
		/// Фунція відмінює чоловічі по-батькові
		/// </summary>
		/// <returns>true - якщо слово успішно змінене, false - якщо невдалося провідміняти слово</returns>
		protected override bool ManPatrName()
		{
			if (In(Last(2), new[] { "ич", "іч" }))
			{
				WordForms(workingWord, new[] { "а", "у", "а", "ем", "у", "у" });
				return true;
			}
			return false;
		}

		/// <summary>
		/// Фунція відмінює жіночі по-батькові
		/// </summary>
		/// <returns>true - якщо слово успішно змінене, false - якщо невдалося провідміняти слово</returns>
		protected override bool WomanPatrName()
		{
			if (In(Last(3), new[] { "вна" }))
			{
				WordForms(workingWord, new[] { "и", "і", "у", "ою", "і", "о" }, 1);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Визначення статі, за правилами імені
		/// <param name="word">Слово</param>
		/// </summary>
		protected override void GenderByName(Word word)
		{
			SetWorkingWord(word.Value);
			GenderProbability prob = new GenderProbability();

			// Попробуем выжать максимум из имени
			// Если имя заканчивается на "й", то скорее всего мужчина
			if (Last(1) == "й")
				prob.Man += 0.9f;

			if (InNames(workingWord, new[] { "петро", "микола" }))
				prob.Man += 30;

			if (In(Last(2), new[] { "он", "ов", "ав", "ам", "ол", "ан", "рд", "мп", "ко", "ло" }))
				prob.Man += 0.5f;

			if (In(Last(3), new[] { "бов", "нка", "яра", "ила", "опа" }))
				prob.Woman += 0.5f;

			if (In(Last(1), consonant))
				prob.Man += 0.01f;

			if (Last(1) == "ь")
				prob.Man += 0.02f;

			if (In(Last(2), new[] { "дь" }))
				prob.Woman += 0.1f;

			if (In(Last(3), new[] { "ель", "бов" }))
				prob.Woman += 0.4f;

			word.GenderProbability = prob;
		}

		/// <summary>
		/// Визначення статі, за правилами прізвища
		/// <param name="word">Слово</param>
		/// </summary>
		protected override void GenderBySurName(Word word)
		{
			SetWorkingWord(word.Value);
			GenderProbability prob = new GenderProbability();

			if (In(Last(2), new[] { "ов", "ин", "ев", "єв", "ін", "їн", "ий", "їв", "ів", "ой", "ей" }))
				prob.Man += 0.4f;

			if (In(Last(3), new[] { "ова", "ина", "ева", "єва", "іна", "мін" }))
				prob.Woman += 0.4f;

			if (In(Last(2), new[] { "ая" }))
				prob.Woman += 0.4f;

			word.GenderProbability = prob;
		}

		/// <summary>
		/// Визначення статі, за правилами по-батькові
		/// <param name="word">Слово</param>
		/// </summary>
		protected override void GenderByPatrName(Word word)
		{
			SetWorkingWord(word.Value);

			if (Last(2) == "ич")
				word.GenderProbability = new GenderProbability(10, 0); // мужчина

			if (Last(2) == "на")
				word.GenderProbability = new GenderProbability(0, 12); // женщина
		}

		/// <summary>
		/// Ідентифікує слово визначаючи чи це ім’я, чи це прізвище, чи це побатькові
		/// <param name="word">Слово</param>
		/// </summary>
		protected override void DetectFioPart(Word word)
		{
			SetWorkingWord(word.Value);

			// Считаем вероятность
			float first = 0;
			float surname = 0;
			float patr = 0;

			// Если смахивает на отчество
			if (In(Last(3), new[] { "вна", "чна", "ліч" }) || In(Last(4), new[] { "ьмич", "ович" }))
				patr += 3;

			// Похоже на имя
			if (In(Last(3), new[] { "тин" }) || In(Last(4), new[] { "ьмич", "юбов", "івна", "явка", "орив", "кіян" }))
				first += 0.5f;

			// Исключения
			if (InNames(word.Value, new[] { "лев", "гаїна", "афіна", "антоніна", "ангеліна", "альвіна", "альбіна", "аліна", "павло", "олесь", "микола", "мая", "англеліна", "елькін", "мерлін" }))
				first += 10;

			// Похоже на фамилию
			if (In(Last(2), new[] { "ов", "ін", "ев", "єв", "ий", "ин", "ой", "ко", "ук", "як", "ца", "их", "ик", "ун", "ок", "ша", "ая", "га", "єк", "аш", "ив", "юк", "ус", "це", "ак", "бр", "яр", "іл", "ів", "ич", "сь", "ей", "нс", "яс", "ер", "ай", "ян", "ах", "ць", "ющ", "іс", "ач", "уб", "ох", "юх", "ут", "ча", "ул", "вк", "зь", "уц", "їн", "де", "уз", "юр", "ік", "іч", "ро" }))
				surname += 0.4f;

			if (In(Last(3), new[] { "ова", "ева", "єва", "тих", "рик", "вач", "аха", "шен", "мей", "арь", "вка", "шир", "бан", "чий", "іна", "їна", "ька", "ань", "ива", "аль", "ура", "ран", "ало", "ола", "кур", "оба", "оль", "нта", "зій", "ґан", "іло", "шта", "юпа", "рна", "бла", "еїн", "има", "мар", "кар", "оха", "чур", "ниш", "ета", "тна", "зур", "нір", "йма", "орж", "рба", "іла", "лас", "дід", "роз", "аба", "чан", "ган" }))
				surname += 0.4f;

			if (In(Last(4), new[] { "ьник", "нчук", "тник", "кирь", "ский", "шена", "шина", "вина", "нина", "гана", "гана", "хній", "зюба", "орош", "орон", "сило", "руба", "лест", "мара", "обка", "рока", "сика", "одна", "нчар", "вата", "ндар", "грій" }))
				surname += 0.4f;

			if (Last(1) == "і")
				surname += 0.2f;

			float max = Math.Max(Math.Max(first, surname), patr);

			if (first == max)
				word.FioPart = FioPart.Name;
			else if (surname == max)
				word.FioPart = FioPart.SurName;
			else
				word.FioPart = FioPart.PatrName;
		}
	}
}
