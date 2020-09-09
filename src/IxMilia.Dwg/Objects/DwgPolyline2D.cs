﻿using System.Collections.Generic;
using System.Linq;

namespace IxMilia.Dwg.Objects
{
    public partial class DwgPolyline2D
    {
        public DwgSeqEnd SeqEnd { get; private set; } = new DwgSeqEnd();
        public List<DwgVertex2D> Vertices { get; } = new List<DwgVertex2D>();

        private DwgHandleReference _firstVertexHandle;
        private DwgHandleReference _lastVertexHandle;
        private DwgHandleReference _seqEndHandle;

        internal override IEnumerable<DwgObject> ChildItems
        {
            get
            {
                foreach (var vertex in Vertices)
                {
                    yield return vertex;
                }

                yield return SeqEnd;
            }
        }

        public DwgPolyline2D(IEnumerable<DwgVertex2D> vertices)
            : this()
        {
            Vertices.AddRange(vertices);
        }

        public DwgPolyline2D(params DwgVertex2D[] vertices)
            : this((IEnumerable<DwgVertex2D>)vertices)
        {
        }

        internal override void ReadPostData(BitReader reader)
        {
            _firstVertexHandle = reader.Read_H();
            if (_firstVertexHandle.Code != DwgHandleReferenceCode.HardPointer)
            {
                throw new DwgReadException("Incorrect vertex handle code.");
            }

            _lastVertexHandle = reader.Read_H();
            if (_lastVertexHandle.Code != DwgHandleReferenceCode.HardPointer)
            {
                throw new DwgReadException("Incorrect vertex handle code.");
            }

            _seqEndHandle = reader.Read_H();
            if (_seqEndHandle.Code != DwgHandleReferenceCode.SoftPointer)
            {
                throw new DwgReadException("Incorrect seqend handle code.");
            }
        }

        internal override void OnAfterEntityRead(BitReader reader, DwgObjectCache objectCache)
        {
            Vertices.Clear();
            var currentVertexHandle = _firstVertexHandle;
            while (!currentVertexHandle.PointsToNull)
            {
                var vertex = objectCache.GetObject<DwgVertex2D>(reader, currentVertexHandle.HandleOrOffset);
                Vertices.Add(vertex);
                currentVertexHandle = vertex.Handle.GetNextHandle(vertex.NextEntityHandle);
            }

            SeqEnd = objectCache.GetObject<DwgSeqEnd>(reader, _seqEndHandle.HandleOrOffset);
        }

        internal override void OnBeforeEntityWrite()
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                var currentVertex = Vertices[i];
                currentVertex.Layer = Layer;
                var previousVertex = i == 0
                    ? null
                    : Vertices[i - 1];
                var nextVertex = i == Vertices.Count - 1
                    ? null
                    : Vertices[i + 1];
                currentVertex.PreviousEntityHandle = currentVertex.GetHandleToObject(previousVertex, DwgHandleReferenceCode.HardPointer);
                currentVertex.NextEntityHandle = currentVertex.GetHandleToObject(nextVertex, DwgHandleReferenceCode.HardPointer);
            }

            if (Vertices.Count == 0)
            {
                _firstVertexHandle = new DwgHandleReference(DwgHandleReferenceCode.HardPointer, 0);
                _lastVertexHandle = new DwgHandleReference(DwgHandleReferenceCode.HardPointer, 0);
            }
            else
            {
                _firstVertexHandle = Vertices.First().Handle;
                _lastVertexHandle = Vertices.Last().Handle;
            }

            SeqEnd.Layer = Layer;
            _seqEndHandle = SeqEnd.Handle;
        }

        internal override void WritePostData(BitWriter writer, DwgObjectMap objectMap, int pointerOffset)
        {
            writer.Write_H(new DwgHandleReference(DwgHandleReferenceCode.HardPointer, _firstVertexHandle.HandleOrOffset));
            writer.Write_H(new DwgHandleReference(DwgHandleReferenceCode.HardPointer, _lastVertexHandle.HandleOrOffset));
            writer.Write_H(new DwgHandleReference(DwgHandleReferenceCode.SoftPointer, _seqEndHandle.HandleOrOffset));
        }
    }
}