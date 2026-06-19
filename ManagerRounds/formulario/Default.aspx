<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ManagerRounds.formulario.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:HiddenField ID="hfRevisionId" runat="server" Value="0" />

    <div style="display:flex; align-items:center; justify-content:space-between; margin-bottom:20px;">
        <div>
            <h4 style="font-weight:600; margin:0;">Formulario del día</h4>
            <asp:Label ID="lblFecha" runat="server" style="color:#888; font-size:13px;" />
        </div>
        <asp:Label ID="lblEstatus" runat="server" CssClass="badge-pendiente" 
            style="padding:4px 12px; border-radius:20px; font-size:12px; font-weight:500;" />
    </div>

    <div style="background:#f0f0f0; border-radius:8px; padding:12px 16px; margin-bottom:20px; display:flex; align-items:center; justify-content:space-between;">
        <span style="font-size:13px; color:#555;">
            <asp:Label ID="lblNombreManager" runat="server" />
        </span>
        <span style="font-size:13px; color:#888;">
            <asp:Label ID="lblRespondidas" runat="server" /> respondidas
        </span>
    </div>

    <!-- Panel normal (lunes a jueves) -->
    <asp:Panel ID="pnlSelectorCheck" runat="server">
        <div style="margin-bottom:20px;">
            <p style="font-size:12px; color:#888; margin:0 0 8px;">Tipo de check — selecciona el que corresponde hoy</p>
            <div style="display:flex; gap:8px;">
                <asp:Button ID="btnCheckA" runat="server" Text="A · Área productiva" 
                    CssClass="btn-check-selector btn-check-active" OnClick="btnCheckA_Click" />
                <asp:Button ID="btnCheckB" runat="server" Text="B · Almacenes / lab." 
                    CssClass="btn-check-selector" OnClick="btnCheckB_Click" />
            </div>
        </div>
    </asp:Panel>

    <!-- Panel viernes recuperación -->
    <asp:Panel ID="pnlRecuperacion" runat="server" Visible="false">
        <div style="margin-bottom:20px;">
            <p style="font-size:12px; color:#888; margin:0 0 8px;">Viernes de recuperación — selecciona un check pendiente</p>
            <div style="display:flex; gap:8px; flex-wrap:wrap;">
                <asp:Repeater ID="rptChecksPendientes" runat="server" OnItemCommand="rptChecksPendientes_ItemCommand">
                    <ItemTemplate>
                        <asp:Button runat="server" Text='<%# Eval("CheckId") %>'
                            CommandName="SeleccionarCheck" CommandArgument='<%# Eval("CheckId") %>'
                            CssClass='<%# (bool)Eval("Completado") ? "btn-check-selector btn-check-completado" : "btn-check-selector" %>' />
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <p style="font-size:11px; color:#bbb; margin:8px 0 0;">Los checks en verde ya fueron completados esta semana.</p>
        </div>
    </asp:Panel>

    <!-- Panel al corriente -->
    <asp:Panel ID="pnlAlCorriente" runat="server" Visible="false">
        <div style="background:#EAF3DE; border-radius:8px; padding:24px; text-align:center; margin-bottom:20px;">
            <i class="fas fa-check-circle" style="font-size:36px; color:#639922; margin-bottom:10px; display:block;"></i>
            <p style="font-size:15px; font-weight:500; color:#3B6D11; margin:0;">Todo al corriente</p>
            <p style="font-size:13px; color:#3B6D11; margin:6px 0 0;">Completaste todos los formularios de la semana.</p>
        </div>
    </asp:Panel>

    <!-- Panel corrigiendo -->
    <asp:Panel ID="pnlCorrigiendo" runat="server" Visible="false">
        <div style="background:#FAEEDA; border-radius:8px; padding:10px 16px; margin-bottom:16px; display:flex; align-items:center; gap:10px;">
            <i class="fas fa-exclamation-triangle" style="color:#854F0B;"></i>
            <span style="font-size:13px; color:#854F0B;">Este formulario fue rechazado. Corrige las respuestas y vuelve a enviar.</span>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlFormulario" runat="server" Visible="false">
        <div style="background:#fff; border:1px solid #e8e8e8; border-radius:8px; padding:8px 20px; margin-bottom:20px;">
            <asp:Repeater ID="rptPreguntas" runat="server">
                <ItemTemplate>
                    <div style="padding:20px 0; border-bottom:1px solid #f0f0f0;">
                        <div style="display:flex; align-items:flex-start; gap:14px;">
                            <div style="width:24px; height:24px; border-radius:50%; background:#CC0000; color:#fff; font-size:11px; font-weight:600; display:flex; align-items:center; justify-content:center; flex-shrink:0; margin-top:2px;">
                                <%# Container.ItemIndex + 1 %>
                            </div>
                            <div style="flex:1;">
                                <p style="font-size:13px; font-weight:500; margin:0 0 12px; color:#222; line-height:1.5;"><%# Eval("Pregunta") %></p>
                                <asp:HiddenField ID="hfPreguntaId" runat="server" Value='<%# Eval("id") %>' />
                                <div style="margin-bottom:12px;">
                                    <asp:RadioButtonList ID="rblRespuesta" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="opciones-respuesta">
                                        <asp:ListItem Value="1" Text="Aprobado" />
                                        <asp:ListItem Value="2" Text="Falla" />
                                        <asp:ListItem Value="3" Text="N/A" />
                                    </asp:RadioButtonList>
                                </div>
                                <div style="background:#f8f8f8; border-radius:6px; padding:12px 14px; margin-top:4px;">
                                    <label style="display:flex; align-items:center; gap:8px; font-size:12px; color:#666; cursor:pointer; margin-bottom:8px;">
                                        <asp:CheckBox ID="chkSinComentario" runat="server" CssClass="chk-sin-comentario" />
                                        Sin comentario
                                    </label>
                                    <asp:TextBox ID="txtComentario" runat="server" 
                                        placeholder="Escribe un comentario..." 
                                        CssClass="form-control txt-comentario" 
                                        style="font-size:12px;" />
                                    <asp:Label ID="lblErrorComentario" runat="server" 
                                        style="color:#CC0000; font-size:11px; display:none;" 
                                        Text="Debes escribir un comentario o marcar 'Sin comentario'" />
                                </div>

                                <!-- Panel hallazgo -->
                                <div class="panel-hallazgo" style="display:none; background:#FFF3CD; border-radius:6px; padding:12px 14px; margin-top:8px; border:1px solid #F0A500;">
                                    <p style="font-size:12px; font-weight:500; color:#7A5100; margin:0 0 8px;">
                                        <i class="fas fa-exclamation-triangle mr-1"></i> Hallazgo — Foto del problema
                                    </p>
                                    <asp:FileUpload ID="fuFotoProblema" runat="server" CssClass="form-control-file"
                                        style="font-size:12px;" accept="image/*" />
                                    <img class="preview-foto" style="display:none; margin-top:8px; width:120px; height:80px; object-fit:cover; border-radius:6px; border:1px solid #F0A500;" />
                                    <p style="font-size:11px; color:#aaa; margin:6px 0 0;">Obligatorio al marcar Falla.</p>

                                    <div style="margin-top:10px; border-top:1px solid #F0A500; padding-top:10px;">
                                        <p style="font-size:12px; font-weight:500; color:#7A5100; margin:0 0 8px;">
                                            <i class="fas fa-check-circle mr-1"></i> Foto de cierre (opcional)
                                        </p>
                                        <asp:FileUpload ID="fuFotoCierre" runat="server" CssClass="form-control-file"
                                            style="font-size:12px;" accept="image/*" />
                                        <img class="preview-cierre" style="display:none; margin-top:8px; width:120px; height:80px; object-fit:cover; border-radius:6px; border:1px solid #639922;" />
                                        <p style="font-size:11px; color:#aaa; margin:6px 0 0;">Si ya tienes la evidencia de solución, súbela aquí.</p>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <div style="display:flex; justify-content:flex-end;">
            <asp:Button ID="btnGuardar" runat="server" Text="Guardar formulario" 
                CssClass="btn-astemo" OnClick="btnGuardar_Click" OnClientClick="return validarFormulario();" />
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlYaCompletado" runat="server" Visible="false">
        <div style="background:#EAF3DE; border-radius:8px; padding:24px; text-align:center;">
            <i class="fas fa-check-circle" style="font-size:36px; color:#639922; margin-bottom:10px; display:block;"></i>
            <p style="font-size:15px; font-weight:500; color:#3B6D11; margin:0;">Formulario completado</p>
            <p style="font-size:13px; color:#3B6D11; margin:6px 0 0;">Ya registraste tu formulario del día.</p>
        </div>
    </asp:Panel>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
<style>
    .btn-check-selector {
        border: 1.5px solid #ddd;
        background: #f0f0f0;
        color: #666;
        border-radius: 6px;
        padding: 8px 20px;
        font-size: 13px;
        font-weight: 500;
        cursor: pointer;
        transition: all 0.15s;
    }
    .btn-check-selector:hover { background: #e0e0e0; color: #333; }
    .btn-check-active { background: #CC0000 !important; color: #fff !important; border-color: #CC0000 !important; }
    .btn-check-completado { background: #EAF3DE !important; color: #3B6D11 !important; border-color: #639922 !important; }
    .opciones-respuesta label { margin-right: 16px; font-size: 13px; cursor: pointer; }
    .opciones-respuesta input[type="radio"] { margin-right: 4px; }
</style>

<script>
    document.addEventListener('change', function (e) {
        // Radio — mostrar/ocultar panel hallazgo
        if (e.target && e.target.type === 'radio') {
            var row = e.target.closest('div[style*="flex:1"]');
            if (!row) return;
            var panelHallazgo = row.querySelector('.panel-hallazgo');
            if (panelHallazgo) {
                var esFalla = e.target.value === '2';
                panelHallazgo.style.display = esFalla ? 'block' : 'none';
                if (!esFalla) {
                    var pf = panelHallazgo.querySelector('.preview-foto');
                    var pc = panelHallazgo.querySelector('.preview-cierre');
                    if (pf) pf.style.display = 'none';
                    if (pc) pc.style.display = 'none';
                }
            }
        }

        // Checkbox — mostrar/ocultar comentario
        if (e.target && e.target.type === 'checkbox') {
            var row = e.target.closest('div[style*="background:#f8f8f8"]');
            if (!row) return;
            var txt = row.querySelector('textarea');
            if (txt) {
                txt.style.display = e.target.checked ? 'none' : 'block';
                txt.required = !e.target.checked;
            }
        }

        // File — preview de foto
        if (e.target && e.target.type === 'file') {
            var panel = e.target.closest('.panel-hallazgo');
            if (!panel) return;
            var isProblema = e.target.id && e.target.id.indexOf('fuFotoProblema') >= 0;
            var preview = panel.querySelector(isProblema ? '.preview-foto' : '.preview-cierre');
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

    function validarFormulario() {
        var valido = true;
        var rows = document.querySelectorAll('div[style*="background:#f8f8f8"]');
        rows.forEach(function (row) {
            var chk = row.querySelector('input[type="checkbox"]');
            var txt = row.querySelector('textarea');
            var lbl = row.querySelector('span[style*="color:#CC0000"]');
            if (chk && txt && !chk.checked && txt.value.trim() === '') {
                valido = false;
                if (lbl) lbl.style.display = 'block';
                txt.style.border = '1px solid #CC0000';
            } else {
                if (lbl) lbl.style.display = 'none';
                if (txt) txt.style.border = '';
            }
        });

        var panelesFalla = document.querySelectorAll('.panel-hallazgo');
        panelesFalla.forEach(function (panel) {
            if (panel.style.display !== 'none') {
                var fileInput = panel.querySelector('input[type="file"]');
                if (fileInput && fileInput.files.length === 0) {
                    valido = false;
                    fileInput.style.border = '1px solid #CC0000';
                    alert('Debes subir una foto del problema para cada Falla.');
                }
            }
        });

        if (!valido && rows.length > 0)
            alert('Debes responder todas las preguntas y escribir un comentario o marcar "Sin comentario".');
        return valido;
    }
</script>
</asp:Content>