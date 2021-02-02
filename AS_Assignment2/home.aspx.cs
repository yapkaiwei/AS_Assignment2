using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AS_Assignment2
{
    public partial class home : System.Web.UI.Page
    {

        string SITConnectConnection = System.Configuration.ConfigurationManager.ConnectionStrings["SITConnectConnection"].ConnectionString;
        string email = null;
        string firstName = null;
        string lastName = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["email"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {

                if(!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("login.aspx", false);
                }
                else
                {
                    if (Session["exceededMaxPasswordAge"] != null)
                    {
                        if (Session["exceededMaxPasswordAge"].ToString() == "exceeded")
                        {
                            Response.Redirect("~/changePassword.aspx?exceededMaxPasswordAge=exceeded");
                        }
                    }
                    

                    lbMsg.Text = "Successfully logged in";
                    PanelMsg.Visible = true;
                    div_loggedIn.Visible = true;
                    div_notLoggedIn.Visible = false;

                    email = (string)Session["email"];
                    displayUserInfo(email);

                    div_userInfo.Visible = true;
                }

            }
            else
            {
                div_loggedIn.Visible = false;
                div_notLoggedIn.Visible = true;
                div_userInfo.Visible = false;
                lbMsg.Text = "";
                PanelMsg.Visible = false;

                if(Request.QueryString["loggedOut"] == "true")
                {
                    lbMsg.Text = "Successfully logged out!";
                    PanelMsg.Visible = true;
                }
            }
        }

        protected void displayUserInfo(string email)
        {
            SqlConnection connection = new SqlConnection(SITConnectConnection);
            string sql = "SELECT * FROM [User] WHERE Email=@email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["email"] != DBNull.Value)
                        {
                            lbl_email.Text = HttpUtility.HtmlEncode(reader["email"].ToString());
                        }
                        if (reader["firstName"] != DBNull.Value)
                        {
                            firstName = reader["firstName"].ToString();
                        }
                        if (reader["lastName"] != DBNull.Value)
                        {
                            lastName = reader["lastName"].ToString();
                        }
                        if (reader["dateOfBirth"] != DBNull.Value)
                        {
                            DateTime birthday = DateTime.Parse(reader["dateOfBirth"].ToString());
                            lbl_dob.Text = HttpUtility.HtmlEncode(birthday.ToString("dd/MM/yyyy"));
                        }

                        lbl_name.Text = HttpUtility.HtmlEncode(firstName + " " + lastName);

                    }
                }
            }//try
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
        }

        protected void btn_Logout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            Response.Redirect("home.aspx?loggedOut=true", false);

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