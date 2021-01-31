using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AS_Assignment2
{
    public partial class Login : System.Web.UI.Page
    {

        string SITConnectConnection = System.Configuration.ConfigurationManager.ConnectionStrings["SITConnectConnection"].ConnectionString;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["registered"] == "true")
            {
                lbMsg.Text = "Successfully registered";
                PanelMsg.Visible = true;
            }
        }

        protected void btn_login_Click(object sender, EventArgs e)
        {
            if (ValidateCaptcha())
            {
                string password = tb_password.Text.ToString().Trim();
                string email = tb_email.Text.ToString().Trim();
                SHA512Managed hashing = new SHA512Managed();
                string dbHash = getDBHash(email);
                string dbSalt = getDBSalt(email);
                string errorMsg = "";
                try
                {
                    if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                    {
                        string passWithSalt = password + dbSalt;
                        byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(passWithSalt));
                        string userHash = Convert.ToBase64String(hashWithSalt);
                        int loginCount = getLoginAttempt(email);

                        if (loginCount != -1 && loginCount < 3)
                        {
                            if (userHash.Equals(dbHash))
                            {
                                Session["email"] = email;

                                // Creates a new GUID and stores it into session
                                string guid = Guid.NewGuid().ToString();
                                Session["AuthToken"] = guid;

                                // Create a new cookie with that same guid value
                                Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                                // Reset loginAttempt counter to 0 if successful login
                                SqlConnection connection = new SqlConnection(SITConnectConnection);
                                string sql = "UPDATE [User] SET loginAttempt = 0 WHERE Email=@email";
                                SqlCommand command = new SqlCommand(sql, connection);
                                command.Parameters.AddWithValue("@email", email);

                                command.Connection = connection;
                                connection.Open();
                                command.ExecuteNonQuery();
                                connection.Close();


                                Response.Redirect("home.aspx", false);
                            }
                            else
                            {
                                // If wrong password, increment loginAttempt by 1

                                SqlConnection connection = new SqlConnection(SITConnectConnection);
                                string sql = "UPDATE [User] SET loginAttempt = loginAttempt + 1 WHERE Email=@email";
                                SqlCommand command = new SqlCommand(sql, connection);
                                command.Parameters.AddWithValue("@email", email);

                                command.Connection = connection;
                                connection.Open();
                                command.ExecuteNonQuery();
                                connection.Close();

                                // If loginCount == 2 here because if it's wrong, updated loginAttempt will become 3. This part is just to show the correct error message if get
                                // wrong password at the 3rd time.
                                if (loginCount == 2)
                                {
                                    lbl_msg.Text = "You have exceeded the maximum login attempt, your account will now be locked out until "; //+ unlockTime;

                                    // Set automatic account unlock time at now + 5minutes
                                }
                                else
                                {
                                    lbl_msg.Text = "Email or password is not valid. Please try again.";
                                }
                            }
                        }
                        else
                        {
                            // Check if account lockout has expired yet, if expired, reset counter and attempt login again


                            // If it has already exceeded
                            lbl_msg.Text = "You have exceeded the maximum login attempt, your account will now be locked out until ";  //+ unlockTime;

                            // This part is just to continue incrementing it, by right don't need this for it to work, but useful to check for rainbow table / bruteforce attacks.
                            SqlConnection connection = new SqlConnection(SITConnectConnection);
                            string sql = "UPDATE [User] SET loginAttempt = loginAttempt + 1 WHERE Email=@email";
                            SqlCommand command = new SqlCommand(sql, connection);
                            command.Parameters.AddWithValue("@email", email);

                            command.Connection = connection;
                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                        }

                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }
                finally { }
            }
        }

        protected int getLoginAttempt(string email)
        {
            SqlConnection connection = new SqlConnection(SITConnectConnection);
            string sql = "SELECT loginAttempt FROM [User] WHERE Email=@email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);

            int count = -1;

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["loginAttempt"] != null)
                        {
                            if (reader["loginAttempt"] != DBNull.Value)
                            {
                                string countStr = reader["loginAttempt"].ToString();
                                count = int.Parse(countStr);
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
            return count;

        }

        protected string getDBHash(string email)
        {

            string h = null;

            SqlConnection connection = new SqlConnection(SITConnectConnection);
            string sql = "Select passwordHash FROM [User] WHERE Email=@email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["passwordHash"] != null)
                        {
                            if (reader["passwordHash"] != DBNull.Value)
                            {
                                h = reader["passwordHash"].ToString();
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

        protected string getDBSalt(string email)
        {
            string s = null;

            SqlConnection connection = new SqlConnection(SITConnectConnection);
            string sql = "select passwordSalt FROM [User] WHERE Email=@email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["passwordSalt"] != null)
                        {
                            if (reader["passwordSalt"] != DBNull.Value)
                            {
                                s = reader["passwordSalt"].ToString();
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

        public bool ValidateCaptcha()
        {
            bool result = true;

            // When user submits the recaptcha form, the user gets a response POST parameter.
            // captchaResponse consists of the user click pattern. Behaviouir analytics! AI :)
            string captchaResponse = Request.Form["g-recaptcha-response"];

            // To send a GET request to Google along with the response and secret key.
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
                ("https://www.google.com/recaptcha/api/siteverify?secret=6Ld7TDsaAAAAAIhK2f0hYIDTptzIi6_k0YlXJrUK &response=" + captchaResponse);

            try
            {
                // Codes to receive the Response in JSON format from Google Server
                using (WebResponse webRes = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(webRes.GetResponseStream()))
                    {
                        // The response in JSON format
                        string jsonResponse = readStream.ReadToEnd();

                        JavaScriptSerializer js = new JavaScriptSerializer();

                        // Create jsonObject to handle the response i.e. success or error
                        // Deserialize Json
                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);

                        // Convert the string
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