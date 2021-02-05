using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

namespace AS_Assignment2
{
    public partial class Registration : System.Web.UI.Page
    {
        string SITConnectConnection = System.Configuration.ConfigurationManager.ConnectionStrings["SITConnectConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] ccNumberKey;
        byte[] ccNumberIV;
        byte[] ccExpiryKey;
        byte[] ccExpiryIV;
        byte[] ccCVVKey;
        byte[] ccCVVIV;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["email"] != null)
            {
                throw new HttpException(403, "Click on continue to see it on the webpage!");
            }

            tb_dob.Attributes["max"] = DateTime.Now.AddYears(-10).ToString("yyyy-MM-dd");
            tb_dob.Attributes["min"] = DateTime.Now.AddYears(-100).ToString("yyyy-MM-dd");

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

        protected bool emailExist(string email)
        {
            SqlConnection connection = new SqlConnection(SITConnectConnection);
            string sql = "SELECT email FROM [User] WHERE Email=@email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }


        }

        protected bool validate()
        {
            bool validated = true;
            lbl_emailMsg.Text = "";
            lbl_fNameMsg.Text = "";
            lbl_lNameMsg.Text = "";
            lbl_msg.Text = "";
            lbl_ccNumMsg.Text = "";
            lbl_ccExpiryMsg.Text = "";
            lbl_ccCVVMsg.Text = "";

            string passRequirementMsg = checkPassword(tb_password.Text);

            if (passRequirementMsg == "")
            {
                validated = true;
            }
            else
            {
                lbl_msg.Text = passRequirementMsg;
                validated = false;
            }

            // Check if email exists first
            if (emailExist(tb_email.Text.ToString()))
            {
                lbl_emailMsg.Text = "Email already exists!";
                validated = false;
            }


            // Validate first name and last name, only allow 1 to 50 characters. What are the chances that there's someone whose first name is more than 50 letters?
            if (!Regex.IsMatch(tb_fName.Text, "^[A-Za-z\u0020]{1,50}$"))
            {
                lbl_fNameMsg.Text = "First name is invalid or too long";
                validated = false;
            }

            if (!Regex.IsMatch(tb_lName.Text, "^[A-Za-z\u0020]{1,50}$"))
            {
                lbl_lNameMsg.Text = "Last name is invalid or too long";
                validated = false;
            }

            // Validate email
            if (!Regex.IsMatch(tb_email.Text, @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$"))
            {
                lbl_emailMsg.Text = "Email format is wrong!";
                validated = false;
            }

            // Validate date just to make sure :D
            if (!DateTime.TryParse(tb_dob.Text, out DateTime dob))
            {
                lbl_msg.Text = "Date of Birth is invalid!";
                validated = false;
            }

            // Validate credit card number (Only 4 or 5 followed by 15 other number digits
            if (!Regex.IsMatch(tb_ccNumber.Text, "^[4,5][0-9]{15}$"))
            {
                lbl_ccNumMsg.Text = "Credit Card number is invalid! Only VISA and Mastercard are allowed";
                validated = false;
            }

            // Validate credit card expiry date
            if (!Regex.IsMatch(tb_ccExpiry.Text, @"^(0[1-9]|1[0-2])\/(2[1-9])$"))
            {
                lbl_ccExpiryMsg.Text = "Invalid expiry date or Card has expired!";
                validated = false;
            }

            // Validate credit card CVV
            if (!Regex.IsMatch(tb_ccCVV.Text, "^[0-9]{3}$"))
            {
                lbl_ccCVVMsg.Text = "CVV is invalid!";
                validated = false;
            }


            return validated;



        }

        protected void btn_register_Click(object sender, EventArgs e)
        {
            bool validated = validate();

            if(validated && ValidateCaptcha())
            {
                string password = tb_password.Text.ToString().Trim();

                //Generate random "salt"
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] saltByte = new byte[8];

                //Fills array of bytes with a cryptographically strong sequence of random values.
                rng.GetBytes(saltByte);
                salt = Convert.ToBase64String(saltByte);

                SHA512Managed hashing = new SHA512Managed();

                string pwdWithSalt = password + salt;
                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

                finalHash = Convert.ToBase64String(hashWithSalt);

                RijndaelManaged ccNumberCipher = new RijndaelManaged();
                ccNumberCipher.GenerateKey();
                ccNumberKey = ccNumberCipher.Key;
                ccNumberIV = ccNumberCipher.IV;

                RijndaelManaged ccExpiryCipher = new RijndaelManaged();
                ccExpiryCipher.GenerateKey();
                ccExpiryKey = ccExpiryCipher.Key;
                ccExpiryIV = ccExpiryCipher.IV;

                RijndaelManaged ccCVVCipher = new RijndaelManaged();
                ccCVVCipher.GenerateKey();
                ccCVVKey = ccCVVCipher.Key;
                ccCVVIV = ccCVVCipher.IV;

                createAccount();
                Response.Redirect("Login.aspx?registered=true", true);
            }
            

        }

        public void createAccount()
        {
            try
            {

                DateTime dob = Convert.ToDateTime(tb_dob.Text);
                DateTime unlockTime = Convert.ToDateTime("1/1/2001");

                using (SqlConnection con = new SqlConnection(SITConnectConnection))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO [User] VALUES(@paraEmail,@paraFirstName,@paraLastName,@paraDoB,@paraPassHash," +
                        "@paraPassSalt,@paraCCNumber,@paraCCExpiry,@paraCCCVV,@paraCCNumberIV," +
                        "@paraCCNumberKey,@paraCCExpiryIV,@paraCCExpiryKey,@paraCCCVVIV,@paraCCCVVKey, @paraLoginAttempt," +
                        "@paraUnlockTime, null, null, @paraLastPassSet)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@paraEmail", tb_email.Text.Trim());
                            cmd.Parameters.AddWithValue("@paraFirstName", tb_fName.Text);
                            cmd.Parameters.AddWithValue("@paraLastName", tb_lName.Text);
                            cmd.Parameters.AddWithValue("@paraDoB", dob);
                            cmd.Parameters.AddWithValue("@paraPassHash", finalHash);
                            cmd.Parameters.AddWithValue("@paraPassSalt", salt);
                            cmd.Parameters.AddWithValue("@paraCCNumber", Convert.ToBase64String(encryptCCNumber(tb_ccNumber.Text.Trim())));
                            cmd.Parameters.AddWithValue("@paraCCExpiry", Convert.ToBase64String(encryptCCExpiry(tb_ccExpiry.Text.Trim())));
                            cmd.Parameters.AddWithValue("@paraCCCVV", Convert.ToBase64String(encryptCCCVV(tb_ccCVV.Text.Trim())));
                            cmd.Parameters.AddWithValue("@paraCCNumberIV", Convert.ToBase64String(ccNumberIV));
                            cmd.Parameters.AddWithValue("@paraCCNumberKey", Convert.ToBase64String(ccNumberKey));
                            cmd.Parameters.AddWithValue("@paraCCExpiryIV", Convert.ToBase64String(ccExpiryIV));
                            cmd.Parameters.AddWithValue("@paraCCExpiryKey", Convert.ToBase64String(ccExpiryKey));
                            cmd.Parameters.AddWithValue("@paraCCCVVIV", Convert.ToBase64String(ccCVVIV));
                            cmd.Parameters.AddWithValue("@paraCCCVVKey", Convert.ToBase64String(ccCVVKey));
                            cmd.Parameters.AddWithValue("@paraLoginAttempt", 0);
                            cmd.Parameters.AddWithValue("@paraUnlockTime", unlockTime);
                            cmd.Parameters.AddWithValue("@paraLastPassSet", DateTime.Now);
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

        protected byte[] encryptCCNumber(string data)
        {
            byte[] cipherTextNumber = null;
            try
            {
                RijndaelManaged ccNumberCipher = new RijndaelManaged();
                ccNumberCipher.Key = ccNumberKey;
                ccNumberCipher.IV = ccNumberIV;

                ICryptoTransform encryptTransformNumber = ccNumberCipher.CreateEncryptor();
                byte[] plainTextNumber = Encoding.UTF8.GetBytes(data);
                cipherTextNumber = encryptTransformNumber.TransformFinalBlock(plainTextNumber, 0, plainTextNumber.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherTextNumber;
        }

        protected byte[] encryptCCExpiry(string data)
        {
            byte[] cipherTextExpiry = null;
            try
            {
                RijndaelManaged ccExpiryCipher = new RijndaelManaged();
                ccExpiryCipher.Key = ccExpiryKey;
                ccExpiryCipher.IV = ccExpiryIV;

                ICryptoTransform encryptTransformExpiry = ccExpiryCipher.CreateEncryptor();
                byte[] plainTextExpiry = Encoding.UTF8.GetBytes(data);
                cipherTextExpiry = encryptTransformExpiry.TransformFinalBlock(plainTextExpiry, 0, plainTextExpiry.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherTextExpiry;
        }

        protected byte[] encryptCCCVV(string data)
        {
            byte[] cipherTextCVV = null;
            try
            {
                RijndaelManaged ccCVVCipher = new RijndaelManaged();
                ccCVVCipher.Key = ccCVVKey;
                ccCVVCipher.IV = ccCVVIV;

                ICryptoTransform encryptTransformCVV = ccCVVCipher.CreateEncryptor();
                byte[] plainTextCVV = Encoding.UTF8.GetBytes(data);
                cipherTextCVV = encryptTransformCVV.TransformFinalBlock(plainTextCVV, 0, plainTextCVV.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherTextCVV;
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