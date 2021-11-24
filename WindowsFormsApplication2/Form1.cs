using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }
        public static int countEncoded = 0;
        public static double countnotencoded = 0;
        public double entropynotencoded;
        public double entropyencoded;
        int[] countofzerosandones = new int[2];
        public double izbitochnost_encoded;
        public double izbitochnost_notencoded;
        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.ShowDialog();
            string pathSave = saveFileDialog.FileName;
            string[] masforsave = new string[2];
            masforsave[0] = richTextBox1.Text;
            masforsave[1] = richTextBox2.Text;
            File.WriteAllLines(pathSave, masforsave);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.ShowDialog();
            string pathOpen = openFileDialog.FileName;
            string read;
            int index;
            string[] masforload = new string[2];
            read = File.ReadAllText(pathOpen);
            index = read.IndexOf(']');
            for (int i = 0; i < index+1; i++)
            {
                masforload[0] += read[i];
            }
            for( int k =index+1;k< read.Length-1;k++)
            {
                masforload[1] += read[k];
            }
         
            textBox1.Text = masforload[1];
            richTextBox1.Text = masforload[0];
            
            
        }

        private void btnEncode_Click(object sender, EventArgs e)
        {
            var result = ShennonFano.Encode(textBox1.Text);
            richTextBox1.Text = JsonConvert.SerializeObject(result.Item2);
            richTextBox2.Text = result.Item1;
            var EncodedTextJ = new List<byte>(textBox1.TextLength);
            for (int i = 0;i <textBox1.TextLength;i++)
            {
                EncodedTextJ.AddRange(BitConverter.GetBytes(textBox1.Text[i]));
            }
            countEncoded = EncodedTextJ.ToArray().Length;
            countnotencoded = richTextBox2.TextLength / 8;
            entropyencoded = 0;
            entropynotencoded = 0;
            
            var symbols = new List<Symbol>();
            foreach (var c in result.Item2)
            {
                entropyencoded += (double)1 / c.Probability * Math.Log((double)1/c.Probability, 2);
            }
            foreach (var item in richTextBox2.Text)
            {
                if (item == '0')
                {
                    countofzerosandones[0]++;
                }
                else
                {
                    countofzerosandones[1]++;
                }
            }
            entropynotencoded = ((double)1 / ((double)countofzerosandones[0] / richTextBox2.TextLength)) * Math.Log(((double)1 / ((double)countofzerosandones[0] / richTextBox2.TextLength)), 2) + ((double)1 / ((double)countofzerosandones[1] / richTextBox2.TextLength)) * Math.Log(((double)1 / ((double)countofzerosandones[1] / richTextBox2.TextLength)), 2);
            izbitochnost_encoded = (Convert.ToDouble(Math.Log(result.Item2.Count, 2)) - entropynotencoded) / Convert.ToDouble(Math.Log(result.Item2.Count, 2));
            izbitochnost_notencoded = Convert.ToDouble((1 - entropyencoded) / (Math.Log(richTextBox2.TextLength, 2)));


        }

        private void btnDecode_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(richTextBox1.Text))
            {
                MessageBox.Show("Отсутствует таблица кодировки.");
                return;
            }
            if (textBox1.Text.Any(x => x != '0' && x != '1'))
            {
                MessageBox.Show("Данные не в бинарном виде.");
                return;
            }

            var symbols = new List<Symbol>();

            try
            {
                symbols = JsonConvert.DeserializeObject<List<Symbol>>(richTextBox1.Text);
            }
            catch
            {
                MessageBox.Show("Неверная таблица кодировки."); return;
            }


            richTextBox2.Text = ShennonFano.Decode(textBox1.Text, symbols);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            richTextBox2.Clear();
            textBox1.Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add(Convert.ToString("Коэффицент сжатия равен " + Math.Round((countEncoded) / (countnotencoded),5)));
            listBox1.Items.Add(("Энтропия исходного текста = " + Math.Round(Math.Abs(entropynotencoded), 5))); 
            listBox1.Items.Add("Энтропия закодированного текста = " + Math.Round(Math.Abs(entropyencoded), 5));
            listBox1.Items.Add("Коэф избыточности  исходного текста = " + Math.Round(Math.Abs(izbitochnost_notencoded), 4)); 
            listBox1.Items.Add("Коэф избыточности закодированного текста = " + Math.Round(Math.Abs(izbitochnost_encoded), 5));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            string pathOpen = openFileDialog.FileName;
            using (StreamReader sr = new StreamReader(pathOpen, Encoding.GetEncoding(1251)))
            {
                textBox1.Text = sr.ReadToEnd();

            }

        }
    }
}
