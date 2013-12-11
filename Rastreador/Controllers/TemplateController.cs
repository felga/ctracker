using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rastreador;
using Rastreador.Models;
using Microsoft.Security.Application;
using Helpers;

namespace Rastreador.Controllers
{   
    [Authorize(Roles="admin")]
    public class TemplateController : Controller
    {
		private readonly ITemplateRepository templateRepository;

		// If you are using Dependency Injection, you can delete the following constructor
        public TemplateController() : this(new TemplateRepository())
        {
        }

        public TemplateController(ITemplateRepository templateRepository)
        {
			this.templateRepository = templateRepository;
        }

        //
        // GET: /Template/

        public ViewResult Index()
        {
            return View(templateRepository.AllIncluding(template => template.Usuario));
        }

        //
        // GET: /Template/Details/5

        public ViewResult Details(int id)
        {
            return View(templateRepository.Find(id));
        }

        //
        // GET: /Template/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Template/Create

        [HttpPost]
        public ActionResult Create(Template template)
        {
            if (ModelState.IsValid) {
                //HtmlUtility util = HtmlUtility.Instance;
                //template.HTML = util.SanitizeHtml(template.HTML);
                templateRepository.InsertOrUpdate(template);
                templateRepository.Save();
                return RedirectToAction("Index");
            } else {
				return View();
			}
        }
        
        //
        // GET: /Template/Edit/5
 
        public ActionResult Edit(int id)
        {
             return View(templateRepository.Find(id));
        }

        //
        // POST: /Template/Edit/5

        [HttpPost]
        public ActionResult Edit(Template template)
        {
            if (ModelState.IsValid) {
                //HtmlUtility util = HtmlUtility.Instance;
                //template.HTML = util.SanitizeHtml(template.HTML);
                templateRepository.InsertOrUpdate(template);
                templateRepository.Save();
                return RedirectToAction("Index");
            } else {
				return View();
			}
        }

        //
        // GET: /Template/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View(templateRepository.Find(id));
        }

        //
        // POST: /Template/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            templateRepository.Delete(id);
            templateRepository.Save();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                templateRepository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

