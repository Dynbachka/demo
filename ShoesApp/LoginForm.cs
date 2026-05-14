using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace ShoesApp
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            this.BackColor = Color.White;
            this.Font = new Font("Times New Roman", 10);
            btnLogin.BackColor = ColorTranslator.FromHtml("#00FA9A");
            btnGuest.BackColor = ColorTranslator.FromHtml("#7FFF00");
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataTable result = DatabaseHelper.GetTable(
                "SELECT Id, LastName, FirstName, MiddleName, Role FROM [User] WHERE Login=@l AND Password=@p",
                new SqlParameter("@l", login),
                new SqlParameter("@p", password));

            if (result.Rows.Count == 0)
            {
                MessageBox.Show("Неверный логин или пароль.", "Ошибка входа",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataRow row = result.Rows[0];
            CurrentUser.Id = (int)row["Id"];
            CurrentUser.LastName = row["LastName"].ToString();
            CurrentUser.FirstName = row["FirstName"].ToString();
            CurrentUser.MiddleName = row["MiddleName"] == DBNull.Value ? "" : row["MiddleName"].ToString();
            CurrentUser.Role = row["Role"].ToString();

            new ProductListForm().Show();
            this.Hide();
        }

        private void btnGuest_Click(object sender, EventArgs e)
        {
            CurrentUser.Id = 0;
            CurrentUser.LastName = "Гость";
            CurrentUser.FirstName = "";
            CurrentUser.MiddleName = "";
            CurrentUser.Role = "Гость";

            new ProductListForm().Show();
            this.Hide();
        }
    }
}