using System.Collections.Generic;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Xaml
{
	internal class NamescopingVisitor : IXamlNodeVisitor
	{
		readonly Dictionary<INode, INameScope> scopes = new Dictionary<INode, INameScope>();

		public NamescopingVisitor(HydratationContext context)
		{
			Values = context.Values;
		}

		Dictionary<INode, object> Values { get; set; }

		public bool VisitChildrenFirst
		{
			get { return false; }
		}

		public bool StopOnDataTemplate
		{
			get { return false; }
		}

		public bool StopOnResourceDictionary
		{
			get { return false; }
		}

		public void Visit(ValueNode node, INode parentNode)
		{
			scopes[node] = scopes[parentNode];
		}

		public void Visit(MarkupNode node, INode parentNode)
		{
			scopes[node] = scopes[parentNode];
		}

		public void Visit(ElementNode node, INode parentNode)
		{
			var ns = parentNode == null || IsDataTemplate(node, parentNode) || IsStyle(node, parentNode)
				? new NameScope()
				: scopes[parentNode];
			node.Namescope = ns;
			scopes[node] = ns;
		}

		public void Visit(RootNode node, INode parentNode)
		{
			var ns = new NameScope();
			node.Namescope = ns;
			scopes[node] = ns;
		}

		public void Visit(ListNode node, INode parentNode)
		{
			scopes[node] = scopes[parentNode];
		}

		static bool IsDataTemplate(INode node, INode parentNode)
		{
			var parentElement = parentNode as IElementNode;
			INode createContent;
			if (parentElement != null && parentElement.Properties.TryGetValue(XmlName._CreateContent, out createContent) &&
			    createContent == node)
				return true;
			return false;
		}

		static bool IsStyle(INode node, INode parentNode)
		{
			var pnode = parentNode as ElementNode;
			return pnode != null && pnode.XmlType.Name == "Style";
		}
	}
}