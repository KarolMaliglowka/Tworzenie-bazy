using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;

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
            AdresSerwera.Items.Clear();
            AdresSerwera.Text = "wyszukuję";
            var instance = SqlDataSourceEnumerator.Instance;
            var table = instance.GetDataSources();
            foreach (DataRow row in table.Rows)
            {
                if (row["ServerName"] == DBNull.Value ||
                    !Environment.MachineName.Equals(row["ServerName"].ToString())) continue;
                var item = row["ServerName"].ToString();
                if (row["InstanceName"] != DBNull.Value || !string.IsNullOrEmpty(Convert.ToString(row["InstanceName"]).Trim()))
                {
                    item += @"\" + Convert.ToString(row["InstanceName"]).Trim();
                }
                AdresSerwera.Items.Add(item);
            }
            AdresSerwera.Focus();
            textBox8.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text = "Tworzenie bazy";
        }

        void wybierz()
        {
            AdresSerwera.Text = "- - wybierz - -";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (tabPage1 == tabControl1.SelectedTab)
            {
                #region Pierwszy TAB, utworzenie bazy i użytkownika
                if (AdresSerwera.Text == "- - wybierz - -")
                {
                    MessageBox.Show("Adres serwera wydaje się być niepoprawny!");
                }
                else
                {
                    if (textBox1.Text == "" ||
                        textBox2.Text == "" ||
                        textBox3.Text == "" ||
                        textBox5.Text == "" ||
                        textBox7.Text == "" ||
                        textBox8.Text == "" ||
                        AdresSerwera.Text == "")
                    {
                        MessageBox.Show("Wypełnij wszystkie pola!");
                    }
                    else
                    {
                        if (textBox7.Text == textBox8.Text)
                        {
                            wykonaj_1(AdresSerwera.Text,
                                textBox2.Text,
                                textBox3.Text,
                                textBox1.Text,
                                textBox5.Text,
                                textBox7.Text);
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
                wykonaj_2(AdresSerwera.Text,
                    textBox2.Text,
                    textBox3.Text,
                    textBox4.Text);
                czyszczenie();
                #endregion
            }
        }

        private void textBox2_Validating(object sender, CancelEventArgs e)
        {
            var s = textBox2.Text;
            if (s.Length < 2 || s.Length > 14)
            {
                MessageBox.Show("Niepoprawna wartość pola, hasło powinno mieć od 2 do 14 znaków.", "Informacja",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
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
            textBox4.Clear();
            textBox5.Clear();
            textBox7.Clear();
            textBox8.Clear();
            AdresSerwera.Focus();
            checkBox1.Checked = false;
        }

        // wykonianie instrukcji tworzenia bazy i uzytkownika
        private void wykonaj_1(string server, string login, string password, string dataBase, string dataBaseLogin, string dataBasePassword)
        {
            var connectionString = $"Data Source={server};User ID={login};Password={password}";
            var sql1 =
                $"CREATE DATABASE {dataBase};";
            var sql2 =
                $"CREATE LOGIN {dataBaseLogin} WITH PASSWORD = '{dataBasePassword}';" +
                $"USE {dataBase};" +
                $"CREATE USER {dataBaseLogin} FOR LOGIN {dataBaseLogin};" +
                $"EXEC sp_addrolemember 'db_owner', {dataBaseLogin};";
            var cnn = new SqlConnection(connectionString);
            try
            {
                cnn.Open();
                var cmd = new SqlCommand(sql1, cnn);
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

        private void wykonaj_2(string server, string login, string password, string code)
        {
            var connectionString = $"Data Source={server};User ID={login};Password={password}";
            var cnn = new SqlConnection(connectionString);
            try
            {
                cnn.Open();
                var cmd = new SqlCommand(code, cnn);
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
            var proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "mailto:william@o2.pl";
            proc.Start();
        }

        private void Przycisk_Znajdz_Serwery_Click(object sender, EventArgs e)
        {
            pobieranie_instancji();
            wybierz();
        }
    }
}
