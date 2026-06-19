<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ManagerRounds.dashboard.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
    .tarjeta-dash { cursor:pointer; transition:box-shadow 0.15s; text-decoration:none; display:block; }
    .tarjeta-dash:hover { box-shadow: 0 2px 8px rgba(0,0,0,0.12); }
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div style="display:flex; align-items:center; justify-content:space-between; margin-bottom:20px;">
        <div>
            <h4 style="font-weight:600; margin:0;">Dashboard</h4>
            <p style="color:#888; font-size:13px; margin:4px 0 0;">Manager Rounds · MXQRP1</p>
        </div>
        <div style="display:flex; align-items:center; gap:10px;">
            <asp:LinkButton ID="btnAnterior" runat="server" CssClass="btn btn-outline-secondary btn-sm" OnClick="btnAnterior_Click">‹</asp:LinkButton>
            <div class="text-center">
                <asp:Label ID="lblSemana" runat="server" style="font-size:14px; font-weight:500; display:block;" />
                <asp:Label ID="lblRango" runat="server" style="font-size:12px; color:#888; display:block;" />
            </div>
            <asp:LinkButton ID="btnSiguiente" runat="server" CssClass="btn btn-outline-secondary btn-sm" OnClick="btnSiguiente_Click">›</asp:LinkButton>
        </div>
    </div>

    <asp:HiddenField ID="hfLunes" runat="server" />

   <!-- Fila 1 tarjetas -->
<div style="display:grid; grid-template-columns:repeat(3,1fr); gap:14px; margin-bottom:14px;">
    <a href="/admin/Usuarios.aspx" class="tarjeta-dash">
        <div style="background:#FCEBEB; border-radius:8px; padding:12px 16px; border-left:4px solid #CC0000;">
            <p style="font-size:12px; color:#A32D2D; margin:0 0 4px;">Managers activos</p>
            <asp:Label ID="lblManagersActivos" runat="server" style="font-size:24px; font-weight:600; color:#A32D2D;">0</asp:Label>
        </div>
    </a>
    <a href="/historial/Default.aspx?filtro=completados" class="tarjeta-dash">
        <div style="background:#EAF3DE; border-radius:8px; padding:12px 16px; border-left:4px solid #639922;">
            <p style="font-size:12px; color:#3B6D11; margin:0 0 4px;">Completados hoy</p>
            <asp:Label ID="lblCompletadosHoy" runat="server" style="font-size:24px; font-weight:600; color:#3B6D11;">0</asp:Label>
        </div>
    </a>
    <a href="/historial/Default.aspx?filtro=pendientes" class="tarjeta-dash">
        <div style="background:#FFF3CD; border-radius:8px; padding:12px 16px; border-left:4px solid #F0A500;">
            <p style="font-size:12px; color:#7A5100; margin:0 0 4px;">Pendientes hoy</p>
            <asp:Label ID="lblPendientesHoy" runat="server" style="font-size:24px; font-weight:600; color:#7A5100;">0</asp:Label>
        </div>
    </a>
</div>

<!-- Fila 2 tarjetas -->
<div style="display:grid; grid-template-columns:repeat(3,1fr); gap:14px; margin-bottom:20px;">
    <a href="/historial/Default.aspx" class="tarjeta-dash">
        <div style="background:#E6F1FB; border-radius:8px; padding:12px 16px; border-left:4px solid #378ADD;">
            <p style="font-size:12px; color:#185FA5; margin:0 0 4px;">Cumplimiento semana</p>
            <asp:Label ID="lblCumplimiento" runat="server" style="font-size:24px; font-weight:600; color:#185FA5;">0%</asp:Label>
        </div>
    </a>
    <a href="/historial/Default.aspx?filtro=hallazgos_abiertos" class="tarjeta-dash">
        <div style="background:#FAEEDA; border-radius:8px; padding:12px 16px; border-left:4px solid #854F0B;">
            <p style="font-size:12px; color:#854F0B; margin:0 0 4px;">Hallazgos abiertos</p>
            <asp:Label ID="lblHallazgosAbiertos" runat="server" style="font-size:24px; font-weight:600; color:#854F0B;">0</asp:Label>
        </div>
    </a>
    <a href="/historial/Default.aspx?filtro=hallazgos_cerrados" class="tarjeta-dash">
        <div style="background:#EAF3DE; border-radius:8px; padding:12px 16px; border-left:4px solid #3B6D11;">
            <p style="font-size:12px; color:#3B6D11; margin:0 0 4px;">Hallazgos cerrados esta semana</p>
            <asp:Label ID="lblHallazgosCerrados" runat="server" style="font-size:24px; font-weight:600; color:#3B6D11;">0</asp:Label>
        </div>
    </a>
</div>

    <!-- Tabla managers -->
    <div style="background:#fff; border:1px solid #e8e8e8; border-radius:8px; padding:20px; margin-bottom:20px;">
        <p style="font-size:14px; font-weight:500; margin:0 0 14px;">Estado por manager esta semana</p>
        <asp:GridView ID="gvManagers" runat="server" AutoGenerateColumns="false" 
            CssClass="table" Width="100%" GridLines="None">
            <Columns>
                <asp:BoundField DataField="Nombre" HeaderText="Manager" />
                <asp:BoundField DataField="Seccion" HeaderText="Sección" />
                <asp:TemplateField HeaderText="Lun">
                    <ItemTemplate><%# FormatCalificacion(Eval("Lunes")) %></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Mar">
                    <ItemTemplate><%# FormatCalificacion(Eval("Martes")) %></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Mié">
                    <ItemTemplate><%# FormatCalificacion(Eval("Miercoles")) %></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Jue">
                    <ItemTemplate><%# FormatCalificacion(Eval("Jueves")) %></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Vie">
                    <ItemTemplate><%# FormatCalificacion(Eval("Viernes")) %></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Total semana">
                    <ItemTemplate><%# FormatTotalSemana(Eval("Lunes"), Eval("Martes"), Eval("Miercoles"), Eval("Jueves"), Eval("Viernes")) %></ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <div style="display:flex; gap:20px; margin-top:12px; font-size:12px; color:#888;">
            <span><span style="display:inline-block; width:10px; height:10px; border-radius:50%; background:#639922; margin-right:5px;"></span>≥ 90%</span>
            <span><span style="display:inline-block; width:10px; height:10px; border-radius:50%; background:#F0A500; margin-right:5px;"></span>70–89%</span>
            <span><span style="display:inline-block; width:10px; height:10px; border-radius:50%; background:#E24B4A; margin-right:5px;"></span>&lt; 70%</span>
            <span><span style="display:inline-block; width:10px; height:10px; border-radius:50%; background:#ccc; margin-right:5px;"></span>Sin registro</span>
        </div>
    </div>

    <!-- Gráfica de barras -->
    <div style="background:#fff; border:1px solid #e8e8e8; border-radius:8px; padding:20px;">
        <p style="font-size:14px; font-weight:500; margin:0 0 16px;">Cumplimiento promedio por día</p>
        <div id="graficaBars" style="display:flex; align-items:flex-end; gap:12px; height:140px;">
            <asp:Repeater ID="rptGrafica" runat="server">
                <ItemTemplate>
                    <div style="flex:1; display:flex; flex-direction:column; align-items:center; gap:6px;">
                        <span style="font-size:12px; font-weight:500; color:<%# Eval("Color") %>;"><%# Eval("Etiqueta") %></span>
                        <div style="width:100%; background:<%# Eval("Color") %>; border-radius:4px 4px 0 0; height:<%# Eval("Altura") %>px; min-height:4px;"></div>
                        <span style="font-size:12px; color:#888;"><%# Eval("Dia") %></span>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
</asp:Content>