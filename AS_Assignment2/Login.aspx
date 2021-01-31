<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="AS_Assignment2.Login" ValidateRequest="false"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login</title>

    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" integrity="sha384-JcKb8q3iqJ61gNV9KGb8thSsNjpSL0n8PARn9HuZOnIxN0hoP+VmmDGMN5t9UJ0Z" crossorigin="anonymous">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js" integrity="sha384-B4gt1jrGC7Jh4AgTPSdUtOBvfO8shuf57BaghqFfPlYxofvL8/KUEfYiJOMMV+rV" crossorigin="anonymous"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.bundle.min.js" integrity="sha384-LtrjvnR4Twt/qOuYxE721u19sVFLVSA4hf/rRt6PrZTmiPltdZcI7q7PXQBYTKyf" crossorigin="anonymous"></script>
    <script src="https://www.google.com/recaptcha/api.js?render=6Ld7TDsaAAAAAAc-E7jl5YA5ha5HjrudgqO7RoVq"></script>
</head>
<body>
    <form id="form1" runat="server">
        <nav class="navbar navbar-expand-lg navbar-dark bg-dark">
            <a class="navbar-brand" runat="server" href="~/home.aspx">SITConnect</a>
            <div class="collapse navbar-collapse" id="navbarText">
                <ul class="navbar-nav mr-auto">
                    <li class="nav-item">
                        <a class="nav-link" runat="server" href="~/home.aspx">Home</a>
                    </li>
                </ul>

                <ul class="nav navbar-nav ">
                    <li><a runat="server" class="nav-link active" href="~/login.aspx">Login</a></li>
                    <li><a runat="server" class="nav-link " href="~/register.aspx">Register</a></li>
                </ul>
            </div>
        </nav>
        <div class="mt-3">
            <div class="content-header">
                <div class="container">
                    <h2 class="d-flex justify-content-center">Login</h2>
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

                <div class="form-group row d-flex justify-content-center">
                    <div class="col-4 ">
                        <label for="tb_email" class="col-form-label">Email</label>
                        <asp:TextBox runat="server" class="form-control" ID="tb_email" placeholder="Enter email address" required="true"></asp:TextBox>
                    </div>
                </div>

                <div class="form-group row d-flex justify-content-center">
                    <div class="col-4">
                        <label for="tb_password">Password</label>
                        <asp:TextBox runat="server" class="form-control" ID="tb_password" placeholder="Enter password" TextMode="Password" required="true"></asp:TextBox>

                        <asp:Button ID="btn_login" runat="server" class=" my-3 btn btn-primary" Text="Login" OnClick="btn_login_Click" />
                        <br />
                        <asp:Label ID="lbl_msg" runat="server" Text="" ForeColor="Red"></asp:Label>
                    </div>

                </div>

                <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
            </div>

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
