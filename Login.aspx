<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="AppSecAsgn.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SITConnect</title>
    <meta name="viewport" content="width=device-width,initial-scale=1"/>  
    <link rel="stylesheet" href="~/css/login.css"/> 
    <script src="https://www.google.com/recaptcha/api.js?render=6Ld-FWkeAAAAAAtAYQgDiYC_5X3ppTBPrEWIOMkI"></script>
</head>
<body>
    <nav class="navbar navbar-inverse">  
        <div class="container-fluid">  
            <ul class="nav navbar-nav">  
                <li class="active"><a href="/HomePage.aspx">SITConnect</a></li>  
                <li style="float:right"><a href="/Login.aspx">Login</a></li>
                <li style="float:right"><a href="/Registration.aspx">Register</a></li>
            </ul>  
        </div>  
    </nav>  
    <form id="form1" runat="server">
        <div>
        <fieldset>
        <legend>Login</legend>
        <p>Email : <asp:TextBox ID ="tb_email" runat="server" Height="25px" Width="137px" onkeyup="javascript:validateEmail()"/>
        </p>
        <p>Password : <asp:TextBox ID="tb_pwd" runat="server" type="password" Height="24px" Width="137px" onkeyup="javascript:validatePwd()"/>
        </p>
        <p><asp:Button ID ="btnSubmit" runat="server" Text="Login" OnClick="LoginMe" Height="27px" Width="133px" />
        <br />
        <br />
        <asp:Label ID="lblMessage" runat="server" EnabledViewState="False" ForeColor="Red"></asp:Label>
        </p>
        </fieldset>
        </div>
        <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
    </form>
</body>
    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('6Ld-FWkeAAAAAAtAYQgDiYC_5X3ppTBPrEWIOMkI', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });
    </script>
</html>
