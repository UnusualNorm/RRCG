using RRCGBuild;
using System.Collections.Generic;
using System;

namespace RRCGBuild
{
    // Base type for semantic scopes.
    public interface SemanticScope
    {
        // Define interfaces for implementing keywords
        // (SemanticScope.IBreak might be more descriptive than just IBreak)
        public interface IBreak : SemanticScope
        {
            public void Break();
        }

        public interface IContinue : SemanticScope
        {
            public void Continue();
        }
    }


    // Actual scope definitions
    public class SwitchScope : SemanticScope.IBreak
    {
        public ExecFlow BreakFlow;

        public void Break()
        {
            // Merge the current exec flow into the break flow,
            // then clear the current exec flow.
            BreakFlow.Merge(ExecFlow.current);
            ExecFlow.current.Clear();
        }
    }

    public class WhileScope : SemanticScope.IBreak, SemanticScope.IContinue
    {
        public ExecFlow BlockFlow; // Exec flow of the while loop body. Will cycle back to entry "If" node.
        public ExecFlow DoneFlow; // Exec flow for when the loop is finished/break is invoked.
        public Node EntryIfNode;

        public void Break()
        {
            // Merge the current execution flow into the done flow,
            // then clear the current execution flow.
            // Nodes spawned after this will start a new flow.
            DoneFlow.Merge(ExecFlow.current);
            ExecFlow.current.Clear();
        }

        public void Continue()
        {
            // Advance the current execution flow back to the entry If node.
            // Nodes spawned after this will start a new flow.
            ExecFlow.current.Advance(Context.current, EntryIfNode.Port(0, 0), null);
        }
    }

    public class DoWhileScope : SemanticScope.IBreak, SemanticScope.IContinue
    {
        // TODO: This can probably be replaced with WhileScope?
        //       The trouble is that with WhileScope we have the loopback If node
        //       before the body, but here we have the body before the If node,
        //       so we need to implement Continue differently.

        public ExecFlow ContinueFlow; // Will jump to the input exec of the loopback "If" node
        public ExecFlow DoneFlow; // Exec flow for when the loop is finished/break is invoked.
        public Node LoopbackIfNode;

        public void Break()
        {
            // Merge the current exec flow into
            // the done flow, then clear the current flow.
            // Nodes spawned after this will create a new flow.
            DoneFlow.Merge(ExecFlow.current);
            ExecFlow.current.Clear();
        }

        public void Continue()
        {
            // Merge the current exec flow into
            // the continue flow, then clear the current flow.
            // Nodes spawned after this will create a new flow.
            ContinueFlow.Merge(ExecFlow.current);
            ExecFlow.current.Clear();
        }
    }

    // Scope valid in the expression of an assignment to an identifier. (currently only implemented for declaration intiailizers)
    public class NamedAssignmentScope : SemanticScope
    {
        // The Left-Hand side Identifier of the assignment
        public string Identifier;
    }

    // The accessibility scope is used to determine what can be accessed where.
    // This currently only manages declared/pending gotos and labels, but in
    // the future we can store variable declarations to address the assignment problem.
    public class AccessibilityScope : SemanticScope
    {
        public enum Kind
        {
            General,
            MethodRoot // root accessibility scope of a method
        }

        // Gotos that jump to labels not yet defined
        // (exec flow contains ports waiting to be advanced
        //  once the label is declared)
        public Dictionary<string, ExecFlow> PendingGotos;

        // Labels that have been defined, but are awaiting
        // a suitable Port to resolve to
        public List<string> PendingLabels;

        // Fully resolved labels, ready to jump to
        public Dictionary<string, Port> DeclaredLabels;

        // Variables declared within this scope.
        // Maps identifier name -> getter/setter methods
        public Dictionary<string, (Func<dynamic> Getter, Action<dynamic>? Setter)> DeclaredVariables;

        // Scope kind. May have implications on whether "access operations"
        // running under this scope can search up into enclosing accessibility scopes.
        public Kind ScopeKind;
    }

    // Manages promotion of variables in conditional branches
    public class ConditionalContext : SemanticScope
    {
        public interface IPromotedVariable
        {
            public dynamic RRVariableValue { get; set; }
            public dynamic ValueBeforePromotion { get; }
            public IPromotedVariable NewWithSameRRVariable(dynamic valueBeforePromotion);
        }

        public class PromotedVariable<T> : IPromotedVariable where T : AnyPort, new()
        {
            private Variable<T> RRVariable;
            public PromotedVariable(string identifier, dynamic valueBeforePromotion, Variable<T>? rrVariable)
            {
                ValueBeforePromotion = valueBeforePromotion;

                SemanticStack.current.Push(new NamedAssignmentScope { Identifier = $"Conditional_{identifier}" });
                RRVariable = rrVariable ?? new();
                SemanticStack.current.Pop();
            }

            public dynamic RRVariableValue { get => RRVariable.Value; set => RRVariable.Value = value; }
            public dynamic ValueBeforePromotion { get; private set; }
            public IPromotedVariable NewWithSameRRVariable(dynamic valueBeforePromotion) => new PromotedVariable<T>("", valueBeforePromotion, RRVariable);
        }

        // Variable promotion state
        public Dictionary<string, IPromotedVariable> PromotedVariables;

        // Should the initial assignment reference the RR variable value pin?
        public bool InitialAssignmentsReferenceRRVariable;

        public void MergePromotionsFrom(ConditionalContext b)
        {
            foreach (var identifier in b.PromotedVariables.Keys)
            {
                if (PromotedVariables.ContainsKey(identifier)) continue;
                PromotedVariables[identifier] = b.PromotedVariables[identifier];
            }
        }
    }
}