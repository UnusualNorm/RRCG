using RRCG;
using RRCGGenerated;
using RRCGSource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RRCGBuild
{
    public class ChipBuilder : RRCGGenerated.ChipBuilderGen
    {
        // 
        // Event Nodes
        // 
        #region EventNodes
        public static void EventReceiver(StringPort eventName)
        {
            EventReceiver(new EventReceiverData(eventName.AsData<string>()));
        }

        public static T0 EventReceiver<T0>(StringPort eventName) where T0 : AnyPort, new()
        {
            EventReceiver(eventName);
            var node = Context.lastSpawnedNode;

            return new T0() { Port = node.Port(0, 1) };
        }

        public static (T0, T1) EventReceiver<T0, T1>(StringPort eventName)
            where T0 : AnyPort, new()
            where T1 : AnyPort, new()
        {
            EventReceiver(eventName);
            var node = Context.lastSpawnedNode;

            return (
                new T0() { Port = node.Port(0, 1) },
                new T1() { Port = node.Port(0, 2) }
            );
        }
        public static (T0, T1, T2) EventReceiver<T0, T1, T2>(StringPort eventName)
            where T0 : AnyPort, new()
            where T1 : AnyPort, new()
            where T2 : AnyPort, new()
        {
            EventReceiver(eventName);
            var node = Context.lastSpawnedNode;

            return (
                new T0() { Port = node.Port(0, 1) },
                new T1() { Port = node.Port(0, 2) },
                new T2() { Port = node.Port(0, 3) }
            );
        }
        public static (T0, T1, T2, T3) EventReceiver<T0, T1, T2, T3>(StringPort eventName)
            where T0 : AnyPort, new()
            where T1 : AnyPort, new()
            where T2 : AnyPort, new()
            where T3 : AnyPort, new()
        {
            EventReceiver(eventName);
            var node = Context.lastSpawnedNode;

            return (
                new T0() { Port = node.Port(0, 1) },
                new T1() { Port = node.Port(0, 2) },
                new T2() { Port = node.Port(0, 3) },
                new T3() { Port = node.Port(0, 4) }
            );
        }

        internal static void EventSender(StringPort eventName, EventTarget eventTarget, params AnyPort[] inputs)
        {
            EventSender(new EventSenderData(eventName.AsData<string>(), eventTarget));
            var node = Context.lastSpawnedNode;

            for (var i = 0; i < inputs.Length; i++)
                node.ConnectInputPort(Context.current, inputs[i], node.Port(0, 1 + i));
        }

        internal static void EventDefinition(StringPort eventName, params (StringPort, Type)[] eventDefinition)
        {
            EventDefinition(new EventDefinitionData(
                eventName.AsData<string>(),
                eventDefinition.Select(t => (t.Item1.AsData<string>(), t.Item2)).ToArray()
            ));
        }

        #endregion

        // 
        // Dynamic Pin Nodes
        // 
        #region DynamicPinNodes

        public static StringPort Concat(params StringPort[] ports)
        {
            ChipBuilderGen.StringConcat("", ports[0]);
            return ConnectDynamicPins(1, ports);
        }

        public static IntPort Add(params IntPort[] ports)
        {
            ChipBuilderGen.Add(ports[0]);
            return ConnectDynamicPins(0, ports);
        }
        public static FloatPort Add(params FloatPort[] ports)
        {
            ChipBuilderGen.Add(ports[0]);
            return ConnectDynamicPins(0, ports);
        }
        public static Vector3Port Add(params Vector3Port[] ports)
        {
            ChipBuilderGen.Add(ports[0]);
            return ConnectDynamicPins(0, ports);
        }

        public static IntPort Subtract(params IntPort[] ports)
        {
            ChipBuilderGen.Subtract(ports[0]);
            return ConnectDynamicPins(0, ports);
        }
        public static FloatPort Subtract(params FloatPort[] ports)
        {
            ChipBuilderGen.Subtract(ports[0]);
            return ConnectDynamicPins(0, ports);
        }
        public static Vector3Port Subtract(params Vector3Port[] ports)
        {
            ChipBuilderGen.Subtract(ports[0]);
            return ConnectDynamicPins(0, ports);
        }

        public static IntPort Multiply(params IntPort[] ports)
        {
            ChipBuilderGen.Multiply(ports[0]);
            return ConnectDynamicPins(0, ports);
        }
        public static FloatPort Multiply(params FloatPort[] ports)
        {
            ChipBuilderGen.Multiply(ports[0]);
            return ConnectDynamicPins(0, ports);
        }

        public static IntPort Divide(params IntPort[] ports)
        {
            ChipBuilderGen.Divide(ports[0]);
            return ConnectDynamicPins(0, ports);
        }
        public static FloatPort Divide(params FloatPort[] ports)
        {
            ChipBuilderGen.Divide(ports[0]);
            return ConnectDynamicPins(0, ports);
        }

        public static IntPort Modulo(params IntPort[] ports)
        {
            ChipBuilderGen.Modulo(ports[0]);
            return ConnectDynamicPins(0, ports);
        }
        public static FloatPort Modulo(params FloatPort[] ports)
        {
            ChipBuilderGen.Modulo(ports[0]);
            return ConnectDynamicPins(0, ports);
        }

        public static IntPort Min(params IntPort[] ports)
        {
            ChipBuilderGen.Min(ports[0]);
            return ConnectDynamicPins(0, ports);
        }
        public static FloatPort Min(params FloatPort[] ports)
        {
            ChipBuilderGen.Min(ports[0]);
            return ConnectDynamicPins(0, ports);
        }

        public static IntPort Max(params IntPort[] ports)
        {
            ChipBuilderGen.Max(ports[0]);
            return ConnectDynamicPins(0, ports);
        }
        public static FloatPort Max(params FloatPort[] ports)
        {
            ChipBuilderGen.Max(ports[0]);
            return ConnectDynamicPins(0, ports);
        }

        public static ListPort<T> ListCreate<T>(params T[] ports) where T : AnyPort, new()
        {
            ChipBuilderGen.ListCreate<T>(ports[0]);
            var node = Context.lastSpawnedNode;

            ConnectDynamicPins(0, ports);
            return new ListPort<T> { Port = new Port { Node = node } };
        }

        public static BoolPort And(params BoolPort[] ports)
        {
            ChipBuilderGen.And(ports[0]);
            return ConnectDynamicPins(0, ports);
        }
        public static BoolPort Or(params BoolPort[] ports)
        {
            ChipBuilderGen.Or(ports[0]);
            return ConnectDynamicPins(0, ports);
        }

        // Assumes first pin has already been connected
        private static T ConnectDynamicPins<T>(int offset, params T[] ports) where T : AnyPort, new()
        {
            var node = Context.lastSpawnedNode;
            node.InputCount = ports.Length + offset;

            for (int i = 1; i < ports.Length; i++)
            {
                node.ConnectInputPort(Context.current, ports[i], new Port
                {
                    Node = node,
                    Index = i + offset
                });
            }

            return new T { Port = new Port { Node = node } };
        }
        #endregion

        // 
        // Control Flow
        // 
        #region ControlFlowNodes

        public static void If(BoolPort test, AlternativeExec ifBranch, AlternativeExec elseBranch)
        {
            var prevFlow = ExecFlow.current;
            ExecFlow.current = new ExecFlow();
            ChipBuilderGen.If(test, () => { });
            var node = Context.lastSpawnedNode;

            ExecFlow.current = new ExecFlow();
            ExecFlow.current.Ports.Add(new Port { Node = node });
            var conditionalContext = ConditionalContext.Push(new IfConditionalContext() { test = test, currentBranch = true });
            ifBranch();
            var ifFlow = ExecFlow.current;

            ExecFlow.current = new ExecFlow();
            ExecFlow.current.Ports.Add(new Port { Node = node, Index = 1 });
            conditionalContext.currentBranch = false;
            elseBranch();
            ConditionalContext.Pop();
            var elseFlow = ExecFlow.current;

            ExecFlow.current = prevFlow;

            if (ifFlow.hasAdvancedOrCleared || elseFlow.hasAdvancedOrCleared)
            {
                ExecFlow.current.Advance(Context.current, new Port { Node = node }, new Port { Node = node });

                ExecFlow.current.Clear();
                ExecFlow.current.Merge(ifFlow);
                ExecFlow.current.Merge(elseFlow);
            }
            else
            {
                Context.current.Nodes.Remove(node);
            }
        }

        public static void For(AlternativeExec<IntPort> forEach, IntPort from, IntPort to)
        {
            var index = ChipBuilderGen.For(from, to, (_) => { });
            var node = Context.lastSpawnedNode;

            forEach(index);

            ExecFlow.current = new ExecFlow();
            ExecFlow.current.Ports.Add(node.Port(0, 2));
        }

        public static void ExecutionIntegerSwitch(IntPort match, AlternativeExec failed, Dictionary<IntPort, AlternativeExec> branches)
        {
            ExecutionIntegerSwitch(match);
            var node = Context.lastSpawnedNode;
            node.SwitchCases = new List<string>();

            var execFlow = ExecFlow.current;
            execFlow.Clear();

            // Create exec flow for each delegate
            var delegateToExecFlow = new Dictionary<AlternativeExec, ExecFlow>();
            var defaultFlow = new ExecFlow();

            delegateToExecFlow[failed] = defaultFlow;
            foreach (var branch in branches)
                delegateToExecFlow[branch.Value] = new ExecFlow();

            // Now add the necessary ports to each flow
            int branchIndex = 0;
            delegateToExecFlow[failed].Ports.Add(new Port { Node = node });

            foreach (var branch in branches)
            {
                delegateToExecFlow[branch.Value].Ports.Add(new Port { Node = node, Index = 1 + branchIndex });
                branchIndex++;
            }

            // Finally we can start building the branches.
            // Build the failed branch..
            ExecFlow.current = defaultFlow;
            failed();
            execFlow.Merge(defaultFlow);

            // ..then the other branches.
            branchIndex = 0;
            var evaluatedBranches = new List<AlternativeExec>() { failed };
            foreach (var branch in branches)
            {
                if (branch.Key.Port != null) throw new Exception("Can't create Switch Cases with dynamic data. Make sure to pass an int value!");
                node.SwitchCases.Add(branch.Key.Data.ToString());

                // Only build each branch once.
                if (!evaluatedBranches.Contains(branch.Value))
                {
                    ExecFlow.current = delegateToExecFlow[branch.Value];
                    branch.Value();

                    var branchFlow = ExecFlow.current;
                    execFlow.Merge(branchFlow);

                    evaluatedBranches.Add(branch.Value);
                }

                branchIndex++;
            }

            ExecFlow.current = execFlow;
        }

        public static void ExecutionStringSwitch(StringPort match, AlternativeExec failed, Dictionary<StringPort, AlternativeExec> branches)
        {
            ExecutionStringSwitch(match);
            var node = Context.lastSpawnedNode;
            node.SwitchCases = new List<string>();

            var execFlow = ExecFlow.current;
            execFlow.Clear();

            // Create exec flow for each delegate
            var delegateToExecFlow = new Dictionary<AlternativeExec, ExecFlow>();
            var defaultFlow = new ExecFlow();

            delegateToExecFlow[failed] = defaultFlow;
            foreach (var branch in branches)
                if (!delegateToExecFlow.ContainsKey(branch.Value))
                    delegateToExecFlow[branch.Value] = new ExecFlow();

            // Now add the necessary ports to each flow
            int branchIndex = 0;
            defaultFlow.Ports.Add(new Port { Node = node });

            foreach (var branch in branches)
            {
                delegateToExecFlow[branch.Value].Ports.Add(new Port { Node = node, Index = 1 + branchIndex });
                branchIndex++;
            }

            // Finally we can start building the branches.
            // Build the failed branch..
            ExecFlow.current = defaultFlow;
            failed();
            execFlow.Merge(defaultFlow);

            // ..then the other branches.
            branchIndex = 0;
            var evaluatedBranches = new List<AlternativeExec>() { failed };
            foreach (var branch in branches)
            {
                if (branch.Key.Port != null) throw new Exception("Can't create Switch Cases with dynamic data. Make sure to pass a string value!");
                node.SwitchCases.Add(branch.Key.Data.ToString());

                // Only build each branch once.
                if (!evaluatedBranches.Contains(branch.Value))
                {
                    ExecFlow.current = delegateToExecFlow[branch.Value];
                    branch.Value();

                    var branchFlow = ExecFlow.current;
                    execFlow.Merge(branchFlow);

                    evaluatedBranches.Add(branch.Value);
                }

                branchIndex++;
            }

            ExecFlow.current = execFlow;
        }

        public static void ExecutionAnySwitch(AnyPort match, AlternativeExec failed, Dictionary<AnyPort, AlternativeExec> branches)
        {
            if (match is IntPort) ExecutionIntegerSwitch((IntPort)match, failed, branches.ToDictionary(item => new IntPort() { Data = item.Key.Data }, item => item.Value));
            else if (match is StringPort) ExecutionStringSwitch((StringPort)match, failed, branches.ToDictionary(item => new StringPort() { Data = item.Key.Data }, item => item.Value));
            else throw new Exception("Can't create Switch Cases with dynamic data. Make sure to pass a int or string value!");
        }

        public static T ValueIntegerSwitch<T>(IntPort match, T defaultValue, Dictionary<IntPort, T> cases) where T : AnyPort, new()
        {
            var switchValue = ValueIntegerSwitch(match, defaultValue);
            var node = Context.lastSpawnedNode;
            node.SwitchCases = new List<string>();
            node.InputCount += cases.Count();

            var portIndex = 2;
            foreach (var switchCase in cases)
            {
                if (switchCase.Key.IsActualPort) throw new Exception("Can't create Switch Cases with dynamic data. Make sure to pass a int value!");

                node.SwitchCases.Add(switchCase.Key.Data.ToString());
                node.ConnectInputPort(switchCase.Value, portIndex);

                portIndex++;
            }

            return new T() { Port = switchValue.Port };
        }

        public static T ValueStringSwitch<T>(StringPort match, T defaultValue, Dictionary<StringPort, T> cases) where T : AnyPort, new()
        {
            var switchValue = ValueStringSwitch(match, defaultValue);
            var node = Context.lastSpawnedNode;
            node.SwitchCases = new List<string>();
            node.InputCount += cases.Count();

            var portIndex = 2;
            foreach (var switchCase in cases)
            {
                if (switchCase.Key.IsActualPort) throw new Exception("Can't create Switch Cases with dynamic data. Make sure to pass a string value!");

                node.SwitchCases.Add(switchCase.Key.Data);
                node.ConnectInputPort(switchCase.Value, portIndex);

                portIndex++;
            }

            return new T() { Port = switchValue.Port };
        }


        #endregion

        //
        // Deprecated
        //
        #region Deprecated

        [Obsolete("Use the PascalCase variant instead")]
        public static ListPort<RecRoomObjectPort> RecRoomObjectGetAllwithTag(StringPort tag)
        {
            return RecRoomObjectGetAllWithTag(tag);
        }

        [Obsolete("Use the PascalCase variant instead")]
        public static RecRoomObjectPort RecRoomObjectGetFirstwithTag(StringPort tag)
        {
            return RecRoomObjectGetFirstWithTag(tag);
        }

        [Obsolete("Use the PascalCase variant instead")]
        public static BoolPort GreaterorEqual(IntPort a, IntPort b)
        {
            return GreaterOrEqual(a, b);
        }
        [Obsolete("Use the PascalCase variant instead")]
        public static BoolPort GreaterorEqual(FloatPort a, FloatPort b)
        {
            return GreaterOrEqual(a, b);
        }
        [Obsolete("Use the PascalCase variant instead")]
        public static BoolPort LessorEqual(IntPort a, IntPort b)
        {
            return LessOrEqual(a, b);
        }
        [Obsolete("Use the PascalCase variant instead")]
        public static BoolPort LessorEqual(FloatPort a, FloatPort b)
        {
            return LessOrEqual(a, b);
        }

        #endregion


        // 
        // Misc
        // 

        public static BoolPort Equals(params object[] ports)
        {
            ChipBuilderGen.Equals((AnyPort)ports[0]);
            var node = Context.lastSpawnedNode;
            node.InputCount = ports.Length;

            for (int i = 1; i < ports.Length; i++)
            {
                // casting magic needed to override the C# Equals method
                AnyPort port;
                if (ports[i] is AnyPort) port = (AnyPort)ports[i];
                else port = new AnyPort() { Data = ports[i] };

                node.ConnectInputPort(port, i);
            }

            return new BoolPort { Port = new Port { Node = node } };
        }

        public static T Self<T>() where T : AnyPort, new()
        {
            var self = Self();

            return new T() { Port = self.Port };
        }
    }
}


