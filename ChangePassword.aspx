<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="AppSecAsgn.ChangePassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta name="viewport" content="width=device-width,initial-scale=1"/>  
    <link rel="stylesheet" href="~/css/changepassword.css"/>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ChangePassword ID ="changePwd" runat="server"
                OnChangingPassword="OnChangingPassword"
                RenderOuterTable="false" 
                NewPasswordRegularExpression="^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$" 
                NewPasswordRegularExpressionErrorMessage="Password must be at least 8 characters"
                CancelDestinationPageUrl="~/HomePage.aspx">
            </asp:ChangePassword>
            <br />
            <asp:Label ID="lblMessage" runat="server" />
        </div>
    </form>
</body>
</html>
