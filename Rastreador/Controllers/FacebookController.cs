using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Facebook;
using System.Net;
using System.Dynamic;
using System.IO;
using System.Web.Security;
using Rastreador.Models;

namespace Rastreador.Controllers
{
    public class FacebookController : Controller
    {
        //
        // GET: /Facebook/
        public ActionResult RedirecionarLogin()
        {
            return View();
        }
        public ActionResult Index()
        {
            Session["Redirect"] = "";
            string code = Request.QueryString["code"];
            string appId = "618892811458967";
            string appSecret = "e61f303ae68f738b1418cb96eafa15b1";
            if (code == "" || code == null)
            {

                Session["Redirect"] = "https://www.facebook.com/dialog/oauth?client_id=" + appId + "&redirect_uri=http://apps.facebook.com/rastreadorceltica/";
                return View("RedirecionarLogin");
            }
            var fb = new FacebookClient();
            string userid = Request.Params["userid"];
            if (Request.Params["userid"] == null || Request.Params["userid"] == "")
            {
                //return RedirectToAction("Login");
                dynamic signedRequest = fb.ParseSignedRequest("e61f303ae68f738b1418cb96eafa15b1", Request.Params["signed_request"]);
                userid = signedRequest.user_id;
            }
            dynamic cliente = fb.Get(userid);
            ViewBag.Nome = cliente.name;
            ViewBag.UID = userid;
            string facebookuser = "FacebookUser:" + userid;
            if (Membership.GetUser(facebookuser) == null)
            {
                MembershipCreateStatus createStatus;
                Membership.CreateUser(facebookuser, Membership.GeneratePassword(10,2), facebookuser, null, null, true, null, out createStatus);
                if (createStatus == MembershipCreateStatus.Success)
                {
                    MembershipUser currentUser = Membership.GetUser(facebookuser);
                    Usuario Usuario = new Usuario();
                    Usuario.Id = (Guid)currentUser.ProviderUserKey;
                    Usuario.Login = currentUser.UserName;
                    IUsuarioRepository usuarioRepository = new UsuarioRepository();
                    usuarioRepository.InsertOrUpdate(Usuario);
                    usuarioRepository.Save();
                    Roles.AddUserToRole(facebookuser, "usuario");
                }
            }
            


            return View();
        }

        public ActionResult Login() {
            
            string code = Request.QueryString["code"];
            string appId = "618892811458967";
            string appSecret = "e61f303ae68f738b1418cb96eafa15b1";

            if (code == "" || code == null)
            {              
                Response.Redirect("https://www.facebook.com/dialog/oauth?client_id=" + appId + "&redirect_uri=http://apps.facebook.com/rastreadorceltica/");
            }
            else
            {
                var fb = new FacebookClient();
                fb.AppSecret = appSecret;
                fb.AppId = appId;
                string accessToken = fb.AccessToken;
                fb.AccessToken = accessToken;                
            }
            return View();
        }
    }

}
