using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ControleEstoque.Web.Models;
using ControleEstoque.Web.Models.ADModel;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Configuration;
using System.Text;


namespace ControleEstoque.Web
{
    public partial class Sincronizar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        //Conexão com o banco Oracle do Sei.
        private OracleConnection GetConnection()
        {
            string connection = ConfigurationManager.ConnectionStrings["ConexaoOracle"].ConnectionString;
            return new OracleConnection(connection);
        }

        //Conexão com o banco Oracle do Sei.
        private DataTable GetDataTable(StringBuilder select, Dictionary<string, string> parameters = null)
        {
            OracleConnection oracleConnection = new OracleConnection(ConfigurationManager.ConnectionStrings["ConexaoOracle"].ConnectionString);
            OracleCommand oracleCommand = new OracleCommand();
            OracleDataAdapter oracleDataAdapter = new OracleDataAdapter();
            DataTable dataTable = new DataTable();

            try
            {
                oracleCommand.Connection = oracleConnection;
                oracleCommand.CommandType = CommandType.Text;
                oracleCommand.CommandText = select.ToString();

                if (parameters != null)
                {
                    foreach (var parameter in parameters.Keys)
                    {
                        oracleCommand.Parameters.Add(parameter, parameters[parameter]);
                    }
                }

                oracleDataAdapter.SelectCommand = oracleCommand;
                oracleDataAdapter.Fill(dataTable);
            }
            catch
            {
                if (oracleConnection.State == ConnectionState.Open)
                {
                    oracleConnection.Close();
                }

                dataTable.Clear();
                dataTable.Dispose();
                oracleDataAdapter.Dispose();
                oracleCommand.Dispose();
                oracleConnection.Dispose();
            }
            finally
            {
                if (oracleConnection.State == ConnectionState.Open)
                {
                    oracleConnection.Close();
                }

                oracleDataAdapter.Dispose();
                oracleCommand.Dispose();
                oracleConnection.Dispose();
            }

            return dataTable;
        }

        //Sincronização lista Básica.
        protected void btnSincronizarListaBasica_Click(object sender, EventArgs e)
        {
            DataTable dataTable = new DataTable();
            StringBuilder builder = new StringBuilder();

            builder.Append(" SELECT DISTINCT GRA.GRA_ID AS ID_GRADE, GRA.CUR_ID AS CODIGO_CURSO, SLB.SLB_DATAATIVACAO AS DATA_ATIVAÇÃO, PR.ID AS COD_SEI, UNI.UNM_SIGLA AS UNIDADE_MEDIDA, ");
            builder.Append(" LB.LBA_QUANTIDADE AS QUANTIDADE FROM SITUACAOLISTABASICA SLB INNER JOIN GRADESCURRICULARES GRA ON GRA.GRA_ID = SLB.GRA_ID AND SLB.SLB_SITUACAO = 'A' ");
            builder.Append(" INNER JOIN CURSOS CUR ON GRA.CUR_ID = CUR.CUR_ID INNER JOIN LISTABASICA LB ON SLB.ID = LB.SLB_ID INNER JOIN PRODUTO PR ON LB.PRO_ID = PR.ID ");
            builder.Append(" INNER JOIN UNIDADEMEDIDA UNI ON UNI.ID = PR.UNM_ID ");
            //builder.Append(" WHERE GRA.CUR_ID = " + txtSincronLista.Text.Trim());
            builder.Append(" WHERE SLB.SLB_DATAATIVACAO > '01/01/2016' ");
            builder.Append(" ORDER BY GRA.GRA_ID ");

            dataTable = GetDataTable(builder);

            if (dataTable.Rows.Count > 0)
            {

                using (estoqueEntities conexao = new estoqueEntities())
                {
                    LISTA_BASICA lista = null;

                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        int gradeID = Convert.ToInt32(dataTable.Rows[i]["ID_GRADE"].ToString().Trim());
                        int cursoID = Convert.ToInt32(dataTable.Rows[i]["CODIGO_CURSO"].ToString().Trim());
                        int produtoID = Convert.ToInt32(dataTable.Rows[i]["COD_SEI"].ToString().Trim());
                        string medidaUnid = dataTable.Rows[i]["UNIDADE_MEDIDA"].ToString().Trim();
                        int quant = Convert.ToInt32(dataTable.Rows[i]["QUANTIDADE"].ToString().Trim());
                        string obs = string.Empty;

                        lista = conexao.LISTA_BASICA.Where(p => p.idGrade == gradeID).FirstOrDefault();


                        if (lista == null)
                        {

                            lista = new LISTA_BASICA()
                            {

                                idGrade = gradeID,
                                idCurso = cursoID,
                                codSei = produtoID,
                                unidMedida = medidaUnid,
                                quantEstimada = quant,
                                observacao = obs,
                               

                            };

                        }
                        else
                        {

                            //lista.idGrade = gradeID;
                           // lista.idCurso = cursoID;
                           // lista.idProduto = produtoID;
                           // lista.unidMedida = medidaUnid;
                           // lista.quantEstimada = quant;
                           // lista.observacao = obs;
                           

                        }

                        conexao.Entry<LISTA_BASICA>(lista).State = lista.id == 0 ? System.Data.Entity.EntityState.Added : System.Data.Entity.EntityState.Modified;

                        txtSincronLista.Text = string.Empty;

                    }

                    builder.Clear();
                    dataTable.Clear();
                    conexao.SaveChanges();

                }

            }

        }

        //Sincronização Turma.
        protected void btnSincronizarTurma_Click(object sender, EventArgs e)
        {
            DataTable dataTable = new DataTable();
            StringBuilder builder = new StringBuilder();


            builder.Append(" SELECT  DISTINCT GC.CUR_ID AS CODIGO_CURSO, P.ID AS ID_TURMA, P.PRO_ANO || '.' || LPAD(GC.CUR_ID, 4, '0') || '.' || LPAD(P.PRO_SEQUENCIA, 3, '0') AS CODIGO_DA_PROGRAMACAO, P.PRO_CODORCAM AS CODIGO_ORCAMENTARIO, ");
            builder.Append(" P.PRO_NOME AS NOME_DA_PROGRAMACAO, CASE P.PRO_SITUACAO WHEN 'A' THEN 'ANDAMENTO' WHEN 'R' THEN 'REALIZADO' WHEN 'P' THEN 'PREVISTA' WHEN 'E' THEN 'PLANEJAMENTO' WHEN 'C' THEN 'CANCELADA' WHEN 'T' THEN 'PRÉ-CANCELADA' END AS SITUACAO_DA_TURMA, ");
            builder.Append(" P.PRO_DTINICIO AS DATA_DE_INICIO, P.PRO_DTTERMINO AS DATA_DE_TERMINO, P.PRO_PERIODO AS TURNO, P.PRO_CH AS CH_DO_COMPONENTE, T.TUR_VAGASMAX AS VAGAS, COUNT(DISTINCT(DECODE(M.MAT_STATUS,   'T',   NULL,   'P',   NULL,   'C',   NULL,   M.MAT_ID))) AS MATRICULAS, ");
            builder.Append(" PJ.PJU_ID AS FILIAL, TO_CHAR (P.PRO_HRINICIAL, 'HH24:MI') AS HORA_INICIAL, TO_CHAR (P.PRO_HRFINAL, 'HH24:MI') AS HORA_FINAL ");
            builder.Append(" FROM SAE.TURMAS T ");
            builder.Append(" LEFT JOIN SAE.INSCRICOESTURMAS I ON T.TUR_ID = I.TUR_ID LEFT JOIN SAE.MATRICULAS M ON M.MAT_ID = I.MAT_ID LEFT JOIN SAE.PROGRAMACOES P ON P.PRO_ID = T.PRO_ID ");
            builder.Append(" INNER JOIN PESSOASJURIDICAS PJ ON P.UNI_ID = PJ.PJU_ID INNER JOIN GRADESCURRICULARES GC ON GC.GRA_ID = P.GRA_ID INNER JOIN DN_AREAS DNA ON GC.ADN_CODIGO = DNA.ADN_CODIGO AND DNA.ADN_DATA = (SELECT MAX(ADN_DATA) FROM DN_AREAS WHERE ADN_CODIGO=DNA.ADN_CODIGO) ");
            builder.Append(" INNER JOIN DN_TIPOSCURSOS DTI ON GC.CDN_CODIGO = DTI.CDN_CODIGO INNER JOIN EIXOS EIX ON EIX.ID = DNA.EIX_ID INNER JOIN LOG_LOCALIDADE LOC ON P.LOC_NU_SEQUENCIAL = LOC.LOC_NU_SEQUENCIAL ");
            builder.Append(" WHERE P.ID = " + txtSincronTurma.Text.Trim());
            //builder.Append(" WHERE P.PRO_DTINICIO > '01/01/2015' ");
            builder.Append(" GROUP BY P.ID, PJ.PJU_NOMEABREVIADO, GC.GRA_ID, P.PRO_NOME, CASE P.PRO_CORPORATIVA WHEN 1 THEN 'INCOMPANY' WHEN 0 THEN 'NORMAL' END, LOC.LOC_NO, P.PRO_ANO || '.' || LPAD(GC.CUR_ID, 4, '0') || '.' || LPAD(P.PRO_SEQUENCIA, 3, '0'), ");
            builder.Append(" CASE P.PRO_SITUACAO WHEN 'A' THEN 'ANDAMENTO' WHEN 'R' THEN 'REALIZADO' WHEN 'P' THEN 'PREVISTA' WHEN 'E' THEN 'PLANEJAMENTO' WHEN 'C' THEN 'CANCELADA' WHEN 'T' THEN 'PRÉ-CANCELADA' END, ");
            builder.Append(" P.PRO_CH, P.PRO_PERIODO, P.PRO_DTINICIO, P.PRO_DTTERMINO, P.PRO_HRINICIAL, P.PRO_HRFINAL, T.TUR_VAGASMIN, T.TUR_VAGASMAX, T.TUR_VAGASPSG, T.TUR_VAGASPRONATEC, GC.CUR_ID, PJ.PJU_ID, P.PRO_HRINICIAL, P.PRO_HRFINAL, PJ.PJU_ID, P.PRO_CODORCAM ");


            //builder.Append(" SELECT  DISTINCT GC.CUR_ID AS CODIGO_CURSO, PROG.COD_PROGRAMACAO AS COD_TURMA, P.ID AS ID_TURMA, P.PRO_NOME AS NOME_DA_PROGRAMACAO, P.PRO_CODORCAM AS CODIGO_ORCAMENTARIO,  P.PRO_NOME AS NOME_DA_PROGRAMACAO, ");
            //builder.Append(" CASE P.PRO_SITUACAO WHEN 'A' THEN 'ANDAMENTO' WHEN 'R' THEN 'REALIZADO' WHEN 'P' THEN 'PREVISTA' WHEN 'E' THEN 'PLANEJAMENTO' WHEN 'C' THEN 'CANCELADA' WHEN 'T' THEN 'PRÉ-CANCELADA' END AS SITUACAO_DA_TURMA,  P.PRO_DTINICIO AS DATA_DE_INICIO, ");
            //builder.Append(" P.PRO_DTTERMINO AS DATA_DE_TERMINO, P.PRO_PERIODO AS TURNO, P.PRO_CH AS CH_DO_COMPONENTE, T.TUR_VAGASMAX AS VAGAS, COUNT(DISTINCT(DECODE(M.MAT_STATUS,   'T',   NULL,   'P',   NULL,   'C',   NULL,   M.MAT_ID))) AS MATRICULAS,  PJ.PJU_ID AS FILIAL, ");
            //builder.Append(" TO_CHAR (P.PRO_HRINICIAL, 'HH24:MI') AS HORA_INICIAL, TO_CHAR (P.PRO_HRFINAL, 'HH24:MI') AS HORA_FINAL  FROM SAE.TURMAS T  LEFT JOIN SAE.INSCRICOESTURMAS I ON T.TUR_ID = I.TUR_ID LEFT JOIN SAE.MATRICULAS M ON M.MAT_ID = I.MAT_ID LEFT JOIN SAE.PROGRAMACOES P ON P.PRO_ID = T.PRO_ID ");
            //builder.Append(" INNER JOIN SAE.VW_PROGRAMACOESCURSO PROG ON P.PRO_ID = PROG.PRO_ID   INNER JOIN PESSOASJURIDICAS PJ ON P.UNI_ID = PJ.PJU_ID INNER JOIN GRADESCURRICULARES GC ON GC.GRA_ID = P.GRA_ID INNER JOIN DN_AREAS DNA ON GC.ADN_CODIGO = DNA.ADN_CODIGO AND DNA.ADN_DATA = (SELECT MAX(ADN_DATA) FROM DN_AREAS WHERE ADN_CODIGO=DNA.ADN_CODIGO) ");
            //builder.Append(" INNER JOIN DN_TIPOSCURSOS DTI ON GC.CDN_CODIGO = DTI.CDN_CODIGO INNER JOIN EIXOS EIX ON EIX.ID = DNA.EIX_ID INNER JOIN LOG_LOCALIDADE LOC ON P.LOC_NU_SEQUENCIAL = LOC.LOC_NU_SEQUENCIAL ");
            //builder.Append(" WHERE PROG.COD_PROGRAMACAO = " + txtSincronTurma.Text.Trim());
            //builder.Append(" GROUP BY P.ID, PJ.PJU_NOMEABREVIADO, GC.GRA_ID, P.PRO_NOME, ");
            //builder.Append(" CASE P.PRO_CORPORATIVA WHEN 1 THEN 'INCOMPANY' WHEN 0 THEN 'NORMAL' END, LOC.LOC_NO, P.PRO_ANO || '.' || LPAD(GC.CUR_ID, 4, '0') || '.' || LPAD(P.PRO_SEQUENCIA, 3, '0'),  CASE P.PRO_SITUACAO WHEN 'A' THEN 'ANDAMENTO' WHEN 'R' THEN 'REALIZADO' WHEN 'P' THEN 'PREVISTA' WHEN 'E' THEN 'PLANEJAMENTO' WHEN 'C' THEN 'CANCELADA' WHEN 'T' THEN 'PRÉ-CANCELADA' END, ");
            //builder.Append(" P.PRO_CH, P.PRO_PERIODO, P.PRO_DTINICIO, P.PRO_DTTERMINO, P.PRO_HRINICIAL, P.PRO_HRFINAL, T.TUR_VAGASMIN, T.TUR_VAGASMAX, T.TUR_VAGASPSG, T.TUR_VAGASPRONATEC, GC.CUR_ID, PJ.PJU_ID, P.PRO_HRINICIAL, P.PRO_HRFINAL, PJ.PJU_ID, P.PRO_CODORCAM,PROG.COD_PROGRAMACAO "); 
  

            dataTable = GetDataTable(builder);

            if (dataTable.Rows.Count > 0)
            {
                using (estoqueEntities conexao = new estoqueEntities())
                {
                    TURMA turma = null;

                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        int cursoID = Convert.ToInt32(dataTable.Rows[i]["CODIGO_CURSO"].ToString().Trim());
                        int tID = Convert.ToInt32(dataTable.Rows[i]["ID_TURMA"].ToString().Trim());
                        string tCOD = dataTable.Rows[i]["COD_TURMA"].ToString().Trim();
                        string tDESC = dataTable.Rows[i]["NOME_DA_PROGRAMACAO"].ToString().Trim();
                        string tOrcamento = dataTable.Rows[i]["CODIGO_ORCAMENTARIO"].ToString().Trim();
                        string tSITUACAO = dataTable.Rows[i]["SITUACAO_DA_TURMA"].ToString().Trim();
                        DateTime tDTINICIO = Convert.ToDateTime((dataTable.Rows[i]["DATA_DE_INICIO"].ToString().Trim()));
                        DateTime tDTFINAL = Convert.ToDateTime((dataTable.Rows[i]["DATA_DE_TERMINO"].ToString().Trim()));
                        String hINICIO = dataTable.Rows[i]["HORA_INICIAL"].ToString();
                        String hFINAL = dataTable.Rows[i]["HORA_FINAL"].ToString();
                        string tTURNO = dataTable.Rows[i]["TURNO"].ToString().Trim();
                        int tCH = Convert.ToInt32(dataTable.Rows[i]["CH_DO_COMPONENTE"].ToString().Trim());
                        int tVAGAS = Convert.ToInt32(dataTable.Rows[i]["VAGAS"].ToString().Trim());
                        int tNALUNOS = Convert.ToInt32(dataTable.Rows[i]["MATRICULAS"].ToString().Trim());
                        int tUNIDADE = Convert.ToInt32(dataTable.Rows[i]["FILIAL"].ToString().Trim());

                        turma = conexao.TURMA.Where(p => p.codTurma == tCOD).FirstOrDefault();

                        if (turma == null)
                        {

                            turma = new TURMA()
                            {
                                idCurso = cursoID,
                                idTurma = tID,
                                codTurma = tCOD,
                                codOrcamentario = tOrcamento,
                                turma = tDESC,
                                situacao = tSITUACAO,
                                dataInicial = tDTINICIO,
                                dataFinal = tDTFINAL,
                                horaInicial = hINICIO,
                                horaFinal = hFINAL,
                                turno = tTURNO,
                                cargaHoraria = tCH,
                                vagas = tVAGAS,
                                numAlunos = tNALUNOS,
                                idUnidade = tUNIDADE
                            };

                        }
                        else
                        {
                            //turma.idCurso = cursoID;
                            //turma.idTurma = tID;
                            //turma.codTurma = tCOD;
                            //turma.codOrcamentario = tOrcamento;
                            turma.turma = tDESC;
                            turma.situacao = tSITUACAO;
                            turma.dataInicial = tDTINICIO;
                            turma.dataFinal = tDTFINAL;
                            turma.horaInicial = hINICIO;
                            turma.horaFinal = hFINAL;
                            turma.turno = tTURNO;
                            turma.cargaHoraria = tCH;
                            turma.vagas = tVAGAS;
                            turma.numAlunos = tNALUNOS;
                            turma.idUnidade = tUNIDADE;

                        }

                        conexao.Entry<TURMA>(turma).State = turma.id == 0 ? System.Data.Entity.EntityState.Added : System.Data.Entity.EntityState.Modified;

                        txtSincronTurma.Text = string.Empty;

                    }

                    builder.Clear();
                    dataTable.Clear();
                    conexao.SaveChanges();
                }
            }

        }
        //Sincronização Unidade.
        protected void btnSincronizarUnidade_Click(object sender, EventArgs e)
        {

            DataTable dataTable = new DataTable();
            StringBuilder builder = new StringBuilder();

            builder.Append("  SELECT PJU_ID, PJU_NOMEFANTASIA FROM PESSOASJURIDICAS WHERE PJU_ID < 99 ");


            dataTable = GetDataTable(builder);

            if (dataTable.Rows.Count > 0)
            {
                using (estoqueEntities conexao = new estoqueEntities())
                {

                    UNIDADE unidade = null;

                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        int unidadeSeiId = Convert.ToInt32(dataTable.Rows[i]["PJU_ID"].ToString());
                        string nomeFantasia = dataTable.Rows[i]["PJU_NOMEFANTASIA"].ToString();


                        unidade = conexao.UNIDADE.Where(p => p.idUnidade == unidadeSeiId).FirstOrDefault();


                        if (unidade == null)
                        {
                            unidade = new UNIDADE()
                            {
                                idUnidade = unidadeSeiId,
                                unidade = nomeFantasia,
                                status = true
                            };
                        }
                        else
                        {
                            unidade.idUnidade = unidadeSeiId;
                            unidade.unidade = nomeFantasia;
                        }

                        conexao.Entry<UNIDADE>(unidade).State = unidade.id == 0 ? System.Data.Entity.EntityState.Added : System.Data.Entity.EntityState.Modified;


                    }

                    builder.Clear();
                    dataTable.Clear();
                    conexao.SaveChanges();

                }

            }

        }
        //Sincronização Curso.
        protected void btnSincronizarCurso_Click(object sender, EventArgs e)
        {
            DataTable dataTable = new DataTable();
            StringBuilder builder = new StringBuilder();

            builder.Append(" SELECT DISTINCT GC.CUR_ID CODIGO_CURSO, GC.CUR_DESCRICAO NOME_CURSO, PJ.PJU_ID AS ID_UNIDADE ");
            builder.Append(" FROM SAE.TURMAS T ");
            builder.Append(" LEFT JOIN SAE.PROGRAMACOES P ON P.PRO_ID = T.PRO_ID INNER JOIN PESSOASJURIDICAS PJ ON P.UNI_ID = PJ.PJU_ID ");
            builder.Append(" INNER JOIN GRADESCURRICULARES GC ON GC.GRA_ID = P.GRA_ID ");
            builder.Append(" WHERE GC.CUR_ID = " + txtSincronCurso.Text.Trim());
            builder.Append(" GROUP BY PJ.PJU_ID, P.PRO_NOME, GC.CUR_ID, GC.CUR_DESCRICAO ");

            dataTable = GetDataTable(builder);

            if (dataTable.Rows.Count > 0)
            {
                using (estoqueEntities conexao = new estoqueEntities())
                {
                    CURSOS curso = null;

                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {

                        int cursoID = Convert.ToInt32(dataTable.Rows[i]["CODIGO_CURSO"].ToString());
                        string cursoDESC = dataTable.Rows[i]["NOME_CURSO"].ToString();
                        int unidadeID = Convert.ToInt32(dataTable.Rows[i]["ID_UNIDADE"].ToString());

                        curso = conexao.CURSOS.Where(p => p.idCurso == cursoID && p.idUnidade == unidadeID).FirstOrDefault();

                        if (curso == null)
                        {
                            curso = new CURSOS()
                            {
                                idCurso = cursoID,
                                curso = cursoDESC,
                                idUnidade = unidadeID,

                            };
                        }
                        else
                        {

                            curso.curso = cursoDESC;
                            curso.idUnidade = unidadeID;

                        }

                        conexao.Entry<CURSOS>(curso).State = curso.id == 0 ? System.Data.Entity.EntityState.Added : System.Data.Entity.EntityState.Modified;

                        txtSincronCurso.Text = string.Empty;
                    }

                    builder.Clear();
                    dataTable.Clear();
                    conexao.SaveChanges();
                }
            }
        }
        // Sincronizar Tabela Disciplina, Tabela Instrutor e tabela Disciplina/instrutor.
        protected void btnSincronizarDisciplina_Click(object sender, EventArgs e)
        {
            DataTable dataTable = new DataTable();
            StringBuilder builder = new StringBuilder();

            // nova consulta

            //builder.Append(" SELECT DISTINCT DIS.ID AS ID_DISCIPLINA, DIS.DIS_NOME AS DISCIPLINA, TDT.TUR_DTINICIAL AS DATA_INICIAL, TDT.TUR_DTFINAL AS DATA_FINAL, TO_CHAR(P.PRO_HRINICIAL, 'HH24:MI') AS HORARIO_INICIAL, ");
            //builder.Append(" TO_CHAR(P.PRO_HRFINAL, 'HH24:MI') AS HORARIO_FINAL, T.TUR_CH AS CH_DISCIPLINA, PESU.PES_NOME AS SUPERVISOR, PFU.PFI_CPF AS CPF_SUPERVISOR, PES.PES_NOME AS INSTRUTOR, PES.PES_ID AS INSTRUTOR_ID, ");
            //builder.Append(" PF.PFI_CPF AS CPF_INSTRUTOR, P.PRO_FREQSEMANAL AS FREQUENCIA_SEMANAL, P.ID AS ID_TURMA FROM SAE.TURMAS T INNER JOIN TURMASDATAS TDT ON T.ID = TDT.TUR_ID AND TDT.TDT_STATUS='I' ");
            //builder.Append(" INNER JOIN SAE.DISCIPLINAS DIS ON T.DIS_ID = DIS.DIS_ID INNER JOIN DISCIPLINASGRADES DGR ON DIS.DIS_ID = DGR.DIS_ID INNER JOIN SAE.PROGRAMACOES P ON P.PRO_ID = T.PRO_ID INNER JOIN SAE.VW_PROGRAMACOESCURSO PROG ON P.PRO_ID = PROG.PRO_ID ");
            //builder.Append(" INNER JOIN PESSOASJURIDICAS PJ ON P.UNI_ID = PJ.PJU_ID INNER JOIN GRADESCURRICULARES GC ON GC.GRA_ID = P.GRA_ID LEFT JOIN SAE.INSTRUTORESTURMAS INS ON T.ID = INS.TUR_ID AND INS.ITR_TIPO='I' ");
            //builder.Append(" LEFT JOIN SAE.INSTRUTORESTURMAS SUP ON T.ID = SUP.TUR_ID AND SUP.ITR_TIPO='O' LEFT JOIN SALASPORTURMAS SPT ON T.TUR_ID = SPT.TUR_ID  INNER JOIN SALAS SAL ON SPT.SAL_ID = SAL.ID INNER JOIN PES.PESSOAS PES ON INS.PFI_ID = PES.PES_ID ");
            //builder.Append(" LEFT JOIN PES.PESSOAS PESU ON SUP.PFI_ID = PESU.PES_ID INNER JOIN PES.PESSOASFISICAS PF ON PES.PES_ID = PF.PFI_ID LEFT JOIN PES.PESSOASFISICAS PFU ON PESU.PES_ID = PFU.PFI_ID ");
            //builder.Append(" WHERE TDT.TUR_DTINICIAL > '01/01/2015' ");
            ////builder.Append(" WHERE P.ID = " + txtSicronDisciplina.Text.Trim());
            //builder.Append(" GROUP BY DIS.ID, DIS.DIS_NOME, T.TUR_CH, P.PRO_FREQSEMANAL, P.ID, PES.PES_ID, TDT.TUR_DTINICIAL, TDT.TUR_DTFINAL, TO_CHAR(P.PRO_HRINICIAL, 'HH24:MI'), TO_CHAR(P.PRO_HRFINAL, 'HH24:MI'), PESU.PES_NOME, PFU.PFI_CPF, PES.PES_NOME, PF.PFI_CPF ");

            builder.Append(" SELECT DISTINCT DIS.ID AS ID_DISCIPLINA, DIS.DIS_NOME AS DISCIPLINA, TDT.TUR_DTINICIAL AS DATA_INICIAL, TDT.TUR_DTFINAL AS DATA_FINAL, TO_CHAR(P.PRO_HRINICIAL, 'HH24:MI') AS HORARIO_INICIAL, ");
            builder.Append(" TO_CHAR(P.PRO_HRFINAL, 'HH24:MI') AS HORARIO_FINAL, T.TUR_CH AS CH_DISCIPLINA, PESU.PES_NOME AS SUPERVISOR, PFU.PFI_CPF AS CPF_SUPERVISOR, PES.PES_NOME AS INSTRUTOR, PES.PES_ID AS INSTRUTOR_ID, ");
            builder.Append(" P.UNI_ID AS UNIDADE_ID,PF.PFI_CPF AS CPF_INSTRUTOR, P.PRO_FREQSEMANAL AS FREQUENCIA_SEMANAL, P.ID AS ID_TURMA FROM SAE.TURMAS T INNER JOIN TURMASDATAS TDT ON T.ID = TDT.TUR_ID AND TDT.TDT_STATUS='I' ");
            builder.Append(" INNER JOIN SAE.DISCIPLINAS DIS ON T.DIS_ID = DIS.DIS_ID INNER JOIN DISCIPLINASGRADES DGR ON DIS.DIS_ID = DGR.DIS_ID INNER JOIN SAE.PROGRAMACOES P ON P.PRO_ID = T.PRO_ID ");
            builder.Append(" INNER JOIN SAE.VW_PROGRAMACOESCURSO PROG ON P.PRO_ID = PROG.PRO_ID  INNER JOIN PESSOASJURIDICAS PJ ON P.UNI_ID = PJ.PJU_ID INNER JOIN GRADESCURRICULARES GC ON GC.GRA_ID = P.GRA_ID ");
            builder.Append(" LEFT JOIN SAE.INSTRUTORESTURMAS INS ON T.ID = INS.TUR_ID AND INS.ITR_TIPO='I'  LEFT JOIN SAE.INSTRUTORESTURMAS SUP ON T.ID = SUP.TUR_ID AND SUP.ITR_TIPO='O' "); 
            builder.Append(" LEFT JOIN SALASPORTURMAS SPT ON T.TUR_ID = SPT.TUR_ID  INNER JOIN SALAS SAL ON SPT.SAL_ID = SAL.ID INNER JOIN PES.PESSOAS PES ON INS.PFI_ID = PES.PES_ID "); 
            builder.Append(" LEFT JOIN PES.PESSOAS PESU ON SUP.PFI_ID = PESU.PES_ID INNER JOIN PES.PESSOASFISICAS PF ON PES.PES_ID = PF.PFI_ID LEFT JOIN PES.PESSOASFISICAS PFU ON PESU.PES_ID = PFU.PFI_ID  ");
            builder.Append(" WHERE TDT.TUR_DTINICIAL > '01/01/2015' ");
            //builder.Append(" WHERE P.ID = " + txtSicronDisciplina.Text.Trim());
            builder.Append(" GROUP BY DIS.ID, DIS.DIS_NOME, T.TUR_CH, P.PRO_FREQSEMANAL, P.ID, PES.PES_ID, TDT.TUR_DTINICIAL, TDT.TUR_DTFINAL, TO_CHAR(P.PRO_HRINICIAL, 'HH24:MI'), "); 
            builder.Append(" TO_CHAR(P.PRO_HRFINAL, 'HH24:MI'), PESU.PES_NOME, PFU.PFI_CPF, PES.PES_NOME, PF.PFI_CPF, P.UNI_ID "); 

            dataTable = GetDataTable(builder);

            using (estoqueEntities conexao = new estoqueEntities())
            {
                using (var tran = conexao.Database.BeginTransaction())
                {
                    try
                    {
                        DISCIPLINA disciplina = null;
                        INSTRUTOR instrutor = null;
                        DISCIPLINA_INSTRUTOR discInstrutor = null;

                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            int disciplinaID = Convert.ToInt32(dataTable.Rows[i]["ID_DISCIPLINA"].ToString().Trim());
                            string disciplinaDesc = dataTable.Rows[i]["DISCIPLINA"].ToString();
                            DateTime dInicial = Convert.ToDateTime((dataTable.Rows[i]["DATA_INICIAL"].ToString().Trim()));
                            DateTime dFinal = Convert.ToDateTime((dataTable.Rows[i]["DATA_FINAL"].ToString().Trim()));
                            String hInicial = dataTable.Rows[i]["HORARIO_INICIAL"].ToString();
                            String hFinal = dataTable.Rows[i]["HORARIO_FINAL"].ToString();
                            int ch = Convert.ToInt32(dataTable.Rows[i]["CH_DISCIPLINA"].ToString());
                            String superv = dataTable.Rows[i]["SUPERVISOR"].ToString();
                            String cpfSuperv = dataTable.Rows[i]["CPF_SUPERVISOR"].ToString();
                            string disFreqSemanal = dataTable.Rows[i]["FREQUENCIA_SEMANAL"].ToString();
                            int idDaTurma = Convert.ToInt32(dataTable.Rows[i]["ID_TURMA"].ToString().Trim());

                            disciplina = conexao.DISCIPLINA.Where(p => p.idTurma == idDaTurma && p.idDisciplina == disciplinaID).FirstOrDefault();

                            if (disciplina == null)
                            {
                                disciplina = new DISCIPLINA()
                                {
                                    idDisciplina = disciplinaID,
                                    disciplina = disciplinaDesc,
                                    dataInicial = dInicial,
                                    dataFinal = dFinal,
                                    horaInicial = hInicial,
                                    horaFinal = hFinal,
                                    cargaHoraria = ch,
                                    supervisor = superv,
                                    cpfSupervisor = cpfSuperv,
                                    freqSemanal = disFreqSemanal,
                                    idTurma = idDaTurma,


                                };

                            }
                            else
                            {

                                disciplina.disciplina = disciplinaDesc;
                                disciplina.dataInicial = dInicial;
                                disciplina.dataFinal = dFinal;
                                disciplina.horaInicial = hInicial;
                                disciplina.horaFinal = hFinal;
                                disciplina.cargaHoraria = ch;
                                disciplina.supervisor = superv;
                                disciplina.cpfSupervisor = cpfSuperv;
                                disciplina.freqSemanal = disFreqSemanal;

                            }

                            conexao.Entry<DISCIPLINA>(disciplina).State = disciplina.id == 0 ? System.Data.Entity.EntityState.Added : System.Data.Entity.EntityState.Modified;

                            conexao.SaveChanges();

                            long instrutorID = Convert.ToInt64(dataTable.Rows[i]["INSTRUTOR_ID"].ToString().Trim());
                            string instrutorDESC = dataTable.Rows[i]["INSTRUTOR"].ToString();
                            string instrutorCPF = dataTable.Rows[i]["CPF_INSTRUTOR"].ToString();
                            int unidadeID = Convert.ToInt32(dataTable.Rows[i]["UNIDADE_ID"].ToString());

                            instrutor = conexao.INSTRUTOR.Where(p => p.cpf.Equals(instrutorCPF)).FirstOrDefault();

                            if (instrutor == null)
                            {
                                instrutor = new INSTRUTOR()
                                {
                                    idInstrutor = instrutorID,
                                    instrutor = instrutorDESC,
                                    cpf = instrutorCPF,
                                    idUnidade = unidadeID


                                };

                            }
                            else
                            {
                                instrutor.idInstrutor = instrutorID;
                                instrutor.instrutor = instrutorDESC;
                                //instrutor.cpf = instrutorCPF;
                                instrutor.idUnidade = unidadeID;

                            }

                            conexao.Entry<INSTRUTOR>(instrutor).State = instrutor.id == 0 ? System.Data.Entity.EntityState.Added : System.Data.Entity.EntityState.Modified;

                            conexao.SaveChanges();

                            //int disc_id = Convert.ToInt32(dataTable.Rows[i]["ID_DISCIPLINA"].ToString().Trim());
                            //long inst_id = Convert.ToInt64(dataTable.Rows[i]["INSTRUTOR_ID"].ToString().Trim());

                            discInstrutor = conexao.DISCIPLINA_INSTRUTOR.Where(p => p.idDisciplina == disciplina.idDisciplina && p.idInstrutor == instrutor.idInstrutor && p.idTurma == disciplina.idTurma).FirstOrDefault();

                            if (discInstrutor == null)
                            {
                                discInstrutor = new DISCIPLINA_INSTRUTOR()

                                {
                                    idDisciplina = disciplina.idDisciplina,
                                    idInstrutor = instrutor.idInstrutor,
                                    idTurma = disciplina.idTurma,
                                    
                                    //DISCIPLINA = disciplina,
                                    //INSTRUTOR = instrutor

                                };

                            }
                            else
                            {

                            }

                            conexao.Entry<DISCIPLINA_INSTRUTOR>(discInstrutor).State = discInstrutor.id == 0 ? System.Data.Entity.EntityState.Added : System.Data.Entity.EntityState.Modified;

                            conexao.SaveChanges();

                            txtSicronDisciplina.Text = string.Empty;

                        }

                        builder.Clear();
                        dataTable.Clear();
                        conexao.SaveChanges();
                        tran.Commit();

                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Warning", "alert('Atenção<br/> Entre em contato com a Administração do Sistema.', 'warning');", true);
                        throw;
                    }

                }
            }
        }

    }

}