using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.Data;
using ControleEstoque.Web.Models;
using ControleEstoque.Web.Models.ADModel;
using System.Text;
using System.Data.SqlClient;
using ControleEstoque.Web.Classes;
using System.IO;
using Microsoft.Reporting.WebForms;
using System.Diagnostics;
using System.ComponentModel;
using Microsoft.ReportingServices;
using Microsoft.Reporting.Common;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Drawing;
using iTextSharp.text.html.simpleparser;
using System.Web.Services;

namespace ControleEstoque.Web
{
    public partial class Saida : System.Web.UI.Page
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
        //Preenchimento do DropDown das Turmas.
        private void FillTurma()
        {
            UserAD usuarioSistemas = Session["UsuarioSistemasLegados"] as UserAD;

            IList<int> listPlanejamento = new List<int>();

            IList<TURMA> listTurma = new List<TURMA>();

            if (usuarioSistemas != null)
            {
                using (estoqueEntities conexao = new estoqueEntities())
                {
                    try
                    {
                        listPlanejamento = conexao.PLANEJAMENTO.Select(p => p.idTurma).Distinct().ToList();

                        if (usuarioSistemas.Memberof != null && (usuarioSistemas.Memberof.Contains(ConfigurationManager.AppSettings["CE-admin"])))
                        {
                            listTurma = conexao.TURMA.Where(p => listPlanejamento.Contains(p.idTurma)).Distinct().ToList();
                        }
                        else
                        {
                            USUARIO user = conexao.USUARIO.Include("DEPOSITO").Where(p => p.usuario.Equals(usuarioSistemas.Samaccountname)).FirstOrDefault();
                            listTurma = conexao.TURMA.Where(p => listPlanejamento.Contains(p.idTurma) && p.idUnidade == user.DEPOSITO.idUnidade).Distinct().ToList();
                        }
                    }
                    catch
                    {

                    }
                }
            }
            ddlSaidaCodTurma.DataTextField = "codTurma";
            ddlSaidaCodTurma.DataValueField = "idTurma";
            ddlSaidaCodTurma.DataSource = listTurma;
            ddlSaidaCodTurma.DataBind();

            ddlSaidaCodTurma.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Selecione", "0"));
        }
        //Preenchimento do DropDown da Disciplina.
        private void FillDisciplina()
        {
            IList<DISCIPLINA> listTurma = new List<DISCIPLINA>();

            using (estoqueEntities conexao = new estoqueEntities())
            {
                if (!string.IsNullOrEmpty(ddlSaidaCodTurma.SelectedValue))
                {
                    int x = Convert.ToInt32(ddlSaidaCodTurma.SelectedValue);
                    listTurma = conexao.DISCIPLINA.Where(p => p.idTurma == x).ToList();
                }
            }
            ddlDisciplinaSaida.DataTextField = "disciplina";
            ddlDisciplinaSaida.DataValueField = "idDisciplina";
            ddlDisciplinaSaida.DataSource = listTurma;
            ddlDisciplinaSaida.DataBind();
        }
        //Preenchimento do DropDown das Aulas.
        private void FillAula()
        {
            try
            {
                IList<TURMA> listAula = new List<TURMA>();

                using (estoqueEntities conexao = new estoqueEntities())
                {
                    int x = Convert.ToInt32(ddlSaidaCodTurma.SelectedValue);
                    listAula = conexao.TURMA.Where(p => p.codTurma.Equals(x)).ToList();

                    for (int i = 0; i < listAula.Count; i++)
                    {

                    }
                    ddlSaidaCodTurma.DataTextField = "codTurma";
                    ddlSaidaCodTurma.DataValueField = "idTurma";
                    ddlSaidaCodTurma.DataSource = listAula;
                    ddlSaidaCodTurma.DataBind();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Preenchimento do DropDown Instrutores.
        private void FillInstrutor()
        {
            IList<DISCIPLINA_INSTRUTOR> listAula = new List<DISCIPLINA_INSTRUTOR>();

            using (estoqueEntities conexao = new estoqueEntities())
            {
                if (!string.IsNullOrEmpty(ddlDisciplinaSaida.SelectedValue))
                {
                    int y = Convert.ToInt32(ddlSaidaCodTurma.SelectedValue);
                    int x = Convert.ToInt32(ddlDisciplinaSaida.SelectedValue);
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

                    ddlInstrutorSaida.DataTextField = "instrutor";
                    ddlInstrutorSaida.DataValueField = "idInstrutor";
                    ddlInstrutorSaida.DataSource = instrutorlista;
                    ddlInstrutorSaida.DataBind();
                }
            }
            FillSaida();
        }
        //Preenchimento da GridView de Saída.
        private void FillSaida()
        {
            int turma = Convert.ToInt32(ddlSaidaCodTurma.SelectedValue);

            if (!string.IsNullOrEmpty(ddlDisciplinaSaida.SelectedValue))
            {
                int disciplina = Convert.ToInt32(ddlDisciplinaSaida.SelectedValue);
                long instrutor = Convert.ToInt64(ddlInstrutorSaida.SelectedValue);
                string aula = ddlAulasSaida.SelectedValue;

                if (turma > 0 && disciplina >= 0 && instrutor >= 0 && !string.IsNullOrEmpty(txtPeriodoSaida.Text) && !string.IsNullOrEmpty(txtPeriodoSaida1.Text) && !string.IsNullOrEmpty(txtTurnoSaida.Text) && !string.IsNullOrEmpty(txtNAlunosSaida.Text) && !string.IsNullOrEmpty(ddlAulasSaida.SelectedValue))
                {
                    try
                    {
                        using (estoqueEntities conn = new estoqueEntities())
                        {
                            if (!string.IsNullOrEmpty(ddlSaidaCodTurma.SelectedValue))
                            {
                                IList<PLANEJAMENTO> lista = new List<PLANEJAMENTO>();

                                    lista = conn.PLANEJAMENTO.Where(p => p.codTurma.Equals(ddlSaidaCodTurma.SelectedItem.Text) && (p.idDisciplina != null && p.idDisciplina == disciplina) && p.aula.Equals(ddlAulasSaida.SelectedItem.Text)).ToList();
                               
                                if (lista != null)
                                {
                                    grdSaida.DataSource = lista;
                                    grdSaida.DataBind();

                                    if (lista.Count > 0)
                                    {
                                        grdSaida.UseAccessibleHeader = true;
                                        grdSaida.HeaderRow.TableSection = TableRowSection.TableHeader;

                                        TableCellCollection cells = grdSaida.HeaderRow.Cells;
                                        cells[0].Attributes.Add("data-class", "expand");
                                        cells[1].Attributes.Add("data-sort-ignore", "true");
                                        cells[2].Attributes.Add("data-sort-ignore", "true");
                                        cells[3].Attributes.Add("data-sort-ignore", "true");
                                        cells[4].Attributes.Add("data-sort-ignore", "true");
                                        cells[5].Attributes.Add("data-hide", "all");
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
        //Preenchimento da Grid de Produtos Extras.
        private void FillExcecao()
        {
            IList<PLANEJAMENTO> lista = new List<PLANEJAMENTO>();
            IList<PLANEJAMENTO_AUXILIAR> listAuxiliar = new List<PLANEJAMENTO_AUXILIAR>();
            int turma = Convert.ToInt32(ddlSaidaCodTurma.SelectedValue);

            if (!string.IsNullOrEmpty(ddlDisciplinaSaida.SelectedValue))
            {
                int disciplina = Convert.ToInt32(ddlDisciplinaSaida.SelectedValue);
                long instrutor = Convert.ToInt64(ddlInstrutorSaida.SelectedValue);
                string aula = ddlAulasSaida.SelectedValue;

                if (turma > 0 && disciplina >= 0 && instrutor >= 0 && !string.IsNullOrEmpty(txtPeriodoSaida.Text) && !string.IsNullOrEmpty(txtPeriodoSaida1.Text) && !string.IsNullOrEmpty(txtTurnoSaida.Text) && !string.IsNullOrEmpty(txtNAlunosSaida.Text) && !string.IsNullOrEmpty(ddlAulasSaida.SelectedValue))
                {
                    try
                    {
                        using (estoqueEntities conexao = new estoqueEntities())
                        {
                            if (!string.IsNullOrEmpty(ddlSaidaCodTurma.SelectedValue))
                            {
                                lista = conexao.PLANEJAMENTO.Where(p => p.codTurma.Equals(ddlSaidaCodTurma.SelectedItem.Text) && (p.idDisciplina != null && p.idDisciplina == disciplina) && p.aula.Equals(ddlAulasSaida.SelectedItem.Text)).ToList();
                                foreach (var item in lista)
                                {
                                    PLANEJAMENTO_AUXILIAR pAuxiliar = conexao.PLANEJAMENTO_AUXILIAR.Where(p => p.idPlanejamento == item.id).FirstOrDefault();

                                    if (pAuxiliar != null)
                                    {
                                        listAuxiliar.Add(pAuxiliar);
                                    }
                                }
                            }
                        }
                        if (listAuxiliar != null)
                        {
                            grdExcecao.DataSource = listAuxiliar;
                            grdExcecao.DataBind();

                            if (listAuxiliar.Count > 0)
                            {
                                grdExcecao.UseAccessibleHeader = true;
                                grdExcecao.HeaderRow.TableSection = TableRowSection.TableHeader;

                                TableCellCollection cells = grdExcecao.HeaderRow.Cells;
                                cells[0].Attributes.Add("data-class", "expand");
                                cells[1].Attributes.Add("data-sort-ignore", "true");
                                cells[2].Attributes.Add("data-sort-ignore", "true");
                                cells[3].Attributes.Add("data-sort-ignore", "true");
                                cells[4].Attributes.Add("data-hide", "all");
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
        //private void CarregarProduto(string pLetra)
        //{
        //    FillSaida();
        //    FillExcecao();
        //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ActiveOne", "$(function(){ LetterActive('collapseOne', '" + pLetra + "'); });", true);
        //}

        //protected void btn_collapseOne_Click(object sender, EventArgs e)
        //{
        //    CarregarProduto(((Button)sender).Text);
        //}
        protected void ddlSaidaCurso_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillTurma();

            ddlSaidaCodTurma_SelectedIndexChanged(this, null);
        }
        protected void ddlSaidaCodTurma_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ddlSaidaCodTurma.SelectedValue.Equals(""))
            {
                using (estoqueEntities conexao = new estoqueEntities())
                {
                    int x = Convert.ToInt32(ddlSaidaCodTurma.SelectedValue);
                    TURMA preencherTurma = conexao.TURMA.Where(p => p.idTurma == x).FirstOrDefault();

                    if (!ddlSaidaCodTurma.SelectedValue.Equals("0"))
                    {
                        txtPeriodoSaida.Text = Convert.ToString(String.Format("{0:dd/MM/yyyy}", preencherTurma.dataInicial));
                        txtPeriodoSaida1.Text = Convert.ToString(String.Format("{0:dd/MM/yyyy}", preencherTurma.dataFinal));
                        txtTurnoSaida.Text = preencherTurma.turno;
                        txtNAlunosSaida.Text = Convert.ToString(preencherTurma.numAlunos);
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/>Necessário o preenchimento de todos os campos.', 'warning');", true);
                    }
                }
            }

            FillDisciplina();
            ddlDisciplina_SelectedIndexChanged(this, null);
        }
        //Evento do DropDown Instrutor.
        protected void ddlInstrutor_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillSaida();
            FillExcecao();
        }
        //Evento do DropDown Aulas.
        protected void ddlAulasSaida_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillSaida();
            FillExcecao();
            FillListaSaida("A");
        }
        //Preenchimento da GridView com dados das tabelas Produto / Planejamento_Itens_Extra / Itens_Entrada
        protected void grdSaida_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                using (estoqueEntities conexao = new estoqueEntities())
                {
                    PLANEJAMENTO planejar = (PLANEJAMENTO)e.Row.DataItem;

                    PRODUTO produto = conexao.PRODUTO.Where(p => p.codSei == planejar.codSei).FirstOrDefault();

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
                    PLANEJAMENTO_ITENS_EXTRA itemExtra = conexao.PLANEJAMENTO_ITENS_EXTRA.Where(p => p.idPlanejamento == planejar.id).FirstOrDefault();

                    if (itemExtra != null)
                    {
                        e.Row.Cells[4].Text = itemExtra.quantidade.ToString();
                        e.Row.Cells[5].Text = itemExtra.justificativa;
                    }
                    else
                    {
                        e.Row.Cells[4].Text = string.Empty;
                        e.Row.Cells[5].Text = string.Empty;
                    }

                    ITENS_ENTRADA iEntrada = conexao.ITENS_ENTRADA.Where(p => p.codSei == planejar.codSei).FirstOrDefault();

                    if (iEntrada != null)
                    {
                        int entEstoque = conexao.ITENS_ENTRADA.Where(p => p.codSei == planejar.codSei && p.idDeposito == iEntrada.idDeposito).Sum(p => p.quantEntrada.HasValue ? p.quantEntrada.Value : 0);

                        e.Row.Cells[1].Text = iEntrada.localizacao;
                        e.Row.Cells[6].Text = entEstoque.ToString();
                        if (entEstoque <= 30)
                        {
                            e.Row.Cells[6].ForeColor = Color.Red;
                        }
                        else
                        {
                            e.Row.Cells[6].ForeColor = Color.Black;
                        }
                    }
                    else
                    {
                        e.Row.Cells[6].Text = string.Empty;
                        e.Row.Cells[1].Text = string.Empty;
                    }
                }
            }
        }
        protected void grdSaida_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }
        //Preenchimento da GridView com as Tabelas Produto /Itens_Entrada.
        protected void grdExcecao_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                using (estoqueEntities conexao = new estoqueEntities())
                {
                    PLANEJAMENTO_AUXILIAR planejar = (PLANEJAMENTO_AUXILIAR)e.Row.DataItem;

                    PLANEJAMENTO pAuxiliar = conexao.PLANEJAMENTO.Where(p => p.id == planejar.idPlanejamento).FirstOrDefault();
                    if (pAuxiliar != null)
                    {
                        PRODUTO prodAuxiliar = conexao.PRODUTO.Where(p => p.codSei == planejar.codSei).FirstOrDefault();

                        e.Row.Cells[0].Text = prodAuxiliar.produto;
                        e.Row.Cells[2].Text = prodAuxiliar.unidMedida;
                        e.Row.Cells[3].Text = planejar.quantidade.ToString();
                        e.Row.Cells[4].Text = planejar.justificativa.ToString();
                    }
                    else
                    {
                        e.Row.Cells[0].Text = string.Empty;
                        e.Row.Cells[2].Text = string.Empty;
                        e.Row.Cells[3].Text = string.Empty;
                        e.Row.Cells[4].Text = string.Empty;
                    }
                    ITENS_ENTRADA iEntrada = conexao.ITENS_ENTRADA.Where(p => p.codSei == planejar.codSei).FirstOrDefault();

                    if (iEntrada != null)
                    {
                        int entEstoque = conexao.ITENS_ENTRADA.Where(p => p.codSei == planejar.codSei && p.idDeposito == iEntrada.idDeposito).Sum(p => p.quantEntrada.HasValue ? p.quantEntrada.Value : 0);

                        e.Row.Cells[1].Text = iEntrada.localizacao;
                        e.Row.Cells[5].Text = entEstoque.ToString();
                        if (entEstoque <= 30)
                        {
                            e.Row.Cells[5].ForeColor = Color.Red;
                        }
                        else
                        {
                            e.Row.Cells[5].ForeColor = Color.Black;
                        }
                    }
                    else
                    {
                        e.Row.Cells[1].Text = string.Empty;
                        e.Row.Cells[5].Text = string.Empty;
                    }
                }
            }
        }
        //Evento do DropDown Disciplina.
        protected void ddlDisciplina_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (estoqueEntities conexao = new estoqueEntities())
            {
                if (!string.IsNullOrEmpty(ddlDisciplinaSaida.SelectedValue))
                {
                    int x = Convert.ToInt32(ddlDisciplinaSaida.SelectedValue);
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
                    ddlAulasSaida.DataValueField = "Value";
                    ddlAulasSaida.DataTextField = "Text";
                    ddlAulasSaida.DataSource = list;
                    ddlAulasSaida.DataBind();
                }
            }
            FillInstrutor();
            FillSaida();
            FillExcecao();
        }
        protected void grdExcecao_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }
        //Botão imprimir saída com Gerar PDF no ITextSharp.
        protected void btnImprimirSaida_Click(object sender, EventArgs e)
        {
            if (grdSaida.Rows.Count > 0 || grdExcecao.Rows.Count > 0)
            {
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=saida_materiais.pdf");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);

                StringWriter sw = new StringWriter();

                HtmlTextWriter hw = new HtmlTextWriter(sw);

                grdSaida.HeaderRow.Style.Add("font-family", "Arial, Helvetica, sans-serif;");
                grdSaida.HeaderRow.Style.Add("font-size", "8px");
                grdSaida.HeaderRow.Style.Add("color", "#284775");
                grdSaida.Style.Add("font-family", "Arial, Helvetica, sans-serif;");
                grdSaida.Style.Add("font-size", "7px");
                grdSaida.RenderControl(hw);

                if (grdExcecao.Rows.Count > 0)
                {
                    hw.WriteBreak();
                    hw.WriteLine("Produto Extra:");
                    hw.WriteBreak();
                    hw.WriteBreak();
                    grdExcecao.HeaderRow.Style.Add("font-family", "Arial, Helvetica, sans-serif;");
                    grdExcecao.HeaderRow.Style.Add("font-size", "8px");
                    grdExcecao.HeaderRow.Style.Add("color", "#284775");
                    grdExcecao.Style.Add("font-family", "Arial, Helvetica, sans-serif;");
                    grdExcecao.Style.Add("font-size", "6px");
                    grdExcecao.RenderControl(hw);
                }
                hw.WriteBreak();
                hw.WriteBreak();
                hw.WriteBreak();
                hw.Write("Observação: ");
                hw.WriteBreak();
                hw.WriteBreak();
                hw.Write("__________________________________________________________________________________________________________");
                hw.Write("__________________________________________________________________________________________________________");
                hw.Write("____________________________________________________________________________________________________________________________________");
                hw.WriteBreak();
                hw.WriteBreak();
                hw.Write("Assinatura do Instrutor(a): ");
                hw.WriteBreak();
                hw.WriteBreak();
                hw.Write("_____________________________________________");

                StringReader sr = new StringReader(sw.ToString());

                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);

                string imageFilePath = Server.MapPath(".") + "/Content/images/header_listaBasica.png";

                HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                PdfWriter.GetInstance(pdfDoc, Response.OutputStream);

                pdfDoc.Open();
                pdfDoc.NewPage();

                pdfDoc.Add(new Paragraph());
                pdfDoc.Add(new Chunk());

                iTextSharp.text.Image imagem1 = iTextSharp.text.Image.GetInstance(imageFilePath);
                pdfDoc.Add(imagem1);

                pdfDoc.Add(new Chunk("Turma: " + ddlSaidaCodTurma.SelectedItem.Text));
                pdfDoc.Add(new Paragraph());
                pdfDoc.Add(new Chunk());
                pdfDoc.Add(new Chunk("Disciplina: " + ddlDisciplinaSaida.SelectedItem.Text));
                pdfDoc.Add(new Paragraph());
                pdfDoc.Add(new Chunk());
                pdfDoc.Add(new Chunk("Instrutor(a): " + ddlInstrutorSaida.SelectedItem.Text));
                pdfDoc.Add(new Paragraph());
                pdfDoc.Add(new Chunk());
                pdfDoc.Add(new Chunk("Data: " + DateTime.Now));
                pdfDoc.Add(new Paragraph());
                pdfDoc.Add(new Chunk());
                pdfDoc.Add(new Chunk("" + ddlAulasSaida.SelectedItem.Text));
                pdfDoc.Add(new Paragraph());
                pdfDoc.Add(new Chunk());

                htmlparser.Parse(sr);

                pdfDoc.Close();
                Response.Write(pdfDoc);
                Response.Flush();
                Response.Clear();
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/>Necessário o preenchimento de todos os campos.', 'warning');", true);
            }
        }
        public override void VerifyRenderingInServerForm(Control control)
        {
            return;
        }

        //****************************** PREENCHER GRIDVIEW SAIDA_AUXILIAR **************************/
        private void FillListaSaida(String codBarra)
        {
            int turma = Convert.ToInt32(ddlSaidaCodTurma.SelectedValue);
            int disciplina = Convert.ToInt32(ddlDisciplinaSaida.SelectedValue);
            string aula = ddlAulasSaida.SelectedItem.Text;
            long instrutor = Convert.ToInt64(ddlInstrutorSaida.SelectedValue);

            if (turma > 0 && disciplina > 0 && instrutor > 0 && !string.IsNullOrEmpty(txtPeriodoSaida.Text) && !string.IsNullOrEmpty(txtPeriodoSaida1.Text) && !string.IsNullOrEmpty(txtTurnoSaida.Text) && !string.IsNullOrEmpty(txtNAlunosSaida.Text) && !string.IsNullOrEmpty(ddlAulasSaida.SelectedValue))
            {
                try
                {
                    using (estoqueEntities conexao = new estoqueEntities())
                    {
                        // IList<int> listProduto = conexao.PRODUTO.Where(p => p.codBarra.Equals(codBarra)).Distinct().Select(p => p.codSei).ToList();
                        IList<int> listTurma = conexao.PLANEJAMENTO.Where(p => p.idTurma == turma).Distinct().Select(p => p.idTurma).ToList();

                        if (!string.IsNullOrEmpty(ddlSaidaCodTurma.SelectedValue))
                        {
                            int x = Convert.ToInt32(ddlSaidaCodTurma.SelectedValue);

                            IList<SAIDA_AUXILIAR> saidaAuxiliar = new List<SAIDA_AUXILIAR>();

                            saidaAuxiliar = conexao.SAIDA_AUXILIAR.Where(p => listTurma.Contains(p.idTurma) && p.aula.Equals(aula)).ToList();

                            if (saidaAuxiliar != null)
                            {
                                grdRealizarSaida.DataSource = saidaAuxiliar;
                                grdRealizarSaida.DataBind();

                                if (saidaAuxiliar.Count > 0)
                                {
                                    grdRealizarSaida.UseAccessibleHeader = true;
                                    grdRealizarSaida.HeaderRow.TableSection = TableRowSection.TableHeader;

                                    TableCellCollection cells = grdRealizarSaida.HeaderRow.Cells;
                                    cells[0].Attributes.Add("data-class", "expand");
                                    cells[1].Attributes.Add("data-sort-ignore", "true");
                                    cells[2].Attributes.Add("data-sort-ignore", "true");
                                    cells[3].Attributes.Add("data-sort-ignore", "true");
                                    cells[4].Attributes.Add("data-sort-ignore", "true");
                                    cells[5].Attributes.Add("data-sort-ignore", "true");
                                    cells[6].Attributes.Add("data-sort-ignore", "true");
                                    cells[7].Attributes.Add("data-hide", "all");
                                }
                            }
                        }
                    }

                    //grdRealizarSaida.Visible = true;
                }
                catch (Exception ex)
                {
                    //grdRealizarSaida.Visible = false;
                    throw ex;
                }
            }
            else
            {
                //grdRealizarSaida.Visible = false;
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/>Necessário o preenchimento de todos os campos.', 'warning');", true);
            }
        }

        //******************************** PREENCHER TABELA SAIDA_AUXILIAR ***********/
        private string FillSaidaAuxiliar(string codBarra)
        {
            string msg = string.Empty;

            UserAD usuarioSistemas = Session["UsuarioSistemasLegados"] as UserAD;

            using (estoqueEntities conexao = new estoqueEntities())
            {
                USUARIO user = conexao.USUARIO.Where(p => p.usuario.Equals(usuarioSistemas.Samaccountname)).FirstOrDefault();

                PRODUTO prodAux = conexao.PRODUTO.Where(p => p.codBarra.Equals(codBarra)).FirstOrDefault();

                if (prodAux != null)
                {
                    if (!String.IsNullOrEmpty(prodAux.codBarra))
                    {

                        int x = Convert.ToInt32(ddlSaidaCodTurma.SelectedValue);
                        PLANEJAMENTO planejarSaida = conexao.PLANEJAMENTO.Where(p => p.idTurma == x && p.aula.Equals(ddlAulasSaida.SelectedItem.Text) && p.codSei == prodAux.codSei).FirstOrDefault();
                        PLANEJAMENTO_AUXILIAR planAuxiliar = conexao.PLANEJAMENTO_AUXILIAR.Where(p => p.idTurma == x && p.aula.Equals(ddlAulasSaida.SelectedItem.Text) && p.codSei == prodAux.codSei).FirstOrDefault();
                        if (planejarSaida != null || planAuxiliar != null)
                        {
                            int codsei = planejarSaida != null ? planejarSaida.codSei : planAuxiliar != null ? planAuxiliar.codSei : 0;
                            int turma = planejarSaida != null ? planejarSaida.idTurma : planAuxiliar != null ? planAuxiliar.idTurma : 0;
                            string aula = planejarSaida != null ? planejarSaida.aula : planAuxiliar != null ? planAuxiliar.aula : string.Empty;
                            int curso = planejarSaida != null ? planejarSaida.idCurso : planAuxiliar != null ? planAuxiliar.idCurso : 0;
                            int unidade = planejarSaida != null ? planejarSaida.idUnidade : planAuxiliar != null ? planAuxiliar.idUnidade : 0;

                            SAIDA_AUXILIAR saidaAuxiliar = conexao.SAIDA_AUXILIAR.Where(p => p.codSei == codsei && p.idTurma == turma && p.aula == aula).FirstOrDefault();

                            if (saidaAuxiliar == null)
                            {
                                SAIDA_AUXILIAR saida = new SAIDA_AUXILIAR();

                                saida.codSei = codsei;
                                saida.idCurso = curso;
                                saida.idTurma = turma;
                                saida.idUnidade = unidade;
                                saida.aula = aula;

                                conexao.Entry<SAIDA_AUXILIAR>(saida).State = System.Data.Entity.EntityState.Added;

                                conexao.SaveChanges();

                                txtCodBarra.Text = string.Empty;
                            }
                        }

                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/> Produto não cadastrado no planejamento solicitado.', 'warning');", true);
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/> O Produto já está cadastrado no Sistema.', 'warning');", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/> Produto não cadastrado no planejamento.', 'warning');", true);
                }
            }

            return msg;
        }
        protected void btnEnter_Click(object sender, EventArgs e)
        {
            string codBarra = txtCodBarra.Text;
            string msg = FillSaidaAuxiliar(codBarra);
            if (string.IsNullOrEmpty(msg))
            {
                FillListaSaida(codBarra);
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/>" + msg + "', 'warning');", true);
            }
            FillSaida();
            FillExcecao();
        }

        //*********** SALVAR SAÍDA ****************************************/
        protected void btnFinalizarSaida_Click(object sender, EventArgs e)
        {
            using (estoqueEntities conexao = new estoqueEntities())
            {
                int x = Convert.ToInt32(ddlSaidaCodTurma.SelectedValue);
                SAIDA verificarSaida = conexao.SAIDA.Where(p => p.idTurma == x && p.aula.Equals(ddlAulasSaida.SelectedItem.Text)).FirstOrDefault();

                if (verificarSaida == null)
                {
                    using (var tran = conexao.Database.BeginTransaction())
                    {
                        try
                        {
                            UserAD usuarioSistemas = Session["UsuarioSistemasLegados"] as UserAD;
                            USUARIO user = conexao.USUARIO.Where(p => p.usuario.Equals(usuarioSistemas.Samaccountname)).FirstOrDefault();

                            IList<SAIDA_AUXILIAR> saidaAuxiliar = conexao.SAIDA_AUXILIAR.Where(p => p.idTurma == x).ToList();

                            #region -- Salvar --
                            foreach (var item in saidaAuxiliar)
                            {
                                PLANEJAMENTO planejarSaida = conexao.PLANEJAMENTO.Where(p => p.codTurma.Equals(ddlSaidaCodTurma.SelectedItem.Text) && p.aula.Equals(ddlAulasSaida.SelectedItem.Text) && p.codSei == item.codSei).FirstOrDefault();
                                PLANEJAMENTO_AUXILIAR planAuxiliar = conexao.PLANEJAMENTO_AUXILIAR.Where(p => p.idTurma == x && p.aula.Equals(ddlAulasSaida.SelectedItem.Text) && p.codSei == item.codSei).FirstOrDefault();

                                ITENS_ENTRADA itensEntrada;
                                if (planejarSaida == null)
                                {

                                    itensEntrada = conexao.ITENS_ENTRADA.Where(p => p.codSei == planAuxiliar.codSei && p.quantEntrada > 0).FirstOrDefault();
                                }
                                else
                                {
                                    itensEntrada = conexao.ITENS_ENTRADA.Where(p => p.codSei == planejarSaida.codSei && p.quantEntrada > 0).FirstOrDefault();
                                }

                                if (itensEntrada != null)
                                {
                                    if (planejarSaida != null)
                                    {
                                        PLANEJAMENTO_ITENS_EXTRA plaItenExtra = conexao.PLANEJAMENTO_ITENS_EXTRA.Where(p => p.idPlanejamento == planejarSaida.id).FirstOrDefault();

                                        if (itensEntrada != null)
                                        {
                                            SAIDA realizarSaida = new SAIDA();

                                            realizarSaida.dataSaida = DateTime.Now;
                                            int valorExtra = 0;
                                            if (plaItenExtra != null)
                                            {
                                                valorExtra = plaItenExtra.quantidade != null ? plaItenExtra.quantidade.Value : 0;
                                            }
                                            realizarSaida.despesaRealizada = itensEntrada.valorUnitario * Convert.ToDecimal(planejarSaida.quantidade + valorExtra);
                                            realizarSaida.quantidadeRetorno = 0;
                                            realizarSaida.valorRetorno = 0;
                                            realizarSaida.aula = item.aula;
                                            realizarSaida.idCurso = item.idCurso;
                                            realizarSaida.idUnidade = item.idUnidade;
                                            realizarSaida.idTurma = item.idTurma;
                                            realizarSaida.codSei = item.codSei;
                                            realizarSaida.idUsuario = user.id;
                                            realizarSaida.status = true;

                                            conexao.Entry<SAIDA>(realizarSaida).State = System.Data.Entity.EntityState.Added;

                                            itensEntrada.quantEntrada -= (planejarSaida.quantidade + valorExtra);
                                            conexao.Entry<ITENS_ENTRADA>(itensEntrada).State = System.Data.Entity.EntityState.Modified;
                                        }
                                    }
                                    else if (planAuxiliar != null)
                                    {
                                        SAIDA realizarSaida = new SAIDA();

                                        realizarSaida.dataSaida = DateTime.Now;
                                        realizarSaida.despesaRealizada = itensEntrada.valorUnitario * Convert.ToDecimal(planAuxiliar.quantidade);
                                        realizarSaida.quantidadeRetorno = 0;
                                        realizarSaida.valorRetorno = 0;
                                        realizarSaida.aula = item.aula;
                                        realizarSaida.idCurso = item.idCurso;
                                        realizarSaida.idUnidade = item.idUnidade;
                                        realizarSaida.idTurma = item.idTurma;
                                        realizarSaida.codSei = item.codSei;
                                        realizarSaida.idUsuario = user.id;
                                        realizarSaida.status = true;

                                        conexao.Entry<SAIDA>(realizarSaida).State = System.Data.Entity.EntityState.Added;

                                        itensEntrada.quantEntrada -= planAuxiliar.quantidade;
                                        conexao.Entry<ITENS_ENTRADA>(itensEntrada).State = System.Data.Entity.EntityState.Modified;
                                    }
                                }
                                else
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/>O produto possui estoque insuficiente para efetuar a saída!', 'warning');", true);
                                }
                            }

                            #endregion

                            #region -- Remover --
                            foreach (var item in saidaAuxiliar)
                            {
                                conexao.Entry<SAIDA_AUXILIAR>(item).State = System.Data.Entity.EntityState.Deleted;
                            }

                            conexao.SaveChanges();
                            tran.Commit();

                            #endregion

                        }
                        catch (Exception)
                        {
                            tran.Rollback();
                            throw;
                        }

                    }
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/> Saída já realizada no sistema.', 'warning');", true);
                }

                FillSaida();
                FillExcecao();
                FillListaSaida("A");
            }
        }
        protected void txtQuantidade_TextChanged(object sender, EventArgs e)
        {

        }
        protected void grdRealizarSaida_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                SAIDA_AUXILIAR saidaAuxiliar = (SAIDA_AUXILIAR)e.Row.DataItem;

                using (estoqueEntities conexao = new estoqueEntities())
                {
                    PLANEJAMENTO planejamento = conexao.PLANEJAMENTO.Where(p => p.idTurma == saidaAuxiliar.idTurma && p.codSei == saidaAuxiliar.codSei).FirstOrDefault();
                    PLANEJAMENTO_ITENS_EXTRA plaItenExtra = null;
                    PLANEJAMENTO_AUXILIAR pAuxiliar = conexao.PLANEJAMENTO_AUXILIAR.Where(p => p.idTurma == saidaAuxiliar.idTurma && p.codSei == saidaAuxiliar.codSei).FirstOrDefault();

                    if (planejamento != null)
                    {
                        plaItenExtra = conexao.PLANEJAMENTO_ITENS_EXTRA.Where(p => p.idPlanejamento == planejamento.id && p.codSei == saidaAuxiliar.codSei).FirstOrDefault();

                        e.Row.Cells[3].Text = planejamento.quantidade.ToString();
                        int quant = plaItenExtra != null ? plaItenExtra.quantidade.Value : 0;
                        e.Row.Cells[5].Text = (planejamento.quantidade + quant).ToString();
                    }

                    if (pAuxiliar != null)
                    {
                        int quantAuxiliar = pAuxiliar != null ? pAuxiliar.quantidade : 0;
                        e.Row.Cells[3].Text = pAuxiliar.quantidade.ToString();
                        e.Row.Cells[5].Text = (quantAuxiliar).ToString();
                    }

                    PRODUTO produto = conexao.PRODUTO.Where(p => p.codSei.Equals(saidaAuxiliar.codSei)).FirstOrDefault();
                    if (produto != null)
                    {
                        e.Row.Cells[0].Text = produto.produto;
                        e.Row.Cells[2].Text = produto.unidMedida;
                    }
                    ITENS_ENTRADA iEntrada = conexao.ITENS_ENTRADA.Where(p => p.codSei == saidaAuxiliar.codSei).FirstOrDefault();

                    if (iEntrada != null)
                    {
                        int entEstoque = conexao.ITENS_ENTRADA.Where(p => p.codSei == saidaAuxiliar.codSei && p.idDeposito == iEntrada.idDeposito).Sum(p => p.quantEntrada.HasValue ? p.quantEntrada.Value : 0);

                        e.Row.Cells[1].Text = iEntrada.localizacao;
                        e.Row.Cells[6].Text = entEstoque.ToString();
                        if (entEstoque <= 40)
                        {
                            e.Row.Cells[6].ForeColor = Color.Red;
                        }
                        else
                        {
                            e.Row.Cells[6].ForeColor = Color.Black;
                        }
                    }
                    else
                    {
                        e.Row.Cells[1].Text = string.Empty;
                        e.Row.Cells[6].Text = string.Empty;
                    }

                    if (plaItenExtra != null)
                    {
                        e.Row.Cells[4].Text = plaItenExtra.quantidade.ToString();
                        e.Row.Cells[7].Text = plaItenExtra.justificativa.ToString();
                    }
                    else
                    {
                        e.Row.Cells[4].Text = string.Empty;
                        e.Row.Cells[7].Text = string.Empty;
                    }
                }
            }
        }
        protected void grdRealizarSaida_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Excluir"))
            {
                using (estoqueEntities conexao = new estoqueEntities())
                {
                    int id = Convert.ToInt32(e.CommandArgument);

                    SAIDA_AUXILIAR saidaExcluir = conexao.SAIDA_AUXILIAR.Where(p => p.id == id).FirstOrDefault();

                    if (saidaExcluir != null)
                    {
                        conexao.Entry<SAIDA_AUXILIAR>(saidaExcluir).State = System.Data.Entity.EntityState.Deleted;
                        conexao.SaveChanges();
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/>O procedimento não pode ser realizado, gentileza entrar em contato com o administrador.', 'warning');", true);
                    }
                }
            }
            FillSaida();
            FillExcecao();
            FillListaSaida("A");
        }
    }
}
