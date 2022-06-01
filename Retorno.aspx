<%@ Page Title="" Language="C#" MasterPageFile="~/Principal.Master" AutoEventWireup="true" CodeBehind="Retorno.aspx.cs" Inherits="ControleEstoque.Web.Retorno" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                <h1>Retorno</h1>
            </div>
        </div>
    </div>
    <div>
        <asp:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="." />
    </div>
    <div class="panel-body">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <label>Código da Turma:</label>
                    <asp:DropDownList ID="ddlRetornoCodTurma" CssClass="form-select2 not-postback " OnSelectedIndexChanged="ddlRetornoCodTurma_SelectedIndexChanged" runat="server" AutoPostBack="true"></asp:DropDownList>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <label>Disciplina:</label>
                    <asp:DropDownList ID="ddlRetornoDisciplina" CssClass="form-select2 not-postback " runat="server" OnSelectedIndexChanged="ddlRetornoDisciplina_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
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
                <div class="col-xs-12 col-sm-12 col-md-8 col-lg-8 ">
                    <label id="Label5" class="control-label">Aula: </label>
                    <asp:DropDownList ID="ddlRetornoAula" CssClass="form-select2 not-postback text-center " IDValidation="ddlRetornoAula" runat="server" AutoPostBack="true"></asp:DropDownList>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-4 col-lg-4">
                    <label id="Label3" class="control-label">Instrutor: </label>
                    <asp:DropDownList ID="ddlRetornoInstrutor" CssClass="form-select2 not-postback " runat="server" AutoPostBack="true"></asp:DropDownList>
                </div>
            </div>
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
                        <input id="filterSaida" type="text" class="filter form-control" placeholder="Refine sua pesquisa" />
                    </p>
                    <div class="table-responsive">
                        <asp:GridView ID="grdRetornoSaida" runat="server" AutoGenerateColumns="False" DataKeyNames="id" CssClass="grdRetornoSaida table table-bordered table-condensed footable metro-blue toggle-arrow-tiny"
                            OnRowCommand="grdRetornoSaida_RowCommand" OnRowDataBound="grdRetornoSaida_RowDataBound" data-filter="#filterSaida" data-page-size="10" data-page-navigation=".pagination" data-limit-navigation="5">
                            <Columns>
                                <asp:TemplateField HeaderText="Produto" HeaderStyle-CssClass="col-lg-4 col-md-4 col-sm-4">
                                    <ItemTemplate>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Local" HeaderStyle-CssClass="col-lg-1 col-md-1 col-sm-1">
                                    <ItemTemplate>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Medida" HeaderStyle-CssClass="col-lg-1 col-md-1 col-sm-1">
                                    <ItemTemplate>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Qtde" HeaderStyle-CssClass="col-lg-1 col-md-1 col-sm-1">
                                    <ItemTemplate>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Qtde Retorno">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtqtdRetorno" runat="server" ItemId='<%# "txtqtdRetorno_" + Eval("id") %>' CssClass="form-control-static text-center campos"></asp:TextBox>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="Retorno" DataField="quantidadeRetorno">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Confirmar">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="retornoProduto" runat="server" CommandArgument='<%# Eval("id") %>' CssClass="mylinkbutton glyphicon glyphicon-check" CommandName="Retorno" ToolTip="Retorno"
                                            OnClientClick="javascript: var resposta = confirm('Você deseja realmente proseguir?'); if (!resposta) return false;"></asp:LinkButton>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
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
</asp:Content>
