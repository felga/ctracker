using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace Rastreador.Helpers
{
    public class SendMail
    {
        private string vStrDestinatario;
        private string vStrRemetente;
        private string vStrNomeRemetente;
        private string vStrServidorSMTP;
        private string vStrAssunto;
        private string vStrCorpo;
        private string vStrAnexo;

        #region Destinatario
        public void setDestinatario(string vStrDestinatario)
        {
            this.vStrDestinatario = vStrDestinatario;
        }

        public string getDestinatario()
        {
            return this.vStrDestinatario;
        }
        #endregion

        #region Remetente
        public void setRemetente(string vStrRemetente)
        {
            this.vStrRemetente = vStrRemetente;
            this.vStrNomeRemetente = "cTracker";
        }

        public void setRemetente(string vStrRemetente, string vStrNomeRemetente)
        {
            this.vStrRemetente = vStrRemetente;
            this.vStrNomeRemetente = vStrNomeRemetente;
        }

        public string getRemetente()
        {
            return this.vStrRemetente;
        }
        #endregion

        #region ServidorSMTP
        public void setServidorSMTP(string vStrServidorSMTP)
        {
            this.vStrServidorSMTP = vStrServidorSMTP;
        }

        public string getServidorSMTP()
        {
            return this.vStrServidorSMTP;
        }
        #endregion

        #region Assunto
        public void setAssunto(string vStrAssunto)
        {
            this.vStrAssunto = vStrAssunto;
        }

        public string getAssunto()
        {
            return this.vStrAssunto;
        }
        #endregion

        #region Corpo
        public void setCorpo(string vStrCorpo)
        {
            this.vStrCorpo = vStrCorpo;
        }

        public string getCorpo()
        {
            return this.vStrCorpo;
        }
        #endregion

        #region Anexo
        public void setAnexo(string vStrAnexo)
        {
            this.vStrAnexo = vStrAnexo;
        }

        public string getAnexo()
        {
            return this.vStrAnexo;
        }
        #endregion


        private MailMessage constroiMensagem(List<string> destinatarios)
        {
            MailAddress MailFrom = new MailAddress(vStrRemetente, vStrNomeRemetente);
            MailMessage oMailMessage = new MailMessage(MailFrom, MailFrom); //this.vStrRemetente, this.vStrDestinatario.ToString());
            //oMailMessage.From = MailFrom;
            //oMailMessage.To = MailFrom;
            foreach (string item in destinatarios)
            {
                oMailMessage.Bcc.Add(new MailAddress(item));
            }
            //ICollection<MailAddress> Destinos = new
            try
            {
                oMailMessage.Subject = this.vStrAssunto;
                oMailMessage.IsBodyHtml = true;
                oMailMessage.Body = this.vStrCorpo;
                //ANEXO
                if (vStrAnexo != null)
                {
                    oMailMessage.Attachments.Add(new System.Net.Mail.Attachment(vStrAnexo));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return oMailMessage;

        }

        public void sendEmail()
        {

            try
            {

                List<string> destinatarios = new List<string>();
                foreach (string item in vStrDestinatario.Split(','))
                {
                    destinatarios.Add(item);
                    if (destinatarios.Count >= 10)
                    {
                        SmtpClient client = new SmtpClient();
                        client.Host = "mail.celtica.com.br";
                        client.Port = 25;
                        client.Credentials = new NetworkCredential("ctracker@celtica.com.br", "cTracker123");
                        client.Send(constroiMensagem(destinatarios));
                        destinatarios.Clear();
                    }
                }
                if (destinatarios.Count > 0)
                {
                    SmtpClient client = new SmtpClient();
                    client.Host = "mail.celtica.com.br";
                    client.Port = 25;
                    client.Credentials = new NetworkCredential("ctracker@celtica.com.br", "cTracker123");
                    client.Send(constroiMensagem(destinatarios));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}