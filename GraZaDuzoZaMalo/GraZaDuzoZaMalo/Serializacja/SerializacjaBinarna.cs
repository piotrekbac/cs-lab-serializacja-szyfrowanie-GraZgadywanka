using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using GraZaDuzoZaMalo.Model;

//Piotr Bacior 15 722 - WSEI Kraków

#pragma warning disable SYSLIB0011 // Wyłącza ostrzeżenie o przestarzałym BinaryFormatter

namespace GraZaDuzoZaMalo.Serializacja
{
    //Definiujemy klasę SerializacjaBinarna, która będzie odpowiadać za serializację i deserializację stanu gry do pliku binarnego
    public static class SerializacjaBinarna
    {
        //Teraz definiujemy stałą, która będzie przechowywać nazwę pliku, do którego będziemy zapisywać stan gry
        private const string NazwaPliku = "stanGryBP.bin";

        //Definiujemy teraz metodę Zapisz, która będzie odpowiadać za zapisanie stanu gry do pliku binarnego
        public static void Zapisz(StanGry stan)
        {
            //Sprawdzamy, czy stan gry jest nie null, jeśli tak to zapisujemy go do pliku binarnego
            try
            {
                //Tworzymy teraz strumień do pliku, który nadpisuje plik jeśli istnieje lub tworzy nowy, jeśli nie istnieje
                using (FileStream fs = new FileStream(NazwaPliku, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, stan);
                }
            }
            //Jeśli wystąpi błąd podczas zapisu, to wyświetlamy komunikat o błędzie
            catch (Exception ex)
            {
                //Wyświetlamy komunikat o błędzie zapisu
                Console.WriteLine($"Błąd zapisu: {ex.Message}");
            }
        }

        //Definiujemy teraz metodę Wczytaj, która będzie odpowiadać za odczytanie stanu gry z pliku binarnego
        public static StanGry Wczytaj()
        {
            //Sprawdzamy, czy plik istnieje, jeśli tak to odczytujemy stan gry z pliku binarnego
            try
            {
                //Sprawdzamy, czy plik istnieje, jeśli nie to zwracamy null
                if (!File.Exists(NazwaPliku)) return null;

                //Tworzymy strummień do odczytu pliku, który otwiera plik w trybie odczytu
                using (FileStream fs = new FileStream(NazwaPliku, FileMode.Open))
                {
                    //Tworzymy BinaryFormatter, który będzie odpowiadać za deserializację stanu gry z pliku binarnego
                    BinaryFormatter formatter = new BinaryFormatter();

                    //Deserializujemy stan gry z pliku binarnego i zwracamy go - czyli innymi słowy, odczytujemy stan gry z pliku binarnego
                    return (StanGry)formatter.Deserialize(fs);
                }
            }

            //Jeśli wystąpi błąd podczas odczytu, to wyświetlamy komunikat o błędzie i zwracamy null
            catch (Exception ex)
            {
                //Wyświetlamy komunikat o błędzie odczytu
                Console.WriteLine($"Błąd odczytu: {ex.Message}");

                //Zwracamy null, ponieważ nie udało się odczytać stanu gry
                return null;
            }
        }


        //Definiujemy teraz metodę Usun, która będzie odpowiadać za usunięcie pliku z zapisanym stanem gry
        public static void Usun()
        {
            //Sprawdzamy, czy plik istnieje, jeśli tak to usuwamy go
            try
            {
                //Sprawdzamy, czy plik istnieje, jeśli tak to go usuwamy
                if (File.Exists(NazwaPliku)) File.Delete(NazwaPliku);
            }
            //Jeśli wystąpi błąd podczas usuwania pliku, to wyświetlamy komunikat o błędzie
            catch (Exception ex)
            {
                //Wyświetlamy komunikat o błędzie usuwania pliku
                Console.WriteLine($"Błąd usuwania pliku: {ex.Message}");
            }
        }

        //Definiujemy teraz metodę Istnieje, która będzie odpowiadać za sprawdzenie, czy plik z zapisanym stanem gry istnieje
        public static bool Istnieje() => File.Exists(NazwaPliku);
    }
}

