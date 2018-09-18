using System;
using System.Collections.Generic;

namespace NameCaseLib
{
	/// <summary>
	/// Русские правила склонения ФИО
	/// Правила определения пола по ФИО для русского языка
	/// Система разделения фамилий, имен и отчеств для русского языка
	/// </summary>
	public class Ru : Core
	{
		/// <summary>
		/// Версия языкового файла
		/// </summary>
		public static string LanguageBuild = "20180918-1"; // Формат: ГодМесяцДень-номерИзменения

		/// <summary>
		/// Количество падежей в языке
		/// </summary>
		protected new int caseCount = 6;

		/// <summary>
		/// Список гласных русского языка
		/// </summary>
		private string vowels = "аеёиоуыэюя";

		/// <summary>
		/// Список согласных русского языка
		/// </summary>
		private string consonant = "бвгджзйклмнпрстфхцчшщ";

		/// <summary>
		/// Окончания несклоняемых имен и фамилий
		/// </summary>
		private string[] ovo = new string[] { "ово", "аго", "яго", "ирь" };

		/// <summary>
		/// Окончания неслоняемых имен и фамилий
		/// </summary>
		private string[] ih = new string[] { "их", "ых", "ко" };

		/// <summary>
		/// Создание текущего объекта
		/// </summary>
		public Ru()
		{
			base.caseCount = this.caseCount;
		}

		private Dictionary<string, string> splitSecondExclude = new Dictionary<string, string>()
		{
			{"а", "взйкмнпрстфя"},
			{"б", "а"},
			{"в", "аь"},
			{"г", "а"},
			{"д", "ар"},
			{"е", "бвгдйлмня"},
			{"ё", "бвгдйлмня"},
			{"ж", ""},
			{"з", "а"},
			{"и", "гдйклмнопрсфя"},
			{"й", "ля"},
			{"к", "аст"},
			{"л", "аилоья"},
			{"м", "аип"},
			{"н", "ат"},
			{"о", "вдлнпртя"},
			{"п", "п"},
			{"р", "адикпть"},
			{"с", "атуя"},
			{"т", "аор"},
			{"у", "дмр"},
			{"ф", "аь"},
			{"х", "а"},
			{"ц", "а"},
			{"ч", ""},
			{"ш", "а"},
			{"щ", ""},
			{"ъ", ""},
			{"ы", "дн"},
			{"ь", "я"},
			{"э", ""},
			{"ю", ""},
			{"я", "нс"}
		};

		/// <summary>
		/// Мужские имена, оканчивающиеся на "ьй", 
		/// скло­няются так же, как обычные существительные мужского рода
		/// </summary>
		/// <returns>true - правило было задействовано</returns>
		protected bool ManRule1()
		{
			if (In(Last(1), "ьй"))
			{
				if (Last(2, 1) != "и")
				{
					WordForms(workingWord, new string[] { "я", "ю", "я", "ем", "е" }, 1);
					Rule(101);
					return true;
				}
				else
				{
					WordForms(workingWord, new string[] { "я", "ю", "я", "ем", "и" }, 1);
					Rule(102);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Мужские имена, оканчивающиеся на любой твердый согласный, 
		/// склоняются так же, как обычные существительные мужского рода
		/// </summary>
		/// <returns>true - правило было задействовано</returns>
		protected bool ManRule2()
		{
			if (In(Last(1), consonant))
			{
				if (InNames(workingWord, "павел"))
				{
					lastResult = new string[] { "павел", "павла", "павлу", "павла", "павлом", "павле" };
					Rule(201);
					return true;
				}
				else if (InNames(workingWord, "лев"))
				{
					lastResult = new string[] { "лев", "льва", "льву", "льва", "львом", "льве" };
					Rule(202);
					return true;
				}
				else
				{
					WordForms(workingWord, new string[] { "а", "у", "а", "ом", "е" });
					Rule(203);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Мужские имена, оканчивающиеся на "а", склоняются так же, как и любые 
		/// существительные с таким же окончанием.
		/// Мужские имена, оканчивающиеся на "я", "ья", "ия", "ея", независимо от языка, 
		/// из которого они происходят, склоняются как существительные с соответствующими окончаниями.
		/// </summary>
		/// <returns>true - правило было задействовано</returns>
		protected bool ManRule3()
		{
			if (Last(1) == "а")
			{
				if (!In(Last(2, 1), "кшгх"))
				{
					WordForms(workingWord, new string[] { "ы", "е", "у", "ой", "е" }, 1);
					Rule(301);
					return true;
				}
				else
				{
					WordForms(workingWord, new string[] { "и", "е", "у", "ой", "е" }, 1);
					Rule(302);
					return true;
				}
			}
			else if (Last(1) == "я")
			{
				WordForms(workingWord, new string[] { "и", "е", "ю", "ей", "е" }, 1);
				Rule(303);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Мужские фамилии, оканчивающиеся на "ь", "й", склоняются так же, 
		/// как обычные существительные мужского рода.
		/// </summary>
		/// <returns>true - правило было задействовано</returns>
		protected bool ManRule4()
		{
			if (In(Last(1), "ьй"))
			{
				// Фамилии типа Воробей
				if (Last(3) == "бей")
				{
					WordForms(workingWord, new string[] { "ья", "ью", "ья", "ьем", "ье" }, 2);
					Rule(400);
					return true;
				}
				else if (Last(3, 1) == "а" || In(Last(2, 1), "ел"))
				{
					WordForms(workingWord, new string[] { "я", "ю", "я", "ем", "е" }, 1);
					Rule(401);
					return true;
				}
				// Толстой > Толстым 
				else if (Last(2, 1) == "ы" || Last(3, 1) == "т")
				{
					WordForms(workingWord, new string[] { "ого", "ому", "ого", "ым", "ом" }, 2);
					Rule(402);
					return true;
				}
				// Лесничий
				else if (Last(3) == "чий")
				{
					WordForms(workingWord, new string[] { "ьего", "ьему", "ьего", "ьим", "ьем" }, 2);
					Rule(403);
					return true;
				}
				else if (!In(Last(2, 1), vowels) || Last(2, 1) == "и")
				{
					WordForms(workingWord, new string[] { "ого", "ому", "ого", "им", "ом" }, 2);
					Rule(404);
					return true;
				}
				else
				{
					MakeResultTheSame();
					Rule(405);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Мужские фамилии, оканчивающиеся на "к"
		/// </summary>
		/// <returns>true - правило было задействовано</returns>
		protected bool ManRule5()
		{
			if (Last(1) == "к")
			{
				// Если перед слово на "ок", то нужно убрать "о"
				if (Last(2, 1) == "о")
				{
					WordForms(workingWord, new string[] { "ка", "ку", "ка", "ком", "ке" }, 2);
					Rule(501);
					return true;
				}
				if (Last(2, 1) == "е")
				{
					WordForms(workingWord, new string[] { "ька", "ьку", "ька", "ьком", "ьке" }, 2);
					Rule(502);
					return true;
				}
				else
				{
					WordForms(workingWord, new string[] { "а", "у", "а", "ом", "е" });
					Rule(503);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Мужские фамилии на согласный выбираем ем/ом/ым
		/// </summary>
		/// <returns>true - правило было задействовано</returns>
		protected bool ManRule6()
		{
			if (Last(1) == "ч")
			{
				WordForms(workingWord, new string[] { "а", "у", "а", "ем", "е" });
				Rule(601);
				return true;
			}
			// "е" перед "ц" выпадает
			else if (Last(2) == "ец")
			{
				WordForms(workingWord, new string[] { "ца", "цу", "ца", "цом", "це" }, 2);
				Rule(604);
				return true;
			}
			else if (In(Last(1), "цсршмхт"))
			{
				WordForms(workingWord, new string[] { "а", "у", "а", "ом", "е" });
				Rule(602);
				return true;
			}
			else if (In(Last(1), consonant))
			{
				WordForms(workingWord, new string[] { "а", "у", "а", "ым", "е" });
				Rule(603);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Мужские фамилии, оканчивающиеся на "а", "я"
		/// </summary>
		/// <returns>true - правило было задействовано</returns> 
		protected bool ManRule7()
		{
			if (Last(1) == "а")
			{
				// Если основа оканчивается на "ш", то нужно "и", "ей"
				if (Last(2, 1) == "ш")
				{
					WordForms(workingWord, new string[] { "и", "е", "у", "ей", "е" }, 1);
					Rule(701);
					return true;
				}
				else if (In(Last(2, 1), "хкг"))
				{
					WordForms(workingWord, new string[] { "и", "е", "у", "ой", "е" }, 1);
					Rule(702);
					return true;
				}
				else
				{
					WordForms(workingWord, new string[] { "ы", "е", "у", "ой", "е" }, 1);
					Rule(703);
					return true;
				}
			}
			else if (Last(1) == "я")
			{
				WordForms(workingWord, new string[] { "ой", "ой", "ую", "ой", "ой" }, 2);
				Rule(704);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Несклоняемые мужские фамилии
		/// </summary>
		/// <returns>true - правило было задействовано</returns> 
		protected bool ManRule8()
		{
			if (In(Last(3), ovo) || In(Last(2), ih))
			{
				Rule(8);
				MakeResultTheSame();
				return true;
			}
			return false;
		}

		/// <summary>
		/// Женские имена, оканчивающиеся на "а", склоняются, 
		/// как и любые существительные с таким же окончанием
		/// </summary>
		/// <returns>true - правило было задействовано</returns>
		protected bool WomanRule1()
		{
			if (Last(1) == "а" && Last(2, 1) != "и")
			{
				if (!In(Last(2, 1), "шхкг"))
				{
					WordForms(workingWord, new string[] { "ы", "е", "у", "ой", "е" }, 1);
					Rule(101);
					return true;
				}
				else
				{
					// "ей" после "ш"
					if (Last(2, 1) == "ш")
					{
						WordForms(workingWord, new string[] { "и", "е", "у", "ей", "е" }, 1);
						Rule(102);
						return true;
					}
					else
					{
						WordForms(workingWord, new string[] { "и", "е", "у", "ой", "е" }, 1);
						Rule(103);
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Женские имена, оканчивающиеся на "я", "ья", "ия", "ея", независимо от языка, 
		/// из которого они происходят, склоняются как сущест­вительные с соответствующими окончаниями
		/// </summary>
		/// <returns>true - правило было задействовано</returns> 
		protected bool WomanRule2()
		{
			if (Last(1) == "я")
			{
				if (Last(2, 1) != "и")
				{
					WordForms(workingWord, new string[] { "и", "е", "ю", "ей", "е" }, 1);
					Rule(201);
					return true;
				}
				else
				{
					// Имена: Ия и Лия
					if (InNames(workingWord, "ия") || InNames(workingWord, "лия"))
						WordForms(workingWord, new string[] { "и", "е", "ю", "ей", "е" }, 1);
					else
						WordForms(workingWord, new string[] { "и", "и", "ю", "ей", "и" }, 1);
					Rule(202);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Русские женские имена, оканчивающиеся на мягкий согласный, склоняются, 
		/// как существительные женского рода
		/// </summary>
		/// <returns>true - правило было задействовано</returns>
		protected bool WomanRule3()
		{
			if (Last(1) == "ь")
			{
				WordForms(workingWord, new string[] { "и", "и", "ь", "ью", "и" }, 1);
				Rule(3);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Женские фамилия, оканчивающиеся на "а", "я", склоняются,
		/// как и любые существительные с таким же окончанием
		/// </summary>
		/// <returns>true - правило было задействовано</returns>
		protected bool WomanRule4()
		{
			if (Last(1) == "а")
			{
				if (In(Last(2, 1), "гк"))
				{
					WordForms(workingWord, new string[] { "и", "е", "у", "ой", "е" }, 1);
					Rule(401);
					return true;
				}
				else if (In(Last(2, 1), "ш"))
				{
					WordForms(workingWord, new string[] { "и", "е", "у", "ей", "е" }, 1);
					Rule(402);
					return true;
				}
				else
				{
					WordForms(workingWord, new string[] { "ой", "ой", "у", "ой", "ой" }, 1);
					Rule(403);
					return true;
				}
			}
			else if (Last(1) == "я")
			{
				WordForms(workingWord, new string[] { "ой", "ой", "ую", "ой", "ой" }, 2);
				Rule(404);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Функция пытается применить цепочку правил для мужских имен
		/// </summary>
		/// <returns>true - если было использовано правило из списка, false - если правило не найдено</returns>
		protected override bool ManName()
		{
			return RulesChain(Gender.Man, new int[] { 1, 2, 3 });
		}

		/// <summary>
		/// Функция пытается применить цепочку правил для женских имен
		/// </summary>
		/// <returns>true - если было использовано правило из списка, false - если правило не найдено</returns>
		protected override bool WomanName()
		{
			return RulesChain(Gender.Woman, new int[] { 1, 2, 3 });
		}

		/// <summary>
		/// Функция пытается применить цепочку правил для мужских фамилий
		/// </summary>
		/// <returns>true - если было использовано правило из списка, false - если правило не найдено</returns>
		protected override bool ManSurName()
		{
			return RulesChain(Gender.Man, new int[] { 8, 4, 5, 6, 7 });
		}

		/// <summary>
		/// Функция пытается применить цепочку правил для женских фамилий
		/// </summary>
		/// <returns>true - если было использовано правило из списка, false - если правило не найдено</returns>
		protected override bool WomanSurName()
		{
			return RulesChain(Gender.Woman, new int[] { 4 });
		}

		/// <summary>
		/// Функция склоняет мужские отчества
		/// </summary>
		/// <returns>true - если слово было успешно изменено, false - если не получилось этого сделать</returns>
		protected override bool ManPatrName()
		{
			// Проверяем: действительно ли отчество?
			if (InNames(workingWord, "ильич"))
			{
				WordForms(workingWord, new string[] { "а", "у", "а", "ом", "е" });
				return true;
			}
			else if (Last(2) == "ич")
			{
				WordForms(workingWord, new string[] { "а", "у", "а", "ем", "е" });
				return true;
			}
			return false;
		}

		/// <summary>
		/// Функция склоняет женские отчества
		/// </summary>
		/// <returns>true - если слово было успешно изменено, false - если не получилось этого сделать</returns>
		protected override bool WomanPatrName()
		{
			// Проверяем: действительно ли отчество?
			if (Last(2) == "на")
			{
				WordForms(workingWord, new string[] { "ы", "е", "у", "ой", "е" }, 1);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Определение пола по правилам имен
		/// </summary>
		/// <param name="word">обьект класса слов, для которого нужно определить пол</param>
		protected override void GenderByName(Word word)
		{
			SetWorkingWord(word.Value);

			GenderProbability prob = new GenderProbability();

			// Попробуем выжать максимум из имени
			// Если имя заканчивается на "й", то скорее всего мужчина
			if (Last(1) == "й")
				prob.Man += 0.9f;

			if (In(Last(2), new string[] { "он", "ов", "ав", "ам", "ол", "ан", "рд", "мп" }))
				prob.Man += 0.3f;

			if (In(Last(1), consonant))
				prob.Man += 0.01f;

			if (Last(1) == "ь")
				prob.Man += 0.02f;

			if (In(Last(2), new string[] { "вь", "фь", "ль" }))
				prob.Woman += 0.1f;

			if (In(Last(2), new string[] { "ла" }))
				prob.Woman += 0.04f;

			if (In(Last(2), new string[] { "то", "ма" }))
				prob.Man += 0.01f;

			if (In(Last(3), new string[] { "лья", "вва", "ока", "ука", "ита" }))
				prob.Man += 0.2f;

			if (In(Last(3), new string[] { "има" }))
				prob.Woman += 0.15f;

			if (In(Last(3), new string[] { "лия", "ния", "сия", "дра", "лла", "кла", "опа" }))
				prob.Woman += 0.5f;

			if (In(Last(4), new string[] { "льда", "фира", "нина", "лита", "алья" }))
				prob.Woman += 0.5f;

			word.GenderProbability = prob;
		}

		/// <summary>
		/// Определение пола по правилам фамилий
		/// </summary>
		/// <param name="word">обьект класса слов, для которого нужно определить пол</param>
		protected override void GenderBySurName(Word word)
		{
			SetWorkingWord(word.Value);

			GenderProbability prob = new GenderProbability();

			if (In(Last(2), new string[] { "ов", "ин", "ев", "ий", "ёв", "ый", "ын", "ой" }))
				prob.Man += 0.4f;

			if (In(Last(3), new string[] { "ова", "ина", "ева", "ёва", "ына", "мин" }))
				prob.Woman += 0.4f;

			if (In(Last(2), new string[] { "ая" }))
				prob.Woman += 0.4f;

			word.GenderProbability = prob;
		}

		/// <summary>
		/// Определение пола по правилам отчеств
		/// </summary>
		/// <param name="word">обьект класса слов, для которого нужно определить пол</param>
		protected override void GenderByPatrName(Word word)
		{
			SetWorkingWord(word.Value);

			if (Last(2) == "ич")
				word.GenderProbability = new GenderProbability(10, 0); // мужчина

			if (Last(2) == "на")
				word.GenderProbability = new GenderProbability(0, 12); // женщина
		}

		/// <summary>
		/// Определяет, чем является слово: именем, фамилией или отчеством
		/// </summary>
		/// <param name="word">слово, которое необходимо определить</param>
		protected override void DetectFioPart(Word word)
		{
			SetWorkingWord(word.Value);

			// Считаем вероятность
			float first = 0;
			float surname = 0;
			float patr = 0;

			// Если смахивает на отчество
			if (word.Position > 1 && In(Last(3), new string[] { "вна", "чна", "вич", "ьич" }))
				patr += 3;

			if (In(Last(2), new string[] { "ша" }))
				first += 0.5f;

			// Буквы, на которые никогда не заканчиваются имена
			if (In(Last(1), "еёжхцочшщъыэю"))
				surname += 0.3f;

			// Используем массив характерных окончаний
			if (In(Last(2, 1), vowels + consonant))
			{
				if (!In(Last(1), splitSecondExclude[Last(2, 1)]))
					surname += 0.4f;
			}

			// Сокращённые ласкательные имена типа Аня, Галя и т.д.
			if (Last(1) == "я" && In(Last(3, 1), vowels))
				first += 0.5f;

			// Не бывает имен с такими предпоследними буквами
			if (In(Last(2, 1), "жчщъэю"))
				surname += 0.3f;

			// Слова на мягкий знак. Существует очень мало имен на мягкий знак. Всё остальное фамилии
			if (Last(1) == "ь")
			{
				// Имена типа Нинель, Адель, Асель
				if (Last(3, 2) == "ел")
					first += 0.7f;

				// Просто исключения
				else if (InNames(word.Value, new string[] { "лазарь", "игорь", "любовь" }))
					first += 10;

				// Если не то и не другое, тогда фамилия
				else
					surname += 0.3f;
			}

			// Если две последних букв согласные, то скорее всего это фамилия
			else if (In(Last(1), consonant + "ь") && In(Last(2, 1), consonant + "ь"))
			{
				// Практически все кроме тех, которые оканчиваются на следующие буквы
				if (!In(Last(2), new string[] { "др", "кт", "лл", "пп", "рд", "рк", "рп", "рт", "тр" }))
					surname += 0.25f;
			}

			// Слова, которые оканчиваются на "тин"
			if (Last(3) == "тин" && In(Last(4, 1), "нст"))
				first += 0.5f;

			// Исключения
			if (InNames(word.Value, new string[] { "лев", "яков", "маша", "ольга", "еремей", "исак", "исаак", "ева", "ирина", "элькин", "мерлин" }))
				first += 10;

			// Фамилии, оканчивающиеся на "ли" (кроме имён "Натали" и т.д.)
			if (Last(2) == "ли" && Last(3, 1) != "а")
				surname += 0.4f;

			// Фамилии, оканчивающиеся на "ян" (кроме Касьян, Куприян, Ян и т.д.)
			if (Last(2) == "ян" && word.Value.Length > 2 && !In(Last(3, 1), "ьи"))
				surname += 0.4f;

			// Фамилии, оканчивающиеся на "ур" (кроме имен Артур, Тимур)
			if (Last(2) == "ур")
			{
				if (!InNames(word.Value, new string[] { "артур", "тимур" }))
					surname += 0.4f;
			}

			// Разбор ласкательных имен на "ик"
			if (Last(2) == "ик")
			{
				// Ласкательные буквы перед "ик"
				if (In(Last(3, 1), "лшхд"))
					first += 0.3f;
				else
					surname += 0.4f;
			}

			// Разбор имен и фамилий, оканчивающихся на "ина"
			if (Last(3) == "ина")
			{
				// Все похожие на Катерина и Кристина
				if (In(Last(7), new string[] { "атерина", "ристина" }))
					first += 10;

				// Исключения
				else if (InNames(word.Value, new string[] { "мальвина", "антонина", "альбина", "агриппина", "фаина", "карина", "марина", "валентина", "калина", "аделина", "алина", "ангелина", "галина", "каролина", "павлина", "полина", "элина", "мина", "нина" }))
					first += 10;

				// Иначе фамилия
				else
					surname += 0.4f;
			}

			// Имена типа Николай
			if (Last(4) == "олай")
				first += 0.6f;

			// Фамильные окончания
			if (In(Last(2), new string[] { "ов", "ин", "ев", "ёв", "ый", "ын", "ой", "ук", "як", "ца", "ун", "ок", "ая", "га", "ёк", "ив", "ус", "ак", "яр", "уз", "ах", "ай" }))
				surname += 0.4f;

			if (In(Last(3), new string[] { "ова", "ева", "ёва", "ына", "шен", "мей", "вка", "шир", "бан", "чий", "кий", "бей", "чан", "ган", "ким", "кан", "мар" }))
				surname += 0.4f;

			if (In(Last(4), new string[] { "шена" }))
				surname += 0.4f;

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
