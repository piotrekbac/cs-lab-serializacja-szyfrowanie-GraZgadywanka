using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using GraZaDuzoZaMalo.Model;

//Piotr Bacior 15 722 - WSEI Kraków

namespace GraZaDuzoZaMalo.Serializacja
{
    //Definiujemy klasę SerializacjaXML, która będzie odpowiadać za serializację i deserializację stanu gry do pliku XML
    public static class SerializacjaXML
    {
        //Teraz definiujemy stałą, która będzie przechowywać nazwę pliku, do którego będziemy zapisywać stan gry
        private const string NazwaPliku = "stanGryPB.xml";

        //Teraz definiujemy klucz szyfrowania AES który musi mieć odpowiednią długość, a konkretnie 16 bajtów (128 bitów)
        private static readonly byte[] Klucz = Encoding.UTF8.GetBytes("SuperTajneHasloAhaPB1");

        //Definiujemy wektor inicjalizacyjny (IV) dla AES, który również musi mieć długość 16 bajtów
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("1234567812345678");

        //Przechodzimy teraz do serializacji, która będzie miała za zadanie przechowywać zaszyfrowane i jawne dany stanu naszej gry w pliku XML
        [DataContract]
        private class EncryptedGameState
        {
            //Definiujemy zaszyfrowaną liczbę do odgadnięcia, w postaci stringa, który będzie przechowywany w pliku XML
            [DataMember] public string EncryptedLiczbaDoOdgadniecia;

            //Definiujemy liczbę prób gracza, która będzie przechowywana jako liczba całkowita
            [DataMember] public int LiczbaProb;

            //Definiuemy historię ruchów, która będzie przechowywana jako lista liczb całkowitych
            [DataMember] public List<int> HistoriaRuchow;

            //Definiujemy datę rozpoczęcia gry
            [DataMember] public DateTime StartGry;

            //Definiujemy czas zawieszenia gry, który będzie przechowywany jako TimeSpan
            [DataMember] public TimeSpan CzasZawieszenia;

            //Definiujemy status gry, który będzie przechowywany jako typ wyliczeniowy StatusGry
            [DataMember] public StatusGry Status;
        }

        //Teraz przechodzimy do zdefiniowania metod, które będą odpowiadać za zapis, odczyt i usuwanie stanu gry w pliku XML
        public static void Zapisz(StanGry stan)
        {
            //Sprawdzamy, czy stan gry jest nie null, jeśli tak to zapisujemy go do pliku XML
            try
            {
                //Tworzymy obiekt EncryptedGameState, który będzie przechowywał zaszyfrowane dane stanu gry
                var enc = new EncryptedGameState
                {
                    //Szyfrujemy liczbę do odgadnięcia i zapisujemy ją jako string
                    EncryptedLiczbaDoOdgadniecia = Encrypt(stan.LiczbaDoOdgadniecia.ToString()),

                    //Przypisujemy liczbę prób gracza
                    LiczbaProb = stan.LiczbaProb,

                    //Przypisujemy historię ruchów gracza
                    HistoriaRuchow = stan.HistoriaRuchow,

                    //Przypisujemy datę rozpoczęcia gry
                    StartGry = stan.StartGry,

                    //Przypisujemy czas zawieszenia gry
                    CzasZawieszenia = stan.CzasZawieszenia,

                    //Przypisujemy status gry
                    Status = stan.Status
                };

                //Serializujemy obiekt EncryptedGameState do pliku XML
                var serializer = new DataContractSerializer(typeof(EncryptedGameState));

                //Tworzymy strumień do pliku, który nadpisuje plik jeśli istnieje lub tworzy nowy, jeśli nie istnieje
                using (var fs = new FileStream(NazwaPliku, FileMode.Create))

                //Używamy XmlWriter do zapisu obiektu EncryptedGameState do pliku XML
                using (var writer = XmlWriter.Create(fs, new XmlWriterSettings { Indent = true }))

                    //Serializujemy obiekt EncryptedGameState do pliku XML
                    serializer.WriteObject(writer, enc);
            }

            //Jeśli wystąpi błąd podczas zapisu, to wyświetlamy komunikat o błędzie
            catch (Exception ex)
            {
                //Wyświetlamy komunikat o błędzie zapisu
                Console.WriteLine($"Błąd zapisu XML: {ex.Message}");
            }
        }

        //Definiujemy teraz metodę Wczytaj, która będzie odpowiadać za odczytanie stanu gry z pliku XML
        public static StanGry Wczytaj()
        {
            //Sprawdzamy, czy plik istnieje, jeśli tak to odczytujemy stan gry z pliku XML
            try
            {
                //Sprawdzamy, czy plik istnieje, jeśli nie to zwracamy null
                if (!File.Exists(NazwaPliku)) return null;

                //Tworzymy serializer, który będzie odpowiadać za deserializację obiektu EncryptedGameState z pliku XML
                var serializer = new DataContractSerializer(typeof(EncryptedGameState));

                //Otwieramy plik XML do odczytu
                using (var fs = new FileStream(NazwaPliku, FileMode.Open))

                //Używamy XmlReader do odczytu pliku XML
                using (var reader = XmlReader.Create(fs))
                {
                    //Deserializujemy obiekt EncryptedGameState z pliku XML i zwracamy go jako nowy obiekt StanGry
                    var enc = (EncryptedGameState)serializer.ReadObject(reader);

                    //Tworzymy nowy obiekt StanGry i przypisujemy mu wartości z deserializowanego obiektu
                    return new StanGry
                    {
                        //Deszyfrujemy liczbę do odgadnięcia i konwertujemy ją na int
                        LiczbaDoOdgadniecia = int.Parse(Decrypt(enc.EncryptedLiczbaDoOdgadniecia)),

                        //Przypisujemy liczbę prób gracza
                        LiczbaProb = enc.LiczbaProb,

                        //Przypisujemy historię ruchów gracza
                        HistoriaRuchow = enc.HistoriaRuchow,

                        //Przypisujemy datę rozpoczęcia gry
                        StartGry = enc.StartGry,

                        //Przypisujemy czas zawieszenia gry
                        CzasZawieszenia = enc.CzasZawieszenia,

                        //Przypisujemy status gry
                        Status = enc.Status
                    };
                }
            }
            //Jeśli wystąpi błąd podczas odczytu, to wyświetlamy komunikat o błędzie i zwracamy null
            catch (Exception ex)
            {
                //Wyświetlamy komunikat o błędzie odczytu
                Console.WriteLine($"Błąd odczytu XML: {ex.Message}");

                //Zwracamy null, jeśli wystąpił błąd
                return null;
            }
        }

        //Definiujemy teraz metodę Usun, która będzie odpowiadać za usunięcie pliku XML
        public static void Usun()
        {
            //Sprawdzamy, czy plik istnieje, jeśli tak to usuwamy go
            try
            {
                //Sprawdzamy, czy plik istnieje, jeśli tak to usuwamy go
                if (File.Exists(NazwaPliku)) File.Delete(NazwaPliku);
            }

            //Jeśli wystąpi błąd podczas usuwania pliku, to wyświetlamy komunikat o błędzie
            catch (Exception ex)
            {

                //Wyświetlamy komunikat o błędzie usuwania
                Console.WriteLine($"Błąd usuwania XML: {ex.Message}");
            }
        }

        //Definiujemy teraz metodę Istnieje, która będzie sprawdzać, czy plik XML z zapisanym stanem gry istnieje
        public static bool Istnieje() => File.Exists(NazwaPliku);

        //Definiujemy teraz metody szyfrowania i deszyfrowania danych, które będą używane do szyfrowania i deszyfrowania stanu gry
        private static string Encrypt(string plainText)
        {
            //Sprawdzamy, czy klucz i wektor inicjalizacyjny mają odpowiednią długość
            using var aes = Aes.Create();

            //Ustawiamy klucz i wektor inicjalizacyjny dla AES
            aes.Key = Klucz;

            //Ustawiamy wektor inicjalizacyjny (IV) dla AES
            aes.IV = IV;

            //Tworzymy obiekt szyfrujący AES
            var encryptor = aes.CreateEncryptor();

            //Tworzymy strumień pamięci, do którego będziemy zapisywać zaszyfrowane dane
            using var ms = new MemoryStream();

            //Używamy CryptoStream do szyfrowania danych
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))

            //Używamy StreamWriter do zapisu danych do CryptoStream

            //Teraz zapisujemy dane do CryptoStream, które zostaną zaszyfrowane
            using (var sw = new StreamWriter(cs))

                //Zapisujemy dane do CryptoStream
                sw.Write(plainText);

            //Zwracamy zaszyfrowane dane jako string w formacie Base64
            return Convert.ToBase64String(ms.ToArray());
        }

        //Definiujemy teraz metodę deszyfrowania, która będzie odpowiadać za deszyfrowanie zaszyfrowanych danych
        private static string Decrypt(string cipherText)
        {

            //Sprawdzamy, czy klucz i wektor inicjalizacyjny mają odpowiednią długość
            using var aes = Aes.Create();

            //Ustawiamy klucz i wektor inicjalizacyjny dla AES
            aes.Key = Klucz;

            //Ustawiamy wektor inicjalizacyjny (IV) dla AES
            aes.IV = IV;

            //Tworzymy obiekt deszyfrujący AES
            var decryptor = aes.CreateDecryptor();

            //Tworzymy strumień pamięci, do którego będziemy odczytywać zaszyfrowane dane
            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));

            //Używamy CryptoStream do deszyfrowania danych
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);

            //Używamy StreamReader do odczytu danych z CryptoStream
            using var sr = new StreamReader(cs);

            //Odczytujemy dane z CryptoStream i zwracamy je jako string
            return sr.ReadToEnd();
        }
    }
}
