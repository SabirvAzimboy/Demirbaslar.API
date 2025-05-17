using Demirbaslar.API.Data;
using Demirbaslar.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demirbaslar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepairsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RepairsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Repairs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Repair>>> GetRepairs()
        {
            return await _context.Repairs
                .Include(r => r.Asset)
                .ThenInclude(a => a.CurrentHolder)
                .ToListAsync();
        }

        // GET: api/Repairs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Repair>> GetRepair(int id)
        {
            var repair = await _context.Repairs
                .Include(r => r.Asset)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (repair == null)
            {
                return NotFound();
            }

            return repair;
        }

        // GET: api/Repairs/by-status/Təmirtdə
        [HttpGet("by-status/{status}")]
        public async Task<ActionResult<IEnumerable<Repair>>> GetRepairsByStatus(bool repaired)
        {
            return await _context.Repairs
                .Where(r => r.Repaired == repaired)
                .Include(r => r.Asset)
                .ToListAsync();
        }

        // POST: api/Repairs
        [HttpPost]
        public async Task<ActionResult<Repair>> PostRepair(RepairCreateDto repairDto)
        {

            if (repairDto.RepairPersonId < 1)
            {
                return BadRequest("RepairPersonId gostermeniz gerekiyor!");
            }
            
            var repair = new Repair
            {
                SendDate = repairDto.SendDate,
                RepairPersonId = repairDto.RepairPersonId
            };

            if (repairDto.AssetId<1)
            {
                return BadRequest("AssetId gostermeniz gerekiyor!");
            }
            else
            {
                var asset = await _context.Assets
                    .FirstOrDefaultAsync(e => e.Id == repairDto.AssetId);

                if (asset == null)
                {
                    return BadRequest("Bu Id de Demirbas bulunmadi :(");
                }
                else if (asset.Location==3)
                {
                    return BadRequest("Bu Demirbas zaten Tamirda gozukmekte :(");
                }
                else
                {
                    asset.Location = 3;
                    asset.Perfect = false;
                }

                repair.AssetId=asset.Id;
            }

            _context.Repairs.Add(repair);            
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRepair", new { id = repair.Id }, repair);
        }

        // PUT: api/Repairs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRepair(int id, Repair repair)
        {
            if (id != repair.Id)
            {
                return BadRequest();
            }

            _context.Entry(repair).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RepairExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // PUT: api/Repairs/5/complete
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompleteRepair(int id, [FromBody] RepairCompletionDto dto)
        {
            var repair = await _context.Repairs
                .Include(r => r.Asset)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (repair == null)
            {
                return NotFound();
            }

            // Обновляем данные ремонта
            repair.Repaired = dto.Repaired;
            repair.ReturnDate = DateTime.Now;
            repair.RepairCost = dto.RepairCost;
            repair.Notes = dto.Notes;

            // Обновляем местоположение и статус ОС
            var asset = repair.Asset;
            asset.Location = 0;
            if (dto.Repaired)
            {
                asset.Perfect = true;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Repairs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRepair(int id)
        {
            var repair = await _context.Repairs.FindAsync(id);
            if (repair == null)
            {
                return NotFound();
            }

            _context.Repairs.Remove(repair);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RepairExists(int id)
        {
            return _context.Repairs.Any(e => e.Id == id);
        }
    }

    // DTO для отправления на ремонт
    public class RepairCreateDto
    {
        public int AssetId { get; set; }
        public int RepairPersonId { get; set; }
        public DateTime SendDate { get; set; } = DateTime.Now;
    }

    // DTO для завершения ремонта
    public class RepairCompletionDto
    {
        public bool Repaired { get; set; }
        public decimal? RepairCost { get; set; }        
        public string? Notes { get; set; }
    }
}