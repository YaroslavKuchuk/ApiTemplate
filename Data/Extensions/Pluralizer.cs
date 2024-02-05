using System.Globalization;
using Microsoft.EntityFrameworkCore.Design;

namespace Data.Extensions
{
   public class Pluralizer : IPluralizer
    {
        public string Pluralize(string name)
        {
            var inflector = new Inflector.Inflector(new CultureInfo("en"));
            return inflector.Pluralize(name) ?? name;
        }

        public string Singularize(string name)
        {
            var inflector = new Inflector.Inflector(new CultureInfo("en"));
            return inflector.Singularize(name) ?? name;
        }
    }
}
