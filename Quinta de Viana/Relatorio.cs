using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quinta_de_Viana
{
    class Relatorio
    {
        public int qtdade{ get; set; }
        public int codConta { get; set; }
        public int cod { get; set; }
        public string nome { get; set; }
        public string data { get; set; }
        public double preco { get; set; }

        public string formata()
        {
            return qtdade + " " + codConta + " " + cod + " " + nome + " " + preco+ " "+ data + " " + "\r\n";
 

        }
    }
}
