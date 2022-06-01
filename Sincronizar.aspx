<%@ Page Title="" Language="C#" MasterPageFile="~/Principal.Master" AutoEventWireup="true" CodeBehind="Sincronizar.aspx.cs" Inherits="ControleEstoque.Web.Sincronizar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

        $(document).ready(function () {

            $('.curso').mask("9999.9999.999");
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                <h1>Sincronizar</h1>
            </div>
        </div>
        <div class="form-group">
            <div class="col-xs-12 col-sm-12 col-md-offset-4 col-md-4 col-lg-offset-4 col-lg-4">
                <asp:Button ID="btnSincronizarUnidade" runat="server" Text="Sincronizar Unidade" CssClass="btn btn-primary btn-block"
                    OnClick="btnSincronizarUnidade_Click" />
            </div>
        </div>
        <br />
        <div class="form-group">
            <div class="col-xs-12 col-sm-12 col-md-offset-4 col-md-4 col-lg-offset-4 col-lg-4">
                <label id="Label3" class="control-label">Informe o ID do curso: </label>
                <asp:TextBox ID="txtSincronCurso" runat="server" IDValidation2="txtSincronCurso" CssClass="form-control text-center"></asp:TextBox><br />
                <asp:Button ID="btnSincronizarCurso" runat="server" Text="Sincronizar Curso" CssClass="btn btn-primary btn-block" ValidationGroup="." OnClientClick="return ClientValidation('IDValidation2')"
                    OnClick="btnSincronizarCurso_Click" />
            </div>
        </div>
        <br />
        <div class="form-group">
            <div class="col-xs-12 col-sm-12 col-md-offset-4 col-md-4 col-lg-offset-4 col-lg-4">
                <label id="Label1" class="control-label">Informe o ID da turma: </label>
                <asp:TextBox ID="txtSincronTurma" runat="server" IDValidation3="txtSincronTurma" CssClass="form-control text-center "></asp:TextBox><br />
                <asp:Button ID="btnSincronizarTurma" runat="server" Text="Sincronizar Turma" CssClass="btn btn-primary btn-block" ValidationGroup="." OnClientClick="return ClientValidation('IDValidation3')"
                    OnClick="btnSincronizarTurma_Click" />
            </div>
        </div>
        <br />
        <div class="form-group">
            <div class="col-xs-12 col-sm-12 col-md-offset-4 col-md-4 col-lg-offset-4 col-lg-4">
                <label id="Label5" class="control-label">Informe o ID da turma: </label>
                <asp:TextBox ID="txtSicronDisciplina" runat="server" IDValidation4="txtSicronDisciplina" CssClass="form-control text-center"></asp:TextBox><br />
                <asp:Button ID="btnSincronizarDisciplina" runat="server" Text="Sincronizar Disciplina e Instrutor" CssClass="btn btn-primary btn-block" 
                    OnClick="btnSincronizarDisciplina_Click" />
            </div>
        </div>
        <br />
        <div class="form-group">
            <div class="col-xs-12 col-sm-12 col-md-offset-4 col-md-4 col-lg-offset-4 col-lg-4">
                <label id="Label2" class="control-label">Informe o ID do curso: </label>
                <asp:TextBox ID="txtSincronLista" runat="server" IDValidation6="txtSincronLista" CssClass="form-control text-center"></asp:TextBox><br />
                <asp:Button ID="btnSincronizarListaBasica" runat="server" Text="Sincronizar Lista Básica" CssClass="btn btn-primary btn-block" ValidationGroup="." OnClientClick="return ClientValidation('IDValidation6')"
                    OnClick="btnSincronizarListaBasica_Click" />
            </div>
        </div>
    </div>
</asp:Content>
