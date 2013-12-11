using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using Rastreador.Controllers;
using Rastreador.Models;

namespace Rastreador.Helpers
{
    public class AtualizaEncomendasJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            IEncomendaRepository encomendaRepository = new EncomendaRepository();
            ITemplateRepository templateRepository = new TemplateRepository();
            Template TemplateDefault = templateRepository.All.Where(x => x.IsDefault).FirstOrDefault();

            foreach (Encomenda encomenda in encomendaRepository.All.Where(x => x.UltimaAtualizacao != "Entrega Efetuada"))
            {
                string ultimaatualizacao = Util.GetUltimaAtualizacaoCorreios(encomenda.Codigo);
                if (encomenda.UltimaAtualizacao != ultimaatualizacao.Split('#')[1])
                {
                    IEncomendaRepository encomendaRepository1 = new EncomendaRepository();
                    Encomenda e = encomendaRepository1.Find(encomenda.Id);
                    e.DataUltimaAtualizacao = DateTime.Parse(ultimaatualizacao.Split('#')[0]);
                    e.UltimaAtualizacao = ultimaatualizacao.Split('#')[1];
                    encomendaRepository1.InsertOrUpdate(e);
                    encomendaRepository1.Save();

                    SendMail oSendMail = new SendMail();
                    oSendMail.setAssunto("Atualização de Status de Encomenda");
                    if (e.Usuario.Template != null)
                    {
                        oSendMail.setCorpo(e.Usuario.Template.TemplateFomatado(e));
                    }
                    else
                    {
                        oSendMail.setCorpo(TemplateDefault.TemplateFomatado(e));
                    }
                    oSendMail.setRemetente("contato@celtica.com.br");
                    oSendMail.setDestinatario(e.Email);
                    oSendMail.setServidorSMTP("smtp.celtica.com.br");
                    oSendMail.sendEmail();
                }
            }
            foreach (Encomenda encomenda in encomendaRepository.All.Where(x => x.UltimaAtualizacao == "Sem atualização"))
            {
                IEncomendaRepository encomendaRepository1 = new EncomendaRepository();
                Encomenda e = encomendaRepository1.Find(encomenda.Id);
                e.DataUltimaAtualizacao = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                encomendaRepository1.InsertOrUpdate(e);
                encomendaRepository1.Save();
            }
        }
    }
}