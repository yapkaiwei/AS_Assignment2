<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="AS_Assignment2.Registration" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Register</title>
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" integrity="sha384-JcKb8q3iqJ61gNV9KGb8thSsNjpSL0n8PARn9HuZOnIxN0hoP+VmmDGMN5t9UJ0Z" crossorigin="anonymous">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js" integrity="sha384-B4gt1jrGC7Jh4AgTPSdUtOBvfO8shuf57BaghqFfPlYxofvL8/KUEfYiJOMMV+rV" crossorigin="anonymous"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.bundle.min.js" integrity="sha384-LtrjvnR4Twt/qOuYxE721u19sVFLVSA4hf/rRt6PrZTmiPltdZcI7q7PXQBYTKyf" crossorigin="anonymous"></script>
    <script src="https://www.google.com/recaptcha/api.js?render=6Ld7TDsaAAAAAAc-E7jl5YA5ha5HjrudgqO7RoVq"></script>

    <script type="text/javascript">
        function validate() {
            var str = document.getElementById('<%=tb_password.ClientID %>').value;

            if (str.length < 8) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password Length Must be at Least 8 characters";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("too_short");
            }

            else if (str.search(/[0-9]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password require at least 1 number";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_number");
            }

            else if (str.search(/[A-Z]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password require at least 1 upper case letter";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_uppercase");
            }

            else if (str.search(/[a-z]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password require at least 1 lower case letter";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_lowercase");
            }

            else if (str.search(/[^A-Za-z0-9]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password require at least 1 special character";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_specialchar");
            }

            document.getElementById("lbl_pwdchecker").innerHTML = "Excellent!";
            document.getElementById("lbl_pwdchecker").style.color = "Blue";
        }

        function checkPasswordMatch() {
            var str1 = document.getElementById('<%=tb_password.ClientID %>').value;
            var str2 = document.getElementById('<%=tb_confirmPassword.ClientID %>').value;

            if (str1 != str2) {
                document.getElementById("lbl_pwdMatch").innerHTML = "Password does not match!";
                document.getElementById("lbl_pwdMatch").style.color = "Red";
                return ("no_match");
            }
            else {
                document.getElementById("lbl_pwdMatch").innerHTML = "Password matches!";
                document.getElementById("lbl_pwdMatch").style.color = "Blue";
                return ("match");
            }
        }

        function getCreditCardType(number) {

            // Credit card is unknown first
            var result = "unknown";

            // Check if credit card is mastercard
            if (/^5/.test(number)) {
                result = "mastercard";
            }

            // If not master, is it visa?
            else if (/^4/.test(number)) {
                result = "visa";
            }

            return result;
        }

        function swapCardIcon(event) {
            var value = event.target.value,
                type = getCreditCardType(value);

            switch (type) {
                case "mastercard":
                    document.getElementById("img_cardIconVisa").style.opacity = "0.4";
                    document.getElementById("img_cardIconMaster").style.opacity = "1";
                    break;

                case "visa":
                    document.getElementById("img_cardIconMaster").style.opacity = "0.4";
                    document.getElementById("img_cardIconVisa").style.opacity = "1";
                    break;

                default:
                    document.getElementById("img_cardIconMaster").style.opacity = "1";
                    document.getElementById("img_cardIconVisa").style.opacity = "1";
                    break;
            }
        }

        function checkEmail() {

            var emailStr = document.getElementById('<%=tb_email.ClientID %>').value;

            if (emailStr.search(/^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$/) == -1) {
                document.getElementById("lbl_emailMsg").innerHTML = "Invalid email address";
                return ("Wrong_email_format");
            }

            document.getElementById("lbl_emailMsg").innerHTML = "Valid email!";
            document.getElementById("lbl_emailMsg").style.color = "Blue";
        }


        document.addEventListener("DOMContentLoaded", function () {
            var ccNumTB = document.getElementById("tb_ccNumber");
            ccNumTB.addEventListener("keyup", swapCardIcon, false);
            ccNumTB.addEventListener("blur", swapCardIcon, false);
        }, false);
    </script>

</head>
<body>


    <form id="form1" runat="server" autocomplete="off">
        <nav class="navbar navbar-expand-lg navbar-dark bg-dark">
            <a class="navbar-brand" runat="server" href="~/home.aspx">SITConnect</a>
            <div class="collapse navbar-collapse" id="navbarText">
                <ul class="navbar-nav mr-auto">
                    <li class="nav-item">
                        <a class="nav-link" runat="server" href="~/home.aspx">Home</a>
                    </li>
                </ul>

                <ul class="nav navbar-nav ">
                    <li><a runat="server" class="nav-link" href="~/login.aspx">Login</a></li>
                    <li><a runat="server" class="nav-link active" href="~/register.aspx">Register</a></li>
                </ul>
            </div>
        </nav>

        <div class="content-header">
            <div class="container">
                <h2>Registration</h2>
            </div>
        </div>
        <div class="container">

            <div class="form-row">
                <div class="col-3">
                    <label for="tb_fName">First Name</label>
                    <asp:TextBox runat="server" ID="tb_fName" class="form-control" placeholder="Enter first name" required="true"></asp:TextBox>
                </div>
                <div class="col-3">
                    <label for="tb_lName">Last Name: </label>
                    <asp:TextBox runat="server" ID="tb_lName" class="form-control" placeholder="Enter last name" required="true"></asp:TextBox>
                </div>
            </div>

            <div class="form-row">
                <div class="col-3">
                    <asp:Label ID="lbl_fNameMsg" runat="server" ForeColor="Red" Text=""></asp:Label>
                </div>

                <div class="col-3">
                    <asp:Label ID="lbl_lNameMsg" runat="server" ForeColor="Red" Text=""></asp:Label>
                </div>
            </div>
           
            <hr />

            <div class="form-row">
                <div class="col-3">
                    <label for="tb_email">Email</label>
                    <asp:TextBox runat="server" ID="tb_email" class="form-control" placeholder="Enter email address" required="true" TextMode="Email" onkeyup="javascript:checkEmail();" Pattern="^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$"></asp:TextBox>
                </div>

                <div class="col-3">
                    <label for="tb_dob">Date of Birth</label>
                    <asp:TextBox runat="server" ID="tb_dob" class="form-control" placeholder="DD / MM / YYYY" required="true" TextMode="Date"></asp:TextBox>
                </div>
            </div>

            <div class="form-row">
                <div class="col-5">
                    <asp:Label ID="lbl_emailMsg" runat="server" ForeColor="Red"></asp:Label>
                </div>
            </div>

            <hr />

            <div class="form-row">
                <div class="col-3">
                    <label for="tb_password">Password</label>
                    <asp:TextBox runat="server" ID="tb_password" class="form-control" placeholder="Enter password" onkeyup="javascript:validate(); javascript:checkPasswordMatch()" TextMode="Password" required="true"></asp:TextBox>                
                </div>  
            </div>

            <div class="form-row">
                <div class="col-5">
                    <asp:Label ID="lbl_pwdchecker" runat="server" Text=""></asp:Label>
                </div>
            </div>

            <div class="form-row">
                <div class="col-3">
                    <label for="tb_confirmPassword">Confirm Password</label>
                    <asp:TextBox runat="server" ID="tb_confirmPassword" class="form-control" TextMode="Password" onkeyup="javascript:checkPasswordMatch()" placeholder="Enter password again" required="true"></asp:TextBox>
                </div>
            </div>

            <div class="form-row">
                <div class="col-5">
                    <asp:Label ID="lbl_pwdMatch" runat="server" Text=""></asp:Label>
                </div>
            </div>

            <hr />

            <div class="form-row">
                <div class="col-4">
                    <label for="tb_ccNumber">Card Number</label>
                    <asp:TextBox runat="server" ID="tb_ccNumber" class="form-control" placeholder="Enter credit card number (No spaces)" required="true" MaxLength="16"></asp:TextBox>
                </div>

                <div class="col-2">
                    <div class="row">
                        <div class="column">
                            <img src="/images/mastercard.png" id="img_cardIconMaster" style="height:35px;margin-top:33px;margin-left:10px;"/>
                        </div>
                        <div class="column">
                            <img src="/images/visa.png" id="img_cardIconVisa" style="height:35px;margin-top:33px;margin-left:5px"/>
                        </div>
                    </div>
                </div>
            </div>

            <div class="form-row">
                <div class="col">
                    <asp:Label ID="lbl_ccNumMsg" runat="server" ForeColor="Red" Text=""></asp:Label>
                </div>
            </div>

            <div class="form-row">
                <div class="col-2">
                    <label for="tb_ccExpiry">Expiry Date</label>
                    <asp:TextBox runat="server" ID="tb_ccExpiry" class="form-control" placeholder="mm/yy" required="true" MaxLength="5"></asp:TextBox>
                </div>

                <div class="col-2">
                    <label for="tb_ccCVV">CVV Code</label>
                    <asp:TextBox runat="server" ID="tb_ccCVV" class="form-control" placeholder="i.e. 031" TextMode="Number" MaxLength="3" required="true"></asp:TextBox>
                </div>
            </div>

            <div class="form-row">
                <div class="col-2">
                    <asp:Label ID="lbl_ccExpiryMsg" runat="server" ForeColor="Red" Text=""></asp:Label>
                </div>
                <div class="col-2">
                    <asp:Label ID="lbl_ccCVVMsg" runat="server" ForeColor="Red" Text=""></asp:Label>
                </div>
            </div>

            <div class="mt-3">
                <asp:Button ID="btn_register" runat="server" class="btn btn-primary" Text="Register" OnClick="btn_register_Click" />
            </div>
            



            <br />
            <asp:Label ID="lbl_msg" runat="server" Text="" ForeColor="Red"></asp:Label>

            <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>

        </div>
    </form>

    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('6Ld7TDsaAAAAAAc-E7jl5YA5ha5HjrudgqO7RoVq', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });
    </script>

</body>
</html>
