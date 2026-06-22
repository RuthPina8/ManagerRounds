<%@ Page Title="Historial" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ManagerRounds.historial.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<style>
    .nav-tabs-custom { border-bottom: 2px solid #e8e8e8; margin-bottom: 20px; }
    .nav-tabs-custom .nav-link {
        border: none;
        color: #888;
        font-size: 13px;
        font-weight: 500;
        padding: 10px 18px;
        border-bottom: 2px solid transparent;
        margin-bottom: -2px;
        border-radius: 0;
        background: transparent;
    }
    .nav-tabs-custom .nav-link:hover { color: #333; }
    .nav-tabs-custom .nav-link.active { color: #CC0000; border-bottom: 2px solid #CC0000; }
    .table-historial tr:hover td { background: #fff5f5; }
    .semana-nav {
        display: flex;
        align-items: center;
        gap: 10px;
        background: #f8f8f8;
        border: 1px solid #e8e8e8;
        border-radius: 8px;
        padding: 6px 12px;
    }
    .semana-nav .btn-nav {
        width: 28px; height: 28px;
        border-radius: 6px;
        border: 1px solid #e0e0e0;
        background: #fff;
        color: #666;
        font-size: 14px;
        cursor: pointer;
        display: flex;
        align-items: center;
        justify-content: center;
        transition: all 0.15s;
    }
    .semana-nav .btn-nav:hover { background: #f0f0f0; color: #333; }
    .badge-check-hist { display: inline-block; padding: 2px 8px; border-radius: 20px; font-size: 11px; font-weight: 600; }
    .bitacora-dot { width: 10px; height: 10px; border-radius: 50%; flex-shrink: 0; margin-top: 4px; }
    .bitacora-item { display: flex; gap: 12px; align-items: flex-start; padding: 10px 0; border-bottom: 1px solid #f5f5f5; }
    .bitacora-item:last-child { border-bottom: none; }
</style>

<div class="d-flex justify-content-between align-items-center mb-4">
    <div>
        <h4 style="font-weight:600; margin:0;">Historial</h4>
        <p style="font-size:13px; color:#888; margin:4px 0 0;">Consulta de revisiones y actividad del sistema</p>
    </div>
    <div style="display:flex; align-items:center; gap:10px;">
        <asp:Panel ID="pnlReporte" runat="server" Visible="false">
            <div style="display:flex; align-items:center; gap:6px; background:#f8f8f8; border:1px solid #e8e8e8; border-radius:8px; padding:6px 10px;">
                <i class="fas fa-file-alt" style="color:#888; font-size:13px;"></i>
                <asp:DropDownList ID="ddlMes" runat="server" CssClass="form-control form-control-sm" style="width:100px; border:none; background:transparent; font-size:12px;">
                    <asp:ListItem Value="1">Enero</asp:ListItem>
                    <asp:ListItem Value="2">Febrero</asp:ListItem>
                    <asp:ListItem Value="3">Marzo</asp:ListItem>
                    <asp:ListItem Value="4">Abril</asp:ListItem>
                    <asp:ListItem Value="5">Mayo</asp:ListItem>
                    <asp:ListItem Value="6">Junio</asp:ListItem>
                    <asp:ListItem Value="7">Julio</asp:ListItem>
                    <asp:ListItem Value="8">Agosto</asp:ListItem>
                    <asp:ListItem Value="9">Septiembre</asp:ListItem>
                    <asp:ListItem Value="10">Octubre</asp:ListItem>
                    <asp:ListItem Value="11">Noviembre</asp:ListItem>
                    <asp:ListItem Value="12">Diciembre</asp:ListItem>
                </asp:DropDownList>
                <asp:DropDownList ID="ddlAnio" runat="server" CssClass="form-control form-control-sm" style="width:70px; border:none; background:transparent; font-size:12px;">
                    <asp:ListItem Value="2025">2025</asp:ListItem>
                    <asp:ListItem Value="2026">2026</asp:ListItem>
                    <asp:ListItem Value="2027">2027</asp:ListItem>
                </asp:DropDownList>
                <asp:Button ID="btnDescargarExcel" runat="server" Text="↓ Excel" CssClass="btn btn-sm" style="background:#217346; color:#fff; font-size:12px; padding:4px 10px;" OnClick="btnDescargarExcel_Click" />
                <asp:Button ID="btnDescargarPdf" runat="server" Text="↓ PDF" CssClass="btn btn-sm" style="background:#CC0000; color:#fff; font-size:12px; padding:4px 10px;" OnClick="btnDescargarPdf_Click" />
            </div>
        </asp:Panel>
        <div class="semana-nav">
            <asp:LinkButton ID="btnAnterior" runat="server" CssClass="btn-nav" OnClick="btnAnterior_Click">‹</asp:LinkButton>
            <div class="text-center" style="min-width:140px;">
                <asp:Label ID="lblSemana" runat="server" style="font-size:14px; font-weight:600; color:#222; display:block;" />
                <asp:Label ID="lblRangoSemana" runat="server" style="font-size:11px; color:#888; display:block;" />
            </div>
            <asp:LinkButton ID="btnSiguiente" runat="server" CssClass="btn-nav" OnClick="btnSiguiente_Click">›</asp:LinkButton>
        </div>
    </div>
</div>

<asp:HiddenField ID="hfTab" runat="server" Value="revisiones" />
<asp:HiddenField ID="hfLunes" runat="server" />

<!-- Pestañas -->
<ul class="nav nav-tabs-custom">
    <li class="nav-item">
        <asp:LinkButton ID="tabRevisiones" runat="server" CssClass="nav-link active" OnClick="tabRevisiones_Click">
            <i class="fas fa-clipboard-list mr-1"></i> Revisiones
        </asp:LinkButton>
    </li>
    <li class="nav-item">
        <asp:LinkButton ID="tabBitacora" runat="server" CssClass="nav-link" OnClick="tabBitacora_Click">
            <i class="fas fa-history mr-1"></i> Bitácora
        </asp:LinkButton>
    </li>
</ul>

<!-- PANEL REVISIONES -->
<asp:Panel ID="panelRevisiones" runat="server">
    <div class="card card-mr mb-3">
        <div class="card-body py-2">
            <div class="row">
                <div class="col-md-5">
                    <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control form-control-sm" placeholder="Buscar por manager o sección..." />
                </div>
                <div class="col-md-2">
                    <asp:DropDownList ID="ddlFiltroCheck" runat="server" CssClass="form-control form-control-sm">
                        <asp:ListItem Value="">Todos los checks</asp:ListItem>
                        <asp:ListItem Value="1A">1A</asp:ListItem>
                        <asp:ListItem Value="1B">1B</asp:ListItem>
                        <asp:ListItem Value="2A">2A</asp:ListItem>
                        <asp:ListItem Value="2B">2B</asp:ListItem>
                        <asp:ListItem Value="3A">3A</asp:ListItem>
                        <asp:ListItem Value="3B">3B</asp:ListItem>
                        <asp:ListItem Value="4A">4A</asp:ListItem>
                        <asp:ListItem Value="4B">4B</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-md-3">
                    <asp:DropDownList ID="ddlFiltroEstatus" runat="server" CssClass="form-control form-control-sm">
                        <asp:ListItem Value="">Todos los estatus</asp:ListItem>
                        <asp:ListItem Value="1">Pendiente</asp:ListItem>
                        <asp:ListItem Value="2">Revisado</asp:ListItem>
                        <asp:ListItem Value="3">Rechazado</asp:ListItem>
                        <asp:ListItem Value="4">Corregido</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-md-2">
                    <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn btn-secondary btn-sm btn-block" OnClick="btnFiltrar_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="card card-mr">
        <div class="card-body p-0">
            <asp:GridView ID="gvRevisiones" runat="server" CssClass="table table-historial mb-0"
                AutoGenerateColumns="false" DataKeyNames="id"
                OnRowCommand="gvRevisiones_RowCommand">
                <Columns>
                    <asp:TemplateField HeaderText="Manager">
                        <ItemTemplate>
                            <span style="font-weight:500; font-size:13px;"><%# Eval("Usuarios.Nombre") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Sección">
                        <ItemTemplate>
                            <span style="font-size:12px; color:#888;"><%# Eval("ManagerSecciones.Seccion") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Check">
                        <ItemTemplate>
                            <span class='badge-check-hist badge-check-<%# Eval("Check_id").ToString().ToLower() %>'><%# Eval("Check_id") %></span>
                            <%# (bool)Eval("EntregadoTarde") ? "<span style='background:#FAEEDA; color:#854F0B; font-size:10px; padding:1px 6px; border-radius:10px; margin-left:4px;'>Tarde</span>" : "" %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Fecha y hora">
                        <ItemTemplate>
                            <span style="font-size:12px; color:#888;"><%# ((DateTime?)Eval("FechaInicio"))?.ToString("ddd dd MMM · hh:mm tt") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Calificación">
                        <ItemTemplate>
                            <span class='<%# GetBadgeCalificacion((decimal?)Eval("Calificacion")) %>'>
                                <%# Eval("Calificacion") != null ? Eval("Calificacion") + "%" : "—" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Estatus">
                        <ItemTemplate>
                            <span class='<%# GetBadgeEstatus(Eval("EstatusRevision.Estatus").ToString()) %>'>
                                <%# Eval("EstatusRevision.Estatus") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Acciones">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CommandName="Ver" CommandArgument='<%# Eval("id") %>'
                                CssClass="btn-accion" title="Ver detalle">
                                <i class="fas fa-eye"></i>
                            </asp:LinkButton>
                            <%# Session["rol"].ToString() == "Manager" && Eval("EstatusRevision.Estatus").ToString() == "Pendiente" ? 
                                "<a href='/formulario/Default.aspx?editar=" + Eval("Check_id") + "&revid=" + Eval("id") + "' class='btn-accion ml-1' title='Editar'><i class='fas fa-edit'></i></a>" 
                                : "" %>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Panel>

<!-- PANEL BITÁCORA -->
<asp:Panel ID="panelBitacora" runat="server" Visible="false">
    <div class="card card-mr">
        <div class="card-body">
            <asp:Repeater ID="rptBitacora" runat="server">
                <ItemTemplate>
                    <div class="bitacora-item">
                        <div class="bitacora-dot" style="background:<%# Eval("Color") %>;"></div>
                        <div>
                            <p class="mb-0" style="font-size:13px; color:#333;"><%# Eval("Descripcion") %></p>
                            <p class="mb-0" style="font-size:11px; color:#aaa; margin-top:2px;"><%# Eval("Fecha") %></p>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:Panel>

</asp:Content>