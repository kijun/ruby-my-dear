using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// execute in edit mode
[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class LineRenderer : ShapeRenderer {

    const int MAX_FRAGMENTS = 10000;

    public LineProperty property;
    // TODO used to check dirty, should really belong to lineproperty
    LineProperty cachedProperty;

    bool PropertyHasChanged() {
        return !property.Equals(cachedProperty);
    }

    bool TransformHasChanged() {
        if (Mathf.Approximately(property.length, TransformLength) &&
            Mathf.Approximately(property.width, TransformWidth) &&
            Mathf.Approximately(property.angle, TransformAngle) &&
            property.center == TransformCenter) {
            return false;
        }
        return true;
    }

    bool NeedsRerender() {
        if (property.style == BorderStyle.Dash && property != cachedProperty) {
            return true;
        }
        return false;
    }


    float TransformLength {
        get {
            return transform.localScale.x;
        }
    }

    float TransformWidth {
        get {
            return transform.localScale.y;
        }
    }

    float TransformAngle {
        get {
            return transform.eulerAngles.z;
        }
    }

    Vector2 TransformCenter {
        get {
            return transform.position;
        }
    }

    void Update() {
        // TODO check if edit mode
        // TODO performance
        if (PropertyHasChanged()) {
            transform.localScale = new Vector3(property.length, property.width, 1);
            transform.eulerAngles = transform.eulerAngles.SwapZ(property.angle);
            transform.position = property.center;
            if(NeedsRerender()) {
                OnUpdate();
            }
            cachedProperty = property;
        } else if (TransformHasChanged()) {
            property.length = TransformLength;
            property.width = TransformWidth;
            property.angle = TransformAngle;
            property.center = TransformCenter;
            OnUpdate();
            cachedProperty = property;
        }
    }

    /* RENDERING */
    /* we'll try rendering piecemeal (might be able to construct complicated situations with dashed lines */
    void Render() {
        if (property.style == BorderStyle.None ||
            property.style == BorderStyle.Solid ||
            property.gapLength == 0) {
            RenderSolid();
        } else if (property.style == BorderStyle.Dash) {
            RenderDashed();
        }
    }

    void RenderSolid() {
        using (var vh = new VertexHelper()) {
            MeshUtil.AddRect(BoundsUtil.UnitBounds, vh);
            MeshUtil.UpdateMesh(GetComponent<MeshFilter>(), vh);
            MeshUtil.UpdateColor(GetComponent<MeshRenderer>(), property.color);
        }
    }

    void RenderDashed() {
        int numFragments = Mathf.CeilToInt(property.length/(property.dashLength + property.gapLength));
        if (numFragments > MAX_FRAGMENTS) {
            Debug.LogError("Line has too many fragments (" + numFragments + ")");
            return;
        }

        using (var vh = new VertexHelper()) {
            for (int i=0; i<numFragments; i++) {
                float deltaX = i * (property.dashLength+property.gapLength)/property.length;
                var min = new Vector2(-0.5f + deltaX, -0.5f);
                var max = new Vector2(-0.5f + deltaX + property.dashLength/property.length, 0.5f);
                MeshUtil.AddRect(new Bounds().WithMinMax(min, max), vh);
            }
            MeshUtil.UpdateMesh(GetComponent<MeshFilter>(), vh);
            MeshUtil.UpdateColor(GetComponent<MeshRenderer>(), property.color);
        }
    }

    /*
     * PROPERTIES
     */

    public void OnUpdate() {
        Debug.Log("onupdatecalled");
        Render();
        DefaultShapeStyle.SetDefaultLineStyle(property);
    }

    void SetEndPoints(Vector2 p1, Vector2 p2) {
        property.points = new Tuple<Vector2, Vector2>(p1, p2);
    }

}

