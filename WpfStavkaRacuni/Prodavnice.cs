using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfStavkaRacuni
{
    class Prodavnice
    {
        public int ProdavnicaId { get; set; }
        public string Ime { get; set; }
        public string Adresa { get; set; }
        public override string ToString()
        {
            return string.Format("{0}", Ime);
        }
    }
}
