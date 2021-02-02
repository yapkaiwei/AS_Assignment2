<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="changePassword.aspx.cs" Inherits="AS_Assignment2.changePassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Change Password</title>

    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" integrity="sha384-JcKb8q3iqJ61gNV9KGb8thSsNjpSL0n8PARn9HuZOnIxN0hoP+VmmDGMN5t9UJ0Z" crossorigin="anonymous">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js" integrity="sha384-B4gt1jrGC7Jh4AgTPSdUtOBvfO8shuf57BaghqFfPlYxofvL8/KUEfYiJOMMV+rV" crossorigin="anonymous"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.bundle.min.js" integrity="sha384-LtrjvnR4Twt/qOuYxE721u19sVFLVSA4hf/rRt6PrZTmiPltdZcI7q7PXQBYTKyf" crossorigin="anonymous"></script>

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
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <nav class="navbar navbar-expand-lg navbar-dark bg-dark">
            <a class="navbar-brand" runat="server" href="~/home.aspx">SITConnect</a>
            <div class="collapse navbar-collapse" id="navbarText">
                <ul class="navbar-nav mr-auto">
                    <li class="nav-item">
                        <a class="nav-link active" runat="server" href="~/home.aspx">Home</a>
                    </li>
                </ul>

                <ul class="nav navbar-nav ">
                    <li><a runat="server" class="nav-link" onserverclick="logout_click">Logout</a></li>
                </ul>

            </div>
        </nav>
        <div class="mt-3">
            <div class="content-header">
                <div class="container">
                    <h2 class="d-flex justify-content-center">Change Password</h2>
                </div>
            </div>

            <div class="container">
                <asp:Panel ID="PanelMsg" Visible="false" runat="server" CssClass="mt-3 mx-5 alert alert-dismissable alert-success">
                    <button type="button" class="close" data-dismiss="alert">
                        <span aria-hidden="true">&times;</span>
                    </button>
                    <asp:Label ID="lbMsg" CssClass="d-flex justify-content-center" runat="server"></asp:Label>
                </asp:Panel>
            </div>

            <div class="container">

                <div class="form-row d-flex justify-content-center">
                    <div class="col-4 ">
                        <label for="tb_email" class="col-form-label">Email</label>
                        <asp:TextBox runat="server" class="form-control" ID="tb_email" placeholder="Enter email address" Disabled="true"></asp:TextBox>
                        <hr />
                    </div>
                </div>


                <div class="form-row d-flex justify-content-center">
                    <div class="col-4">
                        <label for="tb_password">Password</label>
                        <asp:TextBox runat="server" ID="tb_password" class="form-control" placeholder="Enter password" onkeyup="javascript:validate(); javascript:checkPasswordMatch()" TextMode="Password" ></asp:TextBox>                
                    </div>  
                </div>

                <div class="form-row d-flex justify-content-center">
                    <div class="col-4">
                        <asp:Label ID="lbl_pwdchecker" runat="server" ForeColor="Red" Text=""></asp:Label>
                    </div>
                </div>

                <div class="form-row d-flex justify-content-center">
                    <div class="col-4">
                        <label for="tb_confirmPassword">Confirm Password</label>
                        <asp:TextBox runat="server" ID="tb_confirmPassword" class="form-control" TextMode="Password" onkeyup="javascript:checkPasswordMatch()" placeholder="Enter password again"></asp:TextBox>
                    </div>
                </div>

                <div class="form-row d-flex justify-content-center">
                    <div class="col-4">
                        <asp:Label ID="lbl_pwdMatch" runat="server" Text=""></asp:Label> <br />
                        <asp:Button ID="btn_changePassword" runat="server" class=" my-3 btn btn-primary" Text="Change Password" OnClick="btn_changePassword_Click"  />
                    </div>
                </div>

            </div>

        </div>
    </form>
</body>
</html>
