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
using System.IO.Ports;
using System.Xml;
using System.Data.SqlClient;
using System.Configuration;

namespace Fingerprint_Attendance
{
    public partial class Registration : Form
    {
        int tongsosv;
        string Whiendien = "";
        string InputData = String.Empty; // Khai báo string buff dùng cho hiển thị dữ liệu sau này.
        delegate void SetTextCallback(string text); // Khai bao delegate SetTextCallBack voi tham so string
        SqlConnection con;

        public Registration()
        {
            InitializeComponent();
            // Khai báo hàm delegate bằng phương thức DataReceived của Object SerialPort;
            // Cái này khi có sự kiện nhận dữ liệu sẽ nhảy đến phương thức DataReceive
            // Nếu ko hiểu đoạn này bạn có thể tìm hiểu về Delegate, còn ko cứ COPY . Ko cần quan tâm
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(DataReceive);
            string[] BaudRate = { "1200", "2400", "4800", "9600", "19200", "38400", "57600", "115200" };
            comboBox2.Items.AddRange(BaudRate);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            textBox4.SelectionStart = textBox4.Text.Length;
            textBox4.ScrollToCaret();
            textBox4.Refresh();
        }

        private void Registration_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = SerialPort.GetPortNames();
            comboBox2.SelectedIndex = 3;

            string conString = ConfigurationManager.ConnectionStrings["QLSV"].ConnectionString.ToString();
            con = new SqlConnection(conString);
            con.Open();
            HienThi();
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {

                label8.Text = ("Disconnected");
                label8.ForeColor = Color.Red;
            }
            else if (serialPort1.IsOpen)
            {

                label8.Text = ("Connected");
                label8.ForeColor = Color.Green;
                //String dataFromAduino = serialPort1.ReadExisting().ToString();
                //textBox4.Text = dataFromAduino;

            }
        }

        private void DataReceive(object obj, SerialDataReceivedEventArgs e)
        {
            
        }

        private void SetText(string text)
        {
            if (this.textBox4.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText); // khởi tạo 1 delegate mới gọi đến SetText
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox4.Text += text;
                if ("Stored! ".Contains(text)) //(text.Contains("Stored!"))
                {

                    themmautin(textBox1.Text, textBox2.Text, textBox3.Text, "", "");
                    textBox1.Text = "";
                    textBox2.Text = "";
                    textBox3.Text = "";
                    DialogResult result = MessageBox.Show("Fingerprint saved successfully , do you want to continue  ? ", "FINGERPRINT ATTENDANCE", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    if (result == DialogResult.Yes)
                    {

                        serialPort1.Write("E");
                        textBox1.Text = (tongsosv + 1).ToString();
                    }

                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                serialPort1.PortName = comboBox1.Text;
                serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
                serialPort1.Open();
            }
            DialogResult result = MessageBox.Show("Successful connection, you are ready ", "FINGERPRINT ATTENDANCE ", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (result == DialogResult.Yes)
            {
                serialPort1.Write("E");
                textBox1.Text = (tongsosv + 1).ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            serialPort1.Write(textBox1.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Home tform1 = new Home();
            tform1.Show();
            this.Close();
        }

        private void Registration_FormClosing(object sender, FormClosingEventArgs e)
        {
            con.Close();
        }

        public void HienThi()
        {
            string sqlSELECT = "SELECT * FROM dssinhvien";
            SqlCommand cmd = new SqlCommand(sqlSELECT, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            tongsosv = dt.Rows.Count;
            textBox1.Text = (tongsosv + 1).ToString();
            dvg2.DataSource = dt;
        }

        public void themmautin(string Msothutu, string Mmasv, string Mhoten, string Mhiendien, string Mvangmat)
        {
            string sqlInser = "insert into dssinhvien values (@B_stt ,@B_mssv,@B_name,@B_attended,@B_absent)";
            SqlCommand cmd = new SqlCommand(sqlInser, con);
            cmd.Parameters.AddWithValue("B_stt", Int32.Parse(Msothutu));
            cmd.Parameters.AddWithValue("B_mssv", Mmasv);
            cmd.Parameters.AddWithValue("B_name", Mhoten);
            cmd.Parameters.AddWithValue("B_attended", Mhiendien);
            cmd.Parameters.AddWithValue("B_absent", Mvangmat);
            cmd.ExecuteNonQuery();
            HienThi();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            serialPort1.Write("E");
            textBox1.Text = (tongsosv + 1).ToString();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Updatemautin(textBox7.Text, textBox5.Text, textBox6.Text,Whiendien, textBox8.Text);
            textBox5.Text = "";
            textBox6.Text = "";
            textBox8.Text = "";
        }

        public void Updatemautin(string Msothutu, string Mmasv, string Mhoten, string Mhiendien, string Mvangmat)
        {
            string sqlInser = "update dssinhvien set MSSV = @B_mssv, Name = @B_name, Attended = @B_attended, Absent = @B_absent where STT = @B_stt";
            SqlCommand cmd = new SqlCommand(sqlInser, con);
            cmd.Parameters.AddWithValue("B_stt", Msothutu);
            cmd.Parameters.AddWithValue("B_mssv", Mmasv);
            cmd.Parameters.AddWithValue("B_name", Mhoten);
            cmd.Parameters.AddWithValue("B_attended", Mhiendien);
            cmd.Parameters.AddWithValue("B_absent", Mvangmat);
            cmd.ExecuteNonQuery();
            HienThi();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to delete the student list ? All information will be deleted !!! ", "FINGERPRINT ATTENDANCE ", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (result == DialogResult.Yes)
            {
                xoadatabase();
                serialPort1.Write("Y");
            }
        }

        public void xoadatabase()
        {
            string sqlInser = "delete from dssinhvien";
            SqlCommand cmd = new SqlCommand(sqlInser, con);
            cmd.ExecuteNonQuery();
            HienThi();
        }

        private void dvg2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //Lưu lại dòng dữ liệu vừa kích chọn
                DataGridViewRow row = this.dvg2.Rows[e.RowIndex];
                //Đưa dữ liệu vào textbox
                textBox7.Text = row.Cells[0].Value.ToString();
                textBox5.Text = row.Cells[1].Value.ToString();
                textBox6.Text = row.Cells[2].Value.ToString();
                Whiendien = row.Cells[3].Value.ToString();
                textBox8.Text = row.Cells[4].Value.ToString();

                //Không cho phép sửa trường STT
                textBox7.Enabled = false;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure to delete this 1 student? ", 
                "FINGERPRINT REGISTRATION ", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (result == DialogResult.Yes)
            {
                Updatemautin(textBox7.Text, "", "", "", "");
            }


            textBox5.Text = "";
            textBox6.Text = "";
            textBox8.Text = "";
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            InputData = serialPort1.ReadExisting();
            if (InputData != String.Empty)
            {
                //textBox1.Text = InputData; // Ko dùng đc như thế này vì khác threads .
                SetText(InputData); // Chính vì vậy phải sử dụng ủy quyền tại đây. Gọi delegate đã khai báo trước đó.
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
