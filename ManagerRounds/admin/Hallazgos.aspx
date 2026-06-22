<%@ Page Title="Hallazgos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Hallazgos.aspx.cs" Inherits="ManagerRounds.admin.Hallazgos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<div class="d-flex justify-content-between align-items-center mb-3">
    <div>
        <h4 style="font-weight:600; margin:0;">Hallazgos</h4>
        <p style="font-size:13px; color:#888; margin:4px 0 0;">Resumen de hallazgos por manager</p>
    </div>
    <div style="display:flex; align-items:center; gap:8px;">
        <asp:DropDownList ID="ddlFiltro" runat="server" CssClass="form-control form-control-sm" AutoPostBack="true" OnSelectedIndexChanged="ddlFiltro_Changed" style="width:160px;">
            <asp:ListItem Value="semana" Selected="True">Esta semana</asp:ListItem>
            <asp:ListItem Value="mes">Este mes</asp:ListItem>
        </asp:DropDownList>
    </div>
</div>

<div class="card card-mr">
    <div class="card-body p-0">
        <asp:GridView ID="gvHallazgos" runat="server" CssClass="table table-hover mb-0"
            AutoGenerateColumns="false" GridLines="None">
            <Columns>
                <asp:TemplateField HeaderText="Manager">
                    <ItemTemplate>
                        <span style="font-weight:500; font-size:13px;"><%# Eval("Nombre") %></span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Sección">
                    <ItemTemplate>
                        <span style="font-size:12px; color:#888;"><%# Eval("Seccion") %></span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Abiertos">
                    <ItemTemplate>
                        <%# (int)Eval("Abiertos") > 0 
                            ? "<span style='background:#FAEEDA; color:#854F0B; padding:3px 10px; border-radius:20px; font-size:12px; font-weight:600;'>" + Eval("Abiertos") + "</span>"
                            : "<span style='color:#ccc; font-size:12px;'>0</span>" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Cerrados">
                    <ItemTemplate>
                        <span style="background:#EAF3DE; color:#3B6D11; padding:3px 10px; border-radius:20px; font-size:12px; font-weight:600;"><%# Eval("Cerrados") %></span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Total">
                    <ItemTemplate>
                        <span style="font-size:12px; color:#888;"><%# Eval("Total") %></span>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</div>

</asp:Content>