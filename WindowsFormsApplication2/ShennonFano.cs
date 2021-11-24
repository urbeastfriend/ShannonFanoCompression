using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication2
{
    public class ShennonFano
    {
        public static Tuple<string, List<Symbol>> Encode(string source)
        {
            var symbols = new List<Symbol>();
            
            foreach (var item in source)
            {
                var current = symbols.FirstOrDefault(x => x.Text == item.ToString());
                if (current == null) symbols.Add(new Symbol() { Text = item.ToString(), Count = 1, Probability = double.NaN, Code = "" });
                else current.Count += 1;
            }

            Func<IEnumerable<Symbol>, int> totalCount = x => x.Aggregate(0, (a, s) => a + s.Count);

            var total = totalCount(symbols);

            symbols.Select(x => x.Probability = total / x.Count).Count();
            symbols.Sort((a, b) => b.Count.CompareTo(a.Count));

            Action<IEnumerable<Symbol>, string, int> recurse = null;
            recurse = (localSymbols, str, depth) =>
            {
                if (localSymbols.Count() == 1)
                {
                    localSymbols.Single().Code = str;
                    return;
                }

                var bestDiff = int.MaxValue;
                int i;
                for (i = 1; i < localSymbols.Count(); i++)
                {
                    var firstPartCount = totalCount(localSymbols.Take(i));
                    var secondPartCount = totalCount(localSymbols.Skip(i));
                    var diff = Math.Abs(firstPartCount - secondPartCount);

                    if (diff < bestDiff) 
                    {
                        bestDiff = diff;
                    }
                    else break;
                }

                i = i - 1;

                recurse(localSymbols.Take(i), str + "0", depth + 1);
                recurse(localSymbols.Skip(i), str + "1", depth + 1);
            };

            recurse(symbols, "", 0);

            return new Tuple<string, List<Symbol>>(string.Join("", source.Select(x => symbols.First(z => z.Text == x.ToString()).Code)), symbols);
        }

        public static string Decode(string source, List<Symbol> symbols)
        {
            var adder = "";
            var result = new StringBuilder();

            foreach (var item in source)
            {
                adder += item.ToString();
                var symbol = symbols.FirstOrDefault(x => x.Code == adder);
                if (symbol == null)
                    continue;
                else
                {
                    result.Append(symbol.Text);
                    adder = "";
                }
            }

            return result.ToString();
        }
    }
}
