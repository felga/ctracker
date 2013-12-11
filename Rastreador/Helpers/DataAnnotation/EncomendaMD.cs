using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;
using Rastreador.Models;

namespace Rastreador
{ 
    [MetadataType(typeof(EncomendaMD))]
    public partial class Encomenda
    {
    }

    public partial class EncomendaMD
    {
        [Required(ErrorMessage="* Campo requerido")]
        [DisplayName("Descrição: (Ex.: Televisão) ")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "* Campo requerido")]
        [DisplayName("Código: ")]
        public string Codigo { get; set; }
		
    }
}

