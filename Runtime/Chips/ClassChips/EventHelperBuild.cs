﻿using RRCG;
using System;
using System.Linq;

namespace RRCGBuild
{
    public abstract class BaseEvent
    {
        protected string EventName;

        protected BaseEvent() { }

        protected void InitNewEvent(params (StringPort, Type)[] eventDefinition)
        {
            var sourceName = SanitizeUtils.SantizeCV2Name(SemanticStackUtils.GetNamedAssignmentName("Event"));

            EventName = $"RRCG_{sourceName}_{Context.current.GetUniqueId()}";

            ChipBuilder.EventDefinition(EventName, eventDefinition);
        }

        protected void InitExistingEvent(string eventName)
        {
            EventName = eventName;
        }

        public void SendLocal(params AnyPort[] values) => ChipBuilder.EventSender(EventName, EventTarget.LOCAL, values);
        public void SendOthers(params AnyPort[] values) => ChipBuilder.EventSender(EventName, EventTarget.OTHERS, values);
        public void SendAll(params AnyPort[] values) => ChipBuilder.EventSender(EventName, EventTarget.ALL, values);
        public void SendAuthority(params AnyPort[] values) => ChipBuilder.EventSender(EventName, EventTarget.AUTHORITY, values);
        public void SendRoomAuthority(params AnyPort[] values) => ChipBuilder.EventSender(EventName, EventTarget.ROOM_AUTHORITY, values);
        public void SendPlayer(PlayerPort player, params AnyPort[] values) => ChipBuilder.EventSender(EventName, EventTarget.PLAYER, new[] { player }.Concat(values).ToArray());
    }


    public class EventDefinition : BaseEvent
    {
        public EventDefinition() => InitNewEvent();
        internal EventDefinition(string existingEvent) => InitExistingEvent(existingEvent);

        public void SendLocal() => base.SendLocal();
        public void SendOthers() => base.SendOthers();
        public void SendAll() => base.SendAll();
        public void SendAuthority() => base.SendAuthority();
        public void SendRoomAuthority() => base.SendRoomAuthority();
        public void SendPlayer(PlayerPort player) => base.SendPlayer(player);

        public void Receiver() => ChipBuilder.EventReceiver(EventName);

    }

    public class EventDefinition<T0> : BaseEvent
        where T0 : AnyPort, new()
    {
        public EventDefinition(StringPort param0Name = null) => InitNewEvent((param0Name ?? "value0", typeof(T0)));

        // The bool param is a hack to make the signature different to the normal constructor, since string implicitly casts to StringPort.
        internal EventDefinition(bool _, string existingEvent) => InitExistingEvent(existingEvent);

        public void SendLocal(T0 value0) => base.SendLocal(value0);
        public void SendOthers(T0 value0) => base.SendOthers(value0);
        public void SendAll(T0 value0) => base.SendAll(value0);
        public void SendAuthority(T0 value0) => base.SendAuthority(value0);
        public void SendRoomAuthority(T0 value0) => base.SendRoomAuthority(value0);
        public void SendPlayer(PlayerPort player, T0 value0) => base.SendPlayer(player, value0);

        public T0 Receiver() => ChipBuilder.EventReceiver<T0>(EventName);
    }

    public class EventDefinition<T0, T1> : BaseEvent
           where T0 : AnyPort, new()
           where T1 : AnyPort, new()
    {
        public EventDefinition(StringPort param0Name = null, StringPort param1Name = null) => InitNewEvent((param0Name ?? "value0", typeof(T0)), (param1Name ?? "value1", typeof(T1)));
        internal EventDefinition(string existingEvent) => InitExistingEvent(existingEvent);

        public void SendLocal(T0 value0, T1 value1) => base.SendLocal(value0, value1);
        public void SendOthers(T0 value0, T1 value1) => base.SendOthers(value0, value1);
        public void SendAll(T0 value0, T1 value1) => base.SendAll(value0, value1);
        public void SendAuthority(T0 value0, T1 value1) => base.SendAuthority(value0, value1);
        public void SendRoomAuthority(T0 value0, T1 value1) => base.SendRoomAuthority(value0, value1);
        public void SendPlayer(PlayerPort player, T0 value0, T1 value1) => base.SendPlayer(player, value0, value1);

        public (T0, T1) Receiver() => ChipBuilder.EventReceiver<T0, T1>(EventName);
    }



    [Obsolete("Use new EventDefinition() or ExistingEvent() instead")]
    public class EventHelper
    {
        private StringPort eventName;
        public EventHelper(StringPort eventName)
        {
            this.eventName = eventName;
        }
        public EventHelper Definition()
        {
            ChipBuilder.EventDefinition(eventName);
            return this;
        }

        public void Sender() => SendLocal();

        public void SendLocal() => ChipBuilder.EventSender(eventName, EventTarget.LOCAL);
        public void SendOthers() => ChipBuilder.EventSender(eventName, EventTarget.OTHERS);
        public void SendAll() => ChipBuilder.EventSender(eventName, EventTarget.ALL);
        public void SendAuthority() => ChipBuilder.EventSender(eventName, EventTarget.AUTHORITY);
        public void SendRoomAuthority() => ChipBuilder.EventSender(eventName, EventTarget.ROOM_AUTHORITY);
        public void SendPlayer(PlayerPort player) => ChipBuilder.EventSender(eventName, EventTarget.PLAYER, player);

        public void Receiver() => ChipBuilder.EventReceiver(eventName);
    }

    [Obsolete("Use new EventDefinition() or ExistingEvent() instead")]
    public class EventHelper<T0> where T0 : AnyPort, new()
    {
        private StringPort eventName;
        private StringPort value0;
        public EventHelper(StringPort eventName, StringPort value0 = null)
        {
            this.eventName = eventName;
            this.value0 = value0 ?? "value0";
        }

        public EventHelper<T0> Definition()
        {
            ChipBuilder.EventDefinition(eventName, (value0, typeof(T0)));
            return this;
        }
        public void Sender(T0 value0) => SendLocal(value0);

        public void SendLocal(T0 value0) => ChipBuilder.EventSender(eventName, EventTarget.LOCAL, value0);
        public void SendOthers(T0 value0) => ChipBuilder.EventSender(eventName, EventTarget.OTHERS, value0);
        public void SendAll(T0 value0) => ChipBuilder.EventSender(eventName, EventTarget.ALL, value0);
        public void SendAuthority(T0 value0) => ChipBuilder.EventSender(eventName, EventTarget.AUTHORITY, value0);
        public void SendRoomAuthority(T0 value0) => ChipBuilder.EventSender(eventName, EventTarget.ROOM_AUTHORITY, value0);
        public void SendPlayer(PlayerPort player, T0 value0) => ChipBuilder.EventSender(eventName, EventTarget.PLAYER, player, value0);

        public T0 Receiver() => ChipBuilder.EventReceiver<T0>(eventName);
    }

    [Obsolete("Use new EventDefinition() or ExistingEvent() instead")]
    public class EventHelper<T0, T1>
        where T0 : AnyPort, new()
        where T1 : AnyPort, new()
    {
        private StringPort eventName;
        private StringPort value0;
        private StringPort value1;
        public EventHelper(
            StringPort eventName,
            StringPort value0 = null,
            StringPort value1 = null
        )
        {
            this.eventName = eventName;
            this.value0 = value0 ?? "value0";
            this.value1 = value1 ?? "value1";
        }
        public EventHelper<T0, T1> Definition()
        {
            ChipBuilder.EventDefinition(
                eventName,
                (value0, typeof(T0)),
                (value1, typeof(T1))
            );
            return this;
        }
        public void Sender(T0 value0, T1 value1) => SendLocal(value0, value1);

        public void SendLocal(T0 value0, T1 value1) => ChipBuilder.EventSender(eventName, EventTarget.LOCAL, value0, value1);
        public void SendOthers(T0 value0, T1 value1) => ChipBuilder.EventSender(eventName, EventTarget.OTHERS, value0, value1);
        public void SendAll(T0 value0, T1 value1) => ChipBuilder.EventSender(eventName, EventTarget.ALL, value0, value1);
        public void SendAuthority(T0 value0, T1 value1) => ChipBuilder.EventSender(eventName, EventTarget.AUTHORITY, value0, value1);
        public void SendRoomAuthority(T0 value0, T1 value1) => ChipBuilder.EventSender(eventName, EventTarget.ROOM_AUTHORITY, value0, value1);
        public void SendPlayer(PlayerPort player, T0 value0, T1 value1) => ChipBuilder.EventSender(eventName, EventTarget.PLAYER, player, value0, value1);

        public (T0, T1) Receiver() => ChipBuilder.EventReceiver<T0, T1>(eventName);
    }

    [Obsolete("Use new EventDefinition() or ExistingEvent() instead")]
    public class EventHelper<T0, T1, T2>
        where T0 : AnyPort, new()
        where T1 : AnyPort, new()
        where T2 : AnyPort, new()
    {
        private StringPort eventName;
        private StringPort value0;
        private StringPort value1;
        private StringPort value2;
        public EventHelper(
            StringPort eventName,
            StringPort value0 = null,
            StringPort value1 = null,
            StringPort value2 = null
        )
        {
            this.eventName = eventName;
            this.value0 = value0 ?? "value0";
            this.value1 = value1 ?? "value1";
            this.value2 = value2 ?? "value2";
        }
        public EventHelper<T0, T1, T2> Definition()
        {
            ChipBuilder.EventDefinition(
                eventName,
                (value0, typeof(T0)),
                (value1, typeof(T1)),
                (value2, typeof(T2))
            );
            return this;
        }
        public void Sender(T0 value0, T1 value1, T2 value2) => SendLocal(value0, value1, value2);

        public void SendLocal(T0 value0, T1 value1, T2 value2) => ChipBuilder.EventSender(eventName, EventTarget.LOCAL, value0, value1, value2);
        public void SendOthers(T0 value0, T1 value1, T2 value2) => ChipBuilder.EventSender(eventName, EventTarget.OTHERS, value0, value1, value2);
        public void SendAll(T0 value0, T1 value1, T2 value2) => ChipBuilder.EventSender(eventName, EventTarget.ALL, value0, value1, value2);
        public void SendAuthority(T0 value0, T1 value1, T2 value2) => ChipBuilder.EventSender(eventName, EventTarget.AUTHORITY, value0, value1, value2);
        public void SendRoomAuthority(T0 value0, T1 value1, T2 value2) => ChipBuilder.EventSender(eventName, EventTarget.ROOM_AUTHORITY, value0, value1, value2);
        public void SendPlayer(PlayerPort player, T0 value0, T1 value1, T2 value2) => ChipBuilder.EventSender(eventName, EventTarget.PLAYER, player, value0, value1, value2);

        public (T0, T1, T2) Receiver() => ChipBuilder.EventReceiver<T0, T1, T2>(eventName);
    }

    [Obsolete("Use new EventDefinition() or ExistingEvent() instead")]
    public class EventHelper<T0, T1, T2, T3>
        where T0 : AnyPort, new()
        where T1 : AnyPort, new()
        where T2 : AnyPort, new()
        where T3 : AnyPort, new()
    {
        private StringPort eventName;
        private StringPort value0;
        private StringPort value1;
        private StringPort value2;
        private StringPort value3;
        public EventHelper(
            StringPort eventName,
            StringPort value0 = null,
            StringPort value1 = null,
            StringPort value2 = null,
            StringPort value3 = null
        )
        {
            this.eventName = eventName;
            this.value0 = value0 ?? "value0";
            this.value1 = value1 ?? "value1";
            this.value2 = value2 ?? "value2";
            this.value3 = value3 ?? "value3";
        }
        public EventHelper<T0, T1, T2, T3> Definition()
        {
            ChipBuilder.EventDefinition(
                eventName,
                (value0, typeof(T0)),
                (value1, typeof(T1)),
                (value2, typeof(T2)),
                (value3, typeof(T3))
            );
            return this;
        }
        public void Sender(T0 value0, T1 value1, T2 value2, T3 value3) => SendLocal(value0, value1, value2, value3);

        public void SendLocal(T0 value0, T1 value1, T2 value2, T3 value3) => ChipBuilder.EventSender(eventName, EventTarget.LOCAL, value0, value1, value2, value3);
        public void SendOthers(T0 value0, T1 value1, T2 value2, T3 value3) => ChipBuilder.EventSender(eventName, EventTarget.OTHERS, value0, value1, value2, value3);
        public void SendAll(T0 value0, T1 value1, T2 value2, T3 value3) => ChipBuilder.EventSender(eventName, EventTarget.ALL, value0, value1, value2, value3);
        public void SendAuthority(T0 value0, T1 value1, T2 value2, T3 value3) => ChipBuilder.EventSender(eventName, EventTarget.AUTHORITY, value0, value1, value2, value3);
        public void SendRoomAuthority(T0 value0, T1 value1, T2 value2, T3 value3) => ChipBuilder.EventSender(eventName, EventTarget.ROOM_AUTHORITY, value0, value1, value2, value3);
        public void SendPlayer(PlayerPort player, T0 value0, T1 value1, T2 value2, T3 value3) => ChipBuilder.EventSender(eventName, EventTarget.PLAYER, player, value0, value1, value2, value3);

        public (T0, T1, T2, T3) Receiver() => ChipBuilder.EventReceiver<T0, T1, T2, T3>(eventName);
    }

}
