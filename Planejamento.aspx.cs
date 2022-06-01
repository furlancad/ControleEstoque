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
using System.Web.Services;
using System.Web.Script.Services;

namespace ControleEstoque.Web
{
    public partial class Planejamento : System.Web.UI.Page
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
                    UserAD usuarioSistemas = Session["UsuarioSistemasLegados"] as UserAD;

                    if (usuarioSistemas != null)
                    {
                        if (usuarioSistemas.Memberof != null && (usuarioSistemas.Memberof.Contains(ConfigurationManager.AppSettings["CE-supervisor"])))
                        {
                            divSupervisor.Visible = true;
                            divUnidade.Visible = true;

                            FillUnidade();
                            FillCurso();
                            FillTurma();
                            FillDisciplina();
                            FillInstrutor();
                            FillAula();
                            FillListaBasica("A");
                        }
                        else
                        {
                            divSupervisor.Visible = false;
                            divUnidade.Visible = false;

                            FillUnidade();
                            FillCurso();
                            FillTurma();
                            FillDisciplina();
                            FillInstrutor();
                            FillAula();
                            FillListaBasica("A");
                        }
                    }
                    else
                    {
                        Response.Redirect("Login.aspx");
                    }
                }
            }
        }
        //Preenchimento do dropDown Unidade.
        private void FillUnidade()
        {
            IList<UNIDADE> listUnidade = new List<UNIDADE>();

            using (estoqueEntities conexao = new estoqueEntities())
            {
                listUnidade = conexao.UNIDADE.Distinct().ToList();
            }
            ddlUnidade.DataTextField = "unidade";
            ddlUnidade.DataValueField = "idUnidade";
            ddlUnidade.DataSource = listUnidade;
            ddlUnidade.DataBind();
        }
        //Preenchimento do DropDown Curso.
        private void FillCurso()
        {
            UserAD usuarioSistemas = Session["UsuarioSistemasLegados"] as UserAD;

            IList<CURSOS> listCurso = new List<CURSOS>();

            using (estoqueEntities conexao = new estoqueEntities())
            {
                if (usuarioSistemas != null)
                {
                    if (usuarioSistemas.Memberof != null && (usuarioSistemas.Memberof.Contains(ConfigurationManager.AppSettings["CE-supervisor"])))
                    {
                        int idunidade = Convert.ToInt32(ddlUnidade.SelectedValue);
                        listCurso = conexao.CURSOS.Where(P => P.idUnidade == idunidade).Distinct().ToList();
                    }
                    //else if (usuarioSistemas.Memberof != null && (usuarioSistemas.Memberof.Contains(ConfigurationManager.AppSettings["CE-instrutor"])))
                    //{
                    //    int idTurma = Convert.ToInt32(ddlCodTurma.SelectedValue);
                    //    listCurso = conexao.CURSOS.Where(P => P.idUnidade == ).Distinct().ToList();
                    //}
                    else
                    {
                        USUARIO user = conexao.USUARIO.Include("DEPOSITO").Where(p => p.usuario.Equals(usuarioSistemas.Samaccountname)).FirstOrDefault();

                        listCurso = conexao.CURSOS.Where(p => p.idUnidade == user.DEPOSITO.idUnidade).ToList();
                    }
                }
            }

            ddlCurso.DataTextField = "curso";
            ddlCurso.DataValueField = "idCurso";
            ddlCurso.DataSource = listCurso;
            ddlCurso.DataBind();

            ddlCurso.Items.Insert(0, new ListItem("Selecione", "0"));
        }
        //Preenchimento do DropDown Turma.
        private void FillTurma()
        {
            IList<TURMA> listTurma = new List<TURMA>();

            using (estoqueEntities conexao = new estoqueEntities())
            {
                UserAD usuarioSistemas = Session["UsuarioSistemasLegados"] as UserAD;

                if (usuarioSistemas != null)
                {
                    if (usuarioSistemas.Memberof != null && (usuarioSistemas.Memberof.Contains(ConfigurationManager.AppSettings["CE-supervisor"])))
                    {
                        int y = Convert.ToInt32(ddlUnidade.SelectedValue);
                        int x = Convert.ToInt32(ddlCurso.SelectedValue);
                        listTurma = conexao.TURMA.Where(p => p.idCurso.Equals(x) && p.idUnidade == y).ToList();
                    }
                    else if (usuarioSistemas.Memberof != null && (usuarioSistemas.Memberof.Contains(ConfigurationManager.AppSettings["CE-instrutor"]))) 
                    {
                        int? y = conexao.INSTRUTOR.Where(p => p.cpf.Equals(usuarioSistemas.Postofficebox)).Select(p => p.idUnidade).FirstOrDefault();
                        if (y != null)
                        {
                            int x = Convert.ToInt32(ddlCurso.SelectedValue);
                            listTurma = conexao.TURMA.Where(p => p.idCurso.Equals(x) && p.idUnidade == y).ToList();
                        }
                    }
                    else
                    {
                        USUARIO usuario = conexao.USUARIO.Where(p =>p.cpf.Equals(usuarioSistemas.Postofficebox)).FirstOrDefault();
                        if (usuario != null)
                        {
                            int x = Convert.ToInt32(ddlCurso.SelectedValue);
                            listTurma = conexao.TURMA.Where(p => p.idCurso.Equals(x) && p.idUnidade == usuario.DEPOSITO.idUnidade).ToList();
                        }
                    }
                }

                ddlCodTurma.DataTextField = "codTurma";
                ddlCodTurma.DataValueField = "idTurma";
                ddlCodTurma.DataSource = listTurma;
                ddlCodTurma.DataBind();

                ddlCodTurma.Items.Insert(0, new ListItem("Selecione", "0"));

                //FillListaBasica("#");
            }
        }
        //Preenchimento do Dropdowm Disciplina 
        private void FillDisciplina()
        {
            IList<DISCIPLINA> listTurma = new List<DISCIPLINA>();

            using (estoqueEntities conexao = new estoqueEntities())
            {
                if (!string.IsNullOrEmpty(ddlCodTurma.SelectedValue))
                {
                    int x = Convert.ToInt32(ddlCodTurma.SelectedValue);
                    listTurma = conexao.DISCIPLINA.Where(p => p.idTurma.Equals(x)).ToList();
                }
            }

            ddlDisciplina.DataTextField = "disciplina";
            ddlDisciplina.DataValueField = "idDisciplina";
            ddlDisciplina.DataSource = listTurma;
            ddlDisciplina.DataBind();

            ddlDisciplina.Items.Insert(0, new ListItem("Selecione", "0"));
        }
        //Preenchimento do DropDown Instrutor.
        private void FillInstrutor()
        {
            IList<DISCIPLINA_INSTRUTOR> listAula = new List<DISCIPLINA_INSTRUTOR>();

            using (estoqueEntities conexao = new estoqueEntities())
            {
                if (!string.IsNullOrEmpty(ddlDisciplina.SelectedValue))
                {
                    int y = Convert.ToInt32(ddlCodTurma.SelectedValue);
                    int x = Convert.ToInt32(ddlDisciplina.SelectedValue);
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
                    ddlInstrutores.DataTextField = "instrutor";
                    ddlInstrutores.DataValueField = "idInstrutor";
                    ddlInstrutores.DataSource = instrutorlista;
                    ddlInstrutores.DataBind();

                    ddlInstrutores.Items.Insert(0, new ListItem("Selecione", "0"));
                }
            }
        }
        //Preenchimento do DropDown Aula.
        private void FillAula()
        {
            IList<ListItem> list = new List<ListItem>();
            using (estoqueEntities conexao = new estoqueEntities())
            {
                if (!string.IsNullOrEmpty(ddlDisciplina.SelectedValue))
                {
                    int x = Convert.ToInt32(ddlDisciplina.SelectedValue);
                    DISCIPLINA preencherTurma = conexao.DISCIPLINA.Where(p => p.idDisciplina == x).FirstOrDefault();

                    if (preencherTurma != null)
                    {
                        Time horaInicial = new Time(preencherTurma.horaInicial);
                        Time horaFinal = new Time(preencherTurma.horaFinal);

                        Time hora = Time.TimeDiff(horaFinal, horaInicial);

                        //TimeSpan final = preencherTurma.dataFinal.Subtract(preencherTurma.dataInicial);
                        int number = preencherTurma.cargaHoraria / hora.Hour;

                        for (int i = 1; i <= number; i++)
                        {
                            list.Add(new ListItem() { Text = "Aula " + i.ToString(), Value = i.ToString() });
                        }
                    }
                }
            }
            ddlAulas.DataValueField = "Value";
            ddlAulas.DataTextField = "Text";
            ddlAulas.DataSource = list;
            ddlAulas.DataBind();
            //ddlAulas.Items.Insert(0, new ListItem("Selecione", "0"));
        }

        //Evento do DropDown Curso.
        protected void ddlCurso_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillTurma();
            ddlCodTurma_SelectedIndexChanged(this, null);
        }
        //Evento do DropDown Turma.
        protected void ddlCodTurma_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (!ddlCodTurma.SelectedValue.Equals(""))
            {

                using (estoqueEntities conexao = new estoqueEntities())
                {

                    int x = Convert.ToInt32(ddlCodTurma.SelectedValue);

                    TURMA preencherTurma = conexao.TURMA.Where(p => p.idTurma == x).FirstOrDefault();
                    if (preencherTurma != null)
                    {
                        txtPeriodo.Text = Convert.ToString(String.Format("{0:dd/MM/yyyy}", preencherTurma.dataInicial));
                        txtPeriodo1.Text = Convert.ToString(String.Format("{0:dd/MM/yyyy}", preencherTurma.dataFinal));
                        txtTurno.Text = preencherTurma.turno;
                        txtNAlunos.Text = Convert.ToString(preencherTurma.numAlunos);
                    }

                }

                FillDisciplina();
                ddlDisciplina_SelectedIndexChanged(this, null);
            }
        }
        //Evento do DropDown Disciplina.
        protected void ddlDisciplina_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillInstrutor();
            FillAula();
            FillListaBasica("A");
        }

        //protected void ddlAulas_SelectedIndexChanged(object sender, EventArgs e)
        //{

        //    FillListaBasica("A");

        //}

        //Preenchimento da gridView com a Lista Básica
        private void FillListaBasica(string pLetra)
        {
            int curso = Convert.ToInt32(ddlCurso.SelectedValue);
            int turma = Convert.ToInt32(ddlCodTurma.SelectedValue);
            int disciplina = Convert.ToInt32(ddlDisciplina.SelectedValue);
            long instrutor = Convert.ToInt64(ddlInstrutores.SelectedValue);
            //int aula = Convert.ToInt32(ddlAulas.SelectedValue);
            if (curso > 0 && turma > 0 && disciplina > 0 && instrutor >= 0 && !string.IsNullOrEmpty(txtPeriodo.Text) && !string.IsNullOrEmpty(txtPeriodo1.Text) && !string.IsNullOrEmpty(txtTurno.Text) && !string.IsNullOrEmpty(txtNAlunos.Text))
            {
                try
                {
                    using (estoqueEntities conexao = new estoqueEntities())
                    {
                        if (!string.IsNullOrEmpty(ddlCurso.SelectedValue))
                        {
                            int x = Convert.ToInt32(ddlCurso.SelectedValue);
                            //int y = Convert.ToInt32(ddlCodTurma.SelectedValue);

                            IList<LISTA_BASICA> lista = new List<LISTA_BASICA>();

                            //if (pLetra.Equals("#"))
                            //{
                            //   TURMA turma = conn.TURMA.Where(p => p.idTurma == y).FirstOrDefault();
                            lista = conexao.LISTA_BASICA.Where(p => p.idCurso == x).ToList();

                            //}
                            //else
                            //{
                            //    IList<int> listaProduto = conn.PRODUTO.Where(p => p.produto.StartsWith(pLetra)).Select(p => p.id).ToList();
                            //    lista = conn.LISTA_BASICA.Where(p => p.idCurso == x && listaProduto.Contains(p.idProduto)).Distinct().ToList();
                            //}

                            if (lista != null)
                            {
                                grdPlanejamento.DataSource = lista;
                                grdPlanejamento.DataBind();

                                if (lista.Count > 0)
                                {
                                    grdPlanejamento.UseAccessibleHeader = true;
                                    grdPlanejamento.HeaderRow.TableSection = TableRowSection.TableHeader;

                                    TableCellCollection cells = grdPlanejamento.HeaderRow.Cells;
                                    cells[0].Attributes.Add("data-class", "expand");
                                    cells[1].Attributes.Add("data-sort-ignore", "true");
                                    cells[2].Attributes.Add("data-sort-ignore", "true");
                                    cells[3].Attributes.Add("data-sort-ignore", "true");
                                    cells[4].Attributes.Add("data-sort-ignore", "true");
                                    cells[5].Attributes.Add("data-sort-ignore", "true");
                                    cells[6].Attributes.Add("data-sort-ignore", "true");
                                    cells[7].Attributes.Add("data-hide", "all");
                                    cells[8].Attributes.Add("data-hide", "all");

                                }
                            }
                        }
                    }
                    grdPlanejamento.Visible = true;
                    panEnter.Visible = true;
                    grdItemExtra.Visible = true;
                }
                catch (Exception ex)
                {
                    grdPlanejamento.Visible = false;
                    panEnter.Visible = false;
                    grdItemExtra.Visible = false;
                    throw ex;
                }
            }
            else
            {
                grdPlanejamento.Visible = false;
                panEnter.Visible = false;
                grdItemExtra.Visible = false;

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/>Necessário o preenchimento de todos os campos.', 'warning');", true);
            }
        }
        /// <summary>
        /// Verificar preenchimento da Grid.
        /// </summary>
        //Preenchimento da GridView de Produtos Extra.
        private void FillGridItensExtra()
        {
            IList<PLANEJAMENTO_AUXILIAR> itens = new List<PLANEJAMENTO_AUXILIAR>();

            using (estoqueEntities conexao = new estoqueEntities())
            {
                UserAD usuarioSistemas = Session["UsuarioSistemasLegados"] as UserAD;
                USUARIO user = conexao.USUARIO.Where(p => p.usuario.Equals(usuarioSistemas.Samaccountname)).FirstOrDefault();

                int curs = Convert.ToInt32(ddlCurso.SelectedValue);
                int turm = Convert.ToInt32(ddlCodTurma.SelectedValue);

                LISTA_BASICA lista = new LISTA_BASICA();
                lista = conexao.LISTA_BASICA.Where(p => p.idCurso == curs).FirstOrDefault();
                PLANEJAMENTO cadastrar = conexao.PLANEJAMENTO.Where(p => p.idListaBasica == lista.id && p.idCurso == curs && p.idTurma == turm).FirstOrDefault();

                itens = conexao.PLANEJAMENTO_AUXILIAR.Where(p => p.idPlanejamento == cadastrar.id).Distinct().ToList();

                grdItemExtra.DataSource = itens;
                grdItemExtra.DataBind();

                if (itens.Count > 0)
                {
                    grdItemExtra.UseAccessibleHeader = true;
                    grdItemExtra.HeaderRow.TableSection = TableRowSection.TableHeader;

                    TableCellCollection cells = grdItemExtra.HeaderRow.Cells;
                    cells[0].Attributes.Add("data-class", "expand");
                    cells[1].Attributes.Add("data-sort-ignore", "true");
                    cells[2].Attributes.Add("data-sort-ignore", "true");
                    cells[3].Attributes.Add("data-sort-ignore", "true");
                    cells[4].Attributes.Add("data-sort-ignore", "true");
                }
            }
        }
        //Evento RowDataBound da GridView Planejamento.
        protected void grdPlanejamento_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                using (estoqueEntities conexao = new estoqueEntities())
                {
                    LISTA_BASICA lista = (LISTA_BASICA)e.Row.DataItem;

                    PRODUTO produto = conexao.PRODUTO.Where(p => p.codSei == lista.codSei).FirstOrDefault();
                    if (produto != null)
                    {
                        e.Row.Cells[1].Text = produto.produto;
                    }
                    else
                    {
                        e.Row.Cells[1].Text = string.Empty;
                    }
                    TextBox txtQuantidade = (TextBox)e.Row.FindControl("txtQuantidade");
                    e.Row.Cells[0].Text = lista.idGrade.ToString();
                    e.Row.Cells[2].Text = lista.unidMedida.ToString();
                    e.Row.Cells[3].Text = lista.quantEstimada.ToString();
                    int turmaid = Convert.ToInt32(ddlCodTurma.SelectedValue);
                    int saldo = lista.PLANEJAMENTO.Where(p => p.idCurso == lista.idCurso && p.idTurma == turmaid && p.codSei == lista.codSei).Sum(p => p.saldo);
                    int quantidade = lista.PLANEJAMENTO.Where(p => p.idCurso == lista.idCurso && p.idTurma == turmaid && p.aula.Equals(ddlAulas.SelectedItem.Text)).Select(p => p.quantidade).FirstOrDefault();
                    int quantLimite = lista.PLANEJAMENTO.Where(p => p.idCurso == lista.idCurso && p.idTurma == turmaid && p.codSei == lista.codSei).Sum(p => p.quantidade);
                    e.Row.Cells[5].Text = (lista.quantEstimada.Value - quantLimite).ToString();

                    if (lista.quantEstimada.Value == quantLimite)
                    {
                        txtQuantidade.Enabled = false;
                    }
                    else
                    {
                        txtQuantidade.Enabled = true;
                    }
                    txtQuantidade.Text = quantidade.ToString();
                }
            }
        }
        //Evento RowCommand da GridView Planejamento.
        protected void grdPlanejamento_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            FillListaBasica("#");
        }
        [WebMethod]
        public static string SalvarPlanejamento(PLANEJAMENTO planejar)
        {
            string msg = string.Empty;
            try
            {
               
            }
            catch
            {
                msg = "Operação não efetuada. Entre em contato com o administrador do sistema._danger";
            }
            return msg;
        }
        //Prenchimento da tabela planejamento e planejamento_itens_extra.
        protected void txtQuantidade_TextChanged(object sender, EventArgs e)
        {
            TextBox txtQuantidade = (TextBox)sender;
            if (!string.IsNullOrEmpty(txtQuantidade.Text))
            {
                UserAD usuarioSistemas = Session["UsuarioSistemasLegados"] as UserAD;

                int id = Convert.ToInt32(txtQuantidade.Attributes["itemid"]);
                using (estoqueEntities conexao = new estoqueEntities())
                {
                    int curs = Convert.ToInt32(ddlCurso.SelectedValue);
                    int turm = Convert.ToInt32(ddlCodTurma.SelectedValue);
                    LISTA_BASICA lista = conexao.LISTA_BASICA.Where(p => p.id == id).FirstOrDefault();
                    USUARIO user = conexao.USUARIO.Where(p => p.usuario.Equals(usuarioSistemas.Samaccountname)).FirstOrDefault();
                    CURSOS curso = conexao.CURSOS.Where(p => p.idCurso == curs && p.idUnidade == user.DEPOSITO.idUnidade).FirstOrDefault();
                    TURMA turma = conexao.TURMA.Where(p => p.idTurma == turm).FirstOrDefault();
                    PLANEJAMENTO cadastrar = conexao.PLANEJAMENTO.Where(p => p.idListaBasica == id && p.idCurso == curs && p.idTurma == turm && p.aula.Equals(ddlAulas.SelectedItem.Text)).FirstOrDefault();
                    INSTRUTOR instrutor = conexao.INSTRUTOR.Where(p => p.cpf.Equals(usuarioSistemas.Postofficebox)).FirstOrDefault();

                    int quantidade = lista.PLANEJAMENTO.Where(p => p.idCurso == lista.idCurso && p.idTurma == turm && p.aula.Equals(ddlAulas.SelectedItem.Text)).Select(p => p.quantidade).FirstOrDefault();
                    int quantLimit = lista.PLANEJAMENTO.Where(p => p.idCurso == lista.idCurso && p.idTurma == turm && p.codSei == lista.codSei).Sum(p => p.quantidade);

                    int saldo = lista.PLANEJAMENTO.Where(p => p.idCurso == lista.idCurso && p.idTurma == turm && p.codSei == lista.codSei).Sum(p => p.saldo);

                    int quantLimite = lista.PLANEJAMENTO.Where(p => p.idCurso == lista.idCurso && p.codSei == lista.codSei && p.idTurma == turm).Sum(p => p.quantidade);

                    int qtd = Convert.ToInt32(txtQuantidade.Text);
                    int quantSaldo = lista.quantEstimada.HasValue ? lista.quantEstimada.Value : 0;

                    if (cadastrar == null)
                    {
                        PLANEJAMENTO cadastro = new PLANEJAMENTO();

                        if (qtd <= (lista.quantEstimada.Value - quantLimit) && qtd > 0)
                        {
                            cadastro.idCurso = curs;
                            cadastro.idUnidade = user.DEPOSITO.idUnidade;
                            cadastro.idTurma = turma.idTurma;
                            cadastro.codTurma = ddlCodTurma.SelectedItem.Text;
                            cadastro.idDisciplina = Convert.ToInt32(ddlDisciplina.SelectedValue);
                            cadastro.aula = ddlAulas.SelectedItem.Text;
                            cadastro.idListaBasica = lista.id;
                            cadastro.codSei = lista.codSei;
                            cadastro.quantidade = Convert.ToInt32(txtQuantidade.Text);
                            cadastro.dataPlanejamento = DateTime.Now;
                            cadastro.saldo = quantSaldo - qtd;

                            if (usuarioSistemas != null)
                            {
                                if (usuarioSistemas.Memberof != null && (usuarioSistemas.Memberof.Contains(ConfigurationManager.AppSettings["CE-supervisor"])))
                                {
                                    cadastro.idInstrutor = Convert.ToInt64(ddlInstrutores.SelectedValue);
                                }
                                else
                                {
                                    if (instrutor != null)
                                    {
                                        cadastro.idInstrutor = instrutor.idInstrutor;
                                    }
                                    else
                                    {
                                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/>Instrutor não cadastrado. Entre em contato com a supervisão.', 'warning');", true);
                                    }
                                }
                                conexao.Entry<PLANEJAMENTO>(cadastro).State = System.Data.Entity.EntityState.Added;
                                conexao.SaveChanges();
                            }
                        }
                        else
                        {
                            if (qtd < 0)
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/>A quantidade não pode ser negativa.', 'warning');", true);
                            }
                            else
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/>A quantidade não pode ser superior ao saldo disponível.', 'warning');", true);
                            }
                        }
                    }
                    else
                    {
                        if (qtd <= (lista.quantEstimada.Value - quantLimit) && qtd > 0)
                        {
                            cadastrar.quantidade = Convert.ToInt32(txtQuantidade.Text);
                            cadastrar.saldo = quantSaldo - qtd;
                        }
                        else
                        {
                            if (qtd < 0)
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/>A quantidade não pode ser negativa.', 'warning');", true);
                            }
                            else
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/>A quantidade não pode ser superior ao saldo disponível.', 'warning');", true);
                            }
                        }
                        conexao.Entry<PLANEJAMENTO>(cadastrar).State = System.Data.Entity.EntityState.Modified;
                        conexao.SaveChanges();
                    }
                }
            }
            FillListaBasica("#");
        }
        //Método para salvar a quantidade extra de produtos na tabela planejamento.
        [WebMethod]
        public static string SalvarItemExtra(string id, string item, string justificar, string turm)
        {
            string retorno = string.Empty;
            try
            {
                UserAD usuarioSistemas = HttpContext.Current.Session["UsuarioSistemasLegados"] as UserAD;

                int listaBasicaId = Convert.ToInt32(id);

                using (estoqueEntities conexao = new estoqueEntities())
                {
                    int turmaId = Convert.ToInt32(turm);

                    LISTA_BASICA lista = conexao.LISTA_BASICA.Where(p => p.id == listaBasicaId).FirstOrDefault();

                    TURMA turma = conexao.TURMA.Where(p => p.idTurma == turmaId).FirstOrDefault();

                    PLANEJAMENTO cadastrar = conexao.PLANEJAMENTO.Where(p => p.idListaBasica == listaBasicaId && p.idTurma == turmaId && p.codSei == lista.codSei).FirstOrDefault();

                    INSTRUTOR instrutor = conexao.INSTRUTOR.Where(p => p.cpf.Equals(usuarioSistemas.Postofficebox)).FirstOrDefault();

                    if (cadastrar != null)
                    {

                        PLANEJAMENTO_ITENS_EXTRA cadastro = conexao.PLANEJAMENTO_ITENS_EXTRA.Where(p => p.idPlanejamento == cadastrar.id && p.codSei == lista.codSei && p.idInstrutor == instrutor.idInstrutor).FirstOrDefault();


                        if (cadastro == null)
                        {
                            cadastro = new PLANEJAMENTO_ITENS_EXTRA();

                            cadastro.idPlanejamento = cadastrar.id;
                            cadastro.codSei = cadastrar.codSei;
                            cadastro.quantidade = Convert.ToInt32(item);
                            cadastro.justificativa = justificar;
                            cadastro.idInstrutor = instrutor.idInstrutor;
                            cadastro.idTurma = cadastrar.idTurma;

                            conexao.Entry<PLANEJAMENTO_ITENS_EXTRA>(cadastro).State = System.Data.Entity.EntityState.Added;

                        }
                        else
                        {

                            cadastro.quantidade = Convert.ToInt32(item);
                            cadastro.justificativa = justificar;

                            conexao.Entry<PLANEJAMENTO_ITENS_EXTRA>(cadastro).State = System.Data.Entity.EntityState.Modified;

                        }
                        conexao.SaveChanges();
                        retorno = "Item cadastrado com sucesso._success";
                    }
                    else
                    {
                        retorno = "Erro ao cadastrar item extra._warning";
                    }

                }

            }
            catch
            {

                retorno = "Erro ao cadastrar item extra._warning";
            }

            return retorno;

        }

        //protected void txtItemExtra_TextChanged(object sender, EventArgs e)
        //{


        //}
        // Evento do DropDown Disciplina.
        protected void ddlInstrutores_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillListaBasica("A");
        }
        // Evento do DropDown Unidade.
        protected void ddlUnidade_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillCurso();
            ddlCurso_SelectedIndexChanged(this, null);

        }
        // Evento do DropDown Aulas.
        protected void ddlAulas_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillListaBasica("A");
        }

        // Buscar Produto Extra na Tabela Produtos********************** 

        protected void btnProdutoExtra_Click(object sender, EventArgs e)
        {
            string[] produto = hdfItemExtra.Value.Split('|');

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
                    PRODUTO produtoExtra = conexao.PRODUTO.Where(p => p.codBarra.Equals(txtProdExtra.Text)).FirstOrDefault();

                    try
                    {
                        string nome = ((UserAD)Session["UsuarioSistemasLegados"]).Samaccountname;

                        USUARIO usuario = conexao.USUARIO.Include("DEPOSITO").Where(p => p.usuario.Equals(nome)).FirstOrDefault();

                        if (produtoExtra == null && usuario != null)
                        {
                            txtProdExtra.Text = string.Empty;
                            txtQuantExtra.Text = string.Empty;
                        }
                        else
                        {

                        }

                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }

            }
            else
            {

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/>Produto não cadastrado corretamente no SEI.', 'warning');", true);

            }

        }

        // Buscar Produto Extra na Tabela Produtos********************** 
        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<string> FindItemExtra(string pTexto)
        {

            using (estoqueEntities conexao = new estoqueEntities())
            {
                IList<PRODUTO> produto = conexao.PRODUTO.Where(p => p.produto.StartsWith(pTexto)).ToList();

                List<string> customers = new List<string>();

                for (int i = 0; i < produto.Count; i++)
                {

                    customers.Add(produto[i].codSei + " | " + produto[i].produto);
                }

                return customers;

            }

        }
        //Salvar Produto Extra na Tabela Planejamento Auxiliar.
        protected void btnItemExtra_Click(object sender, EventArgs e)
        {
            using (estoqueEntities conexao = new estoqueEntities())
            {
                UserAD usuarioSistemas = HttpContext.Current.Session["UsuarioSistemasLegados"] as UserAD;

                int x = Convert.ToInt32(ddlCurso.SelectedValue);
                int y = Convert.ToInt32(ddlCodTurma.SelectedValue);
                int z = Convert.ToInt32(hdfItemExtra.Value.Split('-')[0].Trim());

                LISTA_BASICA lista = conexao.LISTA_BASICA.Where(p => p.idCurso == x).FirstOrDefault();
                PLANEJAMENTO cadastrar = conexao.PLANEJAMENTO.Where(p => p.idListaBasica == lista.id && p.idTurma == y).FirstOrDefault();
                INSTRUTOR instrutor = conexao.INSTRUTOR.Where(p => p.cpf.Equals(usuarioSistemas.Postofficebox)).FirstOrDefault();

                if (cadastrar != null)
                {
                    PLANEJAMENTO_AUXILIAR planAuxiliar = new PLANEJAMENTO_AUXILIAR();

                    planAuxiliar.idPlanejamento = cadastrar.id;
                    planAuxiliar.codSei = z;
                    planAuxiliar.quantidade = Convert.ToInt32(txtQuantExtra.Text);
                    planAuxiliar.justificativa = txtJustificarExtra.Text;
                    planAuxiliar.idInstrutor = instrutor.idInstrutor;
                    planAuxiliar.idTurma = cadastrar.idTurma;
                    planAuxiliar.aula = ddlAulas.SelectedItem.Text;
                    planAuxiliar.idCurso = cadastrar.idCurso;
                    planAuxiliar.idUnidade = cadastrar.idUnidade;

                    conexao.Entry<PLANEJAMENTO_AUXILIAR>(planAuxiliar).State = System.Data.Entity.EntityState.Added;
                    conexao.SaveChanges();

                    txtProdExtra.Text = string.Empty;
                    txtQuantExtra.Text = string.Empty;
                    txtJustificarExtra.Text = string.Empty;
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/> Realizar planejamento antes de incluir itens extras.', 'warning');", true);
                }

            }

            FillGridItensExtra();


            FillListaBasica("A");
        }
        // Evento GridView Planejamento.
        protected void grdPlanejamento_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        protected void grdItemExtra_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                using (estoqueEntities conexao = new estoqueEntities())
                {
                    PLANEJAMENTO_AUXILIAR lista = (PLANEJAMENTO_AUXILIAR)e.Row.DataItem;

                    PRODUTO produto = conexao.PRODUTO.Where(p => p.codSei == lista.codSei).FirstOrDefault();
                    if (produto != null)
                    {
                        e.Row.Cells[1].Text = produto.produto;
                    }
                    else
                    {
                        e.Row.Cells[1].Text = string.Empty;
                    }
                }
            }
        }
        //Evento RowCommand da GridView Produto extra.
        protected void grdItemExtra_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                using (estoqueEntities conexao = new estoqueEntities())
                {
                    int id = Convert.ToInt32(e.CommandArgument);

                    if (e.CommandName.Equals("Excluir"))
                    {
                        PLANEJAMENTO_AUXILIAR ExcluirItensExtra = conexao.PLANEJAMENTO_AUXILIAR.Where(p => p.id == id).FirstOrDefault();

                        if (ExcluirItensExtra != null)
                        {
                            conexao.Entry<PLANEJAMENTO_AUXILIAR>(ExcluirItensExtra).State = System.Data.Entity.EntityState.Deleted;
                            conexao.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            FillGridItensExtra();
            FillListaBasica("A");
        }
    }
}
