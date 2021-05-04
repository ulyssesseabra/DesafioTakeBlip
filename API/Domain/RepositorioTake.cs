using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesafioBlip
{
    /// <summary>
    /// Classe com a propriedades exigidas para criação do carrossel
    /// </summary>
    public class RepositorioTake
    {
        public string AvatarURL { get; set; }
        public string Titulo { get; set; }
        public string SubTitulo { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
