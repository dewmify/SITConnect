using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AppSecAsgn
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoggedIn"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (Session["LoggedIn"] != null)
                {

                }

                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Login.aspx", false);
                }
            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }

        } 
        
        protected void OnChangingPassword(object sender, LoginCancelEventArgs e)
        {
            if (!changePwd.CurrentPassword.Equals(changePwd.NewPassword, StringComparison.CurrentCultureIgnoreCase))
            {
                int rowsAffected = 0;
                string sql = "Update [Accounts] Set [Password] = @NewPassword Where [Email] = @Username and [Password] = @CurrentPassword";
                
                using(SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using(SqlCommand cmd = new SqlCommand(sql))
                    {
                        using (SqlDataAdapter dataAdapter = new SqlDataAdapter())
                        {
                            cmd.Parameters.AddWithValue("@Username", Session["LoggedIn"].ToString());
                            cmd.Parameters.AddWithValue("@CurrentPassword", changePwd.CurrentPassword);
                            cmd.Parameters.AddWithValue("@NewPassword", changePwd.NewPassword);

                            cmd.Connection = con;
                            con.Open();
                            rowsAffected = cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                    if (rowsAffected > 0)
                    {
                        lblMessage.ForeColor = Color.Green;
                        lblMessage.Text = "Password has been successfully!";

                        ChangePasswordAuditLog();
                    }
                    else
                    {
                        lblMessage.ForeColor = Color.Red;
                        lblMessage.Text = "Old password is incorrect.";
                    }
                }
            }
            else
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "Old Password and New Password must not be the same.";
            }

            e.Cancel = true;
        }
        protected void ChangePasswordAuditLog()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("insert into AuditLogs values(@DateAndTime, @UserLog, @ActionLog)"))
                    {
                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@DateAndTime", DateTime.Now);
                            cmd.Parameters.AddWithValue("@UserLog", Session["LoggedIn"].ToString());
                            cmd.Parameters.AddWithValue("@ActionLog", "Has Successfully changed password".ToString());

                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}