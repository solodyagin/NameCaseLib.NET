using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using NameCaseLib;

namespace Test
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(comboBox1.Text))
			{
				textBox1.Clear();
				textBox1.AppendText(string.Format("Версия библиотеки: {0}\r\n", Core.Version));
				if (radioButton1.Checked)
				{
					Ru lang = new Ru();
					textBox1.AppendText(string.Format("[Ru] Версия правил: {0}\r\n", Ru.LanguageBuild));
					textBox1.AppendText("\r\n");

					string[] m = lang.Q(comboBox1.Text);
					WordArray wa = lang.GetWordsArray();
					textBox1.AppendText(string.Format("Фамилия:  {0}\r\n", wa.GetByFioPart(FioPart.SurName).Value));
					textBox1.AppendText(string.Format("Имя:      {0}\r\n", wa.GetByFioPart(FioPart.Name).Value));
					textBox1.AppendText(string.Format("Отчество: {0}\r\n", wa.GetByFioPart(FioPart.PatrName).Value));
					textBox1.AppendText("\r\n");

					textBox1.AppendText(string.Format("Пол: {0}\r\n", lang.GenderAutoDetect().GetDescription()));
					textBox1.AppendText("\r\n");

					textBox1.AppendText("Падежи:\r\n");
					textBox1.AppendText(string.Format(" Именительный   (кто? что?): {0}\r\n", m[0]));
					textBox1.AppendText(string.Format(" Родительный  (кого? чего?): {0}\r\n", m[1]));
					textBox1.AppendText(string.Format(" Дательный    (кому? чему?): {0}\r\n", m[2]));
					textBox1.AppendText(string.Format(" Винительный   (кого? что?): {0}\r\n", m[3]));
					textBox1.AppendText(string.Format(" Творительный   (кем? чем?): {0}\r\n", m[4]));
					textBox1.AppendText(string.Format(" Предложный (о ком? о чём?): {0}\r\n", m[5]));
				}
				else
				{
					Ua lang = new Ua();
					textBox1.AppendText(string.Format("[Ua] Версiя правил: {0}\r\n", Ua.LanguageBuild));
					textBox1.AppendText("\r\n");

					string[] m = lang.Q(comboBox1.Text);
					WordArray wa = lang.GetWordsArray();
					textBox1.AppendText(string.Format("Прізвище: {0}\r\n", wa.GetByFioPart(FioPart.SurName).Value));
					textBox1.AppendText(string.Format("Iм'я:     {0}\r\n", wa.GetByFioPart(FioPart.Name).Value));
					textBox1.AppendText(string.Format("Батькові: {0}\r\n", wa.GetByFioPart(FioPart.PatrName).Value));
					textBox1.AppendText("\r\n");

					Gender gender = lang.GenderAutoDetect();
					textBox1.AppendText(string.Format("Стать: {0}\r\n", (gender == Gender.Man) ? "Чоловіча" : "Жіноча"));
					textBox1.AppendText("\r\n");

					textBox1.AppendText("Відмінки:\r\n");
					textBox1.AppendText(string.Format(" Називний : {0}\r\n", m[0]));
					textBox1.AppendText(string.Format(" Родовий  : {0}\r\n", m[1]));
					textBox1.AppendText(string.Format(" Давальний: {0}\r\n", m[2]));
					textBox1.AppendText(string.Format(" Знахідний: {0}\r\n", m[3]));
					textBox1.AppendText(string.Format(" Орудний  : {0}\r\n", m[4]));
					textBox1.AppendText(string.Format(" Місцевий : {0}\r\n", m[5]));
					textBox1.AppendText(string.Format(" Кличний  : {0}\r\n", m[6]));
				}
			}
		}

		private void comboBox1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				button1.PerformClick();
		}
	}

	public static class EnumExtensions
	{
		public static string GetDescription(this Enum value)
		{
			return value
				.GetType()
				.GetMember(value.ToString())
				.FirstOrDefault()
				?.GetCustomAttribute<DescriptionAttribute>()
				?.Description;
		}
	}
}
