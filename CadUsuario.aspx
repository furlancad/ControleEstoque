<%@ Page Title="" Language="C#" MasterPageFile="~/Principal.Master" AutoEventWireup="true" CodeBehind="CadUsuario.aspx.cs" Inherits="ControleEstoque.Web.CadUsuario" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
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

        $(document).ready(function () {

            $('input[name$="txtUsuario"]').autocomplete({
                minLength: 3,
                autoFocus: true,
                source: function (request, response) {
                    $.ajax({
                        url: "CadUsuario.aspx/FindUsuario",
                        type: "POST",
                        dataType: "json",
                        contentType: "application/json ",
                        data: "{ 'pTexto' : '" + $.trim($('input[name$="txtUsuario"]').val()) + "' }",
                        dataFilter: function (data) { return data; },
                        success: function (data) {
                            response($.map(data.d, function (item) {
                                var texto = item.toString();
                                return { label: texto.split('|')[1], value: texto.split('|')[1] }
                            }))

                        },
                        error: function (jq, status, error) {
                            alert(error);

                        }
                    });
                },
                select: function (request, response) {
                    $('input[name$="hdfCadUsuario"]').val(response.item.label);
                    //var event = $.Event('keypress');
                    //event.which = $.ui.keyCode.ENTER;
                    //event.keyCode = $.ui.keyCode.ENTER;
                    //$('div[id="panEnter"]').trigger(event);
                }
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                <h1>Cadastro de Usuários</h1>
            </div>
        </div>
        <div class="form-group">
            <div class="col-xs-12 col-sm-12 col-md-6 col-lg-offset-2 col-md-offset-2 col-lg-6">
                <label>Depósito:</label>
                <asp:DropDownList ID="ddlDeposito" CssClass="form-select2 not-postback " runat="server" AutoPostBack="true"></asp:DropDownList>
            </div>
        </div>
        <div class="form-group">
            <div class="col-xs-12 col-sm-12 col-md-6 col-lg-offset-2 col-md-offset-2 col-lg-6">
                <label>Usuário:</label>
                <asp:DropDownList ID="ddlUsuario" CssClass="form-select2 not-postback " runat="server" AutoPostBack="true"></asp:DropDownList>
            </div>
        </div>
       <%-- <div class="form-group">
            <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6 col-lg-offset-2 col-md-offset-2">
                <label id="Label2" class="control-label">Usuário: </label>
                <asp:TextBox ID="txtUsuario" IDValidation="txtUsuario" placeholder="Buscar Usuário" CssClass=" form-control text-center " runat="server"></asp:TextBox>
                <%-- <asp:HiddenField ID="hdfCadUsuario" runat="server" />
            </div>
        </div>--%>
        <div class="form-group">
            <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6 col-lg-offset-2 col-md-offset-2">
                <label id="Label7" class="control-label">CPF: </label>
                <asp:TextBox ID="txtCPF" IDValidation="txtCPF" CssClass=" form-control text-center " runat="server" MaxLength="11"></asp:TextBox>
            </div>
        </div>
        <div class="form-group">
            <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6 col-lg-offset-2 col-md-offset-2">
                <label id="Label1" class="control-label">Cargo: </label>
                <asp:TextBox ID="txtCargo" IDValidation="txtCargo" CssClass=" form-control text-center" runat="server"></asp:TextBox>
            </div>
        </div>
        <asp:HiddenField ID="hdfUsuarioid" runat="server" />
        <div class="form-group">
            <div class="col-xs-12 col-sm-12  col-md-3 col-lg-offset-5 col-md-offset-5 col-lg-3">
                <asp:Button ID="btnCadUsuario" runat="server" Text="Cadastrar" CssClass="btn btn-primary btn-block"
                    ValidationGroup="." OnClientClick="return ClientValidation('IDValidation')" OnClick="btnCadUsuario_Click" />
            </div>
        </div>

    </div>

    <div id="accordion" class="panel-group">
        <div id="collapse1" runat="server" class="panel panel-default">
            <div class="panel-heading" itemid="collapseOne">
                <h4 class="panel-title">
                    <span class="glyphicon glyphicon-chevron-down"></span>&nbsp;Usuários
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
                        <input id="filterUsuario" type="text" class="filter form-control" placeholder="Refine sua pesquisa" />
                    </p>
                    <div class="table-responsive">
                        <asp:GridView ID="grdUsuario" runat="server" AutoGenerateColumns="False" DataKeyNames="id" CssClass="grdUsuario table table-bordered table-condensed footable metro-blue toggle-arrow-tiny"
                            OnRowCommand="grdUsuario_RowCommand" OnRowDataBound="grdUsuario_RowDataBound" data-filter="#filterUsuario" data-page-size="10" data-page-navigation=".pagination" data-limit-navigation="5">
                            <Columns>
                                <asp:TemplateField HeaderText="Depósito">
                                    <ItemTemplate>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="Usuário" DataField="usuario">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="CPF" DataField="cpf">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Cargo" DataField="cargo">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Editar">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ibEditar" runat="server" CommandArgument='<%# Eval("id") %>' CommandName="Editar" ToolTip="Editar"
                                            ImageUrl="Content/images/edit.png" />
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
