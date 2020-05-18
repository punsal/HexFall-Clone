using Utility.UI.Information_UI.Data;

namespace Utility.Extension
{
    public static class InformationUIDataExtension
    {
        public static InformationUIData SetDuration(this InformationUIData data, float duration)
        {
            data.Duration = duration;
            return data;
        }
    }
}