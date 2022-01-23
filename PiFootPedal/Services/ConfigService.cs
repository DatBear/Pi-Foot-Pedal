using Newtonsoft.Json;
using PiFootPedal.Data;
using PiFootPedal.Enums;

namespace PiFootPedal.Services;

public class ConfigService
{
    private string _path;
    private PollConfig? _config;

    public ConfigService(string path)
    {
        _path = path;
    }

    public PollConfig? GetConfig(bool forceReload = false)
    {
        if (!forceReload) return _config;

        if (File.Exists(_path))
        {
            var configString = File.ReadAllText(_path);
            _config = JsonConvert.DeserializeObject<PollConfig>(configString);
            return _config;
        }

        _config = null;
        return _config;

    }

    public PollConfig? Save(PollConfig? config = null)
    {
        _config = config ?? _config;
        var configString = JsonConvert.SerializeObject(_config);
        if (!Directory.Exists(Path.GetDirectoryName(_path)))
        {
            var dir = Path.GetDirectoryName(_path);
            if (dir == null) return null;
            Directory.CreateDirectory(dir);
        }

        File.WriteAllText(_path, configString);
        return _config;
    }

    public void Setup()
    {
        _config = new PollConfig
        {
            IsVerbose = true,
            ButtonPins = new HashSet<int> { 5 },
            ButtonActions = new Dictionary<int, ButtonAction>
            {
                { 5,
                    new ButtonAction
                    {
                        Type = ButtonActionType.Hold,
                        Keys = new List<KeyDescription>
                        {
                            new(){ Key = Keys.C, Modifier = ModifierKeys.LeftControl }
                        }
                    }
                }
            }
        };
    }
}