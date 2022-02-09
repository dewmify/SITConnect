<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="AppSecAsgn.Registration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta name="viewport" content="width=device-width,initial-scale=1"/>  
    <link rel="stylesheet" href="~/css/registration.css"/>
    <script type ="text/javascript">
        function validate() {
            var str = document.getElementById('<%=tb_pwd.ClientID%>').value;
            if (str.length < 8) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password length Must be at least 8 characters";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("too_short");

            }
            else if (str.search(/[0-9]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password require at least 1 number";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_number");
            }
            else if (str.search(/[A-Z]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password requires uppercase characters";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_uppercase");
            }
            else if (str.search(/[a-z]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password requires lowercase characters";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_lowercase");
            }
            else if (str.search(/[^A-Za-z0-9]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password requires at least 1 special character";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_specialchar");
            }
            document.getElementById("lbl_pwdchecker").innerHTML = "Excellent!";
            document.getElementById("lbl_pwdchecker").style.color = "Green";

        }
    </script>
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
            Registration
        </div>
        <table class="auto-style1">
            <tr>
                <td class="auto-style2">First Name</td>
                <td>
                    <asp:TextBox ID="tb_fname" runat="server"></asp:TextBox>
                    <br />
                    <asp:RequiredFieldValidator ID="fname_validator" runat="server" ControlToValidate="tb_fname" ErrorMessage="Enter your first name" ForeColor="Red"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="auto-style2">Last Name</td>
                <td>
                    <asp:TextBox ID="tb_lname" runat="server"></asp:TextBox>
                    <br />
                    <asp:RequiredFieldValidator ID="lname_validator" runat="server" ControlToValidate="tb_lname" ErrorMessage="Enter your last name" ForeColor="Red"></asp:RequiredFieldValidator>
                </td>
                
            </tr>
            <tr>
                <td class="auto-style2">Credit Card Info</td>
                <td>
                    <asp:TextBox ID="tb_creditcard" runat="server"></asp:TextBox>
                    <br />
                <asp:RequiredFieldValidator ID="creditcard_validator" runat="server" ControlToValidate="tb_creditcard" ErrorMessage="Enter your credit card details" ForeColor="Red"></asp:RequiredFieldValidator>
                </td> 
            </tr>
            <tr>
                <td class="auto-style2">Email address</td>
                <td>
                    <asp:TextBox ID="tb_email" runat="server"></asp:TextBox>   
                 <br />
                <asp:RequiredFieldValidator ID="email_validator" runat="server" ControlToValidate="tb_email" ErrorMessage="Enter your email" ForeColor="Red"></asp:RequiredFieldValidator>
                </td>
                
            </tr>
            <tr>
                <td class="auto-style2">Password</td>
                <td>
                    <p>
                    <asp:TextBox ID="tb_pwd" runat="server" TextMode="Password" onkeyup="javascript:validate()"></asp:TextBox>
                        <asp:Button ID="Button2" runat="server" Text="Check Password" Width="205px" OnClick="btn_checkPassword_Click" />
                     <asp:Label ID="lbl_pwdchecker" runat="server" Text=""></asp:Label>
                    </p>
                </td>
            </tr>
            <tr>
                <td class="auto-style2">Date of Birth</td>
                <td>
                    <asp:TextBox ID="tb_dob" runat="server" type="date"></asp:TextBox>
                    <br />
                <asp:RequiredFieldValidator ID="dob_validator" runat="server" ControlToValidate="tb_dob" ErrorMessage="Enter your date of birth" ForeColor="Red"></asp:RequiredFieldValidator>
                </td>
                
            </tr>
            <tr>
                <td class="auto-style2">Upload Photo</td>
                <td>
                    <input id="oFile" type="file" runat="server" name="oFile"/>
                    <asp:Panel ID="frmConfirmation" Visible="False" Runat="server">
                        <asp:Label id="lblUploadResult" Runat="server"></asp:Label>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td class="auto-style2">&nbsp;</td>
                <td>
                    <asp:Button ID="submit" runat="server" Text="Submit" Width="185px" />
                </td>
            </tr>
        </table>
    </form>
    
    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('6LdU9mIeAAAAAA8K3u4iFM84VK0oXqhmDfXcQ53p', { action: 'Register' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });
    </script>
</body>
</html>
