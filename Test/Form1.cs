using System;
using System.Windows.Forms;

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
				listBox1.Items.Clear();

				NameCaseLib.Ru ru = new NameCaseLib.Ru();
				listBox1.Items.Add("Именительный  (кто? что?): " + ru.Q(comboBox1.Text, NameCaseLib.NCL.Padeg.IMENITLN));
				listBox1.Items.Add("Родительный (кого? чего?): " + ru.Q(comboBox1.Text, NameCaseLib.NCL.Padeg.RODITLN));
				listBox1.Items.Add("Дательный   (кому? чему?): " + ru.Q(comboBox1.Text, NameCaseLib.NCL.Padeg.DATELN));
				listBox1.Items.Add("Винительный  (кого? что?): " + ru.Q(comboBox1.Text, NameCaseLib.NCL.Padeg.VINITELN));
				listBox1.Items.Add("Творительный  (кем? чем?): " + ru.Q(comboBox1.Text, NameCaseLib.NCL.Padeg.TVORITELN));
				listBox1.Items.Add("Предложны (о ком? о чём?): " + ru.Q(comboBox1.Text, NameCaseLib.NCL.Padeg.PREDLOGN));
			}
		}

		private void comboBox1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				button1.PerformClick();
			}
		}
	}
}
