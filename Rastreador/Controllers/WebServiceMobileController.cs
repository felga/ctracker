using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using System.Xml;
using System.Web.Script.Services;
using System.Web.Security;
using System.Runtime.CompilerServices;
using System.Text;
using System.Security.Principal;
using Rastreador.Models;
using Rastreador.Helpers;

namespace Rastreador.Controllers
{
    [WebService]    
    public class WebServiceMobileController : Controller
    {

        [WebMethod]
        public ActionResult AtualizaToken(string email, string token)
        {
            IUsuarioRepository usuarioRepository = new UsuarioRepository();
            Usuario user = usuarioRepository.FindLogin(email);
            if(token != null && token.Length>0){
                user.Token = token;
                usuarioRepository.InsertOrUpdate(user);
                usuarioRepository.Save();
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<?xml version='1.0' encoding='ISO-8859-1'?><userid>" + user.Id.ToString() + "</userid>");
            return Content(xmlDoc.OuterXml, "text/xml"); ;
        }

        [WebMethod]
        public ActionResult AtualizaDevice(string email, string deviceid, string modelo)
        {
            IUsuarioRepository usuarioRepository = new UsuarioRepository();
            Usuario user = usuarioRepository.FindLogin(email);
            user.DeviceId = deviceid;
            user.Modelo = modelo;
            usuarioRepository.InsertOrUpdate(user);
            usuarioRepository.Save();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<?xml version='1.0' encoding='ISO-8859-1'?><userid>" + user.Id.ToString() + "</userid>");
            return Content(xmlDoc.OuterXml, "text/xml"); ;
        }

        [WebMethod]
        [BasicAuthorize]
        public ActionResult ValidaUsuario()
        {
            IUsuarioRepository usuarioRepository = new UsuarioRepository();
            Usuario user = usuarioRepository.FindLogin(User.Identity.Name);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<?xml version='1.0' encoding='ISO-8859-1'?><userid>" + user.Id.ToString()  + "</userid>");
            return Content(xmlDoc.OuterXml, "text/xml"); ;
        }

        [WebMethod]
        [BasicAuthorize]
        public ActionResult MinhasEncomendas()
        {
            IUsuarioRepository usuarioRepository = new UsuarioRepository();
            IEncomendaRepository encomendaRepository = new EncomendaRepository();
            Usuario user = usuarioRepository.FindLogin(User.Identity.Name);
            //Usuario user = usuarioRepository.FindLogin("rodrigofelga@gmail.com");
            string encomendas = "";
            foreach (Encomenda encomenda in encomendaRepository.All.Where(x => x.UsuarioId == user.Id).OrderByDescending(x => x.DataUltimaAtualizacao))
            {
                encomendas += "<encomenda><id>" + encomenda.Id + "</id><codigo>" + encomenda.Codigo + "</codigo><nome>" + encomenda.Nome + "</nome><data>" + encomenda.DataUltimaAtualizacao.ToString("dd/MM/yyyy HH:mm") + "</data><status>" + encomenda.Status + "</status><atualizacao>" + encomenda.UltimaAtualizacao + "</atualizacao></encomenda>";
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<?xml version='1.0' encoding='ISO-8859-1'?><encomendas>" + encomendas + "</encomendas>");
            return Content(xmlDoc.OuterXml, "text/xml"); ;
        }
    }  
}
