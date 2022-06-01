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
using System.Text;
using System.Xml;
using ControleEstoque.Web.Models.ADModel;

namespace ControleEstoque.Web
{
    public partial class Principal : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region -- Verifica se o usuário está logado na aplicação ---
            if (Session["UsuarioSistemasLegados"] != null)
            {
                //#region -- Seta informações indicando se o sistema está em modo de treinamento ou produção. --
                //NHibernate.ISession session = NHibernateSession.CurrentSession;
                //if (session.Connection.Database.Equals("SYS_DEV"))
                //{
                //    lblInforBase.Text = "..:: TREINAMENTO ::..";
                //    watermark.Visible = true;
                //}
                //else
                //{
                //    watermark.Visible = false;
                //}
                //session.Clear();
                //#endregion

                CriarMenu();

                #region -- Seta os valores dos parâmetros do log4net, também é responsável por ativar o log4net. --
                UserAD user = ((Session["UsuarioSistemasLegados"]) as UserAD);

                if (user != null)
                {
                    //Esta linha eh responsavel por setar os valores para os parâmetros do log4net 
                    log4net.GlobalContext.Properties["user"] = user.Displayname;
                    //Esta linha eh responsavel por ativar o log4net 
                    log4net.Config.XmlConfigurator.Configure();
                }
                #endregion
            }
            else
            {
                Response.Redirect("Login.aspx", true);
            }

            #endregion

            //#region -- Verifica se o sistema está em modo de manutenção. --
            //if (ConfigurationManager.AppSettings["Manutencao"].Equals("1"))
            //{
            //    Response.Redirect("Login.aspx");
            //}
            //#endregion
        }

        protected void lbSair_Click(object sender, EventArgs e)
        {
            Session["UsuarioSistemasLegados"] = null;
            Response.Redirect("Login.aspx");
        }

        private void CriarMenu()
        {
            try
            {
                UserAD usuarioSistemas = Session["UsuarioSistemasLegados"] as UserAD;
                lblUser.InnerText = usuarioSistemas.Samaccountname;

                StringBuilder html = new StringBuilder();

                XmlDocument xmlDoc = new XmlDocument();
                XmlNodeList SubNodes;
                XmlNodeList Nodes;

              
                if (usuarioSistemas.Memberof != null && (usuarioSistemas.Memberof.Contains(ConfigurationManager.AppSettings["CE-instrutor"])))
                {
                    xmlDoc.Load(Server.MapPath("~/master/menu-xml/CE-instrutor.xml"));

                    SubNodes = xmlDoc.SelectNodes("menu/subitem");
                    foreach (XmlNode subitem in SubNodes)
                    {
                        Nodes = subitem.SelectNodes("item");
                        if (Nodes.Count == 0)
                        {
                            html.Append("<li class=\"dropdown\">");
                            html.Append("<li><a href=\"" + subitem.Attributes["url"].Value.ToString() + "\">" + subitem.Attributes["text"].Value.ToString() + "</a></li>");
                            html.Append("</li>");

                        }
                        else
                        {

                            html.Append("<li class=\"dropdown\">");
                            html.Append("<a href=\"#\" class=\"dropdown-toggle\" data-toggle=\"dropdown\" role=\"button\" aria-haspopup=\"true\" aria-expanded=\"false\">" + subitem.Attributes["text"].Value.ToString() + "</a>");
                            html.Append("<ul class=\"dropdown-menu\">");

                            foreach (XmlNode node in Nodes)
                            {
                                html.Append("<li><a href=\"" + node.Attributes["url"].Value.ToString() + "\">" + node.Attributes["text"].Value.ToString() + "</a></li>");
                            }

                            html.Append("</ul>");
                            html.Append("</li>");

                        }


                    }
                }

                else if (usuarioSistemas.Memberof != null && (usuarioSistemas.Memberof.Contains(ConfigurationManager.AppSettings["CE-supervisor"])))
                {
                    xmlDoc.Load(Server.MapPath("~/master/menu-xml/CE-supervisor.xml"));

                    SubNodes = xmlDoc.SelectNodes("menu/subitem");
                    foreach (XmlNode subitem in SubNodes)
                    {
                        Nodes = subitem.SelectNodes("item");
                        if (Nodes.Count == 0)
                        {
                            html.Append("<li class=\"dropdown\">");
                            html.Append("<li><a href=\"" + subitem.Attributes["url"].Value.ToString() + "\">" + subitem.Attributes["text"].Value.ToString() + "</a></li>");
                            html.Append("</li>");


                        }
                        else
                        {

                            html.Append("<li class=\"dropdown\">");
                            html.Append("<a href=\"#\" class=\"dropdown-toggle\" data-toggle=\"dropdown\" role=\"button\" aria-haspopup=\"true\" aria-expanded=\"false\">" + subitem.Attributes["text"].Value.ToString() + "</a>");
                            html.Append("<ul class=\"dropdown-menu\">");



                            foreach (XmlNode node in Nodes)
                            {
                                html.Append("<li><a href=\"" + node.Attributes["url"].Value.ToString() + "\">" + node.Attributes["text"].Value.ToString() + "</a></li>");
                            }

                            html.Append("</ul>");
                            html.Append("</li>");



                        }


                    }
                }
                else if (usuarioSistemas.Memberof != null && (usuarioSistemas.Memberof.Contains(ConfigurationManager.AppSettings["CE-comum"])))
                {
                    xmlDoc.Load(Server.MapPath("~/master/menu-xml/CE-comum.xml"));

                    SubNodes = xmlDoc.SelectNodes("menu/subitem");
                    foreach (XmlNode subitem in SubNodes)
                    {
                        Nodes = subitem.SelectNodes("item");
                        if (Nodes.Count == 0)
                        {
                            html.Append("<li class=\"dropdown\">");
                            html.Append("<li><a href=\"" + subitem.Attributes["url"].Value.ToString() + "\">" + subitem.Attributes["text"].Value.ToString() + "</a></li>");
                            html.Append("</li>");


                        }
                        else
                        {

                            html.Append("<li class=\"dropdown\">");
                            html.Append("<a href=\"#\" class=\"dropdown-toggle\" data-toggle=\"dropdown\" role=\"button\" aria-haspopup=\"true\" aria-expanded=\"false\">" + subitem.Attributes["text"].Value.ToString() + "</a>");
                            html.Append("<ul class=\"dropdown-menu\">");



                            foreach (XmlNode node in Nodes)
                            {
                                html.Append("<li><a href=\"" + node.Attributes["url"].Value.ToString() + "\">" + node.Attributes["text"].Value.ToString() + "</a></li>");
                            }

                            html.Append("</ul>");
                            html.Append("</li>");



                        }


                    }
                }
                else if (usuarioSistemas.Memberof != null && (usuarioSistemas.Memberof.Contains(ConfigurationManager.AppSettings["CE-admin"])))
                {
                    xmlDoc.Load(Server.MapPath("~/master/menu-xml/CE-admin.xml"));

                    SubNodes = xmlDoc.SelectNodes("menu/subitem");
                    foreach (XmlNode subitem in SubNodes)
                    {
                        Nodes = subitem.SelectNodes("item");
                        if (Nodes.Count == 0)
                        {
                            html.Append("<li class=\"dropdown\">");
                            html.Append("<li><a href=\"" + subitem.Attributes["url"].Value.ToString() + "\">" + subitem.Attributes["text"].Value.ToString() + "</a></li>");
                            html.Append("</li>");


                        }
                        else
                        {

                            html.Append("<li class=\"dropdown\">");
                            html.Append("<a href=\"#\" class=\"dropdown-toggle\" data-toggle=\"dropdown\" role=\"button\" aria-haspopup=\"true\" aria-expanded=\"false\">" + subitem.Attributes["text"].Value.ToString() + "</a>");
                            html.Append("<ul class=\"dropdown-menu\">");

                           

                            foreach (XmlNode node in Nodes)
                            {
                                html.Append("<li><a href=\"" + node.Attributes["url"].Value.ToString() + "\">" + node.Attributes["text"].Value.ToString() + "</a></li>");
                            }

                            html.Append("</ul>");
                            html.Append("</li>");



                        }


                    }


                }

                ulMenu.InnerHtml = html.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Principal.Master :: CriarMenu " + ex.Message, ex);
            }
        }
    }
}
