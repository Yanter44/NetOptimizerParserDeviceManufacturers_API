namespace NetOptimizerParserApi.Services.Utility
{
    public static class SupportedChipsets
    {
        public static Dictionary<string, List<string>> ChipsetsAndProccessors = new()
        {
            { "Intel H610", new List<string>() { "i3-12100", "i5-12400", "i7-12700", "i3-13100", "i5-13400", 
                                                 "i7-13700", "i3-14100", "i5-14400", "i7-14700" }
            },

            { "Intel H510", new List<string>() { "i3-10100", "i5-10400", "i7-10700", "i3-11100", "i5-11400", "i7-11700" } 
            },

            { "AMD A520", new List<string>()   { "Ryzen 5 5500", "Ryzen 5 5600", "Ryzen 7 5700X", "Ryzen 7 5800X", "Ryzen 9 5900X", "Ryzen 9 5950X" } 
            },

        };
    }
}
