using System;
using System.Collections.Generic;
using System.IO;

namespace KaraokedoMaclim
{
    public class Musica
    {
        public string Numero { get; set; }
        public string Titulo { get; set; }
        public string Cantor { get; set; }

        // Método para carregar músicas de um arquivo .txt
        public static List<Musica> CarregarMusicas(string caminhoArquivo)
        {
            var musicas = new List<Musica>();

            if (!File.Exists(caminhoArquivo))
                return musicas; // Retorna lista vazia se o arquivo não existir

            var linhas = File.ReadAllLines(caminhoArquivo);

            foreach (var linha in linhas)
            {
                var partes = linha.Split(" - "); // Divide a linha pelo delimitador " - "
                if (partes.Length == 3)
                {
                    musicas.Add(new Musica
                    {
                        Numero = partes[0].Trim(),
                        Titulo = partes[1].Trim(),
                        Cantor = partes[2].Trim()
                    });
                }
            }

            return musicas;
        }
    }
}
