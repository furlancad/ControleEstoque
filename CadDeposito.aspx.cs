using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusyBoxDotNet;
using System.Data.Entity;
using ControleEstoque.Web.Models.ADModel;
using System.Drawing;
using ControleEstoque.Web.Models;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace ControleEstoque.Web
{
    public partial class cadUsuario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioSistemasLegados"] == null)
            {
                Response.Redirect("Login.aspx");
            }
            else
            {
                if (!IsPostBack)
                {
                    FillCadUnidade();
                    FillDeposito("#");
                }
            }
        }
        //Preenchimento DropDown Unidade.
        private void FillCadUnidade()
        {
            IList<UNIDADE> listUnidade = new List<UNIDADE>();

            using (estoqueEntities conexao = new estoqueEntities())
            {
                listUnidade = conexao.UNIDADE.Distinct().ToList();

                ddlUnidade.DataTextField = "unidade";
                ddlUnidade.DataValueField = "idUnidade";
                ddlUnidade.DataSource = listUnidade;
                ddlUnidade.DataBind();

            }
        }
        //Botão cadastro de depósito.
        protected void btnCadDeposito_Click(object sender, EventArgs e)
        {
            using (estoqueEntities conexao = new estoqueEntities())
            {
                int x = Convert.ToInt32(ddlUnidade.SelectedValue);
                DEPOSITO cadDeposito = conexao.DEPOSITO.Where(p => p.deposito.Equals(txtDeposito.Text) && p.idUnidade == x && p.status).FirstOrDefault();

                if (cadDeposito == null)
                {
                    try
                    {
                        cadDeposito = new DEPOSITO();
                        cadDeposito.deposito = txtDeposito.Text;
                        cadDeposito.idUnidade = x;

                        conexao.Entry<DEPOSITO>(cadDeposito).State = System.Data.Entity.EntityState.Added;

                        conexao.SaveChanges();

                    }
                    catch (Exception)
                    {
                        throw;
                    }

                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/> Depósito Já cadastrado.', 'warning');", true);
                }
            }

            FillDeposito("#");
        }
        //Método de preechimento do Depósito.
        private void FillDeposito(string pLetra)
        {
            try
            {
                IList<DEPOSITO> listaDeposito = new List<DEPOSITO>();

                using (estoqueEntities conexao = new estoqueEntities())
                {
                    if (pLetra.Equals("#"))
                    {
                        listaDeposito = conexao.DEPOSITO.Distinct().ToList();
                    }
                    else
                    {
                        listaDeposito = conexao.DEPOSITO.Where(p => p.deposito.StartsWith(pLetra)).Distinct().ToList();
                    }

                    grdDeposito.DataSource = listaDeposito;
                    grdDeposito.DataBind();


                    if (listaDeposito.Count > 0)
                    {
                        grdDeposito.UseAccessibleHeader = true;
                        grdDeposito.HeaderRow.TableSection = TableRowSection.TableHeader;

                        TableCellCollection cells = grdDeposito.HeaderRow.Cells;
                        cells[0].Attributes.Add("data-class", "expand");
                        cells[1].Attributes.Add("data-sort-ignore", "true");
                        cells[2].Attributes.Add("data-sort-ignore", "true");

                    }

                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        private void CarregarDeposito(string pLetra)
        {
            FillDeposito(pLetra);
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ActiveOne", "$(function(){ LetterActive('collapseOne', '" + pLetra + "'); });", true);
        }

        protected void btn_collapseOne_Click(object sender, EventArgs e)
        {
            CarregarDeposito(((Button)sender).Text);
        }
        //Evento RowDataBound da GridView Depósito. 
        protected void grdDeposito_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DEPOSITO deposito = (DEPOSITO)e.Row.DataItem;

                LinkButton lkAtivarDesativar = (LinkButton)e.Row.FindControl("lkAtivarDesativar");
              
                if (deposito.status)
                {
                    e.Row.Cells[1].Text = "Ativo";
                    lkAtivarDesativar.ToolTip = "Ativo";
                    lkAtivarDesativar.CssClass = "mylinkbutton glyphicon glyphicon-ok";
                }
                else
                {
                    e.Row.Cells[1].Text = "Inativo";
                    lkAtivarDesativar.ToolTip = "Inativo";
                    lkAtivarDesativar.CssClass = "mylinkbutton glyphicon glyphicon-remove";
                }
            }
        }
        //Evento RowCommand da GridView Depósito.
        protected void grdDeposito_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("AtivarDesativar"))
            {
                try
                {
                    long id = Convert.ToInt64(e.CommandArgument.ToString().Trim());

                    using (estoqueEntities conexao = new estoqueEntities())
                    {
                        DEPOSITO deposito = conexao.DEPOSITO.Where(p => p.id == id).FirstOrDefault();
                        if (deposito != null)
                        {
                            deposito.status = !deposito.status;
                            conexao.Entry<DEPOSITO>(deposito).State = System.Data.Entity.EntityState.Modified;
                            conexao.SaveChanges();

                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Success", "alert('Sucesso<br/>Operação efetuada com sucesso.', 'success');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/>Não foi possível desativar a unidade. Entre em contato com o administrador do sistema', 'warning');", true);
                        }
                    }
                }
                catch
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/>Operação não efetuada. Entre em contato com o administrador do sistema.', 'warning');", true);
                }
            }
            FillDeposito("#");
        }
    }
}
