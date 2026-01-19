using Legno.Application.Dtos.Setting;

namespace Legno.Application.Abstracts.Services;

public interface ISettingService
{
    Task<SettingDto> AddSettingAsync(CreateSettingDto createSettingDto);
    Task<SettingDto?> GetSettingAsync(string settingId);
    Task<List<SettingDto>> GetAllSettingsAsync();
    Task<SettingDto> UpdateSettingAsync(UpdateSettingDto updateSettingDto);
   
    Task DeleteSettingAsync(string settingId);
    Task<SettingDto?> GetSettingForSettingKeyAsync(string settingKey);
}