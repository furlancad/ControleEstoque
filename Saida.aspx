<%@ Page Title="" Language="C#" MasterPageFile="~/Principal.Master" AutoEventWireup="true" CodeBehind="Saida.aspx.cs" Inherits="ControleEstoque.Web.Saida" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function pageLoad() {
            $('[class*=btnSave]').click(function () {
                $("[id*='mdlLoading']").remove();
            });
        }
        function LetterMouseOver(btn) {
            if (!$(btn).hasClass("selecionada")) {
                $(btn).addClass("ativa");
            }
        }
        function LetterMouseOut(btn) {
            if (!$(btn).hasClass("selecionada")) {
                $(btn).removeClass("ativa");
            }
        }
        function LetterActive(collapse, letra) {
            $('input[value="A"]').not(collapse > "input").addClass("ativa selecionada");

            if (collapse != '') {
                $.each($(".panel-collapse"), function () { $(this).removeClass("in"); });
                $('div[id="' + collapse + '"]').addClass("in");

                $.each($(".panel-heading > h4 > span"), function () { $(this).removeClass("glyphicon-chevron-down"); $(this).addClass("glyphicon-chevron-right"); });
                $('div[itemid="' + collapse + '"] > h4 > span').removeClass("glyphicon-chevron-right");
                $('div[itemid="' + collapse + '"] > h4 > span').addClass("glyphicon-chevron-down");

                $("#" + collapse + " .letra").removeClass("ativa selecionada");

                if (letra == '#') {
                    $('input[name$="btn_' + collapse + '"]').addClass("ativa selecionada");
                }
                else {
                    $('input[name$="btn_' + collapse + '_' + letra + '"]').addClass("ativa selecionada");
                }
            }
            else {
                $('input[value="A"]').addClass("ativa selecionada");
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                <h1>Saída</h1>
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
                    <asp:DropDownList ID="ddlSaidaCodTurma" CssClass="form-select2 not-postback " runat="server" OnSelectedIndexChanged="ddlSaidaCodTurma_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <label>Disciplina:</label>
                    <asp:DropDownList ID="ddlDisciplinaSaida" CssClass="form-select2 not-postback " runat="server" OnSelectedIndexChanged="ddlDisciplina_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-3 col-lg-3">
                    <label id="Label6" class="control-label">Data Inicial: </label>
                    <asp:TextBox ID="txtPeriodoSaida" IDValidation="txtPeriodoSaida" CssClass=" form-control text-center" Enabled="false" runat="server"></asp:TextBox>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-3 col-lg-3">
                    <label id="Label8" class="control-label">Data Final: </label>
                    <asp:TextBox ID="txtPeriodoSaida1" IDValidation="txtPeriodoSaida1" CssClass=" form-control text-center" Enabled="false" runat="server"></asp:TextBox>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-3 col-lg-3">
                    <label id="Label1" class="control-label">Turno: </label>
                    <asp:TextBox ID="txtTurnoSaida" IDValidation="txtTurnoSaida" CssClass=" form-control text-center" Enabled="false" runat="server"></asp:TextBox>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-3 col-lg-3">
                    <label id="Label4" class="control-label">Nº Alunos: </label>
                    <asp:TextBox ID="txtNAlunosSaida" IDValidation="txtNAlunosSaida" CssClass=" form-control text-center" Enabled="false" runat="server"></asp:TextBox>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-8 col-lg-8 ">
                    <label id="Label5" class="control-label">Aula: </label>
                    <asp:DropDownList ID="ddlAulasSaida" CssClass="form-select2 not-postback text-center " IDValidation="ddlAulasSaida" runat="server" OnSelectedIndexChanged="ddlAulasSaida_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-4 col-lg-4">
                    <label id="Label3" class="control-label">Instrutor: </label>
                    <asp:DropDownList ID="ddlInstrutorSaida" CssClass="form-select2 not-postback " runat="server" OnSelectedIndexChanged="ddlInstrutor_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
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
                   <%--  <asp:Button ID="btn_collapseOne" runat="server" Text="#" CssClass="letra" OnClick="btn_collapseOne_Click" onmouseover="LetterMouseOver(this);" onmouseout="LetterMouseOut(this);" />
                    <asp:Button ID="btn_collapseOne_A" runat="server" Text="A" CssClass="letra" OnClick="btn_collapseOne_Click" onmouseover="LetterMouseOver(this);" onmouseout="LetterMouseOut(this);" />
                    <asp:Button ID="btn_collapseOne_B" runat="server" Text="B" CssClass="letra" OnClick="btn_collapseOne_Click" onmouseover="LetterMouseOver(this);" onmouseout="LetterMouseOut(this);" />
                    <asp:Button ID="btn_collapseOne_C" runat="server" Text="C" CssClass="letra" OnClick="btn_collapseOne_Click" onmouseover="LetterMouseOver(this);" onmouseout="LetterMouseOut(this);" />
                    <asp:Button ID="btn_collapseOne_D" runat="server" Text="D" CssClass="letra" OnClick="btn_collapseOne_Click" onmouseover="LetterMouseOver(this);" onmouseout="LetterMouseOut(this);" />
                    <asp:Button ID="btn_collapseOne_E" runat="server" Text="E" CssClass="letra" OnClick="btn_collapseOne_Click" onmouseover="LetterMouseOver(this);" onmouseout="LetterMouseOut(this);" />
                    <asp:Button ID="btn_collapseOne_F" runat="server" Text="F" CssClass="letra" OnClick="btn_collapseOne_Click" onmouseover="LetterMouseOver(this);" onmouseout="LetterMouseOut(this);" />
                    <asp:Button ID="btn_collapseOne_G" runat="server" Text="G" CssClass="letra" OnClick="btn_collapseOne_Click" onmouseover="LetterMouseOver(this);" onmouseout="LetterMouseOut(this);" />
                    <asp:Button ID="btn_collapseOne_H" runat="server" Text="H" CssClass="letra" OnClick="btn_collapseOne_Click" onmouseover="LetterMouseOver(this);" onmouseout="LetterMouseOut(this);" />
                    <asp:Button ID="btn_collapseOne_I" runat="server" Text="I" CssClass="letra" OnClick="btn_collapseOne_Click" onmouseover="LetterMouseOver(this);" onmouseout="LetterMouseOut(this);" />
                    <asp:Button ID="btn_collapseOne_J" runat="server" Text="J" CssClass="letra" OnClick="btn_collapseOne_Click" onmouseover="LetterMouseOver(this);" onmouseout="LetterMouseOut(this);" />
                    <asp:Button ID="btn_collapseOne_K" runat="server" Text="K" CssClass="letra" OnClick="btn_collapseOne_Click" onmouseover="LetterMouseOver(this);" onmouseout="LetterMouseOut(this);" />
                    <asp:Button ID="btn_collapseOne_L" runat="server" Text="L" CssClass="letra" OnClick="btn_collapseOne_Click" onmouseover="LetterMouseOver(this);" onmouseout="LetterMouseOut(this);" />
                    <asp:Button ID="btn_collapseOne_M" runat="server" Text="M" CssClass="letra" OnClick="btn_collapseOne_Click" onmouseover="LetterMouseOver(this);" onmouseout="LetterMouseOut(this);" />
                    <asp:Button ID="btn_collapseOne_N" runat="server" Text="N" CssClass="letra" OnClick="btn_collapseOne_Click" onmouseover="LetterMouseOver(this);" onmouseout="LetterMouseOut(this);" />
                    <asp:Button ID="btn_collapseOne_O" runat="server" Text="O" CssClass="letra" OnClick="btn_collapseOne_Click" onmouseover="LetterMouseOver(this);" onmouseout="LetterMouseOut(this);" />
                    <asp:Button ID="btn_collapseOne_P" runat="server" Text="P" CssClass="letra" OnClick="btn_collapseOne_Click" onmouseover="LetterMouseOver(this);" onmouseout="LetterMouseOut(this);" />
                    <asp:Button ID="btn_collapseOne_Q" runat="server" Text="Q" CssClass="letra" OnClick="btn_collapseOne_Click" onmouseover="LetterMouseOver(this);" onmouseout="LetterMouseOut(this);" />
                    <asp:Button ID="btn_collapseOne_R" runat="server" Text="R" CssClass="letra" OnClick="btn_collapseOne_Click" onmouseover="LetterMouseOver(this);" onmouseout="LetterMouseOut(this);" />
                    <asp:Button ID="btn_collapseOne_S" runat="server" Text="S" CssClass="letra" OnClick="btn_collapseOne_Click" onmouseover="LetterMouseOver(this);" onmouseout="LetterMouseOut(this);" />
                    <asp:Button ID="btn_collapseOne_T" runat="server" Text="T" CssClass="letra" OnClick="btn_collapseOne_Click" onmouseover="LetterMouseOver(this);" onmouseout="LetterMouseOut(this);" />
                    <asp:Button ID="btn_collapseOne_U" runat="server" Text="U" CssClass="letra" OnClick="btn_collapseOne_Click" onmouseover="LetterMouseOver(this);" onmouseout="LetterMouseOut(this);" />
                    <asp:Button ID="btn_collapseOne_V" runat="server" Text="V" CssClass="letra" OnClick="btn_collapseOne_Click" onmouseover="LetterMouseOver(this);" onmouseout="LetterMouseOut(this);" />
                    <asp:Button ID="btn_collapseOne_W" runat="server" Text="W" CssClass="letra" OnClick="btn_collapseOne_Click" onmouseover="LetterMouseOver(this);" onmouseout="LetterMouseOut(this);" />
                    <asp:Button ID="btn_collapseOne_X" runat="server" Text="X" CssClass="letra" OnClick="btn_collapseOne_Click" onmouseover="LetterMouseOver(this);" onmouseout="LetterMouseOut(this);" />
                    <asp:Button ID="btn_collapseOne_Y" runat="server" Text="Y" CssClass="letra" OnClick="btn_collapseOne_Click" onmouseover="LetterMouseOver(this);" onmouseout="LetterMouseOut(this);" />
                    <asp:Button ID="btn_collapseOne_Z" runat="server" Text="Z" CssClass="letra" OnClick="btn_collapseOne_Click" onmouseover="LetterMouseOver(this);" onmouseout="LetterMouseOut(this);" />
                    <div class="clearfix">&nbsp;</div>--%>
                    <p>
                        <input id="filterSaida" type="text" class="filter form-control" placeholder="Refine sua pesquisa" />
                    </p>
                    <div class="table-responsive">
                        <asp:GridView ID="grdSaida" runat="server" AutoGenerateColumns="False" DataKeyNames="id" CssClass="grdSaida table table-bordered table-condensed footable metro-blue toggle-arrow-tiny"
                            OnRowCommand="grdSaida_RowCommand" OnRowDataBound="grdSaida_RowDataBound" data-filter="#filterSaida" data-page-size="10" data-page-navigation=".pagination" data-limit-navigation="5">
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
                                <asp:BoundField HeaderText="Qtde" DataField="quantidade" HeaderStyle-CssClass="col-lg-1 col-md-1 col-sm-1">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Qtde Extra" HeaderStyle-CssClass="col-lg-1 col-md-1 col-sm-1">
                                    <ItemTemplate>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Justificativa" HeaderStyle-CssClass="col-lg-3 col-md-3 col-sm-3">
                                    <ItemTemplate>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                  <asp:TemplateField HeaderText="Estoque" HeaderStyle-CssClass="col-lg-1 col-md-1 col-sm-1">
                                    <ItemTemplate>
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
    <div id="accordion1" class="panel-group">
        <div id="Div2" runat="server" class="panel panel-default">
            <div class="panel-heading" itemid="collapseOne">
                <h4 class="panel-title">
                    <span class="glyphicon glyphicon-chevron-down"></span>&nbsp;Produto Extra
                </h4>
            </div>
            <div id="Div3" class="panel-collapse collapse in">
                <div class="panel-body">
                    <p>
                        <input id="Text1" type="text" class="filter form-control" placeholder="Refine sua pesquisa" />
                    </p>
                    <div class="table-responsive">
                        <asp:GridView ID="grdExcecao" runat="server" AutoGenerateColumns="False" DataKeyNames="id" CssClass="grdExcecao table table-bordered table-condensed footable metro-blue toggle-arrow-tiny"
                            OnRowCommand="grdExcecao_RowCommand" OnRowDataBound="grdExcecao_RowDataBound" data-filter="#filterExcecao" data-page-size="10" data-page-navigation=".pagination" data-limit-navigation="5">
                            <Columns>
                                <asp:TemplateField HeaderText="Produto">
                                    <ItemTemplate>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Local">
                                    <ItemTemplate>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Medida">
                                    <ItemTemplate>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Qtde">
                                    <ItemTemplate>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Justificativa">
                                    <ItemTemplate>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                 <asp:TemplateField HeaderText="Estoque">
                                    <ItemTemplate>
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
    <div class="form-group">
        <div class="col-xs-12 col-sm-12 col-md-2 col-lg-2 col-md-offset-11">
            <asp:ImageButton ID="btnImprimirSaida" runat="server" OnClick="btnImprimirSaida_Click" ImageUrl="Content/images/print.gif" CssClass="btn btn-default img-responsive img-button btnSave" />
    </div>
    </div>
    <br />
    <br />
    <br />
    <div id="accordion2" class="panel-group">
        <div class="form-horizontal">
            <div id="Div7" runat="server" class="panel panel-default">
                <div class="panel-heading" itemid="collapseOne">
                    <h4 class="panel-title">
                        <span class="glyphicon glyphicon-chevron-down"></span>&nbsp;Realizar Saída
                    </h4>
                </div>
            </div>
            <div class="form-group">
                <asp:Panel ID="panCodBarra" runat="server" DefaultButton="btnEnter">
                    <div class="col-xs-12 col-sm-12 col-md-4 col-lg-4 ">
                        <label id="lblCodBarra" class="control-label">Código de Barras:</label>
                        <asp:TextBox ID="txtCodBarra" runat="server" IDValidation="txtCodBarra" CssClass="form-control text-center"></asp:TextBox>
                    </div>
                    <asp:Button ID="btnEnter" OnClick="btnEnter_Click" runat="server" Style="display: none" />
                </asp:Panel>
            </div>
        </div>
        <p>
            <input id="filterEntrada" type="text" class="filter form-control" placeholder="Refine sua pesquisa" />
        </p>
        <div id="Div4" runat="server" class="">
            <div class="table-responsive">
                <asp:GridView ID="grdRealizarSaida" runat="server" AutoGenerateColumns="False" DataKeyNames="id" CssClass="grdRealizarSaida table table-bordered table-condensed footable metro-blue toggle-arrow-tiny"
                    OnRowCommand="grdRealizarSaida_RowCommand" OnRowDataBound="grdRealizarSaida_RowDataBound" data-filter="#filterPlanejamento" data-page-size="10" data-page-navigation=".pagination" data-limit-navigation="5">
                    <Columns>
                        <asp:TemplateField HeaderText="Produto">
                            <ItemTemplate>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="left" />
                            <ItemStyle HorizontalAlign="left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Local">
                            <ItemTemplate>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="left" />
                            <ItemStyle HorizontalAlign="left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Medida">
                            <ItemTemplate>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Quant">
                            <ItemTemplate>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Quant Extra">
                            <ItemTemplate>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Total">
                            <ItemTemplate>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Estoque">
                            <ItemTemplate>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Justificativa">
                            <ItemTemplate>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Excluir">
                            <ItemTemplate>
                                <asp:ImageButton ID="ibExcluir" runat="server" CommandArgument='<%# Eval("id") %>' CommandName="Excluir" ToolTip="Excluir"
                                            ImageUrl="Content/images/delete.png" OnClientClick="javascript: var resposta = confirm('Deseja realmente excluir este produto?'); if (!resposta) return false;" />
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
        <div class="col-xs-12 col-sm-12 col-md-3 col-lg-3 col-md-offset-9">
            <asp:Button ID="btnFinalizarSaida" runat="server" Text="Salvar" OnClick="btnFinalizarSaida_Click" CssClass="btn btn-primary btn-block" />
        </div>
    </div>
</asp:Content>
