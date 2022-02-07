using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AppSecAsgn
{
    public partial class HomePage : System.Web.UI.Page
    {
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
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
        }
    }
}