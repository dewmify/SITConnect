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
        int loginAttempts = 0;
        DateTime lockoutDateTime;
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
            string pwd = HttpUtility.HtmlEncode(tb_pwd.Text.ToString().Trim());
            string email = HttpUtility.HtmlEncode(tb_email.Text.ToString().Trim());
            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBHash(email);
            string dbSalt = getDBSalt(email);


            getLockoutDateTime(email);
            getLoginAttempts(email);

            TimeSpan timeDiff = DateTime.Now.Subtract(lockoutDateTime);
            Int32 minLocked = Convert.ToInt32(timeDiff.TotalMinutes);
            Int32 minLeft = 20 - minLocked;


            try
            {
                string pwdAndSalt = pwd + dbSalt;
                byte[] hashAndSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdAndSalt));
                string userHash = Convert.ToBase64String(hashAndSalt);

                DataSet dataSet = new DataSet();

                SqlConnection connection = new SqlConnection(MYDBConnectionString);
                string sql = "Select * from Accounts Where [Email]='"+ email + "'";

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
                            if (userHash == dbHash && minLeft <= 0)
                            {
                                Session["LoggedIn"] = tb_email.Text.Trim();

                                string guid = Guid.NewGuid().ToString();
                                Session["AuthToken"] = guid;

                                Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                                Random random = new Random();
                                rndNumber = random.Next(000000, 999999).ToString();
                                createOTP(email, rndNumber);

                                SendVCode(rndNumber);

                                updateLoginAttempts(tb_email.Text, 0);
                                setLockoutDateTime(tb_email.Text, DateTime.Parse("2003 - 04 - 11 00:00:00"));


                                Response.Redirect("Verify.aspx", false);
                            }
                            else
                            {
                                loginAttempts += 1;
                                updateLoginAttempts(tb_email.Text.Trim(), loginAttempts);

                                if (minLeft > 0)
                                {
                                    lblMessage.Text = "You are still locked out, try again later in " + minLeft.ToString() + " minutes";

                                }
                                else
                                {
                                    switch (loginAttempts)
                                    {
                                        case 1:
                                            lblMessage.Text = "Invalid login details. 2 login attempts remaining.";
                                            break;
                                        case 2:
                                            lblMessage.Text = "Invalid login details. 1 login attempt remaining.";
                                            break;
                                        case 3:
                                            setLockoutDateTime(tb_email.Text.Trim(), DateTime.Now);
                                            updateLoginAttempts(tb_email.Text, 0);
                                            lblMessage.Text = "Too many attempts. You have been locked out, try again later in " + minLeft.ToString() + " minutes";
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
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
                Subject = "2FA Login",
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

        protected void updateLoginAttempts(string email, int loginAttempts)
        {
            using(SqlConnection con = new SqlConnection(MYDBConnectionString))
            {
                string sql = "update [Accounts] set [LoginAttempts] = @LoginAttempts where [Email] = @EMAIL";
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@LoginAttempts", loginAttempts);
                    cmd.Parameters.AddWithValue("@EMAIL", email);

                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }

        protected void setLockoutDateTime(string email, DateTime lockoutTime)
        {
            using (SqlConnection con = new SqlConnection(MYDBConnectionString))
            {
                string sql = "update [Accounts] set [LockoutDateTime] = @LockoutDateTime where [Email] = @EMAIL";
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    using(SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@LockoutDateTime", lockoutTime);
                        cmd.Parameters.AddWithValue("@EMAIL", email);

                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
        }

        protected void getLockoutDateTime(string email)
        {
            SqlConnection con = new SqlConnection(MYDBConnectionString);
            string sql = "select [LockoutDateTime] from [Accounts] where [Email] = @EMAIL";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@EMAIL", email);
            try
            {
                con.Open();
                using(SqlDataReader sqlDataReader = cmd.ExecuteReader())
                {
                    while(sqlDataReader.Read())
                    {
                        if(sqlDataReader["LockoutDateTime"] != DBNull.Value)
                        {
                            lockoutDateTime = (DateTime)sqlDataReader["LockoutDateTime"];
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
                con.Close();
            }
        }

        protected string getLoginAttempts(string email)
        {
            string h = null;
            SqlConnection con = new SqlConnection(MYDBConnectionString);
            string sql = "select LoginAttempts from Accounts where Email = @EMAIL";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@EMAIL", email);
            try
            {
                con.Open();
                using(SqlDataReader sqlDataReader = cmd.ExecuteReader())
                {
                    while (sqlDataReader.Read())
                    {
                        if(sqlDataReader["LoginAttempts"] != DBNull.Value)
                        {
                            loginAttempts = (int)sqlDataReader["LoginAttempts"];

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { con.Close(); }
            return h;
        }
    }
}