using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Net;
using System.Runtime.InteropServices;
using System.Diagnostics;
namespace timerprogram
{
    public partial class Form2 : Form
    {
        MySqlConnection con;
        Stopwatch watch = new Stopwatch();
        int fetchid;
        string myIP;
        public Form2()
        {
            InitializeComponent();
            con = Form1.con;
        }
        const int SC_ClOSE = 0xF060;
        const int MF_GRAYED = 0x1;
        const int MF_ENABLED = 0x00000000;
        const int MF_DISABLED = 0x00000002;

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr HWNDValue, bool Revert);
        [DllImport("user32.dll")]
        private static extern int EnableMenuItem(IntPtr tmenu,int target,int targetStatus);


        private void Form2_Load(object sender, EventArgs e)
        {
            string hostName = Dns.GetHostName();
            myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            label2.Text = "Login as : " + Form1.username;
            EnableMenuItem(GetSystemMenu(this.Handle, false), SC_ClOSE, MF_GRAYED);
            MySqlDataAdapter adpt = new MySqlDataAdapter("select * from projects_list", con);
            DataSet ds = new DataSet();
            adpt.Fill(ds);
            for(int i=0;i<ds.Tables[0].Rows.Count;i++)
            {
                comboBox1.Items.Add(Convert.ToString(ds.Tables[0].Rows[i][1]));
            }

            MySqlCommand cmd = new MySqlCommand("insert into log values('" + Form1.username + "','" + DateTime.Now.ToString("MM/dd/yyyy") + "','" + DateTime.Now.ToString("hh:mm") + "','" + myIP + "')", con);
            cmd.ExecuteNonQuery();
            comboBox1.SelectedIndex = 0;
            timer1.Enabled = true;
            timer1.Interval = 1000;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            button1.Enabled = false;
            watch.Start();
            string proj = comboBox1.SelectedItem.ToString();
            string user = label2.Text;
            string date = DateTime.Now.ToString("MM/dd/yyyy");
            string start = DateTime.Now.ToString("hh:mm");
            MySqlCommand cmd = new MySqlCommand("insert into data(proj,user,ip,date,start) values('" + proj + "','" + Form1.username + "','" + myIP + "','" + date + "','" + start + "')", con);
            cmd.ExecuteNonQuery();
            MySqlDataAdapter adpt = new MySqlDataAdapter("select * from data", con);
            DataSet ds = new DataSet();
            adpt.Fill(ds);
            int rownum = ds.Tables[0].Rows.Count;
            fetchid = Convert.ToInt32(ds.Tables[0].Rows[rownum - 1][0]);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            string end = DateTime.Now.ToString("hh:mm");
            MySqlCommand cmd = new MySqlCommand("update data set end='"+end+"' where id='"+fetchid+"'", con);
            cmd.ExecuteNonQuery();
            watch.Stop();
            TimeSpan ts = watch.Elapsed;
            string elapsedTime = String.Format("{0:00}H {1:00}Min",ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            string elapsedtime1= String.Format("{0:00} : {1:00} : {2:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            MySqlCommand cmd1 = new MySqlCommand("update data set totaltime='" + elapsedTime + "' where id='" + fetchid + "'", con);
            cmd1.ExecuteNonQuery();
            MySqlCommand cmd2 = new MySqlCommand("insert into total_time_elapsed_on_project values('" + comboBox1.SelectedItem.ToString() + "','" + Form1.username + "','" + elapsedtime1 + "')", con);
            cmd2.ExecuteNonQuery();
            button3.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(watch.IsRunning)
            {
                EnableMenuItem(GetSystemMenu(this.Handle, false), SC_ClOSE, MF_GRAYED);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
