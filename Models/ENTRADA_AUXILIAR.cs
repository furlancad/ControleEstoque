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
    
    public partial class ENTRADA_AUXILIAR
    {
        public int id { get; set; }
        public int codSei { get; set; }
        public int idDeposito { get; set; }
        public Nullable<System.DateTime> dataVencimento { get; set; }
        public Nullable<int> quantEntrada { get; set; }
        public string localizacao { get; set; }
        public string observacao { get; set; }
        public int idUsuario { get; set; }
    }
}
