<%@ Page Title="" Language="C#" MasterPageFile="~/Principal.Master" AutoEventWireup="true" CodeBehind="Planejamento.aspx.cs" Inherits="ControleEstoque.Web.Planejamento" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(function () {
            $('input[name$="txtItemExtra"]').on('keydown keypress keyup', function (event) {
                if (event.keyCode == '13') {
                    return false;
                }
            });
            $('.footable').bind({
                'footable_row_expanded': function () {
                    $('.footable-row-detail-inner').addClass("col-xs-12 col-sm-12 col-md-12 col-lg-12");
                    $('.footable-row-detail-inner').attr("style", "padding: 0 0 0 0;");

                    $('.footable-row-detail-row').each(function () {
                        $('.footable-row-detail-row').addClass("col-xs-4 col-sm-3 col-md-3 col-lg-4");
                        $('.footable-row-detail-row').attr("style", "padding: 0 0 0 40px;");

                        $('.footable-row-detail-name').addClass("col-xs-12 col-sm-12 col-md-7 col-lg-6");
                        $('.footable-row-detail-name').attr("style", "padding: 0 0 0 50px;");

                        $('.footable-row-detail-value').addClass("col-xs-10 col-sm-10 col-md-6 col-lg-5");
                        $('.footable-row-detail-value').attr("style", "padding: 0 0 0 0;");
                    });
                }
            });

            $('input[name$="txtProdExtra"]').autocomplete({
                minLength: 3,
                autoFocus: true,
                source: function (request, response) {
                    $.ajax({
                        url: "Planejamento.aspx/FindItemExtra",
                        type: "POST",
                        dataType: "json",
                        contentType: "application/json ",
                        data: "{ 'pTexto' : '" + $.trim($('input[name$="txtProdExtra"]').val()) + "' }",
                        //dataFilter: function (data) { return data; },
                        success: function (data) {
                            response($.map(data.d, function (item) {
                                return { label: item.toString().split('|')[0] + " - " + item.toString().split('|')[1], value: item.toString().split('|')[1] }
                            }))

                        },
                        error: function (jq, status, error) {
                            alert(error);

                        }
                    });
                },
                select: function (request, response) {
                    $('input[name$="hdfItemExtra"]').val(response.item.label);
                    var event = $.Event('keypress');
                    event.which = $.ui.keyCode.ENTER;
                    event.keyCode = $.ui.keyCode.ENTER;
                    $('div[id="panEnter"]').trigger(event);
                }
            });
        });

        function extra(obj) {

            console.log($(obj)[0].event);

            var row_id = $(obj).attr('id').split('_')[3];
            var item = $('input[id$="txtItemExtra_' + row_id + '"').eq(1).val();
            var justificar = $('input[id$="txtJustificativa_' + row_id + '"').eq(1).val();
            var turm = $('select[name$="ddlCodTurma"] option[selected="selected"]').val();

            if (item != "" && justificar != "" && turm != "0") {

                var data = new Object;

                data.id = $(obj).attr('itemid').split('_')[1];
                data.item = item;
                data.justificar = justificar;
                data.turm = turm;

                var options = {
                    async: true,
                    type: "POST",
                    url: "Planejamento.aspx/SalvarItemExtra",
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
        $(document).ready(function () {
            $('.curso').mask("9999.9999.999");
        });

        function ext(e) {
            if (e.keyCode == "13") {
                return false;
            }

            return true;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                <h1>Planejamento</h1>
            </div>
        </div>
    </div>
    <div>
        <asp:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="." />
    </div>
    <div class="panel-body">
        <div class="form-horizontal">
            <div class="form-group">
                <div id="divUnidade" runat="server">
                    <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                        <label>Unidade:</label>
                        <asp:DropDownList ID="ddlUnidade" CssClass="form-select2 not-postback " runat="server" OnSelectedIndexChanged="ddlUnidade_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <label>Curso:</label>
                    <asp:DropDownList ID="ddlCurso" CssClass="form-select2 not-postback " runat="server" OnSelectedIndexChanged="ddlCurso_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <label>Código da Turma:</label>
                    <asp:DropDownList ID="ddlCodTurma" CssClass="form-select2 not-postback curso " runat="server" OnSelectedIndexChanged="ddlCodTurma_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <label>Disciplina:</label>
                    <asp:DropDownList ID="ddlDisciplina" CssClass="form-select2 not-postback " runat="server" OnSelectedIndexChanged="ddlDisciplina_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                </div>
                <div id="divSupervisor" runat="server">
                    <div class="clearfix clear">&nbsp;</div>
                    <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                        <label>Instrutor: </label>
                        <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddlInstrutores" SetFocusOnError="True" Display="None" ValidationGroup="." Style="vertical-align: top">&nbsp;</asp:RequiredFieldValidator>--%>
                        <asp:DropDownList ID="ddlInstrutores" IDValidation="ddlInstrutores" runat="server" OnSelectedIndexChanged="ddlInstrutores_SelectedIndexChanged" CssClass="ddlInstrutores form-select2" AutoPostBack="true"></asp:DropDownList>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-2 col-lg-2">
                    <label id="Label6" class="control-label">Data Inicial: </label>
                    <asp:TextBox ID="txtPeriodo" CssClass=" form-control text-center" Enabled="false" runat="server"></asp:TextBox>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-2 col-lg-2">
                    <label id="Label2" class="control-label">Data Final: </label>
                    <asp:TextBox ID="txtPeriodo1" CssClass=" form-control text-center" Enabled="false" runat="server"></asp:TextBox>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-2 col-lg-2">
                    <label id="Label1" class="control-label">Turno: </label>
                    <asp:TextBox ID="txtTurno" CssClass=" form-control text-center" Enabled="false" runat="server"></asp:TextBox>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-3 col-lg-2">
                    <label id="Label4" class="control-label">Nº Alunos: </label>
                    <asp:TextBox ID="txtNAlunos" CssClass=" form-control text-center" Enabled="false" runat="server"></asp:TextBox>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-4 col-lg-4">
                    <label id="Label3" class="control-label">Aula: </label>
                    <asp:DropDownList ID="ddlAulas" CssClass="form-select2 not-postback " OnSelectedIndexChanged="ddlAulas_SelectedIndexChanged" runat="server" AutoPostBack="true"></asp:DropDownList>
                </div>
            </div>
        </div>
    </div>
    <div id="accordion" class="panel-group">
        <p>
            <input id="filterPlanejamento" type="text" class="filter form-control" placeholder="Buscar item" />
        </p>
        <div id="Div2" runat="server" class="">
            <div class="table-responsive">
                <asp:GridView ID="grdPlanejamento" runat="server" AutoGenerateColumns="False" DataKeyNames="idCurso" CssClass="grdPlanejamento table table-bordered table-condensed footable metro-blue toggle-arrow-tiny"
                    OnRowCommand="grdPlanejamento_RowCommand" OnRowDataBound="grdPlanejamento_RowDataBound" data-filter="#filterPlanejamento" data-page-size="10" data-page-navigation=".pagination" data-limit-navigation="5" OnSelectedIndexChanged="grdPlanejamento_SelectedIndexChanged">
                    <Columns>
                        <asp:BoundField HeaderText="Grade" DataField="idGrade">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Produto">
                            <ItemTemplate>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="left" />
                            <ItemStyle HorizontalAlign="left" />
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Medida" DataField="unidMedida">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:BoundField HeaderText="Disponível" DataField="quantEstimada">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Quantidade">
                            <ItemTemplate>
                                <asp:TextBox ID="txtQuantidade" OnTextChanged="txtQuantidade_TextChanged" AutoPostBack="true" itemid='<%# Eval("id") %>' runat="server" CssClass="form-control text-center quantidade numeric"></asp:TextBox>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Saldo">
                            <ItemTemplate>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qtd Extra" ItemStyle-CssClass="glyphicon glyphicon-plus col-md-1 col-lg-1 col-xs-1 col-sm-1">
                            <ItemTemplate>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Quantidade">
                            <ItemTemplate>
                                <asp:TextBox ID="txtItemExtra" runat="server" ItemId='<%# "txtItemExtra_" + Eval("id") %>' onkeydown="return ext(event);" onblur="extra(this);" CssClass="form-control-static itemExtra"></asp:TextBox>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Justificativa">
                            <ItemTemplate>
                                <asp:TextBox ID="txtJustificativa" Width="530px" runat="server" ItemId='<%# "txtJustificativa_" + Eval("id") %>' onkeydown="return ext(event);" onblur="extra(this);" CssClass="form-control-static itemExtra"></asp:TextBox>
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
    <div class="form-horizontal">
        <asp:Panel ID="panEnter" runat="server" DefaultButton="btnItemExtra">
            <div class="form-group">
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <h1>Produto Extra</h1>
                </div>
            </div>
            <div class="form-group">
                <div class="col-xs-12 col-sm-12 col-md-9 col-lg-9">
                    <label id="Label5" class="control-label">Produto: </label>
                    <asp:TextBox ID="txtProdExtra" runat="server" IDValidation="txtProdExtra" placeholder="Buscar Produto" CssClass="form-control text-center" OnClick="btnProdutoExtra_Click"></asp:TextBox>
                    <asp:HiddenField ID="hdfItemExtra" runat="server" />
                </div>
                <asp:Button ID="btnEnter" runat="server" Style="display: none" />
                <div class="col-xs-12 col-sm-12 col-md-3 col-lg-3 ">
                    <label id="lblCodBarra" class="control-label">Quantidade: </label>
                    <asp:TextBox ID="txtQuantExtra" runat="server" IDValidation="txtQuantExtra" CssClass="form-control text-center numeric"></asp:TextBox>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-9 col-lg-9 ">
                    <label id="Label7" class="control-label">Justificativa: </label>
                    <asp:TextBox ID="txtJustificarExtra" runat="server" IDValidation="txtJustificarExtra" CssClass="form-control text-center"></asp:TextBox>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-3 col-lg-3">
                    <label id="Label8" class="control-label">&nbsp;</label>
                    <asp:Button ID="btnItemExtra" runat="server" Text="Cadastrar" ValidationGroup="." OnClientClick="return ClientValidation('IDValidation')"
                        CssClass="btn btn-primary btn-block" OnClick="btnItemExtra_Click" />
                </div>
            </div>
        </asp:Panel>
    </div>
    <div id="Div1" runat="server" class="">
        <div class="table-responsive">
            <asp:GridView ID="grdItemExtra" runat="server" AutoGenerateColumns="False" DataKeyNames="id" CssClass="grdItemExtra table table-bordered table-condensed footable metro-blue toggle-arrow-tiny"
                OnRowCommand="grdItemExtra_RowCommand" OnRowDataBound="grdItemExtra_RowDataBound" data-filter="#filterItemExtra" data-page-size="10" data-page-navigation=".pagination" data-limit-navigation="5" OnSelectedIndexChanged="grdPlanejamento_SelectedIndexChanged">
                <Columns>
                    <asp:BoundField HeaderText="codSei" DataField="codSei">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Produto">
                        <ItemTemplate>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="Quantidade" DataField="quantidade">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="Justificativa" DataField="justificativa">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Excluir">
                        <ItemTemplate>
                            <asp:ImageButton ID="ibEditar" runat="server" CommandArgument='<%# Eval("id") %>' CommandName="Excluir" ToolTip="Excluir"
                                ImageUrl="Content/images/delete.png" />
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
    <%-- </div>
        </div>
    </div>--%>
    <%--CssClass="form-control-static sizeTextBox text-left "--%>
    <%-- CssClass="form-control-static sizeTextBox text-left --%>
</asp:Content>
