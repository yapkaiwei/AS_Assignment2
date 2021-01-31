<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="home.aspx.cs" Inherits="AS_Assignment2.home" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SITConnect</title>

    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" integrity="sha384-JcKb8q3iqJ61gNV9KGb8thSsNjpSL0n8PARn9HuZOnIxN0hoP+VmmDGMN5t9UJ0Z" crossorigin="anonymous"/>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js" integrity="sha384-B4gt1jrGC7Jh4AgTPSdUtOBvfO8shuf57BaghqFfPlYxofvL8/KUEfYiJOMMV+rV" crossorigin="anonymous"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.bundle.min.js" integrity="sha384-LtrjvnR4Twt/qOuYxE721u19sVFLVSA4hf/rRt6PrZTmiPltdZcI7q7PXQBYTKyf" crossorigin="anonymous"></script>
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

                <div id="div_notLoggedIn" visible="false" runat="server">
                    <ul class="nav navbar-nav ">
                        <li><a runat="server" class="nav-link" href="~/login.aspx">Login</a></li>
                        <li><a runat="server" class="nav-link" href="~/register.aspx">Register</a></li>
                    </ul>
                </div>

                <div id="div_loggedIn" visible="false" runat="server">
                    <ul class="nav navbar-nav ">
                        <li><asp:Button runat="server" style="background-color:#343A40;border:0px;" class="nav-link" id="btn_Logout" OnClick="btn_Logout_Click" Text="Logout"></asp:Button></li>
                    </ul>
                </div>

            </div>
        </nav>

        <div class="container">
            <asp:Panel ID="PanelMsg" Visible="false" runat="server" CssClass="mt-3 mx-5 alert alert-dismissable alert-success">
                <button type="button" class="close" data-dismiss="alert">
                    <span aria-hidden="true">&times;</span>
                </button>
                <asp:Label ID="lbMsg" CssClass="d-flex justify-content-center" runat="server"></asp:Label>
            </asp:Panel>

            <div class="d-flex justify-content-center my-5">
                <asp:Image id="img_main" runat="server"  AlternateText="SITConnect Banner" ImageUrl="~/images/SITConnect.png" />
            </div>

        </div>

        <div class="container" id="div_userInfo" visible="false" runat="server">
            <h2>Welcome, <asp:Label ID="lbl_name" runat="server"></asp:Label></h2>
            <h4>Here are your details :</h4>
            <p>Email: <asp:Label ID="lbl_email" runat="server"></asp:Label></p>
            <p>Date of Birth: <asp:Label ID="lbl_dob" runat="server"></asp:Label></p>
        </div>
        

        

    </form>
</body>
</html>
