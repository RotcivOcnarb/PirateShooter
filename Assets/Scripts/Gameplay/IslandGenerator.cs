using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IslandGenerator : MonoBehaviour {
    [SerializeField] float mapScale = 2;
    [SerializeField] float mapThreshold = 0.5f;
    [SerializeField] float perlinScale = 0.1f;
    [SerializeField] Vector2 offsetSeed;
    [SerializeField] Camera mainCamera;

    List<PolygonCollider2D> instancedColliders;
    Vector2Int currentCoordinate;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    private void Start() {

        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sortingLayerName = "Default";
        meshRenderer.sortingOrder = 0;
        instancedColliders = new List<PolygonCollider2D>();

        UpdateIslands();
    }

    public void UpdateIslands() {

        Vector3 worldMin = GameManager.Instance.mainCamera.ViewportToWorldPoint(new Vector2(-0.5f, -0.5f));
        Vector3 worldMax = GameManager.Instance.mainCamera.ViewportToWorldPoint(new Vector2(1.5f, 1.5f));

        List<List<Vector2>> polygons = MarchingSquares.MarchSquares(worldMin, worldMax, 1 / mapScale, PerlinValue, mapThreshold, out Mesh mesh);
        polygons = polygons.Where(polygon => polygon.Count > 2).ToList();
        meshFilter.mesh = mesh;

        while (instancedColliders.Count < polygons.Count) {
            PolygonCollider2D collider = new GameObject("Collider").AddComponent<PolygonCollider2D>();
            instancedColliders.Add(collider);
        }
        for (int i = instancedColliders.Count - 1; i > polygons.Count; i--) {
            Destroy(instancedColliders[i].gameObject);
            instancedColliders.RemoveAt(i);
        }

        for(int i = 0; i < polygons.Count; i++) {
            instancedColliders[i].SetPath(0, polygons[i]);
        }


    }


    private void Update() {

        Vector2Int coord = MarchingSquares.WorldToCoordinate(GameManager.Instance.player.transform.position, mapScale);

        if (currentCoordinate != coord) {
            currentCoordinate = coord;
            UpdateIslands();
        }


    }
    public float PerlinValue(Vector3 worldPosition) {
        return Mathf.PerlinNoise(
                (worldPosition.x + offsetSeed.x) * perlinScale,
                (worldPosition.y + offsetSeed.y) * perlinScale
                );
    }

}
