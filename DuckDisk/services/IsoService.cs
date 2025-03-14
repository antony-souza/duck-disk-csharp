using DuckDisk.interfaces;
using DuckDisk.models;
using DuckDisk.utils;
using Figgle;

namespace DuckDisk.services
{
    public class IsoService : IIso
    {
        public Task<IsoModel> GetIsoDetails(string isoPath)
        {
            FileInfo isoInfo = new FileInfo(isoPath);

            IsoModel isoModel = new IsoModel
            {
                Path = isoInfo.FullName,
                Name = isoInfo.Name,
                Size = isoInfo.Length,
            };

            var formatSize = FormatBytes.Format(isoInfo.Length);
            Console.WriteLine("\nDetalhes da ISO:");
            Console.WriteLine();
            Console.WriteLine($"Nome: {isoModel.Name}");
            Console.WriteLine($"Tamanho: {formatSize}");
            Console.WriteLine($"Data de Criação: {isoInfo.CreationTime}");
            Console.WriteLine($"Data da Última Modificação: {isoInfo.LastWriteTime}");
            Console.WriteLine($"Caminho Completo: {isoModel.Path}");

            return Task.FromResult(isoModel);
        }

        public Task<bool> ValidateIsoAsync(string isoPath)
        {
            throw new NotImplementedException();
        }
        
        public void IsoMenu()
        {
            Console.Write("Digite o caminho completo do arquivo ISO: ");
            string isoPath = Console.ReadLine();
            
            if (!File.Exists(isoPath))
            {
                Console.WriteLine("Arquivo ISO não encontrado!");
                return;
            }

            IsoService isoService = new IsoService();

            while (true)
            {
                Console.Clear();
                DuckDiskTitle.DisplayTitle();
                Console.WriteLine("----- Menu de Arquivos ISO -----");
                Console.WriteLine();
                Console.WriteLine("1. Exibir detalhes da ISO");
                Console.WriteLine("2. Validar ISO (não implementado)");
                Console.WriteLine("3. Sair");
                Console.WriteLine();
                Console.Write("Escolha uma opção: ");
                var opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        isoService.GetIsoDetails(isoPath).Wait();
                        break;

                    case "2":
                        Console.WriteLine("\nValidação de ISO não implementada.");
                        break;

                    case "3":
                        Console.WriteLine("\nSaindo...");
                        return;

                    default:
                        Console.WriteLine("\nOpção inválida. Tente novamente.");
                        break;
                }

                Console.WriteLine("\nPressione qualquer tecla para voltar ao Menu de Arquivos ISO...");
                Console.ReadKey();
            }
        }
    }
}
