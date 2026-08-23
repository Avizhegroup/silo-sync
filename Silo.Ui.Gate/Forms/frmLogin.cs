namespace Silo.Ui.Gate;

public partial class frmLogin : Form
{
    public frmLogin()
    {
        InitializeComponent();
        
    }

    private void frmLogin_Load(object sender, EventArgs e)
    {

    }

    private  void btnLogin_ClickAsync(object sender, EventArgs e)
    {
        Login();
    }

    private async Task Login()
    {
        var apiBusiness = new DAL.ApiBusiness();
        try
        {
            if (await apiBusiness.Login(txtUsername.Text.TrimEnd(), txtPassword.Text.TrimEnd()))
            {
                if (txtUsername.Text.Trim().ToLower()=="admin"  ||
                     txtUsername.Text.Trim().ToLower()=="a.taheri" ||
                     txtUsername.Text.Trim().ToLower()=="j.negahdari")
                {
                    DialogResult = DialogResult.Yes;
                    this.Hide();
                }
            }
            else
            {
                DialogResult = DialogResult.No;

                MessageBox.Show("کاربری شما مجاز به اجرای این عملیات نمی باشد");
            }
        }
        catch (Exception ex)
        {

        }
    }
    //private bool SelectUserType(string username, string password)
    //{
    //    bool flagResult = false;
    //    SqlParameter[] result = null;
    //    try
    //    {
    //        SqlConnection con = new SqlConnection { ConnectionString = TDA_RFIDConnect_AntennaConnector.Properties.Settings.Default.Connection_String };
    //        using (SqlCommand cmd = new SqlCommand())
    //        {
    //            cmd.CommandType = CommandType.Text;
    //            cmd.CommandText = "SELECT DegEdu FROM tbl_Personals WHERE (PosDesc = N'" + username +
    //                      "') AND (loginPass = N'" + password + "') AND (IsActive=1)";
    //            cmd.Connection = con;
    //            con.Open();
    //            SqlDataReader reader = cmd.ExecuteReader();

    //            if (reader.Read())
    //            {

    //                if (reader[0].ToString() == "1")
    //                {
    //                    flagResult = true;
    //                       DialogResult = DialogResult.Yes;
    //                }

    //            }
    //            else
    //                DialogResult = DialogResult.No;


    //            reader.Close();
    //            con.Close();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        result = null;
    //    }

    //    return flagResult;





    //}

    private void btnExit_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.No;
        this.Hide();

    }
}
