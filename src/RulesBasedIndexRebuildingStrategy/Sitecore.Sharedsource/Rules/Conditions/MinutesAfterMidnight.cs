namespace Sitecore.Sharedsource.Rules.Conditions
{
  using System;

  using Assert = Sitecore.Diagnostics.Assert;

  using SC = Sitecore;

  /// <summary>
  /// Rules engine condition to evaluate whether the system time
  /// is within the specified range of minutes since midnight
  /// </summary>
  /// <typeparam name="T">Rules context type 
  /// (<see cref="IndexingStrategyRuleContext"/> class).</typeparam>
  public class MinutesAfterMidnight<T> :
    SC.Rules.Conditions.OperatorCondition<T>
    where T : SC.Rules.RuleContext
  {
    /// <summary>
    /// Gets or sets the start minute for evaluation.
    /// </summary>
    public int? StartMinute { get; set; }

    /// <summary>
    /// Gets or sets the end minute for evaluation.
    /// </summary>
    public int? EndMinute { get; set; }

    /// <summary>
    /// Implementation allows index rebuilding if the system minute
    /// is between StartMinute and EndMinute minutes since midnight, inclusive.
    /// </summary>
    /// <param name="ruleContext">Rule processing context.</param>
    /// <returns>True if this condition allows index rebuilding.</returns>
    protected override bool Execute(T ruleContext)
    {
      Assert.ArgumentNotNull(ruleContext, "ruleContext");
      Assert.ArgumentNotNull(this.StartMinute, "StartMinute");
      Assert.ArgumentNotNull(this.EndMinute, "EndMinute");
      DateTime now = DateTime.Now;
      int minutes = (now.Hour * 60) + now.Minute;
      return minutes >= this.StartMinute.Value && minutes <= this.EndMinute.Value;
    }
  }
}