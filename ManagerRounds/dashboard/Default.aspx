<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ManagerRounds.dashboard.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div style="display:flex; align-items:center; justify-content:space-between; margin-bottom:20px;">
        <div>
            <h4 style="font-weight:600; margin:0;">Dashboard</h4>
            <p style="color:#888; font-size:13px; margin:4px 0 0;">Manager Rounds · MXQRP1</p>
        </div>
    </div>

    <div style="display:grid; grid-template-columns:repeat(4,1fr); gap:14px; margin-bottom:20px;">
        <div style="background:#FCEBEB; border-radius:8px; padding:16px; border-left:4px solid #CC0000;">
            <p style="font-size:12px; color:#A32D2D; margin:0 0 4px;">Managers activos</p>
            <asp:Label ID="lblManagersActivos" runat="server" style="font-size:28px; font-weight:600; color:#A32D2D;">0</asp:Label>
        </div>
        <div style="background:#EAF3DE; border-radius:8px; padding:16px; border-left:4px solid #639922;">
            <p style="font-size:12px; color:#3B6D11; margin:0 0 4px;">Completados hoy</p>
            <asp:Label ID="lblCompletadosHoy" runat="server" style="font-size:28px; font-weight:600; color:#3B6D11;">0</asp:Label>
        </div>
        <div style="background:#FAEEDA; border-radius:8px; padding:16px; border-left:4px solid #BA7517;">
            <p style="font-size:12px; color:#854F0B; margin:0 0 4px;">Pendientes hoy</p>
            <asp:Label ID="lblPendientesHoy" runat="server" style="font-size:28px; font-weight:600; color:#854F0B;">0</asp:Label>
        </div>
        <div style="background:#E6F1FB; border-radius:8px; padding:16px; border-left:4px solid #378ADD;">
            <p style="font-size:12px; color:#185FA5; margin:0 0 4px;">Cumplimiento semana</p>
            <asp:Label ID="lblCumplimiento" runat="server" style="font-size:28px; font-weight:600; color:#185FA5;">0%</asp:Label>
        </div>
    </div>

    <div style="background:#fff; border:1px solid #e8e8e8; border-radius:8px; padding:20px;">
        <p style="font-size:14px; font-weight:500; margin:0 0 14px;">Estado por manager esta semana</p>
        <asp:GridView ID="gvManagers" runat="server" AutoGenerateColumns="false" 
            CssClass="table" Width="100%" GridLines="None">
            <Columns>
                <asp:BoundField DataField="Nombre" HeaderText="Manager" />
                <asp:BoundField DataField="Seccion" HeaderText="Sección" />
                <asp:BoundField DataField="Lunes" HeaderText="Lun" NullDisplayText="—" DataFormatString="{0:0}%" />
                <asp:BoundField DataField="Martes" HeaderText="Mar" NullDisplayText="—" DataFormatString="{0:0}%" />
                <asp:BoundField DataField="Miercoles" HeaderText="Mié" NullDisplayText="—" DataFormatString="{0:0}%" />
                <asp:BoundField DataField="Jueves" HeaderText="Jue" NullDisplayText="—" DataFormatString="{0:0}%" />
                <asp:BoundField DataField="Viernes" HeaderText="Vie" NullDisplayText="—" DataFormatString="{0:0}%" />
            </Columns>
        </asp:GridView>
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
</asp:Content>