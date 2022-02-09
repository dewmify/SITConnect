<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HomePage.aspx.cs" Inherits="AppSecAsgn.HomePage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SITConnect</title>
    <meta name="viewport" content="width=device-width,initial-scale=1"/>  
    <link rel="stylesheet" href="~/css/homepage.css"/>
</head>
<body>
    <nav class="navbar navbar-inverse">  
        <div class="container-fluid">  
            <ul class="nav navbar-nav">  
                <li class="active"><a href="/HomePage.aspx">SITConnect</a></li>  
                <li style="float:right"><a href="/ChangePassword.aspx">Change Password</a></li>
            </ul>  
        </div>  
    </nav>
    <form id="form1" runat="server">
        <div>
            <fieldset>
                <legend>HomePage</legend>
                <br />
                <asp:Label ID="lblMessage" runat="server" EnableViewState="false" />
                <br />
                <br />
                <asp:Button ID="btnLogout" runat="server" Text="Logout" OnClick="LogoutMe" Visible="false" />
                <p />
            </fieldset>
        </div>
    </form>
</body>
</html>
