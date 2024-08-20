namespace Owls_BE.Helper
{
    public static class DateTime_VN
    {

        public static DateTime DateTimeNow()
        {
            DateTime utcNow = DateTime.UtcNow;
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, vietnamTimeZone);

            return vietnamTime;
        }
    }
}
