﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ControleEstoque.Web.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ConexaoSql : DbContext
    {
        public ConexaoSql()
            : base("name=ConexaoSql")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<GC_AFASTADO> GC_AFASTADO { get; set; }
        public DbSet<GC_DEMISSAO_NOVO> GC_DEMISSAO_NOVO { get; set; }
        public DbSet<GC_DIRETORIO> GC_DIRETORIO { get; set; }
        public DbSet<GC_FERIAS> GC_FERIAS { get; set; }
        public DbSet<GC_FERIAS_SENDS_PENDENTES> GC_FERIAS_SENDS_PENDENTES { get; set; }
        public DbSet<GC_SITUACAO> GC_SITUACAO { get; set; }
        public DbSet<GC_USUARIOS> GC_USUARIOS { get; set; }
        public DbSet<CONTRATACAO_AUDITORIA> CONTRATACAO_AUDITORIA { get; set; }
        public DbSet<CONTRATACAO_COLABORADOR> CONTRATACAO_COLABORADOR { get; set; }
        public DbSet<CONTRATACAO_DIRETORIOS> CONTRATACAO_DIRETORIOS { get; set; }
        public DbSet<CONTRATACAO_FERIAS_SENDS_PENDENTES> CONTRATACAO_FERIAS_SENDS_PENDENTES { get; set; }
        public DbSet<CONTRATACAO_STATUS> CONTRATACAO_STATUS { get; set; }
        public DbSet<CONTRATACAO_SUBSTITUTO_ARQUIVO> CONTRATACAO_SUBSTITUTO_ARQUIVO { get; set; }
        public DbSet<CONTRATACAO_USUARIO> CONTRATACAO_USUARIO { get; set; }
        public DbSet<CONTRATACAO_ARQUIVO> CONTRATACAO_ARQUIVO { get; set; }
        public DbSet<CONTRATACAO_SUBSTITUTO> CONTRATACAO_SUBSTITUTO { get; set; }
    }
}
