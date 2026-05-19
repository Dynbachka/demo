namespace ShoesApp
{
    partial class ProductListForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblUserName = new System.Windows.Forms.Label();
            this.btnLogout = new System.Windows.Forms.Button();
            this.flpProducts = new System.Windows.Forms.FlowLayoutPanel();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.cboSupplier = new System.Windows.Forms.ComboBox();
            this.btnSortAsc = new System.Windows.Forms.Button();
            this.btnSortDesc = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(532, 9);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(44, 16);
            this.lblUserName.TabIndex = 0;
            this.lblUserName.Text = "label1";
            // 
            // btnLogout
            // 
            this.btnLogout.Location = new System.Drawing.Point(780, 9);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(75, 23);
            this.btnLogout.TabIndex = 1;
            this.btnLogout.Text = "выйти";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // flpProducts
            // 
            this.flpProducts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpProducts.AutoScroll = true;
            this.flpProducts.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpProducts.Location = new System.Drawing.Point(12, 105);
            this.flpProducts.Name = "flpProducts";
            this.flpProducts.Size = new System.Drawing.Size(843, 390);
            this.flpProducts.TabIndex = 2;
            this.flpProducts.WrapContents = false;
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(12, 77);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(100, 22);
            this.txtSearch.TabIndex = 3;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // cboSupplier
            // 
            this.cboSupplier.FormattingEnabled = true;
            this.cboSupplier.Location = new System.Drawing.Point(417, 75);
            this.cboSupplier.Name = "cboSupplier";
            this.cboSupplier.Size = new System.Drawing.Size(121, 24);
            this.cboSupplier.TabIndex = 4;
            this.cboSupplier.SelectedIndexChanged += new System.EventHandler(this.cboSupplier_SelectedIndexChanged);
            // 
            // btnSortAsc
            // 
            this.btnSortAsc.Location = new System.Drawing.Point(593, 77);
            this.btnSortAsc.Name = "btnSortAsc";
            this.btnSortAsc.Size = new System.Drawing.Size(134, 23);
            this.btnSortAsc.TabIndex = 5;
            this.btnSortAsc.Text = "по возсраст";
            this.btnSortAsc.UseVisualStyleBackColor = true;
            this.btnSortAsc.Click += new System.EventHandler(this.btnSortAsc_Click);
            // 
            // btnSortDesc
            // 
            this.btnSortDesc.Location = new System.Drawing.Point(733, 77);
            this.btnSortDesc.Name = "btnSortDesc";
            this.btnSortDesc.Size = new System.Drawing.Size(122, 23);
            this.btnSortDesc.TabIndex = 6;
            this.btnSortDesc.Text = "по убыв";
            this.btnSortDesc.UseVisualStyleBackColor = true;
            this.btnSortDesc.Click += new System.EventHandler(this.btnSortDesc_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(254, 77);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(134, 23);
            this.btnAdd.TabIndex = 7;
            this.btnAdd.Text = "добавить товар";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // ProductListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(867, 496);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnSortDesc);
            this.Controls.Add(this.btnSortAsc);
            this.Controls.Add(this.cboSupplier);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.flpProducts);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.lblUserName);
            this.Name = "ProductListForm";
            this.Text = "ProductListForm";
            this.Load += new System.EventHandler(this.ProductListForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.FlowLayoutPanel flpProducts;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.ComboBox cboSupplier;
        private System.Windows.Forms.Button btnSortAsc;
        private System.Windows.Forms.Button btnSortDesc;
        private System.Windows.Forms.Button btnAdd;
    }
}