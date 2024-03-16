namespace CopperGameTools.Builder
{
    public struct ProjFileKey
    {
        public ProjFileKey(string key, string value, int line)
        {
            Key = key;
            Value = value;
            Line = line;
        }

        public string Key { get; } /* Name of Key */
        public string Value { get; set; } /* Value of Key*/
        public int Line { get; } /* Line of Key */
    }
}
