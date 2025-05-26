using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using GraZaDuzoZaMalo.Model;

namespace GraZaDuzoZaMalo.Serializacja
{
    public static class SerializacjaXML
    {
        private const string NazwaPliku = "stanGry.xml";
        private static readonly byte[] Klucz = Encoding.UTF8.GetBytes("SuperTajneHaslo1");
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("1234567812345678");

        [DataContract]
        private class EncryptedGameState
        {
            [DataMember] public string EncryptedLiczbaDoOdgadniecia;
            [DataMember] public int LiczbaProb;
            [DataMember] public List<int> HistoriaRuchow;
            [DataMember] public DateTime StartGry;
            [DataMember] public TimeSpan CzasZawieszenia;
            [DataMember] public StatusGry Status;
        }

        public static void Zapisz(StanGry stan)
        {
            try
            {
                var enc = new EncryptedGameState
                {
                    EncryptedLiczbaDoOdgadniecia = Encrypt(stan.LiczbaDoOdgadniecia.ToString()),
                    LiczbaProb = stan.LiczbaProb,
                    HistoriaRuchow = stan.HistoriaRuchow,
                    StartGry = stan.StartGry,
                    CzasZawieszenia = stan.CzasZawieszenia,
                    Status = stan.Status
                };

                var serializer = new DataContractSerializer(typeof(EncryptedGameState));
                using (var fs = new FileStream(NazwaPliku, FileMode.Create))
                using (var writer = XmlWriter.Create(fs, new XmlWriterSettings { Indent = true }))
                    serializer.WriteObject(writer, enc);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Błąd zapisu XML: {ex.Message}");
            }
        }

        public static StanGry Wczytaj()
        {
            try
            {
                if (!File.Exists(NazwaPliku)) return null;

                var serializer = new DataContractSerializer(typeof(EncryptedGameState));
                using (var fs = new FileStream(NazwaPliku, FileMode.Open))
                using (var reader = XmlReader.Create(fs))
                {
                    var enc = (EncryptedGameState)serializer.ReadObject(reader);
                    return new StanGry
                    {
                        LiczbaDoOdgadniecia = int.Parse(Decrypt(enc.EncryptedLiczbaDoOdgadniecia)),
                        LiczbaProb = enc.LiczbaProb,
                        HistoriaRuchow = enc.HistoriaRuchow,
                        StartGry = enc.StartGry,
                        CzasZawieszenia = enc.CzasZawieszenia,
                        Status = enc.Status
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd odczytu XML: {ex.Message}");
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
                Console.WriteLine($"Błąd usuwania XML: {ex.Message}");
            }
        }

        public static bool Istnieje() => File.Exists(NazwaPliku);

        private static string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = Klucz;
            aes.IV = IV;
            var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
                sw.Write(plainText);
            return Convert.ToBase64String(ms.ToArray());
        }

        private static string Decrypt(string cipherText)
        {
            using var aes = Aes.Create();
            aes.Key = Klucz;
            aes.IV = IV;
            var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }
}
