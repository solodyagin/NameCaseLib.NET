using System;

namespace NameCaseLib
{
	/// <summary>
	/// Класс для создания динамического массива слов
	/// </summary>
	public class WordArray
	{
		private int _capacity = 4;
		private Word[] _words;

		/// <summary>
		/// Создаем новый массив слов со стандартной длиной
		/// </summary>
		public WordArray()
		{
			_words = new Word[_capacity];
		}

		/// <summary>
		/// Получаем из массива слов слово с указаным индексом
		/// </summary>
		/// <param name="id">Индекс слова</param>
		/// <returns>объект класса Word</returns>
		public Word GetWord(int id)
		{
			return _words[id];
		}

		private void EnlargeArray()
		{
			Word[] tmp = new Word[_capacity * 2];
			Array.Copy(_words, tmp, Length);
			_words = tmp;
			_capacity *= 2;
		}

		/// <summary>
		/// Добавляем в массив слов новое слово
		/// </summary>
		/// <param name="word">объект класса Word</param>
		public void AddWord(Word word)
		{
			if (Length >= _capacity)
				EnlargeArray();
			_words[Length] = word;
			Length++;
		}

		/// <summary>
		/// Вовращает количество слов в массиве
		/// </summary>
		public int Length { get; private set; } = 0;

		/// <summary>
		/// Находит имя/фамилию/отчество среди слов в массиве
		/// </summary>
		/// <param name="fioPart">Что нужно найти</param>
		/// <returns>объект класса Word</returns>
		public Word GetByFioPart(FioPart fioPart)
		{
			for (int i = 0; i < Length; i++)
			{
				if (_words[i].FioPart == fioPart)
					return _words[i];
			}
			return new Word("");
		}
	}
}
