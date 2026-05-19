using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShoesApp
{
    public partial class ProductEditForm : Form
    {
        private string articul;          
        private string currentPhotoPath;
        private string newPhotoPath;     

        public ProductEditForm(string articul)
        {
            InitializeComponent();
            this.articul = articul;
            this.BackColor = Color.White;
            this.Font = new Font("Times New Roman", 10);
        }

        private void ProductEditForm_Load(object sender, EventArgs e)
        {
            cboCategory.Items.AddRange(new string[]
            {
                "Женская обувь", "Мужская обувь", "Детская обувь"
            });

            DataTable authors = DatabaseHelper.GetTable(
                "SELECT DISTINCT author FROM items ORDER BY author");
            foreach (DataRow row in authors.Rows)
                cboAuthor.Items.Add(row["author"].ToString());

            if (string.IsNullOrEmpty(articul))
            {
                this.Text = "Добавление товара — ООО Обувь";
                lblArticul.Visible = false;
                txtArticul.Visible = false;
                btnDelete.Visible = false;
                LoadStubPhoto();
            }
            else
            {
                this.Text = "Редактирование товара — ООО Обувь";
                btnDelete.Visible = true;
                LoadProduct();
            }
        }

        private void LoadProduct()
        {
            DataTable dt = DatabaseHelper.GetTable(
                "SELECT * FROM items WHERE articul = @a",
                new SqlParameter("@a", articul));

            if (dt.Rows.Count == 0) return;
            DataRow row = dt.Rows[0];

            txtArticul.Text = row["articul"].ToString();
            txtName.Text = row["name_item"].ToString();
            cboCategory.Text = row["category"].ToString();
            txtAbout.Text = row["about"] == DBNull.Value ? "" : row["about"].ToString();
            cboAuthor.Text = row["author"].ToString();
            txtCourier.Text = row["courier"] == DBNull.Value ? "" : row["courier"].ToString();
            txtPrice.Text = row["price"].ToString();
            txtUnit.Text = row["unit"].ToString();
            txtCount.Text = row["count"].ToString();
            txtDiscount.Text = row["discount"] == DBNull.Value ? "0" : row["discount"].ToString();

            currentPhotoPath = row["photo"] == DBNull.Value ? "" : row["photo"].ToString();
            LoadPhoto(currentPhotoPath);
        }

        private void LoadPhoto(string path)
        {
            string fullPath = Path.Combine(Application.StartupPath, path);
            string stub = Path.Combine(Application.StartupPath, "picture.png");

            if (!string.IsNullOrEmpty(path) && File.Exists(fullPath))
                pbPhoto.Image = Image.FromFile(fullPath);
            else if (File.Exists(stub))
                pbPhoto.Image = Image.FromFile(stub);
            else
                pbPhoto.Image = null;
        }

        private void LoadStubPhoto()
        {
            string stub = Path.Combine(Application.StartupPath, "picture.png");
            if (File.Exists(stub))
                pbPhoto.Image = Image.FromFile(stub);
        }

        private void btnPhoto_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp";
                dlg.Title = "Выберите фото товара";

                if (dlg.ShowDialog() != DialogResult.OK) return;

                newPhotoPath = dlg.FileName;

                pbPhoto.Image = Image.FromFile(newPhotoPath);
                pbPhoto.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show(
                    "Заполните поле «Наименование товара».",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(cboCategory.Text))
            {
                MessageBox.Show(
                    "Выберите категорию товара из списка.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cboCategory.Focus();
                return;
            }

            decimal price;
            if (!decimal.TryParse(txtPrice.Text.Replace(',', '.'),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out price) || price < 0)
            {
                MessageBox.Show(
                    "Цена должна быть числом больше или равным 0.\nПример: 1990,50",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPrice.Focus();
                return;
            }

            int count;
            if (!int.TryParse(txtCount.Text, out count) || count < 0)
            {
                MessageBox.Show(
                    "Количество должно быть целым числом не менее 0.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCount.Focus();
                return;
            }

            int discount = 0;
            if (!string.IsNullOrWhiteSpace(txtDiscount.Text))
            {
                if (!int.TryParse(txtDiscount.Text, out discount) || discount < 0 || discount > 100)
                {
                    MessageBox.Show(
                        "Скидка должна быть целым числом от 0 до 100.",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtDiscount.Focus();
                    return;
                }
            }

            string finalPhotoPath = currentPhotoPath;
            if (!string.IsNullOrEmpty(newPhotoPath))
            {
                try
                {
                    string ext = Path.GetExtension(newPhotoPath);
                    string fileName = "photo_" + DateTime.Now.Ticks + ext;
                    string destPath = Path.Combine(Application.StartupPath, fileName);

                    using (Image original = Image.FromFile(newPhotoPath))
                    using (Bitmap resized = new Bitmap(original, new Size(300, 200)))
                        resized.Save(destPath);

                    if (!string.IsNullOrEmpty(currentPhotoPath))
                    {
                        string oldFull = Path.Combine(Application.StartupPath, currentPhotoPath);
                        if (File.Exists(oldFull))
                            File.Delete(oldFull);
                    }

                    finalPhotoPath = fileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Не удалось сохранить изображение:\n" + ex.Message,
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            try
            {
                if (string.IsNullOrEmpty(articul))
                {
                    string newArticul = txtArticul.Visible ? txtArticul.Text.Trim() : GenerateArticul();

                    DatabaseHelper.ExecuteNonQuery(@"
                        INSERT INTO items
                            (articul, name_item, category, about, author, courier,
                             price, unit, count, discount, photo)
                        VALUES
                            (@art, @name, @cat, @about, @author, @courier,
                             @price, @unit, @count, @disc, @photo)",
                        new SqlParameter("@art", newArticul),
                        new SqlParameter("@name", txtName.Text.Trim()),
                        new SqlParameter("@cat", cboCategory.Text.Trim()),
                        new SqlParameter("@about", txtAbout.Text.Trim()),
                        new SqlParameter("@author", cboAuthor.Text.Trim()),
                        new SqlParameter("@courier", txtCourier.Text.Trim()),
                        new SqlParameter("@price", price),
                        new SqlParameter("@unit", txtUnit.Text.Trim()),
                        new SqlParameter("@count", count),
                        new SqlParameter("@disc", discount),
                        new SqlParameter("@photo", string.IsNullOrEmpty(finalPhotoPath)
                                                     ? (object)DBNull.Value : finalPhotoPath));

                    MessageBox.Show("Товар успешно добавлен!", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    DatabaseHelper.ExecuteNonQuery(@"
                        UPDATE items SET
                            name_item = @name,
                            category  = @cat,
                            about     = @about,
                            author    = @author,
                            courier   = @courier,
                            price     = @price,
                            unit      = @unit,
                            count     = @count,
                            discount  = @disc,
                            photo     = @photo
                        WHERE articul = @art",
                        new SqlParameter("@name", txtName.Text.Trim()),
                        new SqlParameter("@cat", cboCategory.Text.Trim()),
                        new SqlParameter("@about", txtAbout.Text.Trim()),
                        new SqlParameter("@author", cboAuthor.Text.Trim()),
                        new SqlParameter("@courier", txtCourier.Text.Trim()),
                        new SqlParameter("@price", price),
                        new SqlParameter("@unit", txtUnit.Text.Trim()),
                        new SqlParameter("@count", count),
                        new SqlParameter("@disc", discount),
                        new SqlParameter("@photo", string.IsNullOrEmpty(finalPhotoPath)
                                                     ? (object)DBNull.Value : finalPhotoPath),
                        new SqlParameter("@art", articul));

                    MessageBox.Show("Товар успешно обновлён!", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка при сохранении:\n" + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show(
                "Вы уверены, что хотите удалить товар «" + txtName.Text + "»?\n" +
                "Это действие нельзя отменить.",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            DataTable check = DatabaseHelper.GetTable(
                "SELECT COUNT(*) AS cnt FROM orderInfo WHERE articul = @a",
                new SqlParameter("@a", articul));

            int cnt = Convert.ToInt32(check.Rows[0]["cnt"]);
            if (cnt > 0)
            {
                MessageBox.Show(
                    $"Нельзя удалить товар «{txtName.Text}» — он входит в {cnt} заказ(ов).\n" +
                    "Сначала удалите связанные заказы.",
                    "Удаление запрещено",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            try
            {
                DatabaseHelper.ExecuteNonQuery(
                    "DELETE FROM items WHERE articul = @a",
                    new SqlParameter("@a", articul));

                if (!string.IsNullOrEmpty(currentPhotoPath))
                {
                    string fullPath = Path.Combine(Application.StartupPath, currentPhotoPath);
                    if (File.Exists(fullPath))
                        File.Delete(fullPath);
                }

                MessageBox.Show("Товар успешно удалён.", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка при удалении:\n" + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string GenerateArticul()
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var rnd = new Random();
            char[] result = new char[6];
            for (int i = 0; i < 6; i++)
                result[i] = chars[rnd.Next(chars.Length)];
            return new string(result);
        }
    }
}