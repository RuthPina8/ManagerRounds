<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ManagerRounds.formulario.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div style="display:flex; align-items:center; justify-content:space-between; margin-bottom:20px;">
        <div>
            <h4 style="font-weight:600; margin:0;">Formulario del día</h4>
            <asp:Label ID="lblFecha" runat="server" style="color:#888; font-size:13px;" />
        </div>
        <asp:Label ID="lblEstatus" runat="server" CssClass="badge-pendiente" 
            style="padding:4px 12px; border-radius:20px; font-size:12px; font-weight:500;" />
    </div>

    <div style="background:#f0f0f0; border-radius:8px; padding:12px 16px; margin-bottom:20px; display:flex; align-items:center; justify-content:space-between;">
        <span style="font-size:13px; color:#555;">
            <asp:Label ID="lblNombreManager" runat="server" />
        </span>
        <span style="font-size:13px; color:#888;">
            <asp:Label ID="lblRespondidas" runat="server" /> respondidas
        </span>
    </div>

    <div style="margin-bottom:20px;">
        <p style="font-size:12px; color:#888; margin:0 0 8px;">Tipo de check — selecciona el que corresponde hoy</p>
        <div style="display:flex; gap:8px;">
            <asp:Button ID="btnCheckA" runat="server" Text="A · Área productiva" 
                CssClass="btn-astemo" OnClick="btnCheckA_Click" />
            <asp:Button ID="btnCheckB" runat="server" Text="B · Almacenes / lab." 
                style="background:#f0f0f0; color:#444; border:none; border-radius:6px; padding:8px 18px; font-size:13px; cursor:pointer;"
                OnClick="btnCheckB_Click" />
        </div>
    </div>

    <asp:Panel ID="pnlFormulario" runat="server" Visible="false">
        <div style="background:#fff; border:1px solid #e8e8e8; border-radius:8px; padding:20px; margin-bottom:20px;">
            <asp:Repeater ID="rptPreguntas" runat="server">
                <ItemTemplate>
                    <div style="padding:14px 0; border-bottom:1px solid #f0f0f0;">
                        <div style="display:flex; align-items:flex-start; gap:12px;">
                            <div style="width:22px; height:22px; border-radius:50%; background:#CC0000; color:#fff; font-size:11px; font-weight:500; display:flex; align-items:center; justify-content:center; flex-shrink:0; margin-top:2px;">
                                <%# Container.ItemIndex + 1 %>
                            </div>
                            <div style="flex:1;">
                                <p style="font-size:13px; margin:0 0 8px;"><%# Eval("Pregunta") %></p>
                                <asp:HiddenField ID="hfPreguntaId" runat="server" Value='<%# Eval("id") %>' />
                                <div style="display:flex; gap:6px; margin-bottom:8px;">
                                    <asp:RadioButtonList ID="rblRespuesta" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="opciones-respuesta">
                                        <asp:ListItem Value="1" Text="Cumple" />
                                        <asp:ListItem Value="2" Text="No cumple" />
                                        <asp:ListItem Value="3" Text="N/A" />
                                    </asp:RadioButtonList>
                                </div>
                                <div>
                                    <asp:CheckBox ID="chkSinComentario" runat="server" Text=" Sin comentario" 
                                        style="font-size:12px; color:#888;" />
                                    <asp:TextBox ID="txtComentario" runat="server" 
                                        placeholder="Escribe un comentario..." 
                                        CssClass="form-control" style="font-size:12px; margin-top:6px;" />
                                </div>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <div style="display:flex; justify-content:flex-end;">
            <asp:Button ID="btnGuardar" runat="server" Text="Guardar" 
                CssClass="btn-astemo" OnClick="btnGuardar_Click" />
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlYaCompletado" runat="server" Visible="false">
        <div style="background:#EAF3DE; border-radius:8px; padding:24px; text-align:center;">
            <i class="fas fa-check-circle" style="font-size:36px; color:#639922; margin-bottom:10px; display:block;"></i>
            <p style="font-size:15px; font-weight:500; color:#3B6D11; margin:0;">Formulario completado</p>
            <p style="font-size:13px; color:#3B6D11; margin:6px 0 0;">Ya registraste tu formulario del día.</p>
        </div>
    </asp:Panel>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
<style>
    .opciones-respuesta label {
        margin-right: 12px;
        font-size: 13px;
        cursor: pointer;
    }
</style>
</asp:Content>