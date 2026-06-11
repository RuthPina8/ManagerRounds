<%@ Page Title="Detalle de Revisión" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Detalle.aspx.cs" Inherits="ManagerRounds.historial.Detalle" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<div class="d-flex justify-content-between align-items-center mb-3">
    <h4 class="mb-0">Detalle de Revisión</h4>
    <a href="/historial/Default.aspx" class="btn btn-outline-secondary btn-sm">
        <i class="fas fa-arrow-left mr-1"></i> Volver
    </a>
</div>

<asp:Label ID="lblMensaje" runat="server" Visible="false" CssClass="alert d-block mb-3" />

<!-- Info general -->
<div class="card card-mr mb-3">
    <div class="card-body">
        <div class="row">
            <div class="col-md-3">
                <p class="text-muted mb-1" style="font-size:12px;">Manager</p>
                <asp:Label ID="lblManager" runat="server" style="font-size:14px; font-weight:500;" />
            </div>
            <div class="col-md-3">
                <p class="text-muted mb-1" style="font-size:12px;">Sección</p>
                <asp:Label ID="lblSeccion" runat="server" style="font-size:14px;" />
            </div>
            <div class="col-md-2">
                <p class="text-muted mb-1" style="font-size:12px;">Check</p>
                <asp:Label ID="lblCheck" runat="server" style="font-size:14px;" />
            </div>
            <div class="col-md-2">
                <p class="text-muted mb-1" style="font-size:12px;">Fecha</p>
                <asp:Label ID="lblFecha" runat="server" style="font-size:14px;" />
            </div>
            <div class="col-md-1">
                <p class="text-muted mb-1" style="font-size:12px;">Calificación</p>
                <asp:Label ID="lblCalificacion" runat="server" style="font-size:14px; font-weight:500;" />
            </div>
            <div class="col-md-1">
                <p class="text-muted mb-1" style="font-size:12px;">Estatus</p>
                <asp:Label ID="lblEstatus" runat="server" />
            </div>
        </div>
    </div>
</div>

<!-- Respuestas -->
<div class="card card-mr mb-3">
    <div class="card-body p-0">
        <asp:GridView ID="gvRespuestas" runat="server" CssClass="table table-hover mb-0"
            AutoGenerateColumns="false">
            <Columns>
                <asp:TemplateField HeaderText="#">
                    <ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Pregunta">
                    <ItemTemplate><%# Eval("Preguntas.Pregunta") %></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Respuesta">
                    <ItemTemplate>
                        <span class='<%# GetBadgeRespuesta(Eval("TiposRespuesta.Respuesta").ToString()) %>'>
                            <%# Eval("TiposRespuesta.Respuesta") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Comentario">
                    <ItemTemplate><%# Eval("Comentario") %></ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</div>

<!-- Panel revisor -->
<asp:Panel ID="panelRevisor" runat="server" Visible="false">
    <div class="card card-mr">
        <div class="card-body">
            <p style="font-size:14px; font-weight:500;" class="mb-3">Acción del Revisor</p>
            <div class="form-group">
                <label style="font-size:13px;">Comentario <small class="text-muted">(obligatorio para rechazar)</small></label>
                <asp:TextBox ID="txtComentario" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
            </div>
            <div class="d-flex" style="gap:8px;">
                <asp:Button ID="btnAprobar" runat="server" Text="Aprobar" CssClass="btn btn-success" OnClick="btnAprobar_Click" />
                <asp:Button ID="btnRechazar" runat="server" Text="Rechazar" CssClass="btn btn-danger" OnClick="btnRechazar_Click" />
            </div>
        </div>
    </div>
</asp:Panel>

</asp:Content>
