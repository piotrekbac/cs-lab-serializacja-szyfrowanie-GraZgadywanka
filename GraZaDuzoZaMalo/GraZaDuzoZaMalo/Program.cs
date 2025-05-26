using System;
using GraZaDuzoZaMalo.Model;
using GraZaDuzoZaMalo.Serializacja;
using GraZaDuzoZaMalo.Backup;

//Piotr Bacior 15 722 - WSEI Kraków

namespace GraZaDuzoZaMalo
{
    class Program
    {
        //Definiujemy globalną zmienna przechowująca stan gry
        static StanGry stan;

        //Definiujemy zmienną decydująca o trybie serializacji: true = XML + szyfrowanie, false = binarna - Piotr Bacior 15 722 - WSEI Kraków
        static bool uzyjSerializacjiXML = false;

        static void Main()
        {
            //Ustawiamy kodowanie konsoli na UTF-8, aby obsługiwać polskie znaki
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            //Wczytaj stan gry (jeśli istnieje) z odpowiedniego źródła
            if (uzyjSerializacjiXML ? SerializacjaXML.Istnieje() : SerializacjaBinarna.Istnieje())
            {
                //Jeśli istnieje zapisany stan gry, to pytamy użytkownika, czy chce go wczytać
                Console.WriteLine("Wykryto zapisany stan gry. Czy chcesz kontynuować? (T/N): ");

                //Wczytujemy decyzję użytkownika 
                string decyzja = Console.ReadLine()?.ToUpper();

                //Jeśli użytkownik zdecyduje się kontynuować, to wczytujemy stan gry
                if (decyzja == "T")
                {
                    //Wczytujemy stan gry z odpowiedniego źródła (XML lub binarny)
                    stan = uzyjSerializacjiXML ? SerializacjaXML.Wczytaj() : SerializacjaBinarna.Wczytaj();

                    //Jeśli stan gry został wczytany, to informujemy użytkownika o tym i przywracamy status gry
                    if (stan != null)
                    {
                        //Informujemy użytkownika o przywróceniu stanu gry
                        Console.WriteLine("Stan gry został przywrócony.");

                        //Przywracamy status gry i czas rozpoczęcia
                        stan.Status = StatusGry.Trwa;

                        //Jeśli gra była zawieszona, to przywracamy czas zawieszenia
                        stan.StartGry = DateTime.Now; 
                    }

                    //Jeśli stan gry nie został wczytany (np. plik jest uszkodzony lub niekompletny), to informujemy użytkownika i rozpoczynamy nową grę
                    else
                    {
                        //Informujemy użytkownika, że nie udało się przywrócić stanu gry
                        Console.WriteLine("Nie udało się przywrócić stanu. Rozpoczynanie nowej gry.");
                    }

                    //Usuwamy zapisany stan gry, aby uniknąć problemów przy kolejnych uruchomieniach
                    if (uzyjSerializacjiXML) SerializacjaXML.Usun();

                    //Jeśli nie używamy serializacji XML, to usuwamy plik binarny
                    else SerializacjaBinarna.Usun();
                }

                //Jeśli użytkownik zdecyduje się nie kontynuować, to usuwamy zapisany stan gry
                else
                {
                    //Informujemy użytkownika, że zapisany stan gry zostanie usunięty
                    if (uzyjSerializacjiXML) SerializacjaXML.Usun();

                    //Jeśli nie używamy serializacji XML, to usuwamy plik binarny
                    else SerializacjaBinarna.Usun();
                }
            }

            //Jeżeli stan nowej gry nie wczytał stanu 
            if (stan == null)
            {
                //Inicjalizujemy nowy stan gry
                stan = new StanGry
                {
                    //Losujemy liczbę do odgadnięcia z zakresu 1-100
                    LiczbaDoOdgadniecia = new Random().Next(1, 100)
                };

                //Wypisujemy informację o rozpoczęciu nowej gry
                Console.WriteLine("Rozpoczęto nową grę!");
            }

            //Definiujemy automatyczny backup, który będzie wykonywany co 10 sekund
            var backup = new AutoBackup(() => stan);

            //Teraz przechodzimy do głównej pętli gry, gdzie użytkownik będzie mógł zgadywać liczbę
            while (true)
            {

                //Wypisujemy komunikat o zgadywaniu liczby dla użytkownika
                Console.Write("Zgadnij liczbę (lub X, aby zakończyć i zapisać grę): ");

                //Wczytujemy propozycję użytkownika
                string input = Console.ReadLine();

                //Sprawdzamy, czy użytkownik chce zakończyć grę i zapisać stan
                if (input.ToUpper() == "X")
                {
                    //Jeśli użytkownik chce zakończyć grę, to zapisujemy stan gry i ustawiamy status na Zawieszona
                    stan.Status = StatusGry.Zawieszona;

                    //Ustawiamy czas zawieszenia na aktualny czas
                    stan.CzasZawieszenia += DateTime.Now - stan.StartGry;

                    //Jeżeli gracz chce zapisać stan gry, to zapisujemy go do odpowiedniego pliku
                    if (uzyjSerializacjiXML) SerializacjaXML.Zapisz(stan);

                    //Jeśli nie używamy serializacji XML, to zapisujemy stan gry do pliku binarnego
                    else SerializacjaBinarna.Zapisz(stan);

                    //Informujemy użytkownika, że gra została zapisana i kończymy pętlę gry
                    Console.WriteLine("Gra została zapisana. Do zobaczenia!");

                    //Zatrzymujemy automatyczny backup
                    break;
                }

                //Sprawdzamy, czy użytkownik wprowadził poprawną liczbę
                if (int.TryParse(input, out int propozycja))
                {
                    //Dodajemy propozycję do historii ruchów
                    stan.HistoriaRuchow.Add(propozycja);

                    //Zwiększamy liczbę prób
                    stan.LiczbaProb++;

                    //Sprawdzamy, czy propozycja jest równa, mniejsza lub większa od liczby do odgadnięcia
                    if (propozycja == stan.LiczbaDoOdgadniecia)
                    {

                        //Jeśli propozycja jest równa liczbie do odgadnięcia, to informujemy użytkownika o wygranej
                        Console.WriteLine("Brawo! Odgadłeś!");

                        //Ustawamy status gry na Zakończona
                        stan.Status = StatusGry.Zakonczona;

                        //Kończymy pętlę gry
                        break;
                    }

                    //Jeśli propozycja jest mniejsza lub większa od liczby do odgadnięcia, to informujemy użytkownika - wzależności od tego, czy jest za mało czy za dużo
                    else if (propozycja < stan.LiczbaDoOdgadniecia)
                    {
                        //Jeśli propozycja jest mniejsza od liczby do odgadnięcia, to informujemy użytkownika, że jest za mało
                        Console.WriteLine("Za mało!");
                    }

                    //Jeśli propozycja jest większa od liczby do odgadnięcia, to informujemy użytkownika, że jest za dużo
                    else
                    {
                        //Jeśli propozycja jest większa od liczby do odgadnięcia, to informujemy użytkownika, że jest za dużo
                        Console.WriteLine("Za dużo!");
                    }
                }

                //Jeśli użytkownik wprowadził niepoprawne dane (np. tekst zamiast liczby), to informujemy go o tym
                else
                {
                    //Informujemy użytkownika, że wprowadził niepoprawne dane i prosi o ponowne wprowadzenie liczby
                    Console.WriteLine("Nieprawidłowe dane. Spróbuj ponownie.");
                }
            }

            //Wypisujemy podsumowanie gry
            Console.WriteLine($"\nLiczba prób: {stan.LiczbaProb}");
            Console.WriteLine("Twoje propozycje: " + string.Join(", ", stan.HistoriaRuchow));

            //Jeśli gra została zawieszona, to informujemy o czasie zawieszenia
            var czasTrwania = DateTime.Now - stan.StartGry - stan.CzasZawieszenia;
            Console.WriteLine($"Czas gry (bez zawieszeń): {czasTrwania:mm\\:ss}");

            //Ustawiamy status gry na Zakończona, jeśli gra została zakończona i zatrzymujemy automatyczny backup
            backup.Zatrzymaj();
        }
    }
}
