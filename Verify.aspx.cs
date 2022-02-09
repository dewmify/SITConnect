using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AppSecAsgn
{
    public partial class Verify : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDBConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoggedIn"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Login.aspx", false);
                }
            }

        }

        protected string vCodeOTP(string email)
        {
            string otp = null;
            SqlConnection con = new SqlConnection(MYDBConnectionString);
            string sql = "select Verification from Accounts where Email = @EMAIL";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@EMAIL", email);
            try
            {
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        otp = reader["Verification"].ToString();
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return otp;
        }

        protected void VerifyCode(object sender, EventArgs e)
        {
            if(HttpUtility.HtmlEncode(tb_vfCode.Text.ToString()) == vCodeOTP(Session["LoggedIn"].ToString()))
            {
                LoginAuditLog();
                
                Response.Redirect("Homepage.aspx", false);

            }
            else
            {
                lblMessage.Text = "Verification code is incorrect";
            }
        }

        protected void LoginAuditLog()
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
                            cmd.Parameters.AddWithValue("@ActionLog", "Has Successfully logged into account".ToString());

                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
;
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