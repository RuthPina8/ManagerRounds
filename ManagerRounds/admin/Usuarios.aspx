<%@ Page Title="Usuarios" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Usuarios.aspx.cs" Inherits="ManagerRounds.admin.Usuarios" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<style>
    .table-usuarios tr:hover td { background: #fff5f5; }

    .badge-rol-manager { background: #FAEEDA; color: #854F0B; border-radius: 20px; padding: 3px 10px; font-size: 12px; font-weight: 500; }
    .badge-rol-revisor { background: #EEEDFE; color: #534AB7; border-radius: 20px; padding: 3px 10px; font-size: 12px; font-weight: 500; }
    .badge-rol-admin   { background: #E6F1FB; color: #185FA5; border-radius: 20px; padding: 3px 10px; font-size: 12px; font-weight: 500; }

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
</style>

<div class="d-flex justify-content-between align-items-center mb-3">
    <div>
        <h4 style="font-weight:600; margin:0;">Gestión de Usuarios</h4>
        <p style="font-size:13px; color:#888; margin:4px 0 0;">Managers, revisores y administradores del sistema</p>
    </div>
    <button type="button" class="btn btn-astemo" data-toggle="modal" data-target="#modalUsuario" onclick="limpiarModal()">
        <i class="fas fa-plus mr-1"></i> Nuevo Usuario
    </button>
</div>

<asp:Label ID="lblMensaje" runat="server" Visible="false" CssClass="alert d-block mb-3" />

<div class="card card-mr">
    <div class="card-body p-0">
        <asp:GridView ID="gvUsuarios" runat="server" CssClass="table table-usuarios mb-0"
            AutoGenerateColumns="false" DataKeyNames="id"
            OnRowCommand="gvUsuarios_RowCommand">
            <Columns>
                <asp:TemplateField HeaderText="Nómina">
                    <ItemTemplate>
                        <span style="font-size:12px; color:#888; font-family:monospace;"><%# Eval("Nomina") %></span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Nombre">
                    <ItemTemplate>
                        <span style="font-weight:500; font-size:13px;"><%# Eval("Nombre") %></span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Correo">
                    <ItemTemplate>
                        <span style="font-size:12px; color:#888;"><%# Eval("Email") ?? "—" %></span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Rol">
                    <ItemTemplate>
                        <span class='<%# GetBadgeRol(Eval("Roles.Rol").ToString()) %>'><%# Eval("Roles.Rol") %></span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Estatus">
                    <ItemTemplate>
                        <span class='<%# (bool)Eval("Activo") ? "badge badge-activo" : "badge badge-inactivo" %>'>
                            <%# (bool)Eval("Activo") ? "Activo" : "Inactivo" %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CommandName="Editar" CommandArgument='<%# Eval("id") %>'
                            CssClass="btn-accion" title="Editar usuario">
                            <i class="fas fa-edit"></i>
                        </asp:LinkButton>
                        <asp:LinkButton runat="server" CommandName="Toggle" CommandArgument='<%# Eval("id") %>'
                            CssClass="btn-accion"
                            title='<%# (bool)Eval("Activo") ? "Desactivar usuario" : "Activar usuario" %>'>
                            <i class='<%# (bool)Eval("Activo") ? "fas fa-user-slash" : "fas fa-user-check" %>'></i>
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</div>

<div class="d-flex justify-content-between align-items-center mt-3">
    <asp:Label ID="lblPaginacion" runat="server" style="font-size:12px; color:#888;" />
    <div style="display:flex; gap:6px;">
        <asp:Button ID="btnPagAnterior" runat="server" Text="← Anterior" CssClass="btn btn-outline-secondary btn-sm" OnClick="btnPagAnterior_Click" />
        <asp:Button ID="btnPagSiguiente" runat="server" Text="Siguiente →" CssClass="btn btn-outline-secondary btn-sm" OnClick="btnPagSiguiente_Click" />
    </div>
</div>

<!-- Modal -->
<div class="modal fade" id="modalUsuario" tabindex="-1" role="dialog">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">
                    <asp:Label ID="lblTituloModal" runat="server" Text="Nuevo Usuario" />
                </h5>
                <button type="button" class="close" data-dismiss="modal"><span>&times;</span></button>
            </div>
            <div class="modal-body">
                <asp:HiddenField ID="hfIdUsuario" runat="server" Value="0" />
                <div class="form-group">
                    <label>Nombre completo</label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" MaxLength="100" />
                </div>
                <div class="form-group">
                    <label>Nómina</label>
                    <asp:TextBox ID="txtNomina" runat="server" CssClass="form-control" MaxLength="20" />
                </div>
                <div class="form-group">
                    <label>Correo electrónico</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" MaxLength="100" placeholder="nombre@astemo.com" />
                </div>
                <div class="form-group">
                    <label>Contraseña <small class="text-muted">(dejar vacío para no cambiar)</small></label>
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" MaxLength="50" />
                </div>
                <div class="form-group">
                    <label>Rol</label>
                    <asp:DropDownList ID="ddlRol" runat="server" CssClass="form-control" onchange="toggleSeccion(this.value)">
                        <asp:ListItem Value="1">Manager</asp:ListItem>
                        <asp:ListItem Value="2">Revisor</asp:ListItem>
                        <asp:ListItem Value="5">Admin</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div id="panelSeccion">
                    <div class="form-group">
                        <label>Sección</label>
                        <asp:DropDownList ID="ddlSeccion" runat="server" CssClass="form-control"></asp:DropDownList>
                    </div>
                    <div class="form-group">
                        <label>Tipo de área</label>
                        <asp:DropDownList ID="ddlTipoArea" runat="server" CssClass="form-control">
                            <asp:ListItem Value="1">Área Productiva</asp:ListItem>
                            <asp:ListItem Value="2">Almacenes / lab.</asp:ListItem>
                        </asp:DropDownList>
                    </div>
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
    function toggleSeccion(rolId) {
        var panel = document.getElementById('panelSeccion');
        panel.style.display = rolId == '1' ? 'block' : 'none';
    }

    function limpiarModal() {
        document.getElementById('<%= hfIdUsuario.ClientID %>').value = '0';
        document.getElementById('<%= lblTituloModal.ClientID %>').innerText = 'Nuevo Usuario';
        document.getElementById('<%= txtNombre.ClientID %>').value = '';
        document.getElementById('<%= txtNomina.ClientID %>').value = '';
        document.getElementById('<%= txtEmail.ClientID %>').value = '';
        document.getElementById('<%= txtPassword.ClientID %>').value = '';
        document.getElementById('<%= ddlRol.ClientID %>').value = '1';
        document.getElementById('<%= ddlTipoArea.ClientID %>').value = '1';
        toggleSeccion('1');
    }
</script>

</asp:Content>