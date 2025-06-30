using UnityEngine;

public class ScreenEdgeColliders : MonoBehaviour
{
    public float thickness = 1f;
    public float bottomOffset = 100f;

    void Start()
    {
        Camera cam = Camera.main;
        float camHeight = cam.orthographicSize * 2f;
        float camWidth = camHeight * cam.aspect;

        float leftX = cam.transform.position.x - (camWidth / 2f);
        float rightX = cam.transform.position.x + (camWidth / 2f);
        float topY = cam.transform.position.y + (camHeight / 2f);
        float bottomY = cam.transform.position.y - (camHeight / 2f);

        CreateWall("LeftWall", new Vector2(leftX - (thickness / 2f), cam.transform.position.y), new Vector2(thickness, camHeight));
        CreateWall("RightWall", new Vector2(rightX + (thickness / 2f), cam.transform.position.y), new Vector2(thickness, camHeight));
        CreateWall("TopWall", new Vector2(cam.transform.position.x, topY + (thickness / 2f)), new Vector2(camWidth, thickness));
        CreateWall("BottomWall", new Vector2(cam.transform.position.x, bottomY - (thickness / 2f) + bottomOffset), new Vector2(camWidth, thickness));
    }

    void CreateWall(string name, Vector2 position, Vector2 size)
    {
        GameObject wall = new(name);
        wall.transform.parent = transform;
        wall.transform.position = position;

        BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
        collider.size = size;
        collider.isTrigger = false;
    }
}
