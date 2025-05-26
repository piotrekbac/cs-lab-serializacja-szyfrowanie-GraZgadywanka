using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using GraZaDuzoZaMalo.Model;

//Piotr Bacior 15 722 - WSEI Kraków

#pragma warning disable SYSLIB0011 // Wyłącza ostrzeżenie o przestarzałym BinaryFormatter

namespace GraZaDuzoZaMalo.Serializacja
{
    public static class SerializacjaBinarna
    {
        private const string NazwaPliku = "stanGryBP.bin";

        public static void Zapisz(StanGry stan)
        {
            try
            {
                using (FileStream fs = new FileStream(NazwaPliku, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, stan);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd zapisu: {ex.Message}");
            }
        }

        public static StanGry Wczytaj()
        {
            try
            {
                if (!File.Exists(NazwaPliku)) return null;

                using (FileStream fs = new FileStream(NazwaPliku, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    return (StanGry)formatter.Deserialize(fs);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd odczytu: {ex.Message}");
                return null;
            }
        }

        public static void Usun()
        {
            try
            {
                if (File.Exists(NazwaPliku)) File.Delete(NazwaPliku);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd usuwania pliku: {ex.Message}");
            }
        }

        public static bool Istnieje() => File.Exists(NazwaPliku);
    }
}

