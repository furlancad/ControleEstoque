<%@ Page Title="" Language="C#" MasterPageFile="~/Principal.Master" AutoEventWireup="true" CodeBehind="CadDeposito.aspx.cs" Inherits="ControleEstoque.Web.cadUsuario" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
        $(function () {
            $(".letra").click(function () {
                var collapse = $(this).parent().parent().attr('id');
                $("#" + collapse + " .letra").removeClass("ativa selecionada");
                $(this).addClass("ativa selecionada");
            });

            $('input[name$="txtDataAgendamento"]').blur(function () {
                var data = new Object;
                data.id = jQuery(this).attr('itemid').split('_')[1];
                data.data = jQuery(this).val();
                var options = {
                    async: true,
                    type: "POST",
                    url: "DemissaoGRH.aspx/SalvarDataAgendamento",
                    data: JSON.stringify(data),
                    contentType: "application/json",
                    dataType: "json",
                    success: function (data) {
                        if (data.d != "") {
                            alert(data.d.split('_')[0], data.d.split('_')[1]);
                        }
                    }
                };

                jQuery.ajax(options);
            });
        });

        function Aqui() {
            $('input[id=$"collapseOne"]').click();
            $('input[id="filterColaboradores"]').val("");
            $('input[id="filterColaboradores"]').val("Data");
            return false;
        }

        function AtivarUsuario(id) {
            $('input[itemid^="txtMotivo_"]').css("border", "");

            var motivo = $('input[itemid="txtMotivo_' + id + '"]').val();

            if (motivo != "") {
                var resposta = confirm('A ação irá ativar o usuário. Deseja prosseguir?');
                if (resposta) {
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                $('input[itemid="txtMotivo_' + id + '"]').css("border", "1px solid red");
                alert('Atenção<br/>Informe o motivo antes de efetuar a ação.', 'warning');
                return false;
            }
        }

        function InativarUsuario(id) {
            $('input[itemid^="txtMotivo_"]').css("border", "");
            $('input[itemid^="txtDataAgendamento_"]').css("border", "");

            var motivo = $('input[itemid="txtMotivo_' + id + '"]').val();
            var agendamento = $('input[itemid="txtDataAgendamento_' + id + '"]').val();

            if (motivo != "" && agendamento != "") {
                var resposta = confirm('A ação irá inativar o usuário. Deseja prosseguir?');
                if (resposta) {
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                $('input[itemid="txtMotivo_' + id + '"]').css("border", "1px solid red");
                $('input[itemid="txtDataAgendamento_' + id + '"]').css("border", "1px solid red");
                alert('Atenção<br/>Informe a data e o motivo antes de efetuar a ação.', 'warning');
                return false;
            }
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
                <h1>Cadastro de Depósito</h1>
            </div>
        </div>
        <div class="form-group">
            <div class="col-xs-12 col-sm-12 col-md-7 col-lg-offset-2 col-md-offset-2 col-lg-7">
                <label>Unidade:</label>
                <asp:DropDownList ID="ddlUnidade" CssClass="form-select2 not-postback " runat="server" AutoPostBack="true"></asp:DropDownList>
            </div>
        </div>
        <div class="form-group">
            <div class="col-xs-12 col-sm-12 col-md-7 col-lg-offset-2 col-md-offset-2 col-lg-7">
                <label id="Label6" class="control-label">Depósito: </label>
                <asp:TextBox ID="txtDeposito" IDValidation="txtDeposito" CssClass=" form-control text-center" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="form-group">
            <div class="col-xs-12 col-sm-12  col-md-3 col-lg-offset-4 col-md-offset-4 col-lg-3">
                <asp:Button ID="btnCadDeposito" runat="server" Text="Cadastrar" ValidationGroup="." OnClientClick="return ClientValidation('IDValidation')" OnClick="btnCadDeposito_Click" CssClass="btn btn-primary btn-block" />
            </div>
        </div>
    </div>




    <div>
        <asp:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="." />
    </div>


    <div id="accordion" class="panel-group">
        <div id="collapse1" runat="server" class="panel panel-default">
            <div class="panel-heading" itemid="collapseOne">
                <h4 class="panel-title">
                    <span class="glyphicon glyphicon-chevron-down"></span>&nbsp;Depósito
                </h4>
            </div>
            <div id="collapseOne" class="panel-collapse collapse in">
                <div class="panel-body">
                    <asp:Button ID="btn_collapseOne" runat="server" Text="#" CssClass="letra" OnClick="btn_collapseOne_Click" onmouseover="LetterMouseOver(this);" onmouseout="LetterMouseOut(this);" />
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
                    <div class="clearfix">&nbsp;</div>
                    <p>
                        <input id="filterDeposito" type="text" class="filter form-control" placeholder="Refine sua pesquisa" />
                    </p>
                    <div class="table-responsive">
                        <asp:GridView ID="grdDeposito" runat="server" AutoGenerateColumns="False" DataKeyNames="id" CssClass="grdDeposito table table-bordered table-condensed footable metro-blue toggle-arrow-tiny"
                            OnRowCommand="grdDeposito_RowCommand" OnRowDataBound="grdDeposito_RowDataBound" data-filter="#filterDeposito" data-page-size="10" data-page-navigation=".pagination" data-limit-navigation="5">
                            <Columns>
                                <asp:BoundField HeaderText="Depósito" DataField="deposito">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Status" DataField="status">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Ativar/Inativar">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lkAtivarDesativar" runat="server" CommandName="AtivarDesativar" CommandArgument='<%# Eval("id") %>' CssClass="mylinkbutton glyphicon glyphicon-envelope" ToolTip="Enviar"
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
