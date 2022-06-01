<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ControleEstoque.Web.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta name="description" content="Controle de Estoque" />
    <meta name="author" content="CE" />
    <title>Controle de Estoque</title>
    <link href="Content/images/favicon1.ico" rel="shortcut icon" type="image/x-icon" />
    <link href="Content/bootstrap.css" rel="stylesheet" />
    <link href="Content/login.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="inner">
            <div class="form-group">
                <div class="form-header">
                    <img alt="" src="Content/images/logo_sistema1.png" class="img-responsive" />
                </div>
            </div>

            <div class="clearfix">&nbsp;</div>

            <div class="form-group">
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" placeholder="Usuário" required="required"></asp:TextBox>
                </div>
            </div>

            <div class="clearfix">&nbsp;</div>

            <div class="form-group">
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <asp:TextBox ID="txtSenha" runat="server" CssClass="form-control" TextMode="Password" placeholder="Senha" required="required"></asp:TextBox>
                </div>
            </div>

            <div class="clearfix">&nbsp;</div>

            <div class="form-group">
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <div class="validation">&nbsp;<asp:Label ID="lblAcessoNegado" runat="server" Text="Usuário e/ou senha inválidos." ForeColor="Yellow" Visible="false"></asp:Label>&nbsp;</div>
                </div>
            </div>

            <div class="clearfix">&nbsp;</div>

            <div class="form-group">
                <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4 pull-right">
                    <asp:Button ID="btnAcessar" runat="server" Text="Entrar" CssClass="btn btn-block btn-primary" OnClick="btnAcessar_Click" />
                </div>
            </div>
        </div>
    </form>

    <script src="Scripts/jquery-3.1.1.js"></script>
    <script src="Scripts/modernizr-2.8.3.js"></script>
    <script src="Scripts/bootstrap.js"></script>
    <script src="Scripts/select2.js"></script>
    <script src="Scripts/app.js"></script>

    <script type="text/javascript">
        $(function () {
            $('.form-select2').select2();

            $('.form-select2').on('select2-opening', function () {
                $('#mdlLoading').modal({ backdrop: 'static', keyboard: false, show: false });
            }).on('select2-selecting', function () {
                if ($(this).hasClass('not-postback')) {
                    $('#mdlLoading').modal({ backdrop: 'static', keyboard: false, show: false });
                }
            }).on('change', function () {
                if ($(this).hasClass('not-postback')) {
                    $('#mdlLoading').modal({ backdrop: 'static', keyboard: false, show: false });
                }
                else {
                    $('#mdlLoading').modal({ backdrop: 'static', keyboard: false, show: true });
                }
            });

            $('button[type="submit"]').click(function () {
                if ($("#password").val() != "") {
                    $("#password").val(Base64.encode($("#password").val()));
                }
            });
        });
    </script>
</body>
</html>
