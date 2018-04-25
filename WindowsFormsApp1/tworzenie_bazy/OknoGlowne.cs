using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Threading;

namespace tworzenie_bazy
{
    public partial class OknoGlowne : Form
    {
        public OknoGlowne()
        {
            InitializeComponent();
            this.Text = "Zakładanie bazy i użytkownika";
            this.ShowIcon = false;
        }

        private void pobieranie_instancji()
        {
            comboBox1.Items.Clear();
            SqlDataSourceEnumerator instance = SqlDataSourceEnumerator.Instance;
            System.Data.DataTable table = instance.GetDataSources();
            foreach (System.Data.DataRow row in table.Rows)
            {
                if (row["ServerName"] != DBNull.Value && Environment.MachineName.Equals(row["ServerName"].ToString()))
                {
                    string item = string.Empty;
                    item = row["ServerName"].ToString();
                    if (row["InstanceName"] != DBNull.Value || !string.IsNullOrEmpty(Convert.ToString(row["InstanceName"]).Trim()))
                    {
                        item += @"\" + Convert.ToString(row["InstanceName"]).Trim();
                    }
                    comboBox1.Items.Add(item);
                }
            }
            comboBox1.Focus();
            textBox8.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        void wybierz()
        {
            comboBox1.Text = "- - wybierz - -";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (tabPage1 == tabControl1.SelectedTab)
            {
                #region Pierwszy TAB, utworzenie bazy i użytkownika
                if (comboBox1.Text == "- - wybierz - -")
                {
                    MessageBox.Show("Adres serwera wydaje się być niepoprawny!");
                }
                else
                {
                    if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox5.Text == "" || textBox7.Text == "" || textBox8.Text == "" || comboBox1.Text == "")
                    {
                        MessageBox.Show("Wypełnij wszystkie pola!");
                    }
                    else
                    {
                        if (textBox7.Text == textBox8.Text)
                        {
                            wykonaj_1(comboBox1.Text, textBox2.Text, textBox3.Text, textBox1.Text, textBox5.Text, textBox7.Text);
                            czyszczenie();
                        }
                        else
                        {
                            MessageBox.Show("Hasła nie są identyczne", "Informacja");
                        }
                    }
                }
                #endregion
            }
            else
            {
                #region Drugi TAB, wykonanie instrukcji z kodu
                wykonaj_2(comboBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text);
                czyszczenie();
                #endregion
            }
        }

        private void textBox2_Validating(object sender, CancelEventArgs e)
        {
            string s = textBox2.Text;
            if (s.Length < 2 || s.Length > 14)
            {
                MessageBox.Show("Niepoprawna wartość pola, hasło powinno mieć od 2 do 14 znaków.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                textBox5.Enabled = false;
                textBox5.Text = textBox1.Text;
            }
            else
            {
                textBox5.Enabled = true;
            }
        }

        private void zakoncz()
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            zakoncz();
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            if (textBox7.Text.Length > 0)
            {
                textBox8.Enabled = true;
            }
            else
            {
                textBox8.Enabled = false;
                textBox8.Clear();
            }
        }

        private void czyszczenie()
        {
            textBox1.Clear();
            //textBox2.Clear();
            //textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox7.Clear();
            textBox8.Clear();
            //comboBox1.Items.Clear();
            comboBox1.Focus();
            //comboBox1.Text = "";
            checkBox1.Checked = false;
        }
        
        // wykonianie instrukcji tworzenia bazy i uzytkownika
        private void wykonaj_1(string instancja, string db_login, string db_haslo, string n_baza, string n_login, string n_haslo)
        {
            string connetionString = null;
            SqlConnection cnn;
            SqlCommand cmd;
            string sql1 = null;
            string sql2 = null;
            connetionString = "Data Source=" + instancja.ToString() + ";User ID=" + db_login.ToString() + ";Password=" + db_haslo.ToString();
            sql1 =
                "CREATE DATABASE " + n_baza.ToString() + ";";
            sql2 =
                "CREATE LOGIN " + n_login.ToString() + " WITH PASSWORD = '" + n_haslo.ToString() + "';" +
                "USE " + n_baza.ToString() + ";" +
                "CREATE USER " + n_login.ToString() + " FOR LOGIN " + n_login.ToString() + ";" +
                "EXEC sp_addrolemember 'db_owner', " + n_login.ToString() + ";";
            cnn = new SqlConnection(connetionString);
            try
            {
                cnn.Open();
                cmd = new SqlCommand(sql1, cnn);
                cmd.ExecuteNonQuery();
                cmd = new SqlCommand(sql2, cnn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cnn.Close();
                MessageBox.Show("Wykonano zadanie, baza i użytkownik utworzeni na serwerze bazdy danych!", "Informacja");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Message);
            }
        }

        private void wykonaj_2(string instancja, string db_login, string db_haslo, string kod)
        {
            string connetionString = null;
            SqlConnection cnn;
            SqlCommand cmd;
            string sql1 = null;
            connetionString = "Data Source=" + instancja.ToString() + ";User ID=" + db_login.ToString() + ";Password=" + db_haslo.ToString();
            sql1 = kod.ToString();
            cnn = new SqlConnection(connetionString);
            try
            {
                cnn.Open();
                cmd = new SqlCommand(sql1, cnn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cnn.Close();
                MessageBox.Show("Wykonano zadanie, baza i użytkownik utworzeni na serwerze bazdy danych!", "Informacja");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Message);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "mailto:william@o2.pl";
            proc.Start();
        }

        //private void tabControl1_Selected(object sender, TabControlEventArgs e)
        //{
        //    if (tabPage3 == tabControl1.SelectedTab)
        //    {
        //        button3.Enabled = false;
        //    }
        //    else
        //    {
        //        button3.Enabled = true;
        //    }
        //}

        private void button4_Click(object sender, EventArgs e)
        {
            string connetionString = null;
            connetionString = "Data Source=" + comboBox1.Text + ";User ID=" + textBox2.Text + ";Password=" + textBox3.Text;
            SqlConnection con = new SqlConnection(connetionString);
            SqlDataAdapter ada = new SqlDataAdapter("select name from sys.databases where database_id > 6", con);
            DataTable dt = new DataTable();
            ada.Fill(dt);
            //comboBox2.DataSource = dt;
            //comboBox2.ValueMember = "name"; 
        }

        private void Przycisk_Znajdz_Serwery_Click(object sender, EventArgs e)
        {
            pobieranie_instancji();
            wybierz();
        }
    }
}
