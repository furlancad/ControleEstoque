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

    public partial class Deposito : RefreshBasePage
    {
        private OracleConnection GetConnection()
        {
            string connection = ConfigurationManager.ConnectionStrings["ConexaoOracle"].ConnectionString;
            return new OracleConnection(connection);
        }

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
                    FillUnidade("#");
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Active", "$(function(){ LetterActive('', 'A'); });", true);
                }

                if (IsRefreshed)
                {
                    Response.Redirect(Request.RawUrl);
                }
            }
        }
        //Preenchimento do DropDown Unidade.
        private void FillUnidade(string pLetra)
        {                      

            try
            {
                IList<UNIDADE> listaUnidade = new List<UNIDADE>(); 
                    

                using (estoqueEntities conexao = new estoqueEntities())
                {
                    if (pLetra.Equals("#"))
                    {

                        listaUnidade = conexao.UNIDADE.Distinct().ToList();

                    }
                    else
                    {
                        listaUnidade = conexao.UNIDADE.Where(p => p.unidade.StartsWith(pLetra)).Distinct().ToList();
                    }

                    

                    grdUnidade.DataSource = listaUnidade;
                    grdUnidade.DataBind();


                    if (listaUnidade.Count > 0)
                    {
                        grdUnidade.UseAccessibleHeader = true;
                        grdUnidade.HeaderRow.TableSection = TableRowSection.TableHeader;

                        TableCellCollection cells = grdUnidade.HeaderRow.Cells;
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
        //Método Carregar Depósito.
        private void CarregarDeposito(string pLetra)
        {
            FillUnidade(pLetra);
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ActiveOne", "$(function(){ LetterActive('collapseOne', '" + pLetra + "'); });", true);
        }

        protected void btn_collapseOne_Click(object sender, EventArgs e)
        {
            CarregarDeposito(((Button)sender).Text);
        }
        //Evento RowDataBound da GridView Unidade.
        protected void grdUnidade_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                UNIDADE deposito = (UNIDADE)e.Row.DataItem;

                LinkButton lkAtivarDesativar = (LinkButton)e.Row.FindControl("lkAtivarDesativar");
                //TextBox txtDataAgendamento = (TextBox)e.Row.FindControl("txtDataAgendamento");

                //using (ConexaoSql conexaoSql = new ConexaoSql())
                //{
                //    CONTRATACAO_STATUS statusAtivo = conexaoSql.CONTRATACAO_STATUS.Where(p => p.descricao.Equals("Ativo")).FirstOrDefault();
                //    CONTRATACAO_USUARIO usuario = conexaoSql.CONTRATACAO_USUARIO.Where(p => p.colaborador_id == colaborador.id).FirstOrDefault();

                //    txtDataAgendamento.Text = usuario.data_desativacao_conta != null ? usuario.data_desativacao_conta.Value.ToString("dd/MM/yyyy") : string.Empty;

                   if (deposito.status)
                   {
                        e.Row.Cells[1].Text = "Ativo";
                        lkAtivarDesativar.ToolTip = "Ativo";
                        lkAtivarDesativar.CssClass = "mylinkbutton glyphicon glyphicon-ok";
                //        lkAtivarDesativar.OnClientClick = "javascript: return InativarUsuario(" + colaborador.id + ")";
                   }
                   else
                   {
                       e.Row.Cells[1].Text = "Inativo";
                       lkAtivarDesativar.ToolTip = "Inativo";
                       lkAtivarDesativar.CssClass = "mylinkbutton glyphicon glyphicon-remove";
                //        lkAtivarDesativar.OnClientClick = "javascript: return AtivarUsuario(" + colaborador.id + ")";
                   }

                //    if (colaborador.data_demissao != null)
                //    {
                //        e.Row.ForeColor = Color.Red;
                //        e.Row.CssClass = "red-color";
                //    }
                //}
            }
        }
        //Evento RowCommand da GridView Unidade.
        protected void grdUnidade_RowCommand(object sender, GridViewCommandEventArgs e)
        {
           

            if (e.CommandName.Equals("AtivarDesativar"))
            {
                try
                {
                    long id = Convert.ToInt64(e.CommandArgument.ToString().Trim());

                    using (estoqueEntities conexao = new estoqueEntities())
                    {

                        UNIDADE unidade = conexao.UNIDADE.Where(p => p.idUnidade == id).FirstOrDefault();
                        if (unidade != null)
                        {
                            unidade.status = !unidade.status;
                            conexao.Entry<UNIDADE>(unidade).State = System.Data.Entity.EntityState.Modified;
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

            FillUnidade("#");
           
        }

    }
}