using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Linq;

namespace DesafioBlip.Business
{
    public class RepositorioTakeBusiness
    {
        /// <summary>
        ///  Listar informações sobre os 5 repositórios de linguagem C# mais antigos da Take,ordenados de forma crescente por data de criação.
        ///  Como o GitHub não tem o parametro language para filtrar os resultados dos repositórios da organização, serão coletados todos o registros
        ///  e filtrados.
        ///  Considerando que não se sabe a quantidade de repositórios e sendo essa quantida de flexível, a rotina irá primeiro consultar a quantidade de 
        ///  repositórios da organização e logo buscar de forma paginada.
        ///  Identifiquei que repositorios com mais de uma liguagem vem com a language null, e deve-se buscar as linguagens pelo languages_url, 
        /// não capturei todos por esse metodo para não criar sobrecarga de requisições.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RepositorioTake> GetRepositoriosAntigoCSharp()
        {
            int TotalRepositorios = 0;
            int TotalPaginas = 0;
            List<RepositorioTake> ret = new List<RepositorioTake>();

            HttpClient clientGitHUb = new HttpClient();
            clientGitHUb.BaseAddress = new Uri(@"https://api.github.com");
            clientGitHUb.DefaultRequestHeaders.Accept.Clear();
            clientGitHUb.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            clientGitHUb.DefaultRequestHeaders.Add("User-Agent", "request");

            System.Net.Http.HttpResponseMessage response;

           
            response = clientGitHUb.GetAsync("orgs/takenet").Result;
            if (response.IsSuccessStatusCode)
            {
                TotalRepositorios = ((dynamic)JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result)).public_repos;

              
                TotalPaginas = (int)Math.Ceiling(TotalRepositorios / 100.00);

                for (int pg = 1; pg <= TotalPaginas; pg++)
                {
                    response = clientGitHUb.GetAsync("orgs/takenet/repos?per_page=100&page=" + pg + "&sort=created&direction=asc").Result;
                    if (response.IsSuccessStatusCode)
                    {  
                        string content = response.Content.ReadAsStringAsync().Result;
                        dynamic result = JsonConvert.DeserializeObject(content);
                        for (int index = 0; index < result.Count; index++)
                        {
                            if (result[index].language == "C#")
                            {
                                ret.Add(new RepositorioTake()
                                {
                                    Titulo = result[index].full_name,
                                    SubTitulo = result[index].description,
                                    AvatarURL = result[index].owner.avatar_url,
                                    DataCriacao = result[index].created_at
                                });
                            }

                           
                            if (result[index].language == null)
                            {
                                if (GetRepositorioLanguages((string)result[index].languages_url, "C#"))
                                {
                                    ret.Add(new RepositorioTake()
                                    {
                                        Titulo = result[index].full_name,
                                        SubTitulo = result[index].description,
                                        AvatarURL = result[index].owner.avatar_url,
                                        DataCriacao = result[index].created_at
                                    });

                                }
                            }

                        }
                    }
                    else
                    {
                        throw new Exception("GitHub API: (" + response.StatusCode.ToString() + ") " + response.ReasonPhrase);// Tratamento devido o limite de 60 requisições hora

                    }
                }
            }
            else
            {
                throw new Exception("GitHub API: (" + response.StatusCode.ToString() + ") " + response.ReasonPhrase);// Tratamento devido o limite de 60 requisições hora

            }
            clientGitHUb.Dispose();

            ret = (from repo in ret
                   orderby repo.DataCriacao
                   select repo).Take(5).ToList();


            return ret;
        }
        public bool GetRepositorioLanguages(string languages_url, string searchLanguage)
        {

            bool ret = false;
            HttpClient clientGitHUb = new HttpClient();
            clientGitHUb.BaseAddress = new Uri(@"https://api.github.com");
            clientGitHUb.DefaultRequestHeaders.Accept.Clear();
            clientGitHUb.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            clientGitHUb.DefaultRequestHeaders.Add("User-Agent", "request");
            System.Net.Http.HttpResponseMessage response;
            response = clientGitHUb.GetAsync(languages_url.Replace(@"https://api.github.com/", "")).Result;
            if (response.IsSuccessStatusCode)
            {
                string stringResult = response.Content.ReadAsStringAsync().Result;
                if (stringResult.Contains(searchLanguage))
                    ret = true;
                else
                    ret = false;
            }
            clientGitHUb.Dispose();
            return ret;
        }
    }
}