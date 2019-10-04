using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace serialPortTest
{
    public partial class SerialPortParameter : Form
    {
        public SerialPortParameter()
        {
            InitializeComponent();
        }

        private void SerialPortParameter_Load(object sender, EventArgs e)
        {
            string[] names = SerialPort.GetPortNames();
            foreach (string name in names)
            {
                comboBox1.Items.Add(name);
            }
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 7;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 3;
            comboBox5.SelectedIndex = 1;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            GloableVirables.PortName = comboBox1.Text;
            GloableVirables.BuadRate = int.Parse(comboBox2.Text);
            GloableVirables.dataBits = int.Parse(comboBox4.Text);
            GloableVirables.parity = (Parity)comboBox3.SelectedIndex;
            GloableVirables.stopbits = (StopBits)comboBox5.SelectedIndex;
            this.Close();
        }
    }
}
