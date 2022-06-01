using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ControleEstoque.Web.Models;
using ControleEstoque.Web.Models.ADModel;
using RegistroFrequencia.Web.App_Code;

namespace ControleEstoque.Web
{
    public partial class Retorno : System.Web.UI.Page
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
                    FillTurma();
                    UserAD usuarioSistemas = Session["UsuarioSistemasLegados"] as UserAD;
                }
            }
        }
        // preenchimento do dropdown da turma na página retorno.
        private void FillTurma()
        {
            UserAD usuarioSistemas = Session["UsuarioSistemasLegados"] as UserAD;

            IList<int> listSaida = new List<int>();

            IList<TURMA> listTurma = new List<TURMA>();

            if (usuarioSistemas != null)
            {
                using (estoqueEntities conexao = new estoqueEntities())
                {
                    try
                    {
                        listSaida = conexao.SAIDA.Select(p => p.idTurma).Distinct().ToList();

                        if (usuarioSistemas.Memberof != null && (usuarioSistemas.Memberof.Contains(ConfigurationManager.AppSettings["CE-admin"])))
                        {
                            listTurma = conexao.TURMA.Where(p => listSaida.Contains(p.idTurma)).Distinct().ToList();
                        }
                        else
                        {
                            USUARIO user = conexao.USUARIO.Include("DEPOSITO").Where(p => p.usuario.Equals(usuarioSistemas.Samaccountname)).FirstOrDefault();
                            listTurma = conexao.TURMA.Where(p => listSaida.Contains(p.idTurma) && p.idUnidade == user.DEPOSITO.idUnidade).Distinct().ToList();
                        }
                    }
                    catch
                    {

                    }
                }
            }
            ddlRetornoCodTurma.DataTextField = "codTurma";
            ddlRetornoCodTurma.DataValueField = "idTurma";
            ddlRetornoCodTurma.DataSource = listTurma;
            ddlRetornoCodTurma.DataBind();
            ddlRetornoCodTurma.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Selecione", "0"));
        }
        //Preenchimento do Dropdown da disciplina.
        private void FillDisciplina()
        {
            IList<DISCIPLINA> listTurma = new List<DISCIPLINA>();

            using (estoqueEntities conexao = new estoqueEntities())
            {
                if (!string.IsNullOrEmpty(ddlRetornoCodTurma.SelectedValue))
                {
                    int x = Convert.ToInt32(ddlRetornoCodTurma.SelectedValue);
                    listTurma = conexao.DISCIPLINA.Where(p => p.idTurma == x).ToList();
                }
            }
            ddlRetornoDisciplina.DataTextField = "disciplina";
            ddlRetornoDisciplina.DataValueField = "idDisciplina";
            ddlRetornoDisciplina.DataSource = listTurma;
            ddlRetornoDisciplina.DataBind();
        }
        //Preenchimento do Dropdown Instrutor.
        private void FillInstrutor()
        {
            IList<DISCIPLINA_INSTRUTOR> listAula = new List<DISCIPLINA_INSTRUTOR>();

            using (estoqueEntities conexao = new estoqueEntities())
            {
                if (!string.IsNullOrEmpty(ddlRetornoDisciplina.SelectedValue))
                {
                    int y = Convert.ToInt32(ddlRetornoCodTurma.SelectedValue);
                    int x = Convert.ToInt32(ddlRetornoDisciplina.SelectedValue);
                    listAula = conexao.DISCIPLINA_INSTRUTOR.Where(p => p.idDisciplina == x && p.idTurma == y).ToList();

                    IList<INSTRUTOR> instrutorlista = new List<INSTRUTOR>();

                    for (int i = 0; i < listAula.Count; i++)
                    {
                        long inst = listAula[i].idInstrutor;
                        INSTRUTOR instrutor = conexao.INSTRUTOR.Where(p => p.idInstrutor == inst).FirstOrDefault();
                        if (instrutor != null)
                        {
                            instrutorlista.Add(instrutor);
                        }
                    }
                    ddlRetornoInstrutor.DataTextField = "instrutor";
                    ddlRetornoInstrutor.DataValueField = "idInstrutor";
                    ddlRetornoInstrutor.DataSource = instrutorlista;
                    ddlRetornoInstrutor.DataBind();
                }
            }
            FillRetornoSaida();
        }
        //Preenchimento da gridview de retorno da saída de produtos.
        private void FillRetornoSaida()
        {
            int turma = Convert.ToInt32(ddlRetornoCodTurma.SelectedValue);

            if (!string.IsNullOrEmpty(ddlRetornoDisciplina.SelectedValue))
            {
                int disciplina = Convert.ToInt32(ddlRetornoDisciplina.SelectedValue);
                long instrutor = Convert.ToInt64(ddlRetornoInstrutor.SelectedValue);
                string aula = ddlRetornoAula.SelectedValue;

                if (turma > 0 && disciplina >= 0 && instrutor >= 0 && !string.IsNullOrEmpty(txtRetornoPeriodo.Text) && !string.IsNullOrEmpty(txtRetornoPeriodo1.Text) && !string.IsNullOrEmpty(txtRetornoTurno.Text) && !string.IsNullOrEmpty(txtRetornoNAlunos.Text) && !string.IsNullOrEmpty(ddlRetornoAula.SelectedValue))
                {
                    try
                    {
                        using (estoqueEntities conexao = new estoqueEntities())
                        {
                            if (!string.IsNullOrEmpty(ddlRetornoCodTurma.SelectedValue))
                            {
                                IList<SAIDA> lista = new List<SAIDA>();

                                lista = conexao.SAIDA.Where(p => p.idTurma == turma).ToList();

                                if (lista != null)
                                {
                                    grdRetornoSaida.DataSource = lista;
                                    grdRetornoSaida.DataBind();

                                    if (lista.Count > 0)
                                    {
                                        grdRetornoSaida.UseAccessibleHeader = true;
                                        grdRetornoSaida.HeaderRow.TableSection = TableRowSection.TableHeader;

                                        TableCellCollection cells = grdRetornoSaida.HeaderRow.Cells;
                                        cells[0].Attributes.Add("data-class", "expand");
                                        cells[1].Attributes.Add("data-sort-ignore", "true");
                                        cells[2].Attributes.Add("data-sort-ignore", "true");
                                        cells[3].Attributes.Add("data-sort-ignore", "true");
                                        cells[4].Attributes.Add("data-sort-ignore", "true");
                                        cells[5].Attributes.Add("data-sort-ignore", "true");
                                        cells[6].Attributes.Add("data-sort-ignore", "true");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/>Necessário o preenchimento de todos os campos.', 'warning');", true);
                }
            }
            else
            {

            }
        }
        //Preenchimento da gridview de retorno dos produtos extras.

        //Metodo de preenchimento do Período / Turno / número de alunos
        protected void ddlRetornoCodTurma_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ddlRetornoCodTurma.SelectedValue.Equals(""))
            {
                using (estoqueEntities conexao = new estoqueEntities())
                {
                    int x = Convert.ToInt32(ddlRetornoCodTurma.SelectedValue);
                    TURMA preencherTurma = conexao.TURMA.Where(p => p.idTurma == x).FirstOrDefault();

                    if (!ddlRetornoCodTurma.SelectedValue.Equals("0"))
                    {
                        txtRetornoPeriodo.Text = Convert.ToString(String.Format("{0:dd/MM/yyyy}", preencherTurma.dataInicial));
                        txtRetornoPeriodo1.Text = Convert.ToString(String.Format("{0:dd/MM/yyyy}", preencherTurma.dataFinal));
                        txtRetornoTurno.Text = preencherTurma.turno;
                        txtRetornoNAlunos.Text = Convert.ToString(preencherTurma.numAlunos);
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/>Necessário o preenchimento de todos os campos.', 'warning');", true);
                    }
                }
            }
            FillDisciplina();
            ddlRetornoDisciplina_SelectedIndexChanged(this, null);
        }
        //Método de preenchimento do Dropdown das aulas.
        protected void ddlRetornoDisciplina_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (estoqueEntities conexao = new estoqueEntities())
            {
                if (!string.IsNullOrEmpty(ddlRetornoDisciplina.SelectedValue))
                {
                    int x = Convert.ToInt32(ddlRetornoDisciplina.SelectedValue);
                    DISCIPLINA preencherTurma = conexao.DISCIPLINA.Where(p => p.idDisciplina == x).FirstOrDefault();

                    Time horaInicial = new Time(preencherTurma.horaInicial);
                    Time horaFinal = new Time(preencherTurma.horaFinal);

                    Time hora = Time.TimeDiff(horaFinal, horaInicial);

                    //TimeSpan final = preencherTurma.dataFinal.Subtract(preencherTurma.dataInicial);
                    int number = preencherTurma.cargaHoraria / hora.Hour;
                    IList<System.Web.UI.WebControls.ListItem> list = new List<System.Web.UI.WebControls.ListItem>();
                    for (int i = 1; i <= number; i++)
                    {
                        list.Add(new System.Web.UI.WebControls.ListItem() { Text = "Aula " + i.ToString(), Value = i.ToString() });
                    }
                    ddlRetornoAula.DataValueField = "Value";
                    ddlRetornoAula.DataTextField = "Text";
                    ddlRetornoAula.DataSource = list;
                    ddlRetornoAula.DataBind();
                }
            }
            FillInstrutor();
        }
        //Preenchimento da gridview  com dados da tabela Produto / Planejamento_Itens extra / Itens Entrada.
        protected void grdRetornoSaida_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                using (estoqueEntities conexao = new estoqueEntities())
                {
                    SAIDA lista = (SAIDA)e.Row.DataItem;

                    PLANEJAMENTO planejamento = conexao.PLANEJAMENTO.Where(p => p.idTurma == lista.idTurma && p.codSei == lista.codSei).FirstOrDefault();
                    PLANEJAMENTO_ITENS_EXTRA plaItenExtra = null;
                    PLANEJAMENTO_AUXILIAR pAuxiliar = conexao.PLANEJAMENTO_AUXILIAR.Where(p => p.idTurma == lista.idTurma && p.codSei == lista.codSei).FirstOrDefault();

                    if (lista.quantidadeRetorno != 0)
                    {
                        TextBox txtqtdRetorno = (TextBox)e.Row.FindControl("txtqtdRetorno");
                        txtqtdRetorno.Enabled = false;
                    }
                    if (planejamento != null)
                    {
                        plaItenExtra = conexao.PLANEJAMENTO_ITENS_EXTRA.Where(p => p.idPlanejamento == planejamento.id && p.codSei == lista.codSei).FirstOrDefault();

                        int quant = plaItenExtra != null ? plaItenExtra.quantidade.Value : 0;
                        e.Row.Cells[3].Text = (planejamento.quantidade + quant).ToString();
                    }
                    if (pAuxiliar != null)
                    {
                        int quantAuxiliar = pAuxiliar != null ? pAuxiliar.quantidade : 0;

                        e.Row.Cells[3].Text = (quantAuxiliar).ToString();
                    }
                    PRODUTO produto = conexao.PRODUTO.Where(p => p.codSei == lista.codSei).FirstOrDefault();
                    if (produto != null)
                    {
                        e.Row.Cells[0].Text = produto.produto;
                        e.Row.Cells[2].Text = produto.unidMedida;
                    }
                    else
                    {
                        e.Row.Cells[0].Text = string.Empty;
                        e.Row.Cells[2].Text = string.Empty;
                    }
                    ITENS_ENTRADA iEntrada = conexao.ITENS_ENTRADA.Where(p => p.codSei == lista.codSei).FirstOrDefault();

                    if (iEntrada != null)
                    {
                        int entEstoque = conexao.ITENS_ENTRADA.Where(p => p.codSei == lista.codSei && p.idDeposito == iEntrada.idDeposito).Sum(p => p.quantEntrada.HasValue ? p.quantEntrada.Value : 0);

                        e.Row.Cells[1].Text = iEntrada.localizacao;
                    }
                    else
                    {
                        e.Row.Cells[1].Text = string.Empty;
                    }
                }
            }
        }
        protected void grdRetornoSaida_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Retorno"))
            {
                WebControl wc = ((WebControl)e.CommandSource);
                GridViewRow row = ((GridViewRow)wc.NamingContainer);
                TextBox txtqtdRetorno = (TextBox)((grdRetornoSaida.Rows[row.RowIndex]).Cells[4].Controls[1]);
                int y = Convert.ToInt32(ddlRetornoCodTurma.SelectedValue);

                using (estoqueEntities conexao = new estoqueEntities())
                {
                    int id = Convert.ToInt32(e.CommandArgument);
                    SAIDA realizarRetorno = conexao.SAIDA.Where(p => p.id == id && p.idTurma == y && p.aula.Equals(ddlRetornoAula.SelectedItem.Text)).FirstOrDefault();

                    PLANEJAMENTO planejamento = conexao.PLANEJAMENTO.Where(p => p.idTurma == realizarRetorno.idTurma && p.codSei == realizarRetorno.codSei).FirstOrDefault();
                    int preencherPlanejar = Convert.ToInt32(planejamento != null ? planejamento.quantidade : 0);
                    PLANEJAMENTO_ITENS_EXTRA plaItenExtra = conexao.PLANEJAMENTO_ITENS_EXTRA.Where(p => p.idPlanejamento == preencherPlanejar && p.codSei == realizarRetorno.codSei).FirstOrDefault();                     
                    PLANEJAMENTO_AUXILIAR pAuxiliar = conexao.PLANEJAMENTO_AUXILIAR.Where(p => p.idTurma == realizarRetorno.idTurma && p.codSei == realizarRetorno.codSei).FirstOrDefault();

                    ITENS_ENTRADA itensEntrada = conexao.ITENS_ENTRADA.Where(p => p.codSei == realizarRetorno.codSei).FirstOrDefault();

                    PRODUTO produto = conexao.PRODUTO.Where(p => p.codSei == realizarRetorno.codSei).FirstOrDefault();

                    realizarRetorno.quantidadeRetorno = Convert.ToInt32(txtqtdRetorno.Text);
                    int quantAuxiliar = pAuxiliar != null ? pAuxiliar.quantidade : 0;
                    int preencherItemExtra = Convert.ToInt32(plaItenExtra != null ? plaItenExtra.quantidade : 0);

                    if (realizarRetorno != null)
                    {
                        if (realizarRetorno.quantidadeRetorno > 0 && realizarRetorno.quantidadeRetorno <= preencherPlanejar + preencherItemExtra)
                        {
                            realizarRetorno.valorRetorno = realizarRetorno.quantidadeRetorno * itensEntrada.valorUnitario;
                            conexao.Entry<SAIDA>(realizarRetorno).State = System.Data.Entity.EntityState.Modified;
                            conexao.SaveChanges();
                        }
                        else if (realizarRetorno.quantidadeRetorno > 0 && realizarRetorno.quantidadeRetorno <= quantAuxiliar)
                        {
                            realizarRetorno.valorRetorno = realizarRetorno.quantidadeRetorno * itensEntrada.valorUnitario;
                            conexao.Entry<SAIDA>(realizarRetorno).State = System.Data.Entity.EntityState.Modified;
                            conexao.SaveChanges();
                        }
                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/> Valor incompatível com a quantidade informada.', 'warning');", true);
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/> Não existe valor suficiente para realização do retorno.', 'warning');", true);
                    }
                }
            }

            FillRetornoSaida();
        }
    }
}