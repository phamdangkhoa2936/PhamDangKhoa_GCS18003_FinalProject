using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.IO.Ports;
using System.Xml;

namespace Fingerprint_Attendance
{
    public partial class Check_Attendance : Form
    {
        int values = 0;
        int tongsosv = 0;
        string InputData = String.Empty; // Khai báo string buff dùng cho hiển thị dữ liệu sau này.
        delegate void SetTextCallback(string text); // Khai bao delegate SetTextCallBack voi tham so string
        SqlConnection con;

        public Check_Attendance()
        {
            InitializeComponent();
            // Khai báo hàm delegate bằng phương thức DataReceived của Object SerialPort;
            // Cái này khi có sự kiện nhận dữ liệu sẽ nhảy đến phương thức DataReceive
            // Nếu ko hiểu đoạn này bạn có thể tìm hiểu về Delegate
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(DataReceive);
            string[] BaudRate = { "1200", "2400", "4800", "9600", "19200", "38400", "57600", "115200" };
            comboBox2.Items.AddRange(BaudRate);
        }

        private void Check_Attendance_Load(object sender, EventArgs e)
        {
            timer1.Start();
            comboBox1.DataSource = SerialPort.GetPortNames();
            comboBox2.SelectedIndex = 3;
            string conString = ConfigurationManager.ConnectionStrings["QLSV"].ConnectionString.ToString();
            con = new SqlConnection(conString);
            con.Open();
            HienThi();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = DateTime.Now.ToLongTimeString();
            if (!serialPort1.IsOpen)
            {
                label5.Text = ("Disconnect");
                label5.ForeColor = Color.Red;
            }
            else if (serialPort1.IsOpen)
            {
                label5.Text = ("Connect");
                label5.ForeColor = Color.Green;
                //String dataFromAduino = serialPort1.ReadExisting().ToString();
                //textBox1.Text = dataFromAduino;
            }
        }

        private void DataReceive(object obj, SerialDataReceivedEventArgs e)
        {
            InputData = serialPort1.ReadExisting();
            if (InputData != String.Empty)
            {
                //textBox1.Text = InputData; // Ko dùng đc như thế này vì khác threads .
                SetText(InputData); // Chính vì vậy phải sử dụng ủy quyền tại đây. Gọi delegate đã khai báo trước đó.
            }
        }

        private void SetText(string text)
        {
            if (this.textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText); // khởi tạo 1 delegate mới gọi đến SetText
                this.Invoke(d, new object[] { text });
            }
            else
            {
                if (int.TryParse(text, out values))
                {
                    capnhat(values, label2.Text);
                    timkiem(values);
                    DialogResult result = MessageBox.Show("Hello , please continue ", "FINGERPRINT ATTENDANCE", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result == DialogResult.Yes)
                    {
                        serialPort1.Write("P");
                    }
                }
                this.textBox1.Text += text;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                serialPort1.PortName = comboBox1.Text;
                serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
                serialPort1.Open();
            }
            if (tongsosv == 0)
            {

                DialogResult result = MessageBox.Show("Database is empty !! You must log out and sign up for a student", "FINGERPRINT ATTENDANCE", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (result == DialogResult.Yes)
                {
                    Registration tform2 = new Registration();
                    tform2.Show();
                    this.Close();
                }
            }
            else
            {
                DialogResult result = MessageBox.Show("Successfully connected, you are ready ", "FINGERPRINT ATTENDANCE ", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (result == DialogResult.Yes)
                {
                    serialPort1.Write("P");

                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret();
            textBox1.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Home tform1 = new Home();
            tform1.Show();
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (tongsosv == 0)
            {
                DialogResult result = MessageBox.Show("Database is empty !! You must log out and sign up for a student", "FINGERPRINT ATTENDANCE", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (result == DialogResult.Yes)
                {
                    Registration tform2 = new Registration();
                    tform2.Show();
                    this.Close();
                }
            }
            else
            {
                serialPort1.Write("P");
            }
        }

        private void Check_Attendance_FormClosing(object sender, FormClosingEventArgs e)
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
            dvg3.DataSource = dt;
            tongsosv = dt.Rows.Count;
        }

        public void capnhat(int thutu, string thoigian)
        {
            string sqlUpdate = "UPDATE dssinhvien SET Attended = @B_attended WHERE STT = @B_stt";
            SqlCommand cmd = new SqlCommand(sqlUpdate, con);

            cmd.Parameters.AddWithValue("B_stt", thutu.ToString());
            cmd.Parameters.AddWithValue("B_attended", thoigian);
            cmd.ExecuteNonQuery();
            HienThi();
        }

        public void timkiem(int thutu)
        {
            string sqlTimKiem = "SELECT * FROM dssinhvien WHERE STT = @Bstt";
            SqlCommand cmd = new SqlCommand(sqlTimKiem, con);
            cmd.Parameters.AddWithValue("Bstt", thutu.ToString());
            cmd.ExecuteNonQuery();
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dvg4.DataSource = dt;

        }

        private void button5_Click(object sender, EventArgs e)
        {
            xuatfilexcel();
        }

        public void xuatfilexcel()
        {
            if (dvg3.RowCount > 0)
            {
                Microsoft.Office.Interop.Excel.Application xcelApp = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel._Workbook workbook = xcelApp.Workbooks.Add(Type.Missing);
                Microsoft.Office.Interop.Excel._Worksheet worksheet = null;
                worksheet = workbook.Sheets["Sheet1"];
                worksheet = workbook.ActiveSheet;
                worksheet.Name = "CustomerDetails";
                worksheet.Cells[1, 1] = "Attendance Report";

                //xcelApp.Application.Workbooks.Add(Type.Missing);

                for (int i = 1; i < dvg3.ColumnCount + 1; i++)
                {
                    worksheet.Cells[2, i] = dvg3.Columns[i - 1].HeaderText;

                }
                for (int i = 0; i < dvg3.RowCount; i++)
                {
                    for (int j = 0; j < dvg3.ColumnCount; j++)
                    {
                        worksheet.Cells[i + 3, j + 1] = dvg3.Rows[i].Cells[j].Value.ToString();

                    }
                }
                worksheet.Cells[dvg3.RowCount + 4, dvg3.ColumnCount - 3] = "TP Ho Chi Minh , day  " + dateTimePicker1.Value.ToShortDateString() + " -at:  " + label2.Text;
                var savefileDialog = new SaveFileDialog();
                savefileDialog.FileName = "output";
                savefileDialog.DefaultExt = ".xlsx";

                if (savefileDialog.ShowDialog() == DialogResult.OK)
                {
                    workbook.SaveAs(savefileDialog.FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                }
                xcelApp.Quit();
                //xcelApp.Columns.AutoFit();
                //xcelApp.Visible = true;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
