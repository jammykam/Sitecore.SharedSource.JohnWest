namespace Sitecore.Sharedsource .Pipelines.RenderWord.ProcessWordNodeTree
{
  using System.Collections.Generic;

  using HtmlAgilityPack;

  using Sitecore.Pipelines;

  public class ProcessWordNodeTreeArgs : PipelineArgs
  {
    /// The element from which processors in this pipeline should begin processing.
    private HtmlNode _current = null;

    // following sibling element of the element from which to begin processing
    private HtmlNode _next = null;

    // stores the token table extracted from the document.
    public Dictionary<string, string> TokenTable { get; set; }

    public ProcessWordNodeTreeArgs(HtmlNode node)
    {
      this.CurrentElement = node;
    }

    /// <summary>
    /// The element from which processors in this pipeline should begin processing.
    /// </summary>
    public HtmlNode CurrentElement
    {
      get
      {
        return this._current;
      }

      set
      {
        this._current = value;
        this._next = this.GetNextElement(value);
      }
    }

    /// <summary>
    /// Following sibling element of CurrentElement.
    /// </summary>
    public HtmlNode NextElement
    {
      get
      {
        return this._next;
      }

      set
      {
        this._next = value;
        this._current = this.GetPreviousElement(value);
      }
    }

    /// <summary>
    /// Retrieve the following sibling element of an element, 
    /// or null if no such element exists.
    /// </summary>
    /// <param name="node">The element for which to determine the next sibling element.</param>
    /// <returns>The next sibling element or null if no such element exists.</returns>
    public HtmlNode GetNextElement(HtmlNode node)
    {
      if (node == null)
      {
        return null;
      }

      while ((node = node.NextSibling) != null)
      {
        if (node.NodeType == HtmlNodeType.Element)
        {
          return node;
        }
      }

      return null;
    }

    /// <summary>
    /// Retrieve the previous sibling element of an element, 
    /// or null if no such element exists.
    /// </summary>
    /// <param name="node">The element for which to determine the previous sibling element.</param>
    /// <returns>The previous sibling element or null if no such element exists.</returns>
    private HtmlNode GetPreviousElement(HtmlNode node)
    {
      if (node == null)
      {
        return null;
      }

      while ((node = node.PreviousSibling) != null)
      {
        if (node.NodeType == HtmlNodeType.Element)
        {
          return node;
        }
      }

      return null;
    }
  }
}