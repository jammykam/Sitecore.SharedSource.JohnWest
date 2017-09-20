namespace Sitecore.Sharedsource.Data.Query
{
  using System;

  using SC = Sitecore;

  public class Query : SC.Data.Query.Query
  {
    protected void AddFunctions()
    {
      this.Function += Functions;
    }

    public Query() : base()
    {
      this.AddFunctions();
    }

    public Query(SC.Data.Query.Opcode query) : base(query)
    {
      this.AddFunctions();
    }

    public Query(string query) : base(query)
    {
      this.AddFunctions();
    }

    public static object Functions(SC.Data.Query.FunctionArgs args)
    {
      SC.Diagnostics.Assert.ArgumentNotNull(args, "args");
      SC.Diagnostics.Assert.IsNotNullOrEmpty(
        args.FunctionName, 
        "args.FunctionName");

      switch (args.FunctionName.ToLowerInvariant())
      {
        case "containsignorecase":
          return ContainsIgnoreCase(args);
          break;
        case "tolower":
          return ToLower(args);
          break;
        case "versioncount":
          return VersionCount(args);
          break;
      }

      return SC.Data.Query.Functions.FunctionCall(args); 
    }

    protected static object ToLower(SC.Data.Query.FunctionArgs args)
    {
      SC.Diagnostics.Assert.ArgumentNotNull(args, "args");

      if (args.Arguments.Length != 1)
      {
        throw new SC.Exceptions.QueryException(
          "Too many or to few arguments in ToLower()");
      }

      object result = args.Arguments[0].Evaluate(args.Query, args.ContextNode);
      return result == null ? String.Empty : result.ToString().ToLower();
    }

    protected static object VersionCount(SC.Data.Query.FunctionArgs args)
    {
      SC.Diagnostics.Assert.ArgumentNotNull(args, "args");

      if (args.Arguments.Length != 1)
      {
        throw new SC.Exceptions.QueryException(
          "Too many or to few arguments in VersionCount()");
      }

      SC.Data.Query.QueryContext queryContext = args.Arguments[0].Evaluate(
        args.Query,
        args.ContextNode) as SC.Data.Query.QueryContext;

      if (queryContext == null)
      {
        return null;
      }

      SC.Data.Items.Item item = queryContext.GetQueryContextItem();

      if (item == null)
      {
        return null;
      }

      return item.Versions.Count;
    }

    protected static bool ContainsIgnoreCase(SC.Data.Query.FunctionArgs args)
    {
      SC.Diagnostics.Assert.ArgumentNotNull(args, "args");

      if (args.Arguments.Length != 2)
      {
        throw new SC.Exceptions.QueryException(
          "Too many or to few arguments in ContainsIgnoreCase()");
      }

      object x = args.Arguments[0].Evaluate(args.Query, args.ContextNode);
      object y = args.Arguments[1].Evaluate(args.Query, args.ContextNode);

      if (!(x is string) || !(y is string))
      {
        throw new SC.Exceptions.QueryException(
          "String expression expected in ContainsIgnoreCase()");
      }

      return x.ToString().IndexOf(
        y.ToString(), 
        StringComparison.OrdinalIgnoreCase) > -1;
    }
  }
}