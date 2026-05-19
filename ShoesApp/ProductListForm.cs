using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShoesApp
{
    public partial class ProductListForm : Form
    {
        private string sortColumn = "name_item";
        private string sortDirection = "ASC";

        private static bool editFormOpen = false;

        public ProductListForm()
        {
            InitializeComponent();
            this.BackColor = Color.White;
            this.Font = new Font("Times New Roman", 10);
            this.Text = "Список товаров — ООО Обувь";
        }

        private void ProductListForm_Load(object sender, EventArgs e)
        {
            lblUserName.Text = CurrentUser.FullName + " (" + CurrentUser.Role + ")";

            bool canSearch = CurrentUser.IsManager || CurrentUser.IsAdministrator;
            txtSearch.Visible = canSearch;
            cboSupplier.Visible = canSearch;
            btnSortAsc.Visible = canSearch;
            btnSortDesc.Visible = canSearch;

            btnAdd.Visible = CurrentUser.IsAdministrator;

            if (canSearch)
                LoadSuppliers();

            LoadProducts();
        }

        private void LoadSuppliers()
        {
            DataTable dt = DatabaseHelper.GetTable(
                "SELECT DISTINCT courier FROM items WHERE courier IS NOT NULL ORDER BY courier");

            cboSupplier.Items.Clear();
            cboSupplier.Items.Add("Все поставщики");
            foreach (DataRow row in dt.Rows)
                cboSupplier.Items.Add(row["courier"].ToString());

            cboSupplier.SelectedIndex = 0;
        }

        private void LoadProducts()
        {
            flpProducts.Controls.Clear();

            string search = txtSearch.Visible ? txtSearch.Text.Trim() : "";
            string supplier = (cboSupplier.Visible && cboSupplier.SelectedIndex > 0)
                              ? cboSupplier.SelectedItem.ToString() : "";

            string safeCol = sortColumn == "count" ? "count" : "name_item";
            string safeDir = sortDirection == "DESC" ? "DESC" : "ASC";

            string query = $@"
                SELECT articul, name_item, category, about,
                       author, courier, price, discount, count, unit, photo
                FROM items
                WHERE 1=1
                {(supplier != "" ? "AND courier = @supplier" : "")}
                {(search != "" ? @"AND (
                    articul   LIKE @search OR
                    name_item LIKE @search OR
                    category  LIKE @search OR
                    about     LIKE @search OR
                    author    LIKE @search OR
                    courier   LIKE @search
                )" : "")}
                ORDER BY {safeCol} {safeDir}";

            var parameters = new System.Collections.Generic.List<SqlParameter>();
            if (supplier != "")
                parameters.Add(new SqlParameter("@supplier", supplier));
            if (search != "")
                parameters.Add(new SqlParameter("@search", "%" + search + "%"));

            DataTable data = DatabaseHelper.GetTable(query, parameters.ToArray());

            foreach (DataRow row in data.Rows)
                flpProducts.Controls.Add(CreateCard(row));
        }

        private Panel CreateCard(DataRow row)
        {
            string name = row["name_item"].ToString();
            string category = row["category"].ToString();
            string about = row["about"] == DBNull.Value ? "" : row["about"].ToString();
            string author = row["author"].ToString();
            string courier = row["courier"] == DBNull.Value ? "" : row["courier"].ToString();
            decimal price = Convert.ToDecimal(row["price"]);
            int discount = row["discount"] == DBNull.Value ? 0 : Convert.ToInt32(row["discount"]);
            int quantity = Convert.ToInt32(row["count"]);
            string unit = row["unit"].ToString();
            string photo = row["photo"] == DBNull.Value ? "" : row["photo"].ToString();
            string articul = row["articul"].ToString();

            decimal finalPrice = discount > 0
                                 ? Math.Round(price * (1 - discount / 100m), 2)
                                 : price;

            Panel card = new Panel
            {
                Width = flpProducts.ClientSize.Width - 25,
                Height = 140,
                BackColor = GetCardColor(discount, quantity),
                Margin = new Padding(4, 4, 4, 0),
                Tag = articul  
            };
            card.Paint += (s, e) =>
                ControlPaint.DrawBorder(e.Graphics, card.ClientRectangle,
                    Color.Gray, ButtonBorderStyle.Solid);

            card.Click += Card_Click;

            PictureBox pb = new PictureBox
            {
                Width = 120,
                Height = 120,
                Location = new Point(5, 5),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White,
                Tag = articul
            };
            pb.Click += Card_Click; 
            string fullPath = Path.Combine(Application.StartupPath, photo);
            string stub = Path.Combine(Application.StartupPath, "picture.png");
            if (!string.IsNullOrEmpty(photo) && File.Exists(fullPath))
                pb.Image = Image.FromFile(fullPath);
            else if (File.Exists(stub))
                pb.Image = Image.FromFile(stub);

            Panel center = new Panel
            {
                Left = 130,
                Top = 5,
                Width = card.Width - 245,
                Height = 130,
                BackColor = Color.Transparent,
                Tag = articul
            };
            center.Click += Card_Click;

            Label lblTitle = new Label
            {
                Text = category + "  |  " + name,
                Font = new Font("Times New Roman", 10, FontStyle.Bold),
                AutoSize = false,
                Width = center.Width,
                Height = 20,
                Location = new Point(0, 2),
                BackColor = Color.Transparent,
                Tag = articul
            };
            lblTitle.Click += Card_Click;

            Label lblAbout = MakeLabel("Описание: " + about, 0, 24, center.Width, articul);
            Label lblAuthor = MakeLabel("Производитель: " + author, 0, 42, center.Width, articul);
            Label lblCourier = MakeLabel("Поставщик: " + courier, 0, 60, center.Width, articul);

            Label lblPrice = new Label
            {
                AutoSize = false,
                Width = center.Width,
                Height = 18,
                Location = new Point(0, 78),
                BackColor = Color.Transparent,
                Font = new Font("Times New Roman", 10),
                Tag = articul
            };
            lblPrice.Click += Card_Click;

            if (discount > 0)
            {
                lblPrice.Paint += (s, pe) =>
                {
                    string old = "Цена: " + price.ToString("0.##");
                    string neu = "  → " + finalPrice.ToString("0.##");
                    using (Font sf = new Font("Times New Roman", 10, FontStyle.Strikeout))
                    using (SolidBrush rb = new SolidBrush(Color.Red))
                    using (SolidBrush bb = new SolidBrush(Color.Black))
                    {
                        SizeF sz = pe.Graphics.MeasureString(old, sf);
                        pe.Graphics.DrawString(old, sf, rb, 0, 0);
                        pe.Graphics.DrawString(neu, ((Label)s).Font, bb, sz.Width, 0);
                    }
                };
            }
            else
            {
                lblPrice.Text = "Цена: " + price.ToString("0.##");
            }

            Label lblUnit = MakeLabel("Ед. изм.: " + unit, 0, 96, center.Width, articul);

            Label lblQty = new Label
            {
                Text = "Кол-во на складе: " + quantity + " шт.",
                Location = new Point(0, 114),
                AutoSize = false,
                Width = center.Width,
                Height = 18,
                BackColor = Color.Transparent,
                ForeColor = quantity == 0 ? Color.Red : Color.Black,
                Tag = articul
            };
            lblQty.Click += Card_Click;

            center.Controls.AddRange(new Control[]
                { lblTitle, lblAbout, lblAuthor, lblCourier, lblPrice, lblUnit, lblQty });

            Panel right = new Panel
            {
                Left = card.Width - 110,
                Top = 5,
                Width = 100,
                Height = 120,
                BackColor = Color.Transparent
            };

            Label lblDiscount = new Label
            {
                Text = discount > 0 ? "Скидка\n" + discount + "%" : "Без\nскидки",
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                Width = 100,
                Height = 120,
                Location = new Point(0, 0),
                Font = new Font("Times New Roman", 10, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = discount > 0 ? Color.DarkRed : Color.Gray
            };

            right.Controls.Add(lblDiscount);
            card.Controls.AddRange(new Control[] { pb, center, right });
            return card;
        }

        private void Card_Click(object sender, EventArgs e)
        {
            if (!CurrentUser.IsAdministrator) return;

            if (editFormOpen)
            {
                MessageBox.Show(
                    "Окно редактирования уже открыто.\nЗакройте его перед открытием нового.",
                    "Предупреждение",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string articul = ((Control)sender).Tag?.ToString();
            if (string.IsNullOrEmpty(articul)) return;

            editFormOpen = true;
            ProductEditForm editForm = new ProductEditForm(articul);
            editForm.FormClosed += (s, args) =>
            {
                editFormOpen = false;
                LoadProducts(); 
            };
            editForm.Show();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (editFormOpen)
            {
                MessageBox.Show(
                    "Закройте открытое окно редактирования перед добавлением нового товара.",
                    "Предупреждение",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            editFormOpen = true;
            ProductEditForm editForm = new ProductEditForm(""); 
            editForm.FormClosed += (s, args) =>
            {
                editFormOpen = false;
                LoadProducts();
            };
            editForm.Show();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e) => LoadProducts();

        private void cboSupplier_SelectedIndexChanged(object sender, EventArgs e) => LoadProducts();

        private void btnSortAsc_Click(object sender, EventArgs e)
        {
            sortColumn = "count";
            sortDirection = "ASC";
            LoadProducts();
        }

        private void btnSortDesc_Click(object sender, EventArgs e)
        {
            sortColumn = "count";
            sortDirection = "DESC";
            LoadProducts();
        }

        private Color GetCardColor(int discount, int quantity)
        {
            if (quantity == 0) return Color.LightBlue;
            if (discount > 15) return ColorTranslator.FromHtml("#2E8B57");
            return Color.White;
        }

        private Label MakeLabel(string text, int x, int y, int width, string tag = "")
        {
            var lbl = new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = false,
                Width = width,
                Height = 18,
                BackColor = Color.Transparent,
                Tag = tag
            };
            lbl.Click += Card_Click;
            return lbl;
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            foreach (Form f in Application.OpenForms)
                if (f is LoginForm) { f.Show(); break; }
            this.Close();
        }

       
    }
}