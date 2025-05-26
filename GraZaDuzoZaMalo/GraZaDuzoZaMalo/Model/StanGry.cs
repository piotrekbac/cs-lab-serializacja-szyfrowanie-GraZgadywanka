using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Piotr Bacior 15 722 - WSEI Kraków

namespace GraZaDuzoZaMalo.Model
{
    //Definiujemy teraz Serializowalną klasę StanGry, która będzie przechowywać stan gry 
    [Serializable]
    public class StanGry
    {
        //Definiujemy teraz LiczbaDoOdgadniecia, która będzie przechowywać liczbę do odgadnięcia
        public int LiczbaDoOdgadniecia { get; set; }

        //Definiujemy teraz LiczbaProb, która będzie przechowywać liczbę prób wykonaną przez gracza
        public int LiczbaProb { get; set; }

        //Definiujemy teraz listę HistoriaRuchow, która będzie przechowywać historię ruchów gracza
        public List<int> HistoriaRuchow { get; set; }

        //Teraz definiujemy StartGry, która będzie przechowywać datę i czas rozpoczęcia gry
        public DateTime StartGry { get; set; }

        //Natomiast teraz przechodzimy do zdefiniowania CzasZawieszenia, która będzie przechowywać czas zawieszenia gry
        public TimeSpan CzasZawieszenia { get; set; }

        //Teraz natomiast definiujemy CzasZawieszeniaStart, która będzie przechowywać datę i czas rozpoczęcia zawieszenia gry
        public DateTime? CzasZawieszeniaStart { get; set; }

        //Finalnie definiujemy Status, który będzie przechowywać aktalny status gry (czy jest zawieszczona, skończona bądź dalej trwa)
        public StatusGry Status { get; set; }


        //Przechodzimy do właściwego zdefiniowania konstruktora klasy StanGry, który będzie inicjalizował wszystkie właściwości
        public StanGry()
        {
            //Inicjalizujemy pustą listę HistoriaRuchow, ustawiamy status gry na Trwa, a datę rozpoczęcia gry na aktualny czas
            HistoriaRuchow = new List<int>();

            //Teraz definiujemy status gry, który będzie ustawiony na Trwa, a czas zawieszenia na zero
            Status = StatusGry.Trwa;

            //Ustawiamy datę rozpoczęcia gry na aktualny czas
            StartGry = DateTime.Now;

            //Inicjalizujemy CzasZawieszenia na zero, ponieważ gra nie jest zawieszona
            CzasZawieszenia = TimeSpan.Zero;
        }
    }
}