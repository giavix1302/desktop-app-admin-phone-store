using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace AdminPhoneStore.Helpers
{
    /// <summary>
    /// Helper để decode và xử lý JWT tokens
    /// </summary>
    public static class JwtHelper
    {
        /// <summary>
        /// Decode JWT và extract claims
        /// </summary>
        public static Dictionary<string, string> DecodeJwt(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token cannot be null or empty", nameof(token));

            var parts = token.Split('.');
            if (parts.Length != 3)
                throw new ArgumentException("Invalid JWT format", nameof(token));

            // Decode payload (part 1 is header, part 2 is payload)
            var payload = parts[1];
            
            // Add padding if needed (base64url)
            var padding = payload.Length % 4;
            if (padding != 0)
            {
                payload += new string('=', 4 - padding);
            }
            payload = payload.Replace('-', '+').Replace('_', '/');

            var payloadBytes = Convert.FromBase64String(payload);
            var payloadJson = Encoding.UTF8.GetString(payloadBytes);

            // Parse JSON
            var claims = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(payloadJson);
            
            if (claims == null)
                return new Dictionary<string, string>();

            // Convert JsonElement to string
            var result = new Dictionary<string, string>();
            foreach (var claim in claims)
            {
                if (claim.Value.ValueKind == JsonValueKind.String)
                {
                    result[claim.Key] = claim.Value.GetString() ?? string.Empty;
                }
                else if (claim.Value.ValueKind == JsonValueKind.Number)
                {
                    result[claim.Key] = claim.Value.GetInt64().ToString();
                }
                else
                {
                    result[claim.Key] = claim.Value.ToString();
                }
            }

            return result;
        }

        /// <summary>
        /// Lấy expiration time từ JWT (exp claim)
        /// </summary>
        public static DateTime? GetExpirationTime(string token)
        {
            try
            {
                var claims = DecodeJwt(token);
                
                if (claims.TryGetValue("exp", out var expValue))
                {
                    // exp is Unix timestamp (seconds since epoch)
                    if (long.TryParse(expValue, out var expTimestamp))
                    {
                        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                        return epoch.AddSeconds(expTimestamp);
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Kiểm tra token đã hết hạn chưa
        /// </summary>
        public static bool IsTokenExpired(string token)
        {
            var expirationTime = GetExpirationTime(token);
            if (expirationTime == null)
                return true; // Nếu không có exp claim, coi như đã hết hạn

            return expirationTime.Value <= DateTime.UtcNow;
        }

        /// <summary>
        /// Kiểm tra token sắp hết hạn (trong vòng X phút)
        /// </summary>
        public static bool IsTokenExpiringSoon(string token, int minutesBeforeExpiration = 5)
        {
            var expirationTime = GetExpirationTime(token);
            if (expirationTime == null)
                return true;

            var timeUntilExpiration = expirationTime.Value - DateTime.UtcNow;
            return timeUntilExpiration.TotalMinutes <= minutesBeforeExpiration;
        }
    }
}
