using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Procedural.Algorithms
{
    public class LSystem
    {
        public string DefaultAxiom { get; set; }

        public Dictionary<char, List<Func<string>>> Rules { get; private set; }
        // TODO: probability for rules
        // TODO: context sensitive rules

        public void RegisterRule(char input, Func<string> result)
        {
            List<Func<string>> rules;
            if (!Rules.TryGetValue(input, out rules))
            {
                rules = new List<Func<string>>();
                Rules.Add(input, rules);
            }

            rules.Add(result);
        }

        public LSystem()
        {
            Rules = new Dictionary<char, List<Func<string>>>();
        }

        public string Get(string axiom, int seed)
        {
            Random r = new Random(seed);
            List<string> retVal = new List<string>();

            foreach (var character in axiom)
            {
                List<Func<string>> rules;
                if (Rules.TryGetValue(character, out rules))
                {
                    if (rules.Any())
                    {
                        if (rules.Count > 1)
                        {
                            var index = (int)Math.Round((double)rules.Count / r.NextDouble());
                            retVal.Add(rules[index]());
                        }
                        else retVal.Add(rules.First()());
                    }
                }
                else retVal.Add(character.ToString());
            }

            return string.Concat(retVal);
        }

        public string Get(string axiom, int level, int seed)
        {
            string retVal = axiom;

            for (int i = 1; i < level + 1; i++)
            {
                retVal = Get(retVal, seed);
            }

            return retVal;
        }

        public virtual void Get(int level, int seed)
        {
            Get(DefaultAxiom, level, seed);
        }
    }
}