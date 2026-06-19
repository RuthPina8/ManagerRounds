<%@ Page Title="Preguntas HSE" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Preguntas.aspx.cs" Inherits="ManagerRounds.admin.Preguntas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<style>
    .badge-check-1a { background: #E6F1FB; color: #185FA5; border-radius: 20px; padding: 3px 10px; font-size: 12px; font-weight: 600; }
    .badge-check-1b { background: #EEEDFE; color: #534AB7; border-radius: 20px; padding: 3px 10px; font-size: 12px; font-weight: 600; }
    .badge-check-2a { background: #FAEEDA; color: #854F0B; border-radius: 20px; padding: 3px 10px; font-size: 12px; font-weight: 600; }
    .badge-check-2b { background: #FAEEDA; color: #854F0B; border-radius: 20px; padding: 3px 10px; font-size: 12px; font-weight: 600; }
    .badge-check-3a { background: #FCEBEB; color: #A32D2D; border-radius: 20px; padding: 3px 10px; font-size: 12px; font-weight: 600; }
    .badge-check-3b { background: #FCEBEB; color: #A32D2D; border-radius: 20px; padding: 3px 10px; font-size: 12px; font-weight: 600; }
    .badge-check-4a { background: #EAF3DE; color: #3B6D11; border-radius: 20px; padding: 3px 10px; font-size: 12px; font-weight: 600; }
    .badge-check-4b { background: #EAF3DE; color: #3B6D11; border-radius: 20px; padding: 3px 10px; font-size: 12px; font-weight: 600; }

    .table-preguntas tr:hover td { background: #fff5f5; }
    .table-preguntas tr.inactiva td { opacity: 0.5; }
    .table-preguntas tr.inactiva td .pregunta-texto { text-decoration: line-through; }

    .btn-accion {
        width: 30px; height: 30px;
        border-radius: 6px;
        border: 1px solid #e8e8e8;
        background: #fff;
        color: #888;
        font-size: 14px;
        cursor: pointer;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        transition: all 0.15s;
        margin-right: 4px;
    }

    .btn-accion:hover { background: #f4f6f8; color: #333; }

    .dropdown-check-badge {
        display: inline-block;
        padding: 2px 8px;
        border-radius: 20px;
        font-size: 11px;
        font-weight: 600;
        margin-right: 6px;
    }
</style>

<div class="d-flex justify-content-between align-items-center mb-3">
    <h4 class="mb-0">Preguntas HSE</h4>
    <div class="d-flex" style="gap:8px;">
        <div class="dropdown">
            <button class="btn btn-outline-secondary dropdown-toggle" type="button" id="btnChecks" data-toggle="dropdown">
                <i class="fas fa-sliders-h mr-1"></i> Gestionar checks
            </button>
            <div class="dropdown-menu dropdown-menu-right" style="min-width:280px;">
                <h6 class="dropdown-header" style="font-size:10px; text-transform:uppercase; letter-spacing:0.05em;">Activar / desactivar check</h6>
                <asp:Repeater ID="rptChecks" runat="server" OnItemCommand="rptChecks_ItemCommand">
                    <ItemTemplate>
                        <div class="dropdown-item d-flex justify-content-between align-items-center" style="padding:8px 16px;">
                            <span style="font-size:13px;">
                                <span class="dropdown-check-badge badge-check-<%# Eval("CheckId").ToString().ToLower() %>"><%# Eval("CheckId") %></span>
                                <%# Eval("Descripcion") %>
                            </span>
                            <asp:LinkButton runat="server"
                                CommandName='<%# (bool)Eval("Activo") ? "DesactivarCheck" : "ActivarCheck" %>'
                                CommandArgument='<%# Eval("CheckId") %>'
                                CssClass="btn-accion"
                                title='<%# (bool)Eval("Activo") ? "Desactivar" : "Activar" %>'>
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
        <div class="row">
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
        <asp:GridView ID="gvPreguntas" runat="server" CssClass="table table-preguntas mb-0"
            AutoGenerateColumns="false" DataKeyNames="id"
            OnRowCommand="gvPreguntas_RowCommand"
            OnRowDataBound="gvPreguntas_RowDataBound">
            <Columns>
                <asp:TemplateField HeaderText="Check">
                    <ItemTemplate>
                        <span class='badge-check-<%# Eval("Check_id").ToString().ToLower() %>'><%# Eval("Check_id") %></span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Clasificación">
                    <ItemTemplate>
                        <span style="font-size:12px; color:#888;"><%# Eval("Clasificaciones.Clasificacion") %></span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Pregunta">
                    <ItemTemplate>
                        <span class="pregunta-texto" style="font-size:13px;"><%# Eval("Pregunta") %></span>
                    </ItemTemplate>
                </asp:TemplateField>
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
                            CssClass="btn-accion" title="Editar pregunta">
                            <i class="fas fa-edit"></i>
                        </asp:LinkButton>
                        <asp:LinkButton runat="server" CommandName="Toggle" CommandArgument='<%# Eval("id") %>'
                            CssClass="btn-accion"
                            title='<%# (bool)Eval("Activo") ? "Desactivar pregunta" : "Activar pregunta" %>'>
                            <i class='<%# (bool)Eval("Activo") ? "fas fa-eye-slash" : "fas fa-eye" %>'></i>
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</div>

<!-- Paginación -->
<div class="d-flex justify-content-between align-items-center mt-3">
    <asp:Label ID="lblPaginacion" runat="server" style="font-size:12px; color:#888;" />
    <div style="display:flex; gap:6px;">
        <asp:Button ID="btnPagAnterior" runat="server" Text="← Anterior" CssClass="btn btn-outline-secondary btn-sm" OnClick="btnPagAnterior_Click" />
        <asp:Button ID="btnPagSiguiente" runat="server" Text="Siguiente →" CssClass="btn btn-outline-secondary btn-sm" OnClick="btnPagSiguiente_Click" />
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
                        <asp:ListItem Value="1A">1A — 5S + Seguridad · Área Productiva</asp:ListItem>
                        <asp:ListItem Value="1B">1B — 5S + Seguridad · Almacenes</asp:ListItem>
                        <asp:ListItem Value="2A">2A — Puesta a punto · Área Productiva</asp:ListItem>
                        <asp:ListItem Value="2B">2B — Puesta a punto · Almacenes</asp:ListItem>
                        <asp:ListItem Value="3A">3A — Prod. no conforme · Área Productiva</asp:ListItem>
                        <asp:ListItem Value="3B">3B — Prod. no conforme · Almacenes</asp:ListItem>
                        <asp:ListItem Value="4A">4A — 4M / P-Y · Área Productiva</asp:ListItem>
                        <asp:ListItem Value="4B">4B — Doctos. · Almacenes</asp:ListItem>
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