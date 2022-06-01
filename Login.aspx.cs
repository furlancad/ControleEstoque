using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using ControleEstoque.Web.Models.ADModel;

namespace ControleEstoque.Web
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ClearSession();
                txtUsuario.Focus();
            }
        }

        private void ClearSession()
        {
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
        }
        //Botão Logar Usuário
        protected void btnAcessar_Click(object sender, EventArgs e)
        {
            ManageAD manager = new ManageAD();

            bool isValidUser = manager.IsValidUser(txtUsuario.Text.Trim(), txtSenha.Text.Trim());

            if (isValidUser)
            {
                UserAD us = manager.GetUserHabilitado(txtUsuario.Text.Trim());

                if (!string.IsNullOrEmpty(us.Postofficebox))
                {
                    Session["UsuarioSistemasLegados"] = us;
                    Response.Redirect("Index.aspx", true);
                }
                else
                {
                    lblAcessoNegado.Text = "CPF do usuário não cadastrado.";
                    lblAcessoNegado.Visible = true;
                }
            }
            else
            {
                lblAcessoNegado.Text = "Usuário e/ou senha inválidos ou usuário desabilitado.";
                lblAcessoNegado.Visible = true;
            }
        }
    }
}