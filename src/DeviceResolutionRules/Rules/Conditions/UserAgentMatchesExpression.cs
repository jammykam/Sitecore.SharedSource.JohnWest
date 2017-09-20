namespace Sitecore.Sharedsource.Rules.Conditions
{
  using System;
  using System.Text.RegularExpressions;
  using System.Web;

  public class UserAgentMatchesExpression<T> : 
    Sitecore.Rules.Conditions.OperatorCondition<T>
    where T : Sitecore.Rules.RuleContext
  {
    public string Expression
    {
      get; 
      set; 
    }

    protected override bool Execute(T ruleContext)
    {
      if (HttpContext.Current == null 
        || String.IsNullOrEmpty(HttpContext.Current.Request.UserAgent))
      {
        return false;
      }

      return Regex.IsMatch(
        HttpContext.Current.Request.UserAgent, 
        this.Expression, 
        RegexOptions.IgnoreCase);
    }
  }
}