﻿using System.Collections;
using System.Collections.Generic;

namespace IxMilia.Dwg.Objects
{
    public partial class DwgBlockControlObject : IDictionary<string, DwgBlockHeader>
    {
        private Dictionary<string, DwgBlockHeader> _blockHeaders = new Dictionary<string, DwgBlockHeader>();

        internal override IEnumerable<DwgObject> ChildItems => _blockHeaders.Values;

        internal override void PreWrite()
        {
            _blockHeaderCount = (short)_blockHeaders.Count;
            _blockHeaderHandles.Clear();
            foreach (var blockHeader in _blockHeaders.Values)
            {
                _blockHeaderHandles.Add(new DwgHandleReference(DwgHandleReferenceCode.None, blockHeader.Handle.HandleOrOffset));
                blockHeader.BlockControlHandle = new DwgHandleReference(DwgHandleReferenceCode.HardPointer, Handle.HandleOrOffset);
            }
        }

        internal override void PoseParse(BitReader reader, DwgObjectCache objectCache)
        {
            base.PoseParse(reader, objectCache);
            _blockHeaders.Clear();
            if (_blockHeaderHandles.Count != _blockHeaderCount)
            {
                throw new DwgReadException("Mismatch between reported block header count and block header handles read.");
            }

            if (_xDictionaryObjectHandle.Code != DwgHandleReferenceCode.SoftPointer)
            {
                throw new DwgReadException("Incorrect XDictionary object handle code.");
            }

            foreach (var blockHeaderHandle in _blockHeaderHandles)
            {
                if (blockHeaderHandle.Code != DwgHandleReferenceCode.None)
                {
                    throw new DwgReadException("Incorrect child block header handle code.");
                }

                var blockHeader = objectCache.GetObject<DwgBlockHeader>(reader, blockHeaderHandle.HandleOrOffset);
                if (!blockHeader.BlockControlHandle.IsEmpty && blockHeader.BlockControlHandle.HandleOrOffset != Handle.HandleOrOffset)
                {
                    throw new DwgReadException("Incorrect block header control object parent handle reference.");
                }

                _blockHeaders.Add(blockHeader.Name, blockHeader);
            }

            if (_modelSpaceBlockHeaderHandle.Code != DwgHandleReferenceCode.SoftPointer)
            {
                throw new DwgReadException("Incorrect model space block header handle code.");
            }

            if (_paperSpaceBlockHeaderHandle.Code != DwgHandleReferenceCode.SoftPointer)
            {
                throw new DwgReadException("Incorrect paper space block header handle code.");
            }
        }

        public void Add(DwgBlockHeader blockHeader) => Add(blockHeader.Name, blockHeader);

        #region IDictionary<string, DwgBlockHeader> implementation

        public ICollection<string> Keys => ((IDictionary<string, DwgBlockHeader>)_blockHeaders).Keys;

        public ICollection<DwgBlockHeader> Values => ((IDictionary<string, DwgBlockHeader>)_blockHeaders).Values;

        public int Count => ((IDictionary<string, DwgBlockHeader>)_blockHeaders).Count;

        public bool IsReadOnly => ((IDictionary<string, DwgBlockHeader>)_blockHeaders).IsReadOnly;

        public DwgBlockHeader this[string key] { get => ((IDictionary<string, DwgBlockHeader>)_blockHeaders)[key]; set => ((IDictionary<string, DwgBlockHeader>)_blockHeaders)[key] = value; }

        public void Add(string key, DwgBlockHeader value) => ((IDictionary<string, DwgBlockHeader>)_blockHeaders).Add(key, value);

        public bool ContainsKey(string key) => ((IDictionary<string, DwgBlockHeader>)_blockHeaders).ContainsKey(key);

        public bool Remove(string key) => ((IDictionary<string, DwgBlockHeader>)_blockHeaders).Remove(key);

        public bool TryGetValue(string key, out DwgBlockHeader value) => ((IDictionary<string, DwgBlockHeader>)_blockHeaders).TryGetValue(key, out value);

        public void Add(KeyValuePair<string, DwgBlockHeader> item) => ((IDictionary<string, DwgBlockHeader>)_blockHeaders).Add(item);

        public void Clear() => ((IDictionary<string, DwgBlockHeader>)_blockHeaders).Clear();

        public bool Contains(KeyValuePair<string, DwgBlockHeader> item) => ((IDictionary<string, DwgBlockHeader>)_blockHeaders).Contains(item);

        public void CopyTo(KeyValuePair<string, DwgBlockHeader>[] array, int arrayIndex) => ((IDictionary<string, DwgBlockHeader>)_blockHeaders).CopyTo(array, arrayIndex);

        public bool Remove(KeyValuePair<string, DwgBlockHeader> item) => ((IDictionary<string, DwgBlockHeader>)_blockHeaders).Remove(item);

        public IEnumerator<KeyValuePair<string, DwgBlockHeader>> GetEnumerator() => ((IDictionary<string, DwgBlockHeader>)_blockHeaders).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IDictionary<string, DwgBlockHeader>)_blockHeaders).GetEnumerator();

        #endregion

    }
}
