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
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync($"Equipamento/{equipamento.Instalacao}/{equipamento.Lote}", content);

            if (response.IsSuccessStatusCode)
            {
                ViewBag.Mensagem = "Equipamento alterado com sucesso!";
            }
            else
            {
                ViewBag.Erro = "Erro ao alterar equipamento.";
            }
        }
        catch (Exception ex)
        {
            ViewBag.Erro = $"Erro na comunicação com a API: {ex.Message}";
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
                ViewBag.Mensagem = "Equipamento deletado com sucesso!";
            }
            else
            {
                ViewBag.Erro = "Erro ao deletar equipamento.";
            }
        }
        catch (Exception ex)
        {
            ViewBag.Erro = $"Erro na comunicação com a API: {ex.Message}";
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
                ViewBag.Mensagem = "Equipamento cadastrado com sucesso!";
            }
            else
            {
                ViewBag.Erro = "Erro ao cadastrar equipamento.";
            }
        }
        catch (Exception ex)
        {
            ViewBag.Erro = $"Erro na comunicação com a API: {ex.Message}";
        }

        return View("Index");
    }
}
