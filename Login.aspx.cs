using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AppSecAsgn
{
    public partial class Login : System.Web.UI.Page
    {

        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDBConnection"].ConnectionString;
        static string rndNumber;
        protected void Page_Load(object sender, EventArgs e)
        {

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


        protected void LoginMe(object sender, EventArgs e)
        {
            string pwd = tb_pwd.Text.ToString().Trim();
            string email = tb_email.Text.ToString().Trim();
            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBHash(email);
            string dbSalt = getDBSalt(email);

            try
            {
                string pwdAndSalt = pwd + dbSalt;
                byte[] hashAndSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdAndSalt));
                string userHash = Convert.ToBase64String(hashAndSalt);

                DataSet dataSet = new DataSet();

                SqlConnection connection = new SqlConnection(MYDBConnectionString);
                string sql = "Select * from Accounts Where [Email]='"
                    + email
                    + "' and [StatusId] = 1;";

                connection.Open();

                SqlDataAdapter dataAdapter = new SqlDataAdapter(sql, connection);

                dataAdapter.Fill(dataSet);
                connection.Close();

                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    if (ValidateCaptcha())
                    {
                        if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                        {
                            if (userHash == dbHash)
                            {
                                Session["LoginCount"] = 0;
                                Session["LoggedIn"] = tb_email.Text.Trim();

                                string guid = Guid.NewGuid().ToString();
                                Session["AuthToken"] = guid;

                                Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                                Random random = new Random();
                                rndNumber = random.Next(000000, 999999).ToString();
                                createOTP(email, rndNumber);

                                SendVCode(rndNumber);


                                Response.Redirect("Verify.aspx", false);
                            }
                            else
                            {
                                lblMessage.Text = "Invalid login details";

                            }
                        }
                    }
                }
                else
                {
                    Session["LoginCount"] = Convert.ToInt32(Session["LoginCount"]) + 1;
                    if (Convert.ToInt32(Session["LoginCount"]) > 3)
                    {
                        lblMessage.Text = DeactivateAccount();
                    }
                    else
                    {
                        lblMessage.Text = "Invalid Login Details";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        private string DeactivateAccount()
        {

            DataSet dataSet = new DataSet();
            string pwd = tb_pwd.Text.ToString().Trim();
            string email = tb_email.Text.ToString().Trim();
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "Select * from Accounts Where [Email]='"
                + email
                + "';Update Accounts set StatusId = 0 Where [Email]='"
                + email
                + "';";
            connection.Open();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(sql, connection);
            dataAdapter.Fill(dataSet);
            if(dataSet.Tables[0].Rows.Count > 0)
            {
                return "Your Account has been locked, contact an admin to retrieve your account.";
            }
            else
            {
                return "Entered user id does not belong to this application";
            }
            connection.Close();
        }

        protected string createOTP(string userid, string rndNumber)
        {
            string otp = null;
            SqlConnection con = new SqlConnection(MYDBConnectionString);
            string sql = "update Accounts set Verification = @Verification where Email = @Email";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Verification", rndNumber);
            cmd.Parameters.AddWithValue("@EMAIL", userid);
            try
            {
                con.Open();
                using(SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Verification"] != null)
                        {
                            otp = reader["Verification"].ToString();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { con.Close(); }
            return otp;
        }

        protected string SendVCode(string vcode)
        {
            string fromaddress = "SITConnect <sitconnect123@gmail.com>";
            string str = null;
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("sitconnect123@gmail.com", "SITConnect@3182"),
                EnableSsl = true
            };
            var mailMessage = new MailMessage
            {
                Subject = "Account Verification",
                Body = "Dear User, your verification code is " + vcode + "\nThank you for using our website!"
            };
            mailMessage.To.Add(tb_email.Text.ToString());
            mailMessage.From = new MailAddress(fromaddress);
            try
            {
                smtpClient.Send(mailMessage);
                return str;
            }
            catch
            {
                throw;
            }
        }

        public class MyObject
        {
            public string success { get; set; }
            public List<string> ErrorMessage { get; set; }
        }

        public bool ValidateCaptcha()
        {
            bool result = true;

            string response = Request.Form["g-recaptcha-response"];

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(" https://www.google.com/recaptcha/api/siteverify?secret=6Ld-FWkeAAAAAKOeBM13sQc1F1fsgeSHelhe2Vhq &response=" + response);
            try
            {
                using (WebResponse webResponse = req.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(webResponse.GetResponseStream()))
                    {
                        string jsonResponse = sr.ReadToEnd();


                        JavaScriptSerializer js = new JavaScriptSerializer();

                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);

                        result = Convert.ToBoolean(jsonObject.success);

                    }
                }

                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

    }
}