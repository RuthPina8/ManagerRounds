<%@ Page Title="Historial" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ManagerRounds.historial.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<div class="d-flex justify-content-between align-items-center mb-3">
    <h4 class="mb-0">Historial</h4>
    <div class="d-flex align-items-center" style="gap:8px;">
        <asp:LinkButton ID="btnAnterior" runat="server" CssClass="btn btn-outline-secondary btn-sm" OnClick="btnAnterior_Click">‹</asp:LinkButton>
        <div class="text-center">
            <asp:Label ID="lblSemana" runat="server" CssClass="d-block font-weight-500" style="font-size:14px;" />
            <asp:Label ID="lblRangoSemana" runat="server" CssClass="d-block text-muted" style="font-size:12px;" />
        </div>
        <asp:LinkButton ID="btnSiguiente" runat="server" CssClass="btn btn-outline-secondary btn-sm" OnClick="btnSiguiente_Click">›</asp:LinkButton>
    </div>
</div>

<!-- Pestañas -->
<ul class="nav nav-tabs mb-3">
    <li class="nav-item">
        <asp:LinkButton ID="tabRevisiones" runat="server" CssClass="nav-link active" OnClick="tabRevisiones_Click">Revisiones</asp:LinkButton>
    </li>
    <li class="nav-item">
        <asp:LinkButton ID="tabBitacora" runat="server" CssClass="nav-link" OnClick="tabBitacora_Click">Bitácora</asp:LinkButton>
    </li>
</ul>

<asp:HiddenField ID="hfTab" runat="server" Value="revisiones" />
<asp:HiddenField ID="hfLunes" runat="server" />

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
            <asp:GridView ID="gvRevisiones" runat="server" CssClass="table table-hover mb-0"
                AutoGenerateColumns="false" DataKeyNames="id"
                OnRowCommand="gvRevisiones_RowCommand">
                <Columns>
                    <asp:TemplateField HeaderText="Manager">
                        <ItemTemplate><%# Eval("Usuarios.Nombre") %></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Sección">
                        <ItemTemplate><%# Eval("ManagerSecciones.Seccion") %></ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Check_id" HeaderText="Check" />
                    <asp:TemplateField HeaderText="Fecha y hora">
                        <ItemTemplate><%# ((DateTime?)Eval("FechaInicio"))?.ToString("ddd dd MMM · hh:mm tt") %></ItemTemplate>
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
                    <asp:TemplateField HeaderText="Ver">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CommandName="Ver" CommandArgument='<%# Eval("id") %>'
                                CssClass="btn btn-sm btn-outline-secondary">
                                <i class="fas fa-eye"></i>
                            </asp:LinkButton>
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
                    <div class="d-flex mb-3" style="gap:12px; align-items:flex-start;">
                        <div style="width:8px; height:8px; border-radius:50%; background:<%# Eval("Color") %>; margin-top:5px; flex-shrink:0;"></div>
                        <div>
                            <p class="mb-0" style="font-size:13px;"><%# Eval("Descripcion") %></p>
                            <p class="mb-0 text-muted" style="font-size:12px;"><%# Eval("Fecha") %></p>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:Panel>

</asp:Content>