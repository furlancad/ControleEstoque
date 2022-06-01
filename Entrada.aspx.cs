using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Entity;
using ControleEstoque.Web.Models;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.Data;
using BusyBoxDotNet;
using System.Data.SqlClient;
using ControleEstoque.Web.Models.ADModel;
using System.Text;
using System.Web.Script.Services;
using System.Web.Services;


namespace ControleEstoque.Web
{

    public partial class Entrada : System.Web.UI.Page
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

                    FillGridEntrada();
                    FillEntrada();
                    txtCodBarra.Focus();
                }
            }
        }
        private void FillEntrada()
        {
            if (!String.IsNullOrEmpty(txtCodBarra.Text))
            {
                UserAD usuarioSistemas = Session["UsuarioSistemasLegados"] as UserAD;


                using (estoqueEntities conexao = new estoqueEntities())
                {
                    USUARIO user = conexao.USUARIO.Where(p => p.usuario.Equals(usuarioSistemas.Samaccountname)).FirstOrDefault();

                    PRODUTO entradaProd = conexao.PRODUTO.Where(p => p.codBarra.Equals(txtCodBarra.Text)).FirstOrDefault();

                    if (entradaProd != null)
                    {
                        if (!String.IsNullOrEmpty(entradaProd.codBarra))
                        {
                            ENTRADA_AUXILIAR entradaAuxiliar = conexao.ENTRADA_AUXILIAR.Where(p => p.codSei.Equals(entradaProd.codSei)).FirstOrDefault();

                            if (entradaAuxiliar == null)
                            {

                                ENTRADA_AUXILIAR entrada = new ENTRADA_AUXILIAR();

                                entrada.codSei = entradaProd.codSei;
                                entrada.idDeposito = user.idDeposito;
                                entrada.dataVencimento = null;
                                entrada.quantEntrada = null;
                                entrada.localizacao = null;
                                entrada.observacao = null;
                                entrada.idUsuario = user.id;

                                conexao.Entry<ENTRADA_AUXILIAR>(entrada).State = System.Data.Entity.EntityState.Added;

                                conexao.SaveChanges();

                                txtCodBarra.Text = string.Empty;
                            }

                        }
                        else
                        {

                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/> O Produto já está cadastrado no Sistema.', 'warning');", true);

                        }
                    }
                    else
                    {

                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/> O Produto não está cadastrado no sistema. Entre em contato com a Administração do Sistema.', 'warning');", true);

                    }
                }
            }

            FillGridEntrada();
        }

        private void FillGridEntrada()
        {
            UserAD usuarioSistemas = Session["UsuarioSistemasLegados"] as UserAD;

            IList<ENTRADA_AUXILIAR> entrada = new List<ENTRADA_AUXILIAR>();

            using (estoqueEntities conexao = new estoqueEntities())
            {
                USUARIO user = conexao.USUARIO.Where(p => p.usuario.Equals(usuarioSistemas.Samaccountname)).FirstOrDefault();
                entrada = conexao.ENTRADA_AUXILIAR.Where(p => p.idUsuario == user.id).Distinct().ToList();

                grdEntrada.DataSource = entrada;
                grdEntrada.DataBind();

            }
            if (entrada.Count > 0)
            {
                grdEntrada.UseAccessibleHeader = true;
                grdEntrada.HeaderRow.TableSection = TableRowSection.TableHeader;

                TableCellCollection cells = grdEntrada.HeaderRow.Cells;
                cells[0].Attributes.Add("data-class", "expand");
                cells[1].Attributes.Add("data-sort-ignore", "true");
                cells[2].Attributes.Add("data-sort-ignore", "true");
                cells[3].Attributes.Add("data-sort-ignore", "true");
                cells[4].Attributes.Add("data-sort-ignore", "true");
                cells[5].Attributes.Add("data-hide", "all");
            }

        }
        protected void btnEnter_Click(object sender, EventArgs e)
        {
            FillEntrada();
        }

        [WebMethod]
        public static string SalvarEntradaAuxiliar(string itemid, string data)
        {
            string msg = string.Empty;

            try
            {
                using (estoqueEntities conexao = new estoqueEntities())
                {
                    int id = Convert.ToInt32(itemid.Split('_')[1]);
                    ENTRADA_AUXILIAR entradaAuxiliar = conexao.ENTRADA_AUXILIAR.Where(p => p.id == id).FirstOrDefault();
                    if (entradaAuxiliar != null)
                    {
                        string campo = itemid.Split('_')[0];

                        if (campo.Equals("txtVencimento"))
                        {
                            entradaAuxiliar.dataVencimento = !string.IsNullOrEmpty(data) ? Convert.ToDateTime(data) : (DateTime?)null;
                        }
                        else if (campo.Equals("txtQuantidade"))
                        {
                            entradaAuxiliar.quantEntrada = !string.IsNullOrEmpty(data) ? Convert.ToInt32(data) : (int?)null;
                        }
                        else if (campo.Equals("txtLocalizacao"))
                        {
                            entradaAuxiliar.localizacao = !string.IsNullOrEmpty(data) ? data : null;
                        }
                        else if (campo.Equals("txtObservacao"))
                        {
                            entradaAuxiliar.observacao = !string.IsNullOrEmpty(data) ? data : null;
                        }

                        conexao.Entry<ENTRADA_AUXILIAR>(entradaAuxiliar).State = System.Data.Entity.EntityState.Modified;

                        conexao.SaveChanges();

                        msg = "Item cadastrado com sucesso._success";

                    }

                }
            }
            catch
            {
                msg = "Operação não efetuada. Entre em contato com o administrador do sistema._danger";
            }

            return msg;
        }
        protected void grdEntrada_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ENTRADA_AUXILIAR entradaAuxiliar = (ENTRADA_AUXILIAR)e.Row.DataItem;

                TextBox txtVencimento = (TextBox)e.Row.FindControl("txtVencimento");
                TextBox txtQuantidade = (TextBox)e.Row.FindControl("txtQuantidade");
                TextBox txtLocalizacao = (TextBox)e.Row.FindControl("txtLocalizacao");
                TextBox txtObservacao = (TextBox)e.Row.FindControl("txtObservacao");


                using (estoqueEntities conexao = new estoqueEntities())
                {
                    PRODUTO produto = conexao.PRODUTO.Where(p => p.codSei.Equals(entradaAuxiliar.codSei)).FirstOrDefault();
                    if (produto != null)
                    {
                        e.Row.Cells[0].Text = produto.codSei.ToString();
                        e.Row.Cells[1].Text = produto.produto;
                    }
                    else { }

                    txtVencimento.Text = entradaAuxiliar.dataVencimento.HasValue ? entradaAuxiliar.dataVencimento.Value.ToString("dd/MM/yyyy") : string.Empty;
                    txtQuantidade.Text = entradaAuxiliar.quantEntrada.HasValue ? entradaAuxiliar.quantEntrada.Value.ToString() : string.Empty;
                    txtLocalizacao.Text = !string.IsNullOrEmpty(entradaAuxiliar.localizacao) ? entradaAuxiliar.localizacao : string.Empty;
                    txtObservacao.Text = !string.IsNullOrEmpty(entradaAuxiliar.observacao) ? entradaAuxiliar.observacao : string.Empty;
                }
            }
        }
        protected void grdEntrada_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }
        protected void btnSalvarEntrada_Click(object sender, EventArgs e)
        {
            using (estoqueEntities conexao = new estoqueEntities())
            {
                using (var tran = conexao.Database.BeginTransaction())
                {
                    try
                    {
                        UserAD usuarioSistemas = Session["UsuarioSistemasLegados"] as UserAD;
                        USUARIO user = conexao.USUARIO.Where(p => p.usuario.Equals(usuarioSistemas.Samaccountname)).FirstOrDefault();

                        IList<ENTRADA_AUXILIAR> listaAuxiliar = conexao.ENTRADA_AUXILIAR.Where(p => p.idDeposito == user.idDeposito).ToList();

                        #region -- Salvar --
                        foreach (var item in listaAuxiliar)
                        {
                            // ITENS_ENTRADA itemEntrada = conexao.ITENS_ENTRADA.Where(p => p.idProduto == item.idProduto).FirstOrDefault();

                            PRODUTO produto = conexao.PRODUTO.Where(p => p.codSei == item.codSei).FirstOrDefault();

                            //if (itemEntrada == null)
                            //{
                            ENTRADA entrada = new ENTRADA();
                            entrada.dataEntrada = DateTime.Now;
                            entrada.usuario = user.usuario;
                            entrada.observacao = item.observacao;

                            conexao.Entry<ENTRADA>(entrada).State = System.Data.Entity.EntityState.Added;

                            conexao.SaveChanges();

                            ITENS_ENTRADA itemEntrada = new ITENS_ENTRADA();
                            itemEntrada.quantEntrada = item.quantEntrada;
                            itemEntrada.localizacao = item.localizacao;
                            itemEntrada.valorUnitario = produto.valorUnitario.HasValue ? produto.valorUnitario.Value : 0;
                            itemEntrada.dataVencimento = item.dataVencimento;
                            itemEntrada.idEntrada = entrada.id;
                            itemEntrada.codSei = produto.codSei;
                            itemEntrada.PRODUTO = produto;
                            itemEntrada.ENTRADA = entrada;
                            itemEntrada.idDeposito = user.idDeposito;

                            conexao.Entry<ITENS_ENTRADA>(itemEntrada).State = System.Data.Entity.EntityState.Added;

                            conexao.SaveChanges();
                        }

                        #endregion

                        #region -- Remover --

                        foreach (var item in listaAuxiliar)
                        {
                            conexao.Entry<ENTRADA_AUXILIAR>(item).State = System.Data.Entity.EntityState.Deleted;
                        }

                        conexao.SaveChanges();
                        tran.Commit();

                        #endregion

                        FillGridEntrada();

                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}


