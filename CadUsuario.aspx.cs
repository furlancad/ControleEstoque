using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using ControleEstoque.Web.Models;
using ControleEstoque.Web.Models.ADModel;

namespace ControleEstoque.Web
{
    public partial class CadUsuario : System.Web.UI.Page
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
                    FillCadDeposito();
                   
                    FillUsuario("#");

                    FillUsuariosAD();
                }
            }
        }
        //Preenchimento do DropDown Depósito.
        private void FillCadDeposito()
        {
            IList<DEPOSITO> listDeposito = new List<DEPOSITO>();

            using (estoqueEntities conexao = new estoqueEntities())
            {
                listDeposito = conexao.DEPOSITO.Distinct().ToList();

                ddlDeposito.DataTextField = "deposito";
                ddlDeposito.DataValueField = "id";
                ddlDeposito.DataSource = listDeposito;
                ddlDeposito.DataBind();
            }
        }

        //Lista de Usuários do Active Directory.
        private void FillUsuariosAD()
        {
            ManageAD managed = new ManageAD();
            IList<UserAD> lista = managed.FindAllUsuariosAtivos().OrderBy(p => p.Samaccountname).ToList();


            ddlUsuario.DataTextField = "displayName";
            ddlUsuario.DataValueField = "Samaccountname";
            ddlUsuario.DataSource = lista;
            ddlUsuario.DataBind();
        }

        //[System.Web.Services.WebMethod]
        //[System.Web.Script.Services.ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public static IList<string> FindUsuario(string pTexto)
        //{
            
        //    ManageAD managed = new ManageAD();
        //    IList<UserAD> lista = managed.FindAllUsuariosAtivos().Where(p => p.Samaccountname.Equals(pTexto.ToLower())).OrderBy(p => p.Samaccountname).ToList();

        //    List<string> customers = new List<string>();

        //    for (int i = 0; i < lista.Count; i++)
        //    {

        //        customers.Add(lista[i].Samaccountname.ToString().Trim() + " | " + lista[i].Displayname.ToString().Trim());
        //    }

        //    return customers;
       

        //}

        //Botão Cadastrar Usuário.
        protected void btnCadUsuario_Click(object sender, EventArgs e)
        {
            try
            {
                using (estoqueEntities conexao = new estoqueEntities())
                {

                    if (String.IsNullOrEmpty(hdfUsuarioid.Value))
                    {
                        int x = Convert.ToInt32(ddlDeposito.SelectedValue);

                        USUARIO cadUsuario = conexao.USUARIO.Where(p => p.usuario.Equals(ddlUsuario.SelectedItem.Text) && p.idDeposito == x).FirstOrDefault();
                        if (cadUsuario == null)
                        {

                            cadUsuario = new USUARIO();
                            cadUsuario.usuario = ddlUsuario.SelectedValue;
                            cadUsuario.responsavel = ddlUsuario.SelectedItem.Text;
                            cadUsuario.cpf = txtCPF.Text;
                            cadUsuario.cargo = txtCargo.Text;
                            cadUsuario.DEPOSITO = conexao.DEPOSITO.Where(p => p.id == x).FirstOrDefault();

                            conexao.Entry<USUARIO>(cadUsuario).State = System.Data.Entity.EntityState.Added;

                            conexao.SaveChanges();

                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Success", "alert('Sucesso<br/>Operação efetuada com sucesso.', 'success');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/> Usuário Já cadastrado.', 'warning');", true);
                        }
                    }
                    else
                    {
                        int y = Convert.ToInt32(hdfUsuarioid.Value);
                        USUARIO user = conexao.USUARIO.Where(p => p.id == y).FirstOrDefault();
                        user.cpf = txtCPF.Text;
                        user.cargo = txtCargo.Text;

                        conexao.Entry<USUARIO>(user).State = System.Data.Entity.EntityState.Modified;

                        conexao.SaveChanges();

                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Success", "alert('Sucesso<br/>Operação efetuada com sucesso.', 'success');", true);
                    }

                    // LIMPAR CAMPOS
                    ddlDeposito.SelectedIndex = 0;
                    ddlUsuario.SelectedIndex = 0;
                    txtCPF.Text = string.Empty;
                    txtCargo.Text = string.Empty;
                }

                FillUsuario("#");
            }
            catch
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/>Operação não efetuada. Entre em contato com o administrador do sistema.', 'warning');", true);
            }
            
        }
        //Listar Usuários.
        private void CarregarUsuarios(string pLetra)
        {
            FillUsuario(pLetra);
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ActiveOne", "$(function(){ LetterActive('collapseOne', '" + pLetra + "'); });", true);
        }

        protected void btn_collapseOne_Click(object sender, EventArgs e)
        {
            CarregarUsuarios(((Button)sender).Text);
        }
        //PREENCHER USUÁRIO

        //Preencher GridView Usuários.
        private void FillUsuario(string pLetra)
        {

            try
            {
                IList<USUARIO> listaUsuario = new List<USUARIO>();


                using (estoqueEntities conexao = new estoqueEntities())
                {
                    IList<USUARIO> listUsuario = new List<USUARIO>();

                    if (pLetra.Equals("#"))
                    {

                        listaUsuario = conexao.USUARIO.Distinct().ToList();

                    }
                    else
                    {
                        listaUsuario = conexao.USUARIO.Where(p => p.usuario.StartsWith(pLetra)).Distinct().ToList();
                    }


                    grdUsuario.DataSource = listaUsuario;
                    grdUsuario.DataBind();


                    if (listaUsuario.Count > 0)
                    {
                        grdUsuario.UseAccessibleHeader = true;
                        grdUsuario.HeaderRow.TableSection = TableRowSection.TableHeader;

                        TableCellCollection cells = grdUsuario.HeaderRow.Cells;
                        cells[0].Attributes.Add("data-class", "expand");
                        cells[1].Attributes.Add("data-sort-ignore", "true");
                        cells[2].Attributes.Add("data-sort-ignore", "true");
                        cells[3].Attributes.Add("data-sort-ignore", "true");
                        cells[4].Attributes.Add("data-sort-ignore", "true");

                    }

                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        protected void grdUsuario_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                USUARIO usuario = (USUARIO)e.Row.DataItem;

                e.Row.Cells[0].Text = usuario.DEPOSITO.deposito;

            }
        }

        //Evento RowCommand da GridView Usuário.
        protected void grdUsuario_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                using (estoqueEntities conexao = new estoqueEntities())
                {
                    int id = Convert.ToInt32(e.CommandArgument);

                    if (e.CommandName.Equals("Editar"))
                    {
                        USUARIO editUsuario = conexao.USUARIO.Where(p => p.id == id).FirstOrDefault();

                        //ViewState["AmbienteId"] = AmbienteId;
                        ddlDeposito.SelectedValue = editUsuario.DEPOSITO.id.ToString();
                        hdfUsuarioid.Value = editUsuario.id.ToString();
                        ddlUsuario.SelectedValue = editUsuario.usuario;
                        txtCPF.Text = editUsuario.cpf;
                        txtCargo.Text = editUsuario.cargo;

                        //    if (editUsuario.OrcamentoUnidades.Cidade != null)
                        //    {
                        //        ddlMunicipio.SelectedValue = ambiente.OrcamentoUnidades.Cidade.Id.ToString();
                        //    }
                        //    txtNumeroAmbientesAdministrativos.Text = ambiente.OrcamentoUnidades.NumeroAmbientesAdministrativos.ToString();
                        //    txtNumeroAmbientesEducacionais.Text = ambiente.OrcamentoUnidades.NumeroAmbientesEducacionais.ToString();
                        //}
                        //else if (e.CommandName.Equals("Excluir"))
                        //{
                        //    serviceAmbiente.Remove(id);

                        //this.Master.addSuccess("Operação efetuada com sucesso.");
                    }
                }
            }
            catch (Exception ex)
            {
                //this.Master.addError(ex.Message.Replace("\r\n", "\\r\\n"));
            }
        }
    }
}
