<%@ Page Title="Mi Perfil" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Perfil.aspx.cs" Inherits="ManagerRounds.Account.Perfil" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<div class="d-flex justify-content-between align-items-center mb-3">
    <h4 class="mb-0">Mi Perfil</h4>
</div>

<asp:Label ID="lblMensaje" runat="server" Visible="false" CssClass="alert d-block mb-3" />

<div class="card card-mr mb-3">
    <div class="card-body">
        <div class="row mb-3">
            <div class="col-md-6">
                <p class="text-muted mb-1" style="font-size:12px;">Nombre</p>
                <asp:Label ID="lblNombre" runat="server" style="font-size:15px; font-weight:500;" />
            </div>
            <div class="col-md-3">
                <p class="text-muted mb-1" style="font-size:12px;">Nómina</p>
                <asp:Label ID="lblNomina" runat="server" style="font-size:15px;" />
            </div>
            <div class="col-md-3">
                <p class="text-muted mb-1" style="font-size:12px;">Rol</p>
                <asp:Label ID="lblRol" runat="server" style="font-size:15px;" />
            </div>
        </div>
        <asp:Panel ID="panelSeccion" runat="server" Visible="false">
            <div class="row">
                <div class="col-md-6">
                    <p class="text-muted mb-1" style="font-size:12px;">Sección</p>
                    <asp:Label ID="lblSeccion" runat="server" style="font-size:15px;" />
                </div>
                <div class="col-md-6">
                    <p class="text-muted mb-1" style="font-size:12px;">Tipo de área</p>
                    <asp:Label ID="lblTipoArea" runat="server" style="font-size:15px;" />
                </div>
            </div>
        </asp:Panel>
    </div>
</div>

<div class="card card-mr">
    <div class="card-body">
        <p style="font-size:14px; font-weight:500;" class="mb-3">Cambiar contraseña</p>
        <div class="form-group">
            <label style="font-size:13px;">Contraseña actual</label>
            <asp:TextBox ID="txtPasswordActual" runat="server" CssClass="form-control" TextMode="Password" MaxLength="50" />
        </div>
        <div class="form-group">
            <label style="font-size:13px;">Nueva contraseña</label>
            <asp:TextBox ID="txtPasswordNueva" runat="server" CssClass="form-control" TextMode="Password" MaxLength="50" />
        </div>
        <div class="form-group">
            <label style="font-size:13px;">Confirmar nueva contraseña</label>
            <asp:TextBox ID="txtPasswordConfirm" runat="server" CssClass="form-control" TextMode="Password" MaxLength="50" />
        </div>
        <asp:Button ID="btnCambiarPassword" runat="server" Text="Cambiar contraseña" CssClass="btn btn-astemo" OnClick="btnCambiarPassword_Click" />
    </div>
</div>

</asp:Content>
