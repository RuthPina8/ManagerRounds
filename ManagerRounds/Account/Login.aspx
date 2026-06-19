<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ManagerRounds.Account.Login" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Manager Rounds · Login</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@4.6.2/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body {
            font-family: 'Segoe UI', sans-serif;
            background: #f4f6f8;
            display: flex;
            align-items: center;
            justify-content: center;
            min-height: 100vh;
            margin: 0;
        }

        .login-box {
            background: #fff;
            border-radius: 10px;
            padding: 36px 40px;
            width: 360px;
            box-shadow: 0 2px 16px rgba(0,0,0,0.08);
        }

        .login-header {
            text-align: center;
            margin-bottom: 28px;
        }

        .login-icon {
            width: 52px;
            height: 52px;
            background: #CC0000;
            border-radius: 10px;
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0 auto 14px;
        }

        .login-icon i {
            color: #fff;
            font-size: 22px;
        }

        .login-header h5 {
            font-size: 17px;
            font-weight: 600;
            margin: 0;
            color: #222;
        }

        .login-header p {
            font-size: 13px;
            color: #888;
            margin: 4px 0 0;
        }

        .form-label {
            font-size: 13px;
            color: #555;
            margin-bottom: 5px;
        }

        .form-control {
            font-size: 13px;
            border-radius: 6px;
            border: 1px solid #ddd;
            padding: 9px 12px;
        }

        .form-control:focus {
            border-color: #CC0000;
            box-shadow: 0 0 0 2px rgba(204,0,0,0.1);
        }

        .btn-login {
            width: 100%;
            background: #CC0000;
            color: #fff;
            border: none;
            border-radius: 6px;
            padding: 10px;
            font-size: 14px;
            font-weight: 500;
            margin-top: 8px;
            cursor: pointer;
        }

        .btn-login:hover {
            background: #aa0000;
        }

        .error-msg {
            background: #FCEBEB;
            color: #A32D2D;
            border-radius: 6px;
            padding: 8px 12px;
            font-size: 13px;
            margin-bottom: 14px;
            display: none;
        }
    </style>
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-box">
            <div class="login-header">
             
                <img src="/Content/astemo-logo.png" alt="Astemo" style="height:60px; margin: 0 auto 14px; display:block;" />
                <h4>Manager Rounds</h4>
            </div>

            <div class="error-msg" id="errorMsg" runat="server">
                Nómina o contraseña incorrectos.
            </div>

            <div class="form-group">
                <label class="form-label">Nómina</label>
                <asp:TextBox ID="txtNomina" runat="server" CssClass="form-control" placeholder="Ej. 71208377" />
            </div>

            <div class="form-group">
                <label class="form-label">Contraseña</label>
                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="••••••••" />
            </div>

            <asp:Button ID="btnLogin" runat="server" Text="Iniciar sesión" CssClass="btn-login" OnClick="btnLogin_Click" />
            <div style="text-align:center; margin-top:12px;">
    <a href="/Account/Recuperar.aspx" style="font-size:12px; color:#CC0000; text-decoration:none;">¿Olvidaste tu contraseña?</a>
</div>
            <center><p style="font-size:11px; color:#bbb; margin-top:12px;">Astemo · MXQRP1</p></center>
        </div>
            
    </form>
    <script src="https://cdn.jsdelivr.net/npm/jquery@3.6.0/dist/jquery.min.js"></script>
   
</body>
</html>
