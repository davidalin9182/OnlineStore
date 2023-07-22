using System.Security.Claims;

using JsonSerializer = System.Text.Json.JsonSerializer;
namespace Proiect_IR
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var data = session.GetString(key);
            return data == null ? default(T) : JsonSerializer.Deserialize<T>(data);
        }

        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            var data = JsonSerializer.Serialize(value);
            session.SetString(key, data);
        }

    }
}
