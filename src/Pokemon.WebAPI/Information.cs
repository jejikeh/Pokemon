using System.Reflection;

namespace Pokemon.WebAPI;

public record Information(string Name, string Description)
{
    public static bool TryParse(string input, out Information? information)
    {
        information = default;
        var splitArray = input.Split(',', 2);
        if (splitArray.Length != 2)
            return false;
        information = new(splitArray[0], splitArray[1]);
        return true;
    }

    public static async ValueTask<Information?> BindAsync(HttpContext context, ParameterInfo parameterInfo)
    {
        var input = context.GetRouteValue(parameterInfo.Name!) as string ?? string.Empty;
        TryParse(input, out var information);
        return information;
    }
}