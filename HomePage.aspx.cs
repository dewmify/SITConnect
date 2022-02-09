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
    public partial class HomePage : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDBConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoggedIn"] != null)
            {
                lblMessage.Text = "Congratulations!, you are logged in";
                lblMessage.ForeColor = System.Drawing.Color.Green;
                btnLogout.Visible = true;
            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }

        }
        protected void LogoutMe(object sender, EventArgs e)
        {
            
            LogoutAuditLog();
            
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            

            Response.Redirect("Login.aspx", false);

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Request.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Request.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }

            if (Request.Cookies["AuthToken"] != null)
            {
                Request.Cookies["AuthToken"].Value = string.Empty;
                Request.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }
        }

        protected void LogoutAuditLog()
        {
            try
            {
                using(SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using(SqlCommand cmd = new SqlCommand("insert into AuditLogs values(@DateAndTime, @UserLog, @ActionLog)"))
                    {
                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@DateAndTime", DateTime.Now);
                            cmd.Parameters.AddWithValue("@UserLog", Session["LoggedIn"].ToString());
                            cmd.Parameters.AddWithValue("@ActionLog", "Has Successfully logged out of account".ToString());

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