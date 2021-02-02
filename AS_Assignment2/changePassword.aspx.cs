using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AS_Assignment2
{
    public partial class changePassword : System.Web.UI.Page
    {
        string SITConnectConnection = System.Configuration.ConfigurationManager.ConnectionStrings["SITConnectConnection"].ConnectionString;
        static string finalHash;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["email"] == null)
            {
                throw new HttpException(403, "Click on continue to see it on the webpage!");
            }

            tb_email.Text = Session["email"].ToString();

            if (Request.QueryString["exceededMaxPasswordAge"] == "exceeded")
            {
                lbMsg.Text = "Password has expired, please change your password!";
                PanelMsg.Visible = true;
            }
        }

        protected void btn_changePassword_Click(object sender, EventArgs e)
        {
            string passRequirementMsg = checkPassword(tb_password.Text);
            lbl_pwdchecker.Text = "";

            if (passRequirementMsg == "")
            {
                string password = tb_password.Text.ToString().Trim();
                string email = Session["email"].ToString();

                // Old password hash values
                string currentStoredHash = getDBCurrentHash(email);
                string oldPassHash1 = getDBOldHash1(email);
                string oldPassHash2 = getDBOldHash2(email);
                DateTime lastPassSet = getDBLastPassSet(email);
                DateTime invalidLastPassSet = Convert.ToDateTime("1/1/2001");

                // Get db salt value for this account
                string dbSalt = getDBSalt(email);

                SHA512Managed hashing = new SHA512Managed();

                string pwdWithSalt = password + dbSalt;
                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

                finalHash = Convert.ToBase64String(hashWithSalt);

                // Minimum password age ( cannot change password within 5mins from last change of password )
                if (DateTime.Now > lastPassSet.AddMinutes(5) && lastPassSet != invalidLastPassSet)
                {
                    // Check if finalHash matches past 3 passwords
                    if (finalHash.Equals(currentStoredHash) || finalHash.Equals(oldPassHash1) || finalHash.Equals(oldPassHash2))
                    {
                        lbl_pwdchecker.Text = "You cannot reuse any of your past 3 password!";
                    }
                    else
                    {
                        // Update new password, and move old password --> exPassHash1 --> exPassHash2
                        SqlConnection connection = new SqlConnection(SITConnectConnection);
                        string sql = "UPDATE [User] SET " +
                            "exPassHash2 = exPassHash1," +
                            "exPassHash1 = passwordHash," +
                            "passwordHash = @paraPassHash," +
                            "lastPassSet = @paraLastPassSet " +
                            "WHERE Email=@email";
                        SqlCommand command = new SqlCommand(sql, connection);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@paraPassHash", finalHash);
                        command.Parameters.AddWithValue("@paraLastPassSet", DateTime.Now);

                        command.Connection = connection;
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();

                        lbMsg.Text = "Password changed successfully";
                        PanelMsg.Visible = true;


                        Session.Clear();
                        Session.Abandon();
                        Session.RemoveAll();
                        Response.Redirect("login.aspx?passwordChanged=true", false);

                        if (Request.Cookies["ASP.NET_SessionId"] != null)
                        {
                            Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                            Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
                        }

                        if (Request.Cookies["AuthToken"] != null)
                        {
                            Response.Cookies["AuthToken"].Value = string.Empty;
                            Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
                        }
                    }
                }
                else
                {
                    lbl_pwdchecker.Text = "You cannot change your password so frequently!";
                }




            }
            else
            {
                lbl_pwdchecker.Text = passRequirementMsg;
            }
        }

        private string checkPassword(string password)
        {

            string passRequirementMsg = "";

            if (password.Length < 8)
            {
                passRequirementMsg += "Password Length Must be at Least 8 characters" + "<br/>";
            }

            if (!Regex.IsMatch(password, "[a-z]"))
            {
                passRequirementMsg += "Password require at least 1 lower case letter" + "<br/>";
            }
            if (!Regex.IsMatch(password, "[A-Z]"))
            {
                passRequirementMsg += "Password require at least 1 upper case letter" + "<br/>";
            }
            if (!Regex.IsMatch(password, "[0-9]"))
            {
                passRequirementMsg += "Password require at least 1 number" + "<br/>";
            }
            if (!Regex.IsMatch(password, "[^A-Za-z0-9]"))
            {
                passRequirementMsg += "Password require at least 1 special character" + "<br/>";
            }
            if (!password.Equals(tb_confirmPassword.Text.ToString()))
            {
                passRequirementMsg += "Password does not match with confirm password" + "<br/>";
            }

            return passRequirementMsg;
        }

        protected string getDBCurrentHash(string email)
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

        protected string getDBOldHash1(string email)
        {

            string h = null;

            SqlConnection connection = new SqlConnection(SITConnectConnection);
            string sql = "Select exPassHash1 FROM [User] WHERE Email=@email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["exPassHash1"] != null)
                        {
                            if (reader["exPassHash1"] != DBNull.Value)
                            {
                                h = reader["exPassHash1"].ToString();
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

        protected string getDBOldHash2(string email)
        {

            string h = null;

            SqlConnection connection = new SqlConnection(SITConnectConnection);
            string sql = "Select exPassHash2 FROM [User] WHERE Email=@email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["exPassHash2"] != null)
                        {
                            if (reader["exPassHash2"] != DBNull.Value)
                            {
                                h = reader["exPassHash2"].ToString();
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

        protected DateTime getDBLastPassSet(string email)
        {

            DateTime lastPassSet = Convert.ToDateTime("1/1/2001");

            SqlConnection connection = new SqlConnection(SITConnectConnection);
            string sql = "Select lastPassSet FROM [User] WHERE Email=@email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["lastPassSet"] != null)
                        {
                            if (reader["lastPassSet"] != DBNull.Value)
                            {
                                string str = reader["lastPassSet"].ToString();
                                lastPassSet = DateTime.Parse(str);
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
            return lastPassSet;
        }

        public void logout_click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            Response.Redirect("~/home.aspx?loggedOut=true", false);

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }

            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies["AuthToken"].Value = string.Empty;
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }
        }

    }
}