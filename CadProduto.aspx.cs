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

namespace ControleEstoque.Web
{
    public partial class CadProduto : System.Web.UI.Page
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
                    FillCadProduto("#");
                    //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Active", "$(function(){ LetterActive('', 'A'); });", true);
                }
                if (IsRefreshed)
                {
                    Response.Redirect(Request.RawUrl);
                }
            }
        }
        // Select de Produtos da Tabela do Sei.

        private OracleConnection GetConnection()
        {
            string connection = ConfigurationManager.ConnectionStrings["ConexaoOracle"].ConnectionString;
            return new OracleConnection(connection);
        }

        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string FindCodBarra(string data)
        {
            string retorno = string.Empty;
            using (estoqueEntities conn = new estoqueEntities())
            {
                PRODUTO produto = conn.PRODUTO.Where(p => p.codBarra.Equals(data.Trim())).FirstOrDefault();
                if (produto != null)
                {
                    retorno = "Produto já cadastrado";
                }
            }

            return retorno;
        }
        //Webmethod conexão oracle Sei.
        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<string> FindCadastrar(string pTexto)
        {

            OracleConnection cn = new OracleConnection();
            //Declarar um objeto Oracle Command
            OracleCommand dbCommand = cn.CreateCommand();
            //Criar um objeto DataTable
            DataTable oDt = new DataTable();

            //Atribuir à variável cn o Valor da função GetConnection
            cn = new OracleConnection(ConfigurationManager.ConnectionStrings["ConexaoOracle"].ConnectionString);

            int x = 0;
            int.TryParse(pTexto.Trim(), out x);

            if (x > 0)
            {
                dbCommand.CommandText = "SELECT DISTINCT PRO.ID, PRO.PRO_NOME, PRO.PRO_VALORMERCADO, UNI.UNM_SIGLA FROM PRODUTO PRO INNER JOIN UNIDADEMEDIDA UNI ON UNI.ID = PRO.UNM_ID WHERE PRO.ID = " + pTexto.ToUpper().Trim() + "  ORDER BY PRO.ID";
            }
            else
            {
                dbCommand.CommandText = "SELECT DISTINCT PRO.ID, PRO.PRO_NOME, PRO.PRO_VALORMERCADO, UNI.UNM_SIGLA FROM PRODUTO PRO INNER JOIN UNIDADEMEDIDA UNI ON UNI.ID = PRO.UNM_ID WHERE PRO.PRO_NOME LIKE '" + pTexto.ToUpper().Trim() + "%' ORDER BY PRO.ID";
            }
            //Informar o nome do comando que será executado            


            dbCommand.CommandType = CommandType.Text;

            //Criar o tratamento para 
            try
            {
                //A conexão que será usada é a que foi declarada no início do código
                dbCommand.Connection = cn;



                //dbCommand.Parameters.Add();
                //dbCommand.Parameters.Add();

                //Criar um objeto Oracle Data Adapter
                OracleDataAdapter oDa = new OracleDataAdapter(dbCommand);

                //Preenchendo o DataTable
                oDa.Fill(oDt);

                //this.Master.addMessage("Operação efetuada com sucesso!");

                ////Resultado da Função
                //return oDt;                                     
                List<string> customers = new List<string>();

                for (int i = 0; i < oDt.Rows.Count; i++)
                {
                    customers.Add(oDt.Rows[i].ItemArray[0].ToString().Trim() + " | " + oDt.Rows[i].ItemArray[1].ToString().Trim() + " | " + oDt.Rows[i].ItemArray[2].ToString().Trim() + " | " + oDt.Rows[i].ItemArray[3].ToString().Trim());
                }
                return customers;
            }
            catch (Exception ex)
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
                dbCommand.Dispose();
                cn.Dispose();
                throw ex;

            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
                dbCommand.Dispose();
                cn.Dispose();

            }

        }
        //Preencher GridView produtos.
        private void FillCadProduto(string pLetra)
        {
            try
            {
                IList<PRODUTO> cadProduto = new List<PRODUTO>();

                using (estoqueEntities conexao = new estoqueEntities())
                {
                    if (pLetra.Equals("#"))
                    {
                        cadProduto = conexao.PRODUTO.Distinct().ToList();
                    }
                    else
                    {
                        cadProduto = conexao.PRODUTO.Where(p => p.produto.StartsWith(pLetra)).Distinct().ToList();
                    }
                    grdCadProduto.DataSource = cadProduto;
                    grdCadProduto.DataBind();
                }

                if (cadProduto.Count > 0)
                {
                    grdCadProduto.UseAccessibleHeader = true;
                    grdCadProduto.HeaderRow.TableSection = TableRowSection.TableHeader;

                    TableCellCollection cells = grdCadProduto.HeaderRow.Cells;
                    cells[0].Attributes.Add("data-class", "expand");
                    cells[1].Attributes.Add("data-sort-ignore", "true");
                    cells[2].Attributes.Add("data-sort-ignore", "true");
                    cells[3].Attributes.Add("data-sort-ignore", "true");
                    cells[4].Attributes.Add("data-sort-ignore", "true");
                }

            }
            catch (Exception)
            {
                throw;

            }
        }

        protected void grdCadProduto_RowDataBound(object sender, GridViewRowEventArgs e)
        {


        }

        protected void grdCadProduto_RowCommand(object sender, GridViewCommandEventArgs e)
        {


        }

        protected void btnEnter_Click(object sender, EventArgs e)
        {
            FillCadProduto("#");
        }

        private void LimparCampos()
        {
            txtCadProduto.Text = string.Empty;
        }

        private void IniciaCampos()
        {
            txtCadProduto.Focus();
        }


        public bool IsRefreshed { get; set; }

        private void CarregarProduto(string pLetra)
        {
            FillCadProduto(pLetra);
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ActiveOne", "$(function(){ LetterActive('collapseOne', '" + pLetra + "'); });", true);
        }

        protected void btn_collapseOne_Click(object sender, EventArgs e)
        {
            CarregarProduto(((Button)sender).Text);
        }

        protected void btnCadProduto_Click(object sender, EventArgs e)
        {
            string[] produto = hdfCadProduto.Value.Split('|');

            int validacao = 0;

            foreach (var item in produto)
            {
                if (String.IsNullOrEmpty(item.Trim()))
                {
                    validacao++;

                }

            }
            if (validacao == 0)
            {
                using (estoqueEntities conexao = new estoqueEntities())
                {
                    using (var tran = conexao.Database.BeginTransaction())
                    {
                        PRODUTO cadproduto = conexao.PRODUTO.Where(p => p.codBarra.Equals(txtCodBarra.Text)).FirstOrDefault();

                        //if (cadproduto.codSei < 0)
                        //{
                        //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/>Código do SEI não cadastrado corretamente no SEI.', 'warning');", true);
                        //}
                        //else if (cadproduto.produto == null)
                        //{
                        //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/>Produto não cadastrado corretamente no SEI.', 'warning');", true);

                        //}
                        //else if (cadproduto.valorUnitario == null)
                        //{
                        //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/>Valor Unitário não cadastrado corretamente no SEI.', 'warning');", true);
                        //}
                        //else if (cadproduto.unidMedida == null)
                        //{
                        //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/>Unidade de Medida não cadastrado corretamente no SEI.', 'warning');", true);
                        //}
                        //else
                        //{
                        try
                        {
                            string nome = ((UserAD)Session["UsuarioSistemasLegados"]).Samaccountname;

                            USUARIO usuario = conexao.USUARIO.Include("DEPOSITO").Where(p => p.usuario.Equals(nome)).FirstOrDefault();

                            if (cadproduto == null && usuario != null)
                            {
                                cadproduto = new PRODUTO();
                                cadproduto.codBarra = txtCodBarra.Text;
                                cadproduto.codSei = Convert.ToInt32(hdfCadProduto.Value.Split('|')[0].ToString().Trim());
                                cadproduto.produto = hdfCadProduto.Value.Split('|')[1].ToString().Trim();
                                cadproduto.valorUnitario = Convert.ToDecimal(hdfCadProduto.Value.Split('|')[2].ToString().Trim());
                                cadproduto.unidMedida = hdfCadProduto.Value.Split('|')[3].ToString().Trim();

                                conexao.Entry<PRODUTO>(cadproduto).State = System.Data.Entity.EntityState.Added;
                                conexao.SaveChanges();

                                ENTRADA entrada = new ENTRADA();
                                entrada.dataEntrada = DateTime.Now;
                                entrada.usuario = usuario.usuario;
                                entrada.observacao = string.Empty;

                                conexao.Entry<ENTRADA>(entrada).State = System.Data.Entity.EntityState.Added;
                                conexao.SaveChanges();

                                ITENS_ENTRADA itensEntrada = new ITENS_ENTRADA();

                                itensEntrada.quantEntrada = null;
                                itensEntrada.localizacao = string.Empty;
                                itensEntrada.valorUnitario = Convert.ToDecimal(hdfCadProduto.Value.Split('|')[2].ToString().Trim());
                                itensEntrada.dataVencimento = null;
                                itensEntrada.idEntrada = entrada.id;
                                itensEntrada.ENTRADA = entrada;

                                itensEntrada.codSei = cadproduto.id;
                                itensEntrada.PRODUTO = cadproduto;
                                itensEntrada.idDeposito = usuario.idDeposito;

                                //UNIDADE unidade = conn.UNIDADE.Where(p => p.idUnidade == usuario.DEPOSITO.idUnidade).FirstOrDefault();
                                //itensEntrada.UNIDADE = unidade;

                                conexao.Entry<ITENS_ENTRADA>(itensEntrada).State = System.Data.Entity.EntityState.Added;
                                conexao.SaveChanges();
                                tran.Commit();

                                txtCodBarra.Text = string.Empty;
                                txtCadProduto.Text = string.Empty;

                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Success", "alert('Sucesso<br/> Produto cadastrado com sucesso.', 'success');", true);
                            }
                            else if (cadproduto == null && usuario == null)
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/> Usuário não cadastrado.', 'warning');", true);
                            }
                            else
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/> Produto Já cadastrado.', 'warning');", true);
                            }

                        }
                        catch (Exception)
                        {
                            tran.Rollback();

                            throw;
                        }
                    }

                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/>Produto não cadastrado corretamente no SEI.', 'warning');", true);
            }

            FillCadProduto("#");
        }
    }
}

