using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using HtmlAgilityPack;

namespace Rastreador.Helpers
{
    public static class Util
    {
        public static string GetUltimaAtualizacaoCorreios(string codigo) {
            string parametros = "?P_COD_UNI=" + codigo + "&P_LINGUA=001&P_TIPO=001";
            // Cria o objeto de requisição
            WebRequest requisicao = WebRequest.Create("http://websro.correios.com.br/sro_bin/txect01$.QueryList" + parametros);
            // Realiza a requisição
            HttpWebResponse retorno = (HttpWebResponse)requisicao.GetResponse();
            // Lê o objeto e faz a atribuição à variável
            StreamReader stream = new StreamReader(retorno.GetResponseStream(), Encoding.GetEncoding("ISO-8859-1"));
            string dados = stream.ReadToEnd();
            // Transforma em um documento HTML - HtmlAgilityPack
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(dados);
            // Captura apenas a tabela contendo os dados do envio
            HtmlNode tabela = html.DocumentNode.SelectSingleNode("//table");
            if (tabela != null)
            {
                string status = FormataSituacao(tabela.SelectNodes("//td")[5].InnerHtml);
                string atualizacao = FormataSituacao(tabela.SelectNodes("//td")[4].InnerHtml);
                if (status == "Encaminhado")
                {
                    atualizacao = tabela.SelectNodes("//td")[6].InnerHtml;
                }
                atualizacao = atualizacao.Split('-')[0].Trim();
                return tabela.SelectNodes("//td")[3].InnerHtml + "#" + status + "#" + atualizacao;               
            }
            else
                return DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "#Sem atualização#Sem atualização";

        }
        private static string FormataSituacao(string situacao) {
            if(situacao.Contains("font color")){
                string color = situacao.Substring(13, 6);                
                situacao = situacao.Replace("<font color=\"" + color + "\">","").Replace("</font>","");
            }                
            return situacao;
        }
    }
}