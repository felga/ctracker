using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rastreador.Models;
using Rastreador.Helpers;
using System.Net;
using Facebook;
using System.Dynamic;
using System.IO;
using System.Threading;

namespace Rastreador.Controllers
{
    public class JobsController : Controller
    {

        public void AtualizaEncomendas()
        {
            Thread t = new Thread(Atualizar);
            t.Start();
        }
        private void Atualizar() {
            IEncomendaRepository encomendaRepository = new EncomendaRepository();
            foreach (Encomenda encomenda in encomendaRepository.All.Where(x => x.Status != "Entrega Efetuada"))
            {
                try
                {
                    string ultimaatualizacao = Util.GetUltimaAtualizacaoCorreios(encomenda.Codigo);
                    string ultimaat = ultimaatualizacao.Split('#')[2];
                    string ultimostatus = ultimaatualizacao.Split('#')[1];
                    if (ultimostatus.Length > 50)
                    {
                        ultimostatus = ultimostatus.Substring(0, 50);
                    }
                    if (encomenda.UltimaAtualizacao != ultimaat || encomenda.Status != ultimostatus)
                    {
                        IEncomendaRepository encomendaRepository1 = new EncomendaRepository();
                        Encomenda e = encomendaRepository1.Find(encomenda.Id);
                        e.DataUltimaAtualizacao = DateTime.ParseExact(ultimaatualizacao.Split('#')[0], "dd/MM/yyyy HH:mm", null);
                        e.Status = ultimaatualizacao.Split('#')[1];
                        if (e.Status.Length > 50)
                        {
                            e.Status = e.Status.Substring(0, 50);
                        }
                        e.UltimaAtualizacao = ultimaatualizacao.Split('#')[2];
                        encomendaRepository1.InsertOrUpdate(e);
                        encomendaRepository1.Save();

                        if (e.Email.StartsWith("FacebookUser:"))
                        {
                            EnviaNotificacaoFacebook(e);
                        }
                        else
                        {
                            EnviaEmail(e);
                        }
                        if (e.Usuario.Token != null && e.Usuario.Token != "")
                        {
                            EnviaPushiOS(e);
                        }
                    }
                }
                catch (Exception ex)
                {
                    EnviaEmailErro(encomenda, ex, "1");
                }

            }
            DateTime data = DateTime.Now.AddDays(-15);
            foreach (Encomenda encomenda in encomendaRepository.All.Where(x => x.UltimoEmail < data && x.Status == "Sem atualização"))
            {
                try
                {
                    if (encomenda.Usuario.Token != null && encomenda.Usuario.Token != "")
                    {
                        EnviaPushiOSSA(encomenda);
                    }
                    if (encomenda.Email.StartsWith("FacebookUser:"))
                    {
                        EnviaNotificacaoFacebookSA(encomenda);
                    }
                    IEncomendaRepository encomendaRepository1 = new EncomendaRepository();
                    Encomenda e = encomendaRepository1.Find(encomenda.Id);
                    e.UltimoEmail = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), "dd/MM/yyyy HH:mm", null);
                    encomendaRepository1.InsertOrUpdate(e);
                    encomendaRepository1.Save();
                }
                catch (Exception ex)
                {
                    EnviaEmailErro(encomenda,ex,"2");
                }

            }
            foreach (Encomenda encomenda in encomendaRepository.All.Where(x => x.Status == "Sem atualização"))
            {
                try
                {
                    IEncomendaRepository encomendaRepository1 = new EncomendaRepository();
                    Encomenda e = encomendaRepository1.Find(encomenda.Id);
                    e.DataUltimaAtualizacao = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), "dd/MM/yyyy HH:mm", null);
                    encomendaRepository1.InsertOrUpdate(e);
                    encomendaRepository1.Save();
                }
                catch (Exception ex)
                {
                    EnviaEmailErro(encomenda, ex, "3");
                }

            }
        }
        private void EnviaEmail(Encomenda enc) {
            try
            {
                ITemplateRepository templateRepository = new TemplateRepository();
                Template TemplateDefault = templateRepository.All.Where(x => x.IsDefault).FirstOrDefault();
                SendMail oSendMail = new SendMail();
                oSendMail.setAssunto("Atualização de Status de Encomenda");
                if (enc.Usuario.Template != null)
                {
                    oSendMail.setCorpo(enc.Usuario.Template.TemplateFomatado(enc));
                }
                else
                {
                    oSendMail.setCorpo(TemplateDefault.TemplateFomatado(enc));
                }
                oSendMail.setRemetente("ctracker@celtica.com.br");
                oSendMail.setDestinatario(enc.Email);

                oSendMail.setServidorSMTP("smtp.celtica.com.br");
                oSendMail.sendEmail();
            }
            catch (Exception)
            {
                

            }

        }
        private void EnviaEmailErro(Encomenda enc, Exception ex, string onde)
        {
            try
            {
                SendMail oSendMail = new SendMail();
                oSendMail.setAssunto("Erro em encomenda Encomenda" + onde);
                oSendMail.setCorpo("Enc id: " + enc.Id + "<br />" + ex.Message + "<br />" + ex.StackTrace);
                oSendMail.setRemetente("ctracker@celtica.com.br");
                oSendMail.setDestinatario("ctracker@celtica.com.br");

                oSendMail.setServidorSMTP("smtp.celtica.com.br");
                oSendMail.sendEmail();
            }
            catch (Exception)
            {
                
            }
        }
        private void EnviaPushiOS(Encomenda enc) {
            string textomensagem = "Encomenda: " + enc.Nome + "\nStatus: " + enc.Status;
            string url = @"http://api.redfoundry.com/v2/Application/SendPushNotification.api?applicationcode=DB1-1680F&applicationversion=1.0&messagetext=" + textomensagem + "&devicessearchrules=%3CArrayOfDevicesSearchRuleGroup%3E%3CDevicesSearchRuleGroup%3E%3CDevicesSearchRules%3E%3CDevicesSearchRule%3E%3CRuleType%3EProperties%3C/RuleType%3E%3CAttributeName%3Eemail%3C/AttributeName%3E%3CAttributeValue%3E" + enc.Usuario.Login + "%3C/AttributeValue%3E%3COperator%3EEqual%3C/Operator%3E%3C/DevicesSearchRule%3E%3C/DevicesSearchRules%3E%3COperator%3EEqual%3C/Operator%3E%3C/DevicesSearchRuleGroup%3E%3C/ArrayOfDevicesSearchRuleGroup%3E";
            var credCache = new CredentialCache();
        credCache.Add(new Uri(url), "Basic",
                  new NetworkCredential("DB1-1680F", "e1127c39-7066-405f-98b5-bb3d0f5627ef"));

            WebRequest request = WebRequest.Create(url);
            request.Credentials = credCache;
            HttpWebResponse retorno = (HttpWebResponse)request.GetResponse();
        }

        private void EnviaPushiOSSA(Encomenda enc)
        {
            TimeSpan tempo = DateTime.Now - (DateTime)enc.DataCadastro;
            int dias = tempo.Days;
            string textomensagem = "Sua encomenda " + enc.Nome + " não é atualizada há " + dias.ToString() + " dias. Você inseriu o código correto?";
            string url = @"http://api.redfoundry.com/v2/Application/SendPushNotification.api?applicationcode=DB1-1680F&applicationversion=1.0&messagetext=" + textomensagem + "&devicessearchrules=%3CArrayOfDevicesSearchRuleGroup%3E%3CDevicesSearchRuleGroup%3E%3CDevicesSearchRules%3E%3CDevicesSearchRule%3E%3CRuleType%3EProperties%3C/RuleType%3E%3CAttributeName%3Eemail%3C/AttributeName%3E%3CAttributeValue%3E" + enc.Usuario.Login + "%3C/AttributeValue%3E%3COperator%3EEqual%3C/Operator%3E%3C/DevicesSearchRule%3E%3C/DevicesSearchRules%3E%3COperator%3EEqual%3C/Operator%3E%3C/DevicesSearchRuleGroup%3E%3C/ArrayOfDevicesSearchRuleGroup%3E";
            var credCache = new CredentialCache();
            credCache.Add(new Uri(url), "Basic",
                      new NetworkCredential("DB1-1680F", "e1127c39-7066-405f-98b5-bb3d0f5627ef"));

            WebRequest request = WebRequest.Create(url);
            request.Credentials = credCache;
            HttpWebResponse retorno = (HttpWebResponse)request.GetResponse();
        }

        private void EnviaNotificacaoFacebookSA(Encomenda enc)
        {

            FacebookClient app = new FacebookClient(getAppAccessToken());
            app.AppId = "618892811458967";
            app.AppSecret = "e61f303ae68f738b1418cb96eafa15b1";
            dynamic parameters = new ExpandoObject();
            TimeSpan tempo = DateTime.Now - (DateTime)enc.DataCadastro;
            int dias = tempo.Days;
            parameters.template = "Sua encomenda " + enc.Nome + " não é atualizada há " + dias.ToString() + " dias. Você inseriu o código correto?";
            dynamic myId = app.Post(enc.Email.Split(':')[1] + "/notifications", parameters);
        }
        private void EnviaNotificacaoFacebook(Encomenda enc)
        {

            FacebookClient app = new FacebookClient(getAppAccessToken());
            app.AppId = "618892811458967";
            app.AppSecret = "e61f303ae68f738b1418cb96eafa15b1";
            dynamic parameters = new ExpandoObject();
            parameters.template = "A encomenda " + enc.Nome + " foi atualizada: " + enc.Status;
            dynamic myId = app.Post(enc.Email.Split(':')[1] + "/notifications", parameters);
        }

        private string getAppAccessToken()
        {
            string facebookAppId = "618892811458967";
            string facebookAppSecret = "e61f303ae68f738b1418cb96eafa15b1";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
                String.Format("https://graph.facebook.com/oauth/access_token?grant_type=client_credentials&client_id={0}&client_secret={1}",
                facebookAppId, facebookAppSecret));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var responseStream = new StreamReader(response.GetResponseStream());
            var responseString = responseStream.ReadToEnd();

            if (responseString.Contains("access_token="))
            {
                return responseString.Replace("access_token=", string.Empty);
            }

            return "";
        }
    }
}
