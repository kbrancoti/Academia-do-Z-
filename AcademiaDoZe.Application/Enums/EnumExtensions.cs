// Kaio Fernandes Branco
using System.ComponentModel.DataAnnotations;
using System.Reflection;
namespace AcademiaDoZe.Application.Enums;
public static class EnumExtensions { public static string GetDisplayName(this Enum value) { var type = value.GetType(); var field = type.GetField(value.ToString()); var display = field?.GetCustomAttribute<DisplayAttribute>()?.Name; if (display is not null) return display; if (type.GetCustomAttribute<FlagsAttribute>() is not null) { var names = Enum.GetValues(type).Cast<Enum>().Where(flag => Convert.ToInt64(flag) != 0 && value.HasFlag(flag)).Select(flag => type.GetField(flag.ToString())?.GetCustomAttribute<DisplayAttribute>()?.Name ?? flag.ToString()); return string.Join(", ", names); } return value.ToString(); } }
