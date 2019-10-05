using ExpressionEvaluator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace serialPortTest
{
    public partial class Form1 : Form
    {
        private string light1 = string.Empty;   //光照
        private double Temp = 0.0;     //温度
        private double Hum = 0.0;      //湿度
        private static int sb = 0;
        private static int rb = 0;

        private static string connectCom = "01 06 00 00 00 00 00 00 00 00 00 00";

        private static string humAndTempCom = "03 03 01 00 00 00 00 00 00 00 00 00";

        private static string lightCom = "01 03 02 00 00 00 00 00 00 00 00 00";

        private static string lightSetCom = "03 05 01 01 01 01 01 00 00 00 00 00";

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            SendBytes.Text = sb.ToString();
            ReceiveBytes.Text = rb.ToString();
            SendMsgBox.Text = connectCom;
            label19.Text = "0";
            label20.Text = "0";
            label21.Text = "0";
            timer1.Enabled = false;
            textBox2.Text = humAndTempCom;
            textBox3.Text = lightCom;
            checkBox5.Checked = true;

            string result = CRCHelper.ToModbusCRC16(SendMsgBox.Text, true);
            textBox1.Text = result;
            SendMsgBox.Text = SendMsgBox.Text + " " + result;

            textBox4.Text = "6";
            textBox5.Text = "8";
            textBox6.Text = "10";
            textBox9.Text = "12";
            textBox14.Text = "6";
            textBox15.Text = "8";

            textBox12.Text = "摄氏度";
            textBox13.Text = "%";
            textBox16.Text = "Lux";
            label42.Text = "摄氏度";
            label43.Text = "%";
            label44.Text = "Lux";
            GetMt();
            timer2.Start();

            //newestHaT = "02 03 01 06 56 78 46 00 00 00 00 00 38 87";
            //newestLight = "02 03 02 78 90 00 00 00 00 00 00 00 38 87";
        }

        private static string tb10 = string.Empty;
        private static string tb11 = string.Empty;
        private static string tb17 = string.Empty;

        private void GetMt()
        {
            string content = string.Empty;
            if (File.Exists(Auth.path))
            {
                content = File.ReadAllText(Auth.path);
            }
            string _afterContent = Helper.symmetry_Decode(content, Auth.key);
            _afterContent = _afterContent.TrimEnd(';');
            foreach (var item in _afterContent.Split(';'))
            {
                if (item.Contains("Temp"))
                {
                    tb10 = item.Trim().Split(':')[1];
                    textBox10.Text = tb10;
                }
                if (item.Contains("Hum"))
                {
                    tb11 = item.Trim().Split(':')[1];
                    textBox11.Text = tb11;
                }
                if (item.Contains("Lig"))
                {
                    tb17 = item.Trim().Split(':')[1];
                    textBox17.Text = tb17;
                }
            }
        }

        /// <summary>
        /// 清除发送内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Label13_Click(object sender, EventArgs e)
        {
            SendMsgBox.Text = string.Empty;
        }


        private void Label11_Click(object sender, EventArgs e)
        {
            ReceivingBox.Text = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button1_Click(object sender, EventArgs e)
        {
            (new SerialPortParameter()).ShowDialog();
            if (!string.IsNullOrWhiteSpace(GloableVirables.PortName))
                label6.Text = GloableVirables.PortName;

            if (GloableVirables.BuadRate != 0)
                label7.Text = GloableVirables.BuadRate.ToString();


            if (GloableVirables.dataBits != 0)
                label8.Text = GloableVirables.dataBits.ToString();


            label9.Text = GloableVirables.parity.ToString();
            label10.Text = GloableVirables.stopbits.ToString();
        }

        /// <summary>
        /// 打开或关闭串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button2_Click(object sender, EventArgs e)
        {
            if (!_openStatus)
            {
                if (!OpenParamsCheck())
                    return;
                OpenSerialPortWithParamSetting();
                if (_currentSerialPort != null && _currentSerialPort.IsOpen)
                    button2.Text = "关闭串口";
                _openStatus = true;
                PortStatus.Text = "串口"+ GloableVirables.PortName +"已连接";
                PortStatus.ForeColor = Color.Red;


            }
            else
            {
                if (_currentSerialPort != null && _currentSerialPort.IsOpen)
                {
                    _currentSerialPort.Close();
                    button2.Text = "打开串口";
                    _openStatus = false;
                    PortStatus.Text = "串口未连接";
                    PortStatus.ForeColor = Color.Black;
                }
            }
        }

        private SerialPort _currentSerialPort;
        private bool _openStatus = false;

        /// <summary>
        /// 打开串口
        /// </summary>
        private void OpenSerialPortWithParamSetting()
        {

            //初始化串口设置并开启
            if (_currentSerialPort != null)
            {
                if (_currentSerialPort.IsOpen)
                    _currentSerialPort.Close();
            }
            _currentSerialPort = new SerialPort();

            _currentSerialPort.BaudRate = GloableVirables.BuadRate;
            _currentSerialPort.PortName = GloableVirables.PortName;
            _currentSerialPort.DataBits = GloableVirables.dataBits;
            _currentSerialPort.Parity = GloableVirables.parity;
            _currentSerialPort.StopBits = GloableVirables.stopbits;
            _currentSerialPort.DataReceived += new SerialDataReceivedEventHandler(_currentSerialPort_DataReceived);
            _currentSerialPort.Open();
        }

        /// <summary>
        /// 打开串口前的参数验证
        /// </summary>
        /// <returns></returns>
        private bool OpenParamsCheck()
        {
            if (string.IsNullOrWhiteSpace(GloableVirables.PortName))
            {
                MessageBox.Show("请选择串口");
                return false;
            }
            if (GloableVirables.BuadRate == 0)
            {
                MessageBox.Show("请选择波特率");
                return false;
            }
            return true;
        }

        private string[] hexData = new string[14];
        private byte[] asciiData = new byte[128];
        private StringBuilder strBuild = new StringBuilder();

        private static object _lock = new object();

        //最新温湿度采集数据
        private string newestHaT = string.Empty;
        //最新光照采集数据
        private string newestLight = string.Empty;
        //最新光照设置返回数据
        private string newestLightSetting = string.Empty;

        private bool sendMessageLock = false;

        /// <summary>
        /// 串口接收事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _currentSerialPort_DataReceived(object _currentSerialPortObj, SerialDataReceivedEventArgs e)
        {
            lock(_lock)
            {
                try
                {
                    int btr = 0;
                    do
                    {
                        btr += _currentSerialPort.BytesToRead;
                        //读取接收的数据
                        byte[] received_buf = new byte[btr];
                        _currentSerialPort.Read(received_buf, 0, btr);
                        foreach (var item in received_buf)
                        {
                            strBuild.Append(item.ToString("X2"));
                        }
                        //ui控件实时响应
                        Application.DoEvents();
                    }
                    while (_currentSerialPort.BytesToRead > 0);

                    if (strBuild.Length >= 28)
                     {
                        if (!checkReturnData(strBuild, _currentSendCom)) {
                            sendMessageLock = false;
                        }
                    }
                    this.BeginInvoke(new EventHandler(delegate
                    {
                        if (!string.IsNullOrWhiteSpace(complatedReturnMsg))
                        {
                            ReceivingBox.Text += complatedReturnMsg + Environment.NewLine;
                            complatedReturnMsg = string.Empty;
                            label35.Text = newestHaT;
                            label41.Text = newestLight;
                            sendMessageLock = false;
                            strBuild.Clear();
                        }
                        rb += btr;
                        ReceiveBytes.Text = rb.ToString();
                    }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message?.ToString());
                }
            }          
        }

        private StringBuilder stringBu = new StringBuilder();


        //private void ConnectToServer()
        //{
        //    string _connectCom = connectCom + CRCHelper.ToModbusCRC16(connectCom, true);

        //    SendMessage(_connectCom);
        //}

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="msg"></param>
        private void SendMessage(string msg)
        {
            if (!sendMessageLock)
            {
                sendMessageLock = true;
                if (_currentSerialPort != null && _currentSerialPort.IsOpen)
                {
                    string msgtext = SendMsgBox.Text.Replace(" ", "");
                    if (string.IsNullOrWhiteSpace(msgtext))
                        msgtext = msg.Trim().Replace(" ","");

                    label45.Text = "已发送: " + msgtext;
                    int sLong = 0;

                    sLong = (msgtext.Length - msgtext.Length % 2) / 2;

                    byte[] hexBytes = new byte[sLong];
                    for (int i = 0; i < sLong; i++)
                    {
                        hexBytes[i] = Convert.ToByte(msgtext.Substring(i * 2, 2), 16);
                    }
                    _currentSerialPort.Write(hexBytes, 0, sLong);
                    sLong = _currentSerialPort.BytesToWrite;
                    this.BeginInvoke(new EventHandler(delegate
                    {
                        sb += sLong;
                        this.SendBytes.Text = sb.ToString();
                    }));
                }
            }
            
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            string sendMsg = SendMsgBox.Text;
            string result = CRCHelper.ToModbusCRC16(sendMsg, true);
            textBox1.Text = result;
            SendMsgBox.Text = SendMsgBox.Text + " " + result;

        }

        private static string humAndTemp = string.Empty;
        private static string humAndTempCrc = string.Empty;


        private static string light = string.Empty;
        private static string lightCrc = string.Empty;
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (checkBox3.Checked) {
                if (!string.IsNullOrWhiteSpace(humAndTemp)) {
                   
                    if (!string.IsNullOrWhiteSpace(humAndTempCrc)) {
                        _currentSendCom = DataTypeEnum.HandT;
                        SendMessage(humAndTemp + humAndTempCrc);
                    }                 
                }
            }
            if (checkBox4.Checked) {
                if (!string.IsNullOrWhiteSpace(light))
                {
                    if (!string.IsNullOrWhiteSpace(lightCrc))
                    {
                        _currentSendCom = DataTypeEnum.Light;
                        SendMessage(light + lightCrc);
                    }
                }
            }
        }

        private bool timerStatus = false;
        private void Button5_Click(object sender, EventArgs e)
        {
            if (IsAutoSend())
            {
                if (!timerStatus)
                {
                    timerStatus = true;
                    button5.Text = "点击关闭连续采集";
                    humAndTemp = string.IsNullOrWhiteSpace(textBox2.Text) ? "" : textBox2.Text;
                    if (!string.IsNullOrWhiteSpace(humAndTemp))
                    {
                        humAndTempCrc = CRCHelper.ToModbusCRC16(humAndTemp, true);
                    }

                    light = string.IsNullOrWhiteSpace(textBox3.Text) ? "" : textBox3.Text;
                    if (!string.IsNullOrWhiteSpace(light))
                    {
                        lightCrc = CRCHelper.ToModbusCRC16(light, true);
                    }
                    timer1.Enabled = true;
                    timer1.Start();
                }
                else
                {
                    button5.Text = "点击连续采集数据";
                    timer1.Stop();
                    timer1.Enabled = false;
                    timerStatus = false;
                }
            }
            else {
                string sendMsg = SendMsgBox.Text;
                if (!string.IsNullOrWhiteSpace(sendMsg))
                {
                    if (checkBox7.Checked)
                    {
                        if (sendMsg.Trim().Replace(" ", "").Length < 28) {
                            string crc16 = CRCHelper.ToModbusCRC16(sendMsg, true);
                            sendMsg = sendMsg + crc16;
                            SendMsgBox.Text = SendMsgBox.Text + crc16;
                        }
                    }
                    SendMessage(sendMsg);
                }
                else
                    MessageBox.Show("请输入发送的指令");
            }                 
        }


        private static string complatedReturnMsg = string.Empty;
        private static DataTypeEnum _currentSendCom = DataTypeEnum.Connection;

        private bool checkReturnData(StringBuilder stringBuilder,DataTypeEnum dataTypeEnum)
        {
            bool comComplateed = false;
            string failMsg = string.Empty;
            if (stringBuilder.Length > 14) {
                string msg = stringBuilder.ToString();
                if (dataTypeEnum == DataTypeEnum.HandT)
                {
                    //温湿度
                    if (msg.StartsWith("020301") || msg.IndexOf("020301") > 0)
                    {
                        //成功获取
                        msg = msg.Substring(msg.IndexOf("020301") + 6);
                        if (!string.IsNullOrWhiteSpace(msg) && msg.Length >= 22)
                        {
                            complatedReturnMsg = "020301" + msg.Substring(0, 22);
                            string checkData = complatedReturnMsg.Substring(complatedReturnMsg.Length - 4);
                            string subData1 = complatedReturnMsg.Substring(0, complatedReturnMsg.Length - 4);
                            if (checkData.Equals(CRCHelper.ToModbusCRC16(subData1, true)))
                            {
                                newestHaT = complatedReturnMsg;
                                comComplateed = true;
                            }
                            else
                                complatedReturnMsg = string.Empty;
                        }
                    }                   
                }
                else if (dataTypeEnum == DataTypeEnum.Light)
                {
                    //光照
                    if (msg.StartsWith("020302") || msg.IndexOf("020302") > 0)
                    {
                        //成功获取
                        msg = msg.Substring(msg.IndexOf("020302") + 6);
                        if (!string.IsNullOrWhiteSpace(msg) && msg.Length >= 22)
                        {
                            complatedReturnMsg = "020302" + msg.Substring(0, 22);

                            string checkData = complatedReturnMsg.Substring(complatedReturnMsg.Length - 4);
                            string subData1 = complatedReturnMsg.Substring(0, complatedReturnMsg.Length - 4);
                            if (checkData.Equals(CRCHelper.ToModbusCRC16(subData1, true)))
                            {
                                newestLight = complatedReturnMsg;
                                comComplateed = true;
                            }
                            else
                                complatedReturnMsg = string.Empty;
                        }
                    }
                }
                else if (dataTypeEnum == DataTypeEnum.Connection)
                {
                    //通信状态
                    if (msg.StartsWith("0206") || msg.IndexOf("0206") > 0)
                    {
                        //成功获取
                        msg = msg.Substring(msg.IndexOf("0206") + 4);
                        if (!string.IsNullOrWhiteSpace(msg) && msg.Length >= 24)
                        {
                            complatedReturnMsg = "0206" + msg.Substring(0, 24);
                            string checkData = complatedReturnMsg.Substring(complatedReturnMsg.Length - 4);
                            string subData1 = complatedReturnMsg.Substring(0, complatedReturnMsg.Length - 4);
                            if (checkData.Equals(CRCHelper.ToModbusCRC16(subData1, true)))
                                comComplateed = true;
                            else
                                complatedReturnMsg = string.Empty;

                        }
                    }
                }
                else if (dataTypeEnum == DataTypeEnum.SettingLight)
                {
                    //光照设置
                    if (msg.StartsWith("0205") || msg.IndexOf("0205") > 0)
                    {
                        //成功获取
                        msg = msg.Substring(msg.IndexOf("0205") + 4);
                        if (!string.IsNullOrWhiteSpace(msg) && msg.Length >= 24)
                        {
                            complatedReturnMsg = "0205" + msg.Substring(0, 24);
                            string checkData = complatedReturnMsg.Substring(complatedReturnMsg.Length - 4);
                            string subData1 = complatedReturnMsg.Substring(0, complatedReturnMsg.Length - 4);
                            if (checkData.Equals(CRCHelper.ToModbusCRC16(subData1, true)))
                            {
                                newestLightSetting = complatedReturnMsg;
                                comComplateed = true;
                            }
                            else
                                complatedReturnMsg = string.Empty;
                        }
                    }
                }
            }
            if (comComplateed)
            {
                //this.BeginInvoke(new EventHandler(delegate {
                //    this.ReceivingBox.Text += stringBuilder.ToString();
                //}));
            }
            return comComplateed;
        }

        #region checkBoxStatus
        private bool IsAutoSend()
        {
            if (checkBox3.Checked || checkBox4.Checked)
                return true;
            else 
                return false;
        }

        private void CheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked || checkBox4.Checked)
                AutoPostCheckSetFalse();
            else
                AutoPostCheckSetEnable();
            if (checkBox3.Checked)
                checkBox4.Checked = false;

        }

        private void AutoPostCheckSetEnable()
        {
            //checkBox1.Checked = true;
            checkBox1.Enabled = true;
            //checkBox2.Checked = true;
            checkBox2.Enabled = true;
            //checkBox6.Checked = true;
            checkBox6.Enabled = true;
            //checkBox5.Checked = true;
            checkBox5.Enabled = true;
        }
        private void AutoPostCheckSetFalse()
        {
            checkBox1.Checked = false;
            checkBox1.Enabled = false;
            checkBox2.Checked = false;
            checkBox2.Enabled = false;
            checkBox6.Checked = false;
            checkBox6.Enabled = false;
            checkBox5.Checked = false;
            checkBox5.Enabled = false;
            SendMsgBox.Text = string.Empty;
        }

        private void CheckBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked || checkBox4.Checked)
                AutoPostCheckSetFalse();
            else
                AutoPostCheckSetEnable();
            if (checkBox4.Checked)
                checkBox3.Checked = false;
        }

        private void CheckBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
            {
                SendMsgBox.Text = connectCom;
                button5.Text = "发送";
                checkBox1.Checked = false;
                checkBox2.Checked = false;
                checkBox6.Checked = false;
            }              
        }

        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                _currentSendCom = DataTypeEnum.Light;
                SendMsgBox.Text = lightCom;
                button5.Text = "发送";
                checkBox1.Checked = false;
                checkBox5.Checked = false;
                checkBox6.Checked = false;
            }
        }

        private void CheckBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
            {
                _currentSendCom = DataTypeEnum.SettingLight;
                SendMsgBox.Text = lightSetCom;
                button5.Text = "发送";
                checkBox1.Checked = false;
                checkBox2.Checked = false;
                checkBox5.Checked = false;
            }
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                _currentSendCom = DataTypeEnum.HandT;
                SendMsgBox.Text = humAndTempCom;
                button5.Text = "发送";
                checkBox5.Checked = false;
                checkBox2.Checked = false;
                checkBox6.Checked = false;
            }
        }

        private void TextBox12_TextChanged(object sender, EventArgs e)
        {
            label42.Text = textBox12.Text.Trim();
        }

        private void TextBox13_TextChanged(object sender, EventArgs e)
        {
            label43.Text = textBox13.Text.Trim();
        }

        private void TextBox16_TextChanged(object sender, EventArgs e)
        {
            label44.Text = textBox16.Text.Trim();
        }

        #endregion

        private string CalculateValue(string _expression)
        {
            var types = new TypeRegistry();
            types.RegisterDefaultTypes();

            var expression = new CompiledExpression(_expression) { TypeRegistry = types };
            var result = expression.Eval();
            return result.ToString() ;
        }


        private void Timer2_Tick(object sender, EventArgs e)
        {
            string temExpression = tb10;
            string humExpression = tb11;
            string lightExpression = tb17;

            if (!string.IsNullOrWhiteSpace(newestHaT)) {
                newestHaT = newestHaT.Trim().Replace(" ","") ;
                if (!string.IsNullOrWhiteSpace(temExpression)) {

                    int _xhIndex = 6;
                    int _xlIndex = 8;
                    if (!string.IsNullOrWhiteSpace(textBox4.Text)) {
                        if (int.TryParse(textBox4.Text,out _xhIndex)) {
                            _xhIndex = int.Parse(textBox4.Text);
                        }
                    }
                    //计算温度
                    string _xh = newestHaT.Substring(_xhIndex, 2);
                    int int_xh = Convert.ToInt32(_xh,16);
                    if (!string.IsNullOrWhiteSpace(textBox5.Text))
                    {
                        if (int.TryParse(textBox5.Text, out _xlIndex))
                        {
                            _xlIndex = int.Parse(textBox5.Text);
                        }
                    }

                    string _xl = newestHaT.Substring(_xlIndex, 2);
                    int int_xl = Convert.ToInt32(_xl, 16);
                    temExpression = temExpression.Replace("XH", int_xh.ToString());
                    temExpression = temExpression.Replace("XL", int_xl.ToString());

                    try
                    {
                        string _temValue = CalculateValue(temExpression);
                        label19.Text = _temValue;
                    }
                    catch (Exception)
                    {
                        label19.Text = "错误";
                    }
                   

                }

                if (!string.IsNullOrWhiteSpace(humExpression)) {
                    //计算湿度
                    int _yhIndex = 10;
                    int _ylIndex = 12;
                    if (!string.IsNullOrWhiteSpace(textBox6.Text))
                    {
                        if (int.TryParse(textBox6.Text, out _yhIndex))
                        {
                            _yhIndex = int.Parse(textBox6.Text);
                        }
                    }

                    string _yh = newestHaT.Substring(_yhIndex, 2);
                    int int_yh = Convert.ToInt32(_yh,16);
                    if (!string.IsNullOrWhiteSpace(textBox9.Text))
                    {
                        if (int.TryParse(textBox9.Text, out _ylIndex))
                        {
                            _ylIndex = int.Parse(textBox9.Text);
                        }
                    }
                    string _yl = newestHaT.Substring(_ylIndex, 2);
                    int int_yl = Convert.ToInt32(_yl, 16);
                    humExpression = humExpression.Replace("YH", int_yh.ToString());
                    humExpression = humExpression.Replace("YL", int_yl.ToString());

                    try
                    {
                        string _temValue = CalculateValue(humExpression);
                        label20.Text = _temValue;
                    }
                    catch (Exception ex)
                    {
                        label20.Text = "错误" +ex.Message;
                    }
                   
                }             
            }


            if (!string.IsNullOrWhiteSpace(newestLight)) {
                newestLight = newestLight.Trim().Replace(" ", "");
                if (!string.IsNullOrWhiteSpace(lightExpression))
                {
                    int _zhIndex = 6;
                    int _zlIndex = 8;

                    if (!string.IsNullOrWhiteSpace(textBox14.Text))
                    {
                        if (int.TryParse(textBox14.Text, out _zhIndex))
                        {
                            _zhIndex = int.Parse(textBox14.Text);
                        }
                    }
                    //计算光照
                    string _zh = newestLight.Substring(_zhIndex, 2);
                    int int_zh = Convert.ToInt32(_zh, 16);
                    if (!string.IsNullOrWhiteSpace(textBox15.Text))
                    {
                        if (int.TryParse(textBox15.Text, out _zlIndex))
                        {
                            _zlIndex = int.Parse(textBox15.Text);
                        }
                    }

                    string _zl = newestLight.Substring(_zlIndex, 2);
                    int int_zl = Convert.ToInt32(_zl, 16);

                    lightExpression = lightExpression.Replace("ZH", int_zh.ToString());
                    lightExpression = lightExpression.Replace("ZL", int_zl.ToString());

                    try
                    {
                        string _temValue = CalculateValue(lightExpression);
                        label21.Text = _temValue;
                    }
                    catch (Exception ex)
                    {
                        label21.Text = "错误" + ex.Message;
                    }                
                }
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            (new Auth()).ShowDialog();
            GetMt();
        }
    }
}
