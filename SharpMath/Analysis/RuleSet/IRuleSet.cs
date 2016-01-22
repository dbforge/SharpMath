using System.Collections.Generic;

namespace SharpMath.Analysis.RuleSet
{
    public interface IRuleSet
    {
        IEnumerable<Rule> Rules { get; set; }
    }
}