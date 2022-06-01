<%@ Page Title="" Language="C#" MasterPageFile="~/Principal.Master" AutoEventWireup="true" CodeBehind="Entrada.aspx.cs" Inherits="ControleEstoque.Web.Entrada" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">

        $(function () {
            $(".letra").click(function () {
                var collapse = $(this).parent().parent().attr('id');
                $("#" + collapse + " .letra").removeClass("ativa selecionada");
                $(this).addClass("ativa selecionada");
            });

           $('.campos').blur(function () {
                var data = new Object;
                data.itemid = $(this).attr('itemid');
                data.data = $(this).val();
                var options = {
                    async: true,
                    type: "POST",
                    url: "Entrada.aspx/SalvarEntradaAuxiliar",
                    data: JSON.stringify(data),
                    contentType: "application/json",
                    dataType: "json",
                    success: function (data) {
                        if (data.d != "") {
                            alert(data.d.split('_')[0], data.d.split('_')[1]);
                        }
                    }
                };

                $.ajax(options);
            });
        });

        $(document).ready(function () {
            $('input[name$="txtCadProduto"]').blur(function () { $('input[name$="btnEnter"]').click(); });
            $('input[name$="txtCadProduto"]').autocomplete({
                minLength: 3,
                autoFocus: true,
                source: function (request, response) {
                    $.ajax({
                        url: "Cadastrar.aspx/FindCadastrar",
                        type: "POST",
                        dataType: "json",
                        contentType: "application/json ",
                        data: "{ 'pTexto' : '" + $.trim($('input[name$="txtCadProduto"]').val()) + "' }",
                        dataFilter: function (data) { return data; },
                        success: function (data) {
                            response($.map(data.d, function (item) {
                                return { label: item.toString().split(';')[0] + " - " + item.toString().split(';')[1] + " - " + item.toString().split(';')[3], value: item.toString().split(';')[1] }
                            }))

                        },
                        error: function (jq, status, error) {
                            alert(error);

                        }
                    });
                },
                select: function (request, response) {
                    $('input[name$="txtCadProduto"]').val(response.item.value);
                    var event = $.Event('keypress');
                    event.which = $.ui.keyCode.ENTER;
                    event.keyCode = $.ui.keyCode.ENTER;
                    $('div[id="panEnter"]').trigger(event);
                }
            });
        });

        function teste(obj) {            
            var data = new Object;
            data.itemid = $(obj).attr('itemid');
            data.data = $(obj).val();
            var options = {
                async: true,
                type: "POST",
                url: "Entrada.aspx/SalvarEntradaAuxiliar",
                data: JSON.stringify(data),
                contentType: "application/json",
                dataType: "json",
                success: function (data) {
                    if (data.d != "") {
                        alert(data.d.split('_')[0], data.d.split('_')[1]);
                    }
                }
            };

            $.ajax(options);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                <h1>Entrada</h1>
            </div>
        </div>
    </div>
    <div>
        <asp:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="." />
    </div>
    <div id="accordion" class="panel-group">
        <div class="form-horizontal">
            <div class="form-group">
                <asp:Panel ID="panCodBarra" runat="server" DefaultButton="btnEnter">
                    <div class="col-xs-12 col-sm-12 col-md-4 col-lg-4 ">
                        <label id="lblCodBarra" class="control-label">Código de Barras: </label>
                        <asp:TextBox ID="txtCodBarra" runat="server" IDValidation="txtCodBarra" CssClass="form-control text-center"></asp:TextBox>
                    </div>
                    <asp:Button ID="btnEnter" OnClick="btnEnter_Click" runat="server" Style="display: none" />
                </asp:Panel>
            </div>
        </div>
        <p>
            <input id="filterEntrada" type="text" class="filter form-control" placeholder="Refine sua pesquisa" />
        </p>
        <div class="table-responsive">
            <asp:GridView ID="grdEntrada" runat="server" AutoGenerateColumns="False" DataKeyNames="id" CssClass="grdEntrada table table-bordered table-condensed footable metro-blue toggle-arrow-tiny"
                OnRowCommand="grdEntrada_RowCommand" OnRowDataBound="grdEntrada_RowDataBound" data-filter="#filterEntrada" data-page-size="10" data-page-navigation=".pagination" data-limit-navigation="5">
                <Columns>
                     <asp:TemplateField HeaderText="Código Sei">
                        <ItemTemplate>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Produto">
                        <ItemTemplate>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Data Vencimento">
                        <ItemTemplate>
                            <asp:TextBox ID="txtVencimento" runat="server" ItemId='<%# "txtVencimento_" + Eval("id") %>' CssClass="date form-control text-center campos"></asp:TextBox>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Quantidade">
                        <ItemTemplate>
                            <asp:TextBox ID="txtQuantidade" runat="server" ItemId='<%# "txtQuantidade_" + Eval("id") %>' CssClass="form-control text-center campos"></asp:TextBox>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Localização">
                        <ItemTemplate>
                            <asp:TextBox ID="txtLocalizacao" runat="server" ItemId='<%# "txtLocalizacao_" + Eval("id") %>' CssClass="form-control text-center campos"></asp:TextBox>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Observação"  >
                        <ItemTemplate>
                            <asp:TextBox ID="txtObservacao" Width="1007px" runat="server" ItemId='<%# "txtObservacao_" + Eval("id") %>' onBlur="teste(this)" CssClass="form-control-static sizeTextBox text-left campos"></asp:TextBox>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    Nenhum registro encontrado.
                </EmptyDataTemplate>
            </asp:GridView>
            <div class="col-xs-12 col-sm-12 col-md-3 col-lg-3 col-md-offset-9">
                <asp:Button ID="btnSalvarEntrada" runat="server" Text="Salvar" OnClick="btnSalvarEntrada_Click" CssClass="btn btn-primary btn-block" />
            </div>
        </div>
    </div>
</asp:Content>
