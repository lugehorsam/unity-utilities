﻿/**using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Utilities.Meshes
{

    /// <summary>
    /// https://gamedevelopment.tutsplus.com/tutorials/how-to-dynamically-slice-a-convex-shape--gamedev-14479
    /// </summary>
    public struct SliceDatum
    {
        const float CONNECTION_MARGIN = .0001f;

        /// <summary>
        /// In local coordinates relative to the triangleToSlice it sliced.
        /// </summary>
        public ReadOnlyCollection<Vector3> SlicePositions
        {
            get { return new ReadOnlyCollection<Vector3>(slicePositions); }
        }

        private readonly Vector3[] slicePositions;

        public ReadOnlyCollection<Vector3> IntersectionVertices
        {
            get { return new ReadOnlyCollection<Vector3>(intersectionVertices); }
        }

        private readonly TriangleMesh[] trianglesToSlice;

        private readonly List<Vector3> intersectionVertices;

        public ISliceable Sliceable { get; private set; }

        private SliceDatum(IList<GestureFrame> gestureFrames, ISliceable sliceable)
        {
            intersectionVertices = new List<Vector3>();
            Sliceable = sliceable;

            trianglesToSlice = TriangleMesh.FromMesh(sliceable.Mesh);

            List<Vector3> slicePositionsList = new List<Vector3>();

            for (int i = 0; i < gestureFrames.Count; i++)
            {
                if (gestureFrames[i].HitForCollider(this.Sliceable.Collider) != null)
                {
                    Vector3 worldPoint = Camera.main.ScreenToWorldPoint(gestureFrames[i].Position);
                    Vector3 localPoint = Sliceable.Collider.transform.InverseTransformVector(worldPoint);
                    slicePositionsList.Add(localPoint);
                }
            }

            this.slicePositions = slicePositionsList.ToArray();

            foreach (TriangleMesh triangle in trianglesToSlice)
            {
                intersectionVertices = GetIntersectionsWithTriangle(triangle);
            }

        }

        public static SliceDatum[] FromGesture(Gesture gesture, ISliceable sliceable)
        {
            List<SliceDatum> sliceData = new List<SliceDatum>();

            Gesture[] collisionGestures = gesture.Filter(
                (frame) =>
                {
                    RaycastHit? hit = frame.HitForCollider(sliceable.Collider);
                    return hit.HasValue && hit.Value.collider == sliceable.Collider;
                },
                includeEdgeFrames: true
            );

            foreach (Gesture collisionGesture in collisionGestures)
            {
                SliceDatum datum = new SliceDatum(collisionGesture.GestureFrames, sliceable);
                sliceData.Add(datum);
            }
            return sliceData.ToArray();
        }

        public MeshDatum[] ApplySlice()
        {
            if (IntersectionVertices.Count < 2)
            {

                return null;
            }

            var newMeshes = new List<MeshDatum>();

            foreach (var triangleToSlice in trianglesToSlice)
            {
                newMeshes.AddRange(
                    SliceTriangle(triangleToSlice)
                );
            }

            return newMeshes.ToArray();
        }

        MeshDatum[] SliceTriangle(TriangleMesh triangleToSlice)
        {
            List<MeshDatum> newMeshes = new List<MeshDatum>();

            var singleMesh = new MeshDatum(new List<TriangleMesh>());
            var combinedMesh = new MeshDatum(new List<TriangleMesh>());

            newMeshes.Add(singleMesh);
            newMeshes.Add(combinedMesh);

            var tri1 = CreateSubTriangle(triangleToSlice, triangleToSlice[0]);
            var tri2 = CreateSubTriangle(triangleToSlice, triangleToSlice[1], new[] {tri1});
            var tri3 = CreateSubTriangle(triangleToSlice, triangleToSlice[2], new[] {tri1, tri2});

            AddSubTriangle(tri1, singleMesh, combinedMesh);
            AddSubTriangle(tri2, singleMesh, combinedMesh);
            AddSubTriangle(tri3, singleMesh, combinedMesh);

            return new []{singleMesh, combinedMesh};
        }

        void AddSubTriangle(TriangleMesh subTriangle, MeshDatum singleMesh, MeshDatum combinedMesh)
        {
            if (SliceWasAlongTriangle(subTriangle))
            {
                combinedMesh.AddTriangle(subTriangle);
            }
            else
            {
                singleMesh.AddTriangle(subTriangle);
            }
        }

        TriangleMesh CreateSubTriangle(TriangleMesh originalTriangle,
            Vertex initialVertex,
            TriangleMesh[] otherNewTriangles = null)
        {
            var subTriangle = new TriangleMesh();
            subTriangle[0] = initialVertex;

            var vertexQueue = new Queue<Vertex>();
            vertexQueue.Enqueue(intersectionVertices[0]);
            vertexQueue.Enqueue(intersectionVertices[1]);

            var origTriVertices = originalTriangle.Vertices.Except(new[] { initialVertex }).ToArray();
            vertexQueue.Enqueue(origTriVertices[0]);
            vertexQueue.Enqueue(origTriVertices[1]);

            var vertIndex = 1;
            while (vertIndex <= 2)
            {
                var candidateVertex = vertexQueue.Dequeue();
                if (otherNewTriangles != null && otherNewTriangles.Any(
                        (newTri) => newTri.HasEdgeThatIntersects(new MeshEdge(initialVertex, candidateVertex))))
                {
                    continue;
                }
                subTriangle[vertIndex] = candidateVertex;
                vertIndex++;
            }
            subTriangle.SortVertices();
            return subTriangle;
        }


        public bool SliceWasAlongTriangle(TriangleMesh triangle)
        {
            MeshEdge thisMeshEdge = ToEdge();

            foreach (MeshEdge edge in triangle.EdgeData)
            {
                bool edgesAreEqual = edge.Equals(thisMeshEdge);
                if (edgesAreEqual)
                {
                    return true;
                }
            }
            return false;
        }

        public List<Vector3> GetIntersectionsWithTriangle(TriangleMesh triangle)
        {
            var intersectionPoints = new List<Vector3>();
            foreach (MeshEdge edge in triangle.EdgeData)
            {
                Vector3? intersectionPoint = edge.GetIntersectionWithEdge(ToEdge(), onThisEdge: true, onOtherEdge: false); //avoid precision issues when comparing to slice
                if (intersectionPoint.HasValue)
                {
                    intersectionPoints.Add(intersectionPoint.Value);
                }
            }
            return intersectionPoints;
        }

        public MeshEdge ToEdge()
        {
            return new MeshEdge(SlicePositions);
        }
    }
}
**/