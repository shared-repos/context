using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Security.Permissions;

namespace System.Threading
{
    [DebuggerDisplay("IsValueCreated={IsValueCreated}, Value={ValueForDebugDisplay}, Count={ValuesCountForDebugDisplay}"), HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
    public class ThreadLocal<T> : IDisposable
    {
        private static IdManager idManager;
        [ThreadStatic]
        private static FinalizationHelper finalizationHelper;
        [ThreadStatic]
        private static LinkedSlotVolatile[] slotArrayStorage;

        private volatile bool initialized;
        private int idComplement;
        private bool trackAllValues;
        private LinkedSlot linkedSlot;
        private Func<T> valueFactory;

        static ThreadLocal()
        {
            idManager = new IdManager();
        }

        public ThreadLocal()
        {
            linkedSlot = new LinkedSlot(null);
            Initialize(null, false);
        }

        public ThreadLocal(Func<T> valueFactory)
        {
            linkedSlot = new LinkedSlot(null);
            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }

            Initialize(valueFactory, false);
        }

        public ThreadLocal(bool trackAllValues)
        {
            linkedSlot = new LinkedSlot(null);
            Initialize(null, trackAllValues);
        }

        public ThreadLocal(Func<T> valueFactory, bool trackAllValues)
        {
            linkedSlot = new LinkedSlot(null);
            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }
            Initialize(valueFactory, trackAllValues);
        }

        private void CreateLinkedSlot(LinkedSlotVolatile[] slotArray, int id, T value)
        {
            LinkedSlot slot = new LinkedSlot(slotArray);
            lock (idManager)
            {
                if (!initialized)
                {
                    throw new ObjectDisposedException("Disposed");
                }

                LinkedSlot next = linkedSlot.Next;
                slot.Next = next;
                slot.Previous = linkedSlot;
                slot.Value = value;
                if (next != null)
                {
                    next.Previous = slot;
                }

                linkedSlot.Next = slot;
                slotArray[id].Value = slot;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            int id;
            lock (idManager)
            {
                id = ~idComplement;
                idComplement = 0;
                if (id < 0 || !initialized)
                {
                    return;
                }

                initialized = false;
                for (LinkedSlot slot = linkedSlot.Next; slot != null; slot = slot.Next)
                {
                    LinkedSlotVolatile[] slotArray = slot.SlotArray;
                    if (slotArray != null)
                    {
                        slot.SlotArray = null;
                        slotArray[id].Value.Value = default(T);
                        slotArray[id].Value = null;
                    }
                }
            }

            linkedSlot = null;
            idManager.ReturnId(id);
        }

        ~ThreadLocal()
        {
            Dispose(false);
        }

        private static int GetNewTableSize(int minSize)
        {
            if (minSize > 0x7fefffff)
            {
                return 0x7fffffff;
            }

            int sz = minSize;
            sz--;
            sz |= sz >> 1;
            sz |= sz >> 2;
            sz |= sz >> 4;
            sz |= sz >> 8;
            sz |= sz >> 0x10;
            sz++;
            if (sz > 0x7fefffff)
            {
                sz = 0x7fefffff;
            }

            return sz;
        }

        private List<T> GetValuesAsList()
        {
            List<T> list = new List<T>();
            int id = ~idComplement;
            if (id == -1)
            {
                return null;
            }

            for (LinkedSlot slot = linkedSlot.Next; slot != null; slot = slot.Next)
            {
                list.Add(slot.Value);
            }

            return list;
        }

        private T GetValueSlow()
        {
            T local;
            int id = ~idComplement;
            if (id < 0)
            {
                throw new ObjectDisposedException("Disposed");
            }

            if (valueFactory == null)
            {
                local = default(T);
            }
            else
            {
                local = valueFactory();
                if (IsValueCreated)
                {
                    throw new InvalidOperationException("ThreadLocal_Value_RecursiveCallsToValue");
                }
            }

            Value = local;
            return local;
        }

        private void GrowTable(ref LinkedSlotVolatile[] table, int minLength)
        {
            LinkedSlotVolatile[] volatileArray = new LinkedSlotVolatile[GetNewTableSize(minLength)];
            lock (idManager)
            {
                for (int i = 0; i < table.Length; i++)
                {
                    LinkedSlot slot = table[i].Value;
                    if (slot != null && slot.SlotArray != null)
                    {
                        slot.SlotArray = volatileArray;
                        volatileArray[i] = table[i];
                    }
                }
            }

            table = volatileArray;
        }

        private void Initialize(Func<T> valueFactory, bool trackAllValues)
        {
            try
            {
                this.valueFactory = valueFactory;
                this.trackAllValues = trackAllValues;
            }
            finally
            {
                idComplement = ~idManager.GetId();
                initialized = true;
            }
        }

        private void SetValueSlow(T value, LinkedSlotVolatile[] slotArray)
        {
            int id = ~idComplement;
            if (id < 0)
            {
                throw new ObjectDisposedException("Disposed");
            }
            if (slotArray == null)
            {
                slotArray = new LinkedSlotVolatile[GetNewTableSize(id + 1)];
                finalizationHelper = new FinalizationHelper(slotArray, trackAllValues);
                slotArrayStorage = slotArray;
            }
            if (id >= slotArray.Length)
            {
                GrowTable(ref slotArray, id + 1);
                finalizationHelper.SlotArray = slotArray;
                slotArrayStorage = slotArray;
            }
            if (slotArray[id].Value == null)
            {
                CreateLinkedSlot(slotArray, id, value);
            }
            else
            {
                LinkedSlot slot = slotArray[id].Value;
                if (!initialized)
                {
                    throw new ObjectDisposedException("Disposed");
                }

                slot.Value = value;
            }
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public bool IsValueCreated
        {
            get
            {
                int id = ~idComplement;
                if (id < 0)
                {
                    throw new ObjectDisposedException("Disposed");
                }

                LinkedSlotVolatile[] volatileArray = slotArrayStorage;
                return (volatileArray != null && id < volatileArray.Length && volatileArray[id].Value != null);
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public T Value
        {
            get
            {
                LinkedSlot slot = null;
                LinkedSlotVolatile[] volatileArray = slotArrayStorage;
                int id = ~idComplement;
                if (volatileArray != null && id >= 0 && id < volatileArray.Length && (slot = volatileArray[id].Value) != null && initialized)
                {
                    return slot.Value;
                }

                return GetValueSlow();
            }
            set
            {
                LinkedSlot slot = null;
                LinkedSlotVolatile[] slotArray = slotArrayStorage;
                int id = ~idComplement;
                if (slotArray != null && id >= 0 && id < slotArray.Length && (slot = slotArray[id].Value) != null && initialized)
                {
                    slot.Value = value;
                }
                else
                {
                    SetValueSlow(value, slotArray);
                }
            }
        }

        internal T ValueForDebugDisplay
        {
            get
            {
                LinkedSlot slot = null;
                LinkedSlotVolatile[] volatileArray = slotArrayStorage;
                int id = ~idComplement;
                if (volatileArray != null && id < volatileArray.Length && (slot = volatileArray[id].Value) != null && initialized)
                {
                    return slot.Value;
                }

                return default(T);
            }
        }


        public IList<T> Values
        {
            get
            {
                if (!trackAllValues)
                {
                    throw new InvalidOperationException("ThreadLocal_ValuesNotAvailable");
                }

                List<T> valuesAsList = GetValuesAsList();
                if (valuesAsList == null)
                {
                    throw new ObjectDisposedException("Disposed");
                }

                return valuesAsList;
            }
        }

        private int ValuesCountForDebugDisplay
        {
            get
            {
                int count = 0;
                for (LinkedSlot slot = linkedSlot.Next; slot != null; slot = slot.Next)
                {
                    count++;
                }

                return count;
            }
        }

        internal List<T> ValuesForDebugDisplay
        {
            get
            {
                return GetValuesAsList();
            }
        }

        private class FinalizationHelper
        {
            private bool trackAllValues;
            internal LinkedSlotVolatile[] SlotArray;

            internal FinalizationHelper(LinkedSlotVolatile[] slotArray, bool trackAllValues)
            {
                SlotArray = slotArray;
                this.trackAllValues = trackAllValues;
            }

            ~FinalizationHelper()
            {
                LinkedSlotVolatile[] slotArray = SlotArray;
                for (int i = 0; i < slotArray.Length; i++)
                {
                    LinkedSlot slot = slotArray[i].Value;
                    if (slot != null)
                    {
                        if (trackAllValues)
                        {
                            slot.SlotArray = null;
                        }
                        else
                        {
                            lock (idManager)
                            {
                                if (slot.Next != null)
                                {
                                    slot.Next.Previous = slot.Previous;
                                }
                                slot.Previous.Next = slot.Next;
                            }
                        }
                    }
                }
            }
        }

        private class IdManager
        {
            private List<bool> freeIds;
            private int nextIdToTry;

            public IdManager()
            {
                freeIds = new List<bool>();
            }

            internal int GetId()
            {
                lock (freeIds)
                {
                    int nextIdToTry = this.nextIdToTry;
                    while (nextIdToTry < freeIds.Count)
                    {
                        if (freeIds[nextIdToTry])
                        {
                            break;
                        }
                        nextIdToTry++;
                    }
                    if (nextIdToTry == freeIds.Count)
                    {
                        freeIds.Add(false);
                    }
                    else
                    {
                        freeIds[nextIdToTry] = false;
                    }
                    this.nextIdToTry = nextIdToTry + 1;
                    return nextIdToTry;
                }
            }

            internal void ReturnId(int id)
            {
                lock (freeIds)
                {
                    freeIds[id] = true;
                    if (id < nextIdToTry)
                    {
                        nextIdToTry = id;
                    }
                }
            }
        }

        private sealed class LinkedSlot
        {
            internal volatile LinkedSlot Next;
            internal volatile LinkedSlot Previous;
            internal volatile LinkedSlotVolatile[] SlotArray;
            internal T Value;

            internal LinkedSlot(LinkedSlotVolatile[] slotArray)
            {
                SlotArray = slotArray;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LinkedSlotVolatile
        {
            internal volatile LinkedSlot Value;
        }

        public delegate TResult Func<TResult>();
    }
}
