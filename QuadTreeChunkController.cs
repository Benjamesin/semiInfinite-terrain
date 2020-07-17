using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTreeChunkController : MonoBehaviour
{
    int[] tri = new int[1350];
    int[] fintri = new int[1350];
    Vector3[] vert;
    // 9450x9450 grid
    public Transform[] players;
    public Chunk prefabChunk;
    public List<Chunk> objPool;
    public List<Chunk> objInUse;
    public struct Build
    {
        public Vector3 start;
        public float size;
    }
    public List<Build> buildQ;

    private void Awake() {
        SetTriangles();
        buildQ = new List<Build>();
        objPool = new List<Chunk>();
        objInUse = new List<Chunk>();
    }

    int timer = 0; 
    private void Update() {
        timer++;
        if (timer > 1) {
            timer = 0;
            if (buildQ.Count > 0) {
                if (objPool.Count > 0) {
                    objInUse.Add(objPool[0]);
                    objPool.RemoveAt(0);
                } else {
                    objInUse.Add(Instantiate(prefabChunk));
                }
                int cc = objInUse.Count - 1;
                objInUse[cc].gameObject.SetActive(true);
                objInUse[cc].vert = vert;
                objInUse[cc].tri = tri;
                objInUse[cc].fintri = fintri;
                objInUse[cc].Position = Vector3.zero;
                objInUse[cc].scale = buildQ[0].size / 15;
                objInUse[cc].transform.position = buildQ[0].start;// -Global.ZeroY(Vector3.one) * objInUse[cc].scale;
                objInUse[cc].InitialBuild();
                buildQ.RemoveAt(0);
            } else {
                RunMapCheck();
                ManageBuild();
            }
        }
        
    }
    void ManageBuild() {
        bool removing = true;
        while (removing) {
            removing = false;
            for (int tt = 0; tt < objInUse.Count; tt++) {
                if (!IsInBuild(objInUse[tt])) {
                    StartCoroutine(SetDisabled(objInUse[tt]));                    
                    objInUse.RemoveAt(tt);
                    removing = true;
                    break;
                }
            }           
        }
    }

    IEnumerator SetDisabled(Chunk chunk) {
        yield return new WaitForSeconds(2.5f);
        chunk.gameObject.SetActive(false);
        objPool.Add(chunk);
    }

    bool IsInBuild(Chunk obj) {
        for (int tt = 0; tt < buildQ.Count; tt++) {
            if (Global.IsSameVector(buildQ[tt].start, obj.transform.position) && Mathf.Abs(obj.scale - (buildQ[tt].size / 15f)) < 1) {
                buildQ.RemoveAt(tt);
                return (true);
            }
        }
        return (false);
    }    

    public void RunMapCheck() {
        GridCheck(Vector3.zero, 21600);
    }

    public void GridCheck(Vector3 start, float size) {
        for (int tt = 0; tt < players.Length; tt++) {
            Vector3 pos = Global.ZeroY(players[tt].position);
            Vector3 center = start + new Vector3(size / 2f, 0, size / 2f);
            float mag = (pos - center).magnitude;
            if (mag < size / 1.15f && size > 200) {
                GridCheck(start, size / 2f);
                GridCheck(start + new Vector3(size / 2f, 0, 0), size / 2f);
                GridCheck(start + new Vector3(size / 2f, 0, size / 2f), size / 2f);
                GridCheck(start + new Vector3(0, 0, size / 2f), size / 2f);
            } else {
                Build bb = new Build() { start = start, size = size };
                buildQ.Add( bb);
            }
        }
    }

    void SetTriangles() {
        int c = 0;
        for (int xx = 0; xx < 16 - 1; xx++) {
            for (int zz = 0; zz < 16 - 1; zz++) {
                tri[c] = xx * 16 + zz; c++;
                tri[c] = xx * 16 + zz + 1; c++;
                tri[c] = (xx + 1) * 16 + zz; c++;
                tri[c] = (xx + 1) * (16) + zz + 1; c++;
                tri[c] = (xx + 1) * (16) + zz; c++;
                tri[c] = xx * (16) + zz + 1; c++;
            }
        }
        vert = new Vector3[tri.Length];
        for (int tt = 0; tt < tri.Length; tt++) {
            vert[tt] = Vector3.zero;
        }
        fintri = new int[tri.Length];
        for (int tt = 0; tt < fintri.Length; tt++) {
            fintri[tt] = tt;
        }
    }
}
