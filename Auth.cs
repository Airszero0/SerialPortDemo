using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace serialPortTest
{
    public partial class Auth : Form
    {
        public Auth()
        {
            InitializeComponent();
        }

        private void hideAll()
        {
            Control.ControlCollection controlCollection = panel1.Controls;
            foreach (Control item in controlCollection)
            {
                item.Visible = false;
                item.Enabled = false;
            }
            panel1.Visible = false;
            panel1.Enabled = false;
        }

        private void showAll()
        {
            Control.ControlCollection controlCollection = panel1.Controls;
            foreach (Control item in controlCollection)
            {
                item.Visible = true;
                item.Enabled = true;
            }
            panel1.Visible = true;
            panel1.Enabled = true;

            GetDecodeContent();
        }

        private void Auth_Load(object sender, EventArgs e)
        {
            hideAll();
        }
        private static int testCount = 0;
        private void Button1_Click(object sender, EventArgs e)
        {
            string code = textBox1.Text.Trim();

            if (code.Equals("yzgkfn_!@#22"))
            {
                showAll();
            }
            else
            {
                textBox1.Text = "验证失败";
                testCount++;
            }
            if (testCount == 5)
            {
                testCount = 0;
                this.Close();
            }
        }
        public static string path = Environment.CurrentDirectory + @"\TextFile1.txt";
        public static string key  = "KopId90z";

        private void GetDecodeContent()
        {     
            string content = string.Empty;
            if (File.Exists(path))
            {
                content = File.ReadAllText(path);
            }
            string _afterContent = Helper.symmetry_Decode(content, key);
            _afterContent = _afterContent.TrimEnd(';');
            foreach (var item in _afterContent.Split(';'))
            {
                if (item.Contains("Temp")) {
                    textBox2.Text = item.Trim().Split(':')[1];
                }
                if (item.Contains("Hum"))
                {
                    textBox3.Text = item.Trim().Split(':')[1];
                }
                if (item.Contains("Lig"))
                {
                    textBox4.Text = item.Trim().Split(':')[1];
                }
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        { 
            if (File.Exists(path)) {
                string content = string.Empty;
                content += "Temp:" + (string.IsNullOrWhiteSpace(textBox2.Text.Trim()) ? "" : textBox2.Text.Trim()) + ";";
                content += "Hum:" + (string.IsNullOrWhiteSpace(textBox2.Text.Trim()) ? "" : textBox3.Text.Trim()) + ";";
                content += "Lig:" + (string.IsNullOrWhiteSpace(textBox2.Text.Trim()) ? "" : textBox4.Text.Trim()) + ";";
                string ecrypt = Helper.symmetry_Encode(content, key);
                File.WriteAllText(path, ecrypt);
                label5.Text = DateTime.Now.ToString("HH:mm:ss") + " 保存成功"; 
            }

        }
    }
}
