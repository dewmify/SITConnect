using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AppSecAsgn
{
    public partial class Login : System.Web.UI.Page
    {

        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDBConnection"].ConnectionString;
        static string rndNumber;
        //Log log = new Log();
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected string getDBHash(string userid)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordHash FROM Account WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                       
                            if (reader["PasswordHash"] != DBNull.Value)
                                {
                                    h = reader["PasswordHash"].ToString();
                                }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;
        }

        protected string getDBSalt(string userid)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PASSWORDSALT FROM ACCOUNT WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PASSWORDSALT"] != null)
                        {
                            if (reader["PASSWORDSALT"] != DBNull.Value)
                            {
                                s = reader["PASSWORDSALT"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }


        protected void LoginMe(object sender, EventArgs e)
        {
            string pwd = tb_pwd.Text.ToString().Trim();
            string userid = tb_userid.Text.ToString().Trim();
            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBHash(userid);
            string dbSalt = getDBSalt(userid);
            try
            {
                //string currentAttempt = Int32.Parse(countAttempt(userid)).ToString();
                //if (Int32.Parse(currentAttempt) != 0)
                //{
                    if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                    {
                        string pwdAndSalt = pwd + dbSalt;
                        byte[] hashAndSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdAndSalt));
                        string userHash = Convert.ToBase64String(hashAndSalt);

                        if (userHash == dbHash)
                        {
                            Session["LoggedIn"] = tb_userid.Text.Trim();

                            string guid = Guid.NewGuid().ToString();
                            Session["AuthToken"] = guid;

                            Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                            //resetAttempt(userid, "3");

                            Random rnd = new Random();
                            rndNumber = (rnd.Next(100000, 999999)).ToString();

                            //log.logged(userid, "Login Success!!");

                            Response.Redirect("Homepage.aspx", false);
                        }
                        else
                        {
                            lblMessage.Text = "Invalid login details";

                            //log.logged(userid, "Login Failed >:(");
                        }
                    }
                //}
                else
                {
                    lblMessage.Text = "You have been locked out of the account due to too many invalid login attempts";

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            { }
        }
    }
}