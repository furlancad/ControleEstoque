//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class GC_FERIAS_SENDS_PENDENTES
    {
        public long id { get; set; }
        public string chapa { get; set; }
        public Nullable<long> matricula { get; set; }
        public string nome { get; set; }
        public string cpf { get; set; }
        public Nullable<System.DateTime> data_inicio_ferias { get; set; }
        public Nullable<System.DateTime> data_ultimo_email_enviado { get; set; }
        public Nullable<long> quantidade_sends_pendentes { get; set; }
        public Nullable<long> quantidade_emails_enviados { get; set; }
    }
}