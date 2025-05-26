using System;
using System.Threading;
using GraZaDuzoZaMalo.Model;
using GraZaDuzoZaMalo.Serializacja;

//Piotr Bacior 15 722 - WSEI Kraków

namespace GraZaDuzoZaMalo.Backup
{
    //Definiuje klasę AutoBackup, która automatycznie wykonuje backup stanu gry co określony interwał czasu
    internal class AutoBackup
    {
        //Definiujemy Timer który odpowiadać będzie odpowiadać za cykliczne wykonywanie backupu
        private Timer _timer;

        //Definiujemy teraz funkcję zwracającą aktualny stan naszej gry
        private Func<StanGry> _pobierzStan;

        //Teraz przechodzimy do zdefiniowania konstruktora klasy AutoBackup, która przyjmuje funkcję pobierającą nasz stan gry oraz interwał w sekundach, co ile ma być wykonywany backup
        public AutoBackup(Func<StanGry> pobierzStan, int interwalSekundy = 10)
        {
            //Pobieramy stan gry poprzez przekazaną funkcję i ustawiamy timer, który będzie wywoływał metodę Backupuj co określony interwał czasu
            _pobierzStan = pobierzStan;

            //Inicjalizujemy timer, który będzie wywoływał metodę Backupuj co określony interwał czasu w sekundach
            _timer = new Timer(BackupujPB, null, 0, interwalSekundy * 1000);
        }

        //Definiujemy metodę BackupujPB, która będzie wykonywać backup stanu gry
        private void BackupujPB(object state)
        {
            //Pobieramy aktualny stan gry poprzez funkcję przekazaną do konstruktora
            var stan = _pobierzStan();

            //Sprawdzamy, czy stan gry jest nie null i czy gra trwa, jeśli tak to zapisujemy stan gry do pliku
            if (stan != null && stan.Status == StatusGry.Trwa)
            {
                //Zapisujemy stan gry do pliku binarnego przy użyciu klasy SerializacjaBinarna
                SerializacjaBinarna.Zapisz(stan);

                //Informujemy użytkownika, że backup został wykonany
                Console.WriteLine("Backup stanu gry wykonany.");
            }
        }

        //Definiujemy finalnie metodę Zatrzymaj, która odpowiada za automatyczny backup i zatrzymuje timer, gdy gra jest zakończona lub użytkownik chce przerwać automatyczny backup
        public void Zatrzymaj()
        {
            //Sprawdzamy, czy timer jest zainicjalizowany i jeśli tak to go zatrzymujemy
            _timer?.Dispose();
        }
    }
}