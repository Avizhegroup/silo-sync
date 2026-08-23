namespace Silo.Ui.Gate;
partial class frmConnectionSetting
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
        this.groupBox3 = new System.Windows.Forms.GroupBox();
        this.txtServerIp = new System.Windows.Forms.TextBox();
        this.label12 = new System.Windows.Forms.Label();
        this.btnSave = new System.Windows.Forms.Button();
        this.label1 = new System.Windows.Forms.Label();
        this.txtServerPort = new System.Windows.Forms.TextBox();
        this.button1 = new System.Windows.Forms.Button();
        this.groupBox3.SuspendLayout();
        this.SuspendLayout();
        // 
        // groupBox3
        // 
        this.groupBox3.Controls.Add(this.button1);
        this.groupBox3.Controls.Add(this.btnSave);
        this.groupBox3.Controls.Add(this.txtServerPort);
        this.groupBox3.Controls.Add(this.label1);
        this.groupBox3.Controls.Add(this.txtServerIp);
        this.groupBox3.Controls.Add(this.label12);
        this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
        this.groupBox3.Location = new System.Drawing.Point(0, 0);
        this.groupBox3.Name = "groupBox3";
        this.groupBox3.Size = new System.Drawing.Size(419, 155);
        this.groupBox3.TabIndex = 4;
        this.groupBox3.TabStop = false;
        // 
        // txtServerIp
        // 
        this.txtServerIp.Font = new System.Drawing.Font("IRANSans(FaNum)", 12.25F);
        this.txtServerIp.Location = new System.Drawing.Point(12, 20);
        this.txtServerIp.Name = "txtServerIp";
        this.txtServerIp.Size = new System.Drawing.Size(303, 35);
        this.txtServerIp.TabIndex = 0;
        // 
        // label12
        // 
        this.label12.AutoSize = true;
        this.label12.Font = new System.Drawing.Font("IRANSans(FaNum)", 12.25F);
        this.label12.Location = new System.Drawing.Point(318, 23);
        this.label12.Name = "label12";
        this.label12.Size = new System.Drawing.Size(86, 28);
        this.label12.TabIndex = 0;
        this.label12.Text = "آیپی سرور :";
        // 
        // btnSave
        // 
        this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
        this.btnSave.Font = new System.Drawing.Font("IRANSans(FaNum)", 11F);
        this.btnSave.Location = new System.Drawing.Point(12, 106);
        this.btnSave.Name = "btnSave";
        this.btnSave.Size = new System.Drawing.Size(296, 43);
        this.btnSave.TabIndex = 2;
        this.btnSave.Text = "ثبت تنظیمات";
        this.btnSave.UseVisualStyleBackColor = false;
        this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
        // 
        // label1
        // 
        this.label1.AutoSize = true;
        this.label1.Font = new System.Drawing.Font("IRANSans(FaNum)", 12.25F);
        this.label1.Location = new System.Drawing.Point(318, 64);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(95, 28);
        this.label1.TabIndex = 0;
        this.label1.Text = "پورت اتصال:";
        // 
        // txtServerPort
        // 
        this.txtServerPort.Font = new System.Drawing.Font("IRANSans(FaNum)", 12.25F);
        this.txtServerPort.Location = new System.Drawing.Point(187, 61);
        this.txtServerPort.Name = "txtServerPort";
        this.txtServerPort.Size = new System.Drawing.Size(128, 35);
        this.txtServerPort.TabIndex = 1;
        this.txtServerPort.Leave += new System.EventHandler(this.txtServerPort_Leave);
        // 
        // button1
        // 
        this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
        this.button1.Font = new System.Drawing.Font("IRANSans(FaNum)", 11F);
        this.button1.Location = new System.Drawing.Point(313, 106);
        this.button1.Name = "button1";
        this.button1.Size = new System.Drawing.Size(95, 43);
        this.button1.TabIndex = 3;
        this.button1.Text = "انصراف";
        this.button1.UseVisualStyleBackColor = false;
        this.button1.Click += new System.EventHandler(this.button1_Click);
        // 
        // frmConnectionSetting
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 19F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(419, 155);
        this.Controls.Add(this.groupBox3);
        this.Font = new System.Drawing.Font("IRANSans(FaNum)", 8.249999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.KeyPreview = true;
        this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
        this.Name = "frmConnectionSetting";
        this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "تنظیم اتصال نرم افزار به سرور";
        this.Load += new System.EventHandler(this.frmConnectionSetting_Load);
        this.groupBox3.ResumeLayout(false);
        this.groupBox3.PerformLayout();
        this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.TextBox txtServerIp;
    private System.Windows.Forms.Label label12;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.TextBox txtServerPort;
    private System.Windows.Forms.Label label1;
}
