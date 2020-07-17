using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
   
    public HeightGen h;
    public MeshFilter m;
    public int[] tri=new int[1350];
    public int[] fintri = new int[1350];
    public Vector3[] vert;
    Color[] colors;
    Vector2[] uv;
    List<Vector3> norm;    
    Mesh mymesh;
    public MeshCollider coll;
    public float scale=1;
    public Vector3 Position;
    public GroupParent gp;
    public Gradient grade;

    private void Start() {
       
      //  InitialBuild();
    }

    private void Update() {
        //ChunkQr.AddToQ(this);
       //CalcVerts();
    }

    public void InitialBuild() {
        mymesh = new Mesh();
        mymesh.SetVertices(vert);
        mymesh.SetTriangles(fintri, 0);
        mymesh.RecalculateBounds();
        norm = new List<Vector3>();
        CalcVerts();
    }

    public void CalcVerts() {
        h.GPUHeightGetter(Position + transform.position, scale);       
        vert = new Vector3[tri.Length];
        colors = new Color[tri.Length];
        uv = new Vector2[tri.Length];
        for (int tt = 0; tt < tri.Length; tt++) {
            vert[tt] = h.h[tri[tt]];
            uv[tt] = (new Vector2(vert[tt].x * scale + transform.position.x, vert[tt].z * scale + transform.position.z))/200f;
        }
        mymesh.SetVertices(vert);
        mymesh.SetUVs(0, uv);
        transform.localScale = new Vector3(scale, 1, scale);
        m.sharedMesh = mymesh;
        m.sharedMesh.RecalculateBounds();
        m.sharedMesh.RecalculateNormals();
        m.sharedMesh.GetNormals(norm);
        for (int tt = 0; tt < vert.Length; tt++) {
            float e = vert[tt].y / 1500f +.035f;
           // float nn = (Vector3.up - norm[tt]).magnitude *.5f;
           // e = e*.95f + nn*.05f;
            colors[tt] = grade.Evaluate(e);
        }
        m.sharedMesh.SetColors(colors);
        coll.sharedMesh = m.sharedMesh;
        
    }

    /*
    Vector3 one, two, normal, three;
    IEnumerator CalculateNormals() {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        for (int tt = 0; tt < tri.Length; tt += 3) {
            one = h.h[tri[tt]] + transform.position ;
            two = h.h[tri[tt + 1]] + transform.position;
            three = h.h[tri[tt + 2]] + transform.position;
            normal =  Vector3.Cross(two - one, three - one).normalized;
            norm[tri[tt]] += normal;
            norm[tri[tt+1]] += normal;
            norm[tri[tt+2]] += normal;
            if (tt % 30 == 29) {
                yield return wait;
            }
        }
        for (int tt = 0; tt < norm.Length; tt++) {
            norm[tt] = norm[tt].normalized;
            if (tt % 80 == 79) {
                yield return wait;
            }
        }
    }
   */
     
}
