using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
                string sql = "Update [Accounts] Set [PasswordHash] = @PasswordHash, [PasswordSalt] = @PasswordSalt Where [Email] = @EMAIL";

                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        using (SqlDataAdapter dataAdapter = new SqlDataAdapter())
                        {
                            string pwd = HttpUtility.HtmlEncode(changePwd.NewPassword.ToString());

                            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                            byte[] saltByte = new byte[8];

                            rng.GetBytes(saltByte);
                            salt = Convert.ToBase64String(saltByte);
                            SHA512Managed hashing = new SHA512Managed();
                            string pwdWithSalt = pwd + salt;
                            byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                            finalHash = Convert.ToBase64String(hashWithSalt);


                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@EMAIL", Session["LoggedIn"].ToString());
                            cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt", salt);

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

        protected string getDBHash(string userid)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordHash FROM Accounts WHERE Email=@USERID";
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
            string sql = "select PASSWORDSALT FROM ACCOUNTS WHERE Email=@USERID";
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
            finally
            {
                connection.Close();
            }
            return s;
        }
    }
}