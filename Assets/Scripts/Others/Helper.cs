// adrien: OK

public static class Helper
{
    public static uint SwapEndianness(this uint x)
    {
        return ((x & 0x000000FFU) << 24) |
               ((x & 0x0000FF00U) << 8) |
               ((x & 0x00FF0000U) >> 8) |
               ((x & 0xFF000000U) >> 24);
    }
}