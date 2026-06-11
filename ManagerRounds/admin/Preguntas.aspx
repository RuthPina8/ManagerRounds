<%@ Page Title="Preguntas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Preguntas.aspx.cs" Inherits="ManagerRounds.admin.Preguntas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<div class="d-flex justify-content-between align-items-center mb-3">
    <h4 class="mb-0">Gestión de Preguntas</h4>
    <div class="d-flex" style="gap:8px;">
        <div class="dropdown">
            <button class="btn btn-outline-secondary dropdown-toggle" type="button" id="btnChecks" data-toggle="dropdown">
                <i class="fas fa-sliders-h mr-1"></i> Gestionar checks
            </button>
            <div class="dropdown-menu dropdown-menu-right" style="min-width:260px;">
                <h6 class="dropdown-header">Activar / desactivar check</h6>
                <asp:Repeater ID="rptChecks" runat="server" OnItemCommand="rptChecks_ItemCommand">
                    <ItemTemplate>
                        <div class="dropdown-item d-flex justify-content-between align-items-center">
                            <span>
                                <span class="badge badge-secondary mr-2"><%# Eval("CheckId") %></span>
                                <%# Eval("Descripcion") %>
                            </span>
                            <asp:LinkButton runat="server"
                                CommandName='<%# (bool)Eval("Activo") ? "DesactivarCheck" : "ActivarCheck" %>'
                                CommandArgument='<%# Eval("CheckId") %>'
                                CssClass='<%# (bool)Eval("Activo") ? "btn btn-sm btn-outline-danger ml-2" : "btn btn-sm btn-outline-success ml-2" %>'>
                                <i class='<%# (bool)Eval("Activo") ? "fas fa-eye-slash" : "fas fa-eye" %>'></i>
                            </asp:LinkButton>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
        <button type="button" class="btn btn-astemo" data-toggle="modal" data-target="#modalPregunta" onclick="limpiarModal()">
            <i class="fas fa-plus mr-1"></i> Nueva pregunta
        </button>
    </div>
</div>

<asp:Label ID="lblMensaje" runat="server" Visible="false" CssClass="alert d-block mb-3" />

<!-- Filtros -->
<div class="card card-mr mb-3">
    <div class="card-body py-2">
        <div class="row" style="gap:0;">
            <div class="col-md-4">
                <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control form-control-sm" placeholder="Buscar pregunta..." />
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
                <asp:DropDownList ID="ddlFiltroClasificacion" runat="server" CssClass="form-control form-control-sm">
                    <asp:ListItem Value="">Todas las clasificaciones</asp:ListItem>
                    <asp:ListItem Value="1">5S + Seguridad</asp:ListItem>
                    <asp:ListItem Value="2">Puesta a punto</asp:ListItem>
                    <asp:ListItem Value="3">Prod. no conforme</asp:ListItem>
                    <asp:ListItem Value="4">4M / P-Y / Eq. Med.</asp:ListItem>
                    <asp:ListItem Value="5">Doctos. / Eq. Med.</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-2">
                <asp:DropDownList ID="ddlFiltroEstatus" runat="server" CssClass="form-control form-control-sm">
                    <asp:ListItem Value="">Activas e inactivas</asp:ListItem>
                    <asp:ListItem Value="1">Solo activas</asp:ListItem>
                    <asp:ListItem Value="0">Solo inactivas</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-1">
                <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn btn-secondary btn-sm btn-block" OnClick="btnFiltrar_Click" />
            </div>
        </div>
    </div>
</div>

<!-- Tabla -->
<div class="card card-mr">
    <div class="card-body p-0">
        <asp:GridView ID="gvPreguntas" runat="server" CssClass="table table-hover mb-0"
            AutoGenerateColumns="false" DataKeyNames="id"
            OnRowCommand="gvPreguntas_RowCommand">
            <Columns>
                <asp:BoundField DataField="Check_id" HeaderText="Check" />
                <asp:TemplateField HeaderText="Clasificación">
                    <ItemTemplate>
                        <%# Eval("Clasificaciones.Clasificacion") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Pregunta" HeaderText="Pregunta" />
                <asp:TemplateField HeaderText="Estatus">
                    <ItemTemplate>
                        <span class='<%# (bool)Eval("Activo") ? "badge badge-activo" : "badge badge-inactivo" %>'>
                            <%# (bool)Eval("Activo") ? "Activa" : "Inactiva" %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CommandName="Editar" CommandArgument='<%# Eval("id") %>'
                            CssClass="btn btn-sm btn-outline-secondary mr-1">
                            <i class="fas fa-edit"></i>
                        </asp:LinkButton>
                        <asp:LinkButton runat="server" CommandName="Toggle" CommandArgument='<%# Eval("id") %>'
                            CssClass='<%# (bool)Eval("Activo") ? "btn btn-sm btn-outline-danger" : "btn btn-sm btn-outline-success" %>'>
                            <i class='<%# (bool)Eval("Activo") ? "fas fa-eye-slash" : "fas fa-eye" %>'></i>
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</div>

<!-- Modal -->
<div class="modal fade" id="modalPregunta" tabindex="-1" role="dialog">
    <div class="modal-dialog modal-lg" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">
                    <asp:Label ID="lblTituloModal" runat="server" Text="Nueva Pregunta" />
                </h5>
                <button type="button" class="close" data-dismiss="modal"><span>&times;</span></button>
            </div>
            <div class="modal-body">
                <asp:HiddenField ID="hfIdPregunta" runat="server" Value="0" />
                <div class="form-group">
                    <label>Check</label>
                    <asp:DropDownList ID="ddlCheck" runat="server" CssClass="form-control">
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
                <div class="form-group">
                    <label>Clasificación</label>
                    <asp:DropDownList ID="ddlClasificacion" runat="server" CssClass="form-control">
                        <asp:ListItem Value="1">5S + Seguridad</asp:ListItem>
                        <asp:ListItem Value="2">Puesta a punto</asp:ListItem>
                        <asp:ListItem Value="3">Prod. no conforme</asp:ListItem>
                        <asp:ListItem Value="4">4M / P-Y / Eq. Med.</asp:ListItem>
                        <asp:ListItem Value="5">Doctos. / Eq. Med.</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="form-group">
                    <label>Texto de la pregunta</label>
                    <asp:TextBox ID="txtPregunta" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancelar</button>
                <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-astemo" OnClick="btnGuardar_Click" />
            </div>
        </div>
    </div>
</div>

<script>
    function limpiarModal() {
        document.getElementById('<%= hfIdPregunta.ClientID %>').value = '0';
        document.getElementById('<%= lblTituloModal.ClientID %>').innerText = 'Nueva Pregunta';
        document.getElementById('<%= txtPregunta.ClientID %>').value = '';
        document.getElementById('<%= ddlCheck.ClientID %>').value = '1A';
        document.getElementById('<%= ddlClasificacion.ClientID %>').value = '1';
    }
</script>

</asp:Content>