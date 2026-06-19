<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Recuperar.aspx.cs" Inherits="ManagerRounds.Account.Recuperar" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Recuperar contraseña · Manager Rounds</title>
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

        .box {
            background: #fff;
            border-radius: 10px;
            padding: 36px 40px;
            width: 380px;
            box-shadow: 0 2px 16px rgba(0,0,0,0.08);
        }

        .box-header {
            text-align: center;
            margin-bottom: 24px;
        }

        .box-header img {
            height: 50px;
            margin-bottom: 12px;
        }

        .box-header h5 {
            font-size: 17px;
            font-weight: 600;
            margin: 0;
            color: #222;
        }

        .box-header p {
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

        .btn-primary {
            width: 100%;
            background: #CC0000;
            color: #fff;
            border: none;
            border-radius: 6px;
            padding: 10px;
            font-size: 14px;
            font-weight: 500;
            cursor: pointer;
        }

        .btn-primary:hover { background: #aa0000; }

        .alert { font-size: 13px; border-radius: 6px; padding: 8px 12px; }
        .alert-danger { background: #FCEBEB; color: #A32D2D; border: none; }
        .alert-success { background: #EAF3DE; color: #3B6D11; border: none; }

        .back-link {
            text-align: center;
            margin-top: 16px;
            font-size: 13px;
            color: #888;
        }

        .back-link a { color: #CC0000; text-decoration: none; }
        .back-link a:hover { text-decoration: underline; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="box">
            <div class="box-header">
                <img src="/Content/astemo-logo.png" alt="Astemo" onerror="this.style.display='none'" />
                <h5>Recuperar contraseña</h5>
                <p>Manager Rounds · MXQRP1</p>
            </div>

            <asp:Label ID="lblMensaje" runat="server" Visible="false" CssClass="alert d-block mb-3" />

            <!-- Paso 1: Nómina -->
            <asp:Panel ID="pnlPaso1" runat="server">
                <div class="form-group">
                    <label class="form-label">Nómina</label>
                    <asp:TextBox ID="txtNomina" runat="server" CssClass="form-control" placeholder="Ej. 71208377" />
                </div>
                <asp:Button ID="btnBuscar" runat="server" Text="Continuar" CssClass="btn btn-primary" OnClick="btnBuscar_Click" />
            </asp:Panel>

            <!-- Paso 2: Pregunta y respuesta -->
            <asp:Panel ID="pnlPaso2" runat="server" Visible="false">
                <div class="form-group">
                    <label class="form-label">Pregunta de seguridad</label>
                    <asp:Label ID="lblPregunta" runat="server" CssClass="form-control d-block" style="background:#f8f8f8; height:auto; min-height:38px;" />
                </div>
                <div class="form-group">
                    <label class="form-label">Respuesta</label>
                    <asp:TextBox ID="txtRespuesta" runat="server" CssClass="form-control" />
                </div>
                <asp:Button ID="btnValidar" runat="server" Text="Validar" CssClass="btn btn-primary" OnClick="btnValidar_Click" />
            </asp:Panel>

            <!-- Paso 3: Nueva contraseña -->
            <asp:Panel ID="pnlPaso3" runat="server" Visible="false">
                <div class="form-group">
                    <label class="form-label">Nueva contraseña</label>
                    <asp:TextBox ID="txtNueva" runat="server" CssClass="form-control" TextMode="Password" />
                </div>
                <div class="form-group">
                    <label class="form-label">Confirmar contraseña</label>
                    <asp:TextBox ID="txtConfirmar" runat="server" CssClass="form-control" TextMode="Password" />
                </div>
                <asp:Button ID="btnResetear" runat="server" Text="Guardar nueva contraseña" CssClass="btn btn-primary" OnClick="btnResetear_Click" />
            </asp:Panel>

            <div class="back-link">
                <a href="/Account/Login.aspx">← Volver al login</a>
            </div>
        </div>
    </form>
</body>
</html>
