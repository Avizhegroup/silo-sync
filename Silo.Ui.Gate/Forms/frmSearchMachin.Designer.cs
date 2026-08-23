namespace Silo.Ui.Gate;

partial class frmSearchMachin
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
        System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
        this.dataGridView1 = new System.Windows.Forms.DataGridView();
        this.groupBox1 = new System.Windows.Forms.GroupBox();
        this.label1 = new System.Windows.Forms.Label();
        this.textBox1 = new System.Windows.Forms.TextBox();
        this.label2 = new System.Windows.Forms.Label();
        this.comboBox1 = new System.Windows.Forms.ComboBox();
        this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.Column3 = new System.Windows.Forms.DataGridViewButtonColumn();
        ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
        this.groupBox1.SuspendLayout();
        this.SuspendLayout();
        // 
        // dataGridView1
        // 
        this.dataGridView1.AllowUserToAddRows = false;
        this.dataGridView1.AllowUserToDeleteRows = false;
        this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
        this.Column4,
        this.Column1,
        this.Column2,
        this.Column3});
        this.dataGridView1.Location = new System.Drawing.Point(1, 78);
        this.dataGridView1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
        this.dataGridView1.Name = "dataGridView1";
        this.dataGridView1.ReadOnly = true;
        this.dataGridView1.RowHeadersWidth = 5;
        this.dataGridView1.RowTemplate.Height = 45;
        this.dataGridView1.Size = new System.Drawing.Size(613, 467);
        this.dataGridView1.TabIndex = 0;
        this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
        // 
        // groupBox1
        // 
        this.groupBox1.Controls.Add(this.comboBox1);
        this.groupBox1.Controls.Add(this.textBox1);
        this.groupBox1.Controls.Add(this.label2);
        this.groupBox1.Controls.Add(this.label1);
        this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
        this.groupBox1.Location = new System.Drawing.Point(0, 0);
        this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
        this.groupBox1.Name = "groupBox1";
        this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
        this.groupBox1.Size = new System.Drawing.Size(618, 72);
        this.groupBox1.TabIndex = 1;
        this.groupBox1.TabStop = false;
        // 
        // label1
        // 
        this.label1.AutoSize = true;
        this.label1.Location = new System.Drawing.Point(543, 28);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(63, 24);
        this.label1.TabIndex = 0;
        this.label1.Text = "نام راننده:";
        // 
        // textBox1
        // 
        this.textBox1.Font = new System.Drawing.Font("IRANSans(FaNum)", 12.25F);
        this.textBox1.Location = new System.Drawing.Point(328, 23);
        this.textBox1.Name = "textBox1";
        this.textBox1.Size = new System.Drawing.Size(209, 35);
        this.textBox1.TabIndex = 1;
        // 
        // label2
        // 
        this.label2.AutoSize = true;
        this.label2.Location = new System.Drawing.Point(248, 28);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(74, 24);
        this.label2.TabIndex = 0;
        this.label2.Text = "نوع ماشین:";
        // 
        // comboBox1
        // 
        this.comboBox1.Font = new System.Drawing.Font("IRANSans(FaNum)", 12.25F);
        this.comboBox1.FormattingEnabled = true;
        this.comboBox1.Location = new System.Drawing.Point(7, 23);
        this.comboBox1.Name = "comboBox1";
        this.comboBox1.Size = new System.Drawing.Size(235, 35);
        this.comboBox1.TabIndex = 2;
        // 
        // Column4
        // 
        this.Column4.HeaderText = "Id";
        this.Column4.Name = "Column4";
        this.Column4.ReadOnly = true;
        this.Column4.Visible = false;
        // 
        // Column1
        // 
        this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
        this.Column1.HeaderText = "نام راننده";
        this.Column1.Name = "Column1";
        this.Column1.ReadOnly = true;
        // 
        // Column2
        // 
        this.Column2.HeaderText = "پلاک ماشین";
        this.Column2.Name = "Column2";
        this.Column2.ReadOnly = true;
        this.Column2.Width = 110;
        // 
        // Column3
        // 
        dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
        dataGridViewCellStyle2.NullValue = "انتخاب";
        this.Column3.DefaultCellStyle = dataGridViewCellStyle2;
        this.Column3.HeaderText = "انتخاب";
        this.Column3.Name = "Column3";
        this.Column3.ReadOnly = true;
        // 
        // frmSearchMachin
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 24F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(618, 548);
        this.Controls.Add(this.groupBox1);
        this.Controls.Add(this.dataGridView1);
        this.Font = new System.Drawing.Font("IRANSans(FaNum)", 10.25F);
        this.KeyPreview = true;
        this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
        this.Name = "frmSearchMachin";
        this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "جستوی ماشین حمل";
        this.Load += new System.EventHandler(this.frmSearchMachin_Load);
        ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
        this.groupBox1.ResumeLayout(false);
        this.groupBox1.PerformLayout();
        this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataGridView dataGridView1;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.ComboBox comboBox1;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
    private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
    private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
    private System.Windows.Forms.DataGridViewButtonColumn Column3;
}
