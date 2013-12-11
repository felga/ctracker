using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rastreador;
using Rastreador.Models;
using Rastreador.Helpers;
using System.Xml;

namespace Rastreador.Controllers
{       
    public class EncomendaController : Controller
    {
		private readonly IUsuarioRepository usuarioRepository;
		private readonly IEncomendaRepository encomendaRepository;

		// If you are using Dependency Injection, you can delete the following constructor
        public EncomendaController() : this(new UsuarioRepository(), new EncomendaRepository())
        {
        }

        public EncomendaController(IUsuarioRepository usuarioRepository, IEncomendaRepository encomendaRepository)
        {
			this.usuarioRepository = usuarioRepository;
			this.encomendaRepository = encomendaRepository;
        }
        
        //
        // GET: /Encomenda/
        [Authorize]
        public ViewResult Index()
        {

            return View(encomendaRepository.AllIncluding(encomenda => encomenda.Usuario).Where(x => x.Usuario.Login == User.Identity.Name).OrderByDescending(x => x.DataUltimaAtualizacao));
        }

        //
        // GET: /Encomenda/Details/5
        [Authorize]
        public ViewResult Details(int id)
        {
            return View(encomendaRepository.Find(id));
        }

        //
        // GET: /Encomenda/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Encomenda/Create

        [HttpPost]
        [Authorize]
        public ActionResult Create(Encomenda encomenda)
        {
            Usuario ousuarioLogado = usuarioRepository.FindLogin(User.Identity.Name);
            encomenda.Email = ousuarioLogado.MembershipUser.Email;
            encomenda.UsuarioId = (Guid)ousuarioLogado.MembershipUser.ProviderUserKey;
            string ultimaatualizacao = Util.GetUltimaAtualizacaoCorreios(encomenda.Codigo);
            if (ultimaatualizacao != "") {
                encomenda.DataUltimaAtualizacao = DateTime.Parse(ultimaatualizacao.Split('#')[0]);
                encomenda.Status = ultimaatualizacao.Split('#')[1];                
                encomenda.UltimaAtualizacao = ultimaatualizacao.Split('#')[2];                
            }
            if (ModelState.IsValid) {
                encomendaRepository.InsertOrUpdate(encomenda);
                encomendaRepository.Save();
                return RedirectToAction("Index");
            } else {
				return View();
			}
        }

        //
        // GET: /Encomenda/Create
        public ActionResult EncomendaCriada()
        {
            return View();
        }

        [HttpPost]
        [BasicAuthorize]
        public ActionResult CreateMobile1(string descricao, string codigo)
        {
            Encomenda encomenda = new Encomenda();
            Usuario ousuarioLogado = usuarioRepository.FindLogin(User.Identity.Name);
            encomenda.Email = ousuarioLogado.MembershipUser.Email;
            encomenda.UsuarioId = (Guid)ousuarioLogado.MembershipUser.ProviderUserKey;
            encomenda.Nome = descricao;
            encomenda.Codigo = codigo;
            encomenda.Transportadora = "Correios";
            encomenda.DataCadastro = DateTime.Now;
            string ultimaatualizacao = Util.GetUltimaAtualizacaoCorreios(encomenda.Codigo);
            if (ultimaatualizacao != "")
            {
                encomenda.DataUltimaAtualizacao = DateTime.Parse(ultimaatualizacao.Split('#')[0]);
                encomenda.Status = ultimaatualizacao.Split('#')[1];
                encomenda.UltimaAtualizacao = ultimaatualizacao.Split('#')[2];
            }
            if (ModelState.IsValid)
            {
                encomendaRepository.InsertOrUpdate(encomenda);
                encomendaRepository.Save();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml("<?xml version='1.0' encoding='ISO-8859-1'?><encomendas>OK</encomendas>");
                return Content(xmlDoc.OuterXml, "text/xml");
            }
            else
            {
                return View();
            }
        }

        public ActionResult CreateMobile()
        {
            ViewBag.Usuario = Request.Params["loginusuario"];
            return View();
        }

        //
        // POST: /Encomenda/Create

        [HttpPost]
        public ActionResult CreateMobile(Encomenda encomenda)
        {
            Usuario ousuarioLogado = usuarioRepository.FindLogin(encomenda.Email);            
            encomenda.UsuarioId = (Guid)ousuarioLogado.MembershipUser.ProviderUserKey;
            string ultimaatualizacao = Util.GetUltimaAtualizacaoCorreios(encomenda.Codigo);
            if (ultimaatualizacao != "")
            {
                encomenda.DataUltimaAtualizacao = DateTime.Parse(ultimaatualizacao.Split('#')[0]);
                encomenda.Status = ultimaatualizacao.Split('#')[1];
                encomenda.UltimaAtualizacao = ultimaatualizacao.Split('#')[2];
            }
            if (ModelState.IsValid)
            {
                encomendaRepository.InsertOrUpdate(encomenda);
                encomendaRepository.Save();
                return RedirectToAction("EncomendaCriada");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult CreateFacebook(Encomenda encomenda)
        {
            Usuario ousuarioLogado = usuarioRepository.FindLogin(encomenda.Email);
            encomenda.UsuarioId = (Guid)ousuarioLogado.MembershipUser.ProviderUserKey;
            string ultimaatualizacao = Util.GetUltimaAtualizacaoCorreios(encomenda.Codigo);
            if (ultimaatualizacao != "")
            {
                encomenda.DataUltimaAtualizacao = DateTime.Parse(ultimaatualizacao.Split('#')[0]);
                encomenda.Status = ultimaatualizacao.Split('#')[1];
                encomenda.UltimaAtualizacao = ultimaatualizacao.Split('#')[2];
            }
            if (ModelState.IsValid)
            {
                encomendaRepository.InsertOrUpdate(encomenda);
                encomendaRepository.Save();
                return RedirectToAction("Index", "Facebook", new { userid = encomenda.Email.Split(':')[1] });
            }
            else
            {
                return View();
            }
        } 

        //
        // GET: /Encomenda/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
			ViewBag.PossibleUsuario = usuarioRepository.All;
             return View(encomendaRepository.Find(id));
        }

        //
        // POST: /Encomenda/Edit/5

        [HttpPost]
        [Authorize]
        public ActionResult Edit(Encomenda encomenda)
        {
            if (ModelState.IsValid) {
                encomendaRepository.InsertOrUpdate(encomenda);
                encomendaRepository.Save();
                return RedirectToAction("Index");
            } else {
				ViewBag.PossibleUsuario = usuarioRepository.All;
				return View();
			}
        }

        //
        // GET: /Encomenda/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            return View(encomendaRepository.Find(id));
        }

        //
        // POST: /Encomenda/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            encomendaRepository.Delete(id);
            encomendaRepository.Save();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [BasicAuthorize]
        public ActionResult DeleteMobile(int id)
        {
            encomendaRepository.Delete(id);
            encomendaRepository.Save();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<?xml version='1.0' encoding='ISO-8859-1'?><encomendas>OK</encomendas>");
            return Content(xmlDoc.OuterXml, "text/xml");
        }

        [HttpPost]
        public ActionResult DeleteFacebook(int id)
        {
            encomendaRepository.Delete(id);
            encomendaRepository.Save();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<?xml version='1.0' encoding='ISO-8859-1'?><encomendas>OK</encomendas>");
            return Content(xmlDoc.OuterXml, "text/xml");
        }

        [Authorize]
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                usuarioRepository.Dispose();
                encomendaRepository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

