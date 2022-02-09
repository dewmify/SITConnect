<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Verify.aspx.cs" Inherits="AppSecAsgn.Verify" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="~/css/verification.css">   
</head>
<body>
    <form id="form1" runat="server">
        <div>
        <fieldset>
        <legend>Enter the Verification code sent to your email</legend>
        <p>Verification code<asp:TextBox ID ="tb_vfCode" runat="server" Height="25px" Width="137px" /></p>
        <p><asp:Button ID ="btnSubmit" runat="server" Text="Validate code" OnClick="VerifyCode" Height="27px" Width="133px" />
        <br />
        <br />
        <asp:Label ID="lblMessage" runat="server" EnabledViewState="False" ForeColor="Red"></asp:Label>
        </p>
        </fieldset>
        </div>
    </form>
</body>
</html>
