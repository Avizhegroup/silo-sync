namespace Silo.Ui.Gate;

partial class frmRegister
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
        this.pictureBox1 = new System.Windows.Forms.PictureBox();
        this.label1 = new System.Windows.Forms.Label();
        this.label2 = new System.Windows.Forms.Label();
        this.label3 = new System.Windows.Forms.Label();
        this.button1 = new System.Windows.Forms.Button();
        this.button2 = new System.Windows.Forms.Button();
        this.lblAppSerial = new System.Windows.Forms.Label();
        this.lblCpuId = new System.Windows.Forms.Label();
        this.txtActivationCode = new System.Windows.Forms.TextBox();
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
        this.SuspendLayout();
        // 
        // pictureBox1
        // 
        this.pictureBox1.Image = global::Silo.Ui.Gate.Resources.ست_اداری;
        this.pictureBox1.Location = new System.Drawing.Point(358, 2);
        this.pictureBox1.Name = "pictureBox1";
        this.pictureBox1.Size = new System.Drawing.Size(351, 357);
        this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
        this.pictureBox1.TabIndex = 38;
        this.pictureBox1.TabStop = false;
        // 
        // label1
        // 
        this.label1.BackColor = System.Drawing.Color.Cornsilk;
        this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.label1.Font = new System.Drawing.Font("Tahoma", 10.25F);
        this.label1.Location = new System.Drawing.Point(226, 42);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(109, 32);
        this.label1.TabIndex = 39;
        this.label1.Text = "سریال نرم افزار:";
        this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        // 
        // label2
        // 
        this.label2.BackColor = System.Drawing.Color.Lavender;
        this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.label2.Font = new System.Drawing.Font("Tahoma", 10.25F);
        this.label2.Location = new System.Drawing.Point(226, 76);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(109, 32);
        this.label2.TabIndex = 39;
        this.label2.Text = "سریال دستگاه:";
        this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        // 
        // label3
        // 
        this.label3.BackColor = System.Drawing.Color.AliceBlue;
        this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.label3.Font = new System.Drawing.Font("Tahoma", 10.25F);
        this.label3.Location = new System.Drawing.Point(226, 110);
        this.label3.Name = "label3";
        this.label3.Size = new System.Drawing.Size(109, 32);
        this.label3.TabIndex = 39;
        this.label3.Text = "کد رجیستر:";
        this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        // 
        // button1
        // 
        this.button1.BackColor = System.Drawing.Color.PaleGreen;
        this.button1.Font = new System.Drawing.Font("Tahoma", 14.25F);
        this.button1.Location = new System.Drawing.Point(27, 169);
        this.button1.Name = "button1";
        this.button1.Size = new System.Drawing.Size(308, 79);
        this.button1.TabIndex = 40;
        this.button1.Text = "ثبت کد فعال ساز";
        this.button1.UseVisualStyleBackColor = false;
        this.button1.Click += new System.EventHandler(this.button1_Click);
        // 
        // button2
        // 
        this.button2.BackColor = System.Drawing.Color.Salmon;
        this.button2.Font = new System.Drawing.Font("Tahoma", 14.25F);
        this.button2.Location = new System.Drawing.Point(27, 254);
        this.button2.Name = "button2";
        this.button2.Size = new System.Drawing.Size(308, 69);
        this.button2.TabIndex = 40;
        this.button2.Text = "انصراف";
        this.button2.UseVisualStyleBackColor = false;
        this.button2.Click += new System.EventHandler(this.button2_Click);
        // 
        // lblAppSerial
        // 
        this.lblAppSerial.BackColor = System.Drawing.Color.Cornsilk;
        this.lblAppSerial.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.lblAppSerial.Font = new System.Drawing.Font("Tahoma", 10.25F);
        this.lblAppSerial.Location = new System.Drawing.Point(27, 42);
        this.lblAppSerial.Name = "lblAppSerial";
        this.lblAppSerial.Size = new System.Drawing.Size(197, 32);
        this.lblAppSerial.TabIndex = 39;
        this.lblAppSerial.Text = " ";
        this.lblAppSerial.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        // 
        // lblCpuId
        // 
        this.lblCpuId.BackColor = System.Drawing.Color.Lavender;
        this.lblCpuId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.lblCpuId.Font = new System.Drawing.Font("Tahoma", 10.25F);
        this.lblCpuId.Location = new System.Drawing.Point(27, 76);
        this.lblCpuId.Name = "lblCpuId";
        this.lblCpuId.Size = new System.Drawing.Size(197, 32);
        this.lblCpuId.TabIndex = 39;
        this.lblCpuId.Text = " ";
        this.lblCpuId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        // 
        // txtActivationCode
        // 
        this.txtActivationCode.BackColor = System.Drawing.Color.White;
        this.txtActivationCode.Font = new System.Drawing.Font("Tahoma", 15.25F);
        this.txtActivationCode.Location = new System.Drawing.Point(27, 110);
        this.txtActivationCode.Name = "txtActivationCode";
        this.txtActivationCode.Size = new System.Drawing.Size(197, 32);
        this.txtActivationCode.TabIndex = 41;
        // 
        // frmRegister
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(711, 360);
        this.Controls.Add(this.txtActivationCode);
        this.Controls.Add(this.button2);
        this.Controls.Add(this.button1);
        this.Controls.Add(this.label3);
        this.Controls.Add(this.lblCpuId);
        this.Controls.Add(this.lblAppSerial);
        this.Controls.Add(this.label2);
        this.Controls.Add(this.label1);
        this.Controls.Add(this.pictureBox1);
        this.Font = new System.Drawing.Font("Tahoma", 8.25F);
        this.Name = "frmRegister";
        this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        this.Text = "فرم رجیستر نرم افزار";
        this.Load += new System.EventHandler(this.frmRegister_Load);
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.Label lblAppSerial;
    private System.Windows.Forms.Label lblCpuId;
    private System.Windows.Forms.TextBox txtActivationCode;
}
