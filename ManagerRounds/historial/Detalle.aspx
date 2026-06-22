<%@ Page Title="Detalle de Revisión" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Detalle.aspx.cs" Inherits="ManagerRounds.historial.Detalle" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<style>
    .foto-hallazgo {
        width: 80px;
        height: 60px;
        object-fit: cover;
        border-radius: 6px;
        cursor: pointer;
        border: 1px solid #e8e8e8;
        transition: transform 0.15s;
    }
    .foto-hallazgo:hover { transform: scale(1.08); box-shadow: 0 2px 8px rgba(0,0,0,0.15); }

    .modal-foto {
        display: none;
        position: fixed;
        top: 0; left: 0;
        width: 100%; height: 100%;
        background: rgba(0,0,0,0.85);
        z-index: 9999;
        align-items: center;
        justify-content: center;
    }
    .modal-foto.open { display: flex; }
    .modal-foto img { max-width: 90%; max-height: 90vh; border-radius: 8px; }
    .modal-foto-close { position: absolute; top: 20px; right: 30px; color: #fff; font-size: 28px; cursor: pointer; }
</style>

<div class="d-flex justify-content-between align-items-center mb-3">
    <h4 class="mb-0">Detalle de Revisión</h4>
    <a href="/historial/Default.aspx" class="btn btn-outline-secondary btn-sm">
        <i class="fas fa-arrow-left mr-1"></i> Volver
    </a>
</div>

<asp:Label ID="lblMensaje" runat="server" Visible="false" CssClass="alert d-block mb-3" />

<!-- Info general -->
<div class="card card-mr mb-3" style="padding:0; overflow:hidden;">
    <div style="display:grid; grid-template-columns:2fr 1.5fr 0.8fr 2fr 1fr 1fr;">
        <div style="padding:16px 20px; border-right:1px solid #f0f0f0; background:#f8f8f8;">
            <p style="font-size:11px; color:#aaa; margin:0 0 4px; text-transform:uppercase; letter-spacing:0.05em;">Manager</p>
            <asp:Label ID="lblManager" runat="server" style="font-size:14px; font-weight:600; color:#222;" />
        </div>
        <div style="padding:16px 20px; border-right:1px solid #f0f0f0; background:#f8f8f8;">
            <p style="font-size:11px; color:#aaa; margin:0 0 4px; text-transform:uppercase; letter-spacing:0.05em;">Sección</p>
            <asp:Label ID="lblSeccion" runat="server" style="font-size:14px; color:#444;" />
        </div>
        <div style="padding:16px 20px; border-right:1px solid #f0f0f0; background:#f8f8f8;">
            <p style="font-size:11px; color:#aaa; margin:0 0 4px; text-transform:uppercase; letter-spacing:0.05em;">Check</p>
            <asp:Label ID="lblCheck" runat="server" style="font-size:14px; color:#444;" />
        </div>
        <div style="padding:16px 20px; border-right:1px solid #f0f0f0; background:#f8f8f8;">
            <p style="font-size:11px; color:#aaa; margin:0 0 4px; text-transform:uppercase; letter-spacing:0.05em;">Fecha</p>
            <asp:Label ID="lblFecha" runat="server" style="font-size:13px; color:#444;" />
        </div>
        <div style="padding:16px 20px; border-right:1px solid #f0f0f0; background:#f8f8f8;">
            <p style="font-size:11px; color:#aaa; margin:0 0 4px; text-transform:uppercase; letter-spacing:0.05em;">Calificación</p>
            <asp:Label ID="lblCalificacion" runat="server" style="font-size:14px; font-weight:600; color:#222;" />
            <asp:Label ID="lblHallazgosPendientes" runat="server" style="font-size:11px; color:#F0A500; display:block;" />
        </div>
        <div style="padding:16px 20px; background:#f8f8f8;">
            <p style="font-size:11px; color:#aaa; margin:0 0 4px; text-transform:uppercase; letter-spacing:0.05em;">Estatus</p>
            <asp:Label ID="lblEstatus" runat="server" />
        </div>
    </div>
</div>

<!-- Panel cerrar hallazgos -->
<asp:Panel ID="panelCerrarHallazgos" runat="server" Visible="false">
    <div class="card card-mr mb-3" style="border-left:4px solid #F0A500;">
        <div class="card-body">
            <p style="font-size:14px; font-weight:600; color:#854F0B; margin:0 0 14px;">
                <i class="fas fa-exclamation-triangle mr-1"></i> Hallazgos pendientes de cierre
            </p>
            <asp:Repeater ID="rptHallazgosAbiertos" runat="server" OnItemCommand="rptHallazgosAbiertos_ItemCommand">
                <ItemTemplate>
                    <div style="background:#FAEEDA; border-radius:6px; padding:12px 14px; margin-bottom:10px; display:flex; align-items:center; gap:12px;">
                        <img src='<%# ResolveUrl(Eval("FotoProblema").ToString()) %>'
                            style="width:80px; height:60px; object-fit:cover; border-radius:6px; cursor:pointer; flex-shrink:0;"
                            onclick='<%# "verFoto(\"" + ResolveUrl(Eval("FotoProblema").ToString()) + "\")" %>' />
                        <div style="flex:1;">
                            <p style="font-size:13px; font-weight:500; color:#854F0B; margin:0 0 6px;"><%# Eval("Preguntas.Pregunta") %></p>
                            <p style="font-size:12px; color:#888; margin:0 0 6px;">Sube la evidencia de solución:</p>
                            <asp:FileUpload ID="fuCierreHallazgo" runat="server" CssClass="form-control-file"
                                style="font-size:12px;" accept="image/*" />
                        </div>
                        <asp:LinkButton runat="server" CommandName="CerrarHallazgo"
                            CommandArgument='<%# Eval("id") %>'
                            CssClass="btn btn-astemo btn-sm" style="flex-shrink:0;">
                            <i class="fas fa-check mr-1"></i> Cerrar hallazgo
                        </asp:LinkButton>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:Panel>

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
                <asp:TemplateField HeaderText="Hallazgo">
                    <ItemTemplate>
                        <%# RenderHallazgo(Eval("FotoProblema")?.ToString(), Eval("FotoCierre")?.ToString(), (bool)Eval("HallazgoCerrado"), Eval("TiposRespuesta.Respuesta").ToString()) %>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</div>

<!-- Panel revisor -->
<asp:Panel ID="panelRevisor" runat="server" Visible="false">
    <div class="card card-mr mb-3">
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

<!-- Panel corregir (Manager) -->
<asp:Panel ID="panelCorregir" runat="server" Visible="false">
    <div style="background:#FAEEDA; border-radius:8px; padding:16px 20px; display:flex; align-items:center; justify-content:space-between;">
        <div>
            <p style="font-weight:600; color:#854F0B; margin:0; font-size:14px;">Este formulario fue rechazado</p>
            <p style="color:#854F0B; margin:4px 0 0; font-size:13px;">Puedes corregir las respuestas y reenviarlo.</p>
        </div>
        <asp:Button ID="btnCorregir" runat="server" Text="Corregir formulario" CssClass="btn btn-astemo" />
    </div>
</asp:Panel>

<!-- Modal visor de foto -->
    <div class="modal-foto" id="modalFoto" onclick="cerrarFoto()">
    <span class="modal-foto-close" onclick="cerrarFoto()">&times;</span>
    <img id="imgModalFoto" src="" alt="Hallazgo" />
</div>
<script>
    function verFoto(src) {
        document.getElementById('imgModalFoto').src = src;
        document.getElementById('modalFoto').classList.add('open');
    }
    function cerrarFoto() {
        document.getElementById('modalFoto').classList.remove('open');
    }

    document.addEventListener('change', function (e) {
        if (e.target && e.target.type === 'file') {
            var panel = e.target.closest('.panel-hallazgo');
            if (!panel) return;
            var isProblema = e.target.id && e.target.id.indexOf('fuFotoProblema') >= 0;
            var previewClass = isProblema ? '.preview-foto' : '.preview-cierre';
            var preview = panel.querySelector(previewClass);
            if (!preview) return;
            if (e.target.files && e.target.files[0]) {
                var reader = new FileReader();
                reader.onload = function (ev) {
                    preview.src = ev.target.result;
                    preview.style.display = 'block';
                };
                reader.readAsDataURL(e.target.files[0]);
            } else {
                preview.style.display = 'none';
            }
        }
    });
</script>

</asp:Content>