namespace Silo.Ui.Gate;

partial class frmLogin
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
        this.btnExit = new System.Windows.Forms.Button();
        this.btnLogin = new System.Windows.Forms.Button();
        this.label7 = new System.Windows.Forms.Label();
        this.label9 = new System.Windows.Forms.Label();
        this.txtUsername = new System.Windows.Forms.TextBox();
        this.txtPassword = new System.Windows.Forms.TextBox();
        this.pictureBox14 = new System.Windows.Forms.PictureBox();
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox14)).BeginInit();
        this.SuspendLayout();
        // 
        // btnExit
        // 
        this.btnExit.Anchor = System.Windows.Forms.AnchorStyles.Top;
        this.btnExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(29)))), ((int)(((byte)(17)))));
        this.btnExit.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnExit.FlatAppearance.BorderColor = System.Drawing.Color.White;
        this.btnExit.FlatAppearance.BorderSize = 0;
        this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnExit.Font = new System.Drawing.Font("IRANSans(FaNum)", 11.75F, System.Drawing.FontStyle.Bold);
        this.btnExit.ForeColor = System.Drawing.Color.White;
        this.btnExit.Location = new System.Drawing.Point(177, 144);
        this.btnExit.Name = "btnExit";
        this.btnExit.Size = new System.Drawing.Size(127, 50);
        this.btnExit.TabIndex = 3;
        this.btnExit.Text = "انصراف";
        this.btnExit.UseVisualStyleBackColor = false;
        this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
        // 
        // btnLogin
        // 
        this.btnLogin.Anchor = System.Windows.Forms.AnchorStyles.Top;
        this.btnLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(166)))), ((int)(((byte)(147)))));
        this.btnLogin.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnLogin.FlatAppearance.BorderColor = System.Drawing.Color.White;
        this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnLogin.Font = new System.Drawing.Font("IRANSans(FaNum)", 11.75F, System.Drawing.FontStyle.Bold);
        this.btnLogin.ForeColor = System.Drawing.Color.White;
        this.btnLogin.Location = new System.Drawing.Point(177, 88);
        this.btnLogin.Name = "btnLogin";
        this.btnLogin.Size = new System.Drawing.Size(127, 50);
        this.btnLogin.TabIndex = 2;
        this.btnLogin.Text = "تأیید";
        this.btnLogin.UseVisualStyleBackColor = false;
        this.btnLogin.Click += new System.EventHandler(this.btnLogin_ClickAsync);
        // 
        // label7
        // 
        this.label7.AutoSize = true;
        this.label7.Font = new System.Drawing.Font("IRANSans(FaNum)", 11F);
        this.label7.Location = new System.Drawing.Point(306, 16);
        this.label7.Name = "label7";
        this.label7.Size = new System.Drawing.Size(76, 25);
        this.label7.TabIndex = 69;
        this.label7.Text = "نام کاربری:";
        // 
        // label9
        // 
        this.label9.AutoSize = true;
        this.label9.Font = new System.Drawing.Font("IRANSans(FaNum)", 11F);
        this.label9.Location = new System.Drawing.Point(306, 54);
        this.label9.Name = "label9";
        this.label9.Size = new System.Drawing.Size(62, 25);
        this.label9.TabIndex = 70;
        this.label9.Text = "رمز عبور:";
        // 
        // txtUsername
        // 
        this.txtUsername.Font = new System.Drawing.Font("IRANSans(FaNum)", 11F);
        this.txtUsername.Location = new System.Drawing.Point(177, 12);
        this.txtUsername.Name = "txtUsername";
        this.txtUsername.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.txtUsername.Size = new System.Drawing.Size(127, 32);
        this.txtUsername.TabIndex = 0;
        // 
        // txtPassword
        // 
        this.txtPassword.Font = new System.Drawing.Font("IRANSans(FaNum)", 11F);
        this.txtPassword.Location = new System.Drawing.Point(177, 50);
        this.txtPassword.Name = "txtPassword";
        this.txtPassword.PasswordChar = '*';
        this.txtPassword.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.txtPassword.Size = new System.Drawing.Size(127, 32);
        this.txtPassword.TabIndex = 1;
        this.txtPassword.UseSystemPasswordChar = true;
        // 
        // pictureBox14
        // 
        this.pictureBox14.BackColor = System.Drawing.Color.Transparent;
        this.pictureBox14.Image = global::Silo.Ui.Gate.Resources.placeholder;
        this.pictureBox14.ImeMode = System.Windows.Forms.ImeMode.NoControl;
        this.pictureBox14.Location = new System.Drawing.Point(11, 12);
        this.pictureBox14.Name = "pictureBox14";
        this.pictureBox14.Size = new System.Drawing.Size(160, 182);
        this.pictureBox14.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
        this.pictureBox14.TabIndex = 71;
        this.pictureBox14.TabStop = false;
        // 
        // frmLogin
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(401, 198);
        this.Controls.Add(this.btnExit);
        this.Controls.Add(this.pictureBox14);
        this.Controls.Add(this.btnLogin);
        this.Controls.Add(this.label7);
        this.Controls.Add(this.label9);
        this.Controls.Add(this.txtUsername);
        this.Controls.Add(this.txtPassword);
        this.Name = "frmLogin";
        this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "کنترل سطح دسترسی کاربران";
        this.Load += new System.EventHandler(this.frmLogin_Load);
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox14)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnExit;
    private System.Windows.Forms.PictureBox pictureBox14;
    private System.Windows.Forms.Button btnLogin;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.TextBox txtUsername;
    private System.Windows.Forms.TextBox txtPassword;
}
