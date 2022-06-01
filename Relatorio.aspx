<%@ Page Title="" Language="C#" MasterPageFile="~/Principal.Master" AutoEventWireup="true" CodeBehind="Relatorio.aspx.cs" Inherits="ControleEstoque.Web.Relatorio" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function pageLoad() {
            $('[class*=btnSave]').click(function () {
                $("[id*='mdlLoading']").remove();
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="panel-body">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <h1>Relatório</h1>
                </div>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <label>Código da Turma:</label>
            <asp:DropDownList ID="ddlCodTurmaRelatorio" CssClass="form-select2 not-postback " runat="server" OnSelectedIndexChanged="ddlCodTurmaRelatorio_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <label>Disciplina:</label>
            <asp:DropDownList ID="ddlDisciplinaRelatorio" CssClass="form-select2 not-postback " runat="server" OnSelectedIndexChanged="ddlDisciplinaRelatorio_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-3 col-lg-3">
            <label id="Label6" class="control-label">Data Inicial: </label>
            <asp:TextBox ID="txtRetornoPeriodo" IDValidation="txtRetornoPeriodo" CssClass=" form-control text-center" Enabled="false" runat="server"></asp:TextBox>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-3 col-lg-3">
            <label id="Label8" class="control-label">Data Final: </label>
            <asp:TextBox ID="txtRetornoPeriodo1" IDValidation="txtRetornoPeriodo1" CssClass=" form-control text-center" Enabled="false" runat="server"></asp:TextBox>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-3 col-lg-3">
            <label id="Label1" class="control-label">Turno: </label>
            <asp:TextBox ID="txtRetornoTurno" IDValidation="txtRetornoTurno" CssClass=" form-control text-center" Enabled="false" runat="server"></asp:TextBox>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-3 col-lg-3">
            <label id="Label4" class="control-label">Nº Alunos: </label>
            <asp:TextBox ID="txtRetornoNAlunos" IDValidation="txtRetornoNAlunos" CssClass=" form-control text-center" Enabled="false" runat="server"></asp:TextBox>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <label id="Label3" class="control-label">Instrutor: </label>
            <asp:DropDownList ID="ddlInstrutorRelatorio" CssClass="form-select2 not-postback " runat="server" AutoPostBack="true"></asp:DropDownList>
        </div>
    </div>
    <div id="accordion" class="panel-group">
        <div id="collapse1" runat="server" class="panel panel-default">
            <div class="panel-heading" itemid="collapseOne">
                <h4 class="panel-title">
                    <span class="glyphicon glyphicon-chevron-down"></span>&nbsp;Lista Básica
                </h4>
            </div>
            <div id="collapseOne" class="panel-collapse collapse in">
                <div class="panel-body">
                    <p>
                        <input id="filterRelatorio" type="text" class="filter form-control" placeholder="Refine sua pesquisa" />
                    </p>
                    <div class="table-responsive">
                        <asp:GridView ID="grdRelatorio"  AutoGenerateColumns="False" DataKeyNames="id" CssClass="grdRelatorio table table-bordered table-condensed footable metro-blue toggle-arrow-tiny"
                            OnRowCommand="grdRelatorio_RowCommand" OnRowDataBound="grdRelatorio_RowDataBound" data-filter="#filterRelatorio" runat="server" data-page-size="10" data-page-navigation=".pagination" data-limit-navigation="5">
                            <Columns>
                                <asp:TemplateField HeaderText="Produto" HeaderStyle-CssClass="col-lg-7 col-md-7 col-sm-7">
                                    <ItemTemplate>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Qtde" HeaderStyle-CssClass="col-lg-1 col-md-1 col-sm-1">
                                    <ItemTemplate>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="Despesa" DataField="despesaRealizada" ControlStyle-CssClass="col-lg-1 col-md-1 col-sm-1">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Qtde Retorno" DataField="quantidadeRetorno" ControlStyle-CssClass="col-lg-1 col-md-1 col-sm-1">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Valor Retorno" DataField="valorRetorno" ControlStyle-CssClass="col-lg-1 col-md-1 col-sm-1">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                            </Columns>
                            <EmptyDataTemplate>
                                Nenhum registro encontrado.
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <br />
    <div class="form-group">
        <div class="col-xs-12 col-sm-12 col-md-2 col-lg-2 col-md-offset-11">
            <asp:ImageButton ID="btnImprimirSaida" runat="server" OnClick="btnImprimirSaida_Click" ImageUrl="Content/images/print.gif" CssClass="btn btn-default img-responsive img-button btnSave" />
        </div>
    </div>
</asp:Content>
