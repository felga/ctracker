//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Rastreador
{
    using System;
    using System.Collections.Generic;
    
    public partial class Encomenda
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public Nullable<System.DateTime> DataCadastro { get; set; }
        public string Codigo { get; set; }
        public System.Guid UsuarioId { get; set; }
        public string Email { get; set; }
        public string Transportadora { get; set; }
        public System.DateTime DataUltimaAtualizacao { get; set; }
        public string UltimaAtualizacao { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> UltimoEmail { get; set; }
    
        public virtual Usuario Usuario { get; set; }
    }
}
