using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ControleEstoque.Web.Models;
using ControleEstoque.Web.Models.ADModel;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using RegistroFrequencia.Web.App_Code;

namespace ControleEstoque.Web
{
    public partial class Relatorio : System.Web.UI.Page
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
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            ddlCodTurmaRelatorio.DataTextField = "codTurma";
            ddlCodTurmaRelatorio.DataValueField = "idTurma";
            ddlCodTurmaRelatorio.DataSource = listTurma;
            ddlCodTurmaRelatorio.DataBind();

            ddlCodTurmaRelatorio.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Selecione", "0"));

        }
        private void FillDisciplina()
        {
            IList<DISCIPLINA> listTurma = new List<DISCIPLINA>();

            using (estoqueEntities conexao = new estoqueEntities())
            {
                if (!string.IsNullOrEmpty(ddlCodTurmaRelatorio.SelectedValue))
                {
                    int x = Convert.ToInt32(ddlCodTurmaRelatorio.SelectedValue);
                    listTurma = conexao.DISCIPLINA.Where(p => p.idTurma == x).ToList();
                }
            }
            ddlDisciplinaRelatorio.DataTextField = "disciplina";
            ddlDisciplinaRelatorio.DataValueField = "idDisciplina";
            ddlDisciplinaRelatorio.DataSource = listTurma;
            ddlDisciplinaRelatorio.DataBind();
        }
        private void FillAula()
        {
            try
            {
                IList<TURMA> listAula = new List<TURMA>();

                using (estoqueEntities conexao = new estoqueEntities())
                {
                    int x = Convert.ToInt32(ddlCodTurmaRelatorio.SelectedValue);
                    listAula = conexao.TURMA.Where(p => p.codTurma.Equals(x)).ToList();

                    for (int i = 0; i < listAula.Count; i++)
                    {

                    }
                    ddlCodTurmaRelatorio.DataTextField = "codTurma";
                    ddlCodTurmaRelatorio.DataValueField = "idTurma";
                    ddlCodTurmaRelatorio.DataSource = listAula;
                    ddlCodTurmaRelatorio.DataBind();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void FillInstrutor()
        {
            IList<DISCIPLINA_INSTRUTOR> listAula = new List<DISCIPLINA_INSTRUTOR>();

            using (estoqueEntities conexao = new estoqueEntities())
            {
                if (!string.IsNullOrEmpty(ddlDisciplinaRelatorio.SelectedValue))
                {
                    int y = Convert.ToInt32(ddlCodTurmaRelatorio.SelectedValue);
                    int x = Convert.ToInt32(ddlDisciplinaRelatorio.SelectedValue);
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
                    ddlInstrutorRelatorio.DataTextField = "instrutor";
                    ddlInstrutorRelatorio.DataValueField = "idInstrutor";
                    ddlInstrutorRelatorio.DataSource = instrutorlista;
                    ddlInstrutorRelatorio.DataBind();
                }
            }
            FillRetornoSaida();
        }
        private void FillRetornoSaida()
        {
            int turma = Convert.ToInt32(ddlCodTurmaRelatorio.SelectedValue);

            if (!string.IsNullOrEmpty(ddlDisciplinaRelatorio.SelectedValue))
            {
                int disciplina = Convert.ToInt32(ddlDisciplinaRelatorio.SelectedValue);
                long instrutor = Convert.ToInt64(ddlInstrutorRelatorio.SelectedValue);

                if (turma > 0 && disciplina >= 0 && instrutor >= 0 && !string.IsNullOrEmpty(txtRetornoPeriodo.Text) && !string.IsNullOrEmpty(txtRetornoPeriodo1.Text) && !string.IsNullOrEmpty(txtRetornoTurno.Text) && !string.IsNullOrEmpty(txtRetornoNAlunos.Text))
                {
                    try
                    {
                        using (estoqueEntities conexao = new estoqueEntities())
                        {
                            if (!string.IsNullOrEmpty(ddlCodTurmaRelatorio.SelectedValue))
                            {
                                IList<SAIDA> lista = new List<SAIDA>();

                                lista = conexao.SAIDA.Where(p => p.idTurma == turma).ToList();

                                if (lista != null)
                                {
                                    grdRelatorio.DataSource = lista;
                                    grdRelatorio.DataBind();

                                    if (lista.Count > 0)
                                    {
                                        grdRelatorio.UseAccessibleHeader = true;
                                        grdRelatorio.HeaderRow.TableSection = TableRowSection.TableHeader;

                                        TableCellCollection cells = grdRelatorio.HeaderRow.Cells;
                                        cells[0].Attributes.Add("data-class", "expand");
                                        cells[1].Attributes.Add("data-sort-ignore", "true");
                                        cells[2].Attributes.Add("data-sort-ignore", "true");
                                        cells[3].Attributes.Add("data-sort-ignore", "true");
                                        cells[4].Attributes.Add("data-sort-ignore", "true");
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
        protected void ddlCodTurmaRelatorio_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ddlCodTurmaRelatorio.SelectedValue.Equals(""))
            {
                using (estoqueEntities conexao = new estoqueEntities())
                {
                    int x = Convert.ToInt32(ddlCodTurmaRelatorio.SelectedValue);
                    TURMA preencherTurma = conexao.TURMA.Where(p => p.idTurma == x).FirstOrDefault();

                    if (!ddlCodTurmaRelatorio.SelectedValue.Equals("0"))
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
            ddlDisciplinaRelatorio_SelectedIndexChanged(this, null);
        }
        protected void ddlDisciplinaRelatorio_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillInstrutor();
        }
        protected void ddlInstrutorRelatorio_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        protected void grdRelatorio_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                using (estoqueEntities conexao = new estoqueEntities())
                {
                    SAIDA lista = (SAIDA)e.Row.DataItem;

                    PLANEJAMENTO planejamento = conexao.PLANEJAMENTO.Where(p => p.idTurma == lista.idTurma && p.codSei == lista.codSei).FirstOrDefault();
                    PLANEJAMENTO_ITENS_EXTRA plaItenExtra = null;
                    PLANEJAMENTO_AUXILIAR pAuxiliar = conexao.PLANEJAMENTO_AUXILIAR.Where(p => p.idTurma == lista.idTurma && p.codSei == lista.codSei).FirstOrDefault();
                    if (planejamento != null)
                    {
                        plaItenExtra = conexao.PLANEJAMENTO_ITENS_EXTRA.Where(p => p.idPlanejamento == planejamento.id && p.codSei == lista.codSei).FirstOrDefault();

                        int quant = plaItenExtra != null ? plaItenExtra.quantidade.Value : 0;
                        e.Row.Cells[1].Text = (planejamento.quantidade + quant).ToString();
                    }
                    if (pAuxiliar != null)
                    {
                        int quantAuxiliar = pAuxiliar != null ? pAuxiliar.quantidade : 0;

                        e.Row.Cells[1].Text = (quantAuxiliar).ToString();
                    }
                    PRODUTO produto = conexao.PRODUTO.Where(p => p.codSei == lista.codSei).FirstOrDefault();
                    if (produto != null)
                    {
                        e.Row.Cells[0].Text = produto.produto;
                    }
                    else
                    {
                        e.Row.Cells[0].Text = string.Empty;
                    }
                }
            }
        }
        protected void grdRelatorio_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }
        public override void VerifyRenderingInServerForm(Control control)
        {
            return;
        }
        protected void btnImprimirSaida_Click(object sender, ImageClickEventArgs e)
        {
            if (grdRelatorio.Rows.Count > 0 )
            {
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=relatorio.pdf");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);

                StringWriter sw = new StringWriter();

                HtmlTextWriter hw = new HtmlTextWriter(sw);

                grdRelatorio.HeaderRow.Style.Add("font-family", "Arial, Helvetica, sans-serif;");
                grdRelatorio.HeaderRow.Style.Add("font-size", "8px");
                grdRelatorio.HeaderRow.Style.Add("color", "#284775");
                grdRelatorio.Style.Add("font-family", "Arial, Helvetica, sans-serif;");
                grdRelatorio.Style.Add("font-size", "7px");
                grdRelatorio.RenderControl(hw);

                StringReader sr = new StringReader(sw.ToString());

                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);

                string imageFilePath = Server.MapPath(".") + "/Content/images/header_relatorio.png";

                HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                PdfWriter.GetInstance(pdfDoc, Response.OutputStream);

                pdfDoc.Open();
                pdfDoc.NewPage();

                pdfDoc.Add(new Paragraph());
                pdfDoc.Add(new Chunk());

                iTextSharp.text.Image imagem1 = iTextSharp.text.Image.GetInstance(imageFilePath);
                pdfDoc.Add(imagem1);

                pdfDoc.Add(new Chunk("Turma: " + ddlCodTurmaRelatorio.SelectedItem.Text));
                pdfDoc.Add(new Paragraph());
                pdfDoc.Add(new Chunk());
                pdfDoc.Add(new Chunk("Disciplina: " + ddlDisciplinaRelatorio.SelectedItem.Text));
                pdfDoc.Add(new Paragraph());
                pdfDoc.Add(new Chunk());
                pdfDoc.Add(new Chunk("Instrutor(a): " + ddlInstrutorRelatorio.SelectedItem.Text));
                pdfDoc.Add(new Paragraph());
                pdfDoc.Add(new Chunk());
                pdfDoc.Add(new Chunk("Data: " + DateTime.Now));
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
    }
}