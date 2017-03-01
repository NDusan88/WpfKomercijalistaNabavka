using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfStavkaRacuni
{
    class StavkaRacuna
    {
        public int StavkaRacunaId { get; set; }
        public int RacunId { get; set; }
        public int ProdavnicaId { get; set; }
        public int Kolicina { get; set; }
        public string Proizvod { get; set; }
        public override string ToString()
        {
            return string.Format("{0} - Kolicina: {1}", Proizvod, Kolicina);
        }
    }
}
