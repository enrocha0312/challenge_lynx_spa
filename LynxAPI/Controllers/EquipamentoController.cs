using LynxAPI.Context;
using LynxAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LynxAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EquipamentoController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EquipamentoController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Equipamento>>> GetAll()
        {
            var equipamentos = await _context.Equipamentos.AsNoTracking().ToListAsync();
            return Ok(equipamentos);
        }
        [HttpGet("{instalacao}/{lote}")]
        public async Task<ActionResult<Equipamento>> GetByInstalacaoAndLote(string instalacao, int lote)
        {
            var equipamento = await _context.Equipamentos
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Instalacao == instalacao && e.Lote == lote);

            if (equipamento == null)
            {
                return NotFound("Equipamento não encontrado.");
            }

            return Ok(equipamento);
        }
        [HttpPost]
        public async Task<ActionResult<Equipamento>> Create([FromBody] Equipamento equipamento)
        {
            if (_context.Equipamentos.Any(e => e.Instalacao == equipamento.Instalacao && e.Lote == equipamento.Lote))
            {
                return Conflict("Já existe um equipamento com essa Instalação e Lote.");
            }

            _context.Equipamentos.Add(equipamento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByInstalacaoAndLote),
                new { instalacao = equipamento.Instalacao, lote = equipamento.Lote },
                equipamento);
        }
        [HttpPut("{instalacao}/{lote}")]
        public async Task<IActionResult> Update(string instalacao, int lote, [FromBody] Equipamento equipamentoAtualizado)
        {
            if (instalacao != equipamentoAtualizado.Instalacao || lote != equipamentoAtualizado.Lote)
            {
                return BadRequest("Os dados da URL não correspondem ao equipamento informado.");
            }

            var equipamentoExistente = await _context.Equipamentos
                .FirstOrDefaultAsync(e => e.Instalacao == instalacao && e.Lote == lote);

            if (equipamentoExistente == null)
            {
                return NotFound("Equipamento não encontrado.");
            }

            // Atualizando os dados
            equipamentoExistente.Fabricante = equipamentoAtualizado.Fabricante;
            equipamentoExistente.Operadora = equipamentoAtualizado.Operadora;
            equipamentoExistente.Modelo = equipamentoAtualizado.Modelo;
            equipamentoExistente.Versao = equipamentoAtualizado.Versao;

            await _context.SaveChangesAsync();

            return NoContent(); 
        }
        [HttpDelete("{instalacao}/{lote}")]
        public async Task<IActionResult> Delete(string instalacao, int lote)
        {
            var equipamento = await _context.Equipamentos
                .FirstOrDefaultAsync(e => e.Instalacao == instalacao && e.Lote == lote);

            if (equipamento == null)
            {
                return NotFound("Equipamento não encontrado.");
            }

            _context.Equipamentos.Remove(equipamento);
            await _context.SaveChangesAsync();

            return NoContent(); 
        }
    }
}
