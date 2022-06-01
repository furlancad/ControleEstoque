<%@ Page Title="" Language="C#" MasterPageFile="~/Principal.Master" AutoEventWireup="true" CodeBehind="CadProduto.aspx.cs" Inherits="ControleEstoque.Web.CadProduto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">



        $(document).ready(function () {

            $('input[name$="txtCadProduto"]').autocomplete({
                minLength: 3,
                autoFocus: true,
                source: function (request, response) {
                    $.ajax({
                        url: "CadProduto.aspx/FindCadastrar",
                        type: "POST",
                        dataType: "json",
                        contentType: "application/json ",
                        data: "{ 'pTexto' : '" + $.trim($('input[name$="txtCadProduto"]').val()) + "' }",
                        dataFilter: function (data) { return data; },
                        success: function (data) {
                            response($.map(data.d, function (item) {
                                //var texto = item.toString().split('|')[0] + " - " + item.toString().split(';')[1].replace("-"," ") + " - " + item.toString().split(';')[2] + " - " + item.toString().split(';')[3];
                                var texto = item.toString();
                                return { label: texto, value: texto.split('|')[1] }
                            }))

                        },
                        error: function (jq, status, error) {
                            alert(error);

                        }
                    });
                },
                select: function (request, response) {
                    //$('input[name$="txtCadProduto"]').val(response.item.value);
                    $('input[name$="hdfCadProduto"]').val(response.item.label);
                    var event = $.Event('keypress');
                    event.which = $.ui.keyCode.ENTER;
                    event.keyCode = $.ui.keyCode.ENTER;
                    $('div[id="panEnter"]').trigger(event);
                }
            });

            $('input[name$="txtCodBarra"]').on("blur", function () {
                if ($(this).val() != "") {

                    var data = new Object;
                    data.data = $(this).val();

                    var options = {
                        async: true,
                        type: "POST",
                        url: "CadProduto.aspx/FindCodBarra",
                        data: JSON.stringify(data),
                        contentType: "application/json",
                        dataType: "json",
                        success: function (data) {
                            if (data.d != "") {

                                alert(data.d, "warning");
                            }
                        }
                    };

                    $.ajax(options);
                }
               

            });

        });

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
                <h1>Cadastro de Produtos</h1>
            </div>
        </div>
        <div class="form-group">
            <asp:Panel ID="panEnter" runat="server" DefaultButton="btnEnter">
                <div class="col-xs-12 col-sm-12 col-md-3 col-lg-3 ">
                    <label id="lblCodBarra" class="control-label">Código de Barras: </label>
                    <asp:TextBox ID="txtCodBarra" runat="server" IDValidation="txtCodBarra" CssClass="form-control text-center"></asp:TextBox>
                </div>

                <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                    <label id="Label5" class="control-label">Produto: </label>
                    <asp:TextBox ID="txtCadProduto" runat="server" IDValidation="txtCadProduto" placeholder="Buscar Produto" CssClass="form-control text-center"></asp:TextBox>
                    <asp:HiddenField ID="hdfCadProduto" runat="server" />
                </div>
                <asp:Button ID="btnEnter" OnClick="btnEnter_Click" runat="server" Style="display: none" />

                <div class="col-xs-12 col-sm-12 col-md-3 col-lg-3">
                    <label id="Label1" class="control-label">&nbsp;</label>
                    <asp:Button ID="btnCadProduto" runat="server" Text="Cadastrar" ValidationGroup="." OnClientClick="return ClientValidation('IDValidation')"
                        OnClick="btnCadProduto_Click" CssClass="btn btn-primary btn-block" />
                </div>
            </asp:Panel>
        </div>
    </div>

    <div id="accordion" class="panel-group">
        <div id="collapse1" runat="server" class="panel panel-default">
            <div class="panel-heading" itemid="collapseOne">
                <h4 class="panel-title">
                    <span class="glyphicon glyphicon-chevron-down"></span>&nbsp;Produtos
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
                        <input id="filterCadProduto" type="text" class="filter form-control" placeholder="Refine sua pesquisa" />
                    </p>
                    <div class="table-responsive">
                        <asp:GridView ID="grdCadProduto" runat="server" AutoGenerateColumns="False" DataKeyNames="id" CssClass="grdCadProduto table table-bordered table-condensed footable metro-blue toggle-arrow-tiny"
                            OnRowCommand="grdCadProduto_RowCommand" OnRowDataBound="grdCadProduto_RowDataBound" data-filter="#filterCadProduto" data-page-size="10" data-page-navigation=".pagination" data-limit-navigation="5">
                            <Columns>
                                <asp:BoundField HeaderText="Código de Barras" DataField="codBarra">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Código SEI" DataField="codSei">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Produto" DataField="produto">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Unid. Medida" DataField="unidMedida">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Valor Unitário (R$)" DataField="valorUnitario">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <%-- <asp:TemplateField HeaderText="Ativar/Inativar">
                            <ItemTemplate>
                                <asp:LinkButton ID="lkAtivarDesativar" runat="server" CommandName="AtivarDesativar" CommandArgument='<%# Eval("ID") %>'></asp:LinkButton>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>--%>
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
