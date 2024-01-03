#nullable enable
using System.Collections.Generic;
using System;
using DeclaredVariable = RRCGBuild.AccessibilityScope.DeclaredVariable;
using PromotedVariable = RRCGBuild.ConditionalContext.PromotedVariable;
using RRCGGenerated;
using System.Linq;
using System.Runtime.CompilerServices;

// TODO: Should we start initializing members within the class declarations?
//       We currently do this at each construction site, which gets quite verbose.

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

        public abstract class BaseIterator : IBreak, IContinue
        {
            // Going with a base class here, because for most
            // iterators the logic will basically be this.

            public ExecFlow BreakFlow; // Break out of loop
            public ExecFlow ContinueFlow; // Jump back to the start of the loop
            public ConditionalContext ConditionalContext; // Required to write variable values when breaking/continuing

            // If this is true, the iterator will need to be built
            // with a manual implementation, and cannot be built with
            // the existing For/For Each nodes.
            public bool NeedsManualImplementation;

            public void Break()
            {
                ConditionalContext.WritePromotedVariables();

                BreakFlow.Merge(ExecFlow.current);
                ExecFlow.current.Clear();
                NeedsManualImplementation = true;
            }

            public void Continue()
            {
                ConditionalContext.WritePromotedVariables();

                // Note: For CV2-native iterators (For, For Each),
                // continue can just be implemented as clearing the current exec flow.
                // But we still store this flow so we can use it in the manual case.
                ContinueFlow.Merge(ExecFlow.current);
                ExecFlow.current.Clear();
            }

            /// <summary>
            /// Ensures the provided ExecFlow connects back to the sourceExec.
            /// Also flags all open iterators on the SemanticStack as requiring a manual implementation if an After Delay port is present.
            /// </summary>
            public void EnsureContinuityAndCheckDelays(ExecFlow execFlow, Port sourceExec)
            {
                var portsToCheck = new Stack<Port>(execFlow.Ports);
                var checkedPorts = new List<Port>();
                bool foundSource = false;

                while (portsToCheck.Count > 0)
                {
                    var currPort = portsToCheck.Pop();
                    var node = currPort.Node;
                    checkedPorts.Add(currPort);

                    if (currPort.EquivalentTo(sourceExec))
                    {
                        foundSource = true;
                        continue; // Don't search behind this node
                    }

                    if (node.Type == ChipType.Delay && currPort.EquivalentTo(node.Port(0, 1)))
                        SemanticStackUtils.AllIteratorsNeedManual();

                    var connectedExecPorts = Context.current.Connections
                        .Where(c => c.isExec && c.To.Node == currPort.Node)
                        .Select(c => c.From);

                    foreach (var port in connectedExecPorts)
                        if (!checkedPorts.Contains(port))
                            portsToCheck.Push(port);
                }

                if (!foundSource) throw new Exception("Iterator discontinuity detected -- provided ExecFlow did not connect back to sourceExec!");
            }
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

    public class WhileScope : SemanticScope.BaseIterator
    {
        // NOTE: For While loops, a manual implementation will always be built
        // because there is no CV2 equivalent.
    }

    public class ForEachScope : SemanticScope.BaseIterator { }

    // Scope valid in the expression of an assignment to an identifier. (currently only implemented for declaration intiailizers)
    public class NamedAssignmentScope : SemanticScope
    {
        // The Left-Hand side Identifier of the assignment
        public string Identifier;
    }

    // The accessibility scope is used to determine what
    // variables/labels/etc are currently accessible.
    public class AccessibilityScope : SemanticScope
    {
        public enum Kind
        {
            General,
            MethodRoot // root accessibility scope of a method
        }

        public class DeclaredVariable
        {
            public Func<dynamic> Getter;
            public Action<dynamic>? Setter;
            public Type Type;

            // Only set when creating promoted variables in a conditional context.
            // This allows us to make use of the "double-set" prevention when we have
            // two seperate conditional contexts that immediately precede/follow eachother.
            public dynamic? RRVariableForPromotion;
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
        public Dictionary<string, DeclaredVariable> DeclaredVariables;

        // Scope kind. May have implications on whether "access operations"
        // running under this scope can search up into enclosing accessibility scopes.
        public Kind ScopeKind;
    }

    // Manages promotion of variables in conditional branches
    public class ConditionalContext : SemanticScope
    {
        // TODO: For declared/promoted variables, we end up using dynamic a lot.
        //       It might not be necessary, but is there anything we can do to improve type safety?
        public class PromotedVariable
        {
            public DeclaredVariable DeclaredVariable;
            public dynamic ValueBeforePromotion;
            public dynamic RRVariableValue
            {
                // When creating PromotedVariables in __ConditionalContext,
                // we always ensure the declared variable has an RRVariableForPromotion.
                get => DeclaredVariable.RRVariableForPromotion!.Value;

                set {
                    // Avoid double-setting the variable value by checking if the value
                    // is the variable getter port.

                    if (((AnyPort)value).EquivalentTo(RRVariableValue)) return;
                    DeclaredVariable.RRVariableForPromotion!.Value = value;
                }
            }
        }

        // Variable promotion state
        public Dictionary<string, PromotedVariable> PromotedVariables;

        /// <summary>
        /// Writes the current C# state of each promoted variable to the RR variable on the current exec flow.
        /// </summary>
        public void WritePromotedVariables()
        {
            foreach (var promotedVariable in PromotedVariables.Values)
                promotedVariable.RRVariableValue = promotedVariable.DeclaredVariable.Getter();
        }

        /// <summary>
        /// Resets the C# state of each promoted variable to either
        /// the pre-promotion state, or the RR variable output (if setToVariableValue is set)
        /// </summary>
        public void ResetPromotedVariables(bool setToVariableValue)
        {
            foreach (var promotedVariable in PromotedVariables.Values)
            {
                if (promotedVariable.DeclaredVariable.Setter == null) continue;

                var value = setToVariableValue ? promotedVariable.RRVariableValue : promotedVariable.ValueBeforePromotion;
                promotedVariable.DeclaredVariable.Setter(value);
            }
        }
    }

    public class ReturnScope : SemanticScope
    {
        public Type? ReturnType;
        public string MethodName;
        public string[]? TupleElementNames;
        public List<Return> Returns = new();

        private bool ReturnTypeIsTupleType;

        public class Return
        {
            public ExecFlow ExecFlow;
            public dynamic? Data;
        }

        public ReturnScope(string methodName, Type? returnType, string[]? tupleElementNames)
        {
            MethodName = methodName;
            ReturnType = returnType;
            TupleElementNames = tupleElementNames;

            ReturnTypeIsTupleType = returnType != null && typeof(ITuple).IsAssignableFrom(returnType);
            int numTupleItems = returnType != null ? returnType.GetGenericArguments().Length : 0;

            if (ReturnTypeIsTupleType && (tupleElementNames == null || tupleElementNames.Length != numTupleItems))
                throw new Exception("Return type was a tuple type, but the number of " +
                                    "provided tuple element names didn't match!");
        }

        public void AddReturn(dynamic? data)
        {
            // ValueTuple doesn't support '=='.
            // We just want to check if it's null, so this'll do
            var dataIsNull = Object.Equals(data, null);

            // Ensure return type
            if (ReturnType == null && !dataIsNull)
                throw new Exception("Return scope had void return type, but return data was provided!");

            if (ReturnType != null)
            {
                if (dataIsNull)
                    throw new Exception("Return scope had non-void return type, but no return data was provided!");

                if (!ReturnType.IsAssignableFrom(data!.GetType()))
                    throw new Exception("The return data type was not compatible with the ReturnScope return type!");
            }

            // Copy the current exec flow, store it with the data
            var returnFlow = new ExecFlow();
            returnFlow.Merge(ExecFlow.current);
            Returns.Add(new Return { ExecFlow = returnFlow, Data = data });

            // Clear the current exec flow & we're done
            ExecFlow.current.Clear();
        }

        public dynamic? FinalizeReturns()
        {
            // Replicate old behaviour for now.
            // But now we can insert event senders to support multiple/conditional returns!
            foreach (var ret in Returns)
                ExecFlow.current.Merge(ret.ExecFlow);

            return Returns.LastOrDefault()?.Data;
        }
    }
}
#nullable disable