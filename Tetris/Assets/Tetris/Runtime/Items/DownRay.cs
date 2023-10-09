using UnityEngine;

namespace Tetris.Core
{
    [RequireComponent(typeof(Cube))]
    public class DownRay : MonoBehaviour
    {
        LineRenderer _lineRenderer;
        [SerializeField]
        float _width = 0.1f;
        [SerializeField]
        Color _color = Color.white;

        void Awake()
        {
            var g = new GameObject("DownRay");
            g.transform.parent = transform;
            _lineRenderer = g.AddComponent<LineRenderer>();
            _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            _lineRenderer.enabled = false;
        }

        void Update()
        {
            if (!GetComponent<Cube>().IsMoving)
            {
                // Debug.Log($"[DownRay] {transform.parent.gameObject.name}: off");
                ClearRay();
                return;
            }
            // Debug.Log($"[DownRay] {transform.parent.gameObject.name}: on");
            Ray ray = new Ray(transform.position, Vector3.down);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            DrawRay(transform.position, hit.point);
        }

        void DrawRay(Vector3 start, Vector3 end)
        {
            Vector3[] pos = new Vector3[2]{
                start,
                end
            };
            _lineRenderer.enabled = true;
            _lineRenderer.SetPositions(pos);
            _lineRenderer.startColor = _color;
            _lineRenderer.endColor = _color;
            _lineRenderer.startWidth = _width;
            _lineRenderer.endWidth = _width;
        }

        void ClearRay()
        {
            _lineRenderer.enabled = false;
        }

        void OnDestroy()
        {
            var mat = _lineRenderer.material;
            if (mat != null)
            {
                Destroy(mat);
            }
        }
    }
}
