using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace CurrentThreatLevel.Models
{
    public class Color
    {
        [JsonProperty("readableName")]
        public string ReadableName { get; }

        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("hex")]
        public string Hex { get; }

        public Color(string name, string hex)
        {
            Name = name;
            Hex = hex;
            ReadableName = getReadableName(name);
        }

        /// <summary>
        /// Since all of the names are referred to in CSS by PascalCased names, but we're  a e s t h e t i c  and want lowercase spaced names.
        /// </summary>
        /// <param name="name"></param>
        private string getReadableName(string name)
        {
            string readableName = "";

            for (int i = 0; i < name.Length; i++)
            {
                if (i == 0)
                {
                    readableName += char.ToLower(name.First());
                }
                else
                {
                    char c = name[i];

                    if (char.IsUpper(c))
                    {
                        readableName += " " + char.ToLower(c);
                    }
                    else
                    {
                        readableName += c;
                    }
                }
            }

            return readableName;
        }
    }
}