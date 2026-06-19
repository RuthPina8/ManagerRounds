<%@ Page Title="Secciones" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Secciones.aspx.cs" Inherits="ManagerRounds.admin.Secciones" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<style>
    .table-secciones tr:hover td { background: #fff5f5; }
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
        <h4 style="font-weight:600; margin:0;">Gestión de Secciones</h4>
        <p style="font-size:13px; color:#888; margin:4px 0 0;">Áreas de planta asignables a los managers</p>
    </div>
    <button type="button" class="btn btn-astemo" data-toggle="modal" data-target="#modalSeccion" onclick="limpiarModal()">
        <i class="fas fa-plus mr-1"></i> Nueva Sección
    </button>
</div>

<asp:Label ID="lblMensaje" runat="server" Visible="false" CssClass="alert d-block mb-3" />

<div class="card card-mr">
    <div class="card-body p-0">
        <asp:GridView ID="gvSecciones" runat="server" CssClass="table table-secciones mb-0"
            AutoGenerateColumns="false" DataKeyNames="id"
            OnRowCommand="gvSecciones_RowCommand">
            <Columns>
                <asp:TemplateField HeaderText="Nombre">
                    <ItemTemplate>
                        <span style="font-weight:500; font-size:13px;"><%# Eval("Nombre") %></span>
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
                            CssClass="btn-accion" title="Editar sección">
                            <i class="fas fa-edit"></i>
                        </asp:LinkButton>
                        <asp:LinkButton runat="server" CommandName="Toggle" CommandArgument='<%# Eval("id") %>'
                            CssClass="btn-accion"
                            title='<%# (bool)Eval("Activo") ? "Desactivar sección" : "Activar sección" %>'>
                            <i class='<%# (bool)Eval("Activo") ? "fas fa-eye-slash" : "fas fa-eye" %>'></i>
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</div>

<!-- Modal -->
<div class="modal fade" id="modalSeccion" tabindex="-1" role="dialog">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">
                    <asp:Label ID="lblTituloModal" runat="server" Text="Nueva Sección" />
                </h5>
                <button type="button" class="close" data-dismiss="modal"><span>&times;</span></button>
            </div>
            <div class="modal-body">
                <asp:HiddenField ID="hfIdSeccion" runat="server" Value="0" />
                <div class="form-group">
                    <label>Nombre de la sección</label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" MaxLength="100" placeholder="Ej. Línea 1, Almacén MP..." />
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
        document.getElementById('<%= hfIdSeccion.ClientID %>').value = '0';
        document.getElementById('<%= lblTituloModal.ClientID %>').innerText = 'Nueva Sección';
        document.getElementById('<%= txtNombre.ClientID %>').value = '';
    }
</script>

</asp:Content>