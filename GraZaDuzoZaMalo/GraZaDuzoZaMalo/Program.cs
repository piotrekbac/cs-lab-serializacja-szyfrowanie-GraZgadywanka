using System;
using GraZaDuzoZaMalo.Model;
using GraZaDuzoZaMalo.Serializacja;
using GraZaDuzoZaMalo.Backup;

namespace GraZaDuzoZaMalo
{
    class Program
    {
        // Globalna zmienna przechowująca stan gry
        static StanGry stan;

        // 🔀 Zmienna decydująca o trybie serializacji: true = XML + szyfrowanie, false = binarna
        static bool uzyjSerializacjiXML = false;

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // 🔄 Wczytaj stan gry (jeśli istnieje) z odpowiedniego źródła
            if (uzyjSerializacjiXML ? SerializacjaXML.Istnieje() : SerializacjaBinarna.Istnieje())
            {
                Console.WriteLine("📦 Wykryto zapisany stan gry. Czy chcesz kontynuować? (T/N): ");
                string decyzja = Console.ReadLine()?.ToUpper();

                if (decyzja == "T")
                {
                    stan = uzyjSerializacjiXML ? SerializacjaXML.Wczytaj() : SerializacjaBinarna.Wczytaj();

                    if (stan != null)
                    {
                        Console.WriteLine("Stan gry został przywrócony.");
                        stan.Status = StatusGry.Trwa;
                        stan.StartGry = DateTime.Now; // Korekta czasu po przywróceniu
                    }
                    else
                    {
                        Console.WriteLine("Nie udało się przywrócić stanu. Rozpoczynanie nowej gry.");
                    }

                    if (uzyjSerializacjiXML) SerializacjaXML.Usun();
                    else SerializacjaBinarna.Usun();
                }
                else
                {
                    if (uzyjSerializacjiXML) SerializacjaXML.Usun();
                    else SerializacjaBinarna.Usun();
                }
            }

            // 🚀 Start nowej gry, jeśli nie wczytano stanu
            if (stan == null)
            {
                stan = new StanGry
                {
                    LiczbaDoOdgadniecia = new Random().Next(1, 100)
                };

                Console.WriteLine("Rozpoczęto nową grę!");
            }

            // 💾 Uruchom automatyczny backup
            var backup = new AutoBackup(() => stan);

            // 🎯 Główna pętla gry
            while (true)
            {
                Console.Write("Zgadnij liczbę (lub X, aby zakończyć i zapisać grę): ");
                string input = Console.ReadLine();

                if (input.ToUpper() == "X")
                {
                    stan.Status = StatusGry.Zawieszona;
                    stan.CzasZawieszenia += DateTime.Now - stan.StartGry;

                    if (uzyjSerializacjiXML) SerializacjaXML.Zapisz(stan);
                    else SerializacjaBinarna.Zapisz(stan);

                    Console.WriteLine("Gra została zapisana. Do zobaczenia!");
                    break;
                }

                if (int.TryParse(input, out int propozycja))
                {
                    stan.HistoriaRuchow.Add(propozycja);
                    stan.LiczbaProb++;

                    if (propozycja == stan.LiczbaDoOdgadniecia)
                    {
                        Console.WriteLine("Brawo! Odgadłeś!");
                        stan.Status = StatusGry.Zakonczona;
                        break;
                    }
                    else if (propozycja < stan.LiczbaDoOdgadniecia)
                    {
                        Console.WriteLine("Za mało!");
                    }
                    else
                    {
                        Console.WriteLine("Za dużo!");
                    }
                }
                else
                {
                    Console.WriteLine("Nieprawidłowe dane. Spróbuj ponownie.");
                }
            }

            // 📊 Podsumowanie
            Console.WriteLine($"\nLiczba prób: {stan.LiczbaProb}");
            Console.WriteLine("Twoje propozycje: " + string.Join(", ", stan.HistoriaRuchow));

            var czasTrwania = DateTime.Now - stan.StartGry - stan.CzasZawieszenia;
            Console.WriteLine($"Czas gry (bez zawieszeń): {czasTrwania:mm\\:ss}");

            backup.Zatrzymaj();
        }
    }
}
