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
    [MetadataType(typeof(TemplateMD))]
    public partial class Template
    {
        public string TemplateFomatado(Encomenda e) {
            string texto = HTML;
            texto = texto.Replace("{Nome}", e.Nome);
            texto = texto.Replace("{Data}", e.DataUltimaAtualizacao.ToString("dd/MM/yyyy HH:mm"));
            string ultimaat = e.Status;
            if (ultimaat == "Encaminhado") {
                ultimaat = e.UltimaAtualizacao;
            }
            texto = texto.Replace("{Status}", ultimaat);
            return texto;
        }
    }

    public partial class TemplateMD
    {
        [Required(ErrorMessage = "* Campo requerido")]
        [DisplayName("HTML:")]
        [DataType(DataType.Html)]
        [AllowHtml]
        public string HTML { get; set; }

    }
}

