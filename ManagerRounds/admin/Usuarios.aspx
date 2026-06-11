<%@ Page Title="Usuarios" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Usuarios.aspx.cs" Inherits="ManagerRounds.admin.Usuarios" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<div class="d-flex justify-content-between align-items-center mb-3">
    <h4 class="mb-0">Gestión de Usuarios</h4>
    <button type="button" class="btn btn-astemo" data-toggle="modal" data-target="#modalUsuario" onclick="limpiarModal()">
        <i class="fas fa-plus mr-1"></i> Nuevo Usuario
    </button>
</div>

<asp:Label ID="lblMensaje" runat="server" Visible="false" CssClass="alert d-block mb-3" />

<div class="card card-mr">
    <div class="card-body p-0">
        <asp:GridView ID="gvUsuarios" runat="server" CssClass="table table-hover mb-0"
            AutoGenerateColumns="false" DataKeyNames="id"
            OnRowCommand="gvUsuarios_RowCommand">
            <Columns>
                <asp:BoundField DataField="Nomina" HeaderText="Nómina" />
                <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                <asp:TemplateField HeaderText="Rol">
                    <ItemTemplate><%# Eval("Roles.Rol") %></ItemTemplate>
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
                            CssClass="btn btn-sm btn-outline-secondary mr-1">
                            <i class="fas fa-edit"></i>
                        </asp:LinkButton>
                        <asp:LinkButton runat="server" CommandName="Toggle" CommandArgument='<%# Eval("id") %>'
                            CssClass='<%# (bool)Eval("Activo") ? "btn btn-sm btn-outline-danger" : "btn btn-sm btn-outline-success" %>'>
                            <i class='<%# (bool)Eval("Activo") ? "fas fa-user-slash" : "fas fa-user-check" %>'></i>
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
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
                    <label>Contraseña <small class="text-muted">(dejar vacío para no cambiar)</small></label>
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" MaxLength="50" />
                </div>
                <div class="form-group">
                    <label>Rol</label>
                    <asp:DropDownList ID="ddlRol" runat="server" CssClass="form-control" onchange="toggleSeccion(this.value)">
                        <asp:ListItem Value="1">Manager</asp:ListItem>
                        <asp:ListItem Value="2">Revisor</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div id="panelSeccion">
                    <div class="form-group">
                        <label>Sección</label>
                        <asp:TextBox ID="txtSeccion" runat="server" CssClass="form-control" MaxLength="100" />
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
        document.getElementById('<%= txtPassword.ClientID %>').value = '';
        document.getElementById('<%= ddlRol.ClientID %>').value = '1';
        document.getElementById('<%= txtSeccion.ClientID %>').value = '';
        document.getElementById('<%= ddlTipoArea.ClientID %>').value = '1';
        toggleSeccion('1');
    }
</script>

</asp:Content>