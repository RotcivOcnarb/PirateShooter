using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MarchingSquares {
    static Vector2[] baseCorners = new Vector2[] {
        Vector2.zero,
        Vector2.up,
        Vector2.one,
        Vector2.right,
    };

    static int[][] edgesType = new int[][] {
        new int[]{ },
        new int[]{3, 2},
        new int[]{2, 1},
        new int[]{3, 1},
        new int[]{1, 0},
        new int[]{3, 2, 1, 0},
        new int[]{2, 0},
        new int[]{3, 0},
        new int[]{0, 3},
        new int[]{0, 2},
        new int[]{2, 1, 0, 3},
        new int[]{0, 1},
        new int[]{1, 3},
        new int[]{1, 2},
        new int[]{2, 3},
        new int[]{},
    };

    static int[][] meshTriangles = new int[][] {
        new int[]{ },
        new int[]{ 2, 3, 3, 3, 3, 0},
        new int[]{ 1, 2, 2, 2, 2, 3},
        new int[]{ 1, 2, 2, 2, 3, 0, 3, 0, 2, 2, 3, 3},
        new int[]{ 0, 1, 1, 1, 1, 2},
        new int[]{ 0, 1, 1, 1, 1, 2, 2, 3, 3, 3, 3, 0},
        new int[]{ 0, 1, 1, 1, 2, 2, 0, 1, 2, 2, 2, 3},
        new int[]{ 0, 1, 1, 1, 2, 2, 0, 1, 2, 2, 3, 0, 3, 0, 2, 2, 3, 3},
        new int[]{ 0, 1, 3, 0, 0, 0},
        new int[]{ 0, 0, 0, 1, 2, 3, 2, 3, 3, 3, 0, 0},
        new int[]{ 0, 1, 3, 0, 0, 0, 1, 2, 2, 2, 2, 3},
        new int[]{ 0, 0, 0, 1, 3, 3, 0, 1, 1, 2, 3, 3, 1, 2, 2, 2, 3, 3},
        new int[]{ 0, 0, 1, 1, 1, 2, 1, 2, 3, 0, 0, 0},
        new int[]{ 0, 0, 1, 1, 1, 2, 0, 0, 1, 2, 2, 3, 2, 3, 3, 3, 0, 0},
        new int[]{ 3, 0, 0, 0, 1, 1, 3, 0, 1, 1, 2, 3, 2, 3, 1, 1, 2, 2},
        new int[]{ 0, 0, 1, 1, 2, 2, 0, 0, 2, 2, 3, 3}
    };

    class Vector2Node {
        public Vector2 value;
        public Vector2Node next;
    }

    public static List<List<Vector2>> MarchSquares(Vector3 worldMin, Vector3 worldMax, float cellSize, Func<Vector3, float> Sample, float threshold, out Mesh renderMesh) {
        Vector2Int coordinateMin = WorldToCoordinate(worldMin, 1 / cellSize);
        Vector2Int coordinateMax = WorldToCoordinate(worldMax, 1 / cellSize);

        Func<Vector3, float> newSample = (coordinate) => {
            if (coordinate.x <= coordinateMin.x) return 0;
            if (coordinate.x >= coordinateMax.x) return 0;
            if (coordinate.y <= coordinateMin.y) return 0;
            if (coordinate.y >= coordinateMax.y) return 0;
            return Sample(coordinate);
        };

        List<List<Vector2>> polygons = new List<List<Vector2>>();

        List<Vector3> meshVertexes = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        Dictionary<(Vector2Int, int), Vector2Node> nodes =
            new Dictionary<(Vector2Int, int), Vector2Node>();

        for (int x = coordinateMin.x; x <= coordinateMax.x; x++) {
            for (int y = coordinateMin.y; y <= coordinateMax.y; y++) {

                Vector2Int coordinate = new Vector2Int(x, y);
                Vector2[] corners = baseCorners.Select(v =>
                    (Vector2)CoordinateToWorld(coordinate, 1 / cellSize) + v / cellSize
                ).ToArray();

                int bitIndex = 0;
                for (int i = 0; i < 4; i++) {
                    int val = (newSample(corners[i]) > threshold ? 1 : 0);
                    bitIndex = (bitIndex << 1) | val;
                }
                int[] edgePairs = edgesType[bitIndex];

                List<Vector2> vertexes = new List<Vector2>();
                for (int p = 0; p < edgePairs.Length / 2; p++) {

                    //Vertex 1
                    int edge0 = edgePairs[p];
                    Vector2 p0 = corners[edge0];
                    Vector2 p1 = corners[(edge0 + 1) % 4];

                    float f0 = newSample(p0);
                    float f1 = newSample(p1);
                    float alpha = Mathf.InverseLerp(f0, f1, threshold);
                    if (alpha == 0 || alpha == 1) alpha = 0.5f;
                    Vector2 v1 = Vector2.Lerp(p0, p1, alpha);


                    //Vertex 2
                    int edge1 = edgePairs[p + 1];
                    p0 = corners[edge1];
                    p1 = corners[(edge1 + 1 ) % 4];

                    f0 = newSample(p0);
                    f1 = newSample(p1);
                    alpha = Mathf.InverseLerp(f0, f1, threshold);
                    if (alpha == 0 || alpha == 1) alpha = 0.5f;

                    Vector2 v2 = Vector2.Lerp(p0, p1, alpha);

                    (Vector2Int, int) v1Key = GenerateKey(coordinate, edge0);
                    (Vector2Int, int) v2Key = GenerateKey(coordinate, edge1);

                    if (!nodes.ContainsKey(v1Key)) {
                        nodes.Add(v1Key, new Vector2Node() { 
                            value = v1,
                        });
                    }
                    if (!nodes.ContainsKey(v2Key)) {
                        nodes.Add(v2Key, new Vector2Node() {
                            value = v2,
                        });
                    }
                    nodes[v1Key].next = nodes[v2Key];
                }

                int[] mesh = meshTriangles[bitIndex];
                for(int p = 0; p < mesh.Length / 6; p++) {

                    Vector2[] tri = new Vector2[3];
                    for(int t = 0; t < 3; t++) {
                        int va = mesh[p * 6 + t * 2 + 0];
                        int vb = mesh[p * 6 + t * 2 + 1];

                        Vector2 vert;

                        if (va != vb) {
                            Vector2 ca = corners[va];
                            Vector2 cb = corners[vb];

                            float f0 = newSample(ca);
                            float f1 = newSample(cb);
                            float alpha = Mathf.InverseLerp(f0, f1, threshold);
                            vert = Vector2.Lerp(ca, cb, alpha);
                        }
                        else {
                            vert = corners[va];
                        }

                        meshVertexes.Add(vert);
                        triangles.Add(triangles.Count);
                        uvs.Add(vert);
                        tri[t] = vert;
                    }

                    
                }


            }
        }

        Mesh meshObject = new Mesh();
        meshObject.vertices = meshVertexes.ToArray();
        meshObject.triangles = triangles.ToArray();
        meshObject.uv = uvs.ToArray();

        List<Vector2Node> allNodes = nodes.Values.ToList();
        while(allNodes.Count > 0) {

            Vector2Node first = allNodes[0];
            Vector2Node current = first;

            List<Vector2> polygon = new List<Vector2>();

            do {
                polygon.Add(current.value);
                allNodes.Remove(current);
                current = current.next;
            } while (current != null && current != first);

            polygons.Add(polygon);
        }

        renderMesh = meshObject;
        return polygons;
    }

    static (Vector2Int, int) GenerateKey(Vector2Int coordinate, int edge) {

        (Vector2Int, int) key = (coordinate, edge);
        if (edge == 1) {
            key = (coordinate + Vector2Int.up, 3);
        }
        if (edge == 2) {
            key = (coordinate + Vector2Int.right, 0);
        }
        return key;
    }


    public static Vector2Int WorldToCoordinate(Vector3 worldPos, float mapScale) {
        return new Vector2Int(
                (int)(worldPos.x * mapScale),
                (int)(worldPos.y * mapScale)
            );
    }

    public static Vector3 CoordinateToWorld(Vector2Int coordinate, float mapScale) {
        return new Vector3(
                coordinate.x / mapScale,
                coordinate.y / mapScale,
                0
            );
    }
}
