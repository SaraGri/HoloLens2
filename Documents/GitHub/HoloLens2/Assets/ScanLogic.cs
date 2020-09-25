using Microsoft.MixedReality.SceneUnderstanding;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.SpatialAwareness.Utilities;
using System;
#if ENABLE_WINMD_SUPPORT
using Microsoft.MixedReality.SceneUnderstanding;
using Windows.Perception.Spatial;
using Windows.Perception.Spatial.Preview;
using UnityEngine.XR.WSA;
#endif
//https://github.com/mtaulty/SceneUnderstanding/blob/master/Assets/Scripts/MyScript.cs
public class ScanLogic : MonoBehaviour
{
    public GameObject rootObject;
    public IMixedRealitySpatialAwarenessMeshObserver observer;
    //eine neue Szene erzeugen, die auf dem Mixed Reality-Headset gelegt wird
    public Scene underStandingscene;
    private bool initialised, suscanning;
   
    
    // Start is called before the first frame update
    void Start()
    {
        //observer = gameObject.GetComponent<IMixedRealitySpatialAwarenessMeshObserver>();
        observer = CoreServices.GetSpatialAwarenessSystemDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();
        
        initialised = false;
        suscanning = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }




    public void startOrResumeObserver()
    {
        if (!observer.IsRunning) {
            observer.Resume();
            Debug.Log("mapping startet");
        }
    }
    public void stopOrSuspendObserver()
    {
        if (observer.IsRunning)
        {
            observer.Suspend();
            Debug.Log("mapping stoppend");
        }
    }

    public async void startSceneUnderstanding()
    {
        if (!suscanning) {
#if ENABLE_WINMD_SUPPORT
         await this.ComputeAsync(10.0f, true);
#endif
            suscanning = true;
        }
    }

    public async void stopSceneUnderstanding()
    {
        if (suscanning)
        {
#if ENABLE_WINMD_SUPPORT
         await this.ComputeAsync(10.0f, false);
#endif
            suscanning = false;
        }
    }

    public async void saveMesh() {
        stopOrSuspendObserver();


        //https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/SpatialAwareness/UsageGuide.html
      /*  foreach (SpatialAwarenessMeshObject meshObject in observer.Meshes.Values)
        {
            // mesh = meshObject.i;
            
        }*/
        await SpatialMeshExporter.Save(Application.persistentDataPath, true);

        //für SceneUnderstanding
        foreach (var sceneObject in underStandingscene.SceneObjects)
        {
                saveSceneUnderstandingMesh(sceneObject);
        }

    }
    //für SceneUnderstanding
    private void saveSceneUnderstandingMesh(SceneObject sceneObject)
    {
        throw new NotImplementedException();
    }

#if ENABLE_WINMD_SUPPORT
    async Task InitialiseAsync()
    {
            if (SceneObserver.IsSupported())
            {
                var access = await SceneObserver.RequestAccessAsync();

                if (access == SceneObserverAccessStatus.Allowed)
                {
                    this.initialised = true;
                } 
            }
    }

     async Task ComputeAsync(float radius, bool scan)
    {
       
        await this.InitialiseAsync();

        if (this.initialised)
        {
            var querySettings = new SceneQuerySettings()
            {
                EnableWorldMesh = scan,  // Requests a static version of the spatial mapping mesh.
                EnableSceneObjectQuads = scan, // Requests that the scene updates quads.
                EnableSceneObjectMeshes = scan, // Requests that the scene updates watertight mesh data.
                EnableOnlyObservedSceneObjects = false // Do not explicitly turn off quad inference.
            };
            this.underStandingscene = await SceneObserver.ComputeAsync(querySettings, radius);
        }
    }
    //https://github.com/microsoft/MixedReality-SceneUnderstanding-Samples/blob/master/Assets/SceneUnderstanding/Core/Understanding/Scripts/SceneUnderstandingManager.cs#L1154
#endif




    //https://docs.microsoft.com/de-de/windows/mixed-reality/scene-understanding-sdk#sceneobjects
    public void saveSceneUnderstandingMesh(IEnumerable<SceneObject> scene)
    {
        stopSceneUnderstanding();
        
        if (!suscanning) {
            // Die Objekte einer Szene interagieren
            foreach (var sceneObject in scene)
            { 
                string filePath = Path.Combine(Application.persistentDataPath, "SceneUnderstandingMesh" + ".txt");
                using (TextWriter writer = File.CreateText(filePath))
                {
                    writer.Write(Serialize(sceneObject));
                }

            }
        }
    }


    public void terminateApp()
    {
        if (true)
        {
            Application.Quit();
        }
    }

    //Die Meshes des Objekts serialisieren
    private static string Serialize(SceneObject meshes)
    {
        StringWriter stream = new StringWriter();
        int offset = 0;
        foreach (var mesh in meshes.Meshes)
        {
            Serializes(mesh, stream, ref offset);
        }
        return stream.ToString();
    }

    //Vertices und Indicies einzelner Meshes
    private static void Serializes(SceneMesh mesh, TextWriter stream, ref int offset)
     {
        System.Numerics.Vector3[] meshVertices = new System.Numerics.Vector3[mesh.VertexCount];
        mesh.GetVertexPositions(meshVertices);

        uint[] meshIndices = new uint[mesh.TriangleIndexCount];
        mesh.GetTriangleIndices(meshIndices);
        // Write vertices to .obj file. Need to make sure the points are transformed so everything is at a single origin.
        for (int i = 0; i < meshVertices.Length; i++)
        {
            // Here Z is negated because Unity Uses Left handed Coordinate system and Scene Understanding uses Right Handed
            stream.WriteLine(string.Format("v {0} {1} {2}", meshVertices[i].X, meshVertices[i].Y, -meshVertices[i].Z));
        }
        //normals werden nicht beachtet
        for (int i = 0; i < meshIndices.Length; i++)
        {
            stream.WriteLine(string.Format("f {0}//{0} {1}//{1} {2}//{2}",
                     meshIndices[i + 0] + 1 + offset,
                     meshIndices[i + 1] + 1 + offset,
                     meshIndices[i + 2] + 1 + offset));
           
        }
         offset += meshVertices.Length;
     }

    /*/// <summary>
    /// Create a unity Mesh from a set of Scene Understanding Meshes
    /// </summary>
    /// <param name="suMeshes">The Scene Understanding mesh to generate in Unity</param>
    private Mesh GenerateUnityMeshFromSceneObjectMeshes(IEnumerable<SceneMesh> suMeshes)
    {
        if (suMeshes == null)
        {
            Debug.LogWarning("SceneUnderstandingManager.GenerateUnityMeshFromSceneObjectMeshes: Meshes is null.");
            return null;
        }

        // Retrieve the data and store it as Indices and Vertices
        List<int> combinedMeshIndices = new List<int>();
        List<Vector3> combinedMeshVertices = new List<Vector3>();

        foreach (SceneMesh suMesh in suMeshes)
        {
            if (suMeshes == null)
            {
                Debug.LogWarning("SceneUnderstandingManager.GenerateUnityMeshFromSceneObjectMeshes: Mesh is null.");
                continue;
            }

            uint[] meshIndices = new uint[suMesh.TriangleIndexCount];
            suMesh.GetTriangleIndices(meshIndices);

            System.Numerics.Vector3[] meshVertices = new System.Numerics.Vector3[suMesh.VertexCount];
            suMesh.GetVertexPositions(meshVertices);

            uint indexOffset = (uint)combinedMeshIndices.Count;

            // Store the Indices and Vertices
            for (int i = 0; i < meshVertices.Length; i++)
            {
                // Here Z is negated because Unity Uses Left handed Coordinate system and Scene Understanding uses Right Handed
                combinedMeshVertices.Add(new Vector3(meshVertices[i].X, meshVertices[i].Y, -meshVertices[i].Z));
            }

            for (int i = 0; i < meshIndices.Length; i++)
            {
                combinedMeshIndices.Add((int)(meshIndices[i] + indexOffset));
            }
        }

        Mesh unityMesh = new Mesh();

        // Unity has a limit of 65,535 vertices in a mesh.
        // This limit exists because by default Unity uses 16-bit index buffers.
        // Starting with 2018.1, Unity allows one to use 32-bit index buffers.
        if (combinedMeshVertices.Count > 65535)
        {
            Debug.Log("SceneUnderstandingManager.GenerateUnityMeshForSceneObjectMeshes: CombinedMeshVertices count is " + combinedMeshVertices.Count + ". Will be using a 32-bit index buffer.");
            unityMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }

        // Apply the Indices and Vertices
        unityMesh.SetVertices(combinedMeshVertices);
        unityMesh.SetIndices(combinedMeshIndices.ToArray(), MeshTopology.Triangles, 0);
        unityMesh.RecalculateNormals();
    https://docs.microsoft.com/de-de/windows/mixed-reality/typography
        return unityMesh;
    }*/
}
