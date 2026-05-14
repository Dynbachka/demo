using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShoesApp
{
    public partial class ProductListForm : Form
    {
        public ProductListForm()
        {
            InitializeComponent();
            this.BackColor = Color.White;
            this.Font = new Font("Times New Roman", 10);
        }

        private void ProductListForm_Load(object sender, EventArgs e)
        {
            lblUserName.Text = CurrentUser.FullName + " (" + CurrentUser.Role + ")";
            LoadProducts();
        }

        private void LoadProducts()
        {
            flpProducts.Controls.Clear();

            DataTable data = DatabaseHelper.GetTable(@"
                SELECT Name, Category, Description, Manufacturer,
                       Supplier, Price, Discount, Quantity, Unit, ImagePath
                FROM Product");

            foreach (DataRow row in data.Rows)
                flpProducts.Controls.Add(CreateCard(row));
        }

        private Panel CreateCard(DataRow row)
        {
            string name = row["Name"].ToString();
            string category = row["Category"].ToString();
            string description = row["Description"].ToString();
            string manufacturer = row["Manufacturer"].ToString();
            string supplier = row["Supplier"].ToString();
            decimal price = Convert.ToDecimal(row["Price"]);
            decimal discount = Convert.ToDecimal(row["Discount"]);
            int quantity = Convert.ToInt32(row["Quantity"]);
            string unit = row["Unit"].ToString();
            string imagePath = row["ImagePath"] == DBNull.Value ? "" : row["ImagePath"].ToString();

            decimal finalPrice = discount > 0
                                   ? Math.Round(price * (1 - discount / 100), 2)
                                   : price;

            Panel card = new Panel
            {
                Width = flpProducts.ClientSize.Width - 25,
                Height = 120,
                BackColor = GetCardColor(discount, quantity),
                Margin = new Padding(4, 4, 4, 0),
                Padding = new Padding(0)
            };
            card.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, card.ClientRectangle,
                    Color.Gray, ButtonBorderStyle.Solid);
            };

            PictureBox pb = new PictureBox
            {
                Width = 120,
                Height = 110,
                Location = new Point(5, 5),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White
            };

            string fullPath = Path.Combine(Application.StartupPath, imagePath);
            string stub = Path.Combine(Application.StartupPath, "picture.png");

            if (!string.IsNullOrEmpty(imagePath) && File.Exists(fullPath))
                pb.Image = Image.FromFile(fullPath);
            else if (File.Exists(stub))
                pb.Image = Image.FromFile(stub);

            Panel center = new Panel
            {
                Left = 130,
                Top = 5,
                Width = card.Width - 250,
                Height = 110,
                BackColor = Color.Transparent
            };

            Label lblTitle = new Label
            {
                Text = category + "  |  " + name,
                Font = new Font("Times New Roman", 10, FontStyle.Bold),
                AutoSize = false,
                Width = center.Width,
                Height = 20,
                Location = new Point(0, 4),
                BackColor = Color.Transparent
            };

            Label lblDesc = MakeLabel("Описание: " + description, 0, 26, center.Width);

            Label lblMfr = MakeLabel("Производитель: " + manufacturer, 0, 44, center.Width);

            Label lblSup = MakeLabel("Поставщик: " + supplier, 0, 60, center.Width);

            Label lblPrice = new Label
            {
                AutoSize = false,
                Width = center.Width,
                Height = 18,
                Location = new Point(0, 76),
                BackColor = Color.Transparent
            };

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

            Label lblUnit = MakeLabel("Ед. изм.: " + unit, 0, 94, center.Width);

            center.Controls.AddRange(new Control[]
                { lblTitle, lblDesc, lblMfr, lblSup, lblPrice, lblUnit });

            Panel right = new Panel
            {
                Left = card.Width - 115,
                Top = 5,
                Width = 110,
                Height = 110,
                BackColor = Color.Transparent
            };

            Label lblDiscount = new Label
            {
                Text = discount > 0 ? "Скидка\n" + discount + "%" : "Без скидки",
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                Width = 110,
                Height = 50,
                Location = new Point(0, 10),
                Font = new Font("Times New Roman", 10, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = discount > 0 ? Color.DarkRed : Color.Gray
            };

            Label lblQty = new Label
            {
                Text = "На складе:\n" + quantity + " шт.",
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                Width = 110,
                Height = 50,
                Location = new Point(0, 62),
                BackColor = Color.Transparent,
                ForeColor = quantity == 0 ? Color.Red : Color.Black
            };

            right.Controls.AddRange(new Control[] { lblDiscount, lblQty });

            card.Controls.AddRange(new Control[] { pb, center, right });
            return card;
        }

        private Color GetCardColor(decimal discount, int quantity)
        {
            if (quantity == 0)
                return Color.LightBlue;
            if (discount > 15)
                return ColorTranslator.FromHtml("#2E8B57");
            return Color.White;
        }

        private Label MakeLabel(string text, int x, int y, int width)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = false,
                Width = width,
                Height = 18,
                BackColor = Color.Transparent
            };
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            foreach (Form f in Application.OpenForms)
            {
                if (f is LoginForm) { f.Show(); break; }
            }
            this.Close();
        }
    }
}