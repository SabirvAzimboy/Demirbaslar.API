using Demirbaslar.API.Data;
using Demirbaslar.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demirbaslar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AssetsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Assets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Assets>>> GetAssets()
        {
            return await _context.Assets
                .Include(a => a.CurrentHolder)
                .ToListAsync();
        }

        // GET: api/Assets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Assets>> GetAsset(int id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset == null)
            {
                return NotFound();
            }
            return asset;
        }

        // GET: api/Assets/by-location/Ambarda
        [HttpGet("by-location/{location}")]
        public async Task<ActionResult<IEnumerable<Assets>>> GetAssetsByLocation(byte location)
        {
            return await _context.Assets
                .Where(a => a.Location == location)
                .ToListAsync();
        }

        // GET: api/Assets/by-status/Saglam
        [HttpGet("by-status/{status}")]
        public async Task<ActionResult<IEnumerable<Assets>>> GetAssetsByStatus(bool perfect)
        {
            return await _context.Assets
                .Where(a => a.Perfect == perfect)
                .ToListAsync();
        }

        // POST: api/Assets
        [HttpPost]
        public async Task<ActionResult<Assets>> PostAsset(AssetCreateDto assetDto)
        {
            var asset = new Assets
            {
                InventoryNumber = assetDto.InventoryNumber,
                Nomenclature = assetDto.Nomenclature,
                Characteristics = assetDto.Characteristics,
                SerialNumber = assetDto.SerialNumber,                
                CommissioningDate = assetDto.CommissioningDate ?? DateTime.Now
            };            
            
            // Если указан табельный номер            
            if (assetDto.CurrentHolderId>0)
            {
                var employee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.Id == assetDto.CurrentHolderId);

                if (employee == null)
                {
                    return BadRequest("Personel bulunmadi (Табельный номер не найден)");
                }

                asset.CurrentHolderId = employee.Id;
                asset.Location = 1; // Zimmetde

                // Если дата ввода в эксплуатацию не указана в DTO - устанавливаем текущую дату
                if (asset.CommissioningDate == null)
                {
                    asset.CommissioningDate = DateTime.Now;
                }                
            }
            
            if (await _context.Assets.AnyAsync(a => a.InventoryNumber == asset.InventoryNumber))
            {
                return BadRequest("Bu inventar numarasi onceden mevcuddur");
            }

            _context.Assets.Add(asset);
            await _context.SaveChangesAsync();

            if (string.IsNullOrEmpty(asset.InventoryNumber))
            {
                asset.InventoryNumber = $"INV-{asset.Id}";
                await _context.SaveChangesAsync(); // Сохраняем сгенерированный номер
            }

            return CreatedAtAction(
                actionName: nameof(GetAsset), // Убедитесь, что это имя вашего GET-метода
                routeValues: new { id = asset.Id },
                value: asset);
        }

        // PUT: api/Assets/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsset(int id, AssetUpdateDto assetDto)
        {
            // 1. Находим существующий актив
            var existingAsset = await _context.Assets.FindAsync(id);
            if (existingAsset == null)
            {
                return NotFound();
            }

            // 2. Проверяем уникальность инвентарного номера (если он изменился)
            if (assetDto.InventoryNumber != null &&
                assetDto.InventoryNumber != existingAsset.InventoryNumber)
            {
                if (await _context.Assets.AnyAsync(a => a.InventoryNumber == assetDto.InventoryNumber))
                {
                    return BadRequest("Инвентарный номер уже существует");
                }
                existingAsset.InventoryNumber = assetDto.InventoryNumber;
            }

            // 3. Обновляем только разрешенные поля
            existingAsset.Nomenclature = assetDto.Nomenclature ?? existingAsset.Nomenclature;
            existingAsset.Characteristics = assetDto.Characteristics ?? existingAsset.Characteristics;
            existingAsset.SerialNumber = assetDto.SerialNumber ?? existingAsset.SerialNumber;
            existingAsset.Location = assetDto.Location != existingAsset.Location ? assetDto.Location : existingAsset.Location;
            existingAsset.Perfect = assetDto.Perfect != existingAsset.Perfect ? assetDto.Perfect : existingAsset.Perfect;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssetExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Assets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsset(int id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset == null) return NotFound();
            _context.Assets.Remove(asset);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DTO для создания ОС
        public class AssetCreateDto
        {
            public string? InventoryNumber { get; set; }
            public string Nomenclature { get; set; } = string.Empty;
            public string Characteristics { get; set; } = string.Empty;
            public string? SerialNumber { get; set; }
            public int CurrentHolderId { get; set; }
            public DateTime? CommissioningDate { get; set; }
        }

        // DTO для обновления
        public class AssetUpdateDto
        {
            public string? InventoryNumber { get; set; }
            public string? Nomenclature { get; set; }
            public string? Characteristics { get; set; }
            public string? SerialNumber { get; set; }
            public int CurrentHolderId { get; set; }
            public DateTime? CommissioningDate { get; set; }
            public byte Location { get; set; }
            public bool Perfect { get; set; }
        }

        private bool AssetExists(int id)
        {
            return _context.Assets.Any(e => e.Id == id); // Исправлено на проверку Assets
        }
    }
}
