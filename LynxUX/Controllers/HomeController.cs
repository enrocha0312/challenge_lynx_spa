using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LynxUX.Models;
using System.Text.Json;
using System.Text;

namespace LynxUX.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly HttpClient _httpClient;
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("http://localhost:5025");
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public async Task<IActionResult> GetAll()
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync("Equipamento");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var equipamentos = JsonSerializer.Deserialize<List<EquipamentoViewModel>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                GerarHistoricoUsuario("Consulta", "Usuário buscou todos os equipamentos.");

                return View("GetAll", equipamentos);
            }
            else
            {
                ViewBag.Erro = "Erro ao buscar equipamentos.";
                return View("GetAll", new List<EquipamentoViewModel>());
            }
        }
        catch (Exception e)
        {
            ViewBag.Erro = $"Erro na comunicação com a API: {e.Message}";
            return View("GetAll", new List<EquipamentoViewModel>());
        }
    }


    public async Task<IActionResult> GetByInstalacaoELote(string instalacao, int lote)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"Equipamento/{instalacao}/{lote}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var equipamento = JsonSerializer.Deserialize<EquipamentoViewModel>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                GerarHistoricoUsuario("Consulta", $"Usuário buscou equipamento com Instalação={instalacao}, Lote={lote}.");

                return View("ExibirEquipamento", equipamento);
            }
            else
            {
                ViewBag.Erro = "Equipamento não encontrado.";
                return View("ExibirEquipamento", null);
            }
        }
        catch (Exception ex)
        {
            ViewBag.Erro = $"Erro na comunicação com a API: {ex.Message}";
            return View("ExibirEquipamento", null);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AlterarEquipamento(EquipamentoViewModel equipamento)
    {
        try
        {
            var json = JsonSerializer.Serialize(equipamento);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync($"Equipamento/{equipamento.Instalacao}/{equipamento.Lote}", content);

            if (response.IsSuccessStatusCode)
            {
                RegistraArquivoLog("Atualizações", $"Equipamento alterado: {JsonSerializer.Serialize(equipamento)}");
                GerarHistoricoUsuario("Alteração", $"Usuário alterou equipamento com Instalação={equipamento.Instalacao}, Lote={equipamento.Lote}.");

                ViewBag.Mensagem = "Equipamento alterado com sucesso!";
            }
            else
            {
                ViewBag.Erro = "Erro ao alterar equipamento.";
            }
        }
        catch (Exception e)
        {
            ViewBag.Erro = $"Erro na comunicação com a API: {e.Message}";
        }

        return View("Index");
    }
    [HttpPost]
    public async Task<IActionResult> DeletarEquipamento(string instalacao, int lote)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"Equipamento/{instalacao}/{lote}");

            if (response.IsSuccessStatusCode)
            {
                RegistraArquivoLog("Delecoes", $"Equipamento deletado: Instalação={instalacao}, Lote={lote}");
                GerarHistoricoUsuario("Exclusão", $"Usuário deletou equipamento com Instalação={instalacao}, Lote={lote}.");

                ViewBag.Mensagem = "Equipamento deletado com sucesso!";
            }
            else
            {
                ViewBag.Erro = "Erro ao deletar equipamento.";
            }
        }
        catch (Exception e)
        {
            ViewBag.Erro = $"Erro na comunicação com a API: {e.Message}";
        }

        return View("Index");
    }


    [HttpPost]
    public async Task<IActionResult> CadastrarEquipamento(EquipamentoViewModel equipamento)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Erro = "Preencha todos os campos corretamente.";
            return View("Index");
        }

        try
        {
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(equipamento),
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response = await _httpClient.PostAsync("Equipamento", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                RegistraArquivoLog("Primeiros cadastros", $"Equipamento cadastrado: {JsonSerializer.Serialize(equipamento)}");
                GerarHistoricoUsuario("Cadastro", $"Usuário cadastrou equipamento: {JsonSerializer.Serialize(equipamento)}.");

                ViewBag.Mensagem = "Equipamento cadastrado com sucesso!";
            }
            else
            {
                ViewBag.Erro = "Erro ao cadastrar equipamento.";
            }
        }
        catch (Exception e)
        {
            ViewBag.Erro = $"Erro na comunicação com a API: {e.Message}";
        }

        return View("Index");
    }


    private void RegistraArquivoLog(string tipo, string mensagem)
    {
        try
        {
            string logsPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            if (!Directory.Exists(logsPath))
            {
                Directory.CreateDirectory(logsPath);
            }

            string filePath = Path.Combine(logsPath, $"{tipo}.txt");
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {mensagem}";

            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                sw.WriteLine(logEntry);
            }
        }
        catch (Exception e)
        {
            _logger.LogError($"Erro ao escrever no log: {e.Message}");
        }
    }
    private void GerarHistoricoUsuario(string acao, string mensagem)
    {
        try
        {
            string historicoPath = Path.Combine(Directory.GetCurrentDirectory(), "Historico");
            if (!Directory.Exists(historicoPath))
            {
                Directory.CreateDirectory(historicoPath);
            }

            string fileName = $"LynxApp_log_{DateTime.Now:yyyyMMdd}.txt";
            string filePath = Path.Combine(historicoPath, fileName);
            string logEntry = $"{DateTime.Now:dd/MM/yyyy HH:mm:ss} - [{acao}] - {mensagem}";

            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                sw.WriteLine(logEntry);
            }
        }
        catch (Exception e)
        {
            _logger.LogError($"Erro ao escrever no histórico do usuário: {e.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> ImportarCsv(IFormFile arquivoCsv)
    {
        if (arquivoCsv == null || arquivoCsv.Length == 0)
        {
            ViewBag.Erro = "Nenhum arquivo foi enviado.";
            return View("Index");
        }

        List<EquipamentoViewModel> equipamentos = new List<EquipamentoViewModel>();

        try
        {
            using (var reader = new StreamReader(arquivoCsv.OpenReadStream()))
            {
                string? linha;
                bool primeiraLinha = true;

                while ((linha = reader.ReadLine()) != null)
                {
                    if (primeiraLinha)
                    {
                        primeiraLinha = false; 
                        continue;
                    }

                    string[] colunas = linha.Split(';');
                    if (colunas.Length != 6)
                    {
                        ViewBag.Erro = "Formato do CSV inválido.";
                        return View("Index");
                    }

                    var equipamento = new EquipamentoViewModel
                    {
                        Instalacao = colunas[0].Trim(),
                        Lote = int.Parse(colunas[1].Trim()),
                        Operadora = Enum.Parse<OperadoraEnum>(colunas[2].Trim(), true),
                        Fabricante = colunas[3].Trim(),
                        Modelo = int.Parse(colunas[4].Trim()),
                        Versao = int.Parse(colunas[5].Trim())
                    };

                    equipamentos.Add(equipamento);
                }
            }

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(equipamentos),
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response = await _httpClient.PostAsync("Equipamento/upload-csv", jsonContent); 

            if (response.IsSuccessStatusCode)
            {
                RegistraArquivoLog("Importacoes", $"Importação concluída. {equipamentos.Count} registros adicionados.");
                GerarHistoricoUsuario("IMPORTACAO", $"Usuário importou {equipamentos.Count} registros via CSV.");
                ViewBag.Mensagem = "Importação realizada com sucesso!";
            }
            else
            {
                ViewBag.Erro = "Erro ao importar os equipamentos.";
            }
        }
        catch (Exception ex)
        {
            ViewBag.Erro = $"Erro no processamento do CSV: {ex.Message}";
        }

        return View("Index");
    }

}
