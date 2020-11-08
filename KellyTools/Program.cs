using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace KellyTools
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                using var httpClient = new HttpClient();
                if (File.Exists(VulkanCodeGenerator.VkXml))
                {
                    Console.WriteLine(VulkanCodeGenerator.VkXml + " already downloaded.");
                }
                else
                {
                    await VulkanCodeGenerator.DownloadAsync(httpClient);
                }

                // VulkanCodeGenerator.GenerateCode();
                var specification = VulkanSpecification.Create(VulkanCodeGenerator.VkXml);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
