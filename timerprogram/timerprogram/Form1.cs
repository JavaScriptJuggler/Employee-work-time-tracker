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
using System.Configuration;
namespace timerprogram
{
    public partial class Form1 : Form
    {
        public static string username;
        public static MySqlConnection con;
        public Form1()
        {
            InitializeComponent();
            string connction = ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString;
            con = new MySqlConnection(connction);
            con.Open();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MySqlCommand cmd = new MySqlCommand("select * from user where user='" + textBox2.Text + "' and pass='" + textBox1.Text + "'", con);
            MySqlDataReader read = cmd.ExecuteReader();
            if (read.Read())
            {
                read.Close();
                username = textBox2.Text;
                Form2 form = new Form2();
                form.Show();
                this.Hide();
            }
            else
            {
                read.Close();
                MessageBox.Show("invalid Password");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}
